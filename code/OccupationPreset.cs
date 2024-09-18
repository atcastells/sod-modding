using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007A3 RID: 1955
[CreateAssetMenu(fileName = "occupation_data", menuName = "Database/Company/Occupation Preset")]
public class OccupationPreset : SoCustomComparison
{
	// Token: 0x060025B0 RID: 9648 RVA: 0x001E7AD3 File Offset: 0x001E5CD3
	[Button(null, 0)]
	public void CopyOutfitFromSelectedPreset()
	{
		if (this.selectedPreset != null)
		{
			this.workOutfit.Clear();
			this.workOutfit.AddRange(this.selectedPreset.workOutfit);
		}
	}

	// Token: 0x04003B29 RID: 15145
	[Tooltip("Collar colour")]
	[Header("Category")]
	public OccupationPreset.workCollar collar;

	// Token: 0x04003B2A RID: 15146
	[Tooltip("Type of work")]
	public OccupationPreset.workType work;

	// Token: 0x04003B2B RID: 15147
	[Tooltip("Additional tags to describe this work")]
	public List<OccupationPreset.workTags> tags = new List<OccupationPreset.workTags>();

	// Token: 0x04003B2C RID: 15148
	[Range(0f, 4f)]
	[Tooltip("The higher the priority, the more chance the position will be filled")]
	public int jobFillPriority = 2;

	// Token: 0x04003B2D RID: 15149
	[Header("Outfit")]
	[Tooltip("If this job requires a certain work outfit, list it here...")]
	public List<ClothesPreset> workOutfit;

	// Token: 0x04003B2E RID: 15150
	[Header("Special Cases")]
	public bool selfEmployed;

	// Token: 0x04003B2F RID: 15151
	public bool receptionist;

	// Token: 0x04003B30 RID: 15152
	public bool canAskAboutJob = true;

	// Token: 0x04003B31 RID: 15153
	public bool janitor;

	// Token: 0x04003B32 RID: 15154
	public bool security;

	// Token: 0x04003B33 RID: 15155
	public bool isCriminal;

	// Token: 0x04003B34 RID: 15156
	public bool isPublicFacing;

	// Token: 0x04003B35 RID: 15157
	[EnableIf("isCriminal")]
	public int minimumPerCity;

	// Token: 0x04003B36 RID: 15158
	[EnableIf("isCriminal")]
	public float societalClass = 0.5f;

	// Token: 0x04003B37 RID: 15159
	[Tooltip("Personality is calculated after job assign; how much to scew personality towards this...")]
	[Header("Personality Fit")]
	public float skewPersonalityTowardsJobFit = 0.4f;

	// Token: 0x04003B38 RID: 15160
	public bool skewHumility;

	// Token: 0x04003B39 RID: 15161
	[Range(0f, 1f)]
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[EnableIf("skewHumility")]
	public float humility;

	// Token: 0x04003B3A RID: 15162
	public bool skewEmotionality;

	// Token: 0x04003B3B RID: 15163
	[EnableIf("skewEmotionality")]
	[Range(0f, 1f)]
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	public float emotionality;

	// Token: 0x04003B3C RID: 15164
	public bool skewExtraversion;

	// Token: 0x04003B3D RID: 15165
	[Range(0f, 1f)]
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	[EnableIf("skewExtraversion")]
	public float extraversion;

	// Token: 0x04003B3E RID: 15166
	public bool skewAgreeableness;

	// Token: 0x04003B3F RID: 15167
	[Range(0f, 1f)]
	[EnableIf("skewAgreeableness")]
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	public float agreeableness;

	// Token: 0x04003B40 RID: 15168
	public bool skewConscientiousness;

	// Token: 0x04003B41 RID: 15169
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	[EnableIf("skewConscientiousness")]
	[Range(0f, 1f)]
	public float conscientiousness;

	// Token: 0x04003B42 RID: 15170
	public bool skewCreativity;

	// Token: 0x04003B43 RID: 15171
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	[EnableIf("skewCreativity")]
	[Range(0f, 1f)]
	public float creativity;

