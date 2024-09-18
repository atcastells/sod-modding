using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006AB RID: 1707
[CreateAssetMenu(fileName = "aigoal_data", menuName = "Database/AI/AI Goal")]
public class AIGoalPreset : SoCustomComparison
{
	// Token: 0x0400302F RID: 12335
	[Tooltip("If true will be added to citizen upon creation")]
	[Header("Application")]
	public bool startingGoal;

	// Token: 0x04003030 RID: 12336
	[Tooltip("Is this goal designed for...")]
	[EnableIf("startingGoal")]
	public AIGoalPreset.StartingGoal appliesTo;

	// Token: 0x04003031 RID: 12337
	[EnableIf("startingGoal")]
	public List<OccupationPreset> appliedToTheseJobs;

	// Token: 0x04003032 RID: 12338
	[Tooltip("Valid if any of these items are found at home...")]
	[EnableIf("startingGoal")]
	public List<InteractablePreset> onlyIfFeaturesItemsAtHome = new List<InteractablePreset>();

	// Token: 0x04003033 RID: 12339
	[Tooltip("If true, don't save with game state")]
	public bool disableSave;

	// Token: 0x04003034 RID: 12340
	[Tooltip("A general category we can use to help goals interact with each other")]
	public AIGoalPreset.GoalCategory category;

	// Token: 0x04003035 RID: 12341
	[Header("Priority")]
	[Range(0f, 11f)]
	[Tooltip("The base priority")]
	public int basePriority = 5;

	// Token: 0x04003036 RID: 12342
	[Tooltip("Random variance to add to the priority")]
	[Range(0f, 12f)]
	public int randomVariance = 1;

	// Token: 0x04003037 RID: 12343
	[Tooltip("Clamp min/max priority")]
	public Vector2 minMaxPriority = new Vector2(0f, 10f);

	// Token: 0x04003038 RID: 12344
	[Tooltip("Multiply base priority by amount of trash carried")]
	public bool multiplyUsingTrashCarried;

	// Token: 0x04003039 RID: 12345
	[Tooltip("If the player owes debt then give this maximum priority")]
	public bool useLateDebtPriority;

	// Token: 0x0400303A RID: 12346
	[Tooltip("If true this will only be ranked within the following hours")]
	public bool onlyImportantBetweenHours;

	// Token: 0x0400303B RID: 12347
	[EnableIf("onlyImportantBetweenHours")]
	public Vector2 validBetweenHours = new Vector2(9f, 20.5f);

	// Token: 0x0400303C RID: 12348
	[Tooltip("Don't update the priority of goals (apart from investigate) while this is active")]
	public bool dontUpdateGoalPriorityWhileActive;

	// Token: 0x0400303D RID: 12349
	[Tooltip("Overrides all priority update rules when created")]
	public bool forcePriorityUpdateOnCreation;

	// Token: 0x0400303E RID: 12350
	[Tooltip("When raining, this acts as a multiplier")]
	public AIGoalPreset.RainFactor rainFactor;

	// Token: 0x0400303F RID: 12351
	[Tooltip("Only important when music is playing in the room")]
	public bool useMusic;

	// Token: 0x04003040 RID: 12352
	[Tooltip("Only important if this citizen is trespassing")]
	public bool useTrespassing;

	// Token: 0x04003041 RID: 12353
	[Tooltip("Lose priority over time")]
	public bool affectPriorityOverTime;

	// Token: 0x04003042 RID: 12354
	[EnableIf("affectPriorityOverTime")]
	[Tooltip("Over the course of one hour, add this to the overall priority multiplier")]
	public float multiplierModifierOverOneHour = -0.1f;

	// Token: 0x04003043 RID: 12355
	[Tooltip("Special case which boosts if this is the sniper victim and the sniper is ready and waiting")]
	public bool sniperVictimBoost;

	// Token: 0x04003044 RID: 12356
	[Header("Trait Modifiers")]
	public List<AIGoalPreset.GoalModifierRule> goalModifiers = new List<AIGoalPreset.GoalModifierRule>();

	// Token: 0x04003045 RID: 12357
	[Header("Other Goal Modifiers")]
	public List<AIGoalPreset> ifGoalsPresent = new List<AIGoalPreset>();

