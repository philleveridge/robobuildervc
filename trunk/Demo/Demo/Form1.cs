using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RobobuilderLib;

namespace Demo
{
    public partial class Form1 : Form
    {
        wckMotion w;
        Demo.BalanceWalk bw;
        Utility u = new Utility();

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

                w = new wckMotion(textBox1.Text, true);
                bw = new BalanceWalk(w);
                standup();
            }
            else
            {
                button1.Enabled = false;
                button2.Text = "Connect";
                if (w != null) w.close();
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
                swin(u);
                bw.motion(u);                 //balanced walk
                standup();
            }
            else
            {
                button1.Text = "Go";
                label1.Text = "";
                bw.wlk = false;
            }
        }

        public void standup()
        {
            w.PlayPose(1000, 10, wckMotion.basic18, true);
        }

        void win_KeyPress(object sender, KeyPressEventArgs e)
        {
            u.kp = true;
            u.ch = e.KeyChar;
            Console.WriteLine("key pressed = {0}", u.ch);
        }

        void swin(Utility u)
        {
            u.p1 = new Pen(Color.FromName("Black"));
            u.p1.DashStyle = (System.Drawing.Drawing2D.DashStyle.DashDot);
            u.p2 = new Pen(Color.FromName("Red"));
            u.win = this;
            u.usePanel(panel1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bw.state == "s")
                bw.state = "R";
            else
                bw.state = "s";
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
            w.wckReadPos(30, 5);
            MessageBox.Show("PSD=" + w.respnse[0]);

            w.wckReadPos(30, 7);
            MessageBox.Show("IR=" + w.respnse[0]);
        }


    }
}
