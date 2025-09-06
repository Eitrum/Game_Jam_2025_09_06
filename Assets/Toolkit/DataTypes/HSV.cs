using System;
using Toolkit.Mathematics;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct HSV
    {
        #region Variables

        public float hue;
        public float saturation;
        public float value; // brightness
        public float alpha;

        #endregion

        #region Properties

        public Color Color {
            get {
                float chroma = value * saturation;
                float part = (hue * 6f);
                float other = chroma * (1f - Mathf.Abs((part % 2) - 1f));
                float min = value - chroma;
                if(part < 0f)
                    return default;
                if(part < 1f)
                    return new Color(chroma + min, other + min, min, alpha);
                if(part < 2f)
                    return new Color(other + min, chroma + min, min, alpha);
                if(part < 3f)
                    return new Color(min, chroma + min, other + min, alpha);
                if(part < 4f)
                    return new Color(min, other + min, chroma + min, alpha);
                if(part < 5f)
                    return new Color(other + min, min, chroma + min, alpha);
                if(part < 6f)
                    return new Color(chroma + min, min, other + min, alpha);

                return default;
            }
            set {
                float max = value.r > value.g ? (value.r > value.b ? value.r : value.b) : (value.g > value.b ? value.g : value.b);
                float min = value.r < value.g ? (value.r < value.b ? value.r : value.b) : (value.g < value.b ? value.g : value.b);

                float d = max - min;

                if(d > 0) {
                    if(max == value.r)
                        this.hue = 60f * (((value.g - value.b) / d) % 6f);
                    else if(max == value.g)
                        this.hue = 60f * (((value.b - value.r) / d) + 2f);
                    else
                        this.hue = 60f * (((value.r - value.g) / d) + 4f);

                    if(hue < 0f)
                        this.hue = (1f + this.hue * MathUtility.Deg2Linear) % 1f;
                    else
                        this.hue = this.hue * MathUtility.Deg2Linear;

                    this.saturation = max > 0 ? d / max : 0;
                    this.value = max;
                }
                else {
                    this.hue = 0;
                    this.saturation = 0;
                    this.value = max;
                }
                this.alpha = value.a;
            }
        }

        public bool IsHDR => value > 1f;

        #endregion

        #region Constructor

        public HSV(float hue, float saturation, float value) {
            this.hue = hue;
            this.saturation = saturation;
            this.value = value;
            this.alpha = 1f;
        }

        public HSV(float hue, float saturation, float value, float alpha) {
            this.hue = hue;
            this.saturation = saturation;
            this.value = value;
            this.alpha = alpha;
        }

        public HSV(Color value) {
            float max = value.r > value.g ? (value.r > value.b ? value.r : value.b) : (value.g > value.b ? value.g : value.b);
            float min = value.r < value.g ? (value.r < value.b ? value.r : value.b) : (value.g < value.b ? value.g : value.b);

            float d = max - min;

            if(d > 0) {
                if(max == value.r)
                    this.hue = 60f * (((value.g - value.b) / d) % 6f);
                else if(max == value.g)
                    this.hue = 60f * (((value.b - value.r) / d) + 2f);
                else
                    this.hue = 60f * (((value.r - value.g) / d) + 4f);

                if(hue < 0f)
                    this.hue = (1f + this.hue * MathUtility.Deg2Linear) % 1f;
                else
                    this.hue = this.hue * MathUtility.Deg2Linear;

                this.saturation = max > 0 ? d / max : 0;
                this.value = max;
            }
            else {
                this.hue = 0;
                this.saturation = 0;
                this.value = max;
            }
            alpha = value.a;
        }

        #endregion

        #region Lerp

        public static HSV Lerp(HSV lhs, HSV rhs, float t) {
            return new HSV(
                Mathf.Lerp(lhs.hue, rhs.hue, t),
                Mathf.Lerp(lhs.saturation, rhs.saturation, t),
                Mathf.Lerp(lhs.value, rhs.value, t),
                Mathf.Lerp(lhs.alpha, rhs.alpha, t));
        }

        public static HSV Lerp(HSV lhs, HSV rhs, float t, bool clampShortestPath) {
            var h0 = lhs.hue;
            var h1 = rhs.hue;

            float h = 0f;

            if(clampShortestPath) {
                h0 = (h0 - ((int)h0 - 1)) % 1f;
                h1 = (h1 - ((int)h1 - 1)) % 1f;

                var l = Mathf.Abs(h1 - h0);
                if(l > 0.5f) {
                    if(h0 < h1)
                        h0 += 1f;
                    else
                        h1 += 1f;
                }
                h = Mathf.Lerp(h0, h1, t) % 1f;
            }
            else
                h = Mathf.Lerp(h0, h1, t);

            return new HSV(
                h,
                Mathf.Lerp(lhs.saturation, rhs.saturation, t),
                Mathf.Lerp(lhs.value, rhs.value, t),
                Mathf.Lerp(lhs.alpha, rhs.alpha, t));
        }

        #endregion

        #region Conversion

        public static implicit operator Color(HSV hsv) => hsv.Color;
        public static implicit operator HSV(Color color) => new HSV(color);

        #endregion
    }
}
