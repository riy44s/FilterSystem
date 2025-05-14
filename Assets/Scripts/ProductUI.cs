using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProductUI : MonoBehaviour
{
    public TextMeshProUGUI productName;
    public TextMeshProUGUI productPrice;
    public Image productImage;

    public void SetProductInfo(GameManager.ProductData product)
    {
        productName.text = product.name;
        productPrice.text = $"${product.price:F2}";
        productImage.sprite = null;

        if (!string.IsNullOrEmpty(product.imagePath))
        {
            StartCoroutine(LoadImageFromWeb(product.imagePath));
        }
    }

    private IEnumerator LoadImageFromWeb(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                productImage.sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero);
            }
            else
            {
                Debug.LogError($"Failed to load image: {webRequest.error}");
            }
        }
    }
}