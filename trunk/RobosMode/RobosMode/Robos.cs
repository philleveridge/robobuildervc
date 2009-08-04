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

        int mode;

        public PCremote(SerialPort s)
        {
            serialPort1 = s;
            message = "";
            DCmode = false;

            if (s.IsOpen) readmode();
            //put into experimental mode

            if (mode == 1)
            {
                Console.WriteLine("PC connection - " + write2serial("p", true));
            }
        }

        public string readVer()         
        {
            string v="?";
            if (mode == 2)
            {
                v = write2serial("v", true);
            } 
            return v; 
        }

        public string readSN()          { return "0000000000000";   }
        public string readDistance()    { return ""; }
        public string availMem()        { return ""; }
        public string resetMem()        { return ""; }
        public string readZeros()       { return ""; }
        public string zeroHuno()        { return ""; }
        public void   setDCmode(bool f) {DCmode = f; }

        public string readXYZ(out Int16 x, out Int16 y, out Int16 z)
        {
            x = 0; y = 0; z = 0; return "";
        }

        public void readmode()
        {
            if (serialPort1.IsOpen)
            {
                string v = write2serial("?", true);
                if (v.StartsWith("Idle")) mode = 1;
                else if (v.StartsWith("?Exper")) mode = 2;
                else if (v.StartsWith("Seria")) mode = 3;
                else mode = 6;
            }
            else
            {
                mode = 0;
            }
        }

        private string expect_serial(string s, string e)
        {
            string r = "";

            if (serialPort1.IsOpen)
            {
                r = serialPort1.ReadExisting();
                serialPort1.ReadTimeout = 2000;
                serialPort1.Write(s);

                try
                {
                    r = serialPort1.ReadTo(e);
                }
                catch (Exception z)
                {
                }
            }
            Console.WriteLine("E=[" + r + e + "]"); //debug
            return r;
        }

        private string write2serial(string s, bool synch)
        {
            string r = "";

            Console.WriteLine(s + "(" + serialPort1.IsOpen + ")");
            if (serialPort1.IsOpen)
            {
                string t = serialPort1.ReadExisting();
                Console.WriteLine("Debug: " + t);

                serialPort1.Write(s);
                if (synch)
                {
                    //wait for a response
                    //generally this is the normal mode
                    serialPort1.ReadTimeout = 5000;
                    try
                    {
                        r = serialPort1.ReadLine();
                        if (r == "\r")
                            r = serialPort1.ReadLine();
                    }
                    catch (Exception z)
                    {
                    }
                    Console.WriteLine("W2=" + t + "[" + r + "]"); //debug
                }
            }
            return r;
        }

        public bool download_basic(string s)
        {
            string r = "";
            if (mode != 2) return false;
            r = write2serial("eC6",true);
            Console.WriteLine("Debug: - " + r);
            for (int i = 0; i < s.Length; i++)
            {
                serialPort1.Write(s[i].ToString());
                int c = serialPort1.ReadByte(); // echo
                if (c > 0) Console.Write((char)c);
            }
            
           Console.WriteLine("Debug: - " +serialPort1.ReadTo("Bytes"));
           return true;
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

        string ReadPos(uint ID)
        {
            string t;
            uint Data1 = 0; //arb
            ID = (5 << 5) | ( ID % 31);
            uint CheckSum = (ID ^ Data1) & 0x7f;
            t = "FF"
                + ID.ToString("X2")
                + Data1.ToString("X2")
                + CheckSum.ToString("X2");
            return t;
        }

        string MovePos(uint ID, uint pos, uint torq)
        {
            string t;
            ID = ((torq % 5) << 5) | (ID % 31);
            uint CheckSum = (ID ^ (pos % 254)) & 0x7f;
            t = "FF"
                + ID.ToString("X2")
                + pos.ToString("X2")
                + CheckSum.ToString("X2");
            return t;
        }

        string Passive(uint ID)
        {
            string t;
            ID = (6 << 5) | (ID % 31);
            uint Data1 = 0x10; //passive
            uint CheckSum = (ID ^ Data1 ) & 0x7f;
            t = "FF"
                + ID.ToString("X2")
                + Data1.ToString("X2")
                + CheckSum.ToString("X2");
            return t;
        }

        public void SyncPosSend(int LastID, int SpeedLevel, byte[] TargetArray, int Index) 
        {
            serialPort1.WriteLine("X" + SyncPosSend(20, TargetArray));
        }

        string SyncPosSend(int NUM_OF_WCKS, byte[] buffer)
        {
            byte t = (2 << 5) | 31;
            int c = 0;
            string outBuffer = "FF";
            outBuffer += t.ToString("X2");

            Console.Write(t + ","); //debug

            t = 16;
            outBuffer += t.ToString("X2");
            t = 0;

            for (int i = 0; i < NUM_OF_WCKS; i++)
            {
                int lt = buffer[i];
 
                if (lt > 254) lt = 254;
                if (lt < 1) lt = 1;
                outBuffer += lt.ToString("X2");
                Console.Write(lt + ","); //debug

                t = (byte)((uint)t ^ lt);
            }
            t &= 0x7f;
            outBuffer += t.ToString("X2");
            return outBuffer;
        }

        public bool wckPassive(int id)                      
        {
            try
            {
                serialPort1.WriteLine("x" + Passive((uint)id));
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                return true;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return false;
            }
        }

        public bool wckReadPos(int id) 
        {
            try
            {
                serialPort1.WriteLine("x" + ReadPos((uint)id));
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                return true;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return false;
            }
        }
        public bool wckMovePos(int id, int pos, int torq)
        {
            try
            {
                serialPort1.WriteLine("x" + MovePos((uint)id, (uint)pos, (uint)torq));
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                return true;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return false;
            }
        }


        public void servoID_readservo() { }

        public void PlayPose(int duration, int no_steps, byte[] spod, bool first) { }


    }
}
