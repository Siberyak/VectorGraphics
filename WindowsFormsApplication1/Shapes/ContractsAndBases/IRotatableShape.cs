using System;

namespace Shapes
{
    public interface IRotatableShape : IShape
    {
        float Rotation { get; }
        //float RotationOffset { get; set; }

        bool AllowRotate { get; set; }
        //void ApplyRotation();
        //void CancelRotation();

        event RotationChangedEventHandler RotationChanged;
    }

    public delegate void RotationChangedEventHandler(IRotatableShape shape, float oldRotation);


    public interface IValueWithDeviation<TOwner, T>
    {
        TOwner Owner { get; }
        T Value { get; }
        T Deviation { get; set; }

        void Accept();
        void Reject();

        event DeviateEventHandler<TOwner, T> Deviate;
        event AcceptedEventHandler<TOwner, T> Accepted;
        event RejectedEventHandler<TOwner, T> Rejected;
    }

    public class ValueWithDeviation<TOwner, T> : IValueWithDeviation<TOwner, T>
    {
        public ValueWithDeviation(TOwner owner)
        {
            Owner = owner;
        }

        public TOwner Owner { get; }
        public T Value { get; }
        public T Deviation { get; set; }

        public void Accept() { }
        public void Reject() { }

        public event DeviateEventHandler<TOwner, T> Deviate;
        public event AcceptedEventHandler<TOwner, T> Accepted;
        public event RejectedEventHandler<TOwner, T> Rejected;
    }

    public delegate void DeviateEventHandler<TOwner, T>(ValueWithDeviation<TOwner, T> valueWithDeviation, T prevDeviation);
    public delegate void AcceptedEventHandler<TOwner, T>(ValueWithDeviation<TOwner, T> valueWithDeviation, T prevValue);
    public delegate void RejectedEventHandler<TOwner, T>(ValueWithDeviation<TOwner, T> valueWithDeviation, T deviation);

    public interface IDDD
    {
        IValueWithDeviation<IDDD, Vector2F> Location { get; }
    }

    public interface IDDD2
    {
        IValueWithDeviation<IDDD2, float> Rotation { get; }
    }

    public class DDD : IDDD, IDDD2
    {
        public IValueWithDeviation<IDDD, Vector2F> Location { get; }
        public IValueWithDeviation<IDDD2, float> Rotation { get; }

        public DDD()
        {
            Location = new ValueWithDeviation<IDDD, Vector2F>(this);
            Rotation = new ValueWithDeviation<IDDD2, float>(this);
        }
    }
}