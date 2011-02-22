using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

using RobobuilderLib;

namespace Demo
{
    public partial class Form1 : Form
    {
        wckMotion w;
        BalanceWalk bw;
        Utility u = new Utility();
        SerialPort s;
        PCremote pcr;

        bool dhf = true; // dance hands mode (servo 12 & 15 rotated by 90deg)

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //connect - disconnect
            if (button2.Text == "Connect")
            {
                button1.Enabled = true;
                button2.Text = "Disconnect";

                if (checkBox1.Checked)
                {
                    // new highspeed comms mode
                    s = new SerialPort(textBox1.Text, 230400);
                }
                else
                {
                    s = new SerialPort(textBox1.Text, 115200);
                }

                s.WriteTimeout = 500;
                s.ReadTimeout = 500;
                s.Open();
                pcr = new PCremote(s);
                w = new wckMotion(pcr); 
                bw = new BalanceWalk(w);

                if (!w.wckReadPos(30, 0))
                {
                    button1.Enabled = false;
                    button2.Text = "Connect";
                    return;
                }
                label1.Text = string.Format("DCMP={0}.{1}", w.respnse[0], w.respnse[1]);

                standup();
            }
            else
            {
                button1.Enabled = false;
                button2.Text = "Connect";
                if (w != null)
                {
                    w.close();
                }
                if (s != null)
                {
                    s.Close();
                }
                bw = null;
                w = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Go")
            {
                button1.Text = "Stop";
                label1.Text = "Delay=" + bw.dely;
                u.win = this;
                u.usePanel(panel1);
                bw.motion(u);                 //balanced walk
                standup();
            }

            button1.Text = "Go";
            label1.Text = "";
            bw.wlk = false;

        }

        public void standup()
        {
            if (dhf)
                w.PlayPose(1000, 10, wckMotion.dh, true);
            else
                w.PlayPose(1000, 10, wckMotion.basic18, true);
        }

        void win_KeyPress(object sender, KeyPressEventArgs e)
        {
            u.kp = true;
            u.ch = e.KeyChar;
            Console.WriteLine("key pressed = {0}", u.ch);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bw.state == "s")
                bw.state = "R";
            else
                bw.state = "s";

            button4.Text = bw.state;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (bw.dely>5) bw.dely -= 5;
            label1.Text = "Delay=" + bw.dely;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bw.dely += 5;
            label1.Text = "Delay=" + bw.dely;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bw.state == "s")
                bw.state = "m";
            else
                bw.state = "s";

            button4.Text = bw.state;
        }

    }
}
