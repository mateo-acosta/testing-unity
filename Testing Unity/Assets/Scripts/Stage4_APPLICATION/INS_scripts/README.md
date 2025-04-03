# Insurance Defense Game - Setup Guide

## Game Overview
The Insurance Defense Game is a 2D tower defense-style game where players protect a castle from various types of villains using different types of insurance (represented as antidotes). The game teaches players about different types of insurance through gameplay mechanics.

## Canvas Setup
1. Create a Canvas (UI > Canvas)
   - Set Canvas to "Screen Space - Overlay"
   - Add a Canvas Scaler component
   - Set UI Scale Mode to "Scale With Screen Size"
   - Set Reference Resolution (e.g., 1920x1080)
2. Add an EventSystem if not present in the scene

## Components Setup

### 1. Difficulty Selection Setup
1. Create a new UI Panel named "DifficultyPanel"
   - Set it as a child of the Canvas
   - Position it in the center of the screen
2. Add three buttons for difficulty options:
   - "No Forcefield" (1000 starting funds)
   - "Weak Forcefield" (800 starting funds)
   - "Strong Forcefield" (500 starting funds)
3. Add the `DifficultyManager` script to the DifficultyPanel
4. Configure in the Inspector:
   - Link the DifficultyPanel GameObject
   - Link the funds TextMeshPro text component
   - Set forcefield health values
   - Set starting funds for each difficulty
5. Set up button onClick events:
   - No Forcefield: DifficultyManager.SelectDifficulty(0)
   - Weak Forcefield: DifficultyManager.SelectDifficulty(1)
   - Strong Forcefield: DifficultyManager.SelectDifficulty(2)

### 2. Forcefield Setup
1. Create a new empty GameObject named "ForceFieldPrefab"
2. Add the `Forcefield` script to it
3. Configure in the Inspector:
   - Set Max Health
   - Set Radius
   - Set Shield Color
4. Create a UI Text (TMP) element for shield health display
5. Link the health TextMeshPro component
6. Save as a prefab
7. Assign the prefab to the DifficultyManager's ForceFieldPrefab field

### 3. Castle Setup (UI Version)
1. In your Canvas, create an empty UI GameObject named "Castle"
   - Tag this GameObject as "Castle_INS"
2. Add an Image component (instead of Sprite Renderer)
   - Assign your castle sprite
   - Set appropriate size using RectTransform
3. Create a child GameObject named "CastleBoundary"
   - Add a Circle Collider 2D component to this child object
   - Set the Circle Collider 2D as trigger
   - Tag this GameObject as "CastleBoundary_INS"
4. Add the `Castle.cs` script to the main Castle GameObject
5. Create a UI Text (TMP) element for health display as a child of the Castle
6. Configure the Inspector values in the Castle script:
   - Set Max Health (default: 100)
   - Set Boundary Radius (default: 2)
   - Link the health TextMeshProUGUI component
   - Link the Circle Collider 2D component from the CastleBoundary child object
7. Position the castle in the center of the screen using RectTransform

### 4. Villain Prefabs (UI Version)
Create four different prefabs for each villain type:
1. For each villain (Thief, Storm, Wildfire, Sickness):
   - Create an empty UI GameObject (UI > Image)
   - Set the Image component:
     - Assign appropriate sprite
     - Set Raycast Target to true
     - Set Image Type to "Simple"
     - Ensure Color alpha is 1 (fully visible)
   - Add a Box Collider 2D or Circle Collider 2D component:
     - Set "Is Trigger" to true
     - Adjust size to match visible sprite area
   - Add the `Villain.cs` script
   - Configure the villain type and move speed (try 300-500 for UI space)
   - Set RectTransform settings:
     - Width and Height to match your sprite (e.g., 50x50)
     - Anchor presets to middle-center
     - Pivot to center (0.5, 0.5)
     - Scale to 1,1,1
   - Save as a prefab in your project

