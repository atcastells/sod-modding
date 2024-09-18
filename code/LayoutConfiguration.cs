using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000774 RID: 1908
[CreateAssetMenu(fileName = "layoutconfig_data", menuName = "Database/Decor/Layout Configuration")]
public class LayoutConfiguration : SoCustomComparison
{
	// Token: 0x0400394C RID: 14668
	[Header("Zoning")]
	public bool assignPurpose = true;

	// Token: 0x0400394D RID: 14669
	[DisableIf("assignPurpose")]
	public AddressPreset addressPreset;

	// Token: 0x0400394E RID: 14670
	[Space(7f)]
	public bool publicFacing = true;

	// Token: 0x0400394F RID: 14671
	public bool isOutside;

	// Token: 0x04003950 RID: 14672
	public bool isLobby;

	// Token: 0x04003951 RID: 14673
	[Header("Room Configuration")]
	public List<RoomTypePreset> roomLayout = new List<RoomTypePreset>();

	// Token: 0x04003952 RID: 14674
	[Tooltip("This room may require an internal hallway to connect far rooms")]
	public bool requiresHallway = true;

	// Token: 0x04003953 RID: 14675
	public RoomConfiguration hallway;

	// Token: 0x04003954 RID: 14676
	[Tooltip("How far away a room is from the entrance to place a hallway (nodes).")]
	public int hallwayDistanceThreshold = 6;

	// Token: 0x04003955 RID: 14677
	[Tooltip("Use the building's default design style")]
	public bool useBuildingDesignStyle;

	// Token: 0x04003956 RID: 14678
	[Header("Interface")]
	public bool overrideEvidencePhotoSettings;

	// Token: 0x04003957 RID: 14679
	[EnableIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoPos = new Vector3(0f, 2.25f, 23f);

	// Token: 0x04003958 RID: 14680
	[EnableIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoEuler = new Vector3(-45f, 180f, 0f);

	// Token: 0x04003959 RID: 14681
	[Header("Doorways")]
	public List<DoorPairPreset> doorwaysNormal = new List<DoorPairPreset>();

	// Token: 0x0400395A RID: 14682
	public List<DoorPairPreset> doorwaysFlat = new List<DoorPairPreset>();

	// Token: 0x0400395B RID: 14683
	public List<DoorPairPreset> roomDividersLeft = new List<DoorPairPreset>();

	// Token: 0x0400395C RID: 14684
	public List<DoorPairPreset> roomDividersCentre = new List<DoorPairPreset>();

	// Token: 0x0400395D RID: 14685
	public List<DoorPairPreset> roomDividersRight = new List<DoorPairPreset>();
}
