using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007C8 RID: 1992
[CreateAssetMenu(fileName = "roomtype_data", menuName = "Database/Room Type Preset")]
public class RoomTypePreset : SoCustomComparison
{
	// Token: 0x04003D09 RID: 15625
	[Header("Contents")]
	public RoomConfiguration forceConfiguration;

	// Token: 0x04003D0A RID: 15626
	[Range(0f, 1f)]
	[Header("Size Settings")]
	[Tooltip("Chance this has of being included")]
	public float chance = 1f;

	// Token: 0x04003D0B RID: 15627
	[Range(0f, 90f)]
	[Tooltip("The address has to be this big for this rooom to be included")]
	public int minimumAddressSize = 18;

	// Token: 0x04003D0C RID: 15628
	[Tooltip("Maximum number of these room types per addresses")]
	[Range(1f, 10f)]
	public int maximumRoomTypesPerAddress = 1;

	// Token: 0x04003D0D RID: 15629
	[Space(7f)]
	[Tooltip("The priority given to this room: Higher priority rooms will override size variables of others.")]
	[Range(0f, 10f)]
	public int cyclePriority = 5;

	// Token: 0x04003D0E RID: 15630
	[Header("Room Shape Settings")]
	[Tooltip("The room at it's smallest must be able to fit this shape inside it.")]
	public Vector2 minimumRoomAreaShape = new Vector2(2f, 2f);

	// Token: 0x04003D0F RID: 15631
	[Tooltip("The room at it's largest can feature this shape.")]
	public Vector2 maximumRoomAreaShape = new Vector2(10f, 10f);

	// Token: 0x04003D10 RID: 15632
	[Tooltip("All nodes in the room must tesselate with this shape.")]
	public Vector2 tesselationShape = new Vector2(2f, 2f);

	// Token: 0x04003D11 RID: 15633
	[Header("Room Placement Weighting")]
	[Range(-3f, 3f)]
	public int floorSpaceWeight = 1;

	// Token: 0x04003D12 RID: 15634
	[Range(-3f, 3f)]
	public int exteriorWallWeight = 1;

	// Token: 0x04003D13 RID: 15635
	[Range(-3f, 3f)]
	public int exteriorWindowWeight = 1;

	// Token: 0x04003D14 RID: 15636
	[Range(-3f, 3f)]
	public int entranceWeight;

	// Token: 0x04003D15 RID: 15637
	[Header("Adjoining Rules")]
	[Tooltip("This must adjoin one of these rooms. If none adjoin all.")]
	[ReorderableList]
	public List<RoomTypePreset> mustAdjoinRooms = new List<RoomTypePreset>();

	// Token: 0x04003D16 RID: 15638
	[Tooltip("Should I feature doors from this preset or the bordering preset? Choose the highest priority")]
	[Range(0f, 10f)]
	public int doorPriority = 1;

	// Token: 0x04003D17 RID: 15639
	[Tooltip("Chance of doorway not featuring a door")]
	[Range(0f, 1f)]
	public float chanceOfNoDoor;

	// Token: 0x04003D18 RID: 15640
	[Tooltip("Maximum number of doors")]
	public int maxDoors = 99;

	// Token: 0x04003D19 RID: 15641
	public bool forceNoDoors;

	// Token: 0x04003D1A RID: 15642
	[Tooltip("Doors will use the 'highest' privacy setting between the 2 rooms")]
	public NewDoor.DoorSetting doorSetting;

	// Token: 0x04003D1B RID: 15643
	[Tooltip("Entrances to this room can be replaced by a divider if there are 2 or more adjoining walls")]
	public bool allowRoomDividers;

	// Token: 0x04003D1C RID: 15644
	[EnableIf("allowRoomDividers")]
	public int maxDividers = 99;

	// Token: 0x04003D1D RID: 15645
	[Tooltip("Only allow room dividers from these room types:")]
	[ReorderableList]
	public List<RoomTypePreset> onlyAllowDividersAdjoining = new List<RoomTypePreset>();

	// Token: 0x04003D1E RID: 15646
	[Header("Special Rules")]
	[Tooltip("If true the address entrance is allowed to open into this room")]
	public bool allowMainAddressEntrance;

	// Token: 0x04003D1F RID: 15647
	[Tooltip("If true the address entrance is allowed to open into this room")]
	public bool allowSecondaryAddressEntrance;

	// Token: 0x04003D20 RID: 15648
	[Tooltip("If both are present, prefer main entrance. Doubles entrance ranking score, so make sure it is >0")]
	public bool preferMainAddressEntrance = true;

	// Token: 0x04003D21 RID: 15649
	[Tooltip("If true this must be connected to an entrance, either by the entrance opening into it or otherwise through a created hallway. This will also skip the doorway creation process for this room.")]
	public bool mustConnectWithEntrance;

	// Token: 0x04003D22 RID: 15650
	[Tooltip("If true this room can be overriden with no penalty")]
	public bool overridable;

	// Token: 0x04003D23 RID: 15651
	[Tooltip("This room can be overwritten rooms with priority up to this value")]
	[Range(0f, 10f)]
	public int overwriteWithPriorityUpTo = 10;

	// Token: 0x04003D24 RID: 15652
	[Tooltip("Also block overrides from these room types")]
	[ReorderableList]
	public List<RoomTypePreset> blockOverridesFromType = new List<RoomTypePreset>();

	// Token: 0x04003D25 RID: 15653
	[Tooltip("If  the block list above is active then it makes sense to limit this room to only being overwritten once, so rooms outside of this block list can't then overwrite this node again.")]
	public int overwriteLimit = 999;

	// Token: 0x04003D26 RID: 15654
	[Tooltip("If true this room will exapnd into null space after all rooms are assigned, regardness of shape settings above.")]
	public bool expandIntoNull;

	// Token: 0x04003D27 RID: 15655
	[Tooltip("Limitations to null room expansion: Will only choose to expand if there are this many or more adjacent tiles")]
	public int expandIntoNullAdjacencyMinimum = 2;

	// Token: 0x04003D28 RID: 15656
	[Tooltip("If true this will share common features with adjacent rooms of the same type (such as light switches and ceiling light styles")]
	public bool shareFeaturesWithCommonAdjacent;

	// Token: 0x04003D29 RID: 15657
	[Tooltip("If true corridor ends can be integrated into this room (eg ends of hallways etc)")]
	public bool allowCorridorReplacement;

	// Token: 0x04003D2A RID: 15658
	[Header("Floor Settings Override")]
	public bool overrideFloorHeight;

	// Token: 0x04003D2B RID: 15659
	[EnableIf("overrideFloorHeight")]
	public int floorHeight;

	// Token: 0x04003D2C RID: 15660
	[Header("Debug")]
	public RoomConfiguration copyFrom;
}
