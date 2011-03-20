using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace MoboRobo
{
    public partial class TermF : Form
    {
        string rx = ".";
        int lc = 0;
        string[] buff;
        int mx = 9;

        public TermF()
        {
            InitializeComponent();
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);

            buff = new string[mx];
            for (int i = 0; i < mx; i++)
                buff[i] = "";
            termw.Text = "BASIC Terminal";
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                rx = e.KeyChar.ToString();
                serialPort1.Write(rx);
            }
        }

        void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialData.Chars:
                    rx = serialPort1.ReadExisting();
                    this.Invoke(new EventHandler(DisplayText));
                    break;
                case SerialData.Eof:
                    rx = "@@";
                    this.Invoke(new EventHandler(DisplayText));
                    break;
            }
        }

        private void scroll()
        {
            for (int i = 1; i < mx; i++)
            {
                buff[i - 1] = buff[i];
            }
            buff[mx-1] = "";
        }

        private string getBuff()
        {
            string l = "";
            for (int i = 0; i < lc; i++)
                l += buff[i]+"\n" ;
            l += buff[lc];
            return l;
        }


        private void DisplayText(object sender, EventArgs e)
        {
            if (rx == null)
                return;

            for (int n = 0; n < rx.Length; n++)
            {
                char c = rx[n];

                //need to handle VT100 secquences

                if (c == '\b')
                {
                    lc = 0;
                }
                if (c == Convert.ToChar(10))
                {
                    lc++;
                    if (lc >= mx)
                    {
                        scroll();
                        lc = mx - 1;
                    }
                }
                else
                {
                    if (c > 27 && c < 127)
                        buff[lc] += c;
                }

                termw.Text = getBuff();
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (menuItem1.Text == "CONNECT")
            {
                serialPort1.PortName = "COM6";
                serialPort1.BaudRate = 115200;
                serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort1_DataReceived);
                serialPort1.Open();
                menuItem1.Text = "QUIT";
            }
            else
            {
                if (serialPort1.IsOpen) 
                    serialPort1.Close();
                this.Close();
            }
        }


    }
}