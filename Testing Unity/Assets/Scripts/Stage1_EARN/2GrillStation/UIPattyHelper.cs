using UnityEngine;
using UnityEngine.UI;

public class UIPattyHelper : MonoBehaviour
{
    [Header("Patty UI Feedback")]
    public Color highlightColor = new Color(1, 1, 1, 1);
    public Color normalColor = new Color(1, 1, 1, 0.9f);
    public float pulseDuration = 0.5f;
    public float scaleWhileDragging = 1.1f;
    
    private Image pattyImage;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isPulsing = false;
    private float pulseTime = 0f;
    
    private void Awake()
    {
        pattyImage = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = pattyImage != null ? pattyImage.color : Color.white;
    }
    
    private void Update()
    {
        // Handle pulsing animation
        if (isPulsing)
        {
            pulseTime += Time.deltaTime;
            float t = Mathf.PingPong(pulseTime / pulseDuration, 1f);
            
            if (pattyImage != null)
            {
                pattyImage.color = Color.Lerp(normalColor, highlightColor, t);
            }
            
            // End pulsing after one complete cycle
            if (pulseTime >= pulseDuration * 2)
            {
                isPulsing = false;
                if (pattyImage != null)
                {
                    pattyImage.color = originalColor;
                }
            }
        }
    }
    
    // Call this when the patty's doneness changes to provide visual feedback
    public void PulseOnDonenessChange()
    {
        isPulsing = true;
        pulseTime = 0f;
    }
    
    // Call when starting to drag
    public void StartDragging()
    {
        transform.localScale = originalScale * scaleWhileDragging;
    }
    
    // Call when ending drag
    public void StopDragging()
    {
        transform.localScale = originalScale;
    }
    
    // Highlight the patty when it reaches a new doneness level
    public void HighlightDoneness(PattyController.PattyDoneness doneness)
    {
        // Different highlight colors for different doneness levels
        switch (doneness)
        {
            case PattyController.PattyDoneness.Raw:
                highlightColor = new Color(1, 0.5f, 0.5f, 1);
                break;
            case PattyController.PattyDoneness.Rare:
                highlightColor = new Color(1, 0.7f, 0.4f, 1);
                break;
            case PattyController.PattyDoneness.Medium:
                highlightColor = new Color(0.9f, 0.7f, 0.3f, 1);
                break;
            case PattyController.PattyDoneness.WellDone:
                highlightColor = new Color(0.7f, 0.5f, 0.3f, 1);
                break;
            case PattyController.PattyDoneness.Burnt:
                highlightColor = new Color(0.3f, 0.3f, 0.3f, 1);
                break;
        }
        
        PulseOnDonenessChange();
    }
} 