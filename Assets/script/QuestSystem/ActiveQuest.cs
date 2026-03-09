using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Collect,
    Kill,
    ReachLocation
}

// 任务状态枚举
public enum QuestStatus
{
    NotAccepted,//未接取
    InProgress,//进行中
    Completable,//可完成
    Claimed,//已完成
}
[Serializable]
public class ActiveQuest
{
    public QuestData Data;
    public QuestStatus status;
    public int currentAmount;
    public ActiveQuest(QuestData data)
    {
        Data = data;
        status = QuestStatus.InProgress;
        currentAmount = 0;
    }
}