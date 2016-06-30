using System;
using System.Drawing;
using System.Globalization;

namespace Shapes
{
    public struct Bounds2F
    {
        public static readonly Bounds2F Empty = new Bounds2F();

        public Vector2F Location { get; private set; }
        public Vector2F Size { get; private set; }

        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }

        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }
        public Vector2F BottomRight { get { return Location + Size; } }
        public Vector2F BottomLeft { get { return Location + new Vector2F(0, Height); } }
        public Vector2F TopRight { get { return Location + new Vector2F(Width, 0); } }
        public Vector2F TopLeft { get { return Location; } }
        public Vector2F Center { get { return Location + Size/2; } }

        public Bounds2F(Vector2F location, Vector2F size) : this()
        {
            var offset = new Vector2F(size.X < 0 ? size.X : 0, size.Y < 0 ? size.Y : 0);
            Location = location + offset;
            Size = new Vector2F(Math.Abs(size.X), Math.Abs(size.Y));
        }

        private Bounds2F(float x, float y, float width, float height) : this(new Vector2F(x,y), new Vector2F(width, height))
        {
        }

        public static implicit operator RectangleF(Bounds2F bounds)
        {
            return new RectangleF(bounds.Location, bounds.Size);
        }

        public static implicit operator Bounds2F(RectangleF rectangle)
        {
            return new Bounds2F(rectangle.Location, rectangle.Size);
        }

        public static explicit operator Bounds2F(Vector2F point)
        {
            return new Bounds2F(point, Vector2F.Zerro);
        }

        public static bool operator ==(Bounds2F a, Bounds2F b)
        {
            return a.Location == b.Location && a.Size == b.Size;
        }

        public static bool operator !=(Bounds2F a, Bounds2F b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bounds2F))
                return false;
            var bounds = (Bounds2F)obj;
            return this == bounds;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Contains(float x, float y)
        {
            return (X <= x && x < X + Width && Y <= y) && (y < Y + Height);
        }

        public bool Contains(Vector2F point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(Bounds2F bounds)
        {
            return (X <= bounds.X && bounds.X + bounds.Width <= X + Width && Y <= bounds.Y) && (bounds.Y + bounds.Height <= Y + Height);
        }

        public void Inflate(float x, float y)
        {
            Location -= new Vector2F(x,y);
            Size += new Vector2F(2*x, 2*y);
        }

        public void Inflate(Vector2F size)
        {
            Inflate(size.X, size.Y);
        }

        public static Bounds2F Inflate(Bounds2F bounds, float x, float y)
        {
            var result = bounds;
            result.Inflate(x, y);
            return result;
        }

        public void Intersect(Bounds2F bounds)
        {
            var result = Intersect(bounds, this);
            Location = result.Location;
            Size = result.Size;
        }

        public static Bounds2F Intersect(Bounds2F a, Bounds2F b)
        {
            var x = Math.Max(a.X, b.X);
            var num1 = Math.Min(a.X + a.Width, b.X + b.Width);
            var y = Math.Max(a.Y, b.Y);
            var num2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num1 >= x && num2 >= y)
                return new Bounds2F(x, y, num1 - x, num2 - y);
            return Empty;
        }

        public bool IntersectsWith(Bounds2F bounds)
        {
            return (bounds.X < X + Width && X < bounds.X + bounds.Width && bounds.Y < Y + Height) && (Y < bounds.Y + bounds.Height);
        }

        public static Bounds2F Union(Bounds2F a, Bounds2F b)
        {
            var x = Math.Min(a.X, b.X);
            var num1 = Math.Max(a.X + a.Width, b.X + b.Width);
            var y = Math.Min(a.Y, b.Y);
            var num2 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new Bounds2F(x, y, num1 - x, num2 - y);
        }

        public void Offset(Vector2F pos)
        {
            Location += pos;
        }

        public void Offset(float x, float y)
        {
            Offset(new Vector2F(x,y));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0},Y={1},Width={2},Height={3}}}", X, Y, Width, Height);
        }

        public static Bounds2F operator +(Bounds2F bounds, Vector2F offset)
        {
            return  new Bounds2F(bounds.Location + offset, bounds.Size);
        }
        public static Bounds2F operator -(Bounds2F bounds, Vector2F offset)
        {
            return bounds + (-offset);
        }
    }
}