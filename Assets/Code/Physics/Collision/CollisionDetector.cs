using System.Collections.Generic;
using UnityEngine;

namespace Physics {
    public static class CollisionDetector {

        public static int BoxAndBox(CollisionData data, ref Box one, ref Box two) {
            Vector3 toCentre = two.GetAxis(3) - one.GetAxis(3);

            float pen = float.MaxValue;
            int best = 0xffffff;

            if (!_tryAxis(ref one, ref two, one.GetAxis(0), toCentre, 0, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, one.GetAxis(1), toCentre, 1, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, one.GetAxis(2), toCentre, 2, ref pen, ref best))
                return 0;

            if (!_tryAxis(ref one, ref two, two.GetAxis(0), toCentre, 3, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, two.GetAxis(1), toCentre, 4, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, two.GetAxis(2), toCentre, 5, ref pen, ref best))
                return 0;

            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(0), two.GetAxis(0)), toCentre, 6, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(0), two.GetAxis(1)), toCentre, 7, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(0), two.GetAxis(2)), toCentre, 8, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(1), two.GetAxis(0)), toCentre, 9, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(1), two.GetAxis(1)), toCentre, 10, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(1), two.GetAxis(2)), toCentre, 11, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(2), two.GetAxis(0)), toCentre, 12, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(2), two.GetAxis(1)), toCentre, 13, ref pen, ref best))
                return 0;
            if (!_tryAxis(ref one, ref two, Vector3.Cross(one.GetAxis(2), two.GetAxis(2)), toCentre, 14, ref pen, ref best))
                return 0;

            Debug.Assert(best != 0xffffff);

            best -= 6;
            int oneAxisIndex = best / 3;
            int twoAxisIndex = best % 3;

            Vector3 minPoint, normal;
            Contact contact = default;
            if (_pointBoxTest(ref one, ref two, out minPoint, out normal, out pen) ||
                _edgePlaneTest(ref one, ref two, twoAxisIndex, out minPoint, out normal, out pen)) {
                contact.SetBodyData(one.body, two.body, data.friction, data.restitution);
                contact.contactNormal = normal;
            }
            else if (_pointBoxTest(ref two, ref one, out minPoint, out normal, out pen) ||
                _edgePlaneTest(ref two, ref one, oneAxisIndex, out minPoint, out normal, out pen)) {
                contact.SetBodyData(two.body, one.body, data.friction, data.restitution);
                contact.contactNormal = -normal;
            }
            else {
                return 0;
            }

            contact.penetration = pen;
            contact.contactPoint = minPoint;
            data.AddContact(contact);

            return 1;

        }

        #region Internal Functions

        static List<Line> lines = new List<Line>();
        static List<Plane> planes = new List<Plane>();
        static List<Vector3> vertices = new List<Vector3>();
        private static bool _pointBoxTest(ref Box one, ref Box two, out Vector3 minPoint, out Vector3 normal, out float pen) {
            vertices.Clear();
            two.GetVertices(vertices);

            foreach (var vertex in vertices) {
                if (one.Inside(vertex)) {
                    minPoint = vertex;
                    _determineNormal(ref one, two.GetAxis(3) - one.GetAxis(3), minPoint, out normal, out pen);
                    return true;
                }
            }
            pen = 0;
            minPoint = normal = default;
            return false;
        }

        private static bool _edgePlaneTest(ref Box one, ref Box two, int twoAxisIndex, out Vector3 minPoint, out Vector3 normal, out float pen) {
            planes.Clear();
            lines.Clear();
            one.GetPlane(planes);
            two.GetEdgeParallelToAxis(0, lines);
            two.GetEdgeParallelToAxis(1, lines);
            two.GetEdgeParallelToAxis(2, lines);

            foreach (var plane in planes) {
                foreach (var edge in lines) {
                    if (plane.ComputeLineIntersectWithPlaneVolume(edge, out float t, out minPoint) == Plane.LineIntersectType.InSegment) {
                        _determineNormal(ref one, two.GetAxis(3) - one.GetAxis(3), minPoint, out normal, out pen);
                        return true;
                    }
                }
            }
            pen = 0;
            minPoint = normal = default;
            return false;
        }

        static List<Plane> determineNormalPlanes = new List<Plane>();
        private static void _determineNormal(ref Box box, Vector3 toCenter, Vector3 minPoint, out Vector3 normal, out float pen) {
            float max = 0, sign = 1.0f;
            int maxIndex = 0;
            pen = default;
            for (int i = 0; i < 3; ++i) {
                float dist = Vector3.Dot(box.GetAxis(i), toCenter);
                if (Mathf.Abs(dist) > max) {
                    max = Mathf.Abs(dist);
                    sign = Mathf.Sign(dist);
                    maxIndex = i;
                }
            }

            normal = box.GetAxis(maxIndex) * sign;

            Line line = new Line() {
                p0 = minPoint,
                p1 = minPoint + normal
            };
            determineNormalPlanes.Clear();
            box.GetPlanePerpendicularToAxis(maxIndex, determineNormalPlanes);
            for (int i = 0; i < determineNormalPlanes.Count; ++i) {
                if (Vector3.Dot(determineNormalPlanes[i].normal, normal) > 0) {
                    determineNormalPlanes[i].ComputeLineIntersect(line, out pen, out _);
                    break;
                }
            }
        }

        private static bool _tryAxis(ref Box one, ref Box two, Vector3 axis, Vector3 toCentre, int index, ref float smallestPenetration, ref int smallestCase) {
            if (axis == Vector3.zero)
                return true;
            float penetration = _penetrationOnAxis(ref one, ref two, axis, toCentre);
            if (penetration < 0)
                return false;
            if (penetration <= smallestPenetration) {
                smallestPenetration = penetration;
                smallestCase = index;
            }
            return true;
        }

        private static float _penetrationOnAxis(ref Box one, ref Box two, Vector3 axis, Vector3 toCentre) {
            float oneProject = _transformToAxis(ref one, axis);
            float twoProject = _transformToAxis(ref two, axis);

            float distance = Mathf.Abs(Vector3.Dot(toCentre, axis));

            return oneProject + twoProject - distance;
        }

        private static float _transformToAxis(ref Box box, Vector3 axis) {
            return
                box.halfSize.x * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(0))) +
                box.halfSize.y * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(1))) +
                box.halfSize.z * Mathf.Abs(Vector3.Dot(axis, box.GetAxis(2)));
        }

        #endregion

    }
}
