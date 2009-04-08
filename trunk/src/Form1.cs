using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.IO;
using System.IO.Ports;

using RobobuilderVC;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;


namespace RobobuilderVC
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdStop;
        private System.IO.Ports.SerialPort serialPort1;
        private TextBox textBox1;
        private TextBox textBox2;
        private ListBox listBox1;
        private Button button5;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private IContainer components;

        int cnt;
        private ListBox listBox2;
        private Label robolibver;
        private PictureBox pictureBox1;
        private CheckBox detectionCheck;
       
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;


        // image processing stuff
        ColorFiltering colorFilter = new ColorFiltering();
        ColorFiltering colorFilter2= new ColorFiltering();
        GrayscaleBT709 grayscaleFilter = new GrayscaleBT709();
        BlobCounter blobCounter = new BlobCounter();
        private System.Windows.Forms.Timer timer2;
        private Button testPM;

        bool serialread;
        private ProgressBar batLevel;
        private ProgressBar micLevel;
        private ProgressBar PSDLevel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem loadRBMToolStripMenuItem;
        private OpenFileDialog openFileDialog1;

        PlayMotion pm;
        private ToolStripMenuItem loadBasToolStripMenuItem;
        private Label label2;
        private CheckBox pollTst;
        private Button modeB;
        private Button progmode;
        Motion m1;

        string[] prog = new string[250];
        private Button button1; int pgsze;

        bool binmode;

        private binxfer btf;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//

			InitializeComponent();
      
            //
			// TODO: Add any constructor code after InitializeComponent call
			//
            cnt = 0;
            serialread = true;

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

            timer2.Enabled = false;
            PSDLevel.Value = 0;
            micLevel.Value = 0;
            batLevel.Value = 8000;

        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button5 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.robolibver = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.detectionCheck = new System.Windows.Forms.CheckBox();
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.testPM = new System.Windows.Forms.Button();
            this.batLevel = new System.Windows.Forms.ProgressBar();
            this.micLevel = new System.Windows.Forms.ProgressBar();
            this.PSDLevel = new System.Windows.Forms.ProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRBMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.pollTst = new System.Windows.Forms.CheckBox();
            this.modeB = new System.Windows.Forms.Button();
            this.progmode = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(6, 296);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(78, 24);
            this.cmdStart.TabIndex = 1;
            this.cmdStart.Text = "Start";
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Location = new System.Drawing.Point(6, 326);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(78, 24);
            this.cmdStop.TabIndex = 2;
            this.cmdStop.Text = "Stop";
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.PortName = "COM4";
            this.serialPort1.WriteBufferSize = 1024;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(89, 338);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(265, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(90, 364);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(264, 88);
            this.textBox2.TabIndex = 11;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Items.AddRange(new object[] {
            "COM3",
            "COM4",
            "COM5"});
            this.listBox1.Location = new System.Drawing.Point(216, 298);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(138, 34);
            this.listBox1.TabIndex = 14;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Red;
            this.button5.Location = new System.Drawing.Point(151, 307);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(50, 25);
            this.button5.TabIndex = 15;
            this.button5.Text = "Open";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 467);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "label1";
            // 
            // listBox2
            // 
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 16;
            this.listBox2.Items.AddRange(new object[] {
            "<undef>"});
            this.listBox2.Location = new System.Drawing.Point(6, 272);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(139, 20);
            this.listBox2.TabIndex = 18;
            // 
            // robolibver
            // 
            this.robolibver.AutoSize = true;
            this.robolibver.Location = new System.Drawing.Point(12, 347);
            this.robolibver.Name = "robolibver";
            this.robolibver.Size = new System.Drawing.Size(0, 13);
            this.robolibver.TabIndex = 19;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(360, 32);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(235, 462);
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // detectionCheck
            // 
            this.detectionCheck.AutoSize = true;
            this.detectionCheck.Location = new System.Drawing.Point(151, 271);
            this.detectionCheck.Name = "detectionCheck";
            this.detectionCheck.Size = new System.Drawing.Size(75, 17);
            this.detectionCheck.TabIndex = 22;
            this.detectionCheck.Text = "visual filter";
            this.detectionCheck.UseVisualStyleBackColor = true;
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point(6, 27);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(322, 242);
            this.videoSourcePlayer.TabIndex = 0;
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer_NewFrame);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // testPM
            // 
            this.testPM.Location = new System.Drawing.Point(6, 364);
            this.testPM.Name = "testPM";
            this.testPM.Size = new System.Drawing.Size(50, 24);
            this.testPM.TabIndex = 23;
            this.testPM.Text = "wave";
            this.testPM.Click += new System.EventHandler(this.testPM_Click);
            // 
            // batLevel
            // 
            this.batLevel.ForeColor = System.Drawing.Color.Lime;
            this.batLevel.Location = new System.Drawing.Point(490, 7);
            this.batLevel.Maximum = 10000;
            this.batLevel.Minimum = 5000;
            this.batLevel.Name = "batLevel";
            this.batLevel.Size = new System.Drawing.Size(59, 18);
            this.batLevel.TabIndex = 24;
            this.batLevel.Value = 8000;
            this.batLevel.Visible = false;
            // 
            // micLevel
            // 
            this.micLevel.ForeColor = System.Drawing.Color.Lime;
            this.micLevel.Location = new System.Drawing.Point(360, 7);
            this.micLevel.Maximum = 255;
            this.micLevel.Name = "micLevel";
            this.micLevel.Size = new System.Drawing.Size(59, 18);
            this.micLevel.TabIndex = 25;
            this.micLevel.Value = 255;
            this.micLevel.Visible = false;
            // 
            // PSDLevel
            // 
            this.PSDLevel.ForeColor = System.Drawing.Color.Lime;
            this.PSDLevel.Location = new System.Drawing.Point(425, 7);
            this.PSDLevel.Maximum = 255;
            this.PSDLevel.Name = "PSDLevel";
            this.PSDLevel.Size = new System.Drawing.Size(59, 18);
            this.PSDLevel.TabIndex = 26;
            this.PSDLevel.Value = 255;
            this.PSDLevel.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(602, 24);
            this.menuStrip1.TabIndex = 28;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRBMToolStripMenuItem,
            this.loadBasToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // loadRBMToolStripMenuItem
            // 
            this.loadRBMToolStripMenuItem.Name = "loadRBMToolStripMenuItem";
            this.loadRBMToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.loadRBMToolStripMenuItem.Text = "Load RBM";
            this.loadRBMToolStripMenuItem.Click += new System.EventHandler(this.loadRBMToolStripMenuItem_Click);
            // 
            // loadBasToolStripMenuItem
            // 
            this.loadBasToolStripMenuItem.Name = "loadBasToolStripMenuItem";
            this.loadBasToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.loadBasToolStripMenuItem.Text = "Load Bas";
            this.loadBasToolStripMenuItem.Click += new System.EventHandler(this.loadBasToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(222, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "MODE: no connection";
            // 
            // pollTst
            // 
            this.pollTst.AutoSize = true;
            this.pollTst.Location = new System.Drawing.Point(560, 9);
            this.pollTst.Name = "pollTst";
            this.pollTst.Size = new System.Drawing.Size(42, 17);
            this.pollTst.TabIndex = 30;
            this.pollTst.Text = "poll";
            this.pollTst.UseVisualStyleBackColor = true;
            // 
            // modeB
            // 
            this.modeB.Location = new System.Drawing.Point(6, 394);
            this.modeB.Name = "modeB";
            this.modeB.Size = new System.Drawing.Size(50, 24);
            this.modeB.TabIndex = 27;
            this.modeB.Text = "mode";
            this.modeB.Click += new System.EventHandler(this.modeB_Click);
            // 
            // progmode
            // 
            this.progmode.Location = new System.Drawing.Point(6, 428);
            this.progmode.Name = "progmode";
            this.progmode.Size = new System.Drawing.Size(50, 24);
            this.progmode.TabIndex = 32;
            this.progmode.Text = "load";
            this.progmode.Visible = false;
            this.progmode.Click += new System.EventHandler(this.progmode_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(304, 271);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 24);
            this.button1.TabIndex = 33;
            this.button1.Text = "Bin Mode";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(602, 497);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progmode);
            this.Controls.Add(this.pollTst);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.modeB);
            this.Controls.Add(this.PSDLevel);
            this.Controls.Add(this.micLevel);
            this.Controls.Add(this.batLevel);
            this.Controls.Add(this.testPM);
            this.Controls.Add(this.detectionCheck);
            this.Controls.Add(this.robolibver);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.videoSourcePlayer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Robobuilder Video Control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

        FilterInfoCollection videoDevices ;
        int mode;

        void showmode()
        {
            switch (mode)
            {
                case 0:
                    label2.Text="MODE: not connected";
                    break;
                case 1:
                    label2.Text="MODE: Idle";
                    break; 
                case 2:
                    label2.Text="MODE: Experimental";
                    break;
                case 3:
                    label2.Text="MODE: Serial";
                    break;
                case 4:
                    label2.Text = "MODE: Program Load";
                    break;
                case 5:
                    label2.Text = "MODE: Program run"; 
                    break;
                case 6:
                    label2.Text = "MODE: ???";
                    break;
            }
        }

        void readmode()
        {
            if (serialPort1.IsOpen)
            {
                string v = write2serial("?", true);
                if (v.StartsWith("Idle")) mode = 1;
                else if (v.StartsWith("?Exper")) mode = 2;
                else if (v.StartsWith("Seria")) mode = 3;
                else mode = 6;
            }
            else
            {
                mode = 0;
            }
            showmode();
        }


        private void Form1_Load(object sender, System.EventArgs e)
		{
            mode = 0;
            showmode();

            label1.Visible = false;
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
                listBox2.SelectedIndex = 2;
                             
            }
            catch (Exception e2)
            {
                Console.WriteLine("Video camera initialisation error - " + e2);
                cmdStart.Enabled = false;
                cmdStop.Enabled=false;
                videoDevices = null;
            }
            try
            {
                listBox1.Items.Clear();
                foreach (string s in SerialPort.GetPortNames())
                {
                    System.Diagnostics.Trace.WriteLine(s);
                    listBox1.Items.Add(s);
                }

                //serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
                serialPort1.Open();
                serial_status();
            }
            catch (Exception e1)
            {
                Console.WriteLine("Comm port error - " + e1);
            }
		}

        // New video frame has arrived
        void videoSourcePlayer_NewFrame( object sender, ref Bitmap image )
        {
            cnt++;
            Graphics g1 = Graphics.FromImage(image);

            string text = string.Format("RobobuilderVC 0.1");

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

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            // stop camera
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();

            if (serialPort1.IsOpen) serialPort1.Close();
		}

		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			// start the video capture.
            label1.Visible = true;

            // connect to camera
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[listBox2.SelectedIndex].MonikerString);
            videoSource.DesiredFrameSize = new Size(320, 240);
            videoSource.DesiredFrameRate = 15;

            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();

		}

		private void cmdStop_Click(object sender, System.EventArgs e)
		{
            // stop camera
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();
		}

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                write2serial(textBox1.Text, true);
                textBox1.Text = "";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            { 
                serialPort1.Close();
            }
            else 
            {
                try
                {
                    serialPort1.WriteBufferSize = 1;

                    serialPort1.Open();
                    readmode();
                    if (mode == 2)
                    {
                        string v = write2serial("v", true);
                        this.Text += v;
                    }
                }
                catch (Exception es)
                {
                    Console.WriteLine("serial port open failed");
                }
            }
            serial_status();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
            serialPort1.PortName = listBox1.SelectedItem.ToString();
            serial_status();
        }

        private void serial_status()
        {
            if (serialPort1.IsOpen)
            {
                button5.Text = "Close";
                button5.BackColor = System.Drawing.Color.Green;
            }
            else
            {
                button5.Text = "Open";
                button5.BackColor = System.Drawing.Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "FPS: " + cnt;
            cnt = 0;
            if (serialPort1.IsOpen && serialread)
            {
                textBox2.AppendText(serialPort1.ReadExisting());

                if ((mode == 2 || mode == 3) && pollTst.Checked)
                {
                    batLevel.Visible = true; micLevel.Visible = true; PSDLevel.Visible = true;
                    string qs = write2serial("q", true);

                    if (qs.Length >= 74 && qs.Substring(0, 1) == "q")
                    {
                        PSDLevel.Value = Convert.ToInt16(qs.Substring(64 + 1, 4), 16);
                        micLevel.Value = Convert.ToInt16(qs.Substring(68 + 1, 4), 16);
                        batLevel.Value = Convert.ToInt16(qs.Substring(72 + 1, 4), 16);

                        Console.WriteLine("Debug: " + batLevel.Value + micLevel.Value + PSDLevel.Value);

                        Console.WriteLine("X=" + qs.Substring(76 + 1, 4)
                            + ", Y=" + qs.Substring(80 + 1, 4)
                            + ", Z=" + qs.Substring(84 + 1, 4));

                        Console.WriteLine("Elapsed time =" + qs.Substring(84 + 1));
                    }
                }
                else
                {
                    batLevel.Visible = false; micLevel.Visible = false; PSDLevel.Visible = false;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //back
            write2serial("e11", true);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Controller Image
            // mouse x/y
            // 
            MouseEventArgs m = (MouseEventArgs)e;

            if (m.X > 45 && m.X < 70 && m.Y > 45 && m.Y < 70)
            {
                System.Console.WriteLine("A pressed");
            }
            else
            if (m.X > 160 && m.X < 180 && m.Y > 45 && m.Y < 70)
            {
                System.Console.WriteLine("B pressed");
            }
            else
            if (m.X > 99 && m.X < 127 && m.Y > 74 && m.Y < 107)
            {
                write2serial("F", true);

            } 
            else
            if (m.X > 105 && m.X < 125 && m.Y > 195 && m.Y < 225)
            {
                write2serial("B", true);

            } 
            else
            if (m.X > 163 && m.X < 190 && m.Y > 141 && m.Y < 158)
            {
                write2serial("R", true);

            }
            else
            if (m.X > 36 && m.X < 66 && m.Y > 140 && m.Y < 160)
            {
                write2serial("L", true);
            }
            else
            {
                System.Console.WriteLine("clicked" + m.X.ToString() + "," + m.Y.ToString());
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Play frames
            string t=pm.Timer();
            if (t == "")
            {
                timer2.Enabled = false;
            }
            else
            {
                sendCmd(t);
                timer2.Interval = pm.CalcFrameInterval();
            }
        }

        private void sendCmd(string n)
        {
            string[] cmds = n.Split(':');

            for (int j = 0; j < cmds.Length; j++)
            {
                string s;
                int l = cmds[j].Length / 2;
                if (l > 0)
                {
                    int cn = Convert.ToInt32(cmds[j].Substring(2,2),16);
                    if ((cn >> 5) < 5 && (cn & 0x1F) == 31)
                    {
                        //SynchPosSend (no response)
                        s = "X";
                    }
                    else
                    {
                        //everything else (wait for response)
                        s = "x";
                    }
                    s +=  l.ToString("X2") + cmds[j];
                    write2serial(s, true);
                }
            }
        }



        private string expect_serial(string s, string e)
        {
            string r = "";
            bool f = timer1.Enabled;
            timer1.Enabled = false;

            if (serialPort1.IsOpen)
            {
                r = serialPort1.ReadExisting();
                serialPort1.ReadTimeout = 2000;
                serialPort1.Write(s);
               
                try
                {
                    r=serialPort1.ReadTo(e);
                }
                catch (Exception z)
                {
                }
            }
            timer1.Enabled = f;
            Console.WriteLine("E=[" + r + e +"]"); //debug
            return r;
        }

        private string write2serial(string s, bool synch)
        {
            string r="";
            bool f=timer1.Enabled;
            if (synch)
                timer1.Enabled = false;

            Console.WriteLine(s + "(" + serialPort1.IsOpen +")");
            if (serialPort1.IsOpen)
            {
                string t = serialPort1.ReadExisting();
                textBox2.AppendText(t);

                serialPort1.Write(s);
                if (synch)
                {
                    //wait for a response
                    //generally this is the normal mode
                    serialPort1.ReadTimeout=5000;
                    try
                    {
                        r = serialPort1.ReadLine();
                        if (r=="\r")
                            r = serialPort1.ReadLine();
                    }
                    catch (Exception z)
                    {
                    }
                    Console.WriteLine("W2=" + t + "[" + r + "]"); //debug
                }
            }
            timer1.Enabled=f;
            return r;
        }

        private void testPM_Click(object sender, EventArgs e)
        {
            /*
            timer2.Enabled = !timer2.Enabled;

            if (timer2.Enabled)
            {
                Wave w = new Wave();
                pm = new PlayMotion(w);

                timer2.Interval = pm.CalcFrameInterval();
                sendCmd(pm.Play());
            }
            else
            {
                Console.WriteLine("stopped");
            }
             */

            bool t1 = timer1.Enabled;
            bool t2 = timer2.Enabled;
            timer1.Enabled = false;
            timer2.Enabled = false;

            readmode();

            //if (mode != 3)
            //    return;

            SerialSlave s = new SerialSlave(serialPort1);

            s.Move(10, 500, new byte[] { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 51,  47, 49, 199, 205, 205});
            s.Play();

            s.Move(40,1000, new byte[] { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 187, 58, 46, 199, 205, 205});
            s.Play(); 
            
            s.Move(10, 500, new byte[] { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 186, 103,46, 199, 205, 205});
            s.Play(); 
            
            s.Move(10, 500, new byte[] { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 187, 58, 46, 199, 205, 205});
            s.Play(); 
            
            s.Move(40,1000, new byte[] { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 51,  47, 49, 199, 205, 205});
            s.Play();

            timer1.Enabled = t1;
            timer2.Enabled = t2;
        }

        private void modeB_Click(object sender, EventArgs e)
        {
            readmode();
        }

        private void loadRBMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";

            // load rbm file
            openFileDialog1.Filter = "Motion File|*.rbm";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
                return;

            m1 = new Motion();
            m1.LoadFile(filename);

            /*
            if (!timer2.Enabled)
            {
                pm = new PlayMotion(m1);
                timer2.Interval = pm.CalcFrameInterval();
                timer2.Enabled = true;
                sendCmd(pm.Play());
            }
            */
            SerialSlave s1 = new SerialSlave(serialPort1);

            for (int x = 0; x < m1.no_scenes; x++)
            {
                s1.Load(m1, x);
                s1.Play();
            }

        }

        private void loadBasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = "";

            bool t1 = timer1.Enabled;
            bool t2 = timer2.Enabled;
            timer1.Enabled = false;
            timer2.Enabled = false;

            // load rbm file
            openFileDialog1.Filter = "basic File|*.rbas";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
                return;

            int i=0;

            textBox2.Text = "";
            progmode.Visible = true;
            progmode.Text = "Load";

            try {

                TextReader tr = new StreamReader(filename);
                string line = "";

                while ((line = tr.ReadLine()) != null)
                {
                    string l=line.Trim(); // +"\r\n";
                    if (l.Length > 0)
                    {
                        prog[i++] = l;
                        textBox2.AppendText(l + "\r\n");
                    }
                }
                tr.Close();

                pgsze = i;

            }
            catch (Exception e1)
            {
                Console.WriteLine("RBas load Exception - " + e1.ToString());
            }

            timer1.Enabled = t1;
            timer2.Enabled = t2;
        }

        private void progmode_Click(object sender, EventArgs e)
        {
            if (progmode.Text == "Load")
            {
                if (serialPort1.IsOpen)
                {
                    String mod;

                    if (mode == 1)
                    {
                        mod = write2serial("p", true);
                        readmode();
                    }
                    if (mode == 2)
                    {
                        mode = 4;
                        showmode();

                        mod = expect_serial("eC0", "> ");

                        for (int j = 0; j < pgsze; j++)
                        {
                            mod = expect_serial(prog[j] + "\r", "> ");
                        }

                        mod = write2serial(".", true); // end of

                        textBox2.AppendText(mod);

                        progmode.Text = "Run";
                    }
                    else
                    {
                        Console.WriteLine("Wrong mode - " + mode);
                    }

                }
            }


            if (progmode.Text == "Run")
            {
                if (serialPort1.IsOpen)
                {
                    String mod;

                    if (mode == 2 || mode == 4)
                    {
                        mode = 5;
                        showmode();
                    }
                    string r = expect_serial("eC1", "End of ");

                    textBox2.Text = r;

                    // don't know if this comes back

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            readmode();

            if (mode == 2)
            {
                binmode = true;
                btf = new binxfer(serialPort1);

                serialPort1.Write("#"); // enter binary mode
                btf.send_msg_basic('v');

                if (btf.recv_packet())
                {
                    Console.WriteLine("Good packet ver = " + btf.buff[0].ToString());
                }
                else
                {
                    Console.WriteLine("Bad packet");
                }

                btf.send_msg_basic('p'); // exit bimary mode (no response required)

                binmode = false;
            }

        }

        private void sendCmd_bin(string n)
        {
            if (!binmode) return;

            string[] cmds = n.Split(':');

            for (int j = 0; j < cmds.Length; j++)
            {
                string s;
                int l = cmds[j].Length / 2;
                if (l > 0)
                {
                    int cn = Convert.ToInt32(cmds[j].Substring(2, 2), 16);
                    if ((cn >> 5) < 5 && (cn & 0x1F) == 31)
                    {
                        //SynchPosSend (no response)
                        btf.send_msg_raw('x', cmds[j]);
                    }
                    else
                    {
                        //everything else (wait for response)
                        btf.send_msg_raw('X', cmds[j]);
                    }
                }
            }
        }

    }
}
