using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string uid;
    public int id;
    public ItemType type;
    public int stackCount;
    public override string ToString()
    {
        return string.Format("[id]:{0}[num]:{1}", id, stackCount);
    }
}
