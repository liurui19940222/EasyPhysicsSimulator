using UnityEngine;

namespace Physics {

    public class GravityGenerator : IForceGenerator {

        public Vector3 gravity { get; set; }

        public GravityGenerator(float gravity) {
            this.gravity = new Vector3(0, -gravity, 0);
        }

        public void UpdateForce(Rigidbody body, float deltaTime) {
            body.AddForce(gravity * body.mass);
        }
    }

}