using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections;
using LSharp;

namespace RobobuilderLib
{
    public partial class Preset_frm : Form
    {
        const int MAXBUTTONS = 15;
        const int MAXDEPTH = 5;

        public string button_dir = "";
        const string button_fmt = "*.csv";

        private System.Windows.Forms.Button[] button_array;
        private string[] fnames;
        int cnt;
        double k = 1.0; //speed factor
        public bool presets_flg;

        public int video_obj_loc;

        PCremote  remote;
        wckMotion wckm;
        Runtime   runtime;

        bool script_active = false;

        PCremote.RemoCon ir_val = PCremote.RemoCon.FAILED;

        public Preset_frm()
        {
            InitializeComponent();

            button_dir = Directory.GetCurrentDirectory();
        }

        private void setupLisp()
        {
            runtime = new Runtime(System.Console.In, System.Console.Out, System.Console.Error);

            runtime.GlobalEnvironment.Set(Symbol.FromName("form"),  this);
            runtime.GlobalEnvironment.Set(Symbol.FromName("pcr"),   remote);
            if (remote != null) runtime.GlobalEnvironment.Set(Symbol.FromName("sport"), remote.serialPort1);
            runtime.GlobalEnvironment.Set(Symbol.FromName("wck"), wckm);
            runtime.EvalString("(load \"init.lisp\")");
            //Console.WriteLine(runtime.EvalString("(map show-doc environment)"));
        }

        public void connect(PCremote r)
        {
            remote = r;
            build_buttons();
            this.Show(); 
            
            if (r != null && r.serialPort1 != null && r.serialPort1.IsOpen)
            {
                wckm = new wckMotion(r);
            }
            else
            {
                MessageBox.Show("Must connect first");
            }
            setupLisp();
        }

        public void disconnect()
        {
            if (wckm != null)
            {
                wckm.close();
                wckm = null;
                remote = null;
            }
            this.Hide();
        }

        public string list_presets()
        {
            string r = "";
            for (int i = 0; i < cnt; i++)
            {
                r += "BUTTON=" + button_array[i].Text + "," + fnames[i] + "\r\n";
            }
            return r;
        }

        public int check(string name)
        {
            for (int i = 0; i < cnt; i++)
            {
                if (button_array[i].Text.Equals(name,StringComparison.InvariantCultureIgnoreCase)) return i;
            }
            return -1;
        }

        static public string convname(string n)
        {
            string t = n.Substring(1 + n.LastIndexOf('\\'));
            t = t.Substring(0, t.LastIndexOf('.'));
            t = t.ToLower();
            t = t.Substring(0, 1).ToUpper() + t.Substring(1);
            return t;
        }

        public void build_buttons()
        {
            button_array = new Button[MAXBUTTONS];
            fnames = new string[MAXBUTTONS];
            cnt = 0;
            update("Basic","");

            Console.WriteLine(button_dir);
            string[] s = Directory.GetFileSystemEntries(button_dir, button_fmt);
            foreach (string n in s)
            {
                string t = convname(n);
                Console.WriteLine(t);
                update(t,n);
            }

            s = Directory.GetFileSystemEntries(button_dir, "*.lisp");
            foreach (string n in s)
            {
                string t = convname(n);
                Console.WriteLine(t);
                //update(t + "," + n);
                string p = File.ReadAllText(n);
                update(t, "L:" + p);
            }

            presets_flg = false;
        }

        public void update(string name, string arg)
        {
            // 
            // add 'unique' button
            // 


            if (check(name) >= 0) return; // already exists

            fnames[cnt] = arg;


            int cols = 3;

            button_array[cnt] = new Button();

            button_array[cnt].Location = new System.Drawing.Point(10 + 80 * (cnt % cols), 10 + 40 * (cnt / cols));
            button_array[cnt].Name = "button-" + cnt;
            button_array[cnt].Size = new System.Drawing.Size(53, 27);
            button_array[cnt].TabIndex = 1;
            button_array[cnt].Text = name;
            button_array[cnt].UseVisualStyleBackColor = true;
            button_array[cnt].Click += new System.EventHandler(this.button_Click);

            if (fnames[cnt].StartsWith("L:"))
                button_array[cnt].ForeColor = System.Drawing.Color.Green; 
            
            this.Controls.Add(button_array[cnt]);

            if (cnt<MAXBUTTONS-1) 
                cnt++;

            presets_flg=true;
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            disconnect();
        }

        private void NewBasicPose()
        {
            if (wckm != null) wckm.BasicPose(1000, 10);
        }

