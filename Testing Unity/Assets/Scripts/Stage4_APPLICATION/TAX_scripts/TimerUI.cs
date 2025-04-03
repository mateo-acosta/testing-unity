using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float maxDecisionTime = 7f;
    private float currentTime;
    
    [Header("UI References")]
    [SerializeField] private Image clockImage;
    [SerializeField] private Image maskImage; // This will be our grey overlay
    
    private bool isTimerActive = false;
    private TaxReturnConveyor currentTaxReturn;

    private void Start()
    {
        // Ensure the mask image starts fully transparent
        if (maskImage != null)
        {
            maskImage.fillAmount = 0f;
            maskImage.type = Image.Type.Filled;
            maskImage.fillMethod = Image.FillMethod.Radial360;
            maskImage.fillOrigin = (int)Image.Origin360.Top;
            maskImage.fillClockwise = true;
        }
    }

    private void Update()
    {
        if (!isTimerActive) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerVisual();
        }
        else if (currentTime <= 0 && isTimerActive)
        {
            TimeExpired();
        }
    }

    public void StartTimer(TaxReturnConveyor taxReturn)
    {
        currentTaxReturn = taxReturn;
        currentTime = maxDecisionTime;
        isTimerActive = true;
        if (maskImage != null)
        {
            maskImage.fillAmount = 0f;
        }
    }

    public void StopTimer()
    {
        isTimerActive = false;
        if (maskImage != null)
        {
            maskImage.fillAmount = 0f;
        }
    }

    private void UpdateTimerVisual()
    {
        if (maskImage != null)
        {
            // Calculate fill amount (0 to 1)
            float fillAmount = 1f - (currentTime / maxDecisionTime);
            maskImage.fillAmount = fillAmount;
        }
    }

    private void TimeExpired()
    {
        isTimerActive = false;
        if (currentTaxReturn != null)
        {
            currentTaxReturn.TimerExpired();
        }
    }
} 