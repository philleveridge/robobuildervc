using System;
using System.Collections.Generic;
using System.Text;

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
    class vectors
    {
        static public string str(int[] a)
        {
            if (a == null) return "null";

            string s = String.Format("{0}", a[0]);
            for (int i = 1; i < a.Length; i++)
            {
                s += String.Format(", {0}", a[i]);
            }
            return s;
        }

        static public int[] convInt(byte[] a)
        {
            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = (int) a[i];
            }
            return r;
        }

        static public byte[] convByte(int[] a)
        {
            if (a == null) return null;
            byte[] r = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < 0)    r[i] = 0;
                if (a[i] >  255) r[i] = 255;
                if (a[i] >= 0 && a[i] <= 255) r[i] = (byte)a[i];
            }
            return r;
        }

        static public bool equals(int[] a, int[] b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        static public int compare(int[] a, int[] b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            if (a.Length != b.Length) return (a.Length - b.Length);

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return (a[i] - b[i]);
            }
            return 0;
        }
        
        static public int[] add(int[] a, int[] b)
        {
            if (a == null) return null;

            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (b == null) r[i] = a[i];
                    else if (i < b.Length) r[i] = a[i] + b[i];
            }
            return r;
        }

        static public int[] append(int[] a, int[] b)
        {
            int[] r = new int[a.Length+b.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = a[i];
            }
            for (int i = 0; i < b.Length; i++)
            {
                r[i+a.Length] = b[i];
            } 
            return r;
        }

        static public int[] sub(int[] a, int[] b)
        {
            if (a == null) return null;

            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (b == null) r[i] = a[i];
                else
                    if (i < b.Length) r[i] = a[i] - b[i];
            }
            return r;
        }

        static public int dotprod(int[] a, int[] b)
        {
            int r = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (i < b.Length) r += (a[i] * b[i]); 
            }
            return r;
        }

        static public double normal(int[] a)
        {
            return Math.Sqrt((double)dotprod(a, a));
        }

        static public int[] match(int[][] a, int[] b)
        {
            double min = normal(sub(a[0], b));
            int[] res = a[1];

            for (int i = 2; i < a.Length; i += 2)
            {
                double t = normal(sub(a[i], b));
                if (t < min)
                {
                    t = min;
                    res = a[i + 1];
                }
            }
            return res;
        }

        // *****************************************************************************************
        // test data / routines
        // *****************************************************************************************

        static public void test()
        {
            int[] a = new int[] { 1, 2, 3 };
            int[] b = new int[] { 3, 2, 4 };

            int[][] testv = new int[][] {  
            new int[] {0, 0, 5},  
            new int[] {0, 0, 4, 0, 0, 0, 0, -4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},

            new int[] {0, 0, 2},  
            new int[] {0, 0, 2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},

            new int[] {0, 0, -5},  
            new int[] {0, 0, -2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            } ;

            Console.WriteLine("Vectors test");
            Console.WriteLine("a="    + vectors.str(a));
            Console.WriteLine("b="    + vectors.str(b));
            Console.WriteLine("a&b="  + vectors.str(vectors.append(a, b)));
            Console.WriteLine("a+b="  + vectors.str(vectors.add(a, b)));
            Console.WriteLine("a-b="  + vectors.str(vectors.sub(a, b)));
            Console.WriteLine("a.b="  + vectors.dotprod(a, b));
            Console.WriteLine("|a|="  + vectors.normal(a));
            Console.WriteLine("M(a)=" + vectors.str(vectors.match(testv, a)));
            Console.WriteLine("M(b)=" + vectors.str(vectors.match(testv, b)));
        }
    }



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

        // private data
        struct db
        {
            public double fit;
            public int[] inputs;
            public int[] outputs;
        };

        List<db> database = new List<db>();

        int count;

        Random n = new Random();

        int[] ub_Huno = new int[] {
        /* ID
          0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
        174,228,254,130,185,254,180,126,208,208,254,224,198,254,228,254};

        int[] lb_Huno = new int[] {
        /* ID
          0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
          1, 70,124, 40, 41, 73, 22,  1,120, 57,  1, 23,  1,  1, 25, 40};

        // constructor

        public Autonomy() 
        {
            init();
        }

        // *****************************************************************************************
        //   database functions
        // *****************************************************************************************

        public void clear_db()
        {
            database.Clear();
        }

        public int match_dbi(int[] b, out double min)
        {
            min = 0.0;
            if (database.Count < 2) return -1;

            min = vectors.normal(vectors.sub(database[0].inputs, b));
            int r = 0;

            for (int i = 1; i < database.Count; i++)
            {
                double t = vectors.normal(vectors.sub(database[i].inputs, b));
                if (t < min)
                {
                    min = t ;
                    r=i;
                }
            }
            return r;
        }

        public int find_db(int[] a)
        {
            int r = -1;
            for (int i = 0; i < database.Count; i++)
            {
                if (vectors.equals(a,database[i].inputs))
                    return i;
            }
            return r;
        }

        public void update_db(int index, double x)
        {
            db entry = new db();
            entry.fit = x;
            entry.inputs = database[index].inputs;
            entry.outputs = database[index].outputs;

            update_db(index, entry);       
        }

        public void update_db(int index, int[] outp)
        {
            db entry = new db();
            entry.fit = database[index].fit;
            entry.inputs = database[index].inputs;
            entry.outputs = outp;

            update_db(index, entry);
        }

        private void update_db(int index, db entry)
        {
            database.RemoveAt(index);
            database.Insert(index, entry);
        }

        public void store_db(int[] a, int[] b, double x)
        {
            db entry      = new db();
            entry.fit     = x;
            entry.inputs  = a;
            entry.outputs = b;

            if (find_db(a) >= 0)
            {
                Console.WriteLine("already exists");
            }
            else
            {
                database.Add(entry);
            }
        }

        private int compare(db a, db b)
        {
            return (vectors.compare(a.inputs, b.inputs));
        }

        public void show_db()
        {
            database.Sort(compare);
            for (int i = 0; i < database.Count; i++)
            {
                Console.WriteLine("[{0}] {3:0.0} ({1}) ({2})", i,
                    vectors.str(database[i].inputs),
                    vectors.str(database[i].outputs),
                    database[i].fit);
            }
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

        public void test()
        {
            vectors.test();

            int[] a = new int[] { 1, 2, 3 };
            int[] b = new int[] { 3, 2, 4 };

            Console.WriteLine("Data gen test");
            Console.WriteLine("acc(a)  =" + vectors.str(readAcc()));
            Console.WriteLine("servo(a)=" + vectors.str(readServos()));

            Console.WriteLine("DB test");
            store_db(a,b, 1.0);
            store_db(readAcc(), b, 2.0);
            store_db(readAcc(), b, 2.0);
            show_db();

            store_db(new int[] {1,2,3}, b, 2.0);
            int f = find_db(a);
            Console.WriteLine("f=" + f);
            if (f>=0) update_db(f, 3.0);

            show_db();
        }

        // *****************************************************************************************
        // main routines
        // *****************************************************************************************

        public void init()
        {
            clear_db();

            o_xyz = null;
            o_d = null;

            minx = 255; maxx = -255;
            miny = 255; maxy = -255;
            minz = 255; maxz = -255;
            mfy = 1.0;
            mfz = 1.0;

            count = 0;
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
            if (s.Length > ub_Huno.Length || s.Length > lb_Huno.Length)
            {
                Console.WriteLine("Error: s[] to long");
                return null;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] > ub_Huno[i])
                {
                    Console.WriteLine("Boundary s[{0}] {1}->{2}", i, s[i], ub_Huno[i]);
                    r[i] = ub_Huno[i];
                }
                else
                    if (s[i] < lb_Huno[i])
                    {
                        Console.WriteLine("Boundary s[{0}] {1}->{2}", i, s[i], lb_Huno[i]);
                        r[i] = lb_Huno[i];
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

            int indx = match_dbi(d, out matchmin);

            Console.WriteLine("bestmatch {0} = {1}", indx, matchmin/mfz);

            if (indx >= 0 && matchmin<mfz)
            {
                m = database[indx].outputs;
                s1 += " M=" + vectors.str(m) + " fit=" + database[indx].fit;
                cp = vectors.add(cp, m);
            }
            else
            {
                store_db(d, ra, 1.0);
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
                int n = find_db(o_d);
                if (n >= 0)
                    update_db(n, ft);
                else
                    Console.WriteLine("error not found {0}", n);
            }

            o_d = (int[])d.Clone();

            return true;
        }
    }
}
