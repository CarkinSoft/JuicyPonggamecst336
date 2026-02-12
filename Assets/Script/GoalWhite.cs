using UnityEngine;

public class GoalWhite : MonoBehaviour
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

        scoreManager.AddBlackPoint();
        ball.Serve(-1);
    }
}