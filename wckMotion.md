# Introduction #

RobobuilderLib.wckMotion class. Provides access through the RBC controller to the wckBus for control of the servos. Works with either standard firmware or DCMP.hex



## Examples ##

## Smooth move ##

To achieve smooth motion with the lib. When moving between two points you need to calculate the "inbetween positions" ot tweens to get that smooth motion. So rather than a servo going from position 20 -> 40 you need it to go to 20 -> 25 -> 30 ->35 -> 40. There is a command "PlayPose" in wckMotion class that will do that for you. i.e.

wckMotion.PlayPose(duration, no\_steps, position\_array, true);

```
            PCremote p = new PCremote("com1"); 
            wckMotion w = new wckMotion(p); 

            w.PlayPose(3000, 10, new byte[] {125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 51, 47, 49, 199, 205, 205 }, true);
```
Note:
Playpose also has variations Playfile (play moves from a csv file) and PlayMatrix that takes a matrix of moves. So you can store built in moves in a csv file and play them like stock motions - I do this in my android app.

Also the interpolation by default is linear or equal steps, but there is a variation where you can pass a type in that switches modes and allows for different types (Accel, De Accel or Accel then Deaccel). This is based on code published in the Robosavvy forum

## Reading a value ##
```
            if (w.wckReadPos(5)) 
            { 
                Console.WriteLine("Servo pos 5 = {0}", w.respnse[1]); 
            } 
            else 
            { 
                Console.WriteLine("Servo 5 NOT CONNECTED"); 
            } 
```

Note: if using the DCMP software you can retrieve sensors by reading the values for Servo 30 and adding the optional parameter, do for instance wckReadPos(30,0) will return DCMP version number in respnse[0,1] array.

See CSharp wiki for full example and code.

# Details #

## Constructor ##
```
RobobuilderLib.wckMotion.wckMotion(string, bool) 
RobobuilderLib.wckMotion.wckMotion(RobobuilderLib.PCremote)
RobobuilderLib.wckMotion.~wckMotion() 
```

## Methods ##
### low level access ###
```
RobobuilderLib.wckMotion.wckBreak() 
RobobuilderLib.wckMotion.wckMovePos(int, int, int) 
RobobuilderLib.wckMotion.wckPassive(int) 
RobobuilderLib.wckMotion.wckPosMove10Bit(int, int, int) 
RobobuilderLib.wckMotion.wckPosRead10Bit(int) 
RobobuilderLib.wckMotion.wckReadBoundary(int) 
RobobuilderLib.wckMotion.wckReadIgain(int) 
RobobuilderLib.wckMotion.wckReadIO(int) 
RobobuilderLib.wckMotion.wckReadMotionData(int) 
RobobuilderLib.wckMotion.wckReadOverload(int) 
RobobuilderLib.wckMotion.wckReadPDgain(int) 
RobobuilderLib.wckMotion.wckReadPos(int) 
RobobuilderLib.wckMotion.wckReadPos(int, int) 
RobobuilderLib.wckMotion.wckReadSpeed(int) 
RobobuilderLib.wckMotion.wckSetBaudRate(int, int) 
RobobuilderLib.wckMotion.wckSetBoundary(int, int, int) 
RobobuilderLib.wckMotion.wckSetID(int, int) 
RobobuilderLib.wckMotion.wckSetIgain(int, int) 
RobobuilderLib.wckMotion.wckSetIgainRT(int, int) 
RobobuilderLib.wckMotion.wckSetOper(byte, byte, byte, byte) 
RobobuilderLib.wckMotion.wckSetOverload(int, int) 
RobobuilderLib.wckMotion.wckSetPDgain(int, int, int) 
RobobuilderLib.wckMotion.wckSetPDgainRT(int, int, int) 
RobobuilderLib.wckMotion.wckSetSpeed(int, int, int) 
RobobuilderLib.wckMotion.wckSetSpeedRT(int, int, int) 
RobobuilderLib.wckMotion.wckWriteIO(int, bool, bool) 
RobobuilderLib.wckMotion.wckWriteMotionData(int, int, int) 
```
### high level access ###
```
RobobuilderLib.wckMotion.PlayFile(string) 
RobobuilderLib.wckMotion.PlayFile(string, int, int) 
RobobuilderLib.wckMotion.PlayMatrix(RobobuilderLib.matrix) 
RobobuilderLib.wckMotion.PlayMatrix(RobobuilderLib.matrix, int, int) 
RobobuilderLib.wckMotion.PlayPose(int, int, object[], bool) 
RobobuilderLib.wckMotion.PlayPose(int, int, byte[], bool) 
RobobuilderLib.wckMotion.SyncPosSend(int, int, object[], int)
RobobuilderLib.wckMotion.SyncPosSend(int, int, byte[], int) 
RobobuilderLib.wckMotion.BasicPose(int, int) 

RobobuilderLib.wckMotion.servoID_readservo(int) 
RobobuilderLib.wckMotion.servoStatus(int, bool) 
RobobuilderLib.wckMotion.set_kfactor(double) 
RobobuilderLib.wckMotion.set_trigger(RobobuilderLib.trigger) 
RobobuilderLib.wckMotion.reset_timer() 

RobobuilderLib.wckMotion.close() 
RobobuilderLib.wckMotion.delay_ms(int) 
```

## properties ##
```
RobobuilderLib.wckMotion.DCMP 
RobobuilderLib.wckMotion.basic_pos 
RobobuilderLib.wckMotion.basic16 
RobobuilderLib.wckMotion.basic18 
RobobuilderLib.wckMotion.initpos 
RobobuilderLib.wckMotion.kfactor 
RobobuilderLib.wckMotion.lb_Huno 
RobobuilderLib.wckMotion.ub_Huno 
RobobuilderLib.wckMotion.Message 
RobobuilderLib.wckMotion.pcR 
RobobuilderLib.wckMotion.pos 
RobobuilderLib.wckMotion.respnse 
RobobuilderLib.wckMotion.serialPort 
RobobuilderLib.wckMotion.sids 
RobobuilderLib.wckMotion.tcnt 
RobobuilderLib.wckMotion.trig 
RobobuilderLib.wckMotion.MAX_SERVOS
```