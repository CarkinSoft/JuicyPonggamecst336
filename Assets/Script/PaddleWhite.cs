using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float paddleSpeed = 1f;
    public float maxZ = 0f;

    [Header("Ball bounce control")]
    public float maxBounceAngle = 45f;

    [Header("Ball speed-up per hit")]
    public float speedMultiplierPerHit = 1.08f;
    public float maxBallSpeed = 25f;

    public float angleSensitivityPower = 0.6f;
    public float separationPush = 0.05f;

    [Header("Angle feel")]
    [Tooltip("Adds a little Z deflection even near the center so hits don't feel 'dead straight'.")]
    public float minZDeflection = 0.15f;

    [Header("Anti multi-bounce")]
    public float ignoreCollisionTime = 0.06f;

    private void Update()
    {
        if (Keyboard.current.dKey.isPressed)
        {
            Vector3 newPos = transform.position + new Vector3(0f, 0f, paddleSpeed) * Time.deltaTime;
            newPos.z = Mathf.Clamp(newPos.z, -maxZ, maxZ);
            transform.position = newPos;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            Vector3 newPos = transform.position - new Vector3(0f, 0f, paddleSpeed) * Time.deltaTime;
            newPos.z = Mathf.Clamp(newPos.z, -maxZ, maxZ);
            transform.position = newPos;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.rigidbody;
        if (ballRb == null) return;

        Collider paddleCol = collision.collider;
        Collider ballCol = ballRb.GetComponent<Collider>();
        if (ballCol == null) return;

        BounceBall(ballRb, paddleCol, collision.GetContact(0).point);

        StartCoroutine(IgnoreCollisionTemporarily(ballCol, paddleCol, ignoreCollisionTime));
    }

    private void BounceBall(Rigidbody ballRb, Collider paddleCol, Vector3 contactPoint)
    {
        float incomingY = ballRb.linearVelocity.y;
        if (Mathf.Abs(incomingY) < 0.001f) return;

        float outY = -Mathf.Sign(incomingY);

        Bounds b = paddleCol.bounds;
        float relativeZ = (contactPoint.z - b.center.z) / Mathf.Max(0.0001f, b.extents.z);
        relativeZ = Mathf.Clamp(relativeZ, -1f, 1f);

        float shaped = Mathf.Sign(relativeZ) * Mathf.Pow(Mathf.Abs(relativeZ), Mathf.Max(0.01f, angleSensitivityPower));

        float angle = shaped * maxBounceAngle;
        float zFromAngle = Mathf.Tan(angle * Mathf.Deg2Rad);

        // NEW: ensure some Z so it angles even when not at the extreme edge
        float z = Mathf.Sign(shaped == 0f ? (Random.value < 0.5f ? -1f : 1f) : shaped) *
                  Mathf.Max(Mathf.Abs(zFromAngle), minZDeflection);

        Vector3 dir = new Vector3(0f, outY, z).normalized;

        float currentSpeed = ballRb.linearVelocity.magnitude;
        float newSpeed = Mathf.Min(Mathf.Max(0.01f, currentSpeed) * speedMultiplierPerHit, maxBallSpeed);

        ballRb.linearVelocity = dir * newSpeed;
        ballRb.position += new Vector3(0f, outY, 0f) * separationPush;
    }

    private IEnumerator IgnoreCollisionTemporarily(Collider ballCol, Collider paddleCol, float seconds)
    {
        Physics.IgnoreCollision(ballCol, paddleCol, true);
        yield return new WaitForSeconds(seconds);
        if (ballCol != null && paddleCol != null)
            Physics.IgnoreCollision(ballCol, paddleCol, false);
    }
}