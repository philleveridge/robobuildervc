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

        public int mode;
        public RobobuilderLib.binxfer btf;

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
                mode = 2;
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

        public void   setDCmode(bool f) 
        {
            if (DCmode == f) return;

            if (f)
            {
                btf = new binxfer(serialPort1);
                Console.WriteLine("Enter Bin mode");

                if (mode == 2)
                {
                    serialPort1.Write("#"); // enter binary mode
                    btf.send_msg_basic('v');
                    if (btf.recv_packet() && btf.buff[0] == 17) // check version
                    {
                        Console.WriteLine("Good packet ver = " + btf.buff[0].ToString());
                        mode = 6;
                    }
                    else
                    {
                        Console.WriteLine("Error in bin mode xfer");
                        return;
                    }
                }
            }
            else
            {

                if (btf != null) btf.send_msg_basic('p'); // exit bimary mode (no response required)
                Console.WriteLine("Exit Bin mode");
                btf = null;
                mode = 2;
            }
            DCmode = f;
        }

        public string readXYZ(out Int16 x, out Int16 y, out Int16 z)
        {
            x = 0; y = 0; z = 0; return "";
        }

        public void readmode()
        {
            if (serialPort1.IsOpen)
            {
                if (mode != 6)
                {
                    string v = write2serial("?", true);
                    if (v.StartsWith("Idle")) mode = 1;
                    else if (v.StartsWith("?Exper")) mode = 2;
                    else if (v.StartsWith("Seria")) mode = 3;
                    else mode = 6;
                }
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

        public string run_basic()
        {
            string r = "";
            if (mode != 2) return "Err - invalid mode";
            write2serial("eC1", false);
            r = serialPort1.ReadExisting();
            Console.WriteLine("Debug: - " + r);
            return r;
        }
    }

    public class wckMotion
    {
        private SerialPort serialPort1;
        PCremote pcR;

        byte[] basic_pos = new byte[] {
                /*0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 */
                143,179,198,83,106,106,69,48,167,141,47,47,49,199,204,204,122,125,127 };

        public byte[] respnse = new byte[32];
        public string Message;
        public byte[] pos;
        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

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
            string sps = SyncPosSend(19, TargetArray);
            pcR.btf.send_msg_raw('x', sps); // read status servo 'id'
            if (!pcR.btf.recv_packet())
            {
                Console.WriteLine("Synch pos send failed");
            }
       }

        string SyncPosSend(int NUM_OF_WCKS, byte[] buffer)
        {
            byte t = (2 << 5) | 31;
            int c = 0;
            string outBuffer = "FF";
            outBuffer += t.ToString("X2");

            Console.Write(t + ","); //debug

            t = (byte) (NUM_OF_WCKS & 0x1F);
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
                pcR.btf.send_msg_raw('X', Passive((uint)id)); // read status servo 'id'
                if (pcR.btf.recv_packet())
                {
                    respnse[0] = pcR.btf.buff[0];
                    respnse[1] = pcR.btf.buff[1];
                    return true;
                }
                return false;
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
                pcR.btf.send_msg_raw('X', ReadPos((uint)id)); // read status servo 'id'
                if (pcR.btf.recv_packet())
                {
                    respnse[0] = pcR.btf.buff[0];
                    respnse[1] = pcR.btf.buff[1];
                    return true;
                }
                return false;
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
                pcR.btf.send_msg_raw('X', MovePos((uint)id, (uint)pos, (uint)torq)); // read status servo 'id'
                if (pcR.btf.recv_packet())
                {
                    respnse[0] = pcR.btf.buff[0];
                    respnse[1] = pcR.btf.buff[1];
                    return true;
                }
                return false;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return false;
            }
        }

        public void servoID_readservo() 
        {
            pcR.btf.send_msg_basic('q'); // query all servo values
            if (pcR.btf.recv_packet())
            {
                pos = new byte[sids.Length];

                for (int id = 0; id < sids.Length; id++)
                {
                    if (respnse[1] < 255)
                    {
                        pos[id] = pcR.btf.buff[id*2];
                    }
                } 
            }
            else
            {
                Message = "servoID_readservo failed";
            } 
        }

        private void delay_ms(int t1)
        {
            System.Threading.Thread.Sleep(t1);
        }

        public void BasicPose(int duration, int no_steps)
        {
            PlayPose(duration, no_steps, basic_pos, true);
        }

        public void PlayPose(int duration, int no_steps, byte[] spod, bool first) 
        {
            byte[] temp = new byte[19]; // numbr of servos

            if (first) servoID_readservo(); // read start positons

            double[] intervals = new double[spod.Length];

            for (int n = 0; n < sids.Length; n++)
            {
                intervals[n] = (double)(spod[n] - pos[n]) / no_steps;
            }

            for (int s = 1; s <= no_steps; s++)
            {
                //
                for (int n = 0; n < 19; n++) // !!!!!!! only first 19 values are releveant - need to get this dynamially
                {
                    temp[n] = (byte)(pos[n] + (double)s * intervals[n]);
                }

                SyncPosSend(pos.Length - 1, 4, temp, 0);

                int td = duration / no_steps;
                if (td<25) td=25;

                delay_ms(td);
            }

            for (int n = 0; n < sids.Length; n++)
            {
                pos[n] = spod[n];
            }
        }

    }
}
