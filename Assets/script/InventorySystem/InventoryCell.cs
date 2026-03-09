using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    private Transform UIIcon;
    private Transform UIName;
    private Transform UINum;

    private InventoryItem inventoryLocalData;
    private ItemData inventoryItem;
    private InventoryPanel uiParent;
    private void Awake()
    {
        InitUIName();
    }
    private void InitUIName()
    {
        UIIcon = transform.Find("Top/Icon");
        UIName = transform.Find("Bottom/Name");
        UINum = transform.Find("Center/Num");
    }
    public void Refresh(InventoryItem inventoryLocalData,InventoryPanel uiParent)
    {
        //数据初始化
        this.inventoryLocalData = inventoryLocalData;
        this.inventoryItem = InventoryManager.Instance.GetInventoryItem(inventoryLocalData.id,inventoryLocalData.type);
        this.uiParent = uiParent;

        //Texture2D t = (Texture2D)Resources.Load(this.inventoryItem.imagePath);
        //Sprite temp = Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(0,0));
        //UIIcon.GetComponent<Image>().sprite = temp;
        UIIcon.GetComponent<Image>().sprite = inventoryItem.Icon;
        UIName.GetComponent<Text>().text = inventoryItem.Name;
        UINum.GetComponent<Text>().text = "X" + inventoryLocalData.stackCount;
    }
}
