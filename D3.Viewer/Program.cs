using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapes;

namespace D3.Viewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            ProcessorsFactory.Register(x => SelectionInputInfoProcessor.New(x));
            ProcessorsFactory.Register(x => ViewPortInputInfoProcessor.New(x));
            ProcessorsFactory.Register(x => MouseOverInputInfoProcessor.New(x));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new D3Form());
        }
    }
}
