using System;
using System.Windows.Forms;

namespace Shapes
{
    public class InputInfo : IInputInfo
    {
        public IViewPort ViewPort { get; private set; }

        protected internal InputInfo(IViewPort viewPort)
        {
            ViewPort = viewPort;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Keys = new KeysInfo();
        }


        public virtual ViewPortEventType EventType { get; protected internal set; }
        public virtual KeysInfo Keys { get; protected set; }
        public virtual float? Wheel { get; protected internal set; }
        public virtual MouseButtons Buttons { get; protected internal set; }
        public virtual Vector2F? ClientPoint { get; protected internal set; }

        public Vector2F? ViewPortPoint
        {
            get { return ClientPoint.HasValue ? ViewPort.ClientToViewPort(ClientPoint.Value) : default(Vector2F?); }
        }

        public virtual Vector2F? ClientOffset { get; protected internal set; }
        public bool Completed { get; set; }

        protected internal void BeforeChanging()
        {
            Wheel = null;
            ClientOffset = null;

            if (EventType != ViewPortEventType.Reset)
                return;

            Keys.Reset();
            ClientPoint = null;
            //ViewPortPoint = null;
            Buttons = MouseButtons.None;
            EventType = ViewPortEventType.None;

        }


        public override string ToString()
        {
            return string.Format("{{{0}: CP[{1}],B[{2}],K[{3}],CO[{4}],W[{5}]}}", EventType, ClientPoint, Buttons, string.Join(",", Keys), ClientOffset, Wheel);
        }
    }

    public abstract class InputInfoProcessor : IInputInfoProcessor
    {
        public enum ViewPortMode
        {
            None = 0,
            Selection
        }

        public ViewPortMode Mode { get; set; }
        public bool IsSelectionMode => Mode == ViewPortMode.Selection;

        public readonly ActionsRegistrator Actions = new ActionsRegistrator();

        public event InputInfoChangedEventHandler Changed;

        protected readonly InputInfo InputInfo;

        protected InputInfoProcessor(IViewPort viewPort)
        {
            Actions.Tag("Owner", this);
            InputInfo = new InputInfo(viewPort);
        }

        protected bool KeysEmptyAndButonIs(IInputInfo info, MouseButtons button)
        {
            return info.Keys.Empty && info.Buttons == button;
        }

        protected internal bool ButtonsChanged(ViewPortEventType eventType, MouseButtons buttons, Vector2F clientPoint)
        {
            InputInfo.BeforeChanging();

            InputInfo.ClientPoint = clientPoint;
            InputInfo.EventType = eventType;

            if (eventType == ViewPortEventType.ButtonDown)
                InputInfo.Buttons = buttons;

            var result = Process();

            if (eventType == ViewPortEventType.ButtonUp)
                InputInfo.Buttons = buttons;

            return result;
        }

        bool Process()
        {
            var processRegistered = Actions.ProcessRegistered(InputInfo);
            var onChanged = OnChanged();

            return processRegistered || onChanged;
        }

        public bool MouseMove(Vector2F clientPoint)
        {
            InputInfo.BeforeChanging();

            if (clientPoint == InputInfo.ClientPoint)
                return false;

            InputInfo.ClientOffset = clientPoint - InputInfo.ClientPoint;
            InputInfo.ClientPoint = clientPoint;

            InputInfo.EventType = ViewPortEventType.MouseMove;

            return Process();
        }

        public bool MouseWheel(float wheel)
        {
            InputInfo.BeforeChanging();
            InputInfo.Wheel = wheel;
            InputInfo.EventType = ViewPortEventType.MouseWheel;
            return Process();
        }

        //public bool KeysChanged(params Keys[] keys)
        //{
        //    return KeysChanged(keys.AsEnumerable());
        //}

        private bool KeysChanged(ViewPortEventType eventType)
        {
            InputInfo.BeforeChanging();
            InputInfo.EventType = eventType;
            return Process();
        }

        private bool KeysRepeated()
        {
            InputInfo.BeforeChanging();
            InputInfo.EventType = ViewPortEventType.KeysRepeated;
            return Process();
        }

        private bool Reset()
        {
            InputInfo.BeforeChanging();

            InputInfo.EventType = ViewPortEventType.Reset;

            return Process();
        }

        public bool KeyDown(Keys keyCode)
        {
            return InputInfo.Keys.Add(keyCode) 
                ? KeysChanged(ViewPortEventType.KeyDown)
                : KeysRepeated();
        }

        public bool KeyUp(Keys keyCode)
        {
            try
            {
                return InputInfo.Keys.Contains(keyCode) && KeysChanged(ViewPortEventType.KeyUp);
            }
            finally
            {
                InputInfo.Keys.Remove(keyCode);
            }
        }

        public bool MouseDown(MouseEventArgs e)
        {
            return (InputInfo.Buttons | e.Button) != InputInfo.Buttons && ButtonsChanged(ViewPortEventType.ButtonDown, InputInfo.Buttons | e.Button, e.Location);
        }

        public bool MouseUp(MouseEventArgs e)
        {
            return (InputInfo.Buttons | e.Button) == InputInfo.Buttons && ButtonsChanged(ViewPortEventType.ButtonUp, InputInfo.Buttons & ~e.Button, e.Location);
        }

        public bool MouseMove(MouseEventArgs e)
        {
            return MouseMove(e.Location);
        }

        public bool MouseWheel(MouseEventArgs e)
        {
            var sign = Math.Abs(e.Delta / 120) * Math.Sign(e.Delta);
            return MouseWheel(sign);
        }

        public virtual void BeforeDrawShape(IShape shape)
        {

        }

        public virtual void BeforeDrawShapes()
        {

        }

        public virtual void AfterDrawShape(IShape shape)
        {
            
        }

        public virtual void AfterDrawShapes()
        {
            
        }

        protected virtual bool OnChanged()
        {
            var handler = Changed;
            return handler != null && handler(this);
        }
    }

    public static class InputInfoExtender
    {
        public static IShape Shape(this IInputInfo x)
        {
            if (!x.ViewPortPoint.HasValue)
                return null;

            var point = x.ViewPortPoint.Value;
            var shape = x.ViewPort.Shapes.LastOrDefault(point);
            return shape;
        }
    }
}