using System;
using UnityEngine;

namespace Toolkit.Mathematics {
    [System.Serializable]
    public struct Triangle {
        #region Variables

        public Vector3 point0;
        public Vector3 point1;
        public Vector3 point2;

        #endregion

        #region Properties

        public unsafe Vector3 this[int index] {
            get {
#if UNITY_EDITOR
                if(index < 0 || index > 2)
                    throw new IndexOutOfRangeException();
#endif
                fixed(Vector3* p = (&point0)) {
                    return *(p + index);
                }
            }
            set {
#if UNITY_EDITOR
                if(index < 0 || index > 2)
                    throw new IndexOutOfRangeException();
#endif
                fixed(Vector3* p = (&point0)) {
                    (*(p + index)) = value;
                }
            }
        }

        public float AnglePoint0 {
            get {
                var p0p1Length = Vector3.Distance(point0, point1);
                var p1p2Length = Vector3.Distance(point1, point2);
                var p2p0Length = Vector3.Distance(point2, point0);
                return Mathf.Rad2Deg * Mathf.Acos((p2p0Length * p2p0Length + p0p1Length * p0p1Length - p1p2Length * p1p2Length) / (2f * p2p0Length * p0p1Length));
            }
        }

        public float AnglePoint1 {
            get {
                var p1p2Length = Vector3.Distance(point1, point2);
                var p2p0Length = Vector3.Distance(point2, point0);
                var p0p1Length = Vector3.Distance(point0, point1);
                return Mathf.Rad2Deg * Mathf.Acos((p0p1Length * p0p1Length + p1p2Length * p1p2Length - p2p0Length * p2p0Length) / (2f * p0p1Length * p1p2Length));
            }
        }

        public float AnglePoint2 {
            get {
                var p2p0Length = Vector3.Distance(point2, point0);
                var p0p1Length = Vector3.Distance(point0, point1);
                var p1p2Length = Vector3.Distance(point1, point2);
                return Mathf.Rad2Deg * Mathf.Acos((p1p2Length * p1p2Length + p2p0Length * p2p0Length - p0p1Length * p0p1Length) / (2f * p1p2Length * p2p0Length));
            }
        }

        public float Perimiter => Vector3.Distance(point0, point1) + Vector3.Distance(point1, point2) + Vector3.Distance(point2, point0);

        public float Area {
            get {
                var p0p1Length = Vector3.Distance(point0, point1);
                var p1p2Length = Vector3.Distance(point1, point2);
                var p2p0Length = Vector3.Distance(point2, point0);
                var p1AngleRad = Mathf.Acos((p0p1Length * p0p1Length + p1p2Length * p1p2Length - p2p0Length * p2p0Length) / (2f * p0p1Length * p1p2Length));
                return (p1p2Length * p0p1Length * Mathf.Sin(p1AngleRad)) / 2f;
            }
        }

        public Vector3 Normal {
            get {
                var u = point1 - point0;
                var v = point2 - point0;

                return new Vector3(
                    (u.y * v.z) - (u.z * v.y),
                    (u.z * v.x) - (u.x * v.z),
                    (u.x * v.y) - (u.y * v.x)).normalized;
            }
        }

        public Vector3 Center {
            get {
                return (point0 + point1 + point2) / 3f;
            }
        }

        public Bounds BoundingBox {
            get {
                var minX = MathUtility.Min(point0.x, point1.x, point2.x);
                var minY = MathUtility.Min(point0.x, point1.x, point2.x);
                var minZ = MathUtility.Min(point0.x, point1.x, point2.x);

                var maxX = MathUtility.Max(point0.x, point1.x, point2.x);
                var maxY = MathUtility.Max(point0.x, point1.x, point2.x);
                var maxZ = MathUtility.Max(point0.x, point1.x, point2.x);

                return new Bounds() {
                    min = new Vector3(minX, minY, minZ),
                    max = new Vector3(maxX, maxY, maxZ)
                };
            }
        }

        #endregion

        #region Constructor

        public Triangle(Vector3 p0, Vector3 p1, Vector3 p2) {
            this.point0 = p0;
            this.point1 = p1;
            this.point2 = p2;
        }

