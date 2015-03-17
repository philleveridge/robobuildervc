# Introduction #

The visual client application works as a complete UI front end to the robobuilderlib software (or using PC direct mode) to the default software (firmware 2.23 and above - some features don't work on earlier versions).

The visual client provides
  * motion capture and editing software,
  * user defined motion control (using L# embedded),
  * a 3D visualisation using Physx and
  * web cam interaction with object detection (based n colour filters)

This code requires **DirectX End-User Runtime** - get it from here : http://www.microsoft.com/downloads/en/resultsForCategory.aspx?displaylang=en&categoryid=2

The code also uses Physx Novodex wrapper for .NET and uses LSharp.NET dll for its scripting language. These libraries have all been packaged into zip - see download area.

_This code is still pre-alpha and not available as a pre-built package_

## Screen images from the Visual Client ##

| **screen Images** | **Description** |
|:------------------|:----------------|
| ![http://robobuildervc.googlecode.com/files/slide0002_image008.jpg](http://robobuildervc.googlecode.com/files/slide0002_image008.jpg) ![http://robobuildervc.googlecode.com/files/slide0002_image006.jpg](http://robobuildervc.googlecode.com/files/slide0002_image006.jpg) | Initial screen (before and after connecting) - PC remote mode - The Basic menu item is auto detected based on firmware S/N |
| ![http://robobuildervc.googlecode.com/files/slide0002_image010.jpg](http://robobuildervc.googlecode.com/files/slide0002_image010.jpg) | Motion editor - load, edit, save motions. Show servo status. Allows for auto capture of both position and accelerometer values |
| ![http://robobuildervc.googlecode.com/files/slide0002_image012.jpg](http://robobuildervc.googlecode.com/files/slide0002_image012.jpg) | Preset motion play mode - with either simple scripting or now L#/Lsharp.Net |
| ![http://robobuildervc.googlecode.com/files/video_frm.jpg](http://robobuildervc.googlecode.com/files/video_frm.jpg) | Video form - object detection using Aforge library. Can be linked to to remote scripting |
| ![http://robobuildervc.googlecode.com/files/slide0002_image002.jpg](http://robobuildervc.googlecode.com/files/slide0002_image002.jpg) | Physx simulation : This uses the Novodex .NET wrapper and Direct X in conjunction with Physx libraries |

  * Automatic links to other wiki pages