using System.Security.Cryptography.X509Certificates;

namespace Shapes
{
    public interface IMovableShape : IShape
    {
        Vector2F Offset { get; set; }
        bool AllowMove { get; }

        void ApplyMove();
    }
}