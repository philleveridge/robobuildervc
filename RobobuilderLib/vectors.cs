using System;

namespace RobobuilderLib
{
    public class vectors
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
                r[i] = (int)a[i];
            }
            return r;
        }

        static public int[] convInt(Object[] a)
        {
            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = (int)a[i];
            }
            return r;
        }

        static public byte[] convByte(int[] a)
        {
            if (a == null) return null;
            byte[] r = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < 0) r[i] = 0;
                if (a[i] > 255) r[i] = 255;
                if (a[i] >= 0 && a[i] <= 255) r[i] = Convert.ToByte(a[i]);
            }
            return r;
        }

        static public byte[] convByte(Object[] a)
        {
            if (a == null) return null;
            byte[] r = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if ((int)a[i] < 0) r[i] = 0;
                if ((int)a[i] > 255) r[i] = 255;
                if ((int)a[i] >= 0 && (int)a[i] <= 255) r[i] = Convert.ToByte(a[i]);
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
            int[] r = new int[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = a[i];
            }
            for (int i = 0; i < b.Length; i++)
            {
                r[i + a.Length] = b[i];
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
            };

            Console.WriteLine("Vectors test");
            Console.WriteLine("a=" + vectors.str(a));
            Console.WriteLine("b=" + vectors.str(b));
            Console.WriteLine("a&b=" + vectors.str(vectors.append(a, b)));
            Console.WriteLine("a+b=" + vectors.str(vectors.add(a, b)));
            Console.WriteLine("a-b=" + vectors.str(vectors.sub(a, b)));
            Console.WriteLine("a.b=" + vectors.dotprod(a, b));
            Console.WriteLine("|a|=" + vectors.normal(a));
            Console.WriteLine("M(a)=" + vectors.str(vectors.match(testv, a)));
            Console.WriteLine("M(b)=" + vectors.str(vectors.match(testv, b)));
        }
    }

}
