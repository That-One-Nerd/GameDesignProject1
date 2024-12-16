# Generation Procedure
This file details the algorithm I will be using to procedurally generate dungeons.
It's not super complex in the grand scheme of things, but I still felt the need to
write it out to help myself understand it before trying to implement it.

## Properties

### RoomObject
- Bounds
- Doors
- CanBeRotated

### RoomDoor
- ParentRoom
- Corresponding
- Disabled
- Tags
- AllowedRoomTags (tags and weights)
