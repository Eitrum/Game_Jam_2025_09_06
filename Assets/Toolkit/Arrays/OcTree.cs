using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public class OcTree
    {
        #region Const

        public const int DEFAULT_CAPACITY = 64;
        public const int DEFAULT_DEPTH = 8;
        public const float DEFAULT_REMOVE_ACCURACY = 0.05f;

        #endregion

        #region Variables

        private Bounds area;
        private int capacity = 0;
        private int depth = 0;
        private List<Vector3> points;

        private bool subdivided = false;
        private OcTree qt0;
        private OcTree qt1;
        private OcTree qt2;
        private OcTree qt3;
        private OcTree qt4;
        private OcTree qt5;
        private OcTree qt6;
        private OcTree qt7;

        #endregion

        #region Properties

        public Bounds Area => area;
        public int Capacity => capacity;
        public int Length => points.Count;
        public int Depth => depth;

        public IReadOnlyList<Vector3> Points => points;
        public bool IsSubdivided => subdivided;
        public OcTree NorthWestBot => qt0;
        public OcTree NorthEastBot => qt1;
        public OcTree SouthWestBot => qt2;
        public OcTree SouthEastBot => qt3;
        public OcTree NorthWestTop => qt4;
        public OcTree NorthEastTop => qt5;
        public OcTree SouthWestTop => qt6;
        public OcTree SouthEastTop => qt7;

        #endregion

        #region Constructor

        public OcTree(float size) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3())) { }
        public OcTree(float size, int capacity) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3()), capacity, DEFAULT_DEPTH) { }
        public OcTree(float size, int capacity, int depth) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3()), capacity, depth) { }

        public OcTree(Vector3 center, float size) : this(new Bounds(center, new Vector3(size, size, size))) { }
        public OcTree(Vector3 center, float size, int capacity) : this(new Bounds(center, size.To_Vector3()), capacity, DEFAULT_DEPTH) { }
        public OcTree(Vector3 center, float size, int capacity, int depth) : this(new Bounds(center, size.To_Vector3()), capacity, depth) { }

        public OcTree(Vector3 center, Vector3 size) : this(new Bounds(center, size)) { }
        public OcTree(Vector3 center, Vector3 size, int capacity) : this(new Bounds(center, size), capacity, DEFAULT_DEPTH) { }
        public OcTree(Vector3 center, Vector3 size, int capacity, int depth) : this(new Bounds(center, size), capacity, depth) { }

        public OcTree(Bounds area) : this(area, DEFAULT_CAPACITY, DEFAULT_DEPTH) { }
        public OcTree(Bounds area, int capacity) : this(area, capacity, DEFAULT_DEPTH) { }
        public OcTree(Bounds area, int capacity, int depth) {
            this.area = area;
            this.capacity = capacity;
            this.depth = depth;
            points = new List<Vector3>(capacity);
        }

        #endregion

        #region Subdivide

        private void Subdivide() {
            area.Subdivide(out Bounds nwb, out Bounds neb, out Bounds swb, out Bounds seb, out Bounds nwt, out Bounds net, out Bounds swt, out Bounds set);
            qt0 = new OcTree(nwb, capacity, depth - 1);
            qt1 = new OcTree(neb, capacity, depth - 1);
            qt2 = new OcTree(swb, capacity, depth - 1);
            qt3 = new OcTree(seb, capacity, depth - 1);
            qt4 = new OcTree(nwt, capacity, depth - 1);
            qt5 = new OcTree(net, capacity, depth - 1);
            qt6 = new OcTree(swt, capacity, depth - 1);
            qt7 = new OcTree(set, capacity, depth - 1);
            subdivided = true;
        }

        #endregion

        #region Add / Remove

        public void AddPoint(Vector3 point) {
            if(!area.Contains(point))
                return;
            if(points.Count < capacity || depth < 1) {
                points.Add(point);
                return;
            }
            else if(!subdivided)
                Subdivide();
            qt0.AddPoint(point);
            qt1.AddPoint(point);
            qt2.AddPoint(point);
            qt3.AddPoint(point);
            qt4.AddPoint(point);
            qt5.AddPoint(point);
            qt6.AddPoint(point);
            qt7.AddPoint(point);
        }

        public void RemovePoint(Vector3 point) => RemovePoint(point, DEFAULT_REMOVE_ACCURACY);
        public void RemovePoint(Vector3 point, float accuracy) {
            var area = new Bounds(point, accuracy.To_Vector3());
            var allPointsInArea = FindPoints(area);
            Vector2 closestPoint = point;
            var sqrDist = float.MaxValue;
            foreach(var p in allPointsInArea)
                if((p - point).sqrMagnitude < sqrDist)
                    closestPoint = p;
            RemovePointFast(closestPoint);
        }

        public bool RemovePointFast(Vector3 point) {
            if(!area.Contains(point))
                return false;
            var result = points.Remove(point);
            if(subdivided) {
                result |= qt0.RemovePointFast(point);
                result |= qt1.RemovePointFast(point);
                result |= qt2.RemovePointFast(point);
                result |= qt3.RemovePointFast(point);
                result |= qt4.RemovePointFast(point);
                result |= qt5.RemovePointFast(point);
                result |= qt6.RemovePointFast(point);
                result |= qt7.RemovePointFast(point);
            }
            return result;
        }

        #endregion

        #region Area Query

        public IEnumerable<Vector3> FindPoints(Bounds area) {
            if(this.area.Intersects(area)) {
                for(int i = 0, length = points.Count; i < length; i++) {
                    if(area.Contains(points[i]))
                        yield return points[i];
                }
                if(subdivided) {
                    foreach(var t in qt0.FindPoints(area))
                        yield return t;
                    foreach(var t in qt1.FindPoints(area))
                        yield return t;
                    foreach(var t in qt2.FindPoints(area))
                        yield return t;
                    foreach(var t in qt3.FindPoints(area))
                        yield return t;
                    foreach(var t in qt4.FindPoints(area))
                        yield return t;
                    foreach(var t in qt5.FindPoints(area))
                        yield return t;
                    foreach(var t in qt6.FindPoints(area))
                        yield return t;
                    foreach(var t in qt7.FindPoints(area))
                        yield return t;
                }
            }
        }

        public IEnumerable<Vector3> FindPoints(Vector3 center, float radius) {
            var sqrArea = new Bounds(center, radius.To_Vector3());
            var sqrDist = radius * radius * radius;
            foreach(var p in FindPoints(sqrArea))
                if((p - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        #endregion
    }

    public class OcTree<T>
    {
        #region Data

        public class Data
        {
            public Vector3 Point { get; private set; }
            public T Item { get; private set; }

            public Data(Vector3 point, T item) {
                this.Point = point;
                this.Item = item;
            }
        }

        #endregion

        #region Const

        public const int DEFAULT_CAPACITY = 64;
        public const int DEFAULT_DEPTH = 8;
        public const float DEFAULT_REMOVE_ACCURACY = 0.05f;

        #endregion

        #region Variables

        private Bounds area;
        private int capacity = 0;
        private int depth = 0;
        private List<Data> points;

        private bool subdivided = false;
        private OcTree<T> qt0;
        private OcTree<T> qt1;
        private OcTree<T> qt2;
        private OcTree<T> qt3;
        private OcTree<T> qt4;
        private OcTree<T> qt5;
        private OcTree<T> qt6;
        private OcTree<T> qt7;

        #endregion

        #region Properties

        public Bounds Area => area;
        public int Capacity => capacity;
        public int Length => points.Count;
        public int Depth => depth;

        public IReadOnlyList<Data> Points => points;
        public bool IsSubdivided => subdivided;
        public OcTree<T> NorthWestBot => qt0;
        public OcTree<T> NorthEastBot => qt1;
        public OcTree<T> SouthWestBot => qt2;
        public OcTree<T> SouthEastBot => qt3;
        public OcTree<T> NorthWestTop => qt4;
        public OcTree<T> NorthEastTop => qt5;
        public OcTree<T> SouthWestTop => qt6;
        public OcTree<T> SouthEastTop => qt7;

        #endregion

        #region Constructor

        public OcTree(float size) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3())) { }
        public OcTree(float size, int capacity) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3()), capacity, DEFAULT_DEPTH) { }
        public OcTree(float size, int capacity, int depth) : this(new Bounds((-size / 2f).To_Vector3(), size.To_Vector3()), capacity, depth) { }

        public OcTree(Vector3 center, float size) : this(new Bounds(center, new Vector3(size, size, size))) { }
        public OcTree(Vector3 center, float size, int capacity) : this(new Bounds(center, size.To_Vector3()), capacity, DEFAULT_DEPTH) { }
        public OcTree(Vector3 center, float size, int capacity, int depth) : this(new Bounds(center, size.To_Vector3()), capacity, depth) { }

        public OcTree(Vector3 center, Vector3 size) : this(new Bounds(center, size)) { }
        public OcTree(Vector3 center, Vector3 size, int capacity) : this(new Bounds(center, size), capacity, DEFAULT_DEPTH) { }
        public OcTree(Vector3 center, Vector3 size, int capacity, int depth) : this(new Bounds(center, size), capacity, depth) { }

        public OcTree(Bounds area) : this(area, DEFAULT_CAPACITY, DEFAULT_DEPTH) { }
        public OcTree(Bounds area, int capacity) : this(area, capacity, DEFAULT_DEPTH) { }
        public OcTree(Bounds area, int capacity, int depth) {
            this.area = area;
            this.capacity = capacity;
            this.depth = depth;
            points = new List<Data>(capacity);
        }

        #endregion

        #region Subdivide

        private void Subdivide() {
            area.Subdivide(out Bounds nwb, out Bounds neb, out Bounds swb, out Bounds seb, out Bounds nwt, out Bounds net, out Bounds swt, out Bounds set);
            qt0 = new OcTree<T>(nwb, capacity, depth - 1);
            qt1 = new OcTree<T>(neb, capacity, depth - 1);
            qt2 = new OcTree<T>(swb, capacity, depth - 1);
            qt3 = new OcTree<T>(seb, capacity, depth - 1);
            qt4 = new OcTree<T>(nwt, capacity, depth - 1);
            qt5 = new OcTree<T>(net, capacity, depth - 1);
            qt6 = new OcTree<T>(swt, capacity, depth - 1);
            qt7 = new OcTree<T>(set, capacity, depth - 1);
            subdivided = true;
        }

        #endregion

        #region Add / Remove

        public void AddPoint(Vector3 point, T item) {
            if(!area.Contains(point))
                return;
            if(points.Count < capacity || depth < 1) {
                points.Add(new Data(point, item));
                return;
            }
            else if(!subdivided)
                Subdivide();
            qt0.AddPoint(point, item);
            qt1.AddPoint(point, item);
            qt2.AddPoint(point, item);
            qt3.AddPoint(point, item);
            qt4.AddPoint(point, item);
            qt5.AddPoint(point, item);
            qt6.AddPoint(point, item);
            qt7.AddPoint(point, item);
        }

        public void RemovePoint(Vector3 point) => RemovePoint(point, DEFAULT_REMOVE_ACCURACY);
        public void RemovePoint(Vector3 point, float accuracy) {
            var area = new Bounds(point, accuracy.To_Vector3());
            var allPointsInArea = FindPoints(area);
            Vector2 closestPoint = point;
            var sqrDist = float.MaxValue;
            foreach(var p in allPointsInArea)
                if((p - point).sqrMagnitude < sqrDist)
                    closestPoint = p;
            RemovePointFast(closestPoint);
        }

        public bool RemovePointFast(Vector3 point) {
            if(!area.Contains(point))
                return false;
            var result = points.Remove(points.Find(x => x.Point.Equals(point)));
            if(subdivided) {
                result |= qt0.RemovePointFast(point);
                result |= qt1.RemovePointFast(point);
                result |= qt2.RemovePointFast(point);
                result |= qt3.RemovePointFast(point);
                result |= qt4.RemovePointFast(point);
                result |= qt5.RemovePointFast(point);
                result |= qt6.RemovePointFast(point);
                result |= qt7.RemovePointFast(point);
            }
            return result;
        }

        #endregion

        #region Area Query

        public IEnumerable<Vector3> FindPoints(Bounds area) {
            if(this.area.Intersects(area)) {
                for(int i = 0, length = points.Count; i < length; i++) {
                    if(area.Contains(points[i].Point))
                        yield return points[i].Point;
                }
                if(subdivided) {
                    foreach(var t in qt0.FindPoints(area))
                        yield return t;
                    foreach(var t in qt1.FindPoints(area))
                        yield return t;
                    foreach(var t in qt2.FindPoints(area))
                        yield return t;
                    foreach(var t in qt3.FindPoints(area))
                        yield return t;
                    foreach(var t in qt4.FindPoints(area))
                        yield return t;
                    foreach(var t in qt5.FindPoints(area))
                        yield return t;
                    foreach(var t in qt6.FindPoints(area))
                        yield return t;
                    foreach(var t in qt7.FindPoints(area))
                        yield return t;
                }
            }
        }

        public IEnumerable<Vector3> FindPoints(Vector3 center, float radius) {
            var sqrArea = new Bounds(center, radius.To_Vector3());
            var sqrDist = radius * radius * radius;
            foreach(var p in FindPoints(sqrArea))
                if((p - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        public IEnumerable<Data> FindData(Bounds area) {
            if(this.area.Intersects(area)) {
                for(int i = 0, length = points.Count; i < length; i++) {
                    if(area.Contains(points[i].Point))
                        yield return points[i];
                }
                if(subdivided) {
                    foreach(var t in qt0.FindData(area))
                        yield return t;
                    foreach(var t in qt1.FindData(area))
                        yield return t;
                    foreach(var t in qt2.FindData(area))
                        yield return t;
                    foreach(var t in qt3.FindData(area))
                        yield return t;
                    foreach(var t in qt4.FindData(area))
                        yield return t;
                    foreach(var t in qt5.FindData(area))
                        yield return t;
                    foreach(var t in qt6.FindData(area))
                        yield return t;
                    foreach(var t in qt7.FindData(area))
                        yield return t;
                }
            }
        }

        public IEnumerable<Data> FindData(Vector3 center, float radius) {
            var sqrArea = new Bounds(center, radius.To_Vector3());
            var sqrDist = radius * radius * radius;
            foreach(var p in FindData(sqrArea))
                if((p.Point - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        public IEnumerable<T> FindItem(Bounds area) {
            if(this.area.Intersects(area)) {
                for(int i = 0, length = points.Count; i < length; i++) {
                    if(area.Contains(points[i].Point))
                        yield return points[i].Item;
                }
                if(subdivided) {
                    foreach(var t in qt0.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt1.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt2.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt3.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt4.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt5.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt6.FindData(area))
                        yield return t.Item;
                    foreach(var t in qt7.FindData(area))
                        yield return t.Item;
                }
            }
        }

        public IEnumerable<T> FindItem(Vector3 center, float radius) {
            var sqrArea = new Bounds(center, radius.To_Vector3());
            var sqrDist = radius * radius * radius;
            foreach(var p in FindData(sqrArea))
                if((p.Point - center).sqrMagnitude < sqrDist)
                    yield return p.Item;
        }

        #endregion
    }
}
