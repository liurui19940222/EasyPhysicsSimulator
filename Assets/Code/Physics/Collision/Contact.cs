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

        public void SetBodyData(Rigidbody one, Rigidbody two, float friction, float restitution) {
            this.one = one;
            this.two = two;
            this.friction = friction;
            this.restitution = restitution;
        }

        public void Resolve(float deltaTime) {
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

        private void ResolveInterpenetration(float deltaTime) {
            if (penetration <= 0) {
                return;
            }
            
            float totalMass = one.mass;
            if (two != null) {
                totalMass += two.mass;
            }

            if (totalMass <= 0) {
                return;
            }

            Vector3 movePerMass = contactNormal * (penetration / totalMass);
            one.position += movePerMass * one.mass;

            if (two != null) {
                two.position += -movePerMass * two.mass;
            }
        }

        private void ResolveVelocity(float deltaTime) {
            float separatingVelocity = CalculateSeparatingVelocity();
            
            if (separatingVelocity > 0) {
                return;
            }

            float newSepVelocity = -separatingVelocity * restitution;
            float deltaVelocity = newSepVelocity - separatingVelocity;

            float totalMass = one.mass;
            if (two != null) {
                totalMass += two.mass;
            }

            float impulse = deltaVelocity * totalMass;
            Vector3 normalImpulse = impulse * contactNormal;

            one.velocity += normalImpulse * one.inverseMass;

            if (two != null) {
                two.velocity += -normalImpulse * two.inverseMass;
            }
        }

    }
}
