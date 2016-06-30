using System.Drawing;

namespace Shapes
{
    public static class Extender
    {
        public static RectangleShape AddRectangle(this IShapesProvider provider, Vector2F location, Vector2F size, bool isSolid = true, float rotation = 0)
        {
            var shape = new RectangleShape(location, size, isSolid) { Rotation = rotation };
            provider.Add(shape);
            return shape;
        }

        public static RectangleShape AddRectangle(this IShapesProvider provider, Vector2F location, Vector2F size, Pen pen, bool isSolid = true, float rotation = 0)
        {
            var shape = provider.AddRectangle(location, size, isSolid, rotation);
            shape.Pen = pen;
            return shape;
        }

        public static EllipseShape AddEllipse(this IShapesProvider provider, Vector2F location, Vector2F size, bool isSolid = true, float rotation = 0)
        {
            var shape = new EllipseShape(location, size, isSolid) { Rotation = rotation };
            provider.Add(shape);
            return shape;
        }

        public static EllipseShape AddEllipse(this IShapesProvider provider, Vector2F location, Vector2F size, Pen pen, bool isSolid = true, float rotation = 0)
        {
            var shape = provider.AddEllipse(location, size, isSolid, rotation);
            shape.Pen = pen;
            return shape;
        }

    }
}