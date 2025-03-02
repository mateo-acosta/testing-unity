using UnityEngine;
using TMPro;
using System.Collections;

public class Castle : MonoBehaviour
{
    [Header("Castle Properties")]
    public int maxHealth = 100;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer castleSprite;
    
    [Header("Components")]
    public CircleCollider2D boundaryCollider;
    
    private int currentHealth;
    private int totalDamageTaken = 0;  // Track total damage for end-game repairs
    private InsuranceGameManager gameManager;
    private Color originalColor;
    
    private void Start()
    {
        currentHealth = maxHealth;
        totalDamageTaken = 0;
        
        gameManager = InsuranceGameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("InsuranceGameManager not found!");
        }
        
        // Store original castle color if sprite exists
        if (castleSprite != null)
        {
            originalColor = castleSprite.color;
        }
        
        // Just verify the collider is set to trigger
        if (boundaryCollider != null)
        {
            boundaryCollider.isTrigger = true;
            Debug.Log($"Castle boundary collider initialized. Radius: {boundaryCollider.radius}");
        }
        else
        {
            Debug.LogWarning("No boundary collider assigned to Castle!");
        }
        
        // Verify health text component
        if (healthText == null)
        {
            Debug.LogError("Health Text (TMP) component not assigned to Castle!");
        }
        
        UpdateHealthDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        if (gameManager == null || gameManager.IsGameOver) return;
        
        Debug.Log($"Castle taking damage: {damage}. Current health: {currentHealth}");
        
        int previousHealth = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        int actualDamage = previousHealth - currentHealth;
        
        if (actualDamage > 0)
        {
            totalDamageTaken += actualDamage;
            
            // Notify game manager of damage for end-game calculations
            gameManager.RecordCastleDamage(actualDamage);
            
            // Flash damage feedback
            StartCoroutine(FlashDamage());
            
            // Update health display
            UpdateHealthDisplay();
            
            Debug.Log($"Castle health reduced to: {currentHealth}. Total damage taken: {totalDamageTaken}");
            
            // Check if castle is destroyed
            if (currentHealth <= 0)
            {
                gameManager.GameOver();
            }
        }
    }
    
    private System.Collections.IEnumerator FlashDamage()
    {
        if (castleSprite == null) yield break;
        
        // Flash red
        castleSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original color
        castleSprite.color = originalColor;
    }
    
    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}";
        }
        else
        {
            Debug.LogWarning("Health Text component is missing!");
        }
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        if (boundaryCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, boundaryCollider.radius);
        }
    }
} 