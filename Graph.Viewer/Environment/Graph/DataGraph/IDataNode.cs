namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     Описание узла
    /// </summary>
    /// <typeparam name="TNodeData">Тип данных узла</typeparam>
    public interface IDataNode<out TNodeData> : IDataNode
    {
        /// <summary>
        ///     Объект данных узла
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