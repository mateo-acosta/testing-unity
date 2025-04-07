using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace BurgerGame
{
    public class OrderManager : MonoBehaviour
    {
        [Header("Order Generation Settings")]
        [SerializeField] private float customerSpawnInterval = 30f;
        [SerializeField] private int maxQueueSize = 10;
        
        [Header("Shift Settings")]
        [SerializeField] private float shiftDuration = 180f; // 3 minutes in seconds
        
        [Header("Events")]
        public UnityEvent<Order> onNewOrderCreated;
        public UnityEvent<float> onShiftTimeUpdated;
        public UnityEvent onShiftEnded;

        private Queue<Order> orderQueue;
        private float shiftTimeRemaining;
        private bool isShiftActive;
        private System.Random random;

        private void Awake()
        {
            orderQueue = new Queue<Order>();
            random = new System.Random();
            
            // Initialize events if null
            if (onNewOrderCreated == null) onNewOrderCreated = new UnityEvent<Order>();
            if (onShiftTimeUpdated == null) onShiftTimeUpdated = new UnityEvent<float>();
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

        public void CompleteOrder(Order completedOrder)
        {
            if (orderQueue.Count > 0)
            {
                Order currentOrder = orderQueue.Peek();
                if (currentOrder.orderId == completedOrder.orderId)
                {
                    orderQueue.Dequeue();
                    completedOrder.isCompleted = true;
                    // Additional completion logic can be added here
                }
            }
        }

        private void EndShift()
        {
            isShiftActive = false;
            onShiftEnded?.Invoke();
        }

        public Queue<Order> GetOrderQueue()
        {
            return orderQueue;
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