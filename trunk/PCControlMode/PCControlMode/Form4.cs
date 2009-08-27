using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms;

namespace RobobuilderLib
{

    public partial class Form4 : Form
    {
        
        private System.Windows.Forms.HScrollBar[] servoPos;
        private System.Windows.Forms.TextBox[] servoID;
        private System.Windows.Forms.CheckBox[] readID;

        List<ServoPoseData> motiondata = new List<ServoPoseData>();

        public string filename = "";
        public string fn = "";
        int h;
        int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        wckMotion dcontrol;
        PCremote remote;

        public Form5 viewport;

        public Form4()
        {
            InitializeComponent();
            setup();
        }

        public void connect(PCremote r)
        {
            remote = r;


            if (r != null && r.serialPort1 != null && r.serialPort1.IsOpen)
            {
                dcontrol = new wckMotion(r);
                servoID_readservo();
            }
            else
            {
                if (viewport == null)
                {
                    MessageBox.Show("Must connect first");
                    return;
                }
            }
            this.Show();
        }

        public void disconnect()
        {
            if (dcontrol != null)dcontrol.close();
            dcontrol = null;
            remote = null;
            this.Hide();
        }

        private void setup()
        {
            servoPos = new System.Windows.Forms.HScrollBar[20];
            servoID = new System.Windows.Forms.TextBox[20];
            readID = new System.Windows.Forms.CheckBox[20];

            int cw = 10;

            for (int i = 0; i < sids.Length; i++)
            {
                servoPos[i] = new System.Windows.Forms.HScrollBar();
                servoID[i] = new System.Windows.Forms.TextBox();
                readID[i] = new System.Windows.Forms.CheckBox();

                // 
                // readID
                // 
                readID[i].AutoSize = true;
                readID[i].Location = new System.Drawing.Point(10 + 200 * (i / cw), 36 + 20 * (i % cw));
                readID[i].Name = "readID-" + i.ToString();
                readID[i].Size = new System.Drawing.Size(80, 17);
                readID[i].TabIndex = 36;
                readID[i].Text = "";
                readID[i].UseVisualStyleBackColor = true;
                readID[i].CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);

                // 
                // servoPos
                // 
                servoPos[i].Location = new System.Drawing.Point(28 + 200 * (i / cw), 33 + 20 * (i % cw));
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
                servoID[i].Location = new System.Drawing.Point(100 + 200 * (i / cw), 36 + 20 * (i % cw));
                servoID[i].Name = "servoID-" + i.ToString();
                servoID[i].ReadOnly = true;
                servoID[i].Size = new System.Drawing.Size(80, 20);
                servoID[i].TabIndex = 12;
                servoID[i].Text = sids[i].ToString();
                servoID[i].Visible = true;


                this.Controls.Add(servoID[i]);
                this.Controls.Add(servoPos[i]);
                this.Controls.Add(readID[i]);
            }

            h = 36 + 20 * (sids.Length + 2) + 10;
        }


        private void servoID_readservo()
        {
            dcontrol.servoID_readservo();

            for (int id = 0; id < sids.Length; id++)
            {
                servoPos[id].Value = dcontrol.pos[id];
                servoID[id].Text = sids[id].ToString() + " - " + servoPos[id].Value.ToString();

                if (viewport != null)
                {
                    viewport.setServoPos(sids[id], servoPos[id].Value);
                }
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.disconnect();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int id = Convert.ToInt32(((HScrollBar)sender).Name.Substring(9));
            int v = ((HScrollBar)sender).Value;
            Console.WriteLine("Id=" + sids[id] + ", V=" + v);

            servoID[id].Text = sids[id].ToString() + " - " + v;
            if (dcontrol != null) dcontrol.wckMovePos(sids[id], v, 2);
            if (viewport != null) viewport.setServoPos(sids[id], v);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((CheckBox)sender).Name.Substring(7));
            bool v = ((CheckBox)sender).Checked;
            Console.WriteLine("Id=" + sids[id] + ", V=" + v);

            if (viewport != null && viewport.Created)
                viewport.selectServo(id,v);

            if (dcontrol == null) return;

            if (v)
            {
                //set passive mode
                dcontrol.wckPassive(id);
                Console.WriteLine("Passive");
            }
            else
            {
                //unset passive mode
                if (dcontrol.wckReadPos(id))
                {
                    Console.WriteLine("Active");
                    int n = (int)dcontrol.respnse[1];
                    dcontrol.wckMovePos(id, n, 0);
                }
            }

            servoID_readservo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fn = textBox1.Text;
        }

