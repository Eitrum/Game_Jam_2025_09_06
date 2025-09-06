using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public class QuadTree
    {
        #region Const

        public const int DEFAULT_CAPACITY = 16;
        public const int DEFAULT_DEPTH = 8;
        public const float DEFAULT_REMOVE_ACCURACY = 0.05f;

        #endregion

        #region Variables

        private Rect area;
        private int capacity = 0;
        private int depth = 0;
        private List<Vector2> points;

        private bool subdivided = false;
        private QuadTree qt0;
        private QuadTree qt1;
        private QuadTree qt2;
        private QuadTree qt3;

        #endregion

        #region Properties

        public Rect Area => area;
        public int Capacity => capacity;
        public int Length => points.Count;
        public int Depth => depth;

        public IReadOnlyList<Vector2> Points => points;
        public bool IsSubdivided => subdivided;
        public QuadTree NorthWest => qt0;
        public QuadTree NorthEast => qt1;
        public QuadTree SouthWest => qt2;
        public QuadTree SouthEast => qt3;

        #endregion

        #region Constructor

        public QuadTree(float size) : this(new Rect(-size / 2f, -size / 2f, size, size)) { }
        public QuadTree(float size, int capacity) : this(new Rect(-size / 2f, -size / 2f, size, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(float size, int capacity, int depth) : this(new Rect(-size / 2f, -size / 2f, size, size), capacity, depth) { }

        public QuadTree(Vector2 center, float size) : this(new Rect(center.x, center.y, size, size)) { }
        public QuadTree(Vector2 center, float size, int capacity) : this(new Rect(center.x, center.y, size, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(Vector2 center, float size, int capacity, int depth) : this(new Rect(center.x, center.y, size, size), capacity, depth) { }

        public QuadTree(Vector2 center, Vector2 size) : this(new Rect(center, size)) { }
        public QuadTree(Vector2 center, Vector2 size, int capacity) : this(new Rect(center, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(Vector2 center, Vector2 size, int capacity, int depth) : this(new Rect(center, size), capacity, depth) { }

        public QuadTree(Rect area) : this(area, DEFAULT_CAPACITY, DEFAULT_DEPTH) { }
        public QuadTree(Rect area, int capacity) : this(area, capacity, DEFAULT_DEPTH) { }
        public QuadTree(Rect area, int capacity, int depth) {
            this.area = area;
            this.capacity = capacity;
            this.depth = depth;
            points = new List<Vector2>(capacity);
        }

        #endregion

        #region Subdivide

        private void Subdivide() {
            area.Subdivide(out Rect nw, out Rect ne, out Rect sw, out Rect se);
            qt0 = new QuadTree(nw, capacity, depth - 1);
            qt1 = new QuadTree(ne, capacity, depth - 1);
            qt2 = new QuadTree(sw, capacity, depth - 1);
            qt3 = new QuadTree(se, capacity, depth - 1);
            subdivided = true;
        }

        #endregion

        #region Add / Remove

        public void AddPoint(Vector2 point) {
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
        }

        public void RemovePoint(Vector2 point) => RemovePoint(point, DEFAULT_REMOVE_ACCURACY);
        public void RemovePoint(Vector2 point, float accuracy) {
            var area = new Rect(point.x - accuracy / 2f, point.y - accuracy / 2f, accuracy, accuracy);
            var allPointsInArea = FindPoints(area);
            Vector2 closestPoint = point;
            var sqrDist = float.MaxValue;
            foreach(var p in allPointsInArea)
                if((p - point).sqrMagnitude < sqrDist)
                    closestPoint = p;
            RemovePointFast(closestPoint);
        }

        public bool RemovePointFast(Vector2 point) {
            if(!area.Contains(point))
                return false;
            var result = points.Remove(point);
            if(subdivided) {
                result |= qt0.RemovePointFast(point);
                result |= qt1.RemovePointFast(point);
                result |= qt2.RemovePointFast(point);
                result |= qt3.RemovePointFast(point);
            }
            return result;
        }

        #endregion

        #region Area Query

        public IEnumerable<Vector2> FindPoints(Rect area) {
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
                }
            }
        }

        public IEnumerable<Vector2> FindPoints(Vector2 center, float radius) {
            var sqrArea = new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f);
            var sqrDist = radius * radius;
            foreach(var p in FindPoints(sqrArea))
                if((p - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        #endregion
    }

    public class QuadTree<T>
    {
        #region Data

        public class Data
        {
            public Vector2 Point { get; private set; }
            public T Item { get; private set; }

            public Data(Vector2 point, T item) {
                this.Point = point;
                this.Item = item;
            }
        }

        #endregion

        #region Const

        public const int DEFAULT_CAPACITY = 16;
        public const int DEFAULT_DEPTH = 8;
        public const float DEFAULT_REMOVE_ACCURACY = 0.05f;

        #endregion

        #region Variables

        private Rect area;
        private int capacity = 0;
        private int depth = 0;
        private List<Data> points;

        private bool subdivided = false;
        private QuadTree<T> qt0;
        private QuadTree<T> qt1;
        private QuadTree<T> qt2;
        private QuadTree<T> qt3;

        #endregion

        #region Properties

        public Rect Area => area;
        public int Capacity => capacity;
        public int Length => points.Count;
        public int Depth => depth;

        public IReadOnlyList<Data> Points => points;
        public bool IsSubdivided => subdivided;
        public QuadTree<T> NorthWest => qt0;
        public QuadTree<T> NorthEast => qt1;
        public QuadTree<T> SouthWest => qt2;
        public QuadTree<T> SouthEast => qt3;

        #endregion

        #region Constructor

        public QuadTree(float size) : this(new Rect(-size / 2f, -size / 2f, size, size)) { }
        public QuadTree(float size, int capacity) : this(new Rect(-size / 2f, -size / 2f, size, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(float size, int capacity, int depth) : this(new Rect(-size / 2f, -size / 2f, size, size), capacity, depth) { }

        public QuadTree(Vector2 center, float size) : this(new Rect(center.x, center.y, size, size)) { }
        public QuadTree(Vector2 center, float size, int capacity) : this(new Rect(center.x, center.y, size, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(Vector2 center, float size, int capacity, int depth) : this(new Rect(center.x, center.y, size, size), capacity, depth) { }

        public QuadTree(Vector2 center, Vector2 size) : this(new Rect(center, size)) { }
        public QuadTree(Vector2 center, Vector2 size, int capacity) : this(new Rect(center, size), capacity, DEFAULT_DEPTH) { }
        public QuadTree(Vector2 center, Vector2 size, int capacity, int depth) : this(new Rect(center, size), capacity, depth) { }

        public QuadTree(Rect area) : this(area, DEFAULT_CAPACITY, DEFAULT_DEPTH) { }
        public QuadTree(Rect area, int capacity) : this(area, capacity, DEFAULT_DEPTH) { }
        public QuadTree(Rect area, int capacity, int depth) {
            this.area = area;
            this.capacity = capacity;
            this.depth = depth;
            points = new List<Data>(capacity);
        }

        #endregion

        #region Subdivide

        private void Subdivide() {
            area.Subdivide(out Rect nw, out Rect ne, out Rect sw, out Rect se);
            qt0 = new QuadTree<T>(nw, capacity, depth - 1);
            qt1 = new QuadTree<T>(ne, capacity, depth - 1);
            qt2 = new QuadTree<T>(sw, capacity, depth - 1);
            qt3 = new QuadTree<T>(se, capacity, depth - 1);
            subdivided = true;
        }

        #endregion

        #region Add / Remove

        public void AddPoint(Vector2 point, T item) {
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
        }

        public void RemovePoint(Vector2 point) => RemovePoint(point, DEFAULT_REMOVE_ACCURACY);
        public void RemovePoint(Vector2 point, float accuracy) {
            var area = new Rect(point.x - accuracy / 2f, point.y - accuracy / 2f, accuracy, accuracy);
            var allPointsInArea = FindPoints(area);
            Vector2 closestPoint = point;
            var sqrDist = float.MaxValue;
            foreach(var p in allPointsInArea)
                if((p - point).sqrMagnitude < sqrDist)
                    closestPoint = p;
            RemovePointFast(closestPoint);
        }

        public bool RemovePointFast(Vector2 point) {
            if(!area.Contains(point))
                return false;
            var result = points.Remove(points.Find(x => x.Point.Equals(point)));
            if(subdivided) {
                result |= qt0.RemovePointFast(point);
                result |= qt1.RemovePointFast(point);
                result |= qt2.RemovePointFast(point);
                result |= qt3.RemovePointFast(point);
            }
            return result;
        }

        #endregion

        #region Area Query

        public IEnumerable<Vector2> FindPoints(Rect area) {
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
                }
            }
        }

        public IEnumerable<Vector2> FindPoints(Vector2 center, float radius) {
            var sqrArea = new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f);
            var sqrDist = radius * radius;
            foreach(var p in FindPoints(sqrArea))
                if((p - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        public IEnumerable<Data> FindItems(Rect area) {
            if(this.area.Intersects(area)) {
                for(int i = 0, length = points.Count; i < length; i++) {
                    if(area.Contains(points[i].Point))
                        yield return points[i];
                }
                if(subdivided) {
                    foreach(var t in qt0.FindItems(area))
                        yield return t;
                    foreach(var t in qt1.FindItems(area))
                        yield return t;
                    foreach(var t in qt2.FindItems(area))
                        yield return t;
                    foreach(var t in qt3.FindItems(area))
                        yield return t;
                }
            }
        }

        public IEnumerable<Data> FindItems(Vector2 center, float radius) {
            var sqrArea = new Rect(center.x - radius, center.y - radius, radius * 2f, radius * 2f);
            var sqrDist = radius * radius;
            foreach(var p in FindItems(sqrArea))
                if((p.Point - center).sqrMagnitude < sqrDist)
                    yield return p;
        }

        #endregion
    }
}
