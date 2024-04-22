using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Filtering
{
    internal class Program
    {

        [STAThread]
        private static void Main()
        {
            Application.Run(new DemoCustom());
        }
    }
}
