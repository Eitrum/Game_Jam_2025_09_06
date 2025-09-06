using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Mathematics {
    [System.Serializable]
    public struct Triangle2D : IReadOnlyList<Vector2> {
        #region Variables

        public Vector2 point0;
        public Vector2 point1;
        public Vector2 point2;

        #endregion

        #region Properties

        public unsafe Vector2 this[int index] {
            get {
#if UNITY_EDITOR
                if(index < 0 || index > 2) // Only check in editor
                    throw new IndexOutOfRangeException();
#endif
                fixed(Vector2* p = (&point0)) {
                    return *(p + index);
                }
            }
            set {
#if UNITY_EDITOR
                if(index < 0 || index > 2) // Only check in editor
                    throw new IndexOutOfRangeException();
#endif
                fixed(Vector2* p = (&point0)) {
                    (*(p + index)) = value;
                }
            }
        }

        public Vector2 Center => (point0 + point1 + point2) / 3f;

        public static Triangle2D Equilateral => new Triangle2D(1f);
        public static Triangle2D Isosceles => new Triangle2D(new Vector2(0, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));
        public static Triangle2D LeftAngled => new Triangle2D(new Vector2(-1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));
        public static Triangle2D RightAngled => new Triangle2D(new Vector2(1f, 1f), new Vector2(1f, -1f), new Vector2(-1f, -1f));

        public bool IsClockwise {
            get {
                var res = 0f;
                res += Vector2.SignedAngle(Vector2.zero, point1 - point0);
                res += Vector2.SignedAngle(point1 - point0, point2 - point0);
                res += Vector2.SignedAngle(point2 - point0, Vector2.zero);
                return res <= 0f;
            }
        }

        public float Base => Vector2.Distance(point1, point2);

        public float Height {
            get {
                return (Area * 2f) / Base;
            }
        }

        public float Area {
            get {
                var a = Vector2.Distance(point0, point1);
                var b = Vector2.Distance(point1, point2);
                var c = Vector2.Distance(point2, point0);
                var s = (a + b + c) / 2f;

                return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
            }
        }

        public float Perimeter =>
            Vector2.Distance(point0, point1) +
            Vector2.Distance(point1, point2) +
            Vector2.Distance(point2, point0);

        public bool IsEmpty =>
            point0.Equals(new Vector2(0, 0), Mathf.Epsilon) &&
            point1.Equals(new Vector2(0, 0), Mathf.Epsilon) &&
            point2.Equals(new Vector2(0, 0), Mathf.Epsilon);

        public Rect BoundingBox {
            get {
                var minX = MathUtility.Min(point0.x, point1.x, point2.x);
                var minY = MathUtility.Min(point0.y, point1.y, point2.y);
                var maxX = MathUtility.Max(point0.x, point1.x, point2.x);
                var maxY = MathUtility.Max(point0.y, point1.y, point2.y);
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        int IReadOnlyCollection<Vector2>.Count => throw new NotImplementedException();

        Vector2 IReadOnlyList<Vector2>.this[int index] => throw new NotImplementedException();

        #endregion

        #region Constructor

        public Triangle2D(Vector2 p0, Vector2 p1, Vector2 p2) {
            this.point0 = p0;
            this.point1 = p1;
            this.point2 = p2;
        }

        /// <summary>
        /// Creates a equilateral triangle
        /// </summary>
        /// <param name="scale"></param>
        public Triangle2D(float scale) {
            this.point0 = new Vector2(0, scale);
            this.point1 = new Vector2(Mathf.Sin(Mathf.Deg2Rad * 120f) * scale, Mathf.Cos(Mathf.Deg2Rad * 120f) * scale);
            this.point2 = new Vector2(Mathf.Sin(Mathf.Deg2Rad * -120f) * scale, Mathf.Cos(Mathf.Deg2Rad * -120f) * scale);
        }

        #endregion

        #region Calculations

        public bool CalculateCircumcenterAndRadiusSquared(out Vector2 center, out float radiusSquared) {
            var dA = point0.x * point0.x + point0.y * point0.y;
            var dB = point1.x * point1.x + point1.y * point1.y;
            var dC = point2.x * point2.x + point2.y * point2.y;

            var aux1 = (dA * (point2.y - point1.y) + dB * (point0.y - point2.y) + dC * (point1.y - point0.y));
            var aux2 = -(dA * (point2.x - point1.x) + dB * (point0.x - point2.x) + dC * (point1.x - point0.x));
            var div = (2 * (point0.x * (point2.y - point1.y) + point1.x * (point0.y - point2.y) + point2.x * (point1.y - point0.y)));

            if(div == 0) {
                center = Vector2.zero;
                radiusSquared = 0f;
                return false;
            }

            center = new Vector2(aux1 / div, aux2 / div);
            radiusSquared = (center.x - point0.x) * (center.x - point0.x) + (center.y - point0.y) * (center.y - point0.y);
            return true;
        }

        public Vector2 CalculateCircumcenter() {
            var dA = point0.x * point0.x + point0.y * point0.y;
            var dB = point1.x * point1.x + point1.y * point1.y;
            var dC = point2.x * point2.x + point2.y * point2.y;

            var aux1 = (dA * (point2.y - point1.y) + dB * (point0.y - point2.y) + dC * (point1.y - point0.y));
            var aux2 = -(dA * (point2.x - point1.x) + dB * (point0.x - point2.x) + dC * (point1.x - point0.x));
            var div = (2 * (point0.x * (point2.y - point1.y) + point1.x * (point0.y - point2.y) + point2.x * (point1.y - point0.y)));

            if(div == 0) {
                throw new DivideByZeroException();
            }

            return new Vector2(aux1 / div, aux2 / div);
        }

        public float CalculateRadiusSquared() {
            var center = CalculateCircumcenter();
            return (center.x - point0.x) * (center.x - point0.x) + (center.y - point0.y) * (center.y - point0.y);
        }

        #endregion

        #region Intersection

        public bool IsPointInsideCircumcircle(Vector2 point) {
            CalculateCircumcenterAndRadiusSquared(out Vector2 center, out float radiusSquared);
            var d_squared = (point.x - center.x) * (point.x - center.x) +
                (point.y - center.y) * (point.y - center.y);
            return d_squared < radiusSquared;
        }

        public bool Contains(Vector2 point) {
            var s = (point0.x - point2.x) * (point.y - point2.y) - (point0.y - point2.y) * (point.x - point2.x);
            var t = (point1.x - point0.x) * (point.y - point0.y) - (point1.y - point0.y) * (point.x - point0.x);

            if((s < 0) != (t < 0) && s != 0 && t != 0)
                return false;

            var d = (point2.x - point1.x) * (point.y - point1.y) - (point2.y - point1.y) * (point.x - point1.x);
            return d == 0 || (d < 0) == (s + t <= 0);
        }

        public bool Insersects(Triangle2D other) {
            // AABB check
            var r0 = BoundingBox;
            var r1 = other.BoundingBox;
            if(!r0.Intersects(r1))
                return false;

            // Check if any point of either triangle is inside the other triangle
            if(Contains(other.point0) || Contains(other.point1) || Contains(other.point2) || other.Contains(point0) || other.Contains(point1) || other.Contains(point2))
                return true;

            // Check if any line overlaps
            if(Line.GetPointAtIntersectionXYClamped(point0, point1, other.point0, other.point1))
                return true;
            if(Line.GetPointAtIntersectionXYClamped(point1, point2, other.point1, other.point2))
                return true;
            if(Line.GetPointAtIntersectionXYClamped(point2, point0, other.point2, other.point0))
                return true;

            return false;
        }

        #endregion

        #region Move

        public void Move(Vector2 direction) {
            point0 += direction;
            point1 += direction;
            point2 += direction;
        }

        public void MoveX(float amount) {
            point0.x += amount;
            point1.x += amount;
            point2.x += amount;
        }

        public void MoveY(float amount) {
            point0.y += amount;
            point1.y += amount;
            point2.y += amount;
        }

        #endregion

        #region Rotate

        public void Rotate(float angle) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);

            point0 = new Vector2(c * point0.x - s * point0.y, s * point0.x + c * point0.y);
            point1 = new Vector2(c * point1.x - s * point1.y, s * point1.x + c * point1.y);
            point2 = new Vector2(c * point2.x - s * point2.y, s * point2.x + c * point2.y);
        }

        public void RotateAroundCenter(float angle) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);
            var center = Center;

            point0 = new Vector2(c * (point0.x - center.x) - s * (point0.y - center.y), s * (point0.x - center.x) + c * (point0.y - center.y)) + center;
            point1 = new Vector2(c * (point1.x - center.x) - s * (point1.y - center.y), s * (point1.x - center.x) + c * (point1.y - center.y)) + center;
            point2 = new Vector2(c * (point2.x - center.x) - s * (point2.y - center.y), s * (point2.x - center.x) + c * (point2.y - center.y)) + center;
        }

        public void RotateAroundPoint(float angle, Vector2 point) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);

            point0 = new Vector2(c * (point0.x - point.x) - s * (point0.y - point.y), s * (point0.x - point.x) + c * (point0.y - point.y)) + point;
            point1 = new Vector2(c * (point1.x - point.x) - s * (point1.y - point.y), s * (point1.x - point.x) + c * (point1.y - point.y)) + point;
            point2 = new Vector2(c * (point2.x - point.x) - s * (point2.y - point.y), s * (point2.x - point.x) + c * (point2.y - point.y)) + point;
        }

        #endregion

        #region Subdivide

        public List<Triangle2D> Subdivide() {
            var p = new Polygon(this);
            p.Subdivide();
            return p.GetTriangles();
        }

        #endregion

        #region Enumerable Impl

        IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator() {
            for(int i = 0; i < 3; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            for(int i = 0; i < 3; i++) {
                yield return this[i];
            }
        }

        #endregion

        #region Static Operators

        public static implicit operator Triangle(Triangle2D t2d) => new Triangle(t2d.point0, t2d.point1, t2d.point2);
        public static implicit operator Polygon(Triangle2D t2d) => new Polygon(t2d);

        public static Triangle2D operator *(Triangle2D lhs, float rhs) => new Triangle2D(lhs.point0 * rhs, lhs.point1 * rhs, lhs.point2 * rhs);
        public static Triangle2D operator /(Triangle2D lhs, float rhs) => new Triangle2D(lhs.point0 / rhs, lhs.point1 / rhs, lhs.point2 / rhs);
        public static Triangle2D operator *(float lhs, Triangle2D rhs) => new Triangle2D(rhs.point0 * lhs, rhs.point1 * lhs, rhs.point2 * lhs);
        public static Triangle2D operator /(float lhs, Triangle2D rhs) => new Triangle2D(rhs.point0 / lhs, rhs.point1 / lhs, rhs.point2 / lhs);

        public static Triangle2D operator +(Triangle2D lhs, Vector2 rhs) => new Triangle2D(lhs.point0 + rhs, lhs.point1 + rhs, lhs.point2 + rhs);
        public static Triangle2D operator -(Triangle2D lhs, Vector2 rhs) => new Triangle2D(lhs.point0 - rhs, lhs.point1 - rhs, lhs.point2 - rhs);
        public static Triangle2D operator +(Vector2 lhs, Triangle2D rhs) => new Triangle2D(rhs.point0 + lhs, rhs.point1 + lhs, rhs.point2 + lhs);
        public static Triangle2D operator -(Vector2 lhs, Triangle2D rhs) => new Triangle2D(rhs.point0 - lhs, rhs.point1 - lhs, rhs.point2 - lhs);

        #endregion
    }
}
