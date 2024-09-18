using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006BE RID: 1726
[CreateAssetMenu(fileName = "audioevent_data", menuName = "Audio/Audio Event")]
public class AudioEvent : SoCustomComparison
{
	// Token: 0x060024F0 RID: 9456 RVA: 0x001E3050 File Offset: 0x001E1250
	private void OnGUIDValueChangedCallback()
	{
		string text = this.guid.Replace("{", "");
		text = text.Replace("}", "");
		this.guid = text;
		this.presetName = base.name;
		GUID guid = GUID.Parse(this.guid);
		EventDescription eventDescription;
		RuntimeManager.StudioSystem.getEventByID(guid, ref eventDescription);
		float num;
		eventDescription.getMinMaxDistance(ref num, ref this.actualSoundRange);
	}

	// Token: 0x04003109 RID: 12553
	[OnValueChanged("OnGUIDValueChangedCallback")]
	[Header("Setup")]
	[Tooltip("Paste the GUID from FMOD in here. Curly braces will be removed.")]
	public string guid;

	// Token: 0x0400310A RID: 12554
	[Tooltip("If enabled, this sound's occlusion will never be checked")]
	public bool disableOcclusion;

	// Token: 0x0400310B RID: 12555
	[Tooltip("If enabled, this will print out debug info in the console")]
	public bool debug;

	// Token: 0x0400310C RID: 12556
	[Tooltip("If enabled, this run the sound through the occlusion and AI checks without physically playing it")]
	public bool isDummyEvent;

	// Token: 0x0400310D RID: 12557
	[Tooltip("Is this a licensed music track?")]
	public bool isLicensed;

	// Token: 0x0400310E RID: 12558
	[Header("Disable")]
	[Tooltip("Disable this in-game completely.")]
	public bool disabled;

	// Token: 0x0400310F RID: 12559
	[Tooltip("If true this sound can penetrate walls")]
	[Header("Occlusion")]
	public bool canPenetrateWalls;

	// Token: 0x04003110 RID: 12560
	[Tooltip("If true this sound can penetrate floors")]
	public bool canPenetrateFloors;

	// Token: 0x04003111 RID: 12561
	[Tooltip("If true this sound can penetrate cielings")]
	public bool canPenetrateCeilings;

	// Token: 0x04003112 RID: 12562
	[Space(7f)]
	public bool overrideMaximumLoops;

	// Token: 0x04003113 RID: 12563
	[InfoBox("If more than one of wall/floor/ceiling penetration is enabled, it is recommended to increase the maximum possible loops for this sound to reach the listener properly", 0)]
	[EnableIf("overrideMaximumLoops")]
	public int overriddenMaxLoops = 80;

	// Token: 0x04003114 RID: 12564
	[Space(7f)]
	[Tooltip("Overrides default occlusion value sound in the audio controller")]
	public bool overrideOcclusionModifier;

	// Token: 0x04003115 RID: 12565
	[InfoBox("If overridden, make sure this is set to below 0, otherwise it may cause occlusion evalulation problems.", 0)]
	[EnableIf("overrideOcclusionModifier")]
	[Tooltip("Each occlusion unit will decrease volume by this amount...")]
	public float occlusionUnitVolumeModifier = -0.1f;

	// Token: 0x04003116 RID: 12566
	[Space(7f)]
	public bool overrideOpenDoorOcclusion;

	// Token: 0x04003117 RID: 12567
	[EnableIf("overrideOpenDoorOcclusion")]
	[Range(0f, 10f)]
	public int openDoorOcclusionUnits = 1;

	// Token: 0x04003118 RID: 12568
	public bool overrideClosedDoorOcclusion;

	// Token: 0x04003119 RID: 12569
	[EnableIf("overrideClosedDoorOcclusion")]
	[Range(0f, 10f)]
	public int closedDoorOcclusionUnits = 5;

	// Token: 0x0400311A RID: 12570
	public bool overrideWindowOcclusion;

	// Token: 0x0400311B RID: 12571
	[Range(0f, 10f)]
	[EnableIf("overrideWindowOcclusion")]
	public int windowOcclusionUnits = 4;

	// Token: 0x0400311C RID: 12572
	public bool overrideWallOcclusion;

	// Token: 0x0400311D RID: 12573
	[EnableIf("overrideWallOcclusion")]
	[Range(0f, 10f)]
	public int wallOcclusionUnits = 7;

	// Token: 0x0400311E RID: 12574
	public bool overrideCeilingOcclusion;

	// Token: 0x0400311F RID: 12575
	[Range(0f, 10f)]
	[EnableIf("overrideCeilingOcclusion")]
	public int ceilingOcclusionUnits = 8;

	// Token: 0x04003120 RID: 12576
	public bool overrideFloorOcclusion;

	// Token: 0x04003121 RID: 12577
	[EnableIf("overrideFloorOcclusion")]
	[Range(0f, 10f)]
	public int floorOcclusionUnits = 8;

	// Token: 0x04003122 RID: 12578
	[Tooltip("On updated occlusion, force the volume to change level at a specific rate (seconds)")]
	[Space(7f)]
	public bool forceVolumeLevelFadeTime;

	// Token: 0x04003123 RID: 12579
	[EnableIf("forceVolumeLevelFadeTime")]
	public float volumeLevelFadeTime = 0.5f;

