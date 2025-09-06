
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.Toxel {
    [System.Serializable]
    public struct EdgeMask {
        #region Variables

        public int[] Edges;

        #endregion

        #region Constructor

        public EdgeMask(int[] edges) {
            this.Edges = edges;
        }

        #endregion

        #region Rotation

        public EdgeMask RotateYClockwise() {
            return new EdgeMask(Edges.Select(RotateYClockwise).ToArray());
        }

        public EdgeMask RotateYCounterClockwise() {
            return new EdgeMask(Edges.Select(RotateYCounterClockwise).ToArray());
        }

        public EdgeMask RotateXClockwise() {
            return new EdgeMask(Edges.Select(RotateXClockwise).ToArray());
        }

        public EdgeMask RotateXCounterClockwise() {
            return new EdgeMask(Edges.Select(RotateXCounterClockwise).ToArray());
        }

        public EdgeMask RotateZClockwise() {
            return new EdgeMask(Edges.Select(RotateZClockwise).ToArray());
        }

        public EdgeMask RotateZCounterClockwise() {
            return new EdgeMask(Edges.Select(RotateZCounterClockwise).ToArray());
        }

        #endregion

        #region Rotate Static Input->output

        public static int RotateYCounterClockwise(int input) {
            switch(input) {
                case 0: return 3;
                case 3: return 2;
                case 2: return 1;
                case 1: return 0;

                case 8: return 9;
                case 9: return 11;
                case 11: return 10;
                case 10: return 8;

                case 4: return 7;
                case 7: return 6;
                case 6: return 5;
                case 5: return 4;
            }
            return input;
        }

        public static int RotateYClockwise(int input) {
            switch(input) {
                case 0: return 1;
                case 3: return 0;
                case 2: return 3;
                case 1: return 2;

                case 8: return 10;
                case 9: return 8;
                case 11: return 9;
                case 10: return 11;

                case 4: return 5;
                case 7: return 4;
                case 6: return 7;
                case 5: return 6;
            }
            return input;
        }

        public static int RotateXClockwise(int input) {
            switch(input) {
                case 4: return 8;
                case 8: return 0;
                case 0: return 9;
                case 9: return 4;

                case 3: return 7;
                case 7: return 5;
                case 5: return 1;
                case 1: return 3;

                case 11: return 6;
                case 6: return 10;
                case 10: return 2;
                case 2: return 11;
            }
            return input;
        }

        public static int RotateXCounterClockwise(int input) {
            switch(input) {
                case 4: return 9;
                case 8: return 4;
                case 0: return 8;
                case 9: return 0;

                case 3: return 1;
                case 7: return 3;
                case 5: return 7;
                case 1: return 5;

                case 11: return 2;
                case 6: return 11;
                case 10: return 6;
                case 2: return 10;
            }
            return input;
        }

        public static int RotateZClockwise(int input) {
            switch(input) {
                case 1: return 8;
                case 8: return 5;
                case 5: return 10;
                case 10: return 1;

                case 4: return 6;
                case 6: return 2;
                case 2: return 0;
                case 0: return 4;

                case 9: return 7;
                case 7: return 11;
                case 11: return 3;
                case 3: return 9;
            }
            return input;
        }

        public static int RotateZCounterClockwise(int input) {
            switch(input) {
                case 1: return 10;
                case 8: return 1;
                case 5: return 8;
                case 10: return 5;

                case 4: return 0;
                case 6: return 4;
                case 2: return 6;
                case 0: return 2;

                case 9: return 3;
                case 7: return 9;
                case 11: return 7;
                case 3: return 11;
            }
            return input;
        }

        #endregion
    }
}
