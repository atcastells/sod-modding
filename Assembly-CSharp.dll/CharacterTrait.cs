using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006D3 RID: 1747
[CreateAssetMenu(fileName = "trait_data", menuName = "Database/Character Trait")]
public class CharacterTrait : SoCustomComparison
{
	// Token: 0x04003205 RID: 12805
	[Tooltip("If true this is a trait that can be picked, if false, this is a 'reason'.")]
	[Header("Type")]
	public bool isTrait = true;

	// Token: 0x04003206 RID: 12806
	[Tooltip("If true, a 'reason' trait will be immediately picked to accompany this trait")]
	public bool needsReson;

	// Token: 0x04003207 RID: 12807
	[Tooltip("If true, this trait requires a partner")]
	public bool requiresPartner;

	// Token: 0x04003208 RID: 12808
	[Tooltip("If true, this trait requires NO partner")]
	public bool requiresSingle;

	// Token: 0x04003209 RID: 12809
	[Tooltip("If true, this trait requires a citizen to have a home")]
	public bool requiresHome;

	// Token: 0x0400320A RID: 12810
	[Tooltip("If true, this trait requires a job")]
	public bool requiresEmployment;

	// Token: 0x0400320B RID: 12811
	[Tooltip("If true, this needs a date")]
	public bool needsDate;

	// Token: 0x0400320C RID: 12812
	[Tooltip("Appears in the 'random interest' pool when acquiring information on the citizen.")]
	public bool featureInInterestPool;

	// Token: 0x0400320D RID: 12813
	[Tooltip("Appears in the 'random affliction' pool when acquiring information on the citizen.")]
	public bool featureInAfflictionPool;

	// Token: 0x0400320E RID: 12814
	[Tooltip("This event happened when their age was...")]
	[EnableIf("needsDate")]
	public Vector2 ageDateRange = new Vector2(5f, 10f);

	// Token: 0x0400320F RID: 12815
	[Tooltip("Use couples anniversary date")]
	[EnableIf("needsDate")]
	public bool useCouplesAnniversary;

	// Token: 0x04003210 RID: 12816
	[Tooltip("This is a password (special case)")]
	public bool isPassword;

	// Token: 0x04003211 RID: 12817
	[Tooltip("Disabled from being assigned automatically")]
	public bool disabled;

	// Token: 0x04003212 RID: 12818
	[Tooltip("Is this considered a positive/neutral/negative trait?")]
	public CharacterTrait.PosNeg postiveNegative = CharacterTrait.PosNeg.neutral;

	// Token: 0x04003213 RID: 12819
	[Header("Pick")]
	[Range(0f, 3f)]
	[EnableIf("isTrait")]
	[Tooltip("When is this anylised to see if it is picked or not? 0 = first, 2 = last")]
	public int pickStage;

	// Token: 0x04003214 RID: 12820
	[Range(0f, 1f)]
	[EnableIf("isTrait")]
	[Tooltip("Chance of assigning this trait completely at random on citizen creation")]
	public float primeBaseChance;

	// Token: 0x04003215 RID: 12821
	[ReorderableList]
	public List<CharacterTrait.TraitPickRule> pickRules = new List<CharacterTrait.TraitPickRule>();

	// Token: 0x04003216 RID: 12822
	[Header("Match")]
	[Range(0f, 1f)]
	[Tooltip("Importance of matching this to base HEXACO personality. This is added to the base chance of either prime or secondary.")]
	public float matchChance;

	// Token: 0x04003217 RID: 12823
	[Space(5f)]
	public bool useHumilityMatch;

	// Token: 0x04003218 RID: 12824
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[Range(0f, 1f)]
	[EnableIf("useHumilityMatch")]
	public float matchHumility;

	// Token: 0x04003219 RID: 12825
	public bool useEmotionalityMatch;

	// Token: 0x0400321A RID: 12826
	[EnableIf("useEmotionalityMatch")]
	[Range(0f, 1f)]
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	public float matchEmotionality;

