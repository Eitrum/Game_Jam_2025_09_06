using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.MiniGames {
    public class MinesweeperEditorWindow : EditorWindow {
        #region Variables

        private const string PATH = "Assets/Toolkit/MiniGames/Editor/Assets/";

        private Minesweeper minesweeper;

        private float tileSize = 25f;
        private short mines = 10;
        private Vector2 settingsScroll = Vector2.zero;

        private double timeStarted;
        private double timeDone;

        private Texture2D minesweeperHeader;
        private Texture2D minesweeperTile;
        private Texture2D minesweeperTileVisited;
        private Texture2D minesweeperFlag;
        private Texture2D minesweeperMine;
        private GUIStyle centerBold;
        private GUIStyle centerLabel;
        private GUIStyle[] centerBoldNumber;
        private Color[] NumberColors = new Color[]{
            ColorTable.LightSkyBlue,
            ColorTable.SpringGreen,
            ColorTable.IndianRed,
            ColorTable.Violet,
            ColorTable.DarkRed,
            ColorTable.DarkCyan,
            ColorTable.DarkBlue,
            ColorTable.Silver
        };

        #endregion

        #region Loading

        [MenuItem("Toolkit/Games/Minesweeper")]
        public static void ShowMinesweeper() {
            var w = GetWindow<MinesweeperEditorWindow>("Minesweeper", true);
            w.position = new Rect(w.position.position, new Vector2(450, 310));
            w.Show();
        }

        private void OnEnable() {
            tileSize = Mathf.Clamp(EditorPrefs.GetFloat("MinesweeperSize", 25f), 10, 30);
            mines = (short)Mathf.Max(EditorPrefs.GetInt("MinesweeperMines", 10), 1);
        }

        private void OnDisable() {
            EditorPrefs.SetFloat("MinesweeperSize", tileSize);
            EditorPrefs.SetInt("MinesweeperMines", mines);
        }

        private void LoadResourcesGUI() {
            if(centerBold == null) {
                centerBold = new GUIStyle(EditorStyles.boldLabel);
                centerBold.alignment = TextAnchor.MiddleCenter;
                centerBold.normal.textColor = Color.black;
                centerLabel = new GUIStyle(EditorStyles.label);
                centerLabel.alignment = TextAnchor.MiddleCenter;
                centerLabel.normal.textColor = Color.black;
            }
            if(centerBoldNumber == null) {
                centerBoldNumber = new GUIStyle[8];
                for(int i = 0; i < 8; i++) {
                    centerBoldNumber[i] = new GUIStyle(centerBold);
                    centerBoldNumber[i].normal.textColor = NumberColors[i];
                }
            }
            if(minesweeperTile == null) {
                minesweeperTile = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "minesweeperTile.png");
            }
            if(minesweeperTileVisited == null) {
                minesweeperTileVisited = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "minesweeperTileVisited.png");
            }
            if(minesweeperFlag == null) {
                minesweeperFlag = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "minesweeperFlag.png");
            }
            if(minesweeperMine == null) {
                minesweeperMine = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "minesweeperMine.png");
            }
        }

        #endregion

        #region Drawing

        private void OnGUI() {
            LoadResourcesGUI();

            var area = new Rect(Vector2.zero, position.size);
            area.SplitVertical(out Rect header, out Rect body, 60f / area.height);
            body.SplitHorizontal(out Rect gameArea, out Rect settingsArea, 1f - (200f / area.width));

            DrawHeader(header);
            DrawSettings(settingsArea);

            gameArea.ShrinkRef(10f);
            DrawGameArea(gameArea);

            Repaint();
        }

        private void DrawHeader(Rect area) {
            area.PadRef(20, 0, 0, 0);
            area.width = 250;
            if(minesweeperHeader == null) {
                minesweeperHeader = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "minesweeperHeader.png");
            }
            GUI.DrawTexture(area, minesweeperHeader);
        }

        #endregion

        #region Draw Settings

        public void SetMinesweeperSize(int width, int height) {
            var gameAreaSize = new Vector2(width * tileSize, height * tileSize);
            gameAreaSize += new Vector2(210 + tileSize / 2f, 70 + tileSize / 2f);
            position = new Rect(position.position, gameAreaSize);
        }

        private double GetTime() {
            if(minesweeper == null)
                return 0d;
            if(minesweeper.IsDone)
                return timeDone - timeStarted;

            if(minesweeper.IsGenerated)
                return EditorApplication.timeSinceStartup - timeStarted;

            return 0d;
        }

        private void DrawSettings(Rect area) {
            area.PadRef(0, 10, 10, 0);
            GUILayout.BeginArea(area);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Current Game:", EditorStyles.boldLabel);
                var flags = minesweeper?.Flags ?? 0;
                var minesLeft = minesweeper?.PlacedMines - minesweeper?.Flags ?? 0;
                EditorGUILayout.LabelField($"Time: {GetTime():0.0}s");
                EditorGUILayout.LabelField($"Flags: {flags}");
                EditorGUILayout.LabelField($"Mines Remaining: {minesLeft}");
            }

            settingsScroll = EditorGUILayout.BeginScrollView(settingsScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

            GUILayout.Space(12f);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Label("Mines", GUILayout.Width(80f));
                    GUILayout.FlexibleSpace();
                    var newMines = (short)Mathf.Max(9, EditorGUILayout.DelayedIntField(mines));
                    if(newMines != mines) {
                        mines = newMines;
                        minesweeper.SetMines(newMines);
                    }
                    GUILayout.Space(26);
                }
                EditorGUILayout.LabelField("Width: " + (minesweeper?.Width ?? 0));
                EditorGUILayout.LabelField("Height: " + (minesweeper?.Height ?? 0));

                EditorGUILayout.LabelField($"Tile Size: {tileSize:0}");
                tileSize = Mathf.RoundToInt(GUILayout.HorizontalSlider(tileSize, 10f, 30f, GUILayout.Width(area.width - 26)));
                GUILayout.Space(10);

            }
            GUILayout.Space(4);
            using(new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.LabelField("Presets:", EditorStyles.boldLabel);
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("Beginner", GUILayout.Width(90f))) {
                        mines = 10;
                        SetMinesweeperSize(9, 9);
                    }
                    GUILayout.Label("09x09 | 10x");
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("Intermediate", GUILayout.Width(90f))) {
                        mines = 40;
                        SetMinesweeperSize(16, 16);
                    }
                    GUILayout.Label("16x16 | 40x");
                }
                using(new EditorGUILayout.HorizontalScope()) {
                    if(GUILayout.Button("Expert", GUILayout.Width(90f))) {
                        mines = 99;
                        SetMinesweeperSize(30, 16);
                    }
                    GUILayout.Label("30x16 | 99x");
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        #endregion

        #region Draw Minesweeper

        private void DrawGameArea(Rect area) {
            GUI.BeginGroup(area);
            // Setup
            var width = Mathf.FloorToInt(area.width / tileSize);
            var height = Mathf.FloorToInt(area.height / tileSize);
            var offset = (area.size - new Vector2(width * tileSize, height * tileSize)) / 2f;
            area.position = Vector2.zero;
            if(minesweeper == null || minesweeper.Width != width || minesweeper.Height != height || mines != minesweeper.Mines) {
                minesweeper = new Minesweeper(width, height, mines);
                timeStarted = 0;
                timeDone = 0;
            }

            // Handle input
            var ev = Event.current;
            if(ev != null && ev.type == EventType.MouseDown) {
                if(minesweeper.IsDone) {
                    if(area.Contains(ev.mousePosition)) {
                        minesweeper.Restart();
                        timeStarted = 0;
                        timeDone = 0;
                    }
                }
                else {
                    var pos = ev.mousePosition;
                    var gridId = Vector2Byte.Floor((pos - offset) / tileSize);
                    if(ev.button == 0) { // Sweep
                        minesweeper.DoAction(gridId.x, gridId.y, Minesweeper.Action.Sweep);
                    }
                    else if(ev.button == 1) { // Flag
                        minesweeper.DoAction(gridId.x, gridId.y, Minesweeper.Action.Flag);
                    }
                }
                GUI.FocusControl(null);
            }
            if(ev != null && ev.type == EventType.KeyDown && ev.keyCode == KeyCode.Space) {
                minesweeper.Restart();
            }

            // Handle Time
            if(timeStarted == 0d && minesweeper.IsGenerated) {
                timeStarted = EditorApplication.timeSinceStartup;
            }
            if(timeDone == 0d && (minesweeper.IsDead || minesweeper.IsSolved)) {
                timeDone = EditorApplication.timeSinceStartup;
            }

            // Drawing
            DrawTiles(offset);
            DrawGameOver(area);

            GUI.EndGroup();
        }

        private void DrawGameOver(Rect area) {
            if(!minesweeper.IsDone)
                return;
            EditorGUI.DrawRect(area, new Color(0.8f, 0.8f, 0.8f, 0.5f));
            if(minesweeper.IsDead) {
                EditorGUI.LabelField(area, "Lost!", centerBold);
            }
            else if(minesweeper.IsSolved) {
                EditorGUI.LabelField(area, "Victory!", centerBold);
                EditorGUI.LabelField(area.Pad(0, 0, 15, -15), $"Time: {(timeDone - timeStarted):0.0}s", centerLabel);
            }
        }

        private void DrawTiles(Vector2 offset) {
            var width = minesweeper.Width;
            var height = minesweeper.Height;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    var area = new Rect(offset.x + x * tileSize + 1f, offset.y + y * tileSize + 1f, tileSize - 1f, tileSize - 1f);
                    var state = minesweeper[x, y];
                    if(state == Minesweeper.State.Unvisited) {
                        GUI.DrawTexture(area, minesweeperTile);
                    }
                    else if(state.HasFlag(Minesweeper.State.Flag)) {
                        GUI.DrawTexture(area, minesweeperTile);
                        GUI.DrawTexture(area, minesweeperFlag);
                    }
                    else {
                        GUI.DrawTexture(area, minesweeperTileVisited);
                        var val = (byte)state;
                        if(val <= 8) {
                            EditorGUI.LabelField(area, $"{val}", centerBoldNumber[val - 1]);
                        }
                        if(state.HasFlag(Minesweeper.State.Bomb)) {
                            EditorGUI.DrawRect(area.Shrink(4f), ColorTable.IndianRed);
                            GUI.DrawTexture(area, minesweeperMine);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
