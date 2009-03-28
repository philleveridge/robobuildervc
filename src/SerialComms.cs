using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace RobobuilderVC
{
    class SerialComms
    {
        SerialPort port;

        public SerialComms(SerialPort p)
        {
            port = p;
        }
    }
}
