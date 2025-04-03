using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class BasketController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 500f;
    public float horizontalBoundary = 400f;

    [Header("References")]
    public RectTransform basketRectTransform;

    private Canvas canvas;
    private BudgetGameManager gameManager;
    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("Basket has no Collider2D! Adding one...");
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (!myCollider.isTrigger)
        {
            Debug.LogWarning("Basket collider is not a trigger. Setting isTrigger to true.");
            myCollider.isTrigger = true;
        }

        Debug.Log($"Basket collider initialized: {myCollider.GetType().Name}, isTrigger={myCollider.isTrigger}, enabled={myCollider.enabled}");
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        gameManager = BudgetGameManager.Instance;
        
        if (basketRectTransform == null)
        {
            basketRectTransform = GetComponent<RectTransform>();
        }

        Debug.Log($"Basket started at position: {basketRectTransform.anchoredPosition}");
    }

    private void Update()
    {
        if (gameManager != null && gameManager.IsGameOver) return;

        // Get horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");

        // Calculate movement
        float movement = horizontalInput * moveSpeed * Time.deltaTime;
        Vector2 currentPosition = basketRectTransform.anchoredPosition;

        // Apply movement with boundaries
        currentPosition.x = Mathf.Clamp(currentPosition.x + movement, -horizontalBoundary, horizontalBoundary);
        basketRectTransform.anchoredPosition = currentPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Basket trigger detected collision with: {other.gameObject.name}");

        BudgetToken token = other.GetComponent<BudgetToken>();
        if (token != null)
        {
            Debug.Log($"Token caught! Value: ${token.value}");
            gameManager.OnTokenCaught(token);
            Destroy(token.gameObject);
        }
        else
        {
            Debug.LogWarning($"Collision with non-token object: {other.gameObject.name}");
        }
    }

    // Alternative collision detection method in case OnTriggerEnter2D isn't working
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Basket collision detected with: {collision.gameObject.name}");
        
        BudgetToken token = collision.gameObject.GetComponent<BudgetToken>();
        if (token != null)
        {
            Debug.Log($"Token caught via collision! Value: ${token.value}");
            gameManager.OnTokenCaught(token);
            Destroy(collision.gameObject);
        }
    }
} 