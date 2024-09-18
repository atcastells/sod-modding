using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200069B RID: 1691
[CreateAssetMenu(fileName = "aiaction_data", menuName = "Database/AI/AI Action")]
public class AIActionPreset : SoCustomComparison
{
	// Token: 0x04002F06 RID: 12038
	[Header("Input")]
	public InteractablePreset.InteractionKey defaultKey;

	// Token: 0x04002F07 RID: 12039
	[Tooltip("Debug this input")]
	public bool debug;

	// Token: 0x04002F08 RID: 12040
	[Range(-10f, 11f)]
	[Tooltip("Useful when there are multiple active actions, the highest takes priority.")]
	public int inputPriority;

	// Token: 0x04002F09 RID: 12041
	[Tooltip("Only available when no first person item selected")]
	public bool unavailableWhenItemSelected;

	// Token: 0x04002F0A RID: 12042
	[EnableIf("unavailableWhenItemSelected")]
	[Tooltip("If left blank, this is available when any first person item is selected...")]
	public List<FirstPersonItem> unavailableWhenItemsSelected = new List<FirstPersonItem>();

	// Token: 0x04002F0B RID: 12043
	[Tooltip("Only available when this first person item selected")]
	public bool onlyAvailableWhenItemSelected;

	// Token: 0x04002F0C RID: 12044
	[EnableIf("onlyAvailableWhenItemSelected")]
	public List<FirstPersonItem> availableWhenItemsSelected = new List<FirstPersonItem>();

	// Token: 0x04002F0D RID: 12045
	public bool holsterCurrentItemOnAction;

	// Token: 0x04002F0E RID: 12046
	[Tooltip("Disable display on UI")]
	public bool disableUIDisplay;

	// Token: 0x04002F0F RID: 12047
	[InfoBox("Use this carefully in conjuction with the goal's location.\nInteractable: Checks for passed interactable, room, then location to find a destination.\nFind Nearest: Finds the nearest.\nInvestigate: Uses investigate position\nNearby Investigate: Uses a destination close to the investigation position\nPause: Uses the AI existing position\nRandom Node Within Location: A random node within the passed gamelocation\nFlee: Destination is somewhere safe\nInteractable LOS: Location within line of sight of the passed interactable\nNearby Random Street Node: Pick somewhere on a close by street.", 0)]
	[Header("Location")]
	public AIActionPreset.ActionLocation actionLocation;

	// Token: 0x04002F10 RID: 12048
	[Tooltip("Check this against the room's action reference to continue using the passed reference.")]
	public bool confirmActionLocation = true;

	// Token: 0x04002F11 RID: 12049
	[Tooltip("If true then AI will pick a random sublocation if no other use position is specified; if false then it will tend to pick the default (centre) or closest sublocation")]
	public bool useRandomNodeSublocation;

	// Token: 0x04002F12 RID: 12050
	[Tooltip("If unable to find a node location for this action, attempt to find one using 'nearest' fuction (can be expensive). If this isn't checked then the goal will be removed.")]
	public AIActionPreset.ActionFinding onUnableToFindLocation = AIActionPreset.ActionFinding.findNearest;

	// Token: 0x04002F13 RID: 12051
	[Tooltip("Where to search when finding a location...")]
	public AIActionPreset.FindSetting searchSetting;

	// Token: 0x04002F14 RID: 12052
	[Tooltip("If the found use point is busy, do this...")]
	public AIActionPreset.ActionBusy onUsePointBusy;

	// Token: 0x04002F15 RID: 12053
	public Interactable.UsePointSlot usageSlot;

	// Token: 0x04002F16 RID: 12054
	[Tooltip("Consider this at the destination if we're close enough")]
	public bool useCloseEnoughSetting;

	// Token: 0x04002F17 RID: 12055
	[Tooltip("How much to factor in robbery priority when searching for location...")]
	public float robberyPriorityMultiplier;

	// Token: 0x04002F18 RID: 12056
	[Tooltip("Avoid choosing repeating interactables as long as this goal exists...")]
	public bool avoidRepeatingInteractables;

	// Token: 0x04002F19 RID: 12057
	[Tooltip("Aids searching by filtering rooms types that this must be in...")]
	public bool filterSearchUsingRoomType;

	// Token: 0x04002F1A RID: 12058
	[EnableIf("filterSearchUsingRoomType")]
	[Tooltip("Aids searching by filtering rooms types that this must be in...")]
	public List<RoomTypePreset> searchRoomType = new List<RoomTypePreset>();

	// Token: 0x04002F1B RID: 12059
	[Tooltip("Limit search to the goal's game location")]
	public bool limitSearchToGoalLocation;

	// Token: 0x04002F1C RID: 12060
	[Tooltip("When finding an action; Always use home as an option (even if above is true)")]
	public bool findOverrideWithHome = true;

	// Token: 0x04002F1D RID: 12061
	[Tooltip("Use special availability settings: Address telephone with nobody answering")]
	public bool requiresTelephone;

	// Token: 0x04002F1E RID: 12062
	[ShowIf("requiresTelephone")]
	[Tooltip("Use special availability settings: Address telephone with no calls active")]
	public bool requiresTelephoneNoCall;

	// Token: 0x04002F1F RID: 12063
	[Tooltip("Skip activation if there is no consumable to hand")]
	public bool activationRequiresConsumable;

