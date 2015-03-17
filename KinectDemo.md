# Introduction #

This is based on Robosavvy  routine used in their impressive Kinect demo. For more details on algorithm:

http://robosavvy.com/forum/viewtopic.php?t=7854&postdays=0&postorder=asc&start=0

This has now been updatd to cover Robosavvy v2 code:

http://robosavvy.com/forum/viewtopic.php?t=8026

C# Source available : http://code.google.com/p/robobuildervc/source/browse/trunk/RoboKinnect

# Details #

The details below are a little out of date - the code is now encapsulated with in RoboKinnectAPI class and includes both Simple (V1) algorithm and the more advanced algorithm

This is based on a Kinect ! ( Xbox 360 Version) and modified the "Shape Game" demo from the Kinect SDK with C#

In the "using" section add the following (as well as in Reference section)

```
using RobobuilderLib; 
```


I also added at the top of the code in the "private state" area

```
wckMotion rbWCK = null; 
PCremote rbPCR = null; 
Boolean bRBConnected = false; 

//Define maximum thresholds for relating Kinect to Shoulder 
Single sMaxKinectHandY = (Single)0.25; 
Byte bMaxShoulder = 185; 
Single sMinKinectHandY  = (Single)0.7; 
Byte bMinShoulder  = 47; 
```

Code to update robot arm positions

```
void UpdateRobobuilderArms(Byte bServo11, Byte bservo14) 
{ 
    //left arm 
    Byte bServo10=127, bServo12=127; 
    try 
    { 
       double dServo11  = Convert.ToDouble(bServo11); 
       //              5E-05x3 - 0.0213x2 + 2.9618x - 46.011 
       bServo10 = Convert.ToByte(0.00005*Math.Pow(dServo11,3)-0.0213*Math.Pow(dServo11,2)+2.9618*dServo11-46.011); 

       //             8E-05x3 - 0.0339x2 + 4.7877x - 108.15 
       bServo12 = Convert.ToByte(0.00008 * Math.Pow(dServo11,3)-0.0339*Math.Pow(dServo11,2)+4.7877*dServo11-108.15); 
     } 
     catch (Exception) 
     { 
     } 
     // right arm 
     Byte  bServo13=127 , bServo15=127; 
     try 
     { 
        double dServo14  = Convert.ToDouble(bservo14);
        //               5E-05x3 - 0.0176x2 + 2.0147x + 87.278 
        bServo13 = Convert.ToByte(0.00005*Math.Pow(dServo14,3)-0.0176*Math.Pow(dServo14,2)+2.0147*dServo14+87.27); 
        //               8E-05x3 - 0.0272x2 + 3.0598x + 24.756 
        bServo15 = Convert.ToByte(0.00008*Math.Pow(dServo14,3)-0.0272*Math.Pow(dServo14,2)+3.0598*dServo14+24.756); 

     } 
     catch (Exception) 
     { 
     } 
     SendArmPosition(bServo10, bServo11, bServo12, bServo13, bservo14, bServo15); 
} 
```

Code to send new positions to robot using Robobuilderlib

```
void SendArmPosition(Byte bServo10, Byte bServo11, Byte bServo12, Byte bServo13, Byte bServo14, Byte bServo15 ) 
{ 
      try { 
           if (!bRBConnected) 
                     return; 

           rbWCK.wckMovePos(10, bServo10, 1); // 3rd parameter is Torque 0~4: 0=maximum 
           rbWCK.wckMovePos(11, bServo11, 1); 
           rbWCK.wckMovePos(12, bServo12, 1); 
           rbWCK.wckMovePos(13, bServo13, 1); 
           rbWCK.wckMovePos(14, bServo14, 1); 
           rbWCK.wckMovePos(15, bServo15, 1); 
       } 
       catch(Exception e) 
       { 
           MessageBox.Show("Unable to Update Servo Position " + e.Message); 
       } 
} 
```

Scale the arms position to a servo ange

