using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubCategoryUI : MonoBehaviour
{
    public TextMeshProUGUI subCategoryName;
    public Button subCategoryButton;
    public Image subCategoryImage;

    public void SetSubCategoryInfo(string name, Sprite image)
    {
        subCategoryName.text = name;
        subCategoryImage.sprite = image;
    }
}