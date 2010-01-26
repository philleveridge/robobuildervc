namespace RobobuilderLib
{
    partial class Video_frm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            this.detectionCheck = new System.Windows.Forms.CheckBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.red_bar = new System.Windows.Forms.HScrollBar();
            this.green_bar = new System.Windows.Forms.HScrollBar();
            this.blue_bar = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdPause = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // videoSourcePlayer
            // 
            this.videoSourcePlayer.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoSourcePlayer.ForeColor = System.Drawing.Color.White;
            this.videoSourcePlayer.Location = new System.Drawing.Point(12, 11);
            this.videoSourcePlayer.Name = "videoSourcePlayer";
            this.videoSourcePlayer.Size = new System.Drawing.Size(322, 242);
            this.videoSourcePlayer.TabIndex = 1;
            this.videoSourcePlayer.VideoSource = null;
            this.videoSourcePlayer.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer_NewFrame);
            // 
            // detectionCheck
            // 
            this.detectionCheck.AutoSize = true;
            this.detectionCheck.Location = new System.Drawing.Point(157, 259);
            this.detectionCheck.Name = "detectionCheck";
            this.detectionCheck.Size = new System.Drawing.Size(75, 17);
            this.detectionCheck.TabIndex = 26;
            this.detectionCheck.Text = "visual filter";
            this.detectionCheck.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 16;
            this.listBox2.Items.AddRange(new object[] {
            "<undef>"});
            this.listBox2.Location = new System.Drawing.Point(12, 260);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(139, 20);
            this.listBox2.TabIndex = 25;
            // 
            // cmdStop
            // 
            this.cmdStop.Location = new System.Drawing.Point(346, 44);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(78, 24);
            this.cmdStop.TabIndex = 24;
            this.cmdStop.Text = "Stop";
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdStart
            // 
            this.cmdStart.Location = new System.Drawing.Point(346, 12);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(78, 24);
            this.cmdStart.TabIndex = 23;
            this.cmdStart.Text = "Start";
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(346, 252);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 24);
            this.button1.TabIndex = 27;
            this.button1.Text = "Close";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 314);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(245, 260);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(31, 22);
            this.panel1.TabIndex = 29;
            this.panel1.Click += new System.EventHandler(this.panel1_click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(282, 260);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(33, 22);
            this.panel2.TabIndex = 30;
            this.panel2.Click += new System.EventHandler(this.panel2_click);
            // 
            // red_bar
            // 
            this.red_bar.Location = new System.Drawing.Point(248, 296);
            this.red_bar.Maximum = 255;
            this.red_bar.Name = "red_bar";
            this.red_bar.Size = new System.Drawing.Size(66, 9);
            this.red_bar.TabIndex = 31;
            this.red_bar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.color_bar_Scroll);
            // 
            // green_bar
            // 
            this.green_bar.Location = new System.Drawing.Point(248, 314);
            this.green_bar.Maximum = 255;
            this.green_bar.Name = "green_bar";
            this.green_bar.Size = new System.Drawing.Size(66, 9);
            this.green_bar.TabIndex = 32;
            this.green_bar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.color_bar_Scroll);
            // 
            // blue_bar
            // 
            this.blue_bar.Location = new System.Drawing.Point(249, 331);
            this.blue_bar.Maximum = 255;
            this.blue_bar.Name = "blue_bar";
            this.blue_bar.Size = new System.Drawing.Size(66, 9);
            this.blue_bar.TabIndex = 33;
            this.blue_bar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.color_bar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(210, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = " ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 314);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = " ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 331);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = " ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(317, 296);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = " ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(317, 314);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 38;
            this.label6.Text = " ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(318, 331);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(10, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = " ";
            // 
            // cmdPause
            // 
            this.cmdPause.Location = new System.Drawing.Point(346, 74);
            this.cmdPause.Name = "cmdPause";
            this.cmdPause.Size = new System.Drawing.Size(78, 24);
            this.cmdPause.TabIndex = 40;
            this.cmdPause.Text = "Pause";
            this.cmdPause.Visible = false;
            this.cmdPause.Click += new System.EventHandler(this.cmdPause_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(322, 242);
            this.pictureBox1.TabIndex = 41;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(pictureBox1_MouseUp);
            // 
            // Video_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 351);
            this.Controls.Add(this.cmdPause);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.blue_bar);
            this.Controls.Add(this.green_bar);
            this.Controls.Add(this.red_bar);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.detectionCheck);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.cmdStop);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.videoSourcePlayer);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Video_frm";
            this.Text = " ";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            System.Console.WriteLine("mouse up = " + e.X + "," + e.Y);
            mu(e.X, e.Y);
        }

        void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Console.WriteLine("mouse down = " + e.X + "," + e.Y);
            md(e.X, e.Y);
        }

        #endregion

        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private System.Windows.Forms.CheckBox detectionCheck;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.HScrollBar red_bar;
        private System.Windows.Forms.HScrollBar green_bar;
        private System.Windows.Forms.HScrollBar blue_bar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdPause;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}