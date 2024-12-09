using UnityEngine;

public class RoomDoor : RoomItem
{
    public string Tag;

    public void SetRoomRotationByDoor(Vector3 desiredDoorRotation)
    {
        Room.transform.rotation *= Quaternion.FromToRotation(Room.transform.rotation.eulerAngles, desiredDoorRotation);
    }
    public void SetRoomPositionByDoor(Vector2 desiredDoorPosition)
    {
        Vector2 curPos = transform.position;
        Vector2 diff = desiredDoorPosition - curPos;

        Room.transform.position += (Vector3)diff;
    }
}
