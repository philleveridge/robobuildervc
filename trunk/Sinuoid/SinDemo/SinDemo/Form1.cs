using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using RobobuilderLib;

namespace SinDemo
{
    public partial class Form1 : Form
    {
        bool runf = false;
        wckMotion w;
        bar[] generation = new bar[3];

        public Form1()
        {
            InitializeComponent();

            build_screen(initdata());
        }

        private void test(int d, bool f)
        {
            bar p2=null;

            while (true)
            {
                //
                bar parent = loaddata();

                for (int i = 0; i < generation.Length; i++)
                {
                    if (p2==null)
                        generation[i] = new bar(parent); 
                    else
                        generation[i] = new bar(parent,p2); 

                    generation[i].fit += i;
                }
                //
                for (int gen = 0; gen < generation.Length; gen++)
                {
                    System.Windows.Forms.Application.DoEvents();

                    for (int i = 0; i < 16; i++)
                    {
                        if (w.wckReadAll())
                        {
                            /*                    
                            Console.WriteLine("X={1}, Y={2}, z={3}, PSD={0}, IR={4}, BTN={5}, SND={6}",
                                w.respnse[0],
                                w.respnse[1],
                                w.respnse[2],
                                w.respnse[3],
                                w.respnse[4],
                                w.respnse[5],
                                w.respnse[6]);
                            */
                            if (w.respnse[4] == 1) //A
                            {
                                generation[gen].fit += 5;
                                Console.WriteLine("f+ {0}", generation[gen].fit);
                            }

                            if (w.respnse[4] == 2) //B
                            {
                                generation[gen].fit -= 5;
                                Console.WriteLine("f- {0}", generation[gen].fit);
                            }
                        }
                        else
                        {
                            // failed
                            Console.WriteLine("Failed to read servos");
                            runf = false;
                            return;
                        }

                        foreach (int s in loadindex())
                        {
                            bar z = generation[gen];

                            int p = z.abc[s].offset + (int)(z.abc[s].amp * Math.Sin(((double)(i + z.abc[s].phase) / 16) * Math.PI * 2));

                            if (f)
                            {
                                w.wckMovePos(s, p, 4);
                            }
                        }


                        System.Threading.Thread.Sleep(d);
                    }

                    if (!runf)
                        return;

                    // sort fitness

                    Array.Sort(generation, delegate(bar a, bar b) { return bar.fmatch(a, b); });

                    // select next parent

                    parent = generation[0];
                    p2 = null; // generation[1];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            w = new wckMotion("COM5", true);

            if (!w.wckReadPos(30, 0))
            {
                textBox1.AppendText("Failed to connect or not DCMP");
                w.close();
                w = null;
                return;
            }

            textBox1.AppendText(string.Format("DCMP {0}.{1}", w.respnse[0], w.respnse[1]));

            w.PlayPose(1000, 10, wckMotion.basicdh, true);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = (runf) ? "Test" : "Stop";
            runf = !runf;
            if (runf)
                test(50, true);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //reset
            resetdata(initdata());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // a generation
            bar child = new bar(loaddata());
            child.print();
            child.breed();
            resetdata(child);
        }

    /****************************************************************************************************/

        private ListBox[]  listBox1  = new ListBox[16];
        private ListBox[]  listBox2  = new ListBox[16];
        private ListBox[]  listBox3  = new ListBox[16];
        private Label[]    label1    = new Label[16];
        private CheckBox[] checkBox1 = new CheckBox[16];

        private bar initdata()
        {
            foo[] f = new foo[] { 
                new foo ( 10.5, 142,  0 ), 
                new foo ( 13.5, 166, 14 ), 
                new foo ( 15.0, 210,  9 ), 
                new foo ( 10.5,  92, 11 ), 
                new foo ( 10.5, 107,  0 ), 
                new foo ( 13.5, 109,  1 ), 
                new foo ( 13.5,  83, 14 ),             
                new foo ( 15.0,  42,  8 ),                         
                new foo ( 11.5, 159, 10 ),      
                new foo ( 10.5, 144,  1 ),      
                new foo ( 22.5 ,100, 11 ),      
                new foo ( 0.0,   70, 11 ),      
                new foo ( 0.0,  152, 11 ), 
                new foo ( 22.5, 188, 11 )};
            return new bar(f);
        }


        private void resetdata(bar data)
        {
            for (int i = 0; i < 14; i++)
            {

                for (double d = -2.0; d <= 2.0; d = d + 0.2)
                {
                    listBox1[i].Items.Add(data.abc[i].amp + d);
                }
                listBox1[i].SelectedIndex = 10;
                listBox2[i].SelectedIndex = data.abc[i].phase;
                listBox3[i].SelectedIndex = data.abc[i].offset - 10;
            }
        }

        private bar loaddata()
        {
            bar r = initdata(); // use init data as defaults

            for (int i = 0; i < r.abc.Length; i++)
            {
                if (listBox1[i].SelectedIndex >= 0)
                    r.abc[i].amp = (double)listBox1[i].Items[listBox1[i].SelectedIndex];

                if (listBox3[i].SelectedIndex >= 0)
                    r.abc[i].phase = (int)listBox2[i].Items[listBox2[i].SelectedIndex];

                if (listBox3[i].SelectedIndex >= 0)
                    r.abc[i].offset = (int)listBox3[i].Items[listBox3[i].SelectedIndex];
            }

            return r;
        }

        private List<int> loadindex()
        {
            List<int> r = new List<int>();
            for (int i = 0; i < 14; i++)
            {
                if (checkBox1[i].Checked)
                {
                    r.Add(i);
                }
            }
            return r;
        }

        private void build_screen(bar data)
        {
            int c;
            for (int i = 0; i < 14; i++)
            {
                listBox1[i] = new System.Windows.Forms.ListBox();
                listBox2[i] = new System.Windows.Forms.ListBox();
                listBox3[i] = new System.Windows.Forms.ListBox();
                label1[i]   = new System.Windows.Forms.Label();
                checkBox1[i] = new System.Windows.Forms.CheckBox();
                // 
                // listBox1
                // 
                listBox1[i].FormattingEnabled = true;
                listBox1[i].Location = new System.Drawing.Point(50, 90+i*16);

                listBox1[i].Name = "amp";
                listBox1[i].Size = new System.Drawing.Size(43, 17);

                for (double d=-2.0; d<=2.0; d=d+0.2)
                {
                    listBox1[i].Items.Add(data.abc[i].amp+d);
                }
                listBox1[i].SelectedIndex = 10;

                // 
                // listBox2
                // 
                listBox2[i].Location = new System.Drawing.Point(99, 90 + i * 16);
                listBox2[i].Name = "phase";
                listBox2[i].Size = new System.Drawing.Size(43, 17);

                for (c = 0; c < 16; c++)
                {
                    listBox2[i].Items.Add(c);
                }
                listBox2[i].SelectedIndex = data.abc[i].phase;
                // 
                // listBox3
                // 
                listBox3[i].Location = new System.Drawing.Point(148, 90 + i * 16);
                listBox3[i].Name = "offset";
                listBox3[i].Size = new System.Drawing.Size(43, 17);

                for (c = 10; c < 240; c++)
                {
                    listBox3[i].Items.Add(c);
                }
                listBox3[i].SelectedIndex = data.abc[i].offset-10;

                // 
                // label1
                // 
                label1[i].AutoSize = true;
                label1[i].Location = new System.Drawing.Point(6, 90 + i * 16);
                label1[i].Name = "label1";
                label1[i].Size = new System.Drawing.Size(19, 13);
                label1[i].Text = i.ToString();
                // 
                // checkBox1
                // 
                checkBox1[i].AutoSize = true;
                checkBox1[i].Location = new System.Drawing.Point(31, 90 + i * 16);
                checkBox1[i].Name = "checkBox1";
                checkBox1[i].Size = new System.Drawing.Size(15, 14);
 

                this.Controls.Add(checkBox1[i]);
                this.Controls.Add(label1[i]);
                this.Controls.Add(listBox3[i]);
                this.Controls.Add(listBox2[i]);
                this.Controls.Add(listBox1[i]);
            }
        }


    }
}
