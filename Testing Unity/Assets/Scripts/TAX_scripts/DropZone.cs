using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Drop Zone Settings")]
    public bool isCorrectZone;
    public Color highlightColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public GameObject feedbackPrefab;
    public float feedbackDuration = 1f;

    [Header("Visual Feedback")]
    public GameObject correctIcon;
    public GameObject incorrectIcon;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private Image backgroundImage;
    private Color originalColor;
    private AudioSource audioSource;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        originalColor = backgroundImage.color;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            backgroundImage.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundImage.color = originalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        TaxReport report = eventData.pointerDrag.GetComponent<TaxReport>();
        if (report != null)
        {
            HandleDrop(report);
        }
        
        backgroundImage.color = originalColor;
    }

    private void HandleDrop(TaxReport report)
    {
        bool playerJudgment = isCorrectZone;
        bool isCorrect = (report.IsCorrectlyFiled == isCorrectZone);

        // Show visual feedback
        ShowFeedback(isCorrect);

        // Play sound
        PlayFeedbackSound(isCorrect);

        // Notify game manager
        TaxGameManager.Instance.HandleReportJudgment(report, playerJudgment);

        // If player judged it as incorrect, show correction popup
        if (!isCorrectZone)
        {
            ShowCorrectionPopup(report);
        }

        // Destroy the report object
        Destroy(report.gameObject);
    }

    private void ShowFeedback(bool isCorrect)
    {
        GameObject feedbackObj = Instantiate(feedbackPrefab, transform);
        RectTransform feedbackRect = feedbackObj.GetComponent<RectTransform>();
        
        // Position the feedback above the drop zone
        feedbackRect.anchoredPosition = Vector2.up * 100f;
        
        // Show appropriate icon
        if (isCorrect)
        {
            correctIcon.SetActive(true);
            incorrectIcon.SetActive(false);
        }
        else
        {
            correctIcon.SetActive(false);
            incorrectIcon.SetActive(true);
        }

        // Destroy feedback after duration
        Destroy(feedbackObj, feedbackDuration);
    }

    private void PlayFeedbackSound(bool isCorrect)
    {
        AudioClip clipToPlay = isCorrect ? correctSound : incorrectSound;
        if (clipToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    private void ShowCorrectionPopup(TaxReport report)
    {
        // Get reference to the correction popup from TaxGameManager
        CorrectionPopup popup = TaxGameManager.Instance.GetComponent<CorrectionPopup>();
        if (popup != null)
        {
            popup.ShowPopup(report);
        }
    }
} 