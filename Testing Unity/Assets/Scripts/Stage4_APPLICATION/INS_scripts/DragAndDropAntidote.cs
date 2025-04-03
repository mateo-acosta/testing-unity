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
    private GameObject dragClone;
    private RectTransform cloneRectTransform;
    private Image cloneImage;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        Debug.Log($"Antidote {antidoteType} initialized at position: {rectTransform.anchoredPosition}");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Create a clone of the antidote for dragging
        dragClone = Instantiate(gameObject, transform.position, Quaternion.identity, canvas.transform);
        cloneRectTransform = dragClone.GetComponent<RectTransform>();
        cloneImage = dragClone.GetComponent<Image>();
        
        // Make the clone semi-transparent
        Color dragColor = cloneImage.color;
        dragColor.a = dragAlpha;
        cloneImage.color = dragColor;
        
        // Ensure clone renders on top while being dragged
        dragClone.transform.SetAsLastSibling();
        
        // Disable the DragAndDrop component on the clone to prevent recursive dragging
        Destroy(dragClone.GetComponent<DragAndDropAntidote>());
        
        Debug.Log($"Started dragging {antidoteType} antidote clone");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone == null) return;
        
        // Update clone position based on mouse/touch position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint))
        {
            cloneRectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragClone == null) return;
        
        Debug.Log($"Ended dragging {antidoteType} antidote clone");
        
        // Check for collision with villains using Physics2D at the clone's position
        Collider2D[] hits = Physics2D.OverlapPointAll(dragClone.transform.position);
        Debug.Log($"Found {hits.Length} potential collisions at position {dragClone.transform.position}");
        
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
        
        // Destroy the clone
        Destroy(dragClone);
        dragClone = null;
    }
} 