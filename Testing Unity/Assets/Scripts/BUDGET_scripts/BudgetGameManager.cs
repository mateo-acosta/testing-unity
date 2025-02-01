using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class BudgetGameManager : MonoBehaviour
{
    [System.Serializable]
    public class BudgetCategory
    {
        public string name;
        public float plannedPercentage;
        public float currentValue;
        public bool isRequired = true;
    }

    [Header("Game Settings")]
    public float monthlyIncome = 5000f;
    public float targetSavings = 100000f;
    public float currentTotalSavings;
    public int currentMonth = 1;

    [Header("Budget Categories")]
    public List<BudgetCategory> budgetCategories = new List<BudgetCategory>();

    [Header("Game Events")]
    public UnityEvent onMonthStart;
    public UnityEvent onMonthComplete;
    public UnityEvent onGameComplete;

    private float currentMonthExpenses;
    private int filledCategories;
    private float accuracyScore;
    private List<float> monthlyAccuracyScores = new List<float>();

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        currentTotalSavings = 0f;
        currentMonth = 1;
        ResetMonthlyValues();
        
        // Initialize default categories if none are set
        if (budgetCategories.Count == 0)
        {
            budgetCategories.Add(new BudgetCategory { name = "Rent", plannedPercentage = 0.3f });
            budgetCategories.Add(new BudgetCategory { name = "Groceries", plannedPercentage = 0.15f });
            budgetCategories.Add(new BudgetCategory { name = "Transportation", plannedPercentage = 0.1f });
            budgetCategories.Add(new BudgetCategory { name = "Entertainment", plannedPercentage = 0.05f });
            budgetCategories.Add(new BudgetCategory { name = "Insurance", plannedPercentage = 0.1f });
        }

        StartNewMonth();
    }

    public void StartNewMonth()
    {
        ResetMonthlyValues();
        onMonthStart?.Invoke();
    }

    private void ResetMonthlyValues()
    {
        currentMonthExpenses = 0f;
        filledCategories = 0;
        foreach (var category in budgetCategories)
        {
            category.currentValue = 0f;
        }
    }

    public bool AssignValueToCategory(string categoryName, float value)
    {
        var category = budgetCategories.Find(c => c.name == categoryName);
        if (category == null) return false;

        if (category.currentValue == 0f)
        {
            category.currentValue = value;
            currentMonthExpenses += value;
            filledCategories++;
            CheckMonthCompletion();
            return true;
        }
        return false;
    }

    private void CheckMonthCompletion()
    {
        int requiredCategories = budgetCategories.Count(c => c.isRequired);
        if (filledCategories >= requiredCategories)
        {
            CompleteMonth();
        }
    }

    private void CompleteMonth()
    {
        float monthSavings = monthlyIncome - currentMonthExpenses;
        currentTotalSavings += monthSavings;
        
        // Calculate accuracy score for this month
        float monthAccuracy = CalculateMonthAccuracy();
        monthlyAccuracyScores.Add(monthAccuracy);
        
        currentMonth++;
        onMonthComplete?.Invoke();

        if (currentTotalSavings >= targetSavings)
        {
            CompleteGame();
        }
        else
        {
            StartNewMonth();
        }
    }

    private float CalculateMonthAccuracy()
    {
        float totalDifference = 0f;
        foreach (var category in budgetCategories)
        {
            float plannedAmount = monthlyIncome * category.plannedPercentage;
            float difference = Mathf.Abs(category.currentValue - plannedAmount) / plannedAmount;
            totalDifference += difference;
        }
        return 1f - (totalDifference / budgetCategories.Count);
    }

    private void CompleteGame()
    {
        accuracyScore = monthlyAccuracyScores.Average();
        onGameComplete?.Invoke();
    }

    public float GetAccuracyScore() => accuracyScore;
    public float GetMonthlyExpenses() => currentMonthExpenses;
    public float GetRemainingTarget() => targetSavings - currentTotalSavings;
} 