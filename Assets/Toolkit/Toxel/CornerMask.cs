using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Toxel {
    [System.Serializable]
    public struct CornerMask {

        #region Variables

        public bool _000;
        public bool _001;
        public bool _100;
        public bool _101;

        public bool _010;
        public bool _011;
        public bool _110;
        public bool _111;

        #endregion

        #region Properties

        public int Mask {
            get {
                int mask = 0;
                // bot
                if(_000) mask |= 1;
                if(_001) mask |= 2;
                if(_100) mask |= 4;
                if(_101) mask |= 8;

                // Top
                if(_010) mask |= 16;
                if(_011) mask |= 32;
                if(_110) mask |= 64;
                if(_111) mask |= 128;

                return mask;
            }
        }

        #endregion

        #region Constructor

        public CornerMask(int mask) {
            _000 = mask.HasFlag(1);
            _001 = mask.HasFlag(2);
            _100 = mask.HasFlag(4);
            _101 = mask.HasFlag(8);

            _010 = mask.HasFlag(16);
            _011 = mask.HasFlag(32);
            _110 = mask.HasFlag(64);
            _111 = mask.HasFlag(128);
        }

        public CornerMask(
            bool _000, bool _001, bool _100, bool _101,
            bool _010, bool _011, bool _110, bool _111) {
            this._000 = _000;
            this._001 = _001;
            this._100 = _100;
            this._101 = _101;

            this._010 = _010;
            this._011 = _011;
            this._110 = _110;
            this._111 = _111;
        }

        #endregion

        #region Rotate

        public CornerMask RotateYClockwise() {
            return new CornerMask(_001, _101, _000, _100, _011, _111, _010, _110);
        }

        public CornerMask RotateYCounterClockwise() {
            return new CornerMask(_100, _000, _101, _001, _110, _010, _111, _011);
        }

        public CornerMask RotateXClockwise() {
            return new CornerMask(_010, _000, _110, _100, _011, _001, _111, _101);
        }

        public CornerMask RotateXCounterClockwise() {
            return new CornerMask(_001, _011, _101, _111, _000, _010, _100, _110);
        }

        public CornerMask RotateZClockwise() {
            return new CornerMask(_100, _101, _110, _111, _000, _001, _010, _011);
        }

        public CornerMask RotateZCounterClockwise() {
            return new CornerMask(_010, _011, _000, _001, _110, _111, _100, _101);
        }

        #endregion
    }
}
