using System;
using Shapes;

namespace TestApplication
{
    public static class Extender
    {
        public static Func<IShape, bool> Next(this Func<IShape, bool> current, Func<IShape, bool> next)
        {
            return current == null
                ? next
                : (next == null ? current : x => current(x) && next(x));
        }
    }
}