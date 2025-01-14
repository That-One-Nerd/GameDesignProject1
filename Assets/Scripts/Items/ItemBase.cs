using System;
using UnityEngine;

[Serializable]
public class ItemBase : IEquatable<ItemBase>
{
    public string HumanName;
    public string Id;
    public Sprite Sprite;

    [TextArea]
    public string Description;

    public int MaxSlotSize;

    public virtual bool Equals(ItemBase other) => Id == other.Id && HumanName == other.HumanName && MaxSlotSize == other.MaxSlotSize;
    public virtual void OnPickup() { }
    public virtual void OnInventoryTick() { }
}
