using UnityEngine;

namespace Physics {

    public class BoxCollider : Collider {

        public override ColliderType type => ColliderType.Box;

        public Box box { get; set; }

        public override void UpdateTransform(Rigidbody rigidbody) {
            Box newBox = box;
            rigidbody.GetTransformMatrix(out newBox.transform);
            newBox.body = rigidbody;
            box = newBox;
        }

    }

}
