namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     �������� �����
    /// </summary>
    /// <typeparam name="TEdgeData">��� ������ �����</typeparam>
    public interface IDataEdge<out TEdgeData> : IDataEdge
    {
        /// <summary>
        ///     ������ ������ �����
        /// </summary>
        TEdgeData Data { get; }
    }

    public interface IDataEdge : IEdge
    {
        new IMutableDataGraph Graph { get; }
        object EdgeData();
    }
}