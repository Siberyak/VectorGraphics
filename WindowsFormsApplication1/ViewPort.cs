using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Timers;
using System.Windows.Forms;


namespace Shapes
{
    public partial class ViewPort : ViewPortBase
    {
        #region TIMER

        protected override void OnTimerCreated()
        {
            base.OnTimerCreated();

            Timer.Elapsed += RefreshByTimer;
            Timer.Start();
        }

        private void RefreshByTimer(object sender, ElapsedEventArgs e)
        {
            var form = Application.OpenForms.OfType<Form>().FirstOrDefault();
            if (form == null)
                return;

            //ProcessSelection();

            Action a = Refresh;
            try
            {
                Timer.Stop();
                form.Invoke(a);
                Timer.Start();
            }
            catch
            {
                Timer.Elapsed -= RefreshByTimer;
                Timer.Stop();
            }
        }

        #endregion

        public ViewPort()
        {
            InitializeComponent();

        }

    }
}
