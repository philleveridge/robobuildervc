package net.robobuilderlib;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.FutureTask;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;



public class Serial 
{
	public  boolean IsOpen = false;
	public int WriteTimeout=500;
	public int ReadTimeout=500;
	
	InputStream in;
	OutputStream out;
	
    private static final ExecutorService THREADPOOL = Executors.newCachedThreadPool();

	private static <T> T call(Callable<T> c, long timeout, TimeUnit timeUnit)
	    throws InterruptedException, ExecutionException, TimeoutException
	{
	    FutureTask<T> t = new FutureTask<T>(c);
	    THREADPOOL.execute(t);
	    return t.get(timeout, timeUnit);
	}
	
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

	public byte ReadByte()  throws Exception
	{
		if (!IsOpen) return 0;
		
    	Integer b = call(new Callable<Integer>() {
            public Integer call() throws Exception
            {
    			return (Integer)in.read();
            }} , ReadTimeout, TimeUnit.MILLISECONDS);

    	Debug.Write("RBYTE: " + b);
    	return b.byteValue();
    }

	public byte ReadByte_b() {
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
