using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RobobuilderLib
{
    public partial class balance_frm : Form
    {
        Random n = new Random();
        List<int> h1 = new List<int>();
        List<int> h2 = new List<int>();

        public PCremote  pcr = null;
        public wckMotion wck = null;

        Autonomy a = new Autonomy();

        int L = 20;

        int[] o_xyz = null;
        int minx=255, maxx=-255, miny=255, maxy=-255, minz=255, maxz=-255;

        List<int[]> database = new List<int[]>();

        public balance_frm()
        {
            InitializeComponent();

            a.test();

            label1.Text = "Press button to start";
        }

        private void calibrateXYZ(int[] a)
        {
            if (a[0] < minx) minx = a[0];
            if (a[1] < miny) miny = a[1];
            if (a[2] < minz) minz = a[2];

            if (a[0] > maxx) maxx = a[0];
            if (a[1] > maxy) maxy = a[1];
            if (a[2] > maxz) maxz = a[2];

            o_xyz = new int[] { minx + ((maxx - minx) / 2), (miny + (maxy - miny) / 2), (minz + (maxz - minz) / 2 )};
        }

        public void rectangle(Graphics g, int x1, int y1, int x2, int y2)
        {
            Pen p2 = new Pen(Color.Green);
            int w = panel1.Width;
            int h = panel1.Height;

            x1 += (w / 2);
            y1 = (h / 2) - y1;

            x2 += (w / 2);
            y2 = (h / 2) - y2;

            g.DrawLine(p2, x1, y1, x2, y1);
            g.DrawLine(p2, x2, y1, x2, y2);
            g.DrawLine(p2, x2, y2, x1, y2);
            g.DrawLine(p2, x1, y2, x1, y1);
        }

        public void plot(Graphics g, int x, int y, string txt, List<int>history)
        {
            g.Clear(Color.Wheat);
            int w = panel1.Width;
            int h = panel1.Height;

            x += (w / 2);
            y = (h / 2) - y;

            history.Add(x); history.Add(y);

            Pen axis = new Pen(Color.Black);
            Pen p1 = new Pen(Color.Red);
            Font f = new Font("Arial", (float)8.25, FontStyle.Regular);

            g.DrawLine(axis, 0, h / 2, w, h / 2);
            g.DrawLine(axis, w / 2, 0, w / 2, h);
            g.DrawEllipse(p1, (x - 6), (y - 6), 14, 14);
            g.DrawString(txt, f, (p1.Brush), (new PointF(10, 10)));

            int c = history.Count;
            if (c >= L)
            {
                int[] v = new int[0];
                for (int i = 0; i < L; i+= 2)
                {
                    int[] t1 = new int[] { history[c - 2 - i], history[c - 1 - i]};
                    v = vectors.append(v, t1);
                }
                if (tailp.Checked)
                {
                    drawlines(g, v);
                }

                //Console.WriteLine("diff {0}:{1}, norm={2}", v[0] - v[L - 2], v[1] - v[L - 1], 
                //     vectors.normal(new int[] {v[0] - v[L - 2], v[1] - v[L - 1]}));
            }
        }

        private void drawlines(Graphics g, int[] points)
        {
            Pen p2 = new Pen(Color.Blue);

            int x = points[0];
            int y = points[1];

            for (int i = 2; i < points.Length; i += 2)
            {
                int xn = points[i];
                int yn = points[i + 1];

                g.DrawLine(p2, x, y, xn, yn);

                x = xn; y = yn;
            }
        }

        private int[] readServos()
        {
            int[] r = new int[16];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = n.Next(0, 254);
            }
            return r;
        }

        private int[] readAcc()
        {
            int[] r = new int[3];
            r[0] = (n.Next(50) - 25);
            r[1] = (n.Next(50) - 25);
            r[2] = (n.Next(50) - 25);

            return r;
        }

        private int[] genRand(int l, int sz)
        {
            int[] r = new int[l];
            for (int i = 0; i < l; i++)
                if (n.Next(5) == 0) r[i] = n.Next(sz) - sz / 2; else r[i] = 0;
            return r;
        }

        private double fitness(int[] a, int[] b)
        {
            return 0.0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 250; 
            if (button1.Text == "Stop")
            {
                show(); //show database

                timer1.Enabled = false;
                button1.Text = "Start";
                if (wck != null) 
                    wck.close();
                wck = null;
            }
            else
            {
                o_xyz = null;
                h1.Clear();
                h2.Clear();
                timer1.Enabled = true;
                button1.Text = "Stop";
                if (pcr != null) wck = new wckMotion(pcr);
            }
        }

        private void show()
        {
            for (int i=0; i<database.Count; i+=2)
            {
                Console.WriteLine("[{0}] ({1}) ({2})",i,vectors.str(database[i]), vectors.str(database[i + 1]));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int[] xyz;
            int[] cp;

            DateTime n = DateTime.Now;


            if (pcr == null || wck == null)
            {
                // read dummy data
                xyz = readAcc(); 
                cp = readServos();
            }
            else
            {
                // read real data
                pcr.setDCmode(false);
                xyz = pcr.readXYZ();
                pcr.setDCmode(true);
                System.Threading.Thread.Sleep(100);  

                wck.servoID_readservo(18);
                cp = vectors.convInt(wck.pos);
            }


            if (h1.Count<L) calibrateXYZ(xyz);

            int[] d = vectors.sub(xyz, o_xyz);

            Console.WriteLine("xyz = {0}, o_xyz={1}, diff={2}",
                vectors.str(xyz), vectors.str(o_xyz), vectors.str(d));

            //display output

            label1.Text = vectors.str(cp);

            if (xyp.Checked)
            {
                Graphics g = panel1.CreateGraphics();
                plot(g, xyz[0], xyz[1], String.Format("XY ({0},{1})", xyz[0], xyz[1]), h1);
                rectangle(g, minx, miny, maxx, maxy);
            }

            if (xzp.Checked)
            {
                Graphics g2 = panel2.CreateGraphics();
                plot(g2, xyz[0], xyz[2], String.Format("XZ ({0},{1})", xyz[0], xyz[2]), h2);
                rectangle(g2, minx, minz, maxx, maxz);
            }

            // learning mode

            int[] ra = genRand(18, 4);
            Console.WriteLine("R= {0}", vectors.str(ra));

            database.Add(d);
            database.Add(ra);

            //search for match and get difference

            int[] m = vectors.match(database, d);
            
            label1.Text += " M=" + vectors.str(m);

            // add difference to cp convert to servo array

            byte[] b = vectors.convByte(vectors.add(cp, m));

            // send to servo

            // check and store fitness

            double ft = fitness(xyz, xyz); // what do we emasure fitness against?

            // where do we store ??

            // debug

            Console.WriteLine("xyz= {0}, L= {1}", vectors.str(xyz), label1.Text);
            Console.WriteLine("t={1} ms, {0} FPS", 1000.0 / (DateTime.Now - n).Milliseconds, (DateTime.Now - n).Milliseconds);

        }

    }
}
