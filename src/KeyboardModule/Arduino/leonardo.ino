#include <Keyboard.h>
byte cmd[3];

void setup() {
  // start serial communication at 9600bps:
  Serial.begin(9600);
  // Initialize control over the keyboard:
  Keyboard.begin();
}

void loop() {
  // check if data has been sent from the computer:
  if (Serial.available()>= 3) {
    // read the first byte:
    Serial.readBytes(cmd, 3);


    // press or release the key based on the control byte:
    if (cmd[0] == 1) { // press
      Keyboard.release((char)cmd[1]);
    } else if (cmd[0] == 2) { // release
      Keyboard.press((char)cmd[1]);
    } else if (cmd[0] == 3) { // print
      Keyboard.print((char)cmd[1]);
    }else if (cmd[0] == 4) { // release all 
      Keyboard.releaseAll();
    }
  }
}