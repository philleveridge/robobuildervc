package net.robobuilderlib;

public class Debug {
	
	//-- debugging --//	
    public static final boolean D = true;
    static String LOG= "MoboRobo";

	public static void WriteLine(String str) {
		// TODO Auto-generated method stub
		Write(str + "\n");	
	}

	public static void Write(String str) {
		// TODO Auto-generated method stub
        if (D) 
        	android.util.Log.i(LOG, "++ " + str);
        else 
        	System.out.print("++ " + str);
	}

}
