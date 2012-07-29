using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace ImageMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap n=null;
        int minR, minG, minB;
        int maxR, maxG, maxB;

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Console.Out.WriteLine("E=" + e.ToString() + sender.ToString());
            MouseEventArgs m = (MouseEventArgs)e;
            Console.Out.WriteLine("X=" + m.X + " Y=" + m.Y);
            double fx = (double)pictureBox1.Image.Width * ((double)m.X / (double)pictureBox1.Size.Width);
            double fy = (double)pictureBox1.Image.Height * ((double)m.Y / (double)pictureBox1.Size.Height);
            Console.Out.WriteLine("X=" + (int)fx + " Y=" + (int)fy);
            int x = (int)fx;
            int y = (int)fy;
            if (n != null)
            {
                if (minB > n.GetPixel(x, y).B) minB = n.GetPixel(x, y).B;
                if (minG > n.GetPixel(x, y).G) minG = n.GetPixel(x, y).G;
                if (minR > n.GetPixel(x, y).R) minR = n.GetPixel(x, y).R;

                if (maxB < n.GetPixel(x, y).B) maxB = n.GetPixel(x, y).B;
                if (maxG < n.GetPixel(x, y).G) maxG = n.GetPixel(x, y).G;
                if (maxR < n.GetPixel(x, y).R) maxR = n.GetPixel(x, y).R;

                show();
            
            }
        }

        void clear()
        {
            maxR=0; maxG=0; maxB =0;
            minR=255; minG=255; minB=255;
            show();
        }

        void show()
        {
            label4.Text = minB.ToString();
            label5.Text = minG.ToString();
            label6.Text = minR.ToString();

            label7.Text = maxB.ToString();
            label8.Text = maxG.ToString();
            label9.Text = maxR.ToString();
        }

        void loadimage()
        {
            if (textBox1.Text.StartsWith("http://"))
            {
                Stream stream = File.OpenRead("temp.jpg");
                n = new Bitmap(stream); 
                stream.Close();
            }
            else
            {
                n = new Bitmap(textBox1.Text);
            }
            pictureBox1.Image = n;
            clear();
        }

        void filter()
        {
            for (int i = 0; i < n.Height; i++)
            {
                for (int j = 0; j < n.Width; j++)
                {
                    Color c = n.GetPixel(j, i);
                    if (c.R >= minR && c.R <= maxR && c.B >= minB && c.B <= maxB && c.G >= minG && c.G <= maxG)
                    {
                        n.SetPixel(j, i, Color.White);
                    }
                    else
                    {
                        n.SetPixel(j, i, Color.Black);
                    }
                }

                pictureBox1.Image = n;
                pictureBox1.Update();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                loadimage();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(textBox1.Text, "temp.jpg");
                loadimage();
            }
            catch
            {
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (n == null) return;
            if (checkBox1.Checked)
                filter();
            else
            {
                loadimage();
            }
        }
    }
}
