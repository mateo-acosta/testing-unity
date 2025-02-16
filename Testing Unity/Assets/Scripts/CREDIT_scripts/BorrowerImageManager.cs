using UnityEngine;
using UnityEngine.UI;

public class BorrowerImageManager : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("The UI prefab that contains the Image component for the borrower")]
    public GameObject borrowerPrefab;
    [Tooltip("The transform where the borrower prefab will be spawned")]
    public Transform spawnPoint;

    [Header("Character Images")]
    [Tooltip("Assign your borrower portrait images here")]
    public Sprite[] borrowerImages;

    private GameObject currentBorrowerObject;
    private Image currentBorrowerImage;
    private int currentImageIndex = -1;

    private void Start()
    {
        if (borrowerImages == null || borrowerImages.Length == 0)
        {
            Debug.LogWarning("No borrower images assigned to BorrowerImageManager!");
            return;
        }

        if (borrowerPrefab == null)
        {
            Debug.LogError("Borrower prefab is not assigned!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned!");
            return;
        }

        SpawnInitialBorrower();
    }

    private void SpawnInitialBorrower()
    {
        if (borrowerPrefab == null || spawnPoint == null) return;

        // Spawn the initial borrower
        SpawnNewBorrower();
    }

    private void SpawnNewBorrower()
    {
        // Destroy the previous borrower object if it exists
        if (currentBorrowerObject != null)
        {
            Destroy(currentBorrowerObject);
        }

        // Spawn the new borrower prefab at the spawn point
        currentBorrowerObject = Instantiate(borrowerPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        currentBorrowerObject.transform.localScale = Vector3.one;
        currentBorrowerObject.transform.localPosition = Vector3.zero;

        // Get the Image component from the spawned prefab
        currentBorrowerImage = currentBorrowerObject.GetComponent<Image>();
        if (currentBorrowerImage == null)
        {
            Debug.LogError("Borrower prefab must have an Image component!");
            return;
        }

        // Set the initial image
        if (currentImageIndex == -1)
        {
            ChangeToNextBorrower();
        }
        else
        {
            currentBorrowerImage.sprite = borrowerImages[currentImageIndex];
        }
    }

    public void ChangeToNextBorrower()
    {
        if (borrowerImages == null || borrowerImages.Length == 0) return;

        // Move to next image index, wrap around if at end
        currentImageIndex = (currentImageIndex + 1) % borrowerImages.Length;

        // Spawn a new borrower with the next image
        SpawnNewBorrower();
    }

    public void ChangeToRandomBorrower()
    {
        if (borrowerImages == null || borrowerImages.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, borrowerImages.Length);
        } while (newIndex == currentImageIndex && borrowerImages.Length > 1);

        currentImageIndex = newIndex;

        // Spawn a new borrower with the random image
        SpawnNewBorrower();
    }

    private void OnDestroy()
    {
        // Clean up any remaining borrower object
        if (currentBorrowerObject != null)
        {
            Destroy(currentBorrowerObject);
        }
    }
} 