using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel:BasePanel
{
    private Transform UIMenu;
    private Transform UIMenuMain;
    private Transform UIMenuPotion;
    private Transform UIMenuMaterial;
    private Transform UICenter;
    private Transform UIScrollView;

    public GameObject InventoryItemPrefab;
    private int type;
    protected override void Awake()
    {
        type = 0;
        base.Awake();
        InitUI();
    }
    private void Start()
    {
        RefreshUI();
    }
    private void InitUI()
    {
        InitUIName();
        InitClick();
    }
    public void RefreshUI()
    {
        RefreshScroll();
    }
    private void RefreshScroll()
    {
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        for(int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }
        switch (type)
        {
            case 0:
                break;
            case 1:
                LoadPotions(scrollContent);
                break;
            case 2:
                LoadMaterials(scrollContent);
                break;
        }
    }

    private void LoadPotions(RectTransform scrollContent)
    {
        foreach (InventoryItem localPotionData in InventoryManager.Instance.GetPotionsLocalData())
        {
            Transform InventoryUIItem = Instantiate(InventoryItemPrefab.transform, scrollContent) as Transform;
            InventoryCell inventoryCell = InventoryUIItem.GetComponent<InventoryCell>();
            inventoryCell.Refresh(localPotionData, this);
        }
    }
    private void LoadMaterials(RectTransform scrollContent)
    {
        foreach (InventoryItem localMaterialsData in InventoryManager.Instance.GetMaterialsLocalData())
        {
            Transform InventoryUIItem = Instantiate(InventoryItemPrefab.transform, scrollContent) as Transform;
            InventoryCell inventoryCell = InventoryUIItem.GetComponent<InventoryCell>();
            inventoryCell.Refresh(localMaterialsData, this);
        }
    }
    private void InitUIName()
    {
        UIMenu = transform.Find("TopCenter/Menu");
        UIMenuMain = transform.Find("TopCenter/Menu/Main");
        UIMenuPotion = transform.Find("TopCenter/Menu/Potion");
        UIMenuMaterial = transform.Find("TopCenter/Menu/Material");
        UICenter = transform.Find("Center");
        UIScrollView = transform.Find("Center/Scroll View");
    }
    private void InitClick()
    {
        UIMenuMain.GetComponent<Button>().onClick.AddListener(OnClickMain);
        UIMenuPotion.GetComponent<Button>().onClick.AddListener(OnClickPotion);
        UIMenuMaterial.GetComponent<Button>().onClick.AddListener(OnClickMaterial);
    }

    private void OnClickMaterial()
    {
        type = 2;
        RefreshUI();
    }

    private void OnClickPotion()
    {
        type = 1;
        RefreshUI();
    }

    private void OnClickMain()
    {
        type = 0;
        RefreshUI();
    }
}

