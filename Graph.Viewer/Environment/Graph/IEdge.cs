namespace KG.SE2.Utils.Graph
{
    public interface IEdge
    {
        //string Caption { get; }
        IGraph Graph { get; }
        INode From { get; }
        INode To { get; }

        //RelationTypes RelationType { get; }
        //Multiplicity Multiplicity { get; }
        bool IsBackreference { get; }
	}
}