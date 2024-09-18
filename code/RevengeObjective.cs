using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
[CreateAssetMenu(fileName = "revenge_data", menuName = "Database/Revenge Objective")]
public class RevengeObjective : SoCustomComparison
{
	// Token: 0x060025B8 RID: 9656 RVA: 0x001E7E04 File Offset: 0x001E6004
	public float Vandalism(int target, int location, float amount)
	{
		NewAddress newAddress = null;
		float result = 0f;
		if (CityData.Instance.addressDictionary.TryGetValue(location, ref newAddress))
		{
			result = (float)newAddress.GetVandalismDamage(true, true, true);
			Game.Log("Jobs: Vandalism damage at " + newAddress.name + " is " + result.ToString(), 2);
		}
		else
		{
			Game.Log("Jobs: Unable to get vandalism damage from address ID " + location.ToString(), 2);
		}
		return result;
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x001E7E74 File Offset: 0x001E6074
	public float VandalismTrash(int target, int location, float amount)
	{
		NewAddress newAddress = null;
		float result = 0f;
		if (CityData.Instance.addressDictionary.TryGetValue(location, ref newAddress))
		{
			result = (float)newAddress.GetVandalismDamage(false, false, true);
			Game.Log("Jobs: Vandalism damage (trash only) at " + newAddress.name + " is " + result.ToString(), 2);
		}
		else
		{
			Game.Log("Jobs: Unable to get vandalism damage from address ID " + location.ToString(), 2);
		}
		return result;
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x001E7EE4 File Offset: 0x001E60E4
	public float VandalismWindow(int target, int location, float amount)
	{
		NewAddress newAddress = null;
		float result = 0f;
		if (CityData.Instance.addressDictionary.TryGetValue(location, ref newAddress))
		{
			result = (float)newAddress.GetVandalismDamage(false, true, false);
			Game.Log("Jobs: Vandalism damage (windows only) at " + newAddress.name + " is " + result.ToString(), 2);
		}
		else
		{
			Game.Log("Jobs: Unable to get vandalism damage from address ID " + location.ToString(), 2);
		}
		return result;
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x001E7F54 File Offset: 0x001E6154
	public bool Handcuff(int target, int location, float amount)
	{
		Human human = null;
		return CityData.Instance.GetHuman(target, out human, true) && human.ai != null && human.ai.restrained && !human.currentGameLocation.IsPublicallyOpen(false);
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x001E7FA0 File Offset: 0x001E61A0
	public bool BeatUp(int target, int location, float amount)
	{
		Human human = null;
		return CityData.Instance.GetHuman(target, out human, true) && human.ai != null && human.ai.ko && !human.isDead;
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x001E7FE8 File Offset: 0x001E61E8
	public bool KickDownDoor(int target, int location, float amount)
	{
		NewAddress newAddress = null;
		if (CityData.Instance.addressDictionary.TryGetValue(location, ref newAddress))
		{
			if (newAddress.entrances.Exists((NewNode.NodeAccess item) => item.door != null && item.door.wall.currentDoorStrength <= 0.01f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x00085A40 File Offset: 0x00083C40
	public bool ManualTrigger(int target, int location, float amount)
	{
		return false;
	}

	// Token: 0x04003C29 RID: 15401
	public bool disabled;

	// Token: 0x04003C2A RID: 15402
	[Range(0f, 10f)]
	[Header("Trait Weighting")]
	public int baseChance = 1;

	// Token: 0x04003C2B RID: 15403
	[Space(7f)]
	[InfoBox("If enabled: The below HEXACO values will combine for a score of 1 to 10: this will be used to calculate the likihood of this being chosen vs others.", 0)]
	[Tooltip("Use the below hexaco values to match to personality.")]
	public bool useHEXACO;

	// Token: 0x04003C2C RID: 15404
	[Range(0f, 10f)]
	[EnableIf("useHEXACO")]
	public int feminineMasculine = 5;

	// Token: 0x04003C2D RID: 15405
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[Range(0f, 10f)]
	[EnableIf("useHEXACO")]
	public int humility = 5;

	// Token: 0x04003C2E RID: 15406
	[EnableIf("useHEXACO")]
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	[Range(0f, 10f)]
	public int emotionality = 5;

	// Token: 0x04003C2F RID: 15407
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	[Range(0f, 10f)]
	[EnableIf("useHEXACO")]
	public int extraversion = 5;

	// Token: 0x04003C30 RID: 15408
	[EnableIf("useHEXACO")]
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	[Range(0f, 10f)]
	public int agreeableness = 5;

	// Token: 0x04003C31 RID: 15409
	[Range(0f, 10f)]
	[EnableIf("useHEXACO")]
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	public int conscientiousness = 5;

	// Token: 0x04003C32 RID: 15410
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	[Range(0f, 10f)]
	[EnableIf("useHEXACO")]
	public int creativity = 5;

	// Token: 0x04003C33 RID: 15411
	[Space(7f)]
	[InfoBox("If enabled: The below traits will be used to calculate the likihood of this being chosen vs others.", 0)]
	[Tooltip("Use character traits to match to personality.")]
	public bool useTraits;

	// Token: 0x04003C34 RID: 15412
	public List<ClothesPreset.TraitPickRule> characterTraitsPoster = new List<ClothesPreset.TraitPickRule>();

	// Token: 0x04003C35 RID: 15413
	public List<ClothesPreset.TraitPickRule> characterTraitsPurp = new List<ClothesPreset.TraitPickRule>();

	// Token: 0x04003C36 RID: 15414
	public List<RevengeObjective.SpecialConditions> specialConditions = new List<RevengeObjective.SpecialConditions>();

	// Token: 0x04003C37 RID: 15415
	[Header("Setup")]
	public string d0Name;

	// Token: 0x04003C38 RID: 15416
	public string d1Name;

	// Token: 0x04003C39 RID: 15417
	public string idTargetName;

	// Token: 0x04003C3A RID: 15418
	public JobPreset.JobTag tag;

	// Token: 0x04003C3B RID: 15419
	[Space(7f)]
	public InterfaceControls.Icon icon = InterfaceControls.Icon.resolve;

	// Token: 0x04003C3C RID: 15420
	[InfoBox("This can be used to dictate an amount; eg how much damage to cause at a property", 0)]
	public Vector2 passedNumberRange = Vector2.zero;

	// Token: 0x04003C3D RID: 15421
	[Tooltip("Multiplies the rewards based on the above number")]
	public Vector2 rewardMultiplier = new Vector2(1f, 1.5f);

	// Token: 0x04003C3E RID: 15422
	[Tooltip("Name as part of resolve questions")]
	public string resolveQuestionName;

	// Token: 0x04003C3F RID: 15423
	public string resolveQuestionNameAlternate;

	// Token: 0x04003C40 RID: 15424
	[Space(10f)]
	[Tooltip("Refers to an answer method within this script that is used to check")]
	public string answerMethod = "Vandalism";

	// Token: 0x020007B5 RID: 1973
	public enum SpecialConditions
	{
		// Token: 0x04003C42 RID: 15426
		mustHaveWindows,
		// Token: 0x04003C43 RID: 15427
		trackProgressFromAddressQuestion,
		// Token: 0x04003C44 RID: 15428
		trackProgressFromNameQuestion
	}
}
