using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace WindowsFormsApplication1
{
    public abstract class Shape
    {
        private IEnumerable<DrawingItem> _items;
        private Vector2F _location;
        private float _rotation;
        private Vector2F _scale;

        protected Shape(Vector2F location, Vector2F scale, float rotation, bool keepAspectRation)
        {
            Location = location;
            Scale = scale;
            Rotation = rotation;
            KeepAspectRation = keepAspectRation;
        }

        protected Shape() : this(Vector2F.Zerro, new Vector2F(1, 1), 0, false)
        {
        }

        protected internal Vector2F Location
        {
            get { return _location; }
            set
            {
                var oldValue = _location;
                _location = value;
                if (_location != oldValue)
                    OnLocationChanged(oldValue);
            }
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

        protected internal float Rotation
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

        protected virtual void OnRotationChanged(float oldValue)
        {
            ResetItems();
        }

        protected internal Vector2F Scale
        {
            get { return _scale; }
            set
            {
                var oldValue = _scale;
                _scale = value;
                if (_scale != oldValue)
                    OnScaleChanged(oldValue);
            }
        }

        protected virtual void OnScaleChanged(Vector2F oldValue)
        {
            ResetItems();
        }

        public bool KeepAspectRation { get; set; }

        protected abstract bool Selectable { get; }

        public abstract Bounds2F Bounds { get; }

        protected abstract IEnumerable<DrawingItem> GetItems();

        public abstract bool Contains(Vector2F point);

        protected IEnumerable<DrawingItem> Items
        {
            get { return _items ?? (_items = GetItems()); }
        }

        public virtual void Draw(IViewPort viewPort, bool selected)
        {
            
            if(Items == null)
                return;

            foreach (var item in Items)
            {
                item.Draw(viewPort, selected);
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
    }
}