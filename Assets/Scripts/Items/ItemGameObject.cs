using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
    public float PickupDistance = 1.5f;
    public float PickupSize = 1.375f;
    public ItemBase ItemData;

    public bool Collected { get; protected set; }

    private PlayerController player;
    private SpriteRenderer sr;

    private Vector2 initialSize;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        sr = GetComponent<SpriteRenderer>();

        initialSize = transform.localScale;
    }

    private bool lastCouldPickup;

    private void Update()
    {
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

    protected virtual void OnEnterPickupRange() { }
    protected virtual void OnStayPickupRange() { }
    protected virtual void OnExitPickupRange() { }
    protected virtual void OnPickup()
    {
        Collected = true;
    }
    protected virtual void OnPickupFailed() { }
}
