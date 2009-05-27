using System;
using System.Text;
using System.IO;
using System.IO.Ports;

// Works directly with existing firmware

namespace RobobuilderVC
{
    class PC_ControlMode
    {
        byte[] header ;
        byte[] respnse = new byte[32];
        byte[] servoPos = new byte[20];

        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 20 };

        SerialPort serialPort1;

        public string mode;
        public string response;

        public PC_ControlMode(SerialPort p)
        {
            serialPort1 = p;

            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 115200;
            serialPort1.ReadTimeout = 3000;

            header = new byte[] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA };

            mode = "Disconnected";
            response = "";
        }


        /**********************************************
         * 
         * connect / disconnect serial port to RBC controller
         * 
         * ********************************************/

        public void connect()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();

                mode = "Disconnected";

            }
            else
            {
                serialPort1.Open();

                // start up on connect
                mode = "Connected";

                response = "Firmware=" + readVer() + ", S/N=" + readSN();
                basicPose();
            }
        }

        /**********************************************
         * 
         * send request/ read response 
         * serial protocol
         * 
         * ********************************************/

        bool command_1B(byte type, byte cmd)
        {
            serialPort1.Write(header, 0, 8);
            serialPort1.Write(new byte[] { type,           //type (1)
                                0x00,                      //platform (1)
                                0x00, 0x00, 0x00, 0x01,    //command size (4)
                                cmd,                       //command contents (1)
                                (byte)(cmd)                //checksum
                            },0,8);
            return true;
        }

        bool displayResponse(bool flag)
        {
            response = "";
            try
            {
                int b = 0;
                int l = 1;

                while (b < 32 && b<(15+l))
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
                    for (int i = 0; i < 7 + l; i++)
                    {
                        response += respnse[8 + i].ToString("X2") + " ";
                    }
                }
                return true;
            }
            catch (Exception e1)
            {
                response = "Timed Out = " + e1.Message;
                return false;
            }
        }

        /**********************************************
         * 
         * direct Command mode  - wcK prorocol
         * 
         * ********************************************/

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
                response = "ReadPos "+id+" = " + respnse[0] + ":" + respnse[1];
                return true;
            }
            catch (Exception e1)
            {
                response = "Failed" +e1.Message;
                return false;
            }
        }

        private bool wckMovePos(int id, int pos, int torq)
        {
            byte[] buff = new byte[4];
            buff[0] = 0xFF;
            buff[1] = (byte)(((torq % 5) << 5) | (id % 31));
            buff[2] = (byte)(pos %254); // arbitary
            buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

            try
            {
                serialPort1.Write(buff, 0, 4);
                respnse[0] = (byte)serialPort1.ReadByte();
                respnse[1] = (byte)serialPort1.ReadByte();
                response  = "MovePos " + id + " = " + respnse[0] + ":" + respnse[1];

                return true;
            }
            catch (Exception e1)
            {
                response  = "Failed" + e1.Message;
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
                    r= respnse[14] + "." + respnse[15];
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

        public void basicPose()
        {
            if (serialPort1.IsOpen)
            {
                command_1B(0x14, 0x07);
                displayResponse(true);
            }
        }

        /**********************************************
         * 
         * Vuilt in commands - Remote / serial prorocol
         * 
         * ********************************************/


        public void DCset()
        {
            // DC mode
            if (serialPort1.IsOpen)
            {
                command_1B(0x10, 0x01);
                displayResponse(true);
            }
        }

        public void DCrelease()
        {
            // DC mode release
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new byte[] {0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A},0, 6);
            }
        }

        public string readDistance()
        {
            // read distance
            string r = "";
            if (serialPort1.IsOpen)
            {
                command_1B(0x16, 0x01);
                if (displayResponse(true))
                    r="Distance=" + (respnse[14] << 8) + respnse[15] + "cm\r\n";
            }
            return r;
        }

        public void resetMem()
        {
            //reset memory
            if (serialPort1.IsOpen)
            {
                command_1B(0x1F, 0x01); // reset motion memory
                displayResponse(true);

                command_1B(0x1F, 0x02); // reset action memory
                displayResponse(true);
            }
        }

        public string availMem()
        {
            // avail mem
            string r = "";
            if (serialPort1.IsOpen)
            {
                command_1B(0x0F, 0x01);
                if (displayResponse(false))
                    r="Avail mem=" + ((respnse[14] << 24) + (respnse[15]<<16)
                        + (respnse[16] << 8) + respnse[17]) 
                        + " Bytes\r\n";
            }
            return r;
        }

        public void readZeros()
        {
            //read zeros
            if (serialPort1.IsOpen)
            {
                command_1B(0x0B, 0x01);
                displayResponse(true);
            }
        }

        public void setZeros()
        {
            //set zeros to Standard Huno
            byte[] MotionZeroPos = new byte[] {
                /* ID
                 0 ,1 ,2 ,3 ,4 ,5 ,6 ,7 ,8 ,9 ,10,11,12,13,14,15 */
                125,201,163,67,108,125,48,89,184,142,89,39,124,162,211,127};

            if (serialPort1.IsOpen)
            {
                serialPort1.Write(header, 0, 8);
                serialPort1.Write(new byte[] { 
                        0x0E,        //type (1)
                        0x00,                      //platform (1)
                        0x00, 0x00, 0x00, (byte)MotionZeroPos.Length,    //command size (4)
                     }, 0, 6);

                serialPort1.Write(MotionZeroPos, 0, 16);

                byte[] cs = new byte[1];

                for (int i = 0; i < MotionZeroPos.Length; i++)
                {
                    cs[0] ^= MotionZeroPos[i];
                }
                serialPort1.Write(cs, 0, 1);
                displayResponse(true);
            }
        }

        public void readservo()
        {
            for (int id = 0; id < sids.Length; id++)
            {
                //readPOS (servoID)

                if (wckReadPos(sids[id]))
                {
                    if (respnse[1] < 255)
                    {
                        servoPos[id] = respnse[1];
                    }
                }
            }
        }

        public bool record()
        {
            //record !

            try
            {
                TextWriter tw = new StreamWriter("motion.txt", true);

                tw.Write("500");

                for (int i = 0; i < sids.Length; i++)
                {
                    tw.Write("," + servoPos[i].ToString());
                }
                tw.WriteLine("");
                tw.Close();

            }
            catch (Exception e1)
            {
                //MessageBox.Show("Error - can't write to file - " + e1);
                return false;
            }
            return true;

        }

        public bool play(bool debug)
        {
            // play

            int n=0;

            try
            {
                TextReader tr = new StreamReader("motion.txt");
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] r = line.Split(',');

                    if (r.Length > 2)
                    {
                        n++;
                        //label2.Text = n.ToString();

                        for (int i = 1; i < r.Length; i++)
                        {
                            byte v = (byte)Convert.ToInt32(r[i]);
                            servoPos[i-1] = v;
                            if (!wckMovePos(sids[i - 1], v, 2))
                            {
                                return false;
                            }
                        }
                    }

                    if (debug)
                    {
                        //MessageBox.Show("next "+n);
                    }
                    else
                    {

                        DateTime t = DateTime.Now;
                        TimeSpan d;
                        do
                        {
                            d = DateTime.Now - t;
                            //Application.DoEvents();
                        } while (d < TimeSpan.FromMilliseconds(Convert.ToInt32(r[0])));
                    }
                }

                tr.Close();
            }
            catch (Exception e1)
            {
                //MessageBox.Show("Error - can't load file " + e1);
                return false;
            }
            return true;

        }

    }
}
