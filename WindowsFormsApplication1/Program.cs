using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Application.ThreadException += ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ProcessorsFactory.Register(x => SelectionInputInfoProcessor.New(x, SelectionInputInfoProcessor.ViewPortMode.Selection));
            ProcessorsFactory.Register(x => MouseOverInputInfoProcessor.New(x));

            Application.Run(new Form2());
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
    }
}
