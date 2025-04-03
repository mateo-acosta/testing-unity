using UnityEngine;
using UnityEngine.EventSystems;

public class DropPanel : MonoBehaviour, IDropHandler
{
    public bool isCorrectPanel;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.tag = isCorrectPanel ? "CorrectPanel" : "IncorrectPanel";
        // Both panels should also have the "DropPanel" tag for general detection
        if (!gameObject.CompareTag("DropPanel"))
        {
            gameObject.tag = "DropPanel";
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // The DraggableTaxReturn component will handle the actual game logic
            // This just ensures the object is properly positioned if needed
            RectTransform draggedRect = eventData.pointerDrag.GetComponent<RectTransform>();
            if (draggedRect != null)
            {
                draggedRect.anchoredPosition = rectTransform.anchoredPosition;
            }
        }
    }
} 