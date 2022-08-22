using System.Collections.Generic;
using UnityEngine;

namespace Physics {
    public struct Box {

        public Matrix4x4 transform;

        public Vector3 halfSize;

        public Rigidbody body;

        public Vector3 GetAxis(int columnIndex) {
            return transform.GetColumn(columnIndex);
        }

        public void GetVertices(List<Vector3> list) {
            list.Add(transform.MultiplyPoint(new Vector3(-halfSize.x, halfSize.y, -halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(halfSize.x, halfSize.y, -halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(halfSize.x, halfSize.y, halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(-halfSize.x, halfSize.y, halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(halfSize.x, -halfSize.y, -halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(halfSize.x, -halfSize.y, halfSize.z)));
            list.Add(transform.MultiplyPoint(new Vector3(-halfSize.x, -halfSize.y, halfSize.z)));
        }

        public void GetPlane(List<Plane> list) {
            GetPlanePerpendicularToAxis(0, list);
            GetPlanePerpendicularToAxis(1, list);
            GetPlanePerpendicularToAxis(2, list);
        }

        public void GetPlanePerpendicularToAxis(int axisIndex, List<Plane> list) {
            switch (axisIndex) {
                case 0:
                    var v3 = transform.MultiplyPoint(new Vector3(-halfSize.x, 0, 0));
                    var v4 = transform.MultiplyPoint(new Vector3(halfSize.x, 0, 0));
                    Plane left = new Plane();
                    left.center = v3;
                    left.normal = -transform.GetColumn(0);
                    left.right = transform.GetColumn(2);
                    left.halfSize = new Vector2(halfSize.z, halfSize.y);
                    Plane right = new Plane();
                    right.center = v4;
                    right.normal = transform.GetColumn(0);
                    right.right = -transform.GetColumn(2);
                    right.halfSize = new Vector2(halfSize.z, halfSize.y);
                    list.Add(left);
                    list.Add(right);
                    break;
                case 1:
                    var v5 = transform.MultiplyPoint(new Vector3(0, -halfSize.y, 0));
                    var v6 = transform.MultiplyPoint(new Vector3(0, halfSize.y, 0));
                    Plane bottom = new Plane();
                    bottom.center = v5;
                    bottom.normal = -transform.GetColumn(1);
                    bottom.right = -transform.GetColumn(0);
                    bottom.halfSize = new Vector2(halfSize.x, halfSize.z);
                    Plane top = new Plane();
                    top.center = v6;
                    top.normal = transform.GetColumn(1);
                    top.right = transform.GetColumn(0);
                    top.halfSize = new Vector2(halfSize.x, halfSize.z);
                    list.Add(bottom);
                    list.Add(top);
                    break;
                case 2:
                    var v1 = transform.MultiplyPoint(new Vector3(0, 0, -halfSize.z));
                    var v2 = transform.MultiplyPoint(new Vector3(0, 0, halfSize.z));
                    Plane front = new Plane();
                    front.center = v1;
                    front.normal = -transform.GetColumn(2);
                    front.right = -transform.GetColumn(0);
                    front.halfSize = new Vector2(halfSize.x, halfSize.y);
                    Plane back = new Plane();
                    back.center = v2;
                    back.normal = transform.GetColumn(2);
                    back.right = transform.GetColumn(0);
                    back.halfSize = new Vector2(halfSize.x, halfSize.y);
                    list.Add(front);
                    list.Add(back);
                    break;
            }
        }

        public void GetEdgeParallelToAxis(int axisIndex, List<Line> list) {
            Line line0, line1, line2, line3; 
            Vector3 v0, v1, v2, v3, v4, v5, v6, v7;
            v0 = transform.MultiplyPoint(new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
            v1 = transform.MultiplyPoint(new Vector3(halfSize.x, halfSize.y, -halfSize.z));
            v2 = transform.MultiplyPoint(new Vector3(halfSize.x, halfSize.y, halfSize.z));
            v3 = transform.MultiplyPoint(new Vector3(-halfSize.x, halfSize.y, halfSize.z));
            v4 = transform.MultiplyPoint(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
            v5 = transform.MultiplyPoint(new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
            v6 = transform.MultiplyPoint(new Vector3(halfSize.x, -halfSize.y, halfSize.z));
            v7 = transform.MultiplyPoint(new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
            switch (axisIndex) {
                case 0:
                    line0.p0 = v4;
                    line0.p1 = v5;
                    line1.p0 = v7;
                    line1.p1 = v6;
                    line2.p0 = v3;
                    line2.p1 = v2;
                    line3.p0 = v0;
                    line3.p1 = v1;
                    break;
                case 1:
                    line0.p0 = v4;
                    line0.p1 = v0;
                    line1.p0 = v5;
                    line1.p1 = v1;
                    line2.p0 = v6;
                    line2.p1 = v2;
                    line3.p0 = v7;
                    line3.p1 = v3;
                    break;
                case 2:
                    line0.p0 = v4;
                    line0.p1 = v7;
                    line1.p0 = v5;
                    line1.p1 = v6;
                    line2.p0 = v1;
                    line2.p1 = v2;
                    line3.p0 = v0;
                    line3.p1 = v3;
                    break;
                default:
                    line0 = line1 = line2 = line3 = default;
                    break;
            }
            list.Add(line0);
            list.Add(line1);
            list.Add(line2);
            list.Add(line3);
        }

        public bool Inside(Vector3 point) {
            point = transform.inverse.MultiplyPoint(point);
            return Mathf.Abs(point.x) < halfSize.x && Mathf.Abs(point.y) < halfSize.y && Mathf.Abs(point.z) < halfSize.z;
        }

    }
}
