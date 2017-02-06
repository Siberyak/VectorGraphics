using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer;

namespace KG.SE2.Utils.Graph
{
    public static class GraphExtender
    {
        #region Methods

        private static IDataEdge Edge<T>(IDataNode @from, IDataNode to, T edgeData)
        {
            if (to == null || @from == null)
            {
                return null;
            }

            IMutableDataGraph graph = to.Graph;
            if (graph != @from.Graph)
            {
                throw new Exception("ноды из разных графов");
            }

            return graph.Add(@from, to, edgeData);
        }

        /// <summary>
        ///     на кого смотрю я
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static IEnumerable<IDataNode> Referenced<TEdge>(this INode node, Func<TEdge, bool> predicate = null)
            where TEdge : IDataEdge
        {

            if (node == null)
                return Enumerable.Empty<IDataNode>();

            var enumerable = node.References
                .OfType<TEdge>();

            if(predicate != null)
                enumerable = enumerable.Where(predicate);

            var nodes = enumerable.Select(x => x.To)
                .OfType<IDataNode>()
                .ToArray();

            return nodes;
        }

        /// <summary>
        ///     кто смотри на меня
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static IEnumerable<IDataNode> ReferencedBy<TEdge>(this INode node, Func<TEdge, bool> predicate = null)
            where TEdge : IDataEdge
        {
            if (node == null)
                return Enumerable.Empty<IDataNode>();

            var enumerable = node.BackReferences
                .OfType<TEdge>();

            if (predicate != null)
                enumerable = enumerable.Where(predicate);

            return enumerable
                .Select(x => x.From)
                .OfType<IDataNode>()
                .ToArray();
        }

        private static void RemoveBackReference<TEdge>(IDataNode @from, IDataNode to, Func<TEdge, bool> predicate = null)
            where TEdge : IDataEdge
        {
            RemoveReference(to, @from, predicate);
        }

        private static void RemoveReference<TEdge>(IDataNode @from, IDataNode to, Func<TEdge, bool> predicate = null)
            where TEdge : IDataEdge
        {
            if (to == null || @from == null)
            {
                return;
            }

            IMutableDataGraph graph = to.Graph;
            if (graph != @from.Graph)
            {
                throw new Exception("ноды из разных графов");
            }

            graph.Remove(@from.References.OfType<TEdge>()
                .FirstOrDefault(x => (predicate?.Invoke(x) ?? true) && x.To == to));
        }

        #endregion


        public static IEnumerable<TNode> Nodes<TNode>(this IGraph graph, Func<TNode, bool> predicate = null)
            where TNode : INode
        {
            return predicate == null ? graph.Nodes.OfType<TNode>() : graph.Nodes.OfType<TNode>().Where(predicate);
        }


        public static IEnumerable<TEdge> Edges<TEdge>(this IGraph graph, Func<TEdge, bool> predicate = null)
            where TEdge : IEdge
        {
            return predicate == null ? graph.Edges.OfType<TEdge>() : graph.Edges.OfType<TEdge>().Where(predicate);
        }


        public static IEnumerable<TEdge> References<TEdge>(this INode node, Func<TEdge, bool> predicate = null)
            where TEdge : IEdge
        {
            return Edges(node.References, predicate);
        }

        public static IEnumerable<TEdge> BackReferences<TEdge>(this INode node, Func<TEdge, bool> predicate = null)
            where TEdge : IEdge
        {
            return Edges(node.BackReferences, predicate);
        }

        public static IEnumerable<TEdge> Edges<TEdge>(this INode node, Func<TEdge, bool> predicate = null)
            where TEdge : IEdge
        {
            return node.References(predicate).Union(node.BackReferences(predicate));
        }

        static IEnumerable<TEdge> Edges<TEdge>(IEnumerable<IEdge> edges, Func<TEdge, bool> predicate = null)
            where TEdge : IEdge
        {
            return predicate == null ? edges.OfType<TEdge>() : edges.OfType<TEdge>().Where(predicate);
        }


