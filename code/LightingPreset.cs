using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000775 RID: 1909
[CreateAssetMenu(fileName = "lighting_data", menuName = "Database/Lighting Preset")]
public class LightingPreset : SoCustomComparison
{
	// Token: 0x0400395E RID: 14686
	[Header("Light")]
	[Tooltip("Cool Colours")]
	public List<CityControls.WindowColour> coolColours = new List<CityControls.WindowColour>();

	// Token: 0x0400395F RID: 14687
	[Tooltip("Warm Colours")]
	public List<CityControls.WindowColour> warmColours = new List<CityControls.WindowColour>();

	// Token: 0x04003960 RID: 14688
	[Tooltip("If no room is assigned, use this intensity")]
	public float defaultIntensity = 550f;

	// Token: 0x04003961 RID: 14689
	public float defaultRange = 9f;

	// Token: 0x04003962 RID: 14690
	[Tooltip("how much the intensity changes per room size, also range")]
	public float intensityRoomSizeMultiplier = 1f;

	// Token: 0x04003963 RID: 14691
	[Tooltip("Clamped intensity range")]
	public Vector2 intensityRange = new Vector2(100f, 1600f);

	// Token: 0x04003964 RID: 14692
	[Tooltip("Fade in or out when turned on or off")]
	public bool fadeOnOff = true;

	// Token: 0x04003965 RID: 14693
	[Tooltip("Fade in/out by this speed")]
	public float fadeSpeed = 10f;

	// Token: 0x04003966 RID: 14694
	[Tooltip("When setup, set the light")]
	public bool onByDefault = true;

	// Token: 0x04003967 RID: 14695
	[Tooltip("Distance at which the emission is culled completely")]
	public float fadeDistance = 40f;

	// Token: 0x04003968 RID: 14696
	[Tooltip("If true, uses the tv broadcast material")]
	[Header("Materials")]
	public bool useBroadcastMaterial;

	// Token: 0x04003969 RID: 14697
	[Tooltip("Use an alternate material when on (shared)")]
	[DisableIf("useBroadcaseMaterial")]
	public Material useOnMaterial;

	// Token: 0x0400396A RID: 14698
	[Tooltip("Dynamically alter emissive (create instanced material)")]
	public bool useInstancedEmissive;

	// Token: 0x0400396B RID: 14699
	[Tooltip("Emmission multiplier")]
	[EnableIf("useInstancedEmissive")]
	public float emissionMultiplier = 1f;

	// Token: 0x0400396C RID: 14700
	[Tooltip("Special option to make this hang down")]
	[Header("Atrium Lights")]
	public bool isAtriumLight;

	// Token: 0x0400396D RID: 14701
	[EnableIf("isAtriumLight")]
	[Tooltip("What is the minimum number of floors this atrium covers before it is allowed to feature this light?")]
	public int minimumFloors = 2;

	// Token: 0x0400396E RID: 14702
	[EnableIf("isAtriumLight")]
	public GameObject cablePrefab;

	// Token: 0x0400396F RID: 14703
	[EnableIf("isAtriumLight")]
	public GameObject bulbPrefab;

	// Token: 0x04003970 RID: 14704
	[EnableIf("isAtriumLight")]
	public GameObject endBulbPrefab;

	// Token: 0x04003971 RID: 14705
	[EnableIf("isAtriumLight")]
	[Tooltip("Spawn a bulb every x metres")]
	public float heightInterval = 8f;

	// Token: 0x04003972 RID: 14706
	[Header("Additional")]
	[Tooltip("Can this light also spawn a ceiling fan? Must be set up in lighting config")]
	public bool allowCeilingFans;

	// Token: 0x04003973 RID: 14707
	[Header("Volumetrics")]
	public bool enableVolumetrics = true;

	// Token: 0x04003974 RID: 14708
	[Tooltip("The atmosphere setting in the room preset is multiplied by this")]
	public float atmosphereMultiplier = 1f;

	// Token: 0x04003975 RID: 14709
	[Header("Shadows")]
	public bool enableShadows = true;

	// Token: 0x04003976 RID: 14710
	public LightingPreset.ShadowMode shadowMode;

	// Token: 0x04003977 RID: 14711
	public LightingPreset.ShadowResolution resolution = LightingPreset.ShadowResolution.medium;

	// Token: 0x04003978 RID: 14712
	[Tooltip("Distance at which shadows are culled completely.")]
	public float shadowFadeDistance = 40f;

	// Token: 0x04003979 RID: 14713
	[Header("Flickering")]
	[Range(0f, 1f)]
	public float chanceOfFlicker;

	// Token: 0x0400397A RID: 14714
	[Tooltip("When flickering, use this multiplier on the flicker colour to determin the actual colour (basically a darker version of flicker colour)")]
	public Vector2 flickerMultiplierRange = new Vector2(0f, 0.2f);

	// Token: 0x0400397B RID: 14715
	[Tooltip("When flickering, how fast it pulses")]
	public Vector2 flickerPulseRange = new Vector2(1f, 1.1f);

	// Token: 0x0400397C RID: 14716
	[Tooltip("Flickering lasts this long")]
	public Vector2 flickerIntervalRange = new Vector2(0.25f, 2f);

	// Token: 0x0400397D RID: 14717
	[Tooltip("Intervals between flickering are this long")]
	public Vector2 flickerNormalityIntervalRange = new Vector2(0.1f, 10f);

	// Token: 0x02000776 RID: 1910
	public enum ShadowMode
	{
		// Token: 0x0400397F RID: 14719
		everyFrame,
		// Token: 0x04003980 RID: 14720
		onEnable,
		// Token: 0x04003981 RID: 14721
		onDemand,
		// Token: 0x04003982 RID: 14722
		dynamicSystemStatic,
		// Token: 0x04003983 RID: 14723
		dynamicSystemSlowerUpdate
	}

	// Token: 0x02000777 RID: 1911
	public enum ShadowResolution
	{
		// Token: 0x04003985 RID: 14725
		low,
		// Token: 0x04003986 RID: 14726
		medium,
		// Token: 0x04003987 RID: 14727
		high,
		// Token: 0x04003988 RID: 14728
		ultra
	}
}
