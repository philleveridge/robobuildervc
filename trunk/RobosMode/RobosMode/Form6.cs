using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace RobosMode
{
    public partial class Form6 : Form
    {
        Basic compiler;

        public Form6()
        {
            InitializeComponent();

            compiler = new Basic();
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void compile_btn_Click(object sender, EventArgs e)
        {
            if (compiler.Compile(input.Text))
            {
                output.Text = "Complete - ready to download\r\n";
            }
            else
            {
                output.Text = "error on line " + compiler.lineno + " : " + compiler.errno + "\r\n";
                output.Text += compiler.Dump();
            }
        }

        private void download_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not available yet");
        }
    }
}
