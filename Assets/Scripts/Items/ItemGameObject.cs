using UnityEngine;

[ExecuteAlways]
public class ItemGameObject : MonoBehaviour
{
    public float PickupDistance = 1.5f;
    public float PickupSize = 1.375f;
    public ItemBase ItemData;

    public bool Collected { get; protected set; }

    private Collider2D col;
    private PlayerController player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 initialSize;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        initialSize = transform.localScale;
    }

    private bool lastCouldPickup;

    private void Update()
    {
        if (!Application.isPlaying)
        {
            Awake();
            sr.sprite = ItemData.Sprite;
            return;
        }

        float desiredSize, pickupDistSqr = PickupDistance * PickupDistance;

        Vector2 diff = player.transform.position - transform.position;
        float diffMagSqr = diff.x * diff.x + diff.y * diff.y;
        if (!Collected && diffMagSqr <= pickupDistSqr)
        {
            if (!lastCouldPickup) OnEnterPickupRange();
            OnStayPickupRange();

            desiredSize = PickupSize;
            lastCouldPickup = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (player.TryPickupItem(ItemData))
                {
                    desiredSize = 1;
                    OnPickup();
                }
                else OnPickupFailed();
            }
        }
        else
        {
            if (lastCouldPickup) OnExitPickupRange();

            desiredSize = 1;
            lastCouldPickup = false;
        }

        transform.localScale = Vector2.Lerp(transform.localScale, initialSize * desiredSize, Time.deltaTime * 15);

        if (col != null) col.enabled = !Collected;
        if (Collected)
        {
            Vector3 playerPos = player.transform.position, curPos = transform.position;
            transform.position = Vector3.Lerp(curPos, playerPos, Time.deltaTime * 10);

            if (sr != null)
            {
                Color clearCol = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                sr.color = Color.Lerp(sr.color, clearCol, Time.deltaTime * 10);

                if (sr.color.a <= 1e-3) Destroy(gameObject);
            }

            if (diffMagSqr <= 1e-4) Destroy(gameObject);
        }
    }

    public void GiveRandomVelocity(float max = 10)
    {
        if (rb == null) return;

        Vector2 dir = Random.insideUnitCircle;
        rb.velocity = dir * max;
    }

    public virtual void OnDrop() { }
    protected virtual void OnEnterPickupRange() { }
    protected virtual void OnStayPickupRange() { }
    protected virtual void OnExitPickupRange() { }
    protected virtual void OnPickup()
    {
        Collected = true;
        ItemData.OnPickup();
    }
    protected virtual void OnPickupFailed() { }
}
