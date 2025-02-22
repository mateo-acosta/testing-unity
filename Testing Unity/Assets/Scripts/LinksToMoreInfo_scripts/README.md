# More Info Menu System Implementation Guide

This guide explains how to implement the More Info menu system with its tab navigation and hyperlink functionality.

## Overview

The system consists of two main components:
1. `TabManager.cs` - Manages the tab switching and content visibility
2. `HyperlinkButton.cs` - Handles opening URLs for individual link buttons

## Setup Instructions

### 1. Set Up the Link Buttons

For each "Link" button in your content:
1. Add the `HyperlinkButton.cs` script to the button GameObject
2. In the Inspector, set the URL for that specific link
3. In the Button component's OnClick() event:
   - Add the button's GameObject
   - Select the HyperlinkButton -> OpenURL function

### 2. Configure the Tab System

1. On your MenuPopup GameObject, add the `TabManager.cs` script
2. In the Inspector, assign:
   - Selected Tab Title Text: The TextMeshProUGUI component showing the current tab title
   - Tab Buttons: Array of the 5 category tab buttons (Budgeting, Insurance, etc.)
   - Tab Contents: Array of 5 elements (one for each category)
3. For each Tab Content element:
   - Set the Tab Title (e.g., "Budgeting", "Insurance", etc.)
   - Assign the Content Container (e.g., BudgetingContent, InsuranceContent, etc.)
   - Assign the ScrollRect component for that content's scroll view

### 3. ScrollRect Setup

For each category's content:
1. Ensure the ScrollRect component is properly configured:
   - Vertical = true
   - Horizontal = false (unless horizontal scrolling is needed)
   - Content = Reference to the content container
   - Viewport = Reference to the mask/viewport object
2. Make sure the content's RectTransform is properly sized to allow scrolling

### 4. Hierarchy Structure

```
MenuPopup
├── Text_SelectedTabTitle
├── TabMenu (with tab buttons)
└── ScrollView
    ├── Viewport (with mask)
    └── Content
        ├── BudgetingContent (with ScrollRect)
        │   ├── Gross v. Net Pay
        │   │   ├── ImageFrame
        │   │   └── Button_Link (with HyperlinkButton)
        │   ├── Needs v. Wants
        │   │   ├── ImageFrame
        │   │   └── Button_Link (with HyperlinkButton)
        │   └── ...
        ├── InsuranceContent (with ScrollRect)
        ├── InvestingContent (with ScrollRect)
        ├── CreditContent (with ScrollRect)
        └── TaxesContent (with ScrollRect)
```

## How It Works

1. Tab Selection:
   - Clicking a tab button shows its corresponding content
   - Selected tab remains highlighted in blue
   - Tab title updates to match selected category
   - Scroll position resets to top
   - Scrolling is enabled for the selected content only

2. Link Buttons:
   - Each green "Link" button operates independently
   - Clicking a link button opens its specific URL in a new browser tab
   - URLs are configured individually for each button

## Troubleshooting

1. Links not opening:
   - Verify the URL is set in the HyperlinkButton component
   - Check that the Button's OnClick event is properly set to call OpenURL
   - Ensure the URL includes "https://" or "http://"

2. Content not scrolling:
   - Verify ScrollRect component is assigned in TabManager for each tab
   - Check that vertical scrolling is enabled
   - Ensure content size is larger than viewport
   - Verify content and viewport references in ScrollRect component

3. Tab content not showing:
   - Check that content containers are properly assigned in TabManager
   - Verify the GameObject references aren't missing
   - Make sure content container names match the hierarchy

4. Tab highlighting issues:
   - Ensure tab buttons have Image components
   - Check that the TabManager's color settings are configured

## Best Practices

1. Keep URLs up to date and valid
2. Use descriptive names for content containers and buttons
3. Maintain consistent button and layout styling
4. Test scrolling functionality for all tabs
5. Organize content hierarchically as shown in the structure above 