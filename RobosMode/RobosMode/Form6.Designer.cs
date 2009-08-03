namespace RobosMode
{
    partial class Form6
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
            this.input = new System.Windows.Forms.TextBox();
            this.close_btn = new System.Windows.Forms.Button();
            this.compile_btn = new System.Windows.Forms.Button();
            this.download_btn = new System.Windows.Forms.Button();
            this.output = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // input
            // 
            this.input.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.input.Location = new System.Drawing.Point(14, 21);
            this.input.Multiline = true;
            this.input.Name = "input";
            this.input.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.input.Size = new System.Drawing.Size(521, 203);
            this.input.TabIndex = 0;
            this.input.Text = "5 PRINT \"Test Passive\"\r\n10 SERVO 12=@\r\n20 Print \"Servo 12 = \"; $SERVO:12\r\n30 wait" +
                " 500\r\n40 goto 20\r\n";
            // 
            // close_btn
            // 
            this.close_btn.Location = new System.Drawing.Point(481, 308);
            this.close_btn.Name = "close_btn";
            this.close_btn.Size = new System.Drawing.Size(55, 27);
            this.close_btn.TabIndex = 1;
            this.close_btn.Text = "Close";
            this.close_btn.UseVisualStyleBackColor = true;
            this.close_btn.Click += new System.EventHandler(this.close_btn_Click);
            // 
            // compile_btn
            // 
            this.compile_btn.Location = new System.Drawing.Point(12, 306);
            this.compile_btn.Name = "compile_btn";
            this.compile_btn.Size = new System.Drawing.Size(55, 27);
            this.compile_btn.TabIndex = 2;
            this.compile_btn.Text = "Compile";
            this.compile_btn.UseVisualStyleBackColor = true;
            this.compile_btn.Click += new System.EventHandler(this.compile_btn_Click);
            // 
            // download_btn
            // 
            this.download_btn.Location = new System.Drawing.Point(84, 306);
            this.download_btn.Name = "download_btn";
            this.download_btn.Size = new System.Drawing.Size(74, 27);
            this.download_btn.TabIndex = 3;
            this.download_btn.Text = "Download";
            this.download_btn.UseVisualStyleBackColor = true;
            this.download_btn.Click += new System.EventHandler(this.download_btn_Click);
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(12, 230);
            this.output.Multiline = true;
            this.output.Name = "output";
            this.output.ReadOnly = true;
            this.output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.output.Size = new System.Drawing.Size(523, 65);
            this.output.TabIndex = 4;
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 345);
            this.Controls.Add(this.output);
            this.Controls.Add(this.download_btn);
            this.Controls.Add(this.compile_btn);
            this.Controls.Add(this.close_btn);
            this.Controls.Add(this.input);
            this.Name = "Form6";
            this.Text = "Basic Compiler V0.1 ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input;
        private System.Windows.Forms.Button close_btn;
        private System.Windows.Forms.Button compile_btn;
        private System.Windows.Forms.Button download_btn;
        private System.Windows.Forms.TextBox output;
    }
}