using System;
using System.Collections.Generic;
using System.Linq;
using KG.SE2.Utils.Graph;

namespace DataLayer
{
    public partial class Environment<TTimeUnit, TOffsetUnit>
    {
        public class Absolute : DataNode<string>//, IItem
        {
            private readonly TTimeUnit _defaultTimeUnit = default(TTimeUnit);
            public TTimeUnit Value => _defaultTimeUnit;
            public TOffsetUnit ToLeft { get { throw new NotImplementedException(); } }
            public TOffsetUnit ToRight { get { throw new NotImplementedException(); } }
            public bool Critical { get { throw new NotImplementedException(); } }
            public Dependency CriticalLeft { get { throw new NotImplementedException(); } }
            public Dependency CriticalRight { get { throw new NotImplementedException(); } }

            public bool TrySetValue(TTimeUnit value, out TTimeUnit? possibleValue)
            {
                possibleValue = default(TTimeUnit?);
                return false;
            }

            public IEnumerable<Dependency> Predecessors => Enumerable.Empty<Dependency>();
            public IEnumerable<Dependency> Followers => References.OfType<Dependency>();

            public Dependency AddPredecessor(IItem predecessor, TOffsetUnit? left = null, TOffsetUnit? right = null)
            {
                throw new NotSupportedException();
            }

            internal Absolute(MutableDataGraph dataGraph) : base(dataGraph, "Absolute")
            {
                if (dataGraph.Nodes.OfType<Absolute>().Any())
                    throw new NotSupportedException("Absolute can be only one!");
            }
        }
    }
}