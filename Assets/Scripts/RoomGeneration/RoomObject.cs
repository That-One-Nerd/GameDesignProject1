using UnityEngine;

[ExecuteAlways]
public class RoomObject : MonoBehaviour
{
    public string Identifier;
    public Bounds Bounds;
    public RoomDoor[] Doors;

    private void Awake()
    {
        Doors = GetComponentsInChildren<RoomDoor>();
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
        if (Bounds.extents.z != 0 || Bounds.center.z != 0)
        {
            Bounds.extents = (Vector2)Bounds.extents;
            Bounds.size = (Vector2)Bounds.size;
        }
    }

    internal void OnDrawGizmos()
    {
        // Draw room mesh.
        Gizmos.color = new Color(1, 0.5f, 0); // Orange
        foreach (RoomDoor roomDoor in Doors)
        {
            GizmosHelper.DrawLine(transform, roomDoor.transform.localPosition, Bounds.center);
        }
    }
    internal void OnDrawGizmosSelected()
    {
        // Draw room bounds.
        Gizmos.color = Color.white;
        GizmosHelper.DrawRect(transform, Bounds);

        // Draw each door.
        Gizmos.color = Color.green;
        Bounds doorBounds = new Bounds(Vector2.zero, new Vector2(2, 1));
        foreach (RoomDoor door in Doors)
        {
            GizmosHelper.DrawRect(door.transform, doorBounds);
        }
    }
}
