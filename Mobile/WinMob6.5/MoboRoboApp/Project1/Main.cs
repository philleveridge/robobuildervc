using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace MoboRobo
{
    public partial class Main : Form
    {
        PCremote pcr=null;
        bool basicMode = false; // is Basic.hex firmware ?
        string cport = "COM6";

        string map = "0,1;1,5;2,4;3,9;4,6;5,11;6,8;7,3;8,10;9,7";  //in ch, out motion
        string[] tr;

        private System.Windows.Forms.Label[] b = new System.Windows.Forms.Label[10];

        public Main()
        {
            //InitializeComponent();
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
            if (File.Exists("Config.txt")) 
            {
                string s;
                StreamReader n = File.OpenText("Config.txt");
                while (!n.EndOfStream)
                {
                    s = n.ReadLine().ToUpper();
                    if (s.StartsWith("COM"))
                        cport = s;
                    else if (s.StartsWith("MAP"))
                        map = s.Substring(5);
                }
                n.Close();
            }
            tr = map.Split(';');
            setupkeys();
            InitializeComponent();

        }

        void showcontroller(bool f)
        {
            pictureBox1.Visible = f;
            for (int i = 0; i <= 9; i++) b[i].Visible = f;
        }

        void setupkeys()
        {
        	for (int i=0; i<=9; i++)
       	    {
           		b[i] = new System.Windows.Forms.Label();
        		b[i].Size = new System.Drawing.Size(12, 12);
                b[i].Text = tr[i].Substring(0, 1);
       		    b[i].Name = "b" + i;
       		    Controls.Add(this.b[i]);
                b[i].Visible = false ;
            }

            b[0].Location = new System.Drawing.Point(48, 10);
            b[1].Location = new System.Drawing.Point(136, 40);
            b[2].Location = new System.Drawing.Point(48, 40);
            b[3].Location = new System.Drawing.Point(109, 60);
            b[4].Location = new System.Drawing.Point(26, 60);
            b[5].Location = new System.Drawing.Point(161, 60); 
            b[6].Location = new System.Drawing.Point(65, 60);
            b[7].Location = new System.Drawing.Point(136, 80);
            b[8].Location = new System.Drawing.Point(48, 80);
            b[9].Location = new System.Drawing.Point(136, 10);
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string ch = e.KeyChar.ToString();

            if (!serialPort1.IsOpen)   // not connected
            {
                if (ch == "b")
                {
                    basicMode = !basicMode;

                    label1.Text = (basicMode)?"BASIC MODE":"FIRMWARE";
                }

                if (ch == "t")
                {
                    TermF t = new TermF();
                    t.Show();
                }
                if (ch == "c")
                {
                    showcontroller(!pictureBox1.Visible);
                }
                return;

            }

            int motion = 0;
            foreach (string s in tr)
            {
                if (s.StartsWith(ch))
                {
                    motion = Convert.ToInt32(s.Substring(2));
                    break;
                }
            }

            label1.Text = ch + ":" + motion.ToString();
            if (basicMode)
            {
                // pass IR equivalent
                serialPort1.Write(new byte[1] {Convert.ToByte(motion)},0,1);
                return;
            }

            if (motion != 0 && pcr != null) // send motion
                pcr.runMotion(motion);

        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            serialPort1.WriteTimeout = 2000;
            serialPort1.ReadTimeout = 2000;


            if (serialPort1.IsOpen)
            {
                if (pcr != null) pcr.Close();
                pcr = null;
                serialPort1.Close();
                menuItem2.Text = "CONNECT";
                showcontroller(false);
                return;
            }

            try
            {
                label1.Text = "Connecting ..." + cport;
                serialPort1.PortName = cport;
                serialPort1.Open();
            }
            catch
            {
                label1.Text = "CANNOT CONNECT " + cport;
                showcontroller(false);
            }

            if (serialPort1.IsOpen)
            {
                label1.Text = "Port open - " + serialPort1.PortName;

                try
                {
                    if (!basicMode)
                    {
                        if (pcr == null)
                            pcr = new PCremote(serialPort1);

                        this.Text += " Ver = " + pcr.readVer() + " : " + pcr.readSN();
                    }
                    else
                        this.Text += " *";
                    showcontroller(true);
                    menuItem2.Text = "";

                }
                catch (Exception err)
                {
                    label1.Text += " READ FAILED " + err.Message;
                    showcontroller(false);
                }

                //serialPort1.Close();
            }
        
        }
    }
}