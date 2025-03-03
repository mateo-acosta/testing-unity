using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(TaxReturn))]
public class TaxReturnConveyor : MonoBehaviour
{
    private RectTransform rectTransform;
    private TaxReturn taxReturn;
    private TaxGameManager gameManager;
    private bool isMoving = true;
    private bool isAnimating = false;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] [Range(0.2f, 0.8f)] private float screenWidthMultiplier = 0.4f;
    [SerializeField] [Range(0.2f, 0.8f)] private float screenHeightMultiplier = 0.4f;

    [Header("Animation Settings")]
    [SerializeField] private float approveAnimationDuration = 1f;
    [SerializeField] private float discardAnimationDuration = 1f;
    [SerializeField] private Ease approveEaseType = Ease.InOutQuad;
    [SerializeField] private Ease discardEaseType = Ease.InOutQuad;

    private Vector2 centerPosition;
    private Vector2 startPosition;
    private Vector2 discardPosition;
    private Vector2 approvePosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        taxReturn = GetComponent<TaxReturn>();
        gameManager = FindFirstObjectByType<TaxGameManager>();

        // Calculate positions based on screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Set positions relative to screen size using inspector-configurable multipliers
        startPosition = new Vector2(-screenWidth * screenWidthMultiplier, 0);
        centerPosition = new Vector2(0, 0);
        discardPosition = new Vector2(0, screenHeight * screenHeightMultiplier);
        approvePosition = new Vector2(screenWidth * screenWidthMultiplier, 0);

        // Set initial position
        rectTransform.anchoredPosition = startPosition;
    }

    private void Update()
    {
        if (!gameManager.isGameActive || isAnimating) return;

        if (isMoving)
        {
            // Move towards center
            Vector2 targetPos = centerPosition;
            Vector2 currentPos = rectTransform.anchoredPosition;
            float step = moveSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.MoveTowards(currentPos, targetPos, step);

            // Check if reached center
            if (Vector2.Distance(currentPos, targetPos) < 1f)
            {
                isMoving = false;
                CheckForInput();
            }
        }
        else
        {
            CheckForInput();
        }
    }

    private void CheckForInput()
    {
        // Check for keyboard input
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ClassifyTaxReturn(false); // Classify as incorrect
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ClassifyTaxReturn(true); // Classify as correct
        }
    }

    public void ClassifyTaxReturn(bool classifiedAsCorrect)
    {
        if (isAnimating) return;
        isAnimating = true;

        // Create animation sequence
        Sequence sequence = DOTween.Sequence();

        if (classifiedAsCorrect)
        {
            // Animate to the right
            sequence.Append(
                rectTransform.DOAnchorPos(approvePosition, approveAnimationDuration)
                .SetEase(approveEaseType)
            );
        }
        else
        {
            // Animate upward and scale down
            sequence.Append(
                rectTransform.DOAnchorPos(discardPosition, discardAnimationDuration)
                .SetEase(discardEaseType)
            )
            .Join(
                rectTransform.DOScale(Vector3.zero, discardAnimationDuration)
                .SetEase(discardEaseType)
            );
        }

        // After animation completes
        sequence.OnComplete(() => {
            // Call the game manager's classification handler
            gameManager.HandleTaxReturnClassification(classifiedAsCorrect, taxReturn.isCorrect);
        });
    }

    // Optional: Add method to visualize the movement path in the editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && rectTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position + (Vector3)startPosition;
            Vector3 center = transform.position + (Vector3)centerPosition;
            Vector3 approve = transform.position + (Vector3)approvePosition;
            Vector3 discard = transform.position + (Vector3)discardPosition;

            Gizmos.DrawLine(start, center);
            Gizmos.DrawLine(center, approve);
            Gizmos.DrawLine(center, discard);
        }
    }
} 