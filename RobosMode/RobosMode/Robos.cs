using System;
using System.Text;
using System.IO.Ports;

namespace RobobuilderLib
{
    // remote connection to home brew OS

    public class PCremote
    {
        bool DCmode;
        public SerialPort serialPort1;
        public string message;

        public PCremote(SerialPort s)
        {
            serialPort1 = s;
            message = "";
            DCmode = false;
        }

        public string readVer()         { return "Robos"; }
        public string readSN()          { return "0000000000000";   }
        public string readDistance()    { return ""; }
        public string availMem()        { return ""; }
        public string resetMem()        { return ""; }
        public string readZeros()       { return ""; }
        public string zeroHuno()        { return ""; }
        public void setDCmode(bool f)   {DCmode = f;}

        public string readXYZ(out Int16 x, out Int16 y, out Int16 z)
        {
            x = 0; y = 0; z = 0; return "";
        }
    }

    public class wckMotion
    {
        private SerialPort serialPort1;
        PCremote pcR;

        public byte[] respnse = new byte[32];
        public string Message;
        public byte[] pos;

        public wckMotion(PCremote r)
        {
            serialPort1 = r.serialPort1;
            pcR = r;
            pcR.setDCmode(true);
        }
        ~wckMotion()
        {
            close();
        }
        public void close()
        {
            pcR.setDCmode(false);
        }
        public bool wckPassive(int id)                      { return false; }
        public bool wckReadPos(int id)                      { return false; }
        public bool wckMovePos(int id, int pos, int torq)   { return false; }
        public void SyncPosSend(int LastID, int SpeedLevel, byte[] TargetArray, int Index) { }
        public void servoID_readservo() { }

        public void PlayPose(int duration, int no_steps, byte[] spod, bool first) { }


    }
}
