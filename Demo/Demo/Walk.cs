using System;
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

        byte[][] rstep = new byte[][] {
            new byte[] {123, 156, 212,  80, 108, 126, 73, 40, 150, 141,  68, 44, 40, 138, 208, 195},
            new byte[] {130, 165, 201,  81, 115, 134, 81, 31, 147, 149,  72, 44, 40, 145, 209, 201},
            new byte[] {132, 171, 197,  83, 117, 137, 86, 28, 148, 152,  78, 43, 41, 154, 209, 206},
            new byte[] {132, 175, 195,  87, 117, 139, 91, 27, 152, 154,  87, 43, 43, 164, 209, 211},
            new byte[] {132, 178, 197,  91, 117, 137, 95, 28, 157, 152,  97, 43, 48, 172, 209, 213},
            new byte[] {130, 179, 201,  95, 115, 134, 96, 31, 161, 149, 105, 43, 53, 179, 210, 214},
            new byte[] {127, 178, 206,  98, 112, 130, 95, 35, 166, 145, 111, 42, 57, 182, 210, 214},
            new byte[] {124, 175, 212, 100, 109, 127, 92, 40, 170, 142, 113, 42, 59, 183, 210, 214}
        };

        byte[][] lstep = new byte[][] {
            new byte[] {124, 175, 212, 100, 109, 127, 92, 40, 170, 142, 113, 42, 59, 183, 210, 214},
            new byte[] {120, 172, 217, 102, 105, 123, 88, 46, 170, 138, 111, 42, 57, 182, 210, 214},
            new byte[] {116, 167, 221, 103, 101, 120, 83, 51, 169, 135, 106, 43, 53, 179, 210, 214},
            new byte[] {113, 162, 224, 102,  98, 118, 77, 55, 167, 133, 97,  43, 48, 173, 209, 213},
            new byte[] {111, 157, 225, 98,   96, 118, 73, 57, 163, 133, 87,  43, 43, 164, 209, 211},
            new byte[] {113, 153, 224, 93,   98, 118, 70, 55, 159, 133, 79,  43, 41, 154, 209, 206},
            new byte[] {116, 152, 221, 89,  101, 120, 69, 51, 155, 135, 72,  44, 40, 146, 209, 201},
            new byte[] {120, 153, 217, 84,  105, 123, 70, 46, 152, 138, 69,  44, 40, 140, 208, 197},
            new byte[] {123, 156, 212, 80,  108, 126, 73, 40, 150, 141, 68,  44, 40, 138, 208, 195}
            };

        byte[][] rstep_r;
        byte[][] lstep_r;

        struct compare
        {
            public int min;
            public int max;
            public int[] dp;

            public compare(int a, int b, int[] c)
            { min = a; max = b; dp = c; }
        };

        compare[] z2 = new compare[] {
                new compare( 14, 20, new int[] {0,0,0,0,0,0,0,0,0,0, 4,0,0,-4,0,0}),
                new compare( 10, 15, new int[] {0,0,0,0,0,0,0,0,0,0, 2,0,0,-2,0,0}),
                new compare(  5, 11, new int[] {0,0,0,0,0,0,0,0,0,0, 1,0,0,-1,0,0}),
                new compare(-11, -5, new int[] {0,0,0,0,0,0,0,0,0,0,-1,0,0, 1,0,0}),
                new compare(-14,-10, new int[] {0,0,0,0,0,0,0,0,0,0,-2,0,0, 2,0,0}),
                new compare(-20,-13, new int[] {0,0,0,0,0,0,0,0,0,0,-4,0,0, 4,0,0})
            };

        compare[] Zx = new compare[] {
                new compare( 6, 15, new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, -2, 0 }),
                new compare( 4,  7, new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, -1, 0 }),
                new compare(-7, -4, new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  1, 0, 0,  1, 0 }),
                new compare(-15,-6, new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  2, 0, 0,  2, 0 })    
            };

        public BalanceWalk(wckMotion w1)
        {
            w = w1;
            n_of_s = countServos(22);
            Console.WriteLine("Balance walk - {0}", n_of_s);
            lstep_r = reverse(lstep);
            rstep_r = reverse(rstep);
        }

        public void standup()
        {
            if (n_of_s < 16) return;
            w.PlayPose(1000, 10, (n_of_s < 18) ? wckMotion.basic16 : wckMotion.basic18, true);
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

        byte[] bcheck(int[] p, byte[] min, byte[] max)
        {
            byte[] r = new byte[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                if (i < min.Length && i < max.Length)
                    r[i] = (byte)((p[i] > max[i]) ? max[i] : ((p[i] < min[i]) ? min[i] : p[i]));
            }

            return r;
        }

        int[] rmatch(int g, compare[] c)
        {
            int[] res = null;
            foreach (compare r in c)
            {
                if (g >= r.min && g < r.max)
                {
                    res = r.dp;
                    break;
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

        int[] add_delta(int[] a, int[] b)
        {
            if (a == null) return b;

            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = a[i] + ((b != null && i < b.Length) ? b[i] : 0);
            }
            return r;
        }

        int[] add_delta(int[] a, byte[] b)
        {
            int[] r;

            if (a == null && b == null) return null;

            if (a == null)
            {
                r = new int[b.Length];
                for (int i = 0; i < b.Length; i++)
                {
                    r[i] = b[i];
                }
                return r;
            }

            r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = a[i] + ((b != null && i < b.Length) ? b[i] : 0);
            }
            return r;
        }

        public byte[] getallServos(int p)
        {
            byte[] r = new byte[p];
            for (int i = 0; i < p; i++)
            {
                if (w.wckReadPos(i))
                    r[i] = w.respnse[1];
                else
                    r[i] = 0;
            }
            return r;
        }

        byte[][] reverse(byte[][] z)
        {
            byte[][] r = new byte[z.Length][];

            for (int i = 0; i < z.Length; i++)
                r[i] = z[z.Length-i-1];

            return r;
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

        void pwin(Utility u, int coord, int n, double t)
        {
            int nx = ((n * 10) % 280) - 140;
            int ny = 4 * coord;

            u.plot("(Acc=" + ny + " Rate=" + String.Format("{0:#.#}", t) + " ms)", nx, ny);
            u.drawlist(new int[] { -125, 40, 125, 40 }, 4, (Pen)((ny > 40) ? u.p2 : u.p1)); //limit
            u.drawlist(new int[] { -125, -40, 125, -40 }, 4, (Pen)((ny < -40) ? u.p2 : u.p1)); //limit    
            coords.store(nx, ny);
            u.drawlist(coords.getlast(6), 6, new Pen(Color.FromName("Blue")));
        }

        public void motion (Utility u)
        {
            byte[][] cpos=null;
            byte[] sbase=null;
            int[] az=null;
            int counter=0;

            int x=0, z=0;
            Double rt = 0;

            coords = new CList();
            calibrateXYZ();

            sbase = new byte[16] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

            int nc = 0;
            st = DateTime.Now.Ticks;
            wlk = true;
            int d=0;

            while (wlk)
            {
                System.Windows.Forms.Application.DoEvents();

                if (w.wckReadAll())
                {
                    x = w.cbyte(w.respnse[0])-gx;
                    z = w.cbyte(w.respnse[2])-gz;
                    d = w.respnse[3];
                    if (w.respnse[4] < 255)
                        Console.WriteLine("IR pressed = {0}",w.respnse[4]);
                }

                if ((nc++ % 10) == 0)
                {
                    //timing loop
                    rt = (DateTime.Now.Ticks - st) / (10 * TimeSpan.TicksPerMillisecond);
                    st = DateTime.Now.Ticks;
                }

                pwin(u, z, nc, rt);

                if (counter == 0)
                {
                    switch (state)
                    {
                        case "R":
                            cpos = rstep;
                            state = "L";
                            break;
                        case "L":
                            cpos = lstep;
                            state = "R";
                            if (d < 15) state = "r";
                            break;
                        case "r":
                            cpos = lstep_r;
                            state = "l";
                            break;
                        case "l":
                            cpos = rstep_r;
                            state = "r";
                            if (d > 25) state = "R";
                            break;
                        case "s":
                            cpos = new byte[1][];
                            cpos[0] = wckMotion.basic16;
                            break;
                    }
                }


                try
                {
                    int[] dz = rmatch(z, z2);

                    if (az == null)
                        az= dz;
                    else
                    {
                        if (dz!=null)
                        {
                            az = add_delta(az ,dz);
                        }
                    }
                    sbase = bcheck(add_delta(az, cv18(cpos[counter++])), lb_Huno, ub_Huno);

                    if (counter >= cpos.Length) counter = 0;

                    w.SyncPosSend(15, 4, sbase, 0);
                    System.Threading.Thread.Sleep(dely); 

                }
                catch
                {
                    Console.WriteLine("fail");
                    wlk = false;
                }




                Console.Write(state);

            }
	        standup();
        }

    }
}
