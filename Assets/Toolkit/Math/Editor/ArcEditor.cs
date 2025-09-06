using UnityEngine;
using UnityEditor;

namespace Toolkit.Mathematics
{
    public static class ArcEditor
    {
        #region GUI Styles

        private static GUIStyle leftAlignedLabelStyle;
        private static GUIStyle centerAlignedLabelStyle;
        private static GUIStyle rightAlignedLabelStyle;

        private static void ValidateGUIStyle() {
            if(leftAlignedLabelStyle == null) {
                leftAlignedLabelStyle = new GUIStyle(EditorStyles.label);
                leftAlignedLabelStyle.richText = true;
            }
            if(centerAlignedLabelStyle == null) {
                centerAlignedLabelStyle = new GUIStyle(EditorStyles.label);
                centerAlignedLabelStyle.alignment = TextAnchor.UpperCenter;
                centerAlignedLabelStyle.richText = true;
            }
            if(rightAlignedLabelStyle == null) {
                rightAlignedLabelStyle = new GUIStyle(EditorStyles.label);
                rightAlignedLabelStyle.alignment = TextAnchor.UpperRight;
                rightAlignedLabelStyle.richText = true;
            }
        }

        #endregion

        #region Draw Preview

        public static void Draw2DPreview(Rect area, Arc arc) {
            var t = arc.TimeAtPeakHeight();
            if(t > 0f && t != Mathf.Infinity)
                Draw2DPreview(area, arc, Mathf.Abs(t * 2f));
            else
                Draw2DPreview(area, arc, 1f);
        }

        /// <summary>
        /// This will draw how the arc looks like from a 2D view.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="arc"></param>
        public static void Draw2DPreview(Rect area, Arc arc, float time) {
            if(area.height < EditorGUIUtility.singleLineHeight * 2f || area.width < 80f)
                return;

            EditorGUI.DrawRect(area, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            GUI.BeginClip(area);

            ValidateGUIStyle();
            var pos = arc.Position;

            var ground = area.height - 24f;
            var halfTime = time / 2f;
            Vector3 mid = arc.Evaluate(halfTime);
            Vector3 end = arc.Evaluate(time);

            GUI.Label(new Rect(area.x + 10f, area.height - 20f, area.width / 2f, 18f),
                $"0.0s (<color=red>{pos.x:0.00}</color>, <color=lime>{pos.y:0.00}</color>, <color=blue>{pos.z:0.00}</color>)", leftAlignedLabelStyle);
            GUI.Label(new Rect(area.width / 2f - 10, area.height - 20f, area.width / 2f, 18f),
                $"{time:0.##}s (<color=red>{end.x:0.00}</color>, <color=lime>{end.y:0.00}</color>, <color=blue>{end.z:0.00}</color>)", rightAlignedLabelStyle);
            GUI.Label(new Rect(0, 4f, area.width, 18f),
                $"{halfTime:0.##}s (<color=red>{mid.x:0.00}</color>, <color=lime>{mid.y:0.00}</color>, <color=blue>{mid.z:0.00}</color>)", centerAlignedLabelStyle);

            var totalDistance = Vector3.Distance(pos.To_XZ(), end.To_XZ());
            var drawRangeX = new MinMax(70f, area.width - 70f);
            var drawRangeY = new MinMax(ground, ground - (area.height - 50));
            var heightRange = new MinMax(pos.y, mid.y);


            GL.PushMatrix();
            try {
                GL.Clear(true, false, Color.black);
                EditorGUIDrawUtility.DefaultMaterial.SetPass(0);
                EditorGUIDrawUtility.GLDrawLine(new Vector2(0, ground), new Vector2(area.width, ground), ColorTable.Brown);
                EditorGUIDrawUtility.GLDrawLine(new Vector2(0, ground - 1), new Vector2(area.width, ground - 1), ColorTable.Green);
                
                var stepTime = time / 50f;
                var oldPos = pos;
                var oldPer = 0f;
                var t = stepTime;
                while(t < time) {
                    var newPos = arc.Evaluate(t);
                    var dist = Vector3.Distance(oldPos.To_XZ(out float oy), newPos.To_XZ(out float ny));
                    var diff = dist / totalDistance;
                    var y0 = heightRange.InverseEvaluate(oy);
                    var y1 = heightRange.InverseEvaluate(ny);

                    if(y0 <= 1f && y1 <= 1f)
                        EditorGUIDrawUtility.GLDrawLine(
                            new Vector2(drawRangeX.Evaluate(oldPer), drawRangeY.Evaluate(y0)),
                            new Vector2(drawRangeX.Evaluate(oldPer + diff), drawRangeY.Evaluate(y1)),
                            Color.white);

                    oldPos = newPos;
                    oldPer += diff;
                    t += stepTime;
                }
            }
            finally {
                GL.PopMatrix();
                GUI.EndClip();
            }
        }

        #endregion
    }
}
