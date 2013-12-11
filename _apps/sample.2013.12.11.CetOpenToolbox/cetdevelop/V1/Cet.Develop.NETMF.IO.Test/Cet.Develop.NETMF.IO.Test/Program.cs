﻿//#define MASTER_TCP
//#define MASTER_UDP
//#define MASTER_RTU
#define SLAVE_RTU
//#define SLAVE_TCP
//#define SLAVE_UDP

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using System.IO.Ports;

using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using Cet.Develop.NETMF.IO.Protocols;

/*
 * Copyright 2012 Mario Vernari (http://cetdevelop.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * 09/Apr/2012
 *  Changed the pin assignment to fit the UART test
 *  
 * 20/Apr/2012
 *  Added the "force multiple coils" command handler
 *  Corrected some imperfection
 **/
namespace Cet.Develop.NETMF.IO
{
    public class Program
    {
        private static InputPort[] _inputs;
        private static OutputPort[] _coils;
        private static AnalogInput[] _analogs;



        public static void Main()
        {
#if MASTER_TCP || MASTER_UDP || SLAVE_TCP || SLAVE_UDP
            //setup the board IP
            NetworkInterface.GetAllNetworkInterfaces()[0]
                .EnableStaticIP("192.168.0.99", "255.255.255.0", "192.168.0.1");

            string localip = NetworkInterface.GetAllNetworkInterfaces()[0]
                .IPAddress;

            Debug.Print("The local IP address of your Netduino Plus is " + localip);

            //define coils, inputs, and analogs
            _inputs = new InputPort[8];
            _coils = new OutputPort[6];
            _analogs = new AnalogInput[6];

            _inputs[0] = new InputPort(Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullUp);
            _inputs[1] = new InputPort(Pins.GPIO_PIN_D1, true, Port.ResistorMode.PullUp);
            _inputs[2] = new InputPort(Pins.GPIO_PIN_D2, true, Port.ResistorMode.PullUp);
            _inputs[3] = new InputPort(Pins.GPIO_PIN_D3, true, Port.ResistorMode.PullUp);
            _inputs[4] = new InputPort(Pins.GPIO_PIN_D4, true, Port.ResistorMode.PullUp);
            _inputs[5] = new InputPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.PullUp);
            _inputs[6] = new InputPort(Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp);
            _inputs[7] = new InputPort(Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp);

            _coils[0] = new OutputPort(Pins.GPIO_PIN_D8, false);
            _coils[1] = new OutputPort(Pins.GPIO_PIN_D9, false);
            _coils[2] = new OutputPort(Pins.GPIO_PIN_D10, false);
            _coils[3] = new OutputPort(Pins.GPIO_PIN_D11, false);
            _coils[4] = new OutputPort(Pins.GPIO_PIN_D12, false);
            _coils[5] = new OutputPort(Pins.GPIO_PIN_D13, false);

            _analogs[0] = new AnalogInput(Pins.GPIO_PIN_A0);
            _analogs[1] = new AnalogInput(Pins.GPIO_PIN_A1);
            _analogs[2] = new AnalogInput(Pins.GPIO_PIN_A2);
            _analogs[3] = new AnalogInput(Pins.GPIO_PIN_A3);
            _analogs[4] = new AnalogInput(Pins.GPIO_PIN_A4);
            _analogs[5] = new AnalogInput(Pins.GPIO_PIN_A5);
#else
            //define coils, inputs, and analogs
            _inputs = new InputPort[4];
            _coils = new OutputPort[6];
            _analogs = new AnalogInput[6];

            _inputs[0] = new InputPort(Pins.GPIO_PIN_D4, true, Port.ResistorMode.PullUp);
            _inputs[1] = new InputPort(Pins.GPIO_PIN_D5, true, Port.ResistorMode.PullUp);
            _inputs[2] = new InputPort(Pins.GPIO_PIN_D6, true, Port.ResistorMode.PullUp);
            _inputs[3] = new InputPort(Pins.GPIO_PIN_D7, true, Port.ResistorMode.PullUp);