	// Token: 0x04003124 RID: 12580
	[Header("Suspicion")]
	[Tooltip("Can this audio trigger an AI reaction?")]
	public bool canBeSuspicious;

	// Token: 0x04003125 RID: 12581
	[Tooltip("This event will by default trigger an AI reaction")]
	[EnableIf("canBeSuspicious")]
	public bool alwaysSuspicious;

	// Token: 0x04003126 RID: 12582
	[Tooltip("This event will only trigger an AI reaction if 1) The actor is trespassing and 2) The listener is allowed to go there without trespassing.")]
	[EnableIf("canBeSuspicious")]
	public bool suspiciousIfTresspassing;

	// Token: 0x04003127 RID: 12583
	[Tooltip("This event will only trigger an AI reaction if the address is empty apart from the player and the following number of people")]
	[EnableIf("canBeSuspicious")]
	public bool onlySuspiciousIfEmptyAddress;

	// Token: 0x04003128 RID: 12584
	[Tooltip("Only suspicious if not caused by an enforcer")]
	[EnableIf("canBeSuspicious")]
	public bool onlySuspiciousIfNotEnforcer = true;

	// Token: 0x04003129 RID: 12585
	[EnableIf("canBeSuspicious")]
	public int suspiciousIfCitizenCount = 2;

	// Token: 0x0400312A RID: 12586
	[Tooltip("If this event is urgency (eg gunshot), AI will immediately run for investiation")]
	public bool urgentResponse;

	// Token: 0x0400312B RID: 12587
	[Tooltip("AI hearing this event triggers this amount of audio focus. 1 Will mean they investigate immediately, any less will be added within their focus window...")]
	[Range(0f, 1f)]
	public float audioFocus = 1f;

	// Token: 0x0400312C RID: 12588
	[EnableIf("canBeSuspicious")]
	[Space(5f)]
	[Tooltip("Force this to display a red outline if this loop is heard by the player")]
	public bool forceOutlineForLoopIfPlayerTrespassing;

	// Token: 0x0400312D RID: 12589
	public AudioEvent.MemoryTag citizenMemoryTag;

	// Token: 0x0400312E RID: 12590
	[Tooltip("Will citizens get scared by this?")]
	[Space(7f)]
	public float spookValue;

	// Token: 0x0400312F RID: 12591
	[Tooltip("Enforcers do not get scared by this")]
	public bool noSpookIfEnforcer = true;

	// Token: 0x04003130 RID: 12592
	[Header("AI Hearing and Emulation")]
	[Tooltip("If the AI is asleep, how likely is it that they awaken upon hearing this? (Only works in conjunction with suspicious sounds)")]
	[Range(0f, 1f)]
	public float awakenChance = 0.5f;

	// Token: 0x04003131 RID: 12593
	[ReadOnly]
	[Tooltip("The actual sound range set in FMOD.")]
	public float actualSoundRange;

	// Token: 0x04003132 RID: 12594
	[Tooltip("The effective sound range that the AI can hear (applied to actual sound range above)")]
	public float hearingRange = 20f;

	// Token: 0x04003133 RID: 12595
	[Tooltip("If in stealth mode, also apply the following modifier to the hearing range...")]
	public float stealthModeModifier;

	// Token: 0x04003134 RID: 12596
	[Tooltip("If running apply the following modifier to the hearing range...")]
	public float runModifier;

	// Token: 0x04003135 RID: 12597
	[Tooltip("Can the AI dance to this?")]
	public bool canDanceTo;

	// Token: 0x04003136 RID: 12598
	[Range(0f, 1f)]
	[Header("Overrides")]
	public float masterVolumeScale = 1f;

	// Token: 0x04003137 RID: 12599
	[Tooltip("If true then the AI 'sound level' above is overridden based on surface type. Important: Only works for footsteps!")]
	public bool modifyBasedOnSurface;

	// Token: 0x04003138 RID: 12600
	[EnableIf("modifyBasedOnSurface")]
	public float concreteHearingRangeModifier;

	// Token: 0x04003139 RID: 12601
	[EnableIf("modifyBasedOnSurface")]
	public float woodHearingRangeModifier;

	// Token: 0x0400313A RID: 12602
	[EnableIf("modifyBasedOnSurface")]
	public float carpetHearingRangeModifier;

	// Token: 0x0400313B RID: 12603
	[EnableIf("modifyBasedOnSurface")]
	public float tileHearingRangeModifier;

	// Token: 0x0400313C RID: 12604
	[EnableIf("modifyBasedOnSurface")]
	public float plasterHearingRangeModifier;

	// Token: 0x0400313D RID: 12605
	[EnableIf("modifyBasedOnSurface")]
	public float fabricHearingRangeModifier;

	// Token: 0x0400313E RID: 12606
	[EnableIf("modifyBasedOnSurface")]
	public float metalHearingRangeModifier;

	// Token: 0x0400313F RID: 12607
	[EnableIf("modifyBasedOnSurface")]
	public float glassHearingRangeModifier;

	// Token: 0x020006BF RID: 1727
	public enum MemoryTag
	{
		// Token: 0x04003141 RID: 12609
		none,
		// Token: 0x04003142 RID: 12610
		gunshot,
		// Token: 0x04003143 RID: 12611
		scream
	}
}
