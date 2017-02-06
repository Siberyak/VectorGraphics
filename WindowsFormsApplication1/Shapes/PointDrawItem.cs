using System.Drawing;
using System.Windows.Forms;

namespace Shapes
{
    public class PointDrawItem : DrawingItem
    {
        public PointDrawItem(PointShape shape) : base(shape)
        {
        }

        public override void Reset()
        {
        }

        public override void Draw(IViewPort viewPort)
        {
            var shape = (PointShape) Shape;
            viewPort.Graphics.FillRectangles(new SolidBrush(shape.Color), new []{new RectangleF(shape.CenterLocation, new SizeF(1,1)) });
        }
    }

    public class TextDrawItem : DrawingItem
    {
        public TextDrawItem(LabelShape shape) : base(shape)
        {
        }

        public override void Reset()
        {
        }

        public override void Draw(IViewPort viewPort)
        {
            var labelShape = (LabelShape) Shape;
            viewPort.Graphics.DrawString(labelShape.Text, labelShape.Font ?? Control.DefaultFont, Brushes.Black, labelShape.CenterLocation);
        }
    }
}