namespace Physics {

    public class BoxCollider : Collider {

        public override ColliderType type => ColliderType.Box;

        public Box box { get; set; }

        public BoxCollider(Rigidbody rigidbody) : base(rigidbody) { }

        public override void UpdateTransform() {
            Box newBox = box;
            rigidbody.GetTransformMatrix(out newBox.transform);
            newBox.body = rigidbody;
            box = newBox;
        }

        public override void DetectCollision(CollisionData collision, Collider otherCollider) {
            switch (otherCollider.type) {
                case ColliderType.Box:
                    Box boxOne = box;
                    Box boxTwo = ((BoxCollider)otherCollider).box;
                    CollisionDetector.BoxAndBox(collision, ref boxOne, ref boxTwo);
                    break;
                case ColliderType.Sphere:
                    break;
                case ColliderType.Plane:
                    break;
            }
        }

    }

}
