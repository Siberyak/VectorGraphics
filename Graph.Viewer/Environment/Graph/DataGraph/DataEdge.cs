namespace KG.SE2.Utils.Graph
{
    public class DataEdge<TEdgeData> : IDataEdge<TEdgeData>
    {
        private readonly MutableDataGraph _dataGraph;

        public DataEdge(MutableDataGraph dataGraph, TEdgeData data, INode @from, INode to, bool isBackreference)
        {
            _dataGraph = dataGraph;
            From = @from;
            To = to;
            IsBackreference = isBackreference;
            Data = data;
        }

        public IMutableDataGraph Graph => _dataGraph;

        IGraph IEdge.Graph => Graph;

        object IDataEdge.EdgeData()
        {
            return Data;
        }

        INode IEdge.From => From;

        INode IEdge.To => To;

        bool IEdge.IsBackreference => IsBackreference;

        TEdgeData IDataEdge<TEdgeData>.Data => Data;

        protected virtual INode From { get; }

        protected virtual INode To { get; }

        protected virtual bool IsBackreference { get; }

        protected virtual TEdgeData Data { get; }


        public override string ToString()
        {
            return $"{GetType().Name}, Data: [{Data}], From: [{From}], To: [{To}]";
        }
    }
}