http://forums.netduino.com/index.php?/topic/12236-netduino-3-ethernet-firmware-v432-update-3/

Posted 18 September 2015 - 04:16 AM
Version: 4.3.2 Update 3 (version 4.3.2.3)

Download link
Download Netduino v4.3.2.3 firmware now

Updates in this release
1. DNS queries now fail over to secondary servers.
2. Empty list of DNS servers now work with MFDeploy.
3. DHCP maximum message length option (DHCP option 57) corrected.

Additional updates from recent releases
1. Pins.ONBOARD_BTN now disables the reset feature properly

Pre-requisites for this firmware
1. Visual Studio 2012 or Visual Studio 2013. Or Visual Studio 2015 (beta).
2. .NET Micro Framework SDK v4.3 QFE2 or newer
3. NETMF plug-in for Visual Studio 2012, NETMF plug-in for Visual Studio 2013 or NETMF plug-in for Visual Studio 2015 (beta).
4. Netduino SDK v4.3.2.1 or newer

How to flash this firmware
1. Detach your Netduino from your computer to turn it off.
2. Press and hold your Netduino's pushbutton while plugging it in via USB; this will put it in bootloader mode.
3. Run the Netduino Update tool (see link above).
a. If your device does not appear, install the STDFU drivers + tools v3.0.3.
b. If your device appears as "STM Device in DFU Mode", click on "Options", select your board type from the Product selection box and close the Options window.
4. Select the checkbox next to your device and press "Upgrade"
5. Wait while the upgrade operation completes. After flashing, your Netduino will reboot and will be removed from the upgrade list.
6. After flashing, set your network settings using MFDeploy. In MFDeploy, select the Target > Configuration > Networking menu. Re-enter your IP address and Wi-Fi settings.

How to find your current version of Netduino firmware
1. Go to the Start Menu > Programs > Microsoft .NET Micro Framework 4.3
2. Run MFDeploy.
3. Plug your Netduino into your PC using a Micro USB cable.
4. In the Device section at top, select USB instead of Serial. Your Netduino should appear in the drop-down; if not, select it.
5. Select the Target menu > Device Capabilities option.
6. In the output box, find the "SolutionReleaseInfo.solutionVersion" value. This is your firmware version.

Enjoy, and please let us know if you run into any troubles.

Chris

Download link
Download Netduino v4.3.2.3 firmware now 