﻿#define HOMEBREW
#undef HOMEBREW

using System;
using System.Windows.Forms;
using System.IO;


namespace RobobuilderLib
{

    public partial class Main : Form
    {
        Preset_frm presets = new Preset_frm();
        Video_frm videoc = new Video_frm();
        MotionEdit_frm medit = new MotionEdit_frm();
        Display3D_frm view = null;
        Basic_frm bc = new Basic_frm();

        PCremote pcR;

        public Main()
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
                        case "BASIC":
                            string bp = nvp[1]; // filename of basic pose
                            break;
                        case "DEBUG":
                            break;
                        case "COM":
                            serialPort1.PortName = nvp[1];
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

                string sn = pcR.readSN();
                label1.Text = "Firmware=" + v + ", S/N=" + sn;

                // check Firmware - and if Homebrew enable Basic
                if (sn.Substring(0,2)=="HB") 
                {
                    button3.Visible = true;
                    toolStripMenuItem1.Visible = true;
                }

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
            button4.Enabled = f;
            button8.Enabled = f;
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
            textBox1.AppendText(pcR.readZeros() +"\r\n");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // read IR - not working
            button2.Enabled = false;
            pcR.readIR(10000, new callBack(addMessage));
            button2.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // read Button - not working
            button4.Enabled = false;
            pcR.readButton(10000, new callBack(addMessage));
            button4.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // read Soundlevel - not working
            button8.Enabled = false;
            pcR.readsoundLevel(10000, 1, new callBack(addMessage));
            button8.Enabled = false;
        }

        public void addMessage()
        {
            textBox1.AppendText(pcR.message + "\r\n");
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
            presets.connect(pcR);
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
            if (view == null) view = new Display3D_frm();
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

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            bc.pcr = pcR; //
            bc.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // homebrew test

            wckMotion test = new wckMotion(pcR);

            if (test.wckReadPos(0))
            {
                textBox1.AppendText("Test =" + test.respnse[0].ToString() + test.respnse[1].ToString() + "\r\n");
            }
            else
            {
                textBox1.AppendText("Test Failed\r\n");
            }

#if HOMEBREW

            pcR.btf.send_msg_basic('q');
            if (pcR.btf.recv_packet())
            {
                textBox1.AppendText("Query Test\r\n" +
                     "0=" + pcR.btf.buff[0].ToString() + " " + pcR.btf.buff[1].ToString() + "\r\n" +
                     "1=" + pcR.btf.buff[2].ToString() + " " + pcR.btf.buff[3].ToString() + "\r\n" +
                     "2=" + pcR.btf.buff[4].ToString() + " " + pcR.btf.buff[5].ToString() + "\r\n" +
                     "3=" + pcR.btf.buff[6].ToString() + " " + pcR.btf.buff[7].ToString() + "\r\n" +
                     "4=" + pcR.btf.buff[8].ToString() + " " + pcR.btf.buff[9].ToString() + "\r\n" +
                     "5=" + pcR.btf.buff[10].ToString() + " " + pcR.btf.buff[11].ToString() + "\r\n"
                    );
            }
            else
            {
                textBox1.AppendText("Test Failed\r\n");
            }
#endif

            test.close();
            test = null;
        }



    }
}
