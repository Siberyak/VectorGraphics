using System;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public enum ViewPortEventType
    {
        None = 0,
        MouseDown, MouseUp, MouseMove, MouseWheel, MouseClick, MouseDoubleClick,
        
        /// <summary>
        /// кроме стрелок
        /// </summary>
        KeyDown,

        /// <summary>
        /// кроме стрелок
        /// </summary>
        KeyUp, 
        /// <summary>
        /// для стрелок: для них кидаются KeyDown и KeyUp
        /// </summary>
        KeyPress
    }

    public class MoveMouseActionInfo : IEquatable<MoveMouseActionInfo>
    {
        public MoveMouseActionInfo(Func<MouseEventArgs, Keys, bool> isSuitable, MouseMoveAction startMove, MouseMoveAction move, MouseMoveAction endMove)
        {
            _isSuitable = isSuitable;
            StartMove = startMove;
            Move = move;
            EndMove = endMove;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) State;
                hashCode = (hashCode*397) ^ (StartMove != null ? StartMove.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Move != null ? Move.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (EndMove != null ? EndMove.GetHashCode() : 0);
                return hashCode;
            }
        }

        public ActionState State { get; private set; }

        protected Func<MouseEventArgs, Keys, bool> _isSuitable { get; private set; }
        protected MouseMoveAction StartMove { get; private set; }
        protected MouseMoveAction Move { get; private set; }
        protected MouseMoveAction EndMove { get; private set; }


        public bool IsSuitable(MouseEventArgs e, Keys keys)
        {
            if (_isSuitable(e, keys)) 
                return true;

            if (State != ActionState.Processing) 
                return false;

            State = ActionState.Ending;
            return true;
        }

        public void Process(Vector2F offset, MouseEventArgs e)
        {
            switch (State)
            {
                case ActionState.None:
                    if (StartMove != null)
                        StartMove(offset, e);
                    State = ActionState.Started;
                    break;
                case ActionState.Started:
                case ActionState.Processing:
                    if (Move != null)
                        Move(offset, e);
                    State = ActionState.Processing;
                    break;
                case ActionState.Ending:
                    if (EndMove != null)
                        EndMove(offset, e);
                    State = ActionState.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MoveMouseActionInfo) obj);
        }

        public virtual bool Equals(MoveMouseActionInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return State == other.State && Equals(StartMove, other.StartMove) && Equals(Move, other.Move) && Equals(EndMove, other.EndMove);
        }


    }
}