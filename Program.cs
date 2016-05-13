using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RenamerPro
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
            MainForm form1 = new MainForm();
            form1.MaximizeBox = false;
            Application.Run(form1);
        }
    }
}
