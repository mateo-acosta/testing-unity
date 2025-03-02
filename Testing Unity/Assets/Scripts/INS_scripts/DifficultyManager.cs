using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject difficultyPanel;
    public TextMeshProUGUI startingFundsText;
    public TextMeshProUGUI difficultyDescriptionText;
    
    [Header("Play Buttons")]
    public Button noInsuranceButton;
    public Button weakInsuranceButton;
    public Button strongInsuranceButton;
    
    [Header("Insurance Settings")]
    public GameObject weakForceFieldPrefab;
    public GameObject strongForceFieldPrefab;
    [Tooltip("Health for Weak Insurance (1x Shield)")]
    public float weakForceFieldHealth = 50f;
    [Tooltip("Health for Strong Insurance (2x Shield)")]
    public float strongForceFieldHealth = 100f;
    
    [Header("Forcefield Position")]
    public float forceFieldOffset = 10f; // Distance in front of castle
    
    [Header("Cash System")]
    public const float INITIAL_CASH = 1000f;
    [Tooltip("Cost for No Insurance")]
    public float noInsuranceCost = 0f;
    [Tooltip("Cost for Weak Insurance")]
    public float weakInsuranceCost = 200f;
    [Tooltip("Cost for Strong Insurance")]
    public float strongInsuranceCost = 500f;
    
    private InsuranceGameManager gameManager;
    
    private void Start()
    {
        gameManager = InsuranceGameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("InsuranceGameManager not found!");
            return;
        }
        
        SetupDifficultyPanel();
    }
    
    private void SetupDifficultyPanel()
    {
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
            
            // Display initial cash and costs
            if (startingFundsText != null)
            {
                startingFundsText.text = $"${INITIAL_CASH:N0}";
            }
            
            if (difficultyDescriptionText != null)
            {
                difficultyDescriptionText.text = 
                    $"Choose Your Insurance Level:\n\n" +
                    $"No Insurance: FREE\n" +
                    $"Weak Insurance: ${weakInsuranceCost:N0}\n" +
                    $"Strong Insurance: ${strongInsuranceCost:N0}";
            }
            
            // Verify forcefield prefabs
            if (weakForceFieldPrefab == null || strongForceFieldPrefab == null)
            {
                Debug.LogError("One or more forcefield prefabs are missing!");
            }
        }
        
        // Set up button listeners
        if (noInsuranceButton != null)
            noInsuranceButton.onClick.AddListener(() => SelectDifficulty(0));
        if (weakInsuranceButton != null)
            weakInsuranceButton.onClick.AddListener(() => SelectDifficulty(1));
        if (strongInsuranceButton != null)
            strongInsuranceButton.onClick.AddListener(() => SelectDifficulty(2));
        else
            Debug.LogWarning("One or more play buttons are not assigned!");
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (noInsuranceButton != null)
            noInsuranceButton.onClick.RemoveAllListeners();
        if (weakInsuranceButton != null)
            weakInsuranceButton.onClick.RemoveAllListeners();
        if (strongInsuranceButton != null)
            strongInsuranceButton.onClick.RemoveAllListeners();
    }
    
    public void SelectDifficulty(int difficulty)
    {
        GameObject castle = GameObject.FindGameObjectWithTag("Castle_INS");
        if (castle == null)
        {
            Debug.LogError("Castle not found!");
            return;
        }
        
        // Get forcefield cost based on difficulty
        float forceFieldCost = difficulty switch
        {
            0 => noInsuranceCost,     // No Insurance
            1 => weakInsuranceCost,   // Weak Insurance
            2 => strongInsuranceCost, // Strong Insurance
            _ => noInsuranceCost
        };
        
        // Calculate remaining cash after forcefield purchase
        float remainingCash = INITIAL_CASH - forceFieldCost;
        
        // Create and configure forcefield if insurance is selected
        if (difficulty > 0)
        {
            // Select appropriate prefab
            GameObject prefabToUse = difficulty == 1 ? weakForceFieldPrefab : strongForceFieldPrefab;
            
            if (prefabToUse != null)
            {
                // Position forcefield in front of castle
                Vector3 castlePos = castle.transform.position;
                Vector3 spawnPos = castlePos + Vector3.forward * forceFieldOffset;
                
                GameObject forceFieldObj = Instantiate(prefabToUse, spawnPos, Quaternion.identity, castle.transform);
                Forcefield forcefield = forceFieldObj.GetComponent<Forcefield>();
                
                if (forcefield != null)
                {
                    // Configure forcefield based on insurance level
                    forcefield.maxHealth = difficulty == 1 ? (int)weakForceFieldHealth : (int)strongForceFieldHealth;
                    forcefield.width = 120f;
                    forcefield.height = 80f;
                    
                    // Set different colors for different insurance levels
                    forcefield.shieldColor = difficulty == 1 
                        ? new Color(0.2f, 0.6f, 1f, 0.3f) // Blue for weak
                        : new Color(0.6f, 1f, 0.2f, 0.3f); // Green for strong
                    
                    Debug.Log($"Spawned {(difficulty == 1 ? "weak" : "strong")} forcefield at position {spawnPos}");
                }
                else
                {
                    Debug.LogError("Forcefield component not found on prefab!");
                }
            }
            else
            {
                Debug.LogError($"Missing {(difficulty == 1 ? "weak" : "strong")} forcefield prefab!");
            }
        }
        
        // Hide difficulty panel
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }
        
        // Initialize the game manager with cash information and start the game
        gameManager.InitializeGameState(INITIAL_CASH, forceFieldCost, remainingCash);
        
        string difficultyName = difficulty switch
        {
            0 => "No Insurance",
            1 => "Weak Insurance",
            2 => "Strong Insurance",
            _ => "Unknown"
        };
        
        Debug.Log($"Starting game with {difficultyName} - Cost: ${forceFieldCost:N0}, Remaining: ${remainingCash:N0}");
    }
} 