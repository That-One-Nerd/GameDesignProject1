using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : Singleton<DungeonGenerator>
{
    // TODO: Add a weighting system.

    public Transform DungeonRoot;
    public string StartingRoomId;
    public Vector2Int RoomCountRange;

    private RoomObject[] possibleRooms;

    private void Start()
    {
        possibleRooms = Resources.LoadAll<RoomObject>("Rooms");
        Debug.Log($"Loaded {possibleRooms.Length} room assets.");
        MakeDungeon();
    }

    public void MakeDungeon()
    {
        int rooms = Random.Range(RoomCountRange.x, RoomCountRange.y + 1);
        Debug.Log($"{rooms} rooms will be generated.");

        List<RoomDoor> emptyDoors = new List<RoomDoor>();
        RoomObject activeRoom = GetRoomById(StartingRoomId, true);
        emptyDoors.AddRange(activeRoom.Doors);

        while (rooms > 0 && emptyDoors.Count > 0)
        {
            // Pick random empty door and spawn a room there.
            RoomDoor door = emptyDoors[Random.Range(0, emptyDoors.Count)];
            RoomDoor matchingDoor = GetRoomDoorByTag(door.Tag, true);
            if (matchingDoor == null)
            {
                emptyDoors.Remove(door);
                continue;
            }

            matchingDoor.SetRoomRotationByDoor(door.transform.rotation.eulerAngles);
            matchingDoor.SetRoomPositionByDoor(door.transform.position);

            List<RoomDoor> extraDoors = new List<RoomDoor>(matchingDoor.Room.Doors);
            extraDoors.Remove(matchingDoor);
            emptyDoors.AddRange(extraDoors);

            rooms--;
        }
    }

    public RoomObject GetRoomById(string id, bool instantiate)
    {
        RoomObject[] could = possibleRooms.Where(x => x.Identifier.StartsWith(id)).ToArray();
        RoomObject chosen = could[Random.Range(0, could.Length)];

        if (instantiate) return Instantiate(chosen, DungeonRoot);
        else return chosen;
    }
    public RoomDoor GetRoomDoorByTag(string tag, bool instantiate)
    {
        RoomObject[] could = possibleRooms.Where(x => x.Doors.Any(y => y.Tag.StartsWith(tag))).ToArray();
        if (could.Length == 0) return null;

        RoomObject chosen = could[Random.Range(0, could.Length)];
        if (instantiate) chosen = Instantiate(chosen, DungeonRoot);

        RoomDoor[] couldDoors = chosen.Doors.Where(x => x.Tag.StartsWith(tag)).ToArray();
        return couldDoors[Random.Range(0, couldDoors.Length)];
    }
}