	// Token: 0x04003046 RID: 12358
	public float otherGoalPriorityModifier;

	// Token: 0x04003047 RID: 12359
	[Header("Timing Priority")]
	public bool useTiming;

	// Token: 0x04003048 RID: 12360
	[Tooltip("How important is timing to this goal? (Will add this much to overall if @ time)")]
	[EnableIf("useTiming")]
	[Range(0f, 10f)]
	public int timingImportance = 3;

	// Token: 0x04003049 RID: 12361
	[EnableIf("useTiming")]
	[Tooltip("When will the priority start being boosted: From this amount of time before trigger time")]
	[Range(0f, 3f)]
	public float earlyTimingWindow = 0.5f;

	// Token: 0x0400304A RID: 12362
	[Tooltip("Cancel the goal if too late (below time)")]
	[EnableIf("useTiming")]
	public bool cancelIfLate;

	// Token: 0x0400304B RID: 12363
	[EnableIf("cancelIfLate")]
	[Tooltip("Cancel the goal if this late to execute")]
	[Range(0f, 3f)]
	public float cancelIfThisLate = 1f;

	// Token: 0x0400304C RID: 12364
	[Tooltip("Cancel if this has been active for too long")]
	public bool cancelAfterTime;

	// Token: 0x0400304D RID: 12365
	[Range(0f, 24f)]
	[Tooltip("Cancel the goal if it has been active for this time")]
	[EnableIf("cancelAfterTime")]
	public float cancelAfter = 1f;

	// Token: 0x0400304E RID: 12366
	[Tooltip("Run if this citizen becomes late")]
	public bool runIfLate;

	// Token: 0x0400304F RID: 12367
	[Tooltip("Increases priority with hunger (inverse nourishment)")]
	[Header("Stat Priority")]
	[Range(0f, 10f)]
	public int nourishmentImportance;

	// Token: 0x04003050 RID: 12368
	[Range(0f, 10f)]
	[Tooltip("Increases priority with thirst (inverse hydration)")]
	public int hydrationImportance;

	// Token: 0x04003051 RID: 12369
	[Range(0f, 10f)]
	[Tooltip("Increases priority with laziness (inverse altertness)")]
	public int alertnessImportance;

	// Token: 0x04003052 RID: 12370
	[Tooltip("Increases priority with tiredness (inverse energy)")]
	[Range(0f, 10f)]
	public int energyImportance;

	// Token: 0x04003053 RID: 12371
	[Tooltip("Increases priority with bordem (inverse excitement)")]
	[Range(0f, 10f)]
	public int excitementImportance;

	// Token: 0x04003054 RID: 12372
	[Tooltip("Increases priority with todo (inverse chores)")]
	[Range(0f, 10f)]
	public int choresImportance;

	// Token: 0x04003055 RID: 12373
	[Range(0f, 10f)]
	[Tooltip("Increases priority with dirtiness (inverse hygiene)")]
	public int hygieneImportance;

	// Token: 0x04003056 RID: 12374
	[Tooltip("Increases priority with loo (inverse bladder)")]
	[Range(0f, 10f)]
	public int bladderImportance;

	// Token: 0x04003057 RID: 12375
	[Range(0f, 10f)]
	[Tooltip("Increases priority with need for heat (inverse heat)")]
	public int heatImportance;

	// Token: 0x04003058 RID: 12376
	[Range(0f, 10f)]
	[Tooltip("Increases priority with need for heat (inverse heat)")]
	public int drunkImportance;

	// Token: 0x04003059 RID: 12377
	[Range(0f, 10f)]
	[Tooltip("Increases priority with need for breath")]
	public int breathImportance;

	// Token: 0x0400305A RID: 12378
	[Range(0f, 15f)]
	[Tooltip("Increases priority when poisioned")]
	public int poisonImportance;

	// Token: 0x0400305B RID: 12379
	[Range(0f, 50f)]
	[Tooltip("Increases priority when blinded")]
	public int blindedImportance;

	// Token: 0x0400305C RID: 12380
	[Tooltip("This goal will be removed when all actions have been completed")]
	[Header("Completion")]
	public bool completable;

