﻿#define HOMEBREW

using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

namespace RobobuilderLib
{
    // remote connection to home brew OS

    public delegate void callBack(int n);

    public class PCremote
    {
        public enum RemoCon
        {
            FAILED = 0,
            A = 0x01, B, LeftTurn, Forward, RightTurn, Left, Stop, Right, Punch_Left, Back, Punch_Right,
            N1, N2, N3, N4, N5, N6, N7, N8, N9, N0,

            S_A = 0x16, S_B, S_LeftTurn, S_Forward, S_RightTurn, S_Left, S_Stop, S_Right, S_Punch_Left, S_Back, S_Punch_Right,
            S_N1, S_N2, S_N3, S_N4, S_N5, S_N6, S_N7, S_N8, S_N9, S_N0,

            H_A = 0x2B, H_B, H_LeftTurn, H_Forward, H_RightTurn, H_Left, H_Stop, H_Right, H_Punch_Left, H_Back, H_Punch_Right,
            H_N1, H_N2, H_N3, H_N4, H_N5, H_N6, H_N7, H_N8, H_N9, H_N0
        };

        bool DCmode;
        public SerialPort serialPort;
        public string message;

        public int mode;
        public RobobuilderLib.binxfer btf;

        public bool dbg = false;

        public PCremote(SerialPort s)
        {
            serialPort = s;
            message = "";
            dbg = true; 
            connect();
        }

        public PCremote(string sp)
        {
            serialPort = new SerialPort(sp, 115200);
            message = "";
            serialPort.Open();
            dbg = true;
            connect();
        }

        public void setdbg(bool x)
        {
            dbg = x;
            btf.dbg = x;
        }

        public void connect()
        {
            if (serialPort.IsOpen)
            {
                //put into bin mode
                Console.WriteLine("PC connection - " + write2serial("b", true));
                btf = new binxfer(serialPort);
                btf.dbg = dbg;
                if (dbg) Console.WriteLine("Enter Bin mode");
            }
            else
            {
                Console.WriteLine("No connection !");
            }
        }

        public void close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public string readVer()         
        {
            string v;

            try
            {
                btf.send_msg_basic('v');
            }
            catch (Exception e1)
            {
                Console.WriteLine("comm failed" + e1.Message);
                return "";
            }
            finally
            {
                if (btf.recv_packet() && btf.buff[0] == 17) // check version
                {
                    if (dbg) Console.WriteLine("Good packet ver = " + btf.buff[0].ToString());
                    v = btf.buff[0].ToString();
                }
                else
                {
                    Console.WriteLine("Error in bin mode xfer");
                    v = "err";
                }
            }
            return v;
        }

        private bool quickread(out int psd, out int x, out int y, out int z)
        {
            psd = x = y = z = 0;
            try
            {
                btf.send_msg_basic('Q');
            }
            catch (Exception e1)
            {
                Console.WriteLine("comm failed" + e1.Message);
                return false;
            }

            if (btf.recv_packet())
            {
                psd = (int)(btf.buff[0]);
                x = (int)(btf.buff[1]);
                y = (int)(btf.buff[3]);
                z = (int)(btf.buff[5]);
                if (x > 127) x = x - 256;
                if (y > 127) y = y - 256;
                if (z > 127) z = z - 256;
                if (dbg) Console.WriteLine("qr={0} {1} {2} {3}", psd, x, y, z); 
                return true;
            }
            else
            {
                Console.WriteLine("Error in bin mode xfer");
            }
            return false;
        }

        public int readPSD()
        {
            int psd;
            try
            {
                btf.send_msg_basic('D');
            }
            catch (Exception e1)
            {
                Console.WriteLine("comm failed" + e1.Message);
                return 0;
            }

            if (btf.recv_packet())
                psd = (int)(btf.buff[0]);
            else
                psd = 0;
            return psd;
        }

        public string readDistance() 
        {
            return readPSD().ToString();
        }

        public int[] readXYZ()
        {
            Int16 x, y, z;
            readXYZ(out x, out y, out z);
            return (new int[3] { (int)x, (int)y, (int)z });
        }

        public string readXYZ(out Int16 a, out Int16 b, out Int16 c)
        {
            int p, x, y, z;
            if (quickread(out p, out x, out y, out z))
            {
                a = (Int16)x; b = (Int16)y; c = (Int16)z;
                string s = String.Format("X={0}, Y={1}, Z={2}", x,y,z);
                if (dbg) Console.WriteLine(s);
                return s;
            }
            else
            {
                a = b = c = 0;
                return "X=0, Y=0, Z=0";
            }
        }

        public int readButton(int timeout, callBack x)
        {
            return 0;
        }

