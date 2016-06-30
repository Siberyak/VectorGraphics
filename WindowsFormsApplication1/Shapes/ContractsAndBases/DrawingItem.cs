using System.Drawing;

namespace Shapes
{
    public abstract class DrawingItem
    {
        protected IShape Shape;
        protected DrawingItem(IShape shape)
        {
            Shape = shape;
        }

        public abstract void Reset();

        public abstract void Draw(IViewPort viewPort);
    }
}