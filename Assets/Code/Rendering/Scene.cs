using Physics;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public PhysicsSettings settings;

    private World _world;

    private List<Physics.Rigidbody> _bodies;

    private List<ForceRegistration> _forceRegistrations;

    private Dictionary<Physics.Rigidbody, GameObject> _bodyElements;

    private Dictionary<ForceRegistration, LineRenderer> _forceRegistrationElements;

    private void Start() {
        _bodyElements = new Dictionary<Physics.Rigidbody, GameObject>();
        _forceRegistrationElements = new Dictionary<ForceRegistration, LineRenderer>();
        _bodies = new List<Physics.Rigidbody>();
        _forceRegistrations = new List<ForceRegistration>();
        _world = new World(settings.gravity);

        var body0 = _world.Create(new Vector3(0, 8, 0), 5);
        body0.SetCollider(Vector3.one * 0.5f);
        _world.AddAnchorSpring(body0, new Vector3(0, 8, 0), 5f, 0).JoinBody(new Vector3(0.5f, 0.5f, 0.0f));
        //_world.AddDrag(body0, settings.dragConst1, settings.dragConst2);

        //var particle1 = _world.Create(new Vector3(-3, 8, 0), 5);
        //_world.AddSpring(particle0, particle1, 5.0f, 0.5f);
        //_world.AddDrag(particle1, settings.dragConst1, settings.dragConst2);
    }

    private void FixedUpdate() {
        // 更新物理
        _world.Tick(Time.fixedDeltaTime);
    }

    private void Update() {
        // 渲染粒子
        _bodies.Clear();
        _world.GetRigidbodies(_bodies);

        for (int i = 0; i < _bodies.Count; ++i) {
            if (!_bodyElements.TryGetValue(_bodies[i], out GameObject go)) {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "Physics.Rigidbody";
                var collider = _bodies[i].GetCollider();
                switch (collider.type) {
                    case ColliderType.Box:
                        go.transform.localScale = ((Physics.BoxCollider)collider).box.halfSize * 2;
                        break;
                    case ColliderType.Sphere:
                        break;
                    case ColliderType.Plane:
                        break;
                }
                _bodyElements.Add(_bodies[i], go);
            }
        }

        foreach (var kv in _bodyElements) {
            kv.Value.transform.position = kv.Key.position;
            kv.Value.transform.rotation = kv.Key.rotation;
        }

        // 渲染作用力
        _forceRegistrations.Clear();
        _world.GetForceRegistrations(_forceRegistrations);
        for (int i = 0; i < _forceRegistrations.Count; ++i) {
            if (!_forceRegistrationElements.TryGetValue(_forceRegistrations[i], out LineRenderer line)) {
                if (_forceRegistrations[i].generator is GravityGenerator) {
                    continue;
                }
                var go = new GameObject();
                go.hideFlags = HideFlags.HideInHierarchy;
                line = go.AddComponent<LineRenderer>();
                line.sharedMaterial = settings.forceLineMat;
                line.positionCount = 2;
                line.startWidth = line.endWidth = 0.1f;
                _forceRegistrationElements.Add(_forceRegistrations[i], line);
                var gen = _forceRegistrations[i].generator;
                if (gen is AnchoredSpringGenerator anchorGen) {
                    var anchorGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    anchorGo.transform.localScale = Vector3.one * 0.3f;
                    anchorGo.transform.SetParent(go.transform);
                    anchorGo.GetComponent<MeshRenderer>().sharedMaterial = settings.anchorMat;
                }
            }
        }

        foreach (var kv in _forceRegistrationElements) {
            var registration = kv.Key;
            var line = kv.Value;
            if (registration.generator is AnchoredSpringGenerator anchoredSpringGen) {
                line.SetPosition(0, anchoredSpringGen.anchor);
                line.SetPosition(1, registration.body.position);
                line.transform.GetChild(0).position = anchoredSpringGen.anchor;
            }
            else if (registration.generator is SpringGenerator springGen) {
                line.SetPosition(0, registration.body.position);
                line.SetPosition(1, springGen.other.position);
            }
        }
    }

}
