using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BudgetGameManager : MonoBehaviour
{
    public static BudgetGameManager Instance { get; private set; }
    public bool IsGameOver { get; private set; } = false;

    [Header("Game Settings")]
    public float monthlyIncome = 5000f;
    public float savingsGoal = 100000f;
    public float totalAccumulatedSavings = 0f;
    public int currentMonth = 1;

    [Header("UI References")]
    public Slider savingsProgressSlider;
    public TextMeshProUGUI monthCountText;
    public TextMeshProUGUI totalSavingsText;
    public GameObject gameOverPanel;

    [Header("Category Management")]
    public CategorySlot[] categorySlots;
    private int filledSlotsCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        UpdateMonthDisplay();
        UpdateSavingsDisplay();
        savingsProgressSlider.maxValue = savingsGoal;
        savingsProgressSlider.value = 0;
    }

    public void OnCategorySlotFilled()
    {
        filledSlotsCount++;
        if (filledSlotsCount >= categorySlots.Length)
        {
            CompleteMonth();
        }
    }

    public void OnCategorySlotEmptied()
    {
        filledSlotsCount = Mathf.Max(0, filledSlotsCount - 1);
    }

    private void CompleteMonth()
    {
        // Calculate total expenses
        float totalExpenses = 0f;
        foreach (CategorySlot slot in categorySlots)
        {
            if (slot.currentToken != null)
            {
                totalExpenses += slot.currentToken.value;
            }
        }

        // Calculate and add monthly savings
        float monthlySavings = monthlyIncome - totalExpenses;
        totalAccumulatedSavings += monthlySavings;

        // Update UI and game state
        currentMonth++;
        UpdateMonthDisplay();
        UpdateSavingsDisplay();

        // Reset for next month
        ResetMonth();

        // Check win condition
        if (totalAccumulatedSavings >= savingsGoal)
        {
            GameComplete();
        }
    }

    private void ResetMonth()
    {
        filledSlotsCount = 0;
        foreach (CategorySlot slot in categorySlots)
        {
            slot.ResetSlot();
        }
    }

    private void UpdateMonthDisplay()
    {
        if (monthCountText != null)
        {
            monthCountText.text = $"Month: {currentMonth}";
        }
    }

    private void UpdateSavingsDisplay()
    {
        if (savingsProgressSlider != null)
        {
            savingsProgressSlider.value = totalAccumulatedSavings;
        }
        if (totalSavingsText != null)
        {
            totalSavingsText.text = $"${totalAccumulatedSavings:N0} / {savingsGoal:N0}";
        }
    }

    private void GameComplete()
    {
        Debug.Log("Congratulations! Savings goal reached!");
        IsGameOver = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
} 