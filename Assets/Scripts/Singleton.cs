using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"Duplicate instance of {nameof(T)}! Ignoring this one.");
            Destroy(this);
            return;
        }
        Instance = (T)this;
    }
}
