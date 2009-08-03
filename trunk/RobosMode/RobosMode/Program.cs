using System;
using System.Windows.Forms;

namespace RobobuilderLib
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 robos = new Form1();
            robos.Text = "Robos Control code";
            Application.Run(robos);
        }
    }
}
