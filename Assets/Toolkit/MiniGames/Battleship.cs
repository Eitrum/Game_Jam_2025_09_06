using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.MiniGames
{
    public class Battleship
    {
        #region Variables

        private const int ISLAND = 1 << 0;
        private const int SHIP = 1 << 1;
        private const int HIT = 1 << 2;

        private byte[,] islandBitMask;
        private Map player1;
        private List<Ship> player1Ships;
        private Map player2;
        private List<Ship> player2Ships;

        #endregion

        #region Properties

        public int Width => player1.Width;
        public int Height => player1.Height;

        #endregion

        #region Constructor

        public Battleship(int size) : this(size, size) { }

        public Battleship(int size, IReadOnlyList<Vector2Int> islandTiles) : this(size, size, islandTiles) { }

        public Battleship(int width, int height) : this(width, height, null) { }

        public Battleship(int width, int height, IReadOnlyList<Vector2Int> islandTiles) {
            player1 = new Map(width, height, islandTiles);
            player1Ships = new List<Ship>() {
                new Ship(Ship.DefaultShape.PatrolBoat),
                new Ship(Ship.DefaultShape.Submarine),
                new Ship(Ship.DefaultShape.Destroyer),
                new Ship(Ship.DefaultShape.Battleship),
                new Ship(Ship.DefaultShape.Carrier),
            };
            player2 = new Map(width, height, islandTiles);
            player2Ships = new List<Ship>() {
                new Ship(Ship.DefaultShape.PatrolBoat),
                new Ship(Ship.DefaultShape.Submarine),
                new Ship(Ship.DefaultShape.Destroyer),
                new Ship(Ship.DefaultShape.Battleship),
                new Ship(Ship.DefaultShape.Carrier),
            };

            foreach(var p in player1Ships)
                p.PlaceAtRandomLocation(player1);

            foreach(var p in player2Ships)
                p.PlaceAtRandomLocation(player2);
        }

        #endregion

        public Map GetMap(bool isMainPlayer) => isMainPlayer ? player1 : player2;
        public IReadOnlyList<Ship> GetShips(bool isMainPlayer) => isMainPlayer ? player1Ships : player2Ships;

        #region Check

        public static bool IsIsland(byte b) => (b & ISLAND) == ISLAND;
        public static bool IsShip(byte b) => (b & SHIP) == SHIP;
        public static bool IsHit(byte b) => (b & HIT) == HIT;

        #endregion

        public class Map
        {
            #region Variables

            private int width, height;
            private byte[,] mapData;

            #endregion

            #region Properties

            public int Width => width;
            public int Height => height;
            public byte[,] Data => mapData;

            #endregion

            #region Constructor

            public Map(int width, int height, IReadOnlyList<Vector2Int> islandTiles) {

                this.width = width;
                this.height = height;
                mapData = new byte[width, height];

                if(islandTiles != null)
                    foreach(var t in islandTiles)
                        AddIsland(t);
            }

            #endregion

            #region Check

            public bool IsFreeTile(Vector2Int tile) {
                if(tile.x < 0 || tile.x >= width || tile.y < 0 || tile.y >= height)
                    return false;
                var t = mapData[tile.x, tile.y];
                return !(IsIsland(t) || IsShip(t));
            }

            #endregion

            #region Modifications

            public void AddIsland(Vector2Int tile) {
                if(tile.x < 0 || tile.x >= width || tile.y < 0 || tile.y >= height)
                    return;
                mapData[tile.x, tile.y] |= ISLAND;
            }

            public void AddShip(Vector2Int tile) {
                if(tile.x < 0 || tile.x >= width || tile.y < 0 || tile.y >= height)
                    return;
                mapData[tile.x, tile.y] |= SHIP;
            }

            public void AddShip(Ship ship) {
                var pos = ship.Position;
                foreach(var s in ship.Shape)
                    AddShip(pos + s);
            }

            public void MarkHit(Vector2Int tile) {
                if(tile.x < 0 || tile.x >= width || tile.y < 0 || tile.y >= height)
                    return;
                mapData[tile.x, tile.y] |= HIT;
            }

            #endregion
        }

        public class Ship
        {
            #region Variables

            private Vector2Int position;
            private List<Vector2Int> shape;

            #endregion

            #region Properties

            public Vector2Int Position {
                get => position;
                set => position = value;
            }

            public IReadOnlyList<Vector2Int> Shape => shape;

            #endregion

            #region Constructor

            public Ship(DefaultShape shape) {
                this.position = new Vector2Int();
                this.shape = new List<Vector2Int>();
                var l = GetLengthFromDefaultShape(shape);
                for(int i = 0; i < l; i++)
                    this.shape.Add(new Vector2Int(0, i));
            }

            public Ship(IReadOnlyList<Vector2Int> shape) {
                this.position = new Vector2Int();
                this.shape = new List<Vector2Int>(shape);
            }

            #endregion

            #region Translate

            public void SetPosition(Vector2Int position) => this.position = position;

            #endregion

            #region Rotation

            public void RotateRight() {
                for(int i = 0, length = shape.Count; i < length; i++)
                    shape[i] = new Vector2Int(shape[i].y, -shape[i].x);
            }

            public void RotateLeft() {
                for(int i = 0, length = shape.Count; i < length; i++)
                    shape[i] = new Vector2Int(-shape[i].y, shape[i].x);
            }

            #endregion

            #region Utility

            public bool IsSunken(Map map) {
                for(int i = 0, length = shape.Count; i < length; i++) {
                    var p = position + shape[i];
                    if(!Battleship.IsHit(map.Data[p.x, p.y]))
                        return false;
                }

                return true;
            }

            public bool PlaceAtRandomLocation(Map map) {
                // Initial rotation
                for(int i = 0, length = Random.Range(0, 4); i < length; i++)
                    RotateRight();

                var w = map.Width;
                var h = map.Height;

                for(int i = 0; i < 4; i++) {
                    for(int a = 0; a < 80; a++) {
                        var pos = new Vector2Int(Random.Range(0, w), Random.Range(0, h));
                        if(DoesFit(this, pos, map)) {
                            this.position = pos;
                            map.AddShip(this);
                            return true;
                        }
                    }
                }
                return false;
            }

            public static bool DoesFit(Ship ship, Map map) {
                for(int i = 0, length = ship.shape.Count; i < length; i++) {
                    var p = ship.position + ship.shape[i];
                    if(!map.IsFreeTile(p))
                        return false;
                }

                return true;
            }

            public static bool DoesFit(Ship ship, Vector2Int overridePosition, Map map) {
                for(int i = 0, length = ship.shape.Count; i < length; i++) {
                    var p = overridePosition + ship.shape[i];
                    if(!map.IsFreeTile(p))
                        return false;
                }

                return true;
            }

            public static int GetLengthFromDefaultShape(DefaultShape shape) {
                switch(shape) {
                    case DefaultShape.PatrolBoat: return 2;
                    case DefaultShape.Submarine:
                    case DefaultShape.Destroyer:
                        return 3;
                    case DefaultShape.Battleship: return 4;
                    case DefaultShape.Carrier: return 5;
                }
                return 1;
            }

            public enum DefaultShape
            {
                PatrolBoat = 0, // 1x2
                Submarine = 1, // 1x3
                Destroyer = 2, // 1x3
                Battleship = 3, // 1x4
                Carrier = 4, // 1x5
            }

            #endregion
        }
    }
}
