namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     �������� ����������� �����
    /// </summary>
    public interface IMutableDataGraph : IGraph
    {
        /// <summary>
        ///     �������� ����
        /// </summary>
        IDataNode<TNodeData> Add<TNodeData>(TNodeData data);

        /// <summary>
        ///     �������� �����
        /// </summary>
        IDataEdge<TEdgeData> Add<TEdgeData>(INode from, INode to, TEdgeData data = default(TEdgeData), bool isBackreference = false);

        /// <summary>
        ///     ���������� ���� �� ������ ����
        /// </summary>
        /// <typeparam name="TNodeData">��� ������ ����</typeparam>
        /// <param name="data">������ ������ ����</param>
        /// <returns>����</returns>
        IDataNode<TNodeData> FindNode<TNodeData>(TNodeData data);

        /// <summary>
        ///     ���������� ����� �� ������ �����
        /// </summary>
        /// <typeparam name="TEdgeData">��� ������ �����</typeparam>
        /// <param name="data">������ ������ �����</param>
        /// <returns>�����</returns>
        IDataEdge<TEdgeData> FindEdge<TEdgeData>(TEdgeData data);

        /// <summary>
        ///     ������� ����
        /// </summary>
        void Remove(INode node);

        /// <summary>
        ///     ������� �����
        /// </summary>
        void Remove(IEdge edge);
    }
}