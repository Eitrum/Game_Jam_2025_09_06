using System;
using UnityEngine;

namespace Toolkit
{
    public struct Matrix3x3
    {
        #region Variables

        public float m00;
        public float m01;
        public float m02;

        public float m10;
        public float m11;
        public float m12;

        public float m20;
        public float m21;
        public float m22;

        #endregion

        #region Properties

        public unsafe float this[int index] {
            get {
                if(index < 0 || index >= 12)
                    throw new IndexOutOfRangeException();
                fixed(float* v = &m00) {
                    return *(v + index);
                }
            }
            set {
                if(index < 0 || index >= 12)
                    throw new IndexOutOfRangeException();
                fixed(float* v = &m00) {
                    *(v + index) = value;
                }
            }
        }

        public Vector3 Row0 => new Vector3(m00, m01, m02);
        public Vector3 Row1 => new Vector3(m10, m11, m12);
        public Vector3 Row2 => new Vector3(m20, m21, m22);

        public Vector3 Col0 => new Vector3(m00, m10, m20);
        public Vector3 Col1 => new Vector3(m01, m11, m21);
        public Vector3 Col2 => new Vector3(m02, m12, m22);

        #endregion

        #region Constructor

        public Matrix3x3(Vector3 v0, Vector3 v1, Vector3 v2) {
            m00 = v0.x;
            m01 = v0.y;
            m02 = v0.z;

            m10 = v1.x;
            m11 = v1.y;
            m12 = v1.z;

            m20 = v2.x;
            m21 = v2.y;
            m22 = v2.z;
        }

        public Matrix3x3(float m00,
            float m01,
            float m02,
            float m10,
            float m11,
            float m12,
            float m20,
            float m21,
            float m22) {

            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;

            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;

            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
        }

        #endregion
    }
}
