using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Timers;

namespace Shapes
{
    public class RectangleShape : PathedShape
    {
        public RectangleShape(Vector2F size, bool isSolid) : base(size, isSolid)
        {
        }

        public RectangleShape(Vector2F centerLocation, Vector2F size, bool isSolid) : base(centerLocation, size, isSolid)
        {
        }

        protected override void GetPathFigures(GraphicsPath path, Bounds2F selfBounds)
        {
            path.AddRectangle(selfBounds);
        }
    }
}