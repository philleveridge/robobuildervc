using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace RobobuilderLib
{
    public delegate void callBack(int n);

    public class PCremote
    {
        public SerialPort serialPort1;
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
                                cmd                         //checksum
                            }, 0, 8);
            return true;
        }

        bool command_nB(byte platform, byte type, byte[] buffer)
        {
            serialPort1.Write(header, 0, 8);
            serialPort1.Write(new byte[] { 
                                type,                                   //type (1)
                                platform,                               //platform (1)
                                0x00, 0x00, 0x00, (byte)buffer.Length,  //command size (4)
                            }, 0, 6);

            serialPort1.Write(buffer, 0, buffer.Length);

            byte[] cs = new byte[1];

            for (int i = 0; i < buffer.Length; i++)
            {
                cs[0] ^= buffer[i];
            }
            serialPort1.Write(cs, 0, 1);
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

                    if (b < header.Length && respnse[b] != header[b])
                    {
                        Console.WriteLine("skip [" + b + "]=" + respnse[b]);
                        b = 0;
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
                bool tf = DCmode; setDCmode(false);
                command_1B(0x1A, 1); // reset motion memory
                if (displayResponse(false))
                {
                    x = (Int16)(((respnse[15] << 8) + (respnse[14])));
                    y = (Int16)(((respnse[17] << 8) + (respnse[16])));
                    z = (Int16)(((respnse[19] << 8) + (respnse[18])));
                    r = "X=" + x.ToString() + ", Y=" + y.ToString() + ", Z=" + z.ToString() ;
                }
                setDCmode(tf);
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
                 0 ,1  ,2  ,3 ,4  ,5  ,6 ,7 ,8  ,9  ,10,11,12 ,13 ,14, 15, 16,17,18*/
            	125,202,162,66,108,124,48,88,184,142,90,40,125,161,210,127,4, 0, 0};

            if (serialPort1.IsOpen)
            {
                command_nB(0, 0x0E, MotionZeroPos);
                displayResponse(true);
            }
            return r;
        }

        public enum RemoCon {
            FAILED=0,
            A=0x01,B,LeftTurn,Forward,RightTurn,Left,Stop,Right,Punch_Left,Back,Punch_Right,
            N1,N2,N3,N4,N5,N6,N7,N8,N9,B0,
        
            S_A=0x16,S_B,S_LeftTurn,S_Forward,S_RightTurn,S_Left,S_Stop,S_Right,S_Punch_Left,S_Back,S_Punch_Right,
            S_N1,S_N2,S_N3,S_N4,S_N5,S_N6,S_N7,S_N8,S_N9,S_B0,

            H_A=0x2B,H_B,H_LeftTurn,H_Forward,H_RightTurn,H_Left,H_Stop,H_Right,H_Punch_Left,H_Back,H_Punch_Right,
            H_N1,H_N2,H_N3,H_N4,H_N5,H_N6,H_N7,H_N8,H_N9,H_B0
       };

        public RemoCon readIR(int timeout_ms, callBack x)
        {
            int n=0;

            int tmp = serialPort1.ReadTimeout;
            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout_ms);

            if (serialPort1.IsOpen)
            {
                serialPort1.ReadTimeout = timeout_ms;

                command_1B(25, 0x01);

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[15] * 256 + respnse[14];
                    }
                    Console.WriteLine(message);
                    x(n); //callback
                }
            }

            serialPort1.ReadTimeout = tmp;

            return (RemoCon)n;
        }


        public int readButton(int timeout, callBack x)
        {
            int n = 0;
            int tmp = serialPort1.ReadTimeout;

            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout);

            if (serialPort1.IsOpen)
            {
                command_1B(24, 0x01);

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[15] * 256 + respnse[14];
                    }
                    x(n);
                    Console.WriteLine(DateTime.Now.ToString() + " = " + message + " n=" + n);
               }
             }
             serialPort1.ReadTimeout = tmp;
             return n;
        }

        public int readsoundLevel(int timeout, int level, callBack x)
        {
            int n = 0;
            int tmp = serialPort1.ReadTimeout;

            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout);


            if (serialPort1.IsOpen)
            {
                command_nB(1, 23, new byte[] { (byte)(level%256), (byte)(level/256) });

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[15] * 256 + respnse[14]; // sound level returned
                    }
                    x(n);
                    Console.WriteLine(DateTime.Now.ToString() + " = " + message + " n=" + n);
                }
            }
            serialPort1.ReadTimeout = tmp;
            return n;
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

        // for compatibility with Homebrew OS

        public bool download_basic(string s)
        {
            return false;
        }

        public string run_basic()
        {
            return "Err - invalid mode";
        }

    }
}
