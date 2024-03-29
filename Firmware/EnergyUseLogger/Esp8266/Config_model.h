const char *ESP8266_NAME = "OHA_EUL_1";
const char *ESP8266_PASSWORD = "123";
const char *WIFI_SSID = "WIFI_SSID";
const char *WIFI_PASSWORD = "WIFI_PASSWORD";
const char *SENSOR_ID = "SENSOR_ID";
const char *SECRET_API_TOKEN = "SECRET_API_TOKEN";
const char *HTTPS_FRINGER_PRINT = "HTTPS_FRINGER_PRINT";
const int INTERVAL_TIME_SAVE_LOG = 10;
const String END_POINT_HOST = "HOST:PORT";
const String END_POINT_URL_LOG_BATCH = "https://" + END_POINT_HOST + "/api/sensor/" + String(SENSOR_ID) + "/logBatch";
const String END_POINT_URL_UNIX_TIME_UTC = "https://" + END_POINT_HOST + "/api/sensor/" + String(SENSOR_ID) + "/unixTimeUtc";
