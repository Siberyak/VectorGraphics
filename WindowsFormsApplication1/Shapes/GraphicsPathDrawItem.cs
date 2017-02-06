using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Shapes
{
    public class GraphicsPathDrawItem : DrawingItem
    {

        protected readonly object _locker = new object();
        private GraphicsPath _path;
        private Func<IShape, GraphicsPath> _getPath;

        private Pen _pen;

        public GraphicsPathDrawItem(IShape shape, Pen pen, Func<IShape, GraphicsPath> getPath)
            : this(shape, pen)
        {
            _getPath = getPath;
        }

        public GraphicsPathDrawItem(IShape shape, Pen pen)
            : base(shape)
        {
            _pen = pen;
        }

        public override void Reset()
        {
            lock (_locker)
            {
                if (_path == null)
                    return;

                _path.Dispose();
                _path = null;
            }
        }

        public override void Draw(IViewPort viewPort)
        {

            if (viewPort == null)
                return;

            lock (_locker)
            {
                DrawPath(viewPort);
            }
        }

        private void DrawPath(IViewPort viewPort)
        {
            var path = _path ?? (_path = _getPath == null ? null : _getPath(Shape));

            if (path == null)
                return;

            path = ScaleByViewPort(path, viewPort.Zoom);

            viewPort.Graphics.DrawPath(_pen, path);
            
        }

        protected internal static GraphicsPath ScaleByViewPort(GraphicsPath path, float scale)
        {
            if (!(Math.Abs(scale - 1) > float.Epsilon))
                return path;

            path = (GraphicsPath)path.Clone();

            var matrix = new Matrix();
            matrix.Scale(scale, scale);
            path.Transform(matrix);

            return path;
        }
    }
}