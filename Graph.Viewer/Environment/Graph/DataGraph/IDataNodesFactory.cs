namespace KG.SE2.Utils.Graph
{
    /// <summary>
    ///     Описание фабрики для создания узла
    /// </summary>
    /// <typeparam name="TNodeData">Тип данных узла</typeparam>
    public interface IDataNodesFactory<TNodeData>
    {
        /// <summary>
        ///     Создать узел
        /// </summary>
        /// >
        IDataNode<TNodeData> Create(TNodeData data);
    }
}