        private void servo_select(object sender, EventArgs e)
        {
            Console.WriteLine("Servo - " + ((Label)(sender)).Text);
        }

        private void record_Click(object sender, EventArgs e)
        {
            servoID_readservo();

            ServoPoseData n = new ServoPoseData();
            n.Time = 500;
            n.Steps = 10;
            n.S0 = servoPos[0].Value;
            n.S1 = servoPos[1].Value;
            n.S2 = servoPos[2].Value;
            n.S3 = servoPos[3].Value;
            n.S4 = servoPos[4].Value;
            n.S5 = servoPos[5].Value;
            n.S6 = servoPos[6].Value;
            n.S7 = servoPos[7].Value;
            n.S8 = servoPos[8].Value;
            n.S9 = servoPos[9].Value;
            n.S10 = servoPos[10].Value;
            n.S11 = servoPos[11].Value;
            n.S12 = servoPos[12].Value;
            n.S13 = servoPos[13].Value;
            n.S14 = servoPos[14].Value;
            n.S15 = servoPos[15].Value;
            n.S16 = servoPos[16].Value;
            n.S17 = servoPos[17].Value;
            n.S18 = servoPos[18].Value;
            motiondata.Add(n);

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = motiondata;
            dataGridView1.Refresh();
        }

        private void playAll_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Play - motiondata line - " +
                dataGridView1.CurrentRow.Index);

            int n = 0;
            bool ff = true;

            foreach (ServoPoseData r in motiondata)
            {
                label2.Text = n.ToString();
                dataGridView1.CurrentCell = dataGridView1.Rows[n].Cells[0];
                dataGridView1.Refresh();

                byte[] t = new byte[19];
                t[0] = (byte)r.S0;
                t[1] = (byte)r.S1;
                t[2] = (byte)r.S2;
                t[3] = (byte)r.S3;
                t[4] = (byte)r.S4;
                t[5] = (byte)r.S5;
                t[6] = (byte)r.S6;
                t[7] = (byte)r.S7;
                t[8] = (byte)r.S8;
                t[9] = (byte)r.S9;
                t[10] = (byte)r.S10;
                t[11] = (byte)r.S11;
                t[12] = (byte)r.S12;
                t[13] = (byte)r.S13;
                t[14] = (byte)r.S14;
                t[15] = (byte)r.S15;
                t[16] = (byte)r.S16;
                t[17] = (byte)r.S17;
                t[18] = (byte)r.S18;

                dcontrol.PlayPose(r.Time, r.Steps, t, ff);
                if (ff) ff = false;
                
                n++;

                if (debugFlag.Checked)
                {
                    MessageBox.Show("Next " + n);
                }

            }
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            if (s.ShowDialog() == DialogResult.OK)
            {
                filename = s.FileName;
                fnstring.Text = filename;
            }
            else
                return;

            try
            {
                TextWriter tw = new StreamWriter(filename);

                tw.WriteLine("#V=01");
                tw.WriteLine("#T,N,0,1,2,3,4,5,6,7,8,9,10,11,12,1,3,14,15,16,17,18,X,Y,Z");

                foreach (ServoPoseData r in motiondata)
                {
                    tw.Write(r.Time + ",");
                    tw.Write(r.Steps + ",");
                    tw.Write(r.S0 + ",");
                    tw.Write(r.S1 + ",");
                    tw.Write(r.S2 + ",");
                    tw.Write(r.S3 + ",");
                    tw.Write(r.S4 + ",");
                    tw.Write(r.S5 + ",");
                    tw.Write(r.S6 + ",");
                    tw.Write(r.S7 + ",");
                    tw.Write(r.S8 + ",");
                    tw.Write(r.S9 + ",");
                    tw.Write(r.S10 + ",");
                    tw.Write(r.S11 + ",");
                    tw.Write(r.S12 + ",");
                    tw.Write(r.S13 + ",");
                    tw.Write(r.S14 + ",");
                    tw.Write(r.S15 + ",");
                    tw.Write(r.S16 + ",");
                    tw.Write(r.S17 + ",");
                    tw.Write(r.S18 + ",");
                    tw.Write(r.X + ",");
                    tw.Write(r.Y + ",");
                    tw.WriteLine(r.Z);
                }

                tw.Close();

                // ---------------------------

                string[] a1 = filename.Split('\\');
                string[] a2 = a1[a1.Length - 1].Split('.');
                //presets.update(a2[0]);

            }
            catch (Exception e1)
            {
                MessageBox.Show("Error - can't write to file - " + e1);
            }
        }

        private void loadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog s = new OpenFileDialog();
            if (s.ShowDialog() == DialogResult.OK)
            {
                filename = s.FileName;
                fnstring.Text = filename;
            }
            else
                return;

            int n = 0;
            motiondata.Clear();

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("#")) // comment
                        continue;

                    string[] r = line.Split(',');

                    if (r.Length > 2)
                    {
                        n++;
                        ServoPoseData t = new ServoPoseData();
                        t.Time = Convert.ToInt32(r[0]);
                        t.Steps = Convert.ToInt32(r[1]);
                        t.S0 = Convert.ToInt32(r[2]);
                        t.S1 = Convert.ToInt32(r[3]);
                        t.S2 = Convert.ToInt32(r[4]);
                        t.S3 = Convert.ToInt32(r[5]);
                        t.S4 = Convert.ToInt32(r[6]);
                        t.S5 = Convert.ToInt32(r[7]);
                        t.S6 = Convert.ToInt32(r[8]);
                        t.S7 = Convert.ToInt32(r[9]);
                        t.S8 = Convert.ToInt32(r[10]);
                        t.S9 = Convert.ToInt32(r[11]);
                        t.S10 = Convert.ToInt32(r[12]);
                        t.S11 = Convert.ToInt32(r[13]);
                        t.S12 = Convert.ToInt32(r[14]);
                        t.S13 = Convert.ToInt32(r[15]);
                        t.S14 = Convert.ToInt32(r[16]);
                        t.S15 = Convert.ToInt32(r[17]);
                        t.S16 = Convert.ToInt32(r[18]);
                        t.S17 = Convert.ToInt32(r[19]);
                        t.S18 = Convert.ToInt32(r[20]);

                        motiondata.Add(t);
                    }

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = motiondata;
                    dataGridView1.Refresh();
                }
                tr.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error - can't load file " + e1.Message);
                filename = "";
            }
            //dataGridView1.Rows[0].Selected = true;
        }

        private void updateRow_Click(object sender, EventArgs e)
        {
            //update current row
            if (dataGridView1.CurrentRow == null)
                record_Click(sender, e);

            int j = dataGridView1.CurrentRow.Index;

            Console.WriteLine("Update - motiondata line - " +j);
            servoID_readservo();

            ServoPoseData n = new ServoPoseData();
            n.Time = 500;
            n.Steps = 10;
            n.S0 = servoPos[0].Value;
            n.S1 = servoPos[1].Value;
            n.S2 = servoPos[2].Value;
            n.S3 = servoPos[3].Value;
            n.S4 = servoPos[4].Value;
            n.S5 = servoPos[5].Value;
            n.S6 = servoPos[6].Value;
            n.S7 = servoPos[7].Value;
            n.S8 = servoPos[8].Value;
            n.S9 = servoPos[9].Value;
            n.S10 = servoPos[10].Value;
            n.S11 = servoPos[11].Value;
            n.S12 = servoPos[12].Value;
            n.S13 = servoPos[13].Value;
            n.S14 = servoPos[14].Value;
            n.S15 = servoPos[15].Value;
            n.S16 = servoPos[16].Value;
            n.S17 = servoPos[17].Value;
            n.S18 = servoPos[18].Value;

            motiondata[j] = n;

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = motiondata;
            dataGridView1.Refresh();
        }

        private void playRow_Click(object sender, EventArgs e)
        {
            // play current line 

            int j;

            if (dataGridView1.CurrentRow == null) return;
            
            j = dataGridView1.CurrentRow.Index;

            Console.WriteLine("Play - motiondata line - " + j);

            ServoPoseData r = motiondata[j];

            byte[] t = new byte[19];
            t[0] = (byte)r.S0;
            t[1] = (byte)r.S1;
            t[2] = (byte)r.S2;
            t[3] = (byte)r.S3;
            t[4] = (byte)r.S4;
            t[5] = (byte)r.S5;
            t[6] = (byte)r.S6;
            t[7] = (byte)r.S7;
            t[8] = (byte)r.S8;
            t[9] = (byte)r.S9;
            t[10] = (byte)r.S10;
            t[11] = (byte)r.S11;
            t[12] = (byte)r.S12;
            t[13] = (byte)r.S13;
            t[14] = (byte)r.S14;
            t[15] = (byte)r.S15;
            t[16] = (byte)r.S16;
            t[17] = (byte)r.S17;
            t[18] = (byte)r.S18;

            if (dcontrol != null) dcontrol.PlayPose(r.Time, r.Steps, t, true);
            //if (sim_mode && viewport != null) viewport.se

        }

        private void setBasic_Click(object sender, EventArgs e)
        {
            // set basic pose !
            dcontrol.BasicPose(1000, 10);
            servoID_readservo();
        }

        private void queryValues_Click(object sender, EventArgs e)
        {
            //read and load current servo positions
            servoID_readservo();
        }

        private void delRow_Click(object sender, EventArgs e)
        {
            // delete current row
            if (dataGridView1.CurrentRow == null)
                return;

            int j = dataGridView1.CurrentRow.Index;

            Console.WriteLine("Delete - motiondata line - " + j);

            motiondata.RemoveAt(j);

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = motiondata;
            dataGridView1.Refresh();
        }

        private void autopose_Click(object sender, EventArgs e)
        {
            // every x seconds write to array current possitons
            Int16 x, y, z;

            if (autopose.Text == "Stop")
            {
                autopose.Text = "Auto";
                return;
            }

            autopose.Text = "Stop";

            while (autopose.Text == "Stop")
            {
                System.Threading.Thread.Sleep(2000);  //2 s
                record_Click(null, null);

                Console.WriteLine(remote.readXYZ(out x, out y, out z));
                xV.Text = "X=" + x.ToString();
                yV.Text = "Y=" + y.ToString();
                zV.Text = "Z=" + z.ToString();

                Application.DoEvents();
            }
        }

        private void rAcc_Click(object sender, EventArgs e)
        {
            Int16 x, y, z;
            // read acceleromter
            Console.WriteLine(remote.readXYZ(out x, out y, out z));
            xV.Text = "X=" + x.ToString();
            yV.Text = "Y=" + y.ToString();
            zV.Text = "Z=" + z.ToString();
        }

        private void all_pass_chk_CheckedChanged(object sender, EventArgs e)
        {
            for (int id = 0; id < sids.Length; id++)
            {
                if (dcontrol == null) return;

                if (all_pass_chk.Checked)
                {
                    //set passive mode
                    dcontrol.wckPassive(sids[id]);
                    Console.WriteLine("Passive");
                }
                else
                {
                    //unset passive mode
                    if (dcontrol.wckReadPos(sids[id]))
                    {
                        Console.WriteLine("Active");
                        int n = (int)dcontrol.respnse[1];
                        dcontrol.wckMovePos(sids[id], n, 0);
                    }
                }
            }

        }

    }

    class ServoPoseData
    {
        public int Time { get; set; }
        public int Steps { get; set; }
        public int S0 { get; set; }
        public int S1 { get; set; }
        public int S2 { get; set; }
        public int S3 { get; set; }
        public int S4 { get; set; }
        public int S5 { get; set; }
        public int S6 { get; set; }
        public int S7 { get; set; }
        public int S8 { get; set; }
        public int S9 { get; set; }
        public int S10 { get; set; }
        public int S11 { get; set; }
        public int S12 { get; set; }
        public int S13 { get; set; }
        public int S14 { get; set; }
        public int S15 { get; set; }
        public int S16 { get; set; }
        public int S17 { get; set; }
        public int S18 { get; set; }
        public Int16 X { get; set; }
        public Int16 Y { get; set; }
        public Int16 Z { get; set; }
    }

}
