namespace Physics {
    public class Identified {

        private static int _identity;

        public int instanceID { get; set; }

        public Identified() {
            instanceID = ++_identity;
        }

    }
}
