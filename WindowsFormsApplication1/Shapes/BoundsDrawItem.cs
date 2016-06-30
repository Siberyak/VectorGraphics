using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Shapes
{
    public class BoundsDrawItem : GraphicsPathDrawItem
    {
        private static Pen _pen = new Pen(Color.LightSlateGray) { DashStyle = DashStyle.Dot };

        public BoundsDrawItem(IShape shape, Func<IShape, GraphicsPath> getPath)
            : base(shape, _pen, getPath)
        {
        }
    }
}