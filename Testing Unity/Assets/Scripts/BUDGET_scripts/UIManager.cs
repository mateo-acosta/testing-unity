using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Info")]
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI expensesText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI targetRemainingText;
    public Slider progressBar;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI monthsPlayedText;
    public TextMeshProUGUI accuracyScoreText;
    public TextMeshProUGUI finalSavingsText;

    private BudgetGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<BudgetGameManager>();
        if (gameManager != null)
        {
            gameManager.onMonthStart.AddListener(UpdateUI);
            gameManager.onMonthComplete.AddListener(UpdateUI);
            gameManager.onGameComplete.AddListener(ShowGameOver);
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (gameManager == null) return;

        if (monthText != null)
            monthText.text = $"Month {gameManager.currentMonth}";

        if (incomeText != null)
            incomeText.text = $"Monthly Income: ${gameManager.monthlyIncome:N0}";

        if (expensesText != null)
            expensesText.text = $"Current Expenses: ${gameManager.GetMonthlyExpenses():N0}";

        if (savingsText != null)
            savingsText.text = $"Total Savings: ${gameManager.currentTotalSavings:N0}";

        if (targetRemainingText != null)
        {
            float remaining = gameManager.GetRemainingTarget();
            targetRemainingText.text = $"Remaining to Target: ${remaining:N0}";
        }

        if (progressBar != null)
        {
            progressBar.value = gameManager.currentTotalSavings / gameManager.targetSavings;
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (monthsPlayedText != null)
                monthsPlayedText.text = $"Months to Complete: {gameManager.currentMonth}";

            if (accuracyScoreText != null)
                accuracyScoreText.text = $"Budget Accuracy: {gameManager.GetAccuracyScore():P0}";

            if (finalSavingsText != null)
                finalSavingsText.text = $"Final Savings: ${gameManager.currentTotalSavings:N0}";
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.onMonthStart.RemoveListener(UpdateUI);
            gameManager.onMonthComplete.RemoveListener(UpdateUI);
            gameManager.onGameComplete.RemoveListener(ShowGameOver);
        }
    }
} 