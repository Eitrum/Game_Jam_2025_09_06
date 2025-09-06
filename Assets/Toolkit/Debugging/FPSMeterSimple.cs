using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Debugging {
    public class FPSMeterSimple : MonoBehaviour {

        public enum Corner {
            Topleft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        [SerializeField] private bool disableFPSCap = false;
        [SerializeField] private Corner corner = Corner.Topleft;
        [SerializeField, Min(0.2f)] private float updateFrequency = 1f;

        private GUIStyle style;
        private int frameCount;
        private float t = 0f;
        private int fps;

        public Corner Alignment {
            get => corner;
            set => corner = value;
        }

        private void Awake() {
            if(disableFPSCap) {
                Application.targetFrameRate = -1;
                QualitySettings.vSyncCount = 0;
            }
        }

        private void Update() {
            frameCount++;
            t += Time.deltaTime;
            if(t > updateFrequency) {
                t -= updateFrequency;
                fps = Mathf.RoundToInt(frameCount / updateFrequency);
                frameCount = 0;
            }
        }

        private static TextAnchor ToAlignment(Corner corner) {
            switch(corner) {
                case Corner.Topleft: return TextAnchor.UpperLeft;
                case Corner.TopRight: return TextAnchor.UpperRight;
                case Corner.BottomLeft: return TextAnchor.LowerLeft;
                case Corner.BottomRight: return TextAnchor.LowerRight;
            }
            return TextAnchor.UpperLeft;
        }

        private void OnGUI() {
            var rect = new Rect(20, 20, Screen.width - 40, Screen.height - 40);
            if(style == null) {
                style = new GUIStyle(GUI.skin.label);
                style.alignment = ToAlignment(corner);
            }
            style.alignment = ToAlignment(corner);
            GUI.Label(rect, $"FPS [ {fps:0000} ]", style);
        }
    }
}
