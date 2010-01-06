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
        List<int>         h1 = new List<int>();
        List<int>         h2 = new List<int>();

        public PCremote  pcr = null;
        public wckMotion wck = null;

        Autonomy a ;

        public balance_frm()
        {
            InitializeComponent();

            a = new Autonomy();
            a.test();

            label1.Text = "Press button to start";
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

            drawlines(g, new int[] {x1,y1, x2,y1, x2,y2, x1,y2, x1,y1}, p2);
        }

        public void plot(Graphics g, int x, int y, string txt, List<int>history)
        {
            int L = 2 * Autonomy.L;

            g.Clear(Color.Wheat);
            int w = panel1.Width;
            int h = panel1.Height;

            x += (w / 2);
            y = (h / 2) - y;

            history.Add(x); history.Add(y);

            Pen axis = new Pen(Color.Black);
            Pen p1 = new Pen(Color.Red);
            Pen p2 = new Pen(Color.Blue);
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
                    drawlines(g, v, p2);
                }
            }
        }

        private void drawlines(Graphics g, int[] points, Pen p2)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 250; 
            if (button1.Text == "Stop")
            {
                a.show_db(); //show database

                timer1.Enabled = false;
                button1.Text = "Start";
                if (wck != null) 
                    wck.close();
                wck = null;
            }
            else
            {
                a.init();

                h1.Clear();
                h2.Clear();
                timer1.Enabled = true;
                button1.Text = "Stop";
                if (pcr != null) wck = new wckMotion(pcr);
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
                xyz = a.readAcc();
                cp = a.readServos();
            }
            else
            {
                // read real data
                pcr.setDCmode(false);
                xyz = pcr.readXYZ();
                pcr.setDCmode(true);
                System.Threading.Thread.Sleep(100);

                wck.servoID_readservo(Autonomy.MAXSERVOS);
                cp = vectors.convInt(wck.pos);
            }

            if (xyp.Checked)
            {
                Graphics g = panel1.CreateGraphics();
                plot(g, xyz[0], xyz[1], String.Format("XY ({0},{1})", xyz[0], xyz[1]), h1);
                rectangle(g, a.minx, a.miny, a.maxx, a.maxy);
            }

            if (xzp.Checked)
            {
                Graphics g2 = panel2.CreateGraphics();
                plot(g2, xyz[0], xyz[2], String.Format("XZ ({0},{1})", xyz[0], xyz[2]), h2);
                rectangle(g2, a.minx, a.minz, a.maxx, a.maxz);
            }

            // autonomy

            string s1=vectors.str(cp), s2="";

            a.update(xyz,cp, ref s1, ref s2);

            label1.Text = s1;
            label2.Text = s2;

            if (pcr != null && wck != null && a.servos != null)
            {
                Console.WriteLine("s={0} ", vectors.str(vectors.convInt(a.servos)));
                //wck.SyncPosSend(15, 2, a.servos, 0);
            }

            // debug

            Console.WriteLine("t={1} ms, {0} FPS", 1000.0 / (DateTime.Now - n).Milliseconds, (DateTime.Now - n).Milliseconds);

        }

    }
}
