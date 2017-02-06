using System.Collections.Generic;

namespace Shapes
{
    public interface IShapesProvider : IEnumerable<IShape>
    {
        void Add(IShape shape);
        void Remove(IShape shape);
        bool Contains(IShape shape);

        void Clear();

        IShape FirstOrDefault(Vector2F point);
        IShape LastOrDefault(Vector2F point);
        IEnumerable<IShape> Contains(Vector2F point);
        IEnumerable<IShape> Contains(Bounds2F bounds);
        IEnumerable<IShape> IntersectsWith(Bounds2F bounds);
    }
}