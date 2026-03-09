using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopPanel : BasePanel
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject BuyButton;
    [SerializeField] private Text ButtonText;
    [SerializeField] private GameObject ShopCellPrefab;
    [SerializeField] private Transform UICostText;
    [SerializeField] private int maxExhibitions = 10; // 最大同时显示数量

    private ShopItems ShopItems;
    private int Cost;
    private Queue<GameObject> activeExhibitions = new Queue<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<ShopItem, int> purchasedItems = new Dictionary<ShopItem, int>();
    protected override void Awake()
    {
        base.Awake();
        ShopItems = Resources.Load<ShopItems>("Data/ShopTable");
        BuyButton.GetComponent<Button>().onClick.AddListener(Buy);
        RefreshItems();
    }
    private void Update()
    {
        UpdateButton();
    }
    public void AddPurchase(ShopItem item, int amount)
    {
        if (purchasedItems.ContainsKey(item))
            purchasedItems[item] += amount;
        else
            purchasedItems.Add(item, amount);
    }
    private void RefreshItems()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (ShopItem item in ShopItems.shopItems)
            Instantiate(ShopCellPrefab, content).GetComponent<ShopCell>().Refresh(item, this);
    }

    public void SpawnExhibition(GameObject prefab)
    {
        GameObject exhibition = GetFromPool(prefab);
        ConfigureExhibition(exhibition);
        ManageExhibitionLimit();
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        InitializePoolIfNeeded(prefab);
        return objectPools[prefab].Count > 0 ? objectPools[prefab].Dequeue() : CreateNewExhibition(prefab);
    }

    private void InitializePoolIfNeeded(GameObject prefab)
    {
        if (!objectPools.ContainsKey(prefab))
            objectPools[prefab] = new Queue<GameObject>();
    }

    private GameObject CreateNewExhibition(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        PooledObject pooled = obj.AddComponent<PooledObject>();
        pooled.OriginalPrefab = prefab;
        return obj;
    }

    private void ConfigureExhibition(GameObject exhibition)
    {
        exhibition.SetActive(true);
        exhibition.transform.SetParent(content); // 根据需求调整父对象
        activeExhibitions.Enqueue(exhibition);
    }

    private void ManageExhibitionLimit()
    {
        if (activeExhibitions.Count <= maxExhibitions) return;

        GameObject oldest = activeExhibitions.Dequeue();
        ReturnToPool(oldest);
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        PooledObject pooled = obj.GetComponent<PooledObject>();
        if (pooled && pooled.OriginalPrefab)
            objectPools[pooled.OriginalPrefab].Enqueue(obj);
    }

    public void AddCost(int cost) => Cost += cost;
    public void RefreshCost()
    {
        UICostText.GetComponent<Text>().text = "$" + Cost;
    }
    public void Buy()
    {
        // 扣除金币
        LocalData.Instance.PlayerInfo.Spend(Cost);

        // 添加到背包
        foreach (var item in purchasedItems)
        {
            ShopItem shopItem = item.Key;
            int amount = item.Value;
            InventoryManager.Instance.AddItem(shopItem.ID, amount, shopItem.Type);
        }

        // 清空购买记录
        purchasedItems.Clear();
        Cost = 0;
        RefreshCost();

        // 重置所有商品的购买数量显示
        foreach (Transform child in content)
        {
            ShopCell cell = child.GetComponent<ShopCell>();
            if (cell != null)
                cell.ResetOrderAmount();
        }
    }
    private void UpdateButton()
    {
        if (Cost == 0)
        {
            BuyButton.SetActive(false);
            return;
        }
        BuyButton.SetActive(true);
        bool canAfford = Cost <= LocalData.Instance.PlayerInfo.CheckGold;
        BuyButton.GetComponent<Button>().interactable = canAfford;
        ButtonText.text = canAfford ? "成交！" : "金币不足";
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
public class PooledObject : MonoBehaviour
{
    public GameObject OriginalPrefab;
}