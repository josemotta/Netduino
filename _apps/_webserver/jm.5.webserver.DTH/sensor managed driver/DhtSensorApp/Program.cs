//---------------------------------------------------------------------------
//<copyright file="Program.cs">
//
// Copyright 2011 Stanislav "CW" Simicek
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//</copyright>
//---------------------------------------------------------------------------
namespace DhtSensorApp
{
  using System.Threading;
  using CW.NETMF;
  using CW.NETMF.Sensors;
  using Microsoft.SPOT;
  using SecretLabs.NETMF.Hardware.Netduino;

  /// <summary>
  /// This application demonstrates the usage of DHT11/DHT22 sensors.
  /// </summary>
  public static class Program
  {
    public static void Main()
    {
      // TODO: Change pins and pull-up resistor type according to the actual wiring
      var dhtSensor = new Dht11Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.External);
    //var dhtSensor = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.External);

      while(true)
      {
        Thread.Sleep(2000);

        if(dhtSensor.Read())
        {
          Debug.Print("DHT sensor Read() ok, RH = " + dhtSensor.Humidity.ToString("F1") + "%, Temp = " + dhtSensor.Temperature.ToString("F1") + "°C");
        }
        else
        {
          Debug.Print("DHT sensor Read() failed");
        }
      }
    }
  }
}
