using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shapes
{
    public class ShapesProvider : IShapesProvider
    {
        protected readonly List<IShape> Items = new List<IShape>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<IShape> IEnumerable<IShape>.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual IEnumerator<IShape> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool Contains(IShape shape)
        {
            return Items.Contains(shape);
        }

        public void Remove(IShape shape)
        {
            if (shape == null || !Items.Contains(shape))
                return;

            Items.Remove(shape);
        }

        public void Add(IShape shape)
        {
            if (shape == null || Items.Contains(shape))
                return;

            Items.Add(shape);
        }

        public IShape FirstOrDefault(Vector2F point)
        {
            return Items.FirstOrDefault(x => x.Contains(point));
        }

        public IShape LastOrDefault(Vector2F point)
        {
            return Items.LastOrDefault(x => x.Contains(point));
        }

        public IEnumerable<IShape> Contains(Vector2F point)
        {
            return Items.Where(x => x.Contains(point)).ToArray();
        }

        public IEnumerable<IShape> Contains(Bounds2F bounds)
        {
            return Items.Where(x => x.Contains(bounds)).ToArray();
        }

        public IEnumerable<IShape> IntersectsWith(Bounds2F bounds)
        {
            return Items.Where(x => x.IntersectsWith(bounds)).ToArray();
        }
    }
}