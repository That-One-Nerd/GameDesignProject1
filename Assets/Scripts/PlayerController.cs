using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Animations")]
    public float AnimationSpeed;

    private float overhealTimer;

    private Animator anim;
    private CameraController cam;
    private DungeonGenerator generator;
    private Rigidbody2D rb;

    private void Awake()
    {
        Active = true;

        anim = GetComponent<Animator>();
        cam = FindObjectOfType<CameraController>();
        generator = FindObjectOfType<DungeonGenerator>();
        rb = GetComponent<Rigidbody2D>();

        generator.OnDungeonRegenerated.AddListener(OnDungeonGenerated);

        BaseStats.Reset();
        Inventory.Reset();

        CurrentHealth = 300;
    }

    private void OnDungeonGenerated()
    {
        Room = generator.DungeonStartingRoom;
        cam.UpdateCamera();
    }

    private void Update()
    {
        MovementTick();
        //OverhealTick();
        InventoryTick();

        TempDeathTick();
    }

    private bool ready = false;
    private void TempDeathTick()
    {
        if (Room != DungeonGenerator.Instance.DungeonStartingRoom) ready = true;
        if (!ready) return;

        if (CurrentHealth > 1)
        {
            while (overhealTimer < 0)
            {
                CurrentHealth--;
                overhealTimer += Mathf.Max(0.002f * CurrentHealth, Time.deltaTime * 1.001f);
            }
            overhealTimer -= Time.deltaTime;
        }
        else
        {
            // Janky ahh code. It works for now though.
            CurrentHealth = 0;
            Active = false;
            GetComponent<SpriteRenderer>().enabled = false;
            FindObjectOfType<InventoryDisplay>()?.RefreshInventory();
            Destroy(FindObjectOfType<InventoryDisplay>());
            int count = (from slot in Inventory.ItemSlots select slot.Count).Sum();
            GameObject.Find("Heads Up Display").transform.Find("Inventory").GetComponent<RawImage>().enabled = true;
            GameObject.Find("Heads Up Display").transform.Find("Inventory").Find("Slots").gameObject.SetActive(true);
            GameObject.Find("Heads Up Display").transform.Find("Finish Text").GetComponent<TextMeshProUGUI>().text =
                $"You collected a total of {count} items!\n" +
                 "Press [R] to retry or to challenge a friend!";
            GameObject.Find("Heads Up Display").transform.Find("Finish Text").gameObject.SetActive(true);
            overhealTimer = 1;

            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(GameObject.Find("Heads Up Display"));
                Destroy(GameObject.Find("Dungeon Root"));
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }
        }
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

        float mag = axis.magnitude;
        if (mag > 0.1)
        {
            anim.SetInteger("State", 1);
            anim.SetFloat("Speed", mag * AnimationSpeed);
        }
        else anim.SetInteger("State", 0);
    }
    private void OverhealTick()
    {
        if (CurrentHealth > BaseStats.MaxHealth)
        {
            while (overhealTimer < 0)
            {
                CurrentHealth--;
                FindObjectOfType<HealthBar>().DecreaseCurrentHealthBefore();
                overhealTimer += 1 / (OverhealDegradation + OverhealAcceleration * (CurrentHealth - BaseStats.MaxHealth));
            }
            overhealTimer -= Time.deltaTime;
        }
        else
        {
            overhealTimer = 1;
        }
    }
    private void InventoryTick()
    {
        for (int i = 0; i < Inventory.ItemSlots.Count; i++)
        {
            ItemSlot slot = Inventory.ItemSlots[i];
            if (slot.Item == null) continue;
            slot.Item.OnInventoryTick();
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
