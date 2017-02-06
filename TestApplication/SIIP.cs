using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Shapes;

namespace TestApplication
{
    public class SIIP : SelectionInputInfoProcessor
    {
        private readonly MouseMoveActionInfo _moveInfo = new MouseMoveActionInfo();

        protected SIIP(IViewPort viewPort) : base(viewPort)
        {
            // перемещение мышью
            Actions.OnMouseMove(CanMove, activate: StartMove, deactivate: StopMove, process: ProcessMove, deactivateEventTypes: ViewPortEventType.ButtonUp)
                .Tag("Caption", "перемещение мышью");
        }

        private bool StartMove(IInputInfo info)
        {
            if(!_moveInfo.Start(info))
                return false;

            return true;
        }
        private bool ProcessMove(IInputInfo info)
        {
            if (!_moveInfo.Process(info))
                return false;

            if (_moveInfo.Offset.Length < 3)
                return true;

            foreach (var shape in Selection.OfType<IMovableShape>())
            {
                shape.Offset = _moveInfo.Offset;
            }

            return true;
        }
        private bool StopMove(IInputInfo info)
        {
            foreach (var shape in Selection.OfType<IMovableShape>())
            {
                //shape.ApplyMove();
            }

            return _moveInfo.Stop(info);
        }

        private bool CanMove(IInputInfo x)
        {
            return IsSelectionMode && (_moveInfo.Flag || Selection.Contains(x.Shape())) && x.Keys == Keys.None && x.Buttons == MouseButtons.Left;
        }

        public new static SIIP New(IViewPort viewPort, ViewPortMode mode = ViewPortMode.Selection, string caption = null)
        {
            var processor = new SIIP(viewPort) { Mode = mode };
            if (!string.IsNullOrWhiteSpace(caption))
                processor.Tag("Caption", caption);
            return processor;
        }

        private static SolidBrush _focusedBrush;
        private static SolidBrush _selectedBrush;
        private static SolidBrush _selectingBrush;
        private static SolidBrush _bothBrush;
        private static SolidBrush FocusedBrush => _focusedBrush ?? (_focusedBrush = new SolidBrush(FocusedPen.Color));
        private static SolidBrush SelectedBrush => _selectedBrush ?? (_selectedBrush = new SolidBrush(SelectionPen.Color));
        private static SolidBrush BothBrush => _bothBrush ?? (_bothBrush = new SolidBrush(Color.DarkOliveGreen));
        private static SolidBrush SelectingBrush => _selectingBrush ?? (_selectingBrush = new SolidBrush(SelectingPen.Color));



        protected override void DrawFocused(IShape shape)
        {
            FillShape(shape, FocusedBrush, -2);
        }

        protected override void DrawSelected(IShape shape)
        {
            FillShape(shape, SelectedBrush, -2);
        }

        protected override void DrawSelectedAndAdded(IShape shape)
        {
            FillShape(shape, BothBrush, -2);
        }

        protected override void DrawAddingSelected(IShape shape)
        {
            FillShape(shape, SelectingBrush, -2);
        }

        private void FillShape(IShape shape, SolidBrush brush, int inflate)
        {
            var path = GetShapesBounsPath(inflate, new[] {shape});
            InputInfo.ViewPort.Graphics.FillPath(brush, path);
        }
    }
}