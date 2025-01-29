using UnityEngine;
using TMPro;

public class Castle : MonoBehaviour
{
    [Header("Castle Properties")]
    public int maxHealth = 100;
    public float boundaryRadius = 2f;
    
    [Header("UI References")]
    public TextMeshProUGUI healthText;
    
    [Header("Components")]
    public CircleCollider2D boundaryCollider;
    
    private int currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
        
        // Set up boundary collider
        if (boundaryCollider != null)
        {
            boundaryCollider.radius = boundaryRadius;
            boundaryCollider.isTrigger = true;
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthDisplay();
        
        if (currentHealth <= 0)
        {
            InsuranceGameManager.Instance.GameOver();
        }
    }
    
    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"Castle Health: {currentHealth}";
        }
    }
    
    // Optional: Visualize the boundary in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, boundaryRadius);
    }
} 