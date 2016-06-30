using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Shapes
{
    public partial class ViewPortBase : Control, IViewPort
    {

        #region TIMER

        protected Timer Timer;
        private Timer _invalidateTimer;

        protected override void DestroyHandle()
        {
            if (_invalidateTimer != null)
            {
                _invalidateTimer.Stop();
                _invalidateTimer.Dispose();
            }

            if (Timer != null)
            {
                Timer.Stop();
                Timer.Dispose();
            }

            base.DestroyHandle();
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            Timer = new Timer(50);
            _invalidateTimer = new Timer(1000.0/60);

            OnTimerCreated();
        }

        protected virtual void OnTimerCreated()
        {

        }

        #endregion


        #region Suspend / Resume Invalidate

        readonly SuspendHelper _invalidateSuspender = new SuspendHelper();

        private bool InvalidateSuspended
        {
            get
            {
                return _invalidateSuspender.Suspended;
            }
        }

        private void SuspendInvalidate(object tag = null)
        {
            _invalidateSuspender.Suspend(DoInvalidate, tag);
        }

        private void ResumeInvalidate(object tag = null, bool checkTag = false)
        {
            _invalidateSuspender.Resume(tag, checkTag);
        }




        private bool _awaitingInvalidate;

        protected void DoInvalidate(bool suspended = false)
        {
            if (!InvalidateSuspended && !suspended)
            {
                if (_needInvalidate)
                {
                    _awaitingInvalidate = true;
                }

                _needInvalidate = false;
            }
            else
                _needInvalidate = true;
        }

        #endregion

        public void ChangeZoom(float wheel, Vector2F? zoomPoint = default (Vector2F?))
        {
            var point = zoomPoint ?? Center + Position;

            var dz = 1 + 0.1f * wheel;

            var viewPortPoint = ClientToViewPort(point);

            Zoom = Zoom * dz;

            var p2 = ViewPortToClient(viewPortPoint);

            var dp = (p2 - point);

            Position += dp;
        }

        #region Keys

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Focused)
                return;
            try
            {
                SuspendInvalidate();
                e.Handled = InputInfoProcessor.KeyDown(e.KeyCode);
            }
            finally
            {
                ResumeInvalidate();
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!Focused)
                return;

            try
            {
                SuspendInvalidate();
                e.Handled = InputInfoProcessor.KeyUp(e.KeyCode);
            }
            finally
            {
                ResumeInvalidate();
                base.OnKeyUp(e);
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused)
                Focus();

            try
            {
                SuspendInvalidate();
                InputInfoProcessor.MouseDown(e);
            }
            finally
            {
                ResumeInvalidate();
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Focused)
                return;

            try
            {
                SuspendInvalidate();
                InputInfoProcessor.MouseUp(e);
            }
            finally
            {
                ResumeInvalidate();
                base.OnMouseUp(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!Focused)
                return;
            try
            {
                SuspendInvalidate();
                InputInfoProcessor.MouseMove(e);
            }
            finally
            {
                ResumeInvalidate();
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!Focused)
                return;
            try
            {
                SuspendInvalidate();
                InputInfoProcessor.MouseWheel(e);
            }
            finally
            {
                ResumeInvalidate();
                base.OnMouseWheel(e);
            }
        }

        #endregion

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            var oldBounds = new Bounds2F(Location, Size);
            base.SetBoundsCore(x, y, width, height, specified);
            OnBoundsChanged(oldBounds);
        }

        private void OnBoundsChanged(Bounds2F previous)
        {
            var current = new Bounds2F(Location, Size);
            OnBoundsChanged(new BoundsChangedEventArgs(previous, current));
        }

        public event BoundsChangedEventHandler BoundsChanged;

        protected virtual void OnBoundsChanged(BoundsChangedEventArgs e)
        {
            BoundsChanged?.Invoke(this, e);
        }

        protected Graphics Graphics;
        private Vector2F _position = Vector2F.Zerro;
        private float _zoom = 1;
        private bool _needInvalidate;
        private Vector2F? _center;
        private IShapesProvider _shapes;

        //protected readonly SelectionInputInfoProcessor SelectionInputInfoProcessor;
        //protected readonly InputInfoProcessor _inputInfo2;
        private readonly InputInfoProcessorsCollection _processorsCollection = new InputInfoProcessorsCollection();


        public ViewPortBase()
        {
            InitializeComponent();

            if (DesignMode)
                return;

            //SelectionInputInfoProcessor = new SelectionInputInfoProcessor(this);
            //SelectionInputInfoProcessor.Mode = ViewPortMode.Selection;
            //SelectionInputInfoProcessor.Tag("Caption", "Selection IIP");

            //_inputInfo2 = new InputInfoProcessor(this);
            //_inputInfo2.Tag("Caption", "MouseOver IIP");

            //_processorsCollection = new InputInfoProcessorsCollection(SelectionInputInfoProcessor, _inputInfo2);


            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            DoubleBuffered = true;

            MouseWheelRedirector.Attach(this);
            
            //InputInfo.Changed += info => { WriteLine("{0}", info);return false;};
        }

        public T Add<T>(string caption = null)
            where T : IInputInfoProcessor
        {
            var processor = ProcessorsFactory.Get<T>(this, caption);

            return ReferenceEquals(processor, default(T)) || !_processorsCollection.Add(processor) ? default(T) : processor;
        }

        public bool Add(IInputInfoProcessor processor)
        {
            return _processorsCollection.Add(processor);
        }

        public bool Remove(IInputInfoProcessor processor)
        {
            return _processorsCollection.Remove(processor);
        }

        void IViewPort.Invalidate()
        {
            DoInvalidate();
        }

        public IInputInfoProcessor InputInfoProcessor
        {
            get
            {
                return _processorsCollection;
            }
        }

        protected Vector2F Center
        {
            get
            {
                if (!_center.HasValue)
                    _center = new Vector2F(Width, Height) * 0.5f;

                return _center.Value;
            }
        }

        public RectangleF ClipRectangle { get; set; }

        public IShapesProvider Shapes
        {
            get { return _shapes; }
            set
            {
                if (_shapes == value)
                    return;

                _shapes = value;

                DoInvalidate();
            }
        }

        public Vector2F Position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                    return;

                _position = value;
                DoInvalidate();
            }
        }

        Graphics IViewPort.Graphics
        {
            get { return Graphics; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (Math.Abs(_zoom - value) < float.Epsilon)
                    return;

                _zoom = value;

                DoInvalidate();
            }
        }

        public Bounds2F ViewPortToClientRectangle(Vector2F @from, Vector2F to)
        {
            var location = ViewPortToClient(from);
            var size = ViewPortToClient(to) - location;
            var rectangle = new Bounds2F(location, size);
            rectangle.Offset(-Center + Position );

            return rectangle;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _center = null;
            Refresh();
        }

        protected void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(Focused + @"   " + format, args);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode)
                DoDraw(e);

            base.OnPaint(e);
        }



        #region Draw

        protected virtual void DoDraw(PaintEventArgs e)
        {
            if (NeedDraw())
                return;

            try
            {
                Graphics = e.Graphics;
                ClipRectangle = ClipRectangleToViewPort(e.ClipRectangle);

                PrepareDrawGraphics();

                BeforeDrawShapes();

                DrawShapes();

                AfterDrawShapes();

                Graphics.Flush();

            }
            finally
            {
                Graphics = null;
            }

        }

        protected virtual void BeforeDrawShapes()
        {
            InputInfoProcessor.BeforeDrawShapes();
        }

        protected virtual void DrawShapes()
        {
            foreach (var shape in Shapes)
            {
                DrawShape(shape);
            }
        }

        protected virtual void DrawShape(IShape shape)
        {
            InputInfoProcessor.BeforeDrawShape(shape);
            shape.Draw(this);
            InputInfoProcessor.AfterDrawShape(shape);
        }

        protected virtual void AfterDrawShapes()
        {
            InputInfoProcessor.AfterDrawShapes();
        }

        protected virtual void PrepareDrawGraphics()
        {
            Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Graphics.CompositingMode = CompositingMode.SourceOver;

            var d = Center - Position;
            Graphics.TranslateTransform(d.X, d.Y);
        }

        protected virtual bool NeedDraw()
        {
            return !Shapes.Any();
        }

        #endregion

        protected RectangleF ClipRectangleToViewPort(Rectangle clipRectangle)
        {
            return clipRectangle;
            //var clientRectangle = ClientRectangle;
            //clientRectangle.Intersect(clipRectangle);

            ////Size.AspectRatio(0.5f).ToPointF().CalcOffset();

            //var location = ClientToViewPort(clipRectangle.Location);
            //var rect = new RectangleF(location, clipRectangle.Size.AspectRatio(1/Zoom));

            //return rect;
        }

        protected bool ClientToViewPort(Vector2F? point, ref Vector2F result)
        {
            if (!point.HasValue)
                return false;

            result = (point.Value - Center + Position) / Zoom;
            return true;
        }

        protected bool ViewPortToClient(Vector2F? point, ref Vector2F result)
        {
            if (!point.HasValue)
                return false;

            result = point.Value * Zoom - Position + Center;
            return true;
        }

        public Vector2F ClientToViewPort(Vector2F point)
        {
            return (point - Center + Position) / Zoom;
        }

        public Vector2F ViewPortToClient(Vector2F point)
        {
            return point * Zoom - Position + Center;
        }


    }

    public delegate void BoundsChangedEventHandler(object sender, BoundsChangedEventArgs e);
    public class BoundsChangedEventArgs : EventArgs
    {
        public Bounds2F Previous { get; }
        public Bounds2F Current { get; }

        public BoundsChangedEventArgs(Bounds2F previous, Bounds2F current)
        {
            Previous = previous;
            Current = current;
        }
    }
}