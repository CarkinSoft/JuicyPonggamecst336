using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BallLogic : MonoBehaviour
{
    [Header("Serve")]
    [SerializeField] private float serveSpeed = 8f;

    [Header("Respawn")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;

    private Vector3 initialSpawnPos;
    private Quaternion initialSpawnRot;

    [Header("Goals (Tags on the GOAL trigger objects)")]
    [SerializeField] private string blackGoalTag = "GoalBlack";
    [SerializeField] private string whiteGoalTag = "GoalWhite";

    [Header("Score")]
    [SerializeField] private ScoreManager scoreManager;

    [Header("Bounce SFX")]
    [SerializeField] private AudioClip[] bounceClips;
    [SerializeField] private float minTimeBetweenBounces = 0.05f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float lastBounceTime;
    private bool isDespawning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (bounceClips == null || bounceClips.Length == 0)
            bounceClips = Resources.LoadAll<AudioClip>("Audio");

        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();

        initialSpawnPos = transform.position;
        initialSpawnRot = transform.rotation;
    }

    private void Start()
    {
        ServeUpOrDown();
    }

    private void ServeUpOrDown()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float yDir = Random.value < 0.5f ? -1f : 1f;
        rb.linearVelocity = new Vector3(0f, yDir, 0f) * serveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDespawning) return;

        // If someone already won, don't keep spawning balls
        if (scoreManager != null && scoreManager.GameOver)
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag(blackGoalTag))
        {
            if (scoreManager != null) scoreManager.AddWhiteScore();
            RespawnBall();
            return;
        }

        if (other.CompareTag(whiteGoalTag))
        {
            if (scoreManager != null) scoreManager.AddBlackScore();
            RespawnBall();
            return;
        }

        // Helpful when goal tags aren't set correctly
        Debug.Log($"Ball triggered '{other.name}' but it wasn't tagged '{blackGoalTag}' or '{whiteGoalTag}'. Tag was '{other.tag}'.");
    }

    private void RespawnBall()
    {
        isDespawning = true;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : initialSpawnPos;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : initialSpawnRot;

        GameObject prefabToSpawn = ballPrefab != null ? ballPrefab : gameObject;
        Instantiate(prefabToSpawn, pos, rot);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastBounceTime < minTimeBetweenBounces)
            return;

        lastBounceTime = Time.time;

        if (bounceClips == null || bounceClips.Length == 0)
            return;

        audioSource.PlayOneShot(bounceClips[Random.Range(0, bounceClips.Length)]);
    }
}

