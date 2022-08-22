using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "PhysicsSettings")]
public class PhysicsSettings : ScriptableObject {

    public Material anchorMat;

    public Material forceLineMat;

    public float gravity = 10;

    public float dragConst1 = 0.2f;

    public float dragConst2 = 0.0f;

}

