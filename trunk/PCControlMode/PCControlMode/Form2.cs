using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Collections;

namespace RobobuilderLib
{
    public partial class Form2 : Form
    {
        public SerialPort sp1;
        public PCremote pcR;

        const int MAXBUTTONS = 10;
        const int MAXDEPTH = 5;

        private System.Windows.Forms.Button[] button_array;
        private string[] fnames;
        int cnt;
        double k = 1.0; //speed factor
        public bool presets_flg;

        public Form2()
        {
            InitializeComponent();

            button_array = new Button[MAXBUTTONS];
            fnames = new string[MAXBUTTONS];

            cnt = 0;

            update("Basic");

            presets_flg = false;
        }

        public string list_presets()
        {
            string r = "";
            for (int i = 0; i < cnt; i++)
            {
                r += "BUTTON=" + button_array[i].Text + "," + fnames[i] + "\n";
            }
            return r;
        }

        public int check(string name)
        {
            for (int i = 0; i < cnt; i++)
            {
                if (button_array[i].Text == name) return i;
            }
            return -1;
        }

        public void update(string name)
        {
            // 
            // add 'unique' button
            // 

            string[] args = name.Split(',');

            if (check(args[0]) >= 0) return;

            if (args.Length > 1)
                fnames[cnt] = args[1];
            else
                fnames[cnt] = "";

            int cols = 3;

            button_array[cnt] = new Button();

            button_array[cnt].Location = new System.Drawing.Point(10 + 80 * (cnt % cols), 10 + 40 * (cnt / cols));
            button_array[cnt].Name = "button-" + cnt;
            button_array[cnt].Size = new System.Drawing.Size(53, 27);
            button_array[cnt].TabIndex = 1;
            button_array[cnt].Text = args[0];
            button_array[cnt].UseVisualStyleBackColor = true;
            button_array[cnt].Click += new System.EventHandler(this.button_Click);

            this.Controls.Add(button_array[cnt]);

            if (cnt<MAXBUTTONS-1) cnt++;

            presets_flg=true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        void PlayPose(int duration, int no_steps, byte[] spod)
        {
            if ((sp1 != null)  && (!sp1.IsOpen)) return;
            wckMotion m = new wckMotion(pcR);
            m.PlayPose(duration, no_steps, spod, true);
            m.close();
        }

        private void NewBasicPose()
        {
            PlayPose(1000, 10, new byte[] {
                /*0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 */
                171,179,198,83,105,78,72,49,172,141,47,47,49,200,205,205,122,125,127 });
        }

        private void play(string filename)
        {
            // play
            int n = 0;
            wckMotion m = new wckMotion(pcR);
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

                    if (r.Length > 2)
                    {
                        n++;

                        // label2.Text = n.ToString();

                        byte[] t = new byte[r.Length - 2];

                        for (int i = 2; i < r.Length; i++)
                        {
                            t[i - 2] = Convert.ToByte(r[i]);
                        }

                        m.PlayPose((int)(((double)Convert.ToInt32(r[0]))*k), Convert.ToInt32(r[1]), t, ff);
                        if (ff) ff = false;

                        if (stepflg)
                        {
                            MessageBox.Show("Next " + n);
                        }
                    }

                }
                tr.Close();
                m.close();
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
            return x;
        }

        private void run(string script)
        {
            Console.WriteLine("Script=" + script);
            int[] loop_l = new int[MAXDEPTH];
            int[] loop_c = new int[MAXDEPTH];

            vars = new Hashtable();

            int lc = 0;

            string[] sc = script.Split(';');
            for (int i = 0; i < sc.Length; i++)
            {
                string line = sc[i];
                Console.WriteLine(i + " " + sc[i]);
                string[] words = line.Split(' ');
                int j = check(words[0]);
                if (j >= 0)
                {
                    // call code
                    play(fnames[j]);
                    continue;
                }
                switch (words[0])
                {
                    case "wait":
                        // wait x ms
                        int t = Convert.ToInt32(evalExpr(words[1]));
                        System.Threading.Thread.Sleep(t);
                        break;
                    case "message":
                        MessageBox.Show(evalExpr(line.Substring(8)));
                        break;
                    case "if":
                        // if [condition] [true] [false]
                        break;
                    case "kfactor":
                        // kfactor val
                        k = Convert.ToDouble(evalExpr(words[1]));
                        break;
                    case "setservo":
                        // setservo id pos
                        {
                            int id = Convert.ToInt32(evalExpr(words[1]));
                            int pos = Convert.ToInt32(evalExpr(words[2]));
                            wckMotion m = new wckMotion(pcR);
                            m.wckMovePos(id, pos, 0);
                            m.close();                      
                        }
                        break;
                    case "modservo":
                        // mod id relative-pos
                        {
                            int id = Convert.ToInt32(evalExpr(words[1]));
                            int pos = Convert.ToInt32(evalExpr(words[2]));
                            wckMotion m = new wckMotion(pcR);
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
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "button-0":
                    NewBasicPose();
                    break;
                default:
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
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // run
            string n = action.Text;
            string c = script.Text;
            int i = 0;

            if (n == "" || check(n) >= 0)
            {
                action.Text = "";
                return;
            }

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

        private void button3_Click(object sender, EventArgs e)
        {
            //store 
            string c = script.Text;
            string n = action.Text;

            if (n == "") return;

            int i = check(n);
            c = c.Replace("\r\n", ";");
            Console.WriteLine("Name=" + n + " Script=" + c);

            if (i < 0)
            {
                update(n + ",S:" + c);
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
        
    }
}