        #endregion

        #region Calculate

        public void CalculateAll(out float anglePoint0, out float anglePoint1, out float anglePoint2, out float perimiter, out float area) {
            var p0p1Length = Vector3.Distance(point0, point1);
            var p1p2Length = Vector3.Distance(point1, point2);
            var p2p0Length = Vector3.Distance(point2, point0);
            anglePoint0 = Mathf.Rad2Deg * Mathf.Acos((p2p0Length * p2p0Length + p0p1Length * p0p1Length - p1p2Length * p1p2Length) / (2f * p2p0Length * p0p1Length));
            anglePoint1 = Mathf.Rad2Deg * Mathf.Acos((p0p1Length * p0p1Length + p1p2Length * p1p2Length - p2p0Length * p2p0Length) / (2f * p0p1Length * p1p2Length));
            anglePoint2 = Mathf.Rad2Deg * Mathf.Acos((p1p2Length * p1p2Length + p2p0Length * p2p0Length - p0p1Length * p0p1Length) / (2f * p1p2Length * p2p0Length));
            perimiter = p0p1Length + p1p2Length + p2p0Length;
            area = (p1p2Length * p0p1Length * Mathf.Sin(anglePoint1 * Mathf.Deg2Rad)) / 2f;
        }

        #endregion

        #region Intersect

