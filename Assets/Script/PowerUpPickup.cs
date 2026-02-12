using System.Collections;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public enum Type
    {
        BallSpeed,
        PaddleSize
    }

    [SerializeField] private Type type = Type.BallSpeed;
    [SerializeField] private float multiplier = 1.5f;
    [SerializeField] private float durationSeconds = 4f;

    [Header("Paddles")]
    [SerializeField] private Transform whitePaddle;
    [SerializeField] private Transform blackPaddle;

    [SerializeField] private bool destroyOnPickup = true;

    private void Awake()
    {
        if (whitePaddle == null)
        {
            PaddleWhite p = FindFirstObjectByType<PaddleWhite>();
            if (p != null) whitePaddle = p.transform;
        }

        if (blackPaddle == null)
        {
            PaddleBlack p = FindFirstObjectByType<PaddleBlack>();
            if (p != null) blackPaddle = p.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BallLogic ball = other.GetComponent<BallLogic>();
        if (ball == null) return;

        if (type == Type.BallSpeed)
        {
            ball.ApplySpeedMultiplier(multiplier, durationSeconds);
        }
        else
        {
            StartCoroutine(PaddleSizeRoutine(ball));
        }

        if (destroyOnPickup) Destroy(gameObject);
        else gameObject.SetActive(false);
    }

    private IEnumerator PaddleSizeRoutine(BallLogic ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        Transform target = rb.linearVelocity.y >= 0f ? blackPaddle : whitePaddle;
        if (target == null) yield break;

        Vector3 original = target.localScale;
        target.localScale = new Vector3(original.x, original.y, original.z * multiplier);

        yield return new WaitForSeconds(durationSeconds);

        if (target != null)
            target.localScale = original;
    }
}
