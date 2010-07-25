using System;
using System.IO;

namespace RobobuilderLib
{


    public class vectors
    {


        static public void swap(ref double a, ref double b)
        {
            double t = a; a = b; b = t;
        }

        static public double[] fft(double[] signal)
        {
            /* 
             * from Dr Dobbs 2007 c++ implementation of fft
             * The initial signal is stored in the array data of length 2*nn, 
             * where each even element corresponds to the real part and each 
             * odd element to the imaginary part of a complex number. 
             */

            int n, mmax, m, j, istep, i;
            double wtemp, wr, wpr, wpi, wi, theta;
            double tempr, tempi;

            double[] data = new double[signal.Length*2];
            for (i=0; i<signal.Length; i++)
            {
                data[2*i]=signal[i];
            }

            int nn = data.Length/2;

            // NOTE:::  nn MUST be Power of two = need check
            
            double p = Math.Log((double)nn, 2.0);
            if (p != Math.Floor(p)) return null;

            // reverse-binary reindexing
            n = nn<<1;
            j=1;
            for (i=1; i<n; i+=2) {
                if (j>i) {
                    swap(ref data[j-1], ref data[i-1]);
                    swap(ref data[j], ref data[i]);
                }
                m = nn;
                while (m>=2 && j>m) {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            };

            // here begins the Danielson-Lanczos section
            mmax=2;
            while (n>mmax) {
                istep = mmax<<1;
                theta = -(2 * Math.PI / mmax);  // -(2 * M_PI / mmax);
                wtemp = Math.Sin(0.5*theta);
                wpr = -2.0*wtemp*wtemp;
                wpi = Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m=1; m < mmax; m += 2) {
                    for (i=m; i <= n; i += istep) {
                        j=i+mmax;
                        tempr = wr*data[j-1] - wi*data[j];
                        tempi = wr * data[j] + wi*data[j-1];

                        data[j-1] = data[i-1] - tempr;
                        data[j] = data[i] - tempi;
                        data[i-1] += tempr;
                        data[i] += tempi;
                    }
                    wtemp=wr;
                    wr += wr*wpr - wi*wpi;
                    wi += wi*wpr + wtemp*wpi;
                }
                mmax=istep;
            }
            double max = 0.0;
            int f = 0;
            double[] output = new double[signal.Length / 2]; // first half only - second half is a reflection
            output[0] = 0.0; // drop DC component
            for (i = 1; i < signal.Length/2; i++)
            {
                // absolute value sqrt(real^2 + complex^2)
                output[i] = Math.Sqrt((data[2 * i] * data[2 * i]) + (data[2 * i + 1] * data[2 * i + 1]));
                if (output[i] > max) { max = output[i]; f = i; }
            }
            if (max > 0)
            {
                for (i = 0; i < signal.Length / 2; i++)
                {
                    output[i] = output[i] / max; // n ormalise
                }
            }
            return output; //absolute value normalised, with out DC
        }

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

        static public string str(byte[] a)
        {
            if (a == null) return "null";

            string s = String.Format("{0}", a[0]);
            for (int i = 1; i < a.Length; i++)
            {
                s += String.Format(", {0}", a[i]);
            }
            return s;
        }

        static public string str(double[] a)
        {
            if (a == null) return "null";

            string s = String.Format("{0}", a[0]);
            for (int i = 1; i < a.Length; i++)
            {
                s += String.Format(", {0:0.##}", a[i]);
            }
            return s;
        }

        static public int[] convInt(byte[] a)
        {
            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToInt32(a[i]);
            }
            return r;
        }

