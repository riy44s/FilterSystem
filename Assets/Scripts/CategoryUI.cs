// CategoryUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryUI : MonoBehaviour
{
    public Button categoryButton;
    public Image iconImage;
    public TextMeshProUGUI labelText;
    public Image backgroundImage; // assign in Inspector

    // configure these as you like
    private Color normalColor = Color.white;
    private Color selectedColor = new Color(0.8f, 0.8f, 1f);

    public void SetCategoryInfo(string name, Sprite icon)
    {
        labelText.text = name;
        iconImage.sprite = icon;
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
}
