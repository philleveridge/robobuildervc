﻿using System;
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

            bar parent = new bar(new foo[] { 
                new foo ( 10.5, 142,  0 ), 
                new foo ( 13.5, 166, 14 ), 
                new foo ( 15.0, 210,  9 ), 
                new foo ( 10.5,  92, 11 ), 
                new foo ( 10.5, 107, 16 ), 
                new foo ( 13.5, 109,  1 ), 
                new foo ( 13.5,  83, 14 ),             
                new foo ( 15.0,  42,  8 ),                         
                new foo ( 11.5, 159, 10 ),      
                new foo ( 10.5, 144,  1 ),      
                new foo ( 22.5 ,100, 11 ),      
                new foo ( 0.0,   70, 11 ),      
                new foo ( 0.0,  152, 11 ), 
                new foo ( 22.5, 188, 11 )}
            );

            bar p2=null;

            while (true)
            {
                //
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
                        //foreach (int s in new int[] { 4, 9, 10, 13 })
                        foreach (int s in new int[] { 10, 13 })
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
            runf = !runf;
            if (runf)
                test(50, true);

        }
    }
}
