using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Shapes
{
    public class EllipseShape : PathedShape
    {
        public EllipseShape(Vector2F centerLocation, Vector2F size, bool isSolid) : base(centerLocation, size, isSolid)
        {
        }

        public EllipseShape(Vector2F size, bool isSolid) : base(size, isSolid)
        {
        }

        protected override void GetPathFigures(GraphicsPath path, Bounds2F selfBounds)
        {
            path.AddEllipse(selfBounds);
        }

    }
}