using System;
using System.Collections.Generic;
using System.Text;
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
                SerialPort p = new SerialPort(pn, 115200, Parity.None, 8, StopBits.One);

                PCremote pcr = new PCremote(p);

                p.Open();

                serialNumber = pcr.readSN();

                p.Close();

                return 1;
            }
            catch (Exception e1)
            {
                return 0;
            }
        }
    }
}
