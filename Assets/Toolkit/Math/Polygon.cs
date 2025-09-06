using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public class Polygon
    {
        #region Variables

        private const string TAG = "[Polygon] - ";
        [SerializeField] private List<Vector2> points = new List<Vector2>();

        #endregion

        #region Properties

        public bool IsValid => points.Count > 1;

        public Vector2 this[int index] {
            get => points[index];
            set => points[index] = value;
        }

        public int Count => points.Count;
        public IReadOnlyList<Vector2> Points => points;

        public bool IsClockwise {
            get {
                if(!IsValid)
                    return false;
                var res = 0f;
                for(int i = 1, length = points.Count; i < length; i++) {
                    res += Vector2.SignedAngle(points[i - 1], points[i]);
                }
                res += Vector2.SignedAngle(points[points.Count - 1], points[0]);
                return res <= 0f;
            }
        }

        public Vector2 Center => points.Average();
        public static Polygon Empty => new Polygon();
        public static Polygon Triangle => new Polygon(Triangle2D.Equilateral);
        public static Polygon Square => new Polygon(4);
        public static Polygon Diamond => new Polygon(4, false);
        public static Polygon Hexagon => new Polygon(6);

        public float Area {
            get {
                float resX = 0f;
                float resY = 0f;
                for(int i = 1, length = points.Count; i <= length; i++) {
                    var p0 = points[i - 1];
                    var p1 = points[i % length];
                    resX += p0.x * p1.y;
                    resY += p0.y * p1.x;
                }
                return (resX - resY) / (IsClockwise ? -2f : 2f);
            }
        }

        public float Perimeter {
            get {
                float res = 0f;
                for(int i = 1, length = points.Count; i <= length; i++) {
                    res += Vector2.Distance(points[i - 1], points[i % length]);
                }

                return res;
            }
        }

        #endregion

        #region Constructor

        public Polygon() { }

        public Polygon(IReadOnlyList<Vector2> points) {
            this.points.AddRange(points);
        }

        /// <summary>
        /// Generates a polygon with minimum 3 sides.
        /// </summary>
        /// <param name="sides"></param>
        public Polygon(int sides) : this(sides, true) { }

        public Polygon(int sides, bool sideUp) {
            sides = Mathf.Max(sides, 3);

            var rotPerSide = (-360f / sides) * Mathf.Deg2Rad;
            var offset = (sideUp ? -rotPerSide / 2f : 0f) + Mathf.Deg2Rad * 90f;
            for(int i = 0; i < sides; i++)
                points.Add(new Vector2(Mathf.Cos(offset + rotPerSide * i), Mathf.Sin(offset + rotPerSide * i)));
        }

        private Polygon(Triangle2D triangle) => AddTriangle(triangle);

        #endregion

        #region Add

        public void AddAfter(int index) {
            if(points.Count > 2) {
                if(index < points.Count - 1) {
                    points.Add((points[0] + points[points.Count - 1]) / 2f);
                }
                else {
                    points.Insert(index + 1, (points[index] + points[index + 1]) / 2f);
                }
            }
            else
                points.Add(new Vector2());
        }

        public void AddBefore(int index) {
            if(points.Count > 2) {
                if(index > 0) {
                    points.Insert(index, (points[index] + points[index - 1]) / 2f);
                }
                else {
                    points.Insert(0, (points[0] + points[points.Count - 1]) / 2f);
                }
            }
            else
                points.Add(new Vector2());
        }

        public void AddAfter(int index, Vector2 point) {
            points.Insert(index + 1, point);
        }

        public void AddBefore(int index, Vector2 point) {
            points.Insert(index, point);
        }

        public void Add(Vector2 point) {
            points.Add(point);
        }

        public void AddTriangle(Triangle2D triangle) {
            points.Add(triangle.point0);
            points.Add(triangle.point1);
            points.Add(triangle.point2);
        }

        public void AddLine(Line line) {
            points.Add(line.StartPoint);
            points.Add(line.EndPoint);
        }

        public void AddLineXZ(Line line) {
            points.Add(line.StartPoint.To_Vector2_XZ());
            points.Add(line.EndPoint.To_Vector2_XZ());
        }

        #endregion

        #region Remove

        public void Remove(Vector2 point)
            => Remove(point, 0.01f);

        public void Remove(Vector2 point, float errorDistance) {
            for(int i = points.Count - 1; i >= 0; i--) {
                if(points[i].Equals(point, errorDistance))
                    points.RemoveAt(i);
            }
        }

        public void Remove(int index) {
            points.RemoveAt(index);
        }

        #endregion

        #region Move

        public void Move(Vector2 dir) {
            for(int i = points.Count - 1; i >= 0; i--) {
                points[i] = points[i] + dir;
            }
        }

        public void MoveX(float amount) {
            for(int i = points.Count - 1; i >= 0; i--) {
                points[i] = new Vector2(points[i].x + amount, points[i].y);
            }
        }

        public void MoveY(float amount) {
            for(int i = points.Count - 1; i >= 0; i--) {
                points[i] = new Vector2(points[i].x, points[i].y + amount);
            }
        }

        #endregion

        #region Rotate

        public void Rotate(float angle) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);

            for(int i = points.Count - 1; i >= 0; i--) {
                var p = points[i];
                points[i] = new Vector2(c * p.x - s * p.y, s * p.x + c * p.y);
            }
        }

        public void RotateAroundCenter(float angle) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);
            var center = Center;
            for(int i = points.Count - 1; i >= 0; i--) {
                var p = points[i];
                points[i] = new Vector2(c * (p.x - center.x) - s * (p.y - center.y), s * (p.x - center.x) + c * (p.y - center.y)) + center;
            }
        }

        public void RotateAroundPoint(float angle, Vector2 point) {
            var rad = Mathf.Deg2Rad * -angle;
            var c = Mathf.Cos(rad);
            var s = Mathf.Sin(rad);

            for(int i = points.Count - 1; i >= 0; i--) {
                var p = points[i];
                points[i] = new Vector2(c * (p.x - point.x) - s * (p.y - point.y), s * (p.x - point.x) + c * (p.y - point.y)) + point;
            }
        }

        #endregion

        #region Triangulation

        /// <summary>
        /// Uses ear cutting attempting to find best triangle possible, at O(N^2)
        /// </summary>
        public List<Triangle2D> GetTriangles() {
            if(points.Count < 3) // Unable to create a triangle
                return new List<Triangle2D>();

            bool isClockwise = IsClockwise;
            List<Triangle2D> triangles = new List<Triangle2D>();
            List<Vector2> temporary = new List<Vector2>(points);

            while(temporary.Count > 3) {
                if(FindEar(temporary, isClockwise, out int index, out Triangle2D triangle)) {
                    temporary.RemoveAt(index);
                    triangles.Add(triangle);
                }
                else {
                    Debug.LogError(TAG + "COULD NOT FIND EAR!");
                    break;
                }
            }
            if(temporary.Count == 3)
                triangles.Add(new Triangle2D(temporary[0], temporary[1], temporary[2]));
            return triangles;
        }

        private static bool FindEar(List<Vector2> points, bool isClockwise, out int index, out Triangle2D triangle) {
            if(points.Count < 3) {
                index = 0;
                triangle = new Triangle2D();
                return false;
            }
            index = -1;
            triangle = new Triangle2D();
            float dist = float.MaxValue;

            for(int i = 0, length = points.Count; i < length; i++) {
                var p0 = points[i + 0];
                var p1 = points[(i + 1) % length];
                var p2 = points[(i + 2) % length];
                var tris = new Triangle2D(p0, p1, p2);
                if(isClockwise == tris.IsClockwise) {
                    var d = tris.Perimeter;
                    if(d < dist && !points.Any(x => !(x == p0 || x == p1 || x == p2) && tris.Contains(x))) {
                        triangle = tris;
                        index = (i + 1) % length;
                        dist = d - Mathf.Epsilon;
                    }
                }
            }
            return index > -1;
        }

        #endregion

        #region Scale

        public Line[] Shrink(float value) {
            bool isClockwise = IsClockwise;
            int length = points.Count;
            Line[] lines = new Line[length];
            for(int i = 0; i < length - 1; i++) {
                lines[i] = new Line(points[i], points[i + 1]);
            }
            lines[length - 1] = new Line(points[length - 1], points[0]);

            if(!isClockwise)
                value = -value;


            for(int i = 0; i < length; i++)
                lines[i].MoveRelative(value, 0);

            for(int i = 1; i < length; i++) {
                if(Line.GetPointAtIntersectionXY(lines[i - 1], lines[i], out Vector2 pt))
                    points[i] = pt;
            }

            if(Line.GetPointAtIntersectionXY(lines[0], lines[length - 1], out Vector2 p))
                points[0] = p;

            return lines;
        }

        #endregion

        #region Subdivide

        public void Subdivide() {
            var count = points.Count;
            if(count * 2 > ushort.MaxValue) {
                Debug.LogError(TAG + "Subdivide can't create more than 65535 points!");
                return;
            }
            for(int i = 0; i < count - 1; i++) {
                var index = i * 2;
                points.Insert(index + 1, (points[index] + points[index + 1]) / 2f);
            }
            points.Add((points[0] + points[points.Count - 1]) / 2f);
        }

        public void Subdivide(int iterations) {
            if(iterations > 12) {
                Debug.LogError(TAG + "Subdivide unable to iterate more than 12 times due to performance issues!");
                iterations = Mathf.Min(12, iterations);
            }
            for(int i = 0; i < iterations; i++)
                Subdivide();
        }

        #endregion
    }
}
