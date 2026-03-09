using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData
{
    public string ID;
    public string Name;
    public string Description;
    public QuestType Type;

    [Header("Target")]
    public string TargetTag;
    public int RequiredAmount;

    [Header("Rewards")]
    public int Experience;
    public int Gold;
    public List<WorldItem> Items;
}