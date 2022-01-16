namespace YouiToolkit.Design
{
    public interface IUndoable<T>
    {
        void Redo(T newValue, T oldValue);
        void Undo(T newValue, T oldValue);
    }
}
