namespace Shapes
{
    public class MouseOverInputInfoProcessor : InputInfoProcessor
    {
        private IShape _shape;

        public static MouseOverInputInfoProcessor New(IViewPort viewPort, string caption = null)
        {
            var processor = new MouseOverInputInfoProcessor(viewPort);
            if (!string.IsNullOrWhiteSpace(caption))
                processor.Tag("Caption", caption);
            return processor;
        }

        private bool IsEnterShape(IInputInfo info)
        {
            return _shape == null && info.Shape() != null;
        }

        private bool IsLeaveShape(IInputInfo info)
        {
            return _shape != info.Shape();
        }

        private bool EnterShape(IInputInfo info)
        {
            _shape = info.Shape();
            var result = _shape != null;

            if (result)
            {
                var processor = _shape as IMouseEnterProcessor;
                processor?.Process();
            }

            return result;
        }

        private bool LeaveShape(IInputInfo info)
        {
            var processor = _shape as IMouseLeaveProcessor;
            processor?.Process();

            _shape = null;

            return true;
        }

        public MouseOverInputInfoProcessor(IViewPort viewPort) : base(viewPort)
        {
            Actions.OnMouseMove(IsEnterShape, ViewPortEventType.None, IsLeaveShape, EnterShape, LeaveShape)
                .Tag("Caption", "шейп под мышкой");
        }
    }
}