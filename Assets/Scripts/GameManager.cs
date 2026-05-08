using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameConfig config;
    [SerializeField] private GameObject pauseMenuPanel;  // Drag your pause menu UI here

    public float ScrollSpeed { get; private set; }
    public float Distance { get; private set; }

    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        ScrollSpeed = config.startSpeed;
    }

    void Update()
    {
        // Pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // Only update game logic if not paused
        if (!isPaused)
        {
            ScrollSpeed = Mathf.Min(ScrollSpeed + config.speedIncreaseRate * Time.deltaTime, config.maxSpeed);
            Distance += ScrollSpeed * Time.deltaTime;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time before leaving
        SceneManager.LoadScene("MainMenu");
    }
}
