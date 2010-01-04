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
        List<int> history = new List<int>();
        public PCremote pcr = null;

        public balance_frm()
        {
            InitializeComponent();
        }

        public void plot(int x, int y, string txt)
        {
            Graphics g = panel1.CreateGraphics();
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
            if (c > 5)
            {
                drawlines(g, new int[] { history[c - 2], history[c-1], history[c - 4], history[c - 3], history[c - 6], history[c - 5] });
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

        private int[] readAcc()
        {
            int[] r = new int[3];
            r[0] = (n.Next(50) - 25);
            r[1] = (n.Next(50) - 25);
            r[2] = (n.Next(50) - 25);

            return r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 100; 
            if (button1.Text == "Stop")
            {
                timer1.Enabled = false;
                button1.Text = "Start";
            }
            else
            {
                timer1.Enabled = true;
                button1.Text = "Stop";
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int[] xyz;
            if (pcr == null)
                xyz = readAcc(); // read dummy data
            else
                xyz = pcr.readXYZ();

            plot(xyz[0], xyz[1], String.Format("XY ({0},{1})", xyz[0], xyz[1]));

        }
    }
}
