using System;
using UnityEngine;

[Serializable]
public class LootTable
{
    public Entry[] Entries;
    public Vector2Int ItemCountRange;

    [Serializable]
    public class Entry
    {
        public ItemGameObject Item;
        public float Weight;
    }
}
