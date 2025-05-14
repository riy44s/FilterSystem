using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelUI : MonoBehaviour
{
    public Button[] categoryButtons;

    public Button[] watchMenWomenButtons;
    public Button[] clothMenWomenButtons;
    public Button[] jewelryMenWomenButtons;


    private void Start()
    {
        DisableAllCategoryButtons();
    }

    public void OnCategoryButtonClicked(int categoryIndex)
    {
        DisableAllCategoryButtons();

        switch (categoryIndex)
        {
            case 0: 
                ShowMenWomenButtons(watchMenWomenButtons);
                break;
            case 1: 
                ShowMenWomenButtons(clothMenWomenButtons);
                break;
            case 2: 
                ShowMenWomenButtons(jewelryMenWomenButtons);
                break;
        }

        DisableCategoryButton(categoryIndex);
    }

    private void DisableAllCategoryButtons()
    {
        foreach (Button btn in watchMenWomenButtons)
        {
            btn.gameObject.SetActive(false);
        }

        foreach (Button btn in clothMenWomenButtons)
        {
            btn.gameObject.SetActive(false);
        }

        foreach (Button btn in jewelryMenWomenButtons)
        {
            btn.gameObject.SetActive(false);
        }

        foreach (Button btn in categoryButtons)
        {
            btn.interactable = true;
        }
    }

    private void ShowMenWomenButtons(Button[] categoryButtons)
    {
        foreach (Button btn in categoryButtons)
        {
            btn.gameObject.SetActive(true);
        }
    }

    private void DisableCategoryButton(int categoryIndex)
    {
        categoryButtons[categoryIndex].interactable = false;
    }
}
