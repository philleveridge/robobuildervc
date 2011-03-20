using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace MoboRobo
{
    public partial class Form1 : Form
    {
        PCremote pcr = null;
        wckMotion wk = null;

        bool basicMode = false; // is Basic.hex firmware ?

        bool pause = true;

        string cport = "COM6";

        public Form1()
        {
            InitializeComponent();

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
                }
                n.Close();
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            //quit
            this.Close();
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string ch = e.KeyChar.ToString();
            if (ch==" ")pause = false;
        }

        private void walk()
        {
            pause = true;
            label1.Text = "Press any key";
            while (pause) { System.Windows.Forms.Application.DoEvents(); }
            //

            pcr = new PCremote(serialPort1);
            wk = new wckMotion(pcr);

           

            //
            label1.Text = "Done";
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            //connect
            serialPort1.WriteTimeout = 2000;
            serialPort1.ReadTimeout = 2000;

            if (serialPort1.IsOpen)
            {
                if (pcr != null) pcr.Close();
                pcr = null;
                serialPort1.Close();
                menuItem2.Text = "CONNECT";
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
            }

            if (serialPort1.IsOpen)
            {
                label1.Text = "Port open - " + serialPort1.PortName;

                walk();
            }
        }
    }
}