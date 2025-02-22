using UnityEngine;
using UnityEngine.UI;

public class HyperlinkButton : MonoBehaviour
{
    [SerializeField] private string url;  // URL to open when clicked
    
    public void OpenURL()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning($"No URL set for hyperlink button: {gameObject.name}");
        }
    }
} 