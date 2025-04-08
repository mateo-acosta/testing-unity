using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

namespace BurgerGame
{
    public class OrderDisplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private Transform orderContainer;
        [SerializeField] private GameObject orderPrefab;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerFillImage;
        
        [Header("UI Settings")]
        [SerializeField] private Color activeOrderColor = Color.yellow;
        [SerializeField] private Color queuedOrderColor = Color.white;
        [SerializeField] private Color timerNormalColor = Color.green;
        [SerializeField] private Color timerWarningColor = Color.yellow;
        [SerializeField] private Color timerCriticalColor = Color.red;
        [SerializeField] private float warningThreshold = 60f; // 1 minute warning
        [SerializeField] private float criticalThreshold = 30f; // 30 seconds warning

        private Dictionary<string, GameObject> activeOrderDisplays = new Dictionary<string, GameObject>();
        private float initialShiftDuration;

        private void Start()
        {
            if (orderManager != null)
            {
                orderManager.onNewOrderCreated.AddListener(DisplayNewOrder);
                orderManager.onShiftTimeUpdated.AddListener(UpdateTimer);
                orderManager.onShiftEnded.AddListener(ClearAllOrders);
                
                // Store initial shift duration for fill amount calculation
                initialShiftDuration = orderManager.GetRemainingShiftTime();
            }
            else
            {
                Debug.LogError("OrderManager reference not set in OrderDisplayManager!");
            }
        }

        private void DisplayNewOrder(Order order)
        {
            GameObject orderDisplay = Instantiate(orderPrefab, orderContainer);
            TextMeshProUGUI orderText = orderDisplay.GetComponentInChildren<TextMeshProUGUI>();
            
            if (orderText != null)
            {
                orderText.text = FormatOrderText(order);
                orderText.color = activeOrderDisplays.Count == 0 ? activeOrderColor : queuedOrderColor;
            }
            
            activeOrderDisplays.Add(order.orderId, orderDisplay);
        }

        private string FormatOrderText(Order order)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"Order #{order.orderId.Substring(0, 4)}");
            sb.AppendLine($"Burger: {order.burgerDoneness}");
            
            sb.AppendLine("Toppings:");
            foreach (ToppingType topping in order.toppings)
            {
                sb.AppendLine($"- {topping}");
            }
            
            sb.AppendLine($"Drink: {order.drinkSize} {order.drinkFlavor}");
            
            return sb.ToString();
        }

        public void RemoveOrder(string orderId)
        {
            if (activeOrderDisplays.TryGetValue(orderId, out GameObject orderDisplay))
            {
                Destroy(orderDisplay);
                activeOrderDisplays.Remove(orderId);
                
                // Update colors for remaining orders
                UpdateOrderColors();
            }
        }

        private void UpdateOrderColors()
        {
            bool isFirst = true;
            foreach (GameObject display in activeOrderDisplays.Values)
            {
                TextMeshProUGUI orderText = display.GetComponentInChildren<TextMeshProUGUI>();
                if (orderText != null)
                {
                    orderText.color = isFirst ? activeOrderColor : queuedOrderColor;
                }
                isFirst = false;
            }
        }

        private void UpdateTimer(float timeRemaining)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                // Change color based on remaining time
                if (timeRemaining <= criticalThreshold)
                {
                    timerText.color = timerCriticalColor;
                }
                else if (timeRemaining <= warningThreshold)
                {
                    timerText.color = timerWarningColor;
                }
                else
                {
                    timerText.color = timerNormalColor;
                }
            }
            
            // Update timer fill image if available
            if (timerFillImage != null && initialShiftDuration > 0)
            {
                timerFillImage.fillAmount = timeRemaining / initialShiftDuration;
                
                // Change color based on remaining time
                if (timeRemaining <= criticalThreshold)
                {
                    timerFillImage.color = timerCriticalColor;
                }
                else if (timeRemaining <= warningThreshold)
                {
                    timerFillImage.color = timerWarningColor;
                }
                else
                {
                    timerFillImage.color = timerNormalColor;
                }
            }
        }

        private void ClearAllOrders()
        {
            foreach (GameObject display in activeOrderDisplays.Values)
            {
                Destroy(display);
            }
            activeOrderDisplays.Clear();
        }

        private void OnDestroy()
        {
            if (orderManager != null)
            {
                orderManager.onNewOrderCreated.RemoveListener(DisplayNewOrder);
                orderManager.onShiftTimeUpdated.RemoveListener(UpdateTimer);
                orderManager.onShiftEnded.RemoveListener(ClearAllOrders);
            }
        }
    }
} 