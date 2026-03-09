using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform UIIcon, UIName, UIAmount, UIPrice;

    private ShopItem ShopData;
    private int OrderAmount;
    private ShopPanel uiParent;

    public void Refresh(ShopItem shopItem, ShopPanel panel)
    {
        ShopData = shopItem;
        uiParent = panel;

        UIIcon.GetComponent<Image>().sprite = ShopData.Icon;
        UIName.GetComponent<Text>().text = ShopData.Name;
        UIPrice.GetComponent<Text>().text = $"$ {ShopData.Price}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OrderAmount++;
        uiParent.SpawnExhibition(ShopData.Exhibition);
        RefreshBuyAmount();
        uiParent.AddPurchase(ShopData, 1);
        uiParent.AddCost(ShopData.Price);
        uiParent.RefreshCost();
    }

    private void RefreshBuyAmount() =>
        UIAmount.GetComponent<Text>().text = OrderAmount > 0 ? $"X{OrderAmount}" : "";

    internal void ResetOrderAmount()
    {
        OrderAmount = 0;
        RefreshBuyAmount();
    }
}