	// Token: 0x04003B44 RID: 15172
	[Tooltip("Find a shift matching the below enum")]
	[Header("Work Hours")]
	public bool shiftTimeIsImportant;

	// Token: 0x04003B45 RID: 15173
	[Tooltip("The employee works this shift (if available)")]
	public OccupationPreset.ShiftType shiftType = OccupationPreset.ShiftType.dayShift;

	// Token: 0x04003B46 RID: 15174
	[Tooltip("Does this job count towards the open coverage of the shift they have?")]
	public bool countsTowardsOpenHoursCoverage = true;

	// Token: 0x04003B47 RID: 15175
	[Tooltip("The employee can take a break half way through their shift")]
	public bool lunchBreakAllowed = true;

	// Token: 0x04003B48 RID: 15176
	[Tooltip("Where should the AI go to upon starting the goal?")]
	[Header("AI Behaviour")]
	public OccupationPreset.JobAI jobAIPosition;

	// Token: 0x04003B49 RID: 15177
	[ReorderableList]
	[Tooltip("If AI behaviour is set to patrol, are there any rooms in which it is not allowed?")]
	public List<RoomConfiguration> bannedRooms = new List<RoomConfiguration>();

	// Token: 0x04003B4A RID: 15178
	[Tooltip("The list of actions the AI will perform inside the 'Work' goal.")]
	public List<AIGoalPreset.GoalActionSetup> actionSetup = new List<AIGoalPreset.GoalActionSetup>();

	// Token: 0x04003B4B RID: 15179
	[Tooltip("What interactable will the AI work from?")]
	public InteractablePreset.SpecialCase jobPostion = InteractablePreset.SpecialCase.workDesk;

	// Token: 0x04003B4C RID: 15180
	[Tooltip("Does the AI own their own version of above? If not they will use free available ones.")]
	public bool ownsWorkPosition;

	// Token: 0x04003B4D RID: 15181
	[Tooltip("Where should the AI start to search for their work place?")]
	[ReorderableList]
	public List<RoomConfiguration> preferredRooms = new List<RoomConfiguration>();

	// Token: 0x04003B4E RID: 15182
	[Tooltip("How often should the AI get up and do other tasks. Time range (game time)")]
	[Space(5f)]
	public Vector2 potterFrequency = new Vector2(0.5f, 1f);

	// Token: 0x04003B4F RID: 15183
	[Tooltip("If true the AI will only potter if there is at least 1 other staff memeber on it's work postion interactable type")]
	public bool onlyPotterIfSomebodyElseWorking;

	// Token: 0x04003B50 RID: 15184
	[Tooltip("List of other tasks to randomly do while on the job.")]
	[ReorderableList]
	public List<AIActionPreset> potterActions = new List<AIActionPreset>();

	// Token: 0x04003B51 RID: 15185
	[Tooltip("Allows clean up actions to be inserted which will allow the AI to pick things up from the floor")]
	public bool canPickUpLitter;

	// Token: 0x04003B52 RID: 15186
	[Header("Items")]
	[Tooltip("This person has a name placard at work")]
	public bool namePlacard = true;

	// Token: 0x04003B53 RID: 15187
	[Tooltip("This person has an employee photo")]
	public bool employeePhoto = true;

	// Token: 0x04003B54 RID: 15188
	[Tooltip("This person has business cards")]
	public bool businessCards = true;

	// Token: 0x04003B55 RID: 15189
	[Tooltip("This person has a work rota")]
	public bool workRota = true;

	// Token: 0x04003B56 RID: 15190
	[Tooltip("This person has an employment contract")]
	public bool employmentContract = true;

	// Token: 0x04003B57 RID: 15191
	[Tooltip("List of items to add once this job position is filled")]
	public List<InteractablePreset> jobItems = new List<InteractablePreset>();

	// Token: 0x04003B58 RID: 15192
	[Tooltip("List of items to add to inventory")]
	public List<InteractablePreset> inventoryItems = new List<InteractablePreset>();

	// Token: 0x04003B59 RID: 15193
	public List<GroupPreset> joinGroups = new List<GroupPreset>();

	// Token: 0x04003B5A RID: 15194
	[Header("Dialog Options")]
	public List<DialogPreset> addDialog = new List<DialogPreset>();

