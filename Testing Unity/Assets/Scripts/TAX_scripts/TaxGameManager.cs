using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TaxGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameDuration = 180f; // 3 minutes in seconds
    public float remainingTime;
    public bool isGameActive = false;

    [Header("Score Settings")]
    public int currentScore = 0;
    public int currentStreak = 0;
    public int highestStreak = 0;

    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highestStreakText;
    [SerializeField] private Button approveButton;
    [SerializeField] private Button discardButton;

    [Header("Tax Return Generation")]
    public GameObject correctTaxReturnPrefab;
    public GameObject[] incorrectTaxReturnPrefabs;
    public Transform taxReturnsParent;
    public GameObject currentTaxReturn;

    [Header("Value Ranges")]
    public Vector2 incomeRange = new Vector2(20000, 100000);
    public Vector2 deductionsRange = new Vector2(5000, 20000);
    public Vector2 creditsRange = new Vector2(1000, 5000);

    private void Awake()
    {
        // Remove the mainCanvas check as it's no longer needed
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        remainingTime = gameDuration;
        currentScore = 0;
        currentStreak = 0;
        highestStreak = 0;
        isGameActive = true;
        gameOverPanel.SetActive(false);

        // Set up button listeners
        if (approveButton != null)
        {
            approveButton.onClick.RemoveAllListeners();
            approveButton.onClick.AddListener(() => ClassifyCurrentTaxReturn(true));
        }
        if (discardButton != null)
        {
            discardButton.onClick.RemoveAllListeners();
            discardButton.onClick.AddListener(() => ClassifyCurrentTaxReturn(false));
        }

        UpdateUI();
        SpawnNewTaxReturn();
        StartCoroutine(GameTimer());
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        scoreText.text = $"Score: {currentScore}";
        streakText.text = $"Streak: {currentStreak}";
    }

    private IEnumerator GameTimer()
    {
        while (remainingTime > 0 && isGameActive)
        {
            yield return new WaitForSeconds(1f);
            remainingTime--;
            UpdateUI();

            if (remainingTime <= 0)
            {
                EndGame();
            }
        }
    }

    private void ClassifyCurrentTaxReturn(bool classifiedAsCorrect)
    {
        if (currentTaxReturn != null)
        {
            TaxReturnConveyor conveyor = currentTaxReturn.GetComponent<TaxReturnConveyor>();
            if (conveyor != null)
            {
                conveyor.ClassifyTaxReturn(classifiedAsCorrect);
            }
        }
    }

    public void SpawnNewTaxReturn()
    {
        // Ensure any existing tax return is destroyed
        if (currentTaxReturn != null)
        {
            Destroy(currentTaxReturn);
            currentTaxReturn = null;
        }

        float randomValue = Random.value;
        GameObject prefabToSpawn;

        // 20% chance for correct prefab
        if (randomValue < 0.2f)
        {
            prefabToSpawn = correctTaxReturnPrefab;
        }
        else
        {
            // Equal distribution among incorrect prefabs
            int incorrectIndex = Mathf.FloorToInt((randomValue - 0.2f) * 5f);
            incorrectIndex = Mathf.Clamp(incorrectIndex, 0, incorrectTaxReturnPrefabs.Length - 1);
            prefabToSpawn = incorrectTaxReturnPrefabs[incorrectIndex];
        }

        // Instantiate as a child of the TaxReturns parent
        currentTaxReturn = Instantiate(prefabToSpawn, taxReturnsParent);
        
        // Initialize the tax return values
        TaxReturn taxReturn = currentTaxReturn.GetComponent<TaxReturn>();
        if (taxReturn != null)
        {
            InitializeTaxReturnValues(taxReturn);
        }
    }

    private void InitializeTaxReturnValues(TaxReturn taxReturn)
    {
        if (taxReturn != null)
        {
            float income = RoundToHundred(Random.Range(incomeRange.x, incomeRange.y));
            float deductions = RoundToHundred(Random.Range(deductionsRange.x, deductionsRange.y));
            float credits = RoundToHundred(Random.Range(creditsRange.x, creditsRange.y));
            taxReturn.InitializeValues(income, deductions, credits);
        }
    }

    private float RoundToHundred(float value)
    {
        return Mathf.Round(value / 100f) * 100f;
    }

    public void HandleTaxReturnClassification(bool playerClassifiedAsCorrect, bool actuallyCorrect, bool isTimerExpired)
    {
        Debug.Log($"[TaxGameManager] Classification - Player classified as correct: {playerClassifiedAsCorrect}, Actually correct: {actuallyCorrect}, Timer expired: {isTimerExpired}");
        
        // Correct classification:
        // 1. Correct tax return is placed in correct bucket (playerClassifiedAsCorrect=true, actuallyCorrect=true)
        // 2. Incorrect tax return is placed in incorrect bucket (playerClassifiedAsCorrect=false, actuallyCorrect=false)
        bool isCorrectClassification = (playerClassifiedAsCorrect && actuallyCorrect) || (!playerClassifiedAsCorrect && !actuallyCorrect);
        Debug.Log($"[TaxGameManager] Is classification correct? {isCorrectClassification}");
        
        if (isCorrectClassification)
        {
            // Player correctly classified the tax return
            currentScore += 100;
            
            // Only increase streak if the timer hasn't expired
            if (!isTimerExpired)
            {
                currentStreak++;
                highestStreak = Mathf.Max(highestStreak, currentStreak);
                Debug.Log($"[TaxGameManager] Streak increased to: {currentStreak}");
            }
            else
            {
                Debug.Log("[TaxGameManager] Correct classification but timer expired - no streak increase");
            }
        }
        else
        {
            // Player incorrectly classified the tax return
            currentStreak = 0;
            Debug.Log($"[TaxGameManager] Streak reset to 0");
        }

        UpdateUI();
        SpawnNewTaxReturn();
    }

    private void EndGame()
    {
        isGameActive = false;
        if (currentTaxReturn != null)
        {
            Destroy(currentTaxReturn);
            currentTaxReturn = null;
        }
        
        gameOverPanel.SetActive(true);
        finalScoreText.text = $"Final Score: {currentScore}";
        highestStreakText.text = $"Highest Streak: {highestStreak}";
    }

    public void RestartGame()
    {
        InitializeGame();
    }
} 