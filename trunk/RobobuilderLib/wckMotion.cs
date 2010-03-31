using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;


namespace RobobuilderLib
{
    public class wckMotion
    {
        public const int MAX_SERVOS = 20;

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
      

        /**********************************************
         * 
         * direct Command mode  - wcK prorocol
         * 
         * ********************************************/

        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        private SerialPort serialPort1;
        PCremote   pcR;

        public byte[] respnse = new byte[32];
        public string Message;
        public byte[] pos;

        public wckMotion(PCremote r)
        {
            serialPort1 = r.serialPort1;
            pcR = r;
            pcR.setDCmode(true);
            delay_ms(100);
        }

        ~wckMotion()
        {
            close();
        }

        public void servoStatus(int id, bool f)
        {
            sids[id] = (f) ? id : -1;
        }

        public void close()
        {
           pcR.setDCmode(false);
        }

        public bool wckPassive(int id)
        {
            byte[] buff = new byte[4];
            buff[0] = 0xFF;
            buff[1] = (byte)(6 << 5 | (id % 31));
            buff[2] = 0x10; // Mode=1, arbitary
            buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 4);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                Message = "Passive " + id + " = " + respnse[0] + ":" + respnse[1];
                System.Diagnostics.Debug.WriteLine(Message); // debug
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
            byte[] buff = new byte[4];
            buff[0] = 0xFF;
            buff[1] = (byte)(5 << 5 | (id % 31));
            buff[2] = 0; // arbitary
            buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 4);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                Message = "ReadPos " + id + " = " + respnse[0] + ":" + respnse[1];
                System.Diagnostics.Debug.WriteLine(Message); // debug
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
            byte[] buff = new byte[4];
            buff[0] = 0xFF;
            buff[1] = (byte)(((torq % 5) << 5) | (id % 31));
            buff[2] = (byte)(pos % 254); // arbitary
            buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 4);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                Message = "MovePos " + id + " = " + respnse[0] + ":" + respnse[1];

                return true;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message ;
                return false;
            }
        }

        public void SyncPosSend(int LastID, int SpeedLevel, object[] TargetArray, int Index)
        {
            byte[] b = new byte[TargetArray.Length];
            for (int i = 0; i < TargetArray.Length; i++) 
                b[i] = Convert.ToByte(TargetArray[i]);
            SyncPosSend(LastID, SpeedLevel, b, Index);
        }

        public void SyncPosSend(int LastID, int SpeedLevel, byte[] TargetArray, int Index)
        {
            int i;
            byte CheckSum;
            byte[] buff = new byte[5 + LastID];

            i = 0;
            CheckSum = 0;
            buff[0] = 0xFF;
            buff[1] = (byte)((SpeedLevel << 5) | 0x1f);
            buff[2] = (byte)(LastID + 1);

            while (true)
            {
                if (i > LastID) break;
                buff[3 + i] = TargetArray[Index * (LastID + 1) + i];
                CheckSum ^= (byte)(TargetArray[Index * (LastID + 1) + i]);
                i++;
            }
            CheckSum = (byte)(CheckSum & 0x7f);
            buff[3 + i] = CheckSum;

            //now output buff[]
            for (i = 0; i < buff.Length - 1; i++) Console.Write(buff[i] + ","); Console.WriteLine(buff[i]);

            try
            {
                serialPort1.Write(buff, 0, buff.Length);
                Message = "MoveSyncPos";

                return;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return;
            }
        }

        public bool wckBreak()
        {
            byte[] buff = new byte[4];
            buff[0] = 0xFF;
            buff[1] = (byte)((6 << 5) | 31);
            buff[2] = (byte)0x20;
            buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 4);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                Message = "Break = " + respnse[0] + ":" + respnse[1];
                return true;
            }
            catch (Exception e1)
            {
                Message = "Break Failed" + e1.Message;
                return false;
            }
        }

        /* 
         * wck set operation(s)
         * 
         */ 
        public bool wckSetOper(byte d1,byte d2, byte d3, byte d4)
        {
            byte[] buff = new byte[6];
            buff[0] = 0xFF;
            buff[1] = d1;
            buff[2] = d2;
            buff[3] = d3;
            buff[4] = d4;
            buff[5] = (byte)((buff[1] ^ buff[2] ^ buff[3] ^ buff[4]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 6);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                Message = "Set Oper = " + respnse[0] + ":" + respnse[1];
                return true;
            }
            catch (Exception e1)
            {
                Message = "Set Op Failed" + e1.Message;
                return false;
            }
        }

        public bool wckSetBaudRate(int baudrate, int id)
        {
            byte d1, d3;
            d1=(byte)((7<<5) | (id %31));
            //0(921600bps), 1(460800bps), 3(230400bps), 7(115200bps),
            //15(57600bps), 23(38400bps), 95(9600bps), 191(4800bps),
            switch (baudrate)
            {
                case 115200:
                    d3 = 7;
                    break;
                case 57600:
                    d3 = 15;
                    break;
                case 9600:
                    d3 = 95;
                    break;
                case 4800:
                    d3 = 191;
                    break;
                default:
                    return false;
            }
            return wckSetOper(d1, 0x08, d3, d3);
        }

        public bool wckSetSpeed(int id, int speed, int acceleration)
        {
            byte d1, d3, d4;
            if (speed < 0 || speed > 30) 
                return false;
            if (acceleration < 20 || acceleration > 100) 
                return false;
            d1 = (byte)((7 << 5) | (id % 31));
            d3 = (byte)speed;
            d4 = (byte)acceleration;
            return wckSetOper(d1, 0x0D, d3, d4);
        }

        public bool wckSetPDgain(int id, int pGain, int dGain)
        {
            return false; // not implemented
        }

        public bool wckSetID(int id, int new_id)
        {
            return false; // not implemented
        }

        public bool wckSetIgain(int id, int iGain)
        {
            return false; // not implemented
        }

        public bool wckSetPDgainRT(int id, int pGain, int dGain)
        {
            return false; // not implemented
        }

        public bool wckSetIgainRT(int id, int iGain)
        {
            return false; // not implemented
        }

        public bool wckSetSpeedRT(int id, int speed, int acceleration)
        {
            return false; // not implemented
        }

        public bool wckSetOverload(int id, int overT)
        {
            /*
            1 33 400
            2 44 500
            3 56 600
            4 68 700
            5 80 800
            6 92 900
            7 104 1000
            8 116 1100
            9 128 1200
            10 199 1800
             */
            byte d1, d3=33;
            d1 = (byte)((7 << 5) | (id % 31));
            switch (overT)
            {
                case 400:
                    d3 = 33;
                    break;
                case 500:
                    d3 = 44;
                    break;
                case 600:
                    d3 = 56;
                    break;
                case 700:
                    d3 = 68;
                    break;
                case 800:
                    d3 = 80;
                    break;                 
            }
            return wckSetOper(d1, 0x0F, d3, d3);
        }

        public bool wckSetBoundary(int id, int UBound, int LBound)
        {
            byte d1, d3, d4;
            d1 = (byte)((7 << 5) | (id % 31));
            d3 = (byte)LBound;
            d4 = (byte)UBound;
            return wckSetOper(d1, 0x11, d3, d4);
        }

        public bool wckWriteIO(int id, bool ch0, bool ch1 )
        {
            byte d1, d3;
            d1 = (byte)((7 << 5) | (id % 31));
            d3 = (byte)((byte)((ch0) ? 1 : 0) | (byte)((ch1) ? 3 : 0));
            return wckSetOper(d1, 0x64, d3, d3);
        }

        /* 
         * wck - Read operation(s)
         */ 

        public bool wckReadPDgain(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x0A, 0x00, 0x00);
        }

        public bool wckReadIgain(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x16, 0x00, 0x00);
        }

        public bool wckReadSpeed(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x0E, 0x00, 0x00);
        }

        public bool wckReadOverload(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x10, 0x00, 0x00);
        }

        public bool wckReadBoundary(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x12, 0x00, 0x00);
        }

        public bool wckReadIO(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x65, 0x00, 0x00);
        }

        public bool wckReadMotionData(int id)
        {
            return wckSetOper((byte)((7 << 5) | (id % 31)), 0x97, 0x00, 0x00);
        }

        public bool wckPosRead10Bit(int id)
        {
            return wckSetOper((byte)(7 << 5), 0x09, (byte)id, (byte)id);
        }

        /* 
         * special extended / 10 bit commands
         */ 

        public bool wckWriteMotionData(int id, int pos, int torq)
        {
            return false; // not implemented
        }
        
        public bool wckPosMove10Bit(int id, int pos, int torq)
        {
            return false; // not implemented
        }


        /*********************************************************************************************
         * 
         * higher level functions
         * 
         *********************************************************************************************/

        bool initpos = false;

        public void servoID_readservo(int num)
        {
            if (num == 0) num = sids.Length;
            
            pos = new byte[num];

            for (int id = 0; id < num; id++)
            {
                int n = sids[id];

                if (n>=0 && wckReadPos(n))                 //readPOS (servoID)
                {
                    if (respnse[1] < 255)
                    {
                        pos[id] = respnse[1];
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Id " + id + "not connected" );
                    //sids[id] = -1; // not connected
                }
            }
            initpos = true;
        }

        private void delay_ms(int t1)
        {
            System.Threading.Thread.Sleep(t1);
        }

        public void BasicPose(int duration, int no_steps)
        {
            PlayPose(duration, no_steps, basic_pos, true);
        }

        public void PlayFile(string filename)
        {
            byte[] servo_pos;
            int steps;
            int duration;
            int nos = 20;
            int n = 0;

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("#")) // comment
                    {
                        Console.WriteLine(line);
                        Match m = Regex.Match(line, @"#V=01,N=([0-9]+)");
                        if (m.Success)
                        {
                            nos = Convert.ToInt32(m.Groups[0].Value);
                            Console.WriteLine("nos = {0}", nos);
                        }

                        continue;
                    }

                    string[] r = line.Split(',');

                    nos = r.Length - 2;

                    if (nos > 0)
                    {
                        servo_pos = new byte[nos];
                        n++;

                        duration = Convert.ToInt32(r[0]);
                        steps = Convert.ToInt32(r[1]);

                        for (int i = 0; i < 20; i++)
                            servo_pos[i] = (byte)Convert.ToInt32(r[i + 2]);

                        PlayPose(duration, nos, servo_pos, (n == 1));
                    }

                }
                tr.Close();
            }
            catch (Exception e1)
            {
                Console.WriteLine("Error - can't load file " + e1.Message);
            }
        }

        // NEW:: if byte = 255 = use current positon
        // NEW:: check limits / bounds before sending

        public void PlayPose(int duration, int no_steps, byte[] spod, bool first)
        {
            int cnt = 0;
            byte[] temp = new byte[MAX_SERVOS]; 

            if (first || !initpos) 
                servoID_readservo(0); // read start positons

            double[] intervals = new double[spod.Length];

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

            Console.WriteLine("Debug: diff = " + cnt);

            for (int s = 1; s <= no_steps; s++)
            {
                //
                for (int n = 0; n <= MAX_SERVOS; n++) 
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
