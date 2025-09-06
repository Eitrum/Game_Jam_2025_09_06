/* MIT License

Copyright (c) 2018 Rafael K�bler da Silva

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Mathematics
{
    public static class DelaunayTriangulation
    {
        #region Voronoi

        public static List<Line> GenerateVoronoi(int amount, float width, float height) {
            var tris = GenerateTriangles(amount, width, height);

            var voronoiEdges = new List<Line>();
            foreach(var triangle in tris) {
                foreach(var neighbor in triangle.TrianglesWithSharedEdge) {
                    var edge = new Edge(triangle.Circumcenter, neighbor.Circumcenter);
                    voronoiEdges.Add(new Line(edge.Point1, edge.Point2));
                }
            }

            RepairLines(voronoiEdges, width, height);

            return voronoiEdges;
        }

        public static List<Line> GenerateVoronoi(int amount, float width, float height, System.Random random) {
            var tris = GenerateTriangles(amount, width, height, random);

            var voronoiEdges = new List<Line>();
            foreach(var triangle in tris) {
                foreach(var neighbor in triangle.TrianglesWithSharedEdge) {
                    var edge = new Edge(triangle.Circumcenter, neighbor.Circumcenter);
                    voronoiEdges.Add(new Line(edge.Point1, edge.Point2));
                }
            }

            RepairLines(voronoiEdges, width, height);

            return voronoiEdges;
        }

        #endregion

        #region Internal Generation

        private static void RepairLines(List<Line> lines, float width, float height) {
            Rect area = new Rect(0, 0, width, height);
            for(int i = lines.Count - 1; i >= 0; i--) {
                var spOutside = !area.Contains(lines[i].StartPoint);
                var epOutside = !area.Contains(lines[i].EndPoint);
                if(spOutside && epOutside) {
                    lines.RemoveAt(i);
                    continue;
                }

                if(spOutside) {
                    var sp = lines[i].StartPoint;
                    if(sp.x < 0f)
                        sp = lines[i].GetPointAtIntersectionX(0) ?? sp;
                    if(sp.x > width)
                        sp = lines[i].GetPointAtIntersectionX(width) ?? sp;
                    if(sp.y < 0f)
                        sp = lines[i].GetPointAtIntersectionY(0) ?? sp;
                    if(sp.y > height)
                        sp = lines[i].GetPointAtIntersectionY(height) ?? sp;

                    lines[i] = new Line(sp, lines[i].EndPoint);
                }
                else if(epOutside) {
                    var ep = lines[i].EndPoint;
                    if(ep.x < 0f)
                        ep = lines[i].GetPointAtIntersectionX(0) ?? ep;
                    if(ep.x > width)
                        ep = lines[i].GetPointAtIntersectionX(width) ?? ep;
                    if(ep.y < 0f)
                        ep = lines[i].GetPointAtIntersectionY(0) ?? ep;
                    if(ep.y > height)
                        ep = lines[i].GetPointAtIntersectionY(height) ?? ep;

                    lines[i] = new Line(lines[i].StartPoint, ep);
                }
            }
        }

        private static List<Triangle> GenerateTriangles(int amount, float width, float height) {
            GeneratePoints(amount, width, height, out List<Point> points, out List<Triangle> border);
            return BowyerWatson(points, border);
        }

        private static List<Triangle> GenerateTriangles(int amount, float width, float height, System.Random random) {
            GeneratePoints(amount, width, height, random, out List<Point> points, out List<Triangle> border);
            return BowyerWatson(points, border);
        }

        private static void GeneratePoints(int amount, float width, float height, out List<Point> points, out List<Triangle> border) {
            var p0 = new Point(0, 0);
            var p1 = new Point(0, height);
            var p2 = new Point(width, height);
            var p3 = new Point(width, 0);

            points = new List<Point>() { p0, p1, p2, p3 };
            border = new List<Triangle>() { new Triangle(p0, p1, p2), new Triangle(p0, p2, p3) };

            for(int i = 0; i < amount; i++) {
                points.Add(new Point(Random.Float * width, Random.Float * height));
            }
        }

        private static void GeneratePoints(int amount, float width, float height, System.Random random, out List<Point> points, out List<Triangle> border) {
            var p0 = new Point(0, 0);
            var p1 = new Point(0, height);
            var p2 = new Point(width, height);
            var p3 = new Point(width, 0);

            points = new List<Point>() { p0, p1, p2, p3 };
            border = new List<Triangle>() { new Triangle(p0, p1, p2), new Triangle(p0, p2, p3) };

            for(int i = 0; i < amount; i++) {
                points.Add(new Point(random.NextFloat() * width, random.NextFloat() * height));
            }
        }

        private static List<Triangle> BowyerWatson(List<Point> points, List<Triangle> triangulation) {
            foreach(var point in points) {
                var badTriangles = FindBadTriangles(point, triangulation);
                var polygon = FindHoleBoundaries(badTriangles);

                foreach(var triangle in badTriangles) {
                    foreach(var vertex in triangle.Vertices) {
                        vertex.AdjacentTriangles.Remove(triangle);
                    }
                }
                triangulation.RemoveWhere(o => badTriangles.Contains(o));

                foreach(var edge in polygon.Where(possibleEdge => possibleEdge.Point1 != point && possibleEdge.Point2 != point)) {
                    var triangle = new Triangle(point, edge.Point1, edge.Point2);
                    triangulation.Add(triangle);
                }
            }

            return triangulation;
        }

        private static List<Edge> FindHoleBoundaries(List<Triangle> badTriangles) {
            var edges = new List<Edge>();
            foreach(var triangle in badTriangles) {
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
            }
            var grouped = edges.GroupBy(o => o);
            var boundaryEdges = edges.GroupBy(o => o).Where(o => o.Count() == 1).Select(o => o.First());
            return boundaryEdges.ToList();
        }

        private static Triangle GenerateSupraTriangle(float width, float height) {
            var margin = 500;
            var point1 = new Point(0.5f * width, -2 * width - margin);
            var point2 = new Point(-2 * height - margin, 2 * height + margin);
            var point3 = new Point(2 * width + height + margin, 2 * height + margin);
            return new Triangle(point1, point2, point3);
        }

        private static List<Triangle> FindBadTriangles(Point point, List<Triangle> triangles) {
            var badTriangles = triangles.Where(o => o.IsPointInsideCircumcircle(point));
            return new List<Triangle>(badTriangles);
        }

        #endregion

        #region Classes

        private class Edge
        {
            public Point Point1 { get; }
            public Point Point2 { get; }

            public Edge(Point point1, Point point2) {
                Point1 = point1;
                Point2 = point2;
            }

            public override bool Equals(object obj) {
                if(obj == null) return false;
                if(obj.GetType() != GetType()) return false;
                var edge = obj as Edge;

                var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
                var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
                return samePoints || samePointsReversed;
            }

            public override int GetHashCode() {
                int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
                return hCode.GetHashCode();
            }
        }

        private class Point
        {
            private static int _counter;
            private readonly int _instanceId = _counter++;

            public float X { get; }
            public float Y { get; }
            public HashSet<Triangle> AdjacentTriangles { get; } = new HashSet<Triangle>();

            public Point(float x, float y) {
                X = x;
                Y = y;
            }

            public override string ToString() {
                return $"{nameof(Point)} {_instanceId} {X:0.##}@{Y:0.##}";
            }


            public static implicit operator Vector2(Point p) => new Vector2(p.X, p.Y);
            public static implicit operator Vector3(Point p) => new Vector3(p.X, p.Y);
        }

        private class Triangle
        {
            public Point[] Vertices { get; } = new Point[3];
            public Point Circumcenter { get; private set; }
            public double RadiusSquared;

            public IEnumerable<Triangle> TrianglesWithSharedEdge {
                get {
                    var neighbors = new HashSet<Triangle>();
                    foreach(var vertex in Vertices) {
                        var trianglesWithSharedEdge = vertex.AdjacentTriangles.Where(o => {
                            return o != this && SharesEdgeWith(o);
                        });
                        neighbors.UnionWith(trianglesWithSharedEdge);
                    }

                    return neighbors;
                }
            }

            public Triangle(Point point1, Point point2, Point point3) {
                // In theory this shouldn't happen, but it was at one point so this at least makes sure we're getting a
                // relatively easily-recognised error message, and provides a handy breakpoint for debugging.
                if(point1 == point2 || point1 == point3 || point2 == point3) {
                    throw new ArgumentException("Must be 3 distinct points");
                }

                if(!IsCounterClockwise(point1, point2, point3)) {
                    Vertices[0] = point1;
                    Vertices[1] = point3;
                    Vertices[2] = point2;
                }
                else {
                    Vertices[0] = point1;
                    Vertices[1] = point2;
                    Vertices[2] = point3;
                }

                Vertices[0].AdjacentTriangles.Add(this);
                Vertices[1].AdjacentTriangles.Add(this);
                Vertices[2].AdjacentTriangles.Add(this);
                UpdateCircumcircle();
            }

            private void UpdateCircumcircle() {
                // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
                // https://en.wikipedia.org/wiki/Circumscribed_circle
                var p0 = Vertices[0];
                var p1 = Vertices[1];
                var p2 = Vertices[2];
                var dA = p0.X * p0.X + p0.Y * p0.Y;
                var dB = p1.X * p1.X + p1.Y * p1.Y;
                var dC = p2.X * p2.X + p2.Y * p2.Y;

                var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
                var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
                var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

                if(div == 0) {
                    throw new DivideByZeroException();
                }

                var center = new Point(aux1 / div, aux2 / div);
                Circumcenter = center;
                RadiusSquared = (center.X - p0.X) * (center.X - p0.X) + (center.Y - p0.Y) * (center.Y - p0.Y);
            }

            private bool IsCounterClockwise(Point point1, Point point2, Point point3) {
                var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
                    (point3.X - point1.X) * (point2.Y - point1.Y);
                return result > 0;
            }

            public bool SharesEdgeWith(Triangle triangle) {
                var sharedVertices = Vertices.Where(o => triangle.Vertices.Contains(o)).Count();
                return sharedVertices == 2;
            }

            public bool IsPointInsideCircumcircle(Point point) {
                var d_squared = (point.X - Circumcenter.X) * (point.X - Circumcenter.X) +
                    (point.Y - Circumcenter.Y) * (point.Y - Circumcenter.Y);
                return d_squared < RadiusSquared;
            }
        }

        #endregion
    }
}
