using UnityEngine;

namespace Toolkit.Mathematics {
    [System.Serializable]
    public struct Line {
        #region Variables

        /// Stored as start point and delta
        [SerializeField]
        private float px, py, pz, dx, dy, dz;

        #endregion

        #region Properties

        public Vector3 StartPoint {
            get => new Vector3(px, py, pz);
            set {
                px = value.x;
                py = value.y;
                pz = value.z;
            }
        }
        public Vector3 EndPoint {
            get => new Vector3(px + dx, py + dy, pz + dz);
            set {
                dx = value.x - px;
                dy = value.y - py;
                dz = value.z - pz;
            }
        }
        public Vector3 Direction {
            get => new Vector3(dx, dy, dz);
            set {
                dx = value.x;
                dy = value.y;
                dz = value.z;
            }
        }
        public float Length => Direction.magnitude;

        #endregion

        #region Constructor

        public Line(Vector3 direction) {
            px = 0f;
            py = 0f;
            pz = 0f;
            dx = direction.x;
            dy = direction.y;
            dz = direction.z;
        }

        public Line(Vector3 direction, float distance) {
            direction *= distance;
            px = 0f;
            py = 0f;
            pz = 0f;
            dx = direction.x;
            dy = direction.y;
            dz = direction.z;
        }

        public Line(Vector3 startPoint, Vector3 endPoint) {
            px = startPoint.x;
            py = startPoint.y;
            pz = startPoint.z;
            dx = endPoint.x - startPoint.x;
            dy = endPoint.y - startPoint.y;
            dz = endPoint.z - startPoint.z;
        }

        public Line(Vector3 origin, Vector3 direction, float distance) {
            direction = direction * distance;
            px = origin.x;
            py = origin.y;
            pz = origin.z;
            dx = direction.x;
            dy = direction.y;
            dz = direction.z;
        }

        public Line(Transform t0, Transform t1, Space space = Space.World) {
            if(space == Space.World) {
                var localP0 = t0.position;
                var localP1 = t1.position;
                px = localP0.x;
                py = localP0.y;
                pz = localP0.z;

                dx = localP1.x - localP0.x;
                dy = localP1.y - localP0.y;
                dz = localP1.z - localP0.z;
            }
            else {
                var localP0 = t0.localPosition;
                var localP1 = t1.localPosition;
                px = localP0.x;
                py = localP0.y;
                pz = localP0.z;

                dx = localP1.x - localP0.x;
                dy = localP1.y - localP0.y;
                dz = localP1.z - localP0.z;
            }
        }

        #endregion

        #region Static Constructor

        public static Line Create(Vector3 direction) {
            return new Line(direction);
        }

        public static Line Create(Vector3 direction, float distance) {
            return new Line(direction, distance);
        }

        public static Line Create(Vector3 startPoint, Vector3 endPoint) {
            return new Line(startPoint, endPoint);
        }

        public static Line Create(Vector3 origin, Vector3 direction, float distance) {
            return new Line(origin, direction, distance);
        }

        public static Line Create(Transform t0, Transform t1, Space space = Space.World) {
            return new Line(t0, t1, space);
        }

        public static Line Create<T>(T t0, T t1, Space space = Space.World) where T : Component {
            return new Line(t0.transform, t1.transform, space);
        }

        #endregion

        #region Private Help Functions

        private unsafe float Value(int index) {
            fixed(float* p = &px) {
                return *(p + index);
            }
        }

        private unsafe float Delta(int index) {
            fixed(float* d = &dx) {
                return *(d + index);
            }
        }

        #endregion

        #region Lerp

        public Vector3 Evaluate(float time) {
            return new Vector3(px + dx * time, py + dy * time, pz + dz * time);
        }

        public Vector3 EvaluateClamped(float time) {
            if(time < 0f)
                time = 0f;
            else if(time > 1f)
                time = 1f;
            return new Vector3(px + dx * time, py + dy * time, pz + dz * time);
        }

        #endregion

        #region Closest Point On Line

        public float GetDistanceToLine(Vector3 point) {
            if(dx == 0f && dy == 0f && dz == 0f)
                return Vector3.Distance(StartPoint, point);

            var p = point - StartPoint;
            float t = ((p.x * dx) + (p.y * dy) + (p.z * dz)) / (dx * dx + dy * dy + dz * dz);
            return Vector3.Distance(Evaluate(t), point);
        }

        public float GetDistanceToLineClamped(Vector3 point) {
            if(dx == 0f && dy == 0f && dz == 0f)
                return Vector3.Distance(StartPoint, point);

            var p = point - StartPoint;
            float t = ((p.x * dx) + (p.y * dy) + (p.z * dz)) / (dx * dx + dy * dy + dz * dz);
            return Vector3.Distance(EvaluateClamped(t), point);
        }

        public Vector3 GetClosestPointOnLine(Vector3 point) {
            if(dx == 0f && dy == 0f && dz == 0f)
                return StartPoint;

            var p = point - StartPoint;
            float t = ((p.x * dx) + (p.y * dy) + (p.z * dz)) / (dx * dx + dy * dy + dz * dz);
            return Evaluate(t);
        }