	// Token: 0x04002F20 RID: 12064
	[Tooltip("Pull banned rooms from here...")]
	public AIActionPreset.SourceOfBannedRooms bannedRooms;

	// Token: 0x04002F21 RID: 12065
	[Tooltip("If true this action will execute until it is interupted by something else")]
	[Header("Completion")]
	public bool completableAction = true;

	// Token: 0x04002F22 RID: 12066
	[MinMaxSlider(0f, 120f)]
	[Tooltip("Time taken in minutes")]
	[ShowIf("completableAction")]
	public Vector2 minutesTakenRange = new Vector2(1f, 10f);

	// Token: 0x04002F23 RID: 12067
	[Tooltip("Complete when AI has seen player do something illegal")]
	public bool completeOnSeeIllegal;

	// Token: 0x04002F24 RID: 12068
	[Tooltip("If true, once complete, this action will create another instance of itself, effectively repeating")]
	public bool repeatOnComplete;

	// Token: 0x04002F25 RID: 12069
	[Tooltip("Repeat while the citizen has consumable items...")]
	[EnableIf("repeatOnComplete")]
	public bool repeatWhileHavingConsumables;

	// Token: 0x04002F26 RID: 12070
	[Tooltip("AI controller will not be diabled on idle while this action is active if true")]
	public bool requiresForcedUpdate;

	// Token: 0x04002F27 RID: 12071
	[Tooltip("Don't update the priority of other goals (apart from investigate) while this is active")]
	[Header("Update")]
	public bool dontUpdateGoalPriorityWhileActive;

	// Token: 0x04002F28 RID: 12072
	[Tooltip("Don't update the priority of other goals (apart from investigate) for this long after the action has been started (minutes)")]
	[DisableIf("dontUpdateGoalPriorityWhileActive")]
	public int dontUpdateGoalPriorityFor;

	// Token: 0x04002F29 RID: 12073
	[Tooltip("If true the tick rate can be no higher than below while performing this action")]
	[Space(5f)]
	public bool limitTickRate;

	// Token: 0x04002F2A RID: 12074
	[EnableIf("limitTickRate")]
	public NewAIController.AITickRate minimumTickRate;

	// Token: 0x04002F2B RID: 12075
	[EnableIf("limitTickRate")]
	public NewAIController.AITickRate maximumTickRate = NewAIController.AITickRate.medium;

	// Token: 0x04002F2C RID: 12076
	[Tooltip("If true, this action won't be removed upon goal's RefreshActions()")]
	public bool dontRemoveOnRefresh;

	// Token: 0x04002F2D RID: 12077
	[Tooltip("If true, this action won't be replaced upon goal's RefreshActions()")]
	public bool nonRefreshable;

	// Token: 0x04002F2E RID: 12078
	[Tooltip("Once victim is in LOS, then stop")]
	public bool useLOSCheck;

	// Token: 0x04002F2F RID: 12079
	[Tooltip("Cancel if target is not a valid mugging")]
	public bool cancelIfNonValidMugging;

	// Token: 0x04002F30 RID: 12080
	[Tooltip("Cancel if player is not loitering")]
	public bool cancelIfPlayerNotLoitering;

	// Token: 0x04002F31 RID: 12081
	[Tooltip("Skip creation of this action if the AI is in the following state...")]
	public bool skipIfAIIsInState;

	// Token: 0x04002F32 RID: 12082
	[EnableIf("skipIfAIIsInState")]
	[Tooltip("Skip creation of this action if the AI is in the following state...")]
	public NewAIController.ReactionState skipIfReaction;

	// Token: 0x04002F33 RID: 12083
	[Tooltip("Skip if the player has a guest pass to here")]
	public bool skipIfGuestPass;

	// Token: 0x04002F34 RID: 12084
	[Header("Facing")]
	[Tooltip("Which way will this AI face when arrived at this action")]
	public AIActionPreset.ActionFacingDirection facing = AIActionPreset.ActionFacingDirection.interactableSetting;

	// Token: 0x04002F35 RID: 12085
	[Tooltip("If true, the AI will look around randomly if they don't have a specific target")]
	public bool lookAround;

	// Token: 0x04002F36 RID: 12086
	[Tooltip("If the persuit target isn't in range, cancel this action")]
	public bool cancelIfPersuitTargetNotInRange;

	// Token: 0x04002F37 RID: 12087
	[Tooltip("Face player when interacting")]
	public bool facePlayerWhileTalkingTo = true;

	// Token: 0x04002F38 RID: 12088
	[BoxGroup("Idle Animations")]
	public bool changeIdleOnActivate = true;

	// Token: 0x04002F39 RID: 12089
	[BoxGroup("Idle Animations")]
	[EnableIf("changeIdleOnActivate")]
	public CitizenAnimationController.IdleAnimationState idleAnimationOnActivate;

	// Token: 0x04002F3A RID: 12090
	[Space(5f)]
	[BoxGroup("Idle Animations")]
	public bool changeIdleOnArrival;

	// Token: 0x04002F3B RID: 12091
	[EnableIf("changeIdleOnArrival")]
	[BoxGroup("Idle Animations")]
	public CitizenAnimationController.IdleAnimationState idleAnimationOnArrival;

	// Token: 0x04002F3C RID: 12092
	[Space(5f)]
	[BoxGroup("Idle Animations")]
	public bool changeIdleOnDeactivate;

