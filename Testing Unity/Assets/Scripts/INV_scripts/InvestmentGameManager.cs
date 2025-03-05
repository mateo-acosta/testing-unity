using UnityEngine;
using TMPro;
using System.Collections;

public class InvestmentGameManager : MonoBehaviour
{
    private const float PERIOD_DURATION = 4f;
    private const int PERIODS_PER_MONTH = 2;
    public const int TOTAL_MONTHS = 24;
    private const float MONTHLY_ALLOWANCE = 1000f;

    // Security configurations
    [SerializeField] private SecurityConfig[] securityConfigs = new SecurityConfig[]
    {
        new SecurityConfig { name = "Tech Stock", startingPrice = 100f, volatility = 0.20f, negativeCooldownPeriods = 2 },
        new SecurityConfig { name = "Retail Stock", startingPrice = 50f, volatility = 0.10f, negativeCooldownPeriods = 2 },
        new SecurityConfig { name = "Energy Stock", startingPrice = 75f, volatility = 0.15f, negativeCooldownPeriods = 4 },
        new SecurityConfig { name = "S&P ETF", startingPrice = 200f, volatility = 0.05f, negativeCooldownPeriods = 4 }
    };

    // Balance tracking
    [SerializeField] private TextMeshProUGUI investableCashText;
    [SerializeField] private TextMeshProUGUI portfolioValueText;
    [SerializeField] private TextMeshProUGUI portfolioReturnText;
    [SerializeField] private TextMeshProUGUI periodReturnText;
    private float investableCash;
    private float portfolioValue;
    private float lastPeriodPortfolioValue;
    private float totalInvested = 0f;

    public Security[] securities;
    public int currentPeriod = 0;
    public int currentMonth = 0;

    [SerializeField] private TextMeshProUGUI[] priceTexts;
    [SerializeField] private TextMeshProUGUI[] sharesTexts;

    [SerializeField] private TextMeshProUGUI monthText;

    [SerializeField] private GraphManager graphManager;
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        InitializeSecurities();
        investableCash = MONTHLY_ALLOWANCE;
        lastPeriodPortfolioValue = 0f;
        UpdateAllDisplays();
        StartCoroutine(SimulationLoop());
    }

    private void InitializeSecurities()
    {
        securities = new Security[securityConfigs.Length];
        for (int i = 0; i < securityConfigs.Length; i++)
        {
            securities[i] = new Security(securityConfigs[i]);
        }
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
        investableCashText.text = $"${investableCash:N0}";
    }

    private void UpdatePortfolioValue()
    {
        float totalSecuritiesValue = 0f;
        foreach (var security in securities)
        {
            totalSecuritiesValue += security.CurrentPrice * security.SharesOwned;
        }
        portfolioValue = totalSecuritiesValue;
        
        // Calculate total portfolio return
        float returnPercentage = totalInvested > 0 
            ? ((portfolioValue - totalInvested) / totalInvested) * 100f 
            : 0f;
        
        // Calculate period-over-period return
        float periodReturnPercentage = lastPeriodPortfolioValue > 0
            ? ((portfolioValue - lastPeriodPortfolioValue) / lastPeriodPortfolioValue) * 100f
            : 0f;

        portfolioValueText.text = $"${portfolioValue:N0}";
        portfolioReturnText.text = $"{returnPercentage:N1}%";
        periodReturnText.text = $"{periodReturnPercentage:N1}%";
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

        // Store current value before updating prices
        lastPeriodPortfolioValue = portfolioValue;

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
            totalInvested += sharePrice;
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
        
        // Show game over panel when simulation ends
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public float GetPortfolioValue()
    {
        return portfolioValue;
    }
} 