using System;
using UnityEngine;

namespace Toolkit {
    public static class RectTransformExtensions {

        private static readonly Vector2 zero = new Vector2(0.0f, 0.0f);
        private static readonly Vector2 half = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 one = new Vector2(1.0f, 1.0f);

        #region GetSize

        public static Rect GetRect(this RectTransform rt, bool inParent) {
            if(!inParent)
                return rt.rect;
            using(new PivotAnchorScope(rt, true)) {
                var pos = rt.anchoredPosition;
                var size = rt.sizeDelta;
                pos -= rt.sizeDelta * 0.5f;
                return new Rect(pos, size);
            }
        }

        public static Vector2 GetSize(this RectTransform rt)
            => rt.rect.size;

        #endregion

        #region SetFromRect

        public static void SetFromRect(this RectTransform rt, Rect rect) {
            using(new PivotAnchorScope(rt, true)) {
                rt.anchoredPosition = rect.center;
                rt.sizeDelta = rect.size;
            }
        }

        #endregion

        #region Scope

        public class PivotAnchorScope : IDisposable {

            #region Variables

            public readonly Vector2 AnchorMin;
            public readonly Vector2 AnchorMax;
            public readonly Vector2 Pivot;
            public readonly RectTransform RectTransform;

            #endregion

            #region Constructor

            public PivotAnchorScope(RectTransform rt) {
                this.RectTransform = rt;
                this.AnchorMin = rt.anchorMin;
                this.AnchorMax = rt.anchorMax;
                this.Pivot = rt.pivot;
            }

            public PivotAnchorScope(RectTransform rt, bool center) : this(rt) {
                if(center)
                    Center();
            }

            #endregion

            #region Modify

            public void Center() {
                RectTransform.pivot = half;
                RectTransform.anchorMin = half;
                RectTransform.anchorMax = half;
            }

            public void Expand() {
                RectTransform.pivot = half;
                RectTransform.anchorMin = zero;
                RectTransform.anchorMax = one;
            }

            #endregion

            #region Dispose

            public void Dispose() {
                if(!RectTransform)
                    return;

                RectTransform.pivot = this.Pivot;
                RectTransform.anchorMin = this.AnchorMin;
                RectTransform.anchorMax = this.AnchorMax;
            }

            #endregion
        }
        #endregion
    }
}
