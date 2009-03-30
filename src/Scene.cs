using System;
using System.Text;
using System.IO.Ports;

namespace RobobuilderVC
{
    /*
     * I've implemented a fix for the problem I mentioned last night -- the 
    starting position of the first scene (when loaded from a RAM buffer as 
    opposed to from Flash) is now initialized to the current servo positions.

    This works a treat!  With the robot limp, you can position him however 
    you want -- say, one arm raised -- and then execute a 1-scene motion 
    with this scene:

       scene.TransitionTime= 1000
       scene.Frames =        50
       scene.mPositions =    Array(125,179,199,88,108,126,72,49,163,141,51,47,49,199,205,205)
       scene.mTorque =       Array(4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4)
       scene.mExternalData = Array(3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,  3, 3)

    ...and the robot will gently assume the given position (which is in fact 
    the basic pose position), lowering his arm and straightening his legs 
    and so on over the course of a second.

     */
    class Scene
    {
        public string name;
        public uint TransitionTime;
        public uint Frames;
        public uint[] mPositions;
        public uint[] mTorque;
        public uint[] mExternalData;

        public Scene()
        {
            Frames = 0;
            TransitionTime = 0;
        }

        public string toString()
        {
            return "X" + "00" ;
        }

        public bool play(SerialPort port)
        {
            if (port.IsOpen)
            {
            //check mode
            //send scene data
            }

            return false;
        }
    }
}
