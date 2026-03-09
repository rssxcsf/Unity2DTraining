using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cmd
{

    [MenuItem("CMD/读取StatePanel")]
    public static void ReadStatePanel()
    {
        List<InventoryItem> potion = LocalData.Instance.LoadPotionsInInventory();
        foreach (InventoryItem i in potion)
        {
            Debug.Log(string.Format("【ID】:{0}，【num】:{1}", i.id, i.stackCount));
        }
    }
    [MenuItem("CMD/读取任务数据")]
    public static void Readquestdata()
    {
        List<QuestData> q = GameManager.Instance.GetQuestsData();
        foreach (QuestData qq in q)
        {
            Debug.Log(q);
        }
    }
    [MenuItem("CMD/读取任务")]
    public static void Read()
    {
        List<ActiveQuest> r = LocalData.Instance.LoadActiveQuestsData();
        foreach(ActiveQuest rr in r)
        {
            Debug.Log(rr.currentAmount);
            Debug.Log(rr.Data.Name);
        }
    }
    [MenuItem("CMD/创建背包测试数据")]
    public static void CreateLocalInventoryData()
    {
        LocalData.Instance.Potions = new List<InventoryItem>();
        InventoryItem inventoryLocalItem1 = new()
        {
            uid = Guid.NewGuid().ToString(),
            id =1,
            type = ItemType.Potion,
            stackCount = 7,
        };
        InventoryItem inventoryLocalItem2 = new()
        {
            uid = Guid.NewGuid().ToString(),
            id = 2,
            type = ItemType.Potion,
            stackCount = 7,
        };
        LocalData.Instance.Potions.Add(inventoryLocalItem1);
        LocalData.Instance.Potions.Add(inventoryLocalItem2);
        LocalData.Instance.Save();
    }
    [MenuItem("CMD/创建任务测试数据")]
    public static void CreateLocalQuestsData()
    {
        LocalData.Instance.ActiveQuests = new List<ActiveQuest>();
        ActiveQuest quest = new ActiveQuest(GameManager.Instance.GetQuestByID("1"));
        LocalData.Instance.ActiveQuests.Add(quest);
        LocalData.Instance.Save();
    }
    [MenuItem("CMD/打开地图")]
    public static void LoadPlain()
    {
        UIManager.Instance.OpenPanel("GameMapPanel");
    }
    [MenuItem("CMD/打开任务面板")]
    public static void OpenQuest()
    {
        UIManager.Instance.OpenPanel("QuestPanel");
    }
    [MenuItem("CMD/打开商店")]
    public static void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }
    [MenuItem("CMD/查看玩家信息")]
    public static void ReadPlayerInfo()
    {
        LocalData.Instance.LoadPlayerInfo();
        Debug.Log(LocalData.Instance.PlayerInfo.CheckGold);
    }
    [MenuItem("CMD/创建玩家测试数据")]
    public static void CreatePlayerInfo()
    {
        LocalData.Instance.PlayerInfo = new PlayerInfo();
        LocalData.Instance.PlayerInfo.Earn(100);
        LocalData.Instance.Save();
    }
}