	// Token: 0x04002F3D RID: 12093
	[BoxGroup("Idle Animations")]
	[EnableIf("changeIdleOnDeactivate")]
	public CitizenAnimationController.IdleAnimationState idleAnimationOnDeactivate;

	// Token: 0x04002F3E RID: 12094
	[Space(5f)]
	[BoxGroup("Idle Animations")]
	public bool changeIdleOnComplete;

	// Token: 0x04002F3F RID: 12095
	[EnableIf("changeIdleOnComplete")]
	[BoxGroup("Idle Animations")]
	public CitizenAnimationController.IdleAnimationState idleAnimationOnComplete;

	// Token: 0x04002F40 RID: 12096
	[BoxGroup("Arm Animations")]
	public bool changeArmsOnActivate = true;

	// Token: 0x04002F41 RID: 12097
	[BoxGroup("Arm Animations")]
	[EnableIf("changeArmsOnActivate")]
	public CitizenAnimationController.ArmsBoolSate armsAnimationOnActivate;

	// Token: 0x04002F42 RID: 12098
	[Space(5f)]
	[BoxGroup("Arm Animations")]
	public bool changeArmsOnArrival;

	// Token: 0x04002F43 RID: 12099
	[BoxGroup("Arm Animations")]
	[EnableIf("changeArmsOnArrival")]
	public CitizenAnimationController.ArmsBoolSate armsAnimationOnArrival;

	// Token: 0x04002F44 RID: 12100
	[BoxGroup("Arm Animations")]
	[Space(5f)]
	public bool changeArmsOnDeactivate;

	// Token: 0x04002F45 RID: 12101
	[EnableIf("changeArmsOnDeactivate")]
	[BoxGroup("Arm Animations")]
	public CitizenAnimationController.ArmsBoolSate armsAnimationOnDeactivate;

	// Token: 0x04002F46 RID: 12102
	[BoxGroup("Arm Animations")]
	[Space(5f)]
	public bool changeArmsOnComplete;

	// Token: 0x04002F47 RID: 12103
	[BoxGroup("Arm Animations")]
	[EnableIf("changeArmsOnComplete")]
	public CitizenAnimationController.ArmsBoolSate armsAnimationOnComplete;

	// Token: 0x04002F48 RID: 12104
	[Tooltip("Once destination is reached, tell the AI to lie down")]
	[Space(5f)]
	public bool lying;

	// Token: 0x04002F49 RID: 12105
	[Tooltip("Pull the below from the currently-held consumable item...")]
	[Header("On Progress Stat modifiers")]
	public bool useCurrentConsumable;

	// Token: 0x04002F4A RID: 12106
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	[Tooltip("This is applied as progress increases")]
	public float progressNourishment;

	// Token: 0x04002F4B RID: 12107
	[Tooltip("This is applied as progress increases")]
	[EnableIf("completableAction")]
	[Range(-1f, 1f)]
	public float progressHydration;

	// Token: 0x04002F4C RID: 12108
	[Tooltip("This is applied as progress increases")]
	[EnableIf("completableAction")]
	[Range(-1f, 1f)]
	public float progressAlertness;

	// Token: 0x04002F4D RID: 12109
	[Tooltip("This is applied as progress increases")]
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	public float progressEnergy;

	// Token: 0x04002F4E RID: 12110
	[Tooltip("This is applied as progress increases")]
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	public float progressExcitement;

	// Token: 0x04002F4F RID: 12111
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	[Tooltip("This is applied as progress increases")]
	public float progressChores;

	// Token: 0x04002F50 RID: 12112
	[EnableIf("completableAction")]
	[Range(-1f, 1f)]
	[Tooltip("This is applied as progress increases")]
	public float progressHygeiene;

	// Token: 0x04002F51 RID: 12113
	[Tooltip("This is applied as progress increases")]
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	public float progressBladder;

	// Token: 0x04002F52 RID: 12114
	[EnableIf("completableAction")]
	[Tooltip("This is applied as progress increases")]
	[Range(-1f, 1f)]
	public float progressHeat;

	// Token: 0x04002F53 RID: 12115
	[Tooltip("This is applied as progress increases")]
	[EnableIf("completableAction")]
	[Range(-1f, 1f)]
	public float progressDrunk;

	// Token: 0x04002F54 RID: 12116
	[Tooltip("This is applied as progress increases")]
	[Range(-1f, 1f)]
	[EnableIf("completableAction")]
	public float progressBreath;

	// Token: 0x04002F55 RID: 12117
	[EnableIf("completableAction")]
	[Range(-1f, 1f)]
	public float progressPoisoned;

	// Token: 0x04002F56 RID: 12118
	[Header("Per Hour Stat modifiers")]
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeNourishment;

	// Token: 0x04002F57 RID: 12119
	[Range(-12f, 12f)]
	[Tooltip("This is applied over time")]
	public float overtimeHydration;

	// Token: 0x04002F58 RID: 12120
	[Range(-12f, 12f)]
	[Tooltip("This is applied over time")]
	public float overtimeAlertness;

	// Token: 0x04002F59 RID: 12121
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeEnergy;

	// Token: 0x04002F5A RID: 12122
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeExcitement;

	// Token: 0x04002F5B RID: 12123
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeChores;

	// Token: 0x04002F5C RID: 12124
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeHygiene;

	// Token: 0x04002F5D RID: 12125
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeBladder;

	// Token: 0x04002F5E RID: 12126
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeHeat;