	// Token: 0x0400305D RID: 12381
	[DisableIf("completable")]
	[Tooltip("When actions are completed, restart the above list")]
	public bool loopingActions;

	// Token: 0x0400305E RID: 12382
	[Tooltip("If false this action cannot be interupted once started")]
	[Header("Interuption")]
	public bool interuptable = true;

	// Token: 0x0400305F RID: 12383
	[EnableIf("interuptable")]
	public bool unteruptableByFollowingCategories;

	// Token: 0x04003060 RID: 12384
	[EnableIf("interuptable")]
	public List<AIGoalPreset.GoalCategory> uninteruptableByCategories = new List<AIGoalPreset.GoalCategory>();

	// Token: 0x04003061 RID: 12385
	[Tooltip("If true this action will use this threshold before it is interupted")]
	[EnableIf("interuptable")]
	public bool useInteruptionThreshold;

	// Token: 0x04003062 RID: 12386
	[EnableIf("useInteruptionThreshold")]
	[Range(0f, 10f)]
	[Tooltip("Other goals will have to reach this much above the current priority before this one is interupted...")]
	public int interuptionThreshold = 1;

	// Token: 0x04003063 RID: 12387
	[Header("Delay")]
	[Tooltip("If use point is busy, delay goal from repeating for this time...")]
	public float repeatDelayOnBusy = 0.1f;

	// Token: 0x04003064 RID: 12388
	[Tooltip("If interupted by a more important goal, delay goal from repeating for this time...")]
	public float repeatDelayOnInterupt = 0.1f;

	// Token: 0x04003065 RID: 12389
	[Tooltip("If no actions left, delay goal from repeating for this time...")]
	public float repeatDelayOnFinishActions = 0.1f;

	// Token: 0x04003066 RID: 12390
	[Tooltip("If enabled, enforcers on duty will be allowed everywhere to execute this action.")]
	[Header("Location")]
	public bool allowEnforcersEverywhere;

	// Token: 0x04003067 RID: 12391
	[Space(7f)]
	[InfoBox("Select 'Use Current' when none is needed (location is selected within action).\nNearest Available: Finds the nearest interactable using the first action, and passes it along with gamelocation\nCommercial/Commercial Decision: Will execute a decision based on current stats. Will pass a gamelocation, and sometimes a specific interactable.", 0)]
	public AIGoalPreset.LocationOption locationOption;

	// Token: 0x04003068 RID: 12392
	[InfoBox("The below is only relevent for commerical decisions...", 0)]
	public CompanyPreset.CompanyCategory desireCategory;

	// Token: 0x04003069 RID: 12393
	[Space(7f)]
	public AIGoalPreset.RoomOption roomOption;

	// Token: 0x0400306A RID: 12394
	[Space(7f)]
	public AIGoalPreset.FurnitureOption furnitureOption;

	// Token: 0x0400306B RID: 12395
	[InfoBox("If this is true, the first action's found room location (inside active action) becomes the passed room for the entire goal.", 0)]
	public bool actionFoundRoomBecomesPassedRoom;

	// Token: 0x0400306C RID: 12396
	[Tooltip("If true, this goal will not be active if the actions are not at the passed gamelocation")]
	public bool passedGamelocationIsImportant;

	// Token: 0x0400306D RID: 12397
	[Header("Action Setup")]
	[Tooltip("Where should this goal get the actions from?")]
	public AIGoalPreset.GoalActionSource actionSource;

	// Token: 0x0400306E RID: 12398
	public List<AIGoalPreset.GoalActionSetup> actionsSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x0400306F RID: 12399
	[Tooltip("Potentially raise alarm if certain conditions are met.")]
	public bool raiseAlarm;

	// Token: 0x04003070 RID: 12400
	[Tooltip("Allow AI to trespass while performing this goal")]
	public bool allowTrespass;

	// Token: 0x04003071 RID: 12401
	[Tooltip("Disable all action insertions during this goal")]
	public bool disableActionInsertions;

	// Token: 0x04003072 RID: 12402
	[Tooltip("Send consumables to trash on activation")]
	public bool trashConsumablesOnActivate = true;

	// Token: 0x04003073 RID: 12403
	[Tooltip("Disable the ability to throw objects in combat for this goal")]
	public bool disableThrowing;

