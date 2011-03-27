package net.robobuilderlib;

import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.UUID;

import net.robobuilderlib.RoboAndroid.SwModes;

import android.app.Activity;
import android.app.ProgressDialog;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.os.Vibrator;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.Gallery;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.RadioButton;

import android.widget.TextView;
import android.widget.Toast;


public class RoboAndroid extends Activity implements OnClickListener, OnItemClickListener 
{   	
	public  enum 	SwModes 				{ Notset, Basic, RBFirmware, DCMP };
	public  enum 	OpModes 				{ Notset, Kbd,   Joystick,   Remco };
	
	ListView		m_lvSearch;	
	ProgressDialog	m_progDlg;
	
	SwModes 		swmode					= SwModes.Notset; 
	OpModes 		opmode					= OpModes.Notset; 	
	int				rbconfig				= 0; 				//BIT wise: (0=standard, 1=hipkit, 2=Dance hands)
	
	//-- Bluetooth functionality --//
    boolean 					nobt					=true; // =false; // 
	
	final static int			MAX_DEVICES				= 50;
	 
	BluetoothAdapter 			m_BluetoothAdapter;
	boolean						m_ASDKScanRunning		= false;
	int							m_nDiscoverResult 		= -1;
	int							m_nRoboDev				= -1;
    // Intent request codes
    final 		int 			REQUEST_CONNECT_DEVICE 	= 1,
    							REQUEST_ENABLE_BT 		= 2;
    BluetoothSocket 			m_btSck;									//used to handle Android<->Robot communication
    private static final UUID 	SPP_UUID 				= UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
    Thread						m_hReadThread;   
    
    Bitmap hotspot	=null;
    Menu   mymenu	=null;
	
	public static final int 	idLVFirstItem		= Menu.FIRST + 100;	

	class BTDev {
		String 	m_szName;
		String 	m_szAddress;
		int		m_nBTDEVType; //if 1, it's my Robobuilder, if 0 it's a regular device
		
		
		BTDev(String _name, String _address) {
			m_szName = _name; m_szAddress = _address;  
		}
		BTDev(String _name, String _address, int _type) {
			m_szName = _name; m_szAddress = _address; m_nBTDEVType = _type;  
		}
	}
	BTDev	BTDevs[];
	int		BTCount;
    
	 /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        Debug.WriteLine("++ ON CREATE ++");
        
        //m_BT = new BTNative();
        BTDevs = new BTDev[MAX_DEVICES]; 
     
if (!nobt){
        m_BluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        // If the adapter is null, then Bluetooth is not supported
        
        if (m_BluetoothAdapter == null) {
            Toast.makeText(this, "Bluetooth is not available", Toast.LENGTH_LONG).show();
            finish();
            return;
        }
        if (!m_BluetoothAdapter.isEnabled()) 
        {
        	// enable bluetooth
            Intent enableIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
            startActivityForResult(enableIntent, REQUEST_ENABLE_BT);
        }       
        
        // Register for broadcasts when a device is discovered
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        this.registerReceiver(mReceiver, filter);

        // Register for broadcasts when discovery has finished
        filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
        this.registerReceiver(mReceiver, filter); 
} 
        
        // disable the titlebar
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        // create the interface
        Debug.WriteLine("++ VIEW START ++");
        
        setContentView(R.layout.main);           
        
