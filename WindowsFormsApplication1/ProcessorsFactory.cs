using System;
using System.Collections.Generic;

namespace Shapes
{
    public static class ProcessorsFactory
    {
        static readonly Dictionary<Type, Func<IViewPort, object>> _createProcessorFuncs = new Dictionary<Type, Func<IViewPort, object>>();
        public static void Register<T>(Func<IViewPort, T> create)
            where T : IInputInfoProcessor
        {
            _createProcessorFuncs[typeof (T)] = vp => create(vp);
        }


        public static T Get<T>(IViewPort viewPort, string caption)
            where T : IInputInfoProcessor
        {
            if (ReferenceEquals(viewPort, null))
                return default(T);

            if (!_createProcessorFuncs.ContainsKey(typeof(T)))
                return default(T);

            var processor = _createProcessorFuncs[typeof(T)](viewPort);
            if (!string.IsNullOrWhiteSpace(caption))
                processor.Tag("Caption", caption);

            return (T)processor;
        }

    }
}