using UnityEngine;

public class GoalBlack : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private BallLogic ball;

    private void Awake()
    {
        if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
        if (ball == null) ball = FindFirstObjectByType<BallLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;
        if (scoreManager == null || ball == null) return;

        scoreManager.AddWhitePoint();
        ball.Serve(1);
    }
}
