namespace RobobuilderLib
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.exit_btn = new System.Windows.Forms.Button();
            this.script = new System.Windows.Forms.TextBox();
            this.action = new System.Windows.Forms.TextBox();
            this.run_btn = new System.Windows.Forms.Button();
            this.store_btn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(254, 256);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(56, 25);
            this.exit_btn.TabIndex = 0;
            this.exit_btn.Text = "exit";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // script
            // 
            this.script.Location = new System.Drawing.Point(6, 38);
            this.script.Multiline = true;
            this.script.Name = "script";
            this.script.Size = new System.Drawing.Size(224, 55);
            this.script.TabIndex = 1;
            this.script.Text = "Reach\r\nAim";
            // 
            // action
            // 
            this.action.Location = new System.Drawing.Point(6, 8);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(75, 20);
            this.action.TabIndex = 2;
            // 
            // run_btn
            // 
            this.run_btn.Location = new System.Drawing.Point(87, 8);
            this.run_btn.Name = "run_btn";
            this.run_btn.Size = new System.Drawing.Size(36, 23);
            this.run_btn.TabIndex = 3;
            this.run_btn.Text = "Run";
            this.run_btn.UseVisualStyleBackColor = true;
            this.run_btn.Click += new System.EventHandler(this.run_btn_Click);
            // 
            // store_btn
            // 
            this.store_btn.Location = new System.Drawing.Point(248, 8);
            this.store_btn.Name = "store_btn";
            this.store_btn.Size = new System.Drawing.Size(56, 28);
            this.store_btn.TabIndex = 4;
            this.store_btn.Text = "Store";
            this.store_btn.UseVisualStyleBackColor = true;
            this.store_btn.Click += new System.EventHandler(this.store_btn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.panel1.Controls.Add(this.store_btn);
            this.panel1.Controls.Add(this.run_btn);
            this.panel1.Controls.Add(this.action);
            this.panel1.Controls.Add(this.script);
            this.panel1.Location = new System.Drawing.Point(3, 137);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 113);
            this.panel1.TabIndex = 5;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(271, 9);
            this.vScrollBar1.Maximum = 500;
            this.vScrollBar1.Minimum = 10;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(21, 116);
            this.vScrollBar1.TabIndex = 6;
            this.vScrollBar1.Value = 100;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "1.0";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(14, 263);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(58, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Debug";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(333, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(163, 271);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 36;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 284);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.exit_btn);
            this.Name = "Form2";
            this.Text = "Preset Poses/Actions/Motions";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.TextBox script;
        private System.Windows.Forms.TextBox action;
        private System.Windows.Forms.Button run_btn;
        private System.Windows.Forms.Button store_btn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PictureBox pictureBox1;

    }
}