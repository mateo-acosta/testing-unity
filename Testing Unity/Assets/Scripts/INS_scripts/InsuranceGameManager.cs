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
    public float spawnRadius = 10f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float difficultyIncreaseRate = 0.1f; // How much to decrease spawn interval per minute
    
    [Header("Game Settings")]
    public int damagePerVillain = 10;
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    
    private float nextSpawnTime;
    private float currentSpawnInterval;
    private int score;
    private bool isGameOver;
    
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
        currentSpawnInterval = maxSpawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
        score = 0;
        isGameOver = false;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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
        
        // Gradually increase difficulty
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            maxSpawnInterval - (Time.time / 60f) * difficultyIncreaseRate
        );
        
        // Spawn villains
        if (Time.time >= nextSpawnTime)
        {
            SpawnVillain();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }
    
    private void SpawnVillain()
    {
        // Random angle around the circle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnPosition = new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius,
            0
        );
        
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
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
    
    public void TakeDamage()
    {
        Castle castle = FindFirstObjectByType<Castle>();
        if (castle != null)
        {
            castle.TakeDamage(damagePerVillain);
        }
    }
    
    public void IncreaseScore()
    {
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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 