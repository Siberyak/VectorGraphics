using System.Security.Cryptography.X509Certificates;

namespace Shapes
{
    public interface IMovableShape : IShape
    {
        Vector2F Offset { get; set; }
        bool AllowMove { get; }

        //void ApplyMove();
        //void CancelMove();

        event LocationChangedEventHandler LocationChanged;
        event OffsetChangedEventHandler OffsetChanged;
    }

    public delegate void LocationChangedEventHandler(IMovableShape shape, Vector2F oldLocation);
    public delegate void OffsetChangedEventHandler(IMovableShape shape, Vector2F oldOffset);
}