using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Villain : MonoBehaviour
{
    [Header("Villain Properties")]
    public VillainType villainType;
    public float moveSpeed = 300f;
    public float collisionThreshold = 50f;
    
    [Header("Debug Settings")]
    public bool showDebugInfo = true;
    public float debugLogInterval = 1f;
    
    private RectTransform castleRectTransform;
    private RectTransform myRectTransform;
    private InsuranceGameManager gameManager;
    private Canvas canvas;
    private float nextDebugLog;
    private bool hasCollided = false;
    private Collider2D myCollider;
    private Vector2 lastPosition;
    private float totalDistanceMoved;
    
    private void Start()
    {
        // Store initial position
        lastPosition = transform.position;
        totalDistanceMoved = 0f;
        
        // Get or add collider reference
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            // Add a CircleCollider2D if no collider exists
            myCollider = gameObject.AddComponent<CircleCollider2D>();
            ((CircleCollider2D)myCollider).radius = 30f; // Adjust radius as needed
        }
        myCollider.isTrigger = true;
        
        // Set layer to UI to match forcefield
        gameObject.layer = LayerMask.NameToLayer("UI");
        
        // Find the castle and game manager
        GameObject castle = GameObject.FindGameObjectWithTag("Castle_INS");
        if (castle != null)
        {
            castleRectTransform = castle.GetComponent<RectTransform>();
            
            // Debug collider setup
            CircleCollider2D castleCollider = castle.GetComponentInChildren<CircleCollider2D>();
            if (castleCollider != null)
            {
                collisionThreshold = castleCollider.radius;
            }
            
            LogDebug("Villain initialized", new Dictionary<string, object>
            {
                {"Villain Type", villainType},
                {"Start Position", transform.position},
                {"Castle Position", castleRectTransform.position},
                {"Castle Found", castle != null},
                {"Castle Collider", castleCollider != null ? "Found" : "Missing"},
                {"Collider Type", myCollider.GetType().Name},
                {"Collider Layer", LayerMask.LayerToName(gameObject.layer)}
            });
        }
        else
        {
            LogDebug("Castle not found!", null, LogType.Error);
        }
        
        gameManager = InsuranceGameManager.Instance;
        myRectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Debug villain collider setup
        if (myCollider != null)
        {
            LogDebug("Collider setup", new Dictionary<string, object>
            {
                {"Collider Type", myCollider.GetType().Name},
                {"Is Trigger", myCollider.isTrigger},
                {"Is Enabled", myCollider.enabled},
                {"Layer", LayerMask.LayerToName(gameObject.layer)}
            });
        }
        else
        {
            LogDebug("No Collider2D found!", null, LogType.Error);
        }
    }
    
    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        
        if (castleRectTransform != null && canvas != null && !hasCollided)
        {
            Vector2 currentPos = myRectTransform.position;
            
            // Get the direction in screen space
            Vector2 castleScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, castleRectTransform.position);
            Vector2 villainScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, currentPos);
            Vector2 direction = (castleScreenPoint - villainScreenPoint).normalized;
            
            // Move towards the castle
            Vector3 movement = (Vector3)(direction * moveSpeed * Time.deltaTime * canvas.scaleFactor);
            myRectTransform.position += movement;
            
            // Update total distance moved
            totalDistanceMoved += movement.magnitude;
            
            // Rotate to face the castle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            myRectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);
            
            // Check distance to castle
            float distance = Vector2.Distance(myRectTransform.position, castleRectTransform.position);
            
            // Periodic position logging
            if (Time.time >= nextDebugLog)
            {
                LogDebug("Movement update", new Dictionary<string, object>
                {
                    {"Current Position", myRectTransform.position},
                    {"Distance to Castle", distance},
                    {"Movement Direction", direction},
                    {"Total Distance Moved", totalDistanceMoved},
                    {"Current Speed", movement.magnitude / Time.deltaTime}
                });
                
                // Check for nearby colliders
                Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(myRectTransform.position, collisionThreshold);
                if (nearbyColliders.Length > 0)
                {
                    LogDebug("Nearby colliders", new Dictionary<string, object>
                    {
                        {"Count", nearbyColliders.Length},
                        {"Types", string.Join(", ", nearbyColliders.Select(c => c.gameObject.name))}
                    });
                    
                    // Check for forcefield collision
                    foreach (Collider2D collider in nearbyColliders)
                    {
                        if (collider.GetComponent<Forcefield>() != null)
                        {
                            // Let the forcefield handle the collision through OnTriggerEnter2D
                            return;
                        }
                    }
                }
                
                nextDebugLog = Time.time + debugLogInterval;
            }
            
            // Only check for castle collision if we haven't hit a forcefield
            if (distance <= collisionThreshold)
            {
                LogDebug("Reached castle boundary", new Dictionary<string, object>
                {
                    {"Final Position", myRectTransform.position},
                    {"Total Distance Traveled", totalDistanceMoved},
                    {"Time Alive", Time.time - nextDebugLog}
                });
                
                hasCollided = true;
                if (gameManager != null && !gameManager.IsGameOver)
                {
                    gameManager.TakeDamage();
                }
                
                Destroy(gameObject);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If we've already collided, don't process any more collisions
        if (hasCollided) return;
        
        LogDebug("Trigger collision", new Dictionary<string, object>
        {
            {"Colliding With", other.gameObject.name},
            {"Collider Type", other.GetType().Name},
            {"At Position", transform.position},
            {"Other Position", other.transform.position},
            {"Is Forcefield", other.GetComponent<Forcefield>() != null}
        });
        
        // Check for forcefield collision
        Forcefield forcefield = other.GetComponent<Forcefield>();
        if (forcefield != null)
        {
            // Only set hasCollided if the forcefield is active (not destroyed)
            // This allows the villain to continue moving if it hits a destroyed forcefield
            if (forcefield.gameObject.activeInHierarchy && forcefield.enabled)
            {
                hasCollided = true;
                LogDebug("Collided with active forcefield", new Dictionary<string, object>
                {
                    {"Forcefield Name", forcefield.gameObject.name},
                    {"Forcefield Position", forcefield.transform.position}
                });
            }
            else
            {
                LogDebug("Passed through inactive/destroyed forcefield", new Dictionary<string, object>
                {
                    {"Forcefield Name", forcefield.gameObject.name},
                    {"Forcefield Position", forcefield.transform.position}
                });
            }
        }
    }
    
    public bool TryDefeatWithAntidote(AntidoteType antidoteType)
    {
        if (gameManager != null && gameManager.IsGameOver)
        {
            return false;
        }
        
        LogDebug("Antidote attempt", new Dictionary<string, object>
        {
            {"Villain Type", villainType},
            {"Antidote Type", antidoteType},
            {"Position", transform.position}
        });
        
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
            LogDebug("Villain defeated", new Dictionary<string, object>
            {
                {"Defeat Position", transform.position},
                {"Total Distance Traveled", totalDistanceMoved}
            });
            
            if (gameManager != null)
            {
                gameManager.IncreaseScore();
            }
            Destroy(gameObject);
        }
        
        return isCorrectAntidote;
    }
    
    private void LogDebug(string message, Dictionary<string, object> details = null, LogType logType = LogType.Log)
    {
        if (!showDebugInfo) return;
        
        string detailString = "";
        if (details != null)
        {
            detailString = "\n  " + string.Join("\n  ", details.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
        
        string logMessage = $"[Villain {villainType} Debug] {message}{detailString}";
        
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError(logMessage);
                break;
            case LogType.Warning:
                Debug.LogWarning(logMessage);
                break;
            default:
                Debug.Log(logMessage);
                break;
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw movement path
        if (Application.isPlaying && showDebugInfo)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lastPosition, transform.position);
            lastPosition = transform.position;
            
            // Draw detection radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, collisionThreshold);
        }
    }
}