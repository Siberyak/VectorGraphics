namespace Shapes
{
    public interface IViewPortInputAction
    {
        bool Activate(IInputInfo info);
        bool Deactivate(IInputInfo info);
        bool Process(IInputInfo info);
    }
}