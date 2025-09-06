using System;
using UnityEngine;

namespace Toolkit.MiniGames
{
    public class Minesweeper
    {
        #region Consts

        private const byte BOMB = 128;
        private const byte NON_BOMB_MASK = 127;
        private const byte FLAG = 64;
        private const byte NON_FLAG_MASK = 191;
        private const byte EMPTY = 16;

        #endregion

        #region Enums

        public enum State : byte
        {
            // Unvisited
            Unvisited = 0,
            // Cached Nearby bombs
            Cached1 = 1,
            Cached2 = 2,
            Cached3 = 3,
            Cached4 = 4,
            Cached5 = 5,
            Cached6 = 6,
            Cached7 = 7,
            Cached8 = 8,

            // Visisted empty
            Empty = EMPTY,
            // Markings
            Flag = FLAG,
            Bomb = BOMB,
        }

        public enum Action : byte
        {
            None,
            Sweep,
            Flag
        }

        #endregion

        #region Variables

        private byte width = 10;
        private byte height = 10;
        private System.Random random = new System.Random();

        private short mines = 10;
        private short placedMines = 0;
        private bool dead = false;
        private bool solved = false;
        private int flags = 0;

        private byte[,] playfield;

        #endregion

        #region Properties

        public byte Width => width;
        public byte Height => height;

        public short Mines {
            get => mines;
            set => SetMines(value);
        }
        public short PlacedMines => placedMines;
        public bool IsDead => dead;
        public bool IsSolved => solved;
        public bool IsGenerated => playfield != null;
        public bool IsDone => (dead || solved) && IsGenerated;
        public int Flags => flags;

        public State this[int x, int y] {
            get {
                if(playfield == null)
                    return State.Unvisited;
                if(dead)
                    return (State)playfield[x, y];
                return (State)(playfield[x, y] & NON_BOMB_MASK);
            }
        }

        #endregion

        #region Constructor

        public Minesweeper() { }

        public Minesweeper(byte width, byte height, short mines) {
            this.width = width;
            this.height = height;
            this.mines = mines;
        }

        public Minesweeper(int width, int height, short mines) {
            this.width = (byte)width;
            this.height = (byte)height;
            this.mines = mines;
        }

        #endregion

        #region Public Methods

        public void Resize(int width, int height) {
            this.width = (byte)width;
            this.height = (byte)height;
            Restart();
        }

        public void Resize(int width, int height, short mines) {
            this.width = (byte)width;
            this.height = (byte)height;
            this.mines = mines;
            Restart();
        }

        public void SetMines(short mines) {
            this.mines = Math.Max((short)1, mines);
            Restart();
        }

        public void Restart() {
            playfield = null;
            dead = false;
            solved = false;
            flags = 0;
        }

        #endregion

        #region Actions

        public void DoAction(int x, int y, Action action) {
            if(dead || solved)
                return;
            switch(action) {
                case Action.Flag:
                    ToggleFlag(x, y);
                    break;
                case Action.Sweep:
                    Sweep(x, y);
                    break;
            }

            if(IsSolvedCheck()) {
                solved = true;
            }
        }

        public void ToggleFlag(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return;

            var val = playfield[x, y];
            if((val != 0 && val <= 8) || val == EMPTY) {
                return;
            }

            if(HasFlag(val, FLAG)) {
                playfield[x, y] = (byte)(val - FLAG);
                flags--;
            }
            else {
                playfield[x, y] = (byte)(val + FLAG);
                flags++;
            }
        }

        public void Sweep(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return;
            VerifyGenerated(x, y);
            var val = playfield[x, y];
            if(HasFlag(val, FLAG)) {
                return;
            }
            if(HasFlag(val, BOMB)) {
                dead = true;
                return;
            }
            else if(val == 0) {
                SweepRecursive(x, y);
            }
            else if(val <= 8 && HasFlagsNearby(x, y, val)) {
                ClearNonFlagNearby(x, y);
            }
        }

        #endregion

        #region Private Methods

