using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using RobobuilderLib;






namespace SinDemo
{
    public partial class Form1 : Form
    {

        wckMotion w;

        double[] amps = new double[] {   10.5,	13.5,	 15,	11.5,	10.5,	10.5,	13.5,	15,	11.5,	10.5,	22.5,	 0};
        int[] offas   = new int[]    {    142,	166,	210,	  92,	 107,	109,	  83,	42,	 159,	 144,	 100,	70};
        int[] phase   = new int[]    {      0,	 14,	  8,	  11,	  16,	  1,	  14,	 8,	  10,	   1,	  11,	11};


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            w = new wckMotion("COM5", true);

            if (!w.wckReadPos(30, 0))
            {
                textBox1.AppendText("Failed to connect or not DCMP");
                w.close();
                w = null;
                return;
            }

            textBox1.AppendText(string.Format("DCMP {0}.{1}", w.respnse[0], w.respnse[1]));

            w.PlayPose(1000, 10, wckMotion.dh, true);

        }
    }
}
