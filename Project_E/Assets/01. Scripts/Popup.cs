using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    public void SetContent(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
    }


    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}