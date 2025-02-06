using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CorrectionPopup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public TextMeshProUGUI questionText;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    public GameObject correctFeedback;
    public GameObject incorrectFeedback;
    public TextMeshProUGUI explanationText;

    [Header("Settings")]
    public float feedbackDisplayTime = 2f;
    public int bonusPoints = 50;

    private TaxReport currentReport;
    private float correctRate;
    private List<float> possibleRates = new List<float> { 0.12f, 0.22f, 0.24f, 0.32f };

    private void Awake()
    {
        popupPanel.SetActive(false);
        correctFeedback.SetActive(false);
        incorrectFeedback.SetActive(false);
    }

    public void ShowPopup(TaxReport report)
    {
        currentReport = report;
        correctRate = GetCorrectTaxRate(report.AnnualIncome);
        
        // Set up the question
        questionText.text = $"What should be the correct tax bracket for annual income ${report.AnnualIncome:N0}?";
        
        // Generate and shuffle choices
        List<float> choices = GenerateChoices(correctRate);
        
        // Set up the buttons
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            float rate = choices[i];
            choiceTexts[i].text = $"{rate:P0}";
            
            int index = i; // Capture the index for the lambda
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(rate));
        }
        
        popupPanel.SetActive(true);
    }

    private float GetCorrectTaxRate(float income)
    {
        if (income <= 50000f)
            return 0.12f;
        else if (income <= 100000f)
            return 0.22f;
        else if (income <= 200000f)
            return 0.24f;
        else
            return 0.32f;
    }

    private List<float> GenerateChoices(float correctRate)
    {
        List<float> choices = new List<float>();
        choices.Add(correctRate);
        
        // Add other rates that aren't the correct one
        foreach (float rate in possibleRates)
        {
            if (rate != correctRate && choices.Count < 4)
            {
                choices.Add(rate);
            }
        }
        
        // Shuffle the choices
        for (int i = choices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            float temp = choices[i];
            choices[i] = choices[j];
            choices[j] = temp;
        }
        
        return choices;
    }

    private void OnChoiceSelected(float selectedRate)
    {
        bool isCorrect = Mathf.Approximately(selectedRate, correctRate);
        
        // Show appropriate feedback
        correctFeedback.SetActive(isCorrect);
        incorrectFeedback.SetActive(!isCorrect);
        
        // Show explanation
        string explanation = GenerateExplanation(isCorrect, selectedRate);
        explanationText.text = explanation;
        
        // Award points if correct
        if (isCorrect)
        {
            TaxGameManager.Instance.AddBonusPoints(bonusPoints);
        }
        
        // Hide the popup after delay
        Invoke(nameof(HidePopup), feedbackDisplayTime);
    }

    private string GenerateExplanation(bool isCorrect, float selectedRate)
    {
        if (isCorrect)
        {
            return $"Correct! For an annual income of ${currentReport.AnnualIncome:N0}, " +
                   $"the tax bracket is {correctRate:P0} based on the current tax brackets.";
        }
        else
        {
            return $"The correct tax bracket for an annual income of ${currentReport.AnnualIncome:N0} " +
                   $"is {correctRate:P0}. You selected {selectedRate:P0}.\n\n" +
                   "Tax Brackets:\n" +
                   "Up to $50,000: 12%\n" +
                   "$50,001 - $100,000: 22%\n" +
                   "$100,001 - $200,000: 24%\n" +
                   "Over $200,000: 32%";
        }
    }

    private void HidePopup()
    {
        popupPanel.SetActive(false);
        correctFeedback.SetActive(false);
        incorrectFeedback.SetActive(false);
    }
} 