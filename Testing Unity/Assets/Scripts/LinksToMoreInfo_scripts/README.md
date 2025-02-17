# More Info Tab System Implementation Guide

This guide explains how to implement the More Info tab system that displays different categories of helpful links for users.

## Overview

The system consists of two main components:
1. `TabManager.cs` - Manages the tab switching and content visibility
2. `HyperlinkButton.cs` - Handles opening URLs when link buttons are clicked

## Setup Instructions

### 1. Set Up the Link Buttons

1. For each link in your scroll menu:
   - Add the `HyperlinkButton.cs` script to the button
   - Call `SetURL()` in the inspector or via script to set the URL for each button
   - Organize the buttons into separate container GameObjects (one for each tab)

### 2. Set Up the Tab UI

1. Ensure you have:
   - A TextMeshProUGUI component for the selected tab title
   - A horizontal layout group with your tab buttons
   - Your scroll menu with the link buttons organized in containers

### 3. Configure the TabManager

1. Add the `TabManager.cs` script to the More Info menu's main GameObject
2. In the Inspector, assign:
   - Selected Tab Title Text: Reference to the title TextMeshProUGUI
   - Tab Buttons: Array of your tab buttons
   - Tab Contents: Array of your tabs (size should match number of tab buttons)
3. For each Tab Content element:
   - Set the Tab Title
   - Assign the corresponding link buttons container GameObject

## How It Works

1. When the scene starts, the first tab is automatically selected
2. Clicking a tab button:
   - Updates the selected tab title
   - Hides all other tab content
   - Shows the selected tab's content
3. Clicking a link button opens the associated URL in the default web browser

## Example Structure

```
MoreInfoMenu (GameObject with TabManager)
├── TabTitle (TextMeshProUGUI)
├── TabButtons
│   ├── Tab1Button
│   ├── Tab2Button
│   └── ...
└── ScrollView
    └── Content
        ├── Tab1Container (GameObject)
        │   ├── LinkButton1 (with HyperlinkButton)
        │   ├── LinkButton2 (with HyperlinkButton)
        │   └── ...
        ├── Tab2Container (GameObject)
        │   ├── LinkButton1 (with HyperlinkButton)
        │   └── ...
        └── ...
```

## Troubleshooting

Common issues and solutions:

1. Links not opening:
   - Check if URLs are properly formatted (include https://)
   - Verify HyperlinkButton component is attached to buttons
   - Ensure SetURL() has been called with the correct URL

2. Content not switching:
   - Verify all container GameObjects are properly assigned in TabManager
   - Check that container references aren't missing

3. Layout issues:
   - Ensure layout groups are properly configured
   - Check anchor points and rect transform settings

## Best Practices

1. Keep URLs up to date and valid
2. Use clear, descriptive link titles
3. Organize link buttons logically within their containers
4. Maintain consistent button styling across all tabs
5. Test all links periodically to ensure they're still active 