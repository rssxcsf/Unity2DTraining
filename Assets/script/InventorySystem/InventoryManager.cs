using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private Inventory inventory;
    private Dictionary<int, ItemData> itemMaterialsDataDict;
    private Dictionary<int, ItemData> itemPotionsDataDict;

    private List<InventoryItem> potionsInInventory;
    private List<InventoryItem> materialsInInventory;

    private void Awake()
    {
        InitilizeInventory();
        GetInventory();
        InitializeDict();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void InitilizeInventory()
    {
        potionsInInventory = LocalData.Instance.LoadPotionsInInventory();
        materialsInInventory = LocalData.Instance.LoadMaterialsInInventory();
    }
    private void InitializeDict()
    {
        itemMaterialsDataDict = inventory.materialList.ToDictionary(x => x.ID);
        itemPotionsDataDict = inventory.potionList.ToDictionary(x => x.ID);
    }

    public void AddItem(int itemId, int amount, ItemType type)
    {
        var data = GetItemData(itemId,type);
        if (data == null) return;

        // 处理可堆叠物品逻辑
        if (TryGetStackableItem(itemId, type, out var existingItem))
        {
            existingItem.stackCount += amount;
        }
        else
        {
            switch (type)
            {
                case ItemType.Potion:
                    potionsInInventory.Add(new InventoryItem
                    {
                        uid = System.Guid.NewGuid().ToString(),
                        id = itemId,
                        stackCount = amount,
                        type = ItemType.Potion,
                    });
                    break;
                case ItemType.Material:
                    materialsInInventory.Add(new InventoryItem
                    {
                        uid = System.Guid.NewGuid().ToString(),
                        id = itemId,
                        stackCount = amount,
                        type = ItemType.Material
                    });
                    break;
            }
        }
        LocalData.Instance.Save();
    }

    private bool TryGetStackableItem(int itemId,ItemType type, out InventoryItem item)
    {
        switch (type)
        {
            case ItemType.Potion:
                item = potionsInInventory.FirstOrDefault(x =>
            x.id == itemId &&
            x.stackCount < Const.maxStack);
                return item != null;
            case ItemType.Material:
                item = materialsInInventory.FirstOrDefault(x =>
            x.id == itemId &&
            x.stackCount < Const.maxStack);
                return item != null;
            default:item = null;
                return false;
        }
        
    }

    public ItemData GetItemData(int itemId, ItemType type)
    {
        switch (type)
        {
            case ItemType.Material:
                return itemMaterialsDataDict.TryGetValue(itemId, out var data) ? data : null;
            case ItemType.Potion:
                return itemPotionsDataDict.TryGetValue(itemId, out data) ? data : null;
                default: return null;
        }
    }
    #region 背包系统
    public Inventory GetInventory()
    {
        if (inventory == null)
        {
            inventory = Resources.Load<Inventory>("Data/InventoryTable");//路径
        }
        return inventory;
    }
    public List<InventoryItem> GetMaterialsLocalData()
    {
        return materialsInInventory;
    }
    public List<InventoryItem> GetPotionsLocalData()
    {
        return potionsInInventory;
    }
    public List<InventoryItem> GetSortMaterialsLocalData()
    {
        List<InventoryItem> localItems = LocalData.Instance.LoadMaterialsInInventory();
        //localItems.Sort(new InventoryItemComparer());
        return localItems;
    }
    public ItemData GetInventoryItem(int id, ItemType type)
    {
        List<ItemData> inventoryDataList = new List<ItemData>();
        if (type == ItemType.Potion)
            inventoryDataList = GetInventory().potionList;
        else if (type == ItemType.Material)
            inventoryDataList = GetInventory().materialList;
        foreach (ItemData item in inventoryDataList)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }
    #endregion
}
