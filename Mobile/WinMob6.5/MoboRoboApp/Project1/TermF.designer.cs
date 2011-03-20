namespace MoboRobo
{
    partial class TermF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.termw = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "CONNECT";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // term
            // 
            this.termw.BackColor = System.Drawing.SystemColors.ControlText;
            this.termw.ForeColor = System.Drawing.Color.Green;
            this.termw.Location = new System.Drawing.Point(0, 0);
            this.termw.Name = "term";
            this.termw.Size = new System.Drawing.Size(227, 140);
            this.termw.Text = "Basic Terminal V1\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(252, 178);
            this.Controls.Add(this.termw);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "BASIC ROBOT TERM";
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.Label termw;
    }
}

