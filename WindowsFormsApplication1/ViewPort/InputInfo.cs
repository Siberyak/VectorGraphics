using System;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class InputInfo : IInputInfo, IInputInfoProcessor
    {
        public ViewPortEventType EventType { get; private set; }
        public KeysInfo Keys { get; private set; }
        public float? Wheel { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public Vector2F? ClientPoint { get; private set; }
        public Vector2F? ClientOffset { get; private set; }

        public event InputInfoChangedEventHandler Changed;

        public InputInfo()
        {
            Keys = new KeysInfo();
        }

        void BeforeChanging()
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

        bool Process()
        {
            var processRegistered = Actions.ProcessRegistered(this);
            var onChanged = OnChanged();

            return processRegistered || onChanged;
        }

        

        public override string ToString()
        {
            return string.Format("{{{0}: CP[{1}],B[{2}],K[{3}],CO[{4}],W[{5}]}}", EventType, ClientPoint, Buttons, string.Join(",", Keys), ClientOffset, Wheel);
        }

        private bool ButtonsChanged(ViewPortEventType eventType, MouseButtons buttons, Vector2F clientPoint)
        {
            BeforeChanging();

            if (eventType == ViewPortEventType.ButtonDown)
                Buttons = buttons;
            ClientPoint = clientPoint;
            EventType = eventType;

            var result = Process();

            if (eventType == ViewPortEventType.ButtonUp)
                Buttons = buttons;

            return result;
        }


        public bool MouseMove(Vector2F clientPoint)
        {
            BeforeChanging();

            if(clientPoint == ClientPoint)
                return false;

            ClientOffset = clientPoint - ClientPoint;
            ClientPoint = clientPoint;

            EventType = ViewPortEventType.MouseMove;

            return Process();
        }

        public bool MouseWheel(float wheel)
        {
            BeforeChanging();
            Wheel = wheel;
            EventType = ViewPortEventType.MouseWheel;
            return Process();
        }

        //public bool KeysChanged(params Keys[] keys)
        //{
        //    return KeysChanged(keys.AsEnumerable());
        //}

        private bool KeysChanged(ViewPortEventType eventType)
        {
            BeforeChanging();
            EventType = eventType;
            return Process();
        }

        private bool KeysRepeated()
        {
            BeforeChanging(); 
            EventType = ViewPortEventType.KeysRepeated;
            return Process();
        }

        private bool Reset()
        {
            BeforeChanging(); 
            
            EventType = ViewPortEventType.Reset;

            return Process();
        }

        public bool KeyDown(Keys keyCode)
        {
            return Keys.Add(keyCode) 
                ? KeysChanged(ViewPortEventType.KeyDown)
                : KeysRepeated();
        }

        public bool KeyUp(Keys keyCode)
        {
            try
            {
                return Keys.Contains(keyCode) && KeysChanged(ViewPortEventType.KeyUp);
            }
            finally
            {
                Keys.Remove(keyCode);
            }
        }

        public bool MouseDown(MouseEventArgs e)
        {
            return (Buttons | e.Button) != Buttons && ButtonsChanged(ViewPortEventType.ButtonDown, Buttons | e.Button, e.Location);
        }

        public bool MouseUp(MouseEventArgs e)
        {
            return (Buttons | e.Button) == Buttons && ButtonsChanged(ViewPortEventType.ButtonUp, Buttons & ~e.Button, e.Location);
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

        protected virtual bool OnChanged()
        {
            var handler = Changed;
            return handler != null && handler(this);
        }

        public readonly ActionsRegistrator Actions = new ActionsRegistrator();
    }
}