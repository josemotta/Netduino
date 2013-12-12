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

    ' In the global scope so we can call the leds from the events
    Public Leds(16) As OutputPortShift

    Sub Main()
        ' Defining two 74HC165s daisychained on the SPI bus, pin 10 as latchpin
        Dim IcInChain As Ic74HC165Chain = New Ic74HC165Chain(SPI_Devices.SPI1, Pins.GPIO_PIN_D10, 2)

        ' Defining two 74HC595s daisychained on the SPI bus, pin 9 as latchpin
        Dim IcOutChain As Ic74HC595Chain = New Ic74HC595Chain(SPI_Devices.SPI1, Pins.GPIO_PIN_D9, 2)

        ' Splits up both 74HC165s. Detail: counting starts at 0
        Dim IcIn1 As Ic74HC165 = New Ic74HC165(IcInChain, 0)
        Dim IcIn2 As Ic74HC165 = New Ic74HC165(IcInChain, 1)

        ' Splits up both 74HC595s. Detail: counting starts at 0
        Dim IcOut1 As Ic74HC595 = New Ic74HC595(IcOutChain, 0)
        Dim IcOut2 As Ic74HC595 = New Ic74HC595(IcOutChain, 1)

        ' Defines all 16 leds
        Dim Counter As Integer
        For Counter = 0 To 7
            ' Leds on the first Ic
            Leds(Counter) = New OutputPortShift(IcOut1, CType(Counter, Ic74HC595.Pins), False)
            ' Leds on the second Ic
            Leds(Counter + 8) = New OutputPortShift(IcOut2, CType(Counter, Ic74HC595.Pins), False)
        Next

        ' Defines all 16 buttons
        ' Because 'WithEvents' variables cannot be typed as arrays, we need to allocate the events a bit different in Visual Basic, so we use the AddHandler method
        Dim Buttons(16) As InterruptPortShift
        For Counter = 0 To 7
            ' Buttons on the first Ic
            Buttons(Counter) = New InterruptPortShift(IcIn1, CType(Counter, Ic74HC165.Pins), False, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth)
            AddHandler Buttons(Counter).OnInterrupt, AddressOf IcIn1_OnInterrupt
            Buttons(Counter).EnableInterrupt()
            ' Buttons on the second Ic
            Buttons(Counter + 8) = New InterruptPortShift(IcIn2, CType(Counter, Ic74HC165.Pins), False, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth)
            AddHandler Buttons(Counter + 8).OnInterrupt, AddressOf IcIn2_OnInterrupt
            Buttons(Counter + 8).EnableInterrupt()
        Next

        ' Wait infinite; let the events to their jobs
        Thread.Sleep(Timeout.Infinite)
    End Sub

    ''' <summary>
    ''' Event triggered when a state changes
    ''' </summary>
    ''' <param name="PinId">The changed pin number</param>
    ''' <param name="Value">The current value</param>
    ''' <param name="Time">Time and date of the event</param>
    Sub IcIn1_OnInterrupt(ByVal PinId As UInteger, ByVal Value As UInteger, ByVal Time As Date)
        Debug.Print("State changed of 74HC165 IC 1 port " & PinId.ToString() & " to " & Value.ToString())
        Leds(CInt(PinId)).Write(Value = 0)
    End Sub

    ''' <summary>
    ''' Event triggered when a state changes
    ''' </summary>
    ''' <param name="PinId">The changed pin number</param>
    ''' <param name="Value">The current value</param>
    ''' <param name="Time">Time and date of the event</param>
    Sub IcIn2_OnInterrupt(ByVal PinId As UInteger, ByVal Value As UInteger, ByVal Time As Date)
        Debug.Print("State changed of 74HC165 IC 2 port " & PinId.ToString() & " to " & Value.ToString())
        Leds(CInt(8 + PinId)).Write(Value = 0)
    End Sub

End Module
