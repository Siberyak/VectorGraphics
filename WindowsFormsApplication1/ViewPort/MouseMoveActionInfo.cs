using System.Linq;

namespace Shapes
{
    public class MouseMoveActionInfo
    {
        public Vector2F From;
        public Vector2F To;
        public bool Flag;


        public bool Start(IInputInfo info)
        {
            if (Flag || !info.ClientPoint.HasValue)
                return false;

            if (!info.ViewPortPoint.HasValue)
                return false;

            Flag = true;
            From = info.ViewPortPoint.Value;
            To = From;

            foreach (var shape in info.ViewPort.Shapes.OfType<Shape>())
                shape.SuspendTimer();

            return true;

        }

        public bool Process(IInputInfo info)
        {
            if (!info.ViewPortPoint.HasValue)
                return false;

            To = info.ViewPortPoint.Value;

            if (!Flag || info.EventType != ViewPortEventType.MouseMove)
                return false;

            Offset = To - From;

            return true;
        }

        public Vector2F Offset { get; private set; }

        public bool Stop(IInputInfo info)
        {
            Flag = false;
            foreach (var shape in info.ViewPort.Shapes.OfType<Shape>())
                shape.ResumeTimer();

            return true;
        }
    }
}