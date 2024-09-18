using System;
using UnityEngine;

// Token: 0x020007AC RID: 1964
[CreateAssetMenu(fileName = "physicsprofile_data", menuName = "Database/Physics Profile")]
public class PhysicsProfile : SoCustomComparison
{
	// Token: 0x04003BAF RID: 15279
	[Header("Physics")]
	[Tooltip("Mass of the object when physics is enabled")]
	public float mass = 1f;

	// Token: 0x04003BB0 RID: 15280
	[Tooltip("'How much air resistance affects the object when moving from forces. 0 means no air resistance, and infinity makes the object stop moving immediately.'")]
	public float drag;

	// Token: 0x04003BB1 RID: 15281
	[Tooltip("'How much air resistance affects the object when rotating from torque. 0 means no air resistance. Note that you cannot make the object stop rotating just by setting its Angular Drag to infinity.'")]
	public float angularDrag = 0.05f;

	// Token: 0x04003BB2 RID: 15282
	[Tooltip("If the object is held, it will default to this euler")]
	public Vector3 heldEuler;

	// Token: 0x04003BB3 RID: 15283
	[Tooltip("Add this on to the base tamper distance before it is considered a crime")]
	public float tamperDistanceModifier;

	// Token: 0x04003BB4 RID: 15284
	[Tooltip("Muliply the throw force in the gameplay settings by this.")]
	public float throwForceMultiplier = 1f;

	// Token: 0x04003BB5 RID: 15285
	[Tooltip("Multiply any damage caused by throw impact with this")]
	public float throwDamageMultiplier = 1f;

	// Token: 0x04003BB6 RID: 15286
	[Tooltip("Treat the audio event as caused by player, therefore making AI react to it")]
	public bool treatAsCausedByPlayer;

	// Token: 0x04003BB7 RID: 15287
	[Tooltip("The default collision detection mode")]
	public CollisionDetectionMode collisionMode;

	// Token: 0x04003BB8 RID: 15288
	[Tooltip("If true, this will be destroyed/removed if it's position needs to be reset. If false it will be reset to spawn position.")]
	public bool removeOnReset;

	// Token: 0x04003BB9 RID: 15289
	[Tooltip("Physics collisions use this sound")]
	[Header("Audio")]
	public AudioEvent physicsCollisionAudio;
}
