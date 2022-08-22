using System.Collections.Generic;

namespace Physics {
    public class CollisionData {

        public float friction { get; set; }

        public float restitution { get; set; }

        public List<Contact> contacts { get; private set; }

        public CollisionData(float friction, float restitution) {
            contacts = new List<Contact>();
            this.friction = friction;
            this.restitution = restitution;
        }

        public void AddContact(Contact contact) {
            contacts.Add(contact);
        }

        public void Reset() {
            contacts.Clear();
        }

    }
}
