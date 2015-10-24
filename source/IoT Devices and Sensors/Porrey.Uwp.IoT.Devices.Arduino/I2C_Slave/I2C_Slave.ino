// Copyright © 2015 Daniel Porrey. All Rights Reserved.
//
// This file is part of the IoT Devices and Sensors project.
// 
// IoT Devices and Sensors is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// IoT Devices and Sensors is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with IoT Devices and Sensors. If not, 
// see http://www.gnu.org/licenses/.
//

/*
  Arduino i2c Slave
  written by Daniel Porrey
  Version 1.0.0

  This is the partner sketch for the C# library for communicating
  to the Arduino via the i2c bus from Windows 10 IoT Core on the
  Raspberry Pi 2.

  https://www.hackster.io/porrey

  The C# library is available in NuGet; ID = IoT.Arduino
*/

#include <Wire.h>

// ***
// *** Set the slave address to something unique. If this
// *** is the only i2c device then leave it as 0x04. If
// *** there are multiple i2c devices then make sure it is
// *** different than all of the other devices.
// ***
#define SLAVE_ADDRESS 0x04

// ***
// *** This currently does not need to be larger
// *** than 4. If you create custom commands,
// *** increase this value if you will return more
// *** than 4 bytes for any command.
// ***
#define MAX_OUTPUT_BUFFER 4

// ***
// ***
// ***
typedef void (* CommandCallback)(int, byte*);

// ***
// ***
// ***
struct RegisterMapping
{
  String name;
  unsigned int expectedByteCount;
  CommandCallback commandCallback;
};

// ***
// *** Array of commands supported. The idnex of the array
// *** is the register ID.
// ***
RegisterMapping _mappings[] =
{
  { "pinMode()", 4, pinModeCommand },
  { "digitalRead()", 3, digitalReadCommand },
  { "digitalWrite()", 4, digitalWriteCommand },
  { "analogRead()", 3, analogReadCommand },
  { "analogWrite()", 3, analogWriteCommand },
  { "tone() ", 9, toneCommand1 },
  { "tone() ", 5, toneCommand2 },
  { "noTone", 3, noToneCommand }
};

// ***
// *** Number of mappings defined
// ***
unsigned int _mappingCount = 0;

// ***
// *** Result/error codes
// ***
#define RESULT_SUCCESS                0
#define RESULT_UNEXPECTED_BUFFER      1
#define RESULT_COMMAND_NOT_SUPPORTED  2

// ***
// *** Output buffer. Note each command
// *** will wipe out the previous buffer.
// ***
int _bufferLength = 0;
byte _outBuffer[MAX_OUTPUT_BUFFER];

void setup()
{
  // ***
  // *** Start serial for output
  // ***
  Serial.begin(115200);

  // ***
  // *** Initialize i2c as slave
  // ***
  Wire.begin(SLAVE_ADDRESS);

  // ***
  // *** Define callbacks for i2c communication
  // ***
  Wire.onReceive(receiveData);
  Wire.onRequest(sendData);

  // ***
  // *** Calculate the mapping count
  // ***
  _mappingCount = (sizeof(_mappings) / sizeof(RegisterMapping));
  Serial.print("There are ");
  Serial.print(_mappingCount);
  Serial.println(" command register mappings defined.");

  Serial.println("Ready, waiting for commands...");
}

void loop()
{
  delay(100);
}

