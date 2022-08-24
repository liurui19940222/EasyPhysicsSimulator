using UnityEngine;

namespace Physics {
    public class Rigidbody : Identified {

        public Vector3 position { get; set; }

        public Vector3 velocity { get; set; }

        public Vector3 acceleration { get; set; }

        public Vector3 rotation { get; set; }

        public Vector3 angularVelocity { get; set; }

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

        public bool isStatic { get; set; }

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
            rotation = default;// Quaternion.identity;
        }

        public void SetCollider(Vector3 halfSize) {
            if (!(_collider is BoxCollider boxCollider)) {
                _collider = boxCollider = new BoxCollider(this);   
            }
            boxCollider.box = new Box() { 
                halfSize = halfSize
            };
            Matrix3x3 inertiaTensor = default;
            float coefficient = 1.0f / 12.0f * mass;
            float sqrX = Mathf.Pow(halfSize.x * 2, 2);
            float sqrY = Mathf.Pow(halfSize.y * 2, 2);
            float sqrZ = Mathf.Pow(halfSize.z * 2, 2);
            inertiaTensor.m00 = coefficient * (sqrY + sqrZ);
            inertiaTensor.m11 = coefficient * (sqrX + sqrZ);
            inertiaTensor.m22 = coefficient * (sqrX + sqrY);
            SetInertiaTensor(ref inertiaTensor);
            _collider.UpdateTransform();
        }

        public Collider GetCollider() {
            return _collider;
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
            // update angular velocity
            _inverseInertiaTensorWorld.Transform(_accuTorque, out Vector3 angularAcceleration);
            angularVelocity += angularAcceleration * deltaTime;

            // update velocity
            Vector3 resultingAcc = acceleration;
            resultingAcc += _accuForce * _inverseMass;
            velocity += resultingAcc * deltaTime;

            velocity *= Mathf.Pow(linearDamping, deltaTime);
            angularVelocity *= Mathf.Pow(angularDamping, deltaTime);

            // update position
            position += velocity * deltaTime;

            // update rotation
            rotation += angularVelocity * deltaTime;
            //rotation = Quaternion.Euler(angularVelocity * deltaTime) * rotation;

            CalculateDerivedData();

            ClearForce();
        }

        public void ClearForce() {
            _accuForce = Vector3.zero;
            _accuTorque = Vector3.zero;
        }

        public void CalculateDerivedData() {
            rotation.Normalize();

            Quaternion q = Quaternion.Euler(rotation);
            _transformMatrix.SetTRS(position, q, Vector3.one);

            TransformInertiaTensor();

            _collider.UpdateTransform();
        }

        public void GetTransformMatrix(out Matrix4x4 mat) {
            mat = _transformMatrix;
        }

        public void GetAngularAccelVelocity(Vector3 torque, out Vector3 angularAcceleration) {
            _inverseInertiaTensorWorld.Transform(torque, out angularAcceleration);
        }

        private void SetInertiaTensor(ref Matrix3x3 inertiaTensor) {
            _inverseInertiaTensor = inertiaTensor;
            _inverseInertiaTensor.SetInverse();
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
