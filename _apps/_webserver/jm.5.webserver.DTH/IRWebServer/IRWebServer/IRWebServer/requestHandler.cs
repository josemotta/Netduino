using System;
using System.IO;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using System.Text;
using System.Text.RegularExpressions;
using HttpLibrary;

namespace IRWebServer
{
    class requestHandler
    {
        private string _webfolder;
        private IRCommands _commander;
        private HIH6130 _sensor;
        private ACStatus _AC;
        private Timers _timers;
        private Credential _credentials;

        public requestHandler(string webfolder, IRCommands commander, HIH6130 tempSensor, ACStatus AC, Timers timers, Credential credentials)
        {
            _webfolder = webfolder;
            _commander = commander;
            _sensor = tempSensor;
            _AC = AC;
            _timers = timers;
            _credentials = credentials;
        }

        public void processRequest(HttpRequest request, HttpResponse response)
        {
            string requestedFile = request.RequestedFile;
            Debug.Print("Requested: " + requestedFile);
            //Check for index
            Match needsIndex = Regex.Match(requestedFile, @"[0-9,A-z,/]*/$");
            if (needsIndex.Success)
                requestedFile += "index.htm";

            //Configure the server to handle commands.
            if (requestedFile == "/index.htm")
            {
                if (request["ir"] != null)
                {
                    Debug.Print(request["ir"]);
                    string input = request["ir"];
                    Match isInt = Regex.Match(input, @"^[0-9]*$");
                    if (isInt.Success)
                    {
                        int command = Convert.ToInt32(input);
                        bool current;
                        Debug.Print("IR command received: " + command);
                        if (command == 0)
                        {
                            if (_AC.isOn)
                                current = true;
                            else
                                current = false;
                            while (_AC.isOn == current)
                            {
                                _commander.sendCommand(0);
                                Thread.Sleep(250);
                            }
                            if (!current)
                            {
                                while (!_AC.timerOn)
                                {
                                    _commander.sendCommand(1);
                                    Thread.Sleep(250);
                                }
                            }
                            response.Write("AC power has been toggled", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                        }
                        else if (!_commander.hasCommand(command))
                        {
                            response.Write("Bad IR command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                        }
                        else
                        {
                            _commander.sendCommand(command);
                            response.Write("IR Command received", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                        }
                    }
                    else
                        response.Write("Bad IR command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure status response.
                else if (request["status"] == "get")
                {
                    if (_AC.isOn)
                        response.Write("on", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                    else
                        response.Write("off", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure the onTimer command.
                else if (request["on"] != null)
                {
                    string minutes = (request["on"]);
                    Match isInt = Regex.Match(minutes, @"^[0-9]*$");
                    if (isInt.Success)
                    {
                        _timers.onMinutes = Convert.ToInt32(minutes);
                        //Kill any previously running timers.
                        _timers.onAbort = true;
                        Thread.Sleep(1500);
                        var onThread = new Thread(_timers.onTimer);
                        onThread.Start();
                        response.Write("onTimer started with a time of " + minutes, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                    }
                    else
                        response.Write("Bad onTimer command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure the offTimer command.
                else if (request["off"] != null)
                {
                    string minutes = request["off"];
                    Match isInt = Regex.Match(minutes, @"^[0-9]*$");
                    if (isInt.Success)
                    {
                        _timers.offMinutes = Convert.ToInt32(minutes);
                        //Kill any previously running timers.
                        _timers.offAbort = true;
                        Thread.Sleep(1500);
                        var offThread = new Thread(_timers.offTimer);
                        offThread.Start();
                        response.Write("offTimer started with a time of " + minutes, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                    }
                    else
                        response.Write("Bad offTimer command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure the offTemp command.
                else if (request["offT"] != null)
                {
                    string temp = request["offT"];
                    Match isInt = Regex.Match(temp, @"^[0-9]{1,3}([.][0-9]{1,2})?$");
                    if (isInt.Success)
                    {
                        _timers.lowTemp = Double.Parse(temp);
                        response.Write("Low temp set to " + temp, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                    }
                    else
                        response.Write("Bad low temp command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure the onTemp command.
                else if (request["onT"] != null)
                {
                    string temp = request["onT"];
                    var isInt = Regex.Match(temp, @"^[0-9]{1,3}([.][0-9]{1,2})?$");
                    if (isInt.Success)
                    {
                        _timers.highTemp = Double.Parse(temp);
                        response.Write("High temp set to " + temp, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                    }
                    else
                        response.Write("Bad high temp command", response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure getTemp command
                else if (request["getTemp"] == "get")
                {
                    _sensor.takeMeasurement();
                    //Send a response as a JSON object.
                    string JSON = "{\"temp\": \"" + _sensor.tempF.ToString("f") + "\", \"hum\": \"" + _sensor.hum.ToString("f") + "\", \"index\": \"" + _sensor.heatIndexF.ToString("f") + "\"}";
                    Debug.Print("Temperature sent: " + JSON);
                    response.Write(JSON, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                //Configure getSettings command
                else if (request["getSettings"] == "get")
                {
                    string JSON = "{\"high\": \"" + _timers.highTemp.ToString() + "\", \"low\": \"" + _timers.lowTemp.ToString() + "\"}";
                    Debug.Print("Settings sent: " + JSON);
                    response.Write(JSON, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
                }
                else
                {
                    SendFile(response, "/index.htm");
                }
            }
            //Configure password manager
            else if (requestedFile == "/pass.htm")
            {
                if (request["setPass"] != null)
                {
                    string pass = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(request["setPass"]));
                    if(!_credentials.Keys.Contains(pass))
                        _credentials.Keys.Add(pass);
                }
                else if (request["delPass"] != null && _credentials.Keys.Count > 1)
                {
                    string pass = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(request["delPass"]));
                    if (_credentials.Keys.Contains(pass))
                        _credentials.Keys.Remove(Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(request["delPass"])));
                }
                SendFile(response, "/pass.htm", false);
            }
            //Handle auth page temp requests
            else if (request["getTemp"] == "get" && requestedFile == "/auth.htm")
            {
                _sensor.takeMeasurement();
                //Send a response as a JSON object.
                string JSON = "{\"temp\": \"" + _sensor.tempF.ToString("f") + "\", \"hum\": \"" + _sensor.hum.ToString("f") + "\", \"index\": \"" + _sensor.heatIndexF.ToString("f") + "\"}";
                Debug.Print("Temperature sent: " + JSON);
                response.Write(JSON, response.mimeType[(int)HttpResponse.FileType.TXT], cache: false);
            }
            //No command recieved, send file.
            else
            {
                Debug.Print("Sending " + requestedFile);
                SendFile(response, requestedFile);
            }
        }

        //Look for a file on the SD card and send it back if it exists.
        private void SendFile(HttpResponse response, String file, bool cache = true)
        {
            //Replace / with \
            string[] part = file.Split('/');
            string filePath = _webfolder;
            for (int i = 1; i < part.Length; i++)
            {
                filePath += "\\" + part[i];
            }
            if (File.Exists(filePath))
                response.WriteFile(filePath, cache: cache);
            else
                response.WriteNotFound();
        }
    }
}