        static public int[] convInt(Object[] a)
        {
            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToInt32(a[i]);
            }
            return r;
        }

        static public int[] convInt(Double[] a)
        {
            int[] r = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToInt32(a[i]);
            }
            return r;
        }

        static public double[] random(int n, double size, double offset)
        {
            double[] r = new double[n];
            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < n; i++)
            {
                r[i] = size*rnd.NextDouble()-offset;
            }
            return r;
        }

        static public double maxValue(double[] a)
        {
            double r=a[0];
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > r) r = a[i];
            }
            return r;
        }

        static public double maxValue(int[] a)
        {
            int r = a[0];
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > r) r = a[i];
            }
            return r;
        }

        static public int maxItem(double[] a)
        {
            double r = a[0];
            int n = 0;
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > r)
                {
                    r = a[i];
                    n = i;
                }
            }
            return n;
        }

        static public int maxItem(int[] a)
        {
            int r = a[0];
            int n = 0;
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > r)
                {
                    r = a[i];
                    n = i;
                }
            }
            return n;
        }

        static public int[] random(int n, int size, int offset)
        {
            int[] r = new int[n];
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                r[i] = (int)Math.Floor (size*rnd.NextDouble()) - offset;
            }
            return r;
        }

        static public double[] convDouble(Object[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToDouble(a[i]);
            }
            return r;
        }

        static public double[] convDouble(int[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToDouble(a[i]);
            }
            return r;
        }

        static public double[] convDouble(byte[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                r[i] = Convert.ToDouble(a[i]);
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

        static public byte[] convByte(double[] a)
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

        static public int[] add(int[] a, byte[] b)
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

        static public double[] add(double[] a, double[] b)
        {
            if (a == null) return null;

            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (b == null) r[i] = a[i];
                else if (i < b.Length) r[i] = a[i] + b[i];
            }
            return r;
        }
        static public double sum(double[] a)
        {
            if (a == null) return 0.0;
            double r = 0;

            for (int i = 0; i < a.Length; i++)
            {
               r += a[i] ;
            }
            return r;
        }

        static public double rms(double[] a, double[] b)
        {
            if (a == null || b==null ) return 0.0;
            double rms=0;

            for (int i = 0; i < a.Length; i++)
            {
                double d = 0;
                if (i < b.Length)
                    d = a[i] - b[i];

                rms += (d * d);
            }
            rms = Math.Sqrt(rms / a.Length);
            return rms;
        }

        static public int sum(int[] a)
        {
            if (a == null) return 0;
            int r = 0;

            for (int i = 0; i < a.Length; i++)
            {
                r += a[i];
            }
            return r;
        }

        static public int average(int[] a)
        {
            return sum(a) / a.Length;
        }

        static public double average(double[] a)
        {
            return sum(a) / a.Length;
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

        static public double[] append(double[] a, double[] b)
        {
            double[] r = new double[a.Length + b.Length];
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

        static public double[] scale(double[] a, double[] b, double s)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (i < b.Length) r[i] = a[i] * b[i]; else r[i] = a[i]*s;
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

        static public double[] sub(double[] a, double[] b)
        {
            if (a == null) return null;

            double[] r = new double[a.Length];
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

        static public double dotprod(double[] a, double[] b)
        {
            double r = 0;
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

        static public double normal(double[] a)
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

        static public byte[] bcheck(int[] p, byte[] min, byte[] max)
        {
            byte[] r = new byte[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                if (i < min.Length && i < max.Length)
                    r[i] = (byte)((p[i] > max[i]) ? max[i] : ((p[i] < min[i]) ? min[i] : p[i]));
            }
            return r;
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
            double[] d = new double[] { 5.0, 5.0, 5.0, 5.0, -5.0, -5.0, -5.0, -5.0 };

            Console.WriteLine("fft data=" + vectors.str(d)); 
            Console.WriteLine("fft out="  + vectors.str(vectors.fft(d)));
        }
    }

    public class matrix
    {
        double[,] mat;
        int cols = 0;
        int rows = 0;

        public matrix(int c, int r)
        {
            mat = new double[c, r];
            cols = c;
            rows = r;
        }

        public matrix(int c, int r, double[] a)
        {
            mat = new double[c, r];
            cols = c;
            rows = r;
            set(a);
        }

        public matrix(int c, int r, object[] a)
        {
            mat = new double[c, r];
            cols = c;
            rows = r;
            set(a);
        }

        public matrix(int s)
        {
            mat = new double[s, s];
            cols = s;
            rows = s;
            for (int i = 0; i < s; i++) mat[i, i] = 1.0;
        }

        public matrix resize(int nc, int nr)
        {
            matrix nmat = new matrix(nc,nr);

            for (int i=0; i<nc; i++)
                for (int j = 0; j < nr; j++)
                {
                    if (i < cols && j < rows)
                        nmat.set(i, j, mat[i, j]);
                    else
                        nmat.set(i, j, 0.0);
                }
            return nmat;
        }

        public double get(int c, int r)
        {
            return mat[c, r];
        }

        public int getc()
        {
            return cols;
        }

        public int getr()
        {
            return rows;
        }

        public double[] getrow(int r)
        {
            if (r < 0 || r >= rows) return null;

            double[] a = new double[cols];
            for (int i = 0; i < cols; i++)
                a[i] = mat[i, r];
            return a;
        }

        public double[] getcol(int c)
        {
            if (c < 0 || c >= cols) return null;

            double[] a = new double[rows];
            for (int i = 0; i < rows; i++)
                a[i] = mat[c, i];
            return a;
        }

        public double[] getAll()
        {
            double[] a = new double[cols*rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++) 
                    a[i] = mat[j, i];
            return a;
        }

        public bool set(double[] n)
        {
            if (n.Length != (cols * rows))
            {
                Console.WriteLine("Error - length={0} R={1}, C={2}", n.Length, rows, cols);
                return false;
            }
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[j, i] = n[i * cols + j];
            return true;
        }

        public bool set(Object[] n)
        {
            if (n.Length != (cols * rows))
            {
                Console.WriteLine("Error - length={0} R={1}, C={2}", n.Length, rows, cols);
                return false;
            }
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[j, i] = Convert.ToDouble(n[i*cols + j]);
            return true;
        }

        public void set(int c, int r, double v)
        {
            if (!(c < 0 || c >= cols || r < 0 || r >= rows))
                mat[c, r] = v;
        }

        public void delta(int c, int r, double v)
        {
            if (!(c < 0 || c >= cols || r < 0 || r >= rows))
                mat[c, r] += v;
        }

        public void print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Console.Write("{0:0.#} ", mat[j, i]);
                Console.WriteLine("");
            }
        }

        public bool load(string filename)
        {
            string[] inpt;
            double[] data = new double[rows*cols];
            int c = 0;

            try
            {
                inpt = File.ReadAllLines(filename);
                for (int i = 0; i < inpt.Length; i++)
                {
                    if (inpt[i].StartsWith("#"))
                        continue;
                    string[] v = inpt[i].Split(',');
                    for (int j = 0; j < v.Length; j++)
                    {
                        try
                        {
                            if (v[j] != null && v[j]!= "" && c<rows*cols) 
                                data[c++] = Convert.ToDouble(v[j]);
                        }
                        catch
                        {
                            Console.WriteLine("{0} - Can't convert to double {1}", c, v[j]);
                            data[c++] = 0.0;
                        }
                    }
                }
                if (!set(data))
                {
                    Console.WriteLine("Data import failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File load failed {0} - {1}", filename, e.Message );
                return false;
            }
            return true;
        }

        public bool save(string filename)
        {
            string outp = "";
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    outp += String.Format("{0},", mat[j, i]);
                outp += "\r\n";
            }
            try
            {
                File.WriteAllText(filename, outp);
            } 
            catch 
            {
                return false;

            }
            return true;
        }

        public bool add(matrix x)
        {
            if (x.getc() != cols || x.getr() != rows)
                return false;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[j, i] += x.get(j, i);

            return true;
        }

        public bool subtract(matrix x)
        {
            if (x.getc() != cols || x.getr() != rows)
                return false;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[j, i] -= x.get(j, i);

            return true;
        }

        public bool zeroI()
        {
            if (cols != rows)
                return false;
            for (int s = 0; s < cols; s++) 
                mat[s, s] = 0;
            return true;
        }

        public matrix multiply(matrix x)
        {
            int xc = x.getc();
            int xr = x.getr();

            if (xr != cols )
                return null;

            matrix r = new matrix(xc, rows);

            for (int i=0; i<rows; i++)
                for (int j=0; j<xc; j++)
                {
                    double f = 0;

                    for (int k = 0; k < cols; k++)
                    {
                        f += (mat[k,i] * x.get(j,k));
                    }

                    r.set(j, i, f);
                }

            return r;
        }

        public void scale(double sf)
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    mat[j, i] *= sf;
        }

        public void deltaRow(int rownum, double sf)
        {
            if (rownum >= 0 && rownum < rows)
            {
                for (int j = 0; j < cols; j++)
                    mat[j, rownum] += sf;
            }
        }

        public void deltaCol(int colnum, double sf)
        {
            if (colnum >= 0 && colnum < cols)
            {
                for (int j = 0; j < rows; j++)
                    mat[colnum,j] += sf;
            }
        }

        static public double[] bipolar(bool[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = a[i] ? 1.0 : -1.0;
            return r;
        }

        static public double[] bipolar(object[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = (Convert.ToInt32(a[i])==1) ? 1.0 : -1.0;
            return r;
        }

        static public bool[] revbipolar(double[] a)
        {
            bool[] r = new bool[a.Length];
            for (int i = 0; i < a.Length; i++)
                r[i] = (a[i] >0);
            return r;
        }

        public matrix transpose()
        {
            matrix r = new matrix(rows, cols);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r.set(i,j, mat[j, i]);
            return r;
        }

        static public void test()
        {
            matrix matA = new matrix ( 2, 3);
            matA.set (new object[] { 1.0, 4.0, 2.0, 5.0, 3.0, 6.0});
            matA.print();


            matrix matB = new matrix ( 3, 2);
            matB .set (new object[] { 7.0, 8.0, 9.0, 10.0, 11.0, 12.0});
            matB.print();

            matA.multiply(matB).print();

            matA = new matrix(2,3);
            matA.set(new object[] {0.1, 0.2, 0.3, 0.1, 0.4, 0.2 });
            matA.print();

            matA.load("test.csv");


            matB = new matrix(1,2);
            matB.set(new object[] { 1.0, 2.0 });
            matB.print();

            matA.multiply(matB).print();
        }

    }

}
