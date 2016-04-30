# HastPiControl
[![Build status](https://ci.appveyor.com/api/projects/status/rubndn65rfaaq52j?svg=true)](https://ci.appveyor.com/project/JonBenson/hastpicontrol)

My first Windows 10 IoT project using a Raspberry Pi 2 and PiFace Digital 2 to control my garage door via AllJoyn and/or AutoRemote.

For more details you can read [this article on hackster.io](https://goo.gl/wBlbZu)

![QR Code for article](http://chart.apis.google.com/chart?cht=qr&chs=120x120&choe=UTF-8&chld=H|0&chl=https://goo.gl/wBlbZu)


As part of this project I've setup a [NuGet package](https://www.nuget.org/packages/Hastarin.Devices.MCP23S17) for control of the PiFace Digital 2 based on work by [Peter Oakes](https://microsoft.hackster.io/en-US/peteroakes) on [microsoft.hackster.io](https://microsoft.hackster.io/). 

To add the reference via NuGet:  `Install-Package Hastarin.Devices.MCP23S17`
