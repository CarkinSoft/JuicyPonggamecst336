using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Tooltip("Small push to avoid sticking inside the wall collider.")]
    [SerializeField] private float separationPush = 0.02f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        Vector3 v = rb.linearVelocity;
        float speed = v.magnitude;
        if (speed < 0.0001f) return;

        // Side walls: invert Z, keep the same speed
        v.z = -v.z;

        rb.linearVelocity = v.normalized * speed;

        // Nudge away from wall to prevent repeated contacts
        ContactPoint cp = collision.GetContact(0);
        rb.position += cp.normal * separationPush;
    }
}