```
int ScaleTo(double val, int imin, int imax, double dmin, double dmax) 
{ 
     int r; 
     if (val <dmin> dmax) val = dmax; 
     val = (val - dmin) / (dmax - dmin); 
     r=  (int)((double)(imax - imin) * val) + imin; 
     return r; 
} 
```

Update code (this is called from SkeletonReady when the Player joint positions have been updated.

```
void updaterobot(Microsoft.Kinect.Skeleton playerSkeleton) 
{ 
     //Figure out values for the shoulder based on Hand position. 
     //Kinect returns positive values if the hand is above 
     //the head and negative values if it's bellow the head 
     int ServoRange  = bMaxShoulder - bMinShoulder; 

     //Left Arm  
     int sla = ScaleTo(playerSkeleton.Joints[JointType.HandRight].Position.Y, 0, ServoRange, 0.0, 0.3); 
     Byte bServo11 = Convert.ToByte(255 - (ServoRange - sla + bMinShoulder)); 

     // Right Arm 
     int sra = ScaleTo(playerSkeleton.Joints[JointType.HandLeft].Position.Y, 0, ServoRange, 0.0, 0.3); 
     Byte bServo14 = Convert.ToByte(255-(255 - (ServoRange - sra + bMinShoulder))); 

    UpdateRobobuilderArms(bServo11, bServo14); 
} 
```

There is a COM port problem bug caused by PCRemote trying to close the serial port with a null descriptor - so this code uses the old style initialization, passing in the Serial port - and hence it can't be null.

It will work with either Standard firmware or DCMP firmware - it test servo 30 and if a response is returned it assumes it must be using DCMP firmware.

```
void ConnectToRB(string sCOMMNum) 
{ 
	try { 
		SerialPort s = new SerialPort(sCOMMNum,115200); 
		s.Open(); 
		rbPCR = new RobobuilderLib.PCremote(s); 
		rbWCK = new RobobuilderLib.wckMotion(rbPCR); 

		if (rbWCK.wckReadPos(30)) 
		{ 
			//DCMP is true 
			rbWCK.DCMP = true; 
			rbWCK.PlayPose(1000, 10, wckMotion.basic16, true); 
		} 
		else 
		{ 
			//DCMP false 
			rbPCR.runMotion(7); // basic posture 
			Thread.Sleep(3000); 
			rbWCK.DCMP = false; 
			rbPCR.setDCmode(true); 
		} 
		bRBConnected = true; 
	} 
	catch(Exception e) 
	{ 
		MessageBox.Show("Unable to Connect to Robobuilder\n\n" + e.Message); 
		rbPCR = null; 
		bRBConnected = false; 
	} 
} 
```

This disconnects the serial port from Robot. Notice use of close() to ensure proper exit. This forces a DCmode off when using standard firmware for instance and ensure serial port is closed.
```
void DisconnectFromRB() 
{ 
      if (rbWCK != null) 
         rbWCK.close(); 

      if (rbPCR != null) 
         rbPCR.Close(); 
      rbWCK = null; 
      rbPCR = null; 
      bRBConnected = false; 
} 
```

The RBConnect/Disconnect code is added into the speech recognizer - on the word "Reset". That's how I control the Disconnect remotely at the end.

```
void RecognizerSaidSomething(object sender, SpeechRecognizer.SaidSomethingEventArgs e) 
{ 
	string txt = e.Matched; 
	switch (e.Verb) 
	{ 
		case SpeechRecognizer.Verbs.Reset: 
			if (bRBConnected) 
			{ 
				txt = "Disconnect"; 
				DisconnectFromRB(); 
			} 
			else 
			{ 
				txt = "Connecting ..."; 
				ConnectToRB("COM4"); 
			} 
			break; 
	} 
	FlyingText.NewFlyingText(this.screenRect.Width / 30, \
	  new Point(this.screenRect.Width / 2, this.screenRect.Height / 2), txt); 
}
```

Add into SkeletonsReady the line to actually make robot move. (But only if you're connected.
```
private void SkeletonsReady(object sender, SkeletonFrameReadyEventArgs e) 
{
     ....
     if (bRBConnected) 
           updaterobot(skeleton); 
     ''''
}
```