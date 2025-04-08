# Grill Station Setup Guide

This guide provides instructions for setting up the Grill Station scene based on the existing hierarchy.

## Hierarchy Structure
Based on your current hierarchy:
```
- Stage1_GrillStation2
  - Main Camera
  - Canvas
    - BackgroundPanel
    - RawPattyPlate
      - Circle
      - Outline
      - RawPattySource
    - GrillArea
    - DonePlate
      - Circle
      - Outline
    - PattyDonenessPopup
      - IconFrame
      - Text_Title
      - Text_PattyDoneness
    - Text_PattyClock
    - Text_OrderInfo
    - TabMenu
    - EventSystem
    - GameData
    - GrillStationManager
```

## Step 1: Fix Script Errors
First, make sure all scripts are properly updated to fix the existing errors.

## Step 2: Setup Components

### GrillStationManager GameObject
1. Add the `GrillManager` component to the GrillStationManager GameObject
2. Add the `GrillSceneManager` component to the same GameObject
3. Configure the `GrillManager` component with these references:
   - rawPattyPrefab: Create and assign a patty prefab (instructions below)
   - grill: GrillArea Transform
   - leftPlate: RawPattyPlate Transform
   - rightPlate: DonePlate Transform
   - grillArea: Add PolygonCollider2D to GrillArea and assign it here
   - donenessText: Assign Text_PattyDoneness
   - clockText: Assign Text_PattyClock
4. Configure the `GrillSceneManager` component:
   - grillManager: Reference to the GrillManager component
   - leftPlateCollider: Add PlateCollider to RawPattyPlate and reference it
   - rightPlateCollider: Add PlateCollider to DonePlate and reference it
   - orderInfoText: Assign Text_OrderInfo
   - instructionText: Create a new TextMeshProUGUI for instructions if needed
   - rawPattyPrefab: Same patty prefab as above
   - pattySourceObject: Assign RawPattySource

### Plate Setup
1. Add `PlateCollider` components to both RawPattyPlate and DonePlate GameObjects
2. Set their plateType property appropriately (LeftPlate for RawPattyPlate, RightPlate for DonePlate)
3. Set detectionRadius to approximately 1.0f

### GrillArea Setup
1. Add a `PolygonCollider2D` to the GrillArea
2. Shape the collider to match the visible area of the grill

### RawPattySource Setup
1. Make sure RawPattySource has an Image component with the raw patty sprite
2. Add a `PattyDragHandler` component to RawPattySource
3. Configure it:
   - grillManager: Reference to the GrillManager component
   - pattyPrefab: Assign the patty prefab
   - isSourcePatty: Set to true
4. Add a `CanvasGroup` component to RawPattySource

## Step 3: Create Patty Prefab
1. Create a new UI Image GameObject (not in the scene)
2. Add the `PattyController` component
3. Configure the PattyController with sprites for each doneness level:
   - rawSprite
   - rareSprite
   - mediumSprite
   - wellDoneSprite
   - burntSprite
4. Add a `PattyDragHandler` component
5. Add a `CanvasGroup` component
6. Save as a prefab in your project

## Step 4: Testing and Troubleshooting

### Common Issues:
1. **Patty not draggable**: Ensure RawPattySource has CanvasGroup component
2. **Patty doesn't duplicate**: Check that pattyPrefab is assigned in the RawPattySource's PattyDragHandler
3. **Cannot drop on grill**: Verify GrillArea has PolygonCollider2D configured correctly
4. **Patty state doesn't change**: Confirm all sprite references are assigned

### Testing Procedure:
1. Run the scene
2. Click and drag from RawPattySource
3. A duplicate patty should appear and follow your cursor
4. Drop the patty on the GrillArea to start cooking
5. The Text_PattyClock and Text_PattyDoneness should update
6. When ready, drag the patty to the DonePlate

## Advanced Configuration

### Doneness Timing:
The PattyController has configurable thresholds:
- 0-2 seconds: Raw
- 2.01-4 seconds: Rare
- 4.01-6 seconds: Medium
- 6.01-8 seconds: Well Done
- 8.01+ seconds: Burnt

You can adjust these in the PattyController component if needed 