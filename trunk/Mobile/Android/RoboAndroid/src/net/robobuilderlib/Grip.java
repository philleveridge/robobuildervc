package net.robobuilderlib;

class Grip
{
    static public int SERVOID = 18;
    wckMotion w;

    public Grip(wckMotion a)
    {
        w=a;
    }

    int widepos = 70;
    int closepos = 120;
    int maxit = 15;
    double dt = 0.15;

    public void opengripper(int d)
    {
        int cnt = 0;
        int cp = getServoPos(SERVOID);
        int np = 0;
        int delta = 5;

        if (cp < widepos)
            cp = widepos;

        while ((cnt < maxit) && (Math.abs(cp - widepos) > 2) && (delta > 0))
        {
            cnt++;
            Debug.WriteLine(cnt + ", " + cp + ", " + delta);

            setServoPos(SERVOID, cp - d, 4);
            sleep(dt);
            np = getServoPos(SERVOID);
            delta = Math.abs(cp - np);
            cp = np;
        }
    }

	public void closegripper(int d)
    {
        int cnt = 0;
        int cp = getServoPos(SERVOID);
        int np = 0;
        int delta = 5;

        if (cp > closepos)
            cp = closepos;

        while ((cnt < maxit) && (Math.abs(cp - closepos) > 2) && (delta > 0))
        {
            cnt++;
            Debug.WriteLine(cnt + ", " + cp + ", " + delta);
            setServoPos(SERVOID, d + cp, 2);
            sleep(dt);
            np = getServoPos(SERVOID);
            delta = Math.abs(cp - np);
            cp = np;
        }
    }

	private void sleep(double dt2) {
		w.delay_ms((int) dt2*1000);
		
	}

	private int getServoPos(int n) {
        if (w.wckReadPos(n))
            return w.respnse[1];
        else
            return 0;
	}
	
    private int setServoPos(int n, int p, int t) {
        if (w.wckMovePos(n,p,t))
            return 1;
        else
            return 0;
		
	}
}