        startIntro(); 
    }
    
    @Override
    public synchronized void onPause() {
        super.onPause();
        Debug.WriteLine("- ON PAUSE -");
    }

    @Override
    public void onStop() {
        super.onStop();
        Debug.WriteLine("-- ON STOP --");
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        // Stop the Bluetooth services
        Disconnect(0);
        Debug.WriteLine("--- ON DESTROY ---");
    }
       
    // The BroadcastReceiver that listens for discovered devices and
    // changes the title when discovery is finished
    public final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
                        
            // When discovery finds a device
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                // Get the BluetoothDevice object from the Intent
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                //if (device.getBondState() != BluetoothDevice.BOND_BONDED) // If it's already paired, skip it, because it's been listed already
            	//-- ignore duplicates
				boolean duplicate = false;
				for (int j=0;j<BTCount;j++)
					if (BTDevs[j].m_szAddress.compareTo(device.getAddress()) == 0) { duplicate = true; break; }
				if (duplicate)
					; //this is a duplicate
				else
				{
					if (device.getName().startsWith("ESD200"))
						BTDevs[BTCount] = new BTDev(device.getName(), device.getAddress(), 1);
					else
						BTDevs[BTCount] = new BTDev(device.getName(), device.getAddress(), 0);
	                BTCount++;
				}
                
            // When discovery is finished
            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
            	m_ASDKScanRunning = false; 
            }
        }
    };

	@Override
	public void onClick(View v) {      
		int cmdId = v.getId();	
		Debug.WriteLine("++ ON CLICK ++" + cmdId);
	}
    
	@Override
	public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3) {
		
		Debug.WriteLine("++ ON CLICK ITEM ++");
        
		int nIndex = -1, nCounter = 0;
		for (int i=0;i<BTCount;i++)
		{
			if (arg2 == nCounter) {
				nIndex = i;
				break;
			}
			nCounter++;
		}
		// connect to 
		if (BTDevs[nIndex].m_nBTDEVType == 1)
		{
			//connect to ROBOT
			m_lvSearch.setAdapter(null);
			StartReadThread(nIndex);
		}
		else 
			Toast.makeText(getBaseContext(), 
					"This is not ROBO", Toast.LENGTH_SHORT).show();		
	}
	
	// put the /BTDEvs in the listview
	void PopulateLV()
	{		
		Debug.WriteLine("++ ON POPULATE ++");
        
		ArrayList<Device> m_Devices = new ArrayList<Device>();
		Device device;
        for (int i=0;i<BTCount;i++) 
        {
        	if (BTDevs[i].m_szName.startsWith("ESD200")) 
        	{
        		BTDevs[i].m_nBTDEVType = 1;
        		m_nRoboDev = i;
        	}
        	else 
        	{
        		BTDevs[i].m_nBTDEVType = 0;
        	}
        	device = new Device(BTDevs[i].m_szName, 
        			BTDevs[i].m_szAddress, 
        			BTDevs[i].m_nBTDEVType,
        			0, 
        			idLVFirstItem+i);
        	
        	m_Devices.add(device);
        }       
        
        if ((m_lvSearch =(ListView) findViewById(R.id.in))  == null)
        {
        	Debug.WriteLine("++ LV FAILED ++");
        }
        else
        {  
            CustomAdapter lvAdapter =  new CustomAdapter(this, m_Devices);
            if (lvAdapter!=null) 
            {
            	m_lvSearch.setAdapter(lvAdapter);
            	m_lvSearch.setOnItemClickListener((OnItemClickListener) this);
            }
 
		    if (m_nRoboDev >= 0)
		    	Toast.makeText(getBaseContext(), "ROBO found as " + BTDevs[m_nRoboDev].m_szAddress, 
		    			Toast.LENGTH_LONG).show();
        }
	}
	
	/** Bluetooth Functions **/
	
	// not Blocking, uses events
	int ASDKDiscoverBluetoothDevices()
	{
		if (m_BluetoothAdapter.isDiscovering()) 
    		m_BluetoothAdapter.cancelDiscovery();
        
		int current_devs = BTCount;
		// Request discover from BluetoothAdapter
    	if (!m_BluetoothAdapter.startDiscovery()) return -1; //error
    	
    	m_ASDKScanRunning = true;

    	//  blocking operation:wait to complete
        while (m_ASDKScanRunning);
         
        return BTCount - current_devs;
	}

	final Runnable mUpdateResultsDiscover = new Runnable() {
        public void run() {
        	doneDiscoverBluetoothDevices();
        }
    };
    protected void startDiscoverBluetoothDevices() {
    	// Show Please wait dialog
    	m_progDlg = ProgressDialog.show(this, getString(R.string.app_name), "Scanning, please wait",
    			true);
    	
    	// Fire off a thread to do some work that we shouldn't do directly in the UI thread
	    Thread t = new Thread() {
	    	public void run() 
	    	{
	    		// blocking operation
            		m_nDiscoverResult = ASDKDiscoverBluetoothDevices();
            	//show results
	        	m_Handler.post(mUpdateResultsDiscover);
	    	}
	    };
	    t.start();
    }
    
    private void doneDiscoverBluetoothDevices() 
    {
    	m_progDlg.dismiss();
    	if (m_nDiscoverResult == -1)
    		Toast.makeText(getBaseContext(), "Bluetooth ERROR (is bluetooth on?)", Toast.LENGTH_LONG).show();
    	else if (m_nDiscoverResult == 0)
    		Toast.makeText(getBaseContext(), "No Bluetooth devices found", Toast.LENGTH_LONG).show();
    	else {
    		m_nRoboDev = -1;
    		// populate
			PopulateLV();
    	}
    }
    int Connect(int nIndex)
	{   	
    	Debug.WriteLine("++ ON CLICK CONNECT ++ " + nIndex);
        
		if (nIndex >= BTCount || nIndex<0) return -1; //invalid device
		
		//--connect serial port
		BluetoothDevice ROBOBTDev = m_BluetoothAdapter.getRemoteDevice(BTDevs[nIndex].m_szAddress);
		try {
			m_btSck = ROBOBTDev.createRfcommSocketToServiceRecord(SPP_UUID);
		} catch (IOException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		try { //This is a blocking call and will only return on a successful connection or an exception
			m_btSck.connect();	             
		} catch (IOException e) {
             // Close the socket
             try { m_btSck.close();} catch (IOException e2) { e2.printStackTrace();}
             return -2; 
         }
		return 0;
	}
    
    /**
     * Sends a message.
     * @param message  A string of text to send.
     */
    private void sendMessage(String message) {
    	Debug.WriteLine("++ ON SEND MESSAGE ++");
        
		if (m_btSck != null)
			try {			
		        // Check that there's actually something to send
		        if (message.length() > 0) {
		        	
		        	Debug.WriteLine("++ MESSAGE = " + message);
		            
		            // Get the message bytes and tell the BluetoothChatService to write
		            byte[] send = message.getBytes();	            
					m_btSck.getOutputStream().write(send);
		        }
				
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			else
			{
	            Toast.makeText(this, R.string.not_connected, Toast.LENGTH_SHORT).show();
	            return;
			}
    }	
    
    int Disconnect(int nIndex)
	{
		if (nIndex >= BTCount || nIndex<0) return -1; //invalid device
		
		// DISCONNECT ASDK SOCKETS
		if (m_btSck != null) {
			try {
				m_btSck.close();
			} catch (IOException e) {
				// TODO Auto-generated catch block
            }
			m_btSck = null;
		}
		return 0;
	}

    // Worker functions
	int StartReadThread(final int nIndex)
	{	
		Debug.WriteLine("StartReadThread: Connect");
		if (!nobt)
			Connect(nIndex);
		
		if (swmode==SwModes.DCMP || swmode==SwModes.RBFirmware)
		{
			startSimple();
			return 0;
		}
		
		updateText("\0332JCONNECT - " + checkVersion() + "\n");				
	
		m_hReadThread = new Thread() {
	        public void run() 
	        {        	
	        	while (true) 
	        	{
	        		try {
	                    // Read from the InputStream
	        			byte[] buffer = new byte[1024]; 
	        			int bread;
	        			String r="";
	        			
	        			if (m_btSck==null)
	        			{
	        				r="FAIL";
		        			Disconnect(nIndex);
		        			break;
	        			}
	        			else
	        			{	
		        			bread = m_btSck.getInputStream().read(buffer);	        			        					        			
		        			for (int i=0; i<bread; i++)
		        			{
		        				if (buffer[i] != 13 )
		        					r += (char)buffer[i];
		        			}   
	        			}       

	                    // Send the obtained bytes to the UI Activity
	        			Debug.WriteLine("StartReadThread: Data received:" + r);
	                    
	                    Message m = Message.obtain(m_Handler);
	                    m.obj = (Object)r;
	                    
	                    m_Handler.sendMessage(m); 
	        				                   
	                } catch (IOException e) {
	                	Debug.WriteLine("StartReadThread: disconnected" + e.getMessage());
	        			Disconnect(nIndex);
	        			break;
	                }
	        	}
	        }
        };
        m_hReadThread.start();
		return 0;
	}
	
	private String  checkVersion()
	{
		String r = "?";
    	Debug.WriteLine("++ ON VER ");
    	
    	if (nobt) return "NOBT";
		
		switch (swmode)
		{
		case Basic:
			r = checkVersionBasic();
			break;		
		case DCMP:
			r = checkVersionDCMP();
			break;		
		case RBFirmware:
			r = checkVersionFirmware();
			break;
		}	
    	Debug.WriteLine("++ ON VER =" + r);
		return r;
	}
	
	private String checkVersionFirmware()
	{
    	Debug.WriteLine("++ CHK FIRMWRE ");
		try {
			Serial sp = new Serial(m_btSck.getInputStream(),m_btSck.getOutputStream());
			pcRemote p = new pcRemote(sp);
			return p.readVer();
		}
		catch (IOException e)
		{
		}		
		return "?";
	}
	
	private String  checkVersionDCMP()
	{
    	Debug.WriteLine("++ CHK DCMP ");
		try {
			Serial sp = new Serial(m_btSck.getInputStream(),m_btSck.getOutputStream());
			wckMotion w = new wckMotion(sp);
			if (w.wckReadPos(30, 0)) // DCMP this return version
			{
				String v = "V=" + (int)(w.respnse[0]) + "." + (int)(w.respnse[1]);
				//v += ", Servos=" + w.countServos();
				return v;
			}
		}
		catch (IOException e)
		{
		}			
		return "?";
	}
	
	private String  checkVersionBasic()
	{
    	Debug.WriteLine("++ CHK BASIC ");
    	
		byte[] buffer = new byte[1024]; 
		int bread;
		String r="";
		
		try {	
			InputStream ins = m_btSck.getInputStream();
			
			int nb;
			if ((nb = ins.available())>0)
				ins.skip((long)nb); // clear any existing input
			
			sendMessage("V");
			
			nb=0;
			
			while (nb<15)
			{
				bread = ins.read(buffer);	        			        					        			
				for (int i=0; i<bread; i++)
				{
					if (buffer[i] != 13 )
						r += (char)buffer[i];
				} 
				nb=nb+bread;
			}
			
			int p=r.indexOf("$Revision: ");
			if (p<0)
			{
	        	r = "? [" + r + "]";				
			}
			else
			{
				r = "BASIC V=" + r.substring(p+11, p+14);
			}
		}
		catch (IOException e) {
        	r= "No response";
        }
		return r;
	}
	
	  final Handler m_Handler = new Handler() 
	  {
		  // Instantiating the Handler associated with the main thread.

	      @Override
	      public void handleMessage(Message msg) {  
	    	  updateText((String)msg.obj);
	      }

	  };

	
    private void updateText(String s)
    {
    	Debug.WriteLine("++ Update text :: " + s);
        
        final TextView mt = (TextView) findViewById(R.id.TextView01); 
	    if (mt != null && s != null && s.length()>0) 
	    {
	    	int p = s.indexOf("\0332J");
	    	if (p>=0)
	    	{
	    		mt.setText("");
	    		s = s.substring(p+3);
	    	}
	    	mt.append(s);
	    }
    }
    
    protected void startIntro()
    {
        Button    m_sb;	
    	
    	Debug.WriteLine("++ INTRO ++");   	
    	
        LayoutInflater li = (LayoutInflater)this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        FrameLayout contentPane = (FrameLayout)findViewById(R.id.FrameLayout01); 
        
    	Debug.WriteLine("++ FRAME SET ++");
        
        contentPane.removeAllViews();
        contentPane.addView( li.inflate(R.layout.intro, null) );          
        
        if ((m_sb = (Button) findViewById(R.id.Button01))  == null)
        {
        	Debug.WriteLine("++ BUTTON FAILED ++");
        }
        else
        {          
	        m_sb.setOnClickListener(new OnClickListener() {
	            public void onClick(View v) {
	                // Send a message using content of the edit text widget
	            	// check state of radio button
	            	
	            	RadioButton rb1 = (RadioButton) findViewById(R.id.RadioButton01);
	            	RadioButton rb2 = (RadioButton) findViewById(R.id.RadioButton02);
	            	RadioButton rb3 = (RadioButton) findViewById(R.id.RadioButton03);
	            	swmode=SwModes.Notset;
	            	if (rb1.isChecked()) {swmode=SwModes.Basic;}
	            	if (rb2.isChecked()) {swmode=SwModes.DCMP;}
	            	if (rb3.isChecked()) {swmode=SwModes.RBFirmware;}
	            	
	            	if (swmode==SwModes.Notset) 
	            		return; //ignore click
	            	
	            	if (swmode==SwModes.DCMP)
	            	{
	            		LinearLayout l = (LinearLayout)findViewById(R.id.LinearLayout01);
	            		if (l.getVisibility() == View.INVISIBLE)
	            		{
	            			l.setVisibility(View.VISIBLE);
	            			
		            		CheckBox t;
		            		t = (CheckBox)findViewById(R.id.hk);
		            		t.setChecked((rbconfig& 1)==1);
		            		
		            		t = (CheckBox)findViewById(R.id.dh);
		            		t.setChecked((rbconfig& 2)==2);		        		            		
	            			return;
	            		}
	            		else
		            	{
		            		CheckBox t;
		            		t = (CheckBox)findViewById(R.id.hk);
		            		if (t.isChecked())
		            			rbconfig |= 1;
		            		
		            		t = (CheckBox)findViewById(R.id.dh);
		            		if (t.isChecked())
		            			rbconfig |= 2;
		            	}
	            	}
	            	
	            	Debug.WriteLine("++ BUTTON CHECKED ++" + swmode +"," + rbconfig);	           	
	            	
	            	startKBD();	            	            	
	    			if (nobt) 
	    				StartReadThread(0); // debugmode
	    			else
	    				startDiscoverBluetoothDevices();		
	            }
	        });
        }      
    }
 
    
    protected void startSimple()
    {
        String[] Actions=null;
        
        LayoutInflater li = (LayoutInflater)this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        FrameLayout contentPane = (FrameLayout)findViewById(R.id.FrameLayout01);  
 
        contentPane.removeAllViews();      
        contentPane.addView( li.inflate(R.layout.picklist, null) );
        
        setMenus();
        
        if (swmode==SwModes.Basic || (m_btSck == null && !nobt)) 
        	return; // not BASIC
        
        TextView tv = (TextView) findViewById(R.id.pln);                	
        ListView lv = (ListView) findViewById(R.id.list);   
     
        if (swmode==SwModes.DCMP)
        {
        	tv.setText("DCMP " + checkVersion());
        	
            /* ------------------------------*/
            
            AssetManager assetManager = getAssets();
            	        Actions = null;
            	        try {
            	            Actions = assetManager.list("");
            	        } catch (IOException e) {
            	            Log.e("tag", e.getMessage());
            	        }
            	       
            int f=0; 	        
            for (int i=0; i<Actions.length; i++)
            {
            	if (Actions[i].endsWith(".rbm") || Actions[i].endsWith(".csv")|| Actions[i].endsWith("!") )
            	{
            		Actions[f] = Actions[i];
	            	Actions[f] = Actions[f].replace(".rbm", "");
	            	Actions[f] = Actions[f].replace(".csv", "");
	            	Actions[f] = Actions[f].replace("_", " ");
	            	
	            	if ((Actions[i].endsWith(".dh") && !((rbconfig & 2)==2)) || (Actions[i].endsWith(".18") && !((rbconfig & 1)==1)))
	            		;	
	            	else
	            		f++;
            	}
            	else
            	{
            		Actions[i]="";
            	}
            }
            
            String[] newArray = new String[f];
            for (int i=0; i<f; i++)
            	newArray[i] = Actions[i];
            
            java.util.Arrays.sort(newArray,String.CASE_INSENSITIVE_ORDER);            
            lv.setAdapter(new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, newArray));
           
            
        	lv.setOnItemClickListener(new OnItemClickListener() {
    			@Override
    			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2, long arg3) 
    			{				
    				Serial sp=null;;
    				String s = (arg0.getAdapter().getItem(arg2)).toString();  		        
    				Debug.WriteLine("++ CLICK= " + arg2 + "=" + s );
    				
    				if (nobt) return;
    				
    				if (s.equals("")) return; // ignore blanks
    		        
    		        try {     
    					if (!nobt) sp = new Serial(m_btSck.getInputStream(),m_btSck.getOutputStream());
    					wckMotion w = new wckMotion(sp);
    					
    					if (s.equals("$BASIC POSE!"))
    					{
    						if ((rbconfig & 2)==2) 
    							w.PlayPose(1000, 10, wckMotion.basicdh, true);
    						else if ((rbconfig & 1)==1) 
    							w.PlayPose(1000, 10, wckMotion.basic18, true);
    						else if (rbconfig==0)       
    							w.PlayPose(1000, 10, wckMotion.basic16, true);
    					}
    					else if (s.equals("$OPEN GRIPPER!"))
    					{
    						Grip x = new Grip(w);
    						x.opengripper(4);
    					}
    					else if (s.equals("$CLOSE GRIPPER!"))
    					{
    						Grip x = new Grip(w);
    						x.closegripper(4);
    					}
    					else if (s.equals("$FORWARD!"))
    					{
    						Walk x = new Walk(w);
    						x.forward((rbconfig&1)==1?18:16); 
    					}
    					else
    					{ 
    						s.replace(" ", "_");
    						if (s.endsWith(".dh") && (rbconfig&2)==2)
    						{
		    					InputStream is = getAssets().open(s +".csv");
		    					w.PlayFile(is, 0, 0); 
    						}
    						else if (s.endsWith(".18") && (rbconfig&1)==1)
    						{
		    					InputStream is = getAssets().open(s +".csv");
		    					w.PlayFile(is, 0, 0); 
    						}
    						else
    						{
		    					InputStream is = getAssets().open(s +".rbm");
		    					Motion m = new Motion();
		    					m.LoadFile(is);
		    					m.Play(w, (rbconfig&1)==1, (rbconfig&2)==2);
    						}
    					}
    		        }
    		        catch (IOException e)
    		        {
    		        }  				   			   				
    			}});
        
        }
        
        if (swmode==SwModes.RBFirmware)
        { 
        	tv.setText("FIRMWARE " + checkVersion());
        	
        	Actions = this.getResources().getStringArray(R.array.actions);
        	
            lv.setAdapter(new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, Actions));
        	
        	lv.setOnItemClickListener(new OnItemClickListener() {
    			@Override
    			public void onItemClick(AdapterView<?> arg0, View arg1, int arg2,long arg3) 
    			{
    				// TODO Auto-generated method stub
    				
    				Debug.WriteLine("++ CLICK=" + arg2);			
    				
        			Serial sp;
					try {
						if (m_btSck != null) {
							sp = new Serial(m_btSck.getInputStream(),m_btSck.getOutputStream());
		        			pcRemote pc = new pcRemote(sp);
	                    	pc.run(arg2+1); //built in motion
						}							        			
					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}       							
    			}});
        }       

    }    
    
    void setMenus()
    {
    	//swmod 0=Basic, 1= Firm, 2= DCMP
    	//j,r,k,s
    	Debug.Write("++ MENUS ENABLED");
    	
    	if (mymenu != null && swmode!=SwModes.Notset)
    	{
    		mymenu.getItem(0).setEnabled((swmode==SwModes.Basic) || (swmode==SwModes.RBFirmware));  //joy
    		mymenu.getItem(1).setEnabled(false);  //remco
    		mymenu.getItem(2).setEnabled((swmode==SwModes.Basic)); //kbd
    		mymenu.getItem(3).setEnabled(!(swmode==SwModes.Basic)); //simple
    	}
    }
    
    protected void startKBD()
    {
        LayoutInflater li = (LayoutInflater)this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        FrameLayout contentPane = (FrameLayout)findViewById(R.id.FrameLayout01);  
        
    	TextView  m_tv;
        Button    m_sb;
        
        contentPane.removeAllViews();
        contentPane.addView( li.inflate(R.layout.kbdui, null) );
        
        setMenus();
        
    	hotspot=null;
    	
        if (( m_tv = (TextView) findViewById(R.id.TextView01))  == null)
        {
        	Debug.WriteLine("++ TV FAILED ++");
        }
        else
        {
        	m_tv.setText("");
            updateText("Robo V1.0\nPlease Connect");
        }      
        
        if ((m_sb = (Button) findViewById(R.id.button_send))  == null)
        {
        	Debug.WriteLine("++ BUTTON FAILED ++");
        }
        else
        {
            TextView view = (TextView) findViewById(R.id.edit_text_out);
            view.setText("V");
            
	        m_sb.setOnClickListener(new OnClickListener() {
	            public void onClick(View v) {
	                // Send a message using content of the edit text widget
	                TextView view = (TextView) findViewById(R.id.edit_text_out);
	                String message = view.getText().toString() + "\n";
	                Debug.WriteLine("++ SEND MSG S++");
	                sendMessage(message);
	                view.setText("");
	                Debug.WriteLine("++ SEND MSG E++");
	            }
	        });
        }
    }
    
    protected void startJoystick(OpModes n)
    {
    	ImageView im;    	
    	opmode = n;
    	
        //
        LayoutInflater li = (LayoutInflater)this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        FrameLayout contentPane = (FrameLayout)findViewById(R.id.FrameLayout01);    
        contentPane.removeAllViews();
        if (n==OpModes.Kbd)
        {
        	startKBD();
        	hotspot=null;
        	return;
        }
        else
        {
            contentPane.addView( li.inflate(R.layout.touchui, null) );                 
            setMenus();
        }
    		
        if ((im =(ImageView) findViewById(R.id.ImageView01))  == null)
        {
        	Debug.WriteLine("++ IM FAILED ++");
            return;
        }

        if (n==OpModes.Joystick) {
        	im.setImageResource(R.drawable.joy);
            hotspot = BitmapFactory.decodeResource(getResources(), R.drawable.joyb);      
        }
        if (n==OpModes.Remco) {
        	im.setImageResource(R.drawable.remote);
            hotspot = BitmapFactory.decodeResource(getResources(), R.drawable.remoteb);
        }
        
        im.setAdjustViewBounds(true); // set the ImageView bounds to match the Drawable's dimensions
        //im.setLayoutParams(new Gallery.LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT));
        
        im.setOnTouchListener(new View.OnTouchListener() {
            public boolean onTouch(View v,MotionEvent event) {
                int x = (int)event.getX();
                int y = (int)event.getY();
                Debug.WriteLine("Clicked" + x + "," + y);
                if (hotspot != null)
                {
                	int hx = hotspot.getWidth();
                 	int hy = hotspot.getHeight();
                 	int vx = v.getWidth();
                 	int vy = v.getHeight();             	
                 	
                 	x=(hx*x)/vx;
                 	y=(hy*y)/vy;
                 	
                	int n=hotspot.getPixel(x, y);
                	n = (n &0xFF0000)>>16;               	
                	Debug.Write("Pixel=" + n);     
                	if (n>0 && n<20)
                	{
                		//Vibrator.this.vibrate(50);
                		//                 0 1 2 3  4 5 6  7 8 9 0 1 2 3 4 5 6 7 8 9 
						int act=new int[] {0,7,7,11,5,3,9,10,2,4,5,7,7,7,7,7,7,7,7,7}[n]; //map pixel to remco action
							 
                		switch (swmode)
                		{
                		case Basic: // Basic
                			String m=  ""+(char)('A'+act);
                			if (nobt)
                            	Debug.Write("M=" + m);  
                			else
                				sendMessage(m);
                			break;
                		case DCMP: // DCMP
                			break;
                		case RBFirmware: // FIRMWARE
                			Serial sp;
        					try {
        						if (m_btSck != null) {
        							sp = new Serial(m_btSck.getInputStream(),m_btSck.getOutputStream());
        		        			pcRemote pc = new pcRemote(sp);
        	                    	pc.run(act); //built in motion
        						}							        			
        					} catch (IOException e) {
        						// TODO Auto-generated catch block
        						e.printStackTrace();
        					} 
                			break;
                		case Notset: //
            	            Toast.makeText(v.getContext(), "Please connect", Toast.LENGTH_SHORT).show();
                			break;
                		}
                	}              		
                }
                return true;
            }
        });
    }   
	
    @Override
    public boolean onCreateOptionsMenu(Menu menu) 
    {
        Debug.WriteLine("++ CREATE MENU  ++");
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.option_menu, menu);
        mymenu=menu;
        setMenus();
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) 
    {
        Debug.WriteLine("++ ON MENU ITEM ++");
        
        switch (item.getItemId()) 
        {
        case R.id.joy:		
        	if (opmode!=OpModes.Joystick)
        		startJoystick(OpModes.Joystick);
        	else
        		startJoystick(OpModes.Remco);
            return true;            
        case R.id.remco:		
        	startJoystick(OpModes.Remco);
            return true;        
        case R.id.kbd:		
        	startJoystick(OpModes.Kbd);
            return true;
        case R.id.simple:
			startSimple();		
            return true;
        }
        return false;
    }
}

