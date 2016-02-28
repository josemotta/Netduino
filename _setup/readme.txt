2016.02.28 google search "netduino sdk legacy templates"

https://jeremylindsayni.wordpress.com/2015/01/29/windows-10-tech-preview-9926-and-netduino-plus-2/


Software Requirements

1. Windows 10 Technical Preview (9926)

partindo de uma instalacao Windows 10 para Blackberry PI 2 (separado)
com VS 2015 Community 
com aplicacao Blinky rodando ok

A INSTALAR A PARTIR DE AGORA:

2. Visual Studio 2015 Community Edition
http://forums.netduino.com/index.php?/topic/12067-vs2013-is-history-cant-get-vs2015-working/
artigo que informa que ja funciona bem com VS2015

3. .NET Microframework V4.3 SDK (QFE2-RTM)
https://netmf.codeplex.com/releases/view/133285

4. NETMF plug-in for VS2015:
http://forums.netduino.com/index.php?/topic/12067-vs2013-is-history-cant-get-vs2015-working/
pode usar "netmfvs14.vsix" dentro do folder do netmf-v4.3.2-SDK-R2-Beta

5. Netduino SDK 4.3.2.1
http://cdn.netduino.com/downloads/netduinosdk_NETMF43.exe

funcionou OK setup Windows 10 + VS2015 Community!!
OPS ALGUNS BUGS!!

BUG 1 ---------------------------------------------------------
http://forums.netduino.com/index.php?/topic/12096-vs2015-teething-troubles-and-some-solutions/
No.5 Not relevent but hard to track down. If you get the error 0x80131700 MMP it is because you don't have the 3.5 version of the .net framework installed. In windows 10 goto turn windows features on or off and select .Net Framework  3.5( includes .NET 2.0 and 3.0)

fica abaixo opcao para VS2013

2.b Visual Studio 2013 Community Edition
https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx
Visual Studio Community 2013 with Update 5

4.b .NET MF Visual Studio 2013 Add-in
for VS2013:
https://netmf.codeplex.com/downloads/get/1423120