            _coils[0] = new OutputPort(Pins.GPIO_PIN_D8, false);
            _coils[1] = new OutputPort(Pins.GPIO_PIN_D9, false);
            _coils[2] = new OutputPort(Pins.GPIO_PIN_D10, false);
            _coils[3] = new OutputPort(Pins.GPIO_PIN_D11, false);
            _coils[4] = new OutputPort(Pins.GPIO_PIN_D12, false);
            _coils[5] = new OutputPort(Pins.GPIO_PIN_D13, false);

            _analogs[0] = new AnalogInput(Pins.GPIO_PIN_A0);
            _analogs[1] = new AnalogInput(Pins.GPIO_PIN_A1);
            _analogs[2] = new AnalogInput(Pins.GPIO_PIN_A2);
            _analogs[3] = new AnalogInput(Pins.GPIO_PIN_A3);
            _analogs[4] = new AnalogInput(Pins.GPIO_PIN_A4);
            _analogs[5] = new AnalogInput(Pins.GPIO_PIN_A5);
#endif

#if MASTER_TCP
            //create a TCP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //refine the socket settings
                socket.SetSocketOption(
                    SocketOptionLevel.Tcp,
                    SocketOptionName.NoDelay,
                    true
                    );

                socket.SendTimeout = 2000;
                socket.ReceiveTimeout = 2000;

                //try the connection against the remote device
                var remoteip = new byte[4] { 192, 168, 0, 60 };
                var ipaddr = new IPAddress(remoteip);
                var ept = new IPEndPoint(ipaddr, 502);
                socket.Connect(ept);

                //create a wrapper around the socket
                ICommClient portClient = socket.GetClient();

                //create a client driver
                var driver = new ModbusClient(new ModbusTcpCodec());
                driver.Address = 1;

                while (true)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncWriteMultipleRegisters);
                    command.Offset = 49;
                    command.Count = 4;

                    //attach the Netduino's input values as data
                    command.Data = new ushort[4];
                    for (int i = 0; i < 4; i++)
                        command.Data[i] = (ushort)(_inputs[i].Read() ? 0 : 1);

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(portClient, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                    }
                    else
                    {
                        //some error
                        Debug.Print("Error=" + command.ExceptionCode);
                    }

                    //just a small delay
                    Thread.Sleep(1000);
                }
            }
#endif

#if MASTER_UDP
            //create a UDP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                //try the connection against the remote device
                var remoteip = new byte[4] { 192, 168, 0, 60 };
                var ipaddr = new IPAddress(remoteip);
                var ept = new IPEndPoint(ipaddr, 502);
                socket.Connect(ept);

                //create a wrapper around the socket
                ICommClient portClient = socket.GetClient();

                //create a client driver
                var driver = new ModbusClient(new ModbusTcpCodec());
                driver.Address = 1;

                while (true)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncReadMultipleRegisters);
                    command.Offset = 0;
                    command.Count = 16;

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(portClient, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                        Debug.Print("Success!");
                        for (int i = 0; i < command.Count; i++)
                            Debug.Print("Reg#" + i + "=" + command.Data[i]);
                    }
                    else
                    {
                        //some error
                        Debug.Print("Error=" + command.ExceptionCode);
                    }

                    //just a small delay
                    Thread.Sleep(1000);
                }
            }
#endif

#if MASTER_RTU
            //create an UART port
            using (var uart = new SerialPort("COM2", 38400, Parity.Even, 8))
            {
                //open the serial port
                uart.Open();

                var prm = new SerialPortParams("38400,E,8,1");

                //create a wrapper around the uart
                ICommClient portClient = uart
                    .GetClient(prm);

                //create a client driver
                var driver = new ModbusClient(new ModbusRtuCodec());
                driver.Address = 1;

                while (true)
                {
                    //compose the Modbus command to be submitted
                    var command = new ModbusCommand(ModbusCommand.FuncWriteMultipleRegisters);
                    command.Offset = 49;
                    command.Count = 4;

                    //attach the Netduino's input values as data
                    command.Data = new ushort[4];
                    for (int i = 0; i < 4; i++)
                        command.Data[i] = (ushort)(_inputs[i].Read() ? 0 : 1);

                    //execute the command synchronously
                    CommResponse result = driver
                        .ExecuteGeneric(portClient, command);

                    if (result.Status == CommResponse.Ack)
                    {
                        //command successfully
                    }
                    else
                    {
                        //some error
                        Debug.Print("Error=" + command.ExceptionCode);
                    }

                    //just a small delay
                    Thread.Sleep(1000);
                }
            }
