using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000720 RID: 1824
[CreateAssetMenu(fileName = "firstperson_data", menuName = "Database/First Person Item")]
public class FirstPersonItem : SoCustomComparison
{
	// Token: 0x040034A2 RID: 13474
	[Tooltip("Priority of this within the inventory hierarchy.")]
	[Header("Setup")]
	public int slotPriority;

	// Token: 0x040034A3 RID: 13475
	[Tooltip("Should the arm models be displayed at all?")]
	public bool modelActive = true;

	// Token: 0x040034A4 RID: 13476
	public AnimationClip idleClip;

	// Token: 0x040034A5 RID: 13477
	public Sprite selectionIcon;

	// Token: 0x040034A6 RID: 13478
	public string summaryMsgID;

	// Token: 0x040034A7 RID: 13479
	public string triggerTutorial;

	// Token: 0x040034A8 RID: 13480
	public bool disableBracketDisplayName;

	// Token: 0x040034A9 RID: 13481
	[Tooltip("How fast to play the draw animation")]
	[Header("Animation")]
	public float drawSpeed = 2f;

	// Token: 0x040034AA RID: 13482
	[Tooltip("How fast to play the holster animation")]
	public float holsterSpeed = 2.5f;

	// Token: 0x040034AB RID: 13483
	[Header("Objects")]
	public GameObject leftHandObject;

	// Token: 0x040034AC RID: 13484
	public GameObject rightHandObject;

	// Token: 0x040034AD RID: 13485
	public Vector3 spawnScale = Vector3.one;

	// Token: 0x040034AE RID: 13486
	public bool useFoodSlotItem;

	// Token: 0x040034AF RID: 13487
	public bool useAlternateTrashObjects;

	// Token: 0x040034B0 RID: 13488
	[EnableIf("useAlternateTrashObjects")]
	public GameObject leftHandObjectTrash;

	// Token: 0x040034B1 RID: 13489
	[EnableIf("useAlternateTrashObjects")]
	public GameObject rightHandObjectTrash;

	// Token: 0x040034B2 RID: 13490
	[Header("Interaction")]
	[Tooltip("Setup of actions able to be performed")]
	[ReorderableList]
	public List<FirstPersonItem.FPSInteractionAction> actions = new List<FirstPersonItem.FPSInteractionAction>();

	// Token: 0x040034B3 RID: 13491
	[Tooltip("How this impacts nerve levels of a citizen if drawn")]
	public float drawnNerveModifier;

	// Token: 0x040034B4 RID: 13492
	[Tooltip("Chance of bark trigger")]
	public float barkTriggerChance;

	// Token: 0x040034B5 RID: 13493
	public SpeechController.Bark bark = SpeechController.Bark.threatenByItem;

	// Token: 0x040034B6 RID: 13494
	[Header("Compatibility")]
	public bool compatibleWithLockedIn = true;

	// Token: 0x040034B7 RID: 13495
	public bool compatibleWithHidden = true;

	// Token: 0x040034B8 RID: 13496
	[Header("Audio")]
	public float equipSoundDelay;

	// Token: 0x040034B9 RID: 13497
	public AudioEvent equipEvent;

	// Token: 0x040034BA RID: 13498
	public float holsterSoundDelay;

	// Token: 0x040034BB RID: 13499
	public AudioEvent holsterEvent;

	// Token: 0x040034BC RID: 13500
	public AudioEvent activeLoop;

