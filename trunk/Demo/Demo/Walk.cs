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

        byte[][] rstep = new byte[8][];
        byte[][] lstep = new byte[9][];
        byte[][] rstep_r;
        byte[][] lstep_r;

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

            matrix m = new matrix(16, 17);
            if (m.load("rlstep.csv"))
            {
                for (int i = 0; i < 8; i++)
                    rstep[i] = cv18(vectors.convByte( m.getrow(i)));

                for (int i = 0; i < 9; i++)
                    lstep[i] = cv18(vectors.convByte(m.getrow(8+i)));      
            }

            lstep_r = reverse(lstep);
            rstep_r = reverse(rstep);

            matrix m2 = new matrix(17,7);
            zm = new compare[7];
            if (m2.load("compare.csv"))
            {
                for (int i = 0; i < 7; i++)
                {
                    double[] t = m2.getrow(i);
                    zm[i] = new compare((int)vectors.head(t), vectors.convInt(vectors.tail(t)));
                }
            }
        }

        byte[][] reverse(byte[][] z)
        {
            byte[][] r = new byte[z.Length][];

            for (int i = 0; i < z.Length; i++)
                r[i] = z[z.Length - i - 1];

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
                    {
                        //MessageBox.Show(String.Format("IR pressed = {0}", w.respnse[4]));
                        if (w.respnse[4] == 7) wlk = false;
                    }
                }

                if ((nc++ % 10) == 0)
                {
                    //timing loop
                    rt = (DateTime.Now.Ticks - st) / (10 * TimeSpan.TicksPerMillisecond);
                    st = DateTime.Now.Ticks;
                    w.wckReadPos(30, 5); // for PSD read
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
