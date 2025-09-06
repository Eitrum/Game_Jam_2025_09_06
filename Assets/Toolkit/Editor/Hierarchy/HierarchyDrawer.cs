using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Toolkit {
    public static class HierarchyDrawer {

        #region Styles

        public static class Styles {
            public static readonly Texture SettingsIcon = EditorGUIUtility.TrIconContent("d_Settings Icon").image;
            public static readonly Texture2D Gradient_Small = Resources.Load<Texture2D>("hierarchy_gradient");
            public static readonly Texture2D Gradient_Large = Resources.Load<Texture2D>("hierarchy_gradient_wide");
            public static readonly Texture2D TextBackground = Resources.Load<Texture2D>("hierarchy_text_background");
            public static readonly Color StripeColor = new Color(1, 1, 1, 0.02f);
            public static readonly Color CustomDrawerSideStripes = new Color(0.5f, 0.5f, 0.5f, 0.3f);

            public static GUIStyle NormalLabel = new GUIStyle("TV Line") { richText = true };
            public static GUIStyle PrefabLabel = new GUIStyle("PR PrefabLabel"){ richText = true };
            public static GUIStyle DisabledLabel = new GUIStyle("PR DisabledLabel"){ richText = true };
            public static GUIStyle DisabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel"){ richText = true };
            public static GUIStyle BrokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel"){ richText = true };
            public static GUIStyle DisabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel"){ richText = true };

            public static Texture2D GetGradient(HierarchySettings.BackgroundMode backgroundMode) {
                switch(backgroundMode) {
                    case HierarchySettings.BackgroundMode.Small: return Gradient_Small;
                    case HierarchySettings.BackgroundMode.Medium: return Gradient_Large;
                }
                return Gradient_Large;
            }

            public static GUIStyle GetStyle(GameObjectState state) {
                state = state & ~GameObjectState.PrefabVariant;
                switch(state) {
                    case GameObjectState.Prefab: return PrefabLabel;
                    case GameObjectState.Disabled: return DisabledLabel;
                    case GameObjectState.Broken: return BrokenPrefabLabel;
                    case GameObjectState.Disabled | GameObjectState.Prefab: return DisabledPrefabLabel;
                    case GameObjectState.Broken | GameObjectState.Prefab: return BrokenPrefabLabel;
                    case GameObjectState.Broken | GameObjectState.Prefab | GameObjectState.Disabled: return DisabledBrokenPrefabLabel;
                }
                return NormalLabel;
            }

            public static Texture GetIcon(GameObjectState state) {
                var texture = GameObject;
                if(state.HasFlag(GameObjectState.PrefabVariant))
                    texture = PrefabVariant;
                else if(state.HasFlag(GameObjectState.Prefab))
                    texture = Prefab;
                return texture;
            }

            public static (Texture, GUIStyle) GetIconAndStyle(GameObjectState state) {
                var style = GetStyle(state);
                var texture = GetIcon(state);
                return (texture, style);
            }

            public static Texture GameObject = EditorGUIUtility.TrIconContent("d_GameObject Icon").image;
            public static Texture Prefab = EditorGUIUtility.TrIconContent("d_Prefab Icon").image;
            public static Texture PrefabVariant = EditorGUIUtility.TrIconContent("d_PrefabVariant Icon").image;

            public static Texture GetTextureForGameObject(GameObject go) {
                var isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(go);
                if(isPrefab) {
                    var isVariant = PrefabUtility.IsPartOfVariantPrefab(go);
                    if(isVariant)
                        return PrefabVariant;
                    return Prefab;
                }
                else
                    return GameObject;
            }

            static Styles() {
                DisabledPrefabLabel.normal.textColor = PrefabLabel.normal.textColor.MultiplyAlpha(0.7f);
            }
        }

        #endregion

        public interface ICustomDrawer {
            float Width { get; }
            string Header { get; }
            void DrawHeader(Rect rect) {
                GUI.Label(rect, Header, EditorStylesUtility.CenterAlignedMiniLabel);
            }
            void Draw(Rect area, Context context);
        }

        public class Context {
            public GameObject gameObject;
            public List<Component> components = new List<Component>();

            public Context() { }
            public Context(GameObject go) => Update(go);

            public void Update(GameObject go) {
                this.gameObject = go;
                go.GetComponents(components);
            }
        }

        #region Variables

        private const string TAG = "<color=#00ffff>[Toolkit.HierarchyDrawer]</color> - ";
        public const float ITEM_HEIGHT = 16f;

        private static readonly Color backgroundColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
        private static FieldInfo genericMenuItemsList;

        private static List<ICustomDrawer> customDrawers = new List<ICustomDrawer>();

        public static event System.Action OnCustomDrawerRebuild;
        private static Dictionary<string, ICustomDrawer> registeredCustomDrawers = new Dictionary<string, ICustomDrawer>();
        public static IReadOnlyDictionary<string, ICustomDrawer> RegisteredCustomDrawers => registeredCustomDrawers;

        #endregion

        #region Custom Drawer

        public static void RegisterCustomDrawer<T>(T drawer) where T : ICustomDrawer {
            if(!registeredCustomDrawers.TryAdd(drawer.Header, drawer)) {
                Debug.LogError(TAG + $"Failed to register custom drawer as one already exist with same name '{drawer.Header}'");
            }
        }

        #endregion

        #region Init

        [InitializeOnLoadMethod]
        private static void Initialize() {
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
            genericMenuItemsList = typeof(GenericMenu).GetField("m_MenuItems", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            UnityEditor.SceneManagement.SceneHierarchyHooks.addItemsToGameObjectContextMenu += OnAddItems;
            EditorApplication.delayCall += Rebuild;
        }

        public static void Rebuild() {
            customDrawers.Clear();
            var drawersToRender = HierarchySettings.Drawers;
            foreach(var b in drawersToRender) {
                if(registeredCustomDrawers.TryGetValue(b, out var value)) {
                    customDrawers.Add(value);
                }
            }

            OnCustomDrawerRebuild?.Invoke();
        }

        private static void OnAddItems(GenericMenu menu, GameObject go) {
            menu.AddItem(new GUIContent("Configure"), false, () => Configure(go));
            var menuItemList = genericMenuItemsList.GetValue(menu) as IList;
            var configureItem = menuItemList[menuItemList.Count - 1];
            menuItemList.RemoveAt(menuItemList.Count - 1);
            menuItemList.Insert(0, configureItem);

            menu.AddSeparator("");
            var seperator = menuItemList[menuItemList.Count - 1];
            menuItemList.RemoveAt(menuItemList.Count - 1);
            menuItemList.Insert(1, seperator);
        }

        private static void Configure(GameObject go) {
            // var mousePos = Event.current.mousePosition;
            HierarchyConfigureWindow.Configure(Vector2.zero, go);
        }

        #endregion

        #region Draw

        private static void OnItemGUI(int instanceID, Rect selectionRect) {
            try {
                var obj = EditorUtility.InstanceIDToObject(instanceID);

                if(obj is GameObject go) {
                    OnGameObjectGUI(go, selectionRect);
                }
                else {
                    var count = UnityEngine.SceneManagement.SceneManager.sceneCount;
                    for(int i = count - 1; i >= 0; i--) {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
                        if(scene.GetHashCode() == instanceID) {
                            OnSceneGUI(scene, selectionRect);
                            break;
                        }
                    }
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        [Flags]
        public enum GameObjectState {
            Normal = 0,
            Prefab = 1,
            Disabled = 2,
            Broken = 4,
            PrefabVariant = 8,
        }

        private static void OnSceneGUI(UnityEngine.SceneManagement.Scene scene, Rect area) {
            bool isActive = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() == scene;
            // Handle scene drawers

            var textWidth = Styles.NormalLabel.CalcSize(EditorGUIUtility.TrTempContent(scene.name)).x + 30;
            var remainingWidth = area.width - textWidth;
            var drawerHeaders = (new Rect(area.x + textWidth, area.y, remainingWidth, ITEM_HEIGHT));
            DrawCustomBlocksHeaders(drawerHeaders);
        }

        private static void OnGameObjectGUI(GameObject go, Rect area) {
            var context = new Context(go);
            var state = GameObjectState.Normal;
            if(PrefabUtility.IsAnyPrefabInstanceRoot(go)) {
                state |= GameObjectState.Prefab;
                if(PrefabUtility.IsPartOfVariantPrefab(go))
                    state |= GameObjectState.PrefabVariant;
            }
            if(!go.activeInHierarchy)
                state |= GameObjectState.Disabled;
            if(PrefabUtility.IsPrefabAssetMissing(go))
                state |= GameObjectState.Broken;

            var prop = go.GetComponentInParent<HierarchyItem>(true);
            bool isParent = true;
            string nameToCalculate = null;
            if(prop) {
                DrawBackground(go, area);
                if(HierarchySettings.Stripes)
                    DrawStripes(area);
                isParent = prop.gameObject != go;
                if(isParent) {
                    DrawHighlightAsChild(go, area, prop, state);
                    nameToCalculate = go.name;
                }
                else {
                    DrawHighlight(go, area, prop, state);
                    var cev = Event.current;
                    nameToCalculate = ((cev != null && cev.alt) || string.IsNullOrEmpty(prop.CustomText)) ? go.name : prop.CustomText;
                }
            }
            else {
                if(HierarchySettings.Stripes)
                    DrawStripes(area);
                nameToCalculate = go.name;
            }

            var textWidth = Styles.NormalLabel.CalcSize(EditorGUIUtility.TrTempContent(nameToCalculate)).x + 30;
            var remainingWidth = area.width - textWidth;


            remainingWidth = DrawCustomBlocks(context, new Rect(area.x + textWidth, area.y, remainingWidth, ITEM_HEIGHT));

            // Custom Prop
            if(prop != null && !isParent && prop.TryGetCustomDrawer(out float customDrawerWidth, out Action<Rect> onDrawCustom) && customDrawerWidth < remainingWidth) {
                var customArea = new Rect(area.x + textWidth + remainingWidth - customDrawerWidth - 4, area.y, customDrawerWidth, ITEM_HEIGHT);
                onDrawCustom.Invoke(customArea);
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        public static void DrawBackground(GameObject go, Rect area) {
            EditorGUI.DrawRect(area, backgroundColor);
            // Figure out if selected or not and draw correct color
        }

        public static void DrawStripes(Rect area) {
            var lineIndex = Mathf.RoundToInt(area.y / ITEM_HEIGHT);
            if(lineIndex % 2 == 0)
                EditorGUI.DrawRect(area, Styles.StripeColor);
        }

        public static void DrawHighlight(GameObject go, Rect area, HierarchyItem prop, GameObjectState state) {
            // Draw Gradiant
            var backgroundMode = prop.BackgroundMode == 0 ? HierarchySettings.Background : ((HierarchySettings.BackgroundMode)prop.BackgroundMode);
            switch(backgroundMode) {
                case HierarchySettings.BackgroundMode.Medium:
                case HierarchySettings.BackgroundMode.Small: {
                        var texRect = new Rect(area.x, area.y, Mathf.Min(area.width, Styles.GetGradient(backgroundMode).width), area.height);
                        var col = prop.Color;
                        col.a = Mathf.Min(col.a, 0.75f);
                        GUI.DrawTexture(texRect, Styles.GetGradient(backgroundMode), ScaleMode.ScaleAndCrop, true, 0f, col, 0, 0);
                    }
                    break;
                case HierarchySettings.BackgroundMode.Full: {
                        var texRect = new Rect(area.x, area.y, area.width, area.height);
                        var col = prop.Color;
                        col.a = Mathf.Min(col.a, 0.75f);
                        EditorGUI.DrawRect(texRect, col);
                    }
                    break;
            }

            var style = Styles.GetStyle(state);
            // Draw Icon
            var icon = prop.Icon == null ? Styles.GetIcon(state) : prop.Icon;
            if(icon)
                GUI.DrawTexture(new Rect(area.x, area.y, ITEM_HEIGHT, ITEM_HEIGHT), icon, ScaleMode.ScaleToFit, true, 0f, new Color(1, 1, 1, style.normal.textColor.a), 0f, 0f);

            // Draw Label
            var cev = Event.current;
            var text =  ((cev != null && cev.alt) || string.IsNullOrEmpty(prop.CustomText)) ? go.name : prop.CustomText;
            var ogColor = GUI.contentColor;
            if(prop.CustomFontColor.a > 0.05f)
                GUI.contentColor = prop.CustomFontColor;
            GUI.Label(area.Pad(18, 0, 0, 0), text, style);
            GUI.contentColor = ogColor;
        }

        public static void DrawHighlightAsChild(GameObject go, Rect area, HierarchyItem prop, GameObjectState state) {
            // Draw Gradient
            var backgroundMode = prop.BackgroundMode == 0 ? HierarchySettings.Background : ((HierarchySettings.BackgroundMode)prop.BackgroundMode);
            switch(backgroundMode) {
                case HierarchySettings.BackgroundMode.Medium:
                case HierarchySettings.BackgroundMode.Small: {
                        var texRect = new Rect(area.x, area.y, Mathf.Min(area.width, Styles.GetGradient(backgroundMode).width), area.height);
                        var col = prop.Color;
                        col.a = Mathf.Min(col.a, 0.75f);
                        col.a *= HierarchySettings.ChildColorMultiplier;
                        GUI.DrawTexture(texRect, Styles.GetGradient(backgroundMode), ScaleMode.ScaleAndCrop, true, 0f, col, 0, 0);
                    }
                    break;
                case HierarchySettings.BackgroundMode.Full: {
                        var texRect = new Rect(area.x, area.y, area.width, area.height);
                        var col = prop.Color;
                        col.a = Mathf.Min(col.a, 0.75f);
                        col.a *= HierarchySettings.ChildColorMultiplier;
                        EditorGUI.DrawRect(texRect, col);
                    }
                    break;
            }

            // Draw Icon
            var style = Styles.GetStyle(state);
            // Draw Icon
            GUI.DrawTexture(new Rect(area.x, area.y, ITEM_HEIGHT, ITEM_HEIGHT), Styles.GetIcon(state), ScaleMode.ScaleToFit, true, 0f, new Color(1, 1, 1, style.normal.textColor.a), 0f, 0f);

            // Draw Label
            var ogColor = GUI.contentColor;
            if(prop.CustomFontColor.a > 0.05f)
                GUI.contentColor = prop.CustomFontColor;
            GUI.Label(area.Pad(18, 0, 0, 0), go.name, style);
            GUI.contentColor = ogColor;
        }

        public static float DrawCustomBlocks(Context context, Rect area) {
            try {
                GUI.BeginGroup(area);
                float x = area.width;
                foreach(var c in customDrawers) {
                    x -= c.Width;
                    var reservedArea = new Rect(x, 0, c.Width, ITEM_HEIGHT);
                    try {
                        DrawCustomBlockSideLines(reservedArea);
                        c.Draw(reservedArea, context);
                    }
                    catch { }
                    x -= 4;
                    if(x < 0)
                        break;
                }
                return x;
            }
            catch {
                return 0f;
            }
            finally {
                GUI.EndGroup();
            }
        }

        public static float DrawCustomBlocksHeaders(Rect area) {
            try {
                GUI.BeginGroup(area);
                float x = area.width;
                foreach(var c in customDrawers) {
                    x -= c.Width;
                    var reservedArea = new Rect(x, 0, c.Width, ITEM_HEIGHT);
                    try {
                        DrawCustomBlockSideLines(reservedArea);
                        c.DrawHeader(reservedArea);
                    }
                    catch { }
                    x -= 4;
                    if(x < 0)
                        break;
                }
                return x;
            }
            catch {
                return 0f;
            }
            finally {
                GUI.EndGroup();
            }
        }

        public static void DrawCustomBlockSideLines(Rect area) {
            EditorGUI.DrawRect(new Rect(area.x - 3f, area.y, 1f, area.height), Styles.CustomDrawerSideStripes);
            //EditorGUI.DrawRect(new Rect(area.x + area.width - 1f, area.y, 1f, area.height), Color.gray);
        }

        #endregion
    }
}
