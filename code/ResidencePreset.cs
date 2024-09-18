using System;
using UnityEngine;

// Token: 0x020007B0 RID: 1968
[CreateAssetMenu(fileName = "residence_data", menuName = "Database/Residence Preset")]
public class ResidencePreset : SoCustomComparison
{
	// Token: 0x04003BF9 RID: 15353
	[Header("Settings")]
	[Tooltip("Are NPCs allowed to live here?")]
	public bool habitable = true;

	// Token: 0x04003BFA RID: 15354
	[Tooltip("Is this residence automatically put up for sale?")]
	public bool enableForSale = true;

	// Token: 0x04003BFB RID: 15355
	[Tooltip("Furnish this room even if uninhabited")]
	public bool furnitureIfUnihabited;

	// Token: 0x04003BFC RID: 15356
	[Tooltip("Is this a hotel room that the player can go to when they rent a room?")]
	public bool isHotelRoom;
}
