using System;

// this will idetify next sequence to run
// basic process:
// loop
//   search for best input match
//   inputs- current  positions, acceleromter, IR, distance sensor etc
//   from match - idenitfy potential next outputs
//   generate alterntaives if no good matches
//   select outputs
//   play

namespace RobobuilderLib
{
    class Autonomy
    {
        // constants
        public const int L = 10;
        public const int MAXSERVOS = 16;
        public const int MAXRANGE  = 4;

        // public data
        public int[] o_xyz   = null; //origin
        public int[] o_d     = null; //previous diff
        public byte[] servos = null;
        public int   minx = 255, maxx = -255, miny = 255, maxy = -255, minz = 255, maxz = -255;
        public double mfy = 1.0;
        public double mfz = 1.0;

        dbase db;

        int count;

        Random n = new Random();

        // constructor

        public Autonomy() 
        {
            init();
        }

        // *****************************************************************************************
        //   database functions
        // *****************************************************************************************

        public void test()
        {
            vectors.test();

            int[] a = new int[] { 1, 2, 3 };
            int[] b = new int[] { 3, 2, 4 };

            Console.WriteLine("Data gen test");
            Console.WriteLine("acc(a)  =" + vectors.str(readAcc()));
            Console.WriteLine("servo(a)=" + vectors.str(readServos()));

            Console.WriteLine("DB test");
            db.store_db(a, b, 1.0);
            db.store_db(readAcc(), b, 2.0);
            db.store_db(readAcc(), b, 2.0);
            db.show_db();

            db.store_db(new int[] { 1, 2, 3 }, b, 2.0);
            int f = db.find_db(a);
            Console.WriteLine("f=" + f);
            if (f >= 0) db.update_db(f, 3.0);

            db.show_db();
        }

        // *****************************************************************************************
        // test data / routines
        // *****************************************************************************************

        public int[] readServos()
        {
            int[] r = new int[16];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = n.Next(0, 254);
            }
            return r;
        }

        public int[] readAcc()
        {
            int[] r = new int[3];
            r[0] = (n.Next(50) - 25);
            r[1] = (n.Next(50) - 25);
            r[2] = (n.Next(50) - 25);

            return r;
        }

        // *****************************************************************************************
        // main routines
        // *****************************************************************************************

        public void init()
        {
            db = new dbase();
            db.clear_db();

            o_xyz = null;
            o_d = null;

            minx = 255; maxx = -255;
            miny = 255; maxy = -255;
            minz = 255; maxz = -255;
            mfy = 1.0;
            mfz = 1.0;

            count = 0;
        }

        public void show_db()
        {
            db.show_db();
        }

        public void calibrateXYZ(int[] a)
        {
            if (a[0] < minx) minx = a[0];
            if (a[1] < miny) miny = a[1];
            if (a[2] < minz) minz = a[2];

            if (a[0] > maxx) maxx = a[0];
            if (a[1] > maxy) maxy = a[1];
            if (a[2] > maxz) maxz = a[2];

            o_xyz = new int[] { minx + ((maxx - minx) / 2), (miny + (maxy - miny) / 2), (minz + (maxz - minz) / 2) };

            mfy = vectors.normal(new int[] { ((maxx - minx) / 2), ((maxy - miny) / 2) });
            mfz = vectors.normal(new int[] { ((maxx - minx) / 2), ((maxz - minz) / 2) });
        }

        private int[] genRand(int l, int sz)
        {
            int[] r = new int[l];
            for (int i = 0; i < l; i++)
                if (n.Next(5) == 0) r[i] = n.Next(sz) - sz / 2; else r[i] = 0;
            return r;
        }

        private int[] testBound(int[] s)
        {
            int[] r = new int[s.Length];
            //test boundaries
            if (s.Length > wckMotion.ub_Huno.Length || s.Length > wckMotion.lb_Huno.Length)
            {
                Console.WriteLine("Error: s[] to long");
                return null;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] > wckMotion.ub_Huno[i])
                {
                    Console.WriteLine("Boundary s[{0}] {1}->{2}", i, s[i], wckMotion.ub_Huno[i]);
                    r[i] = wckMotion.ub_Huno[i];
                }
                else
                    if (s[i] < wckMotion.lb_Huno[i])
                    {
                        Console.WriteLine("Boundary s[{0}] {1}->{2}", i, s[i], wckMotion.lb_Huno[i]);
                        r[i] = wckMotion.lb_Huno[i];
                    }
                    else
                        r[i] = s[i];
            }
            return r;
        }

        private double fitness(int[] a, double f)
        {
            return vectors.normal(a) / f;
        }

        public void syncSendServos(int[] s)
        {
            Console.WriteLine("DBG: " + vectors.str(s));

            s = testBound(s);

            servos = vectors.convByte(s);

            // ok ready to send !
        }

        public bool update(int[] xyz, int[] cp, ref string s1, ref string s2)
        {
            int[] m ;
            // calibrate mode
            
            if (count++ < L)  
                calibrateXYZ(xyz);

            // learning mode

            int[] d = vectors.sub(xyz, o_xyz);      // how far are we off centre?

            Console.WriteLine("xyz = {0}, o_xyz={1}, d={2}, norm={3}",
                vectors.str(xyz), vectors.str(o_xyz), vectors.str(d), vectors.normal(d));

            int[] ra = genRand(MAXSERVOS, MAXRANGE);
            Console.WriteLine("R= {0}", vectors.str(ra));

            //search for match and get difference

            double matchmin;

            int indx = db.match_dbi(d, out matchmin);

            Console.WriteLine("bestmatch {0} = {1}", indx, matchmin/mfz);

            if (indx >= 0 && matchmin<mfz)
            {
                m = db.getRow(indx).outputs;
                s1 += " M=" + vectors.str(m) + " fit=" + db.getRow(indx).fit;
                cp = vectors.add(cp, m);
            }
            else
            {
                db.store_db(d, ra, 1.0);
                s1 += " M= No";
            }

            // add difference to cp convert to servo array and send to servos

            syncSendServos(cp);

            // check and store fitness

            double ft = fitness(d, mfz);
            s2 = String.Format("fit= {0:0.##}", ft);            
            
            // store

            if (o_d != null)
            {
                int n = db.find_db(o_d);
                if (n >= 0)
                    db.update_db(n, ft);
                else
                    Console.WriteLine("error not found {0}", n);
            }

            o_d = (int[])d.Clone();

            return true;
        }
    }
}