#endif

#if SLAVE_RTU
            //create an UART port
            using (var uart = new SerialPort("COM2", 38400, Parity.Even, 8))
            {
                //open the serial port
                uart.Open();

                var prm = new SerialPortParams("38400,E,8,1");

                //create a server driver
                var server = new ModbusServer(new ModbusRtuCodec());
                server.Address = 1;

                //listen for an incoming request
                var listener = uart.GetListener(server);
                listener.ServeCommand += new ServeCommandHandler(listener_ServeCommand);
                listener.Start();

                Thread.Sleep(Timeout.Infinite);
            }
#endif

#if SLAVE_TCP
            //create a TCP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                //place it as listener on the port 502 (standard Modbus)
                var ept = new IPEndPoint(IPAddress.Any, 502);
                socket.Bind(ept);
                socket.Listen(10);

                //create a server driver
                var server = new ModbusServer(new ModbusTcpCodec());
                server.Address = 1;

                while (true)
                {
                    //wait for an incoming connection
                    var listener = socket.GetTcpListener(server);
                    listener.ServeCommand += new ServeCommandHandler(listener_ServeCommand);
                    listener.Start();

                    Thread.Sleep(1);
                }
            }
#endif

#if SLAVE_UDP
            //create a UDP socket
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                //bind it to the port 502 (standard Modbus)
                var ept = new IPEndPoint(IPAddress.Any, 502);
                socket.Bind(ept);

                //create a server driver
                var server = new ModbusServer(new ModbusTcpCodec());
                server.Address = 1;

                //listen for an incoming request
                var listener = socket.GetUdpListener(server);
                listener.ServeCommand += new ServeCommandHandler(listener_ServeCommand);
                listener.Start();

                Thread.Sleep(Timeout.Infinite);
            }
#endif

        }


        /// <summary>
        /// Host server for the incoming request from a remote master
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void listener_ServeCommand(object sender, ServeCommandEventArgs e)
        {
            var command = (ModbusCommand)e.Data.UserData;

            //take the proper function command handler
            switch (command.FunctionCode)
            {
                case ModbusCommand.FuncReadCoils:
                    for (int i = 0; i < command.Count; i++)
                        command.Data[i] = (ushort)(_coils[i + command.Offset].Read() ? 1 : 0);
                    break;


                case ModbusCommand.FuncReadInputDiscretes:
                    for (int i = 0; i < command.Count; i++)
                        command.Data[i] = (ushort)(_inputs[i + command.Offset].Read() ? 1 : 0);
                    break;


                case ModbusCommand.FuncWriteCoil:
                    bool state = command.Data[0] != 0;
                    _coils[command.Offset].Write(state);
                    break;


                case ModbusCommand.FuncForceMultipleCoils:
                    for (int i = 0; i < command.Count; i++)
                        _coils[i + command.Offset].Write(command.Data[i] != 0);
                    break;


                case ModbusCommand.FuncReadInputRegisters:
                    for (int i = 0; i < command.Count; i++)
                        command.Data[i] = (ushort)_analogs[i + command.Offset].Read();
                    break;


                case ModbusCommand.FuncReadMultipleRegisters:
                case ModbusCommand.FuncWriteMultipleRegisters:
                case ModbusCommand.FuncWriteSingleRegister:
                case ModbusCommand.FuncReadExceptionStatus:
                    //TODO
                    break;


                default:
                    //return an exception
                    command.ExceptionCode = ModbusCommand.ErrorIllegalFunction;
                    break;
            }
        }


    }
}
