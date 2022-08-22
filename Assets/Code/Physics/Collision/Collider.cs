namespace Physics {

    public enum ColliderType {
        Box,
        Sphere,
        Plane
    }

    public abstract class Collider {

        public abstract ColliderType type { get; }

    }
}
