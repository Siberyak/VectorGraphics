using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Shapes
{
    public class InputInfoProcessorsCollection : IInputInfoProcessor
    {
        readonly List<IInputInfoProcessor> _processors = new List<IInputInfoProcessor>();

        public InputInfoProcessorsCollection(params IInputInfoProcessor[] processors)
        {
            _processors.AddRange(processors.Where(x => !ReferenceEquals(x, null)).GroupBy(x => x).Select(x => x.Key));
        }

        public bool Add(IInputInfoProcessor processor)
        {
            if (ReferenceEquals(processor, null))
                return false;

            lock (_processors)
            {
                var contains = _processors.Contains(processor);
                if (!contains)
                    _processors.Add(processor);

                return !contains;
            }
        }
        public bool Remove(IInputInfoProcessor processor)
        {
            if (ReferenceEquals(processor, null))
                return false;

            lock (_processors)
            {
                var contains = _processors.Contains(processor);
                if (contains)
                    _processors.Add(processor);

                return contains;
            }
        }

        bool Process(Func<IInputInfoProcessor, bool> func)
        {
            lock (_processors)
            {
                return _processors.Select(func).ToArray().Any();
            }
        }

        void Process(Action<IInputInfoProcessor> action)
        {
            lock (_processors)
            {
                foreach (var processor in _processors)
                    action(processor);
            }
        }

        public bool MouseMove(Vector2F clientPoint)
        {
            return Process(x => x.MouseMove(clientPoint));
        }

        public bool MouseWheel(float wheel)
        {
            return Process(x => x.MouseWheel(wheel));
        }

        public bool KeyDown(Keys keyCode)
        {
            return Process(x => x.KeyDown(keyCode));
        }

        public bool KeyUp(Keys keyCode)
        {
            return Process(x => x.KeyUp(keyCode));
        }

        public bool MouseDown(MouseEventArgs e)
        {
            return Process(x => x.MouseDown(e));
        }

        public bool MouseUp(MouseEventArgs e)
        {
            return Process(x => x.MouseUp(e));
        }

        public bool MouseMove(MouseEventArgs e)
        {
            return Process(x => x.MouseMove(e));
        }

        public bool MouseWheel(MouseEventArgs e)
        {
            return Process(x => x.MouseWheel(e));
        }

        public void BeforeDrawShape(IShape shape)
        {
            Process(x => x.BeforeDrawShape(shape));
        }

        public void BeforeDrawShapes()
        {
            Process(x => x.BeforeDrawShapes());
        }

        public void AfterDrawShape(IShape shape)
        {
            Process(x => x.AfterDrawShape(shape));
        }

        public void AfterDrawShapes()
        {
            Process(x => x.AfterDrawShapes());
        }
    }
}