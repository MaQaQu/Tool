namespace YouiToolkit.Design
{
    public abstract class UndoCommand<T>
    {
        /// <summary>
        /// 撤销
        /// </summary>
        public abstract void Undo(IUndoable<T> undoable);

        /// <summary>
        /// 重做
        /// </summary>
        public abstract void Redo(IUndoable<T> undoable);
    }
}
