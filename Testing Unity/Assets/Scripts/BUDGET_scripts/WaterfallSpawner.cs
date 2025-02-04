using UnityEngine;
using UnityEngine.UI;

public class WaterfallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float minTokenValue = 100f;
    public float maxTokenValue = 1000f;
    public Vector2 spawnAreaWidth = new Vector2(-100f, 100f);
    
    [Header("References")]
    public GameObject budgetTokenPrefab;
    public RectTransform spawnArea;
    
    private float nextSpawnTime;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnToken();
            SetNextSpawnTime();
        }
    }

    private void SpawnToken()
    {
        if (budgetTokenPrefab == null || spawnArea == null) return;

        // Create token
        GameObject tokenObj = Instantiate(budgetTokenPrefab, spawnArea);
        RectTransform tokenRect = tokenObj.GetComponent<RectTransform>();
        BudgetToken token = tokenObj.GetComponent<BudgetToken>();

        if (tokenRect != null && token != null)
        {
            // Set random position within spawn area
            float randomX = Random.Range(spawnAreaWidth.x, spawnAreaWidth.y);
            tokenRect.anchoredPosition = new Vector2(randomX, 0);

            // Set random value
            float randomValue = Mathf.Round(Random.Range(minTokenValue, maxTokenValue) / 10f) * 10f; // Round to nearest 10
            token.SetValue(randomValue);
        }
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
} 