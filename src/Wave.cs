using System;
using System.Collections.Generic;
using System.Text;

namespace RobobuilderVC
{
    class Wave : Motion
    {
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
        uint[] Frames ={ 10,   40,   10,   10,   40   };

        uint[] TrTime ={ 500, 1000,  500,  500, 1000  };

        uint[][] iPosition = new uint[][] {
	    /* ID
	                   0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 },
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 },	// Idx:0 - Scene_0
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141,187, 58, 46,199,205,205 },	// Idx:1 - Scene_1
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141,186,103, 46,199,205,205 },	// Idx:2 - Scene_2
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141,187, 58, 46,199,205,205 },	// Idx:3 - Scene_1
	    new uint[] { 125,179,199, 88,108,126, 72, 49,163,141, 51, 47, 49,199,205,205 } 	// Idx:4 - Scene_4
        };
        uint[][] Torque ={
	    /* ID
	                   0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    new uint[] {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:0 - Scene_0
	    new uint[] {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:1 - Scene_1
	    new uint[] {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:2 - Scene_2
	    new uint[] {   2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2 },	// Idx:3 - Scene_1
	    new uint[] {   4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4 } 	// Idx:4 - Scene_4
        };
        uint[][] Port ={
	    /* ID
	                   0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	    new uint[] {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:0 - Scene_0
	    new uint[] {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:1 - Scene_1
	    new uint[] {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:2 - Scene_2
	    new uint[] {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 },	// Idx:3 - Scene_1
	    new uint[] {   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 } 	// Idx:4 - Scene_4
        };
                
        public Wave()
        {
            name = "Built in Wave";
            no_scenes = 5;
            no_servos = 16;
            IGain = RuntimeIGain;
            PGain = RuntimePGain;
            DGain = RuntimeDGain;
            Position = iPosition[0];

            scenes = new Scene[no_scenes];
            for (int i = 0; i < no_scenes; i++)
            {
                scenes[i] = new Scene();
                scenes[i].name = "wave_" + i;
                scenes[i].Frames = Frames[i];
                scenes[i].TransitionTime = TrTime[i];
                scenes[i].mPositions = iPosition[i+1];
                scenes[i].mTorque = Torque[i];
                scenes[i].mExternalData = Port[i];
            }
            Console.WriteLine("Motion: Inbuilt, No Scenes=" + no_scenes);
        }
    }
}
