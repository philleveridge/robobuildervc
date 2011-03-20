package net.robobuilderlib;

public class pcRemote
{
    public Serial serialPort;
    byte[] header = new byte[] { (byte) 0xFF, (byte) 0xFF, (byte) 0xAA, 0x55, (byte) 0xAA, 0x55, 0x37, (byte) 0xBA };
    byte[] respnse = new byte[32];
    boolean DCmode;

    public boolean dbg  = false;


    int data;
    int nob = 0;
    int ml=0;

    // done

    public String message;


    public pcRemote(Serial s)
    {
        s.WriteTimeout = 500;
        s.ReadTimeout = 500;
        if (!s.IsOpen) s.Open();
        setup(s);
    }

    public void setdbg(boolean x)
    {
        dbg = x;
    }

    public void Close()
    {
        setDCmode(false);
        if (serialPort.IsOpen) serialPort.Close();
    }

    void setup(Serial s)
    {
        serialPort = s;
        message = "";
        DCmode = false;
        //send a forced exit DC mode just in case robot was left in that state
        if (serialPort.IsOpen) 
            serialPort.Write(new byte[] { (byte) 0xFF, (byte) 0xE0, (byte) 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
    }

    /**********************************************
      * 
      * send request/ read response 
      * serial protocol
      * 
      ********************************************/

    boolean command_1B(byte type, byte cmd)
    {
        serialPort.Write(header, 0, 8);
        serialPort.Write(new byte[] { type,           //type (1)
                            0x00,                      //platform (1)
                            0x00, 0x00, 0x00, 0x01,    //command size (4)
                            cmd,                       //command contents (1)
                            cmd                         //checksum
                        }, 0, 8);
        return true;
    }

    boolean command_nB(byte platform, byte type, byte[] buffer)
    {
        serialPort.Write(header, 0, 8);
        serialPort.Write(new byte[] { 
                            type,                                   //type (1)
                            platform,                               //platform (1)
                            0x00, 0x00, 0x00, (byte)buffer.length,  //command size (4)
                        }, 0, 6);

        serialPort.Write(buffer, 0, buffer.length);

        byte[] cs = new byte[1];

        for (int i = 0; i < buffer.length; i++)
        {
            cs[0] ^= buffer[i];
        }
        serialPort.Write(cs, 0, 1);
        return true;
    }

    boolean displayResponse(boolean flag)
    {
        try
        {
            int b = 0;
            int l = 1;

            while (b < 32 && b < (15 + l))
            {
                respnse[b] = (byte)serialPort.ReadByte();

                if (b < header.length && respnse[b] != header[b])
                {
                    Debug.WriteLine("skip [" + b + "]=" + respnse[b]);
                    b = 0;
                    continue;
                }

                if (b == 13)
                {
                    l = (respnse[b - 3] << 24) + (respnse[b - 2] << 16) + (respnse[b - 1] << 8) + respnse[b];
                    //Console.WriteLine("L=" + l);
                }
                b++;
            }

            if (flag)
            {
                message = "Response:\n";
                for (int i = 0; i < 7 + l; i++)
                {
                    //message += respnse[8 + i].ToString("X2") + " ";
                    message += respnse[8 + i] + " ";
                }
                message += "\r\n";
            }
            return true;
        }
        catch (Exception e1)
        {
            message = "Timed Out\r\n";
            return false;
        }
    }

    public String readVer()
    {
        //read firmware version number
        String r = "0";

        if (serialPort.IsOpen)
        {
            command_1B((byte)0x12, (byte)0x01);
            if (displayResponse(false))
                r = respnse[14] + "." + respnse[15];
        }
        return r;
    }

    public String readSN()
    {
        // read serial number
        String r = "";

        if (serialPort.IsOpen)
        {
            command_1B((byte)0x0C, (byte)0x01);
            if (displayResponse(false))
            {
                for (int n0 = 0; n0 < 13; n0++)
                    r += (char)respnse[14 + n0];
            }
        }
        return r;
    }

    public int readPSD()
    {
        // read distance
        int r = 0;

        if (serialPort.IsOpen)
        {
            command_1B((byte)0x16, (byte)0x01);
            if (displayResponse(true))
                r = respnse[14] + (respnse[15] << 8);
        }
        return r;
    }


    public int[] readXYZ()
    {
        int[] r = new int[3];
        int x = 0, y = 0, z = 0;

        if (serialPort.IsOpen)
        {
            command_1B((byte)0x1A, (byte)1); // reset motion memory
            if (displayResponse(false))
            {
                x = (int)(((respnse[15] << 8) + (respnse[14])));
                y = (int)(((respnse[17] << 8) + (respnse[16])));
                z = (int)(((respnse[19] << 8) + (respnse[18])));
            }
        }
        r[0]=x; r[1]=y; r[2]=z;
        return r;
    }

    public String availMem()
    {
        // avail mem
        String r = "";
        if (serialPort.IsOpen)
        {
            command_1B((byte)0x0F, (byte)0x01);
            if (displayResponse(false))
                r += "Avail mem=" + ((respnse[14] << 24) + (respnse[15] << 16)
                    + (respnse[16] << 8) + respnse[17])
                    + " Bytes\r\n";
        }
        return r;
    }

    public String resetMem()
    {
        String r = "";
        //reset memory
        if (serialPort.IsOpen)
        {
            command_1B((byte)0x1F, (byte)0x01); // reset motion memory
            displayResponse(true);

            command_1B((byte)0x1F, (byte)0x02); // reset action memory
            displayResponse(true);
        }
        return r;
    }

    public String readZeros()
    {
        //read zeros
        String r = "";
        if (serialPort.IsOpen)
        {
            command_1B((byte)0x0B, (byte)0x01);
            displayResponse(true);
        }
        return r;
    }

    public String a()        { return runMotion(1); }
    public String b()        { return runMotion(2); }
    public String basic()    { return runMotion(7); }
    public String run(int m) { return runMotion(m); }

    public String runMotion(int m)
    {
        //read zeros
        if (m < 1 || m > 42)
        {
            return "Invalid Motion";
        }

        String r = "";
        if (serialPort.IsOpen)
        {
            command_1B((byte)20, (byte)m);
            displayResponse(true);
        }
        return r;
    }

    public String runSound(int m)
    {
        //play a specific sound
        String r = "";

        if (m < 1 || m > 25)
        {
            return "Invalid Sound";
        }

        if (serialPort.IsOpen)
        {
            command_1B((byte)21, (byte)m);
            displayResponse(true);
        }
        return r;
    }

    public String executionStatus(int m)
    {
        //get execution status for specific motion
        String r = "";
        if (serialPort.IsOpen)
        {
            command_1B((byte)30, (byte)m);
            displayResponse(true);
        }
        return r;
    }

    public String zeroHuno()
    {
        String r = "";
        //set zeros to Standard Huno
        byte[] MotionZeroPos = new byte[] {
             /* ID
             0 ,1  ,2  ,3 ,4  ,5  ,6 ,7 ,8  ,9  ,10,11,12 ,13 ,14, 15, 16,17,18*/
        	125,(byte) 202,(byte) 162,66,108,124,48,88,(byte) 184,(byte) 142,90,40,125,(byte) 161,(byte) 210,127,4, 0, 0};

        if (serialPort.IsOpen)
        {
            command_nB((byte)0, (byte)0x0E, MotionZeroPos);
            displayResponse(true);
        }
        return r;
    }

    public void setDCmode(boolean f)
    {
        if (DCmode == f) return; //only output changes
        DCmode = f;
        if (f)
        {
            // DC mode
            if (serialPort.IsOpen)
            {
                command_1B((byte)0x10, (byte)0x01);
                displayResponse(false);
            }
        }
        else
        {
            // end DC mode
            if (serialPort.IsOpen) serialPort.Write(new byte[] { (byte) 0xFF, (byte) 0xE0, (byte) 0xFB, 0x1, 0x00, 0x1A }, 0, 6);
        }
    }

    // for compatibility with Homebrew OS

    public boolean download_basic(String s)
    {
        return false;
    }
}

