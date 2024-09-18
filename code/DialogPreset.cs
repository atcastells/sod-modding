using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000702 RID: 1794
[CreateAssetMenu(fileName = "dialog_data", menuName = "Database/Dialog Option")]
public class DialogPreset : SoCustomComparison
{
	// Token: 0x06002531 RID: 9521 RVA: 0x001E5258 File Offset: 0x001E3458
	public int GetCost(Actor talkingTo, Actor talking = null)
	{
		int num = this.cost;
		if (this.usePercentageCost)
		{
			num = Mathf.RoundToInt((float)GameplayController.Instance.money * ((float)this.cost / 100f));
		}
		if (this.useAllWealthIfNotEnough && GameplayController.Instance.money < num)
		{
			num = GameplayController.Instance.money;
		}
		if (this.specialCase == DialogPreset.SpecialCase.loanSharkPayment && talkingTo != null)
		{
			Human hu = talkingTo as Human;
			if (hu.job != null && hu.job.employer != null)
			{
				GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == hu.job.employer.companyID);
				if (loanDebt != null)
				{
					num = loanDebt.GetRepaymentAmount();
				}
			}
		}
		else if (this.specialCase == DialogPreset.SpecialCase.hotelBill || this.specialCase == DialogPreset.SpecialCase.hotelCheckOut)
		{
			GameplayController.HotelGuest hotelRoom = Toolbox.Instance.GetHotelRoom(talking as Human);
			if (hotelRoom != null)
			{
				num = hotelRoom.bill;
			}
		}
		return num;
	}

	// Token: 0x04003371 RID: 13169
	[Header("Setup")]
	public string msgID;

	// Token: 0x04003372 RID: 13170
	[Tooltip("Is this option added to citizens at the start?")]
	public bool defaultOption;

	// Token: 0x04003373 RID: 13171
	[EnableIf("defaultOption")]
	[Tooltip("Is this used for the telephone calling dialog?")]
	public bool telephoneCallOption;

	// Token: 0x04003374 RID: 13172
	[Tooltip("Is this used for the hospital decision tree dialog?")]
	public bool hospitalDecisionOption;

	// Token: 0x04003375 RID: 13173
	[Tooltip("Must have access to this key for this option to appear")]
	public Evidence.DataKey tiedToKey = Evidence.DataKey.photo;

	// Token: 0x04003376 RID: 13174
	[Tooltip("Ranking within options")]
	public int ranking;

	// Token: 0x04003377 RID: 13175
	[Tooltip("Remove this after it's been said")]
	public bool removeAfterSaying = true;

	// Token: 0x04003378 RID: 13176
	[Tooltip("Replenish after each day: Every 24 hours this will be added to every citizen if not already added")]
	[EnableIf("defaultOption")]
	public bool dailyReplenish;

	// Token: 0x04003379 RID: 13177
	[Tooltip("This dialog will give the player the mission details")]
	public bool isJobDetails;

	// Token: 0x0400337A RID: 13178
	[Tooltip("If false, this will only be active when the associated job is active. If true, this requirement will be ignored.")]
	public bool ignoreActiveJobRequirement;

	// Token: 0x0400337B RID: 13179
	[Tooltip("Special cases")]
	public DialogPreset.SpecialCase specialCase;

	// Token: 0x0400337C RID: 13180
	[Tooltip("This option is selectable for a cost")]
	public int cost;

	// Token: 0x0400337D RID: 13181
	[Tooltip("If ture, the above is a percentage cost of the player's total wealth")]
	public bool usePercentageCost;

	// Token: 0x0400337E RID: 13182
	[Tooltip("If true, and the player doesn't have enough to cover the cost then use the total amount of player's wealth")]
	public bool useAllWealthIfNotEnough;

	// Token: 0x0400337F RID: 13183
	[Tooltip("Only displayed if the current address requires a password")]
	public bool displayIfPasswordUnknown;

	// Token: 0x04003380 RID: 13184
	[Tooltip("Player must input correct string before forcing a success or fail")]
	public DialogPreset.InputSetting inputBox;

	// Token: 0x04003381 RID: 13185
	[Tooltip("Display this dialog in red (illegal)")]
	public bool displayAsIllegal;

	// Token: 0x04003382 RID: 13186
	[Tooltip("Preceeding syntax")]
	public string preceedingSyntax;

	// Token: 0x04003383 RID: 13187
	[Tooltip("Following syntax")]
	public string followingSyntax;

	// Token: 0x04003384 RID: 13188
	[Tooltip("Use a success test to determin the outcome response")]
	[Header("Success Test")]
	public bool useSuccessTest;

	// Token: 0x04003385 RID: 13189
	[Tooltip("Requires the correct password to be successful, if there is one")]
	[EnableIf("useSuccessTest")]
	public bool requiresPassword;

	// Token: 0x04003386 RID: 13190
	[Range(0f, 1f)]
	[EnableIf("useSuccessTest")]
	public float baseChance;

	// Token: 0x04003387 RID: 13191
	[Tooltip("If restrained, the success change is affected this much...")]
	[EnableIf("useSuccessTest")]
	public float affectChanceIfRestrained;

	// Token: 0x04003388 RID: 13192
	[Tooltip("Modify success based on below traits...")]
	[ReorderableList]
	public List<CharacterTrait.TraitPickRule> modifySuccessChanceTraits = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x04003389 RID: 13193
	[Header("Responses")]
	[ReorderableList]
	public List<AIActionPreset.AISpeechPreset> responses = new List<AIActionPreset.AISpeechPreset>();

	// Token: 0x0400338A RID: 13194
	[Tooltip("Add these as player responses following...")]
	[Header("Follow up")]
	public List<DialogPreset> followUpDialogSuccess = new List<DialogPreset>();

	// Token: 0x0400338B RID: 13195
	public List<DialogPreset> followUpDialogFail = new List<DialogPreset>();

	// Token: 0x0400338C RID: 13196
	[Tooltip("Remove these other options")]
	public List<DialogPreset> removeDialog = new List<DialogPreset>();

	// Token: 0x0400338D RID: 13197
	public List<DialogPreset> removeDialogOnSuccess = new List<DialogPreset>();

	// Token: 0x0400338E RID: 13198
	public List<DialogPreset> removeDialogOnFail = new List<DialogPreset>();

	// Token: 0x02000703 RID: 1795
	public enum InputSetting
	{
		// Token: 0x04003390 RID: 13200
		none,
		// Token: 0x04003391 RID: 13201
		addressPassword
	}

	// Token: 0x02000704 RID: 1796
	public enum SpecialCase
	{
		// Token: 0x04003393 RID: 13203
		none,
		// Token: 0x04003394 RID: 13204
		backroomBribe,
		// Token: 0x04003395 RID: 13205
		publicFacingWorkplace,
		// Token: 0x04003396 RID: 13206
		working,
		// Token: 0x04003397 RID: 13207
		workingGuestPass,
		// Token: 0x04003398 RID: 13208
		callInSuspect,
		// Token: 0x04003399 RID: 13209
		talkingToJobPoster,
		// Token: 0x0400339A RID: 13210
		inputName,
		// Token: 0x0400339B RID: 13211
		lastCaller,
		// Token: 0x0400339C RID: 13212
		knowName,
		// Token: 0x0400339D RID: 13213
		lookAroundHome,
		// Token: 0x0400339E RID: 13214
		returnJobItemA,
		// Token: 0x0400339F RID: 13215
		medicalCosts,
		// Token: 0x040033A0 RID: 13216
		starchPitch,
		// Token: 0x040033A1 RID: 13217
		mugging,
		// Token: 0x040033A2 RID: 13218
		neverDisplay,
		// Token: 0x040033A3 RID: 13219
		loanSharkAccept,
		// Token: 0x040033A4 RID: 13220
		loanSharkPayment,
		// Token: 0x040033A5 RID: 13221
		loanSharkPaymentRefuse,
		// Token: 0x040033A6 RID: 13222
		loanSharkAsk,
		// Token: 0x040033A7 RID: 13223
		revealHiddenitemPhoto,
		// Token: 0x040033A8 RID: 13224
		hotelBill,
		// Token: 0x040033A9 RID: 13225
		rentHotelRoomCheap,
		// Token: 0x040033AA RID: 13226
		rentHotelRoomExpensive,
		// Token: 0x040033AB RID: 13227
		hotelCheckOut,
		// Token: 0x040033AC RID: 13228
		hotelRentRoom,
		// Token: 0x040033AD RID: 13229
		mustHaveRoomAtHotel,
		// Token: 0x040033AE RID: 13230
		mustBeMurdererForSuccess,
		// Token: 0x040033AF RID: 13231
		killerCleanUp,
		// Token: 0x040033B0 RID: 13232
		killerCleanUpAccept,
		// Token: 0x040033B1 RID: 13233
		killerCleanUpReject,
		// Token: 0x040033B2 RID: 13234
		killerCleanUpSuccess,
		// Token: 0x040033B3 RID: 13235
		ransomInvestigate,
		// Token: 0x040033B4 RID: 13236
		kidnapperOnly
	}
}
