using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Forcefield : MonoBehaviour
{
    [Header("Forcefield Properties")]
    public int maxHealth = 50;
    public float width = 120f;
    public float height = 80f;
    public Color shieldColor = new Color(0.2f, 0.6f, 1f, 0.3f);
    
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    
    [Header("Debug Settings")]
    public bool showDebugInfo = true;
    
    private int currentHealth;
    private BoxCollider2D forceFieldCollider;
    private SpriteRenderer spriteRenderer;
    private InsuranceGameManager gameManager;
    private int collisionCount = 0;
    
    private void Awake()
    {
        // Get game manager reference
        gameManager = InsuranceGameManager.Instance;
        
        // Setup collider
        forceFieldCollider = gameObject.AddComponent<BoxCollider2D>();
        forceFieldCollider.size = new Vector2(width, height);
        forceFieldCollider.isTrigger = true;
        
        // Set layer to UI (to ensure proper collision with UI elements)
        gameObject.layer = LayerMask.NameToLayer("UI");
        
        // Setup visual representation
        GameObject visual = new GameObject("ForceFieldVisual");
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        
        spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateRectangleSprite();
        spriteRenderer.color = shieldColor;
        spriteRenderer.sortingOrder = 1; // Ensure forcefield renders above castle
        
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        
        LogDebug("Forcefield initialized", new Dictionary<string, object>
        {
            {"Position", transform.position},
            {"Collider Size", forceFieldCollider.size},
            {"Is Trigger", forceFieldCollider.isTrigger},
            {"Layer", gameObject.layer},
            {"Parent", transform.parent?.name ?? "None"},
            {"Local Scale", transform.localScale}
        });
    }
    
    private void OnEnable()
    {
        LogDebug("Forcefield enabled", new Dictionary<string, object>
        {
            {"Active Self", gameObject.activeSelf},
            {"Active in Hierarchy", gameObject.activeInHierarchy},
            {"Collider Enabled", forceFieldCollider?.enabled ?? false}
        });
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        collisionCount++;
        LogDebug($"Trigger Enter ({collisionCount})", new Dictionary<string, object>
        {
            {"Colliding Object", other.gameObject.name},
            {"Colliding Layer", LayerMask.LayerToName(other.gameObject.layer)},
            {"Colliding Position", other.transform.position},
            {"Colliding Tag", other.tag},
            {"Has Villain Component", other.GetComponent<Villain>() != null}
        });
        
        // Check if the colliding object is a villain
        Villain villain = other.GetComponent<Villain>();
        if (villain != null && gameManager != null && !gameManager.IsGameOver)
        {
            LogDebug("Processing villain collision", new Dictionary<string, object>
            {
                {"Villain Type", villain.villainType},
                {"Damage Amount", gameManager.damagePerVillain},
                {"Current Health", currentHealth}
            });
            
            // Take damage from the villain
            TakeDamage(gameManager.damagePerVillain);
            
            // Destroy the villain
            Destroy(other.gameObject);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (showDebugInfo)
        {
            LogDebug("Trigger Stay", new Dictionary<string, object>
            {
                {"Colliding Object", other.gameObject.name},
                {"Duration", Time.time}
            });
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        LogDebug("Trigger Exit", new Dictionary<string, object>
        {
            {"Exiting Object", other.gameObject.name},
            {"Was Villain", other.GetComponent<Villain>() != null}
        });
    }
    
    private Sprite CreateRectangleSprite()
    {
        // Create a texture with dimensions proportional to width and height
        float aspectRatio = width / height;
        int texHeight = 256;
        int texWidth = Mathf.RoundToInt(texHeight * aspectRatio);
        
        Texture2D texture = new Texture2D(texWidth, texHeight);
        
        // Fill the entire texture with white
        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texWidth, texHeight), new Vector2(0.5f, 0.5f));
    }
    
    public void TakeDamage(int damage)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        LogDebug("Taking Damage", new Dictionary<string, object>
        {
            {"Damage Amount", damage},
            {"Previous Health", previousHealth},
            {"New Health", currentHealth}
        });
        
        UpdateHealthDisplay();
        
        // Update transparency based on health percentage
        Color newColor = shieldColor;
        newColor.a = 0.3f * (currentHealth / (float)maxHealth);
        spriteRenderer.color = newColor;
        
        StartCoroutine(FlashEffect());
        
        if (currentHealth <= 0)
        {
            LogDebug("Forcefield Destroyed", new Dictionary<string, object>
            {
                {"Total Collisions", collisionCount},
                {"Final Position", transform.position}
            });
            Destroy(gameObject);
        }
    }
    
    private System.Collections.IEnumerator FlashEffect()
    {
        // Store original color
        Color originalColor = spriteRenderer.color;
        
        // Flash white
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original color
        spriteRenderer.color = originalColor;
    }
    
    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"Shield: {currentHealth}";
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
    }
    
    private void LogDebug(string message, Dictionary<string, object> details = null)
    {
        if (!showDebugInfo) return;
        
        string detailString = "";
        if (details != null)
        {
            detailString = "\n  " + string.Join("\n  ", details.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
        
        Debug.Log($"[Forcefield Debug] {message}{detailString}");
    }
    
    private void OnValidate()
    {
        if (forceFieldCollider != null)
        {
            forceFieldCollider.size = new Vector2(width, height);
            LogDebug("Collider size updated in editor", new Dictionary<string, object>
            {
                {"New Size", forceFieldCollider.size}
            });
        }
    }

    // Add physics-based collision check as backup
    private void FixedUpdate()
    {
        if (forceFieldCollider == null || gameManager == null || gameManager.IsGameOver) return;
        
        // Check for overlapping colliders
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] results = new Collider2D[10];
        int numColliders = forceFieldCollider.Overlap(filter, results);
        
        if (numColliders > 0)
        {
            LogDebug($"FixedUpdate overlap check", new Dictionary<string, object>
            {
                {"Number of overlapping colliders", numColliders},
                {"Collider names", string.Join(", ", System.Array.ConvertAll(results, c => c?.gameObject.name ?? "null").Take(numColliders))}
            });
            
            for (int i = 0; i < numColliders; i++)
            {
                if (results[i] != null)
                {
                    Villain villain = results[i].GetComponent<Villain>();
                    if (villain != null)
                    {
                        LogDebug("Processing FixedUpdate villain collision", new Dictionary<string, object>
                        {
                            {"Villain Type", villain.villainType},
                            {"Villain Position", villain.transform.position}
                        });
                        
                        // Take damage from the villain
                        TakeDamage(gameManager.damagePerVillain);
                        
                        // Destroy the villain
                        Destroy(results[i].gameObject);
                    }
                }
            }
        }
    }
} 