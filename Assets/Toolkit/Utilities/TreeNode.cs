using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit
{
    public interface ITreeNode
    {
        ITreeNode Parent { get; }
        int ChildIndex { get; }
        int ChildCount { get; }
        IReadOnlyList<ITreeNode> Children { get; }
        ITreeNode GetChild(int index);
        int GetChildIndex(ITreeNode node);
        ITreeNode Find(System.Func<ITreeNode, bool> predicate);
        ITreeNode Find(System.Func<ITreeNode, bool> predicate, bool recursive);
        IEnumerable<ITreeNode> GetEnumerable();
    }

    public class TreeNode : TreeNode<object>
    {
        private TreeNode() : base() { }
        public TreeNode(object obj) : base(obj) { }
        public TreeNode(object obj, TreeNode parent) : base(obj, parent) { }

        public static int GetPossiblePaths(ITreeNode node) {
            int count = 0;
            foreach(var n in node.GetEnumerable()) {
                if(n.ChildCount == 0)
                    count++;
            }
            return count;
        }
    }

    public class TreeNode<T> : IEnumerable<TreeNode<T>>, IEnumerable, ITreeNode
    {
        #region Variables

        private T item = default;
        private List<TreeNode<T>> children = new List<TreeNode<T>>();
        private TreeNode<T> parent = null;

        #endregion

        #region Properties

        public T Item {
            get => item;
            set => this.item = value;
        }

        public TreeNode<T> Parent => parent;
        ITreeNode ITreeNode.Parent => parent;
        public IReadOnlyList<TreeNode<T>> Children => children;
        IReadOnlyList<ITreeNode> ITreeNode.Children => children;
        public int ChildIndex => Parent?.GetChildIndex(this) ?? -1;
        public int ChildCount => children.Count;
        public int TotalChildCount {
            get {
                int count = 0;
                foreach(var n in this as IEnumerable<TreeNode<T>>) {
                    count++;
                }
                return count;
            }
        }

        #endregion

        #region Constructor

        protected TreeNode() { }

        public TreeNode(T item) => this.item = item;

        public TreeNode(T item, TreeNode<T> parent) {
            this.item = item;
            this.parent = parent;
        }

        #endregion

        #region Get Child

        public TreeNode<T> GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        ITreeNode ITreeNode.GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        #endregion

        #region Add

        public TreeNode<T> AddChild(T item) {
            var node = new TreeNode<T>(item, this);
            children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(IReadOnlyList<T> items) {
            int length = items.Count;
            TreeNode<T>[] nodes = new TreeNode<T>[length];
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T>(items[i], this);
                nodes[i] = n;
                children.Add(n);
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IReadOnlyList<T> items) {
            int length = items.Count;
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T>(items[i], this);
                children.Add(n);
            }
        }

        public TreeNode<T>[] AddChildren(IEnumerable<T> items) {
            int length = items.Count();
            int index = 0;
            TreeNode<T>[] nodes = new TreeNode<T>[length];
            foreach(var i in items) {
                var n = new TreeNode<T>(i, this);
                children.Add(n);
                nodes[index] = n;
                index++;
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IEnumerable<T> items) {
            foreach(var i in items)
                children.Add(new TreeNode<T>(i, this));
        }



        public T[] AddChildren(params T[] items) {
            foreach(var i in items)
                children.Add(new TreeNode<T>(i, this));
            return items;
        }

        public void AddChildrenNonAlloc(params T[] items) {
            foreach(var i in items)
                children.Add(new TreeNode<T>(i, this));
        }

        #endregion

        #region Remove

        public bool Remove(T item) {
            if(item == null)
                return false;
            var node = Find(item);
            if(node == null)
                return false;
            return children.Remove(node);
        }

        public void Clear() {
            children.Clear();
        }

        #endregion

        #region Find

        public TreeNode<T> Find(T item) {
            if(this.item.Equals(item))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].Equals(item))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T> Find(T item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T>>))
                    if(i.item.Equals(item))
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T> Find(System.Func<T, bool> predicate) {
            if(predicate(item))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i].item))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T> Find(System.Func<T, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T>>))
                    if(predicate(i.item))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        public TreeNode<T> Find(System.Func<TreeNode<T>, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T> Find(System.Func<TreeNode<T>, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        #endregion

        #region Child Index

        public int GetChildIndex(TreeNode<T> node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        public int GetChildIndex(ITreeNode node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        #endregion

        #region Enumerators

        IEnumerator IEnumerable.GetEnumerator() {
            yield return item;
            foreach(var n in children)
                foreach(var r in n)
                    yield return r;
        }

        IEnumerator<TreeNode<T>> IEnumerable<TreeNode<T>>.GetEnumerator() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T>>) {
                    yield return child;
                }
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetEnumerable() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T>>) {
                    yield return child;
                }
            }
        }

        #endregion

        #region ToString

        public override string ToString() {
            return $"{item?.ToString() ?? "null"}";
        }

        public string ToString(bool beautify) {
            if(!beautify)
                return ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            ToStringRecursive(sb, 0);

            return sb.ToString();
        }

        private void ToStringRecursive(System.Text.StringBuilder sb, int indent) {
            sb.AppendLine($"{BEAUTIFY_INDENT.Substring(0, indent)}{item.ToString()}");
            foreach(var child in children)
                child.ToStringRecursive(sb, indent + 1);
        }

        private const string BEAUTIFY_INDENT = "-----------------------------------------------------------";

        #endregion

        #region Other Overrides

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion
    }

    public class TreeNode<T0, T1> : IEnumerable<TreeNode<T0, T1>>, IEnumerable, ITreeNode
    {
        #region Variables

        private T0 item0 = default;
        private T1 item1 = default;
        private List<TreeNode<T0, T1>> children = new List<TreeNode<T0, T1>>();
        private TreeNode<T0, T1> parent = null;

        #endregion

        #region Properties

        public T0 Item0 {
            get => item0;
            set => this.item0 = value;
        }

        public T1 Item1 {
            get => item1;
            set => this.item1 = value;
        }

        public TreeNode<T0, T1> Parent => parent;
        ITreeNode ITreeNode.Parent => parent;
        public IReadOnlyList<TreeNode<T0, T1>> Children => children;
        IReadOnlyList<ITreeNode> ITreeNode.Children => children;
        public int ChildIndex => Parent?.GetChildIndex(this) ?? -1;
        public int ChildCount => children.Count;
        public int TotalChildCount {
            get {
                int count = 0;
                foreach(var n in this as IEnumerable<TreeNode<T0, T1>>) {
                    count++;
                }
                return count;
            }
        }

        #endregion

        #region Constructor

        protected TreeNode() { }

        public TreeNode(T0 item0) {
            this.item0 = item0;
        }

        public TreeNode(T1 item1) {
            this.item1 = item1;
        }

        public TreeNode(T0 item0, T1 item1) {
            this.item0 = item0;
            this.item1 = item1;
        }

        public TreeNode(T0 item0, T1 item1, TreeNode<T0, T1> parent) {
            this.item0 = item0;
            this.item1 = item1;
            this.parent = parent;
        }

        public TreeNode(T0 item0, TreeNode<T0, T1> parent) {
            this.item0 = item0;
            this.parent = parent;
        }

        public TreeNode(T1 item1, TreeNode<T0, T1> parent) {
            this.item1 = item1;
            this.parent = parent;
        }

        #endregion

        #region Get Child

        public TreeNode<T0, T1> GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        ITreeNode ITreeNode.GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        #endregion

        #region Add

        public T AddChild<T>(T child) where T : TreeNode<T0, T1> {
            if(child.parent == null) {
                child.parent = this;
                children.Add(child);
                return child;
            }
            return null;
        }

        public TreeNode<T0, T1> AddChild(T0 item0, T1 item1) {
            var node = new TreeNode<T0, T1>(item0, item1, this);
            children.Add(node);
            return node;
        }

        public TreeNode<T0, T1>[] AddChildren(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1) {
            int length = Math.Min(items0.Count, items1.Count);
            TreeNode<T0, T1>[] nodes = new TreeNode<T0, T1>[length];
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T0, T1>(items0[i], items1[i], this);
                nodes[i] = n;
                children.Add(n);
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1) {
            int length = Math.Min(items0.Count, items1.Count);
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T0, T1>(items0[i], items1[i], this);
                children.Add(n);
            }
        }

        public TreeNode<T0, T1>[] AddChildren(IEnumerable<T0> items0, IEnumerable<T1> items1) {
            int length = Math.Min(items0.Count(), items1.Count());
            int index = 0;
            TreeNode<T0, T1>[] nodes = new TreeNode<T0, T1>[length];

            var item0Enum = items0.GetEnumerator();
            var item1Enum = items1.GetEnumerator();

            while(item0Enum.MoveNext() && item1Enum.MoveNext()) {
                var n = new TreeNode<T0, T1>(item0Enum.Current, item1Enum.Current, this);
                children.Add(n);
                nodes[index] = n;
                index++;
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IEnumerable<T0> items0, IEnumerable<T1> items1) {
            var item0Enum = items0.GetEnumerator();
            var item1Enum = items1.GetEnumerator();

            while(item0Enum.MoveNext() && item1Enum.MoveNext()) {
                children.Add(new TreeNode<T0, T1>(item0Enum.Current, item1Enum.Current, this));
            }
        }

        #endregion

        #region Remove

        public bool Remove(T0 item0, T1 item1) {
            var node = Find(item0, item1);
            if(node == null)
                return false;
            return children.Remove(node);
        }

        public void Clear() {
            children.Clear();
        }

        #endregion

        #region Find

        public TreeNode<T0, T1> Find(T0 item) {
            if(this.item0?.Equals(item) ?? false)
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].item0?.Equals(item) ?? false)
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1> Find(T1 item) {
            if(this.item1?.Equals(item) ?? false)
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].item1?.Equals(item) ?? false)
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1> Find(T0 item0, T1 item1) {
            if((this.item0?.Equals(item0) ?? false) && (this.item1?.Equals(item1) ?? false))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if((children[i].item0?.Equals(item0) ?? false) && (children[i].item1?.Equals(item1) ?? false))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1> Find(T0 item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if(i.item0?.Equals(item) ?? false)
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T0, T1> Find(T1 item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if(i.item1?.Equals(item) ?? false)
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T0, T1> Find(T0 item0, T1 item1, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if((i.item0?.Equals(item0) ?? false) && (i.item1?.Equals(item1) ?? false))
                        return i;
                return null;
            }
            else
                return Find(item0, item1);
        }

        public TreeNode<T0, T1> Find(System.Func<T0, T1, bool> predicate) {
            if(predicate(item0, item1))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i].item0, children[i].item1))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1> Find(System.Func<T0, T1, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if(predicate(i.item0, i.item1))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }


        public TreeNode<T0, T1> Find(System.Func<TreeNode<T0, T1>, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1> Find(System.Func<TreeNode<T0, T1>, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        #endregion

        #region Child Index

        public int GetChildIndex(TreeNode<T0, T1> node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        int ITreeNode.GetChildIndex(ITreeNode node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        #endregion

        #region Enumerators

        IEnumerator IEnumerable.GetEnumerator() {
            yield return (item0, item1);
            foreach(var n in children)
                foreach(var r in n)
                    yield return r;
        }

        IEnumerator<TreeNode<T0, T1>> IEnumerable<TreeNode<T0, T1>>.GetEnumerator() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T0, T1>>) {
                    yield return child;
                }
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetEnumerable() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T0, T1>>) {
                    yield return child;
                }
            }
        }

        #endregion

        #region ToString

        public override string ToString() {
            return $"{item0?.ToString() ?? "null"}, {item1?.ToString() ?? "null"}";
        }

        public string ToString(bool beautify) {
            if(!beautify)
                return ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            ToStringRecursive(sb, 0);

            return sb.ToString();
        }

        private void ToStringRecursive(System.Text.StringBuilder sb, int indent) {
            sb.AppendLine($"{BEAUTIFY_INDENT.Substring(0, indent)}{item0?.ToString() ?? "null"}, {item1?.ToString() ?? "null"}");
            foreach(var child in children)
                child.ToStringRecursive(sb, indent + 1);
        }

        private const string BEAUTIFY_INDENT = "-----------------------------------------------------------";

        #endregion

        #region Other Overrides

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion
    }

    public class TreeNode<T0, T1, T2> : IEnumerable<TreeNode<T0, T1, T2>>, IEnumerable, ITreeNode
    {
        #region Variables

        private T0 item0 = default;
        private T1 item1 = default;
        private T2 item2 = default;
        private List<TreeNode<T0, T1, T2>> children = new List<TreeNode<T0, T1, T2>>();
        private TreeNode<T0, T1, T2> parent = null;

        #endregion

        #region Properties

        public T0 Item0 {
            get => item0;
            set => this.item0 = value;
        }

        public T1 Item1 {
            get => item1;
            set => this.item1 = value;
        }

        public T2 Item2 {
            get => item2;
            set => this.item2 = value;
        }

        public TreeNode<T0, T1, T2> Parent => parent;
        ITreeNode ITreeNode.Parent => parent;
        public IReadOnlyList<TreeNode<T0, T1, T2>> Children => children;
        IReadOnlyList<ITreeNode> ITreeNode.Children => children;
        public int ChildIndex => Parent?.GetChildIndex(this) ?? -1;
        public int ChildCount => children.Count;
        public int TotalChildCount {
            get {
                int count = 0;
                foreach(var n in this as IEnumerable<TreeNode<T0, T1, T2>>) {
                    count++;
                }
                return count;
            }
        }

        #endregion

        #region Constructor

        protected TreeNode() { }

        public TreeNode(T0 item0, T1 item1, T2 item2) {
            this.item0 = item0;
            this.item1 = item1;
            this.item2 = item2;
        }

        public TreeNode(T0 item0, T1 item1, T2 item2, TreeNode<T0, T1, T2> parent) {
            this.item0 = item0;
            this.item1 = item1;
            this.item2 = item2;
            this.parent = parent;
        }

        #endregion

        #region Get Child

        public TreeNode<T0, T1, T2> GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        ITreeNode ITreeNode.GetChild(int index) {
            if(index < 0 || index >= children.Count)
                return null;
            return children[index];
        }

        #endregion

        #region Add

        public TreeNode<T0, T1, T2> AddChild(T0 item0, T1 item1, T2 item2) {
            var node = new TreeNode<T0, T1, T2>(item0, item1, item2, this);
            children.Add(node);
            return node;
        }

        public TreeNode<T0, T1, T2>[] AddChildren(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1, IReadOnlyList<T2> items2) {
            int length = Math.Min(Math.Min(items0.Count, items1.Count), items2.Count);
            TreeNode<T0, T1, T2>[] nodes = new TreeNode<T0, T1, T2>[length];
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T0, T1, T2>(items0[i], items1[i], items2[i], this);
                nodes[i] = n;
                children.Add(n);
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1, IReadOnlyList<T2> items2) {
            int length = Math.Min(items0.Count, items1.Count);
            for(int i = 0; i < length; i++) {
                var n = new TreeNode<T0, T1, T2>(items0[i], items1[i], items2[i], this);
                children.Add(n);
            }
        }

        public TreeNode<T0, T1, T2>[] AddChildren(IEnumerable<T0> items0, IEnumerable<T1> items1, IEnumerable<T2> items2) {
            int length = Math.Min(Math.Min(items0.Count(), items1.Count()), items2.Count());
            int index = 0;
            TreeNode<T0, T1, T2>[] nodes = new TreeNode<T0, T1, T2>[length];

            var item0Enum = items0.GetEnumerator();
            var item1Enum = items1.GetEnumerator();
            var item2Enum = items2.GetEnumerator();

            while(item0Enum.MoveNext() && item1Enum.MoveNext() && item2Enum.MoveNext()) {
                var n = new TreeNode<T0, T1, T2>(item0Enum.Current, item1Enum.Current, item2Enum.Current, this);
                children.Add(n);
                nodes[index] = n;
                index++;
            }
            return nodes;
        }

        public void AddChildrenNonAlloc(IEnumerable<T0> items0, IEnumerable<T1> items1, IEnumerable<T2> items2) {
            var item0Enum = items0.GetEnumerator();
            var item1Enum = items1.GetEnumerator();
            var item2Enum = items2.GetEnumerator();

            while(item0Enum.MoveNext() && item1Enum.MoveNext() && item2Enum.MoveNext()) {
                children.Add(new TreeNode<T0, T1, T2>(item0Enum.Current, item1Enum.Current, item2Enum.Current, this));
            }
        }

        #endregion

        #region Remove

        public bool Remove(T0 item0, T1 item1, T2 item2) {
            var node = Find(item0, item1, item2);
            if(node == null)
                return false;
            return children.Remove(node);
        }

        public void Clear() {
            children.Clear();
        }

        #endregion

        #region Find

        public TreeNode<T0, T1, T2> Find(T0 item) {
            if(this.item0?.Equals(item) ?? false)
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].item0?.Equals(item) ?? false)
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(T1 item) {
            if(this.item1?.Equals(item) ?? false)
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].item1?.Equals(item) ?? false)
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(T2 item) {
            if(this.item2?.Equals(item) ?? false)
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i].item2?.Equals(item) ?? false)
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(T0 item0, T1 item1, T2 item2) {
            if((this.item0?.Equals(item0) ?? false) && (this.item1?.Equals(item1) ?? false) && (this.item2?.Equals(item1) ?? false))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if((children[i].item0?.Equals(item0) ?? false) && (children[i].item1?.Equals(item1) ?? false) && (children[i].item2?.Equals(item2) ?? false))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(T0 item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(i.item0?.Equals(item) ?? false)
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T0, T1, T2> Find(T1 item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(i.item1?.Equals(item) ?? false)
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T0, T1, T2> Find(T2 item, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(i.item2?.Equals(item) ?? false)
                        return i;
                return null;
            }
            else
                return Find(item);
        }

        public TreeNode<T0, T1, T2> Find(T0 item0, T1 item1, T2 item2, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if((i.item0?.Equals(item0) ?? false) && (i.item1?.Equals(item1) ?? false) && (i.item2?.Equals(item2) ?? false))
                        return i;
                return null;
            }
            else
                return Find(item0, item1, item2);
        }

        public TreeNode<T0, T1, T2> Find(System.Func<T0, T1, T2, bool> predicate) {
            if(predicate(item0, item1, item2))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i].item0, children[i].item1, children[i].item2))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(System.Func<T0, T1, T2, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(predicate(i.item0, i.item1, i.item2))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }


        public TreeNode<T0, T1, T2> Find(System.Func<TreeNode<T0, T1, T2>, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        public TreeNode<T0, T1, T2> Find(System.Func<TreeNode<T0, T1, T2>, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate) {
            if(predicate(this))
                return this;

            for(int i = 0, length = children.Count; i < length; i++) {
                if(predicate(children[i]))
                    return children[i];
            }
            return null;
        }

        ITreeNode ITreeNode.Find(System.Func<ITreeNode, bool> predicate, bool recursive) {
            if(recursive) {
                foreach(var i in (this as IEnumerable<TreeNode<T0, T1, T2>>))
                    if(predicate(i))
                        return i;
                return null;
            }
            else
                return Find(predicate);
        }

        #endregion

        #region Child Index

        public int GetChildIndex(TreeNode<T0, T1, T2> node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        int ITreeNode.GetChildIndex(ITreeNode node) {
            for(int i = 0, length = children.Count; i < length; i++) {
                if(children[i] == node)
                    return i;
            }
            return -1;
        }

        #endregion

        #region Enumerators

        IEnumerator IEnumerable.GetEnumerator() {
            yield return (item0, item1, item2);
            foreach(var n in children)
                foreach(var r in n)
                    yield return r;
        }

        IEnumerator<TreeNode<T0, T1, T2>> IEnumerable<TreeNode<T0, T1, T2>>.GetEnumerator() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T0, T1, T2>>) {
                    yield return child;
                }
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetEnumerable() {
            yield return this;
            foreach(var n in children) {
                foreach(var child in n as IEnumerable<TreeNode<T0, T1, T2>>) {
                    yield return child;
                }
            }
        }

        #endregion

        #region ToString

        public override string ToString() {
            return $"{item0?.ToString() ?? "null"}, {item1?.ToString() ?? "null"}, {item2?.ToString() ?? "null"}";
        }

        public string ToString(bool beautify) {
            if(!beautify)
                return ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            ToStringRecursive(sb, 0);

            return sb.ToString();
        }

        private void ToStringRecursive(System.Text.StringBuilder sb, int indent) {
            sb.AppendLine($"{BEAUTIFY_INDENT.Substring(0, indent)}{item0?.ToString() ?? "null"}, {item1?.ToString() ?? "null"}, {item2?.ToString() ?? "null"}");
            foreach(var child in children)
                child.ToStringRecursive(sb, indent + 1);
        }

        private const string BEAUTIFY_INDENT = "-----------------------------------------------------------";

        #endregion

        #region Other Overrides

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion
    }
}
