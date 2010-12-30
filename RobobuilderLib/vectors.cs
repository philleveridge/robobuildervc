using System;
using System.IO;

namespace RobobuilderLib
{

    struct ComplexNumber
    {
        public double Re;
        public double Im;

        public ComplexNumber(double re)
        {
            this.Re = re;
            this.Im = 0;
        }

        public ComplexNumber(double re, double im)
        {
            this.Re = re;
            this.Im = im;
        }

        public static ComplexNumber operator *(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re * n2.Re - n1.Im * n2.Im,
                n1.Im * n2.Re + n1.Re * n2.Im);
        }

        public static ComplexNumber operator +(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re + n2.Re, n1.Im + n2.Im);
        }

        public static ComplexNumber operator -(ComplexNumber n1, ComplexNumber n2)
        {
            return new ComplexNumber(n1.Re - n2.Re, n1.Im - n2.Im);
        }

        public static ComplexNumber operator -(ComplexNumber n)
        {
            return new ComplexNumber(-n.Re, -n.Im);
        }

        public static implicit operator ComplexNumber(double n)
        {
            return new ComplexNumber(n, 0);
        }

        public ComplexNumber PoweredE()
        {
            double e = Math.Exp(Re);
            return new ComplexNumber(e * Math.Cos(Im), e * Math.Sin(Im));
        }

        public double Power2()
        {
            return Re * Re - Im * Im;
        }

        public double AbsPower2()
        {
            return Re * Re + Im * Im;
        }

        public override string ToString()
        {
            return String.Format("{0}+i*{1}", Re, Im);
        }
    }

    public static class FftAlgorithm
    {
        /// <summary>
        /// Calculates FFT using Cooley-Tukey FFT algorithm.
        /// </summary>
        /// <param name="x">input data</param>
        /// <returns>spectrogram of the data</returns>
        /// <remarks>
        /// If amount of data items not equal a power of 2, then algorithm
        /// automatically pad with 0s to the lowest amount of power of 2.
        /// </remarks>
        public static double[] Calculate(double[] x)
        {
            int length;
            int bitsInLength;
            if (IsPowerOfTwo(x.Length))
            {
                length = x.Length;
                bitsInLength = Log2(length) - 1;
            }
            else
            {
                bitsInLength = Log2(x.Length);
                length = 1 << bitsInLength;
                // the items will be pad with zeros
            }

            // bit reversal
            ComplexNumber[] data = new ComplexNumber[length];
            for (int i = 0; i < x.Length; i++)
            {
                int j = ReverseBits(i, bitsInLength);
                data[j] = new ComplexNumber(x[i]);
            }

            // Cooley-Tukey 
            for (int i = 0; i < bitsInLength; i++)
            {
                int m = 1 << i;
                int n = m * 2;
                double alpha = -(2 * Math.PI / n);

                for (int k = 0; k < m; k++)
                {
                    // e^(-2*pi/N*k)
                    ComplexNumber oddPartMultiplier = new ComplexNumber(0, alpha * k).PoweredE();

                    for (int j = k; j < length; j += n)
                    {
                        ComplexNumber evenPart = data[j];
                        ComplexNumber oddPart = oddPartMultiplier * data[j + m];
                        data[j] = evenPart + oddPart;
                        data[j + m] = evenPart - oddPart;
                    }
                }
            }

            // calculate spectrogram
            double[] spectrogram = new double[length];
            for (int i = 0; i < spectrogram.Length; i++)
            {
                spectrogram[i] = data[i].AbsPower2();
            }
            return spectrogram;
        }

        /// <summary>
        /// Gets number of significat bytes.
        /// </summary>
        /// <param name="n">Number</param>
        /// <returns>Amount of minimal bits to store the number.</returns>
        private static int Log2(int n)
        {
            int i = 0;
            while (n > 0)
            {
                ++i; n >>= 1;
            }
            return i;
        }

        /// <summary>
        /// Reverses bits in the number.
        /// </summary>
        /// <param name="n">Number</param>
        /// <param name="bitsCount">Significant bits in the number.</param>
        /// <returns>Reversed binary number.</returns>
        private static int ReverseBits(int n, int bitsCount)
        {
            int reversed = 0;
            for (int i = 0; i < bitsCount; i++)
            {
                int nextBit = n & 1;
                n >>= 1;

                reversed <<= 1;
                reversed |= nextBit;
            }
            return reversed;
        }

        /// <summary>
        /// Checks if number is power of 2.
        /// </summary>
        /// <param name="n">number</param>
        /// <returns>true if n=2^k and k is positive integer</returns>
        private static bool IsPowerOfTwo(int n)
        {
            return n > 1 && (n & (n - 1)) == 0;
        }
    }


    public class vectors
    {

        static public void swap(ref double a, ref double b)
        {
            double t = a; a = b; b = t;
        }

        static public double[] fft(double[] signal)
        {
            return FftAlgorithm.Calculate(signal);
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

    }

}
