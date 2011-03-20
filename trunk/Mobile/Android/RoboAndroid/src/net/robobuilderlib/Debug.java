package net.robobuilderlib;

import android.util.Log;

public class Debug {
	
	//-- debugging --//	
    private static final boolean D = true;
    static String LOG= "MoboRobo";

	public static void WriteLine(String string) {
		// TODO Auto-generated method stub
		Write(string + "\n");	
	}

	public static void Write(String string) {
		// TODO Auto-generated method stub
        if (D) Log.i(LOG, "++ " + string);
	}

}
