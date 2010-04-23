using System;
using System.IO;
using System.IO.Ports;

namespace RobobuilderLib
{
    public class RBC
    {
        public string serialNumber = "";

        static void Main()
        {
            Console.WriteLine("RobobuilderLib test client");

            try
            {
                PCremote r = new PCremote("COM3");

                Console.WriteLine("Ver={0}", r.readVer());

                int[] n = r.readXYZ();
                Console.WriteLine("Acc={0},{1},{2}", n[0], n[1], n[2]);

                Console.WriteLine("PSD={0}", r.readPSD());

                wckMotion w = new wckMotion(r);
     
                w.PlayFile("walk2.csv");

            }
            catch (Exception e)
            {
                Console.WriteLine("Test failed = " + e.Message);
            }

            Console.WriteLine("Finished - press a key"); while (!Console.KeyAvailable) ;

        }
    }
}
