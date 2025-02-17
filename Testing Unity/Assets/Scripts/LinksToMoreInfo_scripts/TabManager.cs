using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabContent
    {
        public string tabTitle;
        public GameObject linkButtonsContainer;  // Container holding the link buttons for this tab
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI selectedTabTitleText;
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private TabContent[] tabContents;

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

        // Update selected tab title
        selectedTabTitleText.text = tabContents[tabIndex].tabTitle;

        // Hide all link button containers
        foreach (var content in tabContents)
        {
            if (content.linkButtonsContainer != null)
            {
                content.linkButtonsContainer.SetActive(false);
            }
        }

        // Show the selected tab's link buttons
        if (tabContents[tabIndex].linkButtonsContainer != null)
        {
            tabContents[tabIndex].linkButtonsContainer.SetActive(true);
        }
    }
} 