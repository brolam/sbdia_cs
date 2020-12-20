
int redPin = A6;
int greenPin = A7;
int bluePin = A8;

void setup() {
  Serial.begin(9600);
  pinMode(redPin, OUTPUT);
  turnOffAllLeds();
}

void turnOffAllLeds(){
  analogWrite(redPin, 0);
  analogWrite(greenPin, 0);
  analogWrite(bluePin, 0);
  Serial.println("CMD: turnOffAllLeds");
}

void printCommand(String cmd) {
  Serial.print("CMD: ");
  Serial.println(cmd);
}

void loop() {
  String cmd = Serial.readString();
  if ( cmd.length() > 0 ) printCommand(cmd);
  if ( cmd.indexOf("RED") > -1 ) {
    turnOffAllLeds();
    analogWrite(redPin, 100);
    analogWrite(greenPin,100);
    analogWrite(bluePin, 255);
    return;
  }

  if ( cmd.indexOf("GREEN") > -1 ) {
    turnOffAllLeds();
    analogWrite(redPin, 0);
    analogWrite(greenPin, 255);
    analogWrite(bluePin, 255);
    return;
  }

  if ( cmd.indexOf("BLUE") > -1 ) {
    turnOffAllLeds();
    analogWrite(redPin, 0);
    analogWrite(greenPin,50);
    analogWrite(bluePin, 255);
    return;
  }

  if ( cmd.indexOf("OFF") > -1 ) {
    turnOffAllLeds();
    return;
  }
  
}
