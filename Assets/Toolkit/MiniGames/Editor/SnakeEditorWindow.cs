using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.MiniGames
{
    public class SnakeEditorWindow : EditorWindow
    {
        private const float GRID_SIZE = 16f;
        private const float GRID_SIZE_HALF = GRID_SIZE / 2f;

        public Snake snake;
        private Snake.Settings settings = new Snake.Settings();

        private Texture2D headerTexture;
        private float updateRate = 0.125f;
        private double time;
        private float updateTime = 0f;
        bool hasChangedDirection = false;
        private Vector2 settingsScroll = Vector2.zero;
        private GUIStyle centerBold;
        private GUIStyle centerLabel;

        [MenuItem("Toolkit/Games/Snake")]
        public static void ShowSnake() {
            var w = GetWindow<SnakeEditorWindow>("Snake", true);
            w.position = new Rect(w.position.position, new Vector2(450, 330));
            w.Show();
        }

        private void OnEnable() {
            time = EditorApplication.timeSinceStartup;
        }

        private void ChangeDirection(Snake.Direction dir) {
            if(hasChangedDirection) {
                snake.Tick();
                updateTime = 0f;
            }
            snake.CurrentDirection = dir;
            hasChangedDirection = true;
        }

        #region Drawing

        private void OnGUI() {
            if(centerBold == null) {
                centerBold = new GUIStyle(EditorStyles.boldLabel);
                centerBold.alignment = TextAnchor.MiddleCenter;
                centerLabel = new GUIStyle(EditorStyles.label);
                centerLabel.alignment = TextAnchor.MiddleCenter;
            }
            var area = new Rect(Vector2.zero, position.size);
            area.SplitVertical(out Rect header, out Rect body, 60f / area.height);
            body.SplitHorizontal(out Rect gameArea, out Rect settingsArea, 1f - (200f / area.width));

            DrawHeader(header);
            DrawSettings(settingsArea);
            gameArea.ShrinkRef(10f);
            DrawSnakeArea(gameArea);

            Repaint();
        }

        private void DrawHeader(Rect area) {
            if(headerTexture == null) {
                headerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Toolkit/MiniGames/Editor/Assets/snakeHeader.png");
            }
            area.PadRef(10, 0, 5, 5);
            area.width = 250;
            GUI.DrawTexture(area, headerTexture);
        }

        private void DrawSettings(Rect area) {
            area.PadRef(0, 10, 10, 0);
            GUILayout.BeginArea(area);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Current Game:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Score: {snake?.Score:000}");
            }

            GUILayout.Space(12f);
            using(new EditorGUILayout.VerticalScope("box")) {
                settingsScroll = EditorGUILayout.BeginScrollView(settingsScroll, GUIStyle.none, GUI.skin.verticalScrollbar);
                EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
                var speed = Mathf.RoundToInt(1f / updateRate);
                EditorGUILayout.LabelField($"Speed: {speed} steps/second");
                speed = Mathf.RoundToInt(GUILayout.HorizontalSlider(speed, 1f, 16f, GUILayout.Width(area.width - 26)));
                updateRate = 1f / speed;

                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Food available: {settings.FoodAtAnyTime}");
                settings.FoodAtAnyTime = (byte)Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.FoodAtAnyTime, 1f, 16f, GUILayout.Width(area.width - 26)));

                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Growth per food: {settings.GrowthPerFood}");
                settings.GrowthPerFood = (byte)Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.GrowthPerFood, 1f, 16f, GUILayout.Width(area.width - 26)));

                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Safety Frames: {settings.SafetyFrames}");
                settings.SafetyFrames = (byte)Mathf.RoundToInt(GUILayout.HorizontalSlider(settings.SafetyFrames, 1f, 16f, GUILayout.Width(area.width - 26)));

                GUILayout.Space(10);
                settings.LoopingWalls = !EditorGUILayout.ToggleLeft("Walls", !settings.LoopingWalls);

                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Snake Area

        private void DrawSnakeArea(Rect area) {
            GUI.BeginGroup(area);
            area.position = Vector2.zero;
            var width = Mathf.FloorToInt(area.width / GRID_SIZE);
            var height = Mathf.FloorToInt(area.height / GRID_SIZE);

            if(snake == null || width != snake.Width || height != snake.Height) {
                snake = new Snake(width, height, settings);
            }

            // Handle Delta Time
            var currentTime = EditorApplication.timeSinceStartup;
            var dt = (float)(currentTime - time);
            time = currentTime;

            // Handle Input
            var ev = Event.current;
            if(ev.type == EventType.KeyDown) {
                switch(ev.keyCode) {
                    case KeyCode.UpArrow:
                        ChangeDirection(Snake.Direction.Up);
                        ev.Use();
                        break;
                    case KeyCode.DownArrow:
                        ChangeDirection(Snake.Direction.Down);
                        ev.Use();
                        break;
                    case KeyCode.LeftArrow:
                        ChangeDirection(Snake.Direction.Left);
                        ev.Use();
                        break;
                    case KeyCode.RightArrow:
                        ChangeDirection(Snake.Direction.Right);
                        ev.Use();
                        break;
                    case KeyCode.Space:
                        snake.Restart();
                        ev.Use();
                        break;
                }
            }
            if(ev != null && ev.type == EventType.MouseDown && ev.button == 0) {
                ev.Use();
                GUI.FocusControl(null);
            }


            // Update snake
            updateTime += dt;
            if(updateTime >= updateRate) {
                updateTime -= updateRate;
                snake.Tick();
                hasChangedDirection = false;
            }

            var offset = (area.size - new Vector2(width * GRID_SIZE, height * GRID_SIZE)) / 2f;

            DrawGrid(offset, width, height);
            DrawFood(offset, snake.Food);
            DrawSnakeBody(offset, snake.Body, snake.CurrentDirection);

            if(snake.IsDead) {
                EditorGUI.DrawRect(area, Color.gray.MultiplyAlpha(0.5f));
                EditorGUI.LabelField(new Rect(area.center - new Vector2(60, 15), new Vector2(120, 15)), "Game Over", centerBold);
                EditorGUI.LabelField(new Rect(area.center - new Vector2(60, 0), new Vector2(120, 15)), $"Score: {snake.Score:000}", centerBold);
                EditorGUI.LabelField(new Rect(area.center - new Vector2(60, -30), new Vector2(120, 15)), $"How to play", centerBold);
                EditorGUI.LabelField(new Rect(area.center - new Vector2(60, -45), new Vector2(120, 15)), $"Arrows to move", centerLabel);
                EditorGUI.LabelField(new Rect(area.center - new Vector2(60, -60), new Vector2(120, 15)), $"Space to restart!", centerLabel);
            }
            GUI.EndGroup();
        }

        private static void DrawGrid(Vector2 offset, int width, int height) {
            var gridColor = new Color(0.3f, 0.3f, 0.3f);
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    EditorGUI.DrawRect(new Rect(offset.x + x * GRID_SIZE + 1f, offset.y + y * GRID_SIZE + 1f, GRID_SIZE - 1f, GRID_SIZE - 1f), gridColor);
                }
            }
        }

        private static void DrawFood(Vector2 offset, IReadOnlyList<Vector2Byte> items) {
            var fruit = ColorTable.IndianRed;
            var stem = ColorTable.Olive;
            for(int i = 0, length = items.Count; i < length; i++) {
                DrawFoodAt(new Rect(offset.x + items[i].x * GRID_SIZE + 1f, offset.y + items[i].y * GRID_SIZE + 1f, GRID_SIZE - 1f, GRID_SIZE - 1f), fruit, stem);
            }
        }

        private static void DrawFoodAt(Rect area, Color fruit, Color stem) {
            EditorGUI.DrawRect(area.Pad(3f, 3f, 2f, 0f), fruit);
            EditorGUI.DrawRect(area.Pad(1f, 1f, 4f, 2f), fruit);
            var stemWidthPad = area.width / 2f - 2f;
            EditorGUI.DrawRect(area.Pad(stemWidthPad, stemWidthPad, -1f, area.height - 4), stem);
        }

        private static void DrawSnakeBody(Vector2 offset, IReadOnlyList<Vector2Byte> items, Snake.Direction direction) {
            var body1 = (Color)ColorTable.DarkOliveGreen;
            var body0 = body1.MultiplyRGB(0.95f);
            var bodyStripes = ColorTable.DarkSlateBlue;
            var length = items.Count;
            var headArea = new Rect(offset.x + items[0].x * GRID_SIZE + 2f, offset.y + items[0].y * GRID_SIZE + 2f, GRID_SIZE - 3f, GRID_SIZE - 3f);
            DrawHead(headArea, direction);

            for(int i = 1; i < length; i++) {
                var c = items[i];
                EditorGUI.DrawRect(new Rect(offset.x + c.x * GRID_SIZE + 2f, offset.y + c.y * GRID_SIZE + 2f, GRID_SIZE - 3f, GRID_SIZE - 3f), (i % 3 == 0) ? body0 : body1);
            }
        }

        private static void DrawHead(Rect area, Snake.Direction dir) {
            var head = ColorTable.DarkOrange;
            var toung = ColorTable.DarkRed;
            EditorGUI.DrawRect(area, head);

            switch(dir) {
                case Snake.Direction.Up:
                    EditorGUI.DrawRect(area.Pad(GRID_SIZE_HALF - 3f, GRID_SIZE_HALF - 3f, -GRID_SIZE_HALF, GRID_SIZE - 3), toung);
                    EditorGUI.DrawRect(area.Pad(2f, area.width - 4, 2f, area.height - 4), Color.black);
                    EditorGUI.DrawRect(area.Pad(area.width - 4, 2f, 2f, area.height - 4), Color.black);
                    break;
                case Snake.Direction.Down:
                    EditorGUI.DrawRect(area.Pad(GRID_SIZE_HALF - 3f, GRID_SIZE_HALF - 3f, GRID_SIZE - 3, -GRID_SIZE_HALF), toung);
                    EditorGUI.DrawRect(area.Pad(2f, area.width - 4, area.height - 4, 2f), Color.black);
                    EditorGUI.DrawRect(area.Pad(area.width - 4, 2f, area.height - 4, 2f), Color.black);
                    break;

                case Snake.Direction.Left:
                    EditorGUI.DrawRect(area.Pad(-GRID_SIZE_HALF, GRID_SIZE - 3, GRID_SIZE_HALF - 3f, GRID_SIZE_HALF - 3f), toung);
                    EditorGUI.DrawRect(area.Pad(2f, area.height - 4, 2f, area.width - 4), Color.black);
                    EditorGUI.DrawRect(area.Pad(2f, area.height - 4, area.width - 4, 2f), Color.black);
                    break;
                case Snake.Direction.Right:
                    EditorGUI.DrawRect(area.Pad(GRID_SIZE - 3, -GRID_SIZE_HALF, GRID_SIZE_HALF - 3f, GRID_SIZE_HALF - 3f), toung);
                    EditorGUI.DrawRect(area.Pad(area.height - 4, 2f, 2f, area.width - 4), Color.black);
                    EditorGUI.DrawRect(area.Pad(area.height - 4, 2f, area.width - 4, 2f), Color.black);
                    break;
            }
        }

        #endregion
    }
}
