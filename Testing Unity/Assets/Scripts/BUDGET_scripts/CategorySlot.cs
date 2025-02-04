using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CategorySlot : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public TextMeshProUGUI categoryNameText;
    public TextMeshProUGUI valueText;
    public RectTransform dropZone;
    
    [Header("Settings")]
    public string categoryName;
    
    public BudgetToken currentToken { get; private set; }
    private BudgetGameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<BudgetGameManager>();
        if (categoryNameText != null)
        {
            categoryNameText.text = categoryName;
        }
        UpdateValueDisplay();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentToken != null) return; // Slot already filled

        BudgetToken droppedToken = eventData.pointerDrag.GetComponent<BudgetToken>();
        if (droppedToken != null)
        {
            AcceptToken(droppedToken);
        }
    }

    private void AcceptToken(BudgetToken token)
    {
        currentToken = token;
        token.transform.SetParent(dropZone);
        token.transform.localPosition = Vector3.zero;
        token.OnPlaced(this);
        
        UpdateValueDisplay();
        gameManager.OnCategorySlotFilled();
    }

    public void ResetSlot()
    {
        if (currentToken != null)
        {
            Destroy(currentToken.gameObject);
            currentToken = null;
            UpdateValueDisplay();
        }
    }

    private void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = currentToken != null ? $"${currentToken.value:N2}" : "Empty";
        }
    }
} 