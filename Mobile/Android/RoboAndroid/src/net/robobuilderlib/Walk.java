﻿package net.robobuilderlib;

class Walk
{
    boolean first = false;
    wckMotion w;
    int n_of_s;
    
    Thread	wThread;  

    public Walk(wckMotion a)
    {
        w = a;
    }

    int[][] step = new int[][] {
        new int[] {123, 156, 212,  80, 108, 126, 73, 40, 150, 141,  68, 44, 40, 138, 208, 195},
        new int[] {130, 165, 201,  81, 115, 134, 81, 31, 147, 149,  72, 44, 40, 145, 209, 201},
        new int[] {132, 171, 197,  83, 117, 137, 86, 28, 148, 152,  78, 43, 41, 154, 209, 206},
        new int[] {132, 175, 195,  87, 117, 139, 91, 27, 152, 154,  87, 43, 43, 164, 209, 211},
        new int[] {132, 178, 197,  91, 117, 137, 95, 28, 157, 152,  97, 43, 48, 172, 209, 213},
        new int[] {130, 179, 201,  95, 115, 134, 96, 31, 161, 149, 105, 43, 53, 179, 210, 214},
        new int[] {127, 178, 206,  98, 112, 130, 95, 35, 166, 145, 111, 42, 57, 182, 210, 214},
        new int[] {124, 175, 212, 100, 109, 127, 92, 40, 170, 142, 113, 42, 59, 183, 210, 214},
        new int[] {120, 172, 217, 102, 105, 123, 88, 46, 170, 138, 111, 42, 57, 182, 210, 214},
        new int[] {116, 167, 221, 103, 101, 120, 83, 51, 169, 135, 106, 43, 53, 179, 210, 214},
        new int[] {113, 162, 224, 102,  98, 118, 77, 55, 167, 133, 97,  43, 48, 173, 209, 213},
        new int[] {111, 157, 225, 98,   96, 118, 73, 57, 163, 133, 87,  43, 43, 164, 209, 211},
        new int[] {113, 153, 224, 93,   98, 118, 70, 55, 159, 133, 79,  43, 41, 154, 209, 206},
        new int[] {116, 152, 221, 89,  101, 120, 69, 51, 155, 135, 72,  44, 40, 146, 209, 201},
        new int[] {120, 153, 217, 84,  105, 123, 70, 46, 152, 138, 69,  44, 40, 140, 208, 197},
        new int[] {123, 156, 212, 80,  108, 126, 73, 40, 150, 141, 68,  44, 40, 138, 208, 195}
        };

    int[][] reverse(int[][] z)
    {
        int[][] r = new int[z.length][];

        for (int i = 0; i < z.length; i++)
            r[i] = z[z.length-i-1];

        return r;
    }

    byte[] cv18(int[] a) // hip conversion
    {
        byte[] r = new byte[a.length];
        
        if (n_of_s > 16)
        {
            a[0] += 18;
            a[5] -= 20;
        }

        for (int i = 0; i < a.length; i++)
            r[i] = (byte)a[i];

        return r;
    }

    void setallLeds(int n, boolean a, boolean b)
    {
        for (int i = 0; i <= n; i++)
        {
            w.wckWriteIO(i, a, b);
        }
    }
    
    public void stop()
    {
        Debug.WriteLine("++ STOP");
        
    	if (wThread != null)
    	{
    		wThread.stop();
    		wThread = null;
    	}
    }

    public void forward (int p)
    {
        n_of_s = p;

        Debug.WriteLine("Continuous walk - reverses if PSD < 15");
        
        if (wThread!= null)
        	stop();
        
		wThread = new Thread() {
	        public void run() 
	        {   	            
	            int dely = 25;
	            
	            int[][] step_r = reverse(step);
	            
	            int count = 0;
	            
	            int[][] nxt = step;
	            
	            
		        while (true)
		        {
		            int d = readdistance();
		            Debug.WriteLine("PSD=" + d);
		            
		            if (d==0) break;

		            if (d > 15 && (count==0 || count==8))
		            {
		            	nxt = step;
		            }
		            else
		            {
		            	nxt = step_r;
		            }
	                w.PlayPose(dely, 1, cv18(nxt[count]), first);
	                count++;
	                if (count>15) count=0;
		        }
	        }};
	    wThread.start();
    }

	private int readdistance() {
        if (w.wckReadPos(30, 5))
        {
            return w.respnse[0];
        }
        return 0;
	}

}

