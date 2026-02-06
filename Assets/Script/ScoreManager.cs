using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class ScoreManager : MonoBehaviour
{
    [FormerlySerializedAs("whiteScoreText")] [Header("References")]
    public TextMeshProUGUI WhiteScoreUI;
    [FormerlySerializedAs("blackScoreText")] public TextMeshProUGUI BlackScoreUI;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Game Rules")]
    public int winningScore = 10;

    [Header("Audio")]
    public AudioClip crowdCheering;
    [SerializeField] private AudioClip winSfx;

    public ScoreManager scoreManager;

    private AudioSource audioSource;
    private int whiteScore = 0;
    private int blackScore = 0;

    public bool GameOver { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        WhiteScoreUI.text = GetWhiteScore().ToString();
        BlackScoreUI.text = GetBlackScore().ToString();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void AddWhiteScore()
    {
        if (GameOver) return;

        whiteScore += 1;
        WhiteScoreUI.text = whiteScore.ToString();
        audioSource.PlayOneShot(crowdCheering);

        CheckWin();
    }

    public void AddBlackScore()
    {
        if (GameOver) return;

        blackScore += 1;
        BlackScoreUI.text = blackScore.ToString();
        audioSource.PlayOneShot(crowdCheering);

        CheckWin();
    }

    private void CheckWin()
    {
        if (whiteScore >= winningScore)
            EndGame("White");
        else if (blackScore >= winningScore)
            EndGame("Black");
    }

    private void EndGame(string winner)
    {
        GameOver = true;

        if (winText != null)
            winText.text = $"{winner} has won!";

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winSfx != null)
            audioSource.PlayOneShot(winSfx);

        // Pause after we show UI / play sound (sound still plays with timeScale=0)
        Time.timeScale = 0f;
    }

    public int GetWhiteScore() => whiteScore;
    public int GetBlackScore() => blackScore;
}