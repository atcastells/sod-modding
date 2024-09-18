using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000791 RID: 1937
[CreateAssetMenu(fileName = "murderweapon_data", menuName = "Database/Murder Weapon")]
public class MurderWeaponPreset : SoCustomComparison
{
	// Token: 0x060025A7 RID: 9639 RVA: 0x001E77C0 File Offset: 0x001E59C0
	public float GetAttackValue(MurderWeaponPreset.AttackValue valueType, Human human)
	{
		MurderWeaponPreset.StatMultiplier statMultiplier = MurderWeaponPreset.StatMultiplier.zero;
		Vector2 zero = Vector2.zero;
		if (valueType == MurderWeaponPreset.AttackValue.accuracy)
		{
			zero = this.attackAccuracy;
			statMultiplier = this.attackAccuracyLerpSource;
		}
		else if (valueType == MurderWeaponPreset.AttackValue.damage)
		{
			zero = this.attackDamage;
			statMultiplier = this.attackDamageLerpSource;
		}
		else if (valueType == MurderWeaponPreset.AttackValue.fireDelay)
		{
			zero = this.fireDelay;
			statMultiplier = this.fireDelayLerpSource;
		}
		else if (valueType == MurderWeaponPreset.AttackValue.range)
		{
			zero = this.weaponMaxRange;
			statMultiplier = this.weaponRangeLerpSource;
		}
		float result = zero.x;
		if (statMultiplier == MurderWeaponPreset.StatMultiplier.one)
		{
			result = zero.y;
		}
		else if (statMultiplier == MurderWeaponPreset.StatMultiplier.random)
		{
			result = Mathf.Lerp(zero.x, zero.y, Toolbox.Instance.Rand(0f, 1f, true));
		}
		else if (statMultiplier == MurderWeaponPreset.StatMultiplier.combatHeft)
		{
			result = Mathf.Lerp(zero.x, zero.y, human.combatHeft);
		}
		else if (statMultiplier == MurderWeaponPreset.StatMultiplier.combatSkill)
		{
			result = Mathf.Lerp(zero.x, zero.y, human.combatSkill);
		}
		return result;
	}

	// Token: 0x04003A82 RID: 14978
	[Header("Configuration")]
	public MurderWeaponPreset.WeaponType type;

	// Token: 0x04003A83 RID: 14979
	public List<InteractablePreset> ammunition = new List<InteractablePreset>();

	// Token: 0x04003A84 RID: 14980
	[Range(0f, 3f)]
	public int murderDifficultyModifier;

	// Token: 0x04003A85 RID: 14981
	[Header("World Items")]
	[Tooltip("Local muzzle position relative to the pivot")]
	public Vector3 muzzleOffset = Vector3.zero;

	// Token: 0x04003A86 RID: 14982
	[Tooltip("Local brass eject relative to the pivot")]
	public Vector3 brassEjectOffset = Vector3.zero;

	// Token: 0x04003A87 RID: 14983
	[Space(7f)]
	public GameObject itemRightOverride;

	// Token: 0x04003A88 RID: 14984
	public Vector3 itemRightLocalPos;

	// Token: 0x04003A89 RID: 14985
	public Vector3 itemRightLocalEuler;

	// Token: 0x04003A8A RID: 14986
	[Space(7f)]
	public GameObject itemLeftOverride;

	// Token: 0x04003A8B RID: 14987
	public Vector3 itemLeftLocalPos;

	// Token: 0x04003A8C RID: 14988
	public Vector3 itemLeftLocalEuler;

	// Token: 0x04003A8D RID: 14989
	[Space(7f)]
	public bool overideUsesCarryAnimation;

	// Token: 0x04003A8E RID: 14990
	[EnableIf("overideUsesCarryAnimation")]
	public int overrideCarryAnimation = 1;

	// Token: 0x04003A8F RID: 14991
	[Header("Personal Defence")]
	[Tooltip("If true, citizens may carry this about to defend themselves")]
	public bool usedInPersonalDefence;

	// Token: 0x04003A90 RID: 14992
	public bool disabled;

	// Token: 0x04003A91 RID: 14993
	[EnableIf("usedInPersonalDefence")]
	[Range(0f, 10f)]
	public int basePriority = 3;

	// Token: 0x04003A92 RID: 14994
	[Space(7f)]
	[MinMaxSlider(0f, 1f)]
	public Vector2 socialClassRange = new Vector2(0f, 1f);

	// Token: 0x04003A93 RID: 14995
	[Range(0f, 10f)]
	public int citizenSpawningWithScore;

	// Token: 0x04003A94 RID: 14996
	[EnableIf("usedInPersonalDefence")]
	public List<MurderPreset.MurdererModifierRule> personalDefenceTraitModifiers = new List<MurderPreset.MurdererModifierRule>();

	// Token: 0x04003A95 RID: 14997
	[Space(7f)]
	public List<OccupationPreset> jobModifierList = new List<OccupationPreset>();

	// Token: 0x04003A96 RID: 14998
	public int jobScoreModifier;

	// Token: 0x04003A97 RID: 14999
	[Tooltip("How this impacts nerve levels of a citizen if drawn")]
	[Space(7f)]
	public float drawnNerveModifier = -0.1f;

	// Token: 0x04003A98 RID: 15000
	[Tooltip("Chance of bark trigger")]
	public float barkTriggerChance;

	// Token: 0x04003A99 RID: 15001
	public SpeechController.Bark bark = SpeechController.Bark.threatenByItem;

	// Token: 0x04003A9A RID: 15002
	[Tooltip("With this weapon, multiply incoming nerve damage by this")]
	public float incomingNerveDamageMultiplier = 1f;

