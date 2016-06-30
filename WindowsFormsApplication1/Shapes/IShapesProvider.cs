using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public interface IShapesProvider : IEnumerable<Shape>
    {
        void Add(Shape shape);
        void Remove(Shape shape);
        bool Contains(Shape shape);

        IEnumerable<Shape> Contains(Vector2F point);
        IEnumerable<Shape> Contains(Bounds2F bounds);
        IEnumerable<Shape> IntersectsWith(Bounds2F bounds);
    }
}