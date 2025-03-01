using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InsuranceGameManager : MonoBehaviour
{
    public static InsuranceGameManager Instance { get; private set; }
    
    [Header("Villain Prefabs")]
    public GameObject thiefPrefab;
    public GameObject stormPrefab;
    public GameObject wildfirePrefab;
    public GameObject sicknessPrefab;
    
    [Header("Spawn Settings")]
    public float spawnRadius = 600f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float difficultyIncreaseRate = 0.1f;
    
    [Header("Game Settings")]
    public int damagePerVillain = 10;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public Canvas gameCanvas;
    
    [Header("Scene References")]
    public GameObject enemiesContainer; // Reference to the Enemies GameObject
    
    private float nextSpawnTime;
    private float currentSpawnInterval;
    private int score;
    private bool isGameOver;
    private RectTransform canvasRectTransform;
    
    public bool IsGameOver => isGameOver;
    
    private void Awake()
    {
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
        ResumeGame();
        currentSpawnInterval = maxSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
        score = 0;
        isGameOver = false;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (gameCanvas == null)
        {
            gameCanvas = FindFirstObjectByType<Canvas>();
        }
        canvasRectTransform = gameCanvas.GetComponent<RectTransform>();
        
        // Find or create the enemies container
        if (enemiesContainer == null)
        {
            enemiesContainer = GameObject.Find("Enemies");
            if (enemiesContainer == null)
            {
                enemiesContainer = new GameObject("Enemies");
                enemiesContainer.transform.SetParent(gameCanvas.transform);
            }
        }
        
        UpdateScoreDisplay();
    }
    
    private void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }
        
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            maxSpawnInterval - (Time.time / 60f) * difficultyIncreaseRate
        );
        
        if (Time.time >= nextSpawnTime)
        {
            SpawnVillain();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }
    
    private void SpawnVillain()
    {
        if (gameCanvas == null || isGameOver || enemiesContainer == null) return;
        
        // Find castle position
        GameObject castle = GameObject.FindGameObjectWithTag("Castle_INS");
        if (castle == null) return;
        
        RectTransform castleRect = castle.GetComponent<RectTransform>();
        Vector2 castleCenter = castleRect.anchoredPosition;
        
        // Calculate spawn position in a circle around the castle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 spawnOffset = new Vector2(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius
        );
        
        Vector2 spawnPosition = castleCenter + spawnOffset;
        
        // Choose random villain type
        GameObject prefabToSpawn = Random.Range(0, 4) switch
        {
            0 => thiefPrefab,
            1 => stormPrefab,
            2 => wildfirePrefab,
            _ => sicknessPrefab
        };
        
        if (prefabToSpawn != null)
        {
            // Instantiate as UI element under the Enemies container
            GameObject villain = Instantiate(prefabToSpawn, enemiesContainer.transform);
            RectTransform villainRect = villain.GetComponent<RectTransform>();
            if (villainRect != null)
            {
                villainRect.anchoredPosition = spawnPosition;
            }
        }
    }
    
    public void TakeDamage()
    {
        if (isGameOver) return;
        
        Castle castle = FindFirstObjectByType<Castle>();
        if (castle != null)
        {
            castle.TakeDamage(damagePerVillain);
        }
    }
    
    public void IncreaseScore()
    {
        if (isGameOver) return;
        
        score++;
        UpdateScoreDisplay();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Pause the game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Debug.Log("Game Over - Game paused");
    }
    
    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
    }
}