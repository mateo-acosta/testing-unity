using UnityEngine;
using TMPro;

public class Castle : MonoBehaviour
{
    [Header("Castle Properties")]
    public int maxHealth = 100;
    
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    
    [Header("Components")]
    public CircleCollider2D boundaryCollider;
    
    private int currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        
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
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log($"Castle taking damage: {damage}. Current health: {currentHealth}");
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthDisplay();
        
        if (currentHealth <= 0)
        {
            Debug.Log("Castle health depleted - Game Over");
            InsuranceGameManager.Instance.GameOver();
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
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