	// Token: 0x04002F5F RID: 12127
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimeDrunk;

	// Token: 0x04002F60 RID: 12128
	[Range(-12f, 12f)]
	[Tooltip("This is applied over time")]
	public float overtimeBreath;

	// Token: 0x04002F61 RID: 12129
	[Tooltip("This is applied over time")]
	[Range(-12f, 12f)]
	public float overtimePoison;

	// Token: 0x04002F62 RID: 12130
	[Header("Movement")]
	[Tooltip("If true this will use rules consistent with AI's investigate urgency state. If false and below is false, it will walk...")]
	public bool useInvestigationUrgency;

	// Token: 0x04002F63 RID: 12131
	[DisableIf("useInvestigationUrgency")]
	[Tooltip("If true this will use running only")]
	public bool forceRun;

	// Token: 0x04002F64 RID: 12132
	[Tooltip("Will run if this citizen can see the player")]
	public bool runIfSeesPlayer;

	// Token: 0x04002F65 RID: 12133
	[Tooltip("If true this will encourage using interactable with people I know and discourgage using them with people I don't. If this has a passed human interactable, I will save a space for them.")]
	[Header("AI")]
	public bool socialRules;

	// Token: 0x04002F66 RID: 12134
	[Tooltip("If true, the AI will detect the player as suspicious while this is active")]
	public bool spookAction;

	// Token: 0x04002F67 RID: 12135
	[Tooltip("Disable sighting updates while this action is active")]
	public bool disableSightingUpdates;

	// Token: 0x04002F68 RID: 12136
	[Tooltip("Attack the persuit target if they are close enough")]
	public bool attackPersuitTargetOnProximity;

	// Token: 0x04002F69 RID: 12137
	[EnableIf("attackPersuitTargetOnProximity")]
	[Tooltip("Throw current items if at suitable range")]
	public bool throwObjectsAtTarget = true;

	// Token: 0x04002F6A RID: 12138
	[Tooltip("Put the AI in combat pose")]
	public AIActionPreset.CombatPose useCombatPose = AIActionPreset.CombatPose.never;

	// Token: 0x04002F6B RID: 12139
	[Tooltip("In addition to the above condition, only use combat pose when escalation of investiation is 1")]
	public bool onlyUseCombatPoseWithEscalationOne = true;

	// Token: 0x04002F6C RID: 12140
	[Tooltip("Go to sleep OnComplete, wake up on end")]
	public bool sleepOnArrival;

	// Token: 0x04002F6D RID: 12141
	[Tooltip("This action is uninteruptable after destination has been reached (overrides the goal's interuption preset settings)")]
	public bool uninteruptableWhileAtLocation;

	// Token: 0x04002F6E RID: 12142
	[Tooltip("While active, Vmail threads can be progressed")]
	public bool progressVmailThreads;

	// Token: 0x04002F6F RID: 12143
	[Tooltip("If true, will disable casual conversation triggers while this is active")]
	public bool disableConversationTriggers;

	// Token: 0x04002F70 RID: 12144
	[Tooltip("If true, will cancel any conversations when activated")]
	public bool exitConversationOnActivate;

	// Token: 0x04002F71 RID: 12145
	[Space(5f)]
	[Tooltip("Interactable presets related to this furniture parent must stay swtiched on while this is active...")]
	public List<InteractablePreset> forcedActive = new List<InteractablePreset>();

	// Token: 0x04002F72 RID: 12146
	[Tooltip("If AI, force perform these actions on the same object if they exist, if not integrated interactables on the same furniture will also be checked: On Arrival")]
	public List<AIActionPreset.AutomaticAction> forcedActionsOnArrival = new List<AIActionPreset.AutomaticAction>();

	// Token: 0x04002F73 RID: 12147
	[Tooltip("If AI, force perform these actions on the same object if they exist, if not integrated interactables on the same furniture will also be checked: On End")]
	public List<AIActionPreset.AutomaticAction> forcedActionsOnComplete = new List<AIActionPreset.AutomaticAction>();

	// Token: 0x04002F74 RID: 12148
	[Tooltip("To complete the above actions, if AI cannot find the appropriate action on the immediate interactable, search this much...")]
	public AIActionPreset.ForcedActionsSearchLevel forcedActionsSearchLevel = AIActionPreset.ForcedActionsSearchLevel.spawnedInteractablesAll;

	// Token: 0x04002F75 RID: 12149
	[Tooltip("Also execute the above actions if action is ended for any reason")]
	public bool executeCompleteActionsOnEnd;

	// Token: 0x04002F76 RID: 12150
	[Tooltip("Only do the above if at location")]
	[EnableIf("executeCompleteActionsOnEnd")]
	public bool executeCompleteActionsOnEndIfArrived = true;

	// Token: 0x04002F77 RID: 12151
	[Tooltip("Automatically execute this action on complete (action controller script)")]
	public bool executeThisOnComplete = true;

	// Token: 0x04002F78 RID: 12152
	[Tooltip("Execute these switch state changes on end along with the above actions...")]
	public List<InteractablePreset.SwitchState> switchStatesOnEnd = new List<InteractablePreset.SwitchState>();

	// Token: 0x04002F79 RID: 12153
	[Tooltip("This action will trigger an interactable being illegally activated if a character tresspassing triggers it")]
	public bool tamperAction;

	// Token: 0x04002F7A RID: 12154
	[Tooltip("This action will close/deactivate an illegally activated object (eg. turn off tv)")]
	public bool tamperResetAction;

