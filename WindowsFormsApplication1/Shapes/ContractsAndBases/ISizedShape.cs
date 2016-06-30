namespace Shapes
{
    public interface ISizedShape : IShape
    {
        Vector2F Size { get; }
        Bounds2F Bounds { get; }
    }
}