using UnityEngine;

namespace Physics {
    public struct Line {

        public Vector3 p0, p1;

        public Vector3 v => p1 - p0;

        public void GetDistance(Line other, out float distance) {
            Vector3 uS = new Vector3 { x = this.p0.x, y = this.p0.y, z = this.p0.z };
            Vector3 uE = new Vector3 { x = this.p1.x, y = this.p1.y, z = this.p1.z };
            Vector3 vS = new Vector3 { x = other.p0.x, y = other.p0.y, z = other.p0.z };
            Vector3 vE = new Vector3 { x = other.p1.x, y = other.p1.y, z = other.p1.z };
            Vector3 w1 = new Vector3 { x = this.p0.x, y = this.p0.y, z = this.p0.z };
            Vector3 w2 = new Vector3 { x = other.p0.x, y = other.p0.y, z = other.p0.z };
            Vector3 u = uE - uS;
            Vector3 v = vE - vS;
            Vector3 w = w1 - w2;
            float a = Vector3.Dot(u, u);
            float b = Vector3.Dot(u, v);
            float c = Vector3.Dot(v, v);
            float d = Vector3.Dot(u, w);
            float e = Vector3.Dot(v, w);
            float D = a * c - b * b;
            float sc, sN, sD = D;
            float tc, tN, tD = D;
            if (D < 0.01) {
                sN = 0;
                sD = 1;
                tN = e;
                tD = c;
            }
            else {
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < 0) {
                    sN = 0;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD) {
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }
            if (tN < 0) {
                tN = 0;
                if (-d < 0) {
                    sN = 0;
                }
                else if (-d > a) {
                    sN = sD;
                }
                else {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD) {
                tN = tD;
                if ((-d + b) < 0) {
                    sN = 0;
                }
                else if ((-d + b) > a) {
                    sN = sD;
                }
                else {
                    sN = (-d + b);
                    sD = a;
                }
            }
            if (Mathf.Abs(sN) < 0.01) {
                sc = 0;
            }
            else {
                sc = sN / sD;
            }
            if (Mathf.Abs(tN) < 0.01) {
                tc = 0;
            }
            else {
                tc = tN / tD;
            }
            Vector3 dP = w + (sc * u) - (tc * v);
            float distance1 = Mathf.Sqrt(Vector3.Dot(dP, dP));
            distance = distance1;
        }

        public bool GetNearestDistance(Vector3 point, out float distance, out Vector3 pointOnLine) {
            Vector3 dir = p1 - p0;
            float lengthA = dir.magnitude;
            float inverseLengthA = 1.0f / lengthA;
            float dot = Vector3.Dot(dir, point - p0);
            float projLen = dot * inverseLengthA;
            pointOnLine = p0 + projLen * dir * inverseLengthA;
            distance = (pointOnLine - point).magnitude;
            return projLen <= lengthA && projLen >= 0;
        }

    }
}
