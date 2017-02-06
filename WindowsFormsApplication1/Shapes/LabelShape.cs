using System.Collections.Generic;
using System.Drawing;

namespace Shapes
{
    public class LabelShape : Shape
    {
        public string Text;
        public Font Font;

        protected override IEnumerable<DrawingItem> GetItems()
        {
            return new[] {new TextDrawItem(this),};
        }

        public override bool Contains(Vector2F point)
        {
            return false;
        }

        public override bool Contains(Bounds2F bounds)
        {
            return false;
        }

        public override bool IntersectsWith(Bounds2F bounds)
        {
            return false;
        }
    }
}