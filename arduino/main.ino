/**
  Drones Competition Project, First Task
  Name: first_task
  Purpose: Takes a character from the application, matches the command to the task and returns the answer if needed.

  @author The Wave
  @version 4.0 16/01/22

  If the code receives the character 'd'(distance), 
  from the application it wiil return the distance once.
  And if the code receives the character 'c'(continuous) it will stream the distance
  to the application until the character 's'(stop) is recieved.

  Video Link - 
  Github Link -
*/


// setting the ulta-sonic sensors pins as 2 (for echo) and 4 (for trig)
#define ECHO_PIN 2
#define TRIG_PIN 4

#define SECOND 1000
#define SERIAL_SPEED 9600

void PrintDistance();

char input_byte = '\0'; // variable which stores the input from the bluetooth module

// variables for the ultra-sonic sensor
long duration = 0; // variable for the duration of sound wave travel
int distance = 0; // variable for the distance measurement

void setup()
{
  // bluetooth module setup:
  Serial.begin(SERIAL_SPEED); // starts the serial monitor in order to communicate with the bluetooth module

  // ultra-sonic sensor setup:
  pinMode(TRIG_PIN, OUTPUT); // sets the TRIG_PIN as an output
  pinMode(ECHO_PIN, INPUT); // sets the ECHO_PIN as an input
}

void loop()
{
if(Serial.available() > 0) { // checks if something has been sent to the bluetooth module
  input_byte= Serial.read(); // stores the character that has been sent to the bluetooth module
    if(input_byte == 'd') { // sends distance once when the character 'd' is received
      PrintDistance();
    } else if(input_byte == 'c') { // sends distance continuously when the character 'c' is received
      while(input_byte == 'c') {
        if(Serial.read() == 's') { // stops sending distance when the character 's' is received
          input_byte = 's';
        } else {
          PrintDistance();
          delay(SECOND);
        }
      }
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
  Serial.print("Distance: ");
  Serial.print(distance);
  Serial.println(" cm");
}