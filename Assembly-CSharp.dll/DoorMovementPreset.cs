using System;
using UnityEngine;

// Token: 0x02000708 RID: 1800
[CreateAssetMenu(fileName = "doormovement_data", menuName = "Database/Door Movement Preset")]
public class DoorMovementPreset : SoCustomComparison
{
	// Token: 0x040033D4 RID: 13268
	[Header("Relative State Positions")]
	public Vector3 closedRelativePos = Vector3.zero;

	// Token: 0x040033D5 RID: 13269
	public Vector3 openRelativePos = Vector3.zero;

	// Token: 0x040033D6 RID: 13270
	[Space(5f)]
	public Vector3 closedRelativeEuler = Vector3.zero;

	// Token: 0x040033D7 RID: 13271
	public Vector3 openRelativeEuler = Vector3.zero;

	// Token: 0x040033D8 RID: 13272
	[Space(5f)]
	public Vector3 closedRelativeScale = Vector3.zero;

	// Token: 0x040033D9 RID: 13273
	public Vector3 openRelativeScale = Vector3.zero;

	// Token: 0x040033DA RID: 13274
	[Header("State Movement")]
	[Tooltip("How fast the door opens")]
	public float doorOpenSpeed = 1f;

	// Token: 0x040033DB RID: 13275
	[Tooltip("How fast the door closes")]
	public float doorCloseSpeed = 1f;

	// Token: 0x040033DC RID: 13276
	public AnimationCurve animationCurve = new AnimationCurve();

	// Token: 0x040033DD RID: 13277
	[Header("Physics")]
	public DoorMovementPreset.PhysicsBehaviour collisionBehaviour;

	// Token: 0x040033DE RID: 13278
	public bool behaviourAppliesWhenOpening = true;

	// Token: 0x040033DF RID: 13279
	public bool behaviourAppliesWhenClosing = true;

	// Token: 0x040033E0 RID: 13280
	[Header("Audio")]
	public AudioEvent openAction;

	// Token: 0x040033E1 RID: 13281
	public AudioEvent closeAction;

	// Token: 0x040033E2 RID: 13282
	public AudioEvent openFinished;

	// Token: 0x040033E3 RID: 13283
	public AudioEvent closeFinished;

	// Token: 0x040033E4 RID: 13284
	public AudioEvent objectImpact;

	// Token: 0x040033E5 RID: 13285
	[Tooltip("If this is true then occlusion won't be calculated.")]
	public bool ignoreOcclusion;

	// Token: 0x040033E6 RID: 13286
	[Tooltip("If true then switch state 1 will be active while animating")]
	public bool switchState1AnimationSync;

	// Token: 0x02000709 RID: 1801
	public enum PhysicsBehaviour
	{
		// Token: 0x040033E8 RID: 13288
		ignore,
		// Token: 0x040033E9 RID: 13289
		physicsEnabled,
		// Token: 0x040033EA RID: 13290
		stopDoorMovement
	}
}
