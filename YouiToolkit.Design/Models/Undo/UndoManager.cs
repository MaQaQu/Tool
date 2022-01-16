using System;
using System.Collections.Generic;
using System.Text;

namespace YouiToolkit.Design
{
    public class UndoManager<T>
    {
        public int MaxCount { get; set; } = 20;
        public int Count => list.Count;
        public int LastIndex => list.Count - 1;

        protected List<UndoCommand<T>> list = new(20);
        protected int p = -1;

        public void RemoveDirty()
        {
            int startIndex = Math.Max(p + 1, default);

            list.RemoveRange(startIndex, list.Count - startIndex);
        }

        public void Add(UndoCommand<T> command)
        {
            if (p < LastIndex)
            {
                RemoveDirty();
            }
            if (list.Count >= MaxCount)
            {
                list.RemoveAt(0);
            }
            
            list.Add(command);
            p = LastIndex;
        }

        public void Undo(IUndoable<T> undoable)
        {
            if (list.Count <= 0)
                return;

            if (p >= default(int) && p <= LastIndex)
            {
                UndoCommand<T> command = list[p];
                command.Undo(undoable);
            }
            if (p >= 0)
            {
                p--;
            }
        }

        public void Redo(IUndoable<T> undoable)
        {
            if (list.Count <= 0)
                return;

            if (p < LastIndex)
            {
                p++;
            }
            if (p >= default(int) && p <= LastIndex)
            {
                UndoCommand<T> command = list[p];
                command.Redo(undoable);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = default; i < list.Count; i++)
            {
                sb.Append($"[{i}] {list[i]}\r\n ");
            }
            return sb.ToString();
        }
    }

#if DEBUG
    public static class UndoTest
    {
        public static void Todo()
        {
            UndoManagerTest test = new();

            test.Add("1"); // 1
            test.Add("2"); // 12
            test.Add("3"); // 123
            test.Add("4"); // 1234
            test.Undo();   // 123
            test.Add("5"); // 1235
            test.Undo();   // 123
            test.Redo();   // 1235
        }
    }

    public class UndoManagerTest
    {
        public UndoableText undoable = new();
        public UndoManager<string> manager = new();

        public void Add(string t)
        {
            undoable.text += t;
            manager.Add(new UndoAddCommand<string>(t));
        }

        public void Redo()
        {
            manager.Redo(undoable);
        }

        public void Undo()
        {
            manager.Undo(undoable);
        }

        public override string ToString() => $"{undoable.text}\r\n{manager}";
    }

    public class UndoableText : IUndoable<string>
    {
        public string text = "";

        public void Redo(string newValue, string oldValue)
        {
            text += newValue;
        }

        public void Undo(string newValue, string oldValue)
        {
            text = text.Replace(oldValue, "");
        }
    }
#endif
}
