using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int MaxHealth;
    public int MaxOverheals;

    public void Reset()
    {
        MaxHealth = 100;
        MaxOverheals = 2;
    }
}
