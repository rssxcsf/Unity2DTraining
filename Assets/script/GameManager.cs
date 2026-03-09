using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private List<QuestData> questsData;
    [SerializeField] private GameObject EntityManager;
    [SerializeField] private GameObject AudioManager;
    [SerializeField] private GameObject QuestManager;
    [SerializeField] private GameObject SubtitleManager;
    [SerializeField] private GameObject InventoryManager;
    [SerializeField] private GameObject UIMiss;

    private bool isGamePause;
    private bool isInventoryOpen;
    private float saveTimer;
    [SerializeField] private float AutoSaveInterval;
    public static GameManager Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SetSubtitleManager();
        SetEntityManager();
        SetAudioManager();
        SetQuestManager();
        SetInventoryManager();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
        OpenInventory();
        Tick();
    }
    private void Tick()//自动保存
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= AutoSaveInterval)
        {
            LocalData.Instance.Save();
            saveTimer = 0f;
        }
    }
    private void SetInventoryManager()
    {
        if (InventoryManager != null)
            Instantiate(InventoryManager);
    }
    private void SetAudioManager()
    {
        if (AudioManager != null)
            Instantiate(AudioManager);
    }
    private void SetEntityManager()
    {
        if (EntityManager != null)
            Instantiate(EntityManager);
    }
    private void SetQuestManager()
    {
        if (QuestManager != null)
            Instantiate(QuestManager);
    }
    private void SetSubtitleManager()
    {
        if (SubtitleManager != null)
            Instantiate(SubtitleManager);
    }
    public void PauseGame()
    {
        if (!isGamePause)
        {
            UIManager.Instance.OpenPanel(UIConst.PauseMenu);
            isGamePause = true;
            Time.timeScale = 0;
        }
        else
        {
            UIManager.Instance.ClosePanel(UIConst.PauseMenu);
            Time.timeScale = 1;
            isGamePause = false;
        }
    }
    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInventoryOpen)
            {
                isInventoryOpen = true;
                Time.timeScale = 0;
                UIManager.Instance.OpenPanel(UIConst.InventoryPanel);
            }
            else
            {
                Time.timeScale = 1;
                UIManager.Instance.ClosePanel(UIConst.InventoryPanel);
                isInventoryOpen = false;
            }
        }
    }
    #region 任务系统
    public List<QuestData> GetQuestsData()
    {
        List<QuestData> quests = new List<QuestData>();
        if(questsData==null)
        {
            quests = Resources.Load<QuestsData>("Data/QuestsData").questsData;
            questsData = quests;
        }
        return questsData;
    }
    public QuestData GetQuestByID(string id)
    {
        List<QuestData> quests = GetQuestsData();
        foreach (QuestData quest in quests)
        {
            if(quest.ID==id)
                return quest;
        }
        Debug.Log("该任务不存在");
        return null;
    }
    #endregion
    public void ShowMiss(Transform transform)
    {
        GameObject miss = Instantiate(UIMiss, transform.position, Quaternion.identity);
        Destroy(miss,0.4f);
    }
}
public static class Const
{
    public const short maxStack = 32767;
}
