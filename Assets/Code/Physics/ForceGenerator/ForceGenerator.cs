namespace Physics {

    public interface IForceGenerator {

        void UpdateForce(Rigidbody body, float deltaTime);

    }

}