	// Token: 0x04003074 RID: 12404
	[Tooltip("Disable trigger to mugging if this goal is active")]
	public bool diabledMugging;

	// Token: 0x04003075 RID: 12405
	[Space(5f)]
	[Tooltip("Pottering: Occasionally the AI will insert one of these actions into the goal.")]
	public bool allowPottering;

	// Token: 0x04003076 RID: 12406
	[EnableIf("allowPottering")]
	public AIGoalPreset.GoalActionSource potterSource;

	// Token: 0x04003077 RID: 12407
	[Tooltip("How often the AI 'potters'. Can be overridden by the above setting")]
	[EnableIf("allowPottering")]
	public Vector2 potterFrequency = new Vector2(0.5f, 1f);

	// Token: 0x04003078 RID: 12408
	[EnableIf("allowPottering")]
	[ReorderableList]
	public List<AIActionPreset> potterActions = new List<AIActionPreset>();

	// Token: 0x04003079 RID: 12409
	[Tooltip("Override the location's lighting behaviour")]
	public bool overrideLightingBehaviour;

	// Token: 0x0400307A RID: 12410
	[EnableIf("overrideLightingBehaviour")]
	public bool onlyOverrideIfAtGamelocation = true;

	// Token: 0x0400307B RID: 12411
	[EnableIf("overrideLightingBehaviour")]
	public List<RoomConfiguration.AILightingBehaviour> lightingBehaviour = new List<RoomConfiguration.AILightingBehaviour>();

	// Token: 0x0400307C RID: 12412
	[Tooltip("Execute the closing of doors as below:")]
	public AIActionPreset.DoorRule doorRule;

	// Token: 0x0400307D RID: 12413
	[Header("Speech")]
	[Range(0f, 1f)]
	public float chanceOfOnTrigger = 0.5f;

	// Token: 0x0400307E RID: 12414
	public List<SpeechController.Bark> onTriggerBark = new List<SpeechController.Bark>();

	// Token: 0x020006AC RID: 1708
	public enum GoalCategory
	{
		// Token: 0x04003080 RID: 12416
		trivial,
		// Token: 0x04003081 RID: 12417
		important,
		// Token: 0x04003082 RID: 12418
		vital
	}

	// Token: 0x020006AD RID: 1709
	public enum StartingGoal
	{
		// Token: 0x04003084 RID: 12420
		all,
		// Token: 0x04003085 RID: 12421
		nonHomelessOnly,
		// Token: 0x04003086 RID: 12422
		homelessOnly
	}

	// Token: 0x020006AE RID: 1710
	public enum RainFactor
	{
		// Token: 0x04003088 RID: 12424
		none,
		// Token: 0x04003089 RID: 12425
		onlyDoWhenRaining,
		// Token: 0x0400308A RID: 12426
		dontDoWhenRaining
	}

	// Token: 0x020006AF RID: 1711
	[Serializable]
	public class GoalModifierRule
	{
		// Token: 0x0400308B RID: 12427
		public CharacterTrait.RuleType rule;

		// Token: 0x0400308C RID: 12428
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x0400308D RID: 12429
		[ShowIf("isTrait")]
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x0400308E RID: 12430
		[Tooltip("Add this to a default priority multiplier of 1.")]
		public float priorityMultiplier = 0.5f;
	}

	// Token: 0x020006B0 RID: 1712
	public enum LocationOption
	{
		// Token: 0x04003090 RID: 12432
		useCurrent,
		// Token: 0x04003091 RID: 12433
		home,
		// Token: 0x04003092 RID: 12434
		work,
		// Token: 0x04003093 RID: 12435
		commercial,
		// Token: 0x04003094 RID: 12436
		nearestAvailable,
		// Token: 0x04003095 RID: 12437
		investigate,
		// Token: 0x04003096 RID: 12438
		commercialDecision,
		// Token: 0x04003097 RID: 12439
		patrolLocation,
		// Token: 0x04003098 RID: 12440
		passedInteractable,
		// Token: 0x04003099 RID: 12441
		passedGamelocation,
		// Token: 0x0400309A RID: 12442
		murderLocation
	}

