using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace MoboRobo
{
    public delegate void callBack(int n);

    public class PCremote
    {
        public SerialPort serialPort;
        byte[] header = new byte[] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA };
        byte[] respnse = new byte[32];
        bool DCmode;

        public bool dbg = false;

        public string message;

        public PCremote(string comport)
        {
            SerialPort t = new SerialPort(comport, 115200);
            t.WriteTimeout = 500;
            t.ReadTimeout = 500;
            t.Open();
            setup(t);
        }

        public PCremote(SerialPort s)
        {
            setup(s);
        }

        ~PCremote()
        {
            Close();
        }

        public void setdbg(bool x)
        {
            dbg = x;
        }

        public void Close()
        {
            setDCmode(false);
            if (serialPort.IsOpen) serialPort.Close();
        }

        void setup(SerialPort s)
        {
            serialPort = s;
            message = "";
            DCmode = false;
            //send a forced exit DC mode just incase robot was left inthat state
            if (serialPort.IsOpen) 
                serialPort.Write(new byte[] { 0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
        }

        /**********************************************
          * 
          * send request/ read response 
          * serial protocol
          * 
          ********************************************/

        bool command_1B(byte type, byte cmd)
        {
            serialPort.Write(header, 0, 8);
            serialPort.Write(new byte[] { type,           //type (1)
                                0x00,                      //platform (1)
                                0x00, 0x00, 0x00, 0x01,    //command size (4)
                                cmd,                       //command contents (1)
                                cmd                         //checksum
                            }, 0, 8);
            return true;
        }

        bool command_nB(byte platform, byte type, byte[] buffer)
        {
            serialPort.Write(header, 0, 8);
            serialPort.Write(new byte[] { 
                                type,                                   //type (1)
                                platform,                               //platform (1)
                                0x00, 0x00, 0x00, (byte)buffer.Length,  //command size (4)
                            }, 0, 6);

            serialPort.Write(buffer, 0, buffer.Length);

            byte[] cs = new byte[1];

            for (int i = 0; i < buffer.Length; i++)
            {
                cs[0] ^= buffer[i];
            }
            serialPort.Write(cs, 0, 1);
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
                    respnse[b] = (byte)serialPort.ReadByte();

                    if (b < header.Length && respnse[b] != header[b])
                    {
                        System.Diagnostics.Debug.WriteLine("skip [" + b + "]=" + respnse[b]);
                        b = 0;
                        continue;
                    }

                    if (b == 13)
                    {
                        l = (respnse[b - 3] << 24) + (respnse[b - 2] << 16) + (respnse[b - 1] << 8) + respnse[b];
                        //Console.WriteLine("L=" + l);
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
            string r = "0";

            if (serialPort.IsOpen)
            {
                command_1B(0x12, 0x01);
                if (displayResponse(false))
                    r = respnse[14] + "." + respnse[15];
            }
            return r;
        }

        public string serial_number()
        {
            return readSN();
        }

        public string readSN()
        {
            // read serial number
            string r = "";

            if (serialPort.IsOpen)
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

        public int readPSD()
        {
            // read distance
            int r = 0;

            if (serialPort.IsOpen)
            {
                command_1B(0x16, 0x01);
                if (displayResponse(true))
                    r = respnse[14] + (respnse[15] << 8);
            }
            return r;
        }

        public string readDistance()
        {
            // read distance
            return readPSD().ToString();
        }

        public int[] accelerometer()
        {
            return readXYZ();
        }

        public int[] readXYZ()
        {
            int x, y, z;
            readXYZ(out x, out y, out z) ;   
            return (new int[3] {(int)x, (int)y, (int)z});
        }

        public string readXYZ(out int x, out int y, out int z)
        {
            string r = "";
            x = 0; y = 0; z = 0;

            if (serialPort.IsOpen)
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
            if (serialPort.IsOpen)
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
            if (serialPort.IsOpen)
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
            if (serialPort.IsOpen)
            {
                command_1B(0x0B, 0x01);
                displayResponse(true);
            }
            return r;
        }

        public string a()        { return runMotion(1); }
        public string b()        { return runMotion(2); }
        public string basic()    { return runMotion(7); }
        public string run(int m) { return runMotion(m); }

        public string runMotion(int m)
        {
            //read zeros
            if (m < 1 || m > 42)
            {
                return "Invalid Motion";
            }

            string r = "";
            if (serialPort.IsOpen)
            {
                command_1B(20, (byte)m);
                displayResponse(true);
            }
            return r;
        }

        public string runSound(int m)
        {
            //play a specific sound
            string r = "";

            if (m < 1 || m > 25)
            {
                return "Invalid Sound";
            }

            if (serialPort.IsOpen)
            {
                command_1B(21, (byte)m);
                displayResponse(true);
            }
            return r;
        }

        public string executionStatus(int m)
        {
            //get execution status for specific motion
            string r = "";
            if (serialPort.IsOpen)
            {
                command_1B(30, (byte)m);
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

            if (serialPort.IsOpen)
            {
                command_nB(0, 0x0E, MotionZeroPos);
                displayResponse(true);
            }
            return r;
        }

        public enum RemoCon {
            FAILED=0,
            A=0x01,B,LeftTurn,Forward,RightTurn,Left,Stop,Right,Punch_Left,Back,Punch_Right,
            N1,N2,N3,N4,N5,N6,N7,N8,N9,N0,
        
            S_A=0x16,S_B,S_LeftTurn,S_Forward,S_RightTurn,S_Left,S_Stop,S_Right,S_Punch_Left,S_Back,S_Punch_Right,
            S_N1,S_N2,S_N3,S_N4,S_N5,S_N6,S_N7,S_N8,S_N9,S_N0,

            H_A=0x2B,H_B,H_LeftTurn,H_Forward,H_RightTurn,H_Left,H_Stop,H_Right,H_Punch_Left,H_Back,H_Punch_Right,
            H_N1,H_N2,H_N3,H_N4,H_N5,H_N6,H_N7,H_N8,H_N9,H_N0
       };

        public RemoCon readIR(int timeout_ms)
        {
            return readIR(timeout_ms, null);
        }

        public RemoCon readIR(int timeout_ms, callBack x)
        {
            int n=0;

            int tmp = serialPort.ReadTimeout;
            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout_ms);

            if (serialPort.IsOpen)
            {
                serialPort.ReadTimeout = timeout_ms;

                command_1B(25, 0x01);

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[14] + (respnse[15] <<8) ;
                    }
                    Console.WriteLine(message);
                    if (x != null)
                        x(n);
                    else
                        break;
                }
            }

            serialPort.ReadTimeout = tmp;

            return (RemoCon)n;
        }


        public int readButton(int timeout)
        {
            return readButton(timeout, null);
        }

        public int readButton(int timeout, callBack x)
        {
            int n = 0;
            int tmp = serialPort.ReadTimeout;

            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout);

            if (serialPort.IsOpen)
            {
                command_1B(24, 0x01);

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[14] + (respnse[15] << 8);
                    }

                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " = " + message + " n=" + n);

                    if (x != null)
                        x(n);
                    else
                        break;
               }
             }
             serialPort.ReadTimeout = tmp;
             return n;
        }

        public int readsoundLevel(int timeout, int level)
        {
            return readsoundLevel(timeout, level, null);
        }

        public int readsoundLevel(int timeout, int level, callBack x)
        {
            int n = 0;
            int tmp = serialPort.ReadTimeout;

            DateTime end = DateTime.Now + TimeSpan.FromMilliseconds((double)timeout);


            if (serialPort.IsOpen)
            {
                command_nB(1, 23, new byte[] { (byte)(level%256), (byte)(level/256) });

                while (DateTime.Now < end)
                {
                    if (displayResponse(true))
                    {
                        n = respnse[14] + (respnse[15] << 8); // sound level
                    }
                    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " = " + message + " n=" + n);
                    if (x != null)
                        x(n);
                    else
                        break;
                }
            }
            serialPort.ReadTimeout = tmp;
            return n;
        }

        public void setDCmode(bool f)
        {
            if (DCmode == f) return; //only output chnages
            DCmode = f;
            if (f)
            {
                // DC mode
                if (serialPort.IsOpen)
                {
                    command_1B(0x10, 0x01);
                    displayResponse(false);
                }
            }
            else
            {
                // end DC mode
                if (serialPort.IsOpen) serialPort.Write(new byte[] { 0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
            }
        }
    }
}
