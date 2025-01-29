using UnityEngine;
using TMPro;
using System.Collections;

public class InvestmentGameManager : MonoBehaviour
{
    private const float PERIOD_DURATION = 4f;
    private const int PERIODS_PER_MONTH = 2;
    public const int TOTAL_MONTHS = 24;
    private const float MONTHLY_ALLOWANCE = 1000f;

    // Balance tracking
    [SerializeField] private TextMeshProUGUI investableCashText;
    [SerializeField] private TextMeshProUGUI portfolioValueText;
    private float investableCash;
    private float portfolioValue;

    public Security[] securities;
    public int currentPeriod = 0;
    public int currentMonth = 0;

    [SerializeField] private TextMeshProUGUI[] priceTexts;
    [SerializeField] private TextMeshProUGUI[] sharesTexts;

    [SerializeField] private TextMeshProUGUI monthText;

    [SerializeField] private GraphManager graphManager;

    private void Start()
    {
        InitializeSecurities();
        investableCash = MONTHLY_ALLOWANCE;
        UpdateAllDisplays();
        StartCoroutine(SimulationLoop());
    }

    private void InitializeSecurities()
    {
        securities = new Security[4]
        {
            new Security("Tech Stock", 100f, 0.20f),
            new Security("Retail Stock", 50f, 0.10f),
            new Security("Energy Stock", 75f, 0.15f),
            new Security("S&P ETF", 200f, 0.05f)
        };
    }

    private void UpdateAllDisplays()
    {
        UpdateInvestableCash();
        UpdatePortfolioValue();
        UpdateSecurityDisplays();
        UpdateMonthDisplay();
    }

    private void UpdateInvestableCash()
    {
        investableCashText.text = $"${investableCash:F2}";
    }

    private void UpdatePortfolioValue()
    {
        float totalSecuritiesValue = 0f;
        foreach (var security in securities)
        {
            totalSecuritiesValue += security.CurrentPrice * security.SharesOwned;
        }
        portfolioValue = totalSecuritiesValue;
        
        // Calculate return percentage based on securities value only
        float initialInvestment = MONTHLY_ALLOWANCE;
        float totalReturn = ((portfolioValue - initialInvestment) / initialInvestment) * 100f;
        
        portfolioValueText.text = $"${portfolioValue:F2}";   
    }

    private void UpdateSecurityDisplays()
    {
        for (int i = 0; i < securities.Length; i++)
        {
            if (priceTexts[i] != null)
                priceTexts[i].text = $"${securities[i].CurrentPrice:F2}";
            if (sharesTexts[i] != null)
                sharesTexts[i].text = $"Shares: {securities[i].SharesOwned}";
        }
    }

    private void UpdatePeriod()
    {
        Debug.Log($"Current Period: {currentPeriod}, Current Month: {currentMonth}");
        currentPeriod++;
        foreach (var security in securities)
        {
            security.UpdatePrice();
        }
        
        if (currentPeriod % PERIODS_PER_MONTH == 0)
        {
            currentMonth++;
            ResetMonthlyAllowance();
        }
        
        UpdateAllDisplays();
        
        if (graphManager != null)
        {
            graphManager.AddDataPoint(portfolioValue);
        }
    }

    private void ResetMonthlyAllowance()
    {
        investableCash += MONTHLY_ALLOWANCE;
        UpdateInvestableCash();
    }

    public void BuyShare(int securityIndex)
    {
        float sharePrice = securities[securityIndex].CurrentPrice;
        if (investableCash >= sharePrice)
        {
            securities[securityIndex].BuyShare();
            investableCash -= sharePrice;
            UpdateAllDisplays();
        }
    }

    public void SellShare(int securityIndex)
    {
        if (securities[securityIndex].SharesOwned > 0)
        {
            float sharePrice = securities[securityIndex].CurrentPrice;
            securities[securityIndex].SellShare();
            investableCash += sharePrice;
            UpdateAllDisplays();
        }
    }

    private void UpdateMonthDisplay()
    {
        monthText.text = $"{currentMonth + 1}";
    }

    private IEnumerator SimulationLoop()
    {
        while (currentMonth < TOTAL_MONTHS)
        {
            yield return new WaitForSeconds(PERIOD_DURATION);
            UpdatePeriod();
        }
    }

    public float GetPortfolioValue()
    {
        return portfolioValue;
    }
} 