	// Token: 0x0400321B RID: 12827
	public bool useExtraversionMatch;

	// Token: 0x0400321C RID: 12828
	[Range(0f, 1f)]
	[EnableIf("useExtraversionMatch")]
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	public float matchExtraversion;

	// Token: 0x0400321D RID: 12829
	public bool useAgreeablenessMatch;

	// Token: 0x0400321E RID: 12830
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	[Range(0f, 1f)]
	[EnableIf("useAgreeablenessMatch")]
	public float matchAgreeableness;

	// Token: 0x0400321F RID: 12831
	public bool useConscientiousnessMatch;

	// Token: 0x04003220 RID: 12832
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	[Range(0f, 1f)]
	[EnableIf("useConscientiousnessMatch")]
	public float matchConscientiousness;

	// Token: 0x04003221 RID: 12833
	public bool useCreativityMatch;

	// Token: 0x04003222 RID: 12834
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	[Range(0f, 1f)]
	[EnableIf("useCreativityMatch")]
	public float matchCreativity;

	// Token: 0x04003223 RID: 12835
	public bool useSocietalClassMatch;

	// Token: 0x04003224 RID: 12836
	[Range(0f, 1f)]
	[EnableIf("useSocietalClassMatch")]
	public float matchSocietalClass;

	// Token: 0x04003225 RID: 12837
	[Range(-1f, 1f)]
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[Header("Effects")]
	public float effectHumility;

	// Token: 0x04003226 RID: 12838
	[Range(-1f, 1f)]
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	public float effectEmotionality;

	// Token: 0x04003227 RID: 12839
	[Range(-1f, 1f)]
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	public float effectExtraversion;

	// Token: 0x04003228 RID: 12840
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	[Range(-1f, 1f)]
	public float effectAgreeableness;

	// Token: 0x04003229 RID: 12841
	[Range(-1f, 1f)]
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	public float effectConscientiousness;

	// Token: 0x0400322A RID: 12842
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	[Range(-1f, 1f)]
	public float effectCreativity;

	// Token: 0x0400322B RID: 12843
	[Space(7f)]
	public float maxHealthModifier;

	// Token: 0x0400322C RID: 12844
	public float recoveryRateModifier;

	// Token: 0x0400322D RID: 12845
	public float combatSkillModifier;

	// Token: 0x0400322E RID: 12846
	public float combatHeftModifier;

	// Token: 0x0400322F RID: 12847
	public float maxNerveModifier;

	// Token: 0x04003230 RID: 12848
	public float breathRecoveryModifier;

