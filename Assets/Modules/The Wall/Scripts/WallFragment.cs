using UnityEngine;

public class WallFragment : MonoBehaviour {

    public Vector3 InitialSpeedBoost;
    public float MinSpeed;
    public float SpeedMul;
    public float MinAngularSpeed;
    public float AngularSpeedMul;

    private enum State {
        Idle,
        ManualColision,
        Exploded
    }

    private State state = State.Idle;

    private void Start() {}

    private void Update() {
        if (rigidbody.velocity == Vector3.zero) return;

        if (state == State.Exploded) {
            return;
        }

        if (state == State.ManualColision) {
            if (rigidbody.velocity.magnitude < MinSpeed) {
                rigidbody.velocity = Vector3.zero;
            } else {
                rigidbody.velocity = new Vector3(0, 0, rigidbody.velocity.z);
            }
        }

        if (rigidbody.velocity.magnitude > MinSpeed) {
            rigidbody.velocity *= SpeedMul;
        }
        if (rigidbody.angularVelocity.magnitude > MinAngularSpeed) {
            rigidbody.angularVelocity *= AngularSpeedMul;
        }
    }

    public void Collide() {
        state = State.ManualColision;
        rigidbody.velocity = new Vector3(rigidbody.velocity.normalized.x * InitialSpeedBoost.x, rigidbody.velocity.normalized.y * InitialSpeedBoost.y, rigidbody.velocity.normalized.z * InitialSpeedBoost.z);
    }

    public void Explode(Vector3 force) {
        state = State.Exploded;

        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

}