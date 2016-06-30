namespace Shapes
{
    public interface IScalableShape : IShape
    {
        bool AllowScale { get; }
        bool KeepAspectRatio { get; }
        Vector2F AspectRatio { get; set; }
        void Zoom(float zoom);
        void Scale(Vector2F scale);
    }
}