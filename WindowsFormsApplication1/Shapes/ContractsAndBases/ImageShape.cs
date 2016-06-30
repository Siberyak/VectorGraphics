using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Shapes
{
    public interface IMouseEnterProcessor
    {
        void Process();
    }

    public interface IMouseLeaveProcessor
    {
        void Process();
    }
    public class ImageShape : PathedShape, IMouseEnterProcessor, IMouseLeaveProcessor
    {
        private readonly Image[] _images;
        private Image _currentImage;

        public Image CurrentImage { get { return _currentImage ?? (_currentImage = _images.FirstOrDefault()); } }

        public ImageShape(Vector2F centerLocation, Vector2F size, params Image[] images)
            : base(centerLocation, size, true)
        {
            _images = images;
        }

        public ImageShape(Vector2F size, params Image[] images)
            : this(Vector2F.Zerro, size, images)
        {}

        public ImageShape(Vector2F centerLocation, Image image)
            : this(centerLocation, image.Size, image)
        {
        }

        public ImageShape(Image image)
            : this(Vector2F.Zerro, image)
        {
        }

        protected override IEnumerable<DrawingItem> GetItems()
        {
            return new[] { new ImageDrawingItem(this) };
        }

        protected override void GetPathFigures(GraphicsPath path, Bounds2F selfBounds)
        {
            path.AddRectangle(selfBounds);
        }

        private class ImageDrawingItem : DrawingItem
        {
            private readonly object _locker = new object();
            private readonly ImageShape _shape;
            private double _zoom;
            private Image _currentImage;

            public ImageDrawingItem(ImageShape shape)
                : base(shape)
            {
                _shape = shape;
            }

            public override void Reset()
            {
                lock (_locker)
                {
                }
            }

            public override void Draw(IViewPort viewPort)
            {
                if (viewPort == null)
                    return;

                lock (_locker)
                {

                    var shape = (SizedShape)_shape;
                    var location = shape.CenterLocation;
                    var size = shape.Size();
                    var rotation = shape.Rotation();

                    var scale = viewPort.Zoom;
                    var original = viewPort.Graphics.Transform;


                    var points = new PointF[]{location};
                    original.TransformPoints(points);

                    var m = new Matrix();
                    var destSize = size * scale;
                    var destLocation = location * scale - destSize / 2;

                    try
                    {
                        m.RotateAt(rotation, points[0], MatrixOrder.Append);
                        m.Multiply(original);

                        viewPort.Graphics.Transform = m;
                        viewPort.Graphics.DrawImage(_shape.CurrentImage, new RectangleF(destLocation, destSize), new RectangleF(PointF.Empty, size), GraphicsUnit.Pixel);
                    }
                    finally
                    {
                        viewPort.Graphics.Transform = original;
                        m.Dispose();
                    }
                }
            }
        }

        void IMouseEnterProcessor.Process()
        {
            NextImage();
        }

        void IMouseLeaveProcessor.Process()
        {
            NextImage();
        }

        private void NextImage()
        {
            if(_images.Length <= 1)
                return;

            _currentImage = _images.SkipWhile(x => x != CurrentImage).Skip(1).LastOrDefault() ?? _images.FirstOrDefault();
        }
    }

}