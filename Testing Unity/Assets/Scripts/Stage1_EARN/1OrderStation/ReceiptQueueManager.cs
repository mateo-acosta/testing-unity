using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;

namespace BurgerGame
{
    public class ReceiptQueueManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScrollRect receiptScrollView;
        [SerializeField] private Transform receiptQueueContent;
        [SerializeField] private GameObject receiptPrefab;
        [SerializeField] private Transform currentOrderDisplay;
        [SerializeField] private TextMeshProUGUI currentOrderText;
        [SerializeField] private Button deliverButton;
        [SerializeField] private TextMeshProUGUI orderGradeText;
        [SerializeField] private GameObject orderGradePanel;
        
        [Header("Prefab References")]
        [Tooltip("Drag the TextMeshPro component from your prefab here for the order title")]
        [SerializeField] private TextMeshProUGUI prefabTitleText;
        [Tooltip("Drag the TextMeshPro component from your prefab here for the order content")]
        [SerializeField] private TextMeshProUGUI prefabContentText;
        
        [Header("UI Settings")]
        [SerializeField] private Color activeOrderColor = Color.yellow;
        [SerializeField] private Color queuedOrderColor = Color.white;
        [SerializeField] private Color correctOrderColor = Color.green;
        [SerializeField] private Color incorrectOrderColor = Color.red;
        [SerializeField] private float receiptSpacing = 5f;
        [SerializeField] private float gradeDisplayTime = 3f;

        private Dictionary<string, GameObject> receiptInstances = new Dictionary<string, GameObject>();
        private Dictionary<string, int> orderNumbers = new Dictionary<string, int>(); // Track order numbers
        private Dictionary<string, TextMeshProUGUI> receiptTitleTexts = new Dictionary<string, TextMeshProUGUI>(); // Track title text components
        private HorizontalLayoutGroup queueLayoutGroup;
        private Order currentOrder;
        private Coroutine hideGradeCoroutine;
        private int nextOrderNumber = 1; // Counter for sequential order numbers

        private void Start()
        {
            if (orderManager != null)
            {
                orderManager.onNewOrderCreated.AddListener(AddReceiptToQueue);
                orderManager.onOrderCompleted.AddListener(HandleOrderCompleted);
                orderManager.onShiftEnded.AddListener(ClearAllReceipts);
            }
            else
            {
                Debug.LogError("OrderManager reference not set in ReceiptQueueManager!");
            }

            // Setup layout group for receipt queue
            queueLayoutGroup = receiptQueueContent.GetComponent<HorizontalLayoutGroup>();
            if (queueLayoutGroup == null)
            {
                queueLayoutGroup = receiptQueueContent.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            
            // Configure only the spacing and alignment
            queueLayoutGroup.spacing = receiptSpacing;
            queueLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            
            // Configure ScrollRect for horizontal scrolling
            if (receiptScrollView != null)
            {
                receiptScrollView.horizontal = true;
                receiptScrollView.vertical = false;
                
                // Ensure viewport has mask for proper display
                if (receiptScrollView.viewport != null && receiptScrollView.viewport.GetComponent<RectMask2D>() == null)
                {
                    receiptScrollView.viewport.gameObject.AddComponent<RectMask2D>();
                }
            }

            // Setup deliver button
            if (deliverButton != null)
            {
                deliverButton.onClick.AddListener(OnDeliverButtonClicked);
            }
            
            // Initialize grade panel
            if (orderGradePanel != null)
            {
                orderGradePanel.SetActive(false);
            }
        }

        private void AddReceiptToQueue(Order order)
        {
            // Create new receipt instance
            GameObject receipt = Instantiate(receiptPrefab, receiptQueueContent);
            receiptInstances.Add(order.orderId, receipt);
            
            // Assign and store sequential order number
            int orderNumber = nextOrderNumber++;
            orderNumbers.Add(order.orderId, orderNumber);

            // Setup receipt UI (only text and color, no size adjustments)
            SetupReceiptUI(receipt, order, orderNumber);

            // If this is the first receipt, make it the current order
            if (receiptInstances.Count == 1)
            {
                SetCurrentOrder(order);
            }
            
            // Scroll to the newest receipt
            StartCoroutine(ScrollToNewReceipt());
        }

        private IEnumerator ScrollToNewReceipt()
        {
            yield return new WaitForEndOfFrame();
            
            if (receiptScrollView != null)
            {
                // Scroll to the leftmost position (to show oldest receipt)
                receiptScrollView.normalizedPosition = new Vector2(0, 0);
            }
        }

        private void SetupReceiptUI(GameObject receipt, Order order, int orderNumber)
        {
            // Find and store the title TextMeshPro component
            TextMeshProUGUI titleText = receipt.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = $"Order {orderNumber}";
                receiptTitleTexts[order.orderId] = titleText; // Store reference for later updates
            }

            // Set receipt content - only change text content
            TextMeshProUGUI contentText = receipt.transform.Find("Content")?.GetComponent<TextMeshProUGUI>();
            if (contentText != null)
            {
                contentText.text = FormatCompactOrderText(order);
            }

            // Set receipt color - only change color
            Image receiptImage = receipt.GetComponent<Image>();
            if (receiptImage != null)
            {
                receiptImage.color = receiptInstances.Count == 1 ? activeOrderColor : queuedOrderColor;
            }
        }

        private string FormatCompactOrderText(Order order)
        {
            return $"{order.burgerDoneness} Burger\n" +
                   $"{order.toppings.Count} Toppings\n" +
                   $"{order.drinkSize} {order.drinkFlavor} Drink";
        }

