using KG.SE2.Utils.Graph;

namespace DataLayer
{
    public partial class Environment<TTimeUnit, TOffsetUnit>
    {
        public class Graph : MutableDataGraph
        {
            public Graph(Environment<TTimeUnit, TOffsetUnit> environment)
            {
                Environment = environment;
            }

            public Environment<TTimeUnit, TOffsetUnit> Environment { get; }
        }
    }
}