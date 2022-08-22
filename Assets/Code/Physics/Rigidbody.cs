using UnityEngine;

namespace Physics {
    public class Rigidbody : Identified {

        public Vector3 position { get; set; }

        public Vector3 velocity { get; set; }

        public Vector3 acceleration { get; set; }

        public Quaternion rotation { get; set; }

        public Vector3 angularVelocity { get; private set; }

        public float linearDamping { get; set; }

        public float angularDamping { get; set; }

        public float mass {
            get => _mass;
            set {
                Debug.Assert(value > 0, "the mass must be greater than zero.");
                _mass = value;
                _inverseMass = 1.0f / _mass;
            }
        }

        public float inverseMass => _inverseMass;

        private float _mass;

        private float _inverseMass;

        private Vector3 _accuForce;

        private Vector3 _accuTorque;

        private Matrix4x4 _transformMatrix;

        private Matrix3x3 _inverseInertiaTensor;

        private Matrix3x3 _inverseInertiaTensorWorld;

        private Collider _collider;

        public Rigidbody(float mass, float linearDamping, float angularDamping) {
            this.mass = mass;
            this.linearDamping = linearDamping;
            this.angularDamping = angularDamping;
            rotation = Quaternion.identity;
        }

        public void SetInertiaTensor(Matrix3x3 inertiaTensor) {
            _inverseInertiaTensor = inertiaTensor;
            _inverseInertiaTensor.SetInverse();
        }

        public void AddForce(Vector3 force) {
            _accuForce += force;
        }

        public void AddForceAtPoint(Vector3 force, Vector3 point) {
            _accuForce += force;
            _accuTorque += Vector3.Cross(point - position, force);
        }

        public void AddForceAtBodyPoint(Vector3 force, Vector3 point) {
            AddForceAtPoint(force, _transformMatrix * point);
        }

        public void Integrate(float deltaTime) {
            // update position
            position += velocity * deltaTime;

            // update rotation
            rotation = rotation.AddScaledVector(angularVelocity, deltaTime);

            // update velocity
            Vector3 resultingAcc = acceleration;
            resultingAcc += _accuForce * _inverseMass;
            velocity += resultingAcc * deltaTime;
            velocity *= Mathf.Pow(linearDamping, deltaTime);

            // update angular velocity
            _inverseInertiaTensorWorld.Transform(_accuTorque, out Vector3 angularAcceleration);
            angularVelocity += angularAcceleration * deltaTime;
            angularVelocity *= Mathf.Pow(angularDamping, deltaTime);

            CalculateDerivedData();
        }

        public void ClearForce() {
            _accuForce = Vector3.zero;
            _accuTorque = Vector3.zero;
        }

        public void CalculateDerivedData() {
            rotation.Normalize();

            _transformMatrix.SetTRS(position, rotation, Vector3.one);

            TransformInertiaTensor();
        }

        private void TransformInertiaTensor() {
            Matrix3x3 rotMat, rotTransposeMat;
            rotMat = _transformMatrix.GetBasis();
            rotMat.GetTranspose(out rotTransposeMat);

            rotMat.Multiply(ref _inverseInertiaTensor);
            rotMat.Multiply(ref rotTransposeMat);

            _inverseInertiaTensorWorld = rotMat;
        }

    }
}
