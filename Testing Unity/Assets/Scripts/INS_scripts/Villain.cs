using UnityEngine;

public class Villain : MonoBehaviour
{
    [Header("Villain Properties")]
    public VillainType villainType;
    public float moveSpeed = 300f;
    public float collisionThreshold = 50f; // Distance at which we consider collision with castle
    
    private RectTransform castleRectTransform;
    private RectTransform myRectTransform;
    private InsuranceGameManager gameManager;
    private Canvas canvas;
    private float debugLogInterval = 1f;
    private float nextDebugLog;
    private bool hasCollided = false;
    
    private void Start()
    {
        // Find the castle and game manager
        GameObject castle = GameObject.FindGameObjectWithTag("Castle_INS");
        if (castle != null)
        {
            castleRectTransform = castle.GetComponent<RectTransform>();
            Debug.Log($"Villain found castle at position: {castleRectTransform.position}");
            
            // Debug collider setup
            CircleCollider2D castleCollider = castle.GetComponentInChildren<CircleCollider2D>();
            if (castleCollider != null)
            {
                Debug.Log($"Found castle collider. IsTrigger: {castleCollider.isTrigger}, Radius: {castleCollider.radius}, Enabled: {castleCollider.enabled}");
                collisionThreshold = castleCollider.radius; // Use castle's collider radius
            }
            else
            {
                Debug.LogError("Castle has no CircleCollider2D!");
            }
        }
        else
        {
            Debug.LogWarning("Villain couldn't find castle!");
        }
        
        gameManager = InsuranceGameManager.Instance;
        myRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Debug villain collider setup
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            Debug.Log($"Villain collider setup - Type: {myCollider.GetType().Name}, IsTrigger: {myCollider.isTrigger}, Enabled: {myCollider.enabled}");
        }
        else
        {
            Debug.LogError($"Villain of type {villainType} has no Collider2D!");
        }
    }
    
    private void Update()
    {
        if (castleRectTransform != null && canvas != null && !hasCollided)
        {
            // Get the direction in screen space
            Vector2 castleScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, castleRectTransform.position);
            Vector2 villainScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, myRectTransform.position);
            Vector2 direction = (castleScreenPoint - villainScreenPoint).normalized;
            
            // Move towards the castle in screen space
            myRectTransform.position += (Vector3)(direction * moveSpeed * Time.deltaTime * canvas.scaleFactor);
            
            // Rotate to face the castle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            myRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
            
            // Check distance to castle
            float distance = Vector2.Distance(myRectTransform.position, castleRectTransform.position);
            
            // Periodic position logging
            if (Time.time >= nextDebugLog)
            {
                Debug.Log($"Villain position: {myRectTransform.position}, Distance to castle: {distance}");
                nextDebugLog = Time.time + debugLogInterval;
            }
            
            // Check for collision with castle
            if (distance <= collisionThreshold)
            {
                Debug.Log($"Villain of type {villainType} reached castle boundary at distance {distance}");
                hasCollided = true;
                if (gameManager != null)
                {
                    gameManager.TakeDamage();
                    Debug.Log("Damage dealt to castle");
                }
                else
                {
                    Debug.LogWarning("GameManager not found when trying to deal damage!");
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    public bool TryDefeatWithAntidote(AntidoteType antidoteType)
    {
        Debug.Log($"Attempting to defeat {villainType} with {antidoteType}");
        
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
            Debug.Log($"Successfully defeated {villainType} with {antidoteType}");
            if (gameManager != null)
            {
                gameManager.IncreaseScore();
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Wrong antidote! {antidoteType} cannot defeat {villainType}");
        }
        
        return isCorrectAntidote;
    }
} 