	// Token: 0x04003A9B RID: 15003
	[Header("Weapon Handling")]
	[Tooltip("At what point during the attack is the trigger executed? Normalized value")]
	[Range(0f, 1f)]
	public float attackTriggerPoint = 0.5f;

	// Token: 0x04003A9C RID: 15004
	[Range(0f, 1f)]
	[Tooltip("At what point during the attack is the trigger removed? Normalized value")]
	public float attackRemovePoint = 0.7f;

	// Token: 0x04003A9D RID: 15005
	[Tooltip("How many shots are fired?")]
	public int shots = 1;

	// Token: 0x04003A9E RID: 15006
	[Tooltip("Weapon range")]
	[Space(7f)]
	public Vector2 weaponMaxRange = Vector2.one;

	// Token: 0x04003A9F RID: 15007
	public float minimumRange = 1f;

	// Token: 0x04003AA0 RID: 15008
	public float maximumBulletRange = 25f;

	// Token: 0x04003AA1 RID: 15009
	public MurderWeaponPreset.StatMultiplier weaponRangeLerpSource = MurderWeaponPreset.StatMultiplier.one;

	// Token: 0x04003AA2 RID: 15010
	[Tooltip("Time in seconds between attacks")]
	[Space(7f)]
	public Vector2 fireDelay = Vector2.one;

	// Token: 0x04003AA3 RID: 15011
	public MurderWeaponPreset.StatMultiplier fireDelayLerpSource = MurderWeaponPreset.StatMultiplier.one;

	// Token: 0x04003AA4 RID: 15012
	[Tooltip("Attack accuracy")]
	[Space(7f)]
	public Vector2 attackAccuracy = Vector2.one;

	// Token: 0x04003AA5 RID: 15013
	public MurderWeaponPreset.StatMultiplier attackAccuracyLerpSource = MurderWeaponPreset.StatMultiplier.one;

	// Token: 0x04003AA6 RID: 15014
	[Space(7f)]
	[Tooltip("Attack damage")]
	public Vector2 attackDamage = Vector2.one;

	// Token: 0x04003AA7 RID: 15015
	public MurderWeaponPreset.StatMultiplier attackDamageLerpSource = MurderWeaponPreset.StatMultiplier.one;

	// Token: 0x04003AA8 RID: 15016
	public float applyPoison;

	// Token: 0x04003AA9 RID: 15017
	[Header("FX Prefabs")]
	public InteractablePreset shellCasing;

	// Token: 0x04003AAA RID: 15018
	public MurderWeaponPreset.EjectBrass ejectBrassSetting = MurderWeaponPreset.EjectBrass.onFire;

	// Token: 0x04003AAB RID: 15019
	public InteractablePreset bulletHole;

	// Token: 0x04003AAC RID: 15020
	public InteractablePreset glassBulletHole;

	// Token: 0x04003AAD RID: 15021
	public InteractablePreset entryWound;

	// Token: 0x04003AAE RID: 15022
	public GameObject bulletRicochet;

	// Token: 0x04003AAF RID: 15023
	public GameObject bulletImpactSpray;

	// Token: 0x04003AB0 RID: 15024
	public GameObject muzzleFlash;

	// Token: 0x04003AB1 RID: 15025
	[Range(0f, 1f)]
	public float bloodPoolAmount;

	// Token: 0x04003AB2 RID: 15026
	[Header("Hits")]
	public SpatterPatternPreset forwardSpatter;

	// Token: 0x04003AB3 RID: 15027
	public SpatterPatternPreset backSpatter;

	// Token: 0x04003AB4 RID: 15028
	[Header("Audio")]
	public AudioEvent fireEvent;

	// Token: 0x04003AB5 RID: 15029
	public AudioEvent impactEvent;

	// Token: 0x04003AB6 RID: 15030
	public AudioEvent impactEventBody;

	// Token: 0x04003AB7 RID: 15031
	public AudioEvent impactEventPlayer;

	// Token: 0x02000792 RID: 1938
	public enum WeaponType
	{
		// Token: 0x04003AB9 RID: 15033
		handgun,
		// Token: 0x04003ABA RID: 15034
		rifle,
		// Token: 0x04003ABB RID: 15035
		shotgun,
		// Token: 0x04003ABC RID: 15036
		blade,
		// Token: 0x04003ABD RID: 15037
		bluntObject,
		// Token: 0x04003ABE RID: 15038
		poison,
		// Token: 0x04003ABF RID: 15039
		strangulation,
		// Token: 0x04003AC0 RID: 15040
		fists
	}

	// Token: 0x02000793 RID: 1939
	public enum StatMultiplier
	{
		// Token: 0x04003AC2 RID: 15042
		zero,
		// Token: 0x04003AC3 RID: 15043
		one,
		// Token: 0x04003AC4 RID: 15044
		random,
		// Token: 0x04003AC5 RID: 15045
		combatSkill,
		// Token: 0x04003AC6 RID: 15046
		combatHeft
	}

	// Token: 0x02000794 RID: 1940
	public enum EjectBrass
	{
		// Token: 0x04003AC8 RID: 15048
		none,
		// Token: 0x04003AC9 RID: 15049
		onFire,
		// Token: 0x04003ACA RID: 15050
		onPumpAction,
		// Token: 0x04003ACB RID: 15051
		revolver
	}

	// Token: 0x02000795 RID: 1941
	public enum AttackValue
	{
		// Token: 0x04003ACD RID: 15053
		range,
		// Token: 0x04003ACE RID: 15054
		fireDelay,
		// Token: 0x04003ACF RID: 15055
		accuracy,
		// Token: 0x04003AD0 RID: 15056
		damage
	}
}
