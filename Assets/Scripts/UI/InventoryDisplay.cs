using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    private Image[] displays;
    private TextMeshProUGUI[] counters;
    private PlayerController player;

    private Image background;

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
    }

    private void Update()
    {
        bool active = Input.GetKey(KeyCode.E);

        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(active);
        background.enabled = active;

        if (active) RefreshInventory();
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
