﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace RobobuilderVC
{
    /*
     * serialSlave mode Binary protocol
     * 
     * BE AD mtype ....
     * mtype:
     *   ?
     *   q
     *   g AA BB CC
     *   p
     *   .
     *   x
     *   m
     *   esc
     *   
     * response
     * BE AD k Mt
     * BE AD E strng 00
     * 
     */



    class SerialSlave
    {
        const byte C = 16;
        const byte PGAIN = 20;
        const byte DGAIN = 30;
        const byte IGAIN = 0;
        const int  S     =(2 + 3 * C);

        public bool load_flag;
        public byte[] motionBuf = new byte[5000];
        public int bfsz;
             
        byte[] scene = new byte[16];
        SerialPort sp1;

        public SerialSlave(SerialPort s)
        {
            load_flag = false;
            bfsz = 0;
            sp1 = s;
        }

        public void Move(uint fm, uint tm, byte[] position)
        {
            int n;
			// with args (No Frames / Time in Ms) - use MotionBuffer

            motionBuf[0] = 1; //number of scenes
            motionBuf[1] = C; //number of servos

            for (n = 0; n < C; n++) { motionBuf[2 + n] = PGAIN; }        //PGAIN

            for (n = 0; n < C; n++) { motionBuf[2 + C + n] = DGAIN; }    //DGAIN

            for (n = 0; n < C; n++) { motionBuf[2 + 2 * C + n] = IGAIN; }//IGAIN		

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

        public void Load(Motion m, int scene_no)
        {
            int n;
            Console.WriteLine("Loading - " + m.scenes[scene_no].name);
            uint tm = m.scenes[scene_no].TransitionTime;
            uint fm = m.scenes[scene_no].Frames;

            motionBuf[0] = 1;           //number of scenes
            motionBuf[1] = (byte)m.no_servos; //number of servos

            for (n = 0; n < C; n++) { motionBuf[2 + n] = (byte)m.PGain[n];         }
            for (n = 0; n < C; n++) { motionBuf[2 + C + n] = (byte)m.DGain[n];     } 
            for (n = 0; n < C; n++) { motionBuf[2 + 2 * C + n] = (byte)m.IGain[n]; }

            motionBuf[S] = (byte)(tm & 255);
            motionBuf[S + 1] = (byte)(tm >> 8);
            motionBuf[S + 2] = (byte)(fm & 255);
            motionBuf[S + 3] = (byte)(fm >> 8);

            for (n = 0; n < C; n++) { motionBuf[S + 4 + n] = (byte)m.scenes[scene_no].mPositions[n]; }
            for (n = 0; n < C; n++) { motionBuf[S + 4 + n + C] = (byte)m.scenes[scene_no].mTorque[n]; }           //torque
            for (n = 0; n < C; n++) { motionBuf[S + 4 + n + C * 2] = (byte)m.scenes[scene_no].mExternalData[n];}  //ext data

            bfsz = S + 4 + n + C * 3;

            load_flag = true;			
        }


        public void sendMessage(byte Mt)
        {
            byte[] buff = new byte[5];
            buff[0] = 0xBE;
            buff[1] = 0xAD;
            buff[2] = Mt;
            sp1.Write(buff, 0, 3);
        }

        public int readResponse()
        {
            int ch = 0;
            int t = 0;

            while (true)
            {
                if (ch != 0xBE) ch = sp1.ReadByte();
                if (ch != 0xBE) continue;
                ch = sp1.ReadByte();
                if (ch == 0xAD) break;
            }
            // read in correct header BE AD
            ch = sp1.ReadByte(); // read type
            t = sp1.ReadByte(); 

            switch (ch) 
            {
                case 'k':
                    // ack for ?
                    switch (t)
                    {
                        case '?':
                        case 'p':
                        case 27:
                            return 0;
                    }
                    break;
                case 'E':
                    // read error string
                    string err = "";
                    while ((ch = sp1.ReadByte()) != 0)
                    {
                        err += Convert.ToString(ch);
                    }
                    Console.WriteLine("Err=" + err);
                    return -1;
            }
            return 0;
        }

        public void SetMode()
        {
            sendMessage((byte)'?');

            if (readResponse() < 0)
                return;
        }

        public void ExitMode()
        {
            sendMessage(27);

            if (readResponse() < 0)
                return;
        }

        public void SendPose()
        {
            sendMessage((byte)'p');

            if (readResponse() < 0)
                return;
        }

        public void Play() 
        { 
            //assuming in SSmode
            //send an 'm'
            //send bfsz (two bytes)
            //send motionBuf

            byte[] buff = new byte[5];

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
