using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000701 RID: 1793
[CreateAssetMenu(fileName = "designstyle_data", menuName = "Database/Decor/Design Style Preset")]
public class DesignStylePreset : SoCustomComparison
{
	// Token: 0x04003365 RID: 13157
	[Tooltip("Include this when using citizen stats to pick a style")]
	[Header("Suited Personality")]
	public bool includeInPersonalityMatching = true;

	// Token: 0x04003366 RID: 13158
	[Tooltip("Compatible Units")]
	[ReorderableList]
	public List<LayoutConfiguration> compatibleAddressTypes = new List<LayoutConfiguration>();

	// Token: 0x04003367 RID: 13159
	[Tooltip("The citizen/company must have at least this much wealth to use this decor")]
	[Range(0f, 1f)]
	public float minimumWealth;

	// Token: 0x04003368 RID: 13160
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[Range(0f, 10f)]
	[Space(5f)]
	public int humility = 5;

	// Token: 0x04003369 RID: 13161
	[Range(0f, 10f)]
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	public int emotionality = 5;

	// Token: 0x0400336A RID: 13162
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	[Range(0f, 10f)]
	public int extraversion = 5;

	// Token: 0x0400336B RID: 13163
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	[Range(0f, 10f)]
	public int agreeableness = 5;

	// Token: 0x0400336C RID: 13164
	[Range(0f, 10f)]
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	public int conscientiousness = 5;

	// Token: 0x0400336D RID: 13165
	[Range(0f, 10f)]
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	public int creativity = 5;

	// Token: 0x0400336E RID: 13166
	[Range(0f, 10f)]
	[Header("Suited Colour Schemes")]
	public int modernity = 5;

	// Token: 0x0400336F RID: 13167
	[Header("Ceilings")]
	public bool allowCoving = true;

	// Token: 0x04003370 RID: 13168
	[Header("Misc.")]
	[Tooltip("Force this style if below ground")]
	public bool isBasement;
}
