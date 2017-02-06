using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;

namespace Shapes
{
    public abstract class Shape : IShape, IRotatableShape
    {
        private IEnumerable<DrawingItem> _items;
        private Vector2F _centerLocation;
        private float _rotation;
        private Vector2F _aspectRatio;

        protected Shape(Vector2F centerLocation, Vector2F aspectRatio, float rotation, bool keepAspectRation)
        {
            CenterLocation = centerLocation;
            AspectRatio = aspectRatio;
            Rotation = rotation;
            KeepAspectRation = keepAspectRation;
        }

        protected Shape() : this(Vector2F.Zerro, new Vector2F(1, 1), 0, false)
        {
        }

        Vector2F IShape.Location
        {
            get { return CenterLocation; }
        }

        public virtual Vector2F CenterLocation
        {
            get { return _centerLocation; }
            set
            {
                var oldValue = _centerLocation;
                _centerLocation = value;
                if (_centerLocation != oldValue)
                    OnLocationChanged(oldValue);
            }
        }

        public Vector2F ViewPortToShape(Vector2F viewPortPoint)
        {
            var shapePoint = (viewPortPoint - CenterLocation).RotateDegrees(-Rotation);
            return shapePoint;
        }

        public Vector2F ShapeToViewPort(Vector2F shapePoint)
        {
            var viewPortPoint = shapePoint.RotateDegrees(Rotation) + CenterLocation;
            return viewPortPoint;
        }

        protected virtual void OnLocationChanged(Vector2F oldValue)
        {
            ResetItems();
        }

        protected virtual void ResetItems()
        {
            if (_items == null)
                return;

            foreach (var item in _items)
                item.Reset();
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                var oldValue = _rotation;
                _rotation = value;
                
                if(Math.Abs(_rotation - oldValue) >= float.Epsilon)
                    OnRotationChanged(oldValue);
            }
        }

        public virtual bool AllowRotate
        {
            get;
            set;
        }

        public void Rotate(float angle)
        {
            _rotation += angle;
        }

        public event RotationChangedEventHandler RotationChanged;

        protected virtual void OnRotationChanged(float oldValue)
        {
            ResetItems();
            RotationChanged?.Invoke(this, oldValue);
        }

        public Vector2F AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                var oldValue = _aspectRatio;
                _aspectRatio = value;
                if (_aspectRatio != oldValue)
                    OnAspectRatioChanged(oldValue);
            }
        }

        protected virtual void OnAspectRatioChanged(Vector2F oldValue)
        {
            ResetItems();
        }

        public bool KeepAspectRation { get; set; }

        //public abstract Bounds2F Bounds { get; }

        protected abstract IEnumerable<DrawingItem> GetItems();

        public abstract bool Contains(Vector2F point);
        public abstract bool Contains(Bounds2F bounds);
        public abstract bool IntersectsWith(Bounds2F bounds);
        protected IEnumerable<DrawingItem> Items => _items ?? (_items = GetItems());

        public virtual void Draw(IViewPort viewPort)
        {
            
            if(Items == null)
                return;

            foreach (var item in Items)
            {
                item.Draw(viewPort);
            }
        }


        private float? _anglePerSecond;
        private DateTime? _prevSignal;

        protected static Timer Timer = new Timer(50) { Enabled = true };

        public void StartRotate(float anglePerSecond)
        {
            _anglePerSecond = anglePerSecond;
            Timer.Elapsed += RotateByTimer;
        }

        readonly SuspendHelper _timerSuspender = new SuspendHelper();

        public void SuspendTimer()
        {
            if (!_anglePerSecond.HasValue)
                return;

            _timerSuspender.Suspend();
        }

        public void ResumeTimer()
        {
            if (!_anglePerSecond.HasValue)
                return;

            _timerSuspender.Resume();
        }

        private void RotateByTimer(object sender, ElapsedEventArgs e)
        {
            if (!_anglePerSecond.HasValue)
                return;

            if (_prevSignal.HasValue && !_timerSuspender.Suspended)
            {

                var dt = Convert.ToSingle((e.SignalTime - _prevSignal.Value).TotalSeconds);
                Rotation += (_anglePerSecond.Value * dt) ;
            }

            _prevSignal = e.SignalTime;
        }

        //public abstract void DrawSelfBounds(IViewPort viewPort, Pen pen, float inflate);
    }
}