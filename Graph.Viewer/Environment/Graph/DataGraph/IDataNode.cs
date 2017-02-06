namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     �������� ����
    /// </summary>
    /// <typeparam name="TNodeData">��� ������ ����</typeparam>
    public interface IDataNode<out TNodeData> : IDataNode
    {
        /// <summary>
        ///     ������ ������ ����
        /// </summary>
        TNodeData Data { get; }
    }

    public interface IDataNode : INode
    {
        new IMutableDataGraph Graph { get; }
        object NodeData();
    }

    public interface IFlexNode : INode
    {
        void AfterAdd();
        void BeforeRemove();
    }


    public static class FelxNodeExtender
    {
        public static void AfterAdd(this INode node)
        {
            (node as IFlexNode)?.AfterAdd();
        }
        public static void BeforeRemove(this INode node)
        {
            (node as IFlexNode)?.BeforeRemove();
        }
    }
}