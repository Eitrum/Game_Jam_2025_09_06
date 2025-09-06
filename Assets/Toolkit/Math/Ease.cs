using System;
using UnityEngine;

namespace Toolkit.Mathematics
{
    public static class Ease
    {
        #region Enums

        public enum Function
        {
            Linear,
            Quad,
            Cubic,
            Quartic,
            Quintic,
            Sin,
            Exponential,
            Circular,
            Bounce,
            Elastic,
            Back
        }

        public enum Type
        {
            In,
            Out,
            InOut
        }

        #endregion

        #region Helper Methods

        public delegate float EaseFunction(float t);

        public static AnimationCurve GetAnimationCurve(Function function, Type type, int steps = 20, bool invert = false)
            => GetAnimationCurve(GetEaseFunction(function, type), steps, invert);

        public static AnimationCurve GetAnimationCurve(EaseFunction easeFunction, int steps = 20, bool invert = false) {
            AnimationCurve curve = new AnimationCurve();
            SetAnimationCurve(curve, easeFunction, steps, invert);
            return curve;
        }

        public static void SetAnimationCurve(AnimationCurve curve, Function function, Type type, int steps = 20, bool invert = false)
            => SetAnimationCurve(curve, GetEaseFunction(function, type), steps, invert);

        public static void SetAnimationCurve(AnimationCurve curve, EaseFunction easeFunction, int steps = 20, bool invert = false) {
            curve.Clear();
            steps = Mathf.Clamp(steps, 4, 100);
            float timePerStep = 1f / (float)steps;
            float timeBetweenSteps = timePerStep / 2f;
            for(int i = 0; i <= steps; i++) {
                float time = timePerStep * (float)i;
                var point = easeFunction(time);
                if(invert) {
                    var before = easeFunction(time + timeBetweenSteps);
                    var after = easeFunction(time + -timeBetweenSteps);
                    curve.AddKey(new Keyframe(1f - time, point, (point - before) / timeBetweenSteps, (after - point) / timeBetweenSteps, 0.5f, 0.5f));
                }
                else {
                    var before = easeFunction(time + -timeBetweenSteps);
                    var after = easeFunction(time + timeBetweenSteps);
                    curve.AddKey(new Keyframe(time, point, (point - before) / timeBetweenSteps, (after - point) / timeBetweenSteps, 0.5f, 0.5f));
                }
            }
        }

        public static EaseFunction GetEaseFunction(Function functionType, Type easeType) {
            switch(functionType) {
                case Function.Linear: return Linear;
                case Function.Quad: return Quad.Get(easeType);
                case Function.Cubic: return Cubic.Get(easeType);
                case Function.Quartic: return Quartic.Get(easeType);
                case Function.Quintic: return Quintic.Get(easeType);
                case Function.Sin: return Sin.Get(easeType);
                case Function.Exponential: return Exponential.Get(easeType);
                case Function.Circular: return Circular.Get(easeType);
                case Function.Bounce: return Bounce.Get(easeType);
                case Function.Elastic: return Elastic.Get(easeType);
                case Function.Back: return Back.Get(easeType);
            }
            return f => f;
        }

        public static Func<float, float> GetEaseFunctionAsFunc(Function functionType, Type easeType) {
            switch(functionType) {
                case Function.Linear: return Linear;
                case Function.Quad: return Quad.GetAsFunc(easeType);
                case Function.Cubic: return Cubic.GetAsFunc(easeType);
                case Function.Quartic: return Quartic.GetAsFunc(easeType);
                case Function.Quintic: return Quintic.GetAsFunc(easeType);
                case Function.Sin: return Sin.GetAsFunc(easeType);
                case Function.Exponential: return Exponential.GetAsFunc(easeType);
                case Function.Circular: return Circular.GetAsFunc(easeType);
                case Function.Bounce: return Bounce.GetAsFunc(easeType);
                case Function.Elastic: return Elastic.GetAsFunc(easeType);
                case Function.Back: return Back.GetAsFunc(easeType);
            }
            return f => f;
        }

        #endregion

        #region Linear

        /// <summary>
        /// Linear ease the specified time.
        /// DOES NOTHING, REALLY!!! WHY YOU USING IT???
        /// </summary>
        /// <param name="time">Time.</param>
        public static float Linear(float time) {
            return time;
        }

        #endregion

        #region Quad

        public static class Quad
        {
            public static float In(float time) {
                return time * time;
            }

