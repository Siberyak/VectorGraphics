using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapes;

namespace TestApplication
{
    public partial class MainForm : Form
    {
        private Vector2F _baseSize;

        public MainForm()
        {
            InitializeComponent();

            if(DesignMode)
                return;

            _viewPort.Shapes = new ShapesProvider();
            var rectangle = _viewPort.Shapes.AddRectangle(Vector2F.Zerro, _viewPort.Size - new Vector2F(10, 10), Pens.BlueViolet);

            _baseSize = _viewPort.Size;

            _viewPort.BoundsChanged += ViewPortBoundsChanged;

            var siip = _viewPort.Add<SIIP>("Selection IIP");

            siip.AllowSelect = siip.AllowSelect.Next(x => x != rectangle);
            siip.AllowFocus = siip.AllowFocus.Next(x => x != rectangle);

            //_viewPort.Add<MouseOverInputInfoProcessor>("Mouse Over IIP");


            
            var cnt = 10;
            var halfCnt = cnt/2;
            var a = cnt - halfCnt*2;
            var size = 250.0f / cnt;

            for (int x = -halfCnt; x < halfCnt + a; x++)
            {
                for (int y = -halfCnt; y < halfCnt + a; y++)
                {
                    _viewPort.Shapes.AddRectangle(Vector2F.Zerro + new Vector2F(x*size, y*size), new Vector2F(size-2, size -2), Pens.Black);
                }
            }


        }

        private void ViewPortBoundsChanged(object sender, BoundsChangedEventArgs e)
        {
            var minK = 0.0001f;

            var wk = e.Current.Width/_baseSize.X - 1;
            var hk = e.Current.Height/_baseSize.Y - 1;

            var k = Math.Min(wk, hk) + 1;
            _viewPort.Zoom = k;
        }
    }

}

