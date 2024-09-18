using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000789 RID: 1929
[CreateAssetMenu(fileName = "murder_data", menuName = "Database/Murder Preset")]
public class MurderPreset : SoCustomComparison
{
	// Token: 0x060025A2 RID: 9634 RVA: 0x001E75E9 File Offset: 0x001E57E9
	[Button(null, 0)]
	public void CopyLeads()
	{
		if (this.copyFrom != null)
		{
			this.leads = new List<MurderPreset.MurderLeadItem>(this.copyFrom.leads);
		}
	}

	// Token: 0x04003A24 RID: 14884
	[Header("Basic Settings")]
	public MurderPreset.CaseType caseType;

	// Token: 0x04003A25 RID: 14885
	[Header("Preset Picking")]
	public bool disabled;

	// Token: 0x04003A26 RID: 14886
	[Tooltip("How often this is picked compared to others...")]
	[Range(0f, 10f)]
	public int frequency = 3;

	// Token: 0x04003A27 RID: 14887
	[Header("Murderer Picking")]
	public Vector2 murdererRandomScoreRange = new Vector2(0f, 1f);

	// Token: 0x04003A28 RID: 14888
	public List<MurderPreset.MurdererModifierRule> murdererTraitModifiers = new List<MurderPreset.MurdererModifierRule>();

	// Token: 0x04003A29 RID: 14889
	public bool useHexaco;

	// Token: 0x04003A2A RID: 14890
	[ShowIf("useHexaco")]
	public HEXACO hexaco;

	// Token: 0x04003A2B RID: 14891
	[Header("Other")]
	[Tooltip("Pick a den")]
	public bool pickDen;

	// Token: 0x04003A2C RID: 14892
	public float kidnapperTimeUntilKill = 5f;

	// Token: 0x04003A2D RID: 14893
	public float minimumTimeBetweenMurders = 5f;

	// Token: 0x04003A2E RID: 14894
	[Tooltip("When not at home, how many occupants are allowed here at maximum for the murder to trigger at this location")]
	[Space(5f)]
	public int nonHomeMaximumOccupantsTrigger = 2;

	// Token: 0x04003A2F RID: 14895
	[Tooltip("When not at home, how many occupants are allowed here at maximum for the triggered murder to be cancelled")]
	public int nonHomeMaximumOccupantsCancel = 4;

	// Token: 0x04003A30 RID: 14896
	[Header("Phase 1: Acquire Murder Weapon/Ammo")]
	public bool requiresAcquirePhase = true;

	// Token: 0x04003A31 RID: 14897
	public bool acquirePassInteractable;

	// Token: 0x04003A32 RID: 14898
	public bool acquirePassRoom = true;

	// Token: 0x04003A33 RID: 14899
	public List<AIGoalPreset.GoalActionSetup> acquireActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A34 RID: 14900
	[Tooltip("Does this require a research state?")]
	[Header("Phase 2: Research")]
	public bool requiresResearchPhase;

	// Token: 0x04003A35 RID: 14901
	[EnableIf("requiresResearchPhase")]
	public bool researchPassInteractable = true;

	// Token: 0x04003A36 RID: 14902
	[EnableIf("requiresResearchPhase")]
	public bool researchPassRoom = true;

	// Token: 0x04003A37 RID: 14903
	[EnableIf("requiresResearchPhase")]
	public List<AIGoalPreset.GoalActionSetup> researchActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A38 RID: 14904
	[Tooltip("Once the murderer has started travelling, block the victim from leaving their location...")]
	[Header("Phase 3: Travel To")]
	public bool blockVictimFromLeavingLocation = true;

	// Token: 0x04003A39 RID: 14905
	public MurderPreset.SuccessfulTravelTrigger travelSuccessTrigger;

	// Token: 0x04003A3A RID: 14906
	public bool travelPassInteractable = true;

	// Token: 0x04003A3B RID: 14907
	public bool travelPassRoom = true;

