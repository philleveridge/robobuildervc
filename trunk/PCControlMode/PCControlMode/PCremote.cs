using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace RobobuilderLib
{
    class PCremote
    {
        SerialPort serialPort1;
        byte[] header;
        byte[] respnse = new byte[32];
        bool DCmode;

        public string message;

        public PCremote(SerialPort s)
        {
            serialPort1 = s;
            header = new byte[] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA };
            message = "";
            DCmode = false;
        }

        /**********************************************
          * 
          * send request/ read response 
          * serial protocol
          * 
          ********************************************/

        bool command_1B(byte type, byte cmd)
        {
            serialPort1.Write(header, 0, 8);
            serialPort1.Write(new byte[] { type,           //type (1)
                                0x00,                      //platform (1)
                                0x00, 0x00, 0x00, 0x01,    //command size (4)
                                cmd,                       //command contents (1)
                                (byte)(cmd)                //checksum
                            }, 0, 8);
            return true;
        }

        bool displayResponse(bool flag)
        {
            try
            {
                int b = 0;
                int l = 1;

                while (b < 32 && b < (15 + l))
                {
                    respnse[b] = (byte)serialPort1.ReadByte();

                    if (b == 0 && respnse[b] != header[b])
                    {
                        Console.WriteLine("skip [" + b + "]=" + respnse[b]);
                        continue;
                    }

                    if (b == 13)
                    {
                        l = (respnse[b - 3] << 24) + (respnse[b - 2] << 16) + (respnse[b - 1] << 8) + respnse[b];
                        Console.WriteLine("L=" + l);
                    }
                    b++;
                }

                if (flag)
                {
                    message = "Response:\n";
                    for (int i = 0; i < 7 + l; i++)
                    {
                        message += respnse[8 + i].ToString("X2") + " ";
                    }
                    message += "\r\n";
                }
                return true;
            }
            catch (Exception e1)
            {
                message = "Timed Out = " + e1.Message + "\r\n";
                return false;
            }
        }

        public string readVer()
        {
            //read firmware version number
            string r = "";

            if (serialPort1.IsOpen)
            {
                command_1B(0x12, 0x01);
                if (displayResponse(false))
                    r = respnse[14] + "." + respnse[15];
            }
            return r;
        }

        public string readSN()
        {
            // read serial number
            string r = "";

            if (serialPort1.IsOpen)
            {
                command_1B(0x0C, 0x01);
                if (displayResponse(false))
                {
                    for (int n0 = 0; n0 < 13; n0++)
                        r += Convert.ToString((char)respnse[14 + n0]);
                }
            }
            return r;
        }

        public string readDistance()
        {
            // read distance
            string r = "";

            if (serialPort1.IsOpen)
            {
                command_1B(0x16, 0x01);
                if (displayResponse(true))
                    r = ((respnse[14] << 8) + respnse[15]).ToString();
            }
            return r;
        }

        public string readXYZ(out Int16 x, out Int16 y, out Int16 z)
        {
            string r = "";
            x = 0; y = 0; z = 0;
            if (serialPort1.IsOpen)
            {
                command_1B(0x1A, 1); // reset motion memory
                if (displayResponse(false))
                {
                    x = (Int16)(((respnse[15] << 8) + (respnse[14])));
                    y = (Int16)(((respnse[17] << 8) + (respnse[16])));
                    z = (Int16)(((respnse[19] << 8) + (respnse[18])));
                    r = "X=" + x.ToString() + ", Y=" + y.ToString() + ", Z=" + z.ToString() ;
                }
            }
            return r;
        }

        public string availMem()
        {
            // avail mem
            string r = "";
            if (serialPort1.IsOpen)
            {
                command_1B(0x0F, 0x01);
                if (displayResponse(false))
                    r += "Avail mem=" + ((respnse[14] << 24) + (respnse[15] << 16)
                        + (respnse[16] << 8) + respnse[17]).ToString()
                        + " Bytes\r\n";
            }
            return r;
        }

        public string resetMem()
        {
            string r = "";
            //reset memory
            if (serialPort1.IsOpen)
            {
                command_1B(0x1F, 0x01); // reset motion memory
                displayResponse(true);

                command_1B(0x1F, 0x02); // reset action memory
                displayResponse(true);
            }
            return r;
        }

        public string readZeros()
        {
            //read zeros
            string r = "";
            if (serialPort1.IsOpen)
            {
                command_1B(0x0B, 0x01);
                displayResponse(true);
            }
            return r;
        }

        public string zeroHuno()
        {
            string r = "";
            //set zeros to Standard Huno
            byte[] MotionZeroPos = new byte[] {
                /* ID
                 0 ,1 ,2 ,3 ,4 ,5 ,6 ,7 ,8 ,9 ,10,11,12,13,14,15 */
             //   125,201,163,67,108,125,48,89,184,142,89,39,124,162,211,127};
            /* ID
                 0 ,1  ,2  ,3 ,4  ,5  ,6 ,7 ,8  ,9  ,10,11,12 ,13 ,14, 15, 16,17,18*/
            	125,202,162,66,108,124,48,88,184,142,90,40,125,161,210,127,4, 0, 0};

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(header, 0, 8);
                serialPort1.Write(new byte[] { 
                        0x0E,        //type (1)
                        0x00,                      //platform (1)
                        0x00, 0x00, 0x00, (byte)MotionZeroPos.Length,    //command size (4)
                     }, 0, 6);

                serialPort1.Write(MotionZeroPos, 0, MotionZeroPos.Length);

                byte[] cs = new byte[1];

                for (int i = 0; i < MotionZeroPos.Length; i++)
                {
                    cs[0] ^= MotionZeroPos[i];
                }
                serialPort1.Write(cs, 0, 1);
                displayResponse(true);
            }
            return r;
        }

        public void setDCmode(bool f)
        {
            if (DCmode == f) return; //only output chnages
            DCmode = f;
            if (f)
            {
                // DC mode
                if (serialPort1.IsOpen)
                {
                    command_1B(0x10, 0x01);
                    displayResponse(false);
                }

            }
            else
            {
                // end DC mode
                if (serialPort1.IsOpen) serialPort1.Write(new byte[] { 0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
            }
        }

    }
}
