namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     Описание фабрики для создания связи
    /// </summary>
    /// <typeparam name="TEdgeData">Тип данных связи</typeparam>
    public interface IDataEdgesFactory<TEdgeData>
    {
        /// <summary>
        ///     Создать связь
        /// </summary>
        IDataEdge<TEdgeData> Create(INode @from, INode to, TEdgeData data, bool isBackreference);
    }
}