	// Token: 0x04003B5B RID: 15195
	[Header("Debug")]
	public OccupationPreset selectedPreset;

	// Token: 0x020007A4 RID: 1956
	public enum workCollar
	{
		// Token: 0x04003B5D RID: 15197
		blueCollar,
		// Token: 0x04003B5E RID: 15198
		whiteCollar,
		// Token: 0x04003B5F RID: 15199
		pinkCollar,
		// Token: 0x04003B60 RID: 15200
		redCollar,
		// Token: 0x04003B61 RID: 15201
		goldCollar,
		// Token: 0x04003B62 RID: 15202
		orangeCollar,
		// Token: 0x04003B63 RID: 15203
		scarletCollar,
		// Token: 0x04003B64 RID: 15204
		blackCollar,
		// Token: 0x04003B65 RID: 15205
		noCollar
	}

	// Token: 0x020007A5 RID: 1957
	public enum workType
	{
		// Token: 0x04003B67 RID: 15207
		Office,
		// Token: 0x04003B68 RID: 15208
		Management,
		// Token: 0x04003B69 RID: 15209
		Labourer,
		// Token: 0x04003B6A RID: 15210
		Janitorial,
		// Token: 0x04003B6B RID: 15211
		Retail,
		// Token: 0x04003B6C RID: 15212
		Service,
		// Token: 0x04003B6D RID: 15213
		Driver,
		// Token: 0x04003B6E RID: 15214
		PublicSector,
		// Token: 0x04003B6F RID: 15215
		Enforcer,
		// Token: 0x04003B70 RID: 15216
		Criminal,
		// Token: 0x04003B71 RID: 15217
		Creative,
		// Token: 0x04003B72 RID: 15218
		Other,
		// Token: 0x04003B73 RID: 15219
		Student,
		// Token: 0x04003B74 RID: 15220
		Unemployed,
		// Token: 0x04003B75 RID: 15221
		Retired,
		// Token: 0x04003B76 RID: 15222
		Illegal
	}

	// Token: 0x020007A6 RID: 1958
	public enum ShiftType
	{
		// Token: 0x04003B78 RID: 15224
		morningShift,
		// Token: 0x04003B79 RID: 15225
		dayShift,
		// Token: 0x04003B7A RID: 15226
		eveningShift,
		// Token: 0x04003B7B RID: 15227
		nightShift
	}

	// Token: 0x020007A7 RID: 1959
	public enum JobAI
	{
		// Token: 0x04003B7D RID: 15229
		workPosition,
		// Token: 0x04003B7E RID: 15230
		random,
		// Token: 0x04003B7F RID: 15231
		randomBuilding,
		// Token: 0x04003B80 RID: 15232
		passedCompanyPosition
	}

	// Token: 0x020007A8 RID: 1960
	public enum workTags
	{
		// Token: 0x04003B82 RID: 15234
		none,
		// Token: 0x04003B83 RID: 15235
		dull,
		// Token: 0x04003B84 RID: 15236
		exciting,
		// Token: 0x04003B85 RID: 15237
		dangerous,
		// Token: 0x04003B86 RID: 15238
		menial,
		// Token: 0x04003B87 RID: 15239
		intern,
		// Token: 0x04003B88 RID: 15240
		stressful,
		// Token: 0x04003B89 RID: 15241
		cushy,
		// Token: 0x04003B8A RID: 15242
		technical,
		// Token: 0x04003B8B RID: 15243
		ceo,
		// Token: 0x04003B8C RID: 15244
		social,
		// Token: 0x04003B8D RID: 15245
		isolated,
		// Token: 0x04003B8E RID: 15246
		professional
	}

	// Token: 0x020007A9 RID: 1961
	public enum Overtime
	{
		// Token: 0x04003B90 RID: 15248
		none,
		// Token: 0x04003B91 RID: 15249
		low,
		// Token: 0x04003B92 RID: 15250
		medium,
		// Token: 0x04003B93 RID: 15251
		high,
		// Token: 0x04003B94 RID: 15252
		veryHigh
	}
}
