﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using RobobuilderLib;

namespace Demo
{
    class BalanceWalk
    {
        public bool wlk; 
        public int dely = 50;
        public string state = "s"; // "R" // "L" or "r" and "l"

        wckMotion w;
        int n_of_s;
        long st;

        CList coords;

        int gx = 0, gy = 0, gz = 0;

        byte[] ub_Huno = new byte[] { 174, 228, 254, 130, 185, 254, 180, 126, 208, 208, 254, 224, 198, 254, 200, 254 };
        byte[] lb_Huno = new byte[] { 1, 70, 124, 40, 41, 73, 22, 1, 120, 57, 1, 46, 1, 1, 25, 40 };
        byte[][] fstep, bstep;

        struct compare
        {
            public int av;
            public int[] dp;

            public compare(int a, int[] c)
            { av = a; dp = c; }
        };

        compare[] zm;

        public BalanceWalk(wckMotion w1)
        {
            w = w1;
            n_of_s = countServos(22);
            Console.WriteLine("Balance walk - {0}", n_of_s);

            matrix m = new matrix("rlstep.csv");

            fstep = new byte[m.getr()][]; 
            bstep = new byte[m.getr()][];

            int r =  m.getr();

            for (int i = 0; i < r; i++)
            {
                fstep[i] = cv18(vectors.convByte(m.getrow(i)));
                bstep[r - i-1] = fstep[i];
            }

            matrix m2 = new matrix("compare.csv");
            zm = new compare[m2.getr()];

            for (int i = 0; i < m2.getr(); i++)
            {
                double[] t = m2.getrow(i);
                zm[i] = new compare((int)vectors.head(t), vectors.convInt(vectors.tail(t)));
            }

        }

        byte[] cv18(byte[] a) // hip conversion
        {
            byte[] r = new byte[a.Length];

            for (int i = 0; i < a.Length; i++)
                r[i] = a[i];

            if (n_of_s > 16)
            {
                r[0] += 18;
                r[5] -= 20;
            }
            return r;
        }

        int countServos(int m)
        {
            for (int i = 0; i < m; i++)
            {
                if (!w.wckReadPos(i))
                {
                    return i;
                }
            }
            return m;
        }

        int[] rmatch(int g, compare[] c)
        {
            int[] res = null;
            int diff = 99;
            foreach (compare r in c)
            {
                if (Math.Abs(r.av - g) < diff)
                {
                    diff = Math.Abs(r.av - g);
                    res = r.dp;
                }
            }
            return res;
        }

        void calibrateXYZ()
        {
            gx = 0; gy = 0; gz = 0;

            if (w.wckReadAll())
            {
                gx = w.cbyte(w.respnse[0]); 
                gy = w.cbyte(w.respnse[1]);
                gz = w.cbyte(w.respnse[2]);
            }
            Console.WriteLine("calibrated: {0},{1},{2}", gx, gy, gz);
        }

        public void motion (Utility u)
        {
            byte[][] cpos=null;
            byte[] sbase=new byte[16] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            int[] az=null;
            int counter=0;

            int x=0, z=0;
            int d = 0; 

            coords = new CList();
            calibrateXYZ();

            int nc = 0;
            Double rt = 0; 
            st = DateTime.Now.Ticks;

            wlk = true;

            while (wlk)
            {
                System.Windows.Forms.Application.DoEvents();

                if (w.wckReadAll())
                {
                    x = w.cbyte(w.respnse[0])-gx;
                    z = w.cbyte(w.respnse[2])-gz;
                    d = w.respnse[3];
                    if (w.respnse[4] < 255)
                    {
                        PCremote.RemoCon ir = (PCremote.RemoCon)(w.respnse[4]);
                        if (ir == PCremote.RemoCon.Stop)
                        {
                            if (state == "R")
                                state = "s";
                            else
                                wlk = false;
                        }
                        if (ir == PCremote.RemoCon.Forward && state == "s") 
                            state = "R";
                    }
                }

                if ((nc++ % 10) == 0)
                {
                    //timing loop
                    rt = (DateTime.Now.Ticks - st) / (10 * TimeSpan.TicksPerMillisecond);
                    st = DateTime.Now.Ticks;
                    w.wckReadPos(30, 5); // for PSD read

                }

                u.pwin(coords, z, nc, rt);

                if (counter == 0)
                {
                    switch (state)
                    {
                        case "R":
                            cpos = fstep;
                            if (d < 15) state = "r";
                            break;

                        case "r":
                            cpos = bstep;
                            state = "r";
                            if (d > 25) state = "R";
                            break;
                        
                        case "s":
                            cpos = new byte[1][];
                            cpos[0] = wckMotion.basic18;
                            break;
                    }
                }

                try
                {
                    int[] dz = rmatch(z, zm);

                    if (az == null)
                        az= dz;
                    else
                    {
                        if (dz!=null)
                        {
                            az = vectors.add(az, dz);
                        }
                    }

                    sbase = vectors.bcheck(vectors.add(az, cpos[counter++]), lb_Huno, ub_Huno);

                    if (counter >= cpos.Length) counter = 0;

                    w.SyncPosSend(15, 4, sbase, 0);
                    System.Threading.Thread.Sleep(dely);

                    Console.Write(state);
                }
                catch
                {
                    Console.WriteLine("fail");
                    wlk = false;
                }
            }
        }

    }
}
