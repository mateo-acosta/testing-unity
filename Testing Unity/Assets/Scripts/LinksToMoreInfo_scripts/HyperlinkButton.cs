using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HyperlinkButton : MonoBehaviour
{
    private string url;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenURL);
    }

    public void SetURL(string newURL)
    {
        url = newURL;
    }

    private void OpenURL()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("No URL set for this hyperlink button!");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OpenURL);
        }
    }
} 