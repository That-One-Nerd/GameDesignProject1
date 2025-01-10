using System;
using UnityEngine;

[Serializable]
public class ItemBase : IEquatable<ItemBase>
{
    public string HumanName;
    public string Id;

    [TextArea]
    public string Description;

    public int MaxSlotSize;

    public virtual bool Equals(ItemBase other) => Id == other.Id;
}
