//define DEBUG 1
//#define ENV_PROD 1

# if ENV_PROD
#include "Config_prod.h"
#else
#include "Config_Dev.h"
#endif

#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClientSecure.h> 
#include "NTPClient.h"
#include <WiFiUdp.h>
#include "FS.h"

const char* ssid     = WIFI_SSID;
const char* password = WIFI_PASSWORD;
const int INTERVAL_TIME_SAVE_LOG = 10;
//SHA1 finger print of certificate use web browser to view and copy
//Azure const char fingerprint[] PROGMEM = "8B 00 83 0B CC F2 46 F7 96 D3 8E B5 E1 2F CF BD 1D 3A 61 50";
//Azure const String host = "sbdia.azurewebsites.net";
const char fingerprint[] PROGMEM = "83 9A FD D6 EE 72 6C BF 60 9F C2 E6 50 E1 C3 42 D2 7B 6A E6";


long lastLogRegistered = 0;
long lastPositionFileLogSent = 0;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP);
HTTPClient http;
WiFiClientSecure httpsClient;
FSInfo fs_info;

struct GetSavedLogs {
  String logs = "";
  long lastPosition = 0;
};

void setup() {

#ifdef DEBUG
  Serial.begin(115200);
#else
  Serial.begin(74880);
#endif

  // Connect to Wi-Fi network with SSID and password
#ifdef DEBUG
  Serial.print("Connecting to ");
  Serial.println(ssid);
#endif
  WiFi.hostname(ESP8266_NAME);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
#ifdef DEBUG
    Serial.print(".");
#endif
  }

#ifdef DEBUG
  Serial.println("WiFi connected.");
#endif

#ifdef DEBUG
  Serial.println("Initialize File System");
  delay(3000);
#endif

  //Initialize File System
  if (SPIFFS.begin())
  {
#ifdef DEBUG
    Serial.println("SPIFFS Initialize....ok");
#endif
  }
  else
  {
#ifdef DEBUG
    Serial.println("SPIFFS Initialization...failed");
#endif
  }
  //Format File System
  /*
    if (SPIFFS.format())
    {
    Serial.println("File System Formated");
    }
    else
    {
    Serial.println("File System Formatting Error");
    }
   */
#ifdef DEBUG
  SPIFFS.info(fs_info);
  Serial.print("totalBytes: ");
  Serial.println(fs_info.totalBytes);
  Serial.print("usedBytes: ");
  Serial.println(fs_info.usedBytes);
  Serial.print("blockSize: ");
  Serial.println(fs_info.blockSize);
  Serial.print("pageSize: ");
  Serial.println(fs_info.pageSize);
  Serial.print("maxOpenFiles: ");
  Serial.println(fs_info.maxOpenFiles);
#endif
  initTimeClient();
}

void loop() {
  sendLogs();
#ifdef DEBUG
  Serial.print("timeClient.getEpochTime(): ");
  Serial.println(timeClient.getEpochTime());
  delay(2000);
#endif
}


void sendLogs() {

  if ( !isTimeToRegisterLog(0)) {
#ifdef DEBUG
    Serial.println("Not isTimeToRegisterLog");
#endif
    return;
  }

  struct GetSavedLogs savedLogs = getSavedLogs();
  String readLog = getReadLog();
  String batchContent = readLog + (savedLogs.logs.length() > 0 ?  "|" + savedLogs.logs : ""); //"1576807563000;1201;1202;1203";
  batchContent.replace("\n", "");
  batchContent.replace("\r", "");
  lastLogRegistered = timeClient.getEpochTime() ;

  if ( ( batchContent.length() > 0 )) {
#ifdef DEBUG
    Serial.println("batchContent: ");
    Serial.println(batchContent);
#endif
  httpsClient.setFingerprint(fingerprint);
  httpsClient.setTimeout(15000); // 15 Seconds
  delay(1000);
/*  
  int r=0; //retry counter
  while((!httpsClient.connect(host, 443)) && (r < 30)){
    delay(100);
#ifdef DEBUG
    Serial.print(".");
#endif
      r++;
  }
#ifdef DEBUG
  if(r==30) {
    Serial.println("Connection failed");
  }
  else {
    Serial.println("Connected to web");
  }
#endif
*/
    http.begin(httpsClient, String(END_POINT_URL));
    http.addHeader("Content-Type", "application/json");
    int httpResponseCode = http.POST(
                             "{\"SecretApiToken\":\"" + String(SECRET_API_TOKEN) +
                             "\",\"Content\":\"" + batchContent +
                             "\",\"SensorId\":\"" + String(SENSOR_ID) + "\" }"
                           );
    if (httpResponseCode == 201) {
#ifdef DEBUG
      Serial.print("httpResponseCode: ");
      Serial.println(httpResponseCode);   //Print return code
#endif
      if ( savedLogs.logs.length() > 0 ) {
        lastPositionFileLogSent = savedLogs.lastPosition;
      }
      deleteLogs();
    } else {
#ifdef DEBUG
      Serial.print("Error on sending POST: ");
      Serial.println(httpResponseCode);
#endif
      saveLog(readLog);
    }
    http.end();
  }

}

