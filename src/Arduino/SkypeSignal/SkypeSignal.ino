#include <Adafruit_NeoPixel.h>

#include "BluetoothSerial.h" 

#define ENABLE_SERIAL 1
#define ENABLE_BT     1

//Const:
int dataPin = 27;  // Arduino PWM data pin D6
int pixels = 1;    // number of Neopixels
int SkypeCommand = 0;
int LoopCommand;

// Parameter 1 = number of pixels in strip
// Parameter 2 = Arduino pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)

#ifdef ENABLE_BT
int BTCommand;
BluetoothSerial ESP_BT; //Object for Bluetooth
#endif

Adafruit_NeoPixel strip=Adafruit_NeoPixel(pixels, dataPin, NEO_GRB + NEO_KHZ800);

void setup()
{

#ifdef ENABLE_SERIAL
  Serial.begin(115200); //Start Serial monitor in 115200
#endif  

#ifdef ENABLE_BT
  ESP_BT.begin("SkypeSignalBT"); //Name of your Bluetooth Signal
#endif

  pinMode (dataPin, OUTPUT);//Specify that LED pin is output

  strip.begin();
  strip.show();
}

void loop()
{

#if ENABLE_SERIAL
   //Read Input and assume that its a int type not a char
   if (Serial.available()>0)
   {
     SkypeCommand = Serial.read() - '0';
   }
#endif
  
#ifdef ENABLE_BT
  if (ESP_BT.available()>0) //Check if we receive anything from Bluetooth
  {
    SkypeCommand = ESP_BT.read() - '0'; //Read what we recevive 
  }
#endif

   if (SkypeCommand > 0)
   {
    LoopCommand = SkypeCommand;
   }
  
   switch (LoopCommand)
   {
    case 1:
      //Online [Green]
      SetLedColour(255, 0, 0);
      break;
    case 2:
      //Busy [Red]
      SetLedColour(0, 255, 0);
      break;
    case 3:
      //Do Not Disturb [Purple]
      SetLedColour(0, 148, 211);
      break;
   case 4:
      //Away/Idle/BRB [Yellow]
      SetLedColour(95, 255, 0);
      break;
    case 5:
      //On a Call/Conf Call [Pulsing Red]
        InACall(); 
      break;
     case 6:
      //Incoming Call
      IncomingCall();
      break;
    case 7:
      //Party Mode
      PartyMode();
      break;
    default:
      //LED's are off
      SetLedColour(0, 0, 0);  
   }  
  	//Echo the output
  	//Serial.println(LoopCommand);  
  
}

void InACall()
{  
    // Fade IN
    for(int k = 0; k < 255; k++) 
    { 
       SetLedColour(0, k, 0);
       delay(4);     
    }
    
     // Fade OUT
    for(int k = 255; k > 0; k--) 
    { 
        SetLedColour(0, k, 0);
        delay(4);     
    }
}

void IncomingCall()
{
    SetLedColour(0, 0, 255);
    delay(1500);
    SetLedColour( 95, 255, 0);
    delay(200);  
}

void SetLedColour(int G, int R, int B)
{
	for (int i = 0; i < pixels; i++) 
	{
		strip.setPixelColor(i, R, G, B);
	}
	strip.show();
 
  //delay(5000); //Snooze for 5 secs so we're not spamming serial connection.
}

void PartyMode()
{
 for (int i = 0; i < pixels; i++) 
  {
    strip.setPixelColor(i, random(0, 255), random(0, 255), random(0, 255));
    strip.show();
    delay(50); //flash delay
    strip.setPixelColor(i, 0, 0, 0);
    strip.show();
    delay(50);
  }
}
