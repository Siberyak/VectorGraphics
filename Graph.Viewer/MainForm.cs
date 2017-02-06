using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataLayer;
using Shapes;

using Item= DataLayer.Environment<float, float>.Item<int>;

namespace Graph.Viewer
{
    public partial class MainForm : Form
    {
        private ShapesProvider _provider;
        private Environment<float, float> _environment;

        public MainForm()
        {
            InitializeComponent();

            _provider = new ShapesProvider();

            _viewPort.Shapes = _provider;

            var viewPortInputInfoProcessor = _viewPort.Add<ViewPortInputInfoProcessor>("View Port IIP");
            viewPortInputInfoProcessor.AllowChangePosition = true;
            viewPortInputInfoProcessor.AllowChangeZoom = true;

            _viewPort.Add<SelectionInputInfoProcessor>("Selection IIP");
            _viewPort.Add<MouseOverInputInfoProcessor>("Mouse Over IIP");



            UpdateData();
        }

        public void UpdateData()
        {
            _environment = new Environment<float, float>
                (
                (time, offset) => time + offset,
                (left, right) => right - left,
                (o1, o2) => o1 + o2
                , x => -x
                );

            InitData();


            var nodes = _environment.Nodes<Environment<float, float>.IItem>().ToArray();


            var y = 0.0f;
            var dict = nodes.ToDictionary(x => x, x => _provider.AddRectangle(new Vector2F(x.Right.ToLeft - x.ToRight, y++), Vector2F.Unit / 5, x.Critical ? Pens.Green : Pens.Black));


            foreach (var item in nodes)
            {

                //_provider.Add(new LableShape() {Text = $"{item}", CenterLocation = from.CenterLocation});

                foreach (var dependency in item.Followers)
                {
                    var from = dict[dependency.Predecessor];
                    var to = dict[dependency.Follower];

                    var shape = _provider.AddLine(() => from.CenterLocation, () => to.CenterLocation);

                    shape.AllowSelect = false;
                }
            }

            
            
            //_provider.AddRectangle(Vector2F.Zerro, Vector2F.Unit);
            //_provider.Add(LineShape.New(Vector2F.Unit, Vector2F.Unit * 5));
            

            _viewPort.Zoom = 10;
        }

        private void InitData()
        {
            var zero = 0.0f;
            var one = 1.0f;
            var two = one + one;

            var i = 1;
            var chain2 = InitChain(8, ref i);
            var chain1 = InitChain(10, ref i);
            var chain3 = InitChain(8, ref i);

            chain2[2].AddPredecessor(chain1[2], one);
            chain1[3].AddPredecessor(chain3[1], one);
        }

        private Item[] InitChain(int length, ref int i)
        {
            Item[] chain = new Item[length];
            for (int j = 0; j < chain.Length; j++)
            {
                chain[j] = _environment.AddItem(i++);
                if (j > 0)
                    chain[j].AddPredecessor(chain[j - 1], 1);
            }
            return chain;
        }

        private void InitData1()
        {
            var zero = 0.0f;
            var one = 1.0f;
            var two = one + one;

            var i = 1;
            for (int j = 0; j < 10; j++)
            {

            }

            var i1 = _environment.AddItem(i++);
            var i2 = _environment.AddItem(i++);
            var i3 = _environment.AddItem(i++);
            var i4 = _environment.AddItem(i++);

            var tmp = zero;
            // 1 -(1:2)-> 2
            i2.AddPredecessor(i1, one + tmp, two + tmp);

            // 3 -(3:4)-> 4
            i4.AddPredecessor(i3, two + one, two + two);

            // 2 -( :-1)-> 4
            i4.AddPredecessor(i2, right: -one / 2);

            var i5 = _environment.AddItem(i++);

            // 4 -(1:)-> 5
            i5.AddPredecessor(i4, right: one);

            var i6 = _environment.AddItem(i++);

            // 6 -(1:)-> 1
            i6.AddPredecessor(i1, right: one);

            var i7 = _environment.AddItem(i++);

            // 7 -(1:)-> 3
            i3.AddPredecessor(i7, right: one);
        }
    }
}