        public static TEdge[][] Pathes<TNode, TEdge>(this TNode from, TNode to)
            where TNode : INode
            where TEdge : IEdge
        {
            var nodes = @from.Graph.Nodes<TNode>().ToList();

            RotatorHelper.Process
                (
                    nodes,
                    node => !Equals(node, @from) && !Equals(node, to)
                            && (
                                !node.BackReferences<TEdge>(x => nodes.Contains((TNode) x.From)).Any()
                                || !node.References<TEdge>(x => nodes.Contains((TNode) x.To)).Any()
                                ),
                    x => nodes.Remove(x)
                );

            TEdge[] edges = nodes.SelectMany(x => x.References<TEdge>()).Where(x => nodes.Contains((TNode)x.To)).ToArray();

            TEdge[][] pathes = Pathes(edges, @from, to).ToArray();
            return pathes;
        }

        private static TEdge[][] Pathes<TNode, TEdge>(TEdge[] edges, TNode from, TNode to)
            where TNode : INode
            where TEdge : IEdge
        {
            if (Equals(@from, to))
                return new TEdge[][] { };

            var references = @from.References<TEdge>()
                .Where(edges.Contains)
                .ToArray();

            List<TEdge[]> pathes = new List<TEdge[]>();

            foreach (var edge in references)
            {
                var item = new[] { edge };

                if (Equals(edge.To, to))
                    pathes.Add(item);
                else
                {
                    var subpathes = Pathes(edges, edge.To, to);
                    pathes.AddRange(subpathes.Select(subpath => item.Concat(subpath).ToArray()));
                }
            }

            return pathes.ToArray();
        }

        public static void CheckPathes<TNode, TEdge>(this TEdge[][] pathes, params TNode[][] subpathes)
            where TNode : INode
            where TEdge : IEdge
        {
            Debug.Assert(pathes.Length == subpathes.Length);


            var list = pathes.Select(x => x.Select(y => y.To).ToArray()).ToList();

            foreach (var current in subpathes)
            {
                var subpath = current.Length > 0
                    ? current.Skip(1).ToArray()
                    : current;

                for (int i = 0; i < list.Count; i++)
                {
                    var items = list[i];
                    if (items.Length != subpath.Length)
                        continue;

                    var find = true;
                    for (int j = 0; j < items.Length; j++)
                    {
                        var item1 = items[j];
                        var item2 = subpath[j];

                        if (!Equals(item1, item2))
                        {
                            find = false;
                            break;
                        }
                    }

                    if (!find)
                        continue;

                    list.RemoveAt(i);
                    break;
                }
            }

            Debug.Assert(list.Count == 0);
        }

        public static TEdge[][] CriticalPathes<TNode, TEdge, TLength>(this IGraph graph, Func<TEdge, TLength> length, Func<TLength, TLength, TLength> aggregate, Func<TNode, bool> predicate = null, Func<TEdge, bool> edgresFilter = null)
            where TNode : INode
            where TEdge : IEdge
            where TLength : IComparable<TLength>
        {
            return graph.CriticalPathes(length, aggregate, Comparer<TLength>.Default, predicate, edgresFilter);
        }

        public static TEdge[][] CriticalPathes<TNode, TEdge, TLength>(this IGraph graph, Func<TEdge, TLength> length, Func<TLength, TLength, TLength> aggregate, IComparer<TLength> lengthComparer, Func<TNode, bool> nodesFilter = null, Func<TEdge, bool> edgresFilter = null)
            where TNode : INode
            where TEdge : IEdge
        {
            var lefts = graph.Nodes(nodesFilter).Where(x => !x.BackReferences<TEdge>().Any()).ToArray();
            var rights = graph.Nodes(nodesFilter).Where(x => !x.References<TEdge>().Any()).ToArray();

            var pairs = lefts.SelectMany(l => rights.Select(r => new { Left = l, Right = r }))
                .Where(x => !Equals(x.Left, x.Right))
                .ToArray();

            var withPathes = pairs.SelectMany(x => x.Left.Pathes<TNode, TEdge>(x.Right).Select(p => new {Pair = x, Path = p}));
            if (edgresFilter != null)
                withPathes = withPathes.Where(x => x.Path.All(edgresFilter));

            var withLengthes = withPathes
                .Select(x => new {x.Pair, x.Path, Length = x.Path.Select(length).Aggregate(aggregate)})
                .OrderByDescending(x => x.Length)
                .ToArray();

            var first = withLengthes.FirstOrDefault();

            return first == null 
                ? new TEdge[][] { } 
                : withLengthes.TakeWhile(x => Equals(first.Length, x.Length)).Select(x => x.Path).ToArray();
        }
    }
}