void pinModeCommand(int bufferSize, byte buffer[])
{
  // ***
  // *** Set pin Mode
  // ***
  Serial.print("\tpinMode(pin = ");
  Serial.print(buffer[2]);
  Serial.print(", mode = ");

  if (buffer[3] == 0)
  {
    Serial.println("INPUT)");
  }
  else if (buffer[3] == 1)
  {
    Serial.println("OUTPUT)");
  }
  else if (buffer[3] == 2)
  {
    Serial.println("INPUT_PULLUP)");
  }

  pinMode(buffer[2], buffer[3]);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void digitalReadCommand(int bufferSize, byte buffer[])
{
  // ***
  // *** Read from a digital port
  // ***
  Serial.print("\tdigitalRead(pin = D");
  Serial.print(buffer[2]);
  Serial.print(") = ");

  byte value = digitalRead(buffer[2]);
  _bufferLength = 1;
  _outBuffer[0] = value;

  if (value == HIGH)
  {
    Serial.println("HIGH");
  }
  else
  {
    Serial.println("LOW");
  }

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void digitalWriteCommand(int bufferSize, byte buffer[])
{
  // ***
  // *** Digital write
  // ***
  Serial.print("\tdigitalWrite(pin = D");
  Serial.print(buffer[2]);
  Serial.print(", value = ");

  if (buffer[3] == 1)
  {
    Serial.println("HIGH)");
  }
  else
  {
    Serial.println("LOW)");
  }

  digitalWrite(buffer[2], buffer[3]);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void analogReadCommand(int bufferSize, byte buffer[])
{
  // ***
  // ***
  // ***
  Serial.print("\tanalogRead(pin = A");
  Serial.print(buffer[2]);
  Serial.print(") = ");

  byte value = analogRead(buffer[2]);

  Serial.println(value);

  _bufferLength = 1;
  _outBuffer[0] = { value };

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void analogWriteCommand(int bufferSize, byte buffer[])
{
  // ***
  // *** Analog write
  // ***
  Serial.print("\tanalogWrite(pin = A");
  Serial.print(buffer[2]);
  Serial.print(", value = ");
  Serial.println(buffer[3]);

  analogWrite(buffer[2], buffer[3]);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void toneCommand1(int bufferSize, byte buffer[])
{
  // ***
  // *** Get the frequency
  // ***
  unsigned int frequency = 0;
  frequency = buffer[4];
  frequency <<= 8;
  frequency += buffer[3];

  // ***
  // *** Get the duration
  // ***
  unsigned long duration = 0;
  duration = buffer[8];
  duration <<= 8;
  duration += buffer[7];
  duration <<= 8;
  duration += buffer[6];
  duration <<= 8;
  duration += buffer[5];

  // ***
  // *** tone(pin, frequency, duration)
  // ***
  Serial.print("\ttone(Pin = ");
  Serial.print(buffer[2]);
  Serial.print(", Frequency = ");
  Serial.print(frequency);
  Serial.print(", Duration = ");
  Serial.print(duration);
  Serial.println(")");

  tone(buffer[2], frequency, duration);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void toneCommand2(int bufferSize, byte buffer[])
{
  // ***
  // *** Get the frequency
  // ***
  unsigned int frequency = 0;
  frequency = buffer[4];
  frequency <<= 8;
  frequency += buffer[3];

  // ***
  // *** tone(pin, frequency)
  // ***
  Serial.print("\ttone(Pin = ");
  Serial.print(buffer[2]);
  Serial.print(", Frequency = ");
  Serial.print(frequency);
  Serial.println(")");

  tone(buffer[2], frequency);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

void noToneCommand(int bufferSize, byte buffer[])
{
  // ***
  // *** noTone(pin)
  // ***
  Serial.print("\tnoTone(Pin = ");
  Serial.print(buffer[2]);
  Serial.println(")");

  noTone(buffer[2]);

  // ***
  // *** Set the result
  // ***
  setResult(RESULT_SUCCESS, 0);
}

int getBuffer(byte buffer[])
{
  int index = 0;

  // ***
  // *** The first byte is the register.
  // ***
  while (Wire.available())
  {
    buffer[index] = Wire.read();
    index++;
  }

  return index;
}

void setResult(byte result, byte value)
{
  _outBuffer[0] = result;
  _outBuffer[1] = value;
  _bufferLength = 2;
}

// ***
// *** Callback for received data
// ***
void receiveData(int byteCount)
{
  Serial.print("Data received: [");
  Serial.print(byteCount);
  Serial.println(" byte(s)]");

  // ***
  // *** Get the buffer
  // ***
  byte buffer[byteCount];
  int count = getBuffer(buffer);

  // ***
  // *** The first 2 bytes are the reigisterId. This
  // *** is mapped to the index in the mapping array.
  // ***
  unsigned int registerId = 0;
  registerId = buffer[1];
  registerId <<= 8;
  registerId += buffer[0];

  Serial.print("\tRegister ID  '");
  Serial.print(registerId);
  Serial.print(" => '");

  if (registerId < _mappingCount)
  {
    // ***
    // *** Show the name of the command
    // ***
    Serial.print(_mappings[registerId].name);
    Serial.println("'");

    if (byteCount == _mappings[registerId].expectedByteCount)
    {
      // ***
      // *** Call the commandCallback for this register
      // ***
      _mappings[registerId].commandCallback(byteCount, buffer);
    }
    else
    {
      Serial.print("\tThe buffer size for this command was unexpected. Expected size is ");
      Serial.println(_mappings[registerId].expectedByteCount);

      // ***
      // *** Specify that the buffer size was unexpected.
      // ***
      setResult(RESULT_UNEXPECTED_BUFFER, 3);
    }
  }
  else
  {
    // ***
    // *** Display not supported message
    // ***
    Serial.println("Not Supported");

    // ***
    // *** Specify that the registerId is not
    // *** supported.
    // ***
    setResult(RESULT_COMMAND_NOT_SUPPORTED, 2);
  }
}

// ***
// *** Callback for sending data
// ***
void sendData()
{
  Serial.println("Request for data:");
  Serial.print("\tThere are ");
  Serial.print(_bufferLength);
  Serial.println(" byte(s) available.");

  if (_bufferLength > 0)
  {
    for (int i = 0; i < _bufferLength; i++)
    {
      Serial.print("\tWriting byte[ ");
      Serial.print(i);
      Serial.print("] = ");
      Serial.println(_outBuffer[i]);
      Wire.write(_outBuffer[i]);
    }

    // ***
    // *** Clear the buffer
    // ***
    _bufferLength = 0;
  }
}
