using System;
using System.Windows.Forms;
using System.IO;

namespace RobobuilderLib
{
    public partial class Form1 : Form
    {
        byte[] header ;
        byte[] respnse = new byte[32];
        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        byte[] pos;

        private System.Windows.Forms.HScrollBar[] servoPos;
        private System.Windows.Forms.TextBox[] servoID;
        private System.Windows.Forms.CheckBox[] readID;

        int h;

        string filename = "";

        public Form1()
        {

            InitializeComponent();

            //

            servoPos = new System.Windows.Forms.HScrollBar[20];
            servoID = new System.Windows.Forms.TextBox[20];
            readID = new System.Windows.Forms.CheckBox[20];

            for (int i = 0; i < sids.Length; i++)
            {
                servoPos[i] = new System.Windows.Forms.HScrollBar();
                servoID[i] = new System.Windows.Forms.TextBox();
                readID[i] = new System.Windows.Forms.CheckBox();

                // 
                // servoPos
                // 
                servoPos[i].Location = new System.Drawing.Point(318, 33+20*i);
                servoPos[i].Minimum = 0;
                servoPos[i].Maximum = 254;
                servoPos[i].Value = 127;
                servoPos[i].Name = "servoPos-" + i.ToString();
                servoPos[i].Size = new System.Drawing.Size(67, 20);
                servoPos[i].TabIndex = 11;
                servoPos[i].Visible = true;
                servoPos[i].Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
                // 
                // servoID
                // 
                servoID[i].Location = new System.Drawing.Point(391, 36+20*i);
                servoID[i].Name = "servoID-"+i.ToString();
                servoID[i].ReadOnly = true;
                servoID[i].Size = new System.Drawing.Size(80, 20);
                servoID[i].TabIndex = 12;
                servoID[i].Text = sids[i].ToString();
                servoID[i].Visible = true;

                // 
                // readID
                // 
                readID[i].AutoSize = true;
                readID[i].Location = new System.Drawing.Point(300, 36+20*i);
                readID[i].Name = "readID-" + i.ToString();
                readID[i].Size = new System.Drawing.Size(80, 17);
                readID[i].TabIndex = 36;
                readID[i].Text = "";
                readID[i].UseVisualStyleBackColor = true;
                readID[i].CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);

                this.Controls.Add(servoID[i]);
                this.Controls.Add(servoPos[i]);
                this.Controls.Add(readID[i]);

            }

            h = 36 + 20 * (sids.Length+2) +10;

            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 115200;
            serialPort1.ReadTimeout = 3000;

            header = new byte[] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA };

