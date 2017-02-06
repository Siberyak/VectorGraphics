using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Shapes
{
    public abstract class PathedShape : SizedShape, ISelectableShape, IMovableShape
    {
        private Pen _pen = Pens.Black;
        private Vector2F _offset;

        public Pen Pen
        {
            get { return _pen; }
            set { _pen = value; }
        }

        public bool IsSolid { get; }

        protected PathedShape(Vector2F centerLocation, Vector2F size, bool isSolid)
            : base(centerLocation, size)
        {
            IsSolid = isSolid;
        }

        protected PathedShape(Vector2F size, bool isSolid) : this(Vector2F.Zerro,  size, isSolid)
        {}

        protected override Bounds2F InitBounds(out Bounds2F selfBounds)
        {
            var path = GetPath(this, out selfBounds);
            return path.GetBounds();
        }

        protected GraphicsPath GetPath(IShape x)
        {
            Bounds2F selfBounds;
            return GetPath(x, out selfBounds);
        }

        protected GraphicsPath GetPath(IShape x, out Bounds2F selfBounds)
        {
            var shape = (SizedShape)x;
            var location = shape.CenterLocation;
            var size = shape.Size();
            var rotation = shape.Rotation();

            selfBounds = new Bounds2F(location, size);
            selfBounds.Offset(-size / 2);

            var path = new GraphicsPath();

            GetPathFigures(path, selfBounds);

            if (Math.Abs(rotation) > float.Epsilon)
            {
                var matrix = new Matrix();
                matrix.RotateAt(rotation, location);

                path.Transform(matrix);
            }

            return path;
        }

        protected abstract void GetPathFigures(GraphicsPath path, Bounds2F selfBounds);

        protected override IEnumerable<DrawingItem> GetItems()
        {
            return new[] { new GraphicsPathDrawItem(this, _pen, GetPath) };
        }


        private Vector2F Translate(Vector2F point)
        {
            var relativePoint = (point - CenterLocation).RotateDegrees(-Rotation);
            return relativePoint + CenterLocation;
        }

        public override bool Contains(Vector2F point)
        {
            var relativePoint = (point - CenterLocation).RotateDegrees(-Rotation);
            var bounds = new Bounds2F(-Size / 2, Size);

            var contains = bounds.Contains(relativePoint);
            return contains && ContainsByPath(point);
        }

        private bool ContainsByPath(Vector2F point)
        {
            GraphicsPath path = GetPath(this);
            return ContainsByPath(path, point);
        }

        protected virtual bool ContainsByPath(GraphicsPath path, Vector2F point)
        {
            
            return IsSolid
                ? path.IsVisible(point)
                : path.IsOutlineVisible(point, Pen);
        }

        public virtual bool AllowSelect { get; set; } = true;

        public override Vector2F CenterLocation
        {
            get { return base.CenterLocation + Offset; }
            set { base.CenterLocation = (value - Offset); }
        }

        public override Bounds2F Bounds { get { return base.Bounds + Offset; } }

        public Vector2F Offset
        {
            get { return _offset; }
            set
            {
                if (_offset == value)
                    return;
                _offset = value;
                ResetItems();
            }
        }

        public bool AllowMove { get; set; }
        public void ApplyMove()
        {
            var offset = Offset;
            Offset = Vector2F.Zerro;
            base.CenterLocation += offset;
        }

        public event LocationChangedEventHandler LocationChanged;
        public event OffsetChangedEventHandler OffsetChanged;

        public void Su(OffsetChangedEventHandler eh)
        {
            throw new NotImplementedException();
        }

        public void Su(LocationChangedEventHandler eh)
        {
            throw new NotImplementedException();
        }

        public void Unsu(OffsetChangedEventHandler eh)
        {
            throw new NotImplementedException();
        }

        public void Unsu(LocationChangedEventHandler eh)
        {
            throw new NotImplementedException();
        }


        protected override void OnLocationChanged(Vector2F oldValue)
        {
            LocationChanged?.Invoke(this, oldValue);

            base.OnLocationChanged(oldValue);
        }

        protected override void OnRotationChanged(float oldValue)
        {
            base.OnRotationChanged(oldValue);
        }

        public override bool Contains(Bounds2F bounds)
        {
            if (IsSolid)
                return base.Contains(bounds);

            var region = new Region(GetPath(this));
            var clone = region.Clone();
            clone.Union(bounds);
            return clone.Equals(region);
        }

        protected static Pen _onePixelPen = new Pen(Color.Empty, 0.001f);
        public override bool IntersectsWith(Bounds2F bounds)
        {
            if (IsSolid)
                return base.IntersectsWith(bounds);

            return new Region(GetPath(this)).IsVisible(bounds);
        }
    }

}