using System.Drawing;

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
}