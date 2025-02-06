using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TaxReport : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    public TextMeshProUGUI incomeText;
    public TextMeshProUGUI filingStatusText;
    public TextMeshProUGUI taxBracketText;
    public TextMeshProUGUI totalTaxText;
    public Image reportBackground;

    [Header("Drag Settings")]
    public float dragSpeed = 1f;
    public Color dragColor = new Color(1f, 1f, 1f, 0.8f);

    // Tax Report Data
    public float AnnualIncome { get; private set; }
    public string FilingStatus { get; private set; }
    public float TaxBracketRate { get; private set; }
    public float TotalTax { get; private set; }
    public bool IsCorrectlyFiled { get; private set; }
    public float TimeToJudge { get; private set; }

    // Drag state
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        if (!canvasGroup)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        originalPosition = rectTransform.position;
        originalScale = rectTransform.localScale;
        originalColor = reportBackground.color;
        GenerateRandomTaxData();
        UpdateUI();
        TimeToJudge = 0f;
    }

    private void Update()
    {
        if (TaxGameManager.Instance.IsGameActive() && !isDragging)
        {
            TimeToJudge += Time.deltaTime;
        }
    }

    private void GenerateRandomTaxData()
    {
        // Generate random income between $30,000 and $500,000
        AnnualIncome = Random.Range(30000f, 500000f);
        
        // Random filing status
        string[] statuses = { "Single", "Married Filing Jointly", "Head of Household" };
        FilingStatus = statuses[Random.Range(0, statuses.Length)];
        
        // Calculate correct tax bracket (simplified for example)
        if (AnnualIncome <= 50000f)
            TaxBracketRate = 0.12f;
        else if (AnnualIncome <= 100000f)
            TaxBracketRate = 0.22f;
        else if (AnnualIncome <= 200000f)
            TaxBracketRate = 0.24f;
        else
            TaxBracketRate = 0.32f;

        // Calculate total tax
        TotalTax = AnnualIncome * TaxBracketRate;
        
        // 80% chance of being correctly filed
        IsCorrectlyFiled = Random.value < 0.8f;
        
        // If incorrectly filed, adjust the displayed tax bracket
        if (!IsCorrectlyFiled)
        {
            float incorrectRate;
            do
            {
                incorrectRate = Random.Range(0.1f, 0.4f);
            } while (Mathf.Approximately(incorrectRate, TaxBracketRate));
            
            TaxBracketRate = incorrectRate;
            TotalTax = AnnualIncome * TaxBracketRate;
        }
    }

    private void UpdateUI()
    {
        incomeText.text = $"Annual Income: ${AnnualIncome:N0}";
        filingStatusText.text = $"Filing Status: {FilingStatus}";
        taxBracketText.text = $"Tax Bracket: {TaxBracketRate:P0}";
        totalTaxText.text = $"Total Tax: ${TotalTax:N0}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TaxGameManager.Instance.IsGameActive()) return;
        
        isDragging = true;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.localScale = originalScale * 1.1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!TaxGameManager.Instance.IsGameActive()) return;
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor * dragSpeed;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!TaxGameManager.Instance.IsGameActive()) return;
        
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.localScale = originalScale;

        // Reset position if not dropped in a valid zone
        rectTransform.position = originalPosition;
    }
} 