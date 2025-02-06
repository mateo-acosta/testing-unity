using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TaxGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI streakText;
    public GameObject gameOverPanel;
    public GameObject tutorialPanel;

    [Header("Game Settings")]
    public float gameDuration = 180f; // 3 minutes
    public int basePoints = 100;
    public float streakMultiplier = 1.5f;
    public float timeBonusThreshold = 3f; // Seconds to make decision for bonus

    [Header("Spawn Settings")]
    public GameObject taxReportPrefab;
    public float spawnInterval = 3f;
    public Transform spawnPoint;

    // Game State
    private int currentScore;
    private int currentStreak;
    private float remainingTime;
    private bool isGameActive;
    private List<TaxReport> activeReports = new List<TaxReport>();

    // Singleton instance
    public static TaxGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowTutorial();
    }

    public void StartGame()
    {
        currentScore = 0;
        currentStreak = 0;
        remainingTime = gameDuration;
        isGameActive = true;
        UpdateUI();
        StartCoroutine(SpawnReports());
        StartCoroutine(GameTimer());
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        isGameActive = false;
    }

    private IEnumerator GameTimer()
    {
        while (remainingTime > 0 && isGameActive)
        {
            remainingTime -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }
        EndGame();
    }

    private IEnumerator SpawnReports()
    {
        while (isGameActive)
        {
            SpawnTaxReport();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnTaxReport()
    {
        GameObject reportObj = Instantiate(taxReportPrefab, spawnPoint.position, Quaternion.identity);
        TaxReport report = reportObj.GetComponent<TaxReport>();
        activeReports.Add(report);
    }

    public void HandleReportJudgment(TaxReport report, bool playerJudgment)
    {
        if (!isGameActive) return;

        bool isCorrect = (report.IsCorrectlyFiled == playerJudgment);
        float timeBonus = report.TimeToJudge < timeBonusThreshold ? 1.5f : 1f;

        if (isCorrect)
        {
            currentStreak++;
            float multiplier = currentStreak >= 3 ? streakMultiplier : 1f;
            AddPoints((int)(basePoints * multiplier * timeBonus));
        }
        else
        {
            currentStreak = 0;
            AddPoints(-basePoints);
        }

        activeReports.Remove(report);
        UpdateUI();
    }

    private void AddPoints(int points)
    {
        currentScore = Mathf.Max(0, currentScore + points);
        UpdateUI();
    }

    public void AddBonusPoints(int points)
    {
        AddPoints(points);
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {currentScore}";
        if (timerText) timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}s";
        if (streakText) streakText.text = $"Streak: {currentStreak}";
    }

    private void EndGame()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
        StopAllCoroutines();
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }
} 
