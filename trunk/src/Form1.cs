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
using AForge.Video.DirectShow;
using AForge.Imaging.ComplexFilters;
using AForge.Imaging.Filters;
using AForge.Vision.Motion;

namespace RobobuilderVC
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{

        private System.Drawing.Bitmap m_Bitmap;
        private System.Drawing.Bitmap p_Bitmap;
        private double Zoom = 1.0;
        VideoCaptureDevice wcam;


		private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdStop;
        private System.IO.Ports.SerialPort serialPort1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextBox textBox1;
        private TextBox textBox2;
        private PictureBox pictureBox2;
        private ListBox listBox1;
        private Button button5;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private IContainer components;

        int cnt;
        private Button button6;
        private ListBox listBox2;
        CountingMotionDetector detector;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//

			InitializeComponent();

           
            //
			// TODO: Add any constructor code after InitializeComponent call
			//
            m_Bitmap = null;
            p_Bitmap = null;
            cnt = 0;
            // create motion detector with motion highlight
            detector = new CountingMotionDetector(true);
            // set minimum objects' size
            detector.MinObjectsWidth = 10;
            detector.MinObjectsHeight = 10;
            detector.MaxObjectsHeight = 100;
            detector.MaxObjectsWidth = 100;

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
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button5 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(6, 269);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(78, 24);
            this.cmdStart.TabIndex = 1;
            this.cmdStart.Text = "Start";
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.Location = new System.Drawing.Point(6, 294);
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 253);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(18, 19);
            this.button1.TabIndex = 6;
            this.button1.Text = "^";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(211, 269);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(18, 19);
            this.button2.TabIndex = 7;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(182, 269);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(18, 19);
            this.button3.TabIndex = 8;
            this.button3.Text = "<";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(197, 286);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(18, 19);
            this.button4.TabIndex = 9;
            this.button4.Text = "v";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(89, 321);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(302, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(90, 347);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(307, 88);
            this.textBox2.TabIndex = 11;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(45, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(320, 240);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "COM3",
            "COM4",
            "COM5"});
            this.listBox1.Location = new System.Drawing.Point(278, 264);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(113, 17);
            this.listBox1.TabIndex = 14;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Red;
            this.button5.Location = new System.Drawing.Point(341, 290);
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
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(235, 286);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(50, 24);
            this.button6.TabIndex = 17;
            this.button6.Text = "wave";
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Items.AddRange(new object[] {
            "<undef>"});
            this.listBox2.Location = new System.Drawing.Point(6, 246);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(147, 17);
            this.listBox2.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(413, 486);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.cmdStart);
            this.Name = "Form1";
            this.Text = "Robobuilder Video Control";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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

        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

		private void Form1_Load(object sender, System.EventArgs e)
		{
            label1.Visible = false;
            try
            {
                //this.AutoScroll = true;
                //this.AutoScrollMinSize = new Size((int)(m_Bitmap.Width * Zoom), (int)(m_Bitmap.Height * Zoom));
                //this.Invalidate();


                // enumerate video devices
                
                // set up chooser
                listBox2.Items.Clear();
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    listBox2.Items.Add(videoDevices[i].Name);
                }
                             
            }
            catch (Exception e2)
            {
                Console.WriteLine("Video camera initialisation error - " + e2);
                cmdStart.Enabled = false;
                cmdStop.Enabled=false;
            }
            try
            {
                listBox1.Items.Clear();
                foreach (string s in SerialPort.GetPortNames())
                {
                    System.Diagnostics.Trace.WriteLine(s);
                    listBox1.Items.Add(s);
                }

                //serialPort1 = new System.IO.Ports.SerialPort();
                serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
                serialPort1.Open();
                serial_status();
            }
            catch (Exception e1)
            {
                Console.WriteLine("Comm port error - " + e1);
            }
		}

        void wcam_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                Monitor.Enter(this) ;
                cnt++;

                // get new frame
                if (m_Bitmap != null)
                    m_Bitmap.Dispose();

                m_Bitmap = (Bitmap)eventArgs.Frame.Clone();

                if (m_Bitmap != null)
                {

                    // feed first image to the detector
                    detector.ProcessFrame(m_Bitmap);
                    // ...
                    // feed next image
                    // check amount of moving objects
                    if (detector.ObjectsCount > 0)
                    {
                        Rectangle[] objects = detector.ObjectRectangles;
                    }
                }


                //Edges ed = new Edges();

                //p_Bitmap = ed.Apply(m_Bitmap);


                // check if we need to draw gesture information on top of image
                //if ( gestureShowTime > 0 )
                //{
                //if ( ( gesture.LeftHand != HandPosition.NotRaised ) || ( gesture.RightHand != HandPosition.NotRaised ) )
                //{
                Graphics g = Graphics.FromImage(m_Bitmap);

                string text = string.Format("Robovision 0.1");

                Font drawFont = new Font("Courier", 13, FontStyle.Bold);
                SolidBrush drawBrush = new SolidBrush(Color.Blue);

                g.DrawString(text, drawFont, drawBrush, new PointF(0, 5));

                drawFont.Dispose();
                drawBrush.Dispose();

                g.Dispose();
                //}
                //gestureShowTime--;
                //}

                pictureBox2.Image = (Bitmap)m_Bitmap;


            }
            catch (Exception e4)
            {
                System.Console.WriteLine("newframe error " + e4);
            }
            finally {
                Monitor.Exit(this);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (m_Bitmap != null)
                g.DrawImage(m_Bitmap, new Rectangle(0,0, (int)(m_Bitmap.Width * Zoom), (int)(m_Bitmap.Height * Zoom)));
        }

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// stop the video capture and serial port
            //wcam.Stop();
            if (wcam != null && wcam.IsRunning)
                wcam.Stop();

            if (serialPort1.IsOpen) serialPort1.Close();
		}

		private void cmdStart_Click(object sender, System.EventArgs e)
		{
			// start the video capture.
            label1.Visible = true;

            // create video source
            wcam = new VideoCaptureDevice(videoDevices[listBox2.SelectedIndex].MonikerString); // hard coded 
            // wcam = new VideoCaptureDevice(videoDevices[2].MonikerString); // hard coded 
            // set NewFrame event handler
            wcam.NewFrame += new AForge.Video.NewFrameEventHandler(wcam_NewFrame);

            wcam.Start();
		}

		private void cmdStop_Click(object sender, System.EventArgs e)
		{
			// stop the video capture
            wcam.SignalToStop();
		}

        private void button1_Click(object sender, EventArgs e)
        {
            // forward
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write("F");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //right
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write("R");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //left
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write("L");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //back
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write("B");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write(textBox1.Text);
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Show all the incoming data in the port's buffer
            string s = serialPort1.ReadExisting();
            Console.WriteLine(s);
            textBox2.Invoke(new SetTextValueHandler(SetTextValue),s);
        }

        delegate void SetTextValueHandler(string value);
        void SetTextValue(string value)
        {
            this.textBox2.AppendText(value);
        }

        private string convert(string s)
        {
            string c = "";
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i]<32)
                    c += ".";
                else
                    c += s[i];

            }
            return c;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            { 
                serialPort1.Close();
            }
            else 
            {
                serialPort1.Open();
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
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //back
            if (!serialPort1.IsOpen) serialPort1.Open();
            serialPort1.Write("e11");  //special event 0x11
        }


    }
}
