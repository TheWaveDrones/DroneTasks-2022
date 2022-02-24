#include<SoftwareSerial.h>
#include <Servo.h>


#define ECHO_PIN A5
#define METER 100
#define BLUETOOTH_READ 12 //WIP
#define BLUETOOTH_WRITE 13 //WIP
#define SERVO1_PIN A4 
#define SERVO2_PIN A3
#define SERVO_POS 90
#define BOUD_RATE 9600

String str;
word last_good_value;
bool servo_one_button;
bool servo_two_button;
char input_byte = '\0';
Servo servo1;
Servo servo2;

SoftwareSerial bluetooth(BLUETOOTH_READ,BLUETOOTH_WRITE);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(BOUD_RATE);
  bluetooth.begin(BOUD_RATE);

  pinMode(ECHO_PIN, INPUT);
  servo1.attach(SERVO1_PIN);
  servo2.attach(SERVO2_PIN);

  
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
  if(bluetooth.available() > 0) { // checks if something has been sent to the bluetooth module
    input_byte = bluetooth.read();
    if(input_byte == 'a'){
      if(servo_one_button)
        servo_one_button = false;
      else
        servo_one_button = true;
    } else if(input_byte == 'b'){
      if(servo_two_button)
        servo_two_button = false;
      else 
        servo_two_button = true;
    }
  }
  if(distance > 3 * METER || (distance > 2 * METER && servo_two_button) || (servo_two_button && servo_one_button)){
    start_both_servos();
  } else if (servo_two_button && distance <= 2 * METER){
    start_second_servo();
  } else if(distance > 2 * METER || servo_one_button){
    start_servo();
  }
}


word read_ultrasonic(){
  word duration;
  word distance;
  duration = pulseIn(ECHO_PIN, HIGH);
  distance = duration/58;
  return distance;
}

void start_servo(){
    for(int servo_pos = SERVO_POS; servo_pos >= 0; --servo_pos){
      servo1.write(servo_pos);
      delay(15);
    }
}

void start_second_servo(){
    for(int servo_pos = SERVO_POS; servo_pos >= 0; --servo_pos){
      servo2.write(servo_pos);
      delay(15);
    }
}


void start_both_servos(){
    for(int servo_pos = SERVO_POS; servo_pos >= 0; --servo_pos){
      servo1.write(servo_pos);
      servo2.write(servo_pos);
      delay(15);
    }
}
