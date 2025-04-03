using UnityEngine;
using TMPro;

public class TaxReturn : MonoBehaviour
{
    [Header("Tax Return Type")]
    public bool isCorrect;
    public TaxReturnErrorType errorType;

    [Header("UI References")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI aboveLineDeductionsText;
    public TextMeshProUGUI adjustedGrossIncomeText;
    public TextMeshProUGUI itemizedDeductionsText;
    public TextMeshProUGUI taxableIncomeText;
    public TextMeshProUGUI taxCreditsText;
    public TextMeshProUGUI finalTaxLiabilityText;

    private float income;
    private float aboveLineDeductions;
    private float itemizedDeductions;
    private float taxCredits;

    public enum TaxReturnErrorType
    {
        None,
        TaxCreditsAddedInstead,
        WrongCalculationOrder,
        WrongLayoutOrder,
        WrongFinalLabel,
        DeductionsAddedInstead
    }

    private void Awake()
    {
        Debug.Log($"[TaxReturn] Awake - GameObject name: {gameObject.name}");
        // Log initial state of components
        Debug.Log($"[TaxReturn] Initial component check:");
        Debug.Log($"- incomeText: {(incomeText != null ? "Set" : "Null")}");
        Debug.Log($"- aboveLineDeductionsText: {(aboveLineDeductionsText != null ? "Set" : "Null")}");
        Debug.Log($"- adjustedGrossIncomeText: {(adjustedGrossIncomeText != null ? "Set" : "Null")}");
        Debug.Log($"- itemizedDeductionsText: {(itemizedDeductionsText != null ? "Set" : "Null")}");
        Debug.Log($"- taxableIncomeText: {(taxableIncomeText != null ? "Set" : "Null")}");
        Debug.Log($"- taxCreditsText: {(taxCreditsText != null ? "Set" : "Null")}");
        Debug.Log($"- finalTaxLiabilityText: {(finalTaxLiabilityText != null ? "Set" : "Null")}");
    }

    public void InitializeValues(float income, float deductions, float credits)
    {
        Debug.Log($"[TaxReturn] InitializeValues called with income: {income}, deductions: {deductions}, credits: {credits}");
        this.income = income;
        this.aboveLineDeductions = deductions;
        this.itemizedDeductions = deductions * 0.8f;
        this.taxCredits = credits;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Debug.Log($"[TaxReturn] UpdateDisplay called on {gameObject.name}");
        
        float adjustedGrossIncome = 0;
        float taxableIncome = 0;
        float finalTaxLiability = 0;

        switch (errorType)
        {
            case TaxReturnErrorType.None:
                CalculateCorrectValues(out adjustedGrossIncome, out taxableIncome, out finalTaxLiability);
                break;

            case TaxReturnErrorType.TaxCreditsAddedInstead:
                CalculateTaxCreditsError(out adjustedGrossIncome, out taxableIncome, out finalTaxLiability);
                break;

            case TaxReturnErrorType.WrongCalculationOrder:
                CalculateWrongOrderError(out adjustedGrossIncome, out taxableIncome, out finalTaxLiability);
                break;

            case TaxReturnErrorType.DeductionsAddedInstead:
                CalculateDeductionsError(out adjustedGrossIncome, out taxableIncome, out finalTaxLiability);
                break;

            default:
                CalculateCorrectValues(out adjustedGrossIncome, out taxableIncome, out finalTaxLiability);
                break;
        }

        // Update UI texts with null checks
        if (incomeText != null) incomeText.text = $"${income:N0}";
        if (aboveLineDeductionsText != null) aboveLineDeductionsText.text = $"${aboveLineDeductions:N0}";
        if (adjustedGrossIncomeText != null) adjustedGrossIncomeText.text = $"${adjustedGrossIncome:N0}";
        if (itemizedDeductionsText != null) itemizedDeductionsText.text = $"${itemizedDeductions:N0}";
        if (taxableIncomeText != null) taxableIncomeText.text = $"${taxableIncome:N0}";

        // Only update tax credits and final liability if this isn't the layout order error prefab
        if (errorType != TaxReturnErrorType.WrongLayoutOrder)
        {
            if (taxCreditsText != null) taxCreditsText.text = $"${taxCredits:N0}";

            if (errorType == TaxReturnErrorType.WrongFinalLabel)
            {
                if (finalTaxLiabilityText != null)
                    finalTaxLiabilityText.text = $"${finalTaxLiability:N0}";
            }
            else
            {
                if (finalTaxLiabilityText != null)
                    finalTaxLiabilityText.text = $"${finalTaxLiability:N0}";
            }
        }
    }

    private void CalculateCorrectValues(out float adjustedGrossIncome, out float taxableIncome, out float finalTaxLiability)
    {
        adjustedGrossIncome = income - aboveLineDeductions;
        taxableIncome = adjustedGrossIncome - itemizedDeductions;
        finalTaxLiability = (taxableIncome * 0.2f) - taxCredits; // Simplified tax rate for game purposes
    }

    private void CalculateTaxCreditsError(out float adjustedGrossIncome, out float taxableIncome, out float finalTaxLiability)
    {
        adjustedGrossIncome = income - aboveLineDeductions;
        taxableIncome = adjustedGrossIncome - itemizedDeductions;
        finalTaxLiability = (taxableIncome * 0.2f) + taxCredits; // Adding instead of subtracting credits
    }

    private void CalculateWrongOrderError(out float adjustedGrossIncome, out float taxableIncome, out float finalTaxLiability)
    {
        adjustedGrossIncome = income - (aboveLineDeductions + itemizedDeductions);
        taxableIncome = adjustedGrossIncome;
        finalTaxLiability = (taxableIncome * 0.2f) - taxCredits;
    }

    private void CalculateDeductionsError(out float adjustedGrossIncome, out float taxableIncome, out float finalTaxLiability)
    {
        adjustedGrossIncome = income + aboveLineDeductions;
        taxableIncome = adjustedGrossIncome + itemizedDeductions;
        finalTaxLiability = (taxableIncome * 0.2f) - taxCredits;
    }
} 