using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BudgetToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public TextMeshProUGUI valueText;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float value;
    public float fallSpeed = 100f;
    
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;
    private CategorySlot currentSlot;
    private bool isDragging = false;
    private bool isPlaced = false;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        UpdateValueDisplay();
    }

    private void Update()
    {
        if (!isDragging && !isPlaced)
        {
            // Make the token fall
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Check if token has fallen off screen
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            if (corners[0].y < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        
        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        transform.SetParent(canvasRectTransform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (currentSlot == null)
        {
            // Return to original position if not dropped in a valid slot
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void OnPlaced(CategorySlot slot)
    {
        currentSlot = slot;
        isPlaced = true;
        canvasGroup.blocksRaycasts = false;
    }

    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = $"${value:N2}";
        }
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateValueDisplay();
    }
} 