using UnityEngine;

namespace Physics {

    public class SpringGenerator : IForceGenerator {

        public Rigidbody other { get; set; }

        public float springConstant { get; set; }

        public float restLength { get; set; }

        public SpringGenerator(Rigidbody other, float springConstant, float restLength) {
            this.other = other;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public void UpdateForce(Rigidbody body, float deltaTime) {
            Vector3 force = body.position - other.position;
            if (force == Vector3.zero) {
                return;
            }
            float distance = force.magnitude;
            force = -(force / distance) * Mathf.Abs(distance - restLength) * springConstant;
            body.AddForce(force);
        }

    }

}
