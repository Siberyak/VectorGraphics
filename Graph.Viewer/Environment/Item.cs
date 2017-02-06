using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using KG.SE2.Utils.Graph;

namespace DataLayer
{
    public partial class Environment<TTimeUnit, TOffsetUnit>
    {
        public interface IItem : IDataNode
        {
            TTimeUnit Value { get; }

            TOffsetUnit ToLeft { get; }
            TOffsetUnit ToRight { get; }

            Dependency CriticalLeft { get; }
            Dependency CriticalRight { get; }

            bool TrySetValue(TTimeUnit value, out TTimeUnit? possibleValue);

            IEnumerable<Dependency> Predecessors { get; }
            IEnumerable<Dependency> Followers { get; }
            IItem Left { get; }
            IItem Right { get; }

            bool Critical { get; }

            Dependency AddPredecessor(IItem predecessor, TOffsetUnit? left = default(TOffsetUnit?), TOffsetUnit? right = default(TOffsetUnit?), object edgeData = null);
        }

        public class Item<TItemData> : DataNode<TItemData>, IItem
        {
            protected internal new Graph Graph => (Graph)base.Graph;

            public Item(Graph graph, TItemData data) : base(graph, data)
            {
            }

            private IItem _left;
            public IItem Left => _left ?? (_left = CriticalLeft?.Left ?? this);

            private IItem _right;
            public IItem Right => _right ?? (_right = CriticalRight?.Right ?? this);

            public Dependency CriticalLeft
            {
                get { return Predecessors.Any() ? Predecessors.OrderByDescending(x => x.ToLeft).FirstOrDefault() : null; }
            }

            public Dependency CriticalRight => Followers.Any() ? Followers.OrderByDescending(x => x.ToRight).FirstOrDefault() : null;

            private TOffsetUnit? _toLeft;
            public TOffsetUnit ToLeft => _toLeft ?? (_toLeft = CriticalLeft?.ToLeft ?? default(TOffsetUnit)).Value;

            private TOffsetUnit? _toRight;
            public TOffsetUnit ToRight => _toRight ?? (_toRight = CriticalRight?.ToRight ?? default(TOffsetUnit)).Value;


            public TTimeUnit Value
            {
                get
                {
                    var dependency = Predecessors.FirstOrDefault();
                    if (dependency == null)
                        return default(TTimeUnit);

                    var predecessorValue = dependency.Predecessor.Value;
                    var value = Graph.Environment.Translate(predecessorValue, dependency.Offset);
                    return value;
                }
            }

            public bool Critical => Left.Right == Right && Left == Right.Left;

            public IEnumerable<Dependency> Predecessors => BackReferences.OfType<Dependency>();
            public IEnumerable<Dependency> Followers => References.OfType<Dependency>();

            public void ResetCriticals()
            {
                _toLeft = null;
                _toRight = null;
                _left = null;
                _right = null;
            }

            public Dependency AddPredecessor(IItem predecessor, TOffsetUnit? left = default(TOffsetUnit?), TOffsetUnit? right = default(TOffsetUnit?), object edgeData = null)
            {
                if (!Graph.Environment.DependencySettings.AllowUndefinedLeft  && !left.HasValue)
                {
                    throw new NotSupportedException();
                }

                var dependency = new Dependency(Graph, predecessor, this, left, right, edgeData);

                Graph.Add((IDataEdge<object>)dependency);
                CheckPredecessor(predecessor, dependency);

                foreach (var node in Graph.Nodes<Item<TItemData>>())
                {
                    node.ResetCriticals();
                }

                foreach (var node in Graph.Edges<Dependency>())
                {
                    node.ResetCriticals();
                }

                return dependency;
            }

            bool IItem.TrySetValue(TTimeUnit value, out TTimeUnit? possibleValue)
            {
                return TrySetValueForward(value, out possibleValue, Enumerable.Empty<Dependency>());
            }

            internal bool TrySetValueForward(TTimeUnit value, out TTimeUnit? possibleValue, IEnumerable<Dependency> dependencies = null)
            {
                dependencies = dependencies ?? Enumerable.Empty<Dependency>();

                possibleValue = default(TTimeUnit);
                return false;
            }

            private void CheckPredecessor(IItem predecessor, Dependency dependency)
            {
                var nodes = Graph.Nodes.OfType<IItem>().ToList();
                var trash = RotatorHelper.Process
                    (
                        nodes,
                        x => !x.Predecessors.Any(d => nodes.Contains(d.Predecessor)),
                        x => nodes.Remove(x)
                    )
                    .ToArray();

                if (trash.Length <= 0)
                    return;

                Graph.Remove(dependency);
                throw new Exception("Try to loop detected!");
            }

            public override string ToString()
            {
                return $"[{Data}] ({ToLeft}:{ToRight})";
            }
        }

    }
}