using System;
using System.Collections.Generic;
using UnityEngine;

namespace BurgerGame
{
    [System.Serializable]
    public class Order
    {
        public string orderId;
        public BurgerDoneness burgerDoneness;
        public List<ToppingType> toppings;
        public DrinkSize drinkSize;
        public DrinkFlavor drinkFlavor;
        public float timeCreated;
        public bool isCompleted;

        public Order()
        {
            orderId = Guid.NewGuid().ToString();
            toppings = new List<ToppingType>();
            timeCreated = Time.time;
            isCompleted = false;
        }

        public int GetTotalFeatures()
        {
            // Count burger doneness as 1, drink size as 1, drink flavor as 1, plus number of toppings
            return 3 + toppings.Count;
        }

        public float CalculateGrade(Order deliveredOrder)
        {
            int correctFeatures = 0;
            
            // Check burger doneness
            if (burgerDoneness == deliveredOrder.burgerDoneness)
                correctFeatures++;

            // Check drink size
            if (drinkSize == deliveredOrder.drinkSize)
                correctFeatures++;

            // Check drink flavor
            if (drinkFlavor == deliveredOrder.drinkFlavor)
                correctFeatures++;

            // Check toppings
            foreach (ToppingType topping in toppings)
            {
                if (deliveredOrder.toppings.Contains(topping))
                    correctFeatures++;
            }

            // Calculate grade as percentage
            return (float)correctFeatures / GetTotalFeatures();
        }
    }
} 