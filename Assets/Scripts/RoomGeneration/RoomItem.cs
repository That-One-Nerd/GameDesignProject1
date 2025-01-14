using UnityEngine;

[ExecuteAlways]
public abstract class RoomItem : MonoBehaviour
{
    public RoomObject ParentRoom { get; private set; }

    protected virtual void Awake()
    {
        ParentRoom = GetComponentInParent<RoomObject>();
    }

    protected virtual void Update()
    {
        if (!Application.isPlaying)
        {
            EditorUpdate();
            return;
        }
    }
    protected virtual void EditorUpdate()
    {
        if (ParentRoom == null)
        {
            Debug.LogError("A room item must have a parent room.");
            return;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (ParentRoom != null) ParentRoom.OnDrawGizmosSelected();
    }
}
