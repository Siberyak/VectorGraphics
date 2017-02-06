namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     �������� ������� ��� �������� �����
    /// </summary>
    /// <typeparam name="TEdgeData">��� ������ �����</typeparam>
    public interface IDataEdgesFactory<TEdgeData>
    {
        /// <summary>
        ///     ������� �����
        /// </summary>
        IDataEdge<TEdgeData> Create(INode @from, INode to, TEdgeData data, bool isBackreference);
    }
}