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

        // If not dropped on a valid panel, return to original position
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("DropPanel"))
        {
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        // Check if the classification is correct
        bool droppedOnCorrectPanel = eventData.pointerEnter.CompareTag("CorrectPanel");
        gameManager.HandleTaxReturnClassification(droppedOnCorrectPanel, taxReturn.isCorrect);
    }
} 