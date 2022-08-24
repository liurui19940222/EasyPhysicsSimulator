using UnityEngine;

namespace Physics {

    public class AnchoredSpringGenerator : IForceGenerator {

        public Vector3 anchor { get; set; }

        public float springConstant { get; set; }

        public float restLength { get; set; }

        private Vector3 _joinPoint;

        public AnchoredSpringGenerator(Vector3 anchor, float springConstant, float restLength) {
            this.anchor = anchor;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public void JoinBody(Vector3 joinPoint) {
            _joinPoint = joinPoint;
        }

        public void UpdateForce(Rigidbody body, float deltaTime) {
            Vector3 force = body.position - anchor;
            if (force == Vector3.zero) {
                return;
            }
            float distance = force.magnitude;
            force = -(force / distance) * Mathf.Abs(distance - restLength) * springConstant;
            body.AddForceAtBodyPoint(force, _joinPoint);
        }
    }

}
