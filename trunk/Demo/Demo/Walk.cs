using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using RobobuilderLib;

namespace Demo
{

    struct compare
    {
        public int av;
        public int[] dp;

        public compare(int a, int[] c)
        { av = a; dp = c; }
    };

    class Compare
    {
        List<compare> items = new List<compare>();

        public Compare(string f)
        {
            matrix m2 = new matrix(f);

            for (int i = 0; i < m2.getr(); i++)
            {
                double[] t = m2.getrow(i);
                items.Add(new compare((int)vectors.head(t), vectors.convInt(vectors.tail(t))));
            }
        }

        public void close()
        {
            items.Clear();
        }

        public void build()
        {

        }

        public int[] match(int g)
        {
            int[] res = null;
            int diff = 99;
            foreach (compare r in items)
            {
                if (Math.Abs(r.av - g) < diff)
                {
                    diff = Math.Abs(r.av - g);
                    res = r.dp;
                }
            }
            return res;
        }
    }

    class BalanceWalk
    {
        public bool wlk; 
        public int dely = 50;
        public string state = "s"; // "R" // "L" or "r" and "l"

        wckMotion w;
        int n_of_s;
        long st;

        bool dhf = false;

        int gx = 0, gy = 0, gz = 0;

        byte[][] fstep;

        Compare zm;

        public BalanceWalk(wckMotion w1, bool dh)
        {
            w = w1;
            dhf = dh;

            n_of_s = countServos(22);
            Console.WriteLine("Balance walk - {0}", n_of_s);

            matrix m;
            
            if (dhf)
                m = new matrix("rls2.csv");
            else
                m = new matrix("rlstep.csv");

            int r = m.getr();

            fstep = new byte[r][];


            for (int i = 0; i < r; i++)
            {
                if (dhf)
                    fstep[i] = vectors.convByte(m.getrow(i));
                else
                    fstep[i] = cv18(vectors.convByte(m.getrow(i)));
            }

            zm = new Compare("compare.csv");
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

        byte[][] reverse(byte[][] z)
        {
            byte[][] r = new byte[z.Length][];

            for (int i = 0; i < z.Length; i++)
                r[i] = z[z.Length - i - 1];

            return r;
        }

        void mirror(bool f)
        {
            byte p1=0,p2=0,p3=0,p4=0,p5=0,p6=0;
            int n = f ? 13 : 10; // reading side
            int m = f ? 10 : 13; // writing side

            w.wckPassive(n);                    
            w.wckPassive(n+1);
            w.wckPassive(n+2);

            if (w.wckReadPos(n))   p1=w.respnse[1];
            if (w.wckReadPos(n+1)) p2=w.respnse[1];
            if (w.wckReadPos(n+2)) p3=w.respnse[1];

            if (w.wckReadPos(m))     p4 = w.respnse[1];
            if (w.wckReadPos(m + 1)) p5 = w.respnse[1];
            if (w.wckReadPos(m + 2)) p6 = w.respnse[1];

            if (p4 != (254 - p1)) w.wckMovePos(m,     254 - p1, 4);
            if (p5 != (254 - p2)) w.wckMovePos(m + 1, 254 - p2, 4);
            if (p6 != (254 - p3)) w.wckMovePos(m + 2, 254 - p3, 4);
        }

        void calibrateXYZ()
        {
            gx = 0; gy = 0; gz = 0;

            if (w.wckReadAll())
            {
                gx = w.cbyte(w.respnse[1]); 
                gy = w.cbyte(w.respnse[2]);
                gz = w.cbyte(w.respnse[3]);
            }
            Console.WriteLine("calibrated: {0},{1},{2}", gx, gy, gz);
        }

        public void motion (Utility u)
        {
            byte[][] cpos=null;
            byte[] sbase=new byte[16] {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
            int[] az=null;
            int counter=0;

            bool bal = true;

            int x=0, z=0, dz=0;
            int d = 0; 

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
                    x = w.cbyte(w.respnse[1])-gx;
                    int tz = w.cbyte(w.respnse[3])-gz;
                    dz = z - tz;
                    z = tz;
                    d = w.respnse[0];

                    if (w.respnse[4] < 255)
                    {
                        switch((PCremote.RemoCon)(w.respnse[4]))
                        {
                            case PCremote.RemoCon.Stop:
                                if (state == "R")
                                    state = "s";
                                else
                                    wlk = false;
                                break;

                            case PCremote.RemoCon.Forward:
                                if (state == "s")
                                    state = "R";
                                break;

                            case PCremote.RemoCon.Back:
                                if (state == "s")
                                    state = "r";
                                break;

                            case PCremote.RemoCon.A:
                                bal = !bal;
                                break;
                        }
                    }

                    Console.WriteLine("{0},{1},{2},{3},{4},{5}", counter, z, dz, d, w.respnse[4], bal);

                }

                if ((nc++ % 10) == 0)
                {
                    //timing loop
                    rt = (DateTime.Now.Ticks - st) / (10 * TimeSpan.TicksPerMillisecond);
                    st = DateTime.Now.Ticks;
                    w.wckReadPos(30, 5); // for PSD read
                }

                u.pwin(z, dz, nc, rt);

                if (counter == 0)
                {
                    switch (state)
                    {
                        case "R":
                            cpos = fstep;
                            if (d < 15) state = "r";
                            break;

                        case "r":
                            cpos = reverse(fstep);
                            state = "r";
                            if (d > 25) state = "R";
                            break;
                        
                        case "s":
                            cpos = new byte[1][];
                            if (dhf)
                                cpos[0] = wckMotion.basicdh;
                            else
                                cpos[0] = wckMotion.basic18;
                            break;

                        case "m":
                            mirror(true);
                            cpos = null;
                            break;
                    }
                }

                try
                {
                    int[] dp = zm.match(z);

                    if (az == null)
                        az= dp;
                    else
                    {
                        if (dp!=null)
                        {
                            az = vectors.add(az, dp);
                        }

                        Console.Write("Pwr={0}", vectors.rms(az));
                    }

                    if (cpos != null)
                    {
                        if (bal)
                            sbase = vectors.bcheck(vectors.add(az, cpos[counter]), wckMotion.lb_Huno, wckMotion.ub_Huno); // lb_Huno, ub_Huno);
                        else
                            sbase = cpos[counter];

                        w.SyncPosSend(15, 4, sbase, 0);
                        System.Threading.Thread.Sleep(dely);

                        if (++counter >= cpos.Length) counter = 0;
                    }

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
