using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace RobobuilderVC
{
    class binxfer
    {
        SerialPort sp1;
        const byte MAGIC_RESPONSE = 0xEA;
        const byte MAGIC_REQUEST = 0xCD;

        public byte[] buff;

        public binxfer(SerialPort s)
        {
            sp1 = s;
        }

        public void send_msg_basic(char mt)         // covers 'q', 'v', 'p'
        {
            byte[] buff = new byte[3];
            buff[0] = MAGIC_REQUEST;
            buff[1] = Convert.ToByte(mt);
            buff[2] = (byte)((buff[0] | buff[1]) & 0x7f);   // checksum
            sp1.Write(buff, 0, 3);
        }

        public void send_msg_move()
        {
        }

        public void send_msg_raw()
        {
        }


        public bool recv_packet()
        {
            int i;
            bool good_packet = false;

            while (sp1.ReadByte() != MAGIC_RESPONSE) ; // ignore till start of message

            byte mt = (byte)sp1.ReadByte();

            switch (mt)
            {
                case (byte)'q':
                    // 43 packets
                    buff = new byte[43];
                    sp1.Read(buff, 0, 43);
                    byte cs = (byte)'q';
                    for (i = 0; i < 42; i++) 
                           cs |= buff[i];
                    good_packet = ((cs &0x7f) == buff[43]);
                    break;
                case (byte)'x':
                case (byte)'X':
                    // 5 packets
                    buff = new byte[3];
                    sp1.Read(buff, 0, 3);
                    good_packet = ((mt | buff[0] | buff[1]) == buff[2]);
                    break;
                case (byte)'v':
                case (byte)'m':
                case (byte)'Z':   
                    // 4 bytes packets
                    buff = new byte[2];
                    sp1.Read(buff, 0, 2);
                    good_packet = ((mt | buff[0]) == buff[1]);
                    break;
                default:    // un-known error
                    return false;
            }
            return good_packet;
        }
    }
}
