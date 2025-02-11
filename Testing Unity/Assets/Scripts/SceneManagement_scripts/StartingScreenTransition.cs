using UnityEngine;

public class StartingScreenTransition : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject startScreenPanel;
    public GameObject levelSelectPanel;

    // This function is called when the Start Game button is clicked
    public void StartGame()
    {
        // Hide the start screen
        startScreenPanel.SetActive(false);
        // Show the level select panel
        levelSelectPanel.SetActive(true);
    }

    public void GoToStartScreen()
    {
        // Hide the start screen
        startScreenPanel.SetActive(true);
        // Show the level select panel
        levelSelectPanel.SetActive(false);
    }
}
