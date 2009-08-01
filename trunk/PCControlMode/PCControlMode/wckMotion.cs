using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;


namespace RobobuilderLib
{
    class wckMotion
    {
        /**********************************************
         * 
         * direct Command mode  - wcK prorocol
         * 
         * ********************************************/

        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
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
        }

        ~wckMotion()
        {
            close();
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
                Console.WriteLine(Message); // debug
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
                Console.WriteLine(Message); // debug
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

        public void servoID_readservo()
        {
            pos = new byte[sids.Length];

            for (int id = 0; id < sids.Length; id++)
            {
                if (wckReadPos(id))                 //readPOS (servoID)
                {
                    if (respnse[1] < 255)
                    {
                        pos[id] = respnse[1];
                    }
                }
            }
        }

        private void delay_ms(int t1)
        {
            System.Threading.Thread.Sleep(t1);
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
