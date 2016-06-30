using System;
using System.Collections.Generic;
using System.Linq;

namespace Shapes
{
    public class UndoRedoScope
    {
        readonly Dictionary<UndoRedoScopeItemInfo, IUndoRedoScopeItem> _items = new Dictionary<UndoRedoScopeItemInfo, IUndoRedoScopeItem>();
        readonly Stack<UndoRedoScopeItemInfo> _undoStack = new Stack<UndoRedoScopeItemInfo>();
        readonly Stack<UndoRedoScopeItemInfo> _redoStack = new Stack<UndoRedoScopeItemInfo>();

        public bool CanUndo => _undoStack.Any();
        public bool CanRedo => _redoStack.Any();


        public void Do(IUndoRedoScopeItem item)
        {
            if (item == null)
                return;

            if (!item.IsDone)
                item.Do();

            foreach (var current in _redoStack.ToArray())
            {
                _items.Remove(current);
            }

            _redoStack.Clear();

            var info = new UndoRedoScopeItemInfo(_undoStack.Count + 1, item.ToString());
            _items.Add(info, item);
            _undoStack.Push(info);
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            var info = _undoStack.Pop();
            var item = _items[info];
            item.Undo();

            var stack = (item.IsDone ? _undoStack : _redoStack);
            stack.Push(info);
            info.Index = stack.Count;
        }


        public void Undo(UndoRedoScopeItemInfo item)
        {
            if(!_undoStack.Contains(item))
                return;

            while (_undoStack.Peek() != item)
            {
                Undo();
            }

            Undo();
        }


        public void Redo()
        {
            if (!CanRedo)
                return;

            var info = _redoStack.Pop();

            var item = _items[info];
            item.Do();

            var stack = (item.IsDone ? _undoStack : _redoStack);
            stack.Push(info);
            info.Index = stack.Count;
        }

        public void Redo(UndoRedoScopeItemInfo item)
        {
            if (!_redoStack.Contains(item))
                return;

            while (_redoStack.Peek() != item)
            {
                Redo();
            }

            Redo();
        }


        public IEnumerable<UndoRedoScopeItemInfo> UndoItems => _undoStack.ToArray();
        public IEnumerable<UndoRedoScopeItemInfo> RedoItems => _redoStack.ToArray();
    }

    public class UndoRedoScopeItemInfo
    {
        public UndoRedoScopeItemInfo(int index, string caption)
        {
            Index = index;
            Caption = caption;
        }

        public int Index { get; internal set; }
        public string Caption { get; }
        public override string ToString()
        {
            return $"{Index}. {Caption}";
        }
    }

    public interface IUndoRedoScopeItem
    {
        /// <summary>
        /// On implimentation: 
        /// executing Do() must chenge value to [true], 
        /// executing Undo() must chenge value to [false].
        /// </summary>
        bool IsDone { get; }
        void Undo();
        void Do();
    }

    public class SimpleUndoRedoScopeItem : IUndoRedoScopeItem
    {
        private readonly Action _do;
        private readonly Action _undo;

        public SimpleUndoRedoScopeItem(Action @do, Action undo, string caption = null, bool isDone = false)
        {
            if (@do == null)
                throw new ArgumentNullException(nameof(@do));
            if (undo == null)
                throw new ArgumentNullException(nameof(undo));

            _do = @do;
            _undo = undo;
            Caption = caption;
            IsDone = isDone;
        }

        public string Caption { get; }
        public bool IsDone { get; private set; }

        public void Undo()
        {
            if (!IsDone)
                return;

            _undo();
            IsDone = false;
        }

        public void Do()
        {
            if (IsDone)
                return;

            _do();
            IsDone = true;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Caption) ? base.ToString() : Caption;
        }

        public static UndoRedoScopeItemInfo Do(UndoRedoScope scope, Action @do, Action undo, string caption = null)
        {
            var item = new SimpleUndoRedoScopeItem(@do, undo, caption);
            scope.Do(item);
            return scope.UndoItems.First();
        }
    }
}