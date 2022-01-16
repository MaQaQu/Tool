namespace YouiToolkit.Design
{
    public class UndoChangeCommand<T> : UndoCommand<T>
    {
        private T newValue;
        private T oldValue;

        public UndoChangeCommand(T value)
        {
            newValue = value;
            oldValue = value;
        }

        public override void Redo(IUndoable<T> undoable)
        {
            undoable?.Redo(newValue, oldValue);
        }

        public override void Undo(IUndoable<T> undoable)
        {
            undoable?.Undo(newValue, oldValue);
        }

        public override string ToString() => $"new={newValue}, old={oldValue}";
    }
}
