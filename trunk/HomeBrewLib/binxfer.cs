using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace RobobuilderLib
{
    public class binxfer
    {
        SerialPort sp1;
        const byte MAGIC_RESPONSE = 0xEA;
        const byte MAGIC_REQUEST = 0xCD;

        public byte[] buff;

        public bool dbg = false;

        public binxfer(SerialPort s)
        {
            sp1 = s;
            sp1.WriteTimeout = 2000;
            sp1.ReadTimeout = 5000;
        }

        public void send_msg_basic(char mt)         // covers 'q', 'v', 'p'
        {
            byte[] buff = new byte[3];
            buff[0] = MAGIC_REQUEST;
            buff[1] = (byte)mt;
            buff[2] = (byte)((buff[0] ^ buff[1]) & 0x7f);   // checksum
            sp1.Write(buff, 0, 3);
            if (dbg) Console.WriteLine("DBG: sendb={0}", BitConverter.ToString(buff));
        }

        public void send_msg_move(byte[] buffer, int bfsz)
        {
            byte cs = (byte)bfsz;
            byte[] header = new byte[5];
            header[0] = MAGIC_REQUEST;
            header[1] = (byte)'m';

            header[2] = (byte)(bfsz & 0xFF);
            header[3] = (byte)((bfsz >> 8) & 0xFF);

            for (int j = 0; j < bfsz; j++)
            {
                cs ^= buffer[j];
            }
            header[4] = (byte)(cs & 0x7f);

            sp1.Write(header, 0, 4);
            sp1.Write(buffer, 0, bfsz);
            sp1.Write(header, 4, 1);
            
            if (dbg) Console.WriteLine("DBG: sendm={0}", BitConverter.ToString(buffer));
        }

        public void send_msg_raw(char mt, string abytes)
        {
            int n = abytes.Length / 2;
            int cs = n;
            byte[] header = new byte[2];
            header[0] = MAGIC_REQUEST;
            header[1] = (byte)mt;

            byte[] b = new byte[n+2];
            b[0] = (byte)(n & 0xFF);
            for (int j = 0; j < n; j++)
            {
                b[j+1] = (byte)Convert.ToInt16(abytes.Substring(j * 2, 2), 16);
                cs ^= b[j+1];
            }
            b[n + 1] = (byte)(cs & 0x7f);

            sp1.Write(header, 0, 2);

            if (dbg) Console.WriteLine("DBG: send={0}", BitConverter.ToString(b));
            sp1.Write(b, 0, n + 2);
        }

        public byte readByte()
        {
            byte b = (byte)sp1.ReadByte();
            if (dbg) Console.WriteLine("DBG: byte={0}", b);
            return b;
        }

        public int bytesToRead()
        {
            int n = sp1.BytesToRead;
            if (n>0 && dbg) Console.WriteLine("DBG: num={0}", n); 
            return n;
        }

        public int readBytes(byte[] buff, int n)
        {
            int r = sp1.Read(buff, 0, n);
            if (dbg) Console.WriteLine("DBG: bytes={0}", BitConverter.ToString(buff));
            return r;
        }
        
        public bool recv_packet()
        {
            int i;
            bool good_packet = false;
            string ignore = "";
            byte b;

            try
            {
                while ((b=readByte()) != MAGIC_RESPONSE) 
                   ignore += b; // ignore till start of message
            }
            catch (Exception e)
            {
                Console.WriteLine("Read error" + e + "ignore = [" + ignore + "]");
                return false;
            }

            byte mt = readByte();
            byte cs;

            switch (mt)
            {
                case (byte)'q':
                    // 43 packets
                    int n = readByte();
                    n = n * 2 + 11;
                    buff = new byte[n];
                    while (bytesToRead() < n) ;
                    readBytes(buff, n);

                    cs = (byte)'q';
                    for (i = 0; i < n-1; i++) 
                           cs ^= buff[i];
                    good_packet = ((cs &0x7f) == buff[n-1]);
                    break;
                case (byte)'x':
                case (byte)'X':
                    // 5 packets
                    buff = new byte[3];
                    while (bytesToRead() < 3) ;
                    readBytes(buff, 3);
                    good_packet = (((mt ^ buff[0] ^ buff[1]) &0x7f) == buff[2]);
                    break;
                case (byte)'v':
                case (byte)'m':
                case (byte)'z':
                case (byte)'P':
                // 4 bytes packets
                    buff = new byte[2];
                    while (bytesToRead() < 2) ;
                    readBytes(buff, 2);
                    good_packet = ((mt ^ buff[0]) == buff[1]);
                    if (mt == 'z')
                    {
                        Console.WriteLine("Protocol error {0}", buff[0]);
                    }
                    break;
                case (byte)'Q':
                    buff = new byte[8];
                    while (bytesToRead() < 8) ;
                    readBytes(buff, 8);
                    cs = mt;
                    for (i = 0; i < 7; i++)
                        cs ^= buff[i];
                    good_packet = ((cs & 0x7f) == buff[7]);
                    break;
                case (byte)'D':
                    // 4 bytes packets
                    buff = new byte[2];
                    while (bytesToRead() < 2) ;
                    readBytes(buff, 2);
                    good_packet = ((mt ^ buff[0]) == buff[1]);
                    break;
                case (byte)'A':
                    // 4 bytes packets
                    buff = new byte[7];
                    while (bytesToRead() < 7) ;
                    readBytes(buff, 7);
                    cs = mt;
                    for (i = 0; i < 6; i++)
                        cs ^= buff[i];
                    good_packet = ((cs & 0x7f) == buff[6]);
                    break;
                case (byte)'I':
                    buff = new byte[3];
                    while (bytesToRead() < 3) ;
                    readBytes(buff, 3);
                    good_packet = (((mt ^ buff[0] ^ buff[1]) & 0x7f) == buff[2]); 
                    break;
                default:    // un-known error
                    return false;
            }

            return good_packet;
        }
    }
}
