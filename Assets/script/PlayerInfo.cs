using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInfo
{
    [SerializeField] private int Gold;
    public bool Spend(int gold)
    {
        if (gold > Gold)
        {
            return false;
        }
        Gold -= gold;
        return true;
    }
    public void Earn(int gold)
    {
        if (gold <= 0) return;
        Gold += gold;
    }
    public int CheckGold => Gold;
}
