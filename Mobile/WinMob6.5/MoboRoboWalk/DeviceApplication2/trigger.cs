using System;

namespace MoboRobo
{

    public class trigger
    {
        public int Xmax, Xmin, Xval;    //accelerometer X axis
        public int Ymax, Ymin, Yval;    //accelerometer Y axis
        public int Zmax, Zmin, Zval;    //accelerometer Z axis
        public int Pmax, Pmin, Pval;    //PSD sensor
        public int Smax, Smin, Sval;    //Sound Level
        public int Ival;                //IR remote

        public bool AccTrig = false;   // trigger on acceleromter val <min or >max
        public bool PSDTrig = false;   // trigger PSD val <min or >max
        public bool SndTrig = false;   // trigger Snd level val <min or >max
        public bool IRTrig = false;   // trigger IR being recieved
        public bool status = false;   // this must be true to activate

        public bool dbg ; //{ get; set; }  // this must be true for debug info
        public bool DCMP ; //{ get; set; }  // this must be true for DCMP high speed mode (custom firmware)

        public int timer;// { get; set; }  //trigger timer (in ms)

        public double[] delta   = null; // offset to add when active

        public trigger()
        {
            timer = 250; //default value

            set_accel(0, 0, 0, 0, 0, 0);
            set_PSD(0, 0);
            set_SND(0, 0);
            set_IR(0);
            AccTrig = false;
            PSDTrig = false;
            SndTrig = false;
            IRTrig = false;
            dbg = false;
            DCMP = false;
        }

        public bool test()
        {
            return
                (AccTrig == true && (Xval < Xmin || Yval < Ymin || Zval < Zmin || Xval > Xmax || Yval > Ymax || Zval > Zmax))
             || (PSDTrig == true && (Pval > Pmax || Pval < Pmin))
             || (IRTrig == true && Ival != 0)
             || (SndTrig == true && (Sval > Smax || Sval < Smin));
        }

        public void set_trigger(bool acc, bool psd, bool snd, bool ir)
        {
            AccTrig = acc;
            PSDTrig = psd;
            SndTrig = snd;
            IRTrig = ir;
        }

        public void set_accel(int minx, int miny, int minz, int maxx, int maxy, int maxz)
        {
            Xmax = maxx; Xmin = minx; Xval = 0;     //defaults : accelerometer X axis
            Ymax = maxy; Ymin = miny; Yval = 0;     //defaults : accelerometer Y axis
            Zmax = maxz; Zmin = minz; Zval = 0;     //defaults : accelerometer Z axis
            AccTrig = true;
        }

        public void set_delta(double[] m)
        {
            delta = m;
        }

        public void set_PSD(int minp, int maxp)
        {
            Pmax = maxp; Pmin = minp; Pval = 0;
            PSDTrig = true;
        }

        public void set_SND(int mins, int maxs)
        {
            Smax = maxs; Smin = mins; Sval = 0;
            SndTrig = true;
        }

        public void set_IR(int ir)
        {
            Ival = 0;
            IRTrig = true;
        }

        public void set_trigger(int n)
        {
            AccTrig = ((n | 1) == 1);
            PSDTrig = ((n | 2) == 2);
            SndTrig = ((n | 4) == 4);
            IRTrig = ((n | 8) == 8);
        }

        public void activate(bool f)
        {
            status = f;
        }

        public bool active()
        {
            return status;
        }

        public void print()
        {
            try
            {
                Console.WriteLine("Trigger status : {0}", (status) ? "On" : "Off");
                Console.WriteLine("Timer          = {0 } ms", timer);
                Console.WriteLine("X={0:###} : {1:###} : {2:###} : {3}", Xmin, Xmax, Xval, ((AccTrig) ? "On" : "Off").ToString());
                Console.WriteLine("Y={0:###} : {1:###} : {2:###}", Ymin, Ymax, Yval);
                Console.WriteLine("Z={0:###} : {1:###} : {2:###}", Zmin, Zmax, Zval);
                Console.WriteLine("P={0:#}   : {1:#}   : {2:#}   : {3}", Pmin, Pmax, Pval, ((PSDTrig) ? "On" : "Off").ToString());
                Console.WriteLine("S={0:#}   : {1:#}   : {2:#}   : {3}", Smin, Smax, Sval, ((SndTrig) ? "On" : "Off").ToString());
                Console.WriteLine("I={0:#}   : {1}", Ival, ((IRTrig) ? "On" : "Off").ToString());
                Console.WriteLine("Delta={0}", ((delta!=null) ? "On" : "Off").ToString());
            }
            catch (Exception e1)
            {
                Console.WriteLine("exception - " + e1.Message);
            }
        }
    }
}
