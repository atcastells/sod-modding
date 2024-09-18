using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000798 RID: 1944
[CreateAssetMenu(fileName = "musiccue_data", menuName = "Audio/Music Cue")]
public class MusicCue : SoCustomComparison
{
	// Token: 0x04003AD6 RID: 15062
	[Header("Setup")]
	public string fmodGUID;

	// Token: 0x04003AD7 RID: 15063
	public bool disabled;

	// Token: 0x04003AD8 RID: 15064
	public bool debug;

	// Token: 0x04003AD9 RID: 15065
	[Header("Track Settings")]
	[Tooltip("If true, only play this track once per game")]
	public bool playOnce;

	// Token: 0x04003ADA RID: 15066
	[Tooltip("If true then when it's appropraite to play this ambient, it will stop a previously playing one.")]
	public bool interrupt;

	// Token: 0x04003ADB RID: 15067
	[Tooltip("If true then this track will stop when the ambient state is switched to something that's not compatible")]
	public bool stopOnIncompatibleStateSwitch;

	// Token: 0x04003ADC RID: 15068
	[Tooltip("If true this track will avoid repetition as much as possible by playing other tracks of the same priority when available")]
	public bool avoidRepetition = true;

	// Token: 0x04003ADD RID: 15069
	[Range(0f, 4f)]
	[Tooltip("The game will choose between the highest available priority tracks. Higher plays first.")]
	public int ambientPriority = 2;

	// Token: 0x04003ADE RID: 15070
	public List<MusicCue.MusicTrigger> triggers = new List<MusicCue.MusicTrigger>();

	// Token: 0x02000799 RID: 1945
	public enum MusicTriggerGameState
	{
		// Token: 0x04003AE0 RID: 15072
		any,
		// Token: 0x04003AE1 RID: 15073
		menu,
		// Token: 0x04003AE2 RID: 15074
		inGame,
		// Token: 0x04003AE3 RID: 15075
		inCutscene
	}

	// Token: 0x0200079A RID: 1946
	public enum MusicTriggerPlayerState
	{
		// Token: 0x04003AE5 RID: 15077
		any,
		// Token: 0x04003AE6 RID: 15078
		safe,
		// Token: 0x04003AE7 RID: 15079
		trespass,
		// Token: 0x04003AE8 RID: 15080
		combat,
		// Token: 0x04003AE9 RID: 15081
		passingTime
	}

	// Token: 0x0200079B RID: 1947
	public enum MusicTriggerPlayerLocation
	{
		// Token: 0x04003AEB RID: 15083
		any,
		// Token: 0x04003AEC RID: 15084
		outdoors,
		// Token: 0x04003AED RID: 15085
		indoors,
		// Token: 0x04003AEE RID: 15086
		playersApartment
	}

	// Token: 0x0200079C RID: 1948
	public enum MusicTriggerEvent
	{
		// Token: 0x04003AF0 RID: 15088
		none,
		// Token: 0x04003AF1 RID: 15089
		newMurderCase,
		// Token: 0x04003AF2 RID: 15090
		caseComplete,
		// Token: 0x04003AF3 RID: 15091
		caseFailed,
		// Token: 0x04003AF4 RID: 15092
		caseUnsolved,
		// Token: 0x04003AF5 RID: 15093
		socialCreditLevelUp,
		// Token: 0x04003AF6 RID: 15094
		resolveScreen,
		// Token: 0x04003AF7 RID: 15095
		arriveAtCrimeScene,
		// Token: 0x04003AF8 RID: 15096
		passingTime
	}

	// Token: 0x0200079D RID: 1949
	[Serializable]
	public class MusicTrigger
	{
		// Token: 0x04003AF9 RID: 15097
		public MusicCue.MusicTriggerGameState onGameState;

		// Token: 0x04003AFA RID: 15098
		public MusicCue.MusicTriggerPlayerState onPlayerSate;

		// Token: 0x04003AFB RID: 15099
		public MusicCue.MusicTriggerPlayerLocation onPlayerLocation;

		// Token: 0x04003AFC RID: 15100
		[Space(7f)]
		public MusicCue.MusicTriggerEvent onEvent;

		// Token: 0x04003AFD RID: 15101
		[Range(0f, 1f)]
		public float eventTriggerChance = 1f;

		// Token: 0x04003AFE RID: 15102
		public bool triggerOnlyOnEvents;

		// Token: 0x04003AFF RID: 15103
		[Space(7f)]
		[Tooltip("If true this will be triggered regardless of the time between tracks")]
		public bool ignoreSilentTimeBetweenTracks;

		// Token: 0x04003B00 RID: 15104
		[Space(7f)]
		public bool onlyInDistricts;

		// Token: 0x04003B01 RID: 15105
		[EnableIf("onlyInDistricts")]
		public List<DistrictPreset> compatibleDistricts = new List<DistrictPreset>();

		// Token: 0x04003B02 RID: 15106
		public bool excludeDistricts;

		// Token: 0x04003B03 RID: 15107
		[EnableIf("excludeDistricts")]
		public List<DistrictPreset> excludedDistricts = new List<DistrictPreset>();

		// Token: 0x04003B04 RID: 15108
		[Space(7f)]
		public bool onlyInBuildings;

		// Token: 0x04003B05 RID: 15109
		[EnableIf("onlyInBuildings")]
		public List<BuildingPreset> compatibleBuildings = new List<BuildingPreset>();

		// Token: 0x04003B06 RID: 15110
		public bool excludeBuildings;

		// Token: 0x04003B07 RID: 15111
		[EnableIf("excludeBuildings")]
		public List<BuildingPreset> excludedBuildings = new List<BuildingPreset>();

		// Token: 0x04003B08 RID: 15112
		[Space(7f)]
		public bool onlyInLocations;

		// Token: 0x04003B09 RID: 15113
		[EnableIf("onlyInLocations")]
		public List<AddressPreset> compatibleAddressTypes = new List<AddressPreset>();

		// Token: 0x04003B0A RID: 15114
		public bool excludeLocations;

		// Token: 0x04003B0B RID: 15115
		[EnableIf("excludeLocations")]
		public List<AddressPreset> excludedAddressTypes = new List<AddressPreset>();

		// Token: 0x04003B0C RID: 15116
		[Space(7f)]
		public bool onlyDuringStatuses;

		// Token: 0x04003B0D RID: 15117
		[EnableIf("onlyDuringStatuses")]
		public List<StatusPreset> compatibleStatuses = new List<StatusPreset>();

		// Token: 0x04003B0E RID: 15118
		public bool excludeStatuses;

		// Token: 0x04003B0F RID: 15119
		[EnableIf("excludeStatuses")]
		public List<StatusPreset> excludedStatuses = new List<StatusPreset>();

		// Token: 0x04003B10 RID: 15120
		[Space(7f)]
		public bool useDecorGrimeRange;

		// Token: 0x04003B11 RID: 15121
		[MinMaxSlider(0f, 1f)]
		public Vector2 grimeRange = new Vector2(0f, 1f);

		// Token: 0x04003B12 RID: 15122
		[InfoBox("Play on these floor ranges, if empty then it will play on any floor", 0)]
		[Space(7f)]
		public List<Vector2> floorRanges = new List<Vector2>();

		// Token: 0x04003B13 RID: 15123
		[InfoBox("Play at this time (24hr clock so 0 = midnight, 12 = mid day etc). If empty then it will play at any time", 0)]
		public List<Vector2> timeRanges = new List<Vector2>();
	}
}
