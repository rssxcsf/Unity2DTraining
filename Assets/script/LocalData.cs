using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalData
{
    private static LocalData _instance;
    public static LocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalData();
                if (PlayerPrefs.HasKey("LocalData"))
                {
                    LocalData loadedData = _instance.Load();
                    _instance.Materials = loadedData.Materials ?? new List<InventoryItem>();
                    _instance.Potions = loadedData.Potions ?? new List<InventoryItem>();
                    _instance.ActiveQuests = loadedData.ActiveQuests ?? new List<ActiveQuest>();
                    _instance.PlayerInfo = loadedData.PlayerInfo ?? new PlayerInfo();
                }
                else
                {
                    _instance.Materials = new List<InventoryItem>();
                    _instance.Potions = new List<InventoryItem>();
                    _instance.ActiveQuests = new List<ActiveQuest>();
                    _instance.PlayerInfo = new PlayerInfo();
                }
            }
            return _instance;
        }
    }
    public List<InventoryItem> Materials;
    public List<InventoryItem> Potions;
    public List<ActiveQuest> ActiveQuests;
    public PlayerInfo PlayerInfo;
    public LocalData Load()
    {
        string DataJson = PlayerPrefs.GetString("LocalData");
        return JsonUtility.FromJson<LocalData>(DataJson);
    }
    public void Save()
    {
        Materials ??= new List<InventoryItem>();
        Potions ??= new List<InventoryItem>();
        ActiveQuests ??= new List<ActiveQuest>();
        PlayerInfo ??= new PlayerInfo();

        string dataJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("LocalData", dataJson);
        PlayerPrefs.Save();
    }
    public PlayerInfo LoadPlayerInfo()
    {
        if (PlayerInfo != null)
        {
            return PlayerInfo;
        }
        if (PlayerPrefs.HasKey("LocalData"))
        {
            return Load().PlayerInfo;
        }
        else
        {
            PlayerInfo = new PlayerInfo();
            return PlayerInfo;
        }
    }
    public List<ActiveQuest> LoadActiveQuestsData()
    {
        if (ActiveQuests != null)
        {
            return ActiveQuests;
        }
        if (PlayerPrefs.HasKey("LocalData"))
        {
            return Load().ActiveQuests;
        }
        else
        {
            ActiveQuests = new List<ActiveQuest>();
            return ActiveQuests;
        }
    }
    public List<InventoryItem> LoadMaterialsInInventory()
    {
        if(Materials != null)
        {
            return Materials;
        }
        if(PlayerPrefs.HasKey("LocalData"))
        {
            return Load().Materials;
        }
        else
        {
            Materials = new List<InventoryItem>();
            return Materials;
        }
    }
    public List<InventoryItem> LoadPotionsInInventory()
    {
        if (Potions != null)
        {
            return Potions;
        }
        if (PlayerPrefs.HasKey("LocalData"))
        {
            return Load().Potions;
        }
        else
        {
            Potions = new List<InventoryItem>();
            return Potions;
        }
    }
}
