using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolkit
{
    public sealed class TLinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        #region Variables

        private TLinkedListNode<T> node = null;
        private int count = 0;
        private Action<TLinkedListNode<T>> onNodeAdded; /// NOT IMPLEMENTED
        private Action<TLinkedListNode<T>> onNodeRemoved;

        #endregion

        #region Properties

        public bool IsEmpty => node == null;

        public int Count => count;
        public TLinkedListNode<T> FirstNode => node;
        public TLinkedListNode<T> LastNode => node?.Previous;
        public T First {
            get {
                if(node == null)
                    return default;
                return node.Value;
            }
        }

        public T Last {
            get {
                if(node == null)
                    return default;
                return node.Previous.Value;
            }
        }

        public event Action<TLinkedListNode<T>> OnNodeAdded {
            add => onNodeAdded += value;
            remove => onNodeAdded -= value;
        }

        public event Action<TLinkedListNode<T>> OnNodeRemoved {
            add => onNodeRemoved += value;
            remove => onNodeRemoved -= value;
        }

        bool ICollection.IsSynchronized => Verify();
        object ICollection.SyncRoot => this;
        bool ICollection<T>.IsReadOnly => false;

        #endregion

        #region Add

        private TLinkedListNode<T> AddDefault(T value) {
            node = new TLinkedListNode<T>(value, this);
            node.Previous = node;
            node.Next = node;
            count++;
            return node;
        }
        private void Insert(TLinkedListNode<T>[] array, TLinkedListNode<T> start, TLinkedListNode<T> end) {
            if(array == null || array.Length == 0)
                return;
            var count = array.Length - 1;
            if(start == null && end == null) {
                if(!IsEmpty) {
                    throw new Exception("Node already has values, and insert nodes are null");
                }
                array[0].Previous = array[count];
                array[count].Next = array[0];
                node = array[0]; // Set array to this list as it creates a circular
                return;
            }
            if(start == null) {
                start = end;
            }
            if(end == null) {
                end = start;
            }
            if(start.List != this || end.List != this) {
                throw new Exception("Nodes list are not equal to this");
            }
            start.Next = array[0];
            array[0].Previous = start;
            array[count].Next = end;
            end.Previous = array[count];
        }
        private TLinkedListNode<T>[] AddArray(IList<T> list) {
            var count = list.Count;
            if(count == 0) {
                return new TLinkedListNode<T>[0];
            }
            if(count == 1) {
                return new TLinkedListNode<T>[] { new TLinkedListNode<T>(list[0], this) };
            }
            TLinkedListNode<T>[] array = new TLinkedListNode<T>[count];
            TLinkedListNode<T> t = new TLinkedListNode<T>(list[0], this);
            TLinkedListNode<T> tOld = t;
            array[0] = t;
            for(int i = 1; i < count; i++) {
                t = new TLinkedListNode<T>(list[i], this);
                tOld.Next = t;
                t.Previous = tOld;
                tOld = t;
                array[i] = t;
            }
            return array;
        }

        public TLinkedListNode<T> Add(T value)
            => AddLast(value);

        public TLinkedListNode<T> AddLast(T value) {
            if(IsEmpty) {
                return AddDefault(value);
            }
            var t = new TLinkedListNode<T>(value, this);
            t.Previous = node.Previous;
            t.Next = node;
            node.Previous.Next = t;
            node.Previous = t;
            count++;
            return t;
        }

        public TLinkedListNode<T> AddFirst(T value) {
            if(IsEmpty) {
                return AddDefault(value);
            }
            var t = new TLinkedListNode<T>(value, this);
            t.Previous = node.Previous;
            t.Next = node;
            node.Previous.Next = t;
            node.Previous = t;
            node = t;
            count++;
            return t;
        }

        public TLinkedListNode<T> AddBefore(T value, TLinkedListNode<T> otherNode) {
            if(IsEmpty) {
                throw new Exception("IT CANT BE EMPTY");
            }
            if(otherNode.List != this) {
                throw new Exception("OTHER NODE HAS WRONG LIST");
            }
            var t = new TLinkedListNode<T>(value, this);
            t.Previous = otherNode.Previous;
            t.Next = otherNode;
            otherNode.Previous.Next = t;
            otherNode.Previous = t;
            count++;
            if(otherNode == node)
                node = t;

            return t;
        }

        public TLinkedListNode<T> AddAfter(T value, TLinkedListNode<T> otherNode) {
            if(IsEmpty) {
                throw new Exception("IT CANT BE EMPTY");
            }
            if(otherNode.List != this) {
                throw new Exception("OTHER NODE HAS WRONG LIST");
            }
            var t = new TLinkedListNode<T>(value, this);
            t.Next = otherNode.Next;
            t.Previous = otherNode;
            otherNode.Next.Previous = t;
            otherNode.Next = t;
            count++;
            return t;
        }

        public TLinkedListNode<T>[] AddLast(IList<T> list) {
            var array = AddArray(list);
            Insert(array, node?.Previous, node);
            count += array.Length;
            return array;
        }

        public TLinkedListNode<T>[] AddFirst(IList<T> list) {
            var array = AddArray(list);
            Insert(array, node?.Previous, node);
            if(list.Count > 0) {
                node = array[0];
            }
            count += array.Length;
            return array;
        }

        public TLinkedListNode<T>[] AddAfter(IList<T> list, TLinkedListNode<T> otherNode) {
            var array = AddArray(list);
            Insert(array, otherNode, otherNode.Next);
            count += array.Length;
            return array;
        }

        public TLinkedListNode<T>[] AddBefore(IList<T> list, TLinkedListNode<T> otherNode) {
            var array = AddArray(list);
            Insert(array, otherNode.Previous, otherNode);
            if(otherNode == node && list.Count > 0) {
                node = array[0];
            }
            count += array.Length;
            return array;
        }

        void ICollection<T>.Add(T item) {
            AddLast(item);
        }

        #endregion

        #region Removing

        public void Clear() {
            node = null;
            count = 0;
        }

        public bool Remove(TLinkedListNode<T> node) {
            if(node == null || node.List != this) {
                return false;
            }

            onNodeRemoved?.Invoke(node);

            //node.Value = default;
            //node.List = null;

            if(count == 1) {
                this.node = null;
                count--;
                return true;
            }

            node.Next.Previous = node.Previous;
            node.Previous.Next = node.Next;

            if(this.node == node) {
                this.node = node.Next;
            }
            count--;
            return true;
        }

        public bool Remove(T value) {
            return Remove(Find(value));
        }

        #endregion

        #region Find

        public TLinkedListNode<T> Find(T value) {
            if(node == null)
                return null;
            TLinkedListNode<T> t = node;
            for(int i = 0; i < count; i++) {
                if(value.Equals(t.Value))
                    return t;
                t = t.Next;
            }
            return null;
        }

        public TLinkedListNode<T> Find(Predicate<T> predicate) {
            if(node == null)
                return null;
            TLinkedListNode<T> t = node;
            for(int i = 0; i < count; i++) {
                if(predicate(t.Value))
                    return t;
                t = t.Next;
            }
            return null;
        }

        public bool Contains(T item) {
            var node = Find(item);
            return node != null;
        }

        #endregion

        #region Index

        public int IndexOf(TLinkedListNode<T> node) {
            if(this.node == node)
                return 0;
            var t = this.node;
            for(int i = 1; i < count; i++) {
                t = t.Next;
                if(t == node) {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Verification

        public bool Verify() {
            if(count == 0 && node == null) {
                return true;
            }
            if((count == 0 && node != null) && (count > 0 && node == null)) {
                return false;
            }
            var t = node;
            for(int i = 1; i < count; i++) {
                t = t.Next;
                if(t == node) {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Enumerator

        public TLinkedListEnumerator<T> GetEnumerator() => new TLinkedListEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() {
            return new TLinkedListEnumerator<T>(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new TLinkedListEnumerator<T>(this);
        }

        #endregion

        #region CopyTo

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            foreach(var i in this) {
                array[arrayIndex++] = i;
            }
        }

        void ICollection.CopyTo(Array array, int index) {
            foreach(var i in this) {
                array.SetValue(i, index++);
            }
        }

        #endregion

        #region Safe Iterator

        public static void Iterate(TLinkedList<T> list, System.Action<T> todo){
            var iterator = list.GetEnumerator();
            TLinkedListNode<T> node;
            while(iterator.MoveNext(out node)){
                todo(node.Value);
            }
        }

        public static void IterateWithNullcheck<TI>(TLinkedList<TI> list, System.Action<TI> todo) where TI : INullable {
            var iterator = list.GetEnumerator();
            TLinkedListNode<TI> node;
            while(iterator.MoveNext(out node)){
                if(node?.Value?.IsNull ?? true)
                    iterator.DestroyCurrent();
                else
                    todo(node.Value);
            }
        }

        #endregion
    }

    public struct TLinkedListEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
    {

        #region Variables

        private TLinkedList<T> list;
        private TLinkedListNode<T> FirstNode => list.FirstNode;
        private TLinkedListNode<T> currentNode;

#if UNITY_EDITOR
        private int infiniteLoopCheck;
#endif

        #endregion

        #region Properties

        public T Current => currentNode.Value;
        public TLinkedListNode<T> Node => currentNode;
        public TLinkedList<T> List => list;
        object IEnumerator.Current => Current;

        #endregion

        #region Constructor

        public TLinkedListEnumerator(TLinkedList<T> list) {
            this.list = list;
            this.currentNode = null;

#if UNITY_EDITOR
            infiniteLoopCheck = list.Count * 2;
#endif
        }

        public TLinkedListEnumerator(TLinkedListNode<T> node) : this(node.List) { }

        #endregion

        #region Move Next

        public bool MoveNext() {
#if UNITY_EDITOR
            if(infiniteLoopCheck-- < 0) {
                UnityEngine.Debug.LogError($"[Linked List] - looped through more objects than original list had!");
                return false;
            }
#endif
            if(list == null)
                return false;
            if(currentNode == null)
                return (currentNode = FirstNode) != null;
            currentNode = currentNode.Next;
            return currentNode != FirstNode && (list != null && list.Count > 0);
        }

        public bool MoveNext(out TLinkedListNode<T> node) {
            if(MoveNext()) {
                node = currentNode;
                return true;
            }
            else {
                node = null;
                return false;
            }
        }

        public bool MoveNext(out T value) {
            if(MoveNext()) {
                value = currentNode.Value;
                return true;
            }
            else {
                value = default;
                return false;
            }
        }

        #endregion

        #region Utility

        public bool DestroyCurrent() {
            if(currentNode == FirstNode) {
                var success = list.Remove(currentNode);
                currentNode = null;
                return success;
            }
            else {
                return list.Remove(currentNode);
            }
        }

        public void Dispose() {
            list = null;
            currentNode = null;
        }

        public void Reset() {
            currentNode = FirstNode;
        }

        public TLinkedListEnumerator<T> Copy(){
            return new TLinkedListEnumerator<T>(){
                list = list,
                currentNode = currentNode,
            };
        }

        #endregion
    }

    public sealed class TLinkedListNode<T>
    {
        public T Value = default;
        public TLinkedList<T> List = default;
        public TLinkedListNode<T> Next = default;
        public TLinkedListNode<T> Previous = default;

        public TLinkedListNode(T value, TLinkedList<T> list) {
            this.Value = value;
            this.List = list;
        }
    }
}
