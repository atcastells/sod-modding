using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200077F RID: 1919
[CreateAssetMenu(fileName = "menu_data", menuName = "Database/Menu Preset")]
public class MenuPreset : SoCustomComparison
{
	// Token: 0x040039CA RID: 14794
	[Header("Items Sold")]
	[ReorderableList]
	public List<InteractablePreset> itemsSold = new List<InteractablePreset>();

	// Token: 0x040039CB RID: 14795
	[Tooltip("If true a receipt will always be created with these items...")]
	public bool createReceipt = true;

	// Token: 0x040039CC RID: 14796
	public AudioEvent purchaseAudio;

	// Token: 0x040039CD RID: 14797
	[Space(7f)]
	public int syncDiskSlots;

	// Token: 0x040039CE RID: 14798
	public List<SyncDiskPreset.Manufacturer> fromManufacturers = new List<SyncDiskPreset.Manufacturer>();

	// Token: 0x040039CF RID: 14799
	public List<SyncDiskPreset> syncDisks = new List<SyncDiskPreset>();
}
