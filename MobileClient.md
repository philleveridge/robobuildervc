# Introduction #

For those without the latest in phone technology there is hope!

![http://robobuildervc.googlecode.com/files/gc.jpg](http://robobuildervc.googlecode.com/files/gc.jpg)

The applications has three modes

  * Default mode - Works with standard firmware. Select either Connect or quit menu options. On connect it will display serial number and firmware version if successful and a graphic of a game controller appears. (see above image) Pressing numbers 0-9 will cause the robot to perform the appropriate motion/action.

|Button Mapping|Motion|
|:-------------|:-----|
|0 |Get Up A|
|1 |Turn Right |
|2 |Forward |
|3 |Attack Left |
|4 |Left |
|5 |Attack Right |
|6 |Right |
|7 |Turn Left |
|8 |Back |
|9 |stand|

  * Basic Mode - Press 'b' before connecting it switches to basic mode, and the controller simply send the corresponding IR character value directly to the robot. Again no use for standard firmware - but this can be used in custom application firmware (or basic programs)

  * Terminal Mode - Press 't' before connecting it will switch into Terminal mode - and then you can get a very basic VT100 terminal emulator so you can interact with your robot. This won't work with standard firmware - but is great if you're running Basic for instance.

## How it works ##

The code should work with any Windows Mobile 6 phone (or 6.5) but I have tested on my [S620 HTC "Smartphone"](http://www.htc.com/uk/product/s620/overview.html) circa 2007.

The [source](http://robobuildervc.googlecode.com/files/RomoMoboApps.zip) requires Visual Studio 2005 professional edition to build. [free express editions won't work as they don't support mobile devices (why MS why ?)](The.md)

The code relies on the phone OS to map the BlueTooth to a serial port when it connects and hence the code uses the RobobuilderLib library to read and write to COM port 6. The code uses PCRemote class re-built against Compact .Net V2 framework to send binary protocol to the robot.

When the application connects to the Robobuilder for the first time its asks for a PIN - after some digging I found its set to 1234. Worth knowing!


## Images ##

| Mode | Image |
|:-----|:------|
| Default/Basic Mode  | <img src='http://robobuildervc.googlecode.com/files/IMG_1388t.jpg' width='400'> <br>
<tr><td> Terminal Mode </td><td> <img src='http://robobuildervc.googlecode.com/files/IMG_1387t.jpg' width='400'> </td></tr>