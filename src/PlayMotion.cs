using System;
using System.Collections.Generic;
using System.Text;

namespace RobobuilderVC
{

    class PlayMotion
    {
        int NUM_OF_WCKS = 16;
        bool F_PLAYING;
        int FrameIdx;
        int SIdx;
        float[] UnitD = new float[32];
        string outBuffer;
        Motion m;

        public PlayMotion(Motion x)
        {
            F_PLAYING = false;
            m = x;
            Console.WriteLine("Motion: " + m.name + ", No Scenes=" + m.no_scenes);
            NUM_OF_WCKS = m.no_servos;
        }

        //------------------------------------------------------------------------------
        // Send a set command to wCK
        // Input	: ID, Data1, Data2, Data3
        // Output	: None
        //------------------------------------------------------------------------------
        string SetCmd(uint ID, uint Data1, uint Data2, uint Data3)
        {
            string t;
            ID=(7<<5)|ID;
            uint CheckSum = (ID ^ Data1 ^ Data2 ^ Data3) & 0x7f;
            t = "FF" 
                + ID.ToString("X2") 
                + Data1.ToString("X2") 
                + Data2.ToString("X2") 
                + Data3.ToString("X2") 
                + CheckSum.ToString("X2");
            t += ":";
            return t;
        }


        //------------------------------------------------------------------------------
        // Send external data for a scene
        // 		
        //------------------------------------------------------------------------------
        string ExPortD()
        {
            uint i;
            string t = "";
            for (i = 0; i < NUM_OF_WCKS; i++)
            {
                t += SetCmd(i, 100, m.scenes[SIdx].mExternalData[i], m.scenes[SIdx].mExternalData[i]);
            }
            return t;
        }

        //------------------------------------------------------------------------------
        // Runtime P,D,I setting
        // 	
        //------------------------------------------------------------------------------
        string SetGain()
        {
	        uint i;
            string t="";
            for (i = 0; i < NUM_OF_WCKS; i++)
            {
                t += SetCmd(i, 11, m.PGain[i], m.DGain[i]);
            }
            for (i = 0; i < NUM_OF_WCKS; i++)
            {
                t += SetCmd(i, 24, m.IGain[i], m.IGain[i]);
            }
            return t;
        }

        //------------------------------------------------------------------------------
        // Build a frame to send
        //------------------------------------------------------------------------------
        void MakeFrame()
        {
            FrameIdx++;			// next frame
            SyncPosSend();		// build new frame
            return;
        }

        //------------------------------------------------------------------------------
        // Calculate the interpolation steps
        // UnitD[] is in range -254 to +254
        //------------------------------------------------------------------------------
        void CalcUnitMove()
        {
            for (int i= 0; i < NUM_OF_WCKS; i++)
            {
                uint spos;
                if (SIdx == 0)
                    spos = m.Position[i];
                else
                    spos = m.scenes[SIdx - 1].mPositions[i];

                uint epos = m.scenes[SIdx].mPositions[i];

                if ( epos != spos)
                {
                    float t = (int)epos - (int)spos;
                    t = t / m.scenes[SIdx].Frames;
                    if (t > 253) t = 254;
                    if (t < -253) t = -254;
                    UnitD[i] = t;
                }
                else
                    UnitD[i] = 0;

            }
        }

        //------------------------------------------------------------------------------
        // Set the Timer1 interrupt based on the number of frames and the run time of the scene
        // 		
        //------------------------------------------------------------------------------
        public int CalcFrameInterval()
        {
            Console.WriteLine("CI=" + m.scenes[SIdx].TransitionTime / m.scenes[SIdx].Frames);
            if (m.scenes[SIdx].TransitionTime / m.scenes[SIdx].Frames < 20)
                return 0;
            else
                return (int)(m.scenes[SIdx].TransitionTime / m.scenes[SIdx].Frames); // ms
        }

        //------------------------------------------------------------------------------
        // 
        //------------------------------------------------------------------------------
        public string Play()
        {
            string t = "";
            SIdx = 0;
	        t = SetGain();				    // set the runtime P,D and I from motion structure
	        //t += ExPortD();			    // Set external port data
	        CalcUnitMove();			        // Calculate the interpolation steps
            return t;
        }


        //------------------------------------------------------------------------------
        // Send a Synchronized Position Send Command
        //------------------------------------------------------------------------------
        void SyncPosSend() 
        {
            byte t = (2 << 5) | 31;
            int c = 0;
            outBuffer = "FF";
            outBuffer += t.ToString("X2");
            t = 16;
            outBuffer += t.ToString("X2");
            t = 0;
            for (int i = 0; i < NUM_OF_WCKS; i++)
            {
                int lt = (int)m.scenes[SIdx].mPositions[i] + (int)(UnitD[i] * (float)FrameIdx);
                if (lt > 254) lt = 254;
                if (lt < 1) lt = 1;
                outBuffer += lt.ToString("X2");
                t = (byte)((uint)t ^ lt);
            }
            t &= 0x7f;
            outBuffer += t.ToString("X2") + ":";
        }

        //------------------------------------------------------------------------------
        // Timer Send Frame to wCK interrupt routine
        //------------------------------------------------------------------------------
        public string Timer() 
        {
            Console.WriteLine("Debug: " + m.scenes[SIdx].name + "," + FrameIdx);
	        if( FrameIdx == m.scenes[SIdx].Frames)
            {   // are we at the end of the scene ?
   	            FrameIdx = 0;

                SIdx++; //next scene

                if (SIdx == m.scenes.Length)
                {
                    SIdx = 0;
                    F_PLAYING = false;					// clear F_PLAYING state
                    return "";
                }

                CalcUnitMove();
	        }
	        MakeFrame();					            // build the wCK frame
            return outBuffer;
        }
    }
}
