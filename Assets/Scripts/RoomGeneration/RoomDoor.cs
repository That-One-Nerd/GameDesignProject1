using UnityEngine;

public class RoomDoor : RoomItem
{
    public float AssignWeight = 1;
    public RoomDoor Match;
    public bool Disabled;
    public string[] Tags;
    public TagWeight[] AllowedMatches;

    private PlayerController player;
    private SpriteRenderer sr;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<PlayerController>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        if (!Application.isPlaying) Awake();

        if (sr != null)
        {
            if (Disabled) sr.color = Color.black;
            else sr.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            if (Disabled || TransitionManager.Instance.Transitioning) return;
            player.Active = false;
            TransitionManager.Instance.Transition(() =>
            {
                DungeonGenerator.Instance.UpdateActiveRoom(Match.ParentRoom);
                player.Room = Match.ParentRoom;
                player.transform.position = Match.transform.position + Match.transform.up * 1.5f;
                player.Active = true;
            });
        }
    }
}
