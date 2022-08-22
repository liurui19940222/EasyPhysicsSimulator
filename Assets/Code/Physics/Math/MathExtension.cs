using UnityEngine;

namespace Physics {
    public static class MathExtension {
    
        public static Matrix3x3 GetBasis(this Matrix4x4 matrix) {
            return new Matrix3x3(
                matrix.m00, matrix.m01, matrix.m02, 
                matrix.m10, matrix.m11, matrix.m12,
                matrix.m20, matrix.m21, matrix.m22);
        }

        public static void Transform(this Matrix3x3 matrix, Vector3 point, out Vector3 outPoint) {
            outPoint.x = matrix.m00 * point.x + matrix.m01 * point.y + matrix.m02 * point.z;
            outPoint.y = matrix.m10 * point.x + matrix.m11 * point.y + matrix.m12 * point.z;
            outPoint.z = matrix.m20 * point.x + matrix.m21 * point.y + matrix.m22 * point.z;
        }

        public static Quaternion AddScaledVector(this Quaternion rotation, Vector3 vector, float scale) {
            Quaternion q = new Quaternion(vector.x * scale, vector.y * scale, vector.z * scale, 0);
            q *= rotation;
            rotation.w += q.w * 0.5f;
            rotation.x += q.x * 0.5f;
            rotation.y += q.y * 0.5f;
            rotation.z += q.z * 0.5f;
            return rotation;
        }

    }
}
