using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGenerator : Singleton<DungeonGenerator>
{
    public RoomObject DungeonStartingRoom { get; private set; }

    [Header("Generation")]
    public Transform DungeonRoot;
    public Vector2Int RoomCountRange;
    public RoomObject StartingRoomPrefab;
    public int MaxRetries;

    [Header("Callbacks")]
    public UnityEvent OnDungeonRegenerated;

    private RoomObject[] possibleRooms;

    private void Start()
    {
        possibleRooms = Resources.LoadAll<RoomObject>("Rooms");
        Debug.Log($"Loaded {possibleRooms.Length} room assets.");
        MakeDungeon();
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Return))
        {
            MakeDungeon();
        }
    }

    public void MakeDungeon() => MakeDungeon(MaxRetries);
    private void MakeDungeon(int retries)
    {
        // Delete previous rooms.
        foreach (Transform child in DungeonRoot)
        {
            Destroy(child.gameObject);
        }

        int rooms = Random.Range(RoomCountRange.x, RoomCountRange.y + 1);
        Debug.Log($"Attempting to generate {rooms} rooms.");

        List<RoomObject> addedRooms = new List<RoomObject>();
        List<RoomDoor> unassignedDoors = new List<RoomDoor>();

        // Pick random starting room.
        RoomObject startingRoom = Instantiate(StartingRoomPrefab, DungeonRoot);
        addedRooms.Add(startingRoom);
        unassignedDoors.AddRange(startingRoom.Doors);
        DungeonStartingRoom = startingRoom;

        List<Collider2D> hitBuffer = new List<Collider2D>();

        while (rooms > 0 && unassignedDoors.Count > 0)
        {
            // Pick a random available door (weighted).
            float weightSum = (from door in unassignedDoors select door.AssignWeight).Sum();
            RoomDoor chosenDoor = null;
            float weightChosen = Random.Range(0, weightSum);
            for (int doorIndex = 0; doorIndex < unassignedDoors.Count; doorIndex++)
            {
                RoomDoor possibleDoor = unassignedDoors[doorIndex];
                if (weightChosen < possibleDoor.AssignWeight)
                {
                    chosenDoor = possibleDoor;
                    break;
                }
                else weightChosen -= possibleDoor.AssignWeight;
            }
            if (chosenDoor.Disabled)
            {
                // Door is disabled, try again.
                unassignedDoors.Remove(chosenDoor);
                continue;
            }

            // Search for a door that matches the allowed tags of this one.
            // First, search for a tag to pick from (also weighted).
            List<TagWeight> matchDoorTags = new List<TagWeight>(chosenDoor.AllowedMatches);
            List<RoomObject> tempRooms = new List<RoomObject>(possibleRooms);
        _retryTag:
            weightSum = (from tag in matchDoorTags select tag.Weight).Sum();
            TagWeight chosenTag = null;
            weightChosen = Random.Range(0, weightSum);
            for (int tagIndex = 0; tagIndex < matchDoorTags.Count; tagIndex++)
            {
                TagWeight possibleTag = matchDoorTags[tagIndex];
                if (weightChosen < possibleTag.Weight)
                {
                    chosenTag = possibleTag;
                    break;
                }
                else weightChosen -= possibleTag.Weight;
            }
            if (chosenTag == null)
            {
                // None of the tags work here. Let's just skip this door.
                chosenDoor.Disabled = true;
                unassignedDoors.Remove(chosenDoor);
                continue;
            }

        _retryRoom:
            // Now that we have our tag, find all room types with matching doors
            // and pick one at random (weighted yet again).
            RoomObject[] matchRooms = tempRooms.Where(x => chosenDoor.AllowedMatches.Any(y => x.Doors.Any(z => z.Tags.Any(w => w == y.Tag)))).ToArray();
            weightSum = (from room in matchRooms select room.Weight).Sum();
            RoomObject chosenRoom = null;
            weightChosen = Random.Range(0, weightSum);
            for (int roomIndex = 0; roomIndex < matchRooms.Length; roomIndex++)
            {
                RoomObject possibleRoom = matchRooms[roomIndex];
                if (weightChosen < possibleRoom.Weight)
                {
                    chosenRoom = possibleRoom;
                    break;
                }
                else weightChosen -= possibleRoom.Weight;
            }
            if (chosenRoom == null)
            {
                // This tag has no matches. Skip it and retry the tag.
                matchDoorTags.Remove(chosenTag);
                goto _retryTag;
            }

            // Pick a random door in this room that fits the tag.
            RoomObject chosenRoomClone = Instantiate(chosenRoom, DungeonRoot);
            chosenRoomClone.name = $"Room {addedRooms.Count}"; //  - {chosenRoom.name}"
            List<RoomDoor> tempDoors = new List<RoomDoor>(chosenRoomClone.Doors);
        _retryOtherDoor:
            RoomDoor[] otherDoors = tempDoors.Where(x => chosenDoor.AllowedMatches.Any(y => x.Tags.Any(z => z == y.Tag))).ToArray();
            weightSum = (from door in otherDoors select door.AssignWeight).Sum();
            RoomDoor otherDoor = null;
            weightChosen = Random.Range(0, weightSum);
            for (int doorIndex = 0; doorIndex < otherDoors.Length; doorIndex++)
            {
                RoomDoor possibleDoor = otherDoors[doorIndex];
                if (weightChosen < possibleDoor.AssignWeight)
                {
                    otherDoor = possibleDoor;
                    break;
                }
                else weightChosen -= possibleDoor.AssignWeight;
            }
            if (otherDoor == null)
            {
                // The doors we might have thought were valid might not actually be.
                // (For instance, if the room can't be rotated). Skip this room.
                Destroy(chosenRoomClone.gameObject);
                tempRooms.Remove(chosenRoom);
                goto _retryRoom;
            }

            // Spawn the room and rotate it to the correct spot.
            if (chosenRoomClone.CanBeRotated)
            {
                float expectedDoorRot = chosenDoor.transform.rotation.eulerAngles.z + 180;
                float curDoorRot = otherDoor.transform.rotation.eulerAngles.z;
                float diff = expectedDoorRot - curDoorRot;

                Vector3 curRoomRot = chosenRoomClone.transform.rotation.eulerAngles;
                curRoomRot.z += diff;
                chosenRoomClone.transform.rotation = Quaternion.Euler(curRoomRot);
            }
            else
            {
                // Check if it's impossible.
                float diff = Mathf.Abs(otherDoor.transform.rotation.eulerAngles.z -
                                       chosenDoor.transform.rotation.eulerAngles.z);
                if (Mathf.Abs(diff - 180) >= 1e-1)
                {
                    // Must be rotated to fit. Skip door.
                    tempDoors.Remove(otherDoor);
                    goto _retryOtherDoor; // Retry the door, not the room or tag.
                }
            }

            // Move the room to position.
            Vector3 expectedDoorPos = chosenDoor.transform.position;
            Vector3 curDoorPos = otherDoor.transform.position;
            Vector3 diffPos = expectedDoorPos - curDoorPos;
            chosenRoomClone.transform.position += diffPos;
            Physics2D.SyncTransforms();

            // See if this specific transform of the room causes it to overlap with
            // any other room.
            for (int i = 0; i < chosenRoomClone.Bounds.Length; i++)
            {
                Collider2D toCheck = chosenRoomClone.Bounds[i];

                ContactFilter2D filter = new ContactFilter2D();
                filter.NoFilter();
                filter.SetLayerMask(1 << 6);

                hitBuffer.Clear();
                toCheck.OverlapCollider(filter, hitBuffer);
                if (hitBuffer.Count(x => !chosenRoomClone.Bounds.Contains(x)) > 0)
                {
                    // Overlapping another room, try a different orientation.
                    // TODO: This could be optimized by using an array, maybe.
                    tempDoors.Remove(otherDoor);
                    goto _retryOtherDoor;
                }
            }

            // Extra stuff.
            chosenDoor.Match = otherDoor;
            otherDoor.Match = chosenDoor;
            addedRooms.Add(chosenRoomClone);
            unassignedDoors.AddRange(chosenRoomClone.Doors);
            unassignedDoors.Remove(otherDoor);

            unassignedDoors.Remove(chosenDoor);
            rooms--;
        }

        if (rooms > 0)
        {
            Debug.Log($"Ran out of available doors! Skipped generating {rooms} rooms.");
        }

        // Make all remaining unavailable doors disabled.
        foreach (RoomDoor door in unassignedDoors) door.Disabled = true;

        if (addedRooms.Count < RoomCountRange.x && retries > 0)
        {
            // Too small, let's retry the whole thing.
            Debug.Log($"Only generated {addedRooms.Count} rooms! Let's try that again...");
            MakeDungeon(retries - 1);
            return;
        }

        // Dungeon completed, invoke callback.
        OnDungeonRegenerated.Invoke();

        UpdateActiveRoom(startingRoom);
    }

    public void UpdateActiveRoom(RoomObject activeRoom)
    {
        GameObject obj = activeRoom.gameObject;
        for (int i = 0; i < DungeonRoot.childCount; i++)
        {
            GameObject child = DungeonRoot.GetChild(i).gameObject;
            child.SetActive(child == obj);
        }
    }
}
