using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ScreenBuffer;
    public float MinSize;

    private Camera cam;
    private PlayerController player;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        Bounds toInclude = player.Room.GetFullBounds();
        cam.transform.position = new Vector3(toInclude.center.x, toInclude.center.y, -10);

        float roomAspect = toInclude.extents.y / toInclude.extents.x,
              screenAspect = (float)Screen.height / Screen.width;

        float newSize;
        if (screenAspect >= roomAspect)
        {
            // Clamp X
            newSize = toInclude.extents.x * screenAspect;
        }
        else
        {
            // Clamp Y
            newSize = toInclude.extents.y;
        }
        cam.orthographicSize = Mathf.Max(newSize * (1 + ScreenBuffer), MinSize);
    }
}
