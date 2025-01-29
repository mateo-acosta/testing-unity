using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragAndDropAntidote : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Antidote Properties")]
    public AntidoteType antidoteType;
    
    [Header("Drag Settings")]
    public float dragAlpha = 0.7f;
    
    private Image image;
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Color originalColor;
    private Camera mainCamera;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        
        // Store original position for resetting
        originalPosition = rectTransform.anchoredPosition;
        originalColor = image.color;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make the image semi-transparent while dragging
        Color dragColor = image.color;
        dragColor.a = dragAlpha;
        image.color = dragColor;
        
        // Ensure this image renders on top while being dragged
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Update position based on mouse/touch position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset color
        image.color = originalColor;
        
        // Check if we're over a villain
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(eventData.position);
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);
        
        bool successfulHit = false;
        foreach (Collider2D hit in hits)
        {
            Villain villain = hit.GetComponent<Villain>();
            if (villain != null)
            {
                successfulHit = villain.TryDefeatWithAntidote(antidoteType);
                if (successfulHit) break;
            }
        }
        
        // Reset position
        rectTransform.anchoredPosition = originalPosition;
    }
} 