using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007D1 RID: 2001
[CreateAssetMenu(fileName = "status_data", menuName = "Database/Status Preset")]
public class StatusPreset : SoCustomComparison
{
	// Token: 0x04003D6C RID: 15724
	[Header("Interface")]
	public Color color = Color.white;

	// Token: 0x04003D6D RID: 15725
	public Color alternateColour = Color.white;

	// Token: 0x04003D6E RID: 15726
	public Sprite icon;

	// Token: 0x04003D6F RID: 15727
	public Sprite alternateIcon;

	// Token: 0x04003D70 RID: 15728
	[Tooltip("After creation, minimize this to an icon for the in-game UI")]
	public bool minimizeToIcon = true;

	// Token: 0x04003D71 RID: 15729
	[Tooltip("Glow when active")]
	public bool pulseBackground;

	// Token: 0x04003D72 RID: 15730
	public bool pulseIcon = true;

	// Token: 0x04003D73 RID: 15731
	public Color pulseIconAdditiveColour = Color.clear;

	// Token: 0x04003D74 RID: 15732
	[Tooltip("Include a description on the detail text")]
	public bool includeDescription = true;

	// Token: 0x04003D75 RID: 15733
	[Tooltip("Instancing rules for this")]
	public StatusPreset.Grouping instancingType;

	// Token: 0x04003D76 RID: 15734
	[Space(7f)]
	[Tooltip("Automatically display a message when this is activated")]
	public bool autoNotificationMessage;

	// Token: 0x04003D77 RID: 15735
	[Range(0f, 10f)]
	[Tooltip("Where this appears on the right side menu hierarchy")]
	public int priority = 4;

	// Token: 0x04003D78 RID: 15736
	[Tooltip("Fade to white the closer the amount is to 0")]
	[Header("Progress")]
	public bool fadeToWhite;

	// Token: 0x04003D79 RID: 15737
	[Tooltip("Use progress bar")]
	public bool enableProgressBar = true;

	// Token: 0x04003D7A RID: 15738
	[EnableIf("enableProgressBar")]
	public StatusPreset.ProgressBarTrack barTracking;

	// Token: 0x04003D7B RID: 15739
	[Header("Checking")]
	[Tooltip("Use the custom named method to check the status of this")]
	public bool useCustomMethod;

	// Token: 0x04003D7C RID: 15740
	[Header("Audio")]
	public AudioEvent onAcquire;

	// Token: 0x04003D7D RID: 15741
	public AudioEvent onRemove;

	// Token: 0x04003D7E RID: 15742
	[Header("Counts")]
	public StatusPreset.StatusCountType countType;

	// Token: 0x04003D7F RID: 15743
	[Tooltip("Override the base colour with the highest count's colour")]
	public bool overrrideColorWithCount;

	// Token: 0x04003D80 RID: 15744
	[Tooltip("Display the count number in the main text")]
	public bool displayCountCountsInMainText = true;

	// Token: 0x04003D81 RID: 15745
	[Tooltip("Replace description based on counts")]
	public bool replaceDescriptionBasedOnCounts;

	// Token: 0x04003D82 RID: 15746
	[Tooltip("Display the address at the end of the detail text")]
	public bool displayAddressInDetailText;

	// Token: 0x04003D83 RID: 15747
	[Tooltip("Display the building at the end of the detail text")]
	public bool displayBuildingInDetailText;

	// Token: 0x04003D84 RID: 15748
	[Tooltip("List counts in detail text")]
	public bool listCountsInDetailText;

	// Token: 0x04003D85 RID: 15749
	[Tooltip("Display the fine total in the main text")]
	public bool displayFineTotalInMainText;

	// Token: 0x04003D86 RID: 15750
	[Tooltip("Alert when new count is added")]
	public bool alertWhenNewCountIsAdded;

	// Token: 0x04003D87 RID: 15751
	[Tooltip("Display total fine when minimized")]
	public bool displayTotalFineWhenMinimized;

	// Token: 0x04003D88 RID: 15752
	[ReorderableList]
	public List<StatusPreset.StatusCountConfig> countConfig = new List<StatusPreset.StatusCountConfig>();

	// Token: 0x04003D89 RID: 15753
	[Header("Attribute Effects (Binary)")]
	public bool stopsRecovery;

	// Token: 0x04003D8A RID: 15754
	public bool stopsSprint;

	// Token: 0x04003D8B RID: 15755
	public bool stopsJump;

	// Token: 0x04003D8C RID: 15756
	[Space(7f)]
	[Header("Attribute Effects (Gradual)")]
	public float recoveryRatePlusMP;

