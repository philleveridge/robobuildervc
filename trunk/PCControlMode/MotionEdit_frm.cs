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

    public partial class MotionEdit_frm : Form
    {
        private System.Windows.Forms.HScrollBar[] servoPos;
        private System.Windows.Forms.TextBox[] servoID;
        private System.Windows.Forms.CheckBox[] readID;

        List<ServoPoseData> motiondata = new List<ServoPoseData>();

        public string filename = "";
        public string fn = "";

        wckMotion dcontrol;
        PCremote remote;

        public Display3D_frm viewport;

        public MotionEdit_frm()
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
                test_servos();
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

            for (int i = 0; i < wckMotion.MAX_SERVOS; i++)
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
                servoID[i].Text = i.ToString();
                servoID[i].Visible = true;

                this.Controls.Add(servoID[i]);
                this.Controls.Add(servoPos[i]);
                this.Controls.Add(readID[i]);
            }

            //h = 36 + 20 * (wckMotion.MAX_SERVOS + 2) + 10;
            servoStatus1.setCallback_event(new callBack(show_Info));
            torq_list.SelectedIndex = 2;
        }

        private void show_Info(int id)
        {
            string m = "Info=" + id;
            if (dcontrol == null) return;

            if (dcontrol.wckReadBoundary(id))
            {
                m += string.Format(" UL=[{0:0},{1:0}]", dcontrol.respnse[0], dcontrol.respnse[1]);
            }

            if (dcontrol.wckReadPDgain(id))
            {
                m += string.Format(" PDI=[{0:0},{1:0},", dcontrol.respnse[0], dcontrol.respnse[1]);
            }

            if (dcontrol.wckReadIgain(id))
            {
                m += string.Format("{0:0}]", dcontrol.respnse[0]);
            }

            if (dcontrol.wckReadSpeed(id))
            {
                m += string.Format(" Speed=[{0:0},{1:0}]", dcontrol.respnse[0], dcontrol.respnse[1]);
            }

            if (dcontrol.wckReadOverload(id))
            {
                m += string.Format(" OverT=[{0:0}]", dcontrol.respnse[0]);
            }

            label1.Text = m;
        }

        private void test_servos()
        {
            for (int id = 0; id < wckMotion.MAX_SERVOS; id++)
            {
                if (dcontrol.wckReadPos(id))                 //readPOS (servoID)
                {
                    if (dcontrol.respnse[1] < 255)
                    {
                        servoPos[id].Value = dcontrol.respnse[1];
                        servoID[id].Text = id + " - " + servoPos[id].Value.ToString();
                        servoStatus1.setStatus(id, true);
                    }
                }
                else
                {
                    Console.WriteLine("Servo Id {0:0} not connected", id);
                    servoStatus1.setStatus(id, false);
                    servoPos[id].Enabled = false;
                    servoID[id].Enabled = false;
                    readID[id].Enabled = false;
                }
            }
        }


        private void servoID_readservo()
        {
            if (dcontrol == null)
            {
                if (viewport != null)
                {
                    for (int id = 0; id < wckMotion.MAX_SERVOS; id++)
                    {
                        servoPos[id].Value = viewport.getServoPos(id);
                        servoID[id].Text = id.ToString() + " - " + servoPos[id].Value.ToString();
                    }
                }
                return;
            }

            //dcontrol.servoID_readservo();

            for (int id = 0; id < wckMotion.MAX_SERVOS; id++)
            {
                if (servoStatus1.getStatus(id))
                {
                    if (dcontrol.wckReadPos(id))                 //readPOS (servoID)
                    {
                        servoPos[id].Value = dcontrol.respnse[1];
                        servoID[id].Text = id.ToString() + " - " + servoPos[id].Value.ToString();

                        if (viewport != null)
                        {
                            viewport.setServoPos(id, servoPos[id].Value);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Skip Id {0:0} not connected", id);
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
            Console.WriteLine("Id=" + id + ", V=" + v);

            if (dcontrol != null)
            {
                int t = torq_list.SelectedIndex;
                Console.WriteLine("t=" + t);
                dcontrol.wckMovePos(id, v, t);

                Console.WriteLine("Return = {0:0} {1:0}", dcontrol.respnse[0], dcontrol.respnse[1]);

                v = dcontrol.respnse[1]; // actual position returned by move
            }
            if (viewport != null) 
                viewport.setServoPos(id, v);

            servoID[id].Text = id.ToString() + " - " + v;
            ((HScrollBar)sender).Value = v;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((CheckBox)sender).Name.Substring(7));
            bool v = ((CheckBox)sender).Checked;
            Console.WriteLine("Id=" + id + ", V=" + v);

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

        private void servo_select(object sender, EventArgs e)
        {
            Console.WriteLine("Servo - " + ((Label)(sender)).Text);
        }

        private void record_Click(object sender, EventArgs e)
        {
            Int16 x,y,z;
            servoID_readservo();

            ServoPoseData n = new ServoPoseData();
            n.Time = 500;
            n.Steps = 10;
            for (int i = 0; i < 19; i++)
                n.S[i] = servoPos[i].Value;

            Console.WriteLine(remote.readXYZ(out x, out y, out z));
            xV.Text = "X=" + x.ToString();
            yV.Text = "Y=" + y.ToString();
            zV.Text = "Z=" + z.ToString();

            n.X = x;
            n.Y = y; 
            n.Z = z;

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
                for (int i = 0; i < 19; i++)
                    t[i] = (byte)r.S[i];

                if (dcontrol != null) dcontrol.PlayPose(r.Time, r.Steps, t, ff);
                if (viewport != null) viewport.PlayPose(r.Time, r.Steps, t, ff);

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
            s.Filter = "Motion file (*.csv)|*.csv";
            if (s.ShowDialog() == DialogResult.OK)
            {
                filename = s.FileName;
                fnstring.Text = Preset_frm.convname(filename);
            }
            else
                return;

            try
            {
                TextWriter tw = new StreamWriter(filename);

                tw.WriteLine("#V=01,N={0},A=1", 19 );
                tw.WriteLine("#T,N,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,X,Y,Z");

                foreach (ServoPoseData r in motiondata)
                {
                    tw.Write(r.Time + ",");
                    tw.Write(r.Steps + ",");

                    for (int i = 0; i < 19; i++)
                        tw.Write(r.S[i] + ",");

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
            s.Filter = "Motion file (*.csv)|*.csv|RoboBuilderMotion (*.rbm)|*.rbm";
            if (s.ShowDialog() == DialogResult.OK)
            {
                filename = s.FileName;
                fnstring.Text = Preset_frm.convname(filename);
            }
            else
                return;

            int n = 0;
            motiondata.Clear();

            if (filename.EndsWith("rbm"))
            {
                // load r bm file and convert from 16 servos to 19

                RobobuilderVC.Motion m = new RobobuilderVC.Motion();
                m.LoadFile(filename);

                int[] diff =new int[m.no_servos];
                for (int delta = 0; delta < m.no_servos; delta++)
                {
                    if (delta<wckMotion.MAX_SERVOS) 
                        diff[delta] = wckMotion.basic_pos[delta] - (int)m.scenes[0].mPositions[delta];
                    else
                        diff[delta] = 0;
                }

                for (int i = 0; i < m.no_scenes; i++)
                {
                    ServoPoseData t = new ServoPoseData();

                    t.Time = (int)m.scenes[i].TransitionTime;
                    t.Steps = (int)m.scenes[i].Frames;

                    for (int k = 0; k < m.no_servos; k++)
                    {
                        t.S[k] = diff[k] + (int)m.scenes[i].mPositions[k];
                    }

                    if (m.no_servos < 17)
                        t.S16 =  125;
                    if (m.no_servos < 18)
                        t.S17 = 125;
                    if (m.no_servos < 19)
                        t.S18 = 127;

                    t.X   = 0;
                    t.Y   = 0;
                    t.Z   = 0;
                    motiondata.Add(t);
                }

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = motiondata;
                dataGridView1.Refresh();
                return;
            }

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

                        for (int i = 0; i < 19; i++)
                            t.S[i] = Convert.ToInt32(r[i+2]);

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

            for (int i = 0; i < 19; i++) 
                n.S[i] = servoPos[i].Value;

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

            for (int i = 0; i < 19; i++) 
                t[i] = (byte)r.S[i];

            if (dcontrol != null) 
                dcontrol.PlayPose(r.Time, r.Steps, t, true);
            if (viewport != null) 
                viewport.PlayPose(r.Time, r.Steps, t, true);

            if (dataGridView1.CurrentRow.Index < dataGridView1.RowCount - 1)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index + 1].Cells[0];
            }
            else
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            }
            dataGridView1.Refresh();
        }

        private void setBasic_Click(object sender, EventArgs e)
        {
            // set basic pose !
            if (dcontrol != null) dcontrol.BasicPose(1000, 10);
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
            for (int id = 0; id < wckMotion.MAX_SERVOS; id++)
            {
                if (dcontrol == null || !servoStatus1.getStatus(id)) return;

                if (all_pass_chk.Checked)
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
            }

        }

    }

    class ServoPoseData
    {
        public int Time { get; set; }
        public int Steps { get; set; }
        public int S0 { get { return S[0]; } set { S[0] = value; } }
        public int S1 { get { return S[1]; } set { S[1] = value; } }
        public int S2 { get { return S[2]; } set { S[2] = value; } }
        public int S3 { get { return S[3]; } set { S[3] = value; } }
        public int S4 { get { return S[4]; } set { S[4] = value; } }
        public int S5 { get { return S[5]; } set { S[5] = value; } }
        public int S6 { get { return S[6]; } set { S[6] = value; } }
        public int S7 { get { return S[7]; } set { S[7] = value; } }
        public int S8 { get { return S[8]; } set { S[8] = value; } }
        public int S9 { get { return S[9]; } set { S[9] = value; } }
        public int S10 { get { return S[10]; } set { S[10] = value; } }
        public int S11 { get { return S[11]; } set { S[11] = value; } }
        public int S12 { get { return S[12]; } set { S[12] = value; } }
        public int S13 { get { return S[13]; } set { S[13] = value; } }
        public int S14 { get { return S[14]; } set { S[14] = value; } }
        public int S15 { get { return S[15]; } set { S[15] = value; } }
        public int S16 { get { return S[16]; } set { S[16] = value; } }
        public int S17 { get { return S[17]; } set { S[17] = value; } }
        public int S18 { get { return S[18]; } set { S[18] = value; } }
        public int S19 { get { return S[19]; } set { S[19] = value; } }
        public int[] S = new int[20];
        public Int16 X { get; set; }
        public Int16 Y { get; set; }
        public Int16 Z { get; set; }
    }

}
