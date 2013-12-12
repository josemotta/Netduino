Imports Microsoft.SPOT
Imports Microsoft.SPOT.Hardware
Imports SecretLabs.NETMF.Hardware
Imports SecretLabs.NETMF.Hardware.Netduino
Imports Toolbox.NETMF.Hardware

'  Copyright 2011 Stefan Thoolen (http://netmftoolbox.codeplex.com/)
'
'  Licensed under the Apache License, Version 2.0 (the "License");
'  you may not use this file except in compliance with the License.
'  You may obtain a copy of the License at
'
'      http://www.apache.org/licenses/LICENSE-2.0
'
'  Unless required by applicable law or agreed to in writing, software
'  distributed under the License is distributed on an "AS IS" BASIS,
'  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'  See the License for the specific language governing permissions and
'  limitations under the License.
Module Module1

    Sub Main()
        ' Defines the SN754410 IC on the correct pins
        Dim MotorDriver As SN754410 = New SN754410(Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4)

        ' Motor 1 half speed backward
        MotorDriver.SetState(SN754410.Motors.Motor1, -50)
        ' Motor 2 half speed forward
        MotorDriver.SetState(SN754410.Motors.Motor2, 50)

        ' Lets run for 5 seconds
        Thread.Sleep(5000)

        ' Motor 1 full speed backward
        MotorDriver.SetState(SN754410.Motors.Motor1, -100)
        ' Motor 2 full speed forward
        MotorDriver.SetState(SN754410.Motors.Motor2, 100)

        ' Lets run for 5 seconds
        Thread.Sleep(5000)

        ' Stops both motors
        MotorDriver.SetState(SN754410.Motors.Motor1, 0)
        MotorDriver.SetState(SN754410.Motors.Motor2, 0)
    End Sub

End Module
