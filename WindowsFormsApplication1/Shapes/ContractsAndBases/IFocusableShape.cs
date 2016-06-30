namespace Shapes
{
    public interface IFocusableShape : ISelectableShape
    {
        bool AllowFocus { get; }
    }
}