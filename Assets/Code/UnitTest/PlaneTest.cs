using Physics;
using UnityEngine;

public class PlaneTest : MonoBehaviour
{
    public Transform transformOne;
    public Transform transformLineA_p0;
    public Transform transformLineA_p1;
    public Transform transformPoint;

    private Line _line;
    private Physics.Plane _plane;
    private float _pointDir;
    private Physics.Plane.LineIntersectType _lineIntersectType;
    private float _lineParam;
    private Vector3 _intersectPoint;

    void Start()
    {
       
    }

    void Update() {
        _plane.center = transformOne.position;
        _plane.normal = transformOne.up;
        _plane.right = transformOne.right;
        _plane.halfSize = new Vector2(transformOne.localScale.x, transformOne.localScale.z) * 5;
        _line.p0 = transformLineA_p0.position;
        _line.p1 = transformLineA_p1.position;

        _pointDir = _plane.ComputePointInWhichHalfSpace(transformPoint.position);
        _lineIntersectType = _plane.ComputeLineIntersectWithPlaneVolume(_line, out _lineParam, out _intersectPoint);
    }

    void OnGUI() {
        GUILayout.Label($"_pointDir:{_pointDir}");
        GUILayout.Label($"_intersectPoint:{_lineIntersectType} t:{_lineParam} intersectPoint:{_intersectPoint}");
    }

    void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;

        Gizmos.color = Color.green;
        DrawLine(_line);
        switch (_lineIntersectType) {
            case Physics.Plane.LineIntersectType.InSegment:
                Gizmos.color = Color.red;
                DrawPoint(_intersectPoint);
                break;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_plane.center, _plane.normal);

        Gizmos.color = oldColor;
    }

    private void DrawPoint(Vector3 point) {
        Gizmos.DrawSphere(point, 0.05f);
    }

    private void DrawLine(Line line) {
        Gizmos.DrawLine(line.p0, line.p1);
    }

}
