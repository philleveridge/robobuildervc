﻿using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections;

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

        PCremote remote;
        wckMotion wckm;

        public Preset_frm()
        {
            InitializeComponent();

            button_dir = Directory.GetCurrentDirectory();
            //build_buttons();
        }

        public void connect(PCremote r)
        {
            this.Show();

            remote = r;
            if (r != null && r.serialPort1 != null && r.serialPort1.IsOpen)
            {
                wckm = new wckMotion(r);
                build_buttons();

                this.Show();
            }
            else
            {
                MessageBox.Show("Must connect first");
                return;
            }
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

            s = Directory.GetFileSystemEntries(button_dir, "*.txt");
            foreach (string n in s)
            {
                string t = convname(n);
                Console.WriteLine(t);
                //update(t + "," + n);
                string p = File.ReadAllText(n);
                update(t, "S:" + p);
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

            if (fnames[cnt].StartsWith("S:")) 
                button_array[cnt].ForeColor = System.Drawing.Color.Red;

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
            wckm.BasicPose(1000, 10);
        }

        private void play(string filename)
        {
            // play
            int n = 0;
            bool ff = true;
            bool stepflg = checkBox1.Checked;

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

                    int l = r.Length;

                    if (r.Length > 2)
                    {
                        n++;

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


        Hashtable vars;

        private string evalExpr(string x)
        {
            //
            IDictionaryEnumerator en = vars.GetEnumerator();

            while (en.MoveNext())
            {
                string k = en.Key.ToString();
                string v = en.Value.ToString();
                x = x.Replace("$" + k, v);
            }

            if (x.StartsWith("\"") && x.EndsWith("\""))
                x = x.Substring(1,x.Length-2);
            else
                x = evalNumeric(x).ToString();

            return x;
        }

        private int evalNumeric(string x)
        {
            //
            int r = 0;
            int t = 0;
            int i = 0;
            int v=0;
            char op='+';
            while (i < x.Length)
            {
                if (x[i] == ' ') continue;
                if (x[i] >= '0' && x[i] <= '9')
                {
                    r = r * 10 + (x[i] - '0');
                }
                if (x[i] == '+' || x[i] == '-' || x[i] == '=' || x[i] == '<' || x[i] == '>' || x[i] == '!')
                {
                    t = r;
                    r = 0;
                    op = x[i];
                }
                i = i + 1;
            }
            switch (op)
            {
                case '+':
                    v = t + r;
                    break;
                case '-':
                    v = t - r;
                    break;
                case '=':
                    v = (t == r) ?1 : 0;
                    break;
                case '<':
                    v = (t < r) ? 1 : 0;
                    break;
                case '>':
                    v = (t > r) ? 1 : 0;
                    break;
                case '!':
                    v = (t != r) ? 1 : 0;
                    break;
            }
            return v;
        }

        private void run(string script)
        {
            Console.WriteLine("Script=" + script);
            int[] loop_l = new int[MAXDEPTH];
            int[] loop_c = new int[MAXDEPTH];

            vars = new Hashtable();

            int lc = 0;

            bool ifcond = true;

            string[] sc = script.Split(';');
            for (int i = 0; i < sc.Length; i++)
            {
                string line = sc[i];
                Console.WriteLine(i + " " + sc[i]);
                string[] words = line.Split(' ');

                if (ifcond == false && words[0] != "else")
                    continue;

                switch (words[0].ToLower())
                {
                    case "getacc":
                        {
                            short x, y, z;
                            Console.WriteLine(remote.readXYZ(out x, out y, out z));
                            vars["gX"] = x;
                            vars["gY"] = y;
                            vars["gZ"] = z;
                        }
                        break;
                    case "wait":
                        // wait x ms
                        int t = Convert.ToInt32(evalExpr(words[1]));
                        System.Threading.Thread.Sleep(t);
                        break;
                    case "alert":
                        output_txt.Text = evalExpr(line.Substring(6));
                        MessageBox.Show(output_txt.Text);
                        break;
                    case "message":
                        output_txt.Text = evalExpr(line.Substring(8));
                        break;
                    case "if":
                        // if [condition] 
                        ifcond = (evalExpr(line.Substring(3))!="0");
                        break;
                    case "else":
                        // if [condition] [false]
                        ifcond = !ifcond;
                        break;
                    case "fi":
                        // if end
                        ifcond = true;
                        break;
                    case "kfactor":
                        // kfactor val (speed up or slow down)
                        k = Convert.ToDouble(evalExpr(words[1]));
                        vScrollBar1.Value = (int)(k * 100f);
                        label1.Text = k.ToString();
                        vars["kf"] = k;
                        break;
                    case "setservo":
                        // setservo id pos
                        {
                            int id = Convert.ToInt32(evalExpr(words[1]));
                            int pos = Convert.ToInt32(evalExpr(words[2]));
                            wckMotion m = new wckMotion(remote);
                            m.wckMovePos(id, pos, 0);
                            m.close();                      
                        }
                        break;
                    case "modservo":
                        // mod id relative-pos
                        {
                            int id = Convert.ToInt32(evalExpr(words[1]));
                            int pos = Convert.ToInt32(evalExpr(words[2]));
                            wckMotion m = new wckMotion(remote);
                            if (m.wckReadPos(id))
                            {
                                m.wckMovePos(id, (int)m.respnse[1] + pos, 0);
                            }
                            m.close();
                        } 
                        break;
                    case "let":
                        // let var (=value or expression)
                        vars[words[1]] = evalExpr(words[2]);
                        break;
                    case "repeat":
                        // loop x times
                        loop_c[lc] = Convert.ToInt32(evalExpr(words[1]));
                        loop_l[lc] = i;
                        vars["_count"] = "0";
                        lc++;
                        break;
                    case "end":
                        // end of loop
                        if (lc > 0)
                        {
                            loop_c[lc-1] -= 1;
                            vars["_count"] = (Convert.ToInt32(vars["_count"]) + 1).ToString();
                            if (loop_c[lc-1] == 0)
                            {
                                lc--;
                            }
                            else
                            {
                                i = loop_l[lc-1];
                            }
                        }
                        break;
                    default:
                        int j = check(words[0]);
                        if (j >= 0)
                        {
                            // call code
                            if (j == 0)
                                NewBasicPose();
                            else
                                play(fnames[j]);
                        }
                        break;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
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

                if (fnames[i].StartsWith("S:"))
                {
                    //load into editor
                    action.Text = ((Button)sender).Text;
                    script.Text = fnames[i].Substring(2).Replace(";", "\r\n");
                    //run script
                    run(fnames[i].Substring(2));                       
                }
                else
                {
                    play(fnames[i]);
                }
            }
        }

        private void run_btn_Click(object sender, EventArgs e)
        {
            // run
            string n = action.Text;
            string c = script.Text;
            int i = 0;

            //if (n == "" || check(n) >= 0)
            //{
            //    action.Text = "";
            //    return;
            //}

            if (MessageBox.Show("Run : " + n + " - OK ?", "Run", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                c = c.Replace("\r\n", ";");
                Console.WriteLine("Script=" + c);
                run(c);
            }
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

            if (n == "") return;

            int i = check(n);
            c = c.Replace("\r\n", ";");
            Console.WriteLine("Name=" + n + " Script=" + c);

            File.WriteAllText(n + ".txt", c);

            if (i < 0)
            {
                update(n ,"S:" + c);
            }
            else
            {
                fnames[i] = "S:" + c;
            }
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            string[] spots = new string[] 
            {
            "@,73,81,87,94",
            "A,43,29,54,42",
            "B,111,28,121,45",
            "F,76,42,87,63",
            "B,75,110,88,129",
            "L,38,79,51,93",
            "R,111,78,127,93",
            "*,45,226,58,236",
            "#,105,225,118,235",
            "1,48,148,59,160",
            "2,75,149,89,160" 
            };

            string h = "";
            foreach (string t in spots)
            {
                string[] r = t.Split(',');
                if ((e.X > Convert.ToInt32(r[1])) && (e.X < Convert.ToInt32(r[3])) && (e.Y > Convert.ToInt32(r[2])) && (e.Y < Convert.ToInt32(r[4])))
                {
                    h = r[0];
                }
            }
            Console.WriteLine("Mouse - " + e.X + "," + e.Y + " : " + h);

            if (h == "@")
            {
                NewBasicPose(); // Red Button
            }
            else
            {
                int i = check(h);  // button defined
                if (i >= 0)
                {
                    if (fnames[i].StartsWith("S:"))
                    {
                        //run script
                        run(fnames[i].Substring(2));
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