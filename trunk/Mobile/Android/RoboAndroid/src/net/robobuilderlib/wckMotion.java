package net.robobuilderlib;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;


public class wckMotion
{
	public  enum MoveTypes { AccelDecel, Accel, Decel, Linear };
	
	public final int MAX_SERVOS = 21;
	//trigger trig;

	static int[] pair_bonds = { 10, 13, 11, 14, 12, 15, 0, 5, 1, 6, 2, 7, 3, 8, 4, 9, 16, 17 };

	static public int[] ub_Huno = new int[] {
	/* ID
	  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	174,228,254,130,185,254,180,126,208,208,254,224,198,254,228,254};

	static public int[] lb_Huno = new int[] {
	/* ID
	  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15 */
	  1, 70,124, 40, 41, 73, 22,  1,120, 57,  1, 23,  1,  1, 25, 40};

	static public byte[] basicdh = new byte[] {
		(byte)143, (byte)179, (byte)198, 83, 105, 106, 68, 46, (byte)167, (byte)140, 77, 70, (byte)152, (byte)165, (byte)181, 98, 120, 124, 99};
	
	static public byte[] basic18 = new byte[] { 
		(byte)143, (byte)179, (byte)198, 83, 106, 106, 69, 48, (byte)167, (byte)141, 47, 47, 49, (byte)199, (byte)192, (byte)204, 122, 125, (byte)255};

	static public byte[] basic16 = new byte[] { 
		125, (byte)179, (byte)199, 88, 108, 126, 72, 49, (byte)163, (byte)141, 51, 47, 49, (byte)199, (byte)205, (byte)205 };

	static public byte[] basic_pos = basic18;

	//public bool DCMP { get; set; }  // this must be true for DCMP high speed mode (custom firmware)

	public MoveTypes cmt ;

	/**********************************************
	 * 
	 * direct Command mode  - wcK protocol
	 * 
	 * ********************************************/

	int[] sids = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

	private Serial serialPort;
	
	public byte[] respnse = new byte[32];
	public String Message;
	public byte[] pos;

	public double kfactor = 1.0f;
	int tcnt;

	public wckMotion(Serial s)
	{
		serialPort =s;
		cmt = MoveTypes.Linear; //default 
	}
/*
	~wckMotion()
	{
		close();
	}
*/
	
	public int bond(int n)
	{
		for (int i = 0; i < pair_bonds.length; i += 2)
		{
			if (n == pair_bonds[i])   return pair_bonds[i + 1];
			if (n == pair_bonds[i+1]) return pair_bonds[i];
		}
		return -1;
	}
	
    public int countServos()
    {
        for (int i = 0; i < MAX_SERVOS; i++)
        {
            if (!wckReadPos(i))
            {
                return i;
            }
        }
        return MAX_SERVOS;
    } 

	public void set_kfactor(double k)
	{
		kfactor =k;
	}


	public void reset_timer()
	{
		tcnt=0;
	}

	public void servoStatus(int id, boolean f)
	{
		sids[id] = (f) ? id : -1;
	}

	public void close()
	{
	   //pcR.setDCmode(false);
		
	}

