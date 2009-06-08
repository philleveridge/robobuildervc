using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RobobuilderVC
{
    class Motion
    {
        public uint[] PGain;
        public uint[] DGain;
        public uint[] IGain;
        public uint[] Position;
        public uint[] extData;
        public string name;

        public Scene[] scenes;

        public int no_servos;
        public int no_scenes;

        public Motion() 
        {
            no_servos = 0;
            no_scenes = 0;
        }

        public bool LoadFile (string filename)
        {

            /*
             *  How about if we loaded (and wrote out) the .rbm files directly.  There 
                basically just ascii files - using ': ' separators here's one (The 
                simple wave - HunoDemo_Hi)
                Ive broken down (and added some comments as to what I think the format). 
                The actual file is one very long string (attached)

                1:13:0000000000000:XXXXXXXXXX:XXXXXXXXXXXXXXXXXXXX:11:  (* File header Version Info etc )
                
                [6] 
                HunoDemo_Hi:0:1:5:2:16:4:   (* header : Filename, Flag?, Flag?, No_scenes, ?, No Servos, ? *)
                [13]
                00:020:030:000:0:125:       (* Initial position for each servo = servo ID, Pgain, Rgain, Igain, ext, Posiiton)
                [19]
             *  01:020:030:000:0:201:
                02:020:030:000:0:163:
                03:020:030:000:0:067:
               ........
                [109]
                000:07:Scene_0:10:500:   (* Scene header:  ?, ?, name, 10 Frames, 500ms)
                [114]
             *  0:00:125:125:2:0:        (*, ?, Servo ID, Start Pos, end Pos, Torque, ext Port)
                0:01:179:179:2:0:
                0:02:199:199:2:0:        (* repeat for each servo *)

             * .... repeat for each scene ...........
             * 
             */


            try
            {
                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Trim();
                    //Console.WriteLine(line);
                    string[] a = line.Split(':');

                    Console.WriteLine("Motion file: " + a[6]);
                    name = a[6];
                    no_servos = Convert.ToInt16(a[11]);
                    Console.WriteLine("Servos: " + no_servos);
                    no_scenes = Convert.ToInt16(a[9]);
                    Console.WriteLine("Scenes: " + no_scenes);

                    IGain = new uint[no_servos];
                    DGain = new uint[no_servos];
                    PGain = new uint[no_servos];
                    Position = new uint[no_servos];
                    extData = new uint[no_servos];
                    scenes = new Scene[no_scenes];

                    int c = 13; // start of 00 postion

                    for (int i = 0; i < no_servos; i++)
                    {
                        PGain[i] = Convert.ToUInt16(a[c + 1 + i * 6]);
                        DGain[i] = Convert.ToUInt16(a[c + 2 + i * 6]);
                        IGain[i] = Convert.ToUInt16(a[c + 3 + i * 6]);
                        extData[i] = Convert.ToUInt16(a[c + 4 + i * 6]);
                        Position[i] = Convert.ToUInt16(a[c + 5 + i * 6]);
                    }

                    c = c + no_servos*6 + 2 ; // start of scene

                    int s = 0;

                    while (c + no_servos * 6 + 5 <= a.Length + 1)
                    {
                        if (s == no_scenes) break;
                        Console.WriteLine("Scene " + s + ": " + a[c]);
                        scenes[s] = new Scene();
                        scenes[s].name = a[c];
                        scenes[s].TransitionTime = Convert.ToUInt16(a[c + 2]);
                        scenes[s].Frames = Convert.ToUInt16(a[c + 1]);

                        scenes[s].mExternalData = new uint[no_servos];
                        scenes[s].mPositions = new uint[no_servos];  // scene end positions
                        scenes[s].mTorque = new uint[no_servos];

                        for (int i = 0; i < no_servos; i++)
                        {
                            scenes[s].mPositions[i] = Convert.ToUInt16(a[c + 6 + i * 6]);
                            scenes[s].mTorque[i] = Convert.ToUInt16(a[c + 7 + i * 6]);
                            scenes[s].mExternalData[i] = Convert.ToUInt16(a[c + 8 + i * 6]);
                        }
                        c += no_servos * 6 + 5;
                        s = s + 1;
                    }
                }
                tr.Close();
            }
            catch (Exception e1)
            {
                Console.WriteLine("RBM load Exception - " + e1.ToString());
            }
            return true;
        }
    }
}