        private string FormatDetailedOrderText(Order order)
        {
            StringBuilder sb = new StringBuilder();
            
            // Use the stored order number instead of a static title
            int orderNum = orderNumbers.ContainsKey(order.orderId) ? orderNumbers[order.orderId] : 0;
            sb.AppendLine($"Order {orderNum}");
            sb.AppendLine($"Burger: {order.burgerDoneness}");
            
            sb.AppendLine("Toppings:");
            foreach (ToppingType topping in order.toppings)
            {
                sb.AppendLine($"- {topping}");
            }
            
            sb.AppendLine($"Drink: {order.drinkSize} {order.drinkFlavor}");
            
            return sb.ToString();
        }
        
        private void OnDeliverButtonClicked()
        {
            if (orderManager != null && receiptInstances.Count > 0)
            {
                orderManager.DeliverButtonPressed();
            }
        }
        
        private void HandleOrderCompleted(OrderGradeEventArgs args)
        {
            // Show grade feedback
            ShowOrderGrade(args);
            
            if (receiptInstances.Count > 0)
            {
                // Remove current receipt
                string currentOrderId = receiptInstances.Keys.First();
                Destroy(receiptInstances[currentOrderId]);
                receiptInstances.Remove(currentOrderId);

                // Update remaining receipt numbers
                UpdateReceiptNumbers();

                // Set next order as current if available
                if (receiptInstances.Count > 0)
                {
                    string nextOrderId = receiptInstances.Keys.First();
                    Order nextOrder = orderManager.GetOrderQueue().FirstOrDefault(o => o.orderId == nextOrderId);
                    if (nextOrder != null)
                    {
                        SetCurrentOrder(nextOrder);
                    }
                }
                else
                {
                    ClearCurrentOrderDisplay();
                }
            }
        }
        
        private void ShowOrderGrade(OrderGradeEventArgs args)
        {
            if (orderGradePanel != null && orderGradeText != null)
            {
                // Show the grade panel
                orderGradePanel.SetActive(true);
                
                // Set text and color based on grade
                int percentage = Mathf.RoundToInt(args.grade * 100);
                orderGradeText.text = $"Order Score: {percentage}%\n" +
                                    $"{(args.isCorrect ? "CORRECT!" : "INCORRECT")}";
                orderGradeText.color = args.isCorrect ? correctOrderColor : incorrectOrderColor;
                
                // Hide after delay
                if (hideGradeCoroutine != null)
                    StopCoroutine(hideGradeCoroutine);
                    
                hideGradeCoroutine = StartCoroutine(HideGradeAfterDelay());
            }
        }
        
        private IEnumerator HideGradeAfterDelay()
        {
            yield return new WaitForSeconds(gradeDisplayTime);
            if (orderGradePanel != null)
                orderGradePanel.SetActive(false);
            hideGradeCoroutine = null;
        }

        private void UpdateReceiptNumbers()
        {
            // Don't update the numbers - keep the original assigned numbers
            foreach (var entry in receiptInstances)
            {
                string orderId = entry.Key;
                
                // Use the stored reference if available
                if (receiptTitleTexts.TryGetValue(orderId, out TextMeshProUGUI titleText) && 
                    orderNumbers.TryGetValue(orderId, out int orderNumber))
                {
                    titleText.text = $"Order {orderNumber}";
                }
            }
        }

        private void SetCurrentOrder(Order order)
        {
            currentOrder = order;
            
            // Update current order display
            if (currentOrderText != null)
            {
                currentOrderText.text = FormatDetailedOrderText(order);
            }

            // Update receipt colors
            foreach (var receipt in receiptInstances.Values)
            {
                Image receiptImage = receipt.GetComponent<Image>();
                if (receiptImage != null)
                {
                    receiptImage.color = queuedOrderColor;
                }
            }

            // Highlight current receipt
            if (receiptInstances.TryGetValue(order.orderId, out GameObject currentReceipt))
            {
                Image currentReceiptImage = currentReceipt.GetComponent<Image>();
                if (currentReceiptImage != null)
                {
                    currentReceiptImage.color = activeOrderColor;
                }
            }
            
            // Enable deliver button
            if (deliverButton != null)
            {
                deliverButton.interactable = true;
            }
        }

        private void ClearCurrentOrderDisplay()
        {
            currentOrder = null;
            
            if (currentOrderText != null)
            {
                currentOrderText.text = "No Active Orders";
            }
            
            // Disable deliver button
            if (deliverButton != null)
            {
                deliverButton.interactable = false;
            }
        }

        private void ClearAllReceipts()
        {
            foreach (var receipt in receiptInstances.Values)
            {
                Destroy(receipt);
            }
            receiptInstances.Clear();
            orderNumbers.Clear(); // Clear order numbers
            receiptTitleTexts.Clear(); // Clear title text references
            nextOrderNumber = 1; // Reset the counter
            ClearCurrentOrderDisplay();
        }

        private void OnDestroy()
        {
            if (orderManager != null)
            {
                orderManager.onNewOrderCreated.RemoveListener(AddReceiptToQueue);
                orderManager.onOrderCompleted.RemoveListener(HandleOrderCompleted);
                orderManager.onShiftEnded.RemoveListener(ClearAllReceipts);
            }
            
            if (deliverButton != null)
            {
                deliverButton.onClick.RemoveListener(OnDeliverButtonClicked);
            }
        }
        
        // Helper method to update a specific receipt's title
        public void UpdateReceiptTitle(string orderId, string newTitle)
        {
            if (receiptTitleTexts.TryGetValue(orderId, out TextMeshProUGUI titleText))
            {
                titleText.text = newTitle;
            }
        }
        
        // Get the current order number
        public int GetCurrentOrderNumber()
        {
            return nextOrderNumber - 1;
        }
        
        // Set the next order number (useful for editor customization)
        public void SetNextOrderNumber(int startNumber)
        {
            nextOrderNumber = Mathf.Max(1, startNumber);
        }
    }
} 