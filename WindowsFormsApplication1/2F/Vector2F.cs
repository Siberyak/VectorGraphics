using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Shapes
{
    public struct Vector2F
    {
        public static readonly Vector2F Zerro = new Vector2F();
        public static readonly Vector2F Unit = new Vector2F(1,1);

        public Vector2F(float x, float y)
            : this()
        {
            X = x;
            Y = y;
        }

        public float X { get; private set; }
        public float Y { get; private set; }

        private float? _length;
        public float Length => (_length ?? (_length = Convert.ToSingle(Math.Sqrt(X*X + Y*Y)))).Value;

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", X, Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2F))
                return false;
            var vector = (Vector2F)obj;
            return this == vector;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vector2F a, Vector2F b)
        {
            return Math.Abs(a.X - b.X) < Double.Epsilon && Math.Abs(a.Y - b.Y) < float.Epsilon;
        }

        public static bool operator !=(Vector2F a, Vector2F b)
        {
            return !(a == b);
        }

        public static Vector2F operator +(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2F operator -(Vector2F a, Vector2F b)
        {
            return new Vector2F(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2F operator -(Vector2F a)
        {
            return new Vector2F(-a.X, -a.Y);
        }

        public static Vector2F operator *(float k, Vector2F a)
        {
            return a * k;
        }

        public static Vector2F operator *(Vector2F a, float k)
        {
            return new Vector2F(a.X * k, a.Y * k);
        }

        public static Vector2F operator /(Vector2F a, float k)
        {
            return a * (1/k);
        }

        public static implicit operator Vector2F(Size size)
        {
            return new Vector2F(size.Width, size.Height);
        }
        public static implicit operator Vector2F(SizeF size)
        {
            return new Vector2F(size.Width, size.Height);
        }

        public static implicit operator Vector2F(Point point)
        {
            return new Vector2F(point.X, point.Y);
        }
        public static implicit operator Vector2F(PointF point)
        {
            return new Vector2F(point.X, point.Y);
        }

        public static implicit operator SizeF(Vector2F vector)
        {
            return new SizeF(vector.X, vector.Y);
        }

        public static implicit operator PointF(Vector2F vector)
        {
            return new PointF(vector.X, vector.Y);
        }
    }

    public static class BoundsExtender
    {
    }

    public static class VectorExtender
    {
        private const float DegToRad = (float)(Math.PI / 180);

        public static Vector2F RotateDegrees(this Vector2F v, float degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector2F RotateRadians(this Vector2F v, float radians)
        {
            var ca = (float)Math.Cos(radians);
            var sa = (float)Math.Sin(radians);
            return new Vector2F(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }

        public static Vector2F Aspect(this Vector2F vector2F, Vector2F aspectRatio)
        {
            return new Vector2F(vector2F.X * aspectRatio.X, vector2F.Y * aspectRatio.Y);
        }

    }
}