### 5. Antidote UI Setup
1. Within your Canvas, create a panel for antidotes
2. For each antidote type (Shield, Tree, Water, Potion):
   - Create an Image UI element
   - Add the `DragAndDropAntidote.cs` script
   - Assign appropriate sprite
   - Configure the antidote type in the inspector
   - Set drag alpha value (default: 0.7)
   - Set RectTransform to desired size and position

### 6. Game Manager Setup
1. Create an empty GameObject named "InsuranceGameManager"
2. Attach the `InsuranceGameManager.cs` script
3. Configure in Inspector:
   - Link all villain prefabs
   - Set spawn settings
   - Set damage per villain
   - Link UI elements:
     - Score Text (TMP)
     - Timer Text (TMP)
     - Funds Text (TMP)
     - Game Over Panel
     - Game Canvas

### 7. UI Layout
1. Organize within the Canvas:
   - Score display (top of screen)
   - Health display (top of screen)
   - Game Over Panel (center, initially disabled)
     - Game Over text
     - Restart button
   - Antidote inventory panel (bottom or side of screen)
     - Use Grid Layout Group for organizing antidotes
     - Set consistent spacing and padding

### 8. Scene Organization
Recommended hierarchy structure:
```
Scene
├── Canvas
│   ├── Castle (tag: Castle_INS)
│   │   ├── CastleBoundary (tag: CastleBoundary_INS)
│   │   └── Health Text
│   ├── Score Text
│   ├── Game Over Panel
│   ├── Antidote Panel
│   │   └── [Antidote Images]
│   └── [Spawned Villains]
├── EventSystem
└── InsuranceGameManager
```

### 9. Tags Setup
Create the following tags in Unity:
- "Castle_INS" - Assign to the main castle GameObject
- "CastleBoundary_INS" - Assign to the castle's boundary child GameObject

## Important Canvas Considerations
1. **RectTransform Anchoring**:
   - Castle: Center anchor preset
   - Antidotes: Bottom or side anchor presets
   - Score/Health: Top anchor presets
   - Game Over Panel: Center anchor preset

2. **Canvas Render Mode**:
   - Keep "Screen Space - Overlay" for consistent UI rendering
   - Ensure sorting order is appropriate (castle behind antidotes)

3. **Scaling**:
   - Use relative scaling with RectTransform
   - Test on different screen resolutions
   - Maintain aspect ratios of sprites

## Game Configuration Tips

1. **Forcefield Balance**:
   - Adjust `weakForceFieldHealth` and `strongForceFieldHealth` in DifficultyManager
   - Modify shield colors for better visibility
   - Test different radius values for optimal gameplay

2. **Economy Balance**:
   - Adjust starting funds for each difficulty level
   - Modify villain defeat rewards in InsuranceGameManager
   - Balance forcefield costs against starting funds

3. **Spawn Radius**: Adjust the spawn radius in the InsuranceGameManager to ensure villains appear at a reasonable distance from the castle.

4. **Difficulty Balance**:
   - Adjust `minSpawnInterval` and `maxSpawnInterval` for spawn frequency
   - Modify `difficultyIncreaseRate` to control how quickly the game becomes harder
   - Tune `damagePerVillain` to balance the challenge

5. **Movement Speed**: Adjust the `moveSpeed` in villain prefabs to ensure they don't move too fast or too slow

6. **UI Positioning**: 
   - Place antidotes in easily accessible locations
   - Ensure health and score displays are clearly visible
   - Make sure the game over panel is centered and readable

## Testing the Game

1. Enter Play mode
2. Verify that:
   - All UI elements are visible and properly positioned
   - Villains spawn at the specified radius
   - Antidotes can be dragged and dropped
   - Castle takes damage when villains reach it
   - Score increases when villains are defeated
   - Game over triggers when castle health reaches 0
   - Restart functionality works properly
   - UI scales properly on different screen sizes

## Common Issues and Solutions

1. If elements are not visible:
   - Check Canvas Scaler settings
   - Verify Image component's color alpha value is not zero
   - Ensure RectTransform is within screen bounds
   - Check Canvas render mode and camera settings

