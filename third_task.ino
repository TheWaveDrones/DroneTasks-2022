/**
  Drones Competition Project, Third Task
  Name: third_task
  Purpose: Takes a character from the application, runs a led and returns the temperature from the sensor.
  @author The Wave
  @version 1.0 08/02/22

  The arduino waits for a character from the application, runs a led and sends back the
  temperature from the temperature sensor to the application.

  Video Link - https://www.youtube.com/watch?v=v7VLhLtnyf4
  Github Link - https://github.com/TheWaveDrones/DroneTasks-2022/
  You should check our github :) All our code is on the github on the Task-3 branch.
*/

#include <Adafruit_NeoPixel.h>
#include <Wire.h>
#include<SoftwareSerial.h>


#define PIN A1
#define NUM_PIXELS 24
#define VALUE 200
#define LM75_addr 0x48 
#define BLUETOOTH_READ 10
#define BLUETOOTH_WRITE 11
#define COMMUNICATION_SPEED 9600

SoftwareSerial bluetooth(BLUETOOTH_READ, BLUETOOTH_WRITE);
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUM_PIXELS,PIN,NEO_GRB + NEO_KHZ800);
float valtemp = 0;

void setup() {
  pixels.begin();
  Wire.begin();
  bluetooth.begin(COMMUNICATION_SPEED);
  configTemp(LM75_addr);
}

void loop() {
  char input_byte;
  if(bluetooth.available()) { 
    input_byte= bluetooth.read();
    if (input_byte == 't'){
        led_runner();
        delay(500);
        bluetooth.print("Temperature : ");
        valtemp = readTemp(LM75_addr);
        bluetooth.println(valtemp);
        delay(1000);
    }
  }
}

void led_runner(){
  for(int i =0; i < NUM_PIXELS; i ++){
    pixels.setPixelColor(i, VALUE/2, 0, VALUE);
    pixels.show();
    delay(50);
    pixels.clear();
    pixels.show();
  }
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
