using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace RobobuilderLib
{
    public partial class Video_frm : Form
    {
        FilterInfoCollection videoDevices;
        int cnt;
        // image processing stuff
        ColorFiltering colorFilter = new ColorFiltering();
        ColorFiltering colorFilter2 = new ColorFiltering();
        GrayscaleBT709 grayscaleFilter = new GrayscaleBT709();
        BlobCounter blobCounter = new BlobCounter();

        public Video_frm()
        {
            InitializeComponent();

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
        }

        private void setup_filter()
        {
            // configure blob counter
            blobCounter.MinWidth = 25;
            blobCounter.MinHeight = 25;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            colorFilter.Blue = new IntRange(0, 100);
            colorFilter.Red = new IntRange(140, 255);
            colorFilter.Green = new IntRange(0, 100);

            colorFilter2.Blue = new IntRange(100, 255);
            colorFilter2.Red = new IntRange(0, 100);
            colorFilter2.Green = new IntRange(0, 100);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            // start the video capture.
            //label1.Visible = true;

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

            string text = string.Format("VideoControl 0.1");

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

                }

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
