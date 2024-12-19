using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerStats BaseStats;

    [Header("Movement")]
    public RoomObject Room;
    public float Speed;

    [Header("Health")]
    public int CurrentHealth;
    public float OverhealDegradation;
    public float OverhealAcceleration;

    private float overhealTimer;

    private CameraController cam;
    private DungeonGenerator generator;
    private Rigidbody2D rb;

    private void Awake()
    {
        cam = FindObjectOfType<CameraController>();
        generator = FindObjectOfType<DungeonGenerator>();
        rb = GetComponent<Rigidbody2D>();

        generator.OnDungeonRegenerated.AddListener(OnDungeonGenerated);
    }

    private void OnDungeonGenerated()
    {
        Room = generator.DungeonStartingRoom;
        cam.UpdateCamera();
    }

    private void Update()
    {
        MovementTick();
        OverhealTick();
    }

    private void MovementTick()
    {
        Vector2 axis = new Vector2(Input.GetAxisRaw("Horizontal"),
                                   Input.GetAxisRaw("Vertical"));
        rb.velocity = Vector2.ClampMagnitude(axis, 1) * Speed;
    }
    private void OverhealTick()
    {
        if (CurrentHealth > BaseStats.MaxHealth)
        {
            while (overhealTimer < 0)
            {
                CurrentHealth--;
                overhealTimer += 1 / (OverhealDegradation + OverhealAcceleration * (CurrentHealth - BaseStats.MaxHealth));
            }
            overhealTimer -= Time.deltaTime;
        }
        else
        {
            overhealTimer = 1;
        }
    }
}
