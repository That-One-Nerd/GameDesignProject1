using UnityEngine;

[ExecuteAlways]
public abstract class RoomItem : MonoBehaviour
{
    public RoomObject ParentRoom { get; private set; }

    private void Awake()
    {
        ParentRoom = GetComponentInParent<RoomObject>();
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
        if (ParentRoom == null)
        {
            Debug.LogError("A room item must have a parent room.");
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        ParentRoom.OnDrawGizmosSelected();
    }
}
