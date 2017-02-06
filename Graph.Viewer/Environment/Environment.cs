using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KG.SE2.Utils.Graph;

namespace DataLayer
{
    public class DependencySettings
    {
        public DependencySettings(bool allowUndefinedLeft)
        {
            AllowUndefinedLeft = allowUndefinedLeft;
        }

        public bool AllowUndefinedLeft { get; }
        public bool AllowUndefinedRight { get; }

    }

    public partial class Environment<TTimeUnit, TOffsetUnit>
        where TTimeUnit : struct, IComparable<TTimeUnit>
        where TOffsetUnit : struct, IComparable<TOffsetUnit>
    {
        private readonly Func<TTimeUnit, TOffsetUnit, TTimeUnit> _translateTimeByOffsetFunc;
        private readonly Func<TTimeUnit, TTimeUnit, TOffsetUnit> _offsetFunc;
        private readonly Func<TOffsetUnit, TOffsetUnit, TOffsetUnit> _aggregateOffsets;

        private readonly Graph _graph;
        public readonly Absolute AbsoluteItem;
        private Func<TOffsetUnit, TOffsetUnit> _negateOffset;

        public DependencySettings DependencySettings { get; }

        public Environment(Func<TTimeUnit, TOffsetUnit, TTimeUnit> translateTimeByOffsetFunc, Func<TTimeUnit, TTimeUnit, TOffsetUnit> offsetFunc, Func<TOffsetUnit, TOffsetUnit, TOffsetUnit> aggregateOffsets, Func<TOffsetUnit, TOffsetUnit> negateOffset, bool allowUndefinedLeft = true)
        {
            DependencySettings = new DependencySettings(allowUndefinedLeft);

            _translateTimeByOffsetFunc = translateTimeByOffsetFunc;
            _offsetFunc = offsetFunc;
            _aggregateOffsets = aggregateOffsets;
            _negateOffset = negateOffset;


            _graph = new Graph(this);

            _graph.Add((IDataNode<string>)(AbsoluteItem = new Absolute(_graph)));
        }

        public TTimeUnit? Translate(TTimeUnit time, TOffsetUnit? offset)
        {
            return offset.HasValue ? _translateTimeByOffsetFunc(time, offset.Value) : default (TTimeUnit?);
        }
        public TTimeUnit Translate(TTimeUnit time, TOffsetUnit offset)
        {
            return _translateTimeByOffsetFunc(time, offset);
        }

        public Item<TItemData> AddItem<TItemData>(TItemData data)
        {
            var item = new Item<TItemData>(_graph, data);
            _graph.Add((IDataNode<TItemData>)item);
            return item;
        }

        public TOffsetUnit Min(TOffsetUnit a, TOffsetUnit b)
        {
            return a.CompareTo(b) < 0 ? a : b;
        }
        public TOffsetUnit Max(TOffsetUnit a, TOffsetUnit b)
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public TTimeUnit Min(TTimeUnit a, TTimeUnit b)
        {
            return a.CompareTo(b) < 0 ? a : b;
        }
        public TTimeUnit Max(TTimeUnit a, TTimeUnit b)
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        private static TTimeUnit? MinMax(TTimeUnit? a, TTimeUnit? b, Func<TTimeUnit, TTimeUnit, TTimeUnit> result)
        {
            var c = a ?? b;
            if (!c.HasValue)
                return default(TTimeUnit?);

            if (!a.HasValue)
                return b.Value;

            if (!b.HasValue)
                return a.Value;

            return result(a.Value, b.Value);
        }
        public TTimeUnit? Min(TTimeUnit? a, TTimeUnit? b)
        {
            return MinMax(a, b, Min);
        }
        public TTimeUnit? Max(TTimeUnit? a, TTimeUnit? b)
        {
            return MinMax(a, b, Max);
        }

        //private TOffsetUnit CalcOffset(TTimeUnit time, TimeInterval posibleInterval)
        //{
        //    if(posibleInterval.Incorrect)
        //        throw new ArgumentOutOfRangeException();

        //    var left = posibleInterval.Left ?? time;
        //    var right = posibleInterval.Right ?? time;

        //    var moveToRight = time.CompareTo(left) < 0;
        //    var moveToLeft = time.CompareTo(right) > 0;

        //    var offset = !moveToLeft && !moveToRight
        //        ? default(TOffsetUnit)
        //        : moveToRight
        //            ? Offset(time, left)
        //            : Offset(right, time);

        //    return offset;
        //}

        private TOffsetUnit Offset(TTimeUnit left, TTimeUnit right)
        {
            return _offsetFunc(left, right);
        }

        public Dependency[][] Pathes(IItem from, IItem to)
        {
            return @from.Pathes<IItem, Dependency>(to);
        }

        public void Check(Dependency dependency, TTimeUnit followerValue, TOffsetUnit offset)
        {
            return;
            Debug.Assert(Equals(dependency.Follower.Value, followerValue));
            Debug.Assert(Equals(dependency.Offset, offset));
        }

        public void CheckPathes(Dependency[][] pathes, params IItem[][] subpathes)
        {
            pathes.CheckPathes(subpathes);
        }

        public Dependency[][] CriticalPathes()
        {
            var criticalPathes = _graph.CriticalPathes<IItem, Dependency, TOffsetUnit>(x => x.OffsetBounds.Left.Value, _aggregateOffsets, x => !(x is Absolute), x => x.OffsetBounds.Left.HasValue);
            return criticalPathes;
        }

        public TOffsetUnit Aggregate(TOffsetUnit a, TOffsetUnit b)
        {
            return _aggregateOffsets(a, b);
        }

        public TOffsetUnit? Aggregate(TOffsetUnit? a, TOffsetUnit? b)
        {
            return a.HasValue && b.HasValue ? Aggregate(a.Value, b.Value) : default(TOffsetUnit?);
        }

        private class NullableComparer<T> : IComparer<T?>
            where T : struct, IComparable<T>
        {
            int IComparer<T?>.Compare(T? x, T? y)
            {
                if (x.HasValue && y.HasValue)
                    return x.Value.CompareTo(y.Value);

                if (x.HasValue)
                    return 1;

                if (y.HasValue)
                    return -1;

                return 0;
            }
        }

        private TOffsetUnit? Negate(TOffsetUnit value)
        {
            return _negateOffset(value);
        }
        private TOffsetUnit? Negate(TOffsetUnit? value)
        {
            return value.HasValue ? Negate(value.Value) : null;
        }

        public IEnumerable<T> Nodes<T>(Func<T, bool> predicate = null) 
            where T : INode
        {
            return _graph.Nodes(predicate);
        }
    }
}