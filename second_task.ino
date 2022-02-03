/**
	Drones Competition Project, Second Task
	Name: second_task
	Purpose: Takes a character from the application, matches the command to the task and returns the answer if needed.
	@author The Wave
	@version 2.0 03/02/22

	The arduino takes the distance from the ultrasonic sensor
	Lights 8 red lights if the distance is more than a meter and less than 2 meters.  
	Lights another 8 blue lights if the distance is more than a 2 meter and less than 3 meters.  
	Lights another 8 green lights if the distance is more than a 3 meter and less than 4 meters.  
	Blinks all white lights if the distance is more than a 4 meter.

	Video Link - 
	Github Link - https://github.com/TheWaveDrones/DroneTasks-2022/
	You should check our github :) All our code is on the github on the Task-2 branch also we have there the 3D model of the drone
	we planned and are going to print and we also have there the schema of the circuit.
*/

#include <Adafruit_NeoPixel.h>


// Arduino constants
#define PIN A1
#define ECHO_PIN 3 
#define SERIAL_SPEED 9600

// Code constants
#define NUM_PIXELS 24
#define VALUE 255
#define METER 100

// Declaring functions
word read_ultrasonic();
void open_red();
void open_blue();
void open_green();
void open_white();

// Declaring global variables
String str;
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUM_PIXELS, PIN, NEO_GRB + NEO_KHZ800);
word last_good_value;

void setup()
{
	pixels.begin();
	Serial.begin(SERIAL_SPEED);
	pinMode(ECHO_PIN, INPUT);
}

void loop()
{
	// Getting distance
	word distance = read_ultrasonic();
	if (distance >= 6 * METER)
		distance = last_good_value;
	else
		last_good_value = distance;
	str = "Distance = " + String(distance);
	Serial.println(str);
	delay(50);

	// Lighting leds according to the distance
	if(distance < METER) {
		pixels.clear();
	} else if(distance < 2 * METER) {
		open_red();
	} else if(distance < 3 * METER) {
		open_blue();
	} else if (distance < 4 * METER) {
		open_green();
	} else if (distance < 6 * METER)
		open_white();

	pixels.show();
	pixels.clear();
}

/**
	Reads the values from the ultrasonic sensor and sends the distance in centimeters.

	@return word representing the distance from the ultrasonic in centimeters.
*/
word read_ultrasonic()
{
	word duration;
	word distance;
	duration = pulseIn(ECHO_PIN, HIGH);
	distance = duration/58;
	return distance;
}

/**
	Opens the red colors on the RGB neopixel.
*/
void open_red()
{
	for(int i = 0; i < 8; i++) {
		pixels.setPixelColor(i,VALUE,0,0);
	}
}

/**
	Opens the blue and red colors on the RGB neopixel.
*/
void open_blue()
{
	open_red();
	for(int i = 8; i < 16; i++) {
		pixels.setPixelColor(i,0,0,VALUE);
	}
}

/**
	Opens the green, blue and red colors on the RGB neopixel.
*/
void open_green()
{
	open_blue();
	for(int i = 16; i < 24; i++) {
		pixels.setPixelColor(i,0,VALUE,0);
	}
}

/**
	Opens the white colors on the RGB neopixel and lights them up.
*/
void open_white()
{
	for(int i = 0; i < 24; i++) {
		pixels.setPixelColor(i, VALUE, VALUE, VALUE);
  	}
	pixels.show();
	delay(500);
	pixels.clear();
	pixels.show();

	for(int i = 0; i < 24; i++) {
		pixels.setPixelColor(i,VALUE,VALUE,VALUE);
	}
	delay(500);
	pixels.show();
	delay(500);
	pixels.show();
	delay(500);

	pixels.clear();
	pixels.show();
}
