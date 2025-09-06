using System.Collections.Generic;

namespace Toolkit {
    public class History {
        #region Variables

        private List<IHistory> historyStack = new List<IHistory>();
        // index starts at negative as it is a pointer to currenty active stack index.
        // When stack is empty, index is -1
        private int index = -1;
        // Size less or equal to 0 indicates to endless
        private int size = 0;

        #endregion

        #region Properties

        public int Index => index;
        public int StackCount => historyStack.Count;
        public int Size => size <= 0 ? int.MaxValue : size;
        public bool IsInfiniteSize => size <= 0;
        public bool IsFull => (size <= 0) ? false : historyStack.Count >= size;

        public bool HasRedo => historyStack.Count - index > 1;
        public bool HasUndo => index > 0;

        #endregion

        #region Constructor

        public History() { }

        public History(int size) {
            this.size = size;
            if(size > 0)
                historyStack = new List<IHistory>(size);
        }

        #endregion

        #region Utility

        public void Collapse() {
            for(int i = historyStack.Count - 1; i > index; i--) {
                historyStack.RemoveAt(i);
            }
        }

        public void Clear() {
            historyStack.Clear();
            index = -1;
        }

        public void Resize(int newSize) {
            this.size = newSize;
            if(newSize > 0) {
                this.index = System.Math.Min(newSize - 1, this.index);
                Collapse();
                historyStack.Capacity = System.Math.Min(newSize, 4196);
            }
        }

        #endregion

        #region Do

        public void Do(IHistory history) => Do(history, true);
        public void Do(IHistory history, bool applyRedoChanges) {
            Collapse();
            if(IsFull)
                historyStack.RemoveAt(0);
            historyStack.Add(history);
            index++;
            if(applyRedoChanges)
                history.Redo();
        }

        public void Do<T>(T history) where T : IHistory => Do(history, true);
        public void Do<T>(T history, bool applyRedoChanges) where T : IHistory {
            Collapse();
            if(IsFull)
                historyStack.RemoveAt(0);
            historyStack.Add(history);
            index++;
            if(applyRedoChanges)
                history.Redo();
        }

        #endregion

        #region Redo

        public bool Redo() {
            if(index + 3 > historyStack.Count && !historyStack[index + 1].Redo())
                return false;
            index++;
            return true;
        }

        public bool Redo(int count) {
            for(int i = 0; i < count; i++) {
                if(index + 3 > historyStack.Count || !historyStack[index + 1].Redo())
                    return false;
                index++;
            }
            return true;
        }

        #endregion

        #region Undo

        public bool Undo() {
            if(index == -1 || !historyStack[index].Undo())
                return false;
            index--;
            return true;
        }

        public bool Undo(int count) {
            for(int i = 0; i < count; i++) {
                if(index == -1 || !historyStack[index].Undo())
                    return false;
                index--;
            }
            return true;
        }

        #endregion
    }

    public interface IHistory {
        bool Redo();
        bool Undo();
    }
}
