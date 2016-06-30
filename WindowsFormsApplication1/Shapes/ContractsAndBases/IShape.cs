namespace Shapes
{
    public interface IShape
    {
        Vector2F Location { get; }
        void Draw(IViewPort viewPort);

        bool Contains(Vector2F point);
        bool Contains(Bounds2F bounds);
        bool IntersectsWith(Bounds2F bounds);

    }

    // ReSharper disable once InconsistentNaming
    public static class IShapeExtender
    {
        public static float Rotation(this IShape shape)
        {
            var sh = shape as IRotatableShape;
            return sh?.Rotation ?? 0f;
        }

        public static Vector2F AspectRatio(this IShape shape)
        {
            var sh = shape as IScalableShape;
            return sh?.AspectRatio ?? Vector2F.Unit;
        }

        public static Bounds2F Bounds(this IShape shape)
        {
            var sh = shape as ISizedShape;
            return sh?.Bounds ?? Bounds2F.Empty;
        }

        public static Vector2F Size(this IShape shape)
        {
            var sizable = shape as ISizedShape;
            var aspectRatio = shape.AspectRatio();
            return sizable?.Size.Aspect(aspectRatio) ?? Vector2F.Zerro;
        }

        public static bool AllowSelect(this IShape shape)
        {
            var sh = shape as ISelectableShape;
            return sh?.AllowSelect == true;
        }

        public static bool AllowFocus(this IShape shape)
        {
            var sh = shape as IFocusableShape;
            return sh?.AllowFocus == true;
        }

    }
}