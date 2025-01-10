using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerInventory : ScriptableObject
{
    public int MaxSpace;
    public List<ItemSlot> ItemSlots;
}
