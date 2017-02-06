using System.Collections.Generic;
using KG.SE2.Utils.Collections;

namespace KG.SE2.Utils.Graph
{
    public class DataNode<TNodeData> : IDataNode<TNodeData>
    {
        private readonly MutableDataGraph _dataGraph;
        private PredicatedList<IEdge> _backReferences;

        private PredicatedList<IEdge> _references;

        public DataNode(MutableDataGraph dataGraph, TNodeData data)
        {
            _dataGraph = dataGraph;
            Data = data;
        }

        public IMutableDataGraph Graph => _dataGraph;

        IGraph INode.Graph => Graph;

        public object NodeData()
        {
            return Data;
        }

        public virtual string Caption => "" + Data;

        IEnumerable<IEdge> INode.References => References;

        IEnumerable<IEdge> INode.BackReferences => BackReferences;

        TNodeData IDataNode<TNodeData>.Data => Data;

        protected virtual IEnumerable<IEdge> References
        {
            get { return _references ?? (_references = new PredicatedList<IEdge>(_dataGraph.EdgesDataList, x => x.IsBackreference ? x.To == this : x.From == this)); }
        }

        protected virtual IEnumerable<IEdge> BackReferences
        {
            get { return _backReferences ?? (_backReferences = new PredicatedList<IEdge>(_dataGraph.EdgesDataList, x => x.IsBackreference ? x.From == this : x.To == this)); }
        }

        protected virtual TNodeData Data { get; }

        public override string ToString()
        {
            return $"Node, Data: [{Data}]";
        }
    }
}