	// Token: 0x04002F7B RID: 12155
	[Tooltip("If true then this citizen can fall asleep while doing this if they are tired enough")]
	public bool canFallAsleep;

	// Token: 0x04002F7C RID: 12156
	[Tooltip("If above is true then citizens can fall asleep after this time")]
	[EnableIf("canFallAsleep")]
	public int fallAsleepAfterMinimum = 20;

	// Token: 0x04002F7D RID: 12157
	[Tooltip("Special case: If this is the sniper killer, allow a sniper shot while this action is active")]
	public bool allowSniperShot;

	// Token: 0x04002F7E RID: 12158
	[Space(7f)]
	[Tooltip("On tick, check state of chosen interactable. If any of these match then cancel this action.")]
	public List<AIActionPreset.CheckActionAgainstState> checkActionAgainstState = new List<AIActionPreset.CheckActionAgainstState>();

	// Token: 0x04002F7F RID: 12159
	[Tooltip("Enable to force a reaction state once this action is activated. Will not switch it back once ended.")]
	[Space(5f)]
	public bool forceReactionState;

	// Token: 0x04002F80 RID: 12160
	[EnableIf("forceReactionState")]
	public NewAIController.ReactionState setReactionState;

	// Token: 0x04002F81 RID: 12161
	[Tooltip("Ignore door keys settings")]
	public bool ignoreLockedDoors;

	// Token: 0x04002F82 RID: 12162
	[Tooltip("Break down doors in my way!")]
	public bool breakDownDoors;

	// Token: 0x04002F83 RID: 12163
	[Header("Allowable Action Insertions")]
	public bool doorsAllowed = true;

	// Token: 0x04002F84 RID: 12164
	public bool deactivateAllowed = true;

	// Token: 0x04002F85 RID: 12165
	[Header("Delay")]
	[Tooltip("If use point is busy, delay goal from repeating for this time...")]
	public float repeatDelayOnActionFail;

	// Token: 0x04002F86 RID: 12166
	[Tooltip("If interupted by a more important goal, delay goal from repeating for this time...")]
	public float repeatDelayOnActionSuccess;

	// Token: 0x04002F87 RID: 12167
	[Header("Basic Actions")]
	[Tooltip("When at the gamelocation, turn all lights off, excluding the destination room.")]
	public bool turnAllGamelocationLightsOff;

	// Token: 0x04002F88 RID: 12168
	public bool overrideGoalLightRule;

	// Token: 0x04002F89 RID: 12169
	[EnableIf("overrideGoalLightRule")]
	public bool onlyOverrideIfAtGamelocation = true;

	// Token: 0x04002F8A RID: 12170
	[EnableIf("overrideGoalLightRule")]
	public List<RoomConfiguration.AILightingBehaviour> lightingBehaviour = new List<RoomConfiguration.AILightingBehaviour>();

	// Token: 0x04002F8B RID: 12171
	public bool overrideGoalDoorRule;

	// Token: 0x04002F8C RID: 12172
	[EnableIf("overrideGoalDoorRule")]
	[Tooltip("Execute the closing of doors as below:")]
	public AIActionPreset.DoorRule doorRule;

	// Token: 0x04002F8D RID: 12173
	[Header("Sounds")]
	[InfoBox("Note: This is only triggered by AI", 0)]
	public AudioEvent onArrivalSound;

	// Token: 0x04002F8E RID: 12174
	public bool isLoop;

	// Token: 0x04002F8F RID: 12175
	[DisableIf("isLoop")]
	public float soundDelay;

	// Token: 0x04002F90 RID: 12176
	[Tooltip("Check to see if we need outdoor clothes when 'make clothed' is enabled below...")]
	[BoxGroup("Outfits")]
	public bool outdoorClothingCheck = true;

	// Token: 0x04002F91 RID: 12177
	[Space(5f)]
	[BoxGroup("Outfits")]
	public bool specificOutfitOnActivate;

	// Token: 0x04002F92 RID: 12178
	[BoxGroup("Outfits")]
	[EnableIf("specificOutfitOnActivate")]
	public ClothesPreset.OutfitCategory allowedOutfitOnActivate;

	// Token: 0x04002F93 RID: 12179
	[BoxGroup("Outfits")]
	[Tooltip("If no specific outfit is required, make sure the citizen is at least clothed!")]
	[DisableIf("specificOutfitOnActivate")]
	public bool makeClothedOnActivate = true;

	// Token: 0x04002F94 RID: 12180
	[Space(5f)]
	[BoxGroup("Outfits")]
	public bool specificOutfitOnArrive;

	// Token: 0x04002F95 RID: 12181
	[BoxGroup("Outfits")]
	[EnableIf("specificOutfitOnArrive")]
	public ClothesPreset.OutfitCategory allowedOutfitOnArrive;

	// Token: 0x04002F96 RID: 12182
	[BoxGroup("Outfits")]
	[Tooltip("If no specific outfit is required, make sure the citizen is at least clothed!")]
	[DisableIf("specificOutfitOnArrive")]
	public bool makeClothedOnArrive = true;

	// Token: 0x04002F97 RID: 12183
	[Space(5f)]
	[BoxGroup("Outfits")]
	public bool specificOutfitOnDeactivate;

