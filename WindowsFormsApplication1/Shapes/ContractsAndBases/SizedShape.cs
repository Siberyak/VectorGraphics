namespace Shapes
{
    public abstract class SizedShape : Shape, ISizedShape
    {
        /// <summary>
        /// текущие собственные размеры с учетом трансформаций
        /// </summary>
        private Bounds2F _bounds;
        /// <summary>
        /// собственные размеры без трансформаций (задаются при инициализации)
        /// </summary>
        protected Bounds2F _selfBounds;

        protected SizedShape(Vector2F centerLocation, Vector2F size)
            : base(centerLocation, Vector2F.Unit, 0, false)
        {
            Size = size;
            InitBounds();
        }

        protected SizedShape(Vector2F size)
            : this(Vector2F.Zerro, size)
        {
        }

        public Vector2F Size { get; private set; }

        public virtual Bounds2F Bounds
        {
            get { return _bounds; }
        }

        protected virtual void InitBounds()
        {
            _bounds = InitBounds(out _selfBounds);
        }

        protected abstract Bounds2F InitBounds(out Bounds2F selfBounds);

        protected override void OnLocationChanged(Vector2F oldValue)
        {
            InitBounds();
            base.OnLocationChanged(oldValue);
        }

        protected override void OnAspectRatioChanged(Vector2F oldValue)
        {
            InitBounds();
            base.OnAspectRatioChanged(oldValue);
        }


        protected override void OnRotationChanged(float oldValue)
        {
            InitBounds();
            base.OnRotationChanged(oldValue);
        }

        public override bool Contains(Bounds2F bounds)
        {
            return Bounds.Contains(bounds);
        }

        public override bool IntersectsWith(Bounds2F bounds)
        {
            return Bounds.IntersectsWith(bounds);
        }

    }
}