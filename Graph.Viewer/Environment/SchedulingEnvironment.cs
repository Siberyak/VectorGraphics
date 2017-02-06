using System;

namespace DataLayer
{
    public partial class SchedulingEnvironment<TTimeUnit, TOffsetUnit>
        where TTimeUnit : struct, IComparable<TTimeUnit>
        where TOffsetUnit : struct, IComparable<TOffsetUnit>
    {
        Environment<TTimeUnit, TOffsetUnit> _environment;

        public SchedulingEnvironment(Func<TTimeUnit, TOffsetUnit, TTimeUnit> translateTimeByOffsetFunc, Func<TTimeUnit, TTimeUnit, TOffsetUnit> offsetFunc, Func<TOffsetUnit, TOffsetUnit, TOffsetUnit> aggregateOffsets, Func<TOffsetUnit, TOffsetUnit> negateOffset, bool allowUndefinedLeft = true)
        {
            _environment = new Environment<TTimeUnit, TOffsetUnit>(translateTimeByOffsetFunc, offsetFunc, aggregateOffsets, negateOffset, allowUndefinedLeft);
        }


        public Task AddTask(Guid id, TOffsetUnit? minDuration, TOffsetUnit? maxDuration)
        {
            return new Task(_environment, id, minDuration, maxDuration);
        }
        
        public enum DependecyType
        {
            FinishStart,
            FinishFinish,
            StartStart,
            StartFinish
        }

        public abstract class ProjectItem
        {
            protected internal readonly Environment<TTimeUnit, TOffsetUnit> Environment;
            protected internal readonly Guid ID;
            protected internal readonly Environment<TTimeUnit, TOffsetUnit>.Item<string> Start;
            protected internal readonly Environment<TTimeUnit, TOffsetUnit>.Item<string> Finish;

            private ProjectItem(Environment<TTimeUnit, TOffsetUnit> environment, Guid id)
            {
                Environment = environment;
                ID = id;

            }

            protected ProjectItem(Environment<TTimeUnit, TOffsetUnit> environment, Guid id, TOffsetUnit? minDuration, TOffsetUnit? maxDuration)
                : this(environment, id)
            {
                if (!(minDuration ?? maxDuration).HasValue)
                    throw new NotSupportedException();
                
                Start = Environment.AddItem($"{GetType().Name}.{id}.start"); 
                Finish = Environment.AddItem($"{GetType().Name}.{id}.finish");
                Finish.AddPredecessor(Start, minDuration, maxDuration);
            }

            public void AddPredecessor(ProjectItem predecessor, DependecyType dependecyType, TOffsetUnit? left, TOffsetUnit? right)
            {
                switch (dependecyType)
                {
                    case DependecyType.FinishStart:
                        Start.AddPredecessor(predecessor.Finish, left, right, dependecyType);
                        break;
                    case DependecyType.FinishFinish:
                        Finish.AddPredecessor(predecessor.Finish, left, right, dependecyType);
                        break;
                    case DependecyType.StartStart:
                        Start.AddPredecessor(predecessor.Start, left, right, dependecyType);
                        break;
                    case DependecyType.StartFinish:
                        Finish.AddPredecessor(predecessor.Start, left, right, dependecyType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dependecyType), dependecyType, null);
                }
            }
        }

        public class Task : ProjectItem
        {
            public Task(Environment<TTimeUnit, TOffsetUnit> environment, Guid id, TOffsetUnit? minDuration, TOffsetUnit? maxDuration) : base(environment, id, minDuration, maxDuration)
            {
            }
        }
    }
}