        private bool IsSolvedCheck() {
            if(playfield == null)
                return false;
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    if(playfield[x, y] == 0 || playfield[x, y] == FLAG)
                        return false;
                }
            }
            return true;
        }

        private bool HasFlagsNearby(int x, int y, int flags) {
            return flags ==
                (IsFlagInt(x - 1, y + 1) +
                IsFlagInt(x - 1, y) +
                IsFlagInt(x - 1, y - 1) +
                IsFlagInt(x + 1, y + 1) +
                IsFlagInt(x + 1, y) +
                IsFlagInt(x + 1, y - 1) +
                IsFlagInt(x, y + 1) +
                IsFlagInt(x, y - 1));
        }

        private int IsFlagInt(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return 0;
            return HasFlag(playfield[x, y], FLAG) ? 1 : 0;
        }

        private bool IsFlag(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return false;
            return HasFlag(playfield[x, y], FLAG);
        }

        private void SweepIfNotFlag(int x, int y) {
            if(IsFlag(x, y))
                return;
            if(IsMine(x, y)) {
                dead = true;
                return;
            }
            else
                SweepRecursive(x, y);

        }

        private void ClearNonFlagNearby(int x, int y) {
            SweepIfNotFlag(x - 1, y + 1);
            SweepIfNotFlag(x - 1, y);
            SweepIfNotFlag(x - 1, y - 1);
            SweepIfNotFlag(x + 1, y + 1);
            SweepIfNotFlag(x + 1, y);
            SweepIfNotFlag(x + 1, y - 1);
            SweepIfNotFlag(x, y + 1);
            SweepIfNotFlag(x, y - 1);
        }

        private void SweepRecursive(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return;

            var val = playfield[x, y];
            if(val == 0) {
                var minesNearby = NearbyMines(x, y);
                if(minesNearby == 0) {
                    playfield[x, y] = EMPTY;
                    SweepRecursive(x - 1, y + 1);
                    SweepRecursive(x - 1, y);
                    SweepRecursive(x - 1, y - 1);
                    SweepRecursive(x + 1, y + 1);
                    SweepRecursive(x + 1, y);
                    SweepRecursive(x + 1, y - 1);
                    SweepRecursive(x, y + 1);
                    SweepRecursive(x, y - 1);
                }
                else {
                    playfield[x, y] = (byte)minesNearby;
                }
            }
        }

        private int NearbyMines(int x, int y) {
            return
            IsMineInt(x - 1, y + 1) +
            IsMineInt(x - 1, y) +
            IsMineInt(x - 1, y - 1) +
            IsMineInt(x + 1, y + 1) +
            IsMineInt(x + 1, y) +
            IsMineInt(x + 1, y - 1) +
            IsMineInt(x, y + 1) +
            IsMineInt(x, y - 1);
        }

        private int IsMineInt(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return 0;
            return HasFlag(playfield[x, y], BOMB) ? 1 : 0;
        }

        private bool IsMine(int x, int y) {
            if(x < 0 || y < 0 || x >= width || y >= height)
                return false;
            return HasFlag(playfield[x, y], BOMB);
        }

        private void VerifyGenerated(int x, int y) {
            if(playfield == null) {
                playfield = new byte[width, height];
                var minesLeft = mines;
                var pos = new Vector2Byte(x, y);
                int attempts = 0;
                do {
                    var xPos = random.Next(0, width);
                    var yPos = random.Next(0, height);
                    if(Vector2Byte.Distance(new Vector2Byte(xPos, yPos), pos) < 2 || playfield[xPos, yPos] == BOMB) {
                        attempts++;
                        if(attempts > 1000) {
                            Debug.LogWarning("Too many attempts at generating a bomb at random position!");
                            break;
                        }
                        continue;
                    }
                    else {
                        if(playfield[x, y] != 0) {
                            throw new Exception("Should not ever be the case as this is in generation!");
                        }
                        playfield[xPos, yPos] = BOMB;
                        minesLeft--;
                        attempts = 0;
                    }
                } while(minesLeft > 0);
                placedMines = (short)((mines) - (minesLeft));
            }
        }

        private static bool HasFlag(byte b, byte val) {
            return (b & val) == val;
        }

        #endregion
    }
}
