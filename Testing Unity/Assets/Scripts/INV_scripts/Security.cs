using UnityEngine;

public class Security
{
    public string Name { get; private set; }
    public float CurrentPrice { get; private set; }
    public float Volatility { get; private set; }
    public int SharesOwned { get; private set; }
    
    private int periodsUntilNextNegativeAllowed = 0;
    private int negativeCooldownPeriods;

    public Security(string name, float startingPrice, float volatility)
    {
        Name = name;
        CurrentPrice = startingPrice;
        Volatility = volatility;
        SharesOwned = 0;
        
        // Set cooldown periods based on security type
        negativeCooldownPeriods = name switch
        {
            "Tech Stock" => 2,
            "Retail Stock" => 2,
            "Energy Stock" => 4,
            "S&P ETF" => 4,
            _ => 0
        };
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