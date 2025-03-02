using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TaxReturn))]
public class DraggableTaxReturn : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private TaxReturn taxReturn;
    private TaxGameManager gameManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        taxReturn = GetComponent<TaxReturn>();
        canvas = GetComponentInParent<Canvas>();
        gameManager = FindFirstObjectByType<TaxGameManager>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameManager.isGameActive) return;
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!gameManager.isGameActive) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!gameManager.isGameActive) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // If nothing was hit, return to original position
        if (eventData.pointerEnter == null)
        {
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        Debug.Log($"[DraggableTaxReturn] Hit object: {eventData.pointerEnter.name} with tag: {eventData.pointerEnter.tag}");
        
        // Find if we hit a drop panel by checking the object and its hierarchy upward
        bool droppedOnCorrectPanel = false;
        bool validDrop = false;
        
        // Start with the hit object and check up the hierarchy
        Transform current = eventData.pointerEnter.transform;
        int hierarchyCheck = 0; // Safety counter to prevent infinite loops
        
        while (current != null && hierarchyCheck < 5) // Limit hierarchy check to 5 levels
        {
            Debug.Log($"[DraggableTaxReturn] Checking object: {current.name} with tag: {current.tag}");
            
            if (current.CompareTag("CorrectPanel"))
            {
                droppedOnCorrectPanel = true;
                validDrop = true;
                Debug.Log("[DraggableTaxReturn] Found CorrectPanel in hierarchy");
                break;
            }
            else if (current.CompareTag("IncorrectPanel"))
            {
                droppedOnCorrectPanel = false;
                validDrop = true;
                Debug.Log("[DraggableTaxReturn] Found IncorrectPanel in hierarchy");
                break;
            }
            // Also check if this object has a DropPanel component attached
            else if (current.GetComponent<DropPanel>() != null)
            {
                DropPanel dropPanel = current.GetComponent<DropPanel>();
                droppedOnCorrectPanel = dropPanel.isCorrectPanel;
                validDrop = true;
                Debug.Log($"[DraggableTaxReturn] Found DropPanel component with isCorrectPanel = {dropPanel.isCorrectPanel}");
                break;
            }
            
            current = current.parent;
            hierarchyCheck++;
        }
        
        // If no valid drop zone found, return to original position
        if (!validDrop)
        {
            Debug.Log("[DraggableTaxReturn] No valid drop zone found, returning to original position");
            rectTransform.anchoredPosition = originalPosition;
            return;
        }
        
        // Log for debugging
        Debug.Log($"[DraggableTaxReturn] Tax Return isCorrect value: {taxReturn.isCorrect}");
        Debug.Log($"[DraggableTaxReturn] Dropped on correct panel?: {droppedOnCorrectPanel}");
        
        // Process the classification with the same streak logic as before
        gameManager.HandleTaxReturnClassification(droppedOnCorrectPanel, taxReturn.isCorrect);
    }
} 