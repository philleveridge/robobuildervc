namespace RobobuilderLib
{
    partial class Display3D_frm
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
            if (simulation_running) return;

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
            this.button1 = new System.Windows.Forms.Button();
            this.viewPort = new System.Windows.Forms.Panel();
            this.pause_msg = new System.Windows.Forms.Label();
            this.Tx = new System.Windows.Forms.TextBox();
            this.Ty = new System.Windows.Forms.TextBox();
            this.Tz = new System.Windows.Forms.TextBox();
            this.Vx = new System.Windows.Forms.TextBox();
            this.Vy = new System.Windows.Forms.TextBox();
            this.Vz = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.sim_btn = new System.Windows.Forms.Button();
            this.viewPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(435, 410);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // viewPort
            // 
            this.viewPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.viewPort.Controls.Add(this.pause_msg);
            this.viewPort.Location = new System.Drawing.Point(23, 17);
            this.viewPort.Name = "viewPort";
            this.viewPort.Size = new System.Drawing.Size(466, 380);
            this.viewPort.TabIndex = 1;
            this.viewPort.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewport_MouseDown);
            // 
            // pause_msg
            // 
            this.pause_msg.AutoSize = true;
            this.pause_msg.BackColor = System.Drawing.Color.White;
            this.pause_msg.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pause_msg.Location = new System.Drawing.Point(18, 12);
            this.pause_msg.Name = "pause_msg";
            this.pause_msg.Size = new System.Drawing.Size(85, 25);
            this.pause_msg.TabIndex = 0;
            this.pause_msg.Text = "Paused";
            this.pause_msg.Visible = false;
            // 
            // Tx
            // 
            this.Tx.Location = new System.Drawing.Point(42, 416);
            this.Tx.Name = "Tx";
            this.Tx.ReadOnly = true;
            this.Tx.Size = new System.Drawing.Size(39, 20);
            this.Tx.TabIndex = 2;
            this.Tx.Text = "1";
            // 
            // Ty
            // 
            this.Ty.Location = new System.Drawing.Point(87, 416);
            this.Ty.Name = "Ty";
            this.Ty.ReadOnly = true;
            this.Ty.Size = new System.Drawing.Size(39, 20);
            this.Ty.TabIndex = 3;
            this.Ty.Text = "1";
            // 
            // Tz
            // 
            this.Tz.Location = new System.Drawing.Point(132, 416);
            this.Tz.Name = "Tz";
            this.Tz.ReadOnly = true;
            this.Tz.Size = new System.Drawing.Size(39, 20);
            this.Tz.TabIndex = 4;
            this.Tz.Text = "-5";
            // 
            // Vx
            // 
            this.Vx.Location = new System.Drawing.Point(215, 416);
            this.Vx.Name = "Vx";
            this.Vx.ReadOnly = true;
            this.Vx.Size = new System.Drawing.Size(39, 20);
            this.Vx.TabIndex = 5;
            this.Vx.Text = "0";
            // 
            // Vy
            // 
            this.Vy.Location = new System.Drawing.Point(260, 416);
            this.Vy.Name = "Vy";
            this.Vy.ReadOnly = true;
            this.Vy.Size = new System.Drawing.Size(39, 20);
            this.Vy.TabIndex = 6;
            this.Vy.Text = "0";
            // 
            // Vz
            // 
            this.Vz.Location = new System.Drawing.Point(305, 416);
            this.Vz.Name = "Vz";
            this.Vz.ReadOnly = true;
            this.Vz.Size = new System.Drawing.Size(39, 20);
            this.Vz.TabIndex = 7;
            this.Vz.Text = "0";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(363, 417);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 17);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Hide";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // sim_btn
            // 
            this.sim_btn.Location = new System.Drawing.Point(463, 9);
            this.sim_btn.Name = "sim_btn";
            this.sim_btn.Size = new System.Drawing.Size(44, 29);
            this.sim_btn.TabIndex = 9;
            this.sim_btn.Text = "Go";
            this.sim_btn.UseVisualStyleBackColor = true;
            this.sim_btn.Click += new System.EventHandler(this.sim_btn_Click);
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 449);
            this.Controls.Add(this.sim_btn);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Vz);
            this.Controls.Add(this.Vy);
            this.Controls.Add(this.Vx);
            this.Controls.Add(this.Tz);
            this.Controls.Add(this.Ty);
            this.Controls.Add(this.Tx);
            this.Controls.Add(this.viewPort);
            this.Controls.Add(this.button1);
            this.Name = "Form5";
            this.Text = "Display 3D";
            this.viewPort.ResumeLayout(false);
            this.viewPort.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel viewPort;
        private System.Windows.Forms.TextBox Tx;
        private System.Windows.Forms.TextBox Ty;
        private System.Windows.Forms.TextBox Tz;
        private System.Windows.Forms.TextBox Vx;
        private System.Windows.Forms.TextBox Vy;
        private System.Windows.Forms.TextBox Vz;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button sim_btn;
        private System.Windows.Forms.Label pause_msg;
    }
}