	// Token: 0x04002F98 RID: 12184
	[EnableIf("specificOutfitOnDeactivate")]
	[BoxGroup("Outfits")]
	public ClothesPreset.OutfitCategory allowedOutfitOnDeactivate;

	// Token: 0x04002F99 RID: 12185
	[Tooltip("If no specific outfit is required, make sure the citizen is at least clothed!")]
	[BoxGroup("Outfits")]
	[DisableIf("specificOutfitOnDeactivate")]
	public bool makeClothedOnDeactivate = true;

	// Token: 0x04002F9A RID: 12186
	[Space(5f)]
	[BoxGroup("Outfits")]
	public bool specificOutfitOnComplete;

	// Token: 0x04002F9B RID: 12187
	[EnableIf("specificOutfitOnComplete")]
	[BoxGroup("Outfits")]
	public ClothesPreset.OutfitCategory allowedOutfitOnComplete;

	// Token: 0x04002F9C RID: 12188
	[BoxGroup("Outfits")]
	[DisableIf("specificOutfitOnComplete")]
	[Tooltip("If no specific outfit is required, make sure the citizen is at least clothed!")]
	public bool makeClothedOnComplete = true;

	// Token: 0x04002F9D RID: 12189
	[BoxGroup("Expressions")]
	public bool setExpressionOnActivate;

	// Token: 0x04002F9E RID: 12190
	[EnableIf("setExpressionOnActivate")]
	[BoxGroup("Expressions")]
	public CitizenOutfitController.Expression activateExpression;

	// Token: 0x04002F9F RID: 12191
	[Space(5f)]
	[BoxGroup("Expressions")]
	public bool setExpressionOnArrive;

	// Token: 0x04002FA0 RID: 12192
	[EnableIf("setExpressionOnArrive")]
	[BoxGroup("Expressions")]
	public CitizenOutfitController.Expression arriveExpression;

	// Token: 0x04002FA1 RID: 12193
	[Space(5f)]
	[BoxGroup("Expressions")]
	public bool setExpressionOnDeactivate;

	// Token: 0x04002FA2 RID: 12194
	[BoxGroup("Expressions")]
	[EnableIf("setExpressionOnDeactivate")]
	public CitizenOutfitController.Expression deactivateExpression;

	// Token: 0x04002FA3 RID: 12195
	[BoxGroup("Expressions")]
	[Space(5f)]
	public bool setExpressionOnComplete;

	// Token: 0x04002FA4 RID: 12196
	[EnableIf("setExpressionOnComplete")]
	[BoxGroup("Expressions")]
	public CitizenOutfitController.Expression completeExpression;

	// Token: 0x04002FA5 RID: 12197
	[Header("Items")]
	[Tooltip("Allow (any) items to be held during this action")]
	public bool allowItems = true;

	// Token: 0x04002FA6 RID: 12198
	[Tooltip("Allow a action-specific custom item to be held")]
	public bool enableCustomItem;

	// Token: 0x04002FA7 RID: 12199
	[Tooltip("Spawn this item in right hand")]
	[EnableIf("enableCustomItem")]
	public GameObject itemRight;

	// Token: 0x04002FA8 RID: 12200
	[EnableIf("enableCustomItem")]
	public Vector3 itemRightLocalPos;

	// Token: 0x04002FA9 RID: 12201
	[EnableIf("enableCustomItem")]
	public Vector3 itemRightLocalEuler;

	// Token: 0x04002FAA RID: 12202
	[Space(7f)]
	[Tooltip("Spawn this item in left hand")]
	[EnableIf("enableCustomItem")]
	public GameObject itemLeft;

	// Token: 0x04002FAB RID: 12203
	[EnableIf("enableCustomItem")]
	public Vector3 itemLeftLocalPos;

	// Token: 0x04002FAC RID: 12204
	[EnableIf("enableCustomItem")]
	public Vector3 itemLeftLocalEuler;

	// Token: 0x04002FAD RID: 12205
	[EnableIf("enableCustomItem")]
	public AIActionPreset.ActionStateFlag spawnCustomItemOn;

	// Token: 0x04002FAE RID: 12206
	[EnableIf("enableCustomItem")]
	public AIActionPreset.ActionStateFlag destroyCustomItemOn = AIActionPreset.ActionStateFlag.onGoalDeactivation;

	// Token: 0x04002FAF RID: 12207
	[Tooltip("Does this require a custom carrying animation?")]
	[EnableIf("enableCustomItem")]
	public bool requiresCarryAnimation;

	// Token: 0x04002FB0 RID: 12208
	[EnableIf("enableCustomItem")]
	public int overrideCarryAnimation = 1;

	// Token: 0x04002FB1 RID: 12209
	[Tooltip("Drop this item on the floor when this action ends")]
	[Space(7f)]
	public InteractablePreset dropItemOnEnd;

	// Token: 0x04002FB2 RID: 12210
	[Range(0f, 1f)]
	[Header("Speech")]
	public float chanceOfOnTrigger = 0.5f;

	// Token: 0x04002FB3 RID: 12211
	public List<SpeechController.Bark> onTriggerBark = new List<SpeechController.Bark>();

	// Token: 0x04002FB4 RID: 12212
	[Range(0f, 1f)]
	public float chanceOfWhileJourney = 0.5f;

	// Token: 0x04002FB5 RID: 12213
	public List<SpeechController.Bark> whileJourneyBark = new List<SpeechController.Bark>();

