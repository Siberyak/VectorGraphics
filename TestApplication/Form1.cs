using System;
using System.Drawing;
using System.Windows.Forms;
using Shapes;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        IShapesProvider _shapesProvider = new ShapesProvider();

        public Form1()
        {
            InitializeComponent();
            if (DesignMode)
                return;

            simpleButton1_Click(null, EventArgs.Empty);

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _userControl3.Shapes = new ShapesProvider();

            _userControl3.Shapes.AddRectangle(PointF.Empty, new SizeF(100, 100), Pens.MediumSpringGreen, rotation: 45).StartRotate(24);
            _userControl3.Shapes.AddRectangle(new PointF(0, -100), new SizeF(100, 50), Pens.OrangeRed).StartRotate(-90);
            _userControl3.Shapes.AddRectangle(new PointF(0, 100), new SizeF(200, 25), Pens.DeepSkyBlue, rotation: 12.5f).StartRotate(-18);


            //var imageShape = new ImageShape(new Point(50, 25), Image.FromFile(@"Images\border-followerspecial.gif")) { Rotation = 33 };
            //imageShape.StartRotate(-23);
            //_userControl3.Shapes.Add(imageShape);





            //==============================================

            _userControl1.Shapes = _shapesProvider;
            _userControl2.Shapes = _shapesProvider;
            


            _shapesProvider.Add(new PointShape(Vector2F.Zerro, Color.Red));

            _shapesProvider.AddRectangle(PointF.Empty, new SizeF(1, 1), Pens.OrangeRed);


            _shapesProvider.AddRectangle(new PointF(0, 100), new SizeF(75, 50), new Pen(Color.Purple) { Width = 3 }, rotation: 30);
            var rrr2 = _shapesProvider.AddRectangle(new PointF(0, -75), new SizeF(100, 100), rotation: 0*45);
            rrr2.StartRotate(60);

            _shapesProvider.AddEllipse(new PointF(-100, 0), new SizeF(50, 75), false)
                //.StartRotate(-90)
                ;

            var rrr = _shapesProvider.AddRectangle(new PointF(100, 0), new SizeF(100, 50), rotation: 0*12.5f);
            rrr.StartRotate(-30);

            var point = rrr.ViewPortToShape(rrr.Bounds.BottomRight);
            var point3 = rrr2.ViewPortToShape(rrr2.Bounds.TopLeft);
            var point2 = point3 + Vector2F.Unit * -50;

            var lineShape = LineShape.New(() => rrr2.ShapeToViewPort(point3), () => rrr2.ShapeToViewPort(point2));
            _shapesProvider.Add(lineShape);
            var lineShape2 = LineShape.New(() => rrr2.ShapeToViewPort(point2), () => rrr.ShapeToViewPort(point));
            _shapesProvider.Add(lineShape2);

            //imageShape = new ImageShape(new PointF(0, 0), Image.FromFile(@"Images\Active-Hover.png")) { Rotation = 0 };
            //imageShape.StartRotate(15);
            //_shapesProvider.Add(imageShape);



            var dim = 10;
            var pen = Pens.Orange;
            _shapesProvider.AddRectangle(new PointF(-dim, 0), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, 0), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(-dim, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(-dim, -dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, -dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(0, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(0, -dim), new SizeF(1, 1), pen);

            dim = 20;
            pen = Pens.OliveDrab;
            _shapesProvider.AddRectangle(new PointF(-dim, 0), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, 0), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(-dim, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(-dim, -dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(dim, -dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(0, dim), new SizeF(1, 1), pen);
            _shapesProvider.AddRectangle(new PointF(0, -dim), new SizeF(1, 1), pen);


            _userControl3.Add<SelectionInputInfoProcessor>("Selection IIP - 3");
            _userControl3.Add<MouseOverInputInfoProcessor>("Mouse Over IIP - 3");

            var vpiip = _userControl1.Add<ViewPortInputInfoProcessor>("View Port IIP - 1");
            vpiip.AllowChangeZoom = true;
            vpiip.AllowChangePosition = true;

//            _userControl1.Add<SelectionInputInfoProcessor>("Selection IIP - 1");
            _userControl1.Add<MouseOverInputInfoProcessor>("Mouse Over IIP - 1");
            _userControl1.Add<SIIP>("SIIP - 1");

            _userControl2.Add<SelectionInputInfoProcessor>("Selection IIP - 2");
            _userControl2.Add<MouseOverInputInfoProcessor>("Mouse Over IIP - 2");
        }
    }
}
