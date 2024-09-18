using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007AA RID: 1962
[CreateAssetMenu(fileName = "particleeffect_data", menuName = "Database/Particle Effect")]
public class ParticleEffect : SoCustomComparison
{
	// Token: 0x04003B95 RID: 15253
	[Header("Breakage")]
	[Tooltip("The relative velocity this has to be travelling at on collision to break")]
	public float damageBreakPoint = 0.1f;

	// Token: 0x04003B96 RID: 15254
	[Tooltip("Deletes the object completely")]
	public bool deleteObject = true;

	// Token: 0x04003B97 RID: 15255
	[Header("VFX")]
	public GameObject effectPrefab;

	// Token: 0x04003B98 RID: 15256
	[EnableIf("deleteObject")]
	[Header("Shatter")]
	public bool shatter = true;

	// Token: 0x04003B99 RID: 15257
	[Tooltip("The size of the shards created")]
	public Vector3 shardSize = new Vector3(0.025f, 0.025f, 0.025f);

	// Token: 0x04003B9A RID: 15258
	[Tooltip("Create a shard every this amount of pixels on the texture")]
	public int shardEveryXPixels = 64;

	// Token: 0x04003B9B RID: 15259
	public float shatterForceMultiplier = 2.4f;

	// Token: 0x04003B9C RID: 15260
	[Tooltip("Use a glass shard material")]
	public bool isGlass;

	// Token: 0x04003B9D RID: 15261
	[Header("Spatter")]
	public ParticleEffect.SpatterTrigger spatterTrigger;

	// Token: 0x04003B9E RID: 15262
	public SpatterPatternPreset spatter;

	// Token: 0x04003B9F RID: 15263
	public float countMultiplier = 1f;

	// Token: 0x04003BA0 RID: 15264
	public bool stickToActors = true;

	// Token: 0x04003BA1 RID: 15265
	public bool spatterIsVandalism;

	// Token: 0x04003BA2 RID: 15266
	[EnableIf("spatterIsVandalism")]
	public int vandalismFine = 20;

	// Token: 0x04003BA3 RID: 15267
	[Header("Object Creation")]
	public ParticleEffect.SpatterTrigger creationTrigger;

	// Token: 0x04003BA4 RID: 15268
	public List<GameObject> objectPool = new List<GameObject>();

	// Token: 0x04003BA5 RID: 15269
	public int instances = 1;

	// Token: 0x04003BA6 RID: 15270
	public bool useRandomRotation;

	// Token: 0x04003BA7 RID: 15271
	public Vector3 localEuler = Vector3.zero;

	// Token: 0x04003BA8 RID: 15272
	[Header("Audio")]
	public List<AudioEvent> impactEvents = new List<AudioEvent>();

	// Token: 0x04003BA9 RID: 15273
	public List<AudioEvent> breakEvents = new List<AudioEvent>();

	// Token: 0x020007AB RID: 1963
	public enum SpatterTrigger
	{
		// Token: 0x04003BAB RID: 15275
		off,
		// Token: 0x04003BAC RID: 15276
		onBreak,
		// Token: 0x04003BAD RID: 15277
		onAnyImpact,
		// Token: 0x04003BAE RID: 15278
		whileInAirOrAnyImpact
	}
}
