using UnityEngine;
using UnityEngine.SceneManagement;

public class FullGameSceneManagement : MonoBehaviour
{
    // Function to load the Level Select scene by index
    public void GoToLevelSelect()
    {
        SceneManager.LoadScene(0);
    }

    // Functions for each mini-game scene, loaded by their build index

    public void LoadBudgetGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadCreditGame()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadInsuranceGame()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadInvestmentGame()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadTaxGame()
    {
        SceneManager.LoadScene(5);
    }
}
