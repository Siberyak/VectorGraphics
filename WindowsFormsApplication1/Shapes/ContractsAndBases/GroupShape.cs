using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shapes
{
    public class GroupShape : IShape, IShapesProvider//, ISelectableShape, IMovableShape
    {
        protected readonly IShapesProvider Provider = new ShapesProvider();
        private Vector2F _centerLocation;
        private float _rotation;
        private Vector2F _aspectRatio;

        public GroupShape()
        {
        }

        Vector2F IShape.Location
        {
            get { return CenterLocation; }
        }

        public virtual Vector2F CenterLocation
        {
            get { return _centerLocation; }
            set
            {
                var oldValue = _centerLocation;
                _centerLocation = value;
                if (_centerLocation != oldValue)
                    OnLocationChanged(oldValue);
            }
        }

        protected virtual void OnLocationChanged(Vector2F oldValue)
        {
            //Provider.ResetItems();
        }

        void IShape.Draw(IViewPort viewPort)
        {
            throw new NotImplementedException();
        }

        bool IShape.Contains(Vector2F point)
        {
            throw new NotImplementedException();
        }

        bool IShape.Contains(Bounds2F bounds)
        {
            throw new NotImplementedException();
        }

        bool IShape.IntersectsWith(Bounds2F bounds)
        {
            throw new NotImplementedException();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.GetEnumerator();
        }

        IEnumerator<IShape> IEnumerable<IShape>.GetEnumerator()
        {
            return Provider.GetEnumerator();
        }

        void IShapesProvider.Add(IShape shape)
        {
            Provider.Add(shape);
        }

        void IShapesProvider.Remove(IShape shape)
        {
            Provider.Remove(shape);
        }

        bool IShapesProvider.Contains(IShape shape)
        {
            return Provider.Contains(shape);
        }

        void IShapesProvider.Clear()
        {
            Provider.Clear();
        }

        IShape IShapesProvider.FirstOrDefault(Vector2F point)
        {
            var localPoint = point; // TODO
            return Provider.FirstOrDefault(localPoint);
        }

        IShape IShapesProvider.LastOrDefault(Vector2F point)
        {
            var localPoint = point; // TODO
            return Provider.LastOrDefault(localPoint);
        }

        IEnumerable<IShape> IShapesProvider.Contains(Vector2F point)
        {
            var localPoint = point; // TODO
            return Provider.Contains(localPoint);
        }

        IEnumerable<IShape> IShapesProvider.Contains(Bounds2F bounds)
        {
            var local = bounds; // TODO
            return Provider.Contains(local);
        }

        IEnumerable<IShape> IShapesProvider.IntersectsWith(Bounds2F bounds)
        {
            var local = bounds; // TODO
            return Provider.IntersectsWith(local);
        }

    }
}