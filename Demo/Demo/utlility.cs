﻿using System.Drawing;
using System.Windows.Forms;
using System;
using System.Text;

namespace Demo
{
    class CList
    {
        const int MAXLIST = 1000;
        int[] history = new int[MAXLIST];
        int hn;

        public CList()
        {
            hn = 0;
        }

        public int count()
        {
            return hn;
        }

        public void store(int x, int y)
        {
            if (hn > MAXLIST-2) hn = 0;
            history[hn++] = x;
            history[hn++] = y;
        }

        public void reset_history()
        {
            hn = 0;
        }

        public int[] getAll()
        {
            return history;
        }

        public int[] getlast(int n)
        {
            int[] p = new int[n];
            for (int i = 0; i < n; i++)
            {
                if (hn - n + i > 0)
                    p[i] = history[hn - n + i];
            }
            return p;
        }
    }

    class Utility
    {
        public Form win = null;
        public Pen p1, p2;
        Graphics g;
        Panel pb;
        CList coords = new CList();

        public bool kp = false;
        public int ch = 0;

        public int getkey()
        {
            int r = 0;
            if (kp) r = ch;
            kp = false;
            return r;
        }

        public Form createwindow(string title, int h, int w)
        {
            win = new Form();

            win.Text = title;  
            win.AutoSize =  true;
            win.TopMost = false;

            pb = new System.Windows.Forms.Panel();
            pb.Size= new Size (h,w);
            pb.Location = new Point(24, 16);

            p1 = new Pen(Color.FromName("Black"));
            p1.DashStyle = (System.Drawing.Drawing2D.DashStyle.DashDot);
            p2 = new Pen(Color.FromName("Red"));

            win.Controls.Add( pb);
            g=pb.CreateGraphics();
            return win;
        }

        public void usePanel(Panel x)
        {
            pb = x;

            p1 = new Pen(Color.FromName("Black"));
            p1.DashStyle = (System.Drawing.Drawing2D.DashStyle.DashDot);
            p2 = new Pen(Color.FromName("Red"));

            g = pb.CreateGraphics();
        }

        public void cwin()
        {
            if (win != null) win.Close();
        }

        public void plot(string txt, int x, int y, bool cf, Pen pen)
        {
            int w  = pb.Width;
            int h  = pb.Height;

            x += (w/2);
            y = (h/2)-y;

            if (win != null) win.Show();
  
            if (cf)
            {
                g.Clear(Color.FromName("White"));
                Pen axis = new Pen(Color.FromName("Black"));
                g.DrawLine(axis, 0, h / 2, w, h / 2);
                g.DrawLine(axis, w / 2, 0, w / 2, h);
            }

            g.DrawEllipse (pen, (x-6),  (y-6), 14, 14);

            if (txt != "")
            {
                Font font = new Font("Arial", (Single)8.25);
                g.DrawString(txt, font, (pen.Brush), new PointF(10, 10));
            }
        }

        public void drawline(Graphics g, int fx, int fy, int tx, int ty, Pen c)
        {       
            int w  = pb.Width;
            int h  = pb.Height;

            fx += (w/2);
            fy = (h/2) - fy;
            tx += (w / 2);
            ty = (h/2) - ty;
   
            g.DrawLine(c, fx, fy, tx, ty);
        }

        public void drawlist(int[] xy, int n, Pen c)
        {
            int i = 0;
            while (i + 4 <= xy.Length && i + 4 <= n)
            {
                drawline(g, xy[i], xy[i + 1], xy[i + 2], xy[i + 3], c);
                i += 2;
            }
        }

        public void pwin(int y, int y2, int n, double t)
        {
            int nx = ((n * 10) % 280) - 140;
            int ny = 4 * y;
            int ny2 = 4 * y2;

            plot("(Acc=" + ny + " Rate=" + String.Format("{0:#.#}", t) + " ms)", nx, ny, true, new Pen (Color.FromName ("Red")));
            plot("", nx, ny2, false, new Pen (Color.FromName ("Blue")));

            drawlist(new int[] { -125, 40, 125, 40 }, 4, (Pen)((ny > 40) ? p2 : p1)); //limit
            drawlist(new int[] { -125, -40, 125, -40 }, 4, (Pen)((ny < -40) ? p2 : p1)); //limit    
            coords.store(nx, ny);
            drawlist(coords.getlast(6), 6, new Pen(Color.FromName("Blue")));
        }
    }
}

