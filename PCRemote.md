# Introduction #
PCRemote class definition

# Details #
## example ##
```
PCremote r = new PCremote("COM1");
Console.WriteLine("PSD={0}", r.readPSD());
```

## constructor ##
```
RobobuilderLib.PCremote.PCremote(string) 
RobobuilderLib.PCremote.PCremote(System.IO.Ports.SerialPort) 
RobobuilderLib.PCremote.~PCremote() 
```

## methods ##
```

RobobuilderLib.PCremote.a() 
RobobuilderLib.PCremote.accelerometer() 
RobobuilderLib.PCremote.availMem() 
RobobuilderLib.PCremote.b() 
RobobuilderLib.PCremote.basic() 
RobobuilderLib.PCremote.Close() 
RobobuilderLib.PCremote.command_1B(byte, byte) 
RobobuilderLib.PCremote.command_nB(byte, byte, byte[]) 
RobobuilderLib.PCremote.displayResponse(bool) 
RobobuilderLib.PCremote.download_basic(string) 
RobobuilderLib.PCremote.executionStatus(int) 
RobobuilderLib.PCremote.readButton(int) 
RobobuilderLib.PCremote.readButton(int, RobobuilderLib.callBack) 
RobobuilderLib.PCremote.readDistance() 
RobobuilderLib.PCremote.readIR(int) 
RobobuilderLib.PCremote.readIR(int, RobobuilderLib.callBack)
RobobuilderLib.PCremote.readPSD() 
RobobuilderLib.PCremote.readSN() 
RobobuilderLib.PCremote.readsoundLevel(int, int)
RobobuilderLib.PCremote.readsoundLevel(int, int, RobobuilderLib.callBack)
RobobuilderLib.PCremote.readVer() 
RobobuilderLib.PCremote.readXYZ() 
RobobuilderLib.PCremote.readXYZ(out int, out int, out int) 
RobobuilderLib.PCremote.readZeros() 
RobobuilderLib.PCremote.resetMem() 
RobobuilderLib.PCremote.run(int) 
RobobuilderLib.PCremote.runMotion(int) 
RobobuilderLib.PCremote.runSound(int) 
RobobuilderLib.PCremote.serial_number() 
RobobuilderLib.PCremote.setdbg(bool) 
RobobuilderLib.PCremote.setDCmode(bool) 
RobobuilderLib.PCremote.setup(System.IO.Ports.SerialPort) 
RobobuilderLib.PCremote.zeroHuno()

```
## properties ##
```
RobobuilderLib.PCremote.dbg
RobobuilderLib.PCremote.DCmode 
RobobuilderLib.PCremote.header 
RobobuilderLib.PCremote.message 
RobobuilderLib.PCremote.respnse 
RobobuilderLib.PCremote.serialPort
```