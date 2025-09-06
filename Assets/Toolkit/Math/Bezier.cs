using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Mathematics
{
    [System.Serializable]
    public struct Bezier
    {
        #region Const

        public const int POINTS = 4;
        public const int DEFAULT_LENGTH_PRECISION = 12;

        #endregion

        #region Variables

        public Vector3 startPoint;
        public Vector3 startHandle;

        public Vector3 endHandle;
        public Vector3 endPoint;

        #endregion

        #region Properties

        public unsafe Vector3 this[int index] {
            get {
                fixed(Vector3* p = (&startPoint)) {
                    return *(p + index);
                }
            }
            set {
                fixed(Vector3* p = (&startPoint)) {
                    (*(p + index)) = value;
                }
            }
        }

        public static Bezier Default {
            get {
                return new Bezier(
                    new Vector3(0f, 0f, 0f),
                    new Vector3(1f, 0f, 0f),
                    new Vector3(2f, 0f, 0f),
                    new Vector3(3f, 0f, 0f));
            }
        }

        #endregion

        #region Constructor

        public Bezier(Vector3 startPoint, Vector3 startHandle, Vector3 endHandle, Vector3 endPoint) {
            this.startPoint = startPoint;
            this.startHandle = startHandle;
            this.endHandle = endHandle;
            this.endPoint = endPoint;
        }

        public Bezier(Transform start, Transform end)
            : this(start, end, Space.World) { }

        public Bezier(Transform start, Transform end, Space space) {
            if(space == Space.World) {
                this.startPoint = start.position;
                this.startHandle = startPoint + start.forward;
                this.endPoint = end.position;
                this.endHandle = endPoint - end.forward;
            }
            else {
                this.startPoint = start.localPosition;
                this.startHandle = startPoint + start.localRotation * new Vector3(0, 0, 1f);
                this.endPoint = end.localPosition;
                this.endHandle = endPoint - end.localRotation * new Vector3(0, 0, 1f);
            }
        }

        public Bezier(Transform start, Transform end, Space space, float distance) {
            if(space == Space.World) {
                this.startPoint = start.position;
                this.startHandle = startPoint + start.rotation * new Vector3(0, 0, distance);
                this.endPoint = end.position;
                this.endHandle = endPoint - end.rotation * new Vector3(0, 0, distance);
            }
            else {
                this.startPoint = start.localPosition;
                this.startHandle = startPoint + start.localRotation * new Vector3(0, 0, distance);
                this.endPoint = end.localPosition;
                this.endHandle = endPoint - end.localRotation * new Vector3(0, 0, distance);
            }
        }

        public Bezier(Transform startPoint, Transform startHandle, Transform endHandle, Transform endPoint)
            : this(startPoint, startHandle, endHandle, endPoint, Space.World) { }

        public Bezier(Transform startPoint, Transform startHandle, Transform endHandle, Transform endPoint, Space space) {
            if(space == Space.World) {
                this.startPoint = startPoint.position;
                this.startHandle = startHandle.position;
                this.endHandle = endHandle.position;
                this.endPoint = endPoint.position;
            }
            else {
                this.startPoint = startPoint.localPosition;
                this.startHandle = startHandle.localPosition;
                this.endHandle = endHandle.localPosition;
                this.endPoint = endPoint.localPosition;
            }
        }

        #endregion

        #region Evaluation

        public Vector3 Evaluate(float t) {
            t = Mathf.Clamp01(t);
            float rt = 1f - t;

            return (rt * rt * rt) * startPoint
                + (3f * rt * rt * t) * startHandle
                + (3f * rt * t * t) * endHandle
                + (t * t * t) * endPoint;
        }

        public static Vector3 Evaluate(Vector3 start, Vector3 startHandle, Vector3 endHandle, Vector3 end, float time) {
            time = Mathf.Clamp01(time);
            float rt = 1f - time;

            return (rt * rt * rt) * start
                + (3f * rt * rt * time) * startHandle
                + (3f * rt * time * time) * endHandle
                + (time * time * time) * end;
        }

        public static Vector3 Evaluate(Transform start, Transform end, float time)
            => Evaluate(start.position, start.position + start.forward, end.position - end.forward, end.position, time);

        public Vector3 EvaluateTangent(float t) {
            t = Mathf.Clamp01(t);
            float rt = 1f - t;
            return (rt * rt * -3f) * startPoint +
                (rt * rt * 3f) * startHandle -
                (rt * (6f * t)) * startHandle -
                (t * t * 3f) * endHandle +
                (6f * t * rt) * endHandle +
                (3f * t * t) * endPoint;
        }

        #endregion

        #region Length

        public float CalculateLength() {
            const float tLength = 1f / DEFAULT_LENGTH_PRECISION;
            float result = 0f;
            var t1 = startPoint;
            for(int i = 1; i <= DEFAULT_LENGTH_PRECISION; i++) {
                var t2 = Evaluate(tLength * i);
                result += Vector3.Distance(t1, t2);
                t1 = t2;
            }
            return result;
        }

        public float CalculateLength(int precision) {
            if(precision < 1) {
                Debug.LogError($"Attempting to calculate length with a precision lower than 1: [{precision}]");
                precision = 1;
            }
            float tLength = 1f / precision;
            float result = 0f;
            var t1 = startPoint;
            for(int i = 1; i <= precision; i++) {
                var t2 = Evaluate(tLength * i);
                result += Vector3.Distance(t1, t2);
                t1 = t2;
            }
            return result;
        }

        #endregion

        #region Arc-Length Parameterization

        public AnimationCurve ArcLengthParameterizedCurve()
            => ArcLengthParameterizedCurve(CalculateLength(DEFAULT_LENGTH_PRECISION * 2));

        public AnimationCurve ArcLengthParameterizedCurve(float estimatedLength)
            => ArcLengthParameterizedCurve(estimatedLength, DEFAULT_LENGTH_PRECISION * 2);

        public AnimationCurve ArcLengthParameterizedCurve(float estimatedLength, int precision) {
            if(precision < 1) {
                Debug.LogError($"Attempting to calculate fixed distance curve with a precision lower than 1: [{precision}]");
                precision = 1;
            }
            AnimationCurve curve = new AnimationCurve();

            float tLength = 1f / precision;
            float result = 0f;
            var t1 = startPoint;
            curve.AddKey(0f, 0f);
            for(int i = 1; i <= precision; i++) {
                var t2 = Evaluate(tLength * i);
                result += Vector3.Distance(t1, t2);
                curve.AddKey(result / estimatedLength, tLength * i);
                t1 = t2;
            }

            for(int i = 0, length = curve.length; i < length; i++) {
                curve.SmoothTangents(i, 1f);
            }

            return curve;
        }

        #endregion

        #region Static Creation

        public static Bezier CreateSmoothCurve(Transform origin, Transform destination) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + origin.forward * dist, p1 + destination.forward * dist, p1);
        }

        public static Bezier CreateSmoothCurve(Pose origin, Pose destination) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + origin.forward * dist, p1 + destination.forward * dist, p1);
        }

        public static Bezier CreateSmoothCurve(Transform origin, Transform destination, bool invertDestinationDirection) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + origin.forward * dist, p1 + (invertDestinationDirection ? -destination.forward : destination.forward) * dist, p1);
        }

        public static Bezier CreateSmoothCurve(Pose origin, Pose destination, bool invertDestinationDirection) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + origin.forward * dist, p1 + (invertDestinationDirection ? -destination.forward : destination.forward) * dist, p1);
        }

        public static Bezier CreateSmoothCurveAdvanced(Pose origin, bool invertOriginHandle, Pose destination, bool invertDestinationHandle) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + (invertOriginHandle ? -origin.forward : origin.forward) * dist, p1 + (invertDestinationHandle ? -destination.forward : destination.forward) * dist, p1);
        }

        public static Bezier CreateSmoothCurveAdvanced(Pose origin, bool invertOriginHandle, float originHandleStrength, Pose destination, bool invertDestinationHandle, float destinationHandleStrength) {
            var p0 = origin.position;
            var p1 = destination.position;
            var dist = Vector3.Distance(p0, p1) / 2f;
            return new Bezier(p0, p0 + (invertOriginHandle ? -origin.forward : origin.forward) * (dist * originHandleStrength), p1 + (invertDestinationHandle ? -destination.forward : destination.forward) * (dist * destinationHandleStrength), p1);
        }

        #endregion

        #region Operator

        public static Bezier operator *(Quaternion quaternion, Bezier bezier) {
            var origin = bezier.startPoint;
            var originHandle = bezier.startHandle - origin;
            var destination = bezier.endPoint - origin;
            var destinationHandle = bezier.endHandle - origin;

            return new Bezier(origin, origin + quaternion * originHandle, origin + quaternion * destinationHandle, origin + quaternion * destination);
        }

        #endregion

        #region Overrides

        public override string ToString() {
            return $"Bezier[({startPoint.x:0.0#}x, {startPoint.y:0.0#}y, {startPoint.z:0.0#}z) ({startHandle.x:0.0#}x, {startHandle.y:0.0#}y, {startHandle.z:0.0#}z) -> ({endHandle.x:0.0#}x, {endHandle.y:0.0#}y, {endHandle.z:0.0#}z) ({endPoint.x:0.0#}x, {endPoint.y:0.0#}y, {endPoint.z:0.0#}z)]";
        }

        #endregion
    }
}
