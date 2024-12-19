using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class RoomObject : MonoBehaviour
{
    public Collider2D[] Bounds;
    public RoomDoor[] Doors;
    public float Weight = 1;
    public bool CanBeRotated;

    private void Awake()
    {
        Doors = GetComponentsInChildren<RoomDoor>();
        Bounds = GetComponentsInChildren<Collider2D>().Where(x => x.gameObject.CompareTag("RoomBounds")).ToArray();

        if (Bounds.Length == 0)
        {
            const string message = "No bounds defining children detected! Make sure they have the \"RoomBounds\" tag.";
            if (Application.isPlaying) Debug.LogError(message);
            else Debug.LogWarning(message);
        }
        else
        {
            for (int i = 0; i < Bounds.Length; i++)
            {
                Collider2D col = Bounds[i];
                col.isTrigger = true;
                col.gameObject.layer = 6;
            }
        }
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
    }

    public Bounds GetFullBounds()
    {
        Bounds result = new Bounds(transform.position, Vector2.zero);
        foreach (Collider2D col in Bounds) result.Encapsulate(col.bounds);
        return result;
    }

    internal void OnDrawGizmos()
    {
        // Draw room mesh.
        Color orange = new Color(1, 0.5f, 0);
        foreach (RoomDoor roomDoor in Doors)
        {
            if (roomDoor.Disabled) Gizmos.color = Color.red;
            else Gizmos.color = orange;
            GizmosHelper.DrawLine(transform, roomDoor.transform.localPosition, Vector2.zero);
        }
    }
    internal void OnDrawGizmosSelected()
    {
        // Draw each door.
        Gizmos.color = Color.green;
        Bounds doorBounds = new Bounds(Vector2.zero, new Vector2(2, 1));
        foreach (RoomDoor door in Doors)
        {
            GizmosHelper.DrawRect(door.transform, doorBounds);
        }
    }
}
