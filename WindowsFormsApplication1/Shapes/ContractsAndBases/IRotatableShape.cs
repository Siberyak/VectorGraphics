namespace Shapes
{
    public interface IRotatableShape : IShape
    {
        float Rotation { get; }
        bool AllowRotate { get; set; }
        void Rotate(float angle);
    }
}