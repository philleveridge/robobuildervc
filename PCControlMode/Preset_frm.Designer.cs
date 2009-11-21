namespace RobobuilderLib
{
    partial class Preset_frm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preset_frm));
            this.exit_btn = new System.Windows.Forms.Button();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.output_txt = new System.Windows.Forms.Label();
            this.store_btn = new System.Windows.Forms.Button();
            this.run_btn = new System.Windows.Forms.Button();
            this.action = new System.Windows.Forms.TextBox();
            this.script = new System.Windows.Forms.TextBox();
            this.dbg_flg = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.mssage_txt = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // exit_btn
            // 
            this.exit_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exit_btn.Location = new System.Drawing.Point(440, 286);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(56, 25);
            this.exit_btn.TabIndex = 0;
            this.exit_btn.Text = "exit";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(306, 9);
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
            this.label1.Location = new System.Drawing.Point(281, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "1.0";
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.panel1.Controls.Add(this.output_txt);
            this.panel1.Controls.Add(this.store_btn);
            this.panel1.Controls.Add(this.run_btn);
            this.panel1.Controls.Add(this.action);
            this.panel1.Controls.Add(this.script);
            this.panel1.Controls.Add(this.dbg_flg);
            this.panel1.Location = new System.Drawing.Point(12, 198);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 113);
            this.panel1.TabIndex = 5;
            this.panel1.Visible = false;
            // 
            // output_txt
            // 
            this.output_txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.output_txt.Location = new System.Drawing.Point(3, 93);
            this.output_txt.Name = "output_txt";
            this.output_txt.Size = new System.Drawing.Size(301, 19);
            this.output_txt.TabIndex = 9;
            // 
            // store_btn
            // 
            this.store_btn.Location = new System.Drawing.Point(261, 5);
            this.store_btn.Name = "store_btn";
            this.store_btn.Size = new System.Drawing.Size(43, 26);
            this.store_btn.TabIndex = 4;
            this.store_btn.Text = "Store";
            this.store_btn.UseVisualStyleBackColor = true;
            this.store_btn.Click += new System.EventHandler(this.store_btn_Click);
            // 
            // run_btn
            // 
            this.run_btn.BackColor = System.Drawing.SystemColors.Control;
            this.run_btn.Location = new System.Drawing.Point(219, 5);
            this.run_btn.Name = "run_btn";
            this.run_btn.Size = new System.Drawing.Size(36, 25);
            this.run_btn.TabIndex = 3;
            this.run_btn.Text = "Run";
            this.run_btn.UseVisualStyleBackColor = false;
            this.run_btn.Click += new System.EventHandler(this.run_btn_Click);
            // 
            // action
            // 
            this.action.Location = new System.Drawing.Point(6, 8);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(75, 20);
            this.action.TabIndex = 2;
            // 
            // script
            // 
            this.script.Location = new System.Drawing.Point(6, 38);
            this.script.Multiline = true;
            this.script.Name = "script";
            this.script.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.script.Size = new System.Drawing.Size(285, 51);
            this.script.TabIndex = 1;
            // 
            // dbg_flg
            // 
            this.dbg_flg.AutoSize = true;
            this.dbg_flg.Location = new System.Drawing.Point(129, 10);
            this.dbg_flg.Name = "dbg_flg";
            this.dbg_flg.Size = new System.Drawing.Size(58, 17);
            this.dbg_flg.TabIndex = 8;
            this.dbg_flg.Text = "Debug";
            this.dbg_flg.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(336, 293);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(59, 17);
            this.checkBox2.TabIndex = 37;
            this.checkBox2.Text = "Script?";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // mssage_txt
            // 
            this.mssage_txt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mssage_txt.Location = new System.Drawing.Point(1, 314);
            this.mssage_txt.Name = "mssage_txt";
            this.mssage_txt.Size = new System.Drawing.Size(501, 19);
            this.mssage_txt.TabIndex = 38;
            // 
            // Preset_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 332);
            this.Controls.Add(this.mssage_txt);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.panel1);
            this.Name = "Preset_frm";
            this.Text = "Preset Poses/Actions/Motions";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button store_btn;
        private System.Windows.Forms.Button run_btn;
        private System.Windows.Forms.TextBox action;
        private System.Windows.Forms.TextBox script;
        private System.Windows.Forms.CheckBox dbg_flg;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label output_txt;
        private System.Windows.Forms.Label mssage_txt;

    }
}