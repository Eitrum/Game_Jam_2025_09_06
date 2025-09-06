using System;
using UnityEditor;

namespace Toolkit
{
    public static class NSOEditorSettings
    {
        #region Variables

        private const string NSO_MODE = "Toolkit.NSO_MODE";
        private const string NSO_COLOR = "Toolkit.NSO_COLOR";

        private const NSOMode NSO_MODE_DEFAULT = NSOMode.NestedRendering;
        private const NSOColor NSO_COLOR_DEFAULT = NSOColor.Default;

        private static NSOMode mode = NSOMode.NestedRendering;
        private static NSOColor color = NSOColor.Default;

        #endregion

        #region Properties

        public static NSOMode Mode {
            get => mode;
            set {
                if(mode != value) {
                    mode = value;
                    EditorPrefs.SetInt(NSO_MODE, (int)value);
                }
            }
        }

        public static NSOColor ColorMode {
            get => color;
            set {
                if(color != value) {
                    color = value;
                    EditorPrefs.SetInt(NSO_COLOR, (int)value);
                }
            }
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Initialize() {
            mode = (NSOMode)EditorPrefs.GetInt(NSO_MODE, (int)NSO_MODE_DEFAULT);
            color = (NSOColor)EditorPrefs.GetInt(NSO_COLOR, (int)NSO_COLOR_DEFAULT);
        }

        #endregion
    }
}
