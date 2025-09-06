using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    public static class EditorGUIDrawUtility
    {
        #region Materials

        private static Material defaultMaterial;
        public static Material DefaultMaterial {
            get {
                if(defaultMaterial == null) {
                    defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored")); // <- If this breaks, it breaks all the drawing anyway, so no need for checks.
                }
                return defaultMaterial;
            }
        }

        #endregion

        #region GL Draw Line

        public static void GLDrawLine(Vector2 start, Vector2 end, Color color) {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(start.x, start.y, 0);
            GL.Vertex3(end.x, end.y, 0);
            GL.End();
        }

        public static void GLDrawLine(Vector2 start, Vector2 end, Color color, float width) {
            var dir = (Quaternion.Euler(0, 0, 90f) * (end - start).normalized) * width;
            GL.Begin(GL.QUADS);
            GL.Color(color);
            GL.Vertex3(start.x + dir.x, start.y + dir.y, 0);
            GL.Vertex3(start.x - dir.x, start.y - dir.y, 0);
            GL.Vertex3(end.x - dir.x, end.y - dir.y, 0);
            GL.Vertex3(end.x + dir.x, end.y + dir.y, 0);
            GL.End();
        }

        #endregion
    }
}
