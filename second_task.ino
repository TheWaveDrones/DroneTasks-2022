#include <Adafruit_NeoPixel.h>


#define PIN A1
#define NUM_PIXELS 24
#define VALUE 255
#define ECHO_PIN 3 
#define METER 100

String str;
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUM_PIXELS,PIN,NEO_GRB + NEO_KHZ800);
word last_good_value;

void setup() {
  // put your setup code here, to run once:
  pixels.begin();
  Serial.begin(9600);
  pinMode(ECHO_PIN, INPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  word distance = read_ultrasonic();
  if(distance >= 600)
    distance = last_good_value;
  else
    last_good_value = distance;
  str = "Distance = " + String(distance);
  Serial.println(str);
  delay(50);

  if(distance < METER)
    pixels.clear();
  else if(distance < 2* METER)
    open_red();
  else if(distance < 3 * METER)
    open_blue();
  else if (distance < 4 * METER)
    open_green();
  else if (distance < 6 * METER)
    open_white();
  pixels.show();
  pixels.clear();
}


word read_ultrasonic(){
  word duration;
  word distance;
  duration = pulseIn(ECHO_PIN, HIGH);
  distance = duration/58;
  return distance;
}

void open_red(){
  for(int i = 0; i < 8; i++){
    pixels.setPixelColor(i,VALUE,0,0);
  }
  
}

void open_blue(){
  open_red();
  for(int i = 8; i < 16; i++){
    pixels.setPixelColor(i,0,0,VALUE);
  }
  
}

void open_green(){
  open_blue();
  for(int i = 16; i < 24; i++){
    pixels.setPixelColor(i,0,VALUE,0);
  }
  
}

void open_white(){
  for(int i = 0; i < 24; i++){
    pixels.setPixelColor(i,VALUE,VALUE,VALUE);
  }
  pixels.show();
  delay(500);
  pixels.clear();
  pixels.show();
  for(int i = 0; i < 24; i++){
    pixels.setPixelColor(i,VALUE,VALUE,VALUE);
  }
  delay(500);
  for(int i = 0; i < 24; i++){
    pixels.setPixelColor(i,VALUE,VALUE,VALUE);
  }
  pixels.show();
  delay(500);
  pixels.show();
  delay(500);
  pixels.clear();
  pixels.show();
}
