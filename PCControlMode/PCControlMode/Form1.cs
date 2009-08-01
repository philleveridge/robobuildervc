﻿using System;
using System.Windows.Forms;
using System.IO;

namespace RobobuilderLib
{
    public partial class Form1 : Form
    {
        Form2 presets = new Form2();
        Form3 videoc = new Form3();
        Form4 medit = new Form4();
        Form5 view = null;
        PCremote pcR;

        public Form1()
        {
            InitializeComponent();

            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 115200;
            serialPort1.ReadTimeout = 3000;

            listBox1.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                listBox1.Items.Add(s);
            }

            label1.Text = "Disconnected";
            loadconfig();

            presets.sp1 = serialPort1;
        }

        void loadconfig()
        {
            try
            {
                string line;
                TextReader tr = new StreamReader("default.ini");
                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    string[] nvp = line.Split('=');
                    string[] v;
                    switch (nvp[0])
                    {
                        case "BUTTON":
                            presets.update(nvp[1]); //
                            break;
                        case "DISPLAY":
                            break;
                        case "DEBUG":
                            break;
                        case "COM":
                            break;
                        case "VIDEO":
                            break;
                    }
                }
                tr.Close();
            }
            catch (Exception e1)
            {
                Console.WriteLine("?Can't open default.ini");
            }
            presets.presets_flg = false; // now exit will ask if any further changes
        }

        void saveconfig()
        {
            try
            {
                TextWriter tw = new StreamWriter("default.ini"); //overwrite if exists
                tw.WriteLine("#Autogenerated");
                string t = presets.list_presets();
                tw.WriteLine(t);
                tw.Close();
            }
            catch (Exception e1)
            {
                Console.WriteLine("?Can't write to default.ini");
            }
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
                listBox1.Enabled = true;

                label1.Text = "Disconnected";
                pcR = null;

            }
            else
            {
                serialPort1.Open();

                if (pcR == null) pcR = new PCremote(serialPort1);

                // start up on connect
                string v = pcR.readVer();
                if (v == "")
                {
                    serialPort1.Close();
                    label1.Text = "Failed to connect";
                    textBox1.AppendText(pcR.message);
                    pcR = null;
                    return;
                }

                label1.Text = "Firmware=" + v + ", S/N=" + pcR.readSN();

                connect.Text = "Close";
                listBox1.Enabled = false;
                set_buttons(true);
                textBox1.Text = "";
            }
        }

        private void clr_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void set_buttons(bool f)
        {
            button1.Enabled = f;
            button2.Enabled = f;
            button5.Enabled = f;
            button6.Enabled = f;
            button7.Enabled = f;
            button9.Enabled = f;
            button10.Enabled = f;
        }

        /**********************************************
         * 
         * Action buttons  - Remote / serial prorocol
         * 
         * ********************************************/

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Ver=" + pcR.readVer() +"\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("S/N="+pcR.readSN()+"\r\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // read distance
            textBox1.AppendText("Distance=" + pcR.readDistance() + "cm\r\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Reset mem - " + pcR.resetMem());
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //read XYZ
            Int16 x,y,z;
            textBox1.AppendText(pcR.readXYZ(out x, out y, out z) + "\r\n");
        }
        
        private void button7_Click(object sender, EventArgs e)
        {
            // avail mem
            textBox1.AppendText(pcR.availMem() + "\r\n");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //read zeros
            textBox1.AppendText(pcR.readZeros() + "\r\n");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // read IR - not working
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.AppendText(pcR.zeroHuno() + "\r\n");
        }

        /**********************************************
         * 
         * Event routines
         * 
         * change of serial port
         * 
         * ********************************************/

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
                serialPort1.PortName = listBox1.Items[listBox1.SelectedIndex].ToString();
        }

        private void s0_Click(object sender, EventArgs e)
        {
            Console.WriteLine(((Label)sender).Text);
        }

        private void presetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presets.Show();
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            videoc.Show();
        }

        private void motionEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            medit.connect(pcR);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (presets.presets_flg == false)
            {
                this.Close();
            }

            switch (MessageBox.Show("Update default.ini?", "exit", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    saveconfig();
                    this.Close();
                    break;
                case DialogResult.No:
                    this.Close();
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        private void viewModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (view == null) view = new Form5();
            view.Show();
            medit.viewport = view;

            while (view.Created)
            {
                view.render();
                Application.DoEvents();
            }
            view = null;
            medit.viewport = null;
        }
    }
}
