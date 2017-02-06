namespace KG.SE2.Utils.Graph
{
    public class DataNodesFactory<TNodeData> : IDataNodesFactory<TNodeData>
    {
        private readonly MutableDataGraph _dataGraph;

        public DataNodesFactory(MutableDataGraph dataGraph)
        {
            _dataGraph = dataGraph;
        }

        public IDataNode<TNodeData> Create(TNodeData data)
        {
            return new DataNode<TNodeData>(_dataGraph, data);
        }
    }
}