	// Token: 0x020006B1 RID: 1713
	public enum RoomOption
	{
		// Token: 0x0400309C RID: 12444
		none,
		// Token: 0x0400309D RID: 12445
		bedroom,
		// Token: 0x0400309E RID: 12446
		job
	}

	// Token: 0x020006B2 RID: 1714
	public enum FurnitureOption
	{
		// Token: 0x040030A0 RID: 12448
		none,
		// Token: 0x040030A1 RID: 12449
		bed,
		// Token: 0x040030A2 RID: 12450
		job
	}

	// Token: 0x020006B3 RID: 1715
	[Serializable]
	public class GoalActionSetup
	{
		// Token: 0x040030A3 RID: 12451
		public List<AIActionPreset> actions = new List<AIActionPreset>();

		// Token: 0x040030A4 RID: 12452
		public AIGoalPreset.ActionCondition condition;

		// Token: 0x040030A5 RID: 12453
		public float chance = 1f;

		// Token: 0x040030A6 RID: 12454
		public List<AIGoalPreset.GoalModifierRule> traitModifiers = new List<AIGoalPreset.GoalModifierRule>();

		// Token: 0x040030A7 RID: 12455
		public List<AIGoalPreset.StatusModifierRule> statusModifiers = new List<AIGoalPreset.StatusModifierRule>();
	}

	// Token: 0x020006B4 RID: 1716
	[Serializable]
	public class StatusModifierRule
	{
		// Token: 0x040030A8 RID: 12456
		public AIGoalPreset.StatusType status;

		// Token: 0x040030A9 RID: 12457
		public AIGoalPreset.StatusCondition condition;

		// Token: 0x040030AA RID: 12458
		public float value;

		// Token: 0x040030AB RID: 12459
		public float chanceModifier;
	}

	// Token: 0x020006B5 RID: 1717
	public enum StatusType
	{
		// Token: 0x040030AD RID: 12461
		health,
		// Token: 0x040030AE RID: 12462
		nerve,
		// Token: 0x040030AF RID: 12463
		nourishment,
		// Token: 0x040030B0 RID: 12464
		hydration,
		// Token: 0x040030B1 RID: 12465
		alertness,
		// Token: 0x040030B2 RID: 12466
		energy,
		// Token: 0x040030B3 RID: 12467
		excitement,
		// Token: 0x040030B4 RID: 12468
		chores,
		// Token: 0x040030B5 RID: 12469
		hygeine,
		// Token: 0x040030B6 RID: 12470
		bladder,
		// Token: 0x040030B7 RID: 12471
		heat,
		// Token: 0x040030B8 RID: 12472
		breath,
		// Token: 0x040030B9 RID: 12473
		onDutyEnforcer
	}

	// Token: 0x020006B6 RID: 1718
	public enum StatusCondition
	{
		// Token: 0x040030BB RID: 12475
		isEqualOrAbove,
		// Token: 0x040030BC RID: 12476
		isEqualOrBelow,
		// Token: 0x040030BD RID: 12477
		isTrue,
		// Token: 0x040030BE RID: 12478
		isFalse
	}

	// Token: 0x020006B7 RID: 1719
	public enum ActionCondition
	{
		// Token: 0x040030C0 RID: 12480
		always,
		// Token: 0x040030C1 RID: 12481
		atHomeOnly,
		// Token: 0x040030C2 RID: 12482
		inPublicOnly,
		// Token: 0x040030C3 RID: 12483
		atWorkOnly,
		// Token: 0x040030C4 RID: 12484
		onlyIfEscalated,
		// Token: 0x040030C5 RID: 12485
		onlyIfDead,
		// Token: 0x040030C6 RID: 12486
		atHomeNoGuestPass,
		// Token: 0x040030C7 RID: 12487
		noGuestPass,
		// Token: 0x040030C8 RID: 12488
		kidnapOnly,
		// Token: 0x040030C9 RID: 12489
		nonKidnapOnly
	}

	// Token: 0x020006B8 RID: 1720
	public enum GoalActionSource
	{
		// Token: 0x040030CB RID: 12491
		thisConfiguration,
		// Token: 0x040030CC RID: 12492
		jobPreset,
		// Token: 0x040030CD RID: 12493
		murderPreset
	}
}
