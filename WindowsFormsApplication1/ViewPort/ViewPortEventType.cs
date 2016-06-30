using System;

namespace Shapes
{
    [Flags]
    public enum ViewPortEventType
    {
        None = 0,
        ButtonDown = 1,
        ButtonUp = 2,
        MouseMove = 4, 
        MouseWheel=8,
        KeyDown = 16,
        KeyUp = 32,
        KeysRepeated = 64,
        Reset=128,
        All = ButtonDown | ButtonUp | MouseMove | MouseWheel | KeyDown | KeyUp | KeysRepeated | Reset
    }


    public static class ViewPortEventTypeExtender
    {
        public static ViewPortEventType Invert(this ViewPortEventType eventType)
        {
            return ViewPortEventType.All & ~eventType;
        }
    }
}