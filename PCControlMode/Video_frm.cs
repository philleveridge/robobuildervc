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

        public void setup_filter(IntRange red, IntRange blue, IntRange green)
        {
            // configure blob counter
            blobCounter.MinWidth = 25;
            blobCounter.MinHeight = 25;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            colorFilter.Blue = blue;
            colorFilter.Red = red;
            colorFilter.Green = green;
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
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            // stop camera
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();
        }

        // New video frame has arrived
        void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            cnt++;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
