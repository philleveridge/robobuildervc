using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace RobobuilderVC
{
    class SerialSlave
    {
        const byte C = 16;
        const byte PGAIN = 20;
        const byte DGAIN = 30;
        const byte IGAIN = 0;
        const int  S     =(2 + 3 * C);

        bool load_flag;
        byte[] motionBuf = new byte[5000];
        byte[] scene = new byte[16];
        int bfsz;

        public SerialSlave()
        {
            load_flag = false;
            bfsz = 0;
        }

        public void Move(uint fm, uint tm, byte[] position)
        {
            int n;
			// with args (No Frames / Time in Ms) - use MotionBuffer

            motionBuf[0] = 1; //number of scenes
            motionBuf[1] = C; //number of servos

            for (n = 0; n < C; n++) { motionBuf[2 + n] = PGAIN; }        //PGAIN

            for (n = 0; n < C; n++) { motionBuf[2 + C + n] = DGAIN; }       //DGAIN

            for (n = 0; n < C; n++) { motionBuf[2 + 2 * C + n] = IGAIN; }     //IGAIN		

            motionBuf[S] = (byte)(tm & 255);
            motionBuf[S + 1] = (byte)(tm >> 8);
            motionBuf[S + 2] = (byte)(fm & 255);
            motionBuf[S + 3] = (byte)(fm >> 8);

            for (n = 0; n < C; n++) { motionBuf[S + 4 + n] = position[n]; }
            for (n = 0; n < C; n++) { motionBuf[S + 4 + n + C] = 3; }           //torque
            for (n = 0; n < C; n++) { motionBuf[S + 4 + n + C * 2] = 0; }       //ext data

            bfsz = S + 4 + n + C * 3;

            load_flag = true;
			
        }
        public void Play(SerialPort sp1) 
        { 
            //assuming in SSmode
            //send an 'm'
            //send bfsz (two bytes)
            //send motionBuf

            byte[] buff = new byte[3];

            buff[0] = Convert.ToByte('m');
            buff[1] = (byte)(bfsz&255);
            buff[2] = (byte)(bfsz>>8);


            if (load_flag && sp1.IsOpen)
            {
                sp1.WriteTimeout = 2000;
                sp1.ReadTimeout = 5000;
                sp1.Write(buff, 0, 3);

                Console.WriteLine(sp1.ReadTo("^"));

                sp1.Write(motionBuf, 0, bfsz);

                Console.WriteLine(sp1.ReadTo("bytes"));

                // its now loaded the motion (or timedout)

                //Console.WriteLine(sp1.ReadTo("Starting"));

                // its now playing the motion

                //Console.WriteLine(sp1.ReadTo("Done with"));

            }
            else
            {
                Console.WriteLine(buff);

                for (int i = 0; i < bfsz; i++)
                {
                    Console.WriteLine(Convert.ToInt16(motionBuf[i]).ToString("X2"));
                }
            }

        }

    }
}
