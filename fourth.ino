/**
  Drones Competition Project, Fourth Task
  Name: fourth
  Purpose: Takes a character from the application, and does the paired command.
  @author The Wave
  @version 2.0 17/02/22

  The arduino waits for a character from the application, sends back one of the following
  the temperature from the temperature sensor, the distance from the ultrasonic solo or via stream
  to the application.

  Video Link - https://youtu.be/YDBQnP9UXbI
  Github Link - https://github.com/TheWaveDrones/DroneTasks-2022/
  You should check our github :) All our code is on the github on the Task-4 branch.
*/

#include <Wire.h>
#include<SoftwareSerial.h>

//pin consts
#define ECHO_PIN A0
#define TRIG_PIN A4

// reducing magic numbers in the code
#define SECOND 1000
#define SERIAL_SPEED 9600

// sensors consts
#define LM75_addr 0x48
#define BLUETOOTH_READ 13
#define BLUETOOTH_WRITE 12
SoftwareSerial bluetooth(BLUETOOTH_READ,BLUETOOTH_WRITE);
float valtemp = 0;

char input_byte = '\0'; // variable which stores the input from the bluetooth module

// variables for the ultra-sonic sensor
long duration = 0; // variable for the duration of sound wave travel
int distance = 0; // variable for the distance measurement




void setup(){
  // ultra-sonic sensor setup:
  pinMode(TRIG_PIN, OUTPUT); // sets the TRIG_PIN as an output
  pinMode(ECHO_PIN, INPUT); // sets the ECHO_PIN as an input
  Wire.begin();
  bluetooth.begin(9600);
  configTemp(LM75_addr);
}


void loop(){
  if(bluetooth.available() > 0) { // checks if something has been sent to the bluetooth module
  input_byte= bluetooth.read(); // stores the character that has been sent to the bluetooth module
    if(input_byte == 'd') { // sends distance once when the character 'd' is received
      PrintDistance();
    } else if(input_byte == 'c') { // sends distance continuously when the character 'c' is received
      while(input_byte == 'c') {
        if(bluetooth.read() == 's') { // stops sending distance when the character 's' is received
          input_byte = 's';
        } else {
          PrintDistance();
          delay(SECOND);
        }
      }
    } else if(input_byte == 't') { // sends temperature once when the character 't' is received 
      bluetooth.print("*");
      bluetooth.print("Temperature : ");
      valtemp = readTemp(LM75_addr);
      bluetooth.println(valtemp);
    }
  }
}

/*
  Calculates and sends the distance to the application through the bluetooth module.
*/
void PrintDistance()
{
  // creates a pulse of ultrasonic waves for 10 microsecond
  digitalWrite(TRIG_PIN,LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN,HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN,LOW);

  // measure the time until the ultrasonic waves come back
  duration = pulseIn(ECHO_PIN,HIGH);

  // calculating distance from duration
  // 0.0343 == distance in cm every microsecond (34,300cm in second / 1,000,000 turns it to microsecond)
  distance = (duration / 2) * 0.0343;

  // sends the calculated distance to the serial monitor and in turn to the bluetooth module
  bluetooth.print("Distance: ");
  bluetooth.print(distance);
  bluetooth.println(" cm");
}

void configTemp(byte addr){
  Wire.beginTransmission(addr);
  Wire.write(0x0);
}

float readTemp(byte addr){
  float temp;
  Wire.requestFrom(addr, 2);
  if(Wire.available()){
    temp = ((Wire.read() << 8 | Wire.read()) >> 5) *0.125;
    return temp;
  }
}
