const int analogInPin = A0;  // Analog input pin that the potentiometer is attached to

int sensorValue = 0;        // value read from the pot
int samplesCount = 0; 
void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
  delay(5000);
}

void loop() {
  if ( samplesCount == 5000 ) { 
    return;
  }
  // read the analog in value:
  sensorValue = analogRead(analogInPin);
  //if (sensorValue == 0 ) return; 
  Serial.println(sensorValue);
  samplesCount = samplesCount + 1;
}