	// Token: 0x04002FB6 RID: 12214
	[Range(0f, 1f)]
	public float chanceOfOnArrival = 0.5f;

	// Token: 0x04002FB7 RID: 12215
	public List<SpeechController.Bark> onArrivalBark = new List<SpeechController.Bark>();

	// Token: 0x04002FB8 RID: 12216
	[Range(0f, 1f)]
	public float chanceOfWhileArrived = 0.5f;

	// Token: 0x04002FB9 RID: 12217
	public bool mustSeeOtherCitizen;

	// Token: 0x04002FBA RID: 12218
	public List<SpeechController.Bark> whileArrivedBark = new List<SpeechController.Bark>();

	// Token: 0x04002FBB RID: 12219
	[Range(0f, 1f)]
	public float chanceOfOnComplete = 0.5f;

	// Token: 0x04002FBC RID: 12220
	public List<SpeechController.Bark> onCompleteBark = new List<SpeechController.Bark>();

	// Token: 0x0200069C RID: 1692
	public enum ActionLocation
	{
		// Token: 0x04002FBE RID: 12222
		interactable,
		// Token: 0x04002FBF RID: 12223
		findNearest,
		// Token: 0x04002FC0 RID: 12224
		investigate,
		// Token: 0x04002FC1 RID: 12225
		nearbyInvestigate,
		// Token: 0x04002FC2 RID: 12226
		pause,
		// Token: 0x04002FC3 RID: 12227
		randomNodeWithinLocation,
		// Token: 0x04002FC4 RID: 12228
		flee,
		// Token: 0x04002FC5 RID: 12229
		interactableLOS,
		// Token: 0x04002FC6 RID: 12230
		meetOther,
		// Token: 0x04002FC7 RID: 12231
		NearbyStreetRandomNode,
		// Token: 0x04002FC8 RID: 12232
		putDownInteractable,
		// Token: 0x04002FC9 RID: 12233
		pickUpInteractable,
		// Token: 0x04002FCA RID: 12234
		randomNodeWithinHome,
		// Token: 0x04002FCB RID: 12235
		interactableSpawn,
		// Token: 0x04002FCC RID: 12236
		proximityToMusic,
		// Token: 0x04002FCD RID: 12237
		player,
		// Token: 0x04002FCE RID: 12238
		tailAndConfrontPlayer,
		// Token: 0x04002FCF RID: 12239
		sniperVantagePoint,
		// Token: 0x04002FD0 RID: 12240
		randomNodeWithinLocationPrioritiseWindows,
		// Token: 0x04002FD1 RID: 12241
		randomNodeWithinDen,
		// Token: 0x04002FD2 RID: 12242
		victimApartmentDoor
	}

	// Token: 0x0200069D RID: 1693
	public enum ActionFacingDirection
	{
		// Token: 0x04002FD4 RID: 12244
		towardsDestination,
		// Token: 0x04002FD5 RID: 12245
		awayFromDestination,
		// Token: 0x04002FD6 RID: 12246
		interactable,
		// Token: 0x04002FD7 RID: 12247
		InverseInteractable,
		// Token: 0x04002FD8 RID: 12248
		accessableDirection,
		// Token: 0x04002FD9 RID: 12249
		investigate,
		// Token: 0x04002FDA RID: 12250
		door,
		// Token: 0x04002FDB RID: 12251
		interactableSetting,
		// Token: 0x04002FDC RID: 12252
		none,
		// Token: 0x04002FDD RID: 12253
		inverseInteractableSetting,
		// Token: 0x04002FDE RID: 12254
		player,
		// Token: 0x04002FDF RID: 12255
		sniperVantagePoint,
		// Token: 0x04002FE0 RID: 12256
		victim,
		// Token: 0x04002FE1 RID: 12257
		awayFromSniperVantagePoint
	}

	// Token: 0x0200069E RID: 1694
	public enum ActionFinding
	{
		// Token: 0x04002FE3 RID: 12259
		doNothing,
		// Token: 0x04002FE4 RID: 12260
		findNearest,
		// Token: 0x04002FE5 RID: 12261
		removeAction,
		// Token: 0x04002FE6 RID: 12262
		removeGoal
	}

	// Token: 0x0200069F RID: 1695
	public enum ActionBusy
	{
		// Token: 0x04002FE8 RID: 12264
		findAlternate,
		// Token: 0x04002FE9 RID: 12265
		skipAction,
		// Token: 0x04002FEA RID: 12266
		skipGoal,
		// Token: 0x04002FEB RID: 12267
		standGuard,
		// Token: 0x04002FEC RID: 12268
		standGuardIfEnforcerSkipGoalNot
	}

	// Token: 0x020006A0 RID: 1696
	public enum FindSetting
	{
		// Token: 0x04002FEE RID: 12270
		nonTrespassing,
		// Token: 0x04002FEF RID: 12271
		onlyPublic,
		// Token: 0x04002FF0 RID: 12272
		allAreas,
		// Token: 0x04002FF1 RID: 12273
		homeOnly,
		// Token: 0x04002FF2 RID: 12274
		workOnly
	}

	// Token: 0x020006A1 RID: 1697
	[Serializable]
	public class AISpeechPreset
	{
		// Token: 0x04002FF3 RID: 12275
		public string dictionaryString;

		// Token: 0x04002FF4 RID: 12276
		public string ddsMessageID;

		// Token: 0x04002FF5 RID: 12277
		public bool isSuccessful;

