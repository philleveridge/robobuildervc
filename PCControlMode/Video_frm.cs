using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace RobobuilderLib
{
    public partial class Video_frm : Form
    {
        //ffSpeechRecognizer rec = new SpeechRecognizer();
        FilterInfoCollection videoDevices;
        int cnt;
        // image processing stuff
        ColorFiltering colorFilter = new ColorFiltering();
        ColorFiltering colorFilter2 = new ColorFiltering();
        GrayscaleBT709 grayscaleFilter = new GrayscaleBT709();
        BlobCounter blobCounter = new BlobCounter();
        Preset_frm pf1;

        bool min_upd = false;
        bool pausev = false;

        float sx=0, sy=0, ex=0, ey=0;

        public Video_frm(Preset_frm p)
        {
            InitializeComponent();

            pf1 = p;

            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                // set up chooser
                listBox2.Items.Clear();
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    listBox2.Items.Add(videoDevices[i].Name);
                }
                listBox2.SelectedIndex = 0;

            }
            catch (Exception e2)
            {
                Console.WriteLine("Video camera initialisation error - " + e2);
                cmdStart.Enabled = false;
                cmdStop.Enabled = false;
                videoDevices = null;
            }

            setup_filter();

            var c = new Choices();
            c.Add("wave");
            c.Add("stand");
            c.Add("basic");

            //var gb = new GrammarBuilder(c);
            //var g = new Grammar(gb);
            //rec.LoadGrammar(g);
            //rec.Enabled = true;
            //rec.SpeechRecognized += rec_SpeechRecognized;

        }

        void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            label1.Text = e.Result.Text;
        }

        public void setup_filter()
        {
            setup_filter(new IntRange(140, 255), new IntRange(0, 100), new IntRange(0, 100));
        }

        public void setup_filter(IntRange red, IntRange green, IntRange blue)
        {
            // configure blob counter
            blobCounter.MinWidth = 25;
            blobCounter.MinHeight = 25;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            colorFilter.Blue = blue;
            colorFilter.Red = red;
            colorFilter.Green = green;

            panel1.BackColor = System.Drawing.Color.FromArgb(red.Min, green.Min, blue.Min);
            panel2.BackColor = System.Drawing.Color.FromArgb(red.Max, green.Max, blue.Max);

            label2.Text = colorFilter.Red.Min.ToString();
            label3.Text = colorFilter.Green.Min.ToString();
            label4.Text = colorFilter.Blue.Min.ToString();
            label5.Text = colorFilter.Red.Max.ToString();
            label6.Text = colorFilter.Green.Max.ToString();
            label7.Text = colorFilter.Blue.Max.ToString();
        }

        public bool IsfilterOn()
        {
            return detectionCheck.Checked;
        }

        public bool IsVideoOn()
        {
            return (videoSourcePlayer.VideoSource != null && videoSourcePlayer.IsRunning);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            // start the video capture.
            label1.Text = "";
            label1.Visible = true;

            // connect to camera
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[listBox2.SelectedIndex].MonikerString);
            videoSource.DesiredFrameSize = new Size(320, 240);
            videoSource.DesiredFrameRate = 15;

            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();
            cmdPause.Visible = true;
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            // stop camera
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();

            cmdPause.Visible = false;
        }

        private void cmdPause_Click(object sender, EventArgs e)
        {
            pausev = !pausev;
            videoSourcePlayer.Visible = !pausev;
            pictureBox1.Image = null;
        }

        // New video frame has arrived
        void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            cnt++;

            if (pausev && pictureBox1.Image!=null)
            {
                return;
            }

            Graphics g1 = Graphics.FromImage(image);


            string text;
            if (pf1.video_obj_loc == 0)
            {
                text = string.Format("VideoControl 0.1");
            }
            else
                text = string.Format("VideoControl 0.1 [{0}]", pf1.video_obj_loc);


            Font drawFont = new Font("Courier", 13, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.Blue);

            g1.DrawString(text, drawFont, drawBrush, new PointF(0, 5));

            drawFont.Dispose();
            drawBrush.Dispose();

            g1.Dispose();

            if (detectionCheck.Checked)
            {
                bool showOnlyObjects = false;

                Bitmap objectsImage = null;

                // color filtering
                if (showOnlyObjects)
                {
                    objectsImage = image;
                    colorFilter.ApplyInPlace(image);
                }
                else
                {
                    objectsImage = colorFilter.Apply(image);
                }

                // lock image for further processing
                BitmapData objectsData = objectsImage.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, image.PixelFormat);

                // grayscaling
                UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));

                // unlock image
                objectsImage.UnlockBits(objectsData);

                // locate blobs 
                blobCounter.ProcessImage(grayImage);
                Rectangle[] rects = blobCounter.GetObjectsRectangles();

                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];

                    // draw rectangle around derected object
                    Graphics g = Graphics.FromImage(image);

                    using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }

                    g.Dispose();

                    int cx = objectRect.X + objectRect.Width / 2;
                    int cy = objectRect.Y + objectRect.Height / 2;

                    cx = ((3 * cx) / image.Width) % 3;      // find centre (cx,cy)
                    cy = ((3 * cy) / image.Height) % 3;

                    // update location sq
                    // 1 4 7
                    // 2 5 8
                    // 3 6 9
                    pf1.video_obj_loc = cx * 3 + cy + 1;    
                }
                else
                    pf1.video_obj_loc = 0;

                // free temporary image
                if (!showOnlyObjects)
                {
                    objectsImage.Dispose();
                }
                grayImage.Dispose();
            }

            if (pausev && pictureBox1.Image == null)
            {
                pictureBox1.Image = (System.Drawing.Image)(image.Clone());
                //bmx = new Bitmap(image);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //exit button
            this.Hide();
        }


        private void color_bar_Scroll(object sender, ScrollEventArgs e)
        {
            int t, a , b;
            t = ((HScrollBar)(sender)).Value;

            switch (((HScrollBar)(sender)).Name)
            {
                case "red_bar":
                    a = colorFilter.Red.Min;
                    b = colorFilter.Red.Max;
                    if (min_upd && t < b) a = t;
                    if (!min_upd && t>a) b = t;
                    colorFilter.Red = new IntRange(a, b);
                    break;
                 case "green_bar":
                    a = colorFilter.Green.Min;
                    b = colorFilter.Green.Max;
                    if (min_upd && t < b) a = t;
                    if (!min_upd && t > a) b = t;
                    colorFilter.Green = new IntRange(a, b);
                    break;          
                case "blue_bar":
                    a = colorFilter.Blue.Min;
                    b = colorFilter.Blue.Max;
                    if (min_upd && t < b) a = t;
                    if (!min_upd && t > a) b = t;
                    colorFilter.Blue = new IntRange(a, b);
                    break;
            }
            panel1.BackColor = System.Drawing.Color.FromArgb(colorFilter.Red.Min, colorFilter.Green.Min, colorFilter.Blue.Min);
            panel2.BackColor = System.Drawing.Color.FromArgb(colorFilter.Red.Max, colorFilter.Green.Max, colorFilter.Blue.Max);

            label2.Text = colorFilter.Red.Min.ToString();
            label3.Text = colorFilter.Green.Min.ToString();
            label4.Text = colorFilter.Blue.Min.ToString();
            label5.Text = colorFilter.Red.Max.ToString();
            label6.Text = colorFilter.Green.Max.ToString();
            label7.Text = colorFilter.Blue.Max.ToString();
        }

        private void panel1_click(object sender, EventArgs e)
        {
            min_upd = true;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel2.BorderStyle = BorderStyle.None;
            red_bar.Value = colorFilter.Red.Min;
            green_bar.Value = colorFilter.Green.Min;
            blue_bar.Value = colorFilter.Blue.Min;
        }

        private void panel2_click(object sender, EventArgs e)
        {
            min_upd = false;
            panel1.BorderStyle = BorderStyle.None;
            panel2.BorderStyle = BorderStyle.FixedSingle;

            red_bar.Value = colorFilter.Red.Max;
            green_bar.Value = colorFilter.Green.Max;
            blue_bar.Value = colorFilter.Blue.Max;
        }

        private void md(int x, int y)
        {
            sx = x;
            sy = y;
        }

        private void mu(int x, int y)
        {
            ColorFiltering cf = new ColorFiltering();

            ex = x;
            ey = y;

            if (pictureBox1.Image == null) return;

            Console.WriteLine("x={0}, y={1}", pictureBox1.Image.Height, pictureBox1.Image.Width);

            Bitmap n = new Bitmap(pictureBox1.Image);

            Graphics g = pictureBox1.CreateGraphics();

              
            Color c = n.GetPixel(x, y);


            cf.Red.Min = c.R;
            cf.Green.Min = c.G;
            cf.Blue.Min = c.B;

            cf.Red.Max = c.R;
            cf.Green.Max = c.G;
            cf.Blue.Max = c.B;

            for (int gx = (int)sx; gx < (int)ex; gx++)
            {
                for (int gy = (int)sy; gy < (int)ey; gy++)
                {
                    c = n.GetPixel(gx, gy);

                    if (c.R < cf.Red.Min)   cf.Red.Min = c.R;
                    if (c.G < cf.Green.Min) cf.Green.Min = c.G;
                    if (c.B < cf.Blue.Min)  cf.Blue.Min = c.B;

                    if (c.R > cf.Red.Max)   cf.Red.Max = c.R;
                    if (c.G > cf.Green.Max) cf.Green.Max = c.G;
                    if (c.B > cf.Blue.Max)  cf.Blue.Max = c.B;
                }
            }

            setup_filter(cf.Red, cf.Green,cf.Blue);

            Console.WriteLine("CF = {0},{1},{2} to {3},{4},{5}",
                cf.Red.Min, cf.Green.Min, cf.Blue.Min,
                cf.Red.Max, cf.Green.Max, cf.Blue.Max);

            g.DrawRectangle(new Pen(Color.White), sx, sy, ex - sx, ey - sy);

        }


    }
}
