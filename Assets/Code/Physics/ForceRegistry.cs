using System.Collections.Generic;

namespace Physics {

    public class ForceRegistration {
        internal Rigidbody body;
        internal IForceGenerator generator;
    }

    public class ForceRegistry {

        private List<ForceRegistration> _registry;

        public ForceRegistry() {
            _registry = new List<ForceRegistration>();
        }

        public void Add(Rigidbody particle, IForceGenerator generator) {
            _registry.Add(new ForceRegistration() { 
                body = particle,
                generator = generator
            });
        }

        public void Remove(Rigidbody particle, IForceGenerator generator) {
            for (int i = 0; i < _registry.Count; ++i) {
                if (_registry[i].body == particle && _registry[i].generator == generator) {
                    _registry.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear() {
            _registry.Clear();
        }

        public void UpdateForces(float deltaTime) {
            for (int i = 0; i < _registry.Count; ++i) {
                _registry[i].generator.UpdateForce(_registry[i].body, deltaTime);
            }
        }

        public void GetForceRegistrations(List<ForceRegistration> list) {
            list.AddRange(_registry);
        }

    }

}