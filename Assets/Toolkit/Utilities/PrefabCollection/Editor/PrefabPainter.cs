using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.Reflection;
using System.Linq;

namespace Toolkit.Utility {
    [EditorTool("Prefab Painter")]
    public class PrefabPainter : EditorTool {
        #region Variables

        GUIContent customToolbarIcon;
        private static PrefabCollection prefabCollection;
        private static PrefabCollection.Entry entry;
        private static PrefabCollection.Variant currentVariant;
        private PrefabPainterSelection selection;

        float xrot = 0f;
        float zrot = 0f;
        float yrot = 0f;
        Vector3 rrot = new Vector3();
        float scaleValue = 0f;
        bool isPlacing = false;
        private Vector3 origin = Vector3.zero;
        private Vector3 originNormal = Vector3.zero;
        private Transform relativeTo;

        private bool isDragPlacing = false;
        Quaternion placeTiltOffset = Quaternion.identity;

        private static Toolkit.PreviewRenderer renderer;
        private static Texture2D folderIcon;
        private static Texture2D folderBackIcon;
        private static Texture2D prefabIcon;
        private static Texture2D prefabRandomIcon;
        private static Texture2D rotationIcon;
        private static Texture2D rotationLockIcon;

        #endregion

        #region Properties

        public override GUIContent toolbarIcon => customToolbarIcon;
        public static Texture2D FolderIcon => folderIcon;
        public static Texture2D FolderBackIcon => folderBackIcon;

        #endregion

        #region Init / Enable / Disable

        [InitializeOnLoadMethod]
        private static void Init() {
            SceneView.beforeSceneGui += (sv) => {
                var ev = Event.current;
                if(PrefabPainterProjectSettings.OpenTool(ev)) {
                    ToolManager.SetActiveTool<PrefabPainter>();
                    ev.Use();
                }
            };
            folderIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/folderIcon.png");
            folderBackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/folderBackIcon.png");
            prefabIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/prefabIcon.png");
            prefabRandomIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/prefabRandomIcon.png");
            rotationIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/rotation.png");
            rotationLockIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/rotationLock.png");
        }

        private void OnEnable() {
            customToolbarIcon = new GUIContent() {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/Utilities/PrefabCollection/Editor/prefabCollectionPainter.png"),
                text = "Prefab Painter",
                tooltip = "Prefab painter allows you to quickly place prefabs in a scene from a list of prefabs inside a Prefab Collection object"
            };
            PrefabCollectionUtility.Refresh();
            if(prefabCollection == null)
                prefabCollection = PrefabCollectionUtility.GetCollections().FirstOrDefault();
            if(entry == null && prefabCollection != null)
                entry = prefabCollection.GetEntry(0);
            if(currentVariant == null && entry != null)
                currentVariant = entry.GetVariant(0);
            NextValues();
            Selection.activeObject = null;
        }

        private void OnDisable() {
            if(renderer != null) {
                renderer.Dispose();
                renderer = null;
            }
        }

        #endregion

        #region Input

        private static bool ToggleSelectionScreen(Event ev) {
            return ev.type == EventType.KeyDown && ev.control && ev.keyCode == KeyCode.Q;
        }

        private static bool ToggleYLock(Event ev) {
            return ev.type == EventType.KeyDown && ev.control && ev.keyCode == KeyCode.W;
        }

        private static bool ToggleVariantLock(Event ev) {
            return ev.type == EventType.KeyDown && ev.control && ev.keyCode == KeyCode.E;
        }

        private static bool ToggleGridSnap(Event ev) {
            return ev.type == EventType.KeyDown && ev.control && ev.keyCode == KeyCode.R;
        }

        private static bool ToggleRotationSnap(Event ev) {
            return ev.type == EventType.KeyDown && ev.control && ev.keyCode == KeyCode.T;
        }

        #endregion

        void NextValues() {
            xrot = Random.value;
            if(!PrefabPainterProjectSettings.LockYRotation)
                yrot = Random.value.Snap(1f / 360f);
            zrot = Random.value;
            scaleValue = Random.value;
            rrot = new Vector3(Random.value, Random.value, Random.value);
            if(!PrefabPainterProjectSettings.VariantLock)
                currentVariant = entry != null ? entry.GetRandomVariant() : null;
        }

