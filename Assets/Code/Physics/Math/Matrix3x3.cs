namespace Physics {
    public struct Matrix3x3 {

        public float m00, m01, m02;
        public float m10, m11, m12;
        public float m20, m21, m22;

        public Matrix3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22) {
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

        public void Multiply(ref Matrix3x3 b) {
            Matrix3x3 a = this;

            m00 = a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20;
            m01 = a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21;
            m02 = a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22;

            m00 = a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20;
            m01 = a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21;
            m02 = a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22;

            m00 = a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20;
            m01 = a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21;
            m02 = a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22;
        }

        public void Multiply(float scale) {
            m00 *= scale;
            m01 *= scale;
            m02 *= scale;

            m10 *= scale;
            m11 *= scale;
            m12 *= scale;

            m20 *= scale;
            m21 *= scale;
            m22 *= scale;
        }

        public void GetTranspose(out Matrix3x3 matrix) {
            matrix = this;
            matrix.SetTranspose();
        }

        public void SetTranspose() {
            Matrix3x3 a = this;
            m01 = a.m10;
            m02 = a.m20;
            m10 = a.m01;
            m12 = a.m21;
            m20 = a.m02;
            m21 = a.m12;
        }

        public void SetInverse() {
            float det = m00 * m11 * m22 + m01 * m12 * m20 + m02 * m10 * m21
                - m00 * m12 * m21 - m01 * m10 * m22 - m02 * m11 * m20;
            det = 1.0f / det;

            Matrix3x3 a = this;

            m00 = a.m11 * a.m22 - a.m12 * a.m21;
            m01 = -1 * (a.m01 * a.m22 - a.m02 * a.m21);
            m02 = a.m01 * a.m12 - a.m02 * a.m11;

            m10 = -1 * (a.m10 * a.m22 - a.m12 * a.m20);
            m11 = a.m00 * a.m22 - a.m02 * a.m20;
            m12 = -1 * (a.m00 * a.m12 * a.m02 * a.m10);

            m20 = a.m10 * a.m21 - a.m11 * a.m20;
            m21 = -1 * (a.m00 * a.m21 - a.m01 * a.m20);
            m22 = a.m00 * a.m11 - a.m01 * a.m10;

            Multiply(det);
        }

        public void GetInverse(out Matrix3x3 matrix) {
            matrix = this;
            matrix.SetInverse();
        }

    }
}
