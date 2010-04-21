using System;
using System.IO;
using System.IO.Ports;

namespace RobobuilderLib
{
    public class RBC
    {
        public string serialNumber = "";

        public RBC()
        {
        }

        public int connect(string pn)
        {
            try
            {

                PCremote pcr = new PCremote(pn);

                pcr.serialPort.Open();

                serialNumber = pcr.readSN();

                pcr.Close();

                return 1;
            }
            catch (Exception e1)
            {
                return 0;
            }
        }
    }
}