        private void play(string filename)
        {
            // play
            int n = 0;
            bool ff = true;
            bool stepflg = dbg_flg.Checked;

            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    n++;

                    line = line.Trim();
                    if (line.StartsWith("#")) // comment
                    {
                        Console.WriteLine(n + ": " + line);
                        continue;
                    }

                    string[] r = line.Split(',');

                    int l = r.Length;

                    if (r.Length > 5)
                    {
                        byte[] t = new byte[r.Length - 5];  //add because includes XYZ now

                        for (int i = 2; i < r.Length-3; i++)
                        {
                            if (r[i] != "")
                                t[i - 2] = Convert.ToByte(r[i]);
                        }

                        wckm.PlayPose((int)(((double)Convert.ToInt32(r[0]))*k), Convert.ToInt32(r[1]), t, ff);
                        if (ff) ff = false;

                        if (stepflg)
                        {
                            MessageBox.Show("Next " + n);
                        }
                    }

                }
                tr.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Error - can't load file " + e1.Message);
            }
        }

        private void Message(string x)
        {
            output_txt.Text = x;
            mssage_txt.Text = x;
            Console.WriteLine(x);
        }

        private PCremote.RemoCon readIR()
        {
            PCremote.RemoCon r;
            while (ir_val == PCremote.RemoCon.FAILED) { Application.DoEvents(); }
            r = ir_val;
            ir_val = PCremote.RemoCon.FAILED;
            return r;
        }

        private void setKfactor(double kf)
        {
            // kfactor val (speed up or slow down)
            k = kf;
            vScrollBar1.Value = (int)(k * 100f);
            label1.Text = k.ToString();
        }

        private double getKfactor()
        {
            // kfactor val (speed up or slow down)
            return k;
        }

        private int readVideo()
        {
            while (video_obj_loc == 0) { Application.DoEvents(); }
            return video_obj_loc;
        }

        private void wait(int t)
        {                               // wait x ms
            for (int n = 0; n < t / 50; n++)
            {
                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
                if (script_active == false) break;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (script_active) return;

            if (checkBox2.Checked && action.Text == "")
            {
                if (!script.Text.EndsWith("\r\n")) script.Text += "\r\n";
                script.Text += ((Button)sender).Text + "\r\n" ;
            }

            if (((Button)sender).Name == "button-0")
            {
                NewBasicPose();
            }
            else
            {
                int i = Convert.ToInt32(((Button)sender).Name.Substring(7));
                Console.WriteLine("Name = " + ((Button)sender).Name + "=" + ((Button)sender).Text +"-"+ fnames[i]);

                if (fnames[i].StartsWith("L:"))
                {
                    //load into editor
                    string c = fnames[i].Substring(2);

                    action.Text = ((Button)sender).Text;
                    script.Text = c;
                }
                else 
                {
                    play(fnames[i]);
                }
            }
        }

        private void run_btn_Click(object sender, EventArgs e)
        {
            // run LISP script

            run_script(script.Text);
        }

        private void run_script(string c)
        {
            if (run_btn.Text == "Stop")
            {
                run_btn.Text = "Run";
                run_btn.BackColor = System.Drawing.SystemColors.ControlLight;
                script_active = false;
                return;
            }

            if (c == "")
            {
                MessageBox.Show("No script");
                return;
            }

            run_btn.Text = "Stop";
            run_btn.BackColor = System.Drawing.Color.Red;

            try
            {
                script_active = true;
                object result = runtime.EvalString(script.Text);
                output_txt.Text = (result == null) ? "null" : result.ToString();
            }
            catch (Exception t)
            {
                output_txt.Text = t.Message;
            }

            run_btn.Text = "Run";
            run_btn.BackColor = System.Drawing.SystemColors.ControlLight;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            // speed up or slow down
            k=(double)(vScrollBar1.Value) / 100;
            label1.Text = k.ToString();
        }

        private void store_btn_Click(object sender, EventArgs e)
        {
            //store 
            string c = script.Text;
            string n = action.Text;

            if (n == "") 
            {
                MessageBox.Show("No name");
                return;
            } 
            
            int i = check(n);
            //c = c.Replace("\r\n", ";");
            Console.WriteLine("Name=" + n + " Script=" + c);

            File.WriteAllText(button_dir + "\\" + n + ".lisp", c);

            if (i < 0)
            {
                update(n, "L:" + c);  // new button
            }
            else
            {
                fnames[i] = "L:" + c; // update script against existing button
            }
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            //FAILED=0,
            //A=0x01,B,LeftTurn,Forward,RightTurn,Left,Stop,Right,Punch_Left,Back,Punch_Right,
            //N1,N2,N3,N4,N5,N6,N7,N8,N9,B0,
            string[] spots = new string[] 
            {
            "A,43,29,54,42",
            "B,111,28,121,45",
            "",
            "F,76,42,87,63",
            "",
            "L,38,79,51,93",
            "@,73,81,87,94",
            "R,111,78,127,93",
            "",
            "B,75,110,88,129",
            "",
            "1,48,148,59,160",
            "2,75,149,89,160",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0",
            "*,45,226,58,236",      // need to rememebr
            "#,105,225,118,235"     // need to rememebr
            };

            int ir;
            string h = "";

            for (ir=0; ir<spots.Length; ir++)
            {
                string[] r = spots[ir].Split(',');
                if (r.Length == 5)
                {
                    if ((e.X > Convert.ToInt32(r[1])) && (e.X < Convert.ToInt32(r[3])) && (e.Y > Convert.ToInt32(r[2])) && (e.Y < Convert.ToInt32(r[4])))
                    {
                        h = r[0];
                        break;
                    }
                }
            }
            Console.WriteLine("Mouse - " + e.X + "," + e.Y + " : " + h);

            if (script_active)
            {
                if (ir < spots.Length)
                    ir_val = (PCremote.RemoCon)(ir+1);
                else
                    ir_val = PCremote.RemoCon.FAILED;
                return;
            }

            if (h == "@")
            {
                NewBasicPose(); // Red Button
            }
            else
            {
                int i = check(h);  // button defined
                if (i >= 0)
                {
                    if (fnames[i].StartsWith("L:"))
                    {
                        //run script
                        run_script(fnames[i].Substring(2));
                    }
                    else
                    {
                        play(fnames[i]);
                    }
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Visible = checkBox2.Checked;
            if (panel1.Visible)
            {
                action.Text = "";
                script.Text = "";
            }
        }
        
    }
}
