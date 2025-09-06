using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.MiniGames
{
    public class BattleshipEditorWindow : EditorWindow
    {
        #region Variables

        private const string PATH = "Assets/Toolkit/Games/Editor/Assets/";

        public class Style
        {
            public Texture2D Header = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipHeader.png");
            public Texture2D WaterTile = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipWaterTile.png");
            public Texture2D IslandTile = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipIslandTile.png");
            public Texture2D MissMarker = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipMissMarker.png");
            public Texture2D HitMarker = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipHitMarker.png");
            public Texture2D Ship = AssetDatabase.LoadAssetAtPath<Texture2D>(PATH + "battleshipShip.png");
        }

        private Style style;
        private float tileSize = 24f;
        private float tileSpacing = 2f;

        private Battleship battleship;

        #endregion

        public float MapWidthInPixels => battleship.Width * (tileSize + tileSpacing) - tileSpacing * 2f;
        public float MapHeightInPixels => battleship.Height * (tileSize + tileSpacing) - tileSpacing * 2f;

        [MenuItem("Toolkit/Games/Battleship")]
        public static void ShowBattleship() {
            var w = GetWindow<BattleshipEditorWindow>("Battleship", true);
            w.position = new Rect(w.position.position, new Vector2(450, 310));
            w.Show();
        }

        private void OnEnable() {
            style = new Style();
            List<Vector2Int> islands = new List<Vector2Int>();
            for(int i = 0; i < 8; i++)
                islands.Add(new Vector2Int(Random.Range(0, 10), Random.Range(0, 10)));
            battleship = new Battleship(10, islands);
        }

        private void OnGUI() {
            GUI.DrawTexture(new Rect(5, 5, 250f, 60f), style.Header);

            // Draw opponent tile
            GUI.BeginGroup(new Rect(10, 80, MapWidthInPixels, MapHeightInPixels));
            DrawMap(false, tileSize, tileSpacing);
            GUI.EndGroup();

            // Draw own tile
            GUI.BeginGroup(new Rect(14f + MapWidthInPixels, 80, MapWidthInPixels / 2f, MapHeightInPixels / 2f));
            DrawMap(true, tileSize / 2f, tileSpacing / 2f);
            GUI.EndGroup();
        }

        private void DrawMap(bool isMainPlayer, float tileSize, float tileSpacing) {
            var map = battleship.GetMap(isMainPlayer);
            var ships = battleship.GetShips(isMainPlayer);
            var w = map.Width;
            var h = map.Height;
            var data = map.Data;
            var gridSize = tileSize + tileSpacing;

            var e = Event.current;

            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    var t = data[x, y];
                    var p = new Rect(x * gridSize, y * gridSize, tileSize, tileSize);

                    GUI.DrawTexture(p, Battleship.IsIsland(t) ? style.IslandTile : style.WaterTile);
                    if(isMainPlayer && Battleship.IsShip(t))
                        GUI.DrawTexture(p, style.Ship);

                    if(Battleship.IsHit(t))
                        GUI.DrawTexture(p, Battleship.IsShip(t) ? style.HitMarker : style.MissMarker);

                    if(e != null && e.type == EventType.MouseDown && e.button == 0 && p.Contains(e.mousePosition)) {
                        map.MarkHit(new Vector2Int(x, y));
                        e.Use();
                    }
                }
            }
            if(!isMainPlayer) {
                foreach(var s in ships) {
                    if(s.IsSunken(map)) {
                        var pos = s.Position;
                        foreach(var sun in s.Shape) {
                            var tpos = pos + sun;
                            var p = new Rect(tpos.x * gridSize, tpos.y * gridSize, tileSize, tileSize);
                            GUI.DrawTexture(p, style.Ship);
                            GUI.DrawTexture(p, style.HitMarker);
                        }
                    }
                }
            }
        }
    }
}
