using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabContent
    {
        public string tabTitle;
        public GameObject contentContainer;  // The content container (BudgetingContent, InsuranceContent, etc.)
        public ScrollRect scrollRect;  // Reference to the ScrollRect for this content
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI selectedTabTitleText;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private TabContent[] tabContents;
    
    [Header("Tab Button Colors")]
    [SerializeField] private Color selectedTabColor = new Color(0, 0.5f, 1f, 1f);
    [SerializeField] private Color normalTabColor = Color.white;
    
    private int currentTabIndex = -1;

    private void Start()
    {
        // Set up tab button listeners
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SelectTab(index));
        }

        // Select first tab by default
        SelectTab(0);
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabContents.Length)
            return;

        // Reset previous tab button color
        if (currentTabIndex >= 0 && currentTabIndex < tabButtons.Length)
        {
            var prevImage = tabButtons[currentTabIndex].GetComponent<Image>();
            if (prevImage != null)
            {
                prevImage.color = normalTabColor;
            }
        }

        currentTabIndex = tabIndex;

        // Update selected tab title
        selectedTabTitleText.text = tabContents[tabIndex].tabTitle;

        // Hide all content containers and disable scrolling
        foreach (var content in tabContents)
        {
            if (content.contentContainer != null)
            {
                content.contentContainer.SetActive(false);
            }
            if (content.scrollRect != null)
            {
                content.scrollRect.enabled = false;
                content.scrollRect.vertical = false;
            }
        }

        // Show the selected tab's content and enable its scrolling
        if (tabContents[tabIndex].contentContainer != null)
        {
            tabContents[tabIndex].contentContainer.SetActive(true);
        }
        if (tabContents[tabIndex].scrollRect != null)
        {
            tabContents[tabIndex].scrollRect.enabled = true;
            tabContents[tabIndex].scrollRect.vertical = true;
            // Reset scroll position to top
            tabContents[tabIndex].scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        // Highlight selected tab button
        var selectedImage = tabButtons[tabIndex].GetComponent<Image>();
        if (selectedImage != null)
        {
            selectedImage.color = selectedTabColor;
        }
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                tabButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
} 