        public Vector3 GetClosestPointOnLineClamped(Vector3 point) {
            if(dx == 0f && dy == 0f && dz == 0f)
                return StartPoint;

            var p = point - StartPoint;
            float t = ((p.x * dx) + (p.y * dy) + (p.z * dz)) / (dx * dx + dy * dy + dz * dz);
            return EvaluateClamped(t);
        }

        public float GetValueFromPointOnLine(Vector3 point) {
            if(dx == 0f && dy == 0f && dz == 0f)
                return Vector3.Distance(point, StartPoint);

            var p = StartPoint - point;
            float t = ((p.x * dx) + (p.y * dy) + (p.z * dz)) / (dx * dx + dy * dy + dz * dz);
            return t;
        }

        #endregion

        #region Intersection

        public Vector3? GetPointAtIntersection(Plane plane) {
            var dir = Direction;
            var dotPlaneDir = Vector3.Dot(plane.normal, dir);
            if(dotPlaneDir == 0f) {
                return null;
            }
            float f = (Vector3.Dot(plane.normal, plane.normal * plane.distance) - Vector3.Dot(plane.normal, StartPoint)) / dotPlaneDir;
            return StartPoint + (dir * f);
        }

        public Vector3? GetPointAtIntersection(int index, float value) {
            var dir = Direction;
            var val = Delta(index);
            if(val == 0f)
                return null;

            var sVal = Value(index);
            dir /= Mathf.Abs(val);
            value -= sVal;

            if(val < 0f)
                return new Vector3(
                    px + (dir.x * -value),
                    py + (dir.y * -value),
                    pz + (dir.z * -value));

            return new Vector3(
                px + (dir.x * value),
                py + (dir.y * value),
                pz + (dir.z * value));
        }

        public Vector3? GetPointAtIntersectionX(float value) {
            return GetPointAtIntersection(0, value);
        }

        public Vector3? GetPointAtIntersectionY(float value) {
            return GetPointAtIntersection(1, value);
        }

        public Vector3? GetPointAtIntersectionZ(float value) {
            return GetPointAtIntersection(2, value);
        }


        // 2d intersection implementation
        public static bool GetPointAtIntersectionXY(Line lhs, Line rhs, out Vector2 point) {
            float det = -lhs.dy * rhs.dx - -rhs.dy * lhs.dx;

            if(det == 0) {
                point = new Vector2();
                return false;
            }
            else {
                float t1 = -lhs.dy * lhs.px + lhs.dx * lhs.py;
                float t2 = -rhs.dy * rhs.px + rhs.dx * rhs.py;
                point = new Vector2((rhs.dx * t1 - lhs.dx * t2) / det, (-lhs.dy * t2 - -rhs.dy * t1) / det);
                return true;
            }
        }

        public static bool GetPointAtIntersectionXY(Vector2 lhs0, Vector2 lhs1, Vector2 rhs0, Vector2 rhs1, out Vector2 point) {
            var lhsd = lhs1-lhs0;
            var rhsd = rhs1-rhs0;
            float det = -lhsd.y * rhsd.x - -rhsd.y * lhsd.x;

            if(det == 0) {
                point = new Vector2();
                return false;
            }
            else {
                float t1 = -lhsd.y * lhs0.x + lhsd.x * lhs0.y;
                float t2 = -rhsd.y * rhs0.x + rhsd.x * rhs0.y;
                point = new Vector2((rhsd.x * t1 - lhsd.x * t2) / det, (-lhsd.y * t2 - -rhsd.y * t1) / det);

                return true;
            }
        }

        public static bool GetPointAtIntersectionXYClamped(Vector2 lhs0, Vector2 lhs1, Vector2 rhs0, Vector2 rhs1) {
            var lhsd = lhs1-lhs0;
            var rhsd = rhs1-rhs0;
            float det = -lhsd.y * rhsd.x - -rhsd.y * lhsd.x;

            if(det == 0) {
                return false;
            }
            else {
                float t1 = -lhsd.y * lhs0.x + lhsd.x * lhs0.y;
                float t2 = -rhsd.y * rhs0.x + rhsd.x * rhs0.y;
                var point = new Vector2((rhsd.x * t1 - lhsd.x * t2) / det, (-lhsd.y * t2 - -rhsd.y * t1) / det);

                float tIntersectionLHS = Vector2.Dot(point - lhs0, lhsd) / Vector2.Dot(lhsd, lhsd);
                float tIntersectionRHS = Vector2.Dot(point - rhs0, rhsd) / Vector2.Dot(rhsd, rhsd);

                if(tIntersectionLHS < 0 || tIntersectionLHS > 1 || tIntersectionRHS < 0 || tIntersectionRHS > 1) {
                    return false;
                }
                return true;
            }
        }

