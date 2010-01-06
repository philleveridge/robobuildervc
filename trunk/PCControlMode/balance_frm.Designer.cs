namespace RobobuilderLib
{
    partial class balance_frm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.xyp = new System.Windows.Forms.CheckBox();
            this.xzp = new System.Windows.Forms.CheckBox();
            this.tailp = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 251);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(490, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 22);
            this.button1.TabIndex = 1;
            this.button1.Text = "Go!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(34, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(212, 176);
            this.panel1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(270, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(212, 176);
            this.panel2.TabIndex = 3;
            // 
            // xyp
            // 
            this.xyp.AutoSize = true;
            this.xyp.Checked = true;
            this.xyp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.xyp.Location = new System.Drawing.Point(34, 17);
            this.xyp.Name = "xyp";
            this.xyp.Size = new System.Drawing.Size(46, 17);
            this.xyp.TabIndex = 4;
            this.xyp.Text = "XY?";
            this.xyp.UseVisualStyleBackColor = true;
            // 
            // xzp
            // 
            this.xzp.AutoSize = true;
            this.xzp.Checked = true;
            this.xzp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.xzp.Location = new System.Drawing.Point(270, 16);
            this.xzp.Name = "xzp";
            this.xzp.Size = new System.Drawing.Size(46, 17);
            this.xzp.TabIndex = 5;
            this.xzp.Text = "XZ?";
            this.xzp.UseVisualStyleBackColor = true;
            // 
            // tailp
            // 
            this.tailp.AutoSize = true;
            this.tailp.Checked = true;
            this.tailp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tailp.Location = new System.Drawing.Point(402, 17);
            this.tailp.Name = "tailp";
            this.tailp.Size = new System.Drawing.Size(49, 17);
            this.tailp.TabIndex = 6;
            this.tailp.Text = "Tail?";
            this.tailp.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "fit";
            // 
            // balance_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 264);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tailp);
            this.Controls.Add(this.xzp);
            this.Controls.Add(this.xyp);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "balance_frm";
            this.Text = "balance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox xyp;
        private System.Windows.Forms.CheckBox xzp;
        private System.Windows.Forms.CheckBox tailp;
        private System.Windows.Forms.Label label2;
    }
}