using UnityEngine;

namespace Physics {
    public struct Plane {

        public enum LineIntersectType {
            NoIntersect,
            InSegment,
            OutSegment,
            Everywhere,
        }

        public Vector3 center;

        public Vector3 normal;

        public Vector3 right;

        public Vector2 halfSize;

        public float ComputePointInWhichHalfSpace(Vector3 pt) {
            float hs = normal.x * (pt.x - center.x) +
                       normal.y * (pt.y - center.y) +
                       normal.z * (pt.z - center.z);

            return hs;
        }

        public LineIntersectType ComputeLineIntersect(Line pline, out float t, out Vector3 pt) {
            t = 0;
            pt = default;
            float plane_dot_line = Vector3.Dot(pline.v, normal);

            if (Mathf.Abs(plane_dot_line) <= float.Epsilon) {
                if (Mathf.Abs(ComputePointInWhichHalfSpace(pline.p0)) <= float.Epsilon)
                    return LineIntersectType.Everywhere;
                else
                    return LineIntersectType.NoIntersect;
            }

            t = -(normal.x * pline.p0.x +
                   normal.y * pline.p0.y +
                   normal.z * pline.p0.z -
                   normal.x * center.x -
                   normal.y * center.y -
                   normal.z * center.z) / (plane_dot_line);

            pt.x = pline.p0.x + pline.v.x * t;
            pt.y = pline.p0.y + pline.v.y * t;
            pt.z = pline.p0.z + pline.v.z * t;

            if (t >= 0.0 && t <= 1.0)
                return LineIntersectType.InSegment;
            else
                return LineIntersectType.OutSegment;
        }

        public LineIntersectType ComputeLineIntersectWithPlaneVolume(Line pline, out float t, out Vector3 pt) {
            var type = ComputeLineIntersect(pline, out t, out pt);
            if (type == LineIntersectType.InSegment) {
                Vector3 forward = Vector3.Cross(normal, right).normalized;
                float dist = Vector3.Dot(right, pt - center);
                if (Mathf.Abs(dist) > halfSize.x) {
                    return LineIntersectType.NoIntersect;
                }
                dist = Vector3.Dot(forward, pt - center);
                if (Mathf.Abs(dist) > halfSize.y) {
                    return LineIntersectType.NoIntersect;
                }
            }
            return type;
        }
    }
}
