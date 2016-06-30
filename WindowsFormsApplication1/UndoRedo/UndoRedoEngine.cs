namespace Shapes
{
    public static class UndoRedoEngine
    {
        public static UndoRedoScope Scope()
        {
            return new UndoRedoScope();
        }
    }
}