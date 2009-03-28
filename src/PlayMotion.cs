using System;
using System.Collections.Generic;
using System.Text;

namespace RobobuilderVC
{

    class PlayMotion
    {
        int NUM_OF_SCENES = 5;
        int NUM_OF_WCKS = 16;
        bool F_PLAYING;
        int FrameIdx;
        int NumOfFrame;
        int SIdx;
        float[] UnitD = new float[32];
        string outBuffer;

        //----------------------------------------------------------
        // Motion Name : HUNODEMO_HI
        //----------------------------------------------------------

        uint[] RuntimePGain ={
	    /* ID
	      0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	     20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20
        };

        uint[] RuntimeDGain ={
	    /* ID
	      0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	     30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30
        };
        uint[] RuntimeIGain ={
	    /* ID
	      0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	      0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        };
        uint[] Frames ={
	       10,   40,   10,   10,   40
        };
        uint[] TrTime ={
	      500, 1000,  500,  500, 1000
        };
        uint[,] Position ={
	    /* ID
	        0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 },
	    { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 },	// Idx:0 - Scene_0
	    { 125,179,199, 88,108,126, 72, 49,163,141,187, 58, 46,199,205,205 },	// Idx:1 - Scene_1
	    { 125,179,199, 88,108,126, 72, 49,163,141,186,103, 46,199,205,205 },	// Idx:2 - Scene_2
	    { 125,179,199, 88,108,126, 72, 49,163,141,187, 58, 46,199,205,205 },	// Idx:3 - Scene_1
	    { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 } 	// Idx:4 - Scene_4
        };
        uint[,] Torque ={
	    /* ID
	        0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:0 - Scene_0
	    {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:1 - Scene_1
	    {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:2 - Scene_2
	    {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:3 - Scene_1
	    {   4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4 } 	// Idx:4 - Scene_4
        };
        uint[,] Port ={
	    /* ID
	        0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:0 - Scene_0
	    {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:1 - Scene_1
	    {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:2 - Scene_2
	    {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:3 - Scene_1
	    {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } 	// Idx:4 - Scene_4
        };


        public PlayMotion()
        {
            F_PLAYING = false;
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
            for (i = 0; i < 16; i++)
            {
                t += SetCmd(i, 100, Port[SIdx, i], Port[SIdx, i]);
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
            for (i = 0; i < 16; i++)
            {
                t += SetCmd(i, 11, RuntimePGain[i], RuntimeDGain[i]);
            }
            // send that buffer
            //string t = "";
            for (i = 0; i < 16; i++)
            {
                t += SetCmd(i, 24, RuntimeIGain[i], RuntimeIGain[i]);
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
            for (int i = 0; i < NUM_OF_WCKS; i++)
            {
                if (Position[SIdx + 1, i] != Position[SIdx, i])
                {
                    float t = (int)Position[SIdx + 1, i] - (int)Position[SIdx, i];
                    t = t / Frames[SIdx];
                    if (t > 253) t = 254;
                    if (t < -253) t = -254;
                    UnitD[i] = t;
                }
                else
                    UnitD[i] = 0;

                //Console.WriteLine("DEBUG S=" + SIdx +"  ui " + i + "=" + UnitD[i]);
            }
        }

        //------------------------------------------------------------------------------
        // Set the Timer1 interrupt based on the number of frames and the run time of the scene
        // 		
        //------------------------------------------------------------------------------
        public int CalcFrameInterval()
        {
            Console.WriteLine("CI=" + TrTime[SIdx] / Frames[SIdx]);
            if (TrTime[SIdx] / Frames[SIdx] < 20)
                return 0;
            else
                return (int)(TrTime[SIdx] / Frames[SIdx]); // ms
        }

        //------------------------------------------------------------------------------
        // 
        //------------------------------------------------------------------------------
        public string Play()
        {
            string t = "";
            SIdx = 0;
	        t = SetGain();				    // set the runtime P,D and I from motion structure
	        //t += ExPortD();			        // Set external port data
	        CalcUnitMove();			    // Calculate the interpolation steps
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
            for (int i = 0; i < 16; i++)
            {
                int lt = (int)Position[SIdx,i] + (int)(UnitD[i]*(float)FrameIdx);
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
            Console.WriteLine("Debug: " + SIdx + "," + FrameIdx);
	        if( FrameIdx == Frames[SIdx] )
            {   // are we at the end of the scene ?
   	            FrameIdx = 0;

                SIdx++; //next scene

                if (SIdx == NUM_OF_SCENES)
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
