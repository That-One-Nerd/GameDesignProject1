using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private Image[] displays;
    private TextMeshProUGUI[] counters;
    private PlayerController player;

    private Image background;

    private readonly object TOOLTIP_LOCK = new object();
    private Tooltip tooltip;

    private void Awake()
    {
        Transform slotsRoot = transform.Find("Slots");
        displays = new Image[slotsRoot.childCount];
        counters = new TextMeshProUGUI[slotsRoot.childCount];
        for (int i = 0; i < slotsRoot.childCount; i++)
        {
            Transform child = slotsRoot.GetChild(i);
            displays[i] = child.Find("Item Display").GetComponent<Image>();
            counters[i] = child.Find("Count Display").GetComponent<TextMeshProUGUI>();
        }

        background = GetComponent<Image>();
        player = FindObjectOfType<PlayerController>();

        tooltip = FindObjectOfType<Tooltip>();
    }

    private void Update()
    {
        bool active = Input.GetKey(KeyCode.E);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
        background.enabled = active;

        if (active)
        {
            RefreshInventory();
            TickTooltip();
        }
        else tooltip.RemoveRequest(TOOLTIP_LOCK);
    }

    private void TickTooltip()
    {
        Vector2 mousePos = Input.mousePosition;

        int hovering = -1;
        for (int i = 0; i < displays.Length; i++)
        {
            Image img = displays[i];
            Vector2 min = img.rectTransform.position,
                    max = min + img.rectTransform.sizeDelta;

            Debug.Log(img.rectTransform.sizeDelta);

            if (min.x > max.x) (min.x, max.x) = (max.x, min.x);
            if (min.y > max.y) (min.y, max.y) = (max.y, min.y);

            if (mousePos.x >= min.x && mousePos.x <= max.x &&
                mousePos.y >= min.y && mousePos.y <= max.y)
            {
                hovering = i;
                break;
            }
        }
        if (hovering == -1 || hovering >= player.Inventory.ItemSlots.Count)
        {
            tooltip.RemoveRequest(TOOLTIP_LOCK);
            return;
        }

        ItemSlot slot = player.Inventory.ItemSlots[hovering];
        if (slot.Item == null)
        {
            tooltip.RemoveRequest(TOOLTIP_LOCK);
            return;
        }
        Tooltip.Request request = new Tooltip.Request()
        {
            Reference = TOOLTIP_LOCK,
            Priority = 10,
            Title = slot.Item.HumanName,
            Content = slot.Item.Description,
            Footer = $"{slot.Count} / {slot.Item.MaxSlotSize}",
            Position = null,
        };
        tooltip.SetRequest(request);
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < player.Inventory.MaxSpace; i++)
        {
            if (i >= displays.Length) break;
            else if (i >= player.Inventory.ItemSlots.Count || player.Inventory.ItemSlots[i].Item == null)
            {
                displays[i].gameObject.SetActive(false);
                counters[i].gameObject.SetActive(false);
                continue;
            }
            displays[i].gameObject.SetActive(true);
            displays[i].sprite = player.Inventory.ItemSlots[i].Item.Sprite;

            counters[i].gameObject.SetActive(player.Inventory.ItemSlots[i].Count > 1);
            counters[i].text = $"x{player.Inventory.ItemSlots[i].Count}";
        }
    }
}
