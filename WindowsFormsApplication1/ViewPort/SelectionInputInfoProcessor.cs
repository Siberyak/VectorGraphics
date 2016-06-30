using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Shapes
{
    public class SelectionInfo
    {
        public IEnumerable<IShape> Selection { get; }
        public IEnumerable<IShape> Selecting { get; }
        public IEnumerable<IShape> Both { get; }

        public SelectionInfo(IEnumerable<IShape> selection = null, IEnumerable<IShape> selecting = null)
        {
            Selection = (selection ?? Enumerable.Empty<IShape>()).ToArray();
            Selecting = (selecting ?? Enumerable.Empty<IShape>()).ToArray();
            Both = Selection.Intersect(Selecting).ToArray();
        }

    }

    public class SelectionInputInfoProcessor : InputInfoProcessor
    {
        public static SelectionInputInfoProcessor New(IViewPort viewPort, ViewPortMode mode = ViewPortMode.Selection, string caption = null)
        {
            var processor = new SelectionInputInfoProcessor(viewPort) {Mode = mode};
            if (!string.IsNullOrWhiteSpace(caption))
                processor.Tag("Caption", caption);
            return processor;
        }

        protected static readonly Pen FocusedPen = new Pen(Color.DeepSkyBlue) { DashStyle = DashStyle.Dot, Width = 2 };
        protected static readonly Pen SelectionPen = new Pen(Color.LightSlateGray) { DashStyle = DashStyle.Dot };
        protected static readonly Pen SelectingPen = new Pen(Color.OrangeRed) { DashStyle = DashStyle.Dot };


        private readonly MouseMoveActionInfo _selectInfo = new MouseMoveActionInfo();
        

        //protected Vector2F From;
        //protected Vector2F To;
        //private bool _selecting;

        public IShape FocusedShape { get; set; }

        protected SelectionInfo _selection = new SelectionInfo();
        public IEnumerable<IShape> Selection
        {
            get { return _selection.Selection; }
            set
            {
                var selection = (value ?? Enumerable.Empty<IShape>()).Where(x => x.AllowSelect()).ToArray();

                if (_selection == null || (!_selection.Selection.Any() && !selection.Any()))
                    return;

                _selection = new SelectionInfo(selection, AddSelection);

                InputInfo.ViewPort.Invalidate();

            }
        }
        public IEnumerable<IShape> AddSelection
        {
            get { return _selection.Selecting; }
            set
            {
                _selection = new SelectionInfo(Selection, (value ?? Enumerable.Empty<IShape>()).Where(x => x.AllowSelect()));
            }
        }


        protected SelectionInputInfoProcessor(IViewPort viewPort)
            : base(viewPort)
        {
            // выделение рамкой
            Actions.OnMouseMove(CanSelectRegion, activate: StartSelection, deactivate: EndSelection, process: ProcessSelection, deactivateEventTypes: ViewPortEventType.ButtonUp)
                .Tag("Caption", "выделение рамкой");

            // добавление к выделению рамкой
            Actions.OnMouseMove(CanAddSelectRegion, deactivateCondition: DontCanAddSelectRegion, deactivateEventTypes: (ViewPortEventType.MouseMove | ViewPortEventType.KeysRepeated).Invert(), activate: StartSelection, deactivate: EndSelection, process: ProcessSelection)
                .Tag("Caption", "добавление к выделению рамкой");

            // выделение всего
            Actions.OnKeyDown(x => IsSelectionMode && x.Keys.Is(Keys.ControlKey, Keys.A), process: SelectAll)
                .Tag("Caption", "выделение всего");

            // клик в шейп
            Actions.OnButtonDown(IsShapeClick, process: ClickShape)
                .Tag("Caption", "клик в шейп");

            // клик мимо шейпа
            Actions.OnButtonDown(IsNotShapeClick, process: ClickNotShape)
                .Tag("Caption", "клик мимо шейпа");

            // ctrl+клик в шейп
            Actions.OnButtonDown(IsShapeCtrlClick, process: CtrlClickShape)
                .Tag("Caption", "выделение под мышкой");

            

        }

        public override void AfterDrawShape(IShape shape)
        {
            base.AfterDrawShape(shape);

            //if (_selection.Selecting.Contains(shape))
            //    DrawAddingSelected(shape);

            //if (_selection.Selection.Contains(shape))
            //    DrawSelected(shape);

            //if (_selection.Both.Contains(shape))
            //    DrawSelectedAndAdded(shape);

            if (FocusedShape == shape)
                DrawFocused(shape);
        }

        protected virtual void DrawSelectedAndAdded(IShape shape)
        {
            
        }

        protected virtual void DrawAddingSelected(IShape shape)
        {
            
        }

        protected virtual void DrawFocused(IShape shape)
        {

        }

        protected virtual void DrawSelected(IShape shape)
        {

        }

        public override void AfterDrawShapes()
        {
            base.AfterDrawShapes();

            DrawSelectedShapesBounds();
            DrawSelectionRegion();
        }

        protected virtual void DrawSelectedShapesBounds()
        {
            DrawShapesBounds(SelectionPen, 3f, Selection);

            DrawShapesBounds(SelectingPen, 2f, AddSelection);


            if (FocusedShape != null)
                DrawShapesBounds(FocusedPen, 5f, new []{ FocusedShape });
        }

        protected virtual void DrawShapesBounds(Pen pen, float inflate, IEnumerable<IShape> shapes)
        {
            var path = GetShapesBounsPath(inflate, shapes);
            InputInfo.ViewPort.Graphics.DrawPath(pen, path);
        }

        protected GraphicsPath GetShapesBounsPath(float inflate, IEnumerable<IShape> shapes)
        {
            var path = new GraphicsPath();
            foreach (var shape in shapes)
            {
                AddShapeBoundsPath(path, shape, inflate/InputInfo.ViewPort.Zoom);
            }

            if (Math.Abs(InputInfo.ViewPort.Zoom - 1) > float.Epsilon)
            {
                var matrinx = new Matrix();
                matrinx.Scale(InputInfo.ViewPort.Zoom, InputInfo.ViewPort.Zoom);

                path.Transform(matrinx);
            }
            return path;
        }

        protected void AddShapeBoundsPath(GraphicsPath path, IShape shape, float inflate)
        {
            var size = shape.Size();
            var location = shape.Location;

            var rotation = shape.Rotation();

            var matrix = Math.Abs(rotation) > float.Epsilon ? new Matrix() : null;
            if (matrix != null)
            {
                matrix.RotateAt(-rotation, location);
                path.Transform(matrix);
            }

            var bounds2F = new Bounds2F(location - size / 2, size);
            bounds2F.Inflate(inflate, inflate);

            path.AddRectangles(new RectangleF[] { bounds2F });

            if (matrix == null)
                return;

            matrix.Invert();
            path.Transform(matrix);
        }

        private void DrawSelectionRegion()
        {
            if (!_selectInfo.Flag)
                return;

            var rectangle = InputInfo.ViewPort.ViewPortToClientRectangle(_selectInfo.From, _selectInfo.To);
            
            //Console.WriteLine($"DrawSelectionRegion: {InputInfo.ViewPort.ViewPortToClient(From)}, {InputInfo.ViewPort.ViewPortToClient(To)} -> {rectangle}");

            InputInfo.ViewPort.Graphics.DrawRectangles(SelectionPen, new RectangleF[] { rectangle });
        }

        private bool CanSelectRegion(IInputInfo x)
        {
            return IsSelectionMode && x.Keys == Keys.ShiftKey && x.Buttons == MouseButtons.Left;
        }

        private bool CanAddSelectRegion(IInputInfo info)
        {
            return IsSelectionMode && CheckPointAndKeysAndButtonAndShape(info, MouseButtons.Left, k => k == Keys.ControlKey);
        }

        private bool DontCanAddSelectRegion(IInputInfo info)
        {
            return !CheckPointAndKeysAndButtonAndShape(info, MouseButtons.Left, k => k == Keys.ControlKey);
        }

        private bool StartSelection(IInputInfo info)
        {
            if (!_selectInfo.Start(info))
                return false;

            //if (_selectActionInfo.Flag || !info.ClientPoint.HasValue)
            //    return false;


            //if (!info.ViewPortPoint.HasValue)
            //    return false;

            //_selectActionInfo.Flag = true;

            //_selectActionInfo.From = info.ViewPortPoint.Value;
            //_selectActionInfo.To = _selectActionInfo.From;

            //foreach (var shape in info.ViewPort.Shapes)
            //    shape.SuspendTimer();


            AddSelection = null;

            if (info.Keys == Keys.ShiftKey)
                ClearSelection();

            return true;
        }

        private bool ProcessSelection(IInputInfo info)
        {
            if (!_selectInfo.Process(info))
                return false;

            if (_selectInfo.Offset.Length < 3)
                return true;


            var bounds = new Bounds2F(_selectInfo.From, _selectInfo.Offset);

            var shapes = info.ViewPort.Shapes.IntersectsWith(bounds).ToArray();

            var addselection = shapes.Where(x => AllowSelect?.Invoke(x) == true).Where(x => x != null).ToArray();

            AddSelection = addselection;

            return true;
        }


        public Func<IShape, bool> AllowSelect { get; set; } = x => x.AllowSelect();
        public Func<IShape, bool> AllowFocus { get; set; } = x => true || x.AllowFocus();

        private bool EndSelection(IInputInfo info)
        {
            ProcessSelection(info);

            if (info.Keys == Keys.ShiftKey)
                Selection = AddSelection;
            else if (info.Keys == Keys.ControlKey)
                Selection = Selection.Union(AddSelection);

            AddSelection = null;

            return _selectInfo.Stop(info);
        }

        private bool SelectAll(IInputInfo info)
        {
            Selection = info.ViewPort.Shapes.Where(x => AllowSelect(x)).ToArray();
            return true;
        }


        private bool ClickNotShape(IInputInfo info)
        {
            info.Completed = true;
            if (IsSelectionMode)
                ClearSelection();
            return true;
        }

        private void ClearSelection()
        {
            Selection = null;
            FocusedShape = null;
        }

        private bool ClickShape(IInputInfo x)
        {
            x.Completed = true;
            var shape = x.Shape();

            if (IsSelectionMode)
            {
                return ClickShapeInSelectionMode(shape);
            }

            return true;
        }

        private bool ClickShapeInSelectionMode(IShape shape)
        {
            FocusedShape = AllowFocus(shape) ? shape : null;

            var contains = Selection.Contains(shape);
            if (!contains)
                Selection = AllowSelect(shape) ? new[] { shape } : new IShape[0];

            return true;
        }

        private bool CtrlClickShape(IInputInfo x)
        {
            x.Completed = true;
            var shape = x.Shape();
            if (IsSelectionMode)
                return CtrlClickShape(shape);

            return true;
        }

        private bool CtrlClickShape(IShape shape)
        {
            Selection = Selection.Contains(shape)
                ? Selection.Except(new[] { shape })
                : Selection.Union(new[] { shape });

            if (FocusedShape == shape)
                FocusedShape = null;
            else if (!Selection.Contains(shape))
                FocusedShape = null;
            else if (AllowFocus(shape))
                FocusedShape = shape;


            return true;
        }

        private bool IsShapeClick(IInputInfo info)
        {
            return CheckPointAndKeysAndButtonAndShape(info, MouseButtons.Left, k => k.Empty, x => x != null && AllowSelect(x));
        }

        private bool IsShapeCtrlClick(IInputInfo info)
        {
            return CheckPointAndKeysAndButtonAndShape(info, MouseButtons.Left, k => k == Keys.ControlKey, x => x != null && AllowSelect(x));
        }

        private bool IsNotShapeClick(IInputInfo info)
        {
            var result = CheckPointAndKeysAndButtonAndShape(info, MouseButtons.Left, k => k.Empty, x => x == null || !AllowSelect(x));
            return result;
        }

        private bool CheckPointAndKeysAndButtonAndShape(IInputInfo x, MouseButtons button, Func<KeysInfo, bool> checkKeys, Func<IShape, bool> checkShape = null)
        {
            if (!x.ViewPortPoint.HasValue)
                return false;

            var point = x.ViewPortPoint.Value;

            if (!checkKeys(x.Keys) || x.Buttons != button)
                return false;

            if (checkShape == null)
                return true;


            var shape = x.ViewPort.Shapes.LastOrDefault(point);
            return checkShape(shape);
        }
    }

  

}