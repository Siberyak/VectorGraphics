namespace KG.SE2.Utils.Graph
{
    public class DataEdgesFactory<TEdgeData> : IDataEdgesFactory<TEdgeData>
    {
        private readonly MutableDataGraph _dataGraph;

        public DataEdgesFactory(MutableDataGraph dataGraph)
        {
            _dataGraph = dataGraph;
        }

        public IDataEdge<TEdgeData> Create(
            INode @from,
            INode to,
            TEdgeData data,
            bool isBackreference)
        {
            return new DataEdge<TEdgeData>(_dataGraph, data, @from, to, isBackreference);
        }
    }
}