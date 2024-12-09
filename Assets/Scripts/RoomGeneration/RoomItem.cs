using UnityEngine;

[ExecuteAlways]
public abstract class RoomItem : MonoBehaviour
{
    public RoomObject Room { get; private set; }

    private void Awake()
    {
        Room = GetComponentInParent<RoomObject>();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            EditorUpdate();
            return;
        }
    }
    private void EditorUpdate()
    {
        Awake();
        if (Room == null)
        {
            Debug.LogError("A room item must have a parent room.");
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Room.OnDrawGizmosSelected();
    }
}
