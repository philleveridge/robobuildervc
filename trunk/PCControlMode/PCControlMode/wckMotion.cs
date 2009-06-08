using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;


namespace RobobuilderLib
{
    class Motion
    {
        /**********************************************
         * 
         * direct Command mode  - wcK prorocol
         * 
         * ********************************************/

        byte[] respnse = new byte[32];
        SerialPort serialPort1;

        public string Message;

        public Motion(SerialPort p)
        {
            serialPort1 = p;
        }

        private bool wckReadPos(int id)
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
                return true;
            }
            catch (Exception e1)
            {
                Message = "Failed" + e1.Message;
                return false;
            }
        }

        private bool wckMovePos(int id, int pos, int torq)
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

        void SyncPosSend(int LastID, int SpeedLevel, byte[] TargetArray, int Index)
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
    }
}