        public int readsoundLevel(int timeout, int level, callBack x)
        {
            return 0;
        }

        public RemoCon readIR(int timeout_ms, callBack x)
        {
            return 0;
        }

        public string readSN()          { return "HB0000101";   }

        public string availMem()        { return "NA"; }
        public string resetMem()        { return "NA"; }
        public string readZeros()       { return "NA"; }
        public string zeroHuno()        { return "NA"; }

        public void   setDCmode(bool f) 
        {
            if (DCmode == f) return;
        }

        private string expect_serial(string s, string e)
        {
            string r = "";

            if (serialPort.IsOpen)
            {
                r = serialPort.ReadExisting();
                serialPort.ReadTimeout = 2000;
                serialPort.Write(s);

                try
                {
                    r = serialPort.ReadTo(e);
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

            if (dbg) Console.WriteLine(s + "(" + serialPort.IsOpen + ")");
            if (serialPort.IsOpen)
            {
                string t = serialPort.ReadExisting();
                if (dbg) Console.WriteLine("Debug: " + t);

                serialPort.Write(s);
                if (synch)
                {
                    //wait for a response
                    //generally this is the normal mode
                    serialPort.ReadTimeout = 5000;
                    try
                    {
                        r = serialPort.ReadLine();
                        if (r == "\r")
                            r = serialPort.ReadLine();
                    }
                    catch (Exception z)
                    {
                    }
                    if (dbg) Console.WriteLine("W2=" + t + "[" + r + "]"); //debug
                }
            }
            return r;
        }

        public bool download_basic(string s)
        {
            try
            {
                btf.send_msg_raw('l', s);
            }
            catch (Exception e1)
            {
                Console.WriteLine("comm failed" + e1.Message);
                return false;
            }

            if (btf.recv_packet())
            {
                return true;
            }
            else
            {
                Console.WriteLine("download failed");
                return false;
            }
        }
    }

    public class wckMotion
    {
        private SerialPort serialPort;
        PCremote pcR;
        public const int MAX_SERVOS = 20;

        trigger trig;

        static public int[] ub_Huno = new int[] {
        /* ID
          0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
        174,228,254,130,185,254,180,126,208,208,254,224,198,254,228,254};

        static public int[] lb_Huno = new int[] {
        /* ID
          0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
          1, 70,124, 40, 41, 73, 22,  1,120, 57,  1, 23,  1,  1, 25, 40};


        static public byte[] basic_pos = new byte[] {
                /*0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 , 19 */
                143,179,198,83,106,106,69,48,167,141,47,47,49,199,204,204,122,125,127,127 };


        public byte[] respnse = new byte[32];
        public string Message;
        public byte[] pos;
        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        public double kfactor = 1.0f;
        int tcnt;

        public wckMotion(PCremote r)
        {
            serialPort = r.serialPort;
            pcR = r;
            pcR.setDCmode(true);
            trig = null;
        }
        ~wckMotion()
        {
            close();
        }
        public void close()
        {
            pcR.setDCmode(false);
        }

        public void set_kfactor(double k)
        {
            kfactor = k;
        }

        public void set_trigger(trigger t)
        {
            trig = t;
        }

        public void reset_timer()
        {
            tcnt = 0;
        }

        string ReadPos(uint ID)
        {
            string t;
            uint Data1 = 0; //arb
            ID = (5 << 5) | (ID % 31);
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
            uint CheckSum = (ID ^ Data1) & 0x7f;
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

            t = (byte)(NUM_OF_WCKS & 0x1F);
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


        public bool wckReadBoundary(int id) {return false;}
        public bool wckReadPDgain(int id) {return false;}
        public bool wckReadIgain(int id) {return false;}
        public bool wckReadSpeed(int id) {return false;}
        public bool wckReadOverload(int id) { return false; }

        /*********************************************************************************************
         * 
         * higher level functions
         * 
         *********************************************************************************************/

        bool initpos = false;

        public bool servoID_readservo(int num)
        {
            if (num == 0) num = sids.Length;

            if (pcR == null || pcR.btf == null)
                return false;

            pcR.btf.send_msg_basic('q'); // query all servo values
            if (pcR.btf.recv_packet())
            {

                if (pcR.dbg) Console.WriteLine("DBG: servo read {0} : {1}", pcR.btf.buff.Length, BitConverter.ToString(pcR.btf.buff));

                int nn = (pcR.btf.buff.Length - 11) / 2;

                if (num < nn)
                    pos = new byte[nn];
                else
                    pos = new byte[num];
               
                for (int id = 0; id < num; id++)
                {
                    if (respnse[1] < 255)
                    {
                        pos[id] = pcR.btf.buff[id * 2];
                    }
                }
                return true;
            }
            else
            {
                Message = "servoID_readservo failed";
                return false;
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

        public bool PlayFile(string filename)
        {
            byte[] servo_pos;
            int steps;
            int duration;
            int nos = 0;
            int n = 0;
            tcnt = 0;

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    //Console.WriteLine(line);

                    if (line.StartsWith("#")) // comment
                    {
                        Console.WriteLine(line);
                        if (line.StartsWith("#V=01,,"))
                            nos = 20;

                        Match m = Regex.Match(line, @"#V=01,N=([0-9]+)");
                        if (m.Success)
                        {
                            nos = Convert.ToInt32(m.Groups[0].Value);
                            Console.WriteLine("nos = {0}", nos);
                        }
                        continue;
                    }

                    string[] r = line.Split(',');

                    if (nos == 0)
                    {
                        if (r.Length > 20)
                            nos = r.Length - 5; // assume XYZ have been appended
                        else
                            nos = r.Length - 2;
                    }

                    if (nos > 0)
                    {
                        servo_pos = new byte[nos];
                        n++;

                        duration = Convert.ToInt32(r[0]);
                        steps = Convert.ToInt32(r[1]);

                        for (int i = 0; i < nos; i++)
                            servo_pos[i] = (byte)Convert.ToInt32(r[i + 2]);

                        if (!PlayPose(duration, steps, servo_pos, (n == 1)))
                            return false;
                    }

                }
                tr.Close();
            }
            catch (Exception e1)
            {
                Console.WriteLine("Error - can't load file " + e1.Message);
                return false;
            }
            return true;
        }


               // NEW:: if byte = 255 = use current positon
        // NEW:: check limits / bounds before sending

        public bool PlayPose(int duration, int no_steps, Object[] spodobj, bool first)
        {
            byte[] s = new byte[spodobj.Length];
            for (int i = 0; i < spodobj.Length; i++)
                s[i] = (byte)spodobj[i];
            return PlayPose(duration, no_steps, s, first);
        }

        public bool PlayPose(int duration, int no_steps, byte[] spod, bool first)
        {
            int cnt = 0;

            byte[] temp = new byte[spod.Length];

            if (first || !initpos)
            {
                if (trig != null && trig.dbg) Console.WriteLine("Debug:  read servo positions {0}", tcnt);

                servoID_readservo(spod.Length); // read start positons
                tcnt = 0;
            }

            double[] intervals = new double[spod.Length];

            duration = (int)(0.5+(double)duration * kfactor);

            if (kfactor != 1.0f) { Console.WriteLine("Kfactor set (0) = Duration= (1)", kfactor, duration); }

            // bounds check
            for (int n = 0; n < spod.Length ; n++)
            {
                if (spod[n] == 255)
                    intervals[n] = 0f;
                else
                {
                    if (n < lb_Huno.Length)
                    {
                        if (spod[n] < lb_Huno[n]) spod[n] = (byte)lb_Huno[n];
                        if (spod[n] > ub_Huno[n]) spod[n] = (byte)ub_Huno[n];
                    }
                    intervals[n] = (double)(spod[n] - pos[n]) / no_steps;
                    cnt++;
                }
            }

            //Console.WriteLine("Debug: diff = " + cnt);

            for (int s = 1; s <= no_steps; s++)
            {
                //
                for (int n = 0; n < spod.Length; n++) 
                {
                    temp[n] = (byte)(pos[n] + (double)s * intervals[n]);
                }

                SyncPosSend(temp.Length - 1, 4, temp, 0);

                int td = duration / no_steps;
                if (td<25) td=25;

                tcnt += td;


                if (trig != null && trig.active() && tcnt > trig.timer)
                {
                    tcnt =0;

                    DateTime n = DateTime.Now;

                    pcR.setDCmode(false);
                    if (trig.AccTrig)
                    {
                        int[] acc = pcR.readXYZ();

                        trig.Xval = acc[0];
                        trig.Yval = acc[1];
                        trig.Zval = acc[2];
                        if (trig.dbg) Console.WriteLine("Dbg: Trigger acc event {0} {1} {2} ", acc[0], acc[1], acc[2]);
                    }

                    if (trig.PSDTrig)
                    {
                        int psd = pcR.readPSD();
                        trig.Pval = psd;
                        if (trig.dbg) Console.WriteLine("Dbg: Trigger psd event {0} ", psd);
                    }

                    if (trig.SndTrig)
                    {
                        //todo
                    }

                    if (trig.IRTrig)
                    {
                        //todo
                    }

                    pcR.setDCmode(true);

                    if (trig.test())
                    {
                        if (trig.dbg)
                        {
                            Console.WriteLine("PlayPose halted due to trigger");
                            trig.print();
                        }
                        return false;
                    }

                    int te = (DateTime.Now - n).Milliseconds;

                    if (trig.dbg) Console.WriteLine("Elsapsed = " + te);    

                    if (te< td) 
                    {
                        td=td-te;  // subtract elsaped time
                        delay_ms(td);
                    }
                }
                else
                {
                    delay_ms(td);
                }
            }

            for (int n = 0; n < spod.Length; n++)
            {
                pos[n] = spod[n];
            }

            return true; // complete
        }

    }


    public class trigger
    {
        public int Xmax, Xmin, Xval;    //accelerometer X axis
        public int Ymax, Ymin, Yval;    //accelerometer Y axis
        public int Zmax, Zmin, Zval;    //accelerometer Z axis
        public int Pmax, Pmin, Pval;    //PSD sensor
        public int Smax, Smin, Sval;    //Sound Level
        public int Ival;                //IR remote

        public bool AccTrig = false;   // trigger on acceleromter val <min or >max
        public bool PSDTrig = false;   // trigger PSD val <min or >max
        public bool SndTrig = false;   // trigger Snd level val <min or >max
        public bool IRTrig = false;   // trigger IR being recieved
        public bool status = false;   // this must be true to activate

        public bool dbg { get; set; }  // this must be true for debug info
        public int timer { get; set; }  //trigger timer (in ms)

        public trigger()
        {
            timer = 250; //default value

            set_accel(0, 0, 0, 0, 0, 0);
            set_PSD(0, 0);
            set_SND(0, 0);
            set_IR(0);
            AccTrig = false;
            PSDTrig = false;
            SndTrig = false;
            IRTrig = false;
            dbg = false;
        }

        public bool test()
        {
            return
                (AccTrig == true && (Xval < Xmin || Yval < Ymin || Zval < Zmin || Xval > Xmax || Yval > Ymax || Zval > Zmax))
             || (PSDTrig == true && (Pval > Pmax || Pval < Pmin))
             || (IRTrig == true && Ival != 0)
             || (SndTrig == true && (Sval > Smax || Sval < Smin));
        }

        public void set_trigger(bool acc, bool psd, bool snd, bool ir)
        {
            AccTrig = acc;
            PSDTrig = psd;
            SndTrig = snd;
            IRTrig = ir;
        }

        public void set_accel(int minx, int miny, int minz, int maxx, int maxy, int maxz)
        {
            Xmax = maxx; Xmin = minx; Xval = 0;     //defaults : accelerometer X axis
            Ymax = maxy; Ymin = miny; Yval = 0;     //defaults : accelerometer Y axis
            Zmax = maxz; Zmin = minz; Zval = 0;     //defaults : accelerometer Z axis
            AccTrig = true;
        }

        public void set_PSD(int minp, int maxp)
        {
            Pmax = maxp; Pmin = minp; Pval = 0;
            PSDTrig = true;
        }

        public void set_SND(int mins, int maxs)
        {
            Smax = maxs; Smin = mins; Sval = 0;
            SndTrig = true;
        }

        public void set_IR(int ir)
        {
            Ival = 0;
            IRTrig = true;
        }

        public void set_trigger(int n)
        {
            AccTrig = ((n | 1) == 1);
            PSDTrig = ((n | 2) == 2);
            SndTrig = ((n | 4) == 4);
            IRTrig = ((n | 8) == 8);
        }

        public void activate(bool f)
        {
            status = f;
        }

        public bool active()
        {
            return status;
        }

        public void print()
        {
            try
            {
                Console.WriteLine("Trigger status : {0}", (status) ? "On" : "Off");
                Console.WriteLine("Timer          = {0 } ms", timer);
                Console.WriteLine("X={0:###} : {1:###} : {2:###} : {3}", Xmin, Xmax, Xval, ((AccTrig) ? "On" : "Off").ToString());
                Console.WriteLine("Y={0:###} : {1:###} : {2:###}", Ymin, Ymax, Yval);
                Console.WriteLine("Z={0:###} : {1:###} : {2:###}", Zmin, Zmax, Zval);
                Console.WriteLine("P={0:#}   : {1:#}   : {2:#}   : {3}", Pmin, Pmax, Pval, ((PSDTrig) ? "On" : "Off").ToString());
                Console.WriteLine("S={0:#}   : {1:#}   : {2:#}   : {3}", Smin, Smax, Sval, ((SndTrig) ? "On" : "Off").ToString());
                Console.WriteLine("I={0:#}   : {1}", Ival, ((IRTrig) ? "On" : "Off").ToString());
            }
            catch (Exception e1)
            {
                Console.WriteLine("exception - " + e1.Message);
            }
        }
    }
}
