using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007CB RID: 1995
[CreateAssetMenu(fileName = "sidejobintro_data", menuName = "Database/Side Job Intro Preset")]
public class SideMissionIntroPreset : SoCustomComparison
{
	// Token: 0x04003D32 RID: 15666
	[Header("Rewards")]
	public int rewardModifier;

	// Token: 0x04003D33 RID: 15667
	[Header("Elements")]
	public List<SideMissionIntroPreset.SideMissionObjectiveBlock> blocks = new List<SideMissionIntroPreset.SideMissionObjectiveBlock>();

	// Token: 0x020007CC RID: 1996
	public enum SideMissionElementType
	{
		// Token: 0x04003D35 RID: 15669
		playerCallsNumber,
		// Token: 0x04003D36 RID: 15670
		acquireInformation,
		// Token: 0x04003D37 RID: 15671
		askStaff,
		// Token: 0x04003D38 RID: 15672
		spawnItems,
		// Token: 0x04003D39 RID: 15673
		photoOfItemLocation,
		// Token: 0x04003D3A RID: 15674
		openedBriefcase,
		// Token: 0x04003D3B RID: 15675
		postSubmission,
		// Token: 0x04003D3C RID: 15676
		playerHasCamera,
		// Token: 0x04003D3D RID: 15677
		setGooseChaseCall,
		// Token: 0x04003D3E RID: 15678
		setMeeting,
		// Token: 0x04003D3F RID: 15679
		handDossier,
		// Token: 0x04003D40 RID: 15680
		setupHomeInvestigation,
		// Token: 0x04003D41 RID: 15681
		submitToPoster,
		// Token: 0x04003D42 RID: 15682
		setHomeMeeting,
		// Token: 0x04003D43 RID: 15683
		setGooseChaseCallIndoorOnly,
		// Token: 0x04003D44 RID: 15684
		tailBriefcase,
		// Token: 0x04003D45 RID: 15685
		playerHasItemInPossession,
		// Token: 0x04003D46 RID: 15686
		leaveItemAtSecretLocation,
		// Token: 0x04003D47 RID: 15687
		destroyItem,
		// Token: 0x04003D48 RID: 15688
		playerHasHandcuffs,
		// Token: 0x04003D49 RID: 15689
		telephoneSubmission,
		// Token: 0x04003D4A RID: 15690
		placeItemInPosterMailbox,
		// Token: 0x04003D4B RID: 15691
		placeItemOfTypeInPosterMailbox
	}

	// Token: 0x020007CD RID: 1997
	[Serializable]
	public class SideMissionObjectiveBlock
	{
		// Token: 0x04003D4C RID: 15692
		public string name;

		// Token: 0x04003D4D RID: 15693
		public SideMissionIntroPreset.SideMissionElementType elementType;

		// Token: 0x04003D4E RID: 15694
		public string dialogReference;

		// Token: 0x04003D4F RID: 15695
		public JobPreset.JobTag tagReference;

		// Token: 0x04003D50 RID: 15696
		public List<JobPreset.StartingSpawnItem> spawnItems = new List<JobPreset.StartingSpawnItem>();

		// Token: 0x04003D51 RID: 15697
		public bool enableUpdateWhileTalking;

		// Token: 0x04003D52 RID: 15698
		public float objectiveDelay;

		// Token: 0x04003D53 RID: 15699
		public List<InteractablePreset> validItems = new List<InteractablePreset>();

		// Token: 0x04003D54 RID: 15700
		public List<FurniturePreset> validFurniture = new List<FurniturePreset>();

		// Token: 0x04003D55 RID: 15701
		public List<JobPreset.DifficultyTag> disableOnDifficulties = new List<JobPreset.DifficultyTag>();

		// Token: 0x04003D56 RID: 15702
		public List<SideMissionIntroPreset> onlyCompativleWithIntros = new List<SideMissionIntroPreset>();

		// Token: 0x04003D57 RID: 15703
		public List<SideMissionHandInPreset> onlyCompatibleWithHandIns = new List<SideMissionHandInPreset>();

		// Token: 0x04003D58 RID: 15704
		public List<JobPreset.JobTag> triggerFailIfItemDestroyed = new List<JobPreset.JobTag>();
	}
}
