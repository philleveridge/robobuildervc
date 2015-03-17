# RobobuilderLib Remote Client #

This project is to create .NET based client software to enable the Robobuilder humanoid robot to be controlled using a PC.

---

If you like this site and FREE open source code it supplies please make a donation

[![](https://www.paypal.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=phil%2eleveridge%40btinternet%2ecom&lc=GB&item_name=Ipswich%20Robotics&item_number=donation&amount=5%2e00&currency_code=GBP&currency_code=GBP&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted) **Thank You for your support**

---


**NEW - see octave plugin**



## Robobuilder Firmware API (v1.9.9.7) ##

This project [RobobuilderLib.DLL](http://robobuildervc.googlecode.com/files/RobobuilderLib.dll) works either :
  * using Robobuilder Standard Firmware (2.23 or above - see http://Robobuilder.net/eng)
  * using DCMP (Open Source Robobuilder Firmware Project)  - see http://code.google.com/p/robobuilderlib/)

For details of using the API from with in your C# programs - see wiki
[CSharp](http://code.google.com/p/robobuildervc/wiki/CSharp)

<a href='http://www.youtube.com/watch?feature=player_embedded&v=jrFZDG9R-E0' target='_blank'><img src='http://img.youtube.com/vi/jrFZDG9R-E0/0.jpg' width='425' height=344 /></a>

Demo using Microsoft Kinect and modified "ShapeGame" adding C# [KinectDemo](KinectDemo.md) for details

## Lisp client ##

This is a set of example lisp programs to show how access can be simplified through a number of Lisp based functions. There is an online PDF tutorial to explain how to use LISP/L# to interface [PDF file](http://robobuildervc.googlecode.com/files/PDFOnline.pdf)

<a href='http://www.youtube.com/watch?feature=player_embedded&v=k8dz1tX9O48' target='_blank'><img src='http://img.youtube.com/vi/k8dz1tX9O48/0.jpg' width='425' height=344 /></a>

Continuous walking whilst reading position sensor by robosavvy pepep

More details: LispClient

## Visual Client ##

The visual client is a .NET applications using RobobuilderLib that provides
  * motion capture and editing software,
  * user defined motion control (using L# embedded),
  * a 3D visualisation using Physx and
  * web cam interaction with object detection (based n colour filters)

More details: VisualClient

## Mobile Client ##
[Download](http://robobuildervc.googlecode.com/files/RoboMoboApps.exe) onto your Smartphone (Win Mob 6.0 or better) and place in a folder - then use file explorer to run it. Make sure you have bluetooth on and connected to COM port 6

### Features: ###
  * Default mode  - Works with standard firmware.
  * Basic mode    - For own apps running on custom firmware i.e. BASIC progs
  * Terminal mode - simple vt100 terminal

More details: MobileClient

## Octave Plugin ##
A plugin for GnuOctave v3.2.3 to enable connection robobuilder using DCMP firmware
More details: OctaveModule

## Android Client ##
A java port of RobobuilderLib
version now available  - Build [r267](https://code.google.com/p/robobuildervc/source/detail?r=267)

<a href='http://www.youtube.com/watch?feature=player_embedded&v=wHr9CdtxnGM' target='_blank'><img src='http://img.youtube.com/vi/wHr9CdtxnGM/0.jpg' width='425' height=344 /></a>

### Features: ###
  * Basic mode    - Provides a terminal session with BASIC firmware
  * Firmware mode - Enables standard motions to be run through simple UI (or virtual joystick)
  * DCMP mode     - Connects to DCMP firmware and simple UI to pick motions from a list

More details: AndroidClient