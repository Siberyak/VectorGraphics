using System.Collections.Generic;

namespace KG.SE2.Utils.Graph
{
    public interface INode
    {
        IGraph Graph { get; }
        IEnumerable<IEdge> References { get; }
        IEnumerable<IEdge> BackReferences { get; }
    }
}