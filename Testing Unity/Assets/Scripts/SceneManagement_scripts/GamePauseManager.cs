using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject pausePanel;  // Assign in inspector
    
    private bool isPaused = false;
    private float previousTimeScale;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Ensure pause panel is hidden at start
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        if (isPaused) return;
        
        // Store current time scale
        previousTimeScale = Time.timeScale;
        
        // Set time scale to 0 (pause)
        Time.timeScale = 0f;
        
        // Show pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        isPaused = true;
    }
    
    public void ResumeGame()
    {
        if (!isPaused) return;
        
        // Restore previous time scale
        Time.timeScale = previousTimeScale;
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        isPaused = false;
    }
    
    private void OnDestroy()
    {
        // Ensure time scale is restored when object is destroyed
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
} 