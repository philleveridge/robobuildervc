package net.robobuilderlib;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;



public class Serial 
{
	public  boolean IsOpen = false;
	public int WriteTimeout;
	public int ReadTimeout;
	
	InputStream in;
	OutputStream out;
	
	public Serial (InputStream a, OutputStream b)
	{
		in = a;
		out= b;
		IsOpen=true;
	}

	public void Write(byte[] buff, int off, int nob) {
		// TODO Auto-generated method stub
		if (!IsOpen) return;
		
		try {
			out.write(buff, off, nob);
		}
		catch (IOException e) {
			return ;
	    }		
	}

	public byte ReadByte() {
		// TODO Auto-generated method stub
		if (!IsOpen) return 0;
		
		try {
			return (byte)in.read();
		} 
		catch (IOException e) {
			return 0;
	    }
	}

	public void Close() {
		// TODO Auto-generated method stub
		IsOpen=false;
		
	}

	public void Open() {
		// TODO Auto-generated method stub
		IsOpen = true;
	}

}
