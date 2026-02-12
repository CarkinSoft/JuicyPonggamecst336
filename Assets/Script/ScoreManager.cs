using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI whiteScoreUI;
    [SerializeField] private TextMeshProUGUI blackScoreUI;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;

    [Header("Rules")]
    [SerializeField] private int winningScore = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip pointSfx;
    [SerializeField] private AudioClip winSfx;

    [Header("Score FX")]
    [SerializeField] private float punchScale = 1.25f;
    [SerializeField] private float punchTime = 0.12f;
    [SerializeField] private Color punchColor = new Color(1f, 0.9f, 0.2f, 1f);

    [Header("Restart")]
    [SerializeField] private float restartDelaySeconds = 8f;

    private AudioSource audioSource;
    private int whiteScore;
    private int blackScore;
    private bool gameOver;

    private Coroutine whiteFx;
    private Coroutine blackFx;

    public bool GameOver => gameOver;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (winPanel != null)
            winPanel.SetActive(false);

        RefreshUI();
    }

    public void AddWhitePoint()
    {
        if (gameOver) return;

        whiteScore++;
        RefreshUI();

        if (pointSfx != null) audioSource.PlayOneShot(pointSfx);
        Punch(whiteScoreUI, ref whiteFx);

        CheckWin();
    }

    public void AddBlackPoint()
    {
        if (gameOver) return;

        blackScore++;
        RefreshUI();

        if (pointSfx != null) audioSource.PlayOneShot(pointSfx);
        Punch(blackScoreUI, ref blackFx);

        CheckWin();
    }

    private void RefreshUI()
    {
        if (whiteScoreUI != null) whiteScoreUI.text = whiteScore.ToString();
        if (blackScoreUI != null) blackScoreUI.text = blackScore.ToString();
    }

    private void Punch(TextMeshProUGUI ui, ref Coroutine routine)
    {
        if (ui == null) return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PunchRoutine(ui));
    }

    private IEnumerator PunchRoutine(TextMeshProUGUI ui)
    {
        Transform t = ui.transform;
        Vector3 baseScale = t.localScale;
        Color baseColor = ui.color;

        ui.color = punchColor;
        t.localScale = baseScale * punchScale;

        yield return new WaitForSecondsRealtime(punchTime);

        ui.color = baseColor;
        t.localScale = baseScale;
    }

    private void CheckWin()
    {
        if (whiteScore >= winningScore) EndGame("White");
        else if (blackScore >= winningScore) EndGame("Black");
    }

    private void EndGame(string winner)
    {
        gameOver = true;

        if (winText != null) winText.text = $"{winner} wins!";
        if (winPanel != null) winPanel.SetActive(true);

        if (winSfx != null) audioSource.PlayOneShot(winSfx);

        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSecondsRealtime(restartDelaySeconds);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}