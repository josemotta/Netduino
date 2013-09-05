http://forums.netduino.com/index.php?/topic/8116-netduino-plus-2-firmware-v422-update-2/

Posted 07 February 2013 - 03:07 PM 

Version: 4.2.2 Update 2 (version 4.2.2.2)

This firmware requires use of the .NET Micro Framework v4.2 SDK (QFE2) or newer and .Netduino 4.2 SDK or newer.

With this firmware, you will have the following resources available for your code:
384KB Flash
100KB+ RAM

This firmware includes the following updates:
1. .NET MF 4.2 QFE2 bugfixes

This firmware also includes the previous updates:
1. Bug fix: SPI clock 'idle high' setting now supported
2. Netduino Plus 1 projects can be upgraded without changing HardwareProvider
3. Now compatible with legacy SecretLabs AnalogInput and PWM classes
4. Bug fix: SPI chip select timing corrected
5. Bug fix: PWM frequency corrected
6. New: more reliable rebooting during deployment
7. Static IP now works (in addition to DHCP default)
8. MAC Addresses are now loaded by TinyCLR
9. Additional I2C bug fix--pins now forced into proper configuration

To find the current version of your Netduino firmware:
1. Go to the Start Menu > Programs > Microsoft .NET Micro Framework 4.2 > Tools
2. Run MFDeploy.exe. Be careful to run MFDeploy.exe and not MFDeploy.exe.config (as file extensions are hidden by default)
3. Plug your Netduino into your PC using a Micro USB cable.
4. In the Device section at top, select USB instead of Serial. Your Netduino should appear in the drop-down; if not, select it.
5. Select the Target menu, Device Capabilities option.
6. In the output box, find the "SolutionReleaseInfo.solutionVersion" value. This is your firmware version.

To flash this firmware:
1. Detach your Netduino
2. Press and hold your Netduino's pushbutton while plugging it in via USB; this will put it in bootloader mode.
3. Erase the firmware on your Netduino using the STDFU Tester v3.0.1 application
> a. Select the "Protocol" tab
> b. Press the "Create from Map" button
> c. Select the "Erase" radio button option
> d. Press the "Go" button
> e. Wait for erase process to complete
4. Flash the attached .DFU file using the ST DfuSe Demonstrator v3.0.2 application (included with STDFU Tester)
> a. Locate the "Upgrade or Verify Action" pane (bottom-right pane)
> b. Press "Choose..." and select the attached DFU file
> c. Check the "Verify after download" option
> d. Press "Upgrade". It will take a few minutes to update your Netduino.
> e. Detach and reattach your Netduino (power cycle) or press "Leave DFU mode"

After flashing, to set your network settings using MFDeploy:
1. Select the Target > Configuration > Networking menu. Re-enter your IP address settings and MAC address.

Enjoy, and please let us know if you run into any troubles.

Chris 

Attached Files
Attached File  NetduinoPlus2_Firmware_4.2.2.2.zip   269.3KB   1154 downloads 