	// Token: 0x04003A3C RID: 14908
	public List<AIGoalPreset.GoalActionSetup> travelActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A3D RID: 14909
	[Header("Phase 4: Execution")]
	public bool executePassInteractable = true;

	// Token: 0x04003A3E RID: 14910
	public bool executePassRoom = true;

	// Token: 0x04003A3F RID: 14911
	public List<AIGoalPreset.GoalActionSetup> executionActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A40 RID: 14912
	[Header("Phase 5: Post")]
	public bool postPassInteractable = true;

	// Token: 0x04003A41 RID: 14913
	public bool postPassRoom = true;

	// Token: 0x04003A42 RID: 14914
	public List<AIGoalPreset.GoalActionSetup> postActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A43 RID: 14915
	[Header("Phase 6: Escape")]
	public bool escapePassInteractable = true;

	// Token: 0x04003A44 RID: 14916
	public bool escapePassRoom = true;

	// Token: 0x04003A45 RID: 14917
	public List<AIGoalPreset.GoalActionSetup> escapeActionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003A46 RID: 14918
	[Header("Leads")]
	public List<MurderPreset.MurderLeadItem> leads = new List<MurderPreset.MurderLeadItem>();

	// Token: 0x04003A47 RID: 14919
	[Header("Resolve")]
	[Tooltip("If true the case will use custom resolve questions")]
	public bool useCustomResolveQuestions;

	// Token: 0x04003A48 RID: 14920
	[EnableIf("useCustomResolveQuestions")]
	public List<Case.ResolveQuestion> customResolveQuestions = new List<Case.ResolveQuestion>();

	// Token: 0x04003A49 RID: 14921
	[Header("Debug")]
	public MurderPreset copyFrom;

	// Token: 0x0200078A RID: 1930
	public enum CaseType
	{
		// Token: 0x04003A4B RID: 14923
		murder,
		// Token: 0x04003A4C RID: 14924
		sniper,
		// Token: 0x04003A4D RID: 14925
		kidnap
	}

	// Token: 0x0200078B RID: 1931
	[Serializable]
	public class MurdererModifierRule
	{
		// Token: 0x04003A4E RID: 14926
		public CharacterTrait.RuleType rule;

		// Token: 0x04003A4F RID: 14927
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x04003A50 RID: 14928
		[ShowIf("isTrait")]
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x04003A51 RID: 14929
		[Tooltip("Add this to a default priority multiplier of 1.")]
		public float scoreModifier = 0.5f;
	}

	// Token: 0x0200078C RID: 1932
	public enum SuccessfulTravelTrigger
	{
		// Token: 0x04003A53 RID: 14931
		whenMurdererIsAtTheSameLocation,
		// Token: 0x04003A54 RID: 14932
		whenMurdererIsAtVantagePoint
	}

	// Token: 0x0200078D RID: 1933
	public enum LeadCitizen
	{
		// Token: 0x04003A56 RID: 14934
		nobody,
		// Token: 0x04003A57 RID: 14935
		victim,
		// Token: 0x04003A58 RID: 14936
		killer,
		// Token: 0x04003A59 RID: 14937
		victimsClosest,
		// Token: 0x04003A5A RID: 14938
		killersClosest,
		// Token: 0x04003A5B RID: 14939
		victimsDoctor,
		// Token: 0x04003A5C RID: 14940
		killersDoctor,
		// Token: 0x04003A5D RID: 14941
		ransom
	}

	// Token: 0x0200078E RID: 1934
	public enum LeadSpawnWhere
	{
		// Token: 0x04003A5F RID: 14943
		victimHome,
		// Token: 0x04003A60 RID: 14944
		victimWork,
		// Token: 0x04003A61 RID: 14945
		killerHome,
		// Token: 0x04003A62 RID: 14946
		killerWork,
		// Token: 0x04003A63 RID: 14947
		ransom,
		// Token: 0x04003A64 RID: 14948
		killerDen
	}

