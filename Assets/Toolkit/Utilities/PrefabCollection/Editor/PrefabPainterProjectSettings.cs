using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toolkit.Utility
{
    public static class PrefabPainterProjectSettings
    {
        #region Variables

        private const string KEY_PATH = "SCENE_PAINTER_KEY";
        private const string KEY_CTRL_PATH = "SCENE_PAINTER_CTRL";
        private const string KEY_SHIFT_PATH = "SCENE_PAINTER_SHIFT";
        private const string KEY_ALT_PATH = "SCENE_PAINTER_ALT";

        private const string DRAW_MODEL_PATH = "SCENE_PAINTER_MODEL";
        private const string DRAW_WIREFRAME_PATH = "SCENE_PAINTER_WIREFRAME";
        private const string WIREFRAME_COLOR_PATH = "SCENE_PAINTER_WIREFRAME_COLOR";
        private const string DRAW_NORMAL_PATH = "SCENE_PAINTER_NORMAL";
        private const string NORMAL_COLOR_PATH = "SCENE_PAINTER_NORMAL_COLOR";

        private const string Y_ROT_LOCK = "SCENE_PAINTER_Y_ROT_LOCK";
        private const string VARIANT_LOCK = "SCENE_PAINTER_VARIANT_LOCK";

        private const string GRID_SNAP = "SCENE_PAINTER_GRID_SNAP";
        private const string GRID_SIZE = "SCENE_PAINTER_GRID_SIZE";
        private const string ROTATION_SNAP = "SCENE_PAINTER_ROTATION_SNAP";
        private const string ROTATION_SNAP_SIZE = "SCENE_PAINTER_ROTATION_SNAP_SIZE";
        private const string MAJOR_ROTATION = "SCENE_PAINTER_MAJOR_ROTATION";
        private const string MINOR_ROTATION = "SCENE_PAINTER_MINOR_ROTATION";

        private static KeyCode keyCode;
        private static bool needCtrl;
        private static bool needShift;
        private static bool needAlt;

        private static bool drawModel = true;
        private static bool drawWireframe = true;
        private static Color32 wireframeColor = ColorTable.DeepSkyBlue;
        private static bool drawNormalPlane = false;
        private static Color32 normalColor = ColorTable.LawnGreen;
        private static Material wireframeMaterial;
        private static Mesh normalPlaneMesh;
        private static Material normalPlaneMaterial;

        private static bool yRotLock = false;
        private static bool variantLock = false;

        private static bool enableGridSnap = false;
        private static float gridSize = 1f;
        private static bool enableRotationSnap = false;
        private static float rotationSnap = 15f;

        private static float majorRotationAmount = 15f;
        private static float minorRotationAmount = 1f;

        #endregion

        #region Properties

        public static KeyCode KeyCode {
            get => keyCode;
            set {
                if(value != keyCode) {
                    keyCode = value;
                    EditorPrefs.SetInt(KEY_PATH, value.ToInt());
                }
            }
        }

        public static bool UseControl {
            get => needCtrl;
            set {
                if(value != needCtrl) {
                    needCtrl = value;
                    EditorPrefs.SetBool(KEY_CTRL_PATH, needCtrl);
                }
            }
        }

        public static bool UseShift {
            get => needShift;
            set {
                if(value != needShift) {
                    needShift = value;
                    EditorPrefs.SetBool(KEY_SHIFT_PATH, needShift);
                }
            }
        }

        public static bool UseAlt {
            get => needAlt;
            set {
                if(value != needAlt) {
                    needAlt = value;
                    EditorPrefs.SetBool(KEY_ALT_PATH, needAlt);
                }
            }
        }

        public static string GetKeyFormatted {
            get {
                string prefix = "";
                if(needCtrl)
                    prefix += "Ctrl+";
                if(needAlt)
                    prefix += "Alt+";
                if(needShift)
                    prefix += "Shift+";
                return prefix + keyCode.ToString();
            }
        }

        public static bool DrawModel {
            get => drawModel;
            set {
                if(drawModel != value) {
                    drawModel = value;
                    EditorPrefs.SetBool(DRAW_MODEL_PATH, value);
                }
            }
        }

        public static bool DrawWireframe {
            get => drawWireframe;
            set {
                if(drawWireframe != value) {
                    drawWireframe = value;
                    EditorPrefs.SetBool(DRAW_WIREFRAME_PATH, value);
                }
            }
        }

        public static Color32 WireframeColor {
            get => wireframeColor;
            set {
                if(!wireframeColor.Equals(value)) {
                    wireframeColor = value;
                    EditorPrefs.SetInt(WIREFRAME_COLOR_PATH, value.ToInt());
                    WireframeMaterial.SetColor("_Color", wireframeColor);
                }
            }
        }
        public static Material WireframeMaterial {
            get {
                if(!wireframeMaterial) {
                    wireframeMaterial = new Material(Shader.Find("Toolkit/Wireframe/Transparent"));
                    wireframeMaterial.SetColor("_Color", wireframeColor);
                }
                return wireframeMaterial;
            }
        }

        public static bool DrawNormal {
            get => drawNormalPlane;
            set {
                if(drawNormalPlane != value) {
                    drawNormalPlane = value;
                    EditorPrefs.SetBool(DRAW_NORMAL_PATH, value);
                }
            }
        }

        public static Material NormalMaterial {
            get {
                if(!normalPlaneMaterial) {
                    normalPlaneMaterial = new Material(Shader.Find("Transparent/VertexLit"));
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/normalPlane.png");
                    Debug.Log("Found Tex!: " + (tex != null));
                    if(tex)
                        normalPlaneMaterial.SetTexture("_MainTex", tex);
                    normalPlaneMaterial.color = NormalColor;
                }
                return normalPlaneMaterial;
            }
        }

        public static Color32 NormalColor {
            get => normalColor;
            set {
                if(!normalColor.Equals(value)) {
                    normalColor = value;
                    EditorPrefs.SetInt(NORMAL_COLOR_PATH, normalColor.ToInt());
                    NormalMaterial.color = value;

                }
            }
        }

        public static bool LockYRotation {
            get => yRotLock;
            set {
                if(yRotLock != value) {
                    yRotLock = value;
                    EditorPrefs.SetBool(Y_ROT_LOCK, value);
                }
            }
        }

        public static bool VariantLock {
            get => variantLock;
            set {
                if(variantLock != value) {
                    variantLock = value;
                    EditorPrefs.SetBool(VARIANT_LOCK, value);
                }
            }
        }

        public static bool EnableSnap {
            get => enableGridSnap;
            set {
                if(enableGridSnap != value) {
                    enableGridSnap = value;
                    EditorPrefs.SetBool(GRID_SNAP, value);
                }
            }
        }

        public static float GridSize {
            get => gridSize;
            set {
                if(gridSize != value) {
                    gridSize = value;
                    EditorPrefs.SetFloat(GRID_SIZE, value);
                }
            }
        }

        public static bool EnableRotationSnap {
            get => enableRotationSnap;
            set {
                if(enableRotationSnap != value) {
                    enableRotationSnap = value;
                    EditorPrefs.SetBool(ROTATION_SNAP, value);
                }
            }
        }

        public static float RotationSnapSize {
            get => rotationSnap;
            set {
                if(rotationSnap != value) {
                    rotationSnap = value;
                    EditorPrefs.SetFloat(ROTATION_SNAP_SIZE, value);
                }
            }
        }

        public static float MinorRotation {
            get => minorRotationAmount;
            set {
                if(minorRotationAmount != value) {
                    minorRotationAmount = value;
                    EditorPrefs.SetFloat(MINOR_ROTATION, value);
                }
            }
        }

        public static float MajorRotation {
            get => majorRotationAmount;
            set {
                if(majorRotationAmount != value) {
                    majorRotationAmount = value;
                    EditorPrefs.SetFloat(MAJOR_ROTATION, value);
                }
            }
        }

        #endregion

        #region Input

        public static bool OpenTool(Event ev) {
            return ev.type == EventType.KeyDown &&
                ev.keyCode == keyCode &&
                ev.shift == UseShift &&
                ev.control == UseControl &&
                ev.alt == UseAlt;
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Initialize() {
            using(new FastEnum.DisableLoggingScope())
                keyCode = EditorPrefs.GetInt(KEY_PATH, (int)KeyCode.U).ToEnum<KeyCode>();
            needCtrl = EditorPrefs.GetBool(KEY_CTRL_PATH, false);
            needShift = EditorPrefs.GetBool(KEY_SHIFT_PATH, false);
            needAlt = EditorPrefs.GetBool(KEY_ALT_PATH, false);

            drawModel = EditorPrefs.GetBool(DRAW_MODEL_PATH, true);
            drawWireframe = EditorPrefs.GetBool(DRAW_WIREFRAME_PATH, true);
            wireframeColor = EditorPrefs.GetInt(WIREFRAME_COLOR_PATH, ColorTable.DeepSkyBlue.ToInt()).ToColor32();
            drawNormalPlane = EditorPrefs.GetBool(DRAW_NORMAL_PATH, false);
            normalColor = EditorPrefs.GetInt(NORMAL_COLOR_PATH, ColorTable.LawnGreen.ToInt()).ToColor32();

            yRotLock = EditorPrefs.GetBool(Y_ROT_LOCK, false);
            variantLock = EditorPrefs.GetBool(VARIANT_LOCK, false);

            enableGridSnap = EditorPrefs.GetBool(GRID_SNAP, false);
            gridSize = EditorPrefs.GetFloat(GRID_SIZE, gridSize);
            enableRotationSnap = EditorPrefs.GetBool(ROTATION_SNAP, false);
            rotationSnap = EditorPrefs.GetFloat(ROTATION_SNAP_SIZE, rotationSnap);
            minorRotationAmount = EditorPrefs.GetFloat(MINOR_ROTATION, 1f);
            majorRotationAmount = EditorPrefs.GetFloat(MAJOR_ROTATION, 15f);

            EditorApplication.playModeStateChanged += (state) => {
                if(state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode) {
                    if(wireframeMaterial)
                        Material.DestroyImmediate(wireframeMaterial);
                    if(normalPlaneMaterial)
                        Material.DestroyImmediate(normalPlaneMaterial);
                }
            };

        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() {
            return new SettingsProvider("Project/Toolkit/Editor/Prefab Painter", SettingsScope.Project) {
                guiHandler = OnGUI
            };
        }

        #endregion

        #region Drawing

        private static void OnGUI(string obj) {
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Tool Select", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Input", GetKeyFormatted, EditorStyles.boldLabel);
                KeyCode = (KeyCode)EditorGUILayout.EnumPopup("Key", keyCode);
                UseControl = EditorGUILayout.Toggle("Ctrl", needCtrl);
                UseShift = EditorGUILayout.Toggle("Shift", needShift);
                UseAlt = EditorGUILayout.Toggle("Alt", needAlt);
            }

            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Draw", EditorStylesUtility.BoldLabel);
                DrawModel = EditorGUILayout.Toggle("Model", drawModel);
                DrawWireframe = EditorGUILayout.Toggle("Wireframe", drawWireframe);
                if(drawWireframe)
                    using(new EditorGUI.IndentLevelScope(1))
                        WireframeColor = EditorGUILayout.ColorField("Color", wireframeColor);
                DrawNormal = EditorGUILayout.Toggle("Normal", drawNormalPlane);
                if(drawNormalPlane)
                    using(new EditorGUI.IndentLevelScope(1))
                        NormalColor = EditorGUILayout.ColorField("Color", normalColor);
            }


            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Config", EditorStylesUtility.BoldLabel);
                LockYRotation = EditorGUILayout.Toggle("Lock Y Rotation", LockYRotation);
                VariantLock = EditorGUILayout.Toggle("Lock Variant", VariantLock);

                EditorGUILayout.LabelField("Snap", EditorStylesUtility.BoldLabel);
                EnableSnap = EditorGUILayout.Toggle("Grid Snap", enableGridSnap);
                GridSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("Grid Size", gridSize));
                EnableRotationSnap = EditorGUILayout.Toggle("Rotation Snap", EnableRotationSnap);
                RotationSnapSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("Rotation Snap Value", RotationSnapSize));

                EditorGUILayout.LabelField("Rotation", EditorStylesUtility.BoldLabel);
                MajorRotation = Mathf.Max(0.01f, EditorGUILayout.FloatField("Normal", MajorRotation));
                MinorRotation = Mathf.Max(0.01f, EditorGUILayout.FloatField("Shift", MinorRotation));
            }
        }

        public static void DrawPlane(Vector3 position, Vector3 normal, Camera camera)
            => DrawPlane(position, normal, camera, 1f);

        public static void DrawPlane(Vector3 position, Vector3 normal, Camera camera, float scale) {
            if(!normalPlaneMesh)
                normalPlaneMesh = PrimitiveUtility.GetMesh(PrimitiveType.Plane);

            var rot = Quaternion.FromToRotation(Vector3.up, normal);
            Graphics.DrawMesh(normalPlaneMesh, Matrix4x4.TRS(position + normal.normalized * 0.02f, rot, scale.To_Vector3()), NormalMaterial, 0, camera);
        }

        #endregion
    }
}
