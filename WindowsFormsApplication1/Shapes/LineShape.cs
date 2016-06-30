using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Shapes
{
    public class LineShape : PathedShape
    {

        private LineShape(Vector2F centerLocation, Vector2F size) : base(centerLocation, size, false)
        {
            Timer.Elapsed += (sender, args) => ResetItems();
        }

        public static LineShape New(Vector2F from, Vector2F to)
        {
            return New(() => from, () => to);
        }

        public static LineShape New(Func<Vector2F> from, Func<Vector2F> to)
        {
            var bounds = Bounds2F.Union((Bounds2F) @from(), (Bounds2F) to());
            var centerLocation = bounds.Center;
            return new LineShape(centerLocation, bounds.Size) { From = from, To = to };
        }

        public Func<Vector2F> From { get; set; }
        public Func<Vector2F> To { get; set; }

        protected override void GetPathFigures(GraphicsPath path, Bounds2F selfBounds)
        {
            var from = From?.Invoke() ?? Vector2F.Zerro;
            var to = To?.Invoke() ?? Vector2F.Zerro;
            path.AddLine(from, to);
            path.CloseAllFigures();
        }

        public override bool Contains(Vector2F point)
        {
            var path = GetPath(this);
            return ContainsByPath(path, point);
        }

        public override bool Contains(Bounds2F bounds)
        {
            return false;
        }
    }
}