        public override void OnToolGUI(EditorWindow window) {
            // Fix anti click
            if(Event.current.type == EventType.Layout) {
                HandleUtility.AddDefaultControl(0);
            }
            if(renderer == null)
                renderer = new PreviewRenderer();

            bool drawPainter = DrawHeader(window);
            drawPainter &= DrawSelection(window);
            if(drawPainter)
                DrawPainter(window);
        }

        #region Draw Header


        private bool DrawHeader(EditorWindow window) {
            using(new GUI.GroupScope(new Rect(Vector2.zero, window.position.size))) {
                var ev = Event.current;
                var totalIconArea = new Rect(10, 10, 150, 40);
                var mainIcon = new Rect(10, 10, 40, 40);
                var yLockIcon = new Rect(55, 10, 40, 40);
                var variantLockIcon = new Rect(100, 10, 40, 40);
                var snapGrid = new Rect(145, 10, 40, 40);
                var snapRot = new Rect(190, 10, 40, 40);
                EditorGUI.DrawRect(mainIcon, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                EditorGUI.DrawRect(yLockIcon, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                EditorGUI.DrawRect(variantLockIcon, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                EditorGUI.DrawRect(snapGrid, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                EditorGUI.DrawRect(snapRot, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                EditorGUI.LabelField(snapGrid, $"Snap\n{(PrefabPainterProjectSettings.EnableSnap ? "On" : "Off")}", EditorStylesUtility.CenterAlignedLabel);
                EditorGUI.LabelField(snapRot, $"Rotation\n{(PrefabPainterProjectSettings.EnableRotationSnap ? "On" : "Off")}", EditorStylesUtility.CenterAlignedLabel);

                GUI.DrawTexture(mainIcon, folderIcon);

                if(PrefabPainterProjectSettings.LockYRotation)
                    GUI.DrawTexture(yLockIcon, rotationLockIcon);
                else
                    GUI.DrawTexture(yLockIcon, rotationIcon);

                if(PrefabPainterProjectSettings.VariantLock)
                    GUI.DrawTexture(variantLockIcon, prefabIcon);
                else
                    GUI.DrawTexture(variantLockIcon, prefabRandomIcon);

                if(ToggleSelectionScreen(ev) || (ev.type == EventType.MouseDown && ev.button == 0 && mainIcon.Contains(ev.mousePosition))) {
                    if(selection == null)
                        selection = new PrefabPainterSelection(prefabCollection, entry, renderer);
                    else
                        selection = null;
                    ev.Use();
                    return false;
                }

                if(ToggleYLock(ev) || (ev.type == EventType.MouseDown && ev.button == 0 && yLockIcon.Contains(ev.mousePosition))) {
                    PrefabPainterProjectSettings.LockYRotation = !PrefabPainterProjectSettings.LockYRotation;
                    ev.Use();
                    return false;
                }

                if(ToggleVariantLock(ev) || (ev.type == EventType.MouseDown && ev.button == 0 && variantLockIcon.Contains(ev.mousePosition))) {
                    PrefabPainterProjectSettings.VariantLock = !PrefabPainterProjectSettings.VariantLock;
                    ev.Use();
                    return false;
                }

                if(ToggleGridSnap(ev) || (ev.type == EventType.MouseDown && ev.button == 0 && snapGrid.Contains(ev.mousePosition))) {
                    PrefabPainterProjectSettings.EnableSnap = !PrefabPainterProjectSettings.EnableSnap;
                    ev.Use();
                    return false;
                }

                if(ToggleRotationSnap(ev) || (ev.type == EventType.MouseDown && ev.button == 0 && snapRot.Contains(ev.mousePosition))) {
                    PrefabPainterProjectSettings.EnableRotationSnap = !PrefabPainterProjectSettings.EnableRotationSnap;
                    ev.Use();
                    return false;
                }

                if(selection == null && PrefabPainterProjectSettings.VariantLock) {
                    var variationArea = new Rect(10, 55, window.position.size.x - 20, 22);
                    variationArea.SplitHorizontal(out Rect varLabelArea, out Rect varIconArea, 90f / variationArea.width, 5f);
                    EditorGUI.DrawRect(varLabelArea, new Color(0.3f, 0.3f, 0.3f, 0.7f));
                    EditorGUI.LabelField(varLabelArea, "Variant", EditorStylesUtility.CenterAlignedBoldLabel);
                    varIconArea.width = 40;
                    if(entry != null) {
                        var count = entry.VariantCount;
                        for(int i = 0; i < count; i++) {
                            var variant = entry.GetVariant(i);
                            var isSelected = currentVariant == variant;
                            EditorGUI.DrawRect(varIconArea, isSelected ? new Color(0.5f, 0.5f, 0.5f, 0.8f) : new Color(0.3f, 0.3f, 0.3f, 0.7f));
                            EditorGUI.LabelField(varIconArea, $"{(i)}", EditorStylesUtility.CenterAlignedBoldLabel);
                            if(ev.type == EventType.MouseDown && ev.button == 0 && varIconArea.Contains(ev.mousePosition)) {
                                currentVariant = variant;
                                ev.Use();
                                return false;
                            }
                            varIconArea.x += 45;
                        }
                    }
                }

                if(totalIconArea.Contains(ev.mousePosition)) {
                    window.Repaint();
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Draw Selection

        private bool DrawSelection(EditorWindow window) {
            using(new GUI.GroupScope(new Rect(Vector2.zero, window.position.size))) {
                if(selection != null) {
                    selection.OnGUI(new Rect(Vector2.zero, window.position.size).Pad(0, 0, 50, 20).Shrink(10f));
                    if(selection.IsDone) {
                        prefabCollection = selection.Collection;
                        if(entry != selection.Entry) {
                            entry = selection.Entry;
                        }
                        currentVariant = entry.GetVariant(0);
                        NextValues();
                        selection = null;
                    }
                    window.Repaint();
                }
            }
            return selection == null;
        }

        #endregion

        #region Draw Painter

        private void DrawPainter(EditorWindow window) {
            var ev = Event.current;

            // If cursor outside of area, do not render
            var areaSize = window.position;
            areaSize.position = Vector2.zero;
            if(!areaSize.Contains(ev.mousePosition))
                return;

            var sceneView = window as SceneView;
            if(sceneView == null) {
                Debug.LogError("Tool GUI is not drawing scene view");
            }

            var position = ev.mousePosition;
            var ray = sceneView.camera.ScreenPointToRay(new Vector2(position.x, (window.position.height - 20) - position.y));
            var obj = HandleUtility.RaySnap(ray);
            if(ev.control) {
                if(obj is RaycastHit tempHit) {
                    var temp = HandleUtility.RaySnap(new Ray(ray.origin + ray.direction * (tempHit.distance + 0.05f), ray.direction));
                    if(temp != null)
                        obj = temp;
                }
            }
            RaycastHit hit = obj != null ? (RaycastHit)obj : default;

            if(ev.type == EventType.Repaint) {
                if(isPlacing || obj != null) {
                    Paint(hit, sceneView.camera);
                }
            }
            if(isDragPlacing)
                Handles.DrawSolidDisc(origin, originNormal, 4f);

            if(obj != null) {
                if(ev.type == EventType.MouseDown && ev.button == 0) {
                    isPlacing = true;
                    origin = hit.point;
                    originNormal = hit.normal;
                    placeTiltOffset = Quaternion.identity;
                    isDragPlacing = false;
                    relativeTo = hit.transform;
                    ev.Use();
                }
            }
            if(isPlacing) {
                if(ev.type == EventType.MouseDrag) {
                    var p = new Plane(originNormal, origin);
                    if(p.Raycast(ray, out float enterValue)) {
                        var dir = ray.GetPoint(enterValue) - origin;
                        if(dir.magnitude > 0.075f && !isDragPlacing) {
                            isDragPlacing = true;
                        }
                        if(isDragPlacing) {
                            placeTiltOffset = Quaternion.RotateTowards(Quaternion.identity, Quaternion.FromToRotation(originNormal, dir), dir.magnitude * 45f);// Quaternion.FromToRotation(Vector3.up, new Vector3(dir.x, 2f, dir.z));// Quaternion.Euler(Mathf.Clamp(dir.z, -2f, 2f) * 45f, 0, Mathf.Clamp(dir.x, -2f, 2f) * -45f);
                        }
                    }
                    ev.Use();
                }
                if(ev.type == EventType.MouseUp && ev.button == 0) {
                    PlaceObjectInWorld();
                    isPlacing = false;
                    placeTiltOffset = Quaternion.identity;
                    isDragPlacing = false;
                    ev.Use();
                }
                if(ev.type == EventType.MouseDown && ev.button == 1) {
                    isPlacing = false;
                    origin = hit.point;
                    originNormal = hit.normal;
                    placeTiltOffset = Quaternion.identity;
                    isDragPlacing = false;
                    ev.Use();
                }
            }

            if(ev.type == EventType.ScrollWheel) {
                if(ev.control) {
                    var index = 0;
                    var count = entry.VariantCount;
                    for(int i = 0; i < count; i++) {
                        if(entry.GetVariant(i) == currentVariant) {
                            index = i;
                        }
                    }
                    if(ev.delta.y > 0)
                        index = (index + 1) % count;
                    else
                        index = (index + count - 1) % count;
                    currentVariant = entry.GetVariant(index);
                }
                else {
                    yrot += (ev.delta.y > 0 ? 1 : -1) * (ev.shift ? 1f / 360f : 15f / 360f);
                }
                ev.Use();
            }

            window.Repaint();
        }

        public void PlaceObjectInWorld() {
            var currentScene = SceneManagement.SceneUtility.GetActiveScene();
            var go = PrefabUtility.InstantiatePrefab(currentVariant.Prefab) as GameObject;

            var norm = originNormal;
            var rot = placeTiltOffset * currentVariant.CalculateRotation(norm, xrot, zrot, yrot, rrot);
            var pos = currentVariant.CalulatePosition(origin, norm, rot);
            if(PrefabPainterProjectSettings.EnableSnap)
                pos = pos.Snap(PrefabPainterProjectSettings.GridSize, relativeTo);
            if(PrefabPainterProjectSettings.EnableRotationSnap)
                rot = rot.Snap(PrefabPainterProjectSettings.RotationSnapSize, relativeTo);

            go.transform.localPosition = pos;
            go.transform.localRotation = rot;
            go.transform.localScale = go.transform.localScale.Multiply(currentVariant.CalculateSize(scaleValue));
            var containerCollection = currentScene.GetRootGameObjects().Select(x => x.GetComponent<PrefabPainterContainerCollection>()).FirstOrDefault(x => x != null);
            if(containerCollection == null) {
                var painterGo = new GameObject("----------------[ Prefab Painter ]----------------");
                containerCollection = painterGo.AddComponent<PrefabPainterContainerCollection>();
                EditorApplication.delayCall += () => {
                    var con = containerCollection.GetContainer(entry.Name);
                    go.transform.SetParent(con.transform);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currentScene);
                };
            }
            else {
                var con = containerCollection.GetContainer(entry.Name);
                go.transform.SetParent(con.transform);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currentScene);
            }
            NextValues();
        }

        public void Paint(RaycastHit hit, Camera camera) {
            var norm = isPlacing ? originNormal : hit.normal;
            var rot = placeTiltOffset * currentVariant.CalculateRotation(norm, xrot, zrot, yrot, rrot);
            var pos = currentVariant.CalulatePosition(isPlacing ? origin : hit.point, norm, rot);

            if(!isPlacing && hit.collider != null)
                relativeTo = hit.transform;

            if(PrefabPainterProjectSettings.EnableSnap)
                pos = pos.Snap(PrefabPainterProjectSettings.GridSize, relativeTo);
            if(PrefabPainterProjectSettings.EnableRotationSnap)
                rot = rot.Snap(PrefabPainterProjectSettings.RotationSnapSize, relativeTo);

            var scale = currentVariant.CalculateSize(scaleValue);
            if(PrefabPainterProjectSettings.DrawModel)
                currentVariant.Renderer.Draw(Matrix4x4.TRS(pos, rot, scale), 0, camera);
            if(PrefabPainterProjectSettings.DrawWireframe)
                currentVariant.Renderer.Draw(PrefabPainterProjectSettings.WireframeMaterial, Matrix4x4.TRS(pos, rot, scale), 0, camera);
            if(PrefabPainterProjectSettings.DrawNormal)
                PrefabPainterProjectSettings.DrawPlane(pos, norm, camera, currentVariant.Renderer.Bounds.extents.magnitude / 2f);
        }

        #endregion
    }
}
