using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Demo/InventoryTable",fileName ="InventoryTable")]
public class Inventory : ScriptableObject
{
    public List<ItemData> materialList = new List<ItemData>();
    public List<ItemData> potionList = new List<ItemData>();
}
[Serializable]
public class ItemData
{
    public int ID;
    public ItemType Type;
    public string Name;
    public string Description;
    public Sprite Icon;
}
public enum ItemType
{
    Material,
    Potion
}
