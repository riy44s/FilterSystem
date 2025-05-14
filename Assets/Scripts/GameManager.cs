using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    

    [System.Serializable]
    public class CategoryData
    {
        public string name;
        public string imagePath;
        public List<SubCategoryData> subCategories;
    }

    [System.Serializable]
    public class SubCategoryData
    {
        public string name;
        public string imagePath;
    }

    [System.Serializable]
    private class RootObject
    {
        public List<CategoryData> categories;
        public List<ProductData> products;
    }

    [System.Serializable]
    public class ProductData
    {
        public int id;
        public string name;
        public float price;
        public string category;
        public string subCategory;
        public string imagePath;
    }

    private RootObject loadedData;
    private List<CategoryData> categories = new List<CategoryData>();
    private List<ProductData> products = new List<ProductData>();

    void Awake()
    {
        Instance = this;
        LoadAllData();
    }

    private void LoadAllData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/Product");
        if (jsonFile == null)
        {
            Debug.LogError("Failed to load JSON file!");
            return;
        }

        try
        {
            loadedData = JsonUtility.FromJson<RootObject>(jsonFile.text);
            categories = loadedData?.categories ?? new List<CategoryData>();
            products = loadedData?.products ?? new List<ProductData>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parsing failed: {e.Message}");
        }

    }

    public List<CategoryData> GetMainCategories() => categories;

    public List<SubCategoryData> GetSubCategories(string mainCategory)
    {
        return categories.Find(c => c.name == mainCategory)?.subCategories;
    }

    public Sprite GetCategoryImage(string categoryName)
    {
        var category = categories.Find(c => c.name == categoryName);
        return LoadSprite(category?.imagePath);
    }

    public Sprite GetSubCategoryImage(string subCategoryName)
    {
        foreach (var category in categories)
        {
            var subCat = category.subCategories.Find(s => s.name == subCategoryName);
            if (subCat != null) return LoadSprite(subCat.imagePath);
        }
        return null;
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return Resources.Load<Sprite>(path);
    }

    public List<ProductData> GetProductsByCategory(string mainCategory, string subCategory = "")
    {
        if (products == null)
        {
            Debug.LogError("Products list is null!");
            return new List<ProductData>();
        }

        if (string.IsNullOrEmpty(subCategory))
            return products.FindAll(p => p != null && p.category == mainCategory);

        return products.FindAll(p => p != null &&
                                   p.category == mainCategory &&
                                   p.subCategory == subCategory);
    }

    public List<ProductData> GetAllProducts()
    {
        return new List<ProductData>(products);
    }

}