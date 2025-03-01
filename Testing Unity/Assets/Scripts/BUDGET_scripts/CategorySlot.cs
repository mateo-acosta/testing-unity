using UnityEngine;
using TMPro;

public class CategorySlot : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI categoryNameText;
    public TextMeshProUGUI valueText;
    
    [Header("Settings")]
    public string categoryName;
    
    private float currentValue = 0f;

    private void Start()
    {
        if (categoryNameText != null)
        {
            categoryNameText.text = categoryName;
        }
        UpdateValueDisplay();
    }

    public void SetValue(float value)
    {
        currentValue = value;
        UpdateValueDisplay();
        Debug.Log($"Category {categoryName} value set to ${currentValue:N2}");
    }

    public float GetValue()
    {
        return currentValue;
    }

    public void ResetSlot()
    {
        currentValue = 0f;
        UpdateValueDisplay();
        Debug.Log($"Category {categoryName} reset");
    }

    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = currentValue > 0 ? $"${currentValue:N2}" : "Not Set";
            Debug.Log($"Updated {categoryName} display to: {valueText.text}");
        }
        else
        {
            Debug.LogWarning($"Category {categoryName} has no valueText assigned!");
        }
    }
    
    // Public method to update the display, can be called from outside
    public void UpdateDisplay()
    {
        UpdateValueDisplay();
    }
} 