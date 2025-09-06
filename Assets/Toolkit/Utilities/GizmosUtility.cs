using System;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit {
    public static class GizmosUtility {
        public enum Representation {
            None,
            Cube,
            Sphere,

            WireCube,
            WireSphere,
        }

        public struct DrawData {
            public Representation Representation;
            public float Size;
            public Color Color;

            public DrawData(Representation representation, float size) {
                this.Representation = representation;
                this.Size = size;
                this.Color = Color.white;
            }

            public DrawData(Representation representation, float size, Color color) {
                this.Representation = representation;
                this.Size = size;
                this.Color = color;
            }
        }

        #region Cube

        public static void DrawCube(Vector3 point, Quaternion rotation, Vector3 size) {
            using(new MatrixScope(Gizmos.matrix * Matrix4x4.TRS(point, rotation, size)))
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }

        public static void DrawWireCube(Vector3 point, Quaternion rotation, Vector3 size) {
            size /= 2f;
            var lbc = point + rotation * new Vector3(-size.x, -size.y, -size.z);
            var rbc = point + rotation * new Vector3(size.x, -size.y, -size.z);

            var luc = point + rotation * new Vector3(-size.x, size.y, -size.z);
            var ruc = point + rotation * new Vector3(size.x, size.y, -size.z);

            var lbf = point + rotation * new Vector3(-size.x, -size.y, size.z);
            var rbf = point + rotation * new Vector3(size.x, -size.y, size.z);

            var luf = point + rotation * new Vector3(-size.x, size.y, size.z);
            var ruf = point + rotation * new Vector3(size.x, size.y, size.z);

            Gizmos.DrawLine(lbc, rbc);
            Gizmos.DrawLine(rbc, ruc);
            Gizmos.DrawLine(ruc, luc);
            Gizmos.DrawLine(luc, lbc);

            Gizmos.DrawLine(lbf, rbf);
            Gizmos.DrawLine(rbf, ruf);
            Gizmos.DrawLine(ruf, luf);
            Gizmos.DrawLine(luf, lbf);

            Gizmos.DrawLine(lbc, lbf);
            Gizmos.DrawLine(rbc, rbf);
            Gizmos.DrawLine(luc, luf);
            Gizmos.DrawLine(ruc, ruf);
        }

        #endregion

        #region Circle

        public static void DrawCircle2D(Vector2 point, float radius)
            => DrawCircle(point, Quaternion.Euler(90, 0, 0), radius, 16);

        public static void DrawCircle2D(Vector2 point, float radius, int segments)
            => DrawCircle(point, Quaternion.Euler(90, 0, 0), radius, segments);

        public static void DrawCircle(Vector3 point, float radius)
            => DrawCircle(point, Quaternion.identity, radius, 16);

        public static void DrawCircle(Vector3 point, Quaternion rotation, float radius)
            => DrawCircle(point, rotation, radius, 16);

        public static void DrawCircle(Vector3 point, Quaternion rotation, float radius, int segments) {
            var @base = new Vector3(0, 0, radius);
            var p = point + rotation * @base;
            float rotPerSegment = 360f / (float)(segments);
            segments++;
            for(int i = 0; i < segments; i++) {
                var p2 = point + (rotation * Quaternion.Euler(0, rotPerSegment * i, 0) * @base);
                Gizmos.DrawLine(p, p2);
                p = p2;
            }
        }

        #endregion

        #region Line

        public static void DrawLine(Toolkit.Mathematics.Line line) {
            Gizmos.DrawLine(line.StartPoint, line.EndPoint);
        }

        public static void DrawLine(Toolkit.Mathematics.Line line, bool infinite) {
            if(infinite)
                Gizmos.DrawLine(line.Evaluate(-100000f), line.Evaluate(100000f));
            else
                Gizmos.DrawLine(line.StartPoint, line.EndPoint);
        }

        public static void DrawGizmos(this Toolkit.Mathematics.Line line) {
            Gizmos.DrawLine(line.StartPoint, line.EndPoint);
        }

        public static void DrawGizmos(this Toolkit.Mathematics.Line line, bool infinite) {
            if(infinite)
                Gizmos.DrawLine(line.Evaluate(-100000f), line.Evaluate(100000f));
            else
                Gizmos.DrawLine(line.StartPoint, line.EndPoint);
        }

        #endregion

        #region Hexagon

        public static void DrawHexagon(HexCoordInt coord, Quaternion rotation, float outerCircle) => DrawHexagon(coord.ToVector3, rotation, outerCircle);
        public static void DrawHexagon(HexCoord coord, Quaternion rotation, float outerCircle) => DrawHexagon(coord.ToVector3, rotation, outerCircle);

        public static void DrawHexagon(Vector3 position, Quaternion rotation) {
            DrawCircle(position, rotation, 1f, 6);
        }

        public static void DrawHexagon(Vector3 position, Quaternion rotation, float outerCircle) {
            DrawCircle(position, rotation, outerCircle, 6);
        }

        #endregion

        #region Arc

        public static void DrawArc(Arc arc) {
            var t = arc.TimeAtPeakHeight();
            if(t > 0f)
                DrawArc(arc, Gizmos.color, Time.fixedDeltaTime, Mathf.CeilToInt(1f / Time.fixedDeltaTime * t * 2f));
            else
                DrawArc(arc, Gizmos.color, Time.fixedDeltaTime, Mathf.CeilToInt(1f / Time.fixedDeltaTime));
        }

        public static void DrawArc(Arc arc, Color color)
            => DrawArc(arc, color, Time.fixedDeltaTime, Mathf.CeilToInt(1f / Time.fixedDeltaTime));

        public static void DrawArc(Arc arc, float timeStep, int steps)
            => DrawArc(arc, Gizmos.color, timeStep, steps);

        public static void DrawArc(Arc arc, Color color, float timeStep, int steps) {
            var tColor = Gizmos.color;
            Gizmos.color = color;
            var pos = arc.Position;
            for(int i = 1; i < steps; i++) {
                var newPos = arc.Evaluate(i * timeStep);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.color = tColor;
        }

        #endregion

        #region Draw Bezier

        public static void DrawBezier(Bezier bezier)
            => DrawBezier(bezier, Gizmos.color, 16);

        public static void DrawBezier(Bezier bezier, Color color)
            => DrawBezier(bezier, color, 16);

        public static void DrawBezier(Bezier bezier, int steps)
            => DrawBezier(bezier, Gizmos.color, steps);

        public static void DrawBezier(Bezier bezier, Color color, int steps) {
            var tcol = Gizmos.color;
            Gizmos.color = color;
            float timeStep = 1f / (steps - 1);
            Vector3 pos = bezier.startPoint;
            for(int i = 1; i < steps; i++) {
                var newPos = bezier.Evaluate(i * timeStep);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.color = tcol;
        }

        public static void DrawBezierWithHandles(Bezier bezier)
            => DrawBezierWithHandles(bezier, Color.white, Color.grey, 16);

        public static void DrawBezierWithHandles(Bezier bezier, int steps)
           => DrawBezierWithHandles(bezier, Color.white, Color.grey, steps);

        public static void DrawBezierWithHandles(Bezier bezier, Color color)
            => DrawBezierWithHandles(bezier, color, Color.grey, 16);

        public static void DrawBezierWithHandles(Bezier bezier, Color color, int steps)
            => DrawBezierWithHandles(bezier, color, Color.grey, steps);

        public static void DrawBezierWithHandles(Bezier bezier, Color color, Color handleColor)
            => DrawBezierWithHandles(bezier, color, handleColor, 16);

        public static void DrawBezierWithHandles(Bezier bezier, Color color, Color handleColor, int steps) {
            var tcol = Gizmos.color;
            Gizmos.color = handleColor;
            Gizmos.DrawLine(bezier.startPoint, bezier.startHandle);
            Gizmos.DrawLine(bezier.endPoint, bezier.endHandle);

            Gizmos.color = color;
            float timeStep = 1f / (steps - 1);
            Vector3 pos = bezier.startPoint;
            for(int i = 1; i < steps; i++) {
                var newPos = bezier.Evaluate(i * timeStep);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.color = tcol;
        }

        #endregion

        #region Heightmap

        public static void DrawHeightmap(Toolkit.Procedural.Terrain.Data data)
            => DrawHeightmap(data.Heightmap);

        public static void DrawHeightmap(Matrix4x4 location, Toolkit.Procedural.Terrain.Data data)
            => DrawHeightmap(location, data.Heightmap);

        public static void DrawHeightmap(float[,] heightmap) {
            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            float h = 0f;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height - 1; y++) {
                    if(y == 0)
                        h = heightmap[x, y];
                    var nh = heightmap[x, y + 1];
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x, nh, y + 1));
                    h = nh;
                }
            }

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width - 1; x++) {
                    if(x == 0)
                        h = heightmap[x, y];
                    var nh = heightmap[x + 1, y];
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x + 1, nh, y));
                    h = nh;
                }
            }
        }

        public static void DrawHeightmap(float[,] heightmap, float multiplier) {
            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            float h = 0f;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height - 1; y++) {
                    if(y == 0)
                        h = heightmap[x, y] * multiplier;
                    var nh = heightmap[x, y + 1] * multiplier;
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x, nh, y + 1));
                    h = nh;
                }
            }

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width - 1; x++) {
                    if(x == 0)
                        h = heightmap[x, y] * multiplier;
                    var nh = heightmap[x + 1, y] * multiplier;
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x + 1, nh, y));
                    h = nh;
                }
            }
        }

        public static void DrawHeightmap(Matrix4x4 location, float[,] heightmap) {
            var mtx = Gizmos.matrix;
            Gizmos.matrix = location;

            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            float h = 0f;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height - 1; y++) {
                    if(y == 0)
                        h = heightmap[x, y];
                    var nh = heightmap[x, y + 1];
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x, nh, y + 1));
                    h = nh;
                }
            }

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width - 1; x++) {
                    if(x == 0)
                        h = heightmap[x, y];
                    var nh = heightmap[x + 1, y];
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x + 1, nh, y));
                    h = nh;
                }
            }

            Gizmos.matrix = mtx;
        }



        public static void DrawHeightmap(Matrix4x4 location, float[,] heightmap, float multiplier) {
            var mtx = Gizmos.matrix;
            Gizmos.matrix = location;

            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);

            float h = 0f;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height - 1; y++) {
                    if(y == 0)
                        h = heightmap[x, y] * multiplier;
                    var nh = heightmap[x, y + 1] * multiplier;
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x, nh, y + 1));
                    h = nh;
                }
            }

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width - 1; x++) {
                    if(x == 0)
                        h = heightmap[x, y] * multiplier;
                    var nh = heightmap[x + 1, y] * multiplier;
                    Gizmos.DrawLine(new Vector3(x, h, y), new Vector3(x + 1, nh, y));
                    h = nh;
                }
            }

            Gizmos.matrix = mtx;
        }

        #endregion

        #region Quad Tree

        public static void DrawQuadTree(QuadTree tree, Color line, Color point, float pointSize) {
            var area = tree.Area;
            Gizmos.color = line;
            // Draw Border
            Gizmos.DrawLine(new Vector3(area.x, area.y), new Vector3(area.x, area.y + area.height));
            Gizmos.DrawLine(new Vector3(area.x, area.y + area.height), new Vector3(area.x + area.width, area.y + area.height));
            Gizmos.DrawLine(new Vector3(area.x + area.width, area.y + area.height), new Vector3(area.x + area.width, area.y));
            Gizmos.DrawLine(new Vector3(area.x + area.width, area.y), new Vector3(area.x, area.y));
            DrawQuadTreeInside(tree, line, point, pointSize);
        }

        private static void DrawQuadTreeInside(QuadTree tree, Color line, Color point, float pointSize) {
            var area = tree.Area;
            var points = tree.Points;
            Gizmos.color = point;
            for(int i = 0, length = points.Count; i < length; i++)
                Gizmos.DrawSphere(points[i], pointSize);

            if(tree.IsSubdivided) {
                Gizmos.color = line;
                var halfx = area.width / 2f;
                var halfy = area.height / 2f;
                Gizmos.DrawLine(new Vector3(area.x + halfx, area.y), new Vector3(area.x + halfx, area.y + area.height));
                Gizmos.DrawLine(new Vector3(area.x, area.y + halfy), new Vector3(area.x + area.width, area.y + halfy));
                DrawQuadTreeInside(tree.NorthWest, line, point, pointSize);
                DrawQuadTreeInside(tree.NorthEast, line, point, pointSize);
                DrawQuadTreeInside(tree.SouthWest, line, point, pointSize);
                DrawQuadTreeInside(tree.SouthEast, line, point, pointSize);
            }
        }

        #endregion

        #region Triangle

        public static void DrawTriangle(Vector3 p0, Vector3 p1, Vector3 p2) {
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p0);
        }

        public static void DrawTriangle(Triangle triangle) {
            Gizmos.DrawLine(triangle.point0, triangle.point1);
            Gizmos.DrawLine(triangle.point1, triangle.point2);
            Gizmos.DrawLine(triangle.point2, triangle.point0);
        }

        public static void DrawTriangle(Triangle triangle, Transform offset) {
            var pos = offset.position;
            var rot = offset.rotation;
            Gizmos.DrawLine(pos + rot * triangle.point0, pos + rot * triangle.point1);
            Gizmos.DrawLine(pos + rot * triangle.point1, pos + rot * triangle.point2);
            Gizmos.DrawLine(pos + rot * triangle.point2, pos + rot * triangle.point0);
        }

        public static void DrawTriangle(Triangle triangle, Vector3 position, Quaternion rotation) {
            Gizmos.DrawLine(position + rotation * triangle.point0, position + rotation * triangle.point1);
            Gizmos.DrawLine(position + rotation * triangle.point1, position + rotation * triangle.point2);
            Gizmos.DrawLine(position + rotation * triangle.point2, position + rotation * triangle.point0);
        }

        #endregion

        #region Polygon

        public static void DrawPolygon(Polygon polygon) {
            if(polygon.Count < 2)
                return;
            var points = polygon.Points;
            for(int i = 1, length = points.Count; i < length; i++) {
                Gizmos.DrawLine(points[i - 1], points[i]);
            }
            Gizmos.DrawLine(points[points.Count - 1], points[0]);
        }

        public static void DrawPolygon(Polygon polygon, Color a, Color b) {
            if(polygon.Count < 2)
                return;
            var points = polygon.Points;
            for(int i = 1, length = points.Count; i < length; i++) {
                Gizmos.color = Color.Lerp(a, b, (i - 1f) / length);
                Gizmos.DrawLine(points[i - 1], points[i]);
            }
            Gizmos.color = b;
            Gizmos.DrawLine(points[points.Count - 1], points[0]);
        }

        public static void DrawPolygon(Polygon polygon, Gradient gradient) {
            if(polygon.Count < 2)
                return;
            var points = polygon.Points;
            for(int i = 1, length = points.Count; i < length; i++) {
                Gizmos.color = gradient.Evaluate((i - 1f) / length);
                Gizmos.DrawLine(points[i - 1], points[i]);
            }
            Gizmos.color = gradient.Evaluate(1f);
            Gizmos.DrawLine(points[points.Count - 1], points[0]);
        }

        #endregion

        #region TransformData

        public static void DrawTransformData(TransformData data) {
            using(new MatrixScope(data.Matrix)) {
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, 1));
            }
        }

        #endregion

        #region Color Scope

        public class ColorScope : IDisposable {
            #region Variables

            private Color previousColor;
            private Color scopeColor;

            #endregion

            #region Properties

            public Color PreviousColor => previousColor;
            public Color ScopeColor => scopeColor;
            public bool IsDisposed { get; private set; }

            #endregion

            #region Constructor

            public ColorScope(Color color) {
                previousColor = Gizmos.color;
                scopeColor = color;
                Gizmos.color = color;
            }

            public ColorScope(Color32 color) : this((Color)color) { }

            public ColorScope(int color32) : this(color32.ToColor()) { }

            public ColorScope(string hex) : this(hex.HexToColor()) { }

            #endregion

            #region Dispose

            public void Dispose() {
                if(!IsDisposed) {
                    Gizmos.color = previousColor;
                    IsDisposed = true;
                }
            }

            #endregion
        }

        #endregion

        #region Matrix Scope

        public class MatrixScope : IDisposable {
            #region Variables

            private Matrix4x4 previousMatrix;
            private Matrix4x4 newMatrix;

            #endregion

            #region Properties

            public Matrix4x4 PreviousMatrix => previousMatrix;
            public Matrix4x4 NewMatrix => newMatrix;

            public Matrix4x4 LastAssignedMatrix => IsDisposed ? previousMatrix : newMatrix;
            public bool IsDisposed { get; private set; }

            #endregion

            #region Constructor

            public MatrixScope(Matrix4x4 matrix) {
                previousMatrix = Gizmos.matrix;
                newMatrix = matrix;
                Gizmos.matrix = matrix;
            }

            public MatrixScope(Transform transform) : this(transform.localToWorldMatrix) { }

            public MatrixScope(Vector3 position, Quaternion rotation, Vector3 scale, Space space) {
                previousMatrix = Gizmos.matrix;
                if(space == Space.World)
                    newMatrix = Matrix4x4.TRS(position, rotation, scale);
                else
                    newMatrix = previousMatrix * Matrix4x4.TRS(position, rotation, scale);

                Gizmos.matrix = newMatrix;
            }

            #endregion

            #region Dispose

            public void Dispose() {
                if(!IsDisposed) {
                    Gizmos.matrix = previousMatrix;
                    IsDisposed = true;
                }
            }

            #endregion
        }

        #endregion

        #region Camera

        public static void DrawCamera(Camera c) {
            using(new GizmosUtility.MatrixScope(c.transform))
                Gizmos.DrawFrustum(Vector3.zero, c.fieldOfView, c.farClipPlane, c.nearClipPlane, c.aspect);
        }

        #endregion
    }
}
