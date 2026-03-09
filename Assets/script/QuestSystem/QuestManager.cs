using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private List<ActiveQuest> activeQuests = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeQuests();
        RegisterEvents();
    }

    private void InitializeQuests()
    {
        activeQuests = LocalData.Instance.LoadActiveQuestsData();
    }
    private void RegisterEvents()
    {
        EventManager.OnEnemyKilled += OnEnemyKilled;
        EventManager.OnItemCollected += OnItemCollected;
    }
    private void CheckQuestProgress(QuestType type, string targetTag)
    {
        //记得检查这里2025.3.14
        foreach (ActiveQuest activeQuest in activeQuests)
        {
            if (activeQuest.status == QuestStatus.InProgress &&
                activeQuest.Data.Type == type &&
                activeQuest.Data.TargetTag == targetTag)
            {
                activeQuest.currentAmount++;

                if (activeQuest.currentAmount >= activeQuest.Data.RequiredAmount)
                {
                    activeQuest.status = QuestStatus.Completable;
                    EventManager.TriggerQuestCompleted(activeQuest);
                }
            }
        }
    }
    // 事件处理
    private void OnEnemyKilled(string enemyTag)
    {
        CheckQuestProgress(QuestType.Kill, enemyTag);
    }

    private void OnItemCollected(string itemTag)
    {
        CheckQuestProgress(QuestType.Collect, itemTag);
    }
}