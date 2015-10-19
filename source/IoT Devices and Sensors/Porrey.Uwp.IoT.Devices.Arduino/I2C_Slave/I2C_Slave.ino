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
  Version 1.0

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
// *** Define the registers (commands)
// ***
#define PIN_MODE      0x00
#define DIGITAL_READ  0x01
#define DIGITAL_WRITE 0x02
#define ANALOG_READ   0x03
#define ANALOG_WRITE  0x04
#define TONE          0x05
#define NO_TONE       0x06

// ***
// *** Result/error codes
// ***
#define RESULT_SUCCESS            0
#define RESULT_BUFFER_TOO_SMALL   1

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

  Serial.println("Ready!");
}

void loop()
{
  delay(100);
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

void pinModeCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 3)
  {
    // ***
    // *** Set pin Mode
    // ***
    Serial.print("pinMode(pin = ");
    Serial.print(buffer[1]);
    Serial.print(", mode = ");

    if (buffer[2] == 0)
    {
      Serial.println("INPUT)");
    }
    else if (buffer[2] == 1)
    {
      Serial.println("OUTPUT)");
    }
    else if (buffer[2] == 2)
    {
      Serial.println("INPUT_PULLUP)");
    }

    pinMode(buffer[1], buffer[2]);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 3);
  }
}

void digitalReadCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 2)
  {
    // ***
    // *** Read from a digital port
    // ***
    Serial.print("digitalRead(pin = D");
    Serial.print(buffer[1]);
    Serial.print(") = ");

    byte value = digitalRead(buffer[1]);
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
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 2);
  }
}

void digitalWriteCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 3)
  {
    // ***
    // *** Digital write
    // ***
    Serial.print("digitalWrite(pin = D");
    Serial.print(buffer[1]);
    Serial.print(", value = ");

    if (buffer[2] == 1)
    {
      Serial.println("HIGH)");
    }
    else
    {
      Serial.println("LOW)");
    }

    digitalWrite(buffer[1], buffer[2]);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 3);
  }
}

void analogReadCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 2)
  {
    // ***
    // ***
    // ***
    Serial.print("analogRead(pin = A");
    Serial.print(buffer[1]);
    Serial.print(") = ");

    byte value = analogRead(buffer[1]);

    Serial.println(value);

    _bufferLength = 1;
    _outBuffer[0] = { value };

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 2);
  }
}

void analogWriteCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 2)
  {
    // ***
    // *** Analog write
    // ***
    Serial.print("analogWrite(pin = A");
    Serial.print(buffer[1]);
    Serial.print(", value = ");
    Serial.println(buffer[2]);

    analogWrite(buffer[1], buffer[2]);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 2);
  }
}

void toneCommand(int bufferSize, byte buffer[])
{
  unsigned int frequency = 0;
  frequency = buffer[3];
  frequency <<= 8;
  frequency += buffer[2];

  if (bufferSize >= 8)
  {
    unsigned long duration = 0;
    duration = buffer[7];
    duration <<= 8;
    duration += buffer[6];
    duration <<= 8;
    duration += buffer[5];
    duration <<= 8;
    duration += buffer[4];

    // ***
    // *** tone(pin, frequency, duration)
    // ***
    Serial.print("Set tone(Pin = ");
    Serial.print(buffer[1]);
    Serial.print(", Frequency = ");
    Serial.print(frequency);
    Serial.print(", Duration = ");
    Serial.print(duration);
    Serial.println(")");

    tone(buffer[1], frequency, duration);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else if (bufferSize >= 4)
  {
    // ***
    // *** tone(pin, frequency)
    // ***
    Serial.print("Set tone(Pin = ");
    Serial.print(buffer[1]);
    Serial.print(", Frequency = ");
    Serial.print(frequency);
    Serial.println(")");

    tone(buffer[1], frequency);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 8);
  }
}

void noToneCommand(int bufferSize, byte buffer[])
{
  if (bufferSize >= 2)
  {
    // ***
    // *** noTone(pin)
    // ***
    Serial.print("Set noTone(Pin = ");
    Serial.print(buffer[1]);
    Serial.println(")");

    noTone(buffer[1]);

    // ***
    // *** Set the result
    // ***
    setResult(RESULT_SUCCESS, 0);
  }
  else
  {
    // ***
    // *** Specify that the buffer is too small. The
    // *** value passed is the expected size of the
    // *** input buffer.
    // ***
    setResult(RESULT_BUFFER_TOO_SMALL, 2);
  }
}

// ***
// *** Callback for received data
// ***
void receiveData(int byteCount)
{
  Serial.print("Data received: [");
  Serial.print(byteCount);
  Serial.println(" byte(s)]");
  Serial.print("\tCommand: ");

  byte buffer[byteCount];
  int count = getBuffer(buffer);

  // ***
  // *** The first byte is the register/command
  // ***
  if (buffer[0] == PIN_MODE)
  {
    pinModeCommand(byteCount, buffer);
  }
  else if (buffer[0] == DIGITAL_READ)
  {
    digitalReadCommand(byteCount, buffer);
  }
  else if (buffer[0] == DIGITAL_WRITE)
  {
    digitalWriteCommand(byteCount, buffer);
  }
  else if (buffer[0] == ANALOG_READ)
  {
    analogReadCommand(byteCount, buffer);
  }
  else if (buffer[0] == ANALOG_WRITE)
  {
    analogWriteCommand(byteCount, buffer);
  }
  else if (buffer[0] == TONE)
  {
    toneCommand(byteCount, buffer);
  }
  else if (buffer[0] == NO_TONE)
  {
    noToneCommand(byteCount, buffer);
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
