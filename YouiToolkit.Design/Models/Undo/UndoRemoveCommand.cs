namespace YouiToolkit.Design
{
    public class UndoRemoveCommand<T> : UndoCommand<T>
    {
        private T addValue;
        private T rmValue;

        public UndoRemoveCommand(T value)
        {
            addValue = value;
            rmValue = value;
        }

        public override void Redo(IUndoable<T> undoable)
        {
            undoable?.Redo(addValue, rmValue);
        }

        public override void Undo(IUndoable<T> undoable)
        {
            undoable?.Undo(addValue, rmValue);
        }

        public override string ToString() => $"add={addValue}, rm={rmValue}";
    }
}
