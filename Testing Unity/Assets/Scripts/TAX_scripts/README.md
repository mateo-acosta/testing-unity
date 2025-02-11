# Tax Education Game - Unity Setup Instructions

This game teaches tax education through a drag-and-drop classification mechanic where players must identify correct and incorrect tax returns.

## Scene Setup

1. Create a new Unity 2D project
2. Import the TextMeshPro Essential Resources (Window > TextMeshPro > Import TMP Essential Resources)

## Canvas Setup

1. Create a Canvas (Right-click in Hierarchy > UI > Canvas)
   - Set Canvas Scaler (Script) properties:
     * UI Scale Mode: Scale With Screen Size
     * Reference Resolution: X = 1920, Y = 1080
     * Screen Match Mode: Match Width Or Height
     * Match: 0.5 (to balance both dimensions)
   - Make sure Canvas has the "MainCanvas" tag

## Panel Setup

1. Create the Correct Panel (Right-click Canvas > UI > Panel)
   - RectTransform settings:
     * Anchor: Left side of screen
     * Position: X = 300, Y = 0
     * Width = 400, Height = 800
   - Image component:
     * Color: Light green (Alpha = 0.5)
   - Add DropPanel script
     * Check "Is Correct Panel" box
   - Add Box Collider 2D:
     * Check "Is Trigger"
   - Tags:
     * Add "DropPanel" tag
     * Add "CorrectPanel" tag

2. Create the Incorrect Panel (Right-click Canvas > UI > Panel)
   - RectTransform settings:
     * Anchor: Right side of screen
     * Position: X = -300, Y = 0
     * Width = 400, Height = 800
   - Image component:
     * Color: Light red (Alpha = 0.5)
   - Add DropPanel script
     * Uncheck "Is Correct Panel" box
   - Add Box Collider 2D:
     * Check "Is Trigger"
   - Tags:
     * Add "DropPanel" tag
     * Add "IncorrectPanel" tag

## UI Elements Setup

1. Create GameUI (Empty GameObject as child of Canvas)
   - RectTransform:
     * Stretch to fill entire canvas

2. Create Timer Display (Right-click GameUI > UI > TextMeshPro - Text)
   - RectTransform:
     * Anchor: Top center
     * Position: X = 0, Y = -50
     * Width = 200, Height = 50
   - TextMeshPro component:
     * Font Size: 36
     * Font Style: Bold
     * Alignment: Center
     * Color: White

3. Create Score Display (Right-click GameUI > UI > TextMeshPro - Text)
   - RectTransform:
     * Anchor: Top left
     * Position: X = 50, Y = -50
     * Width = 200, Height = 50
   - TextMeshPro component:
     * Font Size: 36
     * Alignment: Left
     * Color: White

4. Create Streak Display (Right-click GameUI > UI > TextMeshPro - Text)
   - RectTransform:
     * Anchor: Top right
     * Position: X = -50, Y = -50
     * Width = 200, Height = 50
   - TextMeshPro component:
     * Font Size: 36
     * Alignment: Right
     * Color: White

5. Create Game Over Panel (Right-click GameUI > UI > Panel)
   - RectTransform:
     * Anchor: Center
     * Width = 600, Height = 400
   - Image component:
     * Color: Dark gray (Alpha = 0.9)
   - Initially set to inactive

   a. Add Final Score Text (TMP)
      - Position: Y = 50
      - Font Size: 48
      - Alignment: Center

   b. Add Highest Streak Text (TMP)
      - Position: Y = -50
      - Font Size: 36
      - Alignment: Center

   c. Add Restart Button
      - Position: Y = -150
      - Width = 200, Height = 60
      - Add onClick event pointing to TaxGameManager.RestartGame()

## Tax Return Prefab Setup

1. Create Base Tax Return Prefab
   - Create empty GameObject
   - Add RectTransform:
     * Width = 500, Height = 700
   - Add Panel (Image) component:
     * Color: White
     * Alpha: 1
   - Add CanvasGroup component
   - Add Box Collider 2D:
     * Size matches RectTransform
     * Is Trigger checked
   - Add TaxReturn script
   - Add DraggableTaxReturn script

2. Add TextMeshPro Text Components (create these as children of the base prefab):

   a. Income Text
      - Position: Y = 300
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   b. Above Line Deductions Text
      - Position: Y = 225
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   c. Adjusted Gross Income Text
      - Position: Y = 150
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   d. Itemized Deductions Text
      - Position: Y = 75
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   e. Taxable Income Text
      - Position: Y = 0
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   f. Tax Credits Text
      - Position: Y = -75
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

   g. Final Tax Liability Text
      - Position: Y = -150
      - Font Size: 24
      - Color: Black
      - Text Alignment: Left
      - Margin: Left = 20

3. Create Prefab Variations
   
   a. Correct Tax Return Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Check "Is Correct"
         * Error Type: None
      - Save as "CorrectTaxReturn" prefab

   b. Tax Credits Error Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Uncheck "Is Correct"
         * Error Type: TaxCreditsAddedInstead
      - Save as "TaxCreditsErrorReturn" prefab

   c. Calculation Order Error Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Uncheck "Is Correct"
         * Error Type: WrongCalculationOrder
      - Save as "CalculationOrderErrorReturn" prefab

   d. Layout Order Error Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Uncheck "Is Correct"
         * Error Type: WrongLayoutOrder
      - Save as "LayoutOrderErrorReturn" prefab

   e. Wrong Label Error Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Uncheck "Is Correct"
         * Error Type: WrongFinalLabel
      - Save as "WrongLabelErrorReturn" prefab

   f. Deductions Error Prefab:
      - Duplicate base prefab
      - Configure TaxReturn component:
         * Uncheck "Is Correct"
         * Error Type: DeductionsAddedInstead
      - Save as "DeductionsErrorReturn" prefab

## Game Manager Setup

1. Add TaxGameManager component to the Canvas
2. Configure TaxGameManager in Inspector:
   - Game Settings:
     * Game Duration: 180
   - UI References:
     * Timer Text: Reference to Timer TMP
     * Score Text: Reference to Score TMP
     * Streak Text: Reference to Streak TMP
     * Game Over Panel: Reference to Game Over panel
     * Final Score Text: Reference to Final Score TMP
     * Highest Streak Text: Reference to Highest Streak TMP
   - Tax Return Generation:
     * Correct Tax Return Prefab: Reference to CorrectTaxReturn prefab
     * Incorrect Tax Return Prefabs: Array of 5 incorrect prefabs
     * Tax Return Spawn Point: Reference to spawn point transform
   - Value Ranges:
     * Income Range: Min = 20000, Max = 100000
     * Deductions Range: Min = 5000, Max = 20000
     * Credits Range: Min = 1000, Max = 5000

## Testing

1. Enter Play mode
2. Verify:
   - Timer counts down from 3:00
   - Tax returns spawn in center
   - Drag and drop works smoothly
   - Score updates correctly
   - Streak counter works
   - Game over screen appears
   - Restart button functions

## Common Issues and Solutions

1. If drag and drop doesn't work:
   - Check that CanvasGroup component is present
   - Verify Box Collider 2D settings
   - Ensure all required tags are set

2. If tax returns don't spawn:
   - Check prefab references in TaxGameManager
   - Verify spawn point position

3. If UI elements don't update:
   - Verify all TextMeshPro references
   - Check UI element anchoring 