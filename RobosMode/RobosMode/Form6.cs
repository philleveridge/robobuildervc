﻿using System;
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

            if (pcr == null || pcr.download_basic(c) == false)
            {
                MessageBox.Show("Download failed");
            }
            else
            {
                MessageBox.Show("Download ok");
                output.Text="Complete - Ready to run";
            }
        }


        private void run_btn_Click(object sender, EventArgs e)
        {
            if (run_btn.Text != "Run")
            {
                pcr.serialPort1.Write("\u0027");
            }
            else
            {
                if (pcr != null)
                {
                    run_btn.Text = "Break";
                    output.Text = pcr.run_basic();

                    do
                    {
                        string t = pcr.serialPort1.ReadExisting();
                        output.AppendText(t);
                        if (output.Text.Contains("End of program")) break;
                        if (output.Text.Contains("User Break")) break;
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(1000);
                    }
                    while (pcr.serialPort1.BytesToRead > 0);

                    run_btn.Text = "Run";
                }
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

        private void save_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            if (s.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                File.WriteAllText(s.FileName, input.Text);
                fname.Text = s.FileName;
            }
            catch (Exception e1)
            {
                MessageBox.Show("can't save file - " + e1.Message);
                fname.Text = "";
            }
        }

    }
}