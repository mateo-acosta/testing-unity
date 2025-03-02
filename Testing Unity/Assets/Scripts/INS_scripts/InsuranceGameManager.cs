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
    public float gameDuration = 60f; // Duration in seconds
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI fundsText;
    public GameObject gameOverPanel;
    public Canvas gameCanvas;
    
    [Header("Scene References")]
    public GameObject enemiesContainer;
    
    [Header("Cash System")]
    public float repairCostPerHP = 10f;
    public TextMeshProUGUI finalCashText;
    
    private float nextSpawnTime;
    private float currentSpawnInterval;
    private int score;
    private bool isGameOver;
    private bool isGameStarted;
    private RectTransform canvasRectTransform;
    private float remainingTime;
    private float initialCash = 1000f;
    private float forceFieldCost = 0f;
    private float currentCash = 0f;
    private float totalRepairCost = 0f;
    private int totalCastleDamage = 0;  // Track total castle damage for end-game repairs
    
    public bool IsGameOver => isGameOver;
    public bool IsGameStarted => isGameStarted;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGameManager()
    {
        // Initialize basic components and references
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
        
        // Initialize game state
        isGameStarted = false;
        isGameOver = false;
        score = 0;
        remainingTime = gameDuration;
        currentSpawnInterval = maxSpawnInterval;
        
        // Initialize UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateScoreDisplay();
        UpdateTimerDisplay();
        UpdateFundsDisplay();
        
        // Pause the game until difficulty is selected
        Time.timeScale = 0f;
    }
    
    public void StartGame()
    {
        if (isGameStarted || isGameOver) return;
        
        isGameStarted = true;
        Time.timeScale = 1f;
        nextSpawnTime = Time.time + currentSpawnInterval;
        Debug.Log("Game started - Timer and spawning initialized");
    }
    
    private void Update()
    {
        if (!isGameStarted || isGameOver) return;
        
        // Update timer
        remainingTime -= Time.deltaTime;
        UpdateTimerDisplay();
        
        // Check for time-based game over
        if (remainingTime <= 0)
        {
            GameOver();
            return;
        }
        
        // Update spawn interval based on time
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            maxSpawnInterval - (Time.time / 60f) * difficultyIncreaseRate
        );
        
        // Handle villain spawning
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
        
        // First try to find a forcefield to take the damage
        GameObject castle = GameObject.FindGameObjectWithTag("Castle_INS");
        if (castle != null)
        {
            Forcefield forcefield = castle.GetComponentInChildren<Forcefield>();
            if (forcefield != null)
            {
                // Forcefield takes the damage
                forcefield.TakeDamage(damagePerVillain);
                return;
            }
            
            // If no forcefield or it's destroyed, castle takes damage
            Castle castleComponent = castle.GetComponent<Castle>();
            if (castleComponent != null)
            {
                castleComponent.TakeDamage(damagePerVillain);
            }
        }
    }
    
    public void IncreaseScore()
    {
        if (isGameOver) return;
        
        score++;
        // Add funds for defeating villains
        currentCash += 50f; // Reward for defeating a villain
        
        UpdateScoreDisplay();
        UpdateFundsDisplay();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void InitializeGameState(float initial, float insurance, float remaining)
    {
        initialCash = initial;
        forceFieldCost = insurance;
        currentCash = remaining;
        totalRepairCost = 0f;
        UpdateFundsDisplay();
        
        // Start the game after initialization
        StartGame();
    }
    
    public void RecordCastleDamage(int damage)
    {
        totalCastleDamage += damage;
        Debug.Log($"Recorded castle damage: {damage}. Total damage: {totalCastleDamage}");
    }
    
    private void UpdateFundsDisplay()
    {
        if (fundsText != null)
        {
            fundsText.text = $"Cash: ${currentCash:N0}";
        }
    }
    
    public void GameOver()
    {
        isGameOver = true;
        isGameStarted = false;
        Time.timeScale = 0f;
        
        // Calculate final repair costs
        float repairCost = totalCastleDamage * repairCostPerHP;
        float finalCashBeforeRepairs = currentCash;
        
        // Deduct repair costs if we can afford them
        if (currentCash >= repairCost)
        {
            currentCash -= repairCost;
            totalRepairCost += repairCost;
        }
        else
        {
            // If we can't afford full repairs, use all remaining cash
            totalRepairCost += currentCash;
            currentCash = 0;
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalCashText != null)
            {
                float finalCash = currentCash;
                finalCashText.text = $"Final Summary:\n" +
                                   $"Initial Cash: ${initialCash:N0}\n" +
                                   $"Insurance Cost: -${forceFieldCost:N0}\n" +
                                   $"Castle Damage: {totalCastleDamage} HP\n" +
                                   $"Repair Cost: -${repairCost:N0}\n" +
                                   $"Final Cash: ${finalCash:N0}";
            }
        }
        
        Debug.Log($"Game Over - Final Stats:\n" +
                 $"Total Castle Damage: {totalCastleDamage} HP\n" +
                 $"Repair Cost: ${repairCost:N0}\n" +
                 $"Cash Before Repairs: ${finalCashBeforeRepairs:N0}\n" +
                 $"Final Cash: ${currentCash:N0}");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}