using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Demo/QuestData")]
public class QuestsData : ScriptableObject
{
    public List<QuestData> questsData = new List<QuestData>();
}
