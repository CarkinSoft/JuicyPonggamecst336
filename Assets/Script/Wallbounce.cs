using UnityEngine;

public class Wallbounce : MonoBehaviour
{
    [SerializeField] private float separationPush = 0.02f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null) return;

        Vector3 v = rb.linearVelocity;
        float speed = v.magnitude;
        if (speed < 0.0001f) return;

        v.z = -v.z;
        rb.linearVelocity = v.normalized * speed;

        ContactPoint cp = collision.GetContact(0);
        rb.position += cp.normal * separationPush;
    }
}
