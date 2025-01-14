using UnityEngine;

public class AppearChance : MonoBehaviour
{
    [Range(0, 100f)]
    [InspectorName("% Chance to Appear")]
    public float PercentChance;

    private void Awake()
    {
        if (Random.Range(0, 100f) > PercentChance) Destroy(gameObject);
    }
}
