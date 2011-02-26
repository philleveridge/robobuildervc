using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
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
            

        }

        private void test(int d, bool f)
        {
            while (true)
            {
                //
                for (int i = 0; i < generation.Length; i++)
                    generation[i] = new bar();
                //
                for (int gen = 0; gen < 3; gen++)
                {
                    System.Windows.Forms.Application.DoEvents();

                    for (int i = 0; i < 16; i++)
                    {
                        foreach (int s in new int[] { 10, 13 })
                        {
                            bar z = generation[gen];

                            int p = z.abc[s].offset + (int)(z.abc[s].amp * Math.Sin(((double)(i + z.abc[s].phase) / 16) * Math.PI * 2));

                            Console.WriteLine("{0} : {1}", i, p);

                            if (f)
                            {
                                w.wckMovePos(s, p, 4);
                            }
                        }

                        System.Threading.Thread.Sleep(d);
                    }

                    if (!runf)
                        break;
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

            w.PlayPose(1000, 10, wckMotion.dh, true);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            runf = !runf;
            if (runf)
                test(50, true);

        }
    }
}
