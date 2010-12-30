using System;
using System.IO;
using System.IO.Ports;

namespace RobobuilderLib
{
    public class RBC
    {
        /*
         * test harness for the library
         * 
         */
        static public void ntest()
        {
            Console.WriteLine("Neuron Test");
            Neuron n = new Neuron(1);
            n.mklayer(1, 4);
            n.train(new bool[] { false, true, false, true });
            n.recog(new bool[] { false, true, false, true });
            n.recog(new bool[] { true, true, false, true });
            n.recog(new bool[] { false, true, false, false });
        }

        static public void mtest()
        {
            matrix matA = new matrix(2, 3);
            matA.set(new object[] { 1.0, 4.0, 2.0, 5.0, 3.0, 6.0 });
            matA.print();


            matrix matB = new matrix(3, 2);
            matB.set(new object[] { 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 });
            matB.print();

            matA.multiply(matB).print();

            matA = new matrix(2, 3);
            matA.set(new object[] { 0.1, 0.2, 0.3, 0.1, 0.4, 0.2 });
            matA.print();

            matA.load("test.csv");


            matB = new matrix(1, 2);
            matB.set(new object[] { 1.0, 2.0 });
            matB.print();

            matA.multiply(matB).print();
        }


        // *****************************************************************************************
        // test data / routines
        // *****************************************************************************************

        static public void vtest()
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

        /********************************************************************************/

        static public void ftest()
        {
            double[] d;

            Console.WriteLine("FFT test");
            d = new double[] { 20.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            Console.WriteLine("fft data=" + vectors.str(d));
            Console.WriteLine("fft out=" + vectors.str(vectors.fft(d)));


            Console.WriteLine("FFT test");
            d = new double[] { 5.0, 5.0, 5.0, 5.0, -5.0, -5.0, -5.0, -5.0 };
            Console.WriteLine("fft data=" + vectors.str(d));
            Console.WriteLine("fft out=" + vectors.str(vectors.fft(d)));

            d = new double[] { 2.0, 2.0, 2.0, 2.0, -2.0, -2.0, -2.0, -2.0 };
            Console.WriteLine("\nfft data=" + vectors.str(d));
            Console.WriteLine("fft out=" + vectors.str(vectors.fft(d)));

            d = new double[] { 5.0, 5.0, -5.0, -5.0, 5.0, 5.0, -5.0, -5.0 };
            Console.WriteLine("\nfft data=" + vectors.str(d));
            Console.WriteLine("fft out=" + vectors.str(vectors.fft(d)));

            Console.WriteLine("f   =" + vectors.str(vectors.fft(new double[] { 3.53545201,	4.999999995, 3.535779582,	0.000463268,	-3.535124407,	-4.999999952,	-3.536107124,	-0.000926536 })));
            Console.WriteLine("2f  =" + vectors.str(vectors.fft(new double[] { 4.999999995,	0.000463268,-4.999999952,	-0.000926536,	4.999999866,	0.001389804,	-4.999999737,	-0.001853072 })));
            Console.WriteLine("3f2 =" + vectors.str(vectors.fft(new double[] { 4.619331178,	3.535779582,-1.912935647,	-4.999999952,	-1.91421964,	3.534796774,	4.619862899,	0.001389804 })));
            Console.WriteLine("f+2f=" + vectors.str(vectors.fft(new double[] { 2.553101005,	4.070749997, 4.124014791,	3.141731634,	2.159312796,	2.212250024,	3.729571438,	6.282536732 })));
        }

        /********************************************************************************/

        static public void itest()
        {
            Console.WriteLine("I2C test");

            wckMotion wmt = new wckMotion("COM5", true);

            // acc init
            if (!wmt.I2C_write(0x70, new byte[] { 0x14, 0x03 }))
            {
                Console.WriteLine("I2C test failed");
            }

            for (int z = 0; z < 20; z++)
            {
                // acc get
                wmt.I2C_write(0x70, new byte[] { 0x02 });
                byte[] ib = wmt.I2C_read(0x71, new byte[] { }, 6);

                Console.WriteLine("{0}, {1}, {2}", wmt.cbyte(ib[1]), wmt.cbyte(ib[3]), wmt.cbyte(ib[5]));

                wmt.delay_ms(250);
            }
        }

        /********************************************************************************/

        static public void dtest()
        {
            int[] a = new int[] { 1, 2, 3 };
            int[] b = new int[] { 3, 2, 4 };

            Console.WriteLine("Data gen test");

            Console.WriteLine("DB test");
            dbase db = new dbase();

            db.store_db(a, b, 1.0);
            db.show_db();

            db.store_db(new int[] { 1, 2, 3 }, b, 2.0);
            int f = db.find_db(a);
            Console.WriteLine("f=" + f);
            if (f >= 0) db.update_db(f, 3.0);

            db.show_db();
        }

        static public bool dotest(string s)
        {
            Console.Write("Run test  : " + s + " [Y | N]? ");

            char ch = '\0';
            while (ch != 'Y' && ch != 'N')
            {
                while (!Console.KeyAvailable) ;
                ch = Char.ToUpper(Console.ReadKey(true).KeyChar);
            }
            Console.WriteLine(ch);
            return (ch == 'Y');
        }

        static void Main(string[] argv)
        {
            string fn = "";
            string pn = "COM5";

            if (argv.Length > 0) pn = argv[0];
            if (argv.Length > 1) fn = argv[1];

            String strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

            Console.WriteLine("RobobuilderLib {2} :: {0} :: file='{1}'", pn, fn, strVersion);

            try
            {
                if (dotest("Vectors")) vtest();
                if (dotest("FFT"))     ftest();
                if (dotest("DBase"))   dtest();
                if (dotest("Matrix"))  mtest(); 
                if (dotest("Neuron"))  ntest();
                if (dotest("I2C"))     itest();

                PCremote r = new PCremote(pn);

                if (dotest("PC remote"))
                {
                    Console.WriteLine("Ver={0}", r.readVer());

                    int[] n = r.readXYZ();
                    Console.WriteLine("Acc={0},{1},{2}", n[0], n[1], n[2]);

                    Console.WriteLine("PSD={0}", r.readPSD());
                }

                if (dotest("DC mode"))
                {
                    wckMotion w = new wckMotion(r);

                    if (w.wckReadPos(30))
                    {
                        Console.WriteLine("Servo 30 detected - DCMP mode");
                        w.DCMP = true;
                    }

                    if (dotest("Play File"))    w.PlayFile(fn);
                    if (dotest("Set Pose"))     w.PlayPose(1000, 10, wckMotion.basic18, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Test failed = " + e.Message);
            }

            Console.WriteLine("Finished - press a key"); while (!Console.KeyAvailable) ;

        }
    }


}
