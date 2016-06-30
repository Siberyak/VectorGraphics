using System.Collections.Generic;
using System.Drawing;

namespace Shapes
{
    public class PointShape : Shape
    {
        private Bounds2F _pathBounds;

        public PointShape(Vector2F centerLocation, Color color) : base(centerLocation, new Vector2F(1,1), 0, false)
        {
            _pathBounds = new Bounds2F(CenterLocation, new Vector2F(1, 1));
            Color = color;
        }

        //public override Bounds2F Bounds
        //{
        //    get { return _pathBounds; }
        //}


        public Color Color { get; set; }

        protected override void OnLocationChanged(Vector2F oldValue)
        {
            _pathBounds = new Bounds2F(CenterLocation, new Vector2F(1, 1));
            base.OnLocationChanged(oldValue);
        }

        protected override IEnumerable<DrawingItem> GetItems()
        {
            return new[] { new PointDrawItem(this) };
        }

        public override bool Contains(Vector2F point)
        {
            return CenterLocation == point;
        }

        public override bool Contains(Bounds2F bounds)
        {
            return CenterLocation == bounds.Location && bounds.Size == Vector2F.Zerro;
        }

        public override bool IntersectsWith(Bounds2F bounds)
        {
            return bounds.Contains(CenterLocation);
        }
    }
}