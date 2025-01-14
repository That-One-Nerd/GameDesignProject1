using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool Active;

    [Header("Data")]
    public PlayerStats BaseStats;
    public PlayerInventory Inventory;

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
        Active = true;

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
        if (!Active)
        {
            rb.velocity = Vector2.zero;
            return;
        }

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

    public bool TryPickupItem(ItemBase item)
    {
        // First, try and match the item with any other slot.
        for (int i = 0; i < Inventory.ItemSlots.Count; i++)
        {
            ItemSlot slot = Inventory.ItemSlots[i];
            if (slot.Item == null || (slot.Item.Equals(item) && slot.Count < slot.Item.MaxSlotSize))
            {
                // Item added to slot.
                // If the Equals() is not handled correctly,
                // this might overwrite metadata.
                slot.Item = item;
                slot.Count++;
                return true;
            }
        }

        // It doesn't fit in a slot, try and make a new slot.
        if (Inventory.ItemSlots.Count < Inventory.MaxSpace)
        {
            Inventory.ItemSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = 1
            });
            return true;
        }
        else return false; // No room to make a new slot, it doesn't fit.
    }
}