        public bool Intersect(Ray ray, out Vector3 hit) {
            Vector3 e1 = point1 - point0;
            Vector3 e2 = point2 - point0;
            Vector3 p = Vector3.Cross(ray.direction, e2);
            float det = Vector3.Dot(e1, p);
            if(det > -Mathf.Epsilon && det < Mathf.Epsilon) { hit = new Vector3(); return false; }
            float invDet = 1.0f / det;
            Vector3 t = ray.origin - point0;
            float u = Vector3.Dot(t, p) * invDet;
            if(u < 0 || u > 1) { hit = new Vector3(); return false; }
            Vector3 q = Vector3.Cross(t, e1);
            float v = Vector3.Dot(ray.direction, q) * invDet;
            if(v < 0 || u + v > 1) { hit = new Vector3(); return false; }
            hit = point0 + u * e1 + v * e2;
            return (Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon;
        }

        public bool Intersect(Vector3 point, Vector3 direction, out Vector3 hit) {
            Vector3 e1 = point1 - point0;
            Vector3 e2 = point2 - point0;
            Vector3 p = Vector3.Cross(direction, e2);
            float det = Vector3.Dot(e1, p);
            if(det > -Mathf.Epsilon && det < Mathf.Epsilon) { hit = new Vector3(); return false; }
            float invDet = 1.0f / det;
            Vector3 t = point - point0;
            float u = Vector3.Dot(t, p) * invDet;
            if(u < 0 || u > 1) { hit = new Vector3(); return false; }
            Vector3 q = Vector3.Cross(t, e1);
            float v = Vector3.Dot(direction, q) * invDet;
            if(v < 0 || u + v > 1) { hit = new Vector3(); return false; }
            hit = point0 + u * e1 + v * e2;
            return (Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon;
        }

        /// <summary>
        /// Not optimized at all
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Intersect(Triangle other, out Line result) {
            Vector3 hit0;
            Vector3 hit1;

            if(Intersect(other.point0, other.point1 - other.point0, out hit0)) {
                if(Intersect(other.point1, other.point2 - other.point1, out hit1) ||
                    Intersect(other.point2, other.point0 - other.point2, out hit1) ||
                    other.Intersect(point0, point1 - point0, out hit1) ||
                    other.Intersect(point1, point2 - point1, out hit1) ||
                    other.Intersect(point2, point0 - point2, out hit1)) {
                    result = new Line(hit0, hit1);
                    return true;
                }
            }
            else if(Intersect(other.point1, other.point2 - other.point1, out hit0)) {
                if(Intersect(other.point0, other.point1 - other.point0, out hit1) ||
                    Intersect(other.point2, other.point0 - other.point2, out hit1) ||
                    other.Intersect(point0, point1 - point0, out hit1) ||
                    other.Intersect(point1, point2 - point1, out hit1) ||
                    other.Intersect(point2, point0 - point2, out hit1)) {
                    result = new Line(hit0, hit1);
                    return true;
                }
            }
            else if(Intersect(other.point2, other.point0 - other.point2, out hit0)) {
                if(Intersect(other.point0, other.point1 - other.point0, out hit1) ||
                    Intersect(other.point1, other.point2 - other.point1, out hit1) ||
                    other.Intersect(point0, point1 - point0, out hit1) ||
                    other.Intersect(point1, point2 - point1, out hit1) ||
                    other.Intersect(point2, point0 - point2, out hit1)) {
                    result = new Line(hit0, hit1);
                    return true;
                }
            }
            else {
                if(other.Intersect(point0, point1 - point0, out hit0)) {
                    if(other.Intersect(point1, point2 - point1, out hit1) ||
                        other.Intersect(point2, point0 - point2, out hit1) ||
                        Intersect(other.point0, other.point1 - other.point0, out hit1) ||
                        Intersect(other.point1, other.point2 - other.point1, out hit1) ||
                        Intersect(other.point2, other.point0 - other.point2, out hit1)) {
                        result = new Line(hit0, hit1);
                        return true;
                    }
                }
                else if(other.Intersect(point1, point2 - point1, out hit0)) {
                    if(other.Intersect(point0, point1 - point0, out hit1) ||
                        other.Intersect(point2, point0 - point2, out hit1) ||
                        Intersect(other.point0, other.point1 - other.point0, out hit1) ||
                        Intersect(other.point1, other.point2 - other.point1, out hit1) ||
                        Intersect(other.point2, other.point0 - other.point2, out hit1)) {
                        result = new Line(hit0, hit1);
                        return true;
                    }
                }
                else if(other.Intersect(point2, point0 - point2, out hit0)) {
                    if(other.Intersect(point0, point1 - point0, out hit1) ||
                        other.Intersect(point1, point2 - point1, out hit1) ||
                        Intersect(other.point0, other.point1 - other.point0, out hit1) ||
                        Intersect(other.point1, other.point2 - other.point1, out hit1) ||
                        Intersect(other.point2, other.point0 - other.point2, out hit1)) {
                        result = new Line(hit0, hit1);
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }

        #endregion

        #region Closest/Distance To Point

        public float DistanceToPoint(Vector3 p)
            => Vector3.Distance(p, ClosestPoint(p));

        public void PrecalculateClosestPointCache(out ClosestPointCache cache) {
            cache.edge0x = point1.x - point0.x;
            cache.edge0y = point1.y - point0.y;
            cache.edge0z = point1.z - point0.z;
            cache.edge1x = point2.x - point0.x;
            cache.edge1y = point2.y - point0.y;
            cache.edge1z = point2.z - point0.z;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            static float Dot(float x0, float y0, float z0, float x1, float y1, float z1) => (x0 * x1) + (y0 * y1) + (z0 * z1);

            cache.a = Dot(cache.edge0x, cache.edge0y, cache.edge0z, cache.edge0x, cache.edge0y, cache.edge0z);
            cache.b = Dot(cache.edge0x, cache.edge0y, cache.edge0z, cache.edge1x, cache.edge1y, cache.edge1z);
            cache.c = Dot(cache.edge1x, cache.edge1y, cache.edge1z, cache.edge1x, cache.edge1y, cache.edge1z);
            cache.det = cache.a * cache.c - cache.b * cache.b;
        }

        public struct ClosestPointCache {
            public float edge0x;
            public float edge0y;
            public float edge0z;
            public float edge1x;
            public float edge1y;
            public float edge1z;

            public float a;
            public float b;
            public float c;
            public float det;
        }

        public Vector3 ClosestPoint(Vector3 p, ClosestPointCache cache) {
            float v0topx = point0.x - p.x;
            float v0topy = point0.y - p.y;
            float v0topz = point0.z - p.z;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            static float Dot(float x0, float y0, float z0, float x1, float y1, float z1) => (x0 * x1) + (y0 * y1) + (z0 * z1);

            float d = Dot(cache.edge0x, cache.edge0y, cache.edge0z, v0topx, v0topy, v0topz);
            float e = Dot(cache.edge1x, cache.edge1y, cache.edge1z, v0topx, v0topy, v0topz);

            float s = cache.b * e - cache.c * d;
            float t = cache.b * d - cache.a * e;

            if(s + t <= cache.det) {
                if(s < 0) {
                    if(t < 0) {
                        if(d < 0) {
                            s = Mathf.Clamp01(-d / cache.a);
                            t = 0;
                        }
                        else {
                            s = 0;
                            t = Mathf.Clamp01(-e / cache.c);
                        }
                    }
                    else {
                        s = 0;
                        t = Mathf.Clamp01(-e / cache.c);
                    }
                }
                else if(t < 0) {
                    s = Mathf.Clamp01(-d / cache.a);
                    t = 0;
                }
                else {
                    float invDet = 1 / cache.det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else {
                if(s < 0) {
                    s = Mathf.Clamp01((cache.b - d) / (cache.a - 2 * cache.b + cache.c));
                    t = 1 - s;
                }
                else if(t < 0) {
                    s = Mathf.Clamp01((cache.b - e) / (cache.a - 2 * cache.b + cache.c));
                    t = 1 - s;
                }
                else {
                    float num = cache.c + e - cache.b - d;
                    float denom = cache.a - 2 * cache.b + cache.c;
                    s = Mathf.Clamp01(num / denom);
                    t = 1 - s;
                }
            }

            return new Vector3(
                point0.x + s * cache.edge0x + t * cache.edge1x,
                point0.y + s * cache.edge0y + t * cache.edge1y,
                point0.z + s * cache.edge0z + t * cache.edge1z);
        }

        public Vector3 ClosestPoint(Vector3 p) {
            float edge0x = point1.x - point0.x;
            float edge0y = point1.y - point0.y;
            float edge0z = point1.z - point0.z;
            float edge1x = point2.x - point0.x;
            float edge1y = point2.y - point0.y;
            float edge1z = point2.z - point0.z;
            float v0topx = point0.x - p.x;
            float v0topy = point0.y - p.y;
            float v0topz = point0.z - p.z;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            static float Dot(float x0, float y0, float z0, float x1, float y1, float z1) => (x0 * x1) + (y0 * y1) + (z0 * z1);

            float a = Dot(edge0x, edge0y, edge0z, edge0x, edge0y, edge0z);
            float b = Dot(edge0x, edge0y, edge0z, edge1x, edge1y, edge1z);
            float c = Dot(edge1x, edge1y, edge1z, edge1x, edge1y, edge1z);

            float d = Dot(edge0x, edge0y, edge0z, v0topx, v0topy, v0topz);
            float e = Dot(edge1x, edge1y, edge1z, v0topx, v0topy, v0topz);

            float det = a * c - b * b;
            float s = b * e - c * d;
            float t = b * d - a * e;

            if(s + t <= det) {
                if(s < 0) {
                    if(t < 0) {
                        if(d < 0) {
                            s = Mathf.Clamp01(-d / a);
                            t = 0;
                        }
                        else {
                            s = 0;
                            t = Mathf.Clamp01(-e / c);
                        }
                    }
                    else {
                        s = 0;
                        t = Mathf.Clamp01(-e / c);
                    }
                }
                else if(t < 0) {
                    s = Mathf.Clamp01(-d / a);
                    t = 0;
                }
                else {
                    float invDet = 1 / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else {
                if(s < 0) {
                    s = Mathf.Clamp01((b - d) / (a - 2 * b + c));
                    t = 1 - s;
                }
                else if(t < 0) {
                    s = Mathf.Clamp01((b - e) / (a - 2 * b + c));
                    t = 1 - s;
                }
                else {
                    float num = c + e - b - d;
                    float denom = a - 2 * b + c;
                    s = Mathf.Clamp01(num / denom);
                    t = 1 - s;
                }
            }

            return new Vector3(
                point0.x + s * edge0x + t * edge1x,
                point0.y + s * edge0y + t * edge1y,
                point0.z + s * edge0z + t * edge1z);
        }

        #endregion

        #region Static Angle Calculations

        public static float CalculateAngle(Vector3 p0, Vector3 p1, Vector3 p2) {
            var p0p1Length = Vector3.Distance(p0, p1);
            var p1p2Length = Vector3.Distance(p1, p2);
            var p2p0Length = Vector3.Distance(p2, p0);
            return Mathf.Rad2Deg * Mathf.Acos((p2p0Length * p2p0Length + p0p1Length * p0p1Length - p1p2Length * p1p2Length) / (2f * p2p0Length * p0p1Length));
        }

        public static float CalculateAngleRad(Vector3 p0, Vector3 p1, Vector3 p2) {
            var p0p1Length = Vector3.Distance(p0, p1);
            var p1p2Length = Vector3.Distance(p1, p2);
            var p2p0Length = Vector3.Distance(p2, p0);
            return Mathf.Acos((p2p0Length * p2p0Length + p0p1Length * p0p1Length - p1p2Length * p1p2Length) / (2f * p2p0Length * p0p1Length));
        }

        public static float CalculateAngle(float oppositeLength, float leftSideLength, float rightSideLength) {
            return Mathf.Rad2Deg * Mathf.Acos((rightSideLength * rightSideLength + leftSideLength * leftSideLength - oppositeLength * oppositeLength) / (2f * rightSideLength * leftSideLength));
        }

        public static float CalculateAngleRad(float oppositeLength, float leftSideLength, float rightSideLength) {
            return Mathf.Acos((rightSideLength * rightSideLength + leftSideLength * leftSideLength - oppositeLength * oppositeLength) / (2f * rightSideLength * leftSideLength));
        }

        #endregion

        #region Static Perimiter Calculation

        public static float CalculatePerimiter(Vector3 p0, Vector3 p1, Vector3 p2) {
            return Vector3.Distance(p0, p1) + Vector3.Distance(p1, p2) + Vector3.Distance(p2, p0);
        }

        #endregion

        #region Static Area Calculation

        public static float CalculateArea(Vector3 p0, Vector3 p1, Vector3 p2) {
            var p0p1Length = Vector3.Distance(p0, p1);
            var p1p2Length = Vector3.Distance(p1, p2);
            var p2p0Length = Vector3.Distance(p2, p0);
            var p1AngleRad = Mathf.Acos((p0p1Length * p0p1Length + p1p2Length * p1p2Length - p2p0Length * p2p0Length) / (2f * p0p1Length * p1p2Length));
            return (p1p2Length * p0p1Length * Mathf.Sin(p1AngleRad)) / 2f;
        }

        #endregion

        #region Calculate Third Point

        public static Vector3 CalculateThirdPoint(Vector3 p0, Vector3 p1, float p0p2Distance, float p1p2Distance, Vector3 normal) {
            var p0p1Length = Vector3.Distance(p0, p1);
            var p0Angle = Mathf.Acos((p0p2Distance * p0p2Distance + p0p1Length * p0p1Length - p1p2Distance * p1p2Distance) / (2f * p0p2Distance * p0p1Length));
            return Quaternion.FromToRotation(Vector3.up, normal) * new Vector3(Mathf.Sin(p0Angle) * p0p2Distance, 0, Mathf.Cos(p0Angle) * p0p2Distance);
        }

        public static Vector2 CalculateThirdPoint(Vector2 p0, Vector2 p1, float p0p2Distance, float p1p2Distance) {
            var p0p1Length = Vector2.Distance(p0, p1);
            var p0Angle = Mathf.Acos((p0p2Distance * p0p2Distance + p0p1Length * p0p1Length - p1p2Distance * p1p2Distance) / (2f * p0p2Distance * p0p1Length));
            return new Vector2(Mathf.Sin(p0Angle) * p0p2Distance, Mathf.Cos(p0Angle) * p0p2Distance);
        }

        #endregion
    }
}