String getReadLog() {
  Serial.print("READ");
  while (!Serial.available()) {
  }
  return String(lastLogRegistered) + ";" + Serial.readString();
}

bool isTimeToRegisterLog(int anyLess) {
  return  (( timeClient.getEpochTime() - lastLogRegistered ) > (INTERVAL_TIME_SAVE_LOG - anyLess) );
}

void initTimeClient() {
  timeClient.begin();
  timeClient.forceUpdate();
  for (int attempt = 0; attempt < 10; attempt++) {
    if ( timeClient.getEpochTime() > 1577711219 ) {
#if DEBUG
      Serial.print("Attempt: "); Serial.println(attempt);
      Serial.print("EpochTime: "); Serial.println(timeClient.getEpochTime());
      delay(1000);
#endif
      break;
    } else {
      timeClient.forceUpdate();
    }
  }
  if ( timeClient.getEpochTime() < 1577711219 ) {
    espReset();
  }
  lastLogRegistered = ( timeClient.getEpochTime() - INTERVAL_TIME_SAVE_LOG) ;
}

void saveLog(String readLog) {

  File file = SPIFFS.open("/logs.txt", "a");
  if (!file) {
#if DEBUG
    Serial.println("Erro ao abrir arquivo!");
#endif
  } else {
    if ( fs_info.totalBytes > (file.size() + readLog.length()) ) {
      file.print(readLog + "|");
      if ( lastPositionFileLogSent == -1 ) {
        lastPositionFileLogSent = 0;
      }
    }
#if DEBUG
    Serial.print("file.size(): "); Serial.println(file.size());
    Serial.print("fs_info.totalBytes: "); Serial.println(fs_info.totalBytes);
    Serial.print("readLog.length(): "); Serial.println(readLog.length());
#endif
  }
  file.close();
}

struct GetSavedLogs getSavedLogs() {
  struct GetSavedLogs  savedLogs;
  if ( lastPositionFileLogSent == -1 ) {
    return savedLogs;
  }

  File file = SPIFFS.open("/logs.txt", "r");
  if (!file) {
#if DEBUG
    Serial.println("/logs.txt not found!");
#endif
  } else {
    file.seek(lastPositionFileLogSent);
    for (int iBatch = 0; iBatch < 20; iBatch++) {
      String nextLogContent = file.readStringUntil('|');
      if (nextLogContent.length() > 0 ) {
        savedLogs.logs +=  (iBatch > 0 ? "|" : "")  + nextLogContent;
        savedLogs.lastPosition = file.position() ;
      } else {
        if ( iBatch < 20 ) {
          savedLogs.lastPosition = -1;
        }
        break;
      }
    }

  }
  file.close();
  return savedLogs;
}

void deleteLogs() {
  if ( lastPositionFileLogSent == -1 ) {
    SPIFFS.remove("/logs.txt");
    lastPositionFileLogSent = 0;
#if DEBUG
    Serial.println("SPIFFS.remove(/logs.txt)");
#endif
  }

}

void espReset() {
#if DEBUG
  Serial.println("Restarting in 5 seconds");
  delay(5000);
#endif
  ESP.restart();
}