	// Token: 0x04003231 RID: 12849
	[Header("Limits")]
	[Tooltip("Honesty-Humility (H): sincere, honest, faithful, loyal, modest/unassuming versus sly, deceitful, greedy, pretentious, hypocritical, boastful, pompous")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 limitHumility = new Vector2(0f, 1f);

	// Token: 0x04003232 RID: 12850
	[Tooltip("Emotionality (E): emotional, oversensitive, sentimental, fearful, anxious, vulnerable versus brave, tough, independent, self-assured, stable")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 limitEmotionality = new Vector2(0f, 1f);

	// Token: 0x04003233 RID: 12851
	[Tooltip("Extraversion (X): outgoing, lively, extraverted, sociable, talkative, cheerful, active versus shy, passive, withdrawn, introverted, quiet, reserved")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 limitExtraversion = new Vector2(0f, 1f);

	// Token: 0x04003234 RID: 12852
	[MinMaxSlider(0f, 1f)]
	[Tooltip("Agreeableness (A): patient, tolerant, peaceful, mild, agreeable, lenient, gentle versus ill-tempered, quarrelsome, stubborn, choleric")]
	public Vector2 limitAgreeableness = new Vector2(0f, 1f);

	// Token: 0x04003235 RID: 12853
	[Tooltip("Conscientiousness (C): organized, disciplined, diligent, careful, thorough, precise versus sloppy, negligent, reckless, lazy, irresponsible, absent-minded")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 limitConscientiousness = new Vector2(0f, 1f);

	// Token: 0x04003236 RID: 12854
	[Tooltip("Openness to Experience (O): intellectual, creative, unconventional, innovative, ironic versus shallow, unimaginative, conventional")]
	[MinMaxSlider(0f, 1f)]
	public Vector2 limitCreativity = new Vector2(0f, 1f);

	// Token: 0x04003237 RID: 12855
	[Header("Slang Pool")]
	[Tooltip("Affect the slang usage...")]
	[Range(-1f, 1f)]
	public float slangUsageModifier;

	// Token: 0x04003238 RID: 12856
	[Tooltip("A default slang greeting to be used on anyone in a casual manor")]
	[ReorderableList]
	public List<string> slangGreetingDefault;

	// Token: 0x04003239 RID: 12857
	[Tooltip("Similar to above, but male specific (eg. 'bro')")]
	[ReorderableList]
	public List<string> slangGreetingMale;

	// Token: 0x0400323A RID: 12858
	[Tooltip("Similar to above, but female specific")]
	[ReorderableList]
	public List<string> slangGreetingFemale;

	// Token: 0x0400323B RID: 12859
	[Tooltip("Slang greeting for a lover")]
	[ReorderableList]
	public List<string> slangGreetingLover;

	// Token: 0x0400323C RID: 12860
	[ReorderableList]
	[Tooltip("Slang curse words")]
	public List<string> slangCurse;

	// Token: 0x0400323D RID: 12861
	[ReorderableList]
	[Tooltip("Slang cursing noun word")]
	public List<string> slangCurseNoun;

	// Token: 0x0400323E RID: 12862
	[ReorderableList]
	[Tooltip("Slang praising noun word")]
	public List<string> slangPraiseNoun;

	// Token: 0x0400323F RID: 12863
	[Header("Culture")]
	[Tooltip("Does this affect the number of books this person should have?")]
	public int preferredBookCountModifier;

	// Token: 0x04003240 RID: 12864
	public int sightingLimitMemoryModifier;

	// Token: 0x020006D4 RID: 1748
	public enum PosNeg
	{
		// Token: 0x04003242 RID: 12866
		postive,
		// Token: 0x04003243 RID: 12867
		neutral,
		// Token: 0x04003244 RID: 12868
		negative
	}

	// Token: 0x020006D5 RID: 1749
	public enum RuleType
	{
		// Token: 0x04003246 RID: 12870
		ifAnyOfThese,
		// Token: 0x04003247 RID: 12871
		ifAllOfThese,
		// Token: 0x04003248 RID: 12872
		ifNoneOfThese,
		// Token: 0x04003249 RID: 12873
		ifPartnerAnyOfThese
	}

	// Token: 0x020006D6 RID: 1750
	[Serializable]
	public class TraitPickRule
	{
		// Token: 0x0400324A RID: 12874
		public CharacterTrait.RuleType rule;

		// Token: 0x0400324B RID: 12875
		public List<CharacterTrait> traitList = new List<CharacterTrait>();

		// Token: 0x0400324C RID: 12876
		[ShowIf("isTrait")]
		[Tooltip("If this isn't true then it won't be picked for application at all.")]
		public bool mustPassForApplication = true;

		// Token: 0x0400324D RID: 12877
		[Tooltip("If the rules match, then apply this to the base chance...")]
		[Range(-1f, 1f)]
		[ShowIf("isTrait")]
		public float baseChance;

		// Token: 0x0400324E RID: 12878
		[Tooltip("Since only one reason is picked, this chance is vs other valid chances...")]
		[Range(0f, 10f)]
		[HideIf("isTrait")]
		public int reasonChance = 5;
	}

	// Token: 0x020006D7 RID: 1751
	[Serializable]
	public class SpecialItemPlacementRule
	{
		// Token: 0x0400324F RID: 12879
		public InteractablePreset preset;

		// Token: 0x04003250 RID: 12880
		[Range(0f, 1f)]
		public float chance = 1f;
	}
}
