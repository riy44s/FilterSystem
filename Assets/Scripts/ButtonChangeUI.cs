// ButtonChangeUI.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeUI : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public ProductUI productPrefab;
    public Transform productParent;
    public Transform categoryButtonParent;
    public Transform subCategoryButtonParent;
    public CategoryUI categoryButtonPrefab;
    public SubCategoryUI subCategoryButtonPrefab;
    public Button nextButton;
    public Button previousButton;
    public Button clearButton; 

    [Header("Paging Settings")]
    public int productsPerPage = 10;

    private List<ProductUI> productUIs = new List<ProductUI>();
    private List<SubCategoryUI> subCategoryPool = new List<SubCategoryUI>();
    private List<GameManager.ProductData> currentProducts = new List<GameManager.ProductData>();
    private int currentPage = 0;
    private int totalPages = 0;

    // **multiple** main‐categories
    private HashSet<string> selectedMainCategories = new HashSet<string>();
    // single sub‐category filter
    private string currentSubCategory = "";

    void Start()
    {
        // instantiate pool of product slots
        for (int i = 0; i < productsPerPage; i++)
        {
            var ui = Instantiate(productPrefab, productParent);
            ui.gameObject.SetActive(false);
            productUIs.Add(ui);
        }

        nextButton.onClick.AddListener(OnNext);
        previousButton.onClick.AddListener(OnPrevious);
        clearButton.onClick.AddListener(OnClearButton);

        CreateCategoryButtons();
        InitPaging_All();
    }

    void CreateCategoryButtons()
    {
        // remove old buttons
        foreach (Transform t in categoryButtonParent) Destroy(t.gameObject);

        var mains = gameManager.GetMainCategories();
        foreach (var cat in mains)
        {
            var btn = Instantiate(categoryButtonPrefab, categoryButtonParent);
            btn.SetCategoryInfo(cat.name, gameManager.GetCategoryImage(cat.name));

            // capture for closure
            string catName = cat.name;
            btn.categoryButton.onClick.AddListener(() =>
            {
                OnCategoryToggled(btn, catName);
            });
        }
    }

    void OnCategoryToggled(CategoryUI btn, string catName)
    {
        // toggle in/out of our HashSet
        if (selectedMainCategories.Contains(catName))
        {
            selectedMainCategories.Remove(catName);
            btn.SetSelected(false);
        }
        else
        {
            selectedMainCategories.Add(catName);
            btn.SetSelected(true);
        }

        // reset any sub‐category selection
        currentSubCategory = "";
        // rebuild sub‐category buttons as union across all selected mains
        CreateSubCategoryButtonsForAll();

        // re‐filter & refresh
        InitPaging_ByCategories();
    }

    void CreateSubCategoryButtonsForAll()
    {
        // hide/clear pool
        foreach (var b in subCategoryPool)
        {
            b.gameObject.SetActive(false);
            b.subCategoryButton.onClick.RemoveAllListeners();
        }

        // collect union of all sub‐cats
        var allSubNames = new HashSet<string>();
        foreach (var main in selectedMainCategories)
        {
            var subs = gameManager.GetSubCategories(main);
            if (subs != null)
                foreach (var s in subs) allSubNames.Add(s.name);
        }

        if (allSubNames.Count == 0) return;

        // reuse or instantiate
        int i = 0;
        foreach (var name in allSubNames)
        {
            SubCategoryUI btn;
            if (i < subCategoryPool.Count)
                btn = subCategoryPool[i];
            else
            {
                btn = Instantiate(subCategoryButtonPrefab, subCategoryButtonParent);
                subCategoryPool.Add(btn);
            }

            btn.gameObject.SetActive(true);
            btn.SetSubCategoryInfo(name, gameManager.GetSubCategoryImage(name));
            string subName = name;
            btn.subCategoryButton.onClick.AddListener(() =>
            {
                // select this sub‐category (single)
                currentSubCategory = subName;
                InitPaging_ByCategories();
            });
            i++;
        }
    }

    void InitPaging_All()
    {
        selectedMainCategories.Clear();
        currentSubCategory = "";
        CreateSubCategoryButtonsForAll();

        currentProducts = gameManager.GetAllProducts();
        ComputePagesAndRefresh();
    }

    void InitPaging_ByCategories()
    {
        // start with either all or union of selected mains
        List<GameManager.ProductData> combined;
        if (selectedMainCategories.Count == 0)
        {
            combined = gameManager.GetAllProducts();
        }
        else
        {
            combined = new List<GameManager.ProductData>();
            foreach (var cat in selectedMainCategories)
                combined.AddRange(gameManager.GetProductsByCategory(cat));

            // de‐duplicate by some unique ID
            combined = combined
                .GroupBy(p => p.id)
                .Select(g => g.First())
                .ToList();
        }

        // then apply sub‐cat filter if any
        if (!string.IsNullOrEmpty(currentSubCategory))
        {
            combined = combined
                .Where(p => p.subCategory == currentSubCategory)
                .ToList();
        }

        currentProducts = combined;
        ComputePagesAndRefresh();
    }

    void ComputePagesAndRefresh()
    {
        totalPages = Mathf.CeilToInt(currentProducts.Count / (float)productsPerPage);
        currentPage = 0;
        RefreshProductDisplay();
    }

    void RefreshProductDisplay()
    {
        int startIndex = currentPage * productsPerPage;
        int total = currentProducts.Count;

        for (int i = 0; i < productUIs.Count; i++)
        {
            int idx = startIndex + i;
            if (idx < total)
            {
                productUIs[i].gameObject.SetActive(true);
                productUIs[i].SetProductInfo(currentProducts[idx]);
            }
            else
            {
                productUIs[i].gameObject.SetActive(false);
            }
        }

        previousButton.interactable = (currentPage > 0);
        nextButton.interactable = (startIndex + productsPerPage) < total;
    }

    void OnNext()
    {
        if (currentPage + 1 < totalPages)
        {
            currentPage++;
            RefreshProductDisplay();
        }
    }

    void OnPrevious()
    {
        if (currentPage > 0)
        {
            currentPage--;
            RefreshProductDisplay();
        }
    }

    void OnClearButton()
    {
        // clear UI highlights
        foreach (Transform t in categoryButtonParent)
        {
            var ui = t.GetComponent<CategoryUI>();
            if (ui != null) ui.SetSelected(false);
        }

        foreach (var btn in subCategoryPool)
            btn.gameObject.SetActive(false);

        InitPaging_All();
    }
}
