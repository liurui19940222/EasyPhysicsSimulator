using UnityEngine;

namespace Physics {
    public class DragGenerator : IForceGenerator {

        public float k1 { get; set; }

        public float k2 { get; set; }

        public DragGenerator(float k1, float k2) {
            this.k1 = k1;
            this.k2 = k2;
        }

        public void UpdateForce(Rigidbody body, float deltaTime) {
            Vector3 force = body.velocity;
            if (force == Vector3.zero) {
                return;
            }
            float mag = force.magnitude;
            float coeff = k1 * mag + k2 * mag * mag;
            force = (force / mag) * (-coeff);
            body.AddForce(force);
        }
    }
}
