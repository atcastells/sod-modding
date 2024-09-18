using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200070C RID: 1804
[CreateAssetMenu(fileName = "door_data", menuName = "Database/Door Preset")]
public class DoorPreset : SoCustomComparison
{
	// Token: 0x0400340D RID: 13325
	[Header("Visuals")]
	public GameObject doorModel;

	// Token: 0x0400340E RID: 13326
	public InteractablePreset objectPreset;

	// Token: 0x0400340F RID: 13327
	public GameObject handleModel;

	// Token: 0x04003410 RID: 13328
	public InteractablePreset handlePreset;

	// Token: 0x04003411 RID: 13329
	public Vector3 handleOffset = new Vector3(-1.25f, 1.05f, -0.05f);

	// Token: 0x04003412 RID: 13330
	public bool isTransparent;

	// Token: 0x04003413 RID: 13331
	[Header("Signs")]
	public Vector3 doorSignOffset = new Vector3(-0.68f, 1.8f, -0.1f);

	// Token: 0x04003414 RID: 13332
	[ReorderableList]
	public List<DoorPreset.DoorSign> doorSigns = new List<DoorPreset.DoorSign>();

	// Token: 0x04003415 RID: 13333
	[Header("Decor Settings")]
	public bool inheritColouringFromDecor;

	// Token: 0x04003416 RID: 13334
	[Tooltip("If true the same material colours will be shared over all instances of this furniture for the room")]
	public FurniturePreset.ShareColours shareColours;

	// Token: 0x04003417 RID: 13335
	public List<MaterialGroupPreset.MaterialVariation> variations = new List<MaterialGroupPreset.MaterialVariation>();

	// Token: 0x04003418 RID: 13336
	[Header("Behaviour")]
	[Tooltip("How fast the door opens and closes")]
	public float doorOpenSpeed = 1.47f;

	// Token: 0x04003419 RID: 13337
	[Tooltip("The maximum amount this door can open")]
	public float openAngle = 89.9f;

	// Token: 0x0400341A RID: 13338
	[Tooltip("Can the player peek underneath this door?")]
	public bool canPeakUnderneath;

	// Token: 0x0400341B RID: 13339
	[Tooltip("If open, close the door depending on this behaviour")]
	public DoorPreset.ClosingBehaviour closeBehaviour;

	// Token: 0x0400341C RID: 13340
	[Header("Lock")]
	public DoorPreset.LockType lockType = DoorPreset.LockType.key;

	// Token: 0x0400341D RID: 13341
	[Tooltip("If the above is set to something other than none or key, then setup this lock interactable...")]
	public InteractablePreset lockInteractable;

	// Token: 0x0400341E RID: 13342
	public Vector3 lockOffsetFront = new Vector3(-1.25f, 1.5f, 0f);

	// Token: 0x0400341F RID: 13343
	public Vector3 lockOffsetRear = new Vector3(-1.25f, 1.5f, -0.1f);

	// Token: 0x04003420 RID: 13344
	[Tooltip("The lock is armed when the door movement is closed")]
	public bool armLockOnClose = true;

	// Token: 0x04003421 RID: 13345
	[Tooltip("The door strength range")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 doorStrengthRange = new Vector2(0.1f, 0.2f);

	// Token: 0x04003422 RID: 13346
	[Tooltip("The lock strength range")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 lockStrengthRange = new Vector2(0.1f, 0.2f);

	// Token: 0x04003423 RID: 13347
	[Header("Audio")]
	public AudioEvent audioOpen;

	// Token: 0x04003424 RID: 13348
	public AudioEvent audioClose;

	// Token: 0x04003425 RID: 13349
	public AudioEvent audioCloseAction;

	// Token: 0x04003426 RID: 13350
	public AudioEvent audioLock;

	// Token: 0x04003427 RID: 13351
	public AudioEvent audioUnlock;

	// Token: 0x04003428 RID: 13352
	public AudioEvent audioLockedEntryAttempt;

	// Token: 0x04003429 RID: 13353
	public AudioEvent audioKnockLight;

	// Token: 0x0400342A RID: 13354
	public AudioEvent audioKnockMed;

	// Token: 0x0400342B RID: 13355
	public AudioEvent audioKnockHeavy;

	// Token: 0x0400342C RID: 13356
	public AudioEvent doorBargeContact;

	// Token: 0x0400342D RID: 13357
	public AudioEvent doorBargeBreak;

	// Token: 0x0200070D RID: 1805
	public enum LockType
	{
		// Token: 0x0400342F RID: 13359
		none,
		// Token: 0x04003430 RID: 13360
		key,
		// Token: 0x04003431 RID: 13361
		keypad
	}

	// Token: 0x0200070E RID: 1806
	[Serializable]
	public class DoorSign
	{
		// Token: 0x04003432 RID: 13362
		public List<GameObject> signagePool = new List<GameObject>();

		// Token: 0x04003433 RID: 13363
		public List<RoomConfiguration> ifEntranceToRoom = new List<RoomConfiguration>();

		// Token: 0x04003434 RID: 13364
		public bool placeIfFromPublicArea = true;

		// Token: 0x04003435 RID: 13365
		public bool placeIfFromOutside;

		// Token: 0x04003436 RID: 13366
		public bool placeIfFromInside;

		// Token: 0x04003437 RID: 13367
		public bool onlyPlaceIfInhabited;
	}

	// Token: 0x0200070F RID: 1807
	public enum ClosingBehaviour
	{
		// Token: 0x04003439 RID: 13369
		nothing,
		// Token: 0x0400343A RID: 13370
		closeOnCull,
		// Token: 0x0400343B RID: 13371
		closeOnDespawn
	}
}
