# Introduction #

Simple interface to allow octave programs t talk directly to Robobuilder using serial connection. Assumes robobuilder has been loaded with DCMP firmware.


# Details #


Download robobuilder.oct and run octave from same directory:

```
octave -q
octave:1> robobuilder
octave:2> robobuilder.initserial(0)
DCMP v=3.12
octave:3> robobuilder.wckPosRead(10)
ans =  70
octave:4> robobuilder.readXYZ()
ans =

  -16   -2   62
octave:5> robobuilder.readservos(0)
ans =

 Columns 1 through 16:

  143  185  246   44  101  118   65    0  207  152   70   70  152  182  181   97

```

the first command robobuilder instructs octave to load the module. If its not in the current directory you will need to set pwd. Before any commands are issued first initialise the serial link. The defaults are /dev/ttyUSB0 and 115200.


# Functions available #

|Function name|Inputs|Outputs|Description|
|:------------|:-----|:------|:----------|
|setdevname|string |  |Set serial port name  i.e. "/dev/ttyS0"|
|initserial|int f |  | initialise comms |
|wckPosRead|int ServoID |position 0-254 |Read servo position |
|wckPassive|int id|  |set servo passive mode |
|wckMovePos|int id, int pos, int torq); |  |Move servo to position |
|readPSD|	 |int|read distance sensor value 10-50  |
|readIR|	 |int|read current value on IR |
|standup |	int n |  |stand up motion n normally 16 |
|PlayMotion|	int n |  |pla built in motion 1-19 |
|wckWriteIO|	unsigned char ServoID, unsigned char IO); |  |set io on servo (lights etc) |
|blights|	(lights\_t l) |  |barmeter function |
|readXYZ|  |1x3 matrix returned |read accelerometer|
|SyncPosSend|	int SpeedLevel,Matrix(1:x) |  |Move each servo to position defined in matrix |
|PlayPoseA|	int d, int f, int tq, int flag, matrix(1:x) |  |  |
|readservos|int n|matrix (1,n) |read n servo positions|