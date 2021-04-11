//#define DEBUG 1
#define ENV_PROD 1
#if ENV_PROD
#include "Config_prod.h"
#else
#include "Config_Dev.h"
#endif
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClientSecure.h>
#include <WiFiUdp.h>
#include "FS.h"

long lastLogRegistered = 0;
long lastPositionFileLogSent = 0;
long lastUnixTimeUtc = -1;
HTTPClient http;
WiFiClientSecure httpsClient;

struct GetSavedLogs
{
  String logs = "";
  long lastPosition = 0;
};

void setup()
{
#ifdef DEBUG
  Serial.begin(115200);
#else
  Serial.begin(74880);
#endif
#ifdef DEBUG
  Serial.print("Connecting to ");
  Serial.println(WIFI_SSID);
#endif
  WiFi.hostname(ESP8266_NAME);
  WiFi.mode(WIFI_STA);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
#ifdef DEBUG
    Serial.print(".");
#endif
  }
#ifdef DEBUG
  Serial.println("WiFi connected.");
#endif
  delay(3000);
#ifdef DEBUG
  Serial.println("Initialize File System");
  if (SPIFFS.begin())
  {
    Serial.println("SPIFFS Initialize....ok");
  }
  else
  {
    Serial.println("SPIFFS Initialization...failed");
  }
#else
  SPIFFS.begin();
#endif
  for (int attempt = 1; attempt <= 10; attempt++)
  {
    if (initUnixTimeUtc())
    {
      lastLogRegistered = (getUnixTimeUtc() - INTERVAL_TIME_SAVE_LOG);
      break;
    }
    if (attempt == 10)
      espReset();
  }
}

void loop()
{
  sendLogs();
#ifdef DEBUG
  Serial.print("getUnixTimeUtc: ");
  Serial.println(getUnixTimeUtc());
  delay(2000);
#endif
}

void sendLogs()
{
  if (!isTimeToRegisterLog(0))
  {
#ifdef DEBUG
    Serial.println("Not isTimeToRegisterLog");
#endif
    return;
  }
  struct GetSavedLogs savedLogs = getSavedLogs();
  String readLog = getReadLog();
  String batchContent = readLog + (savedLogs.logs.length() > 0 ? "|" + savedLogs.logs : "");
  batchContent.replace("\n", "");
  batchContent.replace("\r", "");
  lastLogRegistered = getUnixTimeUtc();
  if ((batchContent.length() > 0))
  {
#ifdef DEBUG
    Serial.println("batchContent: ");
    Serial.println(batchContent);
#endif
    httpsClient.setFingerprint(HTTPS_FRINGER_PRINT);
    httpsClient.setTimeout(15000); // 15 Seconds
    http.begin(httpsClient, END_POINT_URL_LOG_BATCH);
    http.addHeader("Content-Type", "application/json");
    http.addHeader("secretApiToken", SECRET_API_TOKEN);
    int httpResponseCode = http.POST("\"" + batchContent + "\"");
    if (httpResponseCode == 201)
    {
#ifdef DEBUG
      Serial.print("httpResponseCode: ");
      Serial.println(httpResponseCode);
#endif
      if (savedLogs.logs.length() > 0)
      {
        lastPositionFileLogSent = savedLogs.lastPosition;
      }
      deleteLogs();
    }
    else
    {
#ifdef DEBUG
      Serial.print("Error on sending POST: ");
      Serial.println(httpResponseCode);
#endif
      saveLog(readLog);
    }
    http.end();
  }
}

String getReadLog()
{
  Serial.print("READ");
  while (!Serial.available())
  {
  }
  return String(lastLogRegistered) + ";" + Serial.readString();
}

bool isTimeToRegisterLog(int anyLess)
{
  return ((getUnixTimeUtc() - lastLogRegistered) > (INTERVAL_TIME_SAVE_LOG - anyLess));
}

bool initUnixTimeUtc()
{
  httpsClient.setFingerprint(HTTPS_FRINGER_PRINT);
  httpsClient.setTimeout(15000); // 15 Seconds
  if (http.begin(httpsClient, END_POINT_URL_UNIX_TIME_UTC))
  {
    http.addHeader("Content-Type", "application/json");
    http.addHeader("secretApiToken", SECRET_API_TOKEN);
    int httpResponseCode = http.GET();
    if (httpResponseCode == 200)
    {
      String payload = http.getString();
      char *p;
      lastUnixTimeUtc = strtoll(payload.c_str(), &p, 10);
      lastUnixTimeUtc = lastUnixTimeUtc - (millis() / 1000);
#if DEBUG
      Serial.print("Http payload: ");
      Serial.println(payload);
      Serial.print("lastUnixTimeUtc: ");
      Serial.println(lastUnixTimeUtc);
#endif
    }
    else
    {
#if DEBUG
      Serial.println(END_POINT_URL_UNIX_TIME_UTC + " return httpResponseCode:" + String(httpResponseCode));
#endif
    }
    http.end();
  }
  else
  {
#if DEBUG
    Serial.println("not http.begin: " + END_POINT_URL_UNIX_TIME_UTC);
#endif
  }
  return lastUnixTimeUtc > -1;
}

long getUnixTimeUtc()
{
  return lastUnixTimeUtc + (millis() / 1000);
}

void saveLog(String readLog)
{
  FSInfo fs_info;
  SPIFFS.info(fs_info);
  File file = SPIFFS.open("/logs.txt", "a");
  if (!file)
  {
#if DEBUG
    Serial.println("Erro ao abrir arquivo!");
#endif
  }
  else
  {
    if (fs_info.totalBytes > (file.size() + readLog.length()))
    {
      file.print(readLog + "|");
      if (lastPositionFileLogSent == -1)
      {
        lastPositionFileLogSent = 0;
      }
    }
#if DEBUG
    Serial.print("file.size(): ");
    Serial.println(file.size());
    Serial.print("fs_info.totalBytes: ");
    Serial.println(fs_info.totalBytes);
    Serial.print("readLog.length(): ");
    Serial.println(readLog.length());
#endif
  }
  file.close();
}

struct GetSavedLogs getSavedLogs()
{
  struct GetSavedLogs savedLogs;
  if (lastPositionFileLogSent == -1)
  {
    return savedLogs;
  }
  File file = SPIFFS.open("/logs.txt", "r");
  if (!file)
  {
#if DEBUG
    Serial.println("/logs.txt not found!");
#endif
  }
  else
  {
    file.seek(lastPositionFileLogSent);
    for (int iBatch = 0; iBatch < 20; iBatch++)
    {
      String nextLogContent = file.readStringUntil('|');
      if (nextLogContent.length() > 0)
      {
        savedLogs.logs += (iBatch > 0 ? "|" : "") + nextLogContent;
        savedLogs.lastPosition = file.position();
      }
      else
      {
        if (iBatch < 20)
        {
          savedLogs.lastPosition = -1;
        }
        break;
      }
    }
  }
  file.close();
  return savedLogs;
}

void deleteLogs()
{
  if (lastPositionFileLogSent == -1)
  {
    SPIFFS.remove("/logs.txt");
    lastPositionFileLogSent = 0;
#if DEBUG
    Serial.println("SPIFFS.remove(/logs.txt)");
#endif
  }
}

void espReset()
{
#if DEBUG
  Serial.println("Restarting in 5 seconds");
  delay(5000);
#endif
  ESP.restart();
}
