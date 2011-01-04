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

    }
}