	// Token: 0x0200078F RID: 1935
	[Serializable]
	public class MurderModifierRule
	{
		// Token: 0x04003A65 RID: 14949
		public MurderPreset.LeadCitizen who;

		// Token: 0x04003A66 RID: 14950
		public CharacterTrait.RuleType rule;

		// Token: 0x04003A67 RID: 14951
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x04003A68 RID: 14952
		[ShowIf("isTrait")]
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x04003A69 RID: 14953
		[Tooltip("Add this to a default priority multiplier of 1.")]
		public float chanceModifier;
	}

	// Token: 0x02000790 RID: 1936
	[Serializable]
	public class MurderLeadItem
	{
		// Token: 0x04003A6A RID: 14954
		public string name;

		// Token: 0x04003A6B RID: 14955
		public bool compatibleWithAllMotives;

		// Token: 0x04003A6C RID: 14956
		[DisableIf("compatibleWithAllMotives")]
		public List<MurderMO> compatibleWithMotives = new List<MurderMO>();

		// Token: 0x04003A6D RID: 14957
		public MurderController.MurderState spawnOnPhase = MurderController.MurderState.acquireEuipment;

		// Token: 0x04003A6E RID: 14958
		public bool tryToSpawnWithEachNewMurder = true;

		// Token: 0x04003A6F RID: 14959
		public MurderPreset.LeadCitizen belongsTo = MurderPreset.LeadCitizen.victim;

		// Token: 0x04003A70 RID: 14960
		[Space(7f)]
		[Range(0f, 1f)]
		[DisableIf("useOrGroup")]
		public float chance = 1f;

		// Token: 0x04003A71 RID: 14961
		[Space(7f)]
		public bool useTraits;

		// Token: 0x04003A72 RID: 14962
		[EnableIf("useTraits")]
		public List<MurderPreset.MurderModifierRule> traitModifiers = new List<MurderPreset.MurderModifierRule>();

		// Token: 0x04003A73 RID: 14963
		[Space(7f)]
		public bool useIf;

		// Token: 0x04003A74 RID: 14964
		[EnableIf("useIf")]
		[Tooltip("Only spawn if a previous object of this letter is spawned...")]
		public JobPreset.JobTag ifTag;

		// Token: 0x04003A75 RID: 14965
		[Space(7f)]
		public bool useOrGroup;

		// Token: 0x04003A76 RID: 14966
		[EnableIf("useOrGroup")]
		[Tooltip("If enabled, only one chosen item from this group will be spawned...")]
		public JobPreset.JobTag orGroup;

		// Token: 0x04003A77 RID: 14967
		[EnableIf("useOrGroup")]
		[Range(0f, 10f)]
		public int chanceRatio = 4;

		// Token: 0x04003A78 RID: 14968
		[Space(7f)]
		public JobPreset.JobTag itemTag;

		// Token: 0x04003A79 RID: 14969
		[Tooltip("What?")]
		public InteractablePreset spawnItem;

		// Token: 0x04003A7A RID: 14970
		[Space(7f)]
		public string vmailThread;

		// Token: 0x04003A7B RID: 14971
		public Vector2 vmailProgressThreshold;

		// Token: 0x04003A7C RID: 14972
		[Tooltip("Where?")]
		public MurderPreset.LeadSpawnWhere where;

		// Token: 0x04003A7D RID: 14973
		public MurderPreset.LeadCitizen writer;

		// Token: 0x04003A7E RID: 14974
		public MurderPreset.LeadCitizen receiver;

		// Token: 0x04003A7F RID: 14975
		public int security = 3;

		// Token: 0x04003A80 RID: 14976
		public int priority = 1;

		// Token: 0x04003A81 RID: 14977
		public InteractablePreset.OwnedPlacementRule ownershipRule;
	}
}
