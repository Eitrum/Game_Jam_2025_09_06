using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.MiniGames
{
    public class Snake
    {
        #region Classes

        public class Settings
        {
            public bool LoopingWalls = false;
            public byte FoodAtAnyTime = 2;
            public byte SafetyFrames = 2;
            public byte StartLength = 3;
            public byte GrowthPerFood = 2;
        }

        public enum Direction : byte
        {
            Up,
            Down,
            Left,
            Right
        }

        #endregion

        #region Variables

        private byte width = 8;
        private byte height = 8;
        private Settings settings;

        // 0 = Up, 1 = Down, 2 = Left, 3 = Right
        private byte direction = 0;
        private byte grow = 0;
        private byte safetyFrames = 0;

        private List<Vector2Byte> snake = new List<Vector2Byte>();
        private List<Vector2Byte> food = new List<Vector2Byte>();
        private byte[,] map;
        private System.Random random = new System.Random();

        #endregion

        #region Properties

        public Settings CurrentSettings => settings;
        public bool IsDead => safetyFrames >= settings.SafetyFrames;
        public bool HasWon => snake.Count >= width * height;
        public int SafetyFrames => safetyFrames;
        public int Growing => grow;
        public Direction CurrentDirection {
            get => (Direction)direction;
            set => direction = (byte)value;
        }

        public int Width => width;
        public int Height => height;
        public IReadOnlyList<Vector2Byte> Food => food;
        public IReadOnlyList<Vector2Byte> Body => snake;

        public int Score => snake.Count;

        #endregion

        #region Constructor

        public Snake() {
            this.settings = new Settings();
            this.map = new byte[width, height];
            LoadStart();
        }

        public Snake(byte width, byte height) {
            this.settings = new Settings();
            this.width = width;
            this.height = height;
            this.map = new byte[width, height];
            LoadStart();
        }

        public Snake(int width, int height) : this((byte)width, (byte)height) { }

        public Snake(Settings settings) {
            this.settings = settings;
            this.map = new byte[width, height];
            LoadStart();
        }

        public Snake(byte width, byte height, Settings settings) {
            this.settings = settings;
            this.width = width;
            this.height = height;
            this.map = new byte[width, height];
            LoadStart();
        }

        public Snake(int width, int height, Settings settings) : this((byte)width, (byte)height, settings) { }

        private void LoadStart() {
            var center = new Vector2Byte(width / 2, height / 2);
            snake.Add(center);
            grow = (byte)(settings.StartLength - 1);
        }

        #endregion

        #region Update

        public void Tick() {
            if(IsDead || HasWon)
                return;

            var head = snake[0];
            var nextHead = head;

            switch(direction) {
                case 0: // Up
                    nextHead.y--;
                    if(nextHead.y == 255) {
                        if(settings.LoopingWalls) {
                            nextHead.y = (byte)(height - 1);
                        }
                        else {
                            safetyFrames++;
                            return;
                        }
                    }
                    break;

                case 1: // Down
                    nextHead.y++;
                    if(nextHead.y >= height) {
                        if(settings.LoopingWalls) {
                            nextHead.y = 0;
                        }
                        else {
                            safetyFrames++;
                            return;
                        }
                    }
                    break;

                case 2: // Left
                    nextHead.x--;
                    if(nextHead.x == 255) {
                        if(settings.LoopingWalls) {
                            nextHead.x = (byte)(width - 1);
                        }
                        else {
                            safetyFrames++;
                            return;
                        }
                    }
                    break;

                case 3: // Right
                    nextHead.x++;
                    if(nextHead.x >= width) {
                        if(settings.LoopingWalls) {
                            nextHead.x = 0;
                        }
                        else {
                            safetyFrames++;
                            return;
                        }
                    }
                    break;
            }

            // Collision Check if there is a body.
            var mapAtHead = map[nextHead.x, nextHead.y];
            if(mapAtHead == 1) {
                safetyFrames++;
                return;
            }

            // Check if there is any food to eat
            if(mapAtHead == 2 && food.Remove(nextHead)) {
                grow += settings.GrowthPerFood;
            }

            // Override the map collision for the new head and set safety frame back to 0
            map[nextHead.x, nextHead.y] = 1;
            safetyFrames = 0;

            // Move the snake
            var snakeLength = snake.Count - 1;
            if(grow > 0) {
                snake.Add(snake[snakeLength]);
                grow--;
            }
            else {
                var tail = snake[snakeLength];
                map[tail.x, tail.y] = 0;
            }

            for(int i = snakeLength; i >= 1; i--) {
                snake[i] = snake[i - 1];
            }
            snake[0] = nextHead;

            // Add food on the map
            if(food.Count < settings.FoodAtAnyTime && snake.Count < width * height - settings.FoodAtAnyTime && !HasWon) {
                Vector2Byte foodPosition = Vector2Byte.zero;
                do {
                    foodPosition.x = (byte)random.Next(0, width);
                    foodPosition.y = (byte)random.Next(0, height);
                } while(map[foodPosition.x, foodPosition.y] != 0);
                map[foodPosition.x, foodPosition.y] = 2;
                food.Add(foodPosition);
            }
        }

        #endregion

        #region Utility

        public void Restart() {
            snake.Clear();
            food.Clear();
            safetyFrames = 0;
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    map[x, y] = 0;
                }
            }
            LoadStart();
        }


        public void Resize(int width, int height)
            => Resize((byte)width, (byte)height);

        public void Resize(byte width, byte height) {
            if(this.width == width && this.height == height) {
                return;
            }
            var wLength = Mathf.Min(this.width, width);
            var hLength = Mathf.Min(this.height, height);
            this.width = width;
            this.height = height;
            var copy = new byte[width, height];
            for(int x = 0; x < wLength; x++) {
                for(int y = 0; y < hLength; y++) {
                    copy[x, y] = map[x, y];
                }
            }
            map = copy;
        }

        #endregion
    }
}
