using System.Collections.Generic;
using UnityEngine;

namespace Physics {

    public class World {

        private List<Rigidbody> _bodies;

        private CollisionData _collisionData;

        private ForceRegistry _forceRegistry;

        private ContactResolver _contactResolver;

        public World(float gravity) {
            ForceGeneratorFactory.Instance.InitFactory(gravity);
            _bodies = new List<Rigidbody>();
            _collisionData = new CollisionData(0, 0);
            _forceRegistry = new ForceRegistry();
            _contactResolver = new ContactResolver();
        }

        public Rigidbody Create(Vector3 position, float mass, bool gravity = true) {
            var particle = new Rigidbody(mass, 0.9f, 0.2f);
            particle.position = position;
            _bodies.Add(particle);
            if (gravity) {
                _forceRegistry.Add(particle, ForceGeneratorFactory.Instance.GetForceGenerator(CommonForceGeneratorType.Gravity));
            }
            return particle;
        }

        public bool RemoveParticle(Rigidbody body) {
            return _bodies.Remove(body);
        }

        public AnchoredSpringGenerator AddAnchorSpring(Rigidbody body, Vector3 anchor, float springConstant, float restLength) {
            AnchoredSpringGenerator anchorSpring = new AnchoredSpringGenerator(anchor, springConstant, restLength);
            _forceRegistry.Add(body, anchorSpring);
            return anchorSpring;
        }

        public void AddSpring(Rigidbody body0, Rigidbody body1, float springConstant, float restLength) {
            IForceGenerator spring0 = new SpringGenerator(body1, springConstant, restLength);
            _forceRegistry.Add(body0, spring0);

            IForceGenerator spring1 = new SpringGenerator(body0, springConstant, restLength);
            _forceRegistry.Add(body1, spring1);
        }

        public void AddDrag(Rigidbody body, float k1, float k2) {
            IForceGenerator drag = new DragGenerator(k1, k2);
            _forceRegistry.Add(body, drag);
        }

        public void Tick(float deltaTime) {
            _forceRegistry.UpdateForces(deltaTime);

            Integrate(deltaTime);

            ComputeContacts();

            _contactResolver.ResolveContacts(_collisionData.contacts, _collisionData.contacts.Count * 2, deltaTime);
        }

        public void GetRigidbodies(List<Rigidbody> list) {
            list.AddRange(_bodies);
        }

        public void GetForceRegistrations(List<ForceRegistration> list) {
            _forceRegistry.GetForceRegistrations(list);
        }

        private void Integrate(float deltaTime) {
            for (int i = 0; i < _bodies.Count; ++i) {
                _bodies[i].Integrate(deltaTime);
            }
        }

        private void ComputeContacts() {
            _collisionData.Reset();

            Collider colliderOne, colliderTwo;
            for (int i = 0; i < _bodies.Count - 1; ++i) {
                for (int j = i + 1; j < _bodies.Count; ++j) {
                    colliderOne = _bodies[i].GetCollider();
                    colliderTwo = _bodies[j].GetCollider();
                    if (colliderOne == null || colliderTwo == null) {
                        continue;
                    }
                    colliderOne.DetectCollision(_collisionData, colliderTwo);
                }
            }
        }

    }

}
