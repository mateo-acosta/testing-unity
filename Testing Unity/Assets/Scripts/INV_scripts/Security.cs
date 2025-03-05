using UnityEngine;

[System.Serializable]
public class SecurityConfig
{
    public string name = "New Security";
    public float startingPrice = 100f;
    public float volatility = 0.1f;
    public int negativeCooldownPeriods = 2;
}

public class Security
{
    public string Name { get; private set; }
    public float CurrentPrice { get; private set; }
    public float Volatility { get; private set; }
    public int SharesOwned { get; private set; }
    
    private int periodsUntilNextNegativeAllowed = 0;
    private int negativeCooldownPeriods;

    public Security(SecurityConfig config)
    {
        Name = config.name;
        CurrentPrice = config.startingPrice;
        Volatility = config.volatility;
        SharesOwned = 0;
        negativeCooldownPeriods = config.negativeCooldownPeriods;
    }

    public void UpdatePrice()
    {
        if (periodsUntilNextNegativeAllowed > 0)
        {
            // Force positive or zero change
            float change = Random.Range(0f, Volatility);
            CurrentPrice *= (1 + change);
            periodsUntilNextNegativeAllowed--;
        }
        else
        {
            float change = Random.Range(-Volatility, Volatility);
            if (change < 0)
            {
                // If we get a negative change, set the cooldown based on security type
                periodsUntilNextNegativeAllowed = negativeCooldownPeriods;
            }
            CurrentPrice *= (1 + change);
        }
    }

    public void BuyShare()
    {
        SharesOwned++;
    }

    public void SellShare()
    {
        if (SharesOwned > 0)
            SharesOwned--;
    }
} 