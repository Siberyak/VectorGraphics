using System.Drawing;

namespace WindowsFormsApplication1
{
    public abstract class DrawingItem
    {
        protected Shape Shape;
        protected DrawingItem(Shape shape)
        {
            Shape = shape;
        }

        public abstract void Reset();

        public abstract void Draw(IViewPort viewPort, bool selected);
    }
}