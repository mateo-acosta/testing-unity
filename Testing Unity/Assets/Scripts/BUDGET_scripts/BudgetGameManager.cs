using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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
    public TextMeshProUGUI currentCategoryText;
    public TextMeshProUGUI monthlySavingsPopupText;
    public GameObject gameOverPanel;
    public GameObject monthlySavingsPopup;
    public GameObject categoryListPanel;

    [Header("Category Management")]
    public CategorySlot[] categorySlots;
    private int currentCategoryIndex = 0;
    private List<int> validCategoryIndices = new List<int>();

    private void Awake()
    {
        Instance = this;
        Debug.Log("BudgetGameManager initialized");
    }

    private void Start()
    {
        // Identify valid category indices and clean up the array
        ValidateCategorySlots();
        
        InitializeUI();
        UpdateCurrentCategoryDisplay();
        UpdateCategoryDashboard();
        
        // Debug category setup
        Debug.Log($"Game started with {categorySlots.Length} categories, {validCategoryIndices.Count} valid");
        for (int i = 0; i < categorySlots.Length; i++)
        {
            if (categorySlots[i] != null)
            {
                Debug.Log($"Category {i}: {categorySlots[i].categoryName}");
            }
            else
            {
                Debug.LogError($"Category slot {i} is null!");
            }
        }
    }

    private void ValidateCategorySlots()
    {
        validCategoryIndices.Clear();
        
        // Find valid category slots and build a list of valid indices
        for (int i = 0; i < categorySlots.Length; i++)
        {
            if (categorySlots[i] != null)
            {
                validCategoryIndices.Add(i);
            }
        }
        
        // If we found null slots, log that information
        if (validCategoryIndices.Count < categorySlots.Length)
        {
            Debug.LogWarning($"Found {categorySlots.Length - validCategoryIndices.Count} null category slots that will be skipped.");
        }
        
        // Ensure we start with a valid category
        if (validCategoryIndices.Count > 0)
        {
            currentCategoryIndex = validCategoryIndices[0];
        }
        else
        {
            Debug.LogError("No valid category slots found!");
        }
    }

    private void InitializeUI()
    {
        UpdateMonthDisplay();
        UpdateSavingsDisplay();
        savingsProgressSlider.maxValue = savingsGoal;
        savingsProgressSlider.value = 0;
        if (monthlySavingsPopup != null)
        {
            monthlySavingsPopup.SetActive(false);
        }
        Debug.Log("UI Initialized");
    }

    public void OnTokenCaught(BudgetToken token)
    {
        Debug.Log($"OnTokenCaught called for token with value: ${token.value}");
        
        if (validCategoryIndices.Count == 0)
        {
            Debug.LogError("No valid categories to assign token to!");
            return;
        }
        
        int currentValidIndex = GetValidCategoryIndexPosition(currentCategoryIndex);
        if (currentValidIndex >= validCategoryIndices.Count)
        {
            Debug.LogWarning($"Cannot assign token - all valid categories are filled");
            return;
        }

        // Get the actual category index from our valid indices list
        int actualCategoryIndex = validCategoryIndices[currentValidIndex];
        
        // Assign the token value to the current category
        CategorySlot currentCategory = categorySlots[actualCategoryIndex];
        if (currentCategory != null)
        {
            Debug.Log($"Assigning value ${token.value} to category: {currentCategory.categoryName}");
            currentCategory.SetValue(token.value);
            
            // Update dashboard
            UpdateCategoryDashboard();
            
            // Move to next valid category
            currentValidIndex++;
            if (currentValidIndex < validCategoryIndices.Count)
            {
                currentCategoryIndex = validCategoryIndices[currentValidIndex];
                Debug.Log($"Advanced to category index: {currentCategoryIndex} (valid index position: {currentValidIndex})");
                
                // Update the display for the next category
                UpdateCurrentCategoryDisplay();
            }
            else
            {
                // All valid categories filled, complete the month
                Debug.Log("All valid categories filled. Completing month...");
                StartCoroutine(CompleteMonthRoutine());
            }
        }
        else
        {
            Debug.LogError($"Unexpected null category at index {actualCategoryIndex}!");
        }
    }
    
    private int GetValidCategoryIndexPosition(int categoryIndex)
    {
        // Find the position of the category index in the valid indices list
        return validCategoryIndices.IndexOf(categoryIndex);
    }

    private IEnumerator CompleteMonthRoutine()
    {
        // Calculate total expenses and savings
        float totalExpenses = 0f;
        foreach (int index in validCategoryIndices)
        {
            CategorySlot slot = categorySlots[index];
            if (slot != null)
            {
                totalExpenses += slot.GetValue();
            }
        }

        float monthlySavings = monthlyIncome - totalExpenses;
        totalAccumulatedSavings += monthlySavings;
        
        Debug.Log($"Month {currentMonth} complete. Income: ${monthlyIncome}, Expenses: ${totalExpenses}, Savings: ${monthlySavings}");

        // Show monthly savings popup
        if (monthlySavingsPopup != null && monthlySavingsPopupText != null)
        {
            monthlySavingsPopupText.text = $"${monthlySavings:N0}";
            monthlySavingsPopup.SetActive(true);
            
            Debug.Log("Showing monthly savings popup");
            
            // Wait for X seconds
            yield return new WaitForSeconds(2f);
            
            monthlySavingsPopup.SetActive(false);
        }

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
        Debug.Log($"Resetting for month {currentMonth}");
        
        // Reset to first valid category
        if (validCategoryIndices.Count > 0)
        {
            currentCategoryIndex = validCategoryIndices[0];
        }
        
        // Reset all category values
        foreach (CategorySlot slot in categorySlots)
        {
            if (slot != null)
            {
                slot.ResetSlot();
            }
        }
        
        // Update UI
        UpdateCurrentCategoryDisplay();
        UpdateCategoryDashboard();
    }

    private void UpdateCurrentCategoryDisplay()
    {
        if (currentCategoryText != null)
        {
            // Check if we have valid categories
            if (validCategoryIndices.Count > 0)
            {
                int currentValidIndex = GetValidCategoryIndexPosition(currentCategoryIndex);
                
                // Check if we're still in a valid range
                if (currentValidIndex >= 0 && currentValidIndex < validCategoryIndices.Count)
                {
                    int actualIndex = validCategoryIndices[currentValidIndex];
                    if (categorySlots[actualIndex] != null)
                    {
                        string categoryName = categorySlots[actualIndex].categoryName;
                        currentCategoryText.text = $"Current Category: {categoryName}";
                        Debug.Log($"Updated current category display to: {categoryName}");
                        return;
                    }
                }
                
                // If we get here, all categories are filled
                currentCategoryText.text = "All Categories Filled";
                Debug.Log("All categories filled, updated display");
            }
            else
            {
                currentCategoryText.text = "No Categories Available";
                Debug.LogError("No valid categories found for display");
            }
        }
        else
        {
            Debug.LogError("currentCategoryText is null!");
        }
    }
    
    private void UpdateCategoryDashboard()
    {
        Debug.Log("Updating category dashboard");
        
        // Update all category value displays in the dashboard
        foreach (CategorySlot slot in categorySlots)
        {
            if (slot != null)
            {
                slot.UpdateDisplay();
            }
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
            totalSavingsText.text = $"${totalAccumulatedSavings:N0} / ${savingsGoal:N0}";
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