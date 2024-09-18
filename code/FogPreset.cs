using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000724 RID: 1828
[CreateAssetMenu(fileName = "fog_data", menuName = "Database/Fog Settings")]
public class FogPreset : SoCustomComparison
{
	// Token: 0x040034F7 RID: 13559
	[Header("Lighting")]
	[Tooltip("Sun rises at this hour (90 degrees to terrain)")]
	public float sunRiseHour = 6.5f;

	// Token: 0x040034F8 RID: 13560
	[Tooltip("Sun sets at this hour (90 degrees to terrain)")]
	public float sunSetHour = 19.5f;

	// Token: 0x040034F9 RID: 13561
	[Tooltip("Sun intensity curve")]
	public AnimationCurve daytimeSunIntensityCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x040034FA RID: 13562
	[Tooltip("Multiply the above curve by this")]
	public float sunIntensityBooster = 2.2f;

	// Token: 0x040034FB RID: 13563
	[Tooltip("Morning sun Colour")]
	public Color morningSunColour = Color.red;

	// Token: 0x040034FC RID: 13564
	[Tooltip("Midday sun Colour")]
	public Color middaySunColour = Color.white;

	// Token: 0x040034FD RID: 13565
	[Tooltip("Evening sun Colour")]
	public Color eveningSunColour = Color.red;

	// Token: 0x040034FE RID: 13566
	[Tooltip("Sun shadow strength curve")]
	public AnimationCurve sunShadowStrengthCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x040034FF RID: 13567
	[Tooltip("Sun dimmer")]
	public AnimationCurve sunVolumetricDimmer;

	// Token: 0x04003500 RID: 13568
	[Tooltip("Sun shadows dimmer")]
	public AnimationCurve sunVolumetricShadowDimmer;

	// Token: 0x04003501 RID: 13569
	[Tooltip("Exterior Ambient curve")]
	public AnimationCurve exteriorAmbientIntensityCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x04003502 RID: 13570
	[Tooltip("Multiply the above curve by this")]
	public float ambientExteriorBooster = 2.2f;

	// Token: 0x04003503 RID: 13571
	[Tooltip("Exterior Ambient curve")]
	public AnimationCurve interiorAmbientIntensityCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x04003504 RID: 13572
	[Tooltip("Multiply the above curve by this")]
	public float ambientInteriorBooster = 2.2f;

	// Token: 0x04003505 RID: 13573
	[ReorderableList]
	[Tooltip("Skybox colour grades w/ fog colour settings")]
	[Header("Colouring")]
	public List<SessionData.SkyboxGradient> skyboxGradientGrading = new List<SessionData.SkyboxGradient>();

	// Token: 0x04003506 RID: 13574
	[Range(0f, 1f)]
	public float skyColourMultiplier = 1f;

	// Token: 0x04003507 RID: 13575
	[Range(0f, 1f)]
	public float fogColourMultiplier = 1f;

	// Token: 0x04003508 RID: 13576
	[Range(0f, 1f)]
	public float ambientLightMultiplier = 1f;

	// Token: 0x04003509 RID: 13577
	[Range(0f, 1f)]
	public float globalLightIntensityMultiplier = 1f;

	// Token: 0x0400350A RID: 13578
	[Header("Fog")]
	[Tooltip("Fog distance ranges")]
	public Vector2 fogDistanceRange = new Vector2(10f, 85f);

	// Token: 0x0400350B RID: 13579
	[Tooltip("Fog distance throughout the day")]
	public AnimationCurve fogDistanceCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x0400350C RID: 13580
	public Vector2 maxFogDistanceRange = new Vector2(10f, 85f);

	// Token: 0x0400350D RID: 13581
	[Tooltip("Max Fog distance throughout the day")]
	public AnimationCurve maxFogDistanceCurve = AnimationCurve.EaseInOut(1f, 1f, 0f, 0f);

	// Token: 0x0400350E RID: 13582
	[Space(7f)]
	public AnimationCurve fogAttenuationCurve;

	// Token: 0x0400350F RID: 13583
	public AnimationCurve volumetricFogDistanceCurve;

	// Token: 0x04003510 RID: 13584
	[Header("Skyline")]
	public AnimationCurve skylineEmissionCurve;

	// Token: 0x04003511 RID: 13585
	[ColorUsage(true, true)]
	public Color skylineEmissionColor;

	// Token: 0x04003512 RID: 13586
	[Header("Weather")]
	public AnimationCurve monthSnowChanceCurve;

	// Token: 0x04003513 RID: 13587
	public AnimationCurve weatherExtremityCurve;

	// Token: 0x04003514 RID: 13588
	public float thunderDelay = 8f;

	// Token: 0x04003515 RID: 13589
	[Header("Temperature")]
	public AnimationCurve monthTempCurve;

	// Token: 0x04003516 RID: 13590
	public AnimationCurve dayTempCurve;

	// Token: 0x04003517 RID: 13591
	public float NoRainModifier = 0.8f;

	// Token: 0x04003518 RID: 13592
	public float NoWindModifier = 0.7f;

	// Token: 0x04003519 RID: 13593
	public float NoSnowModifier = 0.3f;
}
