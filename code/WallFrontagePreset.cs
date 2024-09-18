using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007E4 RID: 2020
[CreateAssetMenu(fileName = "wallfrontage_data", menuName = "Database/Decor/Wall Frontage Preset")]
public class WallFrontagePreset : SoCustomComparison
{
	// Token: 0x04003E97 RID: 16023
	[Header("Visuals")]
	public GameObject gameObject;

	// Token: 0x04003E98 RID: 16024
	[Tooltip("If true this will seach for identical furniture in a room to batch with")]
	public bool allowStaticBatching = true;

	// Token: 0x04003E99 RID: 16025
	[Tooltip("Can this feature a rainy window texture?")]
	public bool isRainyWindow;

	// Token: 0x04003E9A RID: 16026
	[Tooltip("The non-rainy window material")]
	[EnableIf("isRainyWindow")]
	public Material regularGlass;

	// Token: 0x04003E9B RID: 16027
	[EnableIf("isRainyWindow")]
	[Tooltip("The rainy window material")]
	public Material rainyGlass;

	// Token: 0x04003E9C RID: 16028
	[Header("Decor Settings")]
	[Tooltip("If true use across all design styles")]
	public bool universalDesignStyle;

	// Token: 0x04003E9D RID: 16029
	public List<DesignStylePreset> designStyles = new List<DesignStylePreset>();

	// Token: 0x04003E9E RID: 16030
	[Space(7f)]
	public bool inheritColouringFromDecor;

	// Token: 0x04003E9F RID: 16031
	[Tooltip("If true the same material colours will be shared over all instances of this furniture for the room")]
	public FurniturePreset.ShareColours shareColours;

	// Token: 0x04003EA0 RID: 16032
	public List<MaterialGroupPreset.MaterialVariation> variations = new List<MaterialGroupPreset.MaterialVariation>();

	// Token: 0x04003EA1 RID: 16033
	[Header("Interactables")]
	[Tooltip("What interatables will be instanced on this? These won't be spawned but created and searched for within the furniture prefab")]
	public List<FurniturePreset.IntegratedInteractable> integratedInteractables = new List<FurniturePreset.IntegratedInteractable>();

	// Token: 0x04003EA2 RID: 16034
	[Header("Classes")]
	public List<WallFrontageClass> classes = new List<WallFrontageClass>();
}
