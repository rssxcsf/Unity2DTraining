using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ILootable
{
    void Drop(Vector3 position);
    bool CanDrop { get; }
}

[System.Serializable]
public class ItemDrop
{
    public WorldItem item;
    public float rate;
}
public class ItemLootSystem : ILootable
{
    private List<ItemDrop> _drops;
    private bool _isDropped;

    public ItemLootSystem(List<ItemDrop> drops)
    {
        _drops = drops;
    }

    public void Drop(Vector3 position)
    {
        if (_isDropped) return;
        float randomValue = Random.value;
        float accumulatedRate = 0f;

        foreach (var drop in _drops)
        {
            accumulatedRate += drop.rate;
            if (randomValue <= accumulatedRate)
            {
                if (drop.item != null)
                {
                    drop.item.Drop(position);
                }
                _isDropped = true;
                return;
            }
        }
    }
    public bool CanDrop => !_isDropped;
}
