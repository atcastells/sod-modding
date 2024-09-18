using System;
using UnityEngine;

// Token: 0x02000803 RID: 2051
public class RoutineControls : MonoBehaviour
{
	// Token: 0x17000123 RID: 291
	// (get) Token: 0x0600261A RID: 9754 RVA: 0x001EA31F File Offset: 0x001E851F
	public static RoutineControls Instance
	{
		get
		{
			return RoutineControls._instance;
		}
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x001EA326 File Offset: 0x001E8526
	private void Awake()
	{
		if (RoutineControls._instance != null && RoutineControls._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RoutineControls._instance = this;
	}

	// Token: 0x040043DB RID: 17371
	private static RoutineControls _instance;

	// Token: 0x040043DC RID: 17372
	[Header("Stat Multipliers")]
	[Tooltip("Based on 3 meals a day + snack, the hunger rate should be ~+0.125 an hour")]
	public float hungerRate = 0.125f;

	// Token: 0x040043DD RID: 17373
	[Tooltip("...Raise it slightly for thirst")]
	public float thirstRate = 0.2f;

	// Token: 0x040043DE RID: 17374
	[Tooltip("Alertness- similar to meals")]
	public float tirednessRate = 0.12f;

	// Token: 0x040043DF RID: 17375
	[Tooltip("Energy- a 17 hour day depleats energy completely")]
	public float energyRate = 0.058f;

	// Token: 0x040043E0 RID: 17376
	[Tooltip("Boredem rate")]
	public float boredemRate = 0.058f;

	// Token: 0x040043E1 RID: 17377
	[Tooltip("Chores rate - 48 hours")]
	public float choresRate = 0.02f;

	// Token: 0x040043E2 RID: 17378
	[Tooltip("Hygiene rate - 24 hours")]
	public float hygeieneRate = 0.04f;

	// Token: 0x040043E3 RID: 17379
	[Tooltip("Bladder rate - 4 hours")]
	public float bladderRate = 0.25f;

	// Token: 0x040043E4 RID: 17380
	[Tooltip("Drunk rate - 2 hours")]
	public float drunkRate = 0.5f;

	// Token: 0x040043E5 RID: 17381
	[Tooltip("Breath rate - 5 mins")]
	public float breathRate = 0.083f;

	// Token: 0x040043E6 RID: 17382
	[Tooltip("Idle sound rate - 1 hour")]
	public float idleSoundRate = 1f;

	// Token: 0x040043E7 RID: 17383
	[Tooltip("Poison remove rate - 1 hours")]
	public float poisonRate = 1f;

	// Token: 0x040043E8 RID: 17384
	[Tooltip("Blinded remove rate - 5 mins")]
	public float blindedRate = 0.083f;

	// Token: 0x040043E9 RID: 17385
	[Header("Routine")]
	[Tooltip("Citizen decisions on whether to go out to get certain things like food depend on 1) how much time they've spent somewhere, this many hours = 100% decision to go somewhere else")]
	public float commericalDecisionMPTimeSpent = 3f;

	// Token: 0x040043EA RID: 17386
	[Range(0f, 1f)]
	[Tooltip("How likely a citizen will choose to go out to get certain things (eg. Food) when the player is in the same building.")]
	public float commericalDecisionMPlayerSameBuilding = 0.66f;

	// Token: 0x040043EB RID: 17387
	[Tooltip("How likely a citizen will choose to go out to get certain things (eg. Food) when the player is in the same gamelocation.")]
	[Range(0f, 1f)]
	public float commericalDecisionMPlayerSameLocation = 0.33f;

	// Token: 0x040043EC RID: 17388
	[Tooltip("How likely a citizen will choose to go out to get certain things (eg. Food) when the player is not in the above.")]
	[Range(0f, 2f)]
	public float commericalDecisionMPlayerElsewhere = 1f;

	// Token: 0x040043ED RID: 17389
	[Header("Action Reference")]
	public AIGoalPreset workGoal;

	// Token: 0x040043EE RID: 17390
	public AIGoalPreset answerDoorGoal;

	// Token: 0x040043EF RID: 17391
	public AIGoalPreset awakenGoal;

	// Token: 0x040043F0 RID: 17392
	public AIGoalPreset sleepGoal;

	// Token: 0x040043F1 RID: 17393
	public AIGoalPreset patrolGoal;

	// Token: 0x040043F2 RID: 17394
	public AIGoalPreset fleeGoal;

	// Token: 0x040043F3 RID: 17395
	public AIGoalPreset investigateGoal;

	// Token: 0x040043F4 RID: 17396
	public AIGoalPreset postJob;

	// Token: 0x040043F5 RID: 17397
	public AIGoalPreset enforcerResponse;

	// Token: 0x040043F6 RID: 17398
	public AIGoalPreset enforcerGuardDuty;

	// Token: 0x040043F7 RID: 17399
	public AIGoalPreset makeSpecificCall;

	// Token: 0x040043F8 RID: 17400
	public AIGoalPreset layLow;

	// Token: 0x040043F9 RID: 17401
	public AIGoalPreset kidnapperCollectRansom;

	// Token: 0x040043FA RID: 17402
	public AIGoalPreset kidnapperFreeVictim;

	// Token: 0x040043FB RID: 17403
	public AIActionPreset searchArea;

	// Token: 0x040043FC RID: 17404
	public AIActionPreset searchAreaEnforcer;

	// Token: 0x040043FD RID: 17405
	public AIActionPreset hangUp;

	// Token: 0x040043FE RID: 17406
	public AIActionPreset raiseAlarm;

	// Token: 0x040043FF RID: 17407
	public AIActionPreset sleep;

	// Token: 0x04004400 RID: 17408
	public AIActionPreset audioFocus;

	// Token: 0x04004401 RID: 17409
	public AIActionPreset mainLightOn;

	// Token: 0x04004402 RID: 17410
	public AIActionPreset mainLightOff;

	// Token: 0x04004403 RID: 17411
	public AIActionPreset secondaryLightOn;

	// Token: 0x04004404 RID: 17412
	public AIActionPreset secondaryLightOff;

	// Token: 0x04004405 RID: 17413
	public AIActionPreset lockDoor;

	// Token: 0x04004406 RID: 17414
	public AIActionPreset unlockDoor;

	// Token: 0x04004407 RID: 17415
	public AIActionPreset openDoor;

	// Token: 0x04004408 RID: 17416
	public AIActionPreset closeDoor;

	// Token: 0x04004409 RID: 17417
	public AIActionPreset knockOnDoor;

	// Token: 0x0400440A RID: 17418
	public AIActionPreset openLocker;

	// Token: 0x0400440B RID: 17419
	public AIActionPreset closeLocker;

	// Token: 0x0400440C RID: 17420
	public AIActionPreset hide;

	// Token: 0x0400440D RID: 17421
	public AIActionPreset pullPlayerFromHiding;

	// Token: 0x0400440E RID: 17422
	public AIActionPreset answerTelephone;

	// Token: 0x0400440F RID: 17423
	public AIActionPreset makeCall;

	// Token: 0x04004410 RID: 17424
	public AIActionPreset takeMoney;

	// Token: 0x04004411 RID: 17425
	public AIActionPreset pickupFromFloor;

	// Token: 0x04004412 RID: 17426
	public AIActionPreset putBack;

	// Token: 0x04004413 RID: 17427
	public AIActionPreset turnOnMusic;

	// Token: 0x04004414 RID: 17428
	public AIActionPreset disposal;

	// Token: 0x04004415 RID: 17429
	public AIActionPreset bargeDoor;

	// Token: 0x04004416 RID: 17430
	public AIActionPreset standAgainstWall;

	// Token: 0x04004417 RID: 17431
	public AIActionPreset standGuard;

	// Token: 0x04004418 RID: 17432
	public AIActionPreset putUpPoliceTape;

	// Token: 0x04004419 RID: 17433
	public AIActionPreset putUpStreetCrimeScene;

	// Token: 0x0400441A RID: 17434
	public AIActionPreset getHandIn;

	// Token: 0x0400441B RID: 17435
	public AIActionPreset AIPutDownItem;

	// Token: 0x0400441C RID: 17436
	public AIActionPreset AIPickUpItem;

	// Token: 0x0400441D RID: 17437
	public AIActionPreset purchaseItem;

	// Token: 0x0400441E RID: 17438
	public AIActionPreset takeConsumable;

	// Token: 0x0400441F RID: 17439
	public AIActionPreset sit;

	// Token: 0x04004420 RID: 17440
	public AIActionPreset lookBehindSpooked;

	// Token: 0x04004421 RID: 17441
	public AIActionPreset mugging;

	// Token: 0x04004422 RID: 17442
	public AIActionPreset loiterConfront;

	// Token: 0x04004423 RID: 17443
	public AIActionPreset takeFirstPersonItem;

	// Token: 0x04004424 RID: 17444
	public AIActionPreset cleanUp;

	// Token: 0x04004425 RID: 17445
	public AIGoalPreset findDeadBody;

	// Token: 0x04004426 RID: 17446
	public AIGoalPreset smellDeadBody;

	// Token: 0x04004427 RID: 17447
	public AIGoalPreset mourn;

	// Token: 0x04004428 RID: 17448
	public AIGoalPreset stealItem;

	// Token: 0x04004429 RID: 17449
	public AIGoalPreset exitBuilding;

	// Token: 0x0400442A RID: 17450
	public AIGoalPreset missionMeetUpSpecific;

	// Token: 0x0400442B RID: 17451
	public AIGoalPreset giveSelfUp;

	// Token: 0x0400442C RID: 17452
	public AIGoalPreset meetFood;

	// Token: 0x0400442D RID: 17453
	public GroupPreset meetUpFoodMission;

	// Token: 0x0400442E RID: 17454
	public AIGoalPreset toGoGoal;

	// Token: 0x0400442F RID: 17455
	public AIGoalPreset toGoWalkGoal;

	// Token: 0x04004430 RID: 17456
	public BuildingPreset cityHall;

	// Token: 0x04004431 RID: 17457
	[Header("Sales records")]
	[Tooltip("How many sales records are kept")]
	public int salesRecordsThreshold = 100;
}
