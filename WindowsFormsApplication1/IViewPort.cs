using System.Collections.Generic;
using System.Drawing;

namespace WindowsFormsApplication1
{
    public interface IViewPort
    {
        Vector2F Position { get; set; }
        float Zoom { get; }
        //float Rotation { get; }
        Graphics Graphics { get; }
        RectangleF ClipRectangle { get; }

        IEnumerable<Shape> Selection { get; set; }
        IShapesProvider Shapes { get; set; }
    }
}