using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    public bool Opened;
    public LootTable LootTable;

    public Sprite ClosedSprite, OpenedSprite;

    private Collider2D col;
    private PlayerController player;
    private SpriteRenderer sr;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        player = FindObjectOfType<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Opened) sr.sprite = OpenedSprite;
        else sr.sprite = ClosedSprite;

        col.isTrigger = Opened;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject) Open();
    }

    public void Open()
    {
        if (Opened) return;
        DoOpenAnimation();
    }

    private async void DoOpenAnimation()
    {
        Opened = true;
        await Task.Delay(750);

        int itemCount = Random.Range(LootTable.ItemCountRange.x, LootTable.ItemCountRange.y + 1);
        float weightSum = (from entry in LootTable.Entries select entry.Weight).Sum();
        for (int i = 0; i < itemCount; i++)
        {
            // Weighted random choice.
            float weightChosen = Random.Range(0, weightSum);
            ItemGameObject chosen = null;

            for (int j = 0; j < LootTable.Entries.Length; j++)
            {
                LootTable.Entry possibleItem = LootTable.Entries[j];
                if (weightChosen < possibleItem.Weight)
                {
                    chosen = possibleItem.Item;
                    break;
                }
                else weightChosen -= possibleItem.Weight;
            }

            if (chosen == null)
            {
                Debug.LogWarning("There aren't any items to give! The loot table must be empty.");
                break;
            }

            GameObject obj = Instantiate(chosen.gameObject, transform);
            obj.transform.position = transform.position;
            ItemGameObject item = obj.GetComponent<ItemGameObject>();

            item.OnDrop();
            item.GiveRandomVelocity();

            await Task.Delay(150);
        }
    }
}