		// Token: 0x04002FF6 RID: 12278
		[Range(0f, 10f)]
		public int chance = 1;

		// Token: 0x04002FF7 RID: 12279
		[Tooltip("Use parsing for special items in this string")]
		public bool useParsing;

		// Token: 0x04002FF8 RID: 12280
		public bool shout;

		// Token: 0x04002FF9 RID: 12281
		public bool interupt;

		// Token: 0x04002FFA RID: 12282
		public bool onlyIfEnfocerOnDuty;

		// Token: 0x04002FFB RID: 12283
		public bool onlyIfNotEnforcerOnDuty;

		// Token: 0x04002FFC RID: 12284
		[Tooltip("Must feature ANY of these character traits")]
		public List<CharacterTrait> mustFeatureTrait = new List<CharacterTrait>();

		// Token: 0x04002FFD RID: 12285
		[Tooltip("Can't feature ANY of these character traits")]
		public List<CharacterTrait> cantFeatureTrait = new List<CharacterTrait>();

		// Token: 0x04002FFE RID: 12286
		[Tooltip("Must be a killer and feature this motive")]
		public List<MurderMO> mustBeKillerWithMotive = new List<MurderMO>();

		// Token: 0x04002FFF RID: 12287
		[Tooltip("If true this will use the DDS reference from the killer's MO, if there is one. Make sure the dds message in this can be used as a fallback")]
		public bool useMurderMOConfession;

		// Token: 0x04003000 RID: 12288
		public List<Evidence.DataKey> tieKeys = new List<Evidence.DataKey>();

		// Token: 0x04003001 RID: 12289
		public List<Evidence.Discovery> applyDiscovery = new List<Evidence.Discovery>();

		// Token: 0x04003002 RID: 12290
		public bool endsDialog;

		// Token: 0x04003003 RID: 12291
		public bool jobHandIn;

		// Token: 0x04003004 RID: 12292
		public bool startCombat;

		// Token: 0x04003005 RID: 12293
		public bool flee;

		// Token: 0x04003006 RID: 12294
		public bool giveUpSelf;
	}

	// Token: 0x020006A2 RID: 1698
	[Serializable]
	public class AutomaticAction
	{
		// Token: 0x04003007 RID: 12295
		public AIActionPreset forcedAction;

		// Token: 0x04003008 RID: 12296
		public bool proximityCheck;

		// Token: 0x04003009 RID: 12297
		public float additionalDelay;
	}

	// Token: 0x020006A3 RID: 1699
	public enum SourceOfBannedRooms
	{
		// Token: 0x0400300B RID: 12299
		none,
		// Token: 0x0400300C RID: 12300
		jobPreset
	}

	// Token: 0x020006A4 RID: 1700
	public enum CombatPose
	{
		// Token: 0x0400300E RID: 12302
		noChange,
		// Token: 0x0400300F RID: 12303
		always,
		// Token: 0x04003010 RID: 12304
		never,
		// Token: 0x04003011 RID: 12305
		onlyWhenPreviouslyPersuing,
		// Token: 0x04003012 RID: 12306
		onlyWhenAtDestination
	}

	// Token: 0x020006A5 RID: 1701
	public enum ForcedActionsSearchLevel
	{
		// Token: 0x04003014 RID: 12308
		thisObjectOnly,
		// Token: 0x04003015 RID: 12309
		otherIntegratedInteractables,
		// Token: 0x04003016 RID: 12310
		spawnInteractablesChildren,
		// Token: 0x04003017 RID: 12311
		spawnedInteractablesAll,
		// Token: 0x04003018 RID: 12312
		InteractablesOnNode
	}

	// Token: 0x020006A6 RID: 1702
	[Serializable]
	public class CheckActionAgainstState
	{
		// Token: 0x04003019 RID: 12313
		public InteractablePreset.Switch switchState;

		// Token: 0x0400301A RID: 12314
		public bool switchIs;

		// Token: 0x0400301B RID: 12315
		public AIActionPreset.CheckActionOutcome outcome;
	}

	// Token: 0x020006A7 RID: 1703
	public enum CheckActionOutcome
	{
		// Token: 0x0400301D RID: 12317
		cancelAction,
		// Token: 0x0400301E RID: 12318
		cancelGoal
	}

	// Token: 0x020006A8 RID: 1704
	public enum DoorRule
	{
		// Token: 0x04003020 RID: 12320
		normal,
		// Token: 0x04003021 RID: 12321
		dontLock,
		// Token: 0x04003022 RID: 12322
		dontClose,
		// Token: 0x04003023 RID: 12323
		onlyCloseToLocation,
		// Token: 0x04003024 RID: 12324
		onlyLockToLocation
	}

	// Token: 0x020006A9 RID: 1705
	public enum LightRule
	{
		// Token: 0x04003026 RID: 12326
		normal,
		// Token: 0x04003027 RID: 12327
		dontSwitch,
		// Token: 0x04003028 RID: 12328
		onlyWhenArrived
	}

	// Token: 0x020006AA RID: 1706
	public enum ActionStateFlag
	{
		// Token: 0x0400302A RID: 12330
		onActivation,
		// Token: 0x0400302B RID: 12331
		onArrival,
		// Token: 0x0400302C RID: 12332
		onDeactivation,
		// Token: 0x0400302D RID: 12333
		onGoalDeactivation,
		// Token: 0x0400302E RID: 12334
		none
	}
}
