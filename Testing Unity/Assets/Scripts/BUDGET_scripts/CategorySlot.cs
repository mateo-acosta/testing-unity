using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CategorySlot : MonoBehaviour, IDropHandler
{
    public string categoryName;
    public TextMeshProUGUI categoryLabel;
    public TextMeshProUGUI valueText;
    public RectTransform tokenHolder;
    
    private BudgetGameManager gameManager;
    private BudgetToken currentToken;

    private void Start()
    {
        gameManager = FindFirstObjectByType<BudgetGameManager>();
        if (categoryLabel != null)
        {
            categoryLabel.text = categoryName;
        }
        UpdateValueDisplay(0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentToken != null) return; // Already has a token

        BudgetToken droppedToken = eventData.pointerDrag.GetComponent<BudgetToken>();
        if (droppedToken != null)
        {
            if (gameManager.AssignValueToCategory(categoryName, droppedToken.value))
            {
                AcceptToken(droppedToken);
            }
            else
            {
                droppedToken.ReturnToOriginalPosition();
            }
        }
    }

    private void AcceptToken(BudgetToken token)
    {
        currentToken = token;
        token.transform.SetParent(tokenHolder);
        token.transform.localPosition = Vector3.zero;
        UpdateValueDisplay(token.value);
    }

    private void UpdateValueDisplay(float value)
    {
        if (valueText != null)
        {
            valueText.text = value > 0 ? $"${value:N0}" : "Empty";
        }
    }

    public void ResetSlot()
    {
        if (currentToken != null)
        {
            Destroy(currentToken.gameObject);
            currentToken = null;
        }
        UpdateValueDisplay(0);
    }
} 