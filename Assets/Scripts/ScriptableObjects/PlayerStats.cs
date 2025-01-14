using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int MaxHealth;

    public void Reset()
    {
        MaxHealth = 100;
    }
}
