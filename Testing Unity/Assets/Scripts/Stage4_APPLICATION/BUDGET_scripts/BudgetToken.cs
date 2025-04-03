using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
public class BudgetToken : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI valueText;
    public RectTransform rectTransform;

    [Header("Settings")]
    public float value;
    public float fallSpeed = 100f;
    
    private Canvas canvas;
    private bool isDestroyed = false;
    private Collider2D myCollider;
    private Rigidbody2D myRigidbody;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("Token has no Collider2D! Adding one...");
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (!myCollider.isTrigger)
        {
            Debug.LogWarning("Token collider is not a trigger. Setting isTrigger to true.");
            myCollider.isTrigger = true;
        }

        // Add a Rigidbody2D if not present (needed for some 2D collision scenarios)
        myRigidbody = GetComponent<Rigidbody2D>();
        if (myRigidbody == null)
        {
            myRigidbody = gameObject.AddComponent<Rigidbody2D>();
            myRigidbody.bodyType = RigidbodyType2D.Kinematic; // We move it ourselves
            myRigidbody.gravityScale = 0;   // No gravity
        }

        Debug.Log($"Token {GetInstanceID()} initialized: Collider={myCollider.GetType().Name}, isTrigger={myCollider.isTrigger}, Rigidbody2D={myRigidbody != null}");
    }

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        UpdateValueDisplay();
        Debug.Log($"Token {GetInstanceID()} started: Value=${value:N2}, Position={rectTransform.anchoredPosition}");
    }

    private void Update()
    {
        if (BudgetGameManager.Instance != null && BudgetGameManager.Instance.IsGameOver)
        {
            return;
        }

        if (!isDestroyed)
        {
            // Make the token fall
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Check if token has fallen off screen
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            if (corners[0].y < 0)
            {
                Debug.Log($"Token {GetInstanceID()} fell off screen and will be destroyed");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Token {GetInstanceID()} trigger entered with: {other.gameObject.name}");
        
        // Check if collided with basket
        BasketController basket = other.GetComponent<BasketController>();
        if (basket != null)
        {
            Debug.Log($"Token {GetInstanceID()} collided with basket directly");
        }
    }

    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = $"${value:N0}";
        }
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateValueDisplay();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        Debug.Log($"Token {GetInstanceID()} was destroyed");
    }
} 