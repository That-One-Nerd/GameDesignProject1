using UnityEngine;

public static class GizmosHelper
{
    public static void DrawLine(Transform transform, Vector3 from, Vector3 to) => DrawPoints(transform, new Vector3[2]
    {
        from,
        to
    });
    public static void DrawPoints(Transform transform, Vector3[] points)
    {
        // Apply rotation and position.
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.rotation * points[i] + transform.position;
        }

        // Draw points.
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % points.Length]);
        }
    }
    public static void DrawRect(Transform transform, Bounds bounds) => DrawPoints(transform, new Vector3[4]
    {
        new Vector2(bounds.min.x, bounds.min.y),
        new Vector2(bounds.max.x, bounds.min.y),
        new Vector2(bounds.max.x, bounds.max.y),
        new Vector2(bounds.min.x, bounds.max.y)
    });
}
