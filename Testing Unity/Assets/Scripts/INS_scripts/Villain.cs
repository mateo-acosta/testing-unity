using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Villain : MonoBehaviour
{
    [Header("Villain Properties")]
    public VillainType villainType;
    public float moveSpeed = 2f;
    
    [Header("References")]
    private Transform castleTransform;
    private InsuranceGameManager gameManager;
    
    private void Start()
    {
        // Find the castle and game manager
        GameObject castle = GameObject.FindGameObjectWithTag("Castle");
        if (castle != null)
        {
            castleTransform = castle.transform;
        }
        
        gameManager = InsuranceGameManager.Instance;
    }
    
    private void Update()
    {
        if (castleTransform != null)
        {
            // Move towards the castle
            Vector3 direction = (castleTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            // Rotate to face the castle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 to adjust for sprite orientation
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CastleBoundary"))
        {
            // Damage the castle and destroy self
            if (gameManager != null)
            {
                gameManager.TakeDamage();
            }
            Destroy(gameObject);
        }
    }
    
    public bool TryDefeatWithAntidote(AntidoteType antidoteType)
    {
        bool isCorrectAntidote = (villainType, antidoteType) switch
        {
            (VillainType.Thief, AntidoteType.Shield) => true,
            (VillainType.Storm, AntidoteType.Tree) => true,
            (VillainType.Wildfire, AntidoteType.Water) => true,
            (VillainType.Sickness, AntidoteType.Potion) => true,
            _ => false
        };
        
        if (isCorrectAntidote)
        {
            if (gameManager != null)
            {
                gameManager.IncreaseScore();
            }
            Destroy(gameObject);
        }
        
        return isCorrectAntidote;
    }
} 