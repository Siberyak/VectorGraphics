using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Shapes;

namespace D3.Viewer
{
    public partial class D3Form : Form
    {
        private static readonly string[] Classes = new[] { "barbarian", "crusader", "demon-hunter", "monk", "witch-doctor", "wizard" };
        private static readonly string[] Sexes = new[] { "male", "female" };

        public static ClassImages[] Data = Classes.SelectMany(@class => Sexes.Select(sex => string.Format("{0}-{1}", @class, sex)))
                .Select(x => new ClassImages(x))
                .ToArray();

        public class ClassImages
        {
            public string Caption { get; private set; }

            public ClassImages(string caption)
            {
                Caption = caption;

                var img = Image.FromFile(string.Format(@"Images\fallen-{0}.jpg", Caption));
                var size = new Size(img.Width, img.Height / 2);
                Image1 = SplitImage(img, size, Vector2F.Zerro);
                Image2 = SplitImage(img, size, new Vector2F(0, size.Height));
                Image3 = Image.FromFile(string.Format(@"Images\{0}.jpg", Caption));
            }

            public Image Image1 { get; private set; }
            public Image Image2 { get; private set; }
            public Image Image3 { get; private set; }
        }

        static Image SplitImage(Image original, Size size, Vector2F offset)
        {
            var image = new Bitmap(size.Width, size.Height);
            using (var gr = Graphics.FromImage(image))
            {
                var destRect = new RectangleF(PointF.Empty, size);
                var srcRect = new RectangleF(offset, size);
                gr.DrawImage(original, destRect, srcRect, GraphicsUnit.Pixel);
                return image;
            }

        }

        public D3Form()
        {
            InitializeComponent();

            BackColor = Color.FromArgb(26, 23, 18);

            var provider = new ShapesProvider();

            var shapes = Data.Select(x => new ImageShape(x.Image1.Size, x.Image1, x.Image2)).ToArray();
            for (var i = 0; i < shapes.Length; i++)
            {
                
                provider.Add(shapes[i]);

                if(i == 0)
                    continue;

                shapes[i].CenterLocation = shapes[i-1].CenterLocation + new Vector2F(shapes[i - 1].CurrentImage.Width + 3, 0);
            }

            _topViewPort.Height = shapes[0].CurrentImage.Height + 30;
            _topViewPort.Position = new Vector2F(_topViewPort.Width / 2 - shapes[0].CurrentImage.Width, 0);
            _topViewPort.Shapes = provider;

            var viewPortInputInfoProcessor = _topViewPort.Add<ViewPortInputInfoProcessor>("View Port IIP");
            viewPortInputInfoProcessor.AllowChangePosition = true;
            viewPortInputInfoProcessor.AllowChangeZoom = true;

            _topViewPort.Add<SelectionInputInfoProcessor>("Selection IIP");
            _topViewPort.Add<MouseOverInputInfoProcessor>("Mouse Over IIP");
            
            _fillViewPort.Shapes = new ShapesProvider();
            


        }
    }
}

