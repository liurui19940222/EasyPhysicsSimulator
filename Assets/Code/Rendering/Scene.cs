using Physics;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public PhysicsSettings settings;

    private World _particleWorld;

    private List<Physics.Rigidbody> _particles;

    private List<ForceRegistration> _forceRegistrations;

    private Dictionary<Physics.Rigidbody, GameObject> _particleElements;

    private Dictionary<ForceRegistration, LineRenderer> _forceRegistrationElements;

    private void Start() {
        _particleElements = new Dictionary<Physics.Rigidbody, GameObject>();
        _forceRegistrationElements = new Dictionary<ForceRegistration, LineRenderer>();
        _particles = new List<Physics.Rigidbody>();
        _forceRegistrations = new List<ForceRegistration>();
        _particleWorld = new World(settings.gravity);

        var particle0 = _particleWorld.Create(new Vector3(0, 8, 0), 5);
        _particleWorld.AddAnchorSpring(particle0, new Vector3(0, 8, 0), 5f, 0);
        _particleWorld.AddDrag(particle0, settings.dragConst1, settings.dragConst2);

        var particle1 = _particleWorld.Create(new Vector3(-3, 8, 0), 5);
        _particleWorld.AddSpring(particle0, particle1, 5.0f, 0.5f);
        _particleWorld.AddDrag(particle1, settings.dragConst1, settings.dragConst2);
    }

    private void FixedUpdate() {
        // 更新物理
        _particleWorld.Tick(Time.fixedDeltaTime);
    }

    private void Update() {
        // 渲染粒子
        _particles.Clear();
        _particleWorld.GetRigidbodies(_particles);

        for (int i = 0; i < _particles.Count; ++i) {
            if (!_particleElements.TryGetValue(_particles[i], out GameObject go)) {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = "Physics.Rigidbody";
                go.transform.localScale = 2.0f * Vector3.one;
                _particleElements.Add(_particles[i], go);
            }
        }

        foreach (var kv in _particleElements) {
            kv.Value.transform.position = kv.Key.position;
        }

        // 渲染作用力
        _forceRegistrations.Clear();
        _particleWorld.GetForceRegistrations(_forceRegistrations);
        for (int i = 0; i < _forceRegistrations.Count; ++i) {
            if (!_forceRegistrationElements.TryGetValue(_forceRegistrations[i], out LineRenderer line)) {
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
