using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200070A RID: 1802
[CreateAssetMenu(fileName = "doorpair_data", menuName = "Database/Decor/Door Pair Preset")]
public class DoorPairPreset : ScriptableObjectIDSystem
{
	// Token: 0x06002538 RID: 9528 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void UpdateIDs()
	{
	}

	// Token: 0x040033EB RID: 13291
	[Header("Model Options")]
	[Tooltip("The wall model of the parent wall")]
	public List<GameObject> parentWallsLong = new List<GameObject>();

	// Token: 0x040033EC RID: 13292
	[Tooltip("The wall model of the child wall")]
	public List<GameObject> childWallsLong = new List<GameObject>();

	// Token: 0x040033ED RID: 13293
	[Space(7f)]
	[Tooltip("Short walls: There should be 3 here - left, middle and right")]
	public List<GameObject> parentWallsShort = new List<GameObject>();

	// Token: 0x040033EE RID: 13294
	[Tooltip("Short walls: There should be 3 here - left, middle and right")]
	public List<GameObject> childWallsShort = new List<GameObject>();

	// Token: 0x040033EF RID: 13295
	[Tooltip("The corner model. Always on the outside walls.")]
	[Space(7f)]
	public List<GameObject> corners = new List<GameObject>();

	// Token: 0x040033F0 RID: 13296
	[Tooltip("The corner model for outside facing exterior walls (overrides above)")]
	public List<GameObject> quoins = new List<GameObject>();

	// Token: 0x040033F1 RID: 13297
	[Space(7f)]
	[Tooltip("If true, the game will use optimization to replaces walls of 3x rows with a larger section")]
	public bool optimizeSections;

	// Token: 0x040033F2 RID: 13298
	[Tooltip("If true, user is able to place this in the editor")]
	public bool appearInEditor;

	// Token: 0x040033F3 RID: 13299
	[Tooltip("If true, this section can support lightswitches or other wall props")]
	public bool supportsWallProps = true;

	// Token: 0x040033F4 RID: 13300
	[Tooltip("If true, the game will continue to draw building corners around this")]
	public bool isFence;

	// Token: 0x040033F5 RID: 13301
	public bool divider;

	// Token: 0x040033F6 RID: 13302
	public bool dividerLeft;

	// Token: 0x040033F7 RID: 13303
	public bool dividerRight;

	// Token: 0x040033F8 RID: 13304
	[Tooltip("Door object")]
	[Header("Door Options")]
	public bool canFeatureDoor;

	// Token: 0x040033F9 RID: 13305
	[Tooltip("Door offset position")]
	public Vector3 doorOffset = new Vector3(0.7f, 0f, 0f);

	// Token: 0x040033FA RID: 13306
	[Header("Procedural Overrides")]
	[Tooltip("The class of this wall section. When a procedural address is generated, it may override this with another more appropriate model with the same class.")]
	public DoorPairPreset.WallSectionClass sectionClass;

	// Token: 0x040033FB RID: 13307
	[Tooltip("If true then this will force this section to ignore raycasts when generating room culling.")]
	public bool ignoreCullingRaycasts;

	// Token: 0x040033FC RID: 13308
	[Tooltip("Override with this if the floor height is above 0")]
	public DoorPairPreset raisedFloorOverride;

	// Token: 0x040033FD RID: 13309
	[Header("Material Override")]
	public MaterialGroupPreset materialOverride;

	// Token: 0x040033FE RID: 13310
	[Header("Map Overrides")]
	[Tooltip("Override map graphics with this")]
	public List<Texture2D> mapOverride = new List<Texture2D>();

	// Token: 0x040033FF RID: 13311
	[Header("Duct Overrides")]
	public bool overrideWallNormal;

	// Token: 0x04003400 RID: 13312
	[EnableIf("overrideWallNormal")]
	public DoorPairPreset wallNormalOverrride;

	// Token: 0x04003401 RID: 13313
	[Space(5f)]
	public bool overrideDuctLower;

	// Token: 0x04003402 RID: 13314
	[EnableIf("overrideDuctLower")]
	public DoorPairPreset ductLowerOverrride;

	// Token: 0x04003403 RID: 13315
	[Space(5f)]
	public bool overrideDuctUpper;

	// Token: 0x04003404 RID: 13316
	[EnableIf("overrideDuctUpper")]
	public DoorPairPreset ductUpperOverrride;

	// Token: 0x0200070B RID: 1803
	public enum WallSectionClass
	{
		// Token: 0x04003406 RID: 13318
		wall,
		// Token: 0x04003407 RID: 13319
		window,
		// Token: 0x04003408 RID: 13320
		windowLarge,
		// Token: 0x04003409 RID: 13321
		entrance,
		// Token: 0x0400340A RID: 13322
		ventUpper,
		// Token: 0x0400340B RID: 13323
		ventLower,
		// Token: 0x0400340C RID: 13324
		ventTop
	}
}