	// Token: 0x02000721 RID: 1825
	public enum SpecialAction
	{
		// Token: 0x040034BE RID: 13502
		none,
		// Token: 0x040034BF RID: 13503
		block,
		// Token: 0x040034C0 RID: 13504
		handcuff,
		// Token: 0x040034C1 RID: 13505
		takedown,
		// Token: 0x040034C2 RID: 13506
		punch,
		// Token: 0x040034C3 RID: 13507
		consumeTrue,
		// Token: 0x040034C4 RID: 13508
		consumeFalse,
		// Token: 0x040034C5 RID: 13509
		putDown,
		// Token: 0x040034C6 RID: 13510
		attack,
		// Token: 0x040034C7 RID: 13511
		raiseTrue,
		// Token: 0x040034C8 RID: 13512
		raiseFalse,
		// Token: 0x040034C9 RID: 13513
		takePicture,
		// Token: 0x040034CA RID: 13514
		placeCodebreaker,
		// Token: 0x040034CB RID: 13515
		placeDoorWedge,
		// Token: 0x040034CC RID: 13516
		takeOne,
		// Token: 0x040034CD RID: 13517
		placeFurniture,
		// Token: 0x040034CE RID: 13518
		cancelFurniture,
		// Token: 0x040034CF RID: 13519
		give,
		// Token: 0x040034D0 RID: 13520
		placeTracker,
		// Token: 0x040034D1 RID: 13521
		placeFlashbomb,
		// Token: 0x040034D2 RID: 13522
		placeIncapacitator,
		// Token: 0x040034D3 RID: 13523
		takeBriefcaseCash,
		// Token: 0x040034D4 RID: 13524
		openBriefcaseBomb,
		// Token: 0x040034D5 RID: 13525
		rotateFurnLeft,
		// Token: 0x040034D6 RID: 13526
		rotateFurnRight,
		// Token: 0x040034D7 RID: 13527
		putBriefcaseCash
	}

	// Token: 0x02000722 RID: 1826
	[Serializable]
	public class FPSInteractionAction : InteractablePreset.InteractionAction
	{
		// Token: 0x040034D8 RID: 13528
		[Space(7f)]
		public FirstPersonItem.AttackAvailability availability;

		// Token: 0x040034D9 RID: 13529
		public float attackMainSpeed = 2f;

		// Token: 0x040034DA RID: 13530
		public PlayerTransitionPreset attackTrasition;

		// Token: 0x040034DB RID: 13531
		[Tooltip("Minimum time between possible attacks: You might want to match this with the attack animation length")]
		public float attackDelay = 1f;

		// Token: 0x040034DC RID: 13532
		public FirstPersonItem.SpecialAction mainSpecialAction;

		// Token: 0x040034DD RID: 13533
		public bool mainUseSpecialColour;

		// Token: 0x040034DE RID: 13534
		public Color mainSpecialColour = Color.black;

		// Token: 0x040034DF RID: 13535
		public AudioEvent attackEvent;

		// Token: 0x040034E0 RID: 13536
		public bool useCameraJolt;

		// Token: 0x040034E1 RID: 13537
		public Vector2 joltXRange;

		// Token: 0x040034E2 RID: 13538
		public Vector2 joltYRange;

		// Token: 0x040034E3 RID: 13539
		public Vector2 joltZRange;

		// Token: 0x040034E4 RID: 13540
		public float joltAmplitude = 1f;

		// Token: 0x040034E5 RID: 13541
		public float joltSpeed = 1f;
	}

	// Token: 0x02000723 RID: 1827
	public enum AttackAvailability
	{
		// Token: 0x040034E7 RID: 13543
		never,
		// Token: 0x040034E8 RID: 13544
		always,
		// Token: 0x040034E9 RID: 13545
		handcuffs,
		// Token: 0x040034EA RID: 13546
		behindCitizen,
		// Token: 0x040034EB RID: 13547
		onConsuming,
		// Token: 0x040034EC RID: 13548
		onNotConsuming,
		// Token: 0x040034ED RID: 13549
		onNotConsumingButLeftovers,
		// Token: 0x040034EE RID: 13550
		nearPutDown,
		// Token: 0x040034EF RID: 13551
		onRaised,
		// Token: 0x040034F0 RID: 13552
		onNotRaised,
		// Token: 0x040034F1 RID: 13553
		codebreaker,
		// Token: 0x040034F2 RID: 13554
		doorWedge,
		// Token: 0x040034F3 RID: 13555
		giveItem,
		// Token: 0x040034F4 RID: 13556
		tracker,
		// Token: 0x040034F5 RID: 13557
		onRaisedButLeftovers,
		// Token: 0x040034F6 RID: 13558
		onRaisedNotFull
	}
}
