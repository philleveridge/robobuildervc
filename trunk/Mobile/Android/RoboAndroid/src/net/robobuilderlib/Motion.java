package net.robobuilderlib;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

class Motion
{
    public int[] PGain;
    public int[] DGain;
    public int[] IGain;
    public int[] Position;
    public int[] extData;
    public String name;

    public Scene[] scenes;

    public int no_servos;
    public int no_scenes;

    public Motion() 
    {
        no_servos = 0;
        no_scenes = 0;
    }
    
    public boolean Play(wckMotion w, int n)
    {
    	// simple play - each scene
    	// ignores gains and external at the moment
    	// TBC
    	if (no_servos==0 || no_scenes==0) return false;
    	
    	byte[] pos = new byte[no_servos];
    	
    	for (int i=0; i<no_scenes; i++)
    	{  	
    		for (int j=0; j<scenes[i].mPositions.length; j++)
    		{
    			if (j<no_servos)
    			{
    				pos[j]=(byte)scenes[i].mPositions[j];
    			}
    		}
    		
    		if (n> no_servos && no_servos==16)
    		{
    			pos[0] += 18; pos[5] -=18; // adjust for hip kit
    		}
    		
    		w.PlayPose(scenes[i].TransitionTime, scenes[i].Frames, 
    				pos, (i==0));
    	}  	
    	return true;
    }
    
    public boolean LoadFile (String filename)
    {
        try
        {   	
		    FileInputStream fstream = new FileInputStream(filename);
		    DataInputStream in = new DataInputStream(fstream);
	        
	        return LoadFile(in);
        }
        catch (IOException e)
        {
        	return false;
        }      
    }
    
    public boolean LoadFile (InputStream is)
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
            //TextReader tr = new StreamReader(filename);

	        BufferedReader tr = new BufferedReader(new InputStreamReader(is));	 
	        
            String line = "";

            while ((line = tr.readLine()) != null)
            {
                line = line.trim();
                //Console.WriteLine(line);
                String[] a = line.split(":");

                Debug.WriteLine("Motion file: " + a[6]);
                name = a[6];
                no_servos = Integer.parseInt(a[11]);
                Debug.WriteLine("Servos: " + no_servos);
                no_scenes = Integer.parseInt(a[9]);
                Debug.WriteLine("Scenes: " + no_scenes);

                IGain = new int[no_servos];
                DGain = new int[no_servos];
                PGain = new int[no_servos];
                Position = new int[no_servos];
                extData = new int[no_servos];
                scenes = new Scene[no_scenes];

                int c = 13; // start of 00 position

                for (int i = 0; i < no_servos; i++)
                {
                    PGain[i] = Integer.parseInt(a[c + 1 + i * 6]);
                    DGain[i] = Integer.parseInt(a[c + 2 + i * 6]);
                    IGain[i] = Integer.parseInt(a[c + 3 + i * 6]);
                    extData[i] = Integer.parseInt(a[c + 4 + i * 6]);
                    Position[i] = Integer.parseInt(a[c + 5 + i * 6]);
                }

                c = c + no_servos*6 + 2 ; // start of scene

                int s = 0;

                while (c + no_servos * 6 + 5 <= a.length + 1)
                {
                    if (s == no_scenes) break;
                    Debug.WriteLine("Scene " + s + ": " + a[c]);
                    scenes[s] = new Scene();
                    scenes[s].name = a[c];
                    scenes[s].TransitionTime = Integer.parseInt(a[c + 2]);
                    scenes[s].Frames = Integer.parseInt(a[c + 1]);

                    scenes[s].mExternalData = new int[no_servos];
                    scenes[s].mPositions = new int[no_servos];  // scene end positions
                    scenes[s].mTorque = new int[no_servos];

                    for (int i = 0; i < no_servos; i++)
                    {
                        scenes[s].mPositions[i] = Integer.parseInt(a[c + 6 + i * 6]);
                        scenes[s].mTorque[i] = Integer.parseInt(a[c + 7 + i * 6]);
                        scenes[s].mExternalData[i] = Integer.parseInt(a[c + 8 + i * 6]);
                    }
                    c += no_servos * 6 + 5;
                    s = s + 1;
                }
            }
            tr.close();
        }
        catch (IOException e1)
        {
            Debug.WriteLine("RBM load Exception - " + e1);
        }
        return true;
    }
}
