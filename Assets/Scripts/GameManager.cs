using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfig config;
    [SerializeField] private GameObject pauseMenuPanel;

    // Core features - UI
    [Header("Core Features - UI")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private GameObject gameOverPanel;

    // Extension D - High Score
    [Header("Extension D - High Score")]
    [SerializeField] private TextMeshProUGUI highScoreText;  // In-game high score display
    [SerializeField] private TextMeshProUGUI finalHighScoreText; // On Game Over screen

    public float ScrollSpeed { get; private set; }
    public float Distance { get; private set; }

    public bool IsGameOver { get; private set; } = false;

    private int highScore;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        ScrollSpeed = config.startSpeed;

        // Extension D: Load saved high score
        LoadHighScore();
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Extension D: Update high score display
        UpdateHighScoreHUD();
    }

    void Update()
    {
        // Restart Key
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        // Pause input
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOver)
        {
            TogglePause();
        }

        if (!isPaused && !IsGameOver)
        {
            ScrollSpeed = Mathf.Min(ScrollSpeed + config.speedIncreaseRate * Time.deltaTime, config.maxSpeed);
            Distance += ScrollSpeed * Time.deltaTime;

            UpdateDistanceHUD();
        }
    }

    void UpdateDistanceHUD()
    {
        if (distanceText != null)
        {
            distanceText.text = $"Distance: {Mathf.FloorToInt(Distance)}m";
        }
    }

    // Extension D: Load saved high score from PlayerPrefs
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log($"Loaded High Score: {highScore}");
    }

    // Extension D: Save new high score if current distance is higher
    void SaveHighScore()
    {
        int currentDistance = Mathf.FloorToInt(Distance);
        if (currentDistance > highScore)
        {
            highScore = currentDistance;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log($"New High Score: {highScore}");
        }
    }

    // Extension D: Update in-game high score display
    void UpdateHighScoreHUD()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"Best: {highScore}m";
        }
    }

    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        // Extension D: Save high score before showing game over
        SaveHighScore();
        UpdateHighScoreHUD();

        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = $"Distance: {Mathf.FloorToInt(Distance)}m";
            }

            // Extension D: Show high score on game over screen
            if (finalHighScoreText != null)
            {
                finalHighScoreText.text = $"Best: {highScore}m";
            }
        }

        Debug.Log($"Game Over! Distance: {Mathf.FloorToInt(Distance)}m, Best: {highScore}m");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        if (IsGameOver) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        if (IsGameOver) return;

        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}