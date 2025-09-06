using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public static class PoissonDiscSampling
    {
        #region Variables

        private const int REJECTION = 20;
        private const int MAXIMUM_REFERENCE_POINTS = 2000;
        private const float MINIMUM_RADIUS = 0.001f;
        private const float SQR_2 = Mathematics.MathUtility.SQR_2;

        #endregion

        #region Simple

        public static List<Vector2> Simple(float radius, float width, float height, System.Random random) {
            radius = Mathf.Max(MINIMUM_RADIUS, radius);
            var gridSize = radius / SQR_2;
            var gridWidth = width / gridSize;
            var gridHeight = height / gridSize;

            int[,] grid = new int[Mathf.CeilToInt(gridWidth), Mathf.CeilToInt(gridHeight)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> referencePoints = new List<Vector2>();

            var fp = new Vector2(width / 2f, height / 2f);
            points.Add(fp);
            referencePoints.Add(fp);
            grid[(int)(fp.x / gridSize), (int)(fp.y / gridSize)] = points.Count;

            while(referencePoints.Count > 0) {
                var index = 0;// referencePoints.RandomIndex();
                var p = referencePoints[index];
                int attempts = 0;
                while(true) {
                    var rot = random.NextFloat() * Mathf.PI * 2f;
                    var dir = new Vector2(Mathf.Sin(rot), Mathf.Cos(rot));
                    var dist = radius + radius * random.NextFloat();
                    var np = p + dir * dist;

                    if(IsValid(np, radius, gridSize, grid, points)) {
                        points.Add(np);
                        referencePoints.Add(np);
                        grid[(int)(np.x / gridSize), (int)(np.y / gridSize)] = points.Count;
                        break;
                    }

                    attempts++;
                    if(attempts > REJECTION) {
                        referencePoints.RemoveAt(index);
                        break;
                    }
                }
                if(referencePoints.Count > MAXIMUM_REFERENCE_POINTS) {
                    Debug.LogError("No end with: " + points.Count + " points added");
                    break;
                }
            }
            return points;
        }

        public static List<Vector2> Simple(float radius, Rect area, System.Random random)
            => Simple(radius, area.x, area.y, area.width, area.height, random);

        public static List<Vector2> Simple(float radius, float x, float y, float width, float height, System.Random random) {
            var t = Simple(radius, width, height, random);
            for(int i = t.Count - 1; i >= 0; i--) {
                t[i] = new Vector2(x + t[i].x, y + t[i].y);
            }
            return t;
        }

        public static List<Vector2> Simple(float radius, Rect area)
            => Simple(radius, area.x, area.y, area.width, area.height);

        public static List<Vector2> Simple(float radius, float x, float y, float width, float height) {
            var t = Simple(radius, width, height);
            for(int i = t.Count - 1; i >= 0; i--) {
                t[i] = new Vector2(x + t[i].x, y + t[i].y);
            }
            return t;
        }

        public static List<Vector2> Simple(float radius, float width, float height) {
            radius = Mathf.Max(MINIMUM_RADIUS, radius);
            var gridSize = radius / SQR_2;
            var gridWidth = width / gridSize;
            var gridHeight = height / gridSize;

            int[,] grid = new int[Mathf.CeilToInt(gridWidth), Mathf.CeilToInt(gridHeight)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> referencePoints = new List<Vector2>();

            var fp = new Vector2(width / 2f, height / 2f);
            points.Add(fp);
            referencePoints.Add(fp);
            grid[(int)(fp.x / gridSize), (int)(fp.y / gridSize)] = points.Count;

            while(referencePoints.Count > 0) {
                var index = 0;// referencePoints.RandomIndex();
                var p = referencePoints[index];
                int attempts = 0;
                while(true) {
                    var dir = Toolkit.Mathematics.Random.OnUnitCircle;
                    var dist = radius + radius * Toolkit.Mathematics.Random.Float;
                    var np = p + dir * dist;

                    if(IsValid(np, radius, gridSize, grid, points)) {
                        points.Add(np);
                        referencePoints.Add(np);
                        grid[(int)(np.x / gridSize), (int)(np.y / gridSize)] = points.Count;
                        break;
                    }

                    attempts++;
                    if(attempts > REJECTION) {
                        referencePoints.RemoveAt(index);
                        break;
                    }
                }
                if(referencePoints.Count > MAXIMUM_REFERENCE_POINTS) {
                    Debug.LogError("No end with: " + points.Count + " points added");
                    break;
                }
            }
            return points;
        }

        public static IEnumerator<Vector2> SimpleEnumerator(float radius, float width, float height, System.Random random) {
            radius = Mathf.Max(MINIMUM_RADIUS, radius);
            var gridSize = radius / SQR_2;
            var gridWidth = width / gridSize;
            var gridHeight = height / gridSize;

            int[,] grid = new int[Mathf.CeilToInt(gridWidth), Mathf.CeilToInt(gridHeight)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> referencePoints = new List<Vector2>();

            var fp = new Vector2(width / 2f, height / 2f);
            points.Add(fp);
            referencePoints.Add(fp);
            yield return fp;
            grid[(int)(fp.x / gridSize), (int)(fp.y / gridSize)] = points.Count;

            while(referencePoints.Count > 0) {
                var index = 0;// referencePoints.RandomIndex();
                var p = referencePoints[index];
                int attempts = 0;
                while(true) {
                    var rot = random.NextFloat() * Mathf.PI * 2f;
                    var dir = new Vector2(Mathf.Sin(rot), Mathf.Cos(rot));
                    var dist = radius + radius * random.NextFloat();
                    var np = p + dir * dist;

                    if(IsValid(np, radius, gridSize, grid, points)) {
                        yield return np;
                        points.Add(np);
                        referencePoints.Add(np);
                        grid[(int)(np.x / gridSize), (int)(np.y / gridSize)] = points.Count;
                        break;
                    }

                    attempts++;
                    if(attempts > REJECTION) {
                        referencePoints.RemoveAt(index);
                        break;
                    }
                }
                if(referencePoints.Count > MAXIMUM_REFERENCE_POINTS) {
                    break;
                }
            }
        }

        public static IEnumerator<Vector2> SimpleEnumerator(float radius, float width, float height) {
            radius = Mathf.Max(MINIMUM_RADIUS, radius);
            var gridSize = radius / SQR_2;
            var gridWidth = width / gridSize;
            var gridHeight = height / gridSize;

            int[,] grid = new int[Mathf.CeilToInt(gridWidth), Mathf.CeilToInt(gridHeight)];
            List<Vector2> points = new List<Vector2>();
            List<Vector2> referencePoints = new List<Vector2>();

            var fp = new Vector2(width / 2f, height / 2f);
            points.Add(fp);
            referencePoints.Add(fp);
            yield return fp;
            grid[(int)(fp.x / gridSize), (int)(fp.y / gridSize)] = points.Count;

            while(referencePoints.Count > 0) {
                var index = 0;// referencePoints.RandomIndex();
                var p = referencePoints[index];
                int attempts = 0;
                while(true) {
                    var dir = Toolkit.Mathematics.Random.OnUnitCircle;
                    var dist = radius + radius * Toolkit.Mathematics.Random.Float;
                    var np = p + dir * dist;

                    if(IsValid(np, radius, gridSize, grid, points)) {
                        yield return np;
                        points.Add(np);
                        referencePoints.Add(np);
                        grid[(int)(np.x / gridSize), (int)(np.y / gridSize)] = points.Count;
                        break;
                    }

                    attempts++;
                    if(attempts > REJECTION) {
                        referencePoints.RemoveAt(index);
                        break;
                    }
                }
                if(referencePoints.Count > MAXIMUM_REFERENCE_POINTS) {
                    break;
                }
            }
        }

        #endregion

        #region Utility

        private static bool IsValid(Vector2 p, float radius, float gridSize, int[,] grid, IReadOnlyList<Vector2> points) {
            var x = (int)(p.x / gridSize);
            var y = (int)(p.y / gridSize);

            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            if(x < 0 || y < 0 || x >= width || y >= height || grid[x, y] != 0)
                return false;

            var ixs = Mathf.Max(0, x - 2);
            var iys = Mathf.Max(0, y - 2);
            var ixe = Mathf.Min(grid.GetLength(0), x + 3);
            var iye = Mathf.Min(grid.GetLength(1), y + 3);

            for(int ix = ixs; ix < ixe; ix++) {
                for(int iy = iys; iy < iye; iy++) {
                    if(!(ix == x && iy == y) && grid[ix, iy] > 0 && (points[grid[ix, iy] - 1] - p).magnitude < radius) {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
