public class RoomDoor : RoomItem
{
    public float AssignWeight = 1;
    public RoomDoor Match;
    public bool Disabled;
    public string[] Tags;
    public TagWeight[] AllowedMatches;
}
