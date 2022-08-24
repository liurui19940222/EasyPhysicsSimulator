namespace Physics {

    public enum ColliderType {
        Box,
        Sphere,
        Plane
    }

    public abstract class Collider {

        public Rigidbody rigidbody { get; private set; }

        public abstract ColliderType type { get; }

        public Collider(Rigidbody rigidbody) {
            this.rigidbody = rigidbody;
        }

        public abstract void UpdateTransform();

        public abstract void DetectCollision(CollisionData collision, Collider otherCollider);

    }
}
