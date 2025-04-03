using UnityEngine;

public class NPCBorrower : MonoBehaviour
{
    [System.Serializable]
    public enum RiskLevel
    {
        Low,
        Medium,
        High
    }

    [Header("Financial Profile")]
    public float income;
    public float debt;
    public int creditScore;
    public RiskLevel riskLevel;
    public float requestedLoanAmount;

    [Header("Internal Values")]
    [SerializeField] private float maxAcceptableInterestRate;
    [SerializeField] private float repaymentProbability;

    // Constants for generating profiles
    private const float MIN_INCOME = 30000f;
    private const float MAX_INCOME = 150000f;
    private const float MIN_DEBT_RATIO = 0.1f;
    private const float MAX_DEBT_RATIO = 0.8f;
    private const int MIN_CREDIT_SCORE = 300;
    private const int MAX_CREDIT_SCORE = 850;
    private const float MIN_LOAN_AMOUNT = 5000f;
    private const float MAX_LOAN_AMOUNT = 50000f;

    public void GenerateProfile()
    {
        // Generate base financial data
        income = Random.Range(MIN_INCOME, MAX_INCOME);
        float debtRatio = Random.Range(MIN_DEBT_RATIO, MAX_DEBT_RATIO);
        debt = income * debtRatio;
        creditScore = Random.Range(MIN_CREDIT_SCORE, MAX_CREDIT_SCORE);
        
        // Determine risk level based on credit score
        if (creditScore >= 700)
        {
            riskLevel = RiskLevel.Low;
            maxAcceptableInterestRate = Random.Range(0.05f, 0.12f); // 5-12%
            repaymentProbability = Random.Range(0.85f, 0.95f); // 85-95%
        }
        else if (creditScore >= 600)
        {
            riskLevel = RiskLevel.Medium;
            maxAcceptableInterestRate = Random.Range(0.12f, 0.18f); // 12-18%
            repaymentProbability = Random.Range(0.70f, 0.85f); // 70-85%
        }
        else
        {
            riskLevel = RiskLevel.High;
            maxAcceptableInterestRate = Random.Range(0.18f, 0.25f); // 18-25%
            repaymentProbability = Random.Range(0.40f, 0.70f); // 40-70%
        }

        // Generate requested loan amount
        requestedLoanAmount = Random.Range(MIN_LOAN_AMOUNT, MAX_LOAN_AMOUNT);
        requestedLoanAmount = Mathf.Round(requestedLoanAmount / 1000f) * 1000f; // Round to nearest thousand
    }

    public bool WillAcceptInterestRate(float offeredRate)
    {
        return offeredRate <= maxAcceptableInterestRate;
    }

    public bool SimulateRepayment()
    {
        return Random.value <= repaymentProbability;
    }

    public string GetRiskLevelString()
    {
        return riskLevel switch
        {
            RiskLevel.Low => "Low Risk",
            RiskLevel.Medium => "Medium Risk",
            RiskLevel.High => "High Risk",
            _ => "Unknown Risk"
        };
    }

    // Helper method to format currency values
    public static string FormatCurrency(float amount)
    {
        return $"${amount:N2}";
    }
} 