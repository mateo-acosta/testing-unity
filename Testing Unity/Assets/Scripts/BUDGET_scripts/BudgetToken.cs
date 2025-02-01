using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class BudgetToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float value;
    public TextMeshProUGUI valueText;
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;
    private Canvas canvas;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        UpdateValueDisplay();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void SetValue(float newValue)
    {
        value = newValue;
        UpdateValueDisplay();
    }

    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = $"${value:N0}";
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If not dropped on a valid target, return to original position
        if (eventData.pointerCurrentRaycast.gameObject == null || 
            !eventData.pointerCurrentRaycast.gameObject.GetComponent<CategorySlot>())
        {
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
} 