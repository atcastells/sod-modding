using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007E9 RID: 2025
public class CitizenControls : MonoBehaviour
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060025ED RID: 9709 RVA: 0x001E8B05 File Offset: 0x001E6D05
	public static CitizenControls Instance
	{
		get
		{
			return CitizenControls._instance;
		}
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x001E8B0C File Offset: 0x001E6D0C
	private void Awake()
	{
		if (CitizenControls._instance != null && CitizenControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CitizenControls._instance = this;
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x001E8B3A File Offset: 0x001E6D3A
	[Button(null, 0)]
	public void ClearManualAnimation()
	{
		this.getUpManualAnimation.Clear();
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x001E8B48 File Offset: 0x001E6D48
	[Button(null, 0)]
	public void AddManualKeyframe()
	{
		if (this.debugSelectCitizen != null)
		{
			CitizenControls.ManualAnimation manualAnimation = new CitizenControls.ManualAnimation();
			foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.debugSelectCitizen.anchorConfig)
			{
				CitizenControls.LimbPos limbPos = new CitizenControls.LimbPos();
				limbPos.anchor = anchorConfig.anchor;
				limbPos.localPosition = anchorConfig.trans.localPosition;
				limbPos.localRotation = anchorConfig.trans.localRotation;
				manualAnimation.limbData.Add(limbPos);
			}
			this.getUpManualAnimation.Add(manualAnimation);
			Game.Log("Added a manual keyframe to get up animation", 2);
		}
	}

	// Token: 0x04003F7B RID: 16251
	[Header("Movement")]
	[Tooltip("The base speed of a citizen")]
	public float baseCitizenWalkSpeed = 1.85f;

	// Token: 0x04003F7C RID: 16252
	[Tooltip("Citizen run speed multiplier")]
	public float baseCitizenRunSpeed = 6f;

	// Token: 0x04003F7D RID: 16253
	[Tooltip("Acceleration Curve")]
	public AnimationCurve acceleration;

	// Token: 0x04003F7E RID: 16254
	[Tooltip("Decceleration Curve")]
	public AnimationCurve decceleration;

	// Token: 0x04003F7F RID: 16255
	[Tooltip("Ranges for random speed multiplier")]
	public Vector2 movementSpeedMultiplierRange = new Vector2(0.9f, 1.1f);

	// Token: 0x04003F80 RID: 16256
	[Tooltip("Speed citizen turns to face a new direction")]
	public float citizenFaceSpeed = 20f;

	// Token: 0x04003F81 RID: 16257
	[Tooltip("Speed of citizen rotational movement towards a new direction")]
	public float citizenRotationalMovementSpeed = 22f;

	// Token: 0x04003F82 RID: 16258
	[Tooltip("Range of speed of citizen turning head to face something")]
	public Vector2 citizenLookAtSpeed = new Vector2(1f, 3f);

	// Token: 0x04003F83 RID: 16259
	[Tooltip("How far a citizen moves per footstep.")]
	public float citizenFootstepDistance = 1f;

	// Token: 0x04003F84 RID: 16260
	[Tooltip("Amount of movement speed to lose while drunk")]
	public float drunkMovementPenalty = 0.3f;

	// Token: 0x04003F85 RID: 16261
	[Tooltip("Chance of falling over while drunk")]
	public float drunkFallChance = 0.5f;

	// Token: 0x04003F86 RID: 16262
	[Tooltip("Capsule collider changes thickness depending on movement")]
	public Vector2 capsuleMovementThickness = new Vector2(0.12f, 0.31f);

	// Token: 0x04003F87 RID: 16263
	[Space(7f)]
	[Tooltip("When not rendered, citizens move at this times their usual speed")]
	public float offscreenMovementSpeedMultiplier = 1.25f;

	// Token: 0x04003F88 RID: 16264
	[Tooltip("When not rendered, citizens move at this times their usual speed in stairwells")]
	public float offscreenStairwellMovementSpeedMultiplier = 1.85f;

	// Token: 0x04003F89 RID: 16265
	[Header("Visuals")]
	[Tooltip("The average scale of the average in-game height (175.4). This is slightly shorter than the player who is slightly above average height.")]
	public float baseScale = 0.85f;

	// Token: 0x04003F8A RID: 16266
	[Header("Dialog")]
	[Tooltip("How high above a citizens head is th speech bubble?")]
	public float speechBubbleHeight = 0.28f;

	// Token: 0x04003F8B RID: 16267
	public DialogPreset askAboutJob;

	// Token: 0x04003F8C RID: 16268
	[Tooltip("There maximum number of existing speech bubbles onscreen for new one line dialog to be triggered")]
	public int maxSpeechBubbles = 2;

	// Token: 0x04003F8D RID: 16269
	[Header("Bank")]
	public AnimationCurve societalClassSavingsCurve;

	// Token: 0x04003F8E RID: 16270
	[Tooltip("Each of the following essentially boost the soc class by 0.1")]
	[ReorderableList]
	public List<CharacterTrait> savingsBoostTrait = new List<CharacterTrait>();

	// Token: 0x04003F8F RID: 16271
	[ReorderableList]
	[Tooltip("Each of the following essentially reduce the soc class by 0.1")]
	public List<CharacterTrait> savingsDebuffTrait = new List<CharacterTrait>();

	// Token: 0x04003F90 RID: 16272
	[Header("Behaviour")]
	public DialogPreset telephoneGreeting;

	// Token: 0x04003F91 RID: 16273
	public DialogPreset identifyNumberDialog;

	// Token: 0x04003F92 RID: 16274
	public DialogPreset lastCallerDialog;

	// Token: 0x04003F93 RID: 16275
	public DialogPreset policeDialog;

	// Token: 0x04003F94 RID: 16276
	public DialogPreset coverUpOffer;

	// Token: 0x04003F95 RID: 16277
	public DialogPreset coverUpBodyLocation;

	// Token: 0x04003F96 RID: 16278
	public DialogPreset coverUpSuccess;

	// Token: 0x04003F97 RID: 16279
	public List<EvidenceWitness.DialogOption> coverUpConvoOptions = new List<EvidenceWitness.DialogOption>();

	// Token: 0x04003F98 RID: 16280
	[Tooltip("If no valid conversation is found, use this one")]
	public string fallbackTelephoneConversation;

	// Token: 0x04003F99 RID: 16281
	[Tooltip("Minimum investigation time: The minimum time the AI is to keep the investigate goal at maximum priority (in-game time)")]
	public float minimumInvestigateTime = 0.25f;

	// Token: 0x04003F9A RID: 16282
	[Tooltip("How quickly the AI gains one 'persuit lead' (this added per second)")]
	public float persuitChaseLogicAdditionPerSecond = 0.66f;

	// Token: 0x04003F9B RID: 16283
	[Tooltip("The maximum number of persuit logic leads the AI can acculumate")]
	public int maxChaseLogic = 4;

	// Token: 0x04003F9C RID: 16284
	[Tooltip("Persuit response addition from shortest distance to longest distance")]
	public Vector2 persuitTimerThreshold = new Vector2(16f, 2f);

	// Token: 0x04003F9D RID: 16285
	[Tooltip("When target is not in range, how fast to forget them")]
	public float persuitForgetThreshold = 1f;

	// Token: 0x04003F9E RID: 16286
	[Tooltip("When heard an illegal sound, how fast to forget...")]
	public float hearingForgetThreshold = 0.2f;

	// Token: 0x04003F9F RID: 16287
	[Tooltip("The multiplier for the minimum investiation time if the player is persued")]
	public float persuitMinInvestigationTimeMP = 1.5f;

	// Token: 0x04003FA0 RID: 16288
	[Tooltip("The multiplier for the minimum investiation time if the citizen investiates a sighting")]
	public float sightingMinInvestigationTimeMP = 1.25f;

	// Token: 0x04003FA1 RID: 16289
	[Tooltip("The multiplier for the minimum investiation time if the citizen investiates a sound")]
	public float soundMinInvestigationTimeMP = 1f;

	// Token: 0x04003FA2 RID: 16290
	[Tooltip("How much time passes after a sighting of highest rank before a citizen stops looking @ it")]
	[Space(7f)]
	public float lookAtGracePeriod = 0.133f;

	// Token: 0x04003FA3 RID: 16291
	[Tooltip("If somebody in the same room is punched, trigger citizens in the same address within this range to respond...")]
	[Space(7f)]
	public float punchedResponseRange = 11f;

	// Token: 0x04003FA4 RID: 16292
	[Tooltip("How many different citizens does can this person remember?")]
	[Header("Sightings")]
	public int defaultMemoryLimit = 100;

	// Token: 0x04003FA5 RID: 16293
	[Header("Combat")]
	[Tooltip("Citizens recover this amount of health (normalized) over time (game time 1 hour)")]
	public float citizenBaseRecoveryRate = 0.1f;

	// Token: 0x04003FA6 RID: 16294
	[Tooltip("The starting value for citizen combat skill (how fast attacks are)")]
	public Vector2 citizenBaseCombatSkillRange = new Vector2(0.5f, 1.5f);

	// Token: 0x04003FA7 RID: 16295
	[Tooltip("Multiplier for combat heft: See descriptors for how this works")]
	public float citizenCombatHeftMultiplier = 0.25f;

	// Token: 0x04003FA8 RID: 16296
	[Tooltip("Minimum range for throwing an object")]
	public float throwMinRange = 2f;

	// Token: 0x04003FA9 RID: 16297
	[Tooltip("Minimum range for throwing an object")]
	public float throwMaxRange = 8f;

	// Token: 0x04003FAA RID: 16298
	[Space(7f)]
	[Tooltip("Shock damage on impact is multiplied by this")]
	public float nerveDamageShockMultiplier = 1f;

	// Token: 0x04003FAB RID: 16299
	[Tooltip("Nerve damage by a weapon draw is multiplied by this")]
	public float nerveWeaponDrawMultiplier = 1f;

	// Token: 0x04003FAC RID: 16300
	[Tooltip("Nerve impacted when an alarm goes off")]
	public float nerveAlarm = -0.15f;

	// Token: 0x04003FAD RID: 16301
	[Tooltip("Nerve impacted after alarm switch")]
	public float nerveAlarmSwitched = 0.1f;

	// Token: 0x04003FAE RID: 16302
	[Tooltip("Nerve recovery rate as a fraction of health recovery")]
	public float nerveRecoveryRateMultiplier = 0.3f;

	// Token: 0x04003FAF RID: 16303
	[Space(7f)]
	public float doorBargeKOForceMultiplier = 3f;

	// Token: 0x04003FB0 RID: 16304
	[Tooltip("How the force multiplier scales with extra damage received")]
	public float damageRecieveForceMultiplier = 1f;

	// Token: 0x04003FB1 RID: 16305
	[Header("Get up Limb Snapshot")]
	[Tooltip("How long it takes to transition from ragdoll landing position to the start of the get-up animation (seconds)")]
	public float ragdollTransitionTime = 0.68f;

	// Token: 0x04003FB2 RID: 16306
	[Tooltip("The length of the get up animation or longer")]
	public float getUpTimer = 2f;

	// Token: 0x04003FB3 RID: 16307
	public List<CitizenControls.ManualAnimation> getUpManualAnimation;

	// Token: 0x04003FB4 RID: 16308
	[Header("Skills")]
	[Tooltip("How quickly stealth skill is applied when standing still")]
	public float stealthSkillApplicationRate = 1f;

	// Token: 0x04003FB5 RID: 16309
	[Tooltip("How quickly stealth skill is cancelled when moving")]
	public float stealthSkillCancelRate = 5f;

	// Token: 0x04003FB6 RID: 16310
	[Header("LookAt Head Clamping")]
	public float leftExtent = -70f;

	// Token: 0x04003FB7 RID: 16311
	public float rightExtent = 70f;

	// Token: 0x04003FB8 RID: 16312
	public float upExtent = -18f;

	// Token: 0x04003FB9 RID: 16313
	public float downExtent = 18f;

	// Token: 0x04003FBA RID: 16314
	[Tooltip("Animation offset of the citizens lower torso with a scale of 1...")]
	[Header("Animation")]
	public float sittingYOffset;

	// Token: 0x04003FBB RID: 16315
	[Tooltip("Animation offset of the citizens arms with a scale of 1...")]
	public float armsStandingYOffset;

	// Token: 0x04003FBC RID: 16316
	[Header("Fingerprints")]
	public Texture2D unknownPrint;

	// Token: 0x04003FBD RID: 16317
	[ReorderableList]
	public List<Texture2D> prints;

	// Token: 0x04003FBE RID: 16318
	[Header("Traits")]
	public CharacterTrait destitute;

	// Token: 0x04003FBF RID: 16319
	public CharacterTrait litterBug;

	// Token: 0x04003FC0 RID: 16320
	public CharacterTrait likesTheRain;

	// Token: 0x04003FC1 RID: 16321
	public CharacterTrait shoesNormal;

	// Token: 0x04003FC2 RID: 16322
	public CharacterTrait shoesBoots;

	// Token: 0x04003FC3 RID: 16323
	public CharacterTrait shoesHeels;

	// Token: 0x04003FC4 RID: 16324
	public CharacterTrait coffeeLiker;

	// Token: 0x04003FC5 RID: 16325
	public CharacterTrait teaLiker;

	// Token: 0x04003FC6 RID: 16326
	[Header("Physical Traits")]
	public CharacterTrait bald;

	// Token: 0x04003FC7 RID: 16327
	public CharacterTrait shortHair;

	// Token: 0x04003FC8 RID: 16328
	public CharacterTrait longHair;

	// Token: 0x04003FC9 RID: 16329
	public Vector2 shoeSizeRange = new Vector2(4f, 16f);

	// Token: 0x04003FCA RID: 16330
	[Header("AI")]
	[Tooltip("Subdivisions for AI navigation when inside nodes")]
	public List<Vector3> nodeLocalSubdivisions = new List<Vector3>();

	// Token: 0x04003FCB RID: 16331
	[Header("Starting Inventory")]
	public List<CitizenControls.StartingInventory> citizenStartingInventory = new List<CitizenControls.StartingInventory>();

	// Token: 0x04003FCC RID: 16332
	[Header("Misc")]
	public InteractablePreset citizenInteractable;

	// Token: 0x04003FCD RID: 16333
	public InteractablePreset handInteractable;

	// Token: 0x04003FCE RID: 16334
	public AIActionPreset sleep;

	// Token: 0x04003FCF RID: 16335
	public MatchPreset matchWithPhoto;

	// Token: 0x04003FD0 RID: 16336
	public MatchPreset weakVisualSighting;

	// Token: 0x04003FD1 RID: 16337
	public MatchPreset mediumVisualSighting;

	// Token: 0x04003FD2 RID: 16338
	public MatchPreset strongVisualSighting;

	// Token: 0x04003FD3 RID: 16339
	public CharacterTrait randomPassword;

	// Token: 0x04003FD4 RID: 16340
	public InteractablePreset deadBodySearchInteractable;

	// Token: 0x04003FD5 RID: 16341
	public InteractablePreset entryWound;

	// Token: 0x04003FD6 RID: 16342
	public InteractablePreset exitWound;

	// Token: 0x04003FD7 RID: 16343
	public InteractablePreset toothbrush;

	// Token: 0x04003FD8 RID: 16344
	public InteractablePreset addressBook;

	// Token: 0x04003FD9 RID: 16345
	public GameObject umbrella;

	// Token: 0x04003FDA RID: 16346
	public SpatterPatternPreset vomitSpatter;

	// Token: 0x04003FDB RID: 16347
	[Header("Debug")]
	public CitizenOutfitController debugSelectCitizen;

	// Token: 0x04003FDC RID: 16348
	private static CitizenControls _instance;

	// Token: 0x020007EA RID: 2026
	[Serializable]
	public class LimbPos
	{
		// Token: 0x04003FDD RID: 16349
		public CitizenOutfitController.CharacterAnchor anchor;

		// Token: 0x04003FDE RID: 16350
		public Vector3 localPosition;

		// Token: 0x04003FDF RID: 16351
		public Quaternion localRotation;
	}

	// Token: 0x020007EB RID: 2027
	[Serializable]
	public class ManualAnimation
	{
		// Token: 0x04003FE0 RID: 16352
		public float timeline;

		// Token: 0x04003FE1 RID: 16353
		public List<CitizenControls.LimbPos> limbData = new List<CitizenControls.LimbPos>();
	}

	// Token: 0x020007EC RID: 2028
	[Serializable]
	public class StartingInventory
	{
		// Token: 0x04003FE2 RID: 16354
		public string name;

		// Token: 0x04003FE3 RID: 16355
		public List<InteractablePreset> presets = new List<InteractablePreset>();

		// Token: 0x04003FE4 RID: 16356
		public float baseChance;

		// Token: 0x04003FE5 RID: 16357
		public List<MurderPreset.MurdererModifierRule> modifiers = new List<MurderPreset.MurdererModifierRule>();
	}
}