	// Token: 0x04003D8D RID: 15757
	public float maxHealthPlusMP;

	// Token: 0x04003D8E RID: 15758
	public float movementSpeedPlusMP;

	// Token: 0x04003D8F RID: 15759
	public float temperatureGainPlusMP;

	// Token: 0x04003D90 RID: 15760
	public float damageIncomingPlusMP;

	// Token: 0x04003D91 RID: 15761
	public float damageOutgoingPlusMP;

	// Token: 0x04003D92 RID: 15762
	[Space(7f)]
	public float drunkControls;

	// Token: 0x04003D93 RID: 15763
	public float tripChanceWet;

	// Token: 0x04003D94 RID: 15764
	public float tripChanceDrunk;

	// Token: 0x04003D95 RID: 15765
	public float affectHeadBob;

	// Token: 0x04003D96 RID: 15766
	public AnimationCurve headBob;

	// Token: 0x04003D97 RID: 15767
	[Space(7f)]
	public float drunkVision;

	// Token: 0x04003D98 RID: 15768
	public float shiverVision;

	// Token: 0x04003D99 RID: 15769
	public float drunkLensDistort;

	// Token: 0x04003D9A RID: 15770
	public float headacheVision;

	// Token: 0x04003D9B RID: 15771
	[Space(7f)]
	public float bloomIntensityPlusMP;

	// Token: 0x04003D9C RID: 15772
	public float motionBlurPlusMP;

	// Token: 0x04003D9D RID: 15773
	public float chromaticAbberationAmount;

	// Token: 0x04003D9E RID: 15774
	public float vignetteAmount;

	// Token: 0x04003D9F RID: 15775
	public float expsosure;

	// Token: 0x04003DA0 RID: 15776
	[Space(7f)]
	public bool useChannelMixer;

	// Token: 0x04003DA1 RID: 15777
	[Space(7f)]
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int redR;

	// Token: 0x04003DA2 RID: 15778
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int redG;

	// Token: 0x04003DA3 RID: 15779
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int redB;

	// Token: 0x04003DA4 RID: 15780
	[Range(-200f, 200f)]
	[Space(7f)]
	[ShowIf("useChannelMixer")]
	public int greenR;

	// Token: 0x04003DA5 RID: 15781
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int greenG;

	// Token: 0x04003DA6 RID: 15782
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int greenB;

	// Token: 0x04003DA7 RID: 15783
	[Space(7f)]
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int blueR;

	// Token: 0x04003DA8 RID: 15784
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int blueG;

	// Token: 0x04003DA9 RID: 15785
	[ShowIf("useChannelMixer")]
	[Range(-200f, 200f)]
	public int blueB;

	// Token: 0x020007D2 RID: 2002
	public enum Grouping
	{
		// Token: 0x04003DAB RID: 15787
		perPresetType,
		// Token: 0x04003DAC RID: 15788
		perCrimeLocation,
		// Token: 0x04003DAD RID: 15789
		perGuestPass
	}

	// Token: 0x020007D3 RID: 2003
	public enum ProgressBarTrack
	{
		// Token: 0x04003DAF RID: 15791
		none,
		// Token: 0x04003DB0 RID: 15792
		witnesses,
		// Token: 0x04003DB1 RID: 15793
		wantedInBuilding,
		// Token: 0x04003DB2 RID: 15794
		alarmTime,
		// Token: 0x04003DB3 RID: 15795
		guestPassTime
	}

	// Token: 0x020007D4 RID: 2004
	public enum StatusCountType
	{
		// Token: 0x04003DB5 RID: 15797
		none,
		// Token: 0x04003DB6 RID: 15798
		crime
	}

	// Token: 0x020007D5 RID: 2005
	[Serializable]
	public class StatusCountConfig
	{
		// Token: 0x04003DB7 RID: 15799
		public string name;

		// Token: 0x04003DB8 RID: 15800
		public Sprite icon;

		// Token: 0x04003DB9 RID: 15801
		public Color colour;

		// Token: 0x04003DBA RID: 15802
		public StatusPreset.PenaltyRule penaltyRule;

		// Token: 0x04003DBB RID: 15803
		public float penalty;

		// Token: 0x04003DBC RID: 15804
		public AudioEvent onAcquire;
	}

	// Token: 0x020007D6 RID: 2006
	public enum PenaltyRule
	{
		// Token: 0x04003DBE RID: 15806
		fixedValue,
		// Token: 0x04003DBF RID: 15807
		percentageValue,
		// Token: 0x04003DC0 RID: 15808
		objectValueMultiplied
	}
}
