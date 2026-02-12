using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BallLogic : MonoBehaviour
{
    [Header("Serve")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float serveSpeed = 8f;

    [Header("Bounce")]
    [SerializeField] private string paddleTag = "Paddle";
    [SerializeField] private float maxBounceAngle = 45f;
    [SerializeField] private float speedMultiplierPerHit = 1.08f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float minZDeflection = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioClip paddleHitClip;
    [SerializeField] private float minTimeBetweenPaddleHits = 0.05f;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.25f;
    [SerializeField] private float speedForMaxPitch = 20f;
    [SerializeField] private float edgePitchBoost = 0.12f;
    [SerializeField] private float edgeVolumeBoost = 0.15f;

    private Rigidbody rb;
    private AudioSource audioSource;

    private Vector3 defaultSpawnPos;
    private Quaternion defaultSpawnRot;

    private float lastPaddleHitTime;
    private Coroutine speedRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        defaultSpawnPos = transform.position;
        defaultSpawnRot = transform.rotation;

        if (paddleHitClip == null)
            paddleHitClip = Resources.Load<AudioClip>("Audio/PaddleHit");
    }

    private void Start()
    {
        ServeRandom();
    }

    public void ResetToCenter()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : defaultSpawnPos;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : defaultSpawnRot;

        rb.position = pos;
        rb.rotation = rot;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ServeRandom()
    {
        int dir = Random.value < 0.5f ? -1 : 1;
        Serve(dir);
    }

    public void Serve(int yDirection)
    {
        ResetToCenter();
        rb.linearVelocity = new Vector3(0f, Mathf.Sign(yDirection), 0f) * serveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;
        if (!collision.collider.CompareTag(paddleTag)) return;

        if (Time.time - lastPaddleHitTime < minTimeBetweenPaddleHits)
            return;

        lastPaddleHitTime = Time.time;

        Vector3 v = rb.linearVelocity;
        float incomingY = v.y;
        if (Mathf.Abs(incomingY) < 0.001f) incomingY = -1f;

        float outY = -Mathf.Sign(incomingY);

        Bounds b = collision.collider.bounds;
        float relativeZ = 0f;
        if (b.extents.z > 0.0001f)
            relativeZ = Mathf.Clamp((collision.GetContact(0).point.z - b.center.z) / b.extents.z, -1f, 1f);

        float angle = relativeZ * maxBounceAngle;
        float zFromAngle = Mathf.Tan(angle * Mathf.Deg2Rad);

        float z = Mathf.Sign(relativeZ == 0f ? (Random.value < 0.5f ? -1f : 1f) : relativeZ) *
                  Mathf.Max(Mathf.Abs(zFromAngle), minZDeflection);

        Vector3 dir = new Vector3(0f, outY, z).normalized;

        float newSpeed = Mathf.Min(Mathf.Max(serveSpeed, v.magnitude) * speedMultiplierPerHit, maxSpeed);
        rb.linearVelocity = dir * newSpeed;

        PlayPaddleHitSfx(newSpeed, Mathf.Abs(relativeZ));
    }

    private void PlayPaddleHitSfx(float ballSpeed, float edge01)
    {
        if (paddleHitClip == null) return;

        float t = Mathf.Clamp01(ballSpeed / Mathf.Max(0.01f, speedForMaxPitch));
        float pitch = Mathf.Lerp(minPitch, maxPitch, t) + edge01 * edgePitchBoost;
        float volume = 1f + edge01 * edgeVolumeBoost;

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(paddleHitClip, volume);
    }

    public void ApplySpeedMultiplier(float multiplier, float durationSeconds)
    {
        if (speedRoutine != null)
            StopCoroutine(speedRoutine);

        speedRoutine = StartCoroutine(SpeedMultiplierRoutine(multiplier, durationSeconds));
    }

    private IEnumerator SpeedMultiplierRoutine(float multiplier, float durationSeconds)
    {
        multiplier = Mathf.Max(0.1f, multiplier);

        rb.linearVelocity *= multiplier;
        yield return new WaitForSeconds(durationSeconds);

        if (rb != null)
            rb.linearVelocity /= multiplier;

        speedRoutine = null;
    }
}

