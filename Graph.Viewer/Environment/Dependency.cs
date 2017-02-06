using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using KG.SE2.Utils.Graph;

namespace DataLayer
{
    public partial class Environment<TTimeUnit, TOffsetUnit>
    {
        
        public struct TimeInterval
        {

            private readonly Environment<TTimeUnit, TOffsetUnit> _environment;
            public TTimeUnit? Left { get; }
            public TTimeUnit? Right { get; }
            public bool Incorrect { get; }
            public bool Correct => !Incorrect;

            public TOffsetUnit? Offset { get; }

            public TimeInterval(Environment<TTimeUnit, TOffsetUnit> environment, TTimeUnit? left, TTimeUnit? right)
            {
                _environment = environment;
                Left = left;
                Right = right;

                var bothDefined = Left.HasValue && Right.HasValue;
                Incorrect = bothDefined && Left.Value.CompareTo(Right.Value) > 0;
                Offset = !Incorrect && bothDefined ? _environment.Offset(left.Value, right.Value) : default(TOffsetUnit?);
            }

            public TimeInterval Aggregate(TimeInterval interval)
            {
                var left = _environment.Max(Left, interval.Left);
                var right = _environment.Min(Right, interval.Right);

                return new TimeInterval(_environment, left, right);
            }

            public override string ToString()
            {
                return $"{(Incorrect ? '>' :'<')}{Left}-{Right}{(Incorrect ? '<' : '>')}";
            }
        }

        public struct OffsetInterval
        {
            private readonly Environment<TTimeUnit, TOffsetUnit> _environment;
            public TOffsetUnit? Left { get; }
            public TOffsetUnit? Right { get; }

            public bool Incorrect { get; }

            public OffsetInterval(Environment<TTimeUnit, TOffsetUnit> environment, TOffsetUnit? left, TOffsetUnit? right)
            {
                _environment = environment;
                Left = left;
                Right = right;
                Incorrect = Left.HasValue && Right.HasValue && Left.Value.CompareTo(Right.Value) > 0;
            }

            public TimeInterval Translate(TTimeUnit time)
            {
                var left = _environment.Translate(time, Left);
                var right = _environment.Translate(time, Right);

                return new TimeInterval(_environment, left, right);
            }

            public override string ToString()
            {
                var invert = Left.HasValue && Right.HasValue && Left.Value.CompareTo(Right.Value) > 0;

                //var left = invert ? Left : Right;
                //var right = invert ? Right : Left;

                return $"{(invert ? '>' : '<')}{Left}-{Right}{(invert ? '<' : '>')}";
            }

        }

        public class Dependency : DataEdge<object>
        {
            public new Graph Graph => (Graph) base.Graph;

            private TOffsetUnit _offset;

            public OffsetInterval OffsetBounds { get; }

            public TOffsetUnit Offset
            {
                get { return _offset; }
                set { _offset = value; }
            }

            public IItem Predecessor => (IItem)From;
            public IItem Follower => (IItem)To;

            public Dependency(Graph dataGraph, IItem @from, IItem to, TOffsetUnit? left = default(TOffsetUnit?), TOffsetUnit? right = default(TOffsetUnit?), object data = null)
                : base(dataGraph, data, @from, to, false)
            {
                if (!(left ?? right).HasValue)
                    throw new NotSupportedException();

                var interval = new OffsetInterval(dataGraph.Environment, left, right);
                if(interval.Incorrect)
                    throw new ArgumentException();

                OffsetBounds = interval;
            }

            public override string ToString()
            {
                return $"{From} -> {To} {OffsetBounds}";
            }

            private TOffsetUnit? _toLeft;

            public TOffsetUnit ToLeft =>
                true || OffsetBounds.Left.HasValue
                    ? _toLeft ?? (_toLeft = Graph.Environment.Aggregate(Predecessor.ToLeft, OffsetBounds.Left ?? OffsetBounds.Right)).Value
                    : default(TOffsetUnit);

            private TOffsetUnit? _toRight;

            public TOffsetUnit ToRight =>
                true || OffsetBounds.Left.HasValue
                    ? _toRight ?? (_toRight = Graph.Environment.Aggregate(Follower.ToRight, OffsetBounds.Left ?? OffsetBounds.Right)).Value
                    : default(TOffsetUnit);

            public void ResetCriticals()
            {
                _toLeft = null;
                _toRight = null;
            }

            public IItem Left => true || OffsetBounds.Left.HasValue ? Predecessor.Left : Follower;
            public IItem Right => true || OffsetBounds.Left.HasValue ? Follower.Right : Predecessor;
        }
    }
}