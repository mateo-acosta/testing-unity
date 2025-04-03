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
    private int tokensSpawned = 0;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        SetNextSpawnTime();
        
        // Validate the token prefab
        ValidateTokenPrefab();
        
        Debug.Log("WaterfallSpawner initialized");
    }

    private void ValidateTokenPrefab()
    {
        if (budgetTokenPrefab == null)
        {
            Debug.LogError("Budget token prefab is null!");
            return;
        }
        
        // Check if the prefab has the necessary components
        BudgetToken tokenComponent = budgetTokenPrefab.GetComponent<BudgetToken>();
        if (tokenComponent == null)
        {
            Debug.LogError("Token prefab does not have a BudgetToken component!");
        }
        
        Collider2D collider = budgetTokenPrefab.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Token prefab does not have a Collider2D component!");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning("Token prefab's collider is not set as a trigger!");
        }
        
        Debug.Log("Token prefab validation complete");
    }

    private void Update()
    {
        if (BudgetGameManager.Instance != null && BudgetGameManager.Instance.IsGameOver)
        {
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnToken();
            SetNextSpawnTime();
        }
    }

    private void SpawnToken()
    {
        if (budgetTokenPrefab == null || spawnArea == null)
        {
            Debug.LogError("Cannot spawn token: prefab or spawn area is null");
            return;
        }

        // Create token
        GameObject tokenObj = Instantiate(budgetTokenPrefab, spawnArea);
        tokensSpawned++;
        
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
            
            // Ensure the token has a collider
            EnsureTokenHasCollider(tokenObj);
            
            Debug.Log($"Spawned token #{tokensSpawned} at position {randomX}, 0 with value ${randomValue}");
        }
        else
        {
            Debug.LogError($"Spawned token is missing components - RectTransform: {tokenRect != null}, BudgetToken: {token != null}");
        }
    }
    
    private void EnsureTokenHasCollider(GameObject tokenObj)
    {
        Collider2D collider = tokenObj.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = tokenObj.AddComponent<BoxCollider2D>();
            Debug.LogWarning($"Added missing BoxCollider2D to spawned token #{tokensSpawned}");
        }
        
        // Ensure it's a trigger
        collider.isTrigger = true;
        
        // Add Rigidbody2D if it doesn't exist (needed for some collision scenarios)
        Rigidbody2D rb = tokenObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = tokenObj.AddComponent<Rigidbody2D>();
            // Using bodyType instead of isKinematic (which is obsolete)
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            Debug.LogWarning($"Added missing Rigidbody2D to spawned token #{tokensSpawned}");
        }
        
        // Set a meaningful name for debugging
        tokenObj.name = $"Token_{tokensSpawned}_Value_{tokenObj.GetComponent<BudgetToken>().value}";
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
} 