            public static float Out(float time) {
                return -time * (time - 2f);
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * time * time;
                time -= 1f;
                return -0.5f * (time * (time - 2f) - 1f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Cubic

        public class Cubic
        {
            public static float In(float time) {
                return time * time * time;
            }

            public static float Out(float time) {
                time--;
                return (time * time * time + 1f);
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * time * time * time;
                time -= 2f;
                return 0.5f * (time * time * time + 2f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Quartic

        public class Quartic
        {
            public static float In(float time) {
                return time * time * time * time;
            }

            public static float Out(float time) {
                time--;
                return -(time * time * time * time - 1f);
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * time * time * time * time;
                time -= 2f;
                return -0.5f * (time * time * time * time - 2f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Quintic

        public class Quintic
        {
            public static float In(float time) {
                return time * time * time * time * time;
            }

            public static float Out(float time) {
                time--;
                return (time * time * time * time * time + 1f);
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * time * time * time * time * time;
                time -= 2f;
                return 0.5f * (time * time * time * time * time + 2f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Sin

        public class Sin
        {
            public static float In(float time) {
                return -Mathf.Cos((time) * (Mathf.PI / 2)) + 1f;
            }

            public static float Out(float time) {
                return Mathf.Sin((time) * (Mathf.PI / 2f));
            }

            public static float InOut(float time) {
                return -Mathf.Cos(time * Mathf.PI) / 2f + 0.5f;
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Exponential

        public class Exponential
        {
            public static float In(float time) {
                return Mathf.Pow(2f, 10f * (time - 1f));
            }

            public static float Out(float time) {
                return (-Mathf.Pow(2f, -10f * time) + 1f);
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * Mathf.Pow(2f, 10f * (time - 1f));
                time--;
                return 0.5f * (-Mathf.Pow(2f, -10f * time) + 2f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Circular

        public class Circular
        {
            public static float In(float time) {
                return 1f - Mathf.Sqrt(1f - time * time);
            }

            public static float Out(float time) {
                time--;
                return Mathf.Sqrt(1f - time * time);
            }

            public static float InOut(float time) {
                time *= 2f;
                if(time <= 1f)
                    return (1f - Mathf.Sqrt(1f - time * time)) * 0.5f;
                time -= 2f;
                return (Mathf.Sqrt(1f - time * time) + 1f) * 0.5f;
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In: return In;
                    case Type.Out: return Out;
                    case Type.InOut: return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Bounce

        public class Bounce
        {
            public static float In(float time) {
                time = (1f - time);
                if(time < (1f / 2.75f)) {
                    return 1f - (7.5625f * time * time);
                }
                if(time < (2f / 2.75f)) {
                    time -= 1.5f / 2.75f;
                    return 1f - (7.5625f * time * time + 0.75f);
                }
                if(time < (2.5f / 2.75f)) {
                    time -= 2.25f / 2.75f;
                    return 1f - (7.5625f * time * time + 0.9375f);
                }
                time -= 2.625f / 2.75f;
                return 1f - (7.5625f * time * time + 0.984375f);
            }

            public static float Out(float time) {
                if(time < (1f / 2.75f)) {
                    return 7.5625f * time * time;
                }
                if(time < (2f / 2.75f)) {
                    time -= 1.5f / 2.75f;
                    return 7.5625f * time * time + 0.75f;
                }
                if(time < (2.5f / 2.75f)) {
                    time -= 2.25f / 2.75f;
                    return 7.5625f * time * time + 0.9375f;
                }
                time -= 2.625f / 2.75f;
                return 7.5625f * time * time + 0.984375f;
            }

            public static float InOut(float time) {
                if(time < 0.5f)
                    return In(2f * time) * 0.5f;
                return 0.5f * Out(2f * time - 1f) + 0.5f;
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Elastic

        public class Elastic
        {
            public static float In(float time) {
                if(time <= 0f)
                    return 0f;
                if(time >= 1f)
                    return 1f;
                time -= 1f;
                return -Mathf.Pow(2f, 10f * time) * Mathf.Sin((time - 0.1f) * (2f * Mathf.PI) / 0.4f);
            }

            public static float Out(float time) {
                if(time <= 0f)
                    return 0f;
                if(time >= 1f)
                    return 1f;
                return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f) {
                    time -= 1f;
                    return -0.5f * Mathf.Pow(2f, 10f * time) * Mathf.Sin((time - 0.1f) * (2f * Mathf.PI) / 0.4f);
                }
                time -= 1f;
                return Mathf.Pow(2f, -10f * time) * Mathf.Sin((time - 0.1f) * (2f * Mathf.PI) / 0.4f) * 0.5f + 1f;
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }
        }

        #endregion

        #region Back

        public class Back
        {
            const float s1 = 1.70158f;
            const float s2 = 2.5949095f;

            public static float In(float time) {
                return time * time * ((s1 + 1f) * time - s1);
            }

            public static float Out(float time) {
                time -= 1f;
                return time * time * ((s1 + 1f) * time + s1) + 1f;
            }

            public static float InOut(float time) {
                time /= 0.5f;
                if(time < 1f)
                    return 0.5f * (time * time * ((s2 + 1f) * time - s2));
                time -= 2f;
                return 0.5f * (time * time * ((s2 + 1f) * time + s2) + 2f);
            }

            public static EaseFunction Get(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }

            public static Func<float, float> GetAsFunc(Type easeType) {
                switch(easeType) {
                    case Type.In:
                        return In;
                    case Type.Out:
                        return Out;
                    case Type.InOut:
                        return InOut;
                }
                return f => f;
            }
        }

        #endregion
    }
}
