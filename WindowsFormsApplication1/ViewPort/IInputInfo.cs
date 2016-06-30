using System.Windows.Forms;

namespace Shapes
{
    public interface IInputInfo
    {
        IViewPort ViewPort { get; }
        ViewPortEventType EventType { get; }
        KeysInfo Keys { get; }
        float? Wheel { get; }
        MouseButtons Buttons { get; }
        Vector2F? ClientPoint { get; }
        Vector2F? ViewPortPoint { get; }
        Vector2F? ClientOffset { get; }
        bool Completed { get; set; }
    }

    public interface IInputInfoProcessor
    {
        bool MouseMove(Vector2F clientPoint);
        bool MouseWheel(float wheel);
        bool KeyDown(Keys keyCode);
        bool KeyUp(Keys keyCode);
        bool MouseDown(MouseEventArgs e);
        bool MouseUp(MouseEventArgs e);
        bool MouseMove(MouseEventArgs e);
        bool MouseWheel(MouseEventArgs e);
        void BeforeDrawShape(IShape shape);
        void BeforeDrawShapes();
        void AfterDrawShape(IShape shape);
        void AfterDrawShapes();
    }

    public delegate bool InputInfoChangedEventHandler(InputInfoProcessor infoProcessor);
}