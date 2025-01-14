using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInventory : ScriptableObject
{
    public int MaxSpace;
    public List<ItemSlot> ItemSlots;

    public void Reset()
    {
        MaxSpace = 25;
        ItemSlots = new List<ItemSlot>();
    }
}
