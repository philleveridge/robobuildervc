using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace RobobuilderLib
{
    public partial class ServoStatus : UserControl
    {
        const int MAX_SERVOS = 21;

        private Label[] servos = new Label[MAX_SERVOS];
        bool[] status = new bool[MAX_SERVOS];

        callBack Info_event = null;

        // 
        Point[] p = new Point[] { 
                    new System.Drawing.Point(72, 77), // 0
                    new System.Drawing.Point(72, 106),// 1
                    new System.Drawing.Point(72, 140),// 2
                    new System.Drawing.Point(72, 165),// 3
                    new System.Drawing.Point(97, 165),// 4
                    new System.Drawing.Point(42, 77), // 5
                    new System.Drawing.Point(42, 106),// 6
                    new System.Drawing.Point(42, 140),// 7
                    new System.Drawing.Point(42, 165),// 8
                    new System.Drawing.Point(23, 165),// 9
                    new System.Drawing.Point(72, 16), // 10
                    new System.Drawing.Point(97, 16), // 11
                    new System.Drawing.Point(97, 47), // 12
                    new System.Drawing.Point(42, 16), // 13
                    new System.Drawing.Point(17, 16), // 14
                    new System.Drawing.Point(17, 47), // 15
                    new System.Drawing.Point(72, 47), // 16
                    new System.Drawing.Point(42, 47), // 17
                    new System.Drawing.Point(97, 77), // 18 
                    new System.Drawing.Point(17, 77), // 19
                    new System.Drawing.Point(60, 0) //  20
        };

        public ServoStatus()
        {
            InitializeComponent();

            for (int i = 0; i < MAX_SERVOS; i++)
            {
                servos[i] = new System.Windows.Forms.Label();

                servos[i].AutoSize = true; //.Size = new System.Drawing.Size(13, 13);
                servos[i].Text = i.ToString();
                servos[i].Location = p[i];
                servos[i].BackColor = Color.Red;
                servos[i].ForeColor = Color.White;

                servos[i].Click += new System.EventHandler(this.Click1);
                servos[i].MouseHover += new System.EventHandler(this.Click2);

                status[i] = true;

                this.panel1.Controls.Add(servos[i]);
            }
        }

        public bool getStatus(int i)
        {
            if (i >= 0 && i < MAX_SERVOS)
                return status[i];
            else
                return false;
        }

        public void setStatus(int i, bool f)
        {
            if (i >= 0 && i < MAX_SERVOS)
                status[i] = f;

            if (f)
            {
                servos[i].BackColor = Color.White;
                servos[i].ForeColor = Color.Black;
            }
            else
            {
                servos[i].BackColor = Color.Red;
                servos[i].ForeColor = Color.White;
            }
        }

        public void setCallback_event(callBack z)
        {
            Info_event = z;
        }

        private void Click1(object sender, EventArgs e)
        {
            Console.WriteLine(((Label)sender).Text);
            int id = Convert.ToInt32(((Label)sender).Text);
            setStatus(id, !getStatus(id));
        }

        private void Click2(object sender, EventArgs e)
        {
            Console.WriteLine(((Label)sender).Text);
            int id = Convert.ToInt32(((Label)sender).Text);;
            if (Info_event != null) Info_event(id);
        }
    }
}