        public static bool GetPointAtIntersectionXYClamped(Vector2 lhs0, Vector2 lhs1, Vector2 rhs0, Vector2 rhs1, out Vector2 point) {
            var lhsd = lhs1-lhs0;
            var rhsd = rhs1-rhs0;
            float det = -lhsd.y * rhsd.x - -rhsd.y * lhsd.x;

            if(det == 0) {
                point = new Vector2();
                return false;
            }
            else {
                float t1 = -lhsd.y * lhs0.x + lhsd.x * lhs0.y;
                float t2 = -rhsd.y * rhs0.x + rhsd.x * rhs0.y;
                point = new Vector2((rhsd.x * t1 - lhsd.x * t2) / det, (-lhsd.y * t2 - -rhsd.y * t1) / det);

                float tIntersectionLHS = Vector2.Dot(point - lhs0, lhsd) / Vector2.Dot(lhsd, lhsd);
                float tIntersectionRHS = Vector2.Dot(point - rhs0, rhsd) / Vector2.Dot(rhsd, rhsd);

                if(tIntersectionLHS < 0 || tIntersectionLHS > 1 || tIntersectionRHS < 0 || tIntersectionRHS > 1) {
                    float clampedTIntersectionLHS = Mathf.Clamp01(tIntersectionLHS);
                    point = lhs0 + clampedTIntersectionLHS * lhsd;
                    return false;
                }

                return true;
            }
        }

         public static void FindCollisionEnterExitEvents(Line l0, Line l1, float radius0, float radius1, System.Collections.Generic.List<Vector4> points) {
            points.Clear();
            var combined = radius0 + radius1;
            var eitherLength = Mathf.Max(l0.Length, l1.Length);
            if (eitherLength < Mathf.Epsilon) {
                if (Vector3.Distance(l0.StartPoint, l1.StartPoint) < combined) {
                    points.Add(Vector3.Lerp(l0.StartPoint, l1.StartPoint, radius0 / combined).To_Vector4(0f));
                }
                return;
            }

            float step = Mathf.Min(radius0, radius1) / Mathf.Max(l0.Length, l1.Length);
            step = Mathf.Max(0.02f, step);
            float lastDistance = 0;
            bool isInside = false;

            for (float t = 0; t < 1f; t += step) {
                var p0 = l0.Evaluate(t);
                var p1 = l1.Evaluate(t);
                var dist = Vector3.Distance(p0, p1) - combined;
                if (dist < Mathf.Epsilon) {
                    if (!isInside) {
                        var it = Mathf.InverseLerp(lastDistance, dist, 0f);
                        var estimatedT = Mathf.Lerp(t - step, t, it);
                        points.Add(Vector3.Lerp(l0.Evaluate(estimatedT), l1.Evaluate(estimatedT), radius0 / combined).To_Vector4(estimatedT));
                    }
                    isInside = true;
                }
                else {
                    if (isInside) {
                        var it = Mathf.InverseLerp(lastDistance, dist, 0f);
                        var estimatedT = Mathf.Lerp(t - step, t, it);
                        points.Add(Vector3.Lerp(l0.Evaluate(estimatedT), l1.Evaluate(estimatedT), radius0 / combined).To_Vector4(estimatedT));
                    }
                    isInside = false;
                }
                lastDistance = dist;
            }

            // Safety to check 1
            {
                var p0 = l0.Evaluate(1f);
                var p1 = l1.Evaluate(1f);
                var dist = Vector3.Distance(p0, p1) - combined;
                if (dist < Mathf.Epsilon) {
                    if (!isInside) {
                        var it = Mathf.InverseLerp(lastDistance, dist, 0f);
                        var estimatedT = Mathf.Lerp(1f - step, 1f, it);
                        points.Add(Vector3.Lerp(l0.Evaluate(estimatedT), l1.Evaluate(estimatedT), radius0 / combined).To_Vector4(estimatedT));
                    }
                    isInside = true;
                }
                else {
                    if (isInside) {
                        var it = Mathf.InverseLerp(lastDistance, dist, 0f);
                        var estimatedT = Mathf.Lerp(1f - step, 1f, it);
                        points.Add(Vector3.Lerp(l0.Evaluate(estimatedT), l1.Evaluate(estimatedT), radius0 / combined).To_Vector4(estimatedT));
                    }
                    isInside = false;
                }
                lastDistance = dist;
            }
        }

        #endregion

        #region Rotate

        public Line Rotate(Vector3 axis, float angle) {
            var rot = Quaternion.AngleAxis(angle, axis);
            return new Line(rot * StartPoint, rot * EndPoint);
        }

        public Line Rotate(Quaternion rotation) {
            return new Line(rotation * StartPoint, rotation * EndPoint);
        }

        #endregion

        #region Move

        public void MoveRelative(float x, float y) {
            if(!dz.Equals(0, Mathf.Epsilon)) {
                throw new System.NotImplementedException($"Not implemented support for delta z stuff... ({dz})");
            }
            else {
                var angle = Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(0f, 1f), new Vector2(dx, dy));
                var c = Mathf.Cos(angle);
                var s = Mathf.Sin(angle);
                px += c * x - s * y;
                py += s * x + c * y;
            }
        }

        #endregion

        #region Raycasting // Extend this for more support

        public bool Raycast(out RaycastHit hit) {
            return Physics.Raycast(StartPoint, Direction, out hit);
        }

        #endregion
    }
}
