using Physics;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public Transform transformOne;
    public Transform transformTwo;
    public Transform transformLineA_p0;
    public Transform transformLineA_p1;
    public Transform transformLineB_p0;
    public Transform transformLineB_p1;
    public Transform transformPoint;

    private CollisionData _collisionData;

    private Box _boxOne;
    private Box _boxTwo;
    private Line _lineA;
    private Line _lineB;

    private float _lineDistance;
    private Vector3 _pointOnLineA;
    private Vector3 _pointOnLineB;

    private float _pointDistance;
    private bool _projectedOnLine;
    private bool _inside;

    private int _planeIndex;

    void Start()
    {
        _collisionData = new CollisionData(0, 0);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            _planeIndex = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _planeIndex = 1;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _planeIndex = 2;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            _planeIndex = 3;

        if (Input.GetKeyDown(KeyCode.Alpha4))
            _planeIndex = 4;

        if (Input.GetKeyDown(KeyCode.Alpha5))
            _planeIndex = 5;

        _collisionData.Reset();

        UpdateLineData(ref _lineA, transformLineA_p0, transformLineA_p1);
        UpdateLineData(ref _lineB, transformLineB_p0, transformLineB_p1);
        UpdateBoxData(ref _boxOne, transformOne);
        UpdateBoxData(ref _boxTwo, transformTwo);

        CollisionDetector.BoxAndBox(_collisionData, ref _boxOne, ref _boxTwo);

        _lineA.GetDistance(_lineB, out _lineDistance/*, out _pointOnLineA, out _pointOnLineB*/);
        _projectedOnLine = _lineA.GetNearestDistance(transformPoint.position, out _pointDistance, out _pointOnLineA);

        _inside = _boxOne.Inside(transformPoint.position);
    }

    void OnGUI() {
        GUILayout.Label($"boxOne position:{_boxOne.GetAxis(3)}");
        GUILayout.Label($"boxTwo position:{_boxTwo.GetAxis(3)}");
        var contacts = _collisionData.contacts;
        foreach (var contact in contacts) {
            GUILayout.Label($"pen:{contact.penetration}");
        }
    }

    void OnDrawGizmos()
    {
        if (_collisionData == null) {
            return;
        }

        var oldColor = Gizmos.color;

        var contacts = _collisionData.contacts;
        foreach (var contact in contacts) {
            Gizmos.color = Color.red;
            DrawPoint(contact.contactPoint);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(contact.contactPoint, contact.contactNormal);
        }

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(_lineA.p0, _lineA.p1);
        //Gizmos.DrawLine(_lineB.p0, _lineB.p1);
        //Gizmos.color = Color.red;
        //DrawPoint(_pointOnLineA);
        //DrawPoint(_pointOnLineB);
        //DrawPoint(transformPoint.position);

        //Line[] one_lines = new Line[4];
        //_boxOne.GetEdgeParallelToAxis(0, out one_lines[0], out one_lines[1], out one_lines[2], out one_lines[3]);
        //Gizmos.color = Color.red;
        //foreach (var line in one_lines) {
        //    DrawLine(line);
        //}

        //_boxOne.GetEdgeParallelToAxis(1, out one_lines[0], out one_lines[1], out one_lines[2], out one_lines[3]);
        //Gizmos.color = Color.green;
        //foreach (var line in one_lines) {
        //    DrawLine(line);
        //}

        //_boxOne.GetEdgeParallelToAxis(2, out one_lines[0], out one_lines[1], out one_lines[2], out one_lines[3]);
        //Gizmos.color = Color.blue;
        //foreach (var line in one_lines) {
        //    DrawLine(line);
        //}

        //Gizmos.color = Color.cyan;
        //DrawPoint(transformPoint.position);

        //List<Physics.Plane> planes = new List<Physics.Plane>();
        //_boxOne.GetPlane(planes);

        //for (int i = 0; i < planes.Count; ++i) {
        //    if (i == _planeIndex) {
        //        DrawPlane(planes[i]);
        //    }
        //}

        Gizmos.color = oldColor;
    }

    private void UpdateBoxData(ref Box box, Transform trans) {
        box.transform = Matrix4x4.TRS(trans.position, trans.rotation, Vector3.one);
        box.halfSize = trans.localScale * 0.5f;
    }

    private void UpdateLineData(ref Line line, Transform p0, Transform p1) {
        line.p0 = p0.position;
        line.p1 = p1.position;
    }

    private void DrawPoint(Vector3 point) {
        Gizmos.DrawSphere(point, 0.05f);
    }

    private void DrawLine(Line line) {
        Gizmos.DrawLine(line.p0, line.p1);
    }

    private void DrawPlane(Physics.Plane plane) {
        Matrix4x4 m = Gizmos.matrix;
        Matrix4x4 rotate = Matrix4x4.identity;
        rotate.SetColumn(0, plane.right);
        rotate.SetColumn(1, plane.normal);
        rotate.SetColumn(2, Vector3.Cross(plane.right, plane.normal).normalized);
        Gizmos.matrix = Matrix4x4.Translate(plane.center) * rotate * Matrix4x4.Scale(new Vector3(plane.halfSize.x * 2, 0.01f, plane.halfSize.y * 2));
        Gizmos.color = Color.white;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = m;

        Gizmos.color = Color.cyan;
        DrawPoint(plane.center);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(plane.center, plane.normal);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(plane.center, plane.right);
    }

}
