namespace Shapes
{
    public interface ISelectableShape : IShape
    {
        bool AllowSelect { get; }
        Bounds2F Bounds { get; }
    }
}