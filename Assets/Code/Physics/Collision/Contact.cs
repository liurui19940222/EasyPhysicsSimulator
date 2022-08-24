using UnityEngine;

namespace Physics {

    public struct Contact {

        public Rigidbody one;

        public Rigidbody two;

        public float friction;

        public float restitution;

        public float penetration;

        public Vector3 contactPoint;

        public Vector3 contactNormal;

        public Matrix3x3 contactToWorld;

        public void SetBodyData(Rigidbody one, Rigidbody two, float friction, float restitution) {
            this.one = one;
            this.two = two;
            this.friction = friction;
            this.restitution = restitution;
        }

        public void Resolve(float deltaTime) {
            CalculateContactBasis();
            ResolveVelocity(deltaTime);
            ResolveInterpenetration(deltaTime);
        }

        /// <summary>
        /// 计算两个对象的分离速度
        /// </summary>
        /// <returns>分离速度，小于0时表示两个对象有相互靠近的趋势</returns>
        public float CalculateSeparatingVelocity() {
            Vector3 v = one.velocity;
            if (two != null) {
                v -= two.velocity;
            }
            return Vector3.Dot(v, contactNormal);
        }

        public void CalculateContactBasis() {
            Vector3 contactTangent0, contactTangent1;
            if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y)) {
                float s = 1.0f / Mathf.Sqrt(contactNormal.z * contactNormal.z + contactNormal.x * contactNormal.x);

                contactTangent0.x = contactNormal.z * s;
                contactTangent0.y = 0;
                contactTangent0.z = -contactNormal.x * s;

                contactTangent1.x = contactNormal.y * contactTangent0.x;
                contactTangent1.y = contactNormal.z * contactTangent0.x - contactNormal.x * contactTangent0.z;
                contactTangent1.z = -contactNormal.y * contactTangent0.x;
            }
            else {
                float s = 1.0f / Mathf.Sqrt(contactNormal.z * contactNormal.z + contactNormal.y * contactNormal.y);

                contactTangent0.x = 0;
                contactTangent0.y = -contactNormal.z * s;
                contactTangent0.z = contactNormal.y * s;

                contactTangent1.x = contactNormal.y * contactTangent0.z - contactNormal.z * contactTangent0.y;
                contactTangent1.y = -contactNormal.x * contactTangent0.z;
                contactTangent1.z = contactNormal.x * contactTangent0.y;
            }

            contactToWorld.SetColumn(0, contactNormal);
            contactToWorld.SetColumn(1, contactTangent0);
            contactToWorld.SetColumn(2, contactTangent1);
        }

        private void ResolveInterpenetration(float deltaTime) {
            if (penetration <= 0) {
                return;
            }

            float totalMass = 0;
            if (!one.isStatic) {
                totalMass = one.mass;
            }
            if (two != null && !two.isStatic) {
                totalMass += two.mass;
            }
            if (totalMass <= 0) {
                return;
            }

            Vector3 movePerMass = contactNormal * (penetration / totalMass);
            if (!one.isStatic) {
                one.position += movePerMass * one.mass;
            }
            if (two != null && !two.isStatic) {
                two.position += -movePerMass * two.mass;
            }
        }

        private void ResolveVelocity(float deltaTime) {
            float deltaVelocity = CompulteDeltaVelocity(one) + CompulteDeltaVelocity(two);

            Vector3 contactVelocity = CalculateLocalVelocity(one, deltaTime) - CalculateLocalVelocity(two, deltaTime);

            float desiredDeltaVeclocity = -contactVelocity.x * (1 + restitution);

            Vector3 impulseContact = new Vector3(desiredDeltaVeclocity / deltaVelocity, 0, 0);

            Vector3 impulse = contactToWorld.Multiply(impulseContact);

            ApplyImpulse(impulse, one, deltaTime);
            ApplyImpulse(-impulse, two, deltaTime);
        }

        private float CompulteDeltaVelocity(Rigidbody body) {
            if (body == null || body.isStatic) {
                return 0;
            }

            Vector3 relativeContactPosition = contactPoint - body.position;

            Vector3 torquePerUnitImpulse = Vector3.Cross(relativeContactPosition, contactNormal);

            body.GetAngularAccelVelocity(torquePerUnitImpulse, out Vector3 rotationPerUnitImpulse);

            Vector3 velocityPerUnitImpulse = Vector3.Cross(rotationPerUnitImpulse, relativeContactPosition);

            Vector3 velocityPerUnitImpulseContact = contactToWorld.TransformTranspose(velocityPerUnitImpulse);

            float angularComponent = velocityPerUnitImpulseContact.x;

            return angularComponent + body.inverseMass;
        }

        private Vector3 CalculateLocalVelocity(Rigidbody body, float duration) {
            if (body == null || body.isStatic) {
                return Vector3.zero;
            }

            Vector3 relativeContactPosition = contactPoint - body.position;
            Vector3 velocity = Vector3.Cross(body.angularVelocity, relativeContactPosition);
            velocity += body.velocity;

            Vector3 contactVelocity = contactToWorld.TransformTranspose(velocity);

            return contactVelocity;
        }

        private void ApplyImpulse(Vector3 impulse, Rigidbody body, float deltaTime) {
            if (body == null || body.isStatic) {
                return;
            }

            Vector3 velocityChange = impulse * body.inverseMass;

            Vector3 impulsiveTorque = Vector3.Cross(impulse, contactPoint - body.position);

            body.GetAngularAccelVelocity(impulsiveTorque, out Vector3 rotationChange);

            body.velocity += velocityChange;

            body.angularVelocity += rotationChange;
        }
    }
}
