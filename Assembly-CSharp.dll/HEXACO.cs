using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020002E2 RID: 738
[Serializable]
public class HEXACO
{
	// Token: 0x040014EE RID: 5358
	public int outputMin = 1;

	// Token: 0x040014EF RID: 5359
	public int outputMax = 10;

	// Token: 0x040014F0 RID: 5360
	[Space(7f)]
	public bool enableFeminineMasculine;

	// Token: 0x040014F1 RID: 5361
	[Range(0f, 10f)]
	public int feminineMasculine;

	// Token: 0x040014F2 RID: 5362
	[InfoBox("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous", 0)]
	[Space(7f)]
	public bool enableHumility;

	// Token: 0x040014F3 RID: 5363
	[Range(0f, 10f)]
	public int humility;

	// Token: 0x040014F4 RID: 5364
	[InfoBox("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable", 0)]
	[Space(7f)]
	public bool enableEmotionality;

	// Token: 0x040014F5 RID: 5365
	[Range(0f, 10f)]
	public int emotionality;

	// Token: 0x040014F6 RID: 5366
	[Space(7f)]
	[InfoBox("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved", 0)]
	public bool enableExtraversion;

	// Token: 0x040014F7 RID: 5367
	[Range(0f, 10f)]
	public int extraversion;

	// Token: 0x040014F8 RID: 5368
	[InfoBox("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric", 0)]
	[Space(7f)]
	public bool enableAgreeableness;

	// Token: 0x040014F9 RID: 5369
	[Range(0f, 10f)]
	public int agreeableness;

	// Token: 0x040014FA RID: 5370
	[InfoBox("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded", 0)]
	[Space(7f)]
	public bool enableConscientiousness;

	// Token: 0x040014FB RID: 5371
	[Range(0f, 10f)]
	public int conscientiousness;

	// Token: 0x040014FC RID: 5372
	[Space(7f)]
	[InfoBox("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional", 0)]
	public bool enableCreativity;

	// Token: 0x040014FD RID: 5373
	[Range(0f, 10f)]
	public int creativity;
}
