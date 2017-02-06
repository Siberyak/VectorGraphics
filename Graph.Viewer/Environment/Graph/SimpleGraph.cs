using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KG.SE2.Utils.Collections;

namespace KG.SE2.Utils.Graph
{
	public interface INodesFactory
	{
		INode Create();
	}

	public interface IEdgesFactory
	{
		IEdge Create(INode @from, INode to, bool isBackreference);
	}

	public class SimpleGraph : IMutableGraph
	{
		public INodesFactory NodesFactory
		{
			get { return _nodesFactory ?? (_nodesFactory = new SimpleNodesFactory(this)); }
			set { _nodesFactory = value; }
		}

		public IEdgesFactory EdgesFactory
		{
			get { return _edgesFactory ?? (_edgesFactory = new SimpleEdgeFactory(this)); }
			set { _edgesFactory = value; }
		}

		private readonly List<INode> _nodes = new List<INode>();

		private readonly List<IEdge> __edgesList = new List<IEdge>();
		
		internal DataList<IEdge> __edges;

		private INodesFactory _nodesFactory;
		private IEdgesFactory _edgesFactory;

		internal DataList<IEdge> _edges { get { return __edges ?? (__edges = new DataList<IEdge>(__edgesList)); } }

		public IEnumerable<INode> Nodes
		{
			get { return _nodes.ToArray(); }
		}

		public IEnumerable<IEdge> Edges
		{
			get { return _edges.ToArray(); }
		}

		INode IMutableGraph.Add()
		{
			return Add();
		}

		IEdge IMutableGraph.Add(INode @from, INode to, bool isBackreference)
		{
			return Add(from, to, isBackreference);
		}

		public void Remove(INode node)
		{
			foreach (var reference in node.References.ToArray())
				Remove(reference);
			foreach (var reference in node.BackReferences.ToArray())
				Remove(reference);
			_nodes.Remove(node);
		}

		public void Remove(IEdge edge)
		{
			_edges.Remove(edge);
		}

		public INode Add()
		{
			var node = NodesFactory.Create();
			_nodes.Add(node);
			return node;
		}

		public IEdge Add(INode from, INode to, bool isBackreference = false)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");

			if(!_nodes.Contains(from))
				throw new ArgumentException("данная нода не принадлежит этому графу", "from");

			if (!_nodes.Contains(to))
				throw new ArgumentException("данная нода не принадлежит этому графу", "to");

			var edge = EdgesFactory.Create(@from, to, isBackreference);
			_edges.Add(edge);
			return edge;
		}
	}

	public class SimpleNodesFactory : INodesFactory
	{
		private readonly SimpleGraph _graph;

		public SimpleNodesFactory(SimpleGraph graph)
		{
			_graph = graph;
		}

		public INode Create()
		{
			return new SimpleNode(_graph);
		}
	}

	public class SimpleEdgeFactory : IEdgesFactory
	{
		private SimpleGraph _graph;

		public SimpleEdgeFactory(SimpleGraph graph)
		{
			_graph = graph;
		}

		public IEdge Create(INode @from, INode to, bool isBackreference)
		{
			return new SimpleEdge(_graph, from, to, isBackreference);
		}
	}

	public class SimpleNode : INode
	{
		private readonly SimpleGraph _graph;
		PredicatedList<IEdge> _references;
		private PredicatedList<IEdge> _backReferences;

	    public IGraph Graph { get {return _graph;} }

	    private SimpleNode()
		{

		}

		internal SimpleNode(SimpleGraph graph)
			: this()
		{
			_graph = graph;
		}

		public IEnumerable<IEdge> References
		{
			get { return _references ?? (_references = new PredicatedList<IEdge>(_graph._edges, x => x.IsBackreference ? x.To == this : x.From == this)); }
		}

		public IEnumerable<IEdge> BackReferences
		{
			get { return _backReferences ?? (_backReferences = new PredicatedList<IEdge>(_graph._edges, x => x.IsBackreference ? x.From == this : x.To == this)); }
		}

	}

	public class SimpleEdge : IEdge, INotifyPropertyChanged
	{
		private SimpleGraph _graph;

		private bool _isBackreference;
        public IGraph Graph { get { return _graph; } }

        private SimpleEdge()
		{
		}

		internal SimpleEdge(SimpleGraph graph, INode from, INode to, bool isBackreference)
			: this()
		{
			_graph = graph;
			From = from;
			To = to;
			IsBackreference = isBackreference;
		}


		public INode From { get; private set; }

		public INode To { get; private set; }

		public bool IsBackreference
		{
			get { return _isBackreference; }
			set
			{
				if(_isBackreference == value)
					return;

				_isBackreference = value;
				OnPropertyChanged("IsBackreference");
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
		    var handler = PropertyChanged;
		    if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}