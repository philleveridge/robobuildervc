using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace RobosMode
{
    public partial class Form6 : Form
    {
        Basic compiler;
        public RobobuilderLib.PCremote pcr;

        public Form6()
        {
            InitializeComponent();

            compiler = new Basic();

            download_btn.Enabled = false;
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
                output.Text += compiler.Dump();
                download_btn.Enabled = true;
            }
            else
            {
                output.Text = "error on line " + compiler.lineno + " : " + compiler.error_msgs[compiler.errno] + "\r\n";
                output.Text += compiler.Dump();
                download_btn.Enabled = false;
            }
        }

        private void download_btn_Click(object sender, EventArgs e)
        {
            string c= compiler.Download();

            if (pcr== null || pcr.download_basic(c) == false)
            {
                MessageBox.Show("Download failed");
            }
        }

        private void load_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog s = new OpenFileDialog();
            if (s.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                input.Text = File.ReadAllText(s.FileName);
                output.Text = "";
                download_btn.Enabled = false;
                fname.Text = s.FileName;
            }
            catch (Exception e1)
            {
                MessageBox.Show("can't open file - " + e1.Message);
                output.Text = "";
                download_btn.Enabled = false;
                fname.Text = "";
            }
        }
    }
}
