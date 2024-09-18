using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200073F RID: 1855
[CreateAssetMenu(fileName = "group_data", menuName = "Database/Group Preset")]
public class GroupPreset : SoCustomComparison
{
	// Token: 0x04003680 RID: 13952
	[Header("Setup")]
	public GroupPreset.GroupType groupType;

	// Token: 0x04003681 RID: 13953
	[Tooltip("Chance of existance on a per instance basis.")]
	[Range(0f, 1f)]
	public float chance = 1f;

	// Token: 0x04003682 RID: 13954
	[Tooltip("Minimum members")]
	public int minMembers = 2;

	// Token: 0x04003683 RID: 13955
	[Tooltip("Maximum members")]
	public int maxMembers = 2;

	// Token: 0x04003684 RID: 13956
	[Header("Requirements")]
	[Tooltip("Members must have these traits")]
	public List<CharacterTrait> requiredTraits = new List<CharacterTrait>();

	// Token: 0x04003685 RID: 13957
	[Tooltip("Members must have this extraversion value")]
	[Range(0f, 1f)]
	public float minimumExtraversion;

	// Token: 0x04003686 RID: 13958
	[Header("Meet Ups")]
	public bool enableMeetUps = true;

	// Token: 0x04003687 RID: 13959
	[Tooltip("How many times a week this group meets")]
	[EnableIf("enableMeetUps")]
	public int daysPerWeek = 2;

	// Token: 0x04003688 RID: 13960
	[Tooltip("The time range for the meet up time. If set to something other that special interest, this is driven by when both are free (after work etc).")]
	[EnableIf("enableMeetUps")]
	public Vector2 timeRange = new Vector2(18f, 20.5f);

	// Token: 0x04003689 RID: 13961
	[Tooltip("Meet up length")]
	public float meetUpLength = 1.5f;

	// Token: 0x0400368A RID: 13962
	[Tooltip("Possible meeting place address types")]
	[EnableIf("enableMeetUps")]
	public List<CompanyPreset> meetUpLocations = new List<CompanyPreset>();

	// Token: 0x0400368B RID: 13963
	[Tooltip("Meet up goal")]
	[EnableIf("enableMeetUps")]
	public AIGoalPreset meetUpGoal;

	// Token: 0x0400368C RID: 13964
	[Tooltip("The first person will reserve up to 4 seats on arrival...")]
	[EnableIf("enableMeetUps")]
	public bool reserveSeats = true;

	// Token: 0x0400368D RID: 13965
	[Tooltip("Add this distance multiplier when choosing a seat")]
	public float useDistanceMultiplierModifier;

	// Token: 0x0400368E RID: 13966
	[ReorderableList]
	[Header("Evidence")]
	public List<GroupPreset.ClubClue> clues = new List<GroupPreset.ClubClue>();

	// Token: 0x0400368F RID: 13967
	[ReorderableList]
	[Header("Vmails")]
	public List<GroupPreset.MeetUpVmailThread> vmails = new List<GroupPreset.MeetUpVmailThread>();

	// Token: 0x02000740 RID: 1856
	public enum GroupType
	{
		// Token: 0x04003691 RID: 13969
		interestGroup,
		// Token: 0x04003692 RID: 13970
		couples,
		// Token: 0x04003693 RID: 13971
		cheaters,
		// Token: 0x04003694 RID: 13972
		work
	}

	// Token: 0x02000741 RID: 1857
	[Serializable]
	public class MeetUpVmailThread
	{
		// Token: 0x04003695 RID: 13973
		public string name;

		// Token: 0x04003696 RID: 13974
		public string treeID;

		// Token: 0x04003697 RID: 13975
		public GroupPreset.MeetUpVmailSender sender;

		// Token: 0x04003698 RID: 13976
		public GroupPreset.MeetUpVmailSender recevier;
	}

	// Token: 0x02000742 RID: 1858
	[Serializable]
	public class ClubClue
	{
		// Token: 0x04003699 RID: 13977
		public string name;

		// Token: 0x0400369A RID: 13978
		public InteractablePreset preset;

		// Token: 0x0400369B RID: 13979
		public GroupPreset.SpawnAt spawnAt;
	}

	// Token: 0x02000743 RID: 1859
	public enum SpawnAt
	{
		// Token: 0x0400369D RID: 13981
		meetingPlace,
		// Token: 0x0400369E RID: 13982
		leadersApartment,
		// Token: 0x0400369F RID: 13983
		entireGroupsApartments
	}

	// Token: 0x02000744 RID: 1860
	public enum MeetUpVmailSender
	{
		// Token: 0x040036A1 RID: 13985
		groupLeader,
		// Token: 0x040036A2 RID: 13986
		groupRandom,
		// Token: 0x040036A3 RID: 13987
		meetupPlace,
		// Token: 0x040036A4 RID: 13988
		entireGroup,
		// Token: 0x040036A5 RID: 13989
		prioritiseFaithful
	}
}
