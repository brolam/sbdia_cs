#include "FS.h"
FSInfo fs_info;

void setup() {
  Serial.begin(115200);
  while (!SPIFFS.begin())
  {
    Serial.println("SPIFFS Initialization...failed");
    delay(2000);
  }
  Serial.println("SPIFFS Initialize....ok");
}

void loop() {
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
  Serial.println("");
  delay(1000);
  if (  Serial.readString() == "F\n" )
  {
    Serial.println("");
    if (SPIFFS.format())
    { 
      Serial.println("File System Formated");
    }
    else
    {
      Serial.println("File System Formatting Error");
    }
    Serial.println("");
  }
}
