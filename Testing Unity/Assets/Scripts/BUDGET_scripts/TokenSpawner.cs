using UnityEngine;
using System.Collections.Generic;

public class TokenSpawner : MonoBehaviour
{
    public GameObject budgetTokenPrefab;
    public RectTransform spawnArea;
    public int tokensPerMonth = 8;
    public float minValue = 100f;
    public float maxValue = 3000f;
    public float spawnPadding = 10f;

    private BudgetGameManager gameManager;
    private List<BudgetToken> currentTokens = new List<BudgetToken>();

    private void Start()
    {
        gameManager = FindFirstObjectByType<BudgetGameManager>();
        if (gameManager != null)
        {
            gameManager.onMonthStart.AddListener(SpawnTokens);
            gameManager.onMonthComplete.AddListener(ClearTokens);
        }
    }

    public void SpawnTokens()
    {
        ClearTokens();

        for (int i = 0; i < tokensPerMonth; i++)
        {
            SpawnToken();
        }
    }

    private void SpawnToken()
    {
        if (budgetTokenPrefab == null || spawnArea == null) return;

        GameObject tokenObj = Instantiate(budgetTokenPrefab, spawnArea);
        BudgetToken token = tokenObj.GetComponent<BudgetToken>();
        
        if (token != null)
        {
            // Generate random value
            float value = Mathf.Round(Random.Range(minValue, maxValue) / 100f) * 100f; // Round to nearest 100
            token.SetValue(value);

            // Set random position within spawn area
            float xPos = Random.Range(spawnPadding, spawnArea.rect.width - spawnPadding);
            float yPos = Random.Range(spawnPadding, spawnArea.rect.height - spawnPadding);
            token.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

            currentTokens.Add(token);
        }
    }

    public void ClearTokens()
    {
        foreach (var token in currentTokens)
        {
            if (token != null && token.gameObject != null)
            {
                Destroy(token.gameObject);
            }
        }
        currentTokens.Clear();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.onMonthStart.RemoveListener(SpawnTokens);
            gameManager.onMonthComplete.RemoveListener(ClearTokens);
        }
    }
} 