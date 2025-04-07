using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace BurgerGame
{
    [System.Serializable]
    public class OrderGradeEventArgs
    {
        public Order order;
        public float grade;
        public bool isCorrect;
    }

    public class OrderManager : MonoBehaviour
    {
        [Header("Order Generation Settings")]
        [SerializeField] private float customerSpawnInterval = 15f;
        [SerializeField] private int maxQueueSize = 10;
        
        [Header("Shift Settings")]
        [SerializeField] private float shiftDuration = 180f; // 3 minutes in seconds
        [SerializeField] private float gradeThreshold = 0.75f; // Threshold for considering an order correct (75%)
        
        [Header("Events")]
        public UnityEvent<Order> onNewOrderCreated;
        public UnityEvent<float> onShiftTimeUpdated;
        public UnityEvent<OrderGradeEventArgs> onOrderCompleted;
        public UnityEvent onShiftEnded;

        private Queue<Order> orderQueue;
        private float shiftTimeRemaining;
        private bool isShiftActive;
        private System.Random random;
        private List<float> completedOrderGrades = new List<float>();

        private void Awake()
        {
            orderQueue = new Queue<Order>();
            random = new System.Random();
            
            // Initialize events if null
            if (onNewOrderCreated == null) onNewOrderCreated = new UnityEvent<Order>();
            if (onShiftTimeUpdated == null) onShiftTimeUpdated = new UnityEvent<float>();
            if (onOrderCompleted == null) onOrderCompleted = new UnityEvent<OrderGradeEventArgs>();
            if (onShiftEnded == null) onShiftEnded = new UnityEvent();
        }

        private void Start()
        {
            StartShift();
        }

        public void StartShift()
        {
            isShiftActive = true;
            shiftTimeRemaining = shiftDuration;
            orderQueue.Clear();
            completedOrderGrades.Clear();
            StartCoroutine(CustomerSpawnRoutine());
            StartCoroutine(ShiftTimerRoutine());
        }

        private IEnumerator CustomerSpawnRoutine()
        {
            while (isShiftActive)
            {
                if (orderQueue.Count < maxQueueSize)
                {
                    CreateNewOrder();
                }
                yield return new WaitForSeconds(customerSpawnInterval);
            }
        }

        private IEnumerator ShiftTimerRoutine()
        {
            while (shiftTimeRemaining > 0 && isShiftActive)
            {
                shiftTimeRemaining -= Time.deltaTime;
                onShiftTimeUpdated?.Invoke(shiftTimeRemaining);
                
                if (shiftTimeRemaining <= 0)
                {
                    EndShift();
                }
                
                yield return null;
            }
        }

        private void CreateNewOrder()
        {
            Order newOrder = new Order
            {
                burgerDoneness = (BurgerDoneness)random.Next(3),
                drinkSize = (DrinkSize)random.Next(2),
                drinkFlavor = (DrinkFlavor)random.Next(4)
            };
            
            // Initialize the order (sets timeCreated with Time.time)
            newOrder.Initialize();

            // Add random toppings (between 1 and 5 toppings)
            int toppingCount = random.Next(1, 6);
            List<ToppingType> availableToppings = new List<ToppingType>(Enum.GetValues(typeof(ToppingType)) as ToppingType[]);
            
            for (int i = 0; i < toppingCount; i++)
            {
                if (availableToppings.Count > 0)
                {
                    int index = random.Next(availableToppings.Count);
                    newOrder.toppings.Add(availableToppings[index]);
                    availableToppings.RemoveAt(index);
                }
            }

            orderQueue.Enqueue(newOrder);
            onNewOrderCreated?.Invoke(newOrder);
        }

        public void CompleteOrder(Order deliveredOrder)
        {
            if (orderQueue.Count > 0)
            {
                Order actualOrder = orderQueue.Peek();
                float grade = actualOrder.CalculateGrade(deliveredOrder);
                bool isCorrect = grade >= gradeThreshold;
                
                // Add to completed orders list
                completedOrderGrades.Add(grade);
                
                // Remove from queue
                orderQueue.Dequeue();
                
                // Invoke completion event
                OrderGradeEventArgs args = new OrderGradeEventArgs
                {
                    order = actualOrder,
                    grade = grade,
                    isCorrect = isCorrect
                };
                
                onOrderCompleted?.Invoke(args);
            }
        }

        public void DeliverButtonPressed()
        {
            if (orderQueue.Count > 0)
            {
                // In a real implementation, you would get the player's assembled order
                // For now, we'll simulate with the same order for testing
                Order playerOrder = orderQueue.Peek();
                CompleteOrder(playerOrder);
            }
        }

        private void EndShift()
        {
            isShiftActive = false;
            onShiftEnded?.Invoke();
            
            // Calculate final score
            float finalScore = CalculateFinalScore();
            Debug.Log($"Shift ended. Final score: {finalScore:P0}");
        }
        
        private float CalculateFinalScore()
        {
            if (completedOrderGrades.Count == 0)
                return 0f;
                
            float sum = 0f;
            foreach (float grade in completedOrderGrades)
            {
                sum += grade;
            }
            
            return sum / completedOrderGrades.Count;
        }

        public Queue<Order> GetOrderQueue()
        {
            return orderQueue;
        }

        public Order GetCurrentOrder()
        {
            return orderQueue.Count > 0 ? orderQueue.Peek() : null;
        }

        public int GetQueueCount()
        {
            return orderQueue.Count;
        }

        public bool IsShiftActive()
        {
            return isShiftActive;
        }

        public float GetRemainingShiftTime()
        {
            return shiftTimeRemaining;
        }
    }
} 