using System.Collections.Generic;
using System.Drawing;

namespace Shapes
{
    public interface IViewPort
    {
        Vector2F Position { get; set; }
        float Zoom { get; }
        //float Rotation { get; }
        Graphics Graphics { get; }
        RectangleF ClipRectangle { get; }

        IShapesProvider Shapes { get; set; }
        bool Focused { get; }

        Vector2F ClientToViewPort(Vector2F point);
        Vector2F ViewPortToClient(Vector2F point);
        void ChangeZoom(float wheel, Vector2F? zoomPoint = default (Vector2F?));
        void Invalidate();
        Bounds2F ViewPortToClientRectangle(Vector2F from, Vector2F to);
    }



}