	public boolean wckPassive(int id)
	{
		byte[] buff = new byte[4];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)(6 << 5 | (id % 31));
		buff[2] = 0x10; // Mode=1, arbitary
		buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 4);
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			Message = "Passive " + id + " = " + respnse[0] + ":" + respnse[1];
			//System.Diagnostics.Debug.WriteLine(Message); // debug
			return true;
		}
		catch (Exception e1)
		{
			Message = "Failed" + e1.getMessage();
			return false;
		}
	}

	public boolean wckReadAll()
	{
		// requires DCMP >229
		byte[] buff = new byte[4];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)(5 << 5 | (30 % 31));
		buff[2] = (byte)0x0f;      // 
		buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 4);
			respnse[0] = (byte)serialPort.ReadByte(); // x
			respnse[1] = (byte)serialPort.ReadByte(); // y
			respnse[2] = (byte)serialPort.ReadByte(); // z
			respnse[3] = (byte)serialPort.ReadByte(); // PSD
			respnse[4] = (byte)serialPort.ReadByte(); // ir
			respnse[5] = (byte)serialPort.ReadByte(); // buttons
			respnse[6] = (byte)serialPort.ReadByte(); // snd

			Message = "ReadAll = " + respnse[0] + ":" + respnse[1] + ":" + respnse[2] + ":" + respnse[3] + ":" + respnse[4] + ":" + respnse[5] + ":" + respnse[6];
			//System.Diagnostics.Debug.WriteLine(Message); // debug
			return true;
		}
		catch (Exception e1)
		{
			Message = "Failed" + e1.getMessage();
			return false;
		}
	}

	public boolean wckReadPos(int id)
	{
		return wckReadPos(id, 0);
	}

	public boolean wckReadPos(int id, int d1)
	{
		byte[] buff = new byte[4];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)(5 << 5 | (id % 31));
		buff[2] = (byte) d1; // arbitary
		buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 4);
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			Message = "ReadPos " + id + " = " + respnse[0] + ":" + respnse[1];
			//System.Diagnostics.Debug.WriteLine(Message); // debug
			return true;
		}
		catch (Exception e1)
		{
			Message = "Failed" + e1.getMessage();
			Debug.WriteLine(Message);
			return false;
		}
	}

	public boolean wckMovePos(int id, int pos, int torq)
	{
		byte[] buff = new byte[4];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)(((torq % 5) << 5) | (id % 31));
		buff[2] = (byte)(pos % 254); // arbitary
		buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 4);
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			Message = "MovePos " + id + " = " + respnse[0] + ":" + respnse[1];

			return true;
		}
		catch (Exception e1)
		{
			Message = "Failed" + e1.getMessage() ;
			return false;
		}
	}


	public void SyncPosSend(int LastID, int SpeedLevel, byte[] TargetArray, int Index)
	{
		int i;
		byte CheckSum;
		byte[] buff = new byte[5 + LastID];

		i = 0;
		CheckSum = 0;
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)((SpeedLevel << 5) | 0x1f);
		buff[2] = (byte)(LastID + 1);

		while (true)
		{
			if (i > LastID) break;
			buff[3 + i] = TargetArray[Index * (LastID + 1) + i];
			CheckSum ^= (byte)(TargetArray[Index * (LastID + 1) + i]);
			i++;
		}
		CheckSum = (byte)(CheckSum & 0x7f);
		buff[3 + i] = CheckSum;

		//now output buff[]
		//Debug info :: for (i = 0; i < buff.Length - 1; i++) Console.Write(buff[i] + ","); Console.WriteLine(buff[i]);

		try
		{
			serialPort.Write(buff, 0, buff.length);
			Message = "MoveSyncPos";

			return;
		}
		catch (Exception e1)
		{
			Message = "Failed" + e1.getMessage();
			return;
		}
	}

	public boolean wckBreak()
	{
		byte[] buff = new byte[4];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte)((6 << 5) | 31);
		buff[2] = (byte)0x20;
		buff[3] = (byte)((buff[1] ^ buff[2]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 4);
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			Message = "Break = " + respnse[0] + ":" + respnse[1];
			return true;
		}
		catch (Exception e1)
		{
			Message = "Break Failed" + e1.getMessage();
			return false;
		}
	}

	/* 
	 * wck set operation(s)
	 * 
	 */ 
	public boolean wckSetOper(byte d1,byte d2, byte d3, byte d4)
	{
		byte[] buff = new byte[6];
		buff[0] = (byte) 0xFF;
		buff[1] = d1;
		buff[2] = d2;
		buff[3] = d3;
		buff[4] = d4;
		buff[5] = (byte)((buff[1] ^ buff[2] ^ buff[3] ^ buff[4]) & 0x7f);

		try
		{
			serialPort.Write(buff, 0, 6);
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			Message = "Set Oper = " + respnse[0] + ":" + respnse[1];
			return true;
		}
		catch (Exception e1)
		{
			Message = "Set Op Failed" + e1.getMessage();
			return false;
		}
	}

	public boolean wckSetBaudRate(int baudrate, int id)
	{
		byte d1, d3;
		d1=(byte)((7<<5) | (id %31));
		//0(921600bps), 1(460800bps), 3(230400bps), 7(115200bps),
		//15(57600bps), 23(38400bps), 95(9600bps), 191(4800bps),
		switch (baudrate)
		{
			case 115200:
				d3 = 7;
				break;
			case 57600:
				d3 = 15;
				break;
			case 9600:
				d3 = 95;
				break;
			case 4800:
				d3 = (byte) 191;
				break;
			default:
				return false;
		}
		return wckSetOper(d1, (byte) 0x08, d3, d3);
	}

	public boolean wckSetSpeed(int id, int speed, int acceleration)
	{
		byte d1, d3, d4;
		if (speed < 0 || speed > 30) 
			return false;
		if (acceleration < 20 || acceleration > 100) 
			return false;
		d1 = (byte)((7 << 5) | (id % 31));
		d3 = (byte)speed;
		d4 = (byte)acceleration;
		return wckSetOper(d1, (byte) 0x0D, d3, d4);
	}

	public boolean wckSetPDgain(int id, int pGain, int dGain)
	{
		return false; // not implemented
	}

	public boolean wckSetID(int id, int new_id)
	{
		return false; // not implemented
	}

	public boolean wckSetIgain(int id, int iGain)
	{
		return false; // not implemented
	}

	public boolean wckSetPDgainRT(int id, int pGain, int dGain)
	{
		return false; // not implemented
	}

	public boolean wckSetIgainRT(int id, int iGain)
	{
		return false; // not implemented
	}

	public boolean wckSetSpeedRT(int id, int speed, int acceleration)
	{
		return false; // not implemented
	}

	public boolean wckSetOverload(int id, int overT)
	{
		/*
		1 33 400
		2 44 500
		3 56 600
		4 68 700
		5 80 800
		6 92 900
		7 104 1000
		8 116 1100
		9 128 1200
		10 199 1800
		 */
		byte d1, d3=33;
		d1 = (byte)((7 << 5) | (id % 31));
		switch (overT)
		{
			case 400:
				d3 = 33;
				break;
			case 500:
				d3 = 44;
				break;
			case 600:
				d3 = 56;
				break;
			case 700:
				d3 = 68;
				break;
			case 800:
				d3 = 80;
				break;                 
		}
		return wckSetOper(d1, (byte) 0x0F, d3, d3);
	}

	public boolean wckSetBoundary(int id, int UBound, int LBound)
	{
		byte d1, d3, d4;
		d1 = (byte)((7 << 5) | (id % 31));
		d3 = (byte)LBound;
		d4 = (byte)UBound;
		return wckSetOper(d1, (byte) 0x11, d3, d4);
	}

	public boolean wckWriteIO(int id, boolean ch0, boolean ch1 )
	{
		byte d1, d3;
		d1 = (byte)((7 << 5) | (id % 31));
		d3 = (byte)((byte)((ch0) ? 1 : 0) | (byte)((ch1) ? 2 : 0));
		return wckSetOper(d1, (byte) 0x64, d3, d3);
	}

	/* 
	 * wck - Read operation(s)
	 */ 

	public boolean wckReadPDgain(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x0A, (byte)0x00, (byte)0x00);
	}

	public boolean wckReadIgain(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x16, (byte)0x00, (byte)0x00);
	}

	public boolean wckReadSpeed(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x0E, (byte)0x00, (byte)0x00);
	}

	public boolean wckReadOverload(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x10,(byte)0x00, (byte)0x00);
	}

	public boolean wckReadBoundary(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x12, (byte)0x00, (byte)0x00);
	}

	public boolean wckReadIO(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x65, (byte)0x00, (byte)0x00);
	}

	public boolean wckReadMotionData(int id)
	{
		return wckSetOper((byte)((7 << 5) | (id % 31)), (byte)0x97, (byte)0x00, (byte)0x00);
	}

	public boolean wckPosRead10Bit(int id)
	{
		return wckSetOper((byte)(7 << 5), (byte)0x09, (byte)id, (byte)id);
	}

	/* 
	 * special extended / 10 bit commands
	 */ 

	public boolean wckWriteMotionData(int id, int pos, int torq)
	{
		return false; // not implemented
	}
	
	public boolean wckPosMove10Bit(int id, int pos, int torq)
	{
		return false; // not implemented
	}


	/*********************************************************************************************
	 * 
	 * I2C hardware functions (require DCMP)
	 * 
	 *********************************************************************************************/


	/*public int cbyte(byte b) // C# function only
	{
		int i;
		if (b > 127)
		{
			i = (int)b - 256;
		}
		else
		{
			i = (int)b;
		}
		return i;
	}*/
	
	public int cbyte(byte b) {return (int)b;} // in java bytes are signed

	public boolean I2C_write(int addr, byte[] outbuff)
	{
		int n = 0;
		if (outbuff != null) n = outbuff.length;

		byte[] buff = new byte[n+6];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte) 0xBE;                     // servo address 30
		buff[2] = 0x0E;                     // cmd = 0x0E (IC2_out)
		buff[3] = (byte)(addr%256);         // IC2 slave address
		buff[4] = (byte)(outbuff.length);   // no of bytes tos end
		int cs = 0;

		for (int i = 0; i < n; i++)
		{
			buff[5 + i] = outbuff[i];
			cs ^= outbuff[i];
		}
		buff[5 + n] = (byte)(cs & 0x7f);

		try
		{
			//serialPort.Write(buff, 0, buff.Length);
			for (int cb = 0; cb < buff.length; cb++)
			{
				serialPort.Write(buff, cb, 1); // send each byte 
			}
			respnse[0] = (byte)serialPort.ReadByte();
			respnse[1] = (byte)serialPort.ReadByte();
			return true;
		}
		catch (Exception e1)
		{
			Message = "Special Op Failed" + e1.getMessage();
			return false;
		}
	}

	public byte[] I2C_read(int addr, byte[] outbuff, int cnt)
	{
		byte[] inbuff = new byte[cnt];

		byte[] buff = new byte[outbuff.length + 7];
		buff[0] = (byte) 0xFF;
		buff[1] = (byte) 0xBE;                         // servo address 30
		buff[2] = 0x0D;                         // cmd = 0xD (IC2_in)
		buff[3] = (byte)(addr % 256);           // IC2 slave address
		buff[4] = (byte)(outbuff.length + 1);   // no of bytes tos end 
		buff[5] = (byte)(cnt);                  // input bytes required added as first byte

		int cs = cnt;

		for (int i = 0; i < outbuff.length; i++)
		{
			buff[6 + i] = outbuff[i];
			cs ^= outbuff[i];
		}

		buff[6 + outbuff.length] = (byte)(cs & 0x7f);

		try
		{
			serialPort.Write(buff, 0, buff.length);

			for (int j = 0; j < cnt; j++)
			{
				respnse[j] = (byte)serialPort.ReadByte();
				inbuff[j] = respnse[j];
			}
			return inbuff;
		}
		catch (Exception e1)
		{
			Message = "Special Op Failed" + e1.getMessage();
			return null;
		}
	}

	/*********************************************************************************************
	 * 
	 * higher level functions
	 * 
	 *********************************************************************************************/

	boolean initpos = false;

	public void servoID_readservo(int num)
	{
		if (num == 0) num = sids.length;
		
		pos = new byte[num];

		for (int id = 0; id < num; id++)
		{
			int n = sids[id];

			if (n>=0 && wckReadPos(n))                 //readPOS (servoID)
			{
				if (respnse[1] < 255)
				{
					pos[id] = respnse[1];
				}
				else
				{
					pos[id] = 0;
					//System.Diagnostics.Debug.WriteLine(String.Format("Id {0} = {1}", id, respnse[1]));
				}
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Id " + id + "not connected" );
				//sids[id] = -1; // not connected
			}
		}
		initpos = true;
	}

	public void delay_ms(int t1)
	{
		//if (pcR.dbg) Console.WriteLine("dly=" + t1);
		//System.Threading.Thread.Sleep(t1);
		android.os.SystemClock.sleep((long)t1);
	}

	public void BasicPose(int duration, int no_steps)
	{
		PlayPose(duration, no_steps, basic_pos, true);
	}
	
	public boolean PlayFile(String filename)
	{ 
		return PlayFile(filename, 0, 0);
	}

	public boolean PlayFile(String filename, int startrow, int endrow)
	{
		try
		{
		    FileInputStream fstream = new FileInputStream(filename);
		    DataInputStream in = new DataInputStream(fstream);
		    return PlayFile(in, startrow, endrow);
	    }
	    catch (IOException e)
	    {
	    	return false;
	    }  
	}
	
	public boolean PlayFile(InputStream is, int startrow, int endrow)
	{	
		byte[] servo_pos;
		int steps;
		int duration;
		int nos = 0;
		int n = 0;
		tcnt = 0;
		int linecount = 0;

		try
		{
	        BufferedReader tr = new BufferedReader(new InputStreamReader(is));	 
			
			String line = "";

			while ((line = tr.readLine()) != null)
			{
				line = line.trim();
				linecount++;

				if (!(linecount>=startrow && ( linecount<=endrow || endrow==0)))
					continue;

				Debug.WriteLine(linecount + " - " + line);

				if (line.startsWith("#")) // comment
				{
					Debug.WriteLine(line);
					if (line.startsWith("#V=01,,"))
						nos = 18;
					if (line.startsWith("#V=01,N=18,"))
						nos = 18;
					if (line.startsWith("#V=01,N=16,"))
						nos = 16;
					if (line.startsWith("#V=01,N=20,"))
						nos = 20;
					continue;
				}

				String[] r = line.split(",");

				if (nos == 0)
				{
					if (r.length > 20)
						nos = r.length - 5; // assume XYZ have been appended
					else 
						nos = r.length - 2;
				}

				if (nos > 0)
				{
					servo_pos = new byte[nos];
					n++;

					duration = Integer.parseInt(r[0]);
					steps = Integer.parseInt(r[1]);

					for (int i = 0; i < nos; i++)
						servo_pos[i] = (byte)Integer.parseInt(r[i + 2]);

					if (!PlayPose(duration, steps, servo_pos, (n == 1))) 
						return false;
				}

			}
			tr.close();
		}
		catch (Exception e1)
		{
			//Console.WriteLine("Error - can't load file " + e1.getMessage());
			return false;
		}
		return true;
	}

	// Different type of move interpolation
	// from http://robosavvy.com/forum/viewtopic.php?t=5306&start=30
	// original by RN1AsOf091407
	double CalculatePos_AccelDecel(int Distance, double FractionOfMove)
	{
		if ( FractionOfMove < 0.5 )     // Accel:
			return CalculatePos_Accel(Distance /2, FractionOfMove * 2);
		else if (FractionOfMove > 0.5 ) //'Decel:
			return CalculatePos_Decel(Distance/2, (FractionOfMove - 0.5) * 2) + (Distance * 0.5);
		else                            //'= .5! Exact Middle.
			return Distance / 2;
	}

	double CalculatePos_Accel(int Distance, double FractionOfMove) 
	{
		return FractionOfMove * (Distance * FractionOfMove);
	}

	double CalculatePos_Decel(int Distance, double FractionOfMove)
	{
		FractionOfMove = 1 - FractionOfMove;
		return Distance - (FractionOfMove * (Distance * FractionOfMove));
	}

	double CalculatePos_Linear(int Distance, double FractionOfMove)
	{
		return (Distance * FractionOfMove);
	}

	double GetMoveValue(MoveTypes mt, int StartPos, int EndPos, double FractionOfMove)
	{
		int Offset,Distance;
		if (StartPos > EndPos)
		{
			Distance = StartPos - EndPos;
			Offset = EndPos;
			switch (mt)
			{
				case  Accel:			
				//case MoveTypes.Accel:
					return Distance - CalculatePos_Accel(Distance, FractionOfMove) + Offset;
				case AccelDecel:
					return Distance - CalculatePos_AccelDecel(Distance, FractionOfMove) + Offset;
				case Decel:
					return Distance - CalculatePos_Decel(Distance, FractionOfMove) + Offset;
				case Linear:
					return Distance - CalculatePos_Linear(Distance, FractionOfMove) + Offset;
			}
		}
		else
		{
			Distance = EndPos - StartPos;
			Offset = StartPos;
			switch (mt)
			{
				case Accel:
					return CalculatePos_Accel(Distance, FractionOfMove) + Offset;
				case AccelDecel:
					return CalculatePos_AccelDecel(Distance, FractionOfMove) + Offset;
				case Decel:
					return CalculatePos_Decel(Distance, FractionOfMove) + Offset;
				case Linear:
					return CalculatePos_Linear(Distance, FractionOfMove) + Offset;
			}
		}
		return 0.0;
	}

	// NEW:: if byte = 255 = use current position
	// NEW:: check limits / bounds before sending
	// NEW:: move types added, will use linear interpolation between points unless specified


	public boolean PlayPose(int duration, int no_steps, byte[] spod, boolean first)
	{
		return PlayPose(duration, no_steps, spod, first, cmt);
	}
	
	public int cvb2i(byte x) {return (x<0)? 256+x:x;}

	public boolean PlayPose(int duration, int no_steps, byte[] spod, boolean first, MoveTypes ty)
	{
		int cnt = 0;

		byte[] temp = new byte[spod.length];

		if (first || !initpos)
		{
			servoID_readservo(spod.length); // read start positions
			tcnt = 0;
		}

		duration = (int)(0.5+(double)duration * kfactor);

		// bounds check
		for (int n = 0; n < spod.length ; n++)
		{
			if (spod[n] != (byte)255)
			{
				if (n < lb_Huno.length)
				{
					if (cvb2i(spod[n]) < lb_Huno[n]) spod[n] = (byte)lb_Huno[n];
					if (cvb2i(spod[n]) > ub_Huno[n]) spod[n] = (byte)ub_Huno[n];
				}
				cnt++;
			}
		}

		for (int s = 1; s <= no_steps; s++)
		{
			//
			for (int n = 0; n < spod.length; n++) 
			{
				if (spod[n] == (byte)255)
					temp[n] = pos[n];
				else
					temp[n] = (byte)GetMoveValue(ty, cvb2i(pos[n]), cvb2i(spod[n]), (double)s / (double)no_steps);

			}

			SyncPosSend(temp.length - 1, 4, temp, 0);

			int td = duration / no_steps;
			if (td<25) td=25;
			delay_ms(td);
		}

		for (int n = 0; n < spod.length; n++)
		{
			if (spod[n] != (byte)255)
				pos[n] = spod[n];
		}

		return true; // complete
	}

}
