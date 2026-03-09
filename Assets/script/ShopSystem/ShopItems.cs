using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demo/ShopItemsTable", fileName = "ShopTable")]
public class ShopItems:ScriptableObject
{
    public List<ShopItem> shopItems = new List<ShopItem>();
}
[Serializable]
public class ShopItem : ItemData
{
    public GameObject Exhibition;
    public int Price;
}