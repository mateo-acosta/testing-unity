# Budget Game Implementation Guide

## Overview
The Budget Game is a financial education game where players catch falling expense tokens using a basket and assign them to different spending categories. The goal is to manage monthly expenses and accumulate savings to reach a target goal.

## Setup Instructions

### 1. Scene Setup
1. Create a new Unity UI scene
2. Add a Canvas (UI > Canvas)
3. Set the Canvas Scaler to "Scale With Screen Size"
4. Set the reference resolution to 1920x1080

### 2. Game Objects Setup

#### Basket Setup
1. Create an empty UI GameObject named "Basket"
2. Add an Image component for visual representation
3. Add a Box Collider 2D component
   - Enable "Is Trigger"
   - Adjust the size to match the visual representation
4. Add the BasketController script
5. Position the basket near the bottom of the screen

#### Category UI Setup
1. Create a panel for category display
2. For each of the 6 categories:
   - Add a CategorySlot prefab
   - Set unique category names (e.g., Housing, Food, Transportation, etc.)
   - Configure the TextMeshPro components for name and value display

#### Token Spawner Setup
1. Create an empty GameObject named "TokenSpawner"
2. Add the WaterfallSpawner script
3. Create a BudgetToken prefab:
   - UI Image for visual representation
   - TextMeshPro for value display
   - Box Collider 2D (Is Trigger = true)
   - BudgetToken script
4. Assign the BudgetToken prefab to the spawner

#### UI Elements Setup
1. Add TextMeshPro elements for:
   - Current Category
   - Month Counter
   - Total Savings
   - Savings Goal Progress
2. Add a Monthly Savings Popup:
   - Create a panel with TextMeshPro for displaying savings
   - Set it to inactive by default

### 3. Game Manager Setup
1. Add the BudgetGameManager script to an empty GameObject
2. Configure the references:
   - Category Slots array
   - UI elements
   - Monthly income
   - Savings goal

## Required Components

### Scripts
- BudgetGameManager.cs: Main game logic
- BasketController.cs: Player-controlled basket movement
- CategorySlot.cs: Category management
- BudgetToken.cs: Falling expense tokens
- WaterfallSpawner.cs: Token spawning system

### Prefabs
1. Basket
2. CategorySlot
3. BudgetToken
4. Monthly Savings Popup

## Game Flow
1. Player moves the basket using arrow keys
2. Tokens fall from the top of the screen
3. When a token is caught:
   - Its value is assigned to the current category
   - The game advances to the next category
4. When all categories are filled:
   - Monthly savings are calculated
   - A popup shows the savings amount
   - The month advances
   - Categories reset for the next month
5. Game continues until the savings goal is reached

## Testing
1. Verify basket movement and boundaries
2. Test token spawning and falling behavior
3. Confirm category value assignment
4. Check monthly calculations and popup display
5. Validate win condition when savings goal is reached

## Troubleshooting

### Collision Detection Issues
If tokens are not being caught by the basket:

1. **Check the Unity Console** for debug logs showing:
   - Basket collider initialization
   - Token spawning
   - Collision detection events

2. **Collider Setup**:
   - Both the basket and tokens must have Collider2D components
   - Both colliders should have "Is Trigger" enabled
   - Collider sizes should match the visual elements

3. **Physics 2D Settings**:
   - Check Edit > Project Settings > Physics 2D
   - Ensure appropriate collision matrix settings
   - You may need to set specific collision layers for UI elements

4. **Rigidbody Requirements**:
   - At least one object in a collision needs a Rigidbody2D
   - In this implementation, tokens have a kinematic Rigidbody2D

### Category Display Not Updating
If the current category is not being updated:

1. **Check the Console** for log messages from `UpdateCurrentCategoryDisplay()`
2. Verify that `currentCategoryText` is assigned in the inspector
3. Ensure all `CategorySlot` objects are properly assigned in the `categorySlots` array
4. Check that each `CategorySlot` has a valid `categoryName` property set

### Tokens Not Being Destroyed On Catch
If tokens are colliding but not being destroyed:

1. Look for logs showing "Token caught!" or "Token collision detected"
2. Verify that `OnTriggerEnter2D` is being called on the BasketController
3. Check the reference to `gameManager` in BasketController is not null
4. Ensure the `OnTokenCaught` method is being called in BudgetGameManager

### Debug Mode
The current implementation includes extensive debug logging. Check the Unity Console for messages prefixed with:
- "Basket": For basket-related logs
- "Token": For token-related logs including spawning and collisions
- All debug logs include specific IDs to help track individual objects

If problems persist after checking these areas, you can modify the debug logs in the scripts to gain more insight into what's happening. 