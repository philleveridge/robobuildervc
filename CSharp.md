# Introduction #

This wiki explains how to use the RobobuilderLib library with your on C# code running on a PC to remotely control Robobuilder humanoid robot. For details on the to core classes see the following
  * [PCRemote](http://code.google.com/p/robobuildervc/wiki/PCRemote)
  * [wckMotion](http://code.google.com/p/robobuildervc/wiki/wckMotion)
  * [vectors](http://code.google.com/p/robobuildervc/wiki/vectors)
  * [Matrix](http://code.google.com/p/robobuildervc/wiki/Matrix)
  * Trigger

# Details #

Two basic examples
  * Console based application
  * Forms based

# Console based Example #

Download C# VS2005 demo files:
http://robobuilderlib.googlecode.com/files/demo.zip

Here's sample code:

```
using System;
using RobobuilderLib;

namespace Demo
{
    class Program
    {
        public PCremote p;
        public wckMotion w;
        public int nos = 0;

        public void standup()
        {
            if (nos < 16) return;
            w.PlayPose(1000, 10, wckMotion.basic16, true);
        }

        int countServos(int m)
        {
            for (int i = 0; i < m; i++)
            {
                if (!w.wckReadPos(i))
                {
                    nos = i;
                    return i;
                }
            }
            nos = m;
            return m;
        }

        public bool testServo(int id)
        {
            return w.wckReadPos(id);
        }

        static void Main(string[] args)
        {
            string port = "COM5";
            if (args.Length > 0) port = args[0];

            Program g = new Program();
            g.p = new PCremote(port);
            g.w = new wckMotion(g.p);

            Console.WriteLine("Demo - Port: {0} - {1} servos", port, g.countServos(22));

            if (g.testServo(30))
            {
                Console.WriteLine("DCMP mode assumed");
                g.w.DCMP = true;
            }
            else
            {
                Console.WriteLine("Standard firmware assumed {0}", g.p.readVer());
            }
            g.standup();
        }
    }
}
```


Screenshot :

<img src='http://robobuilderlib.googlecode.com/files/demo.png' width='80%'>

Note you need to add <a href='http://robobuildervc.googlecode.com/files/RobobuilderLib.dll'>http://robobuildervc.googlecode.com/files/RobobuilderLib.dll</a> in reference section<br>
<br>
<h1>Forms based</h1>

Here's an example using forms.<br>
<br>
From scratch - create a new windows form project. Within the window form add a button. And then on the button1_click event call connect("COM1"); Also add the connect function as part of the forms application - As with Console based apps you need to add to the "using section" for the IO ports and the RobobuilderLib to reference section.<br>
<br>
The code looks something like this (call with pn would be set to name of COM port):<br>
<pre><code>using System;<br>
using System.Windows.Forms;<br>
using System.IO;<br>
using System.IO.Port;<br>
using RobobuilderLib;<br>
<br>
namespace Demo<br>
{<br>
    public partial class Form1 : Form<br>
    {<br>
       public Form1()<br>
       {<br>
            InitializeComponent();<br>
       }<br>
<br>
       private void button1_Click(object sender, EventArgs e)<br>
       {<br>
             connect("COM1");<br>
       }<br>
<br>
       public int connect(string pn)<br>
       {<br>
            string serialNumber = "";<br>
<br>
            try<br>
            {<br>
                SerialPort p = new SerialPort(pn, 115200, Parity.None, 8, StopBits.One);<br>
<br>
                p.ReadTimeout = 1000;<br>
                p.WriteTimeout = 1000;<br>
<br>
                PCremote pcr = new PCremote(p);<br>
<br>
                p.Open();<br>
<br>
                serialNumber = pcr.readSN();<br>
<br>
                p.Close();<br>
<br>
                return 1;<br>
            }<br>
            catch (Exception e1)<br>
            {<br>
                return 0;<br>
            }<br>
        }<br>
    }<br>
}<br>
</code></pre>

More example functions to read the PSD should be:<br>
<br>
<code>string res = pcr.readDistance();</code>

or read accelerometer:<br>
<br>
<code> int[] result = pcr.readXYZ();</code>

Or to run a motion (in this case basic pose) put:<br>
<br>
<code>pcr.runMotion(7); </code>