2. If villains pass through the castle boundary:
   - Verify the CastleBoundary child object has the correct tag ("CastleBoundary_INS")
   - Make sure the Circle Collider 2D is on the CastleBoundary child object
   - Confirm the Circle Collider 2D is set as a trigger
   - Check that the boundary radius in the Castle script matches the collider radius
   - Ensure the CastleBoundary's transform position is at (0,0,0) relative to its parent

3. If antidotes aren't working:
   - Verify Canvas settings (screen space overlay)
   - Check EventSystem is present in the scene
   - Confirm antidote types match villain types
   - Ensure RectTransforms are properly sized

4. If UI elements are misaligned:
   - Check anchor points and pivot settings
   - Verify RectTransform positioning
   - Test different screen resolutions
   - Use Layout Groups where appropriate

5. If villains are invisible or not spawning correctly:
   - Verify villain prefabs are UI-based (created with UI > Image)
   - Check Image component's alpha value is 1
   - Ensure prefabs are properly linked in InsuranceGameManager
   - Verify the Game Canvas reference is set in InsuranceGameManager
   - Check if villains are spawning outside visible canvas area (adjust spawn radius)

6. If villains don't collide with castle boundary:
   - Ensure both castle boundary and villains have Collider2D components
   - Verify colliders are properly sized
   - Check that both colliders have "Is Trigger" enabled
   - Ensure villains are being spawned as children of the Canvas
   - Verify the RectTransform positions are within the canvas bounds

7. For movement speed issues:
   - UI space uses pixels instead of world units
   - Try increasing moveSpeed to 300-500 for normal movement
   - Adjust based on your canvas size and desired game speed 

## Troubleshooting

1. If forcefields aren't appearing:
   - Verify ForceFieldPrefab is assigned in DifficultyManager
   - Check that the castle has the correct tag ("Castle_INS")
   - Ensure forcefield visual settings are configured

2. If difficulty selection isn't working:
   - Verify button onClick events are properly set
   - Check DifficultyManager component references
   - Ensure Time.timeScale is being properly managed

3. If elements are not visible:
   - Check Canvas Scaler settings
   - Verify Image component's color alpha value is not zero
   - Ensure RectTransform is within screen bounds
   - Check Canvas render mode and camera settings

4. If villains pass through the castle boundary:
   - Verify the CastleBoundary child object has the correct tag ("CastleBoundary_INS")
   - Make sure the Circle Collider 2D is on the CastleBoundary child object
   - Confirm the Circle Collider 2D is set as a trigger
   - Check that the boundary radius in the Castle script matches the collider radius
   - Ensure the CastleBoundary's transform position is at (0,0,0) relative to its parent

5. If antidotes aren't working:
   - Verify Canvas settings (screen space overlay)
   - Check EventSystem is present in the scene
   - Confirm antidote types match villain types
   - Ensure RectTransforms are properly sized

6. If UI elements are misaligned:
   - Check anchor points and pivot settings
   - Verify RectTransform positioning
   - Test different screen resolutions
   - Use Layout Groups where appropriate

7. If villains are invisible or not spawning correctly:
   - Verify villain prefabs are UI-based (created with UI > Image)
   - Check Image component's alpha value is 1
   - Ensure prefabs are properly linked in InsuranceGameManager
   - Verify the Game Canvas reference is set in InsuranceGameManager
   - Check if villains are spawning outside visible canvas area (adjust spawn radius)

8. If villains don't collide with castle boundary:
   - Ensure both castle boundary and villains have Collider2D components
   - Verify colliders are properly sized
   - Check that both colliders have "Is Trigger" enabled
   - Ensure villains are being spawned as children of the Canvas
   - Verify the RectTransform positions are within the canvas bounds

9. For movement speed issues:
   - UI space uses pixels instead of world units
   - Try increasing moveSpeed to 300-500 for normal movement
   - Adjust based on your canvas size and desired game speed 