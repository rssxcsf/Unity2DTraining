using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldItem : MonoBehaviour
{
    public string itemName;
    public int id;
    public ItemType type;
    public void PickUp()
    {
        InventoryManager.Instance.AddItem(id, 1,type);
        Destroy(gameObject);
    }
    public void Drop(Vector3 pos) =>Instantiate(gameObject, pos, Quaternion.identity);
}
