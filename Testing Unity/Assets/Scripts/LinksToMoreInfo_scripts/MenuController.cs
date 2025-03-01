using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button openMenuButton;
    [SerializeField] private Button closeMenuButton;

    private void Start()
    {
        // Ensure menu is closed at start
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        // Set up button listeners
        if (openMenuButton != null)
        {
            openMenuButton.onClick.AddListener(OpenMenu);
        }

        if (closeMenuButton != null)
        {
            closeMenuButton.onClick.AddListener(CloseMenu);
        }
    }

    public void OpenMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void CloseMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (openMenuButton != null)
        {
            openMenuButton.onClick.RemoveListener(OpenMenu);
        }

        if (closeMenuButton != null)
        {
            closeMenuButton.onClick.RemoveListener(CloseMenu);
        }

        // Ensure game is not left paused if script is destroyed
        Time.timeScale = 1f;
    }
} 