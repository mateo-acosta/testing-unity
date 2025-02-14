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
    
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        // Store original position for resetting
        originalPosition = rectTransform.anchoredPosition;
        originalColor = image.color;
        
        Debug.Log($"Antidote {antidoteType} initialized at position: {rectTransform.anchoredPosition}");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Make the image semi-transparent while dragging
        Color dragColor = image.color;
        dragColor.a = dragAlpha;
        image.color = dragColor;
        
        // Ensure this image renders on top while being dragged
        transform.SetAsLastSibling();
        
        Debug.Log($"Started dragging {antidoteType} antidote");
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
        Debug.Log($"Ended dragging {antidoteType} antidote");
        
        // Reset color
        image.color = originalColor;
        
        // Check for collision with villains using Physics2D
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        Debug.Log($"Found {hits.Length} potential collisions at position {transform.position}");
        
        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Checking collision with: {hit.gameObject.name}");
            Villain villain = hit.GetComponent<Villain>();
            if (villain != null)
            {
                Debug.Log($"Found villain of type {villain.villainType}");
                if (villain.TryDefeatWithAntidote(antidoteType))
                {
                    break;
                }
            }
        }
        
        // Reset position
        rectTransform.anchoredPosition = originalPosition;
    }
} 