            listBox1.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                listBox1.Items.Add(s);
            }

            label1.Text = "Disconnected";


        }


        /**********************************************
         * 
         * connect / disconnect serial port to RBC controller
         * 
         * ********************************************/

        private void connect_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                connect.Text = "Connect";
                set_buttons(false);
                set_servocntl(false);
                listBox1.Enabled = true;

                label1.Text = "Disconnected";

            }
            else
            {
                serialPort1.Open();
                connect.Text = "Close";
                listBox1.Enabled = false;
                set_buttons(true);
                set_servocntl(false);
                textBox1.Text = "";

                // start up on connect

                label1.Text = "Firmware=" + readVer() + ", S/N=" + readSN();
                //basicPose();
            }
        }

        private void clr_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void set_buttons(bool f)
        {
            //button1.Enabled = f;
            //button2.Enabled = f;
            button3.Enabled = f;
            button4.Enabled = f;
            button5.Enabled = f;
            button6.Enabled = f;
            button7.Enabled = f;
            button8.Enabled = f;
            button9.Enabled = f;
            button10.Enabled = f;
        }

        private void set_servocntl(bool f)
        {
            // enable servo control
            button4.Enabled = f;
            servomsg.Visible = f;

            if (f)
            {
                Form1.ActiveForm.Size = new System.Drawing.Size(600, h);
            }
            else
                Form1.ActiveForm.Size = new System.Drawing.Size(300, 300);

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
                    textBox1.AppendText("Response:\n");
                    for (int i = 0; i < 7 + l; i++)
                    {
                        textBox1.AppendText(respnse[8 + i].ToString("X2") + " ");
                    }
                    textBox1.AppendText("\r\n");
                }
                return true;
            }
            catch (Exception e1)
            {
                textBox1.AppendText("Timed Out = " + e1.Message + "\r\n");
                return false;
            }
        }

        private string readVer()
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

        private string readSN()
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

        private void NewBasicPose()
        {
            PlayPose(100, 10, new byte[] {
/*0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 */
171,179,198,83,105,78,72,49,172,141,47,47,49,200,205,205,122,125,127 });

        }


        void PlayPose(int duration, int no_steps, byte[] spod )
        {
            if (!serialPort1.IsOpen) return;

            byte[] temp = new byte[19]; // numbr of servos

            // DC mode

            if (!command_1B(0x10, 0x01)) return;
            displayResponse(true);

            new Motion(serialPort1).PlayPose(duration, no_steps, spod);

            // end DC mode

            serialPort1.Write(new byte[] { 0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
        }


        private void servoID_readservo()
        {
            Motion m= new Motion(serialPort1);
            m.servoID_readservo();

            for (int id = 0; id < sids.Length; id++)
            {
                servoPos[id].Value = m.pos[id];
                servoID[id].Text = sids[id].ToString() + " - " + servoPos[id].Value.ToString();
            }
        }

        /**********************************************
         * 
         * Action buttons  - Remote / serial prorocol
         * 
         * ********************************************/

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Ver=" + readVer() +"\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("S/N="+readSN()+"\r\n");
        }


        private void button3_Click(object sender, EventArgs e)
        {
            // DC mode
            if (serialPort1.IsOpen)
            {
                command_1B(0x10, 0x01);
                displayResponse(true);

                set_buttons(false);
                set_servocntl(true);

                servoID_readservo();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // DC mode release
            if (serialPort1.IsOpen)
            {
                serialPort1.Write(new byte[] {0xFF, 0xE0, 0xFB, 0x1, 0x00, 0x1A},0, 6);

                set_buttons(true);
                set_servocntl(false);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // read distance
            if (serialPort1.IsOpen)
            {
                command_1B(0x16, 0x01);
                if (displayResponse(true))
                    textBox1.AppendText("Distance=" + (respnse[14] << 8) + respnse[15] + "cm\r\n");
            }
        }

        private void button6_Click(object sender, EventArgs e)
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
        
        private void button7_Click(object sender, EventArgs e)
        {
            // avail mem
            if (serialPort1.IsOpen)
            {
                command_1B(0x0F, 0x01);
                if (displayResponse(false))
                    textBox1.AppendText("Avail mem=" + ((respnse[14] << 24) + (respnse[15]<<16)
                        + (respnse[16] << 8) + respnse[17]) 
                        + " Bytes\r\n");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // basic Pose
            NewBasicPose();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //read zeros
            if (serialPort1.IsOpen)
            {
                command_1B(0x0B, 0x01);
                displayResponse(true);
            }
        }

        private void button10_Click(object sender, EventArgs e)
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

        /**********************************************
         * 
         * Event routines
         * 
         * change of serial port
         * change of servo id  (DC mode only)
         * change of scrollbar (DC mode only)
         * 
         * ********************************************/

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
                serialPort1.PortName = listBox1.Items[listBox1.SelectedIndex].ToString();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int id = Convert.ToInt32(((HScrollBar)sender).Name.Substring(9));
            int v = ((HScrollBar)sender).Value;
            Console.WriteLine("Id=" + sids[id] + ", V=" + v);

            servoID[id].Text = sids[id].ToString() + " - " + v;
            new Motion(serialPort1).wckMovePos(sids[id], v, 2);
        }

        private void readll_Click(object sender, EventArgs e)
        {
            servoID_readservo();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((CheckBox)sender).Name.Substring(7));
            bool v = ((CheckBox)sender).Checked;
            Console.WriteLine("Id=" + sids[id] + ", V=" + v);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //record !

            if (filename == "")
            {
                SaveFileDialog s = new SaveFileDialog();
                if (s.ShowDialog() == DialogResult.OK)
                    filename = s.FileName;
                else
                    return;
            }

            try
            {
                TextWriter tw = new StreamWriter(filename, true);

                tw.Write("500,10");

                for (int i = 0; i < sids.Length; i++)
                {
                    tw.Write("," + servoPos[i].Value.ToString());
                }
                tw.WriteLine("");
                tw.Close();

            }
            catch (Exception e1)
            {
                MessageBox.Show("Error - can't write to file - " + e1);
            }

        }

        private void delay_ms(int t1)
        {
            DateTime t = DateTime.Now;
            TimeSpan d;
            do
            {
                d = DateTime.Now - t;
                Application.DoEvents();
            } while (d < TimeSpan.FromMilliseconds(t1));
        }

        private void play_Click(object sender, EventArgs e)
        {
            // play

            play.Enabled = false;

            int n=0;

            if (filename == "")
            {
                OpenFileDialog o = new OpenFileDialog();
                if (o.ShowDialog() == DialogResult.OK)
                    filename = o.FileName;
                else
                    return;
            }

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                Motion m = new Motion(serialPort1);

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] r = line.Split(',');

                    if (r.Length > 2)
                    {
                        n++;
                        label2.Text = n.ToString();

                        for (int i = 2; i < r.Length; i++)
                        {
                            int v = Convert.ToInt32(r[i]);
                            servoPos[i-1].Value = v;
                            servoID[i-1].Text = sids[i-1].ToString() + " - " + v;

                            
                            if (!m.wckMovePos(sids[i - 1], v, 2))
                            {
                                MessageBox.Show("Error - can't write to servo");
                                play.Enabled = true;
                                return;
                            }
                        }
                    }

                    if (checkBox1.Enabled)
                    {
                        MessageBox.Show("next "+n);
                    }
                    else
                    {
                        delay_ms(Convert.ToInt32(r[0]));
                    }
                }

                tr.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error - can't load file " + e1);
            }

            play.Enabled = true;

        }

    }
}
