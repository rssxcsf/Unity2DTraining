using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Action<string> OnEnemyKilled;
    public static Action<string> OnItemCollected;
    public static Action<ActiveQuest> OnQuestCompleted;

    public static void TriggerEnemyKilled(string enemyTag)
    {
        OnEnemyKilled?.Invoke(enemyTag);
    }

    public static void TriggerItemCollected(string itemTag)
    {
        OnItemCollected?.Invoke(itemTag);
    }

    public static void TriggerQuestCompleted(ActiveQuest quest)
    {
        OnQuestCompleted?.Invoke(quest);
    }
}