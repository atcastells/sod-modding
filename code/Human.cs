using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000049 RID: 73
public class Human : Actor, IComparable<Human>
{
	// Token: 0x060002F8 RID: 760 RVA: 0x0001E560 File Offset: 0x0001C760
	public void SetJob(Occupation newJob)
	{
		if (newJob == null)
		{
			Game.Log("CityGen: Trying to give null job to " + this.humanID.ToString(), 2);
		}
		if (this.job != null)
		{
			this.job.employee = null;
			if (this.job.isOwner)
			{
				this.job.employer.director = null;
				this.director = null;
				this.job.employer.address.RemoveOwner(this);
			}
			if (this.job.preset.receptionist)
			{
				this.job.employer.receptionist = this;
			}
			if (this.job.preset.janitor)
			{
				this.job.employer.janitor = this;
			}
			if (this.job.preset.security)
			{
				this.job.employer.security = this;
			}
			CityData.Instance.assignedJobsDirectory.Remove(this.job);
			if (this.job.employer != null && this.job.employer.address != this.home)
			{
				this.RemoveFromKeyring(this.job.employer.address);
				this.job.employer.address.RemoveInhabitant(this);
			}
			if (this.job.preset.work == OccupationPreset.workType.Enforcer)
			{
				GameplayController.Instance.enforcers.Remove(this);
				this.isEnforcer = false;
			}
			NewAIGoal newAIGoal = this.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.workGoal);
			if (newAIGoal != null)
			{
				newAIGoal.Remove();
			}
			this.RemovePersonalAffect(InteriorControls.Instance.businessCard, false);
			this.RemovePersonalAffect(InteriorControls.Instance.businessCard, true);
			this.RemovePersonalAffect(InteriorControls.Instance.namePlacard, true);
			this.workAffects.Clear();
			this.job = null;
		}
		this.job = newJob;
		this.job.employee = this;
		this.societalClass = Mathf.Clamp01(this.job.paygrade + Toolbox.Instance.GetPsuedoRandomNumber(-0.1f, 0.1f, this.humanID.ToString(), false));
		if (this.job.isOwner && this.job.employer != null)
		{
			this.SetAsDirector(this.job.employer);
		}
		if (this.job.preset.work == OccupationPreset.workType.Enforcer && !GameplayController.Instance.enforcers.Contains(this))
		{
			GameplayController.Instance.enforcers.Add(this);
			this.isEnforcer = true;
		}
		CityData.Instance.assignedJobsDirectory.Add(this.job);
		if (!SessionData.Instance.isFloorEdit && CityConstructor.Instance.generateNew)
		{
			if (this.job.preset.businessCards)
			{
				this.AddPersonalAffect(InteriorControls.Instance.businessCard, false);
				this.AddPersonalAffect(InteriorControls.Instance.businessCard, true);
			}
			if (this.job.preset.namePlacard)
			{
				this.AddPersonalAffect(InteriorControls.Instance.namePlacard, true);
			}
			if (this.job.preset.employeePhoto)
			{
				this.AddPersonalAffect(InteriorControls.Instance.employeePhoto, true);
			}
			if (this.job.preset.workRota)
			{
				this.AddPersonalAffect(InteriorControls.Instance.workRota, false);
			}
			if (this.job.preset.employmentContract)
			{
				this.AddPersonalAffect(InteriorControls.Instance.employmentContractHome, false);
				this.AddPersonalAffect(InteriorControls.Instance.employmentContractWork, true);
			}
			foreach (InteractablePreset interactable in this.job.preset.jobItems)
			{
				this.AddPersonalAffect(interactable, true);
			}
		}
		if (this.job.employer != null && this.job.employer.address != null)
		{
			this.AddToKeyring(this.job.employer.address, true);
			this.job.employer.address.AddInhabitant(this);
			if (!this.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.workGoal))
			{
				this.ai.CreateNewGoal(RoutineControls.Instance.workGoal, 0f, 0f, null, null, null, null, null, -2);
			}
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0001EA14 File Offset: 0x0001CC14
	public void SetSexualityAndGender()
	{
		if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
		{
			this.humanID = Human.assignID;
			Human.assignID++;
			this.seed = Toolbox.Instance.SeedRand(0, 999999999).ToString();
		}
		this.genderScale = (float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.seed, out this.seed) * 0.01f;
		if (this.genderScale < 0.5f - SocialStatistics.Instance.genderNonBinaryThreshold)
		{
			this.gender = Human.Gender.female;
			this.AddCharacterTrait(SocialStatistics.Instance.femaleTrait);
		}
		else if (this.genderScale > 0.5f + SocialStatistics.Instance.genderNonBinaryThreshold)
		{
			this.gender = Human.Gender.male;
			this.AddCharacterTrait(SocialStatistics.Instance.maleTrait);
		}
		else
		{
			this.gender = Human.Gender.nonBinary;
			this.AddCharacterTrait(SocialStatistics.Instance.nbTrait);
		}
		this.sexuality = (float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.seed, out this.seed) * 0.01f;
		this.homosexuality = (float)Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.seed, out this.seed) * 0.01f;
		if (this.sexuality >= SocialStatistics.Instance.sexualityStraightThreshold)
		{
			if (this.gender == Human.Gender.male)
			{
				this.attractedTo.Add(Human.Gender.female);
				this.AddCharacterTrait(SocialStatistics.Instance.AttractedToFemaleTrait);
			}
			else if (this.gender == Human.Gender.female)
			{
				this.attractedTo.Add(Human.Gender.male);
				this.AddCharacterTrait(SocialStatistics.Instance.AttractedToMaleTrait);
			}
		}
		if (this.homosexuality >= SocialStatistics.Instance.sexualityStraightThreshold)
		{
			if (this.gender == Human.Gender.male)
			{
				this.attractedTo.Add(Human.Gender.male);
				this.AddCharacterTrait(SocialStatistics.Instance.AttractedToMaleTrait);
			}
			else if (this.gender == Human.Gender.female)
			{
				this.attractedTo.Add(Human.Gender.female);
				this.AddCharacterTrait(SocialStatistics.Instance.AttractedToFemaleTrait);
			}
		}
		if (this.attractedTo.Count >= 2)
		{
			this.attractedTo.Add(Human.Gender.nonBinary);
			this.AddCharacterTrait(SocialStatistics.Instance.AttractedToNBTrait);
		}
		else if (this.attractedTo.Count <= 0 && Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > SocialStatistics.Instance.asexualChance)
		{
			this.attractedTo.Add(Human.Gender.nonBinary);
			this.AddCharacterTrait(SocialStatistics.Instance.AttractedToNBTrait);
		}
		this.SetBirthGender();
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0001ECA4 File Offset: 0x0001CEA4
	private void SetBirthGender()
	{
		this.birthGender = this.gender;
		if (this.birthGender != Human.Gender.nonBinary)
		{
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) <= SocialStatistics.Instance.transThreshold)
			{
				if (this.birthGender == Human.Gender.male)
				{
					this.birthGender = Human.Gender.female;
					return;
				}
				if (this.birthGender == Human.Gender.female)
				{
					this.birthGender = Human.Gender.male;
					return;
				}
				if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > 0.5f)
				{
					this.birthGender = Human.Gender.female;
					return;
				}
				this.birthGender = Human.Gender.male;
			}
			return;
		}
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > 0.5f)
		{
			this.birthGender = Human.Gender.female;
			return;
		}
		this.birthGender = Human.Gender.male;
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0001ED80 File Offset: 0x0001CF80
	public void GenerateSuitableGenderAndSexualityForParnter(Citizen newPartner)
	{
		if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
		{
			this.humanID = Human.assignID;
			Human.assignID++;
			this.seed = Toolbox.Instance.SeedRand(0, 999999999).ToString();
		}
		List<Human.Gender> list = new List<Human.Gender>(newPartner.attractedTo);
		if (list.Count <= 0)
		{
			list.Add(Human.Gender.female);
			list.Add(Human.Gender.male);
			list.Add(Human.Gender.nonBinary);
		}
		this.gender = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.seed, out this.seed)];
		if (this.gender == Human.Gender.male)
		{
			this.genderScale = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 0.5f + SocialStatistics.Instance.genderNonBinaryThreshold, this.seed, out this.seed);
		}
		else if (this.gender == Human.Gender.female)
		{
			this.genderScale = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 0.5f - SocialStatistics.Instance.genderNonBinaryThreshold, this.seed, out this.seed);
		}
		else
		{
			this.genderScale = Toolbox.Instance.GetPsuedoRandomNumberContained(0.5f - SocialStatistics.Instance.genderNonBinaryThreshold, 0.5f + SocialStatistics.Instance.genderNonBinaryThreshold, this.seed, out this.seed);
		}
		this.genderScale = (float)Mathf.RoundToInt(this.genderScale * 100f) / 100f;
		this.sexuality = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.homosexuality = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		if (newPartner.sexuality >= SocialStatistics.Instance.sexualityStraightThreshold)
		{
			this.sexuality = Toolbox.Instance.GetPsuedoRandomNumberContained(SocialStatistics.Instance.sexualityStraightThreshold, 1f, this.seed, out this.seed);
		}
		if (newPartner.sexuality >= SocialStatistics.Instance.sexualityGayThreshold)
		{
			this.homosexuality = Toolbox.Instance.GetPsuedoRandomNumberContained(SocialStatistics.Instance.sexualityGayThreshold, 1f, this.seed, out this.seed);
		}
		this.sexuality = (float)Mathf.RoundToInt(this.sexuality * 100f) / 100f;
		this.homosexuality = (float)Mathf.RoundToInt(this.homosexuality * 100f) / 100f;
		if (this.sexuality >= SocialStatistics.Instance.sexualityStraightThreshold)
		{
			if (this.gender == Human.Gender.male)
			{
				this.attractedTo.Add(Human.Gender.female);
			}
			else if (this.gender == Human.Gender.female)
			{
				this.attractedTo.Add(Human.Gender.male);
			}
		}
		if (this.homosexuality >= SocialStatistics.Instance.sexualityStraightThreshold)
		{
			if (this.gender == Human.Gender.male)
			{
				this.attractedTo.Add(Human.Gender.male);
			}
			else if (this.gender == Human.Gender.female)
			{
				this.attractedTo.Add(Human.Gender.female);
			}
		}
		if (this.attractedTo.Count >= 2)
		{
			this.attractedTo.Add(Human.Gender.nonBinary);
		}
		else if (this.attractedTo.Count <= 0 && Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > SocialStatistics.Instance.asexualChance)
		{
			this.attractedTo.Add(Human.Gender.nonBinary);
		}
		if (this.genderScale < 0.5f - SocialStatistics.Instance.genderNonBinaryThreshold)
		{
			this.gender = Human.Gender.female;
			this.AddCharacterTrait(SocialStatistics.Instance.femaleTrait);
		}
		else if (this.genderScale > 0.5f + SocialStatistics.Instance.genderNonBinaryThreshold)
		{
			this.gender = Human.Gender.male;
			this.AddCharacterTrait(SocialStatistics.Instance.maleTrait);
		}
		else
		{
			this.gender = Human.Gender.nonBinary;
			this.AddCharacterTrait(SocialStatistics.Instance.nbTrait);
		}
		this.SetBirthGender();
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0001F160 File Offset: 0x0001D360
	public void SetPersonality()
	{
		this.preferredBookCount = SocialControls.Instance.basePreferredBookCount;
		this.sightingMemoryLimit = CitizenControls.Instance.defaultMemoryLimit;
		this.humility = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.emotionality = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.extraversion = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.agreeableness = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.conscientiousness = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.creativity = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		if (this.job != null && this.job.preset != null && this.job.preset.skewPersonalityTowardsJobFit > 0f)
		{
			if (this.job.preset.skewHumility)
			{
				this.humility = Mathf.Lerp(this.humility, this.job.preset.humility, this.job.preset.skewPersonalityTowardsJobFit);
			}
			if (this.job.preset.skewEmotionality)
			{
				this.emotionality = Mathf.Lerp(this.emotionality, this.job.preset.emotionality, this.job.preset.skewPersonalityTowardsJobFit);
			}
			if (this.job.preset.skewExtraversion)
			{
				this.extraversion = Mathf.Lerp(this.extraversion, this.job.preset.extraversion, this.job.preset.skewPersonalityTowardsJobFit);
			}
			if (this.job.preset.skewAgreeableness)
			{
				this.agreeableness = Mathf.Lerp(this.agreeableness, this.job.preset.agreeableness, this.job.preset.skewPersonalityTowardsJobFit);
			}
			if (this.job.preset.skewConscientiousness)
			{
				this.conscientiousness = Mathf.Lerp(this.conscientiousness, this.job.preset.conscientiousness, this.job.preset.skewPersonalityTowardsJobFit);
			}
			if (this.job.preset.skewHumility)
			{
				this.creativity = Mathf.Lerp(this.conscientiousness, this.job.preset.conscientiousness, this.job.preset.skewPersonalityTowardsJobFit);
			}
		}
		this.snoring = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
		this.snoreDelay = 1.5f + Mathf.Clamp01(this.descriptors.heightCM - 1.3406594f) * 1.5f;
		for (int i = 0; i < 4; i++)
		{
			List<CharacterTrait> list = null;
			if (i == 0)
			{
				list = Toolbox.Instance.stage0Traits;
			}
			else if (i == 1)
			{
				list = Toolbox.Instance.stage1Traits;
			}
			else if (i == 2)
			{
				list = Toolbox.Instance.stage2Traits;
			}
			else
			{
				list = Toolbox.Instance.stage3Traits;
			}
			if (list != null && list.Count > 0)
			{
				Toolbox.Instance.ShuffleListSeedContained(ref list, this.seed, out this.seed);
				using (List<CharacterTrait>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterTrait trait = enumerator.Current;
						if (!this.characterTraits.Exists((Human.Trait item) => item.trait == trait) && (!trait.requiresHome || !(this.home == null)) && (!trait.requiresPartner || !(this.partner == null)) && (!trait.requiresSingle || !(this.partner != null)) && (!trait.requiresEmployment || (this.job != null && this.job.employer != null && !this.job.preset.selfEmployed && !(this.job.employer.address == null))) && !trait.disabled && (!trait.isPassword || !(this.passwordTrait != null)))
						{
							float traitChance = this.GetTraitChance(trait);
							if (traitChance > 0f && Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) <= traitChance)
							{
								this.AddCharacterTrait(trait);
							}
						}
					}
				}
			}
		}
		this.preferredBookCount = Mathf.Max(0, this.preferredBookCount);
		List<Human.BookChoice> list2 = new List<Human.BookChoice>();
		this.sightingMemoryLimit = Mathf.Max(1, this.sightingMemoryLimit);
		for (int j = 0; j < Toolbox.Instance.allBooks.Count; j++)
		{
			BookPreset bookPreset = Toolbox.Instance.allBooks[j];
			int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(j, Toolbox.Instance.allBooks.Count, this.seed, out this.seed);
			Toolbox.Instance.allBooks[j] = Toolbox.Instance.allBooks[psuedoRandomNumberContained];
			Toolbox.Instance.allBooks[psuedoRandomNumberContained] = bookPreset;
		}
		foreach (BookPreset bookPreset2 in Toolbox.Instance.allBooks)
		{
			if (!this.library.Contains(bookPreset2) && (bookPreset2.spawnRule == BookPreset.SpawnRules.onlyAtWork || bookPreset2.spawnRule == BookPreset.SpawnRules.secret || !(this.partner != null) || !this.partner.library.Contains(bookPreset2)))
			{
				float num = this.GetChance(ref bookPreset2.pickRules, bookPreset2.baseChance);
				if (num > 0f)
				{
					num *= bookPreset2.common;
					if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) <= num)
					{
						this.library.Add(bookPreset2);
					}
					else
					{
						list2.Add(new Human.BookChoice
						{
							p = bookPreset2,
							rank = num
						});
					}
				}
			}
		}
		list2.Sort((Human.BookChoice p1, Human.BookChoice p2) => p2.rank.CompareTo(p1.rank));
		while (list2.Count > 0 && this.library.Count < this.preferredBookCount)
		{
			this.library.Add(list2[0].p);
			list2.RemoveAt(0);
		}
		this.booksAwayFromShelf = Mathf.RoundToInt((float)this.library.Count * ((1f - this.conscientiousness) * 0.25f));
		this.booksAwayFromShelf = Mathf.Max(1, this.booksAwayFromShelf);
		this.nonShelfBooks.Clear();
		foreach (BookPreset bookPreset3 in this.library)
		{
			DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
			if (Toolbox.Instance.allDDSMessages.TryGetValue(bookPreset3.ddsMessage, ref ddsmessageSave) && ddsmessageSave.blocks != null)
			{
				if (ddsmessageSave.blocks.Exists((DDSSaveClasses.DDSBlockCondition item) => item.group == 2))
				{
					this.nonShelfBooks.Add(bookPreset3);
				}
			}
		}
		this.booksAwayFromShelf = Mathf.Min(this.booksAwayFromShelf, this.nonShelfBooks.Count);
		List<HandwritingPreset> list3 = new List<HandwritingPreset>();
		foreach (HandwritingPreset handwritingPreset in Toolbox.Instance.allHandwriting)
		{
			int num2 = Mathf.CeilToInt(this.GetChance(ref handwritingPreset.characterTraits, handwritingPreset.baseChance) * 10f);
			for (int k = 0; k < num2; k++)
			{
				list3.Add(handwritingPreset);
			}
		}
		if (list3.Count > 0)
		{
			this.handwriting = list3[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list3.Count, this.seed, out this.seed)];
			return;
		}
		this.handwriting = Toolbox.Instance.allHandwriting[Toolbox.Instance.GetPsuedoRandomNumberContained(0, Toolbox.Instance.allHandwriting.Count, this.seed, out this.seed)];
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0001FAE8 File Offset: 0x0001DCE8
	private float GetTraitChance(CharacterTrait trait)
	{
		float num = trait.primeBaseChance;
		bool flag = true;
		foreach (CharacterTrait.TraitPickRule traitPickRule in trait.pickRules)
		{
			bool flag2 = false;
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.TraitExists(searchTrait))
						{
							flag2 = true;
							break;
						}
					}
					goto IL_17A;
				}
				goto IL_77;
			}
			goto IL_77;
			IL_17A:
			if (flag2)
			{
				num += traitPickRule.baseChance;
				continue;
			}
			if (traitPickRule.mustPassForApplication)
			{
				flag = false;
				continue;
			}
			continue;
			IL_77:
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait2 = enumerator2.Current;
						if (!this.TraitExists(searchTrait2))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_17A;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait3 = enumerator2.Current;
						if (this.TraitExists(searchTrait3))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_17A;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (this.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait searchTrait4 = enumerator2.Current;
							if (this.partner.TraitExists(searchTrait4))
							{
								flag2 = true;
								break;
							}
						}
						goto IL_17A;
					}
				}
				flag2 = false;
				goto IL_17A;
			}
			goto IL_17A;
		}
		if (!flag)
		{
			return 0f;
		}
		int num2 = 0;
		float num3 = 0f;
		if (trait.useHumilityMatch)
		{
			num3 += 1f - Mathf.Abs(this.humility - trait.matchHumility);
			num2++;
		}
		if (trait.useExtraversionMatch)
		{
			num3 += 1f - Mathf.Abs(this.extraversion - trait.matchExtraversion);
			num2++;
		}
		if (trait.useEmotionalityMatch)
		{
			num3 += 1f - Mathf.Abs(this.emotionality - trait.matchEmotionality);
			num2++;
		}
		if (trait.useAgreeablenessMatch)
		{
			num3 += 1f - Mathf.Abs(this.agreeableness - trait.matchAgreeableness);
			num2++;
		}
		if (trait.useConscientiousnessMatch)
		{
			num3 += 1f - Mathf.Abs(this.conscientiousness - trait.matchConscientiousness);
			num2++;
		}
		if (trait.useCreativityMatch)
		{
			num3 += 1f - Mathf.Abs(this.creativity - trait.matchCreativity);
			num2++;
		}
		if (trait.useSocietalClassMatch)
		{
			num3 += 1f - Mathf.Abs(this.societalClass - trait.matchSocietalClass);
			num2++;
		}
		if (num3 != 0f && (float)num2 != 0f)
		{
			num3 /= (float)num2;
		}
		if (!float.IsNaN(num3))
		{
			num += num3 * trait.matchChance;
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0001FE74 File Offset: 0x0001E074
	public bool TraitExists(CharacterTrait searchTrait)
	{
		return searchTrait != null && this.characterTraits.Exists((Human.Trait item) => item != null && item.trait != null && item.trait == searchTrait);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0001FEB4 File Offset: 0x0001E0B4
	public float GetChance(ref List<CharacterTrait.TraitPickRule> pickRules, float baseChance)
	{
		bool flag = true;
		foreach (CharacterTrait.TraitPickRule traitPickRule in pickRules)
		{
			bool flag2 = false;
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = true;
							break;
						}
					}
					goto IL_1D1;
				}
				goto IL_82;
			}
			goto IL_82;
			IL_1D1:
			if (flag2)
			{
				baseChance += traitPickRule.baseChance;
				continue;
			}
			if (traitPickRule.mustPassForApplication)
			{
				flag = false;
				continue;
			}
			continue;
			IL_82:
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_1D1;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_1D1;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese && this.partner != null)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = true;
							break;
						}
					}
				}
				goto IL_1D1;
			}
			goto IL_1D1;
		}
		if (!flag)
		{
			return 0f;
		}
		return Mathf.Clamp01(baseChance);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00020154 File Offset: 0x0001E354
	public Human.Trait AddCharacterTrait(CharacterTrait newTrait)
	{
		if (newTrait == null)
		{
			return null;
		}
		Human.Trait trait = new Human.Trait();
		trait.name = newTrait.name;
		trait.traitID = Human.assignTraitID;
		Human.assignTraitID++;
		trait.trait = newTrait;
		this.humility += newTrait.effectHumility;
		this.emotionality += newTrait.effectEmotionality;
		this.extraversion += newTrait.effectExtraversion;
		this.agreeableness += newTrait.effectAgreeableness;
		this.conscientiousness += newTrait.effectConscientiousness;
		this.creativity += newTrait.effectCreativity;
		this.SetMaxHealth(this.maximumHealth + newTrait.maxHealthModifier, false);
		this.SetRecoveryRate(this.recoveryRate + newTrait.recoveryRateModifier);
		this.SetCombatSkill(this.combatSkill + newTrait.combatSkillModifier);
		this.SetCombatHeft(this.combatHeft + newTrait.combatHeftModifier);
		this.SetMaxNerve(this.maxNerve + newTrait.maxNerveModifier, false);
		this.breathRecoveryRate += newTrait.breathRecoveryModifier;
		this.slangUsage += newTrait.slangUsageModifier;
		this.preferredBookCount += newTrait.preferredBookCountModifier;
		this.sightingMemoryLimit += newTrait.sightingLimitMemoryModifier;
		if (newTrait.isPassword)
		{
			this.passwordTrait = newTrait;
		}
		if (newTrait.needsDate)
		{
			if (newTrait.useCouplesAnniversary)
			{
				trait.date = this.anniversary;
			}
			else
			{
				int num = Toolbox.Instance.GetPsuedoRandomNumberContained((int)newTrait.ageDateRange.x, (int)newTrait.ageDateRange.y, this.seed, out this.seed);
				num = Mathf.Min(num, this.GetAge() - 1);
				int num2 = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 12, this.seed, out this.seed) + 1;
				int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(1, SessionData.Instance.daysInMonths[num2 - 1] + 1, this.seed, out this.seed);
				int lowerRange = SessionData.Instance.yearInt - this.GetAge() + num + SessionData.Instance.publicYear;
				int psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(lowerRange, SessionData.Instance.publicYear, this.seed, out this.seed);
				trait.date = string.Concat(new string[]
				{
					num2.ToString(),
					"/",
					psuedoRandomNumberContained.ToString(),
					"/",
					psuedoRandomNumberContained2.ToString()
				});
			}
		}
		this.limitHumility = new Vector2(Mathf.Max(this.limitHumility.x, newTrait.limitHumility.x), Mathf.Min(this.limitHumility.y, newTrait.limitHumility.y));
		this.limitEmotionality = new Vector2(Mathf.Max(this.limitEmotionality.x, newTrait.limitEmotionality.x), Mathf.Min(this.limitEmotionality.y, newTrait.limitEmotionality.y));
		this.limitExtraversion = new Vector2(Mathf.Max(this.limitExtraversion.x, newTrait.limitExtraversion.x), Mathf.Min(this.limitExtraversion.y, newTrait.limitExtraversion.y));
		this.limitAgreeableness = new Vector2(Mathf.Max(this.limitAgreeableness.x, newTrait.limitAgreeableness.x), Mathf.Min(this.limitAgreeableness.y, newTrait.limitAgreeableness.y));
		this.limitConscientiousness = new Vector2(Mathf.Max(this.limitConscientiousness.x, newTrait.limitConscientiousness.x), Mathf.Min(this.limitConscientiousness.y, newTrait.limitConscientiousness.y));
		this.limitCreativity = new Vector2(Mathf.Max(this.limitCreativity.x, newTrait.limitCreativity.x), Mathf.Min(this.limitCreativity.y, newTrait.limitCreativity.y));
		this.humility = Mathf.Clamp(this.humility, this.limitHumility.x, this.limitHumility.y);
		this.emotionality = Mathf.Clamp(this.emotionality, this.limitEmotionality.x, this.limitEmotionality.y);
		this.extraversion = Mathf.Clamp(this.extraversion, this.limitExtraversion.x, this.limitExtraversion.y);
		this.agreeableness = Mathf.Clamp(this.agreeableness, this.limitAgreeableness.x, this.limitAgreeableness.y);
		this.conscientiousness = Mathf.Clamp(this.conscientiousness, this.limitConscientiousness.x, this.limitConscientiousness.y);
		this.creativity = Mathf.Clamp(this.creativity, this.limitCreativity.x, this.limitCreativity.y);
		if (newTrait == CitizenControls.Instance.litterBug)
		{
			this.isLitterBug = true;
		}
		if (newTrait == CitizenControls.Instance.likesTheRain)
		{
			this.likesTheRain = true;
		}
		this.characterTraits.Add(trait);
		if (newTrait.needsReson)
		{
			List<CharacterTrait> list = new List<CharacterTrait>();
			foreach (CharacterTrait characterTrait in Toolbox.Instance.reasons)
			{
				if ((!characterTrait.requiresHome || !(this.home == null)) && (!characterTrait.requiresPartner || !(this.partner == null)) && (!characterTrait.requiresSingle || !(this.partner != null)) && (!characterTrait.requiresEmployment || (this.job != null && this.job.employer != null && !this.job.preset.selfEmployed && !(this.job.employer.address == null))) && !characterTrait.disabled)
				{
					foreach (CharacterTrait.TraitPickRule traitPickRule in characterTrait.pickRules)
					{
						bool flag = false;
						if (traitPickRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
						{
							using (List<CharacterTrait>.Enumerator enumerator3 = traitPickRule.traitList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									CharacterTrait searchTrait = enumerator3.Current;
									if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag = true;
										break;
									}
								}
								goto IL_808;
							}
							goto IL_6AE;
						}
						goto IL_6AE;
						IL_808:
						if (flag)
						{
							for (int i = 0; i < traitPickRule.reasonChance; i++)
							{
								list.Add(characterTrait);
							}
							continue;
						}
						continue;
						IL_6AE:
						if (traitPickRule.rule == CharacterTrait.RuleType.ifAllOfThese)
						{
							flag = true;
							using (List<CharacterTrait>.Enumerator enumerator3 = traitPickRule.traitList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									CharacterTrait searchTrait = enumerator3.Current;
									if (!this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag = false;
										break;
									}
								}
								goto IL_808;
							}
						}
						if (traitPickRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
						{
							flag = true;
							using (List<CharacterTrait>.Enumerator enumerator3 = traitPickRule.traitList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									CharacterTrait searchTrait = enumerator3.Current;
									if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag = false;
										break;
									}
								}
								goto IL_808;
							}
						}
						if (traitPickRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese && this.partner != null)
						{
							using (List<CharacterTrait>.Enumerator enumerator3 = traitPickRule.traitList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									CharacterTrait searchTrait = enumerator3.Current;
									if (this.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag = true;
										break;
									}
								}
							}
							goto IL_808;
						}
						goto IL_808;
					}
				}
			}
			if (list.Count > 0)
			{
				trait.reason = this.AddCharacterTrait(list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.seed, out this.seed)]);
			}
		}
		return trait;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00020A94 File Offset: 0x0001EC94
	public void SetPartner(Citizen newLover)
	{
		this.partner = newLover;
		newLover.partner = (this as Citizen);
		if (newLover.addressBook == null)
		{
			this.addressBook = newLover.addressBook;
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00020AC0 File Offset: 0x0001ECC0
	public virtual void SetResidence(ResidenceController newHome, bool removePreviousResidence = true)
	{
		if (this.residence != null && removePreviousResidence && this.residence != newHome)
		{
			base.RemoveLocationOfAuthority(this.residence.address);
			this.residence.address.RemoveOwner(this);
			this.residence.address.RemoveInhabitant(this);
			this.RemovePersonalAffect(CitizenControls.Instance.toothbrush, false);
		}
		this.residence = newHome;
		if (this.residence != null)
		{
			this.home = this.residence.address;
		}
		else
		{
			this.home = null;
		}
		if (this.home != null)
		{
			base.AddLocationOfAuthorty(this.home);
			if (!this.isPlayer && this.job != null && this.job.employer != null && this.job.employer.preset.isSelfEmployed && this.job.employer.address == null)
			{
				this.job.employer.SetAddress(this.home);
				if (this.job.employer.placeOfBusiness == null)
				{
					this.job.employer.SetPlaceOfBusiness(this.home);
				}
			}
			this.AddToKeyring(this.home, false);
			foreach (NewRoom newRoom in this.home.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (NewWall newWall in newNode.walls)
					{
						if (newWall.door != null)
						{
							this.AddToKeyring(newWall.door, false);
						}
					}
				}
			}
			this.home.AddOwner(this);
			this.home.AddInhabitant(this);
			if (this.job != null && this.job.employer != null && this.job.preset.selfEmployed)
			{
				this.job.employer.SetAddress(this.home);
				if (this.job.employer.placeOfBusiness == null)
				{
					this.job.employer.SetPlaceOfBusiness(this.home);
				}
			}
		}
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00020D78 File Offset: 0x0001EF78
	public virtual void SetDen(NewAddress newAddress)
	{
		if (this.den != null && newAddress != null && this.den != newAddress)
		{
			base.RemoveLocationOfAuthority(this.den);
		}
		this.den = newAddress;
		if (this.den != null)
		{
			base.AddLocationOfAuthorty(this.den);
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00020DD8 File Offset: 0x0001EFD8
	public void UpdateTickRateOnProx()
	{
		if (this.ai != null)
		{
			if (this.currentRoom == Player.Instance.currentRoom)
			{
				this.ai.SetDesiredTickRate(NewAIController.AITickRate.veryHigh, false);
				return;
			}
			if (this.currentGameLocation == Player.Instance.currentGameLocation)
			{
				this.ai.SetDesiredTickRate(NewAIController.AITickRate.high, false);
				return;
			}
			if (this.currentBuilding != null && this.currentBuilding == Player.Instance.currentBuilding && this.currentGameLocation.floor == Player.Instance.currentGameLocation.floor)
			{
				this.ai.SetDesiredTickRate(NewAIController.AITickRate.medium, false);
				return;
			}
			if (this.currentBuilding == Player.Instance.currentBuilding)
			{
				this.ai.SetDesiredTickRate(NewAIController.AITickRate.low, false);
				return;
			}
			this.ai.SetDesiredTickRate(NewAIController.AITickRate.veryLow, false);
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00020EC8 File Offset: 0x0001F0C8
	public void SetupGeneral()
	{
		this.passcode = new GameplayController.Passcode(GameplayController.PasscodeType.citizen);
		this.passcode.id = this.humanID;
		this.CreateEvidence();
		this.favColourIndex = Toolbox.Instance.GetPsuedoRandomNumberContained(0, SocialStatistics.Instance.favouriteColoursPool.Count, this.seed, out this.seed);
		this.SetupInteractables();
		this.descriptors = new Descriptors(this);
		this.slangUsage = Mathf.Clamp01(this.slangUsage);
		this.GenerateSlang();
		this.SetFootwear(Human.ShoeType.barefoot);
		this.SetPhysicalModelParams();
		if ((SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew) && this.partner != null && this.anniversary.Length <= 0)
		{
			int num = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 12, this.seed, out this.seed) + 1;
			int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(1, SessionData.Instance.daysInMonths[num - 1] + 1, this.seed, out this.seed);
			int num2 = SessionData.Instance.yearInt - this.GetAge() + 19 + SessionData.Instance.publicYear;
			num2 = Mathf.Max(num2, SessionData.Instance.yearInt - this.GetAge() + 19 + SessionData.Instance.publicYear);
			int psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(num2, SessionData.Instance.publicYear, this.seed, out this.seed);
			this.anniversary = string.Concat(new string[]
			{
				num.ToString(),
				"/",
				psuedoRandomNumberContained.ToString(),
				"/",
				psuedoRandomNumberContained2.ToString()
			});
			this.partner.anniversary = this.anniversary;
			this.AddCharacterTrait(SocialStatistics.Instance.relationshipTrait);
			this.partner.AddCharacterTrait(SocialStatistics.Instance.relationshipTrait);
		}
		this.GenerateBloodType();
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000210C4 File Offset: 0x0001F2C4
	private void GenerateBloodType()
	{
		int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.seed, out this.seed);
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio)
		{
			this.bloodType = Human.BloodType.Opos;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio)
		{
			this.bloodType = Human.BloodType.Apos;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio + SocialStatistics.Instance.bloodBPosRatio)
		{
			this.bloodType = Human.BloodType.Bpos;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio + SocialStatistics.Instance.bloodBPosRatio + SocialStatistics.Instance.bloodONegRatio)
		{
			this.bloodType = Human.BloodType.Oneg;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio + SocialStatistics.Instance.bloodBPosRatio + SocialStatistics.Instance.bloodONegRatio + SocialStatistics.Instance.bloodANegRatio)
		{
			this.bloodType = Human.BloodType.Aneg;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio + SocialStatistics.Instance.bloodBPosRatio + SocialStatistics.Instance.bloodONegRatio + SocialStatistics.Instance.bloodANegRatio + SocialStatistics.Instance.bloodABPosRatio)
		{
			this.bloodType = Human.BloodType.ABpos;
			return;
		}
		if ((float)psuedoRandomNumberContained < SocialStatistics.Instance.bloodOPosRatio + SocialStatistics.Instance.bloodAPosRatio + SocialStatistics.Instance.bloodBPosRatio + SocialStatistics.Instance.bloodONegRatio + SocialStatistics.Instance.bloodANegRatio + SocialStatistics.Instance.bloodABPosRatio + SocialStatistics.Instance.bloodBNegRatio)
		{
			this.bloodType = Human.BloodType.Bneg;
			return;
		}
		this.bloodType = Human.BloodType.ABneg;
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00021273 File Offset: 0x0001F473
	public string GetBloodTypeString()
	{
		return Strings.Get("descriptors", "bloodtype_" + this.bloodType.ToString(), Strings.Casing.asIs, false, false, false, null);
	}

	// Token: 0x06000308 RID: 776 RVA: 0x000212A0 File Offset: 0x0001F4A0
	public void GenerateSlang()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		List<string> list = new List<string>(SocialStatistics.Instance.slangGreetingDefault);
		List<string> list2 = new List<string>(SocialStatistics.Instance.slangGreetingMale);
		List<string> list3 = new List<string>(SocialStatistics.Instance.slangGreetingFemale);
		List<string> list4 = new List<string>(SocialStatistics.Instance.slangGreetingLover);
		List<string> list5 = new List<string>(SocialStatistics.Instance.slangCurse);
		List<string> list6 = new List<string>(SocialStatistics.Instance.slangCurseNoun);
		List<string> list7 = new List<string>(SocialStatistics.Instance.slangPraiseNoun);
		foreach (Human.Trait trait in this.characterTraits)
		{
			list.AddRange(trait.trait.slangGreetingDefault);
			list2.AddRange(trait.trait.slangGreetingMale);
			list3.AddRange(trait.trait.slangGreetingFemale);
			list4.AddRange(trait.trait.slangGreetingLover);
			list5.AddRange(trait.trait.slangCurse);
			list6.AddRange(trait.trait.slangPraiseNoun);
			list7.AddRange(trait.trait.slangPraiseNoun);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x000213F8 File Offset: 0x0001F5F8
	public void SetPhysicalModelParams()
	{
		float num = this.descriptors.heightCM / SocialStatistics.Instance.averageHeight * CitizenControls.Instance.baseScale;
		this.modelParent.transform.localScale = new Vector3(num, num, num);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00021440 File Offset: 0x0001F640
	public override void CreateEvidence()
	{
		if (this.evidenceEntry == null)
		{
			string newID = "Human" + this.humanID.ToString();
			this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("citizen", newID, this, this, this, null, null, false, null) as EvidenceWitness);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00021490 File Offset: 0x0001F690
	public void CreateDetails()
	{
		if (this.evidenceEntry == null)
		{
			Game.LogError(this.GetCitizenName() + " has no evidence file!", 2);
		}
		if (this.residence != null && this.residence.address.evidenceEntry == null)
		{
			Game.LogError(this.residence.address.name + " has no evidence file!", 2);
		}
		Evidence dateEvidence = EvidenceCreator.Instance.GetDateEvidence(this.birthday, "date", "", -1, -1, -1);
		this.factDictionary.Add("Birthday", EvidenceCreator.Instance.CreateFact("Birthday", this.evidenceEntry, dateEvidence, null, null, false, null, null, null, false));
		if (this.residence != null)
		{
			this.factDictionary.Add("LivesAt", EvidenceCreator.Instance.CreateFact("LivesAt", this.evidenceEntry, this.residence.address.evidenceEntry, null, null, false, null, null, null, false));
			this.factDictionary.Add("LivesAtBuilding", EvidenceCreator.Instance.CreateFact("LivesAtBuilding", this.evidenceEntry, this.residence.address.building.evidenceEntry, null, null, false, null, null, null, false));
			this.factDictionary.Add("LivesOnFloor", EvidenceCreator.Instance.CreateFact("LivesOnFloor", this.evidenceEntry, this.residence.address.building.evidenceEntry, null, null, false, null, null, null, false));
			if (this.home.telephones.Count > 0)
			{
				this.factDictionary.Add("TelephoneNumber", EvidenceCreator.Instance.CreateFact("IsTelephoneNumber", this.home.telephones[0].telephoneEntry, this.evidenceEntry, null, null, false, null, null, null, false));
			}
			if (this.addressBook == null)
			{
				string newID = "AddressBook" + this.humanID.ToString();
				this.addressBook = EvidenceCreator.Instance.CreateEvidence("AddressBook", newID, this, this, this, null, this.home.evidenceEntry, false, null);
				foreach (Human human in this.home.inhabitants)
				{
					human.addressBook = this.addressBook;
				}
				if (this.partner != null)
				{
					this.partner.addressBook = this.addressBook;
				}
				this.setupAddressBook = false;
			}
		}
		if (this.job != null && this.job.employer != null)
		{
			if (this.job.employer.address != null)
			{
				this.factDictionary.Add("WorksAt", EvidenceCreator.Instance.CreateFact("WorksAt", this.evidenceEntry, this.job.employer.address.evidenceEntry, null, null, false, null, null, null, false));
			}
			if (this.job.employer.address.building != null)
			{
				this.factDictionary.Add("WorksAtBuilding", EvidenceCreator.Instance.CreateFact("WorksAtBuilding", this.evidenceEntry, this.job.employer.address.building.evidenceEntry, null, null, false, null, null, null, false));
			}
			this.factDictionary.Add("WorksHours", EvidenceCreator.Instance.CreateFact("WorksHours", this.evidenceEntry, this.job.employer.employeeRoster, null, null, false, null, null, null, false));
			int page = this.job.employer.employeeRoster.AddStringContentToNewPage(this.GetCitizenName() + " — " + Strings.Get("jobs", this.job.preset.name, Strings.Casing.asIs, false, false, false, null), "\n\n", -1);
			this.job.employer.employeeRoster.AddEvidenceDiscoveryToPage(page, this.evidenceEntry, Evidence.Discovery.jobDiscovery);
			string newID2 = "EmployeeRecord" + this.humanID.ToString();
			Evidence evidence = EvidenceCreator.Instance.CreateEvidence("EmployeeRecord", newID2, this, this, this, null, this.job.employer.address.evidenceEntry, false, null);
			this.job.employer.employeeRoster.AddEvidenceToPage(page, evidence);
			evidence.AddFactLink(this.factDictionary["WorksAt"], Evidence.DataKey.name, false);
			evidence.AddFactLink(this.factDictionary["WorksHours"], Evidence.DataKey.name, false);
			newID2 = "WorkID" + this.humanID.ToString();
			this.workID = EvidenceCreator.Instance.CreateEvidence("WorkID", newID2, this, this, this, null, this.home.evidenceEntry, false, null);
		}
		if (this.home != null)
		{
			int page2 = this.home.building.residentRoster.AddStringContentToNewPage(this.GetCitizenName(), "\n\n", -1);
			this.home.building.residentRoster.AddEvidenceDiscoveryToPage(page2, this.evidenceEntry, Evidence.Discovery.livesAt);
			string newID3 = "ResidentFile" + this.humanID.ToString();
			Evidence evidenceToAdd = EvidenceCreator.Instance.CreateEvidence("ResidentFile", newID3, this, this, this, null, this.home.building.evidenceEntry, false, null);
			this.home.building.residentRoster.AddEvidenceToPage(page2, evidenceToAdd);
			this.home.building.residentRoster.AddFactLink(this.factDictionary["LivesAt"], Evidence.DataKey.name, false);
		}
		foreach (KeyValuePair<CompanyPreset.CompanyCategory, NewAddress> keyValuePair in this.favouritePlaces)
		{
			this.factDictionary.Add("Fav" + keyValuePair.Key.ToString(), EvidenceCreator.Instance.CreateFact("Favourite", this.evidenceEntry, keyValuePair.Value.evidenceEntry, null, null, false, null, null, null, false));
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00021AD4 File Offset: 0x0001FCD4
	public void CalculateAge()
	{
		int[] array = new int[]
		{
			6,
			8,
			6,
			5,
			4,
			3,
			2,
			2,
			1,
			1,
			0
		};
		if (this.job != null)
		{
			if (this.job.paygrade <= 0.25f)
			{
				array = new int[]
				{
					3,
					6,
					7,
					6,
					5,
					4,
					3,
					2,
					2,
					1,
					0
				};
			}
			else if (this.job.paygrade > 0.25f && this.job.paygrade <= 0.5f)
			{
				array = new int[]
				{
					1,
					5,
					6,
					7,
					7,
					6,
					4,
					3,
					2,
					1,
					0
				};
			}
			else if (this.job.paygrade > 0.5f && this.job.paygrade <= 0.75f)
			{
				array = new int[]
				{
					0,
					1,
					4,
					6,
					7,
					6,
					5,
					3,
					2,
					1,
					0
				};
			}
			else if (this.job.paygrade > 0.75f)
			{
				array = new int[]
				{
					0,
					0,
					3,
					5,
					7,
					7,
					6,
					4,
					3,
					2,
					1
				};
			}
			if (this.job.work == OccupationPreset.workType.Unemployed)
			{
				array = new int[]
				{
					6,
					7,
					5,
					4,
					3,
					2,
					1,
					1,
					1,
					0,
					0
				};
			}
			else if (this.job.work == OccupationPreset.workType.Retired)
			{
				array = new int[]
				{
					0,
					0,
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					8,
					12
				};
			}
		}
		else
		{
			array = new int[]
			{
				6,
				7,
				5,
				4,
				3,
				2,
				1,
				1,
				1,
				0,
				0
			};
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < SocialStatistics.Instance.ageRanges.Length; i++)
		{
			for (int j = 0; j < array[i]; j++)
			{
				list.Add(SocialStatistics.Instance.ageRanges[i]);
				list2.Add(i);
			}
		}
		int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count - 1, this.seed, out this.seed);
		int psuedoRandomNumberContained2;
		if (list2[psuedoRandomNumberContained] != 10)
		{
			psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(list[psuedoRandomNumberContained], list[psuedoRandomNumberContained] + 5, this.seed, out this.seed);
		}
		else
		{
			psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(list[psuedoRandomNumberContained], 110, this.seed, out this.seed);
		}
		int num = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 12, this.seed, out this.seed) + 1;
		int psuedoRandomNumberContained3 = Toolbox.Instance.GetPsuedoRandomNumberContained(1, SessionData.Instance.daysInMonths[num - 1] + 1, this.seed, out this.seed);
		int num2 = SessionData.Instance.yearInt - psuedoRandomNumberContained2 - 1 + SessionData.Instance.publicYear;
		if (num > SessionData.Instance.monthInt)
		{
			num2++;
		}
		else if (num == SessionData.Instance.monthInt && psuedoRandomNumberContained3 >= SessionData.Instance.dateInt)
		{
			num2++;
		}
		this.birthday = string.Concat(new string[]
		{
			num.ToString(),
			"/",
			psuedoRandomNumberContained3.ToString(),
			"/",
			num2.ToString()
		});
		string text = num.ToString();
		if (text.Length < 2)
		{
			text = "0" + text;
		}
		string text2 = psuedoRandomNumberContained3.ToString();
		if (text2.Length < 2)
		{
			text2 = "0" + text2;
		}
		this.speedMultiplier = Toolbox.Instance.GetPsuedoRandomNumberContained(CitizenControls.Instance.movementSpeedMultiplierRange.x, CitizenControls.Instance.movementSpeedMultiplierRange.y, this.seed, out this.seed);
		this.movementWalkSpeed = this.speedMultiplier * CitizenControls.Instance.baseCitizenWalkSpeed;
		this.movementRunSpeed = this.speedMultiplier * CitizenControls.Instance.baseCitizenRunSpeed;
		this.walkingSpeedRatio = this.movementWalkSpeed / this.movementRunSpeed;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00021E88 File Offset: 0x00020088
	public int GetAge()
	{
		string[] array = this.birthday.Split('/', 0);
		if (array.Length >= 3)
		{
			int num = 0;
			if (!int.TryParse(array[0], ref num))
			{
				Game.LogError("Unable to parse " + array[0] + " to int", 2);
			}
			int num2 = 0;
			if (!int.TryParse(array[1], ref num2))
			{
				Game.LogError("Unable to parse " + array[1] + " to int", 2);
			}
			int num3 = 0;
			if (!int.TryParse(array[2], ref num3))
			{
				Game.LogError("Unable to parse " + array[2] + " to int", 2);
			}
			int num4 = -1;
			if (SessionData.Instance.monthInt > num - 1)
			{
				num4 = 0;
			}
			else if (SessionData.Instance.monthInt == num - 1 && SessionData.Instance.dateInt >= num2 - 1)
			{
				num4 = 0;
			}
			return SessionData.Instance.yearInt - num3 + num4 + SessionData.Instance.publicYear;
		}
		return 25;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00021F74 File Offset: 0x00020174
	public Descriptors.Age GetAgeGroup()
	{
		int age = this.GetAge();
		if (age < 30)
		{
			return Descriptors.Age.youngAdult;
		}
		if (age > 60)
		{
			return Descriptors.Age.old;
		}
		return Descriptors.Age.adult;
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00021F98 File Offset: 0x00020198
	public void PickPassword()
	{
		if (this.passwordTrait == null)
		{
			this.AddCharacterTrait(CitizenControls.Instance.randomPassword);
		}
		this.passcode.digits.Clear();
		if (this.passwordTrait.name == "Secret-Password-Random")
		{
			this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
			this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, this.seed, out this.seed));
		}
		else if (this.passwordTrait.name == "Secret-Password-0451")
		{
			this.passcode.digits.Add(0);
			this.passcode.digits.Add(4);
			this.passcode.digits.Add(5);
			this.passcode.digits.Add(1);
		}
		else if (this.passwordTrait.name == "Secret-Password-Address")
		{
			if (this.home != null && this.home.floor.floor >= 0)
			{
				string text = this.home.residence.GetResidenceString();
				if (text.Length < 4)
				{
					text = "0" + text;
				}
				for (int i = 0; i < 4; i++)
				{
					int num = 0;
					int.TryParse(text.Substring(i, 1), ref num);
					this.passcode.digits.Add(num);
				}
			}
			else
			{
				this.passcode.digits.Add(0);
				this.passcode.digits.Add(0);
				this.passcode.digits.Add(0);
				this.passcode.digits.Add(1);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-AnniversaryDeadFather")
		{
			string[] array = this.characterTraits.Find((Human.Trait item) => item.trait.name == "Event-FatherDiedWhenYoung").date.Split('/', 0);
			string text2 = array[0];
			string text3 = array[1];
			if (text2.Length < 2)
			{
				text2 = "0" + text2;
			}
			if (text3.Length < 2)
			{
				text3 = "0" + text3;
			}
			string text4 = text2 + text3;
			for (int j = 0; j < 4; j++)
			{
				int num2 = 0;
				int.TryParse(text4.Substring(j, 1), ref num2);
				this.passcode.digits.Add(num2);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-AnniversaryDeadMother")
		{
			string[] array2 = this.characterTraits.Find((Human.Trait item) => item.trait.name == "Event-MotherDiedWhenYoung").date.Split('/', 0);
			string text5 = array2[0];
			string text6 = array2[1];
			if (text5.Length < 2)
			{
				text5 = "0" + text5;
			}
			if (text6.Length < 2)
			{
				text6 = "0" + text6;
			}
			string text7 = text5 + text6;
			for (int k = 0; k < 4; k++)
			{
				int num3 = 0;
				int.TryParse(text7.Substring(k, 1), ref num3);
				this.passcode.digits.Add(num3);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-AnniversaryDeadSibling")
		{
			string[] array3 = this.characterTraits.Find((Human.Trait item) => item.trait.name == "Event-SiblingDiedWhenYoung").date.Split('/', 0);
			string text8 = array3[0];
			string text9 = array3[1];
			if (text8.Length < 2)
			{
				text8 = "0" + text8;
			}
			if (text9.Length < 2)
			{
				text9 = "0" + text9;
			}
			string text10 = text8 + text9;
			for (int l = 0; l < 4; l++)
			{
				int num4 = 0;
				int.TryParse(text10.Substring(l, 1), ref num4);
				this.passcode.digits.Add(num4);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-CountDown")
		{
			int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(4, 10, this.seed, out this.seed);
			this.passcode.digits.Add(psuedoRandomNumberContained);
			this.passcode.digits.Add(psuedoRandomNumberContained - 1);
			this.passcode.digits.Add(psuedoRandomNumberContained - 2);
			this.passcode.digits.Add(psuedoRandomNumberContained - 3);
		}
		else if (this.passwordTrait.name == "Secret-Password-CountUp")
		{
			int psuedoRandomNumberContained2 = Toolbox.Instance.GetPsuedoRandomNumberContained(1, 7, this.seed, out this.seed);
			this.passcode.digits.Add(psuedoRandomNumberContained2);
			this.passcode.digits.Add(psuedoRandomNumberContained2 + 1);
			this.passcode.digits.Add(psuedoRandomNumberContained2 + 2);
			this.passcode.digits.Add(psuedoRandomNumberContained2 + 3);
		}
		else if (this.passwordTrait.name == "Secret-Password-DateOfAnniversary")
		{
			string[] array4 = this.anniversary.Split('/', 0);
			string text11 = array4[0];
			string text12 = array4[1];
			if (text11.Length < 2)
			{
				text11 = "0" + text11;
			}
			if (text12.Length < 2)
			{
				text12 = "0" + text12;
			}
			string text13 = text11 + text12;
			for (int m = 0; m < 4; m++)
			{
				int num5 = 0;
				int.TryParse(text13.Substring(m, 1), ref num5);
				this.passcode.digits.Add(num5);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-DateOfBirth")
		{
			string[] array5 = this.birthday.Split('/', 0);
			string text14 = array5[0];
			string text15 = array5[1];
			if (text14.Length < 2)
			{
				text14 = "0" + text14;
			}
			if (text15.Length < 2)
			{
				text15 = "0" + text15;
			}
			string text16 = text14 + text15;
			for (int n = 0; n < 4; n++)
			{
				int num6 = 0;
				int.TryParse(text16.Substring(n, 1), ref num6);
				this.passcode.digits.Add(num6);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-DateOfBirthPartner")
		{
			string[] array6 = this.partner.birthday.Split('/', 0);
			string text17 = array6[0];
			string text18 = array6[1];
			if (text17.Length < 2)
			{
				text17 = "0" + text17;
			}
			if (text18.Length < 2)
			{
				text18 = "0" + text18;
			}
			string text19 = text17 + text18;
			for (int num7 = 0; num7 < 4; num7++)
			{
				int num8 = 0;
				int.TryParse(text19.Substring(num7, 1), ref num8);
				this.passcode.digits.Add(num8);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-EndWork")
		{
			float num9 = SessionData.Instance.FloatMinutes24H(this.job.endTimeDecialHour);
			int num10 = Mathf.FloorToInt(num9);
			string text20 = num10.ToString();
			if (text20.Length == 1)
			{
				text20 = "0" + text20;
			}
			string text21 = Mathf.RoundToInt((num9 - (float)num10) * 100f).ToString();
			if (text21.Length == 1)
			{
				text21 = "0" + text21;
			}
			string text22 = text20 + text21;
			for (int num11 = 0; num11 < 4; num11++)
			{
				int num12 = 0;
				int.TryParse(text22.Substring(num11, 1), ref num12);
				this.passcode.digits.Add(num12);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-ShoeSize")
		{
			string text23 = this.descriptors.shoeSize.ToString();
			while (text23.Length < 4)
			{
				text23 = "0" + text23;
			}
			for (int num13 = 0; num13 < 4; num13++)
			{
				int num14 = 0;
				int.TryParse(text23.Substring(num13, 1), ref num14);
				this.passcode.digits.Add(num14);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-StairsOnWayToWork")
		{
			string text24 = (this.home.floor.floor + this.job.employer.placeOfBusiness.floor.floor).ToString();
			while (text24.Length < 4)
			{
				text24 = "0" + text24;
			}
			for (int num15 = 0; num15 < 4; num15++)
			{
				int num16 = 0;
				int.TryParse(text24.Substring(num15, 1), ref num16);
				this.passcode.digits.Add(num16);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-StartWork")
		{
			float num17 = SessionData.Instance.FloatMinutes24H(this.job.startTimeDecimalHour);
			int num18 = Mathf.FloorToInt(num17);
			string text25 = num18.ToString();
			if (text25.Length == 1)
			{
				text25 = "0" + text25;
			}
			string text26 = Mathf.RoundToInt((num17 - (float)num18) * 100f).ToString();
			if (text26.Length == 1)
			{
				text26 = "0" + text26;
			}
			string text27 = text25 + text26;
			for (int num19 = 0; num19 < 4; num19++)
			{
				int num20 = 0;
				int.TryParse(text27.Substring(num19, 1), ref num20);
				this.passcode.digits.Add(num20);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-YearOfBirth")
		{
			string[] array7 = this.birthday.Split('/', 0);
			for (int num21 = 0; num21 < 4; num21++)
			{
				int num22 = 0;
				int.TryParse(array7[2].Substring(num21, 1), ref num22);
				this.passcode.digits.Add(num22);
			}
		}
		else if (this.passwordTrait.name == "Secret-Password-YearOfBirthPartner")
		{
			string[] array8 = this.partner.birthday.Split('/', 0);
			for (int num23 = 0; num23 < 4; num23++)
			{
				int num24 = 0;
				int.TryParse(array8[2].Substring(num23, 1), ref num24);
				this.passcode.digits.Add(num24);
			}
		}
		if (this.home != null)
		{
			Interactable interactable = this.WriteNote(Human.NoteObject.note, "970c4114-def0-4e04-8982-da36e01f4905", this, this.home, 1, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, null, false, 0, 0, null);
			if (interactable != null)
			{
				this.passcode.notes.Add(interactable.id);
			}
			if (this.job != null && this.job.employer != null && this.job.preset.ownsWorkPosition && this.job.preset.jobPostion == InteractablePreset.SpecialCase.workDesk)
			{
				float num25 = 0.5f;
				if (this.characterTraits.Exists((Human.Trait item) => item.trait.name == "Quirk-Forgetful"))
				{
					num25 = 1f;
				}
				if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) <= num25)
				{
					Interactable interactable2 = this.WriteNote(Human.NoteObject.note, "970c4114-def0-4e04-8982-da36e01f4905", this, this.job.employer.address, 1, InteractablePreset.OwnedPlacementRule.ownedOnly, 0, null, false, 0, 0, null);
					if (interactable2 != null)
					{
						this.passcode.notes.Add(interactable2.id);
					}
				}
			}
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00022C08 File Offset: 0x00020E08
	public virtual void PrepForStart()
	{
		this.ResetHealthToMaximum();
		this.ResetNerveToMaximum();
		if (this.home != null)
		{
			base.AddLocationOfAuthorty(this.home);
			if (!this.setupAddressBook)
			{
				foreach (Human human in this.home.inhabitants)
				{
					human.addressBook = this.addressBook;
				}
				if (this.partner != null)
				{
					this.partner.addressBook = this.addressBook;
				}
				this.setupAddressBook = true;
			}
			using (Dictionary<int, NewFloor>.Enumerator enumerator2 = this.home.building.floors.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, NewFloor> keyValuePair = enumerator2.Current;
					foreach (NewAddress newAddress in keyValuePair.Value.addresses)
					{
						if (newAddress.addressPreset != null && newAddress.addressPreset.sameBuildingResidentsAuthority)
						{
							base.AddLocationOfAuthorty(newAddress);
						}
					}
				}
				goto IL_158;
			}
		}
		if (this.sleepPosition != null && this.sleepPosition.node != null)
		{
			base.AddLocationOfAuthorty(this.sleepPosition.node.gameLocation);
		}
		IL_158:
		if (this.ai != null && this.job != null)
		{
			this.ai.jobDebug = this.job.name;
			if (this.job.employer != null && this.job.employer.placeOfBusiness != null)
			{
				base.AddLocationOfAuthorty(this.job.employer.placeOfBusiness);
				if (this.job.employer.placeOfBusiness.building != null)
				{
					foreach (KeyValuePair<int, NewFloor> keyValuePair2 in this.job.employer.placeOfBusiness.building.floors)
					{
						foreach (NewAddress newAddress2 in keyValuePair2.Value.addresses)
						{
							if (newAddress2.addressPreset != null && newAddress2.addressPreset.sameBuildingEmployeesAuthority)
							{
								base.AddLocationOfAuthorty(newAddress2);
							}
						}
					}
				}
			}
			foreach (DialogPreset dialogPreset in this.job.preset.addDialog)
			{
				if (!(dialogPreset == null))
				{
					this.evidenceEntry.AddDialogOption(dialogPreset.tiedToKey, dialogPreset, null, null, true);
				}
			}
		}
		if (this.sleepPosition == null && this.home != null)
		{
			Game.LogError(string.Concat(new string[]
			{
				"AI: Citizen has no sleep position at their home: ",
				this.GetCitizenName(),
				" (",
				this.home.name,
				")"
			}), 2);
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.generateNew)
		{
			this.PlaceFavouriteItems();
			this.SpawnInventoryItems();
		}
		if (this.likesTheRain)
		{
			this.ownsUmbrella = false;
		}
		else
		{
			float num = 0.25f + this.conscientiousness;
			if (this.isHomeless)
			{
				num -= 0.4f;
			}
			if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, this.GetCitizenName(), false) < num)
			{
				this.ownsUmbrella = true;
			}
			else
			{
				this.ownsUmbrella = false;
			}
		}
		float num2 = 0f;
		if (this.job != null && this.job.employer != null)
		{
			num2 = SessionData.Instance.GetNextOrPreviousGameTimeForThisHour(this.job.workDaysList, this.job.startTimeDecimalHour, this.job.endTimeDecialHour);
		}
		float num3 = num2 - SessionData.Instance.gameTime;
		if (this.home != null && !this.home.actionReference.ContainsKey(RoutineControls.Instance.sleep))
		{
			Game.LogError(string.Concat(new string[]
			{
				"Object: No bed present at ",
				this.home.name,
				", floor ",
				this.home.floor.floorName,
				", building ",
				this.home.building.buildingID.ToString()
			}), 2);
		}
		if (!this.isDead)
		{
			if (this.home != null && this.job != null && this.job.employer != null)
			{
				if (num3 >= 0.25f)
				{
					if (this.home.actionReference.ContainsKey(RoutineControls.Instance.sleep))
					{
						this.Teleport(this.FindSafeTeleport(this.home, false, true), null, true, false);
					}
					else
					{
						this.Teleport(this.FindSafeTeleport(this.home, false, true), null, true, false);
					}
				}
				else if (this.job.employer.placeOfBusiness != null && this.job.employer.placeOfBusiness.rooms.Count > 1)
				{
					this.Teleport(this.FindSafeTeleport(this.job.employer.placeOfBusiness, false, true), null, true, false);
					this.ai.timeAtCurrentAddress = Mathf.Abs(num3);
				}
				else
				{
					this.Teleport(this.FindSafeTeleport(this.home, false, true), null, true, false);
				}
			}
			else if (this.home != null && this.home.actionReference.ContainsKey(RoutineControls.Instance.sleep))
			{
				this.Teleport(this.FindSafeTeleport(this.home, false, true), null, true, false);
			}
			else
			{
				this.Teleport(this.FindSafeTeleport(CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)], false, true), null, true, false);
			}
		}
		float min = 0f;
		float max = 1f;
		if (num3 <= 0f)
		{
			min = 0.5f;
		}
		this.AddNourishment(Toolbox.Instance.Rand(min, max, false));
		this.AddEnergy(Toolbox.Instance.Rand(min, max, false));
		this.AddHydration(Toolbox.Instance.Rand(0.1f, 1f, false));
		this.AddAlertness(Toolbox.Instance.Rand(0.1f, 1f, false));
		this.AddExcitement(Toolbox.Instance.Rand(0.1f, 1f, false));
		this.AddChores(Toolbox.Instance.Rand(0.1f, 1f, false));
		this.AddHygiene(Toolbox.Instance.Rand(min, max, false));
		this.AddBladder(Toolbox.Instance.Rand(0.1f, 1f, false));
		if (this.ai != null)
		{
			this.ai.lastUpdated = SessionData.Instance.gameTime;
			this.ai.idleSound = Toolbox.Instance.Rand(0f, 1f, false);
			this.ai.nourishment = this.nourishment;
			this.ai.hydration = this.hydration;
			this.ai.alertness = this.alertness;
			this.ai.excitement = this.excitement;
			this.ai.energy = this.energy;
			this.ai.chores = this.chores;
			this.ai.hygiene = this.hygiene;
			this.ai.bladder = this.bladder;
			this.ai.heat = this.heat;
			this.ai.drunk = this.drunk;
			this.ai.breath = this.breath;
		}
		this.UpdateGameLocation(0f);
		this.GenerateVocab();
		if (CityConstructor.Instance != null && CityConstructor.Instance.saveState == null)
		{
			this.GeneratePastVmails();
			this.WalletItemCheck(4);
		}
		this.outfitController.SetCurrentOutfit(ClothesPreset.OutfitCategory.casual, true, false, true);
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000234BC File Offset: 0x000216BC
	public void GenerateVocab()
	{
		foreach (KeyValuePair<string, DDSSaveClasses.DDSTreeSave> keyValuePair in Toolbox.Instance.allDDSTrees)
		{
			if ((keyValuePair.Value.treeType == DDSSaveClasses.TreeType.conversation || keyValuePair.Value.treeType == DDSSaveClasses.TreeType.vmail) && (!keyValuePair.Value.participantA.useJobs || (this.job != null && !(this.job.preset == null) && keyValuePair.Value.participantA.jobs.Contains(this.job.preset.name))) && (!keyValuePair.Value.participantA.useTraits || Toolbox.Instance.DDSTraitConditionLogic(this, null, keyValuePair.Value.participantA.traitConditions, ref keyValuePair.Value.participantA.traits)))
			{
				this.AddDDSVocab(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000235DC File Offset: 0x000217DC
	public virtual void AddDDSVocab(DDSSaveClasses.DDSTreeSave newTree)
	{
		if (!this.dds.ContainsKey(newTree.triggerPoint))
		{
			this.dds.Add(newTree.triggerPoint, new List<DDSSaveClasses.DDSTreeSave>());
		}
		this.dds[newTree.triggerPoint].Add(newTree);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0002362C File Offset: 0x0002182C
	public void GeneratePastVmails()
	{
		if (this.dds.ContainsKey(DDSSaveClasses.TriggerPoint.vmail))
		{
			string seedInput = this.humanID.ToString() + CityData.Instance.seed + this.humanID.ToString();
			int num = Toolbox.Instance.RandContained(7, 15, seedInput, out seedInput);
			int num2 = this.dds.Count * 2;
			while (num > 0 && num2 > 0)
			{
				DDSSaveClasses.DDSTreeSave sp = this.dds[DDSSaveClasses.TriggerPoint.vmail][Toolbox.Instance.RandContained(0, this.dds[DDSSaveClasses.TriggerPoint.vmail].Count, seedInput, out seedInput)];
				if (Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput) > sp.treeChance)
				{
					num2--;
				}
				else if (this.messageThreadFeatures.Exists((StateSaveData.MessageThreadSave item) => item.treeID == sp.id))
				{
					num2--;
				}
				else if (this.messageThreadsStarted.Exists((StateSaveData.MessageThreadSave item) => item.treeID == sp.id))
				{
					num2--;
				}
				else
				{
					bool flag = true;
					List<Human> list = new List<Human>();
					for (int i = 0; i < 4; i++)
					{
						if (i == 0)
						{
							if (!this.DDSParticipantConditionCheck(this, sp.participantA, DDSSaveClasses.TreeType.vmail))
							{
								num2--;
								flag = false;
								break;
							}
						}
						else
						{
							DDSSaveClasses.DDSParticipant ddsparticipant = sp.participantB;
							if (i == 2)
							{
								ddsparticipant = sp.participantC;
							}
							if (i == 3)
							{
								ddsparticipant = sp.participantD;
							}
							if (ddsparticipant.required)
							{
								Human human = null;
								if (!Toolbox.Instance.GetVmailParticipant(this, ddsparticipant, list, out human))
								{
									flag = false;
									break;
								}
								list.Add(human);
							}
						}
					}
					if (!flag)
					{
						num2--;
					}
					else
					{
						Toolbox.Instance.NewVmailThread(this, list, sp.id, SessionData.Instance.gameTime + Toolbox.Instance.GetPsuedoRandomNumberContained(-48f, -12f, this.seed, out this.seed), Toolbox.Instance.GetPsuedoRandomNumberContained(0, sp.messageRef.Count + 1, this.seed, out this.seed), StateSaveData.CustomDataSource.sender, -1);
						num--;
					}
				}
			}
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0002386C File Offset: 0x00021A6C
	public void SetupInteractables()
	{
		List<InteractableController> list = Enumerable.ToList<InteractableController>(base.gameObject.GetComponentsInChildren<InteractableController>());
		if (this.interactableController == null)
		{
			this.interactableController = list.Find((InteractableController item) => item.id == InteractableController.InteractableID.A);
		}
		this.interactable = InteractableCreator.Instance.CreateCitizenInteractable(CitizenControls.Instance.citizenInteractable, this, this.interactableController.transform, this.evidenceEntry);
		this.interactableController.Setup(this.interactable);
		if (!SessionData.Instance.isFloorEdit)
		{
			InteractableController interactableController = list.Find((InteractableController item) => item.id == InteractableController.InteractableID.B);
			this.rightHandInteractable = InteractableCreator.Instance.CreateCitizenInteractable(CitizenControls.Instance.handInteractable, this, interactableController.transform, null);
			interactableController.Setup(this.rightHandInteractable);
			InteractableController interactableController2 = list.Find((InteractableController item) => item.id == InteractableController.InteractableID.C);
			this.leftHandInteractable = InteractableCreator.Instance.CreateCitizenInteractable(CitizenControls.Instance.handInteractable, this, interactableController2.transform, null);
			interactableController2.Setup(this.leftHandInteractable);
		}
	}

	// Token: 0x06000315 RID: 789 RVA: 0x000239B8 File Offset: 0x00021BB8
	public void Load(CitySaveData.HumanCitySave data)
	{
		this.humanID = data.humanID;
		this.seed = Toolbox.Instance.SeedRand(0, 999999999).ToString();
		this.CreateEvidence();
		this.SetupInteractables();
		if (!data.homeless && CityData.Instance.addressDictionary.TryGetValue(data.home, ref this.home))
		{
			this.residence = this.home.residence;
			if (this.home.residence != null)
			{
				this.SetResidence(this.residence, true);
			}
		}
		this.slangUsage = data.slangUsage;
		this.favColourIndex = data.favCol;
		this.anniversary = data.anniversary;
		this.speedMultiplier = data.speedModifier;
		this.movementWalkSpeed = this.speedMultiplier * CitizenControls.Instance.baseCitizenWalkSpeed;
		this.movementRunSpeed = this.speedMultiplier * CitizenControls.Instance.baseCitizenRunSpeed;
		this.walkingSpeedRatio = this.movementWalkSpeed / this.movementRunSpeed;
		if (data.job <= 0)
		{
			this.SetJob(CitizenCreator.Instance.CreateUnemployed());
		}
		else
		{
			this.job = CityData.Instance.jobsDirectory.Find((Occupation item) => item.id == data.job);
			if (this.job == null)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Cant find job ",
					data.job.ToString(),
					" for ",
					data.citizenName,
					" using employed. Setting as unemployed."
				}), 2);
				this.SetJob(CitizenCreator.Instance.CreateUnemployed());
			}
			else if (this.job.employee != null && this.job.employee != this)
			{
				Game.LogError(string.Concat(new string[]
				{
					"employee already set for job ",
					data.job.ToString(),
					" for ",
					data.citizenName,
					". Setting as unemployed."
				}), 2);
				this.SetJob(CitizenCreator.Instance.CreateUnemployed());
			}
			else
			{
				this.SetJob(this.job);
			}
		}
		this.societalClass = data.societalClass;
		this.descriptors = data.descriptors;
		this.descriptors.citizen = this;
		this.bloodType = data.blood;
		this.citizenName = data.citizenName;
		base.name = this.citizenName;
		base.transform.name = base.name;
		this.firstName = data.firstName;
		this.casualName = data.casualName;
		this.surName = data.surName;
		this.genderScale = data.genderScale;
		this.gender = data.gender;
		this.birthGender = data.bGender;
		this.attractedTo = data.attractedTo;
		this.homosexuality = data.homosexuality;
		this.sexuality = data.sexuality;
		this.passcode = data.password;
		this.isHomeless = data.homeless;
		if (this.isHomeless)
		{
			CityData.Instance.homelessDirectory.Add(this as Citizen);
		}
		else
		{
			CityData.Instance.homedDirectory.Add(this as Citizen);
		}
		if (data.handwriting != null && data.handwriting.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<HandwritingPreset>(data.handwriting, out this.handwriting);
		}
		this.birthday = data.birthday;
		string[] array = this.birthday.Split('/', 0);
		string text = array[0].ToString();
		if (text.Length < 2)
		{
			text = "0" + text;
		}
		string text2 = array[1].ToString();
		if (text2.Length < 2)
		{
			text2 = "0" + text2;
		}
		this.sleepNeedMultiplier = data.sleepNeedMultiplier;
		this.snoring = data.snoring;
		this.snoreDelay = data.snoreDelay;
		this.humility = data.humility;
		this.emotionality = data.emotionality;
		this.extraversion = data.extraversion;
		this.agreeableness = data.agreeableness;
		this.conscientiousness = data.conscientiousness;
		this.creativity = data.creativity;
		using (List<CitySaveData.CharTraitSave>.Enumerator enumerator = data.traits.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CitySaveData.CharTraitSave traitSave = enumerator.Current;
				Human.Trait trait = new Human.Trait();
				trait.traitID = traitSave.traitID;
				trait.trait = Toolbox.Instance.allCharacterTraits.Find((CharacterTrait item) => item.name == traitSave.trait);
				trait.name = trait.trait.name;
				trait.date = traitSave.date;
				if (trait.trait == CitizenControls.Instance.litterBug)
				{
					this.isLitterBug = true;
				}
				if (trait.trait == CitizenControls.Instance.likesTheRain)
				{
					this.likesTheRain = true;
				}
				this.characterTraits.Add(trait);
				if (trait.trait.isPassword)
				{
					this.passwordTrait = trait.trait;
				}
			}
		}
		using (List<CitySaveData.CharTraitSave>.Enumerator enumerator = data.traits.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CitySaveData.CharTraitSave traitSave = enumerator.Current;
				if (traitSave.reason > -1)
				{
					this.characterTraits.Find((Human.Trait item) => item.traitID == traitSave.traitID).reason = this.characterTraits.Find((Human.Trait item) => item.traitID == traitSave.reason);
				}
			}
		}
		this.outfitController.outfits = new List<CitizenOutfitController.Outfit>(data.outfits);
		this.SetPhysicalModelParams();
		this.SetMaxHealth(data.maxHealth, true);
		this.SetRecoveryRate(data.recoveryRate);
		this.SetCombatHeft(data.combatHeft);
		this.SetCombatSkill(data.combatSkill);
		this.SetMaxNerve(data.maxNerve, true);
		this.breathRecoveryRate = data.breathRecovery;
		this.sightingMemoryLimit = data.sightingMemory;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00024104 File Offset: 0x00022304
	public void LoadAcquaintances(CitySaveData.HumanCitySave data)
	{
		foreach (CitySaveData.AcquaintanceCitySave data2 in data.acquaintances)
		{
			new Acquaintance(data2);
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00024158 File Offset: 0x00022358
	public void LoadFavourites(CitySaveData.HumanCitySave data)
	{
		Human.<>c__DisplayClass174_0 CS$<>8__locals1 = new Human.<>c__DisplayClass174_0();
		CS$<>8__locals1.data = data;
		int k;
		int j;
		for (k = 0; k < CS$<>8__locals1.data.favItems.Count; k = j + 1)
		{
			RetailItemPreset retailItemPreset = Toolbox.Instance.allItems.Find((RetailItemPreset item) => item.name == CS$<>8__locals1.data.favItems[k]);
			this.itemRanking.Add(retailItemPreset, CS$<>8__locals1.data.favItemRanks[k]);
			j = k;
		}
		int i;
		for (i = 0; i < CS$<>8__locals1.data.favCat.Count; i = j + 1)
		{
			NewAddress newAddress = CityData.Instance.addressDirectory.Find((NewAddress item) => item.id == CS$<>8__locals1.data.favAddresses[i]);
			this.favouritePlaces.Add(CS$<>8__locals1.data.favCat[i], newAddress);
			if (!newAddress.favouredCustomers.Contains(this))
			{
				newAddress.favouredCustomers.Add(this);
			}
			j = i;
		}
		this.GenerateRoutineGoals();
	}

	// Token: 0x06000318 RID: 792 RVA: 0x000242B0 File Offset: 0x000224B0
	public void GenerateRoutineGoals()
	{
		foreach (AIGoalPreset aigoalPreset in Toolbox.Instance.allGoals.FindAll((AIGoalPreset item) => item.startingGoal && (item.appliesTo == AIGoalPreset.StartingGoal.all || (item.appliesTo == AIGoalPreset.StartingGoal.homelessOnly && this.isHomeless) || (item.appliesTo == AIGoalPreset.StartingGoal.nonHomelessOnly && !this.isHomeless && (item.appliedToTheseJobs.Count <= 0 || (this.job != null && item.appliedToTheseJobs.Contains(this.job.preset)))))))
		{
			if (aigoalPreset.onlyIfFeaturesItemsAtHome.Count > 0 && this.home != null)
			{
				bool flag = false;
				if (this.home.entrances.Count <= 0)
				{
					Game.LogError(this.home.name + " features no entrances!", 2);
				}
				else
				{
					foreach (InteractablePreset objectType in aigoalPreset.onlyIfFeaturesItemsAtHome)
					{
						float num;
						if (Toolbox.Instance.FindClosestObjectTo(objectType, this.home.entrances[0].worldAccessPoint, null, this.home, null, out num, false) != null)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			this.ai.CreateNewGoal(aigoalPreset, 0f, 0f, null, null, null, null, null, -2);
		}
		foreach (GroupsController.SocialGroup socialGroup in this.groups)
		{
			GroupPreset groupPreset = null;
			if (Toolbox.Instance.groupsDictionary.TryGetValue(socialGroup.preset, ref groupPreset) && groupPreset.meetUpGoal != null)
			{
				NewAddress newPassedGameLocation = null;
				if (socialGroup.meetingPlace > -1)
				{
					newPassedGameLocation = CityData.Instance.addressDirectory[socialGroup.meetingPlace];
				}
				this.ai.CreateNewGoal(groupPreset.meetUpGoal, 0f, groupPreset.meetUpLength, null, null, newPassedGameLocation, socialGroup, null, -2).name = groupPreset.meetUpGoal.name + ": " + groupPreset.name;
			}
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x000244D8 File Offset: 0x000226D8
	public bool TraitGoalTest(AIGoalPreset goalPreset, out float priorityMultiplier)
	{
		priorityMultiplier = 1f;
		bool result = true;
		foreach (AIGoalPreset.GoalModifierRule goalModifierRule in goalPreset.goalModifiers)
		{
			bool flag = false;
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = true;
							break;
						}
					}
					goto IL_1DE;
				}
				goto IL_8D;
			}
			goto IL_8D;
			IL_1DE:
			if (flag)
			{
				priorityMultiplier += goalModifierRule.priorityMultiplier;
				continue;
			}
			if (goalModifierRule.mustPassForApplication)
			{
				result = false;
				continue;
			}
			continue;
			IL_8D:
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1DE;
				}
			}
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = goalModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (this.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1DE;
				}
			}
			if (goalModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (this.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = goalModifierRule.traitList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait searchTrait = enumerator2.Current;
							if (this.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
							{
								flag = true;
								break;
							}
						}
						goto IL_1DE;
					}
				}
				flag = false;
				goto IL_1DE;
			}
			goto IL_1DE;
		}
		return result;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00024778 File Offset: 0x00022978
	public override void SetVisible(bool vis, bool force = false)
	{
		if (Player.Instance == this)
		{
			return;
		}
		if (this.ai != null && this.ai.isRagdoll)
		{
			vis = true;
		}
		if (vis == this.visible)
		{
			return;
		}
		if (vis)
		{
			CitizenBehaviour.Instance.AddToCitizenRenderQueue(this);
		}
		else
		{
			CitizenBehaviour.Instance.RemoveFromCitizenRenderQueue(this);
			if (this.ai != null)
			{
				this.ai.isTripping = false;
				if (this.animationController != null)
				{
					this.animationController.CancelTrip();
				}
			}
		}
		base.SetVisible(vis, force);
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00024814 File Offset: 0x00022A14
	public override void OnGameLocationChange(bool enableSocialSightings = true, bool forceDisableLocationMemory = false)
	{
		base.OnGameLocationChange(true, false);
		if (this.home != null && this.currentGameLocation == this.home)
		{
			this.isHome = true;
		}
		else
		{
			this.isHome = false;
		}
		if (this.job != null && this.job.employer != null && (!this.job.preset.selfEmployed || this.workPosition == null))
		{
			if (this.currentGameLocation == this.job.employer.address)
			{
				this.isAtWork = true;
			}
			else
			{
				this.isAtWork = false;
			}
		}
		if (this.ai != null && this.ai.closeDoorsNormallyAfterLeaving != null && this.ai.closeDoorsNormallyAfterLeaving != this.currentGameLocation && this.ai.closeDoorsNormallyAfterLeaving != this.previousGameLocation)
		{
			this.ai.dontEverCloseDoors = false;
			this.ai.closeDoorsNormallyAfterLeaving = null;
		}
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00024920 File Offset: 0x00022B20
	public override void OnRoomChange()
	{
		base.OnRoomChange();
		if (this.home != null && this.currentRoom.gameLocation == this.home)
		{
			this.isHome = true;
		}
		else
		{
			this.isHome = false;
		}
		bool allowEnforcersEverywhere = false;
		if (this.ai != null && this.ai.currentGoal != null)
		{
			allowEnforcersEverywhere = this.ai.currentGoal.preset.allowEnforcersEverywhere;
		}
		this.UpdateTrespassing(allowEnforcersEverywhere);
		this.UpdateTickRateOnProx();
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000249AC File Offset: 0x00022BAC
	public override void OnNodeChange()
	{
		if (this.job != null && this.job.employer != null && this.job.preset.selfEmployed && this.workPosition != null)
		{
			if (this.currentNode == this.workPosition.node)
			{
				this.isAtWork = true;
			}
			else
			{
				this.isAtWork = false;
			}
		}
		if (this.currentGameLocation != null && this.animationController != null && !this.likesTheRain && this.ownsUmbrella)
		{
			if (this.currentGameLocation.isOutside && SessionData.Instance.currentRain > 0.1f)
			{
				this.animationController.SetUmbrella(true);
			}
			else if (this.animationController.umbrella)
			{
				this.animationController.SetUmbrella(false);
			}
		}
		if (this.ai != null && this.ai.persuitTarget != null)
		{
			this.ai.InstantPersuitCheck(this.ai.persuitTarget);
		}
		base.OnNodeChange();
		if (GameplayController.Instance.trackedNodes.Contains(this.currentNode))
		{
			Descriptors.BuildType buildType = Descriptors.BuildType.average;
			if (this.descriptors != null)
			{
				buildType = this.descriptors.build;
			}
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, string.Concat(new string[]
			{
				this.currentNode.gameLocation.name,
				": ",
				Strings.Get("ui.gamemessage", "Movement Alert", Strings.Casing.asIs, false, false, false, null),
				" [",
				Strings.Get("descriptors", "Build", Strings.Casing.firstLetterCaptial, false, false, false, null),
				": ",
				Strings.Get("descriptors", buildType.ToString(), Strings.Casing.firstLetterCaptial, false, false, false, null),
				"]"
			}), InterfaceControls.Icon.footprint, AudioControls.Instance.motionTrackerPing, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00024BA0 File Offset: 0x00022DA0
	public override bool IsTrespassing(NewRoom room, out int trespassEscalation, out string debugOutput, bool enforcersAllowedEverywhere = true)
	{
		trespassEscalation = 0;
		bool flag = false;
		debugOutput = string.Empty;
		if (room == null)
		{
			return false;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			return false;
		}
		if (enforcersAllowedEverywhere && this.isEnforcer && (this.outfitController.currentOutfit == ClothesPreset.OutfitCategory.work || this.outfitController.currentOutfit == ClothesPreset.OutfitCategory.outdoorsWork))
		{
			if (Game.Instance.collectDebugData)
			{
				debugOutput = "Enforcer allowed everywhere";
			}
			return false;
		}
		if (this.inAirVent)
		{
			return true;
		}
		if (this.isPlayer && room.gameLocation.thisAsAddress != null)
		{
			if (GameplayController.Instance.guestPasses.ContainsKey(room.gameLocation.thisAsAddress))
			{
				if (Game.Instance.collectDebugData)
				{
					debugOutput = "Guest Pass";
				}
				return false;
			}
			try
			{
				if (room.preset.allowedIfGivenCorrectPassword && room.gameLocation.thisAsAddress.addressPreset != null && room.gameLocation.thisAsAddress.addressPreset.needsPassword && GameplayController.Instance.playerKnowsPasswords.Contains(room.gameLocation.thisAsAddress.id))
				{
					if (Game.Instance.collectDebugData)
					{
						debugOutput = "Knows password";
					}
					return false;
				}
			}
			catch
			{
			}
		}
		if (!room.gameLocation.isCrimeScene || !(room.gameLocation.thisAsAddress != null))
		{
			try
			{
				if (room.preset.AIknowPassword && !this.isPlayer && room.gameLocation != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.addressPreset != null && room.gameLocation.thisAsAddress.addressPreset.needsPassword)
				{
					if (Game.Instance.collectDebugData)
					{
						debugOutput = "Allow AI with password";
					}
					return false;
				}
			}
			catch
			{
			}
			try
			{
				if (!this.locationsOfAuthority.Contains(room.gameLocation))
				{
					if (room.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden)
					{
						if (Game.Instance.collectDebugData)
						{
							debugOutput = "Always forbidden";
						}
						flag = true;
						trespassEscalation = room.preset.escalationLevelNormal;
						if (room.preset.escalationLevelAfterHours != room.preset.escalationLevelNormal && room.gameLocation != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.company != null && !room.gameLocation.thisAsAddress.company.openForBusinessActual)
						{
							trespassEscalation = room.preset.escalationLevelAfterHours;
						}
						trespassEscalation += room.gameLocation.GetAdditionalEscalation(this);
					}
					else if (room.preset.forbidden == RoomConfiguration.Forbidden.allowedDuringOpenHours)
					{
						if (room.gameLocation != null && room.gameLocation.thisAsAddress != null)
						{
							if (room.gameLocation.thisAsAddress.company != null)
							{
								if (this.job != null && this.job.employer == room.gameLocation.thisAsAddress.company)
								{
									if (Game.Instance.collectDebugData)
									{
										debugOutput = "Always allow employees";
									}
									flag = false;
								}
								else if (room.gameLocation.thisAsAddress.company.publicFacing && room.gameLocation.thisAsAddress.company.openForBusinessActual && room.gameLocation.thisAsAddress.company.openForBusinessDesired)
								{
									if (Game.Instance.collectDebugData)
									{
										debugOutput = "Public facing & open";
									}
									flag = false;
								}
								else
								{
									if (Game.Instance.collectDebugData)
									{
										debugOutput = "Not public facing or closed for business";
									}
									flag = true;
								}
							}
							if (room.gameLocation.thisAsAddress.addressPreset != null && room.gameLocation.thisAsAddress.addressPreset.openHoursDicatedByAdjoiningCompany)
							{
								if (Game.Instance.collectDebugData)
								{
									debugOutput = "Open hours dictated by adjacent...";
								}
								flag = false;
								using (List<NewNode.NodeAccess>.Enumerator enumerator = room.gameLocation.entrances.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										NewNode.NodeAccess nodeAccess = enumerator.Current;
										if (nodeAccess.walkingAccess)
										{
											if (nodeAccess.toNode.gameLocation != room.gameLocation)
											{
												if (nodeAccess.toNode.gameLocation.thisAsAddress != null && nodeAccess.toNode.gameLocation.thisAsAddress.company != null)
												{
													if (this.job != null && this.job.employer == nodeAccess.toNode.gameLocation.thisAsAddress.company)
													{
														if (Game.Instance.collectDebugData)
														{
															debugOutput = "Adjacent: Always allow employees";
														}
														flag = false;
														break;
													}
													if (nodeAccess.toNode.gameLocation.thisAsAddress.company.publicFacing && nodeAccess.toNode.gameLocation.thisAsAddress.company.openForBusinessActual && nodeAccess.toNode.gameLocation.thisAsAddress.company.openForBusinessDesired)
													{
														if (Game.Instance.collectDebugData)
														{
															debugOutput = "Adjacent: Public facing & open";
														}
														flag = false;
														break;
													}
													if (Game.Instance.collectDebugData)
													{
														debugOutput = "Adjacent: Not public facing or closed for business";
													}
													flag = true;
													break;
												}
											}
											else if (nodeAccess.fromNode.gameLocation != room.gameLocation && nodeAccess.fromNode.gameLocation.thisAsAddress != null && nodeAccess.fromNode.gameLocation.thisAsAddress.company != null)
											{
												if (this.job != null && this.job.employer == nodeAccess.fromNode.gameLocation.thisAsAddress.company)
												{
													if (Game.Instance.collectDebugData)
													{
														debugOutput = "Adjacent: Always allow employees";
													}
													flag = false;
													break;
												}
												if (nodeAccess.fromNode.gameLocation.thisAsAddress.company.publicFacing && nodeAccess.fromNode.gameLocation.thisAsAddress.company.openForBusinessActual && nodeAccess.fromNode.gameLocation.thisAsAddress.company.openForBusinessDesired)
												{
													if (Game.Instance.collectDebugData)
													{
														debugOutput = "Adjacent: Public facing & open";
													}
													flag = false;
													break;
												}
												if (Game.Instance.collectDebugData)
												{
													debugOutput = "Adjacent: Not public facing or closed for business";
												}
												flag = true;
												break;
											}
										}
									}
									goto IL_6E7;
								}
							}
							flag = true;
						}
						IL_6E7:
						trespassEscalation = room.preset.escalationLevelNormal;
						if (flag)
						{
							if (room.preset.escalationLevelAfterHours != room.preset.escalationLevelNormal && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.company != null)
							{
								flag = !room.gameLocation.thisAsAddress.company.openForBusinessActual;
								if (flag)
								{
									trespassEscalation = room.preset.escalationLevelAfterHours;
								}
							}
							trespassEscalation += room.gameLocation.GetAdditionalEscalation(this);
						}
					}
				}
				else if (Game.Instance.collectDebugData)
				{
					debugOutput = "Location of authority";
				}
			}
			catch
			{
			}
			trespassEscalation = Mathf.Clamp(trespassEscalation, 0, 2);
			return flag;
		}
		if (this.isEnforcer && this.isOnDuty)
		{
			if (Game.Instance.collectDebugData)
			{
				debugOutput = "Enforcer on duty";
			}
			return false;
		}
		if (this.isPlayer && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.allowedAtCrimeScenes) >= 1f)
		{
			if (Game.Instance.collectDebugData)
			{
				debugOutput = "Player allowed at crime scene";
			}
			return false;
		}
		trespassEscalation = 0;
		if (this.isPlayer)
		{
			trespassEscalation = 2;
		}
		if (Game.Instance.collectDebugData)
		{
			debugOutput = "CrimeScene";
		}
		return true;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x000253AC File Offset: 0x000235AC
	public void CreateAcquaintances()
	{
		if (this.partner != null)
		{
			this.AddAcquaintance(this.partner, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowLoverRange, this.seed, out this.seed), Acquaintance.ConnectionType.lover, true, false, Acquaintance.ConnectionType.friend, null);
		}
		if (this.home != null)
		{
			using (Dictionary<int, NewFloor>.Enumerator enumerator = this.home.building.floors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, NewFloor> keyValuePair = enumerator.Current;
					foreach (NewAddress newAddress in keyValuePair.Value.addresses)
					{
						if (newAddress.residence != null && newAddress.residence.address.owners.Count > 0)
						{
							foreach (Human human in newAddress.residence.address.owners)
							{
								if (!(human == null) && !(human == this) && !(human == this.partner))
								{
									if (human.home == this.home)
									{
										this.AddAcquaintance(human, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowHousemateRange, this.seed, out this.seed), Acquaintance.ConnectionType.housemate, true, false, Acquaintance.ConnectionType.friend, null);
									}
									else if (newAddress.floor == this.home.floor)
									{
										this.AddAcquaintance(human, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowNeighborRange, this.seed, out this.seed), Acquaintance.ConnectionType.neighbor, true, false, Acquaintance.ConnectionType.friend, null);
									}
									else
									{
										float num = (this.extraversion + human.extraversion) * 0.1f;
										if (human.job != null)
										{
											if (Mathf.Abs(human.job.startTimeDecimalHour - this.job.startTimeDecimalHour) < 1f)
											{
												for (int i = 0; i < human.job.workDaysList.Count; i++)
												{
													if (this.job.workDaysList.Contains(human.job.workDaysList[i]))
													{
														num += 0.05f;
													}
												}
											}
										}
										else if (human.job == null && this.job == null)
										{
											num += 0.02f;
										}
										if (num >= Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed))
										{
											this.AddAcquaintance(human, num, Acquaintance.ConnectionType.familiarResidence, false, false, Acquaintance.ConnectionType.friend, null);
										}
									}
								}
							}
						}
					}
				}
				goto IL_33F;
			}
		}
		foreach (Citizen addC in CityData.Instance.homelessDirectory)
		{
			this.AddAcquaintance(addC, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowNeighborRange, this.seed, out this.seed), Acquaintance.ConnectionType.neighbor, true, false, Acquaintance.ConnectionType.friend, null);
		}
		IL_33F:
		if (this.job != null && this.job.employer != null)
		{
			foreach (Occupation occupation in this.job.employer.companyRoster)
			{
				if (occupation.employee != null && !(occupation.employee == this))
				{
					float num2 = Mathf.Abs(this.job.paygrade - occupation.paygrade);
					if (occupation == this.job.boss)
					{
						this.AddAcquaintance(this.job.boss.employee, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowBossRange, this.seed, out this.seed), Acquaintance.ConnectionType.boss, true, false, Acquaintance.ConnectionType.friend, null);
					}
					else if (occupation.teamID == this.job.teamID)
					{
						this.AddAcquaintance(occupation.employee, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowWorkTeamRange, this.seed, out this.seed), Acquaintance.ConnectionType.workTeam, true, false, Acquaintance.ConnectionType.friend, null);
					}
					else if (num2 < 1f)
					{
						this.AddAcquaintance(occupation.employee, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowWorkRange, this.seed, out this.seed), Acquaintance.ConnectionType.workOther, true, false, Acquaintance.ConnectionType.friend, null);
					}
					else
					{
						this.AddAcquaintance(occupation.employee, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowWorkOtherRange, this.seed, out this.seed), Acquaintance.ConnectionType.familiarWork, false, false, Acquaintance.ConnectionType.friend, null);
					}
				}
			}
			if (this.job.employer.preset.publicFacing && this.job.preset.isPublicFacing)
			{
				foreach (KeyValuePair<CompanyPreset.CompanyCategory, NewAddress> keyValuePair2 in this.favouritePlaces)
				{
					if (keyValuePair2.Value.company != null && keyValuePair2.Value.company.publicFacing)
					{
						List<Occupation> list = keyValuePair2.Value.company.companyRoster.FindAll((Occupation item) => item.preset.isPublicFacing && item.employee != null);
						int num3 = 0;
						foreach (Occupation occupation2 in list)
						{
							if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, (this.extraversion + occupation2.employee.extraversion) * 0.5f, this.seed, out this.seed) <= 0.33f)
							{
								num3++;
								this.AddAcquaintance(occupation2.employee, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowRegularCustomerRange, this.seed, out this.seed), Acquaintance.ConnectionType.regularCustomer, true, false, Acquaintance.ConnectionType.friend, null);
							}
							if (num3 >= 16)
							{
								break;
							}
						}
						Game.Log(string.Concat(new string[]
						{
							"CityGen: Public facing job ",
							this.job.preset.name,
							" for ",
							this.GetCitizenName(),
							" created ",
							num3.ToString(),
							" customer connections"
						}), 2);
					}
				}
			}
		}
		int num4 = Toolbox.Instance.GetPsuedoRandomNumberContained(0, Mathf.RoundToInt(this.extraversion * 5f), this.seed, out this.seed);
		List<Acquaintance> list2 = this.acquaintances.FindAll((Acquaintance item) => item.connections.Contains(Acquaintance.ConnectionType.friend));
		if (list2 != null)
		{
			num4 -= list2.Count;
		}
		int num5 = 100;
		while (num4 > 0 && num5 > 0)
		{
			Citizen citizen = CityData.Instance.citizenDirectory[Toolbox.Instance.GetPsuedoRandomNumberContained(0, CityData.Instance.citizenDirectory.Count, this.seed, out this.seed)];
			Acquaintance acquaintance;
			if (citizen != this && !this.FindAcquaintanceExists(citizen, out acquaintance))
			{
				this.AddAcquaintance(citizen, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowFriendRange, this.seed, out this.seed), Acquaintance.ConnectionType.friend, true, false, Acquaintance.ConnectionType.friend, null);
				num4--;
			}
			num5--;
		}
		if (this.partner != null)
		{
			float num6 = (1f - this.humility + this.emotionality) / 2f;
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) < num6)
			{
				Acquaintance acquaintance2 = null;
				float num7 = -99999f;
				foreach (Acquaintance acquaintance3 in this.acquaintances)
				{
					if (!acquaintance3.connections.Contains(Acquaintance.ConnectionType.lover) && !(acquaintance3.with.paramour != null) && this.attractedTo.Contains(acquaintance3.with.gender) && acquaintance3.with.attractedTo.Contains(this.gender))
					{
						float num8 = (1f - acquaintance3.with.humility + acquaintance3.with.emotionality) / 2f;
						if (acquaintance3.connections.Contains(Acquaintance.ConnectionType.boss))
						{
							num8 += 0.1f;
						}
						if (num8 > num7)
						{
							acquaintance2 = acquaintance3;
							num7 = num8;
						}
					}
				}
				if (acquaintance2 != null)
				{
					Acquaintance acquaintance4 = null;
					if (acquaintance2.with.FindAcquaintanceExists(this, out acquaintance4))
					{
						this.paramour = (acquaintance2.with as Citizen);
						acquaintance2.secretConnection = Acquaintance.ConnectionType.paramour;
						acquaintance2.known = Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowParamourRange, this.seed, out this.seed);
						this.AddCharacterTrait(SocialControls.Instance.paramour);
						acquaintance2.with.paramour = (this as Citizen);
						acquaintance4.secretConnection = Acquaintance.ConnectionType.paramour;
						acquaintance4.known = acquaintance2.known;
						acquaintance2.with.AddCharacterTrait(SocialControls.Instance.paramour);
					}
				}
			}
		}
		this.acquaintances.Sort();
		this.acquaintances.Reverse();
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00025E20 File Offset: 0x00024020
	public bool FindAcquaintanceExists(Human findC, out Acquaintance returnAcq)
	{
		returnAcq = null;
		if (findC == null)
		{
			return false;
		}
		for (int i = 0; i < this.acquaintances.Count; i++)
		{
			if (this.acquaintances[i].from == findC || this.acquaintances[i].with == findC)
			{
				returnAcq = this.acquaintances[i];
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00025E94 File Offset: 0x00024094
	public void AddAcquaintance(Human addC, float known, Acquaintance.ConnectionType newConnection, bool addInverse = true, bool secretConnection = false, Acquaintance.ConnectionType newSecretConnection = Acquaintance.ConnectionType.friend, GroupsController.SocialGroup group = null)
	{
		if (addC == this)
		{
			return;
		}
		if (addC == null)
		{
			return;
		}
		if (addC.evidenceEntry == null)
		{
			return;
		}
		Acquaintance.ConnectionType newSecretConnection2 = newConnection;
		if (secretConnection)
		{
			newSecretConnection2 = newSecretConnection;
		}
		Acquaintance acquaintance = null;
		if (!this.FindAcquaintanceExists(addC, out acquaintance))
		{
			new Acquaintance(this, addC, known, newConnection, newSecretConnection2, group);
		}
		else
		{
			acquaintance.AddConnection(known, newConnection);
			if (group != null)
			{
				acquaintance.group = group;
			}
		}
		if (addInverse)
		{
			acquaintance = null;
			if (!addC.FindAcquaintanceExists(this, out acquaintance))
			{
				new Acquaintance(addC, this, known, newConnection, newSecretConnection2, group);
				return;
			}
			acquaintance.AddConnection(known, newConnection);
			if (group != null)
			{
				acquaintance.group = group;
			}
		}
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00025F2C File Offset: 0x0002412C
	public void AddDetailToDict(string key, Fact det)
	{
		if (det == null)
		{
			return;
		}
		if (det.toEvidence == null)
		{
			Game.LogError(string.Concat(new string[]
			{
				"Detail: Detail ",
				det.name,
				" has no toItem (",
				det.preset.name,
				")"
			}), 2);
		}
		if (!this.factDictionary.ContainsKey(key))
		{
			this.factDictionary.Add(key, det);
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00025FA0 File Offset: 0x000241A0
	public void Murder(Human killer, bool setTimeOfDeath, MurderController.Murder murder, Interactable weapon, float chanceToScream = 1f)
	{
		if (this.isDead)
		{
			return;
		}
		if (this.ai != null)
		{
			this.ai.ResetInvestigate();
		}
		this.interactableController.gameObject.tag = "Untagged";
		Object.Destroy(this.interactableController);
		this.isDead = true;
		this.ai.deadRagdollTimer = 0f;
		this.WakeUp(true);
		this.ai.SetDesiredTickRate(NewAIController.AITickRate.veryLow, false);
		if (Toolbox.Instance.Rand(0f, 1f, false) <= chanceToScream)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.screamEvent, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
			this.speechController.Speak("38de9e2f-0d2c-4f10-ba3b-040ef3b18037", true, true, null, null);
		}
		if (murder != null && weapon != null)
		{
			murder.SetMurderWeaponActual(weapon);
		}
		if (setTimeOfDeath)
		{
			this.death = new Human.Death(this, murder, killer, weapon);
		}
		this.interactableController.lookAtTarget.localPosition = new Vector3(this.interactableController.lookAtTarget.localPosition.x, this.interactableController.lookAtTarget.localPosition.y - 1.08f, this.interactableController.lookAtTarget.localPosition.z);
		Object.Destroy(this.interactableController.coll);
		this.interactableController.enabled = false;
		Transform bodyAnchor = this.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.upperTorso);
		Interactable interactable = InteractableCreator.Instance.CreateCitizenInteractable(CitizenControls.Instance.deadBodySearchInteractable, this, bodyAnchor, this.evidenceEntry);
		InteractableController interactableController = bodyAnchor.gameObject.AddComponent<InteractableController>();
		MeshCollider meshCollider = bodyAnchor.gameObject.AddComponent<MeshCollider>();
		if (!meshCollider.convex)
		{
			meshCollider.convex = true;
		}
		if (meshCollider.convex)
		{
			meshCollider.isTrigger = true;
			meshCollider.convex = true;
		}
		else
		{
			Game.LogError("Unsupported convex collider in " + meshCollider.name, 2);
		}
		interactableController.coll = meshCollider;
		interactableController.Setup(interactable);
		interactable.SetPolymorphicReference(this);
		this.interactable = interactable;
		interactable.UpdateCurrentActions();
		if (this.ai != null)
		{
			this.ai.EnableAI(false);
			this.ai.enabled = false;
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000261D8 File Offset: 0x000243D8
	public void RemoveFromWorld(bool val)
	{
		if (this.ai != null)
		{
			this.ai.queuedActions.Clear();
		}
		this.removedFromWorld = val;
		this.WakeUp(true);
		this.isDead = val;
		if (val)
		{
			base.transform.position = new Vector3(0f, -99f, 0f);
			if (this.ai != null)
			{
				if (this.ai.isRagdoll && this.animationController != null)
				{
					this.animationController.SetRagdoll(false, false);
				}
				this.ai.EnableAI(false);
				this.ai.enabled = false;
				CitizenBehaviour.Instance.updateList.Remove(this.ai);
				CitizenBehaviour.Instance.veryHighTickRate.Remove(this.ai);
				CitizenBehaviour.Instance.highTickRate.Remove(this.ai);
				CitizenBehaviour.Instance.mediumTickRate.Remove(this.ai);
				CitizenBehaviour.Instance.lowTickRate.Remove(this.ai);
				CitizenBehaviour.Instance.veryLowTickRate.Remove(this.ai);
			}
			if (this.animationController != null)
			{
				this.animationController.SetPauseAnimation(true);
				return;
			}
		}
		else
		{
			this.ResetHealthToMaximum();
			this.ResetNerveToMaximum();
			if (this.home != null)
			{
				this.Teleport(this.FindSafeTeleport(this.home, false, true), null, true, false);
			}
			else
			{
				this.Teleport(this.FindSafeTeleport(CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)], false, true), null, true, false);
			}
			if (this.ai != null)
			{
				this.ai.EnableAI(true);
				this.ai.enabled = true;
				this.ai.SetRestrained(false, 0f);
				this.ai.UpdateTickRate(true);
			}
			if (this.animationController != null)
			{
				this.animationController.SetPauseAnimation(false);
			}
		}
	}

	// Token: 0x06000325 RID: 805 RVA: 0x000263FC File Offset: 0x000245FC
	public override void GoToSleep()
	{
		if (this.isAsleep)
		{
			return;
		}
		this.awakenPromt = 0;
		if (this.currentGameLocation != null && this.currentGameLocation.thisAsAddress == this.home)
		{
			this.isAsleep = true;
			this.ResetNerveToMaximum();
			return;
		}
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00026450 File Offset: 0x00024650
	public override void WakeUp(bool forceImmediate = false)
	{
		base.WakeUp(forceImmediate);
		this.awakenPromt = 0;
		if (this.ai != null)
		{
			if (!this.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.awakenGoal))
			{
				this.ai.CreateNewGoal(RoutineControls.Instance.awakenGoal, 0f, 0f, null, null, null, null, null, -2);
				if (forceImmediate)
				{
					NewAIGoal newAIGoal = this.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.sleepGoal);
					if (newAIGoal != null)
					{
						NewAIAction newAIAction = newAIGoal.actions.Find((NewAIAction item) => item.preset == RoutineControls.Instance.sleep);
						if (newAIAction != null)
						{
							newAIAction.dontUpdateGoalPriorityForExtraTime = 0f;
						}
					}
				}
				this.ai.AITick(false, false);
			}
		}
		if (this.ai != null && !this.isDead)
		{
			if (this.visible)
			{
				if (this.genderScale >= 0.5f)
				{
					AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.maleYawn, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
				}
				else
				{
					AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.femaleYawn, this, this.currentNode, this.lookAtThisTransform.position, null, null, 1f, null, false, null, false);
				}
			}
			this.speechController.TriggerBark(SpeechController.Bark.yawn);
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x000265F7 File Offset: 0x000247F7
	public virtual void AddNourishment(float addVal)
	{
		this.nourishment += addVal;
		this.nourishment = Mathf.Clamp01(this.nourishment);
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00026618 File Offset: 0x00024818
	public virtual void AddHydration(float addVal)
	{
		this.hydration += addVal;
		this.hydration = Mathf.Clamp01(this.hydration);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00026639 File Offset: 0x00024839
	public virtual void AddAlertness(float addVal)
	{
		this.alertness += addVal;
		this.alertness = Mathf.Clamp01(this.alertness);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0002665A File Offset: 0x0002485A
	public virtual void AddEnergy(float addVal)
	{
		this.energy += addVal;
		this.energy = Mathf.Clamp01(this.energy);
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0002667B File Offset: 0x0002487B
	public virtual void AddExcitement(float addVal)
	{
		this.excitement += addVal;
		this.excitement = Mathf.Clamp01(this.excitement);
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0002669C File Offset: 0x0002489C
	public virtual void AddChores(float addVal)
	{
		this.chores += addVal;
		this.chores = Mathf.Clamp01(this.chores);
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000266BD File Offset: 0x000248BD
	public virtual void AddHygiene(float addVal)
	{
		this.hygiene += addVal;
		this.hygiene = Mathf.Clamp01(this.hygiene);
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000266DE File Offset: 0x000248DE
	public void AddBladder(float addVal)
	{
		this.bladder += addVal;
		this.bladder = Mathf.Clamp01(this.bladder);
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00026700 File Offset: 0x00024900
	public void AddBreath(float addVal)
	{
		this.breath += addVal;
		this.breath = Mathf.Clamp01(this.breath);
		if (this.ai != null)
		{
			if (this.breath >= 1f && this.ai.outOfBreath)
			{
				this.ai.SetOutOfBreath(false);
				return;
			}
			if (this.breath <= 0f && !this.ai.outOfBreath)
			{
				this.ai.SetOutOfBreath(true);
			}
		}
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00026787 File Offset: 0x00024987
	public virtual void AddHeat(float addVal)
	{
		this.heat += addVal;
		this.heat = Mathf.Clamp01(this.heat);
	}

	// Token: 0x06000331 RID: 817 RVA: 0x000267A8 File Offset: 0x000249A8
	public virtual void AddDrunk(float addVal)
	{
		this.drunk += addVal;
		this.drunk = Mathf.Clamp01(this.drunk);
	}

	// Token: 0x06000332 RID: 818 RVA: 0x000267C9 File Offset: 0x000249C9
	public virtual void AddSick(float addVal)
	{
		this.sick += addVal;
		this.sick = Mathf.Clamp01(this.sick);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x000267EA File Offset: 0x000249EA
	public virtual void AddHeadache(float addVal)
	{
		this.headache += addVal;
		this.headache = Mathf.Clamp01(this.headache);
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0002680B File Offset: 0x00024A0B
	public virtual void AddWet(float addVal)
	{
		this.wet += addVal;
		this.wet = Mathf.Clamp01(this.wet);
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0002682C File Offset: 0x00024A2C
	public virtual void AddBrokenLeg(float addVal)
	{
		this.brokenLeg += addVal;
		this.brokenLeg = Mathf.Clamp01(this.brokenLeg);
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0002684D File Offset: 0x00024A4D
	public virtual void AddBruised(float addVal)
	{
		this.bruised += addVal;
		this.bruised = Mathf.Clamp01(this.bruised);
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0002686E File Offset: 0x00024A6E
	public virtual void AddBlackEye(float addVal)
	{
		this.blackEye += addVal;
		this.blackEye = Mathf.Clamp01(this.blackEye);
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0002688F File Offset: 0x00024A8F
	public virtual void AddBlackedOut(float addVal)
	{
		this.blackedOut += addVal;
		this.blackedOut = Mathf.Clamp01(this.blackedOut);
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000268B0 File Offset: 0x00024AB0
	public virtual void AddNumb(float addVal)
	{
		this.numb += addVal;
		this.numb = Mathf.Clamp01(this.numb);
	}

	// Token: 0x0600033A RID: 826 RVA: 0x000268D4 File Offset: 0x00024AD4
	public virtual void AddPoisoned(float addVal, Human byWho)
	{
		if (addVal > 0f)
		{
			this.poisoner = byWho;
			if (this.ai != null)
			{
				this.ai.StatusStatUpdate();
			}
		}
		this.poisoned += addVal;
		this.poisoned = Mathf.Clamp01(this.poisoned);
		if (this.poisoned <= 0f)
		{
			this.poisoner = null;
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002693C File Offset: 0x00024B3C
	public virtual void AddBleeding(float addVal)
	{
		this.bleeding += addVal;
		this.bleeding = Mathf.Clamp01(this.bleeding);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0002695D File Offset: 0x00024B5D
	public virtual void AddBlinded(float addVal)
	{
		this.blinded += addVal;
		this.blinded = Mathf.Clamp01(this.blinded);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0002697E File Offset: 0x00024B7E
	public virtual void AddStarchAddiction(float addVal)
	{
		this.starchAddiction += addVal;
		this.starchAddiction = Mathf.Clamp01(this.starchAddiction);
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0002699F File Offset: 0x00024B9F
	public virtual void AddWellRested(float addVal)
	{
		this.wellRested += addVal;
		this.wellRested = Mathf.Clamp01(this.wellRested);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x000269C0 File Offset: 0x00024BC0
	public virtual void AddSyncDiskInstall(float addVal)
	{
		this.syncDiskInstall += addVal;
		this.syncDiskInstall = Mathf.Clamp01(this.syncDiskInstall);
	}

	// Token: 0x06000340 RID: 832 RVA: 0x000269E1 File Offset: 0x00024BE1
	public void SetAsDirector(Company newComp)
	{
		newComp.director = this;
		this.director = newComp;
		if (newComp.address != null)
		{
			newComp.address.AddOwner(this);
		}
	}

	// Token: 0x06000341 RID: 833 RVA: 0x00026A0C File Offset: 0x00024C0C
	public virtual void SetFootwear(Human.ShoeType newType)
	{
		this.footwear = newType;
		if (this.footwear == Human.ShoeType.normal)
		{
			this.footstepEvent = AudioControls.Instance.footstepShoe;
		}
		else if (this.footwear == Human.ShoeType.boots)
		{
			this.footstepEvent = AudioControls.Instance.footstepBoot;
		}
		else if (this.footwear == Human.ShoeType.heel)
		{
			this.footstepEvent = AudioControls.Instance.footstepHeel;
		}
		if (this.isPlayer)
		{
			InterfaceController.Instance.footstepAudioIndicator.SetSoundEvent(this.footstepEvent, true);
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00026A8C File Offset: 0x00024C8C
	public void OnFootstep(bool isRight)
	{
		if (!SessionData.Instance.play || !SessionData.Instance.startedGame)
		{
			return;
		}
		AudioController.Instance.PlayWorldFootstep(this.footstepEvent, this, false);
		if (this.isOnStreet || this.currentGameLocation.isOutside)
		{
			this.footstepDirt += GameplayControls.Instance.outdoorStepDirtAccumulation;
		}
		this.footstepDirt += GameplayControls.Instance.stepDirtRemoval;
		this.footstepBlood += GameplayControls.Instance.stepBloodRemoval;
		if (!(this.currentRoom != null))
		{
			this.footstepDirt = Mathf.Clamp01(this.footstepDirt);
			this.footstepBlood = Mathf.Clamp01(this.footstepBlood);
			return;
		}
		if (this.currentRoom.containsDead)
		{
			foreach (Actor actor in this.currentRoom.currentOccupants)
			{
				if (actor.isDead && actor.currentNode == this.currentNode)
				{
					float num = Vector3.Distance(actor.transform.position, base.transform.position);
					this.footstepBlood += Mathf.Lerp(1f, 0.25f, num);
				}
			}
		}
		this.footstepDirt += this.currentRoom.floorMaterial.affectFootprintDirt;
		this.footstepDirt += this.currentRoom.floorMatKey.grubiness * this.currentRoom.floorMaterial.grubFootprintDirtMultiplier;
		this.footstepDirt = Mathf.Clamp01(this.footstepDirt);
		this.footstepBlood = Mathf.Clamp01(this.footstepBlood);
		if (!this.currentRoom.floorMaterial.allowFootprints)
		{
			return;
		}
		if (this.footwear == Human.ShoeType.barefoot)
		{
			return;
		}
		if (this.isOnStreet)
		{
			return;
		}
		if (Mathf.Max(this.footstepDirt, this.footstepBlood) <= 0.1f)
		{
			return;
		}
		Vector3 position = this.leftFoot.transform.position;
		if (isRight)
		{
			position = this.rightFoot.transform.position;
		}
		if (this.currentNode != null)
		{
			if (this.currentNode.tile.stairwell)
			{
				return;
			}
			position.y = this.currentNode.position.y;
			if (!this.currentNode.HasValidFloor())
			{
				return;
			}
		}
		if (GameplayController.Instance.activeFootprints.ContainsKey(this.currentRoom))
		{
			while (GameplayController.Instance.activeFootprints[this.currentRoom].Count > GameplayControls.Instance.maximumFootprintsPerRoom)
			{
				GameplayController.Instance.footprintsList.Remove(GameplayController.Instance.activeFootprints[this.currentRoom][0]);
				GameplayController.Instance.activeFootprints[this.currentRoom].RemoveAt(0);
			}
		}
		new GameplayController.Footprint(this, position, base.transform.eulerAngles, this.footstepDirt, this.footstepBlood, null);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00026DB4 File Offset: 0x00024FB4
	public void AddPersonalAffect(InteractablePreset interactable, bool isWork = false)
	{
		if (SessionData.Instance.isFloorEdit || !CityConstructor.Instance.generateNew)
		{
			return;
		}
		if (interactable == null)
		{
			Game.LogError("Tried to add null interactable affect for " + this.GetCitizenName(), 2);
			return;
		}
		if (isWork)
		{
			if (this.job != null && this.job.employer != null && this.job.employer.address != null && this.job.employer.address != this.home)
			{
				this.workAffects.Add(interactable);
				return;
			}
			if (this.job != null && this.job.employer != null && (this.job.employer.address == null || this.job.employer.address == this.home))
			{
				this.personalAffects.Add(interactable);
				return;
			}
		}
		else
		{
			this.personalAffects.Add(interactable);
		}
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00026EBC File Offset: 0x000250BC
	public void RemovePersonalAffect(InteractablePreset interactable, bool isWork = false)
	{
		if (SessionData.Instance.isFloorEdit || !CityConstructor.Instance.generateNew)
		{
			return;
		}
		if (interactable == null)
		{
			Game.LogError("Tried to remove null interactable affect for " + this.GetCitizenName(), 2);
			return;
		}
		if (isWork)
		{
			this.workAffects.Remove(interactable);
			return;
		}
		this.personalAffects.Remove(interactable);
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00026F20 File Offset: 0x00025120
	public NewNode FindSafeTeleport(NewGameLocation gameLoc, bool prioritiseWindows = false, bool allowTrespass = true)
	{
		NewNode newNode = null;
		float num = -999999f;
		foreach (NewRoom newRoom in gameLoc.rooms)
		{
			if (gameLoc.thisAsStreet == null)
			{
				if (newRoom.isNullRoom || newRoom.preset == CityControls.Instance.outsideLayoutConfig || newRoom.isBaseNullRoom || newRoom.entrances.Count <= 0)
				{
					continue;
				}
				if (!newRoom.entrances.Exists((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door || item.accessType == NewNode.NodeAccess.AccessType.openDoorway || item.accessType == NewNode.NodeAccess.AccessType.adjacent))
				{
					continue;
				}
			}
			int count;
			string text;
			if (newRoom.nodes.Count > 1 && (allowTrespass || !this.IsTrespassing(newRoom, out count, out text, true)))
			{
				float num2 = -99999f;
				NewNode newNode2 = this.FindSafeTeleport(newRoom, out num2, prioritiseWindows);
				if (num2 > num || newNode == null)
				{
					num = num2;
					newNode = newNode2;
				}
			}
		}
		if (newNode != null)
		{
			return newNode;
		}
		if (gameLoc.nodes.Count > 0)
		{
			string[] array = new string[5];
			array[0] = "Unable to find safe teleport for ";
			array[1] = ((gameLoc != null) ? gameLoc.ToString() : null);
			array[2] = " (no valid nodes exist, so picking random from a pool of ";
			int num3 = 3;
			int count = gameLoc.nodes.Count;
			array[num3] = count.ToString();
			array[4] = ")";
			Game.LogError(string.Concat(array), 2);
			return gameLoc.nodes[Toolbox.Instance.Rand(0, gameLoc.nodes.Count, false)];
		}
		Game.LogError("Unable to find safe teleport for " + ((gameLoc != null) ? gameLoc.ToString() : null) + " (no nodes exist at this location!)", 2);
		return null;
	}

	// Token: 0x06000346 RID: 838 RVA: 0x000270D8 File Offset: 0x000252D8
	public NewNode FindSafeTeleport(NewRoom room, bool prioritiseWindows = false)
	{
		float num;
		return this.FindSafeTeleport(room, out num, prioritiseWindows);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x000270F0 File Offset: 0x000252F0
	public NewNode FindSafeTeleport(NewRoom room, out float bestScore, bool prioritiseWindows = false)
	{
		NewNode newNode = null;
		bestScore = -9999f;
		bool flag = false;
		if (prioritiseWindows && this.ai != null && this.ai.victimsForMurders.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) >= 0.5f)
		{
			flag = true;
		}
		foreach (NewNode newNode2 in room.nodes)
		{
			if (!newNode2.noAccess && newNode2.accessToOtherNodes.Count > 0 && (!(newNode2.room.gameLocation.thisAsAddress != null) || newNode2.floorType == NewNode.FloorTileType.floorOnly || newNode2.floorType == NewNode.FloorTileType.floorAndCeiling))
			{
				float num = Toolbox.Instance.Rand(0f, 2f, true);
				bool flag2 = false;
				foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode2.accessToOtherNodes)
				{
					if (keyValuePair.Value.walkingAccess)
					{
						flag2 = true;
						if (!prioritiseWindows)
						{
							num += 0.1f;
							break;
						}
						break;
					}
				}
				if (flag2)
				{
					if (prioritiseWindows)
					{
						foreach (NewWall newWall in newNode2.room.windows)
						{
							if (newNode2.walls.Contains(newWall) || newNode2.walls.Contains(newWall.otherWall))
							{
								num += 1.75f;
							}
						}
					}
					if (newNode2.walls.Exists((NewWall item) => item.door != null) && !prioritiseWindows)
					{
						num -= 1.2f;
					}
					if (!prioritiseWindows)
					{
						foreach (FurnitureLocation furnitureLocation in newNode2.individualFurniture)
						{
							if (furnitureLocation.furnitureClasses.Count > 0 && furnitureLocation.furnitureClasses[0].occupiesTile)
							{
								num -= 0.5f;
							}
						}
						if (newNode2.noPassThrough)
						{
							num -= 3f;
						}
						num -= (float)newNode2.occupiedSpace.Count * 0.1f;
					}
					else if (this.ai != null && flag && this.ai.victimsForMurders.Count > 0)
					{
						num += Mathf.Max(30f - Vector3.Distance(base.transform.position, this.ai.victimsForMurders[0].murderer.transform.position), 0f) / 15f;
					}
					if (num > bestScore)
					{
						bestScore = num;
						newNode = newNode2;
					}
				}
			}
		}
		if (newNode != null)
		{
			int num2;
			string text;
			if (!prioritiseWindows && this.IsTrespassing(room, out num2, out text, true))
			{
				bestScore -= 5f;
			}
			return newNode;
		}
		Game.LogError("Unable to find safe teleport for " + ((room != null) ? room.ToString() : null) + " (no nodes exist at this location!)", 2);
		return null;
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0002748C File Offset: 0x0002568C
	public void GenerateItemFavs()
	{
		new List<NewAddress>();
		foreach (RetailItemPreset retailItemPreset in Toolbox.Instance.allItems)
		{
			if (retailItemPreset.canBeFavourite && retailItemPreset.minimumWealth <= this.societalClass)
			{
				bool flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = retailItemPreset.cantFeatureTrait.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait blockedTrait = enumerator2.Current;
						if (this.characterTraits.Exists((Human.Trait item) => item.trait == blockedTrait))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = retailItemPreset.mustFeatureTraits.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait blockedTrait = enumerator2.Current;
							if (!this.characterTraits.Exists((Human.Trait item) => item.trait == blockedTrait))
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						int num = Toolbox.Instance.GetPsuedoRandomNumberContained(0, 50, this.seed, out this.seed);
						using (List<CharacterTrait>.Enumerator enumerator2 = retailItemPreset.preferredTraits.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CharacterTrait blockedTrait = enumerator2.Current;
								if (this.characterTraits.Exists((Human.Trait item) => item.trait == blockedTrait))
								{
									num += 20;
								}
							}
						}
						foreach (Descriptors.EthnicitySetting ethnicitySetting in this.descriptors.ethnicities)
						{
							if (retailItemPreset.ethnicity.Contains(ethnicitySetting.group))
							{
								num += Mathf.RoundToInt(ethnicitySetting.ratio * 10f);
							}
						}
						this.itemRanking.Add(retailItemPreset, num);
					}
				}
			}
		}
		for (int i = 0; i < Toolbox.Instance.allCompanyCategories.Count; i++)
		{
			CompanyPreset.CompanyCategory cat = Toolbox.Instance.allCompanyCategories[i];
			List<Company> list = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset != null && item.preset.companyCategories.Contains(cat));
			if (list.Count > 0)
			{
				Company company = null;
				int num2 = -99999;
				foreach (Company company2 in list)
				{
					if (!(company2.placeOfBusiness.thisAsAddress == null) && company2.publicFacing)
					{
						int num3 = 0;
						foreach (MenuPreset menuPreset in company2.preset.menus)
						{
							foreach (InteractablePreset interactablePreset in menuPreset.itemsSold)
							{
								if (interactablePreset.retailItem == null)
								{
									Game.LogError(interactablePreset.name + " has no retail item config...", 2);
								}
								else if (this.itemRanking.ContainsKey(interactablePreset.retailItem))
								{
									num3 += this.itemRanking[interactablePreset.retailItem];
								}
							}
						}
						if (num3 != 0 && company2.preset.menus.Count != 0)
						{
							num3 /= company2.preset.menus.Count;
							num3 = Mathf.RoundToInt((float)num3 * Mathf.Lerp(0.33f, 1f, (float)(3 - company2.preset.companyCategories.IndexOf(cat)) / 3f));
							int num4 = 0;
							if (this.home != null && company2 != null && company2.placeOfBusiness != null && company2.placeOfBusiness.nodes.Count > 0 && this.home.nodes.Count > 0)
							{
								num4 = 100 - Mathf.RoundToInt(Vector3.Distance(company2.placeOfBusiness.nodes[0].position, this.home.nodes[0].position) * 7f);
							}
							int num5 = 0;
							if (this.job.employer != null && this.home != null && company2.placeOfBusiness != null && company2.placeOfBusiness.nodes.Count > 0 && this.home.nodes.Count > 0)
							{
								num5 = 100 - Mathf.RoundToInt(Vector3.Distance(company2.placeOfBusiness.nodes[0].position, this.home.nodes[0].position) * 7f);
							}
							int num6 = num3 * 2 + num4 * 2 + num5 * 2;
							if (this.job.employer == company2)
							{
								num6 -= 1000;
							}
							if (num6 > num2)
							{
								company = company2;
								num2 = num6;
							}
						}
					}
				}
				if (company != null)
				{
					this.favouritePlaces.Add(cat, company.address);
					if (!company.address.favouredCustomers.Contains(this))
					{
						company.address.favouredCustomers.Add(this);
					}
				}
			}
		}
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00027AFC File Offset: 0x00025CFC
	public void SpawnInventoryItems()
	{
		foreach (CitizenControls.StartingInventory startingInventory in CitizenControls.Instance.citizenStartingInventory)
		{
			float num = startingInventory.baseChance;
			float num2 = 0f;
			if (MurderController.Instance.TraitTest(this as Citizen, ref startingInventory.modifiers, out num2))
			{
				num += num2;
				if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) <= num)
				{
					InteractablePreset preset = startingInventory.presets[Toolbox.Instance.GetPsuedoRandomNumberContained(0, startingInventory.presets.Count, this.seed, out this.seed)];
					InteractableCreator.Instance.CreateWorldInteractable(preset, this, this, null, Vector2.zero, Vector2.zero, null, null, "").SetInInventory(this);
				}
			}
		}
		bool flag = false;
		if (this.job != null)
		{
			foreach (InteractablePreset interactablePreset in this.job.preset.inventoryItems)
			{
				InteractableCreator.Instance.CreateWorldInteractable(interactablePreset, this, this, null, Vector2.zero, Vector2.zero, null, null, "").SetInInventory(this);
				if (interactablePreset.weapon != null)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			List<InteractablePreset> list = new List<InteractablePreset>();
			for (int i = 0; i < 2; i++)
			{
				list.Add(null);
			}
			foreach (InteractablePreset interactablePreset2 in Toolbox.Instance.allWeapons)
			{
				if (interactablePreset2.weapon.usedInPersonalDefence && !interactablePreset2.weapon.disabled && this.societalClass >= interactablePreset2.weapon.socialClassRange.x && this.societalClass <= interactablePreset2.weapon.socialClassRange.y)
				{
					float num3 = 0f;
					if (interactablePreset2.weapon.personalDefenceTraitModifiers.Count <= 0 || this.WeaponTraitTest(this as Citizen, ref interactablePreset2.weapon.personalDefenceTraitModifiers, out num3))
					{
						num3 += (float)interactablePreset2.weapon.citizenSpawningWithScore;
						if (this.job != null && this.job.employer != null && interactablePreset2.weapon.jobModifierList.Contains(this.job.preset))
						{
							num3 += (float)interactablePreset2.weapon.jobScoreModifier;
						}
						for (int j = 0; j < Mathf.RoundToInt(num3); j++)
						{
							list.Add(interactablePreset2);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				InteractablePreset chosen = list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, this.GetCitizenName() + CityData.Instance.seed, false)];
				if (chosen != null)
				{
					InteractableCreator.Instance.CreateWorldInteractable(chosen, this, this, null, Vector2.zero, Vector2.zero, null, null, "").SetInInventory(this);
					if (!Game.Instance.devMode || !Game.Instance.collectDebugData)
					{
						return;
					}
					Game.DebugCitizenWeapons debugCitizenWeapons = Game.Instance.debugWeaponsSurvey.Find((Game.DebugCitizenWeapons item) => item.weapon == chosen.weapon);
					if (debugCitizenWeapons != null)
					{
						debugCitizenWeapons.count++;
					}
					else
					{
						Game.Instance.debugWeaponsSurvey.Add(new Game.DebugCitizenWeapons
						{
							weapon = chosen.weapon,
							count = 1
						});
					}
					using (List<Game.DebugCitizenWeapons>.Enumerator enumerator3 = Game.Instance.debugWeaponsSurvey.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Game.DebugCitizenWeapons debugCitizenWeapons2 = enumerator3.Current;
							debugCitizenWeapons2.percentage = (float)debugCitizenWeapons2.count / (float)CityData.Instance.citizenDirectory.Count * 100f;
						}
						return;
					}
				}
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					Game.DebugCitizenWeapons debugCitizenWeapons3 = Game.Instance.debugWeaponsSurvey.Find((Game.DebugCitizenWeapons item) => item.weapon == null);
					if (debugCitizenWeapons3 != null)
					{
						debugCitizenWeapons3.count++;
					}
					else
					{
						Game.Instance.debugWeaponsSurvey.Add(new Game.DebugCitizenWeapons
						{
							weapon = null,
							count = 1
						});
					}
					foreach (Game.DebugCitizenWeapons debugCitizenWeapons4 in Game.Instance.debugWeaponsSurvey)
					{
						debugCitizenWeapons4.percentage = (float)debugCitizenWeapons4.count / (float)CityData.Instance.citizenDirectory.Count * 100f;
					}
				}
			}
		}
	}

	// Token: 0x0600034A RID: 842 RVA: 0x000280A0 File Offset: 0x000262A0
	public bool WeaponTraitTest(Citizen cit, ref List<MurderPreset.MurdererModifierRule> rules, out float output)
	{
		output = 1f;
		bool result = true;
		foreach (MurderPreset.MurdererModifierRule murdererModifierRule in rules)
		{
			bool flag = false;
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = true;
							break;
						}
					}
					goto IL_1DA;
				}
				goto IL_89;
			}
			goto IL_89;
			IL_1DA:
			if (flag)
			{
				output += murdererModifierRule.scoreModifier;
				continue;
			}
			if (murdererModifierRule.mustPassForApplication)
			{
				result = false;
				continue;
			}
			continue;
			IL_89:
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1DA;
				}
			}
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1DA;
				}
			}
			if (murdererModifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (cit.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = murdererModifierRule.traitList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait searchTrait = enumerator2.Current;
							if (cit.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
							{
								flag = true;
								break;
							}
						}
						goto IL_1DA;
					}
				}
				flag = false;
				goto IL_1DA;
			}
			goto IL_1DA;
		}
		return result;
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0002833C File Offset: 0x0002653C
	public void PlaceFavouriteItems()
	{
		this.home == null;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0002834C File Offset: 0x0002654C
	public float GetSimulatedTimeRange(NewGameLocation where, float maxTimeAgo)
	{
		float num = Toolbox.Instance.TravelTimeEstimate(this, this.home.nodes[0], where.nodes[0]);
		List<float> list = new List<float>();
		while (num < maxTimeAgo || list.Count <= 0)
		{
			num += 1f;
			float num2 = SessionData.Instance.gameTime - num + Toolbox.Instance.Rand(0f, 1f, false);
			bool flag = true;
			for (int i = 0; i < this.simulatedPreviousBehaviour.Count; i++)
			{
				if (Mathf.Abs(this.simulatedPreviousBehaviour[i] - num2) < 1f)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.job.IsAtWork(num2);
				list.Add(num2);
			}
		}
		float num3 = list[Toolbox.Instance.Rand(0, list.Count, false)];
		this.simulatedPreviousBehaviour.Add(num3);
		return num3;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00028448 File Offset: 0x00026648
	public Interactable WriteNote(Human.NoteObject newPresetType, string treeID, Human reciever, NewGameLocation placement, int security = 0, InteractablePreset.OwnedPlacementRule ownershipPlacement = InteractablePreset.OwnedPlacementRule.both, int priority = 1, HashSet<NewRoom> dontPlaceInRooms = null, bool printDebug = false, int toneFriendly = 0, int toneFormal = 0, string loadGUID = null)
	{
		List<Human.NoteObject> newPresetType2 = Enumerable.ToList<Human.NoteObject>(new Human.NoteObject[]
		{
			newPresetType
		});
		return this.WriteNote(newPresetType2, treeID, reciever, placement, security, ownershipPlacement, priority, dontPlaceInRooms, printDebug, toneFriendly, toneFormal, loadGUID);
	}

	// Token: 0x0600034E RID: 846 RVA: 0x00028480 File Offset: 0x00026680
	public Interactable WriteNote(List<Human.NoteObject> newPresetType, string treeID, Human reciever, NewGameLocation placement, int security = 0, InteractablePreset.OwnedPlacementRule ownershipPlacement = InteractablePreset.OwnedPlacementRule.both, int priority = 1, HashSet<NewRoom> dontPlaceInRooms = null, bool printDebug = false, int toneFriendly = 0, int toneFormal = 0, string loadGUID = null)
	{
		Interactable interactable = null;
		while (interactable == null && newPresetType.Count > 0)
		{
			Human.NoteObject noteObject = newPresetType[Toolbox.Instance.Rand(0, newPresetType.Count, false)];
			InteractablePreset interactable2 = InteriorControls.Instance.note;
			if (noteObject == Human.NoteObject.letter)
			{
				interactable2 = InteriorControls.Instance.letter;
			}
			else if (noteObject == Human.NoteObject.travelReceipt)
			{
				interactable2 = InteriorControls.Instance.travelReceipt;
			}
			else if (noteObject == Human.NoteObject.vmailLetter)
			{
				interactable2 = InteriorControls.Instance.vmailLetter;
			}
			FurnitureLocation furnitureLocation;
			interactable = placement.PlaceObject(interactable2, reciever, this, reciever, out furnitureLocation, null, true, security, ownershipPlacement, priority, null, printDebug, dontPlaceInRooms, loadGUID, null, treeID, false);
			newPresetType.Remove(noteObject);
		}
		return interactable;
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00028520 File Offset: 0x00026720
	public CitySaveData.HumanCitySave GenerateSaveData()
	{
		CitySaveData.HumanCitySave humanCitySave = new CitySaveData.HumanCitySave();
		humanCitySave.humanID = this.humanID;
		if (this.home != null)
		{
			humanCitySave.home = this.home.id;
			humanCitySave.debugHome = this.home.name;
		}
		humanCitySave.speedModifier = this.speedMultiplier;
		humanCitySave.job = this.job.id;
		if (this.job.employer == null && !this.job.preset.isCriminal)
		{
			humanCitySave.job = 0;
		}
		humanCitySave.societalClass = this.societalClass;
		humanCitySave.blood = this.bloodType;
		humanCitySave.descriptors = this.descriptors;
		humanCitySave.citizenName = this.citizenName;
		humanCitySave.casualName = this.casualName;
		humanCitySave.firstName = this.firstName;
		humanCitySave.surName = this.surName;
		humanCitySave.genderScale = this.genderScale;
		humanCitySave.gender = this.gender;
		humanCitySave.bGender = this.birthGender;
		humanCitySave.attractedTo = this.attractedTo;
		humanCitySave.homosexuality = this.homosexuality;
		humanCitySave.sexuality = this.sexuality;
		if (this.partner != null)
		{
			humanCitySave.partner = this.partner.humanID;
		}
		if (this.paramour != null)
		{
			humanCitySave.paramour = this.paramour.humanID;
		}
		humanCitySave.sleepNeedMultiplier = this.sleepNeedMultiplier;
		humanCitySave.snoring = this.snoring;
		humanCitySave.snoreDelay = this.snoreDelay;
		humanCitySave.humility = this.humility;
		humanCitySave.emotionality = this.emotionality;
		humanCitySave.extraversion = this.extraversion;
		humanCitySave.agreeableness = this.agreeableness;
		humanCitySave.conscientiousness = this.conscientiousness;
		humanCitySave.creativity = this.creativity;
		humanCitySave.birthday = this.birthday;
		humanCitySave.password = this.passcode;
		humanCitySave.maxHealth = this.maximumHealth;
		humanCitySave.recoveryRate = this.recoveryRate;
		humanCitySave.combatHeft = this.combatHeft;
		humanCitySave.combatSkill = this.combatSkill;
		humanCitySave.maxNerve = this.maxNerve;
		humanCitySave.homeless = this.isHomeless;
		humanCitySave.breathRecovery = this.breathRecoveryRate;
		humanCitySave.sightingMemory = this.sightingMemoryLimit;
		if (this.handwriting != null)
		{
			humanCitySave.handwriting = this.handwriting.name;
		}
		humanCitySave.slangUsage = this.slangUsage;
		humanCitySave.anniversary = this.anniversary;
		foreach (Acquaintance acquaintance in this.acquaintances)
		{
			humanCitySave.acquaintances.Add(acquaintance.GenerateSaveData());
		}
		foreach (Human.Trait trait in this.characterTraits)
		{
			CitySaveData.CharTraitSave charTraitSave = new CitySaveData.CharTraitSave();
			charTraitSave.traitID = trait.traitID;
			charTraitSave.trait = trait.trait.name;
			charTraitSave.date = trait.date;
			if (trait.reason != null)
			{
				charTraitSave.reason = trait.reason.traitID;
			}
			humanCitySave.traits.Add(charTraitSave);
		}
		foreach (KeyValuePair<RetailItemPreset, int> keyValuePair in this.itemRanking)
		{
			humanCitySave.favItems.Add(keyValuePair.Key.name);
			humanCitySave.favItemRanks.Add(keyValuePair.Value);
		}
		foreach (KeyValuePair<CompanyPreset.CompanyCategory, NewAddress> keyValuePair2 in this.favouritePlaces)
		{
			humanCitySave.favCat.Add(keyValuePair2.Key);
			humanCitySave.favAddresses.Add(keyValuePair2.Value.id);
		}
		humanCitySave.outfits = new List<CitizenOutfitController.Outfit>(this.outfitController.outfits);
		humanCitySave.favCol = this.favColourIndex;
		return humanCitySave;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00028984 File Offset: 0x00026B84
	public int CompareTo(Human comp)
	{
		float num = this.societalClass;
		if (this.job != null && this.job.employer != null)
		{
			num += (float)this.job.preset.jobFillPriority;
		}
		float num2 = comp.societalClass;
		if (comp.job != null && comp.job.employer != null)
		{
			num2 += (float)comp.job.preset.jobFillPriority;
		}
		return num.CompareTo(num2);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x000289FC File Offset: 0x00026BFC
	public void SpeechTriggerPoint(DDSSaveClasses.TriggerPoint triggerPoint, Actor trackedTarget, AIActionPreset onAction = null)
	{
		if (this.currentCityTile == null)
		{
			return;
		}
		if ((CityConstructor.Instance != null && CityConstructor.Instance.preSimActive) || !SessionData.Instance.play)
		{
			return;
		}
		if (!this.currentCityTile.isInPlayerVicinity)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("... Not in player's vicinity", Actor.HumanDebug.misc);
			}
			return;
		}
		if (this.inConversation)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("... In existing conversation", Actor.HumanDebug.misc);
			}
			return;
		}
		if (this.nextCasualSpeechValidAt > SessionData.Instance.gameTime)
		{
			return;
		}
		if (this.currentRoom.activeConversations.Count > 0)
		{
			return;
		}
		if (InterfaceController.Instance.activeSpeechBubbles.Count > CitizenControls.Instance.maxSpeechBubbles)
		{
			return;
		}
		if (InteractionController.Instance.talkingTo == this.interactable)
		{
			return;
		}
		if (this.currentGameLocation != null && this.currentGameLocation.telephones.Count > 0 && this.currentGameLocation.telephones.Exists((Telephone item) => item.activeReceiver == this))
		{
			return;
		}
		if (this.ai != null)
		{
			if (this.ai.inCombat)
			{
				return;
			}
			if (this.isRunning)
			{
				return;
			}
			if (this.ai.currentAction != null)
			{
				if (this.ai.currentAction.preset.disableConversationTriggers)
				{
					return;
				}
				if (this.ai.currentAction.preset == RoutineControls.Instance.answerTelephone)
				{
					return;
				}
			}
		}
		if (this.dds.ContainsKey(triggerPoint))
		{
			List<DDSSaveClasses.DDSTreeSave> list = new List<DDSSaveClasses.DDSTreeSave>();
			List<List<Human>> list2 = new List<List<Human>>();
			foreach (DDSSaveClasses.DDSTreeSave ddstreeSave in this.dds[triggerPoint])
			{
				if ((ddstreeSave.ignoreGlobalRepeat || !GameplayController.Instance.globalConversationDelay.ContainsKey(ddstreeSave.id)) && Toolbox.Instance.Rand(0f, 1f, false) <= ddstreeSave.treeChance * 0.1f)
				{
					bool flag = true;
					List<Human.SpeechHistory> list3 = null;
					if (ddstreeSave.repeat != DDSSaveClasses.RepeatSetting.noLimit && this.speechHistory.TryGetValue(ddstreeSave, ref list3))
					{
						if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.never)
						{
							continue;
						}
						float num = 1f;
						if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.sixHours)
						{
							num = 6f;
						}
						else if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.twelveHours)
						{
							num = 12f;
						}
						else if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.oneDay)
						{
							num = 24f;
						}
						else if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.twoDays)
						{
							num = 48f;
						}
						else if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.threeDays)
						{
							num = 72f;
						}
						else if (ddstreeSave.repeat == DDSSaveClasses.RepeatSetting.oneWeek)
						{
							num = 168f;
						}
						for (int i = 0; i < list3.Count; i++)
						{
							Human.SpeechHistory speechHistory = list3[i];
							if (ddstreeSave.repeat != DDSSaveClasses.RepeatSetting.never && SessionData.Instance.gameTime > speechHistory.timeStamp + num)
							{
								list3.RemoveAt(i);
								i--;
							}
						}
						if (list3.Count > 0)
						{
							flag = false;
						}
					}
					if (flag)
					{
						List<Human> list4 = new List<Human>();
						for (int j = 0; j < 4; j++)
						{
							if (j == 0)
							{
								if (!this.DDSParticipantConditionCheck(this, ddstreeSave.participantA, ddstreeSave.treeType))
								{
									flag = false;
									break;
								}
							}
							else
							{
								DDSSaveClasses.DDSParticipant ddsparticipant = ddstreeSave.participantB;
								if (j == 2)
								{
									ddsparticipant = ddstreeSave.participantC;
								}
								if (j == 3)
								{
									ddsparticipant = ddstreeSave.participantD;
								}
								if (ddsparticipant.required)
								{
									if (trackedTarget != null)
									{
										Human human = trackedTarget as Human;
										if (human != null && !list4.Contains(human) && human != this && human.DDSParticipantConditionCheck(this, ddsparticipant, ddstreeSave.treeType))
										{
											list4.Add(human);
											goto IL_43E;
										}
									}
									bool flag2 = false;
									foreach (Actor actor in this.currentRoom.currentOccupants)
									{
										Human human2 = actor as Human;
										if (human2 != null && !list4.Contains(human2) && human2 != this && human2.DDSParticipantConditionCheck(this, ddsparticipant, ddstreeSave.treeType))
										{
											list4.Add(human2);
											flag2 = true;
											break;
										}
									}
									if (!flag2)
									{
										flag = false;
										break;
									}
								}
							}
							IL_43E:;
						}
						if (flag)
						{
							list.Add(ddstreeSave);
							list2.Add(list4);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				list.Sort((DDSSaveClasses.DDSTreeSave p1, DDSSaveClasses.DDSTreeSave p2) => p2.priority.CompareTo(p1.priority));
				this.ExecuteConversationTree(list[0], list2[0]);
			}
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00028EFC File Offset: 0x000270FC
	public bool DDSParticipantConditionCheck(Human initiator, DDSSaveClasses.DDSParticipant conditions, DDSSaveClasses.TreeType treeType)
	{
		if (this.ai != null)
		{
			if (this.isRunning)
			{
				return false;
			}
			if (this.ai.inCombat)
			{
				return false;
			}
			if (this.ai.currentAction != null && this.ai.currentAction.preset.disableConversationTriggers)
			{
				return false;
			}
		}
		if (InteractionController.Instance.talkingTo == this.interactable)
		{
			return false;
		}
		if (this.currentGameLocation != null && this.currentGameLocation.telephones.Count > 0 && this.currentGameLocation.telephones.Exists((Telephone item) => item.activeReceiver == this))
		{
			return false;
		}
		if (this.ai != null)
		{
			if (this.isRunning)
			{
				return false;
			}
			if (this.ai.currentAction != null && this.ai.currentAction.preset == RoutineControls.Instance.answerTelephone)
			{
				return false;
			}
			if (this.ai.inCombat)
			{
				return false;
			}
		}
		if (treeType == DDSSaveClasses.TreeType.conversation)
		{
			if (this.inConversation)
			{
				return false;
			}
			if (this.currentNode == null || this.currentNode.isIndoorsEntrance)
			{
				return false;
			}
			if (this.ai != null && this.ai.currentGoal != null && this.ai.currentGoal.preset == RoutineControls.Instance.fleeGoal)
			{
				return false;
			}
			if (this.currentRoom != initiator.currentRoom)
			{
				return false;
			}
			if (initiator != this)
			{
				float num = Vector3.Distance(initiator.transform.position, base.transform.position);
				if (num > 3.5f || num < 1.25f)
				{
					return false;
				}
			}
		}
		Acquaintance acquaintance = null;
		bool flag = false;
		if (initiator != this)
		{
			if (conditions.connection == Acquaintance.ConnectionType.anyone)
			{
				flag = true;
			}
			else if (conditions.connection == Acquaintance.ConnectionType.anyoneNotPlayer && !this.isPlayer)
			{
				flag = true;
			}
			else if (conditions.connection == Acquaintance.ConnectionType.player && this.isPlayer)
			{
				flag = true;
			}
			else if (initiator.FindAcquaintanceExists(this, out acquaintance))
			{
				if (acquaintance.with.isPlayer)
				{
					flag = false;
				}
				else if (acquaintance.connections.Contains(conditions.connection) || conditions.connection == acquaintance.secretConnection)
				{
					flag = true;
				}
				else if (conditions.connection == Acquaintance.ConnectionType.anyAcquaintance)
				{
					flag = true;
				}
				else if (conditions.connection == Acquaintance.ConnectionType.friendOrWork)
				{
					if (acquaintance.connections.Contains(Acquaintance.ConnectionType.friend) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workTeam) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workOther) || acquaintance.connections.Contains(Acquaintance.ConnectionType.familiarWork) || acquaintance.connections.Contains(Acquaintance.ConnectionType.paramour))
					{
						flag = true;
					}
				}
				else if (conditions.connection == Acquaintance.ConnectionType.knowsName && acquaintance.dataKeys.Contains(Evidence.DataKey.name))
				{
					flag = true;
				}
			}
			else if (conditions.connection == Acquaintance.ConnectionType.stranger)
			{
				flag = true;
			}
			if (!flag)
			{
				return false;
			}
			if (conditions.useJobs && (this.job.preset == null || !conditions.jobs.Contains(this.job.preset.name)))
			{
				return false;
			}
			if (conditions.useTraits)
			{
				if (acquaintance == null)
				{
					initiator.FindAcquaintanceExists(this, out acquaintance);
				}
				flag = Toolbox.Instance.DDSTraitConditionLogicAcquaintance(this, acquaintance, conditions.traitConditions, ref conditions.traits);
			}
			if (!flag)
			{
				return false;
			}
		}
		foreach (DDSSaveClasses.TreeTriggers treeTriggers in conditions.triggers)
		{
			if (treeTriggers == DDSSaveClasses.TreeTriggers.lightOnAny)
			{
				if (!this.currentRoom.mainLightStatus && !this.currentRoom.GetSecondaryLightStatus())
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.lightOnMain)
			{
				if (!this.currentRoom.mainLightStatus)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.allLightsOff)
			{
				if (this.currentRoom.mainLightStatus || this.currentRoom.GetSecondaryLightStatus())
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.asleep)
			{
				if (!this.isAsleep)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.awake)
			{
				if (this.isAsleep || this.isDead || this.isStunned)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.dead)
			{
				if (!this.isDead)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.atHome)
			{
				if (!this.isHome)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.atWork)
			{
				if (!this.isAtWork)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.investigatingSound)
			{
				if (this.ai != null)
				{
					if (this.ai.reactionState != NewAIController.ReactionState.investigatingSound)
					{
						return false;
					}
				}
				else if (!this.isPlayer)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.persuing)
			{
				if (this.ai != null)
				{
					if (this.ai.reactionState != NewAIController.ReactionState.persuing)
					{
						return false;
					}
				}
				else if (!this.isPlayer)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.searching)
			{
				if (this.ai != null)
				{
					if (this.ai.reactionState != NewAIController.ReactionState.searching)
					{
						return false;
					}
				}
				else if (!this.isPlayer)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.travelling)
			{
				if (!this.isMoving)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.carrying)
			{
				if (!this.isPlayer)
				{
					return false;
				}
				if (InteractionController.Instance.carryingObject == null)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.notCarrying)
			{
				if (this.isPlayer && InteractionController.Instance.carryingObject != null)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.employee)
			{
				if (this.job.employer != initiator.job.employer)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.nonEmployee)
			{
				if (this.job.employer == initiator.job.employer)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.illegal)
			{
				if (this.isPlayer)
				{
					if (!Player.Instance.illegalStatus)
					{
						return false;
					}
				}
				else if (!this.isTrespassing)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.legal)
			{
				if (this.isPlayer)
				{
					if (Player.Instance.illegalStatus)
					{
						return false;
					}
				}
				else if (this.isTrespassing)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.inCombat)
			{
				if (!(this.ai != null))
				{
					return false;
				}
				if (!this.ai.inCombat)
				{
					return false;
				}
			}
			else if (treeTriggers == DDSSaveClasses.TreeTriggers.notInCombat)
			{
				if (this.ai != null && this.ai.inCombat)
				{
					return false;
				}
			}
			else
			{
				if (treeTriggers == DDSSaveClasses.TreeTriggers.trespassing)
				{
					int num2;
					string text;
					return this.IsTrespassing(this.currentRoom, out num2, out text, true);
				}
				if (treeTriggers == DDSSaveClasses.TreeTriggers.locationOfAuthority)
				{
					return this.locationsOfAuthority.Contains(this.currentGameLocation);
				}
				if (treeTriggers == DDSSaveClasses.TreeTriggers.drunk)
				{
					if (this.drunk > 0.75f)
					{
						return true;
					}
					return false;
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.sober)
				{
					if (this.drunk <= 0.75f)
					{
						return true;
					}
					return false;
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.onStreet)
				{
					if (this.currentGameLocation.thisAsStreet == null)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.indoors)
				{
					if (this.currentGameLocation.thisAsAddress == null)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.privateLocation)
				{
					if (this.currentRoom.preset.forbidden == RoomConfiguration.Forbidden.alwaysAllowed)
					{
						return false;
					}
					if (this.currentRoom.preset.forbidden == RoomConfiguration.Forbidden.allowedDuringOpenHours)
					{
						return !this.currentRoom.IsAccessAllowed(this);
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.publicLocation)
				{
					if (this.currentRoom.preset.forbidden == RoomConfiguration.Forbidden.alwaysForbidden)
					{
						return false;
					}
					if (this.currentRoom.preset.forbidden == RoomConfiguration.Forbidden.allowedDuringOpenHours)
					{
						return this.currentRoom.IsAccessAllowed(this);
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.sat)
				{
					if (this.animationController.idleAnimationState != CitizenAnimationController.IdleAnimationState.sitting)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.noReactionState)
				{
					if (this.ai != null)
					{
						if (this.ai.reactionState != NewAIController.ReactionState.none)
						{
							return false;
						}
						if (this.ai.restrained)
						{
							return false;
						}
					}
					else if (!this.isPlayer)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.investigating)
				{
					if (this.ai != null)
					{
						if (this.ai.reactionState != NewAIController.ReactionState.investigatingSight && this.ai.reactionState != NewAIController.ReactionState.investigatingSound)
						{
							return false;
						}
					}
					else if (!this.isPlayer)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.investigatingVisual)
				{
					if (this.ai != null)
					{
						if (this.ai.reactionState != NewAIController.ReactionState.investigatingSight)
						{
							return false;
						}
					}
					else if (!this.isPlayer)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.unconscious)
				{
					if (!this.isStunned)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.rain)
				{
					if (SessionData.Instance.currentRain < 0.5f)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.brokenSign)
				{
					if (!(this.currentRoom.gameLocation.thisAsAddress != null))
					{
						return false;
					}
					if (!this.currentRoom.gameLocation.thisAsAddress.featuresBrokenSign)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.travellingToWork)
				{
					if (this.ai != null)
					{
						if (!(this.ai.currentGoal.preset == RoutineControls.Instance.workGoal))
						{
							return false;
						}
						if (this.currentGameLocation == this.job.employer.placeOfBusiness)
						{
							return false;
						}
					}
					else if (!this.isPlayer)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.atEatery)
				{
					if (!(this.currentRoom.gameLocation.thisAsAddress != null))
					{
						return false;
					}
					if (this.currentRoom.gameLocation.thisAsAddress.company == null)
					{
						return false;
					}
					if (!this.currentRoom.gameLocation.thisAsAddress.company.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.meal) && !this.currentRoom.gameLocation.thisAsAddress.company.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.snack) && !this.currentRoom.gameLocation.thisAsAddress.company.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.caffeine))
					{
						return false;
					}
					if (this.isAtWork)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.enforcerOnDuty)
				{
					if (!this.isEnforcer || !this.isOnDuty)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.notEnforcerOnDuty)
				{
					if (this.isEnforcer && this.isOnDuty)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.hasJob)
				{
					if (this.job == null || this.job.employer == null)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.unemployed)
				{
					if (this.job != null && this.job.employer != null)
					{
						return false;
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.homeIntenseWallpaper)
				{
					if (this.home != null)
					{
						bool flag2 = false;
						foreach (NewRoom newRoom in this.home.rooms)
						{
							try
							{
								if (newRoom.colourScheme.loudness > 8)
								{
									flag2 = true;
									break;
								}
							}
							catch
							{
							}
						}
						if (!flag2)
						{
							return false;
						}
					}
				}
				else if (treeTriggers == DDSSaveClasses.TreeTriggers.restrained)
				{
					if (this.ai != null)
					{
						return this.ai.restrained;
					}
					return false;
				}
				else
				{
					if (treeTriggers == DDSSaveClasses.TreeTriggers.hasRoomAtHotel)
					{
						if (this.currentGameLocation != null && this.currentGameLocation.thisAsAddress != null && this.currentGameLocation.thisAsAddress.company != null && this.currentGameLocation.thisAsAddress.company.preset != null && this.currentGameLocation.thisAsAddress.company.preset.isHotel)
						{
							GameplayController.HotelGuest hotelRoom = Toolbox.Instance.GetHotelRoom(this);
							if (hotelRoom != null && SessionData.Instance.gameTime < hotelRoom.nextPayment)
							{
								return true;
							}
						}
						return false;
					}
					if (treeTriggers == DDSSaveClasses.TreeTriggers.hasNoRoomAtHotel)
					{
						if (this.currentGameLocation != null && this.currentGameLocation.thisAsAddress != null && this.currentGameLocation.thisAsAddress.company != null && this.currentGameLocation.thisAsAddress.company.preset != null && this.currentGameLocation.thisAsAddress.company.preset.isHotel && Toolbox.Instance.GetHotelRoom(this) != null)
						{
							return false;
						}
						return true;
					}
					else if (treeTriggers == DDSSaveClasses.TreeTriggers.hotelPaymentDue)
					{
						if (this.currentGameLocation != null && this.currentGameLocation.thisAsAddress != null && this.currentGameLocation.thisAsAddress.company != null && this.currentGameLocation.thisAsAddress.company.preset != null && this.currentGameLocation.thisAsAddress.company.preset.isHotel)
						{
							GameplayController.HotelGuest hotelRoom2 = Toolbox.Instance.GetHotelRoom(this);
							if (hotelRoom2 != null && SessionData.Instance.gameTime >= hotelRoom2.lastPayment + 24f)
							{
								return true;
							}
						}
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00029D90 File Offset: 0x00027F90
	public void ExecuteConversationTree(DDSSaveClasses.DDSTreeSave newTree, List<Human> otherParticipants)
	{
		if (newTree == null)
		{
			Game.LogError("Invalid conversation tree!", 2);
			return;
		}
		if (otherParticipants == null || otherParticipants.Count <= 0)
		{
			Game.LogError("Invalid participants for tree " + newTree.name + " " + newTree.id, 2);
			return;
		}
		if (!newTree.ignoreGlobalRepeat && !GameplayController.Instance.globalConversationDelay.ContainsKey(newTree.id))
		{
			GameplayController.Instance.globalConversationDelay.Add(newTree.id, SessionData.Instance.gameTime);
		}
		if (Game.Instance.collectDebugData)
		{
			base.SelectedDebug("Execute speech " + newTree.name, Actor.HumanDebug.misc);
		}
		this.nextCasualSpeechValidAt = SessionData.Instance.gameTime + 0.1f;
		if (!this.speechHistory.ContainsKey(newTree))
		{
			this.speechHistory.Add(newTree, new List<Human.SpeechHistory>());
		}
		Human.SpeechHistory speechHistory = new Human.SpeechHistory
		{
			timeStamp = SessionData.Instance.gameTime
		};
		speechHistory.participants.Add(this);
		speechHistory.participants.AddRange(otherParticipants);
		this.speechHistory[newTree].Add(speechHistory);
		foreach (Human human in otherParticipants)
		{
			if (human == null)
			{
				Game.LogError("Null conversation participant!", 2);
			}
			if (!human.speechHistory.ContainsKey(newTree))
			{
				human.speechHistory.Add(newTree, new List<Human.SpeechHistory>());
			}
			human.speechHistory[newTree].Add(speechHistory);
		}
		Human.ConversationInstance conversationInstance = new Human.ConversationInstance
		{
			tree = newTree,
			participantA = this
		};
		if (otherParticipants.Count >= 1)
		{
			conversationInstance.participantB = otherParticipants[0];
		}
		if (otherParticipants.Count >= 2)
		{
			conversationInstance.participantC = otherParticipants[1];
		}
		if (otherParticipants.Count >= 3)
		{
			conversationInstance.participantD = otherParticipants[2];
		}
		conversationInstance.active = true;
		conversationInstance.room = this.currentRoom;
		this.currentRoom.activeConversations.Add(conversationInstance);
		conversationInstance.currentlyTalking = this;
		this.SetInConversation(conversationInstance, true);
		foreach (Human human2 in otherParticipants)
		{
			human2.SetInConversation(conversationInstance, true);
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00029FF4 File Offset: 0x000281F4
	public virtual void SetInConversation(Human.ConversationInstance newInstance, bool endCall = true)
	{
		if (newInstance != null)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("Set in conversation: " + newInstance.tree.name + " " + newInstance.tree.id, Actor.HumanDebug.misc);
			}
			if (this.isPlayer)
			{
				Game.Log("Player: Player is in conversation! Should this be happening: " + newInstance.tree.name, 2);
			}
			newInstance.treeName = newInstance.tree.name;
			this.currentConversation = newInstance;
			this.inConversation = true;
			if (this.ai != null)
			{
				this.ai.SetUpdateEnabled(true);
				if (this.animationController != null && (this.animationController.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.armsUse || this.animationController.armsBoolAnimationState == CitizenAnimationController.ArmsBoolSate.armsLocking))
				{
					this.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
				}
			}
		}
		else
		{
			this.currentConversation = null;
			this.inConversation = false;
			if (this.currentGameLocation.telephones.Count > 0 && endCall)
			{
				Telephone telephone = this.currentGameLocation.telephones.Find((Telephone item) => item.activeReceiver == this);
				if (telephone != null && telephone.activeCall.Count > 0)
				{
					telephone.activeCall[0].EndCall();
				}
			}
		}
		this.debugConversation = newInstance;
		this.UpdateCurrentNodeSpace();
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002A14C File Offset: 0x0002834C
	public List<string> ParseDDSMessage(DDSSaveClasses.DDSMessageSettings settings, Acquaintance aq, object passedObject = null)
	{
		List<int> list;
		return this.ParseDDSMessage(settings.msgID, aq, out list, false, passedObject, false);
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002A16C File Offset: 0x0002836C
	public List<string> ParseDDSMessage(string msgID, Acquaintance aq, out List<int> outputDisplayGroups, bool forceRealRandom = false, object passedObject = null, bool debug = false)
	{
		List<string> list = new List<string>();
		outputDisplayGroups = new List<int>();
		if (!Game.Instance.printDebug)
		{
			debug = false;
		}
		DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
		string text;
		if (forceRealRandom)
		{
			text = Toolbox.Instance.GenerateSeed(16, false, "");
		}
		else
		{
			text = this.humanID.ToString() + msgID;
		}
		if (Toolbox.Instance.allDDSMessages.TryGetValue(msgID, ref ddsmessageSave))
		{
			List<DDSSaveClasses.DDSBlockSave> list2 = new List<DDSSaveClasses.DDSBlockSave>();
			List<int> list3 = new List<int>();
			for (int i = 0; i < ddsmessageSave.blocks.Count; i++)
			{
				DDSSaveClasses.DDSBlockCondition conditions = ddsmessageSave.blocks[i];
				if (debug)
				{
					Game.Log("DDS: Parse debug: Checking validity of block " + conditions.blockID + "...", 2);
				}
				if (!Toolbox.Instance.allDDSBlocks.ContainsKey(conditions.blockID))
				{
					Game.LogError(string.Concat(new string[]
					{
						"Missing Block! ",
						conditions.blockID,
						" for message ",
						msgID,
						" (",
						ddsmessageSave.name,
						")"
					}), 2);
				}
				else if (conditions.alwaysDisplay)
				{
					if (debug)
					{
						Game.Log("DDS: Parse debug: .... Always display block", 2);
					}
					list2.Add(Toolbox.Instance.allDDSBlocks[conditions.blockID]);
					outputDisplayGroups.Add(conditions.group);
				}
				else
				{
					if (conditions.group > 0)
					{
						if (debug)
						{
							Game.Log("DDS: Parse debug: .... Block uses display group " + conditions.group.ToString(), 2);
						}
						if (list3.Contains(conditions.group))
						{
							if (debug)
							{
								Game.Log("DDS: Parse debug: .... Display group already contains entry", 2);
								goto IL_90A;
							}
							goto IL_90A;
						}
						else
						{
							if (conditions.group >= 21)
							{
								SideJob sideJob = passedObject as SideJob;
								if (sideJob == null)
								{
									Interactable interactable = passedObject as Interactable;
									if (interactable != null)
									{
										sideJob = interactable.jobParent;
									}
								}
								if (sideJob == null && conditions.group <= 49)
								{
									goto IL_90A;
								}
								if (conditions.group >= 37 && conditions.group <= 41)
								{
									if (sideJob.preset.difficultyTag < JobPreset.DifficultyTag.D2A)
									{
										goto IL_90A;
									}
								}
								else
								{
									if (conditions.group <= 31 && conditions.group != (int)(21 + sideJob.preset.difficultyTag))
									{
										goto IL_90A;
									}
									if (conditions.group >= 32 && conditions.group < 42)
									{
										int num = conditions.group - 32;
										if (sideJob.appliedBasicLeads.Count <= num)
										{
											goto IL_90A;
										}
										Game.Log(string.Concat(new string[]
										{
											"Group + ",
											conditions.group.ToString(),
											" LeadPoolIndex: ",
											num.ToString(),
											", applied lead count: ",
											sideJob.appliedBasicLeads.Count.ToString()
										}), 2);
										int displayGroupFromLeadPool = (int)(sideJob.appliedBasicLeads[num] + 1);
										DDSSaveClasses.DDSMessageSave ddsmessageSave2 = null;
										if (!Toolbox.Instance.allDDSMessages.TryGetValue("8d6a4bf8-2f99-46de-ac7a-1b1e628e45cb", ref ddsmessageSave2))
										{
											goto IL_90A;
										}
										using (List<DDSSaveClasses.DDSBlockCondition>.Enumerator enumerator = ddsmessageSave2.blocks.FindAll((DDSSaveClasses.DDSBlockCondition item) => item.group == displayGroupFromLeadPool).GetEnumerator())
										{
											if (!enumerator.MoveNext())
											{
												goto IL_90A;
											}
											DDSSaveClasses.DDSBlockCondition ddsblockCondition = enumerator.Current;
											if (displayGroupFromLeadPool != 15)
											{
												list2.Add(Toolbox.Instance.allDDSBlocks[ddsblockCondition.blockID]);
												goto IL_90A;
											}
											if (sideJob.purp.characterTraits.Exists((Human.Trait item) => item.name == "Affliction-ShortSighted" || item.name == "Affliction-FarSighted"))
											{
												if (sideJob.purp.characterTraits.Exists((Human.Trait item) => item.name == "Quirk-FacialHair"))
												{
													list2.Add(Toolbox.Instance.allDDSBlocks["37f565a8-3b50-4812-81de-dda0eae00f67"]);
													goto IL_90A;
												}
												list2.Add(Toolbox.Instance.allDDSBlocks["a8986a53-80e5-4084-8cdd-10f2100eeb53"]);
												goto IL_90A;
											}
											else
											{
												if (sideJob.purp.characterTraits.Exists((Human.Trait item) => item.name == "Quirk-FacialHair"))
												{
													list2.Add(Toolbox.Instance.allDDSBlocks["80d87458-1f4e-4f3c-bf82-8843d1371820"]);
													goto IL_90A;
												}
												list2.Add(Toolbox.Instance.allDDSBlocks["22274aea-20b9-4e6f-9128-76ddb5c7369f"]);
												goto IL_90A;
											}
										}
									}
									if (conditions.group >= 42 && conditions.group <= 47)
									{
										if (!sideJob.resolveQuestions.Exists((Case.ResolveQuestion item) => item.GetRevengeObjective() != null && item.GetRevengeObjective().tag == (JobPreset.JobTag)(conditions.group - 42)))
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 48)
									{
										if (sideJob.intro.Contains("Staff"))
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 49)
									{
										if (!sideJob.intro.Contains("Staff"))
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 50)
									{
										MurderController.Murder murder = passedObject as MurderController.Murder;
										if (murder == null)
										{
											goto IL_90A;
										}
										if (murder.callingCard == null)
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 51)
									{
										MurderController.Murder murder2 = passedObject as MurderController.Murder;
										if (murder2 == null || murder2.graffitiMsg == null)
										{
											goto IL_90A;
										}
										if (murder2.graffitiMsg.Length <= 0)
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 52)
									{
										if (this.job == null || this.job.employer == null)
										{
											if (debug)
											{
												Game.Log("DDS: Parse debug: .... No job found for " + this.citizenName, 2);
												goto IL_90A;
											}
											goto IL_90A;
										}
									}
									else if (conditions.group == 53)
									{
										Interactable interactable2 = passedObject as Interactable;
										if (interactable2 == null || interactable2.pv == null)
										{
											goto IL_90A;
										}
										Interactable.Passed passedLost = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemBuilding);
										if (passedLost == null)
										{
											goto IL_90A;
										}
										NewBuilding newBuilding = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.buildingID == (int)passedLost.value);
										if (!(newBuilding != null))
										{
											goto IL_90A;
										}
										if (newBuilding.floors.Count < 4)
										{
											goto IL_90A;
										}
									}
									else if (conditions.group == 54)
									{
										Interactable interactable3 = passedObject as Interactable;
										if (interactable3 == null || interactable3.pv == null)
										{
											goto IL_90A;
										}
										Interactable.Passed passedLost = interactable3.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemBuilding);
										if (passedLost == null)
										{
											goto IL_90A;
										}
										NewBuilding newBuilding2 = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.buildingID == (int)passedLost.value);
										if (!(newBuilding2 != null) || newBuilding2.floors.Count >= 4)
										{
											goto IL_90A;
										}
									}
								}
							}
							List<DDSSaveClasses.DDSBlockCondition> list4 = new List<DDSSaveClasses.DDSBlockCondition>();
							foreach (DDSSaveClasses.DDSBlockCondition ddsblockCondition2 in ddsmessageSave.blocks)
							{
								if (ddsblockCondition2.group == conditions.group && (!ddsblockCondition2.useTraits || Toolbox.Instance.DDSTraitConditionLogicAcquaintance(this, aq, ddsblockCondition2.traitConditions, ref ddsblockCondition2.traits)))
								{
									list4.Add(ddsblockCondition2);
								}
							}
							if (list4.Count <= 0)
							{
								goto IL_90A;
							}
							conditions = list4[Toolbox.Instance.RandContained(0, list4.Count, text, out text)];
							list3.Add(conditions.group);
						}
					}
					else if (conditions.useTraits)
					{
						if (debug)
						{
							Game.Log("DDS: Parse debug: .... Block uses trait conditions", 2);
						}
						if (!Toolbox.Instance.DDSTraitConditionLogicAcquaintance(this, aq, conditions.traitConditions, ref conditions.traits))
						{
							if (debug)
							{
								Game.Log("DDS: Parse debug: .... Block does not pass trait conditions", 2);
								goto IL_90A;
							}
							goto IL_90A;
						}
					}
					list2.Add(Toolbox.Instance.allDDSBlocks[conditions.blockID]);
					outputDisplayGroups.Add(conditions.group);
				}
				IL_90A:;
			}
			for (int j = 0; j < list2.Count; j++)
			{
				DDSSaveClasses.DDSBlockSave ddsblockSave = list2[j];
				List<Human.DDSRank> list5 = new List<Human.DDSRank>();
				float num2 = 1f;
				if (aq != null)
				{
					num2 = Mathf.Abs(aq.known - 0.5f) + Mathf.Abs(aq.like - 0.5f);
				}
				list5.Add(new Human.DDSRank
				{
					id = ddsblockSave.id,
					rankRef = num2
				});
				foreach (DDSSaveClasses.DDSReplacement ddsreplacement in ddsblockSave.replacements)
				{
					float num3 = Toolbox.Instance.RandContained(-0.02f, -0.01f, text + ddsreplacement.replaceWithID, out text);
					if (!ddsreplacement.useTraits || Toolbox.Instance.DDSTraitConditionLogicAcquaintance(this, aq, ddsreplacement.traitCondition, ref ddsreplacement.traits))
					{
						if (ddsreplacement.useConnection)
						{
							bool flag = false;
							if (ddsreplacement.connection == Acquaintance.ConnectionType.anyoneNotPlayer && !this.isPlayer)
							{
								flag = true;
							}
							else if (ddsreplacement.connection == Acquaintance.ConnectionType.player && this.isPlayer)
							{
								flag = true;
							}
							else if (aq != null)
							{
								if (aq.connections.Contains(ddsreplacement.connection))
								{
									flag = true;
								}
								else if (ddsreplacement.connection == Acquaintance.ConnectionType.anyAcquaintance)
								{
									flag = true;
								}
								else if (ddsreplacement.connection == Acquaintance.ConnectionType.friendOrWork)
								{
									if (aq.connections.Contains(Acquaintance.ConnectionType.friend) || aq.connections.Contains(Acquaintance.ConnectionType.workTeam) || aq.connections.Contains(Acquaintance.ConnectionType.workOther) || aq.connections.Contains(Acquaintance.ConnectionType.familiarWork))
									{
										flag = true;
									}
								}
								else if (ddsreplacement.connection == Acquaintance.ConnectionType.knowsName && aq.dataKeys.Contains(Evidence.DataKey.name))
								{
									flag = true;
								}
							}
							else if (ddsreplacement.connection == Acquaintance.ConnectionType.stranger)
							{
								flag = true;
							}
							if (!flag)
							{
								continue;
							}
						}
						if (ddsreplacement.useDislikeLike)
						{
							if (aq != null)
							{
								num3 += Mathf.Abs(aq.known - ddsreplacement.strangerKnown) + Mathf.Abs(aq.like - ddsreplacement.dislikeLike);
							}
							else
							{
								num3 += 1f;
							}
						}
						else
						{
							num3 += num2;
						}
						list5.Add(new Human.DDSRank
						{
							id = ddsreplacement.replaceWithID,
							rankRef = num3
						});
					}
				}
				list5.Sort((Human.DDSRank p1, Human.DDSRank p2) => p1.rankRef.CompareTo(p2.rankRef));
				list.Add(list5[0].id);
			}
		}
		return list;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002AD80 File Offset: 0x00028F80
	public virtual void SetDesiredSpeed(float newSpeedRatio)
	{
		this.desiredNormalizedSpeed = Mathf.Clamp01(newSpeedRatio - this.drunk * CitizenControls.Instance.drunkMovementPenalty);
		if (!this.visible || (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive))
		{
			this.currentNormalizedSpeed = this.desiredNormalizedSpeed;
			this.currentMovementSpeed = this.movementRunSpeed * this.currentNormalizedSpeed;
			this.UpdateMovementSpeed();
			return;
		}
		if (this.ai != null)
		{
			this.ai.SetUpdateEnabled(true);
		}
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0002AE0C File Offset: 0x0002900C
	public virtual void SetDesiredSpeed(Human.MovementSpeed newMovement)
	{
		base.SelectedDebug("Set movement speed: " + newMovement.ToString(), Actor.HumanDebug.movement);
		if (newMovement == Human.MovementSpeed.stopped)
		{
			this.SetDesiredSpeed(0f);
		}
		else if (newMovement == Human.MovementSpeed.walking)
		{
			this.SetDesiredSpeed(this.walkingSpeedRatio);
		}
		else if (newMovement == Human.MovementSpeed.running)
		{
			this.SetDesiredSpeed(1f);
		}
		if (!this.visible)
		{
			this.currentNormalizedSpeed = this.desiredNormalizedSpeed;
			this.currentMovementSpeed = this.movementRunSpeed * this.currentNormalizedSpeed;
			this.UpdateMovementSpeed();
			return;
		}
		if (this.ai != null)
		{
			this.ai.SetUpdateEnabled(true);
		}
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0002AEB0 File Offset: 0x000290B0
	public virtual void UpdateMovementSpeed()
	{
		this.currentMovementSpeed = this.movementRunSpeed * this.currentNormalizedSpeed;
		this.animationController.UpdateMovementSpeed();
		if (this.currentNormalizedSpeed <= 0f)
		{
			this.isMoving = false;
			this.isRunning = false;
			return;
		}
		this.isMoving = true;
		this.isRunning = false;
		if (this.currentNormalizedSpeed >= 1f)
		{
			this.isRunning = true;
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0002AF19 File Offset: 0x00029119
	public virtual void SetBed(Interactable passSpecificInteractable)
	{
		this.sleepPosition = passSpecificInteractable;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0002AF22 File Offset: 0x00029122
	public virtual void SetWorkFurniture(Interactable passSpecificInteractable)
	{
		this.workPosition = passSpecificInteractable;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0002AF2C File Offset: 0x0002912C
	public virtual void UpdateConversation()
	{
		if (this.currentConversation == null)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("Conversation is null!", Actor.HumanDebug.misc);
			}
			return;
		}
		if (this.currentConversation.currentMessage == null)
		{
			if (Game.Instance.collectDebugData)
			{
				base.SelectedDebug("Conversation: Set starting message", Actor.HumanDebug.misc);
			}
			this.currentConversation.SetCurrentMessage(this.currentConversation.tree.startingMessage);
		}
		if (this.currentConversation != null && this.currentConversation.currentlyTalking == this)
		{
			if (this.currentConversation.currentLink != null)
			{
				if (SessionData.Instance.gameTime < this.currentConversation.linkDelay)
				{
					this.currentConversation.timeUntilNextSpeech = this.currentConversation.linkDelay - SessionData.Instance.gameTime;
					return;
				}
				this.currentConversation.SetCurrentMessage(this.currentConversation.currentLink.to);
				this.currentConversation.speechTriggered = false;
				this.currentConversation.currentLink = null;
				this.currentConversation.linkDelay = 0f;
				return;
			}
			else
			{
				if (!this.currentConversation.speechTriggered)
				{
					if (Game.Instance.collectDebugData)
					{
						base.SelectedDebug("Conversation: Triggering speech", Actor.HumanDebug.misc);
					}
					Acquaintance aq = null;
					if (this.currentConversation.currentlyTalking != null && this.currentConversation.currentlyTalkingTo != null)
					{
						this.currentConversation.currentlyTalking.FindAcquaintanceExists(this.currentConversation.currentlyTalkingTo, out aq);
					}
					List<int> list;
					foreach (string speechEntryRef in this.ParseDDSMessage(this.currentConversation.currentMessage.msgID, aq, out list, false, null, false))
					{
						this.speechController.Speak("dds.blocks", speechEntryRef, true, false, false, 0f, false, default(Color), null, false, false, null, null, null, null);
					}
					this.currentConversation.speechTriggered = true;
					return;
				}
				if (this.speechController.speechQueue.Count <= 0 && !this.isSpeaking)
				{
					List<Human.DDSRank> conversationTreeLinkRankings = this.GetConversationTreeLinkRankings(this.currentConversation.currentMessage);
					if (Game.Instance.collectDebugData)
					{
						base.SelectedDebug("Conversation: Choose links from: " + conversationTreeLinkRankings.Count.ToString(), Actor.HumanDebug.misc);
					}
					if (conversationTreeLinkRankings.Count <= 0)
					{
						this.currentConversation.EndConversation();
						return;
					}
					this.currentConversation.currentLink = conversationTreeLinkRankings[0].linkRef;
					this.currentConversation.linkDelay = SessionData.Instance.gameTime + Toolbox.Instance.Rand(conversationTreeLinkRankings[0].linkRef.delayInterval.x, conversationTreeLinkRankings[0].linkRef.delayInterval.y, false);
					return;
				}
				else
				{
					this.currentConversation.currentlyTalkingSpeechQueue = this.speechController.speechQueue.Count;
				}
			}
		}
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0002B23C File Offset: 0x0002943C
	public List<Human.DDSRank> GetConversationTreeLinkRankings(DDSSaveClasses.DDSMessageSettings thisMsg)
	{
		List<Human.DDSRank> list = new List<Human.DDSRank>();
		string seedInput = thisMsg.instanceID + CityData.Instance.seed;
		foreach (DDSSaveClasses.DDSMessageLink ddsmessageLink in thisMsg.links)
		{
			float num = Toolbox.Instance.RandContained(-0.01f, 0.01f, seedInput, out seedInput);
			if (ddsmessageLink.useWeights)
			{
				num += ddsmessageLink.choiceWeight;
			}
			if (ddsmessageLink.useKnowLike || ddsmessageLink.useTraits)
			{
				DDSSaveClasses.DDSMessageSettings ddsmessageSettings = this.currentConversation.tree.messageRef[ddsmessageLink.to];
				Human human = this.currentConversation.participantA;
				if (ddsmessageSettings.saidBy == 1)
				{
					human = this.currentConversation.participantB;
				}
				else if (ddsmessageSettings.saidBy == 2)
				{
					human = this.currentConversation.participantC;
				}
				else if (ddsmessageSettings.saidBy == 3)
				{
					human = this.currentConversation.participantD;
				}
				if (ddsmessageLink.useKnowLike)
				{
					Acquaintance acquaintance = null;
					if (this.currentConversation.currentlyTalking.FindAcquaintanceExists(human, out acquaintance))
					{
						float num2 = Mathf.Abs(acquaintance.like - ddsmessageLink.like) + Mathf.Abs(acquaintance.known - ddsmessageLink.know);
						num += (2f - num2) / 2f;
					}
				}
				if (ddsmessageLink.useTraits && Toolbox.Instance.DDSTraitConditionLogic(this.currentConversation.currentlyTalking, human, ddsmessageLink.traitConditions, ref ddsmessageLink.traits))
				{
					num += 1f;
				}
			}
			list.Add(new Human.DDSRank
			{
				linkRef = ddsmessageLink,
				rankRef = num
			});
		}
		list.Sort((Human.DDSRank p1, Human.DDSRank p2) => p2.rankRef.CompareTo(p1.rankRef));
		return list;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0002B43C File Offset: 0x0002963C
	public void AddCurrentConsumable(InteractablePreset newPreset)
	{
		if (newPreset == null)
		{
			return;
		}
		this.currentConsumables.Add(newPreset);
		if (this.ai != null)
		{
			this.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002B470 File Offset: 0x00029670
	public void RemoveCurrentConsumable(InteractablePreset newPreset)
	{
		if (newPreset == null)
		{
			return;
		}
		this.currentConsumables.Remove(newPreset);
		if (!newPreset.destroyWhenAllConsumed)
		{
			List<Interactable.Passed> list = new List<Interactable.Passed>();
			list.Add(new Interactable.Passed(Interactable.PassedVarType.time, SessionData.Instance.gameTime, null));
			if (newPreset.useSameModelAsTrash)
			{
				this.AddTrash(newPreset, this, list);
			}
			else if (newPreset.trashItem != null)
			{
				this.AddTrash(newPreset.trashItem, this, list);
			}
		}
		if (this.ai != null)
		{
			this.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002B504 File Offset: 0x00029704
	public void AddTrash(InteractablePreset trashItem, Human writer, List<Interactable.Passed> passedVars = null)
	{
		if (Game.Instance.collectDebugData)
		{
			base.SelectedDebug("Added to trash: " + ((trashItem != null) ? trashItem.ToString() : null), Actor.HumanDebug.actions);
		}
		MetaObject metaObject = new MetaObject(trashItem, this, writer, this, passedVars);
		this.trash.Add(metaObject.id);
		if (trashItem.disposal == Human.DisposalType.anywhere)
		{
			this.anywhereTrash++;
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0002B570 File Offset: 0x00029770
	public InteractablePreset PickConsumable(ref Dictionary<InteractablePreset, int> prices, out int price, List<InteractablePreset> ignore = null)
	{
		InteractablePreset result = null;
		float num = -9999f;
		price = 0;
		foreach (KeyValuePair<InteractablePreset, int> keyValuePair in prices)
		{
			if (ignore == null || !ignore.Contains(keyValuePair.Key))
			{
				float num2 = Toolbox.Instance.Rand(0f, 2f, false);
				num2 -= (float)keyValuePair.Value * (1f - this.societalClass);
				int num3 = 0;
				if (keyValuePair.Key.retailItem != null && keyValuePair.Key.retailItem.isConsumable)
				{
					if (this.itemRanking.TryGetValue(keyValuePair.Key.retailItem, ref num3))
					{
						num2 += (float)num3;
					}
					float num4 = 0f;
					num4 += Mathf.Max(keyValuePair.Key.retailItem.nourishment - this.nourishment, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.hydration - this.hydration, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.alertness - this.alertness, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.energy - this.energy, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.excitement - this.excitement, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.chores - this.chores, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.hygiene - this.hygiene, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.bladder - this.bladder, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.heat - this.heat, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.drunk - this.drunk, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.sick - this.sick, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.headache - this.headache, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.wet - this.wet, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.brokenLeg - this.brokenLeg, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.bruised - this.bruised, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.blackEye - this.blackEye, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.blackedOut - this.blackedOut, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.numb - this.numb, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.bleeding - this.bleeding, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.wellRested - this.wellRested, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.breath - this.breath, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.poisoned - this.poisoned, 0f);
					num4 += Mathf.Max(keyValuePair.Key.retailItem.health - this.currentHealth, 0f);
					num2 += num4 * 10f;
					if (num2 > num)
					{
						result = keyValuePair.Key;
						num = num2;
						price = keyValuePair.Value;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002BA20 File Offset: 0x00029C20
	public Human GetDoctor()
	{
		string str = this.citizenName + this.humanID.ToString();
		List<Occupation> list = CityData.Instance.jobsDirectory.FindAll((Occupation item) => item.employee != null && item.employee != this && (item.preset.name == "MedicalOfficer" || item.preset.name == "Nurse" || item.preset.name == "ChiefMedicalOfficer"));
		list.Sort((Occupation p1, Occupation p2) => p1.employee.humanID.CompareTo(p2.employee.humanID));
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, str, false)].employee;
		}
		return null;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002BAB0 File Offset: 0x00029CB0
	public Human GetLandlord()
	{
		if (this.home == null)
		{
			return null;
		}
		string str = this.home.id.ToString();
		List<Occupation> list = CityData.Instance.jobsDirectory.FindAll((Occupation item) => item.employee != null && item.employee != this && item.preset.name == "SelfEmployedLandlord");
		list.Sort((Occupation p1, Occupation p2) => p1.employee.humanID.CompareTo(p2.employee.humanID));
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, str, false)].employee;
		}
		return null;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0002BB48 File Offset: 0x00029D48
	public virtual void AddMeshes(List<MeshRenderer> renderers, bool addToOutline = true, bool forceMeshListUpdate = false)
	{
		foreach (MeshRenderer newMesh in renderers)
		{
			this.AddMesh(newMesh, addToOutline, false, false, false);
		}
		if (forceMeshListUpdate)
		{
			this.UpdateMeshList();
			return;
		}
		this.updateMeshList = true;
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0002BBAC File Offset: 0x00029DAC
	public virtual void AddMesh(GameObject newObject, bool addToOutline = true, bool forceMeshListUpdate = false)
	{
		LODGroup component = newObject.GetComponent<LODGroup>();
		if (component != null)
		{
			LOD[] lods = component.GetLODs();
			if (lods.Length != 0)
			{
				foreach (Renderer renderer in lods[0].renderers)
				{
					this.AddMesh(renderer as MeshRenderer, addToOutline, false, false, false);
				}
			}
			if (lods.Length > 1)
			{
				foreach (Renderer renderer2 in lods[1].renderers)
				{
					this.AddMesh(renderer2 as MeshRenderer, addToOutline, false, true, false);
				}
			}
			component.enabled = false;
			Object.Destroy(component);
		}
		else
		{
			foreach (MeshRenderer newMesh in newObject.GetComponentsInChildren<MeshRenderer>())
			{
				this.AddMesh(newMesh, addToOutline, false, true, true);
			}
		}
		if (forceMeshListUpdate)
		{
			this.UpdateMeshList();
			return;
		}
		this.updateMeshList = true;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002BC88 File Offset: 0x00029E88
	public virtual void AddMesh(MeshRenderer newMesh, bool addToOutline = true, bool forceMeshListUpdate = false, bool addToLOD1 = false, bool addToBoth = false)
	{
		if (newMesh == null)
		{
			return;
		}
		if (addToLOD1 && !this.meshesLOD1.Contains(newMesh))
		{
			this.meshesLOD1.Add(newMesh);
			Toolbox.Instance.SetLightLayer(newMesh, this.currentBuilding, false);
		}
		if ((!addToLOD1 || addToBoth) && !this.meshes.Contains(newMesh))
		{
			this.meshes.Add(newMesh);
			Toolbox.Instance.SetLightLayer(newMesh, this.currentBuilding, false);
		}
		if (addToOutline && this.outline != null)
		{
			if (!this.outline.meshesToOutline.Contains(newMesh))
			{
				this.outline.meshesToOutline.Add(newMesh);
			}
			if (this.outline.outlineActive)
			{
				newMesh.gameObject.layer = 30;
			}
		}
		if (forceMeshListUpdate)
		{
			this.UpdateMeshList();
			return;
		}
		this.updateMeshList = true;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002BD68 File Offset: 0x00029F68
	public virtual void RemoveMesh(MeshRenderer newMesh, bool removeFromOutline = true, bool forceMeshListUpdate = false)
	{
		if (newMesh == null)
		{
			return;
		}
		this.meshes.Remove(newMesh);
		this.meshesLOD1.Remove(newMesh);
		if (removeFromOutline && this.outline != null)
		{
			this.outline.meshesToOutline.Remove(newMesh);
			if (this.outline.outlineActive)
			{
				newMesh.gameObject.layer = this.outline.normalLayer;
			}
		}
		if (forceMeshListUpdate)
		{
			this.UpdateMeshList();
			return;
		}
		this.updateMeshList = true;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002BDF0 File Offset: 0x00029FF0
	public virtual void UpdateMeshList()
	{
		this.updateMeshList = false;
		for (int i = 0; i < this.meshes.Count; i++)
		{
			if (this.meshes[i] == null)
			{
				this.meshes.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < this.meshesLOD1.Count; j++)
		{
			if (this.meshesLOD1[j] == null)
			{
				this.meshesLOD1.RemoveAt(j);
				j--;
			}
		}
		if (this.outline != null)
		{
			for (int k = 0; k < this.outline.meshesToOutline.Count; k++)
			{
				if (this.outline.meshesToOutline[k] == null)
				{
					this.outline.meshesToOutline.RemoveAt(k);
					k--;
				}
			}
		}
		this.UpdateLODs();
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0002BED8 File Offset: 0x0002A0D8
	public override void AddNerve(float amount, Actor scaredBy = null)
	{
		if (amount < 0f)
		{
			amount *= 1f - this.drunk;
		}
		if (this.isEnforcer && this.isOnDuty)
		{
			amount = Mathf.Max(0f, amount);
		}
		if (this.ai != null && this.ai.currentWeaponPreset != null && amount < 0f)
		{
			amount *= this.ai.currentWeaponPreset.incomingNerveDamageMultiplier;
		}
		base.AddNerve(amount, scaredBy);
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002BF60 File Offset: 0x0002A160
	public void UpdateLODs()
	{
		if (this.outfitController != null)
		{
			if (this.ai != null && this.ai.isRagdoll)
			{
				if (this.distantLOD.activeSelf)
				{
					this.distantLOD.SetActive(false);
				}
			}
			else if (!this.distantLOD.activeSelf)
			{
				this.distantLOD.SetActive(true);
			}
			LOD[] array = new LOD[3];
			int num = 0;
			float num2 = 0.16f;
			Renderer[] array2 = this.meshes.ToArray();
			array[num] = new LOD(num2, array2);
			int num3 = 1;
			float num4 = 0.03f;
			array2 = this.meshesLOD1.ToArray();
			array[num3] = new LOD(num4, array2);
			int num5 = 2;
			float num6 = 0.001f;
			array2 = new MeshRenderer[]
			{
				this.outfitController.distantLOD
			};
			array[num5] = new LOD(num6, array2);
			LOD[] lods = array;
			this.outfitController.lod.SetLODs(lods);
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002C048 File Offset: 0x0002A248
	public int GetHexacoScore(ref HEXACO hex)
	{
		int num = 0;
		int num2 = 0;
		if (hex.enableFeminineMasculine)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.feminineMasculine - this.genderScale * 10f));
			num++;
		}
		if (hex.enableHumility)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.humility - this.humility * 10f));
			num++;
		}
		if (hex.enableEmotionality)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.emotionality - this.emotionality * 10f));
			num++;
		}
		if (hex.enableExtraversion)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.extraversion - this.extraversion * 10f));
			num++;
		}
		if (hex.enableAgreeableness)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.agreeableness - this.agreeableness * 10f));
			num++;
		}
		if (hex.enableConscientiousness)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.conscientiousness - this.conscientiousness * 10f));
			num++;
		}
		if (hex.enableCreativity)
		{
			num2 += 10 - Mathf.RoundToInt(Mathf.Abs((float)hex.creativity - this.creativity * 10f));
			num++;
		}
		if (num2 <= 0 || num <= 0)
		{
			return Mathf.CeilToInt(Mathf.Lerp((float)hex.outputMin, (float)hex.outputMax, 0.5f));
		}
		float num3 = (float)num2 / (float)num;
		return Mathf.CeilToInt(Mathf.Lerp((float)hex.outputMin, (float)hex.outputMax, num3 / 10f));
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0002C208 File Offset: 0x0002A408
	public void WalletItemCheck(int maxNewItems)
	{
		int num = Mathf.Clamp(Mathf.CeilToInt(this.societalClass * 4f), 2, 4);
		int num2 = 0;
		while (num2 < maxNewItems && this.walletItems.Count < num)
		{
			if (!this.characterTraits.Exists((Human.Trait item) => item.trait == GameplayControls.Instance.donorCardTrait))
			{
				goto IL_C7;
			}
			if (this.walletItems.Exists((Human.WalletItem item) => item.meta > 0 && CityData.Instance.FindMetaObject(item.meta) != null && CityData.Instance.FindMetaObject(item.meta).preset == InteriorControls.Instance.donorCard.name))
			{
				goto IL_C7;
			}
			Human.WalletItem walletItem = new Human.WalletItem();
			walletItem.itemType = Human.WalletItemType.evidence;
			MetaObject metaObject = new MetaObject(InteriorControls.Instance.donorCard, this, this, this, null);
			walletItem.meta = metaObject.id;
			this.walletItems.Add(walletItem);
			IL_27F:
			num2++;
			continue;
			IL_C7:
			if (this.characterTraits.Exists((Human.Trait item) => item.trait == GameplayControls.Instance.creditCardTrait))
			{
				if (!this.walletItems.Exists((Human.WalletItem item) => item.meta > 0 && CityData.Instance.FindMetaObject(item.meta) != null && CityData.Instance.FindMetaObject(item.meta).preset == InteriorControls.Instance.creditCard.name))
				{
					Human.WalletItem walletItem2 = new Human.WalletItem();
					walletItem2.itemType = Human.WalletItemType.evidence;
					MetaObject metaObject2 = new MetaObject(InteriorControls.Instance.creditCard, this, this, this, null);
					walletItem2.meta = metaObject2.id;
					this.walletItems.Add(walletItem2);
					goto IL_27F;
				}
			}
			if (Toolbox.Instance.Rand(0f, 1f, false) > 0.5f)
			{
				if (!this.walletItems.Exists((Human.WalletItem item) => item.itemType == Human.WalletItemType.key) && this.home != null)
				{
					Human.WalletItem walletItem3 = new Human.WalletItem();
					walletItem3.itemType = Human.WalletItemType.key;
					this.walletItems.Add(walletItem3);
					goto IL_27F;
				}
			}
			if (Toolbox.Instance.Rand(0f, 1f, false) >= this.societalClass)
			{
				goto IL_27F;
			}
			if (!this.walletItems.Exists((Human.WalletItem item) => item.itemType == Human.WalletItemType.money))
			{
				Human.WalletItem walletItem4 = new Human.WalletItem();
				walletItem4.itemType = Human.WalletItemType.money;
				walletItem4.money = Mathf.Max(Mathf.RoundToInt(GameplayControls.Instance.walletCashAmountBasedOnWealth.Evaluate(this.societalClass) * Toolbox.Instance.Rand(0.7f, 1.3f, false)), 1);
				this.walletItems.Add(walletItem4);
				goto IL_27F;
			}
			goto IL_27F;
		}
		Toolbox.Instance.ShuffleList(ref this.walletItems);
		for (int i = 0; i < this.walletItems.Count; i++)
		{
			if (this.walletItems[i].itemType == Human.WalletItemType.nothing)
			{
				this.walletItems.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0002C4F0 File Offset: 0x0002A6F0
	public void UpdateLastSighting(Human citizen, bool phoneCall = false, int isSound = 0)
	{
		if (citizen == null)
		{
			return;
		}
		bool poi = false;
		if (MurderController.Instance.activeMurders.Exists((MurderController.Murder item) => item.murderer == citizen && (item.state == MurderController.MurderState.travellingTo || item.state == MurderController.MurderState.executing || item.state == MurderController.MurderState.post || item.state == MurderController.MurderState.escaping)))
		{
			poi = true;
		}
		if (this.lastSightings.ContainsKey(citizen))
		{
			this.lastSightings[citizen].time = SessionData.Instance.gameTime;
			this.lastSightings[citizen].node = citizen.currentNodeCoord;
			this.lastSightings[citizen].run = citizen.isRunning;
			if (citizen.ai != null && citizen.ai.currentExpression != null)
			{
				this.lastSightings[citizen].exp = (int)citizen.ai.currentExpression.expression;
			}
			else
			{
				this.lastSightings[citizen].exp = 0;
			}
			this.lastSightings[citizen].phone = phoneCall;
			if (citizen.drunk > 0.2f)
			{
				this.lastSightings[citizen].drunk = true;
			}
			if (citizen.ai != null && citizen.currentGameLocation.thisAsStreet != null && citizen.ai.currentAction != null && citizen.isMoving && citizen.ai.currentAction.path != null && citizen.ai.pathCursor + 2 < citizen.ai.currentAction.path.accessList.Count)
			{
				this.lastSightings[citizen].mov = true;
				this.lastSightings[citizen].dest = citizen.ai.currentAction.path.GetNodeAhead(citizen.ai.pathCursor + 2).nodeCoord;
			}
			else
			{
				this.lastSightings[citizen].mov = false;
			}
			this.lastSightings[citizen].poi = poi;
			this.lastSightings[citizen].sound = isSound;
			return;
		}
		if (this.lastSightings.Keys.Count >= this.sightingMemoryLimit)
		{
			float num = 999999f;
			Human human = null;
			foreach (Human human2 in this.lastSightings.Keys)
			{
				if (this.lastSightings[human2].time < num)
				{
					num = this.lastSightings[human2].time;
					human = human2;
				}
			}
			if (human != null)
			{
				this.lastSightings.Remove(human);
			}
		}
		Human.Sighting sighting = new Human.Sighting();
		sighting.time = SessionData.Instance.gameTime;
		sighting.node = citizen.currentNodeCoord;
		sighting.run = citizen.isRunning;
		if (citizen.ai != null && citizen.ai.currentExpression != null)
		{
			sighting.exp = (int)citizen.ai.currentExpression.expression;
		}
		sighting.phone = phoneCall;
		if (citizen.drunk > 0.2f)
		{
			sighting.drunk = true;
		}
		if (citizen.ai != null && citizen.currentGameLocation.thisAsStreet != null && citizen.ai.currentAction != null && citizen.isMoving && citizen.ai.currentAction.path != null && citizen.ai.pathCursor + 2 < citizen.ai.currentAction.path.accessList.Count)
		{
			sighting.mov = true;
			sighting.dest = citizen.ai.currentAction.path.GetNodeAhead(citizen.ai.pathCursor + 2).nodeCoord;
		}
		else
		{
			sighting.mov = false;
		}
		sighting.poi = poi;
		sighting.sound = isSound;
		this.lastSightings.Add(citizen, sighting);
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0002CA08 File Offset: 0x0002AC08
	public Vector2 GetSightingDirection(Human.Sighting sighting, out NewGameLocation newDestination)
	{
		newDestination = null;
		NewNode newNode = null;
		NewNode newNode2 = null;
		Vector2 result = Vector2.zero;
		if (sighting.mov && PathFinder.Instance.nodeMap.TryGetValue(sighting.dest, ref newNode2) && PathFinder.Instance.nodeMap.TryGetValue(sighting.node, ref newNode))
		{
			if (newNode2.gameLocation != newNode.gameLocation)
			{
				newDestination = newNode2.gameLocation;
			}
			else
			{
				result = (newNode2.nodeCoord - newNode.nodeCoord).normalized;
			}
		}
		return result;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0002CAA0 File Offset: 0x0002ACA0
	public void RevealSighting(Human prospectCitizen, bool allowCalls, bool allowSounds, bool allowGeneralClue = true)
	{
		if (this.currentGameLocation.currentOccupants.Contains(prospectCitizen))
		{
			this.speechController.Speak("a81a7552-f196-45de-8da3-c6fe1254cfeb", false, false, prospectCitizen, null);
			return;
		}
		if (this.lastSightings.ContainsKey(prospectCitizen) && (allowCalls || !this.lastSightings[prospectCitizen].phone) && (allowSounds || this.lastSightings[prospectCitizen].sound == 0))
		{
			Human.Sighting sighting = this.lastSightings[prospectCitizen];
			this.RevealSighting(prospectCitizen, sighting);
			return;
		}
		if (!allowGeneralClue)
		{
			this.speechController.Speak("aeba5683-cb14-4df7-a95c-04025dfcd5d0", false, false, prospectCitizen, null);
			return;
		}
		if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, this.citizenName, false) > 0.5f && prospectCitizen.job != null && prospectCitizen.job.employer != null)
		{
			this.speechController.Speak("7099af81-6c9b-411d-af8b-df6a5f6d6520", false, false, prospectCitizen, null);
			return;
		}
		if (prospectCitizen.home != null)
		{
			this.speechController.Speak("f916152e-3a58-4f75-ba03-fca880a7a340", false, false, prospectCitizen, null);
			return;
		}
		this.speechController.Speak("aeba5683-cb14-4df7-a95c-04025dfcd5d0", false, false, prospectCitizen, null);
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0002CBC4 File Offset: 0x0002ADC4
	public void RevealSighting(Human prospectCitizen, Human.Sighting sighting)
	{
		float num = 0f;
		int num2 = 0;
		int num3;
		int num4;
		int num5;
		SessionData.Instance.ParseTimeData(sighting.time, out num, out num2, out num3, out num4, out num5);
		if (num2 == SessionData.Instance.dayInt)
		{
			if (sighting.sound == 1)
			{
				this.speechController.Speak("79b84ed9-5c40-4927-9b57-f096cea0930e", false, false, prospectCitizen, null);
			}
			else if (sighting.sound == 2)
			{
				this.speechController.Speak("d2ac8d3d-1a62-410a-8a73-fd69965679e5", false, false, prospectCitizen, null);
			}
			else if (sighting.phone)
			{
				this.speechController.Speak("d6f5edea-422f-4d8b-b31f-7780ac9f2760", false, false, prospectCitizen, null);
			}
			else
			{
				this.speechController.Speak("3a4fc598-8523-4b91-9ced-9fd4e288cf61", false, false, prospectCitizen, null);
			}
		}
		else if (sighting.sound == 1)
		{
			this.speechController.Speak("772b36f5-b8b6-4f4b-9b2f-5e8ad188e5d3", false, false, prospectCitizen, null);
		}
		else if (sighting.sound == 2)
		{
			this.speechController.Speak("be09a225-d2aa-40bd-b69a-bc04505cec48", false, false, prospectCitizen, null);
		}
		else if (sighting.phone)
		{
			this.speechController.Speak("19946aed-4866-4035-838d-fad6b4c5f20b", false, false, prospectCitizen, null);
		}
		else
		{
			this.speechController.Speak("22068015-912e-43bf-be30-c8bf312a2592", false, false, prospectCitizen, null);
		}
		if (sighting.sound <= 0)
		{
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(sighting.node, ref newNode) && newNode.gameLocation.thisAsStreet != null)
			{
				this.speechController.Speak("915bba5b-f242-42b4-8f9e-3315a767ad58", false, false, prospectCitizen, null);
				NewGameLocation newGameLocation = null;
				this.GetSightingDirection(sighting, out newGameLocation);
				if (newGameLocation != null)
				{
					this.speechController.Speak("79beb68a-30a6-46e5-bf79-dfeb4c9d9241", false, false, prospectCitizen, null);
				}
				else if (sighting.mov)
				{
					this.speechController.Speak("79beb68a-30a6-46e5-bf79-dfeb4c9d9241", false, false, prospectCitizen, null);
				}
			}
			if (sighting.run && !sighting.phone)
			{
				this.speechController.Speak("860362f6-c03a-4300-9c6d-9be6d679d9b7", false, false, prospectCitizen, null);
			}
			if (sighting.drunk)
			{
				this.speechController.Speak("193eaf1d-4ad4-461a-a60f-d3f25057a4c5", false, false, prospectCitizen, null);
			}
			if (!sighting.phone)
			{
				if (sighting.exp == 1)
				{
					this.speechController.Speak("c4621de3-8513-48e8-803e-39a1b2558046", false, false, prospectCitizen, null);
					return;
				}
				if (sighting.exp == 2)
				{
					this.speechController.Speak("b3d8492a-962b-4525-a672-599870327d7e", false, false, prospectCitizen, null);
					return;
				}
				if (sighting.exp == 4)
				{
					this.speechController.Speak("fa2ced7b-81e3-42fb-a412-3e41df9417ab", false, false, prospectCitizen, null);
					return;
				}
			}
		}
		else
		{
			this.speechController.Speak("d073a4c0-319c-4ee7-bf99-6a112c234070", false, false, prospectCitizen, null);
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0002CE3C File Offset: 0x0002B03C
	public Vector3 GetNearestVert(Vector3 worldPosition, out CitizenOutfitController.CharacterAnchor nearestBodyPart)
	{
		nearestBodyPart = CitizenOutfitController.CharacterAnchor.upperTorso;
		if (this.outfitController != null)
		{
			float num = float.PositiveInfinity;
			Vector3 result = Vector3.zero;
			bool flag = false;
			foreach (MeshFilter meshFilter in this.outfitController.allCurrentMeshFilters)
			{
				if (!(meshFilter == null))
				{
					if (this.outfitController != null)
					{
						bool flag2 = false;
						foreach (KeyValuePair<CitizenOutfitController.CharacterAnchor, Transform> keyValuePair in this.outfitController.anchorReference)
						{
							if (keyValuePair.Value == meshFilter.transform || keyValuePair.Value == meshFilter.transform.parent)
							{
								nearestBodyPart = keyValuePair.Key;
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							continue;
						}
					}
					Vector3 vector = meshFilter.transform.InverseTransformPoint(worldPosition);
					if (meshFilter.mesh.isReadable)
					{
						foreach (Vector3 vector2 in meshFilter.mesh.vertices)
						{
							float sqrMagnitude = (vector - vector2).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
								result = vector2;
								if (num <= 0.1f)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return result;
		}
		return Vector3.zero;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0002CFF0 File Offset: 0x0002B1F0
	public string GetCitizenName()
	{
		string result = this.citizenName;
		if (StreamingOptionsController.Instance != null && StreamingOptionsController.Instance.customNames.Count > 0 && StreamingOptionsController.Instance.enableTwitchAudienceCitizens && !this.isPlayer)
		{
			try
			{
				if (this.humanID < StreamingOptionsController.Instance.customNames.Count)
				{
					result = this.GetFirstName() + " " + this.GetSurName();
				}
			}
			catch
			{
				Game.Log("Streaming: Failed to get twitch audience name for " + base.name, 2);
			}
		}
		return result;
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002D094 File Offset: 0x0002B294
	public string GetFirstName()
	{
		string result = this.firstName;
		if (StreamingOptionsController.Instance != null && StreamingOptionsController.Instance.customNames.Count > 0 && StreamingOptionsController.Instance.enableTwitchAudienceCitizens && !this.isPlayer && StreamingOptionsController.Instance.customNames.Count > 0 && this.humanID < StreamingOptionsController.Instance.customNames.Count)
		{
			try
			{
				if (StreamingOptionsController.Instance.customNames[this.humanID].firstName != null && StreamingOptionsController.Instance.customNames[this.humanID].firstName.Length > 0)
				{
					result = StreamingOptionsController.Instance.customNames[this.humanID].firstName;
				}
			}
			catch
			{
				Game.Log("Streaming: Failed to get twitch audience name for " + base.name, 2);
			}
		}
		return result;
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0002D198 File Offset: 0x0002B398
	public string GetCasualName()
	{
		string result = this.GetFirstName();
		if (StreamingOptionsController.Instance != null && !StreamingOptionsController.Instance.enableTwitchAudienceCitizens && this.casualName != null && this.casualName.Length > 0)
		{
			result = this.casualName;
		}
		return result;
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0002D1E4 File Offset: 0x0002B3E4
	public string GetSurName()
	{
		string result = this.surName;
		if (StreamingOptionsController.Instance != null && StreamingOptionsController.Instance.enableTwitchAudienceCitizens && !this.isPlayer && StreamingOptionsController.Instance.customNames.Count > 0 && this.humanID < StreamingOptionsController.Instance.customNames.Count)
		{
			try
			{
				if (StreamingOptionsController.Instance.customNames[this.humanID].surName != null && StreamingOptionsController.Instance.customNames[this.humanID].surName.Length > 0)
				{
					result = StreamingOptionsController.Instance.customNames[this.humanID].surName;
				}
			}
			catch
			{
				Game.Log("Streaming: ...Failed to get twitch audience name for " + base.name, 2);
			}
		}
		return result;
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0002D2D4 File Offset: 0x0002B4D4
	public string GetInitialledName()
	{
		string result = string.Empty;
		string text = this.GetFirstName();
		if (text.Length > 0)
		{
			result = text.Substring(0, 1) + ". " + this.GetSurName();
		}
		else
		{
			result = text + ". " + this.GetSurName();
		}
		return result;
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0002D328 File Offset: 0x0002B528
	public string GetInitials()
	{
		string result = string.Empty;
		string text = this.GetFirstName();
		string text2 = this.GetSurName();
		if (text.Length > 0 && text2.Length > 0)
		{
			result = text.Substring(0, 1) + text2.Substring(0, 1);
		}
		return result;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0002D374 File Offset: 0x0002B574
	[Button(null, 0)]
	public void DebugGetAge()
	{
		Game.Log(this.GetAge(), 2);
		Game.Log(this.GetAgeGroup(), 2);
		Game.Log(this.birthday, 2);
		Game.Log(SessionData.Instance.yearInt, 2);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0002D3C4 File Offset: 0x0002B5C4
	public bool TryGiveItem(Interactable givenItem, Human givenBy, bool defaultSuccess, bool enableSpeech = true)
	{
		if (givenItem == null)
		{
			return false;
		}
		bool flag = false;
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			for (int i = 0; i < newBuilding.lostAndFound.Count; i++)
			{
				GameplayController.LostAndFound lf = newBuilding.lostAndFound[i];
				if (lf.preset == givenItem.preset.presetName)
				{
					Human human = null;
					CityData.Instance.citizenDictionary.TryGetValue(lf.ownerID, ref human);
					if (lf.ownerID == this.humanID)
					{
						if (enableSpeech && this.speechController != null)
						{
							this.speechController.Speak("8b018440-f220-4d57-a9c9-9b97adfd46d3", false, false, human, null);
						}
						newBuilding.CompleteLostAndFound(human as Citizen, givenItem.preset, true);
						flag = true;
						break;
					}
					if (human != null && this.home != null && this.home.inhabitants.Exists((Human item) => item.humanID == lf.ownerID))
					{
						if (enableSpeech && this.speechController != null)
						{
							this.speechController.Speak("3caae4b3-3c6a-4c38-b1d2-fe981cb5654e", false, false, human, null);
						}
						newBuilding.CompleteLostAndFound(human as Citizen, givenItem.preset, true);
						if (human.isDead)
						{
							flag = true;
							break;
						}
						human.TryGiveItem(givenItem, givenBy, true, false);
						return true;
					}
					else if (this.job != null && this.job.employer != null && human != null && human.job != null && human.job.employer == this.job.employer)
					{
						if (enableSpeech && this.speechController != null)
						{
							this.speechController.Speak("3caae4b3-3c6a-4c38-b1d2-fe981cb5654e", false, false, human, null);
						}
						newBuilding.CompleteLostAndFound(human as Citizen, givenItem.preset, true);
						if (human.isDead)
						{
							flag = true;
							break;
						}
						human.TryGiveItem(givenItem, givenBy, true, false);
						return true;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			if (defaultSuccess)
			{
				if (givenItem.val <= 1f)
				{
					if (enableSpeech && this.speechController != null)
					{
						this.speechController.Speak("1d7fd151-6ab6-426f-9a26-b0e38d9ddb44", false, false, null, null);
					}
				}
				else if (enableSpeech && this.speechController != null)
				{
					this.speechController.Speak("af828eb2-57b9-4af2-a287-15530fab21ce", false, false, null, null);
				}
			}
			else
			{
				if (givenItem.preset.retailItem != null)
				{
					if (givenItem.preset.retailItem.nourishment > 0.1f && this.nourishment < 0.5f)
					{
						defaultSuccess = true;
						if (enableSpeech && this.speechController != null)
						{
							this.speechController.Speak("48bc129e-0841-4e10-9ac5-00f0acc56ec0", false, false, null, null);
						}
					}
					else if (givenItem.preset.retailItem.hydration > 0.1f && this.hydration < 0.5f)
					{
						defaultSuccess = true;
						if (enableSpeech && this.speechController != null)
						{
							this.speechController.Speak("bf120304-ed08-4e6d-b93b-49b081ca9e7d", false, false, null, null);
						}
					}
				}
				if (givenItem.val >= 10f && !defaultSuccess)
				{
					defaultSuccess = true;
					if (enableSpeech && this.speechController != null)
					{
						this.speechController.Speak("af828eb2-57b9-4af2-a287-15530fab21ce", false, false, null, null);
					}
				}
				if (!defaultSuccess && enableSpeech && this.speechController != null)
				{
					this.speechController.Speak("4e2f4c3e-99b9-4e1a-a9d6-ad8098d5901e", false, false, null, null);
				}
			}
		}
		if (flag || defaultSuccess)
		{
			if (givenItem.preset.retailItem != null && givenItem.preset.consumableAmount > 0f && givenItem.cs > 0f)
			{
				this.AddCurrentConsumable(givenItem.preset);
			}
			if (givenBy != null && givenBy.isPlayer)
			{
				FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.GetInteractable() == givenItem);
				if (inventorySlot != null)
				{
					FirstPersonItemController.Instance.EmptySlot(inventorySlot, false, false, true, true);
					givenItem.SetInInventory(this);
				}
				if (givenItem.preset.retailItem != null && givenItem.preset.retailItem.tags.Contains(RetailItemPreset.Tags.starchProduct))
				{
					int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.starchGive));
					if (num > 0)
					{
						GameplayController.Instance.AddMoney(num, true, "Starch give");
					}
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x04000258 RID: 600
	[Header("ID")]
	public int humanID = -1;

	// Token: 0x04000259 RID: 601
	[NonSerialized]
	public static int assignID = 2;

	// Token: 0x0400025A RID: 602
	[NonSerialized]
	public static int assignTraitID = 1;

	// Token: 0x0400025B RID: 603
	[NonSerialized]
	public string seed;

	// Token: 0x0400025C RID: 604
	[Header("Current Variables")]
	public Human.ShoeType footwear;

	// Token: 0x0400025D RID: 605
	[NonSerialized]
	public AudioEvent footstepEvent;

	// Token: 0x0400025E RID: 606
	public float footstepDirt;

	// Token: 0x0400025F RID: 607
	public float footstepBlood;

	// Token: 0x04000260 RID: 608
	public Transform leftFoot;

	// Token: 0x04000261 RID: 609
	public Transform rightFoot;

	// Token: 0x04000262 RID: 610
	public bool removedFromWorld;

	// Token: 0x04000263 RID: 611
	[Header("Human Attributes")]
	public NewAddress home;

	// Token: 0x04000264 RID: 612
	[NonSerialized]
	public ResidenceController residence;

	// Token: 0x04000265 RID: 613
	public NewAddress den;

	// Token: 0x04000266 RID: 614
	[Header("Movement")]
	[Tooltip("Used for variation of the base values")]
	public float speedMultiplier = 1f;

	// Token: 0x04000267 RID: 615
	[Tooltip("Calculated walking speed")]
	public float movementWalkSpeed = 1.85f;

	// Token: 0x04000268 RID: 616
	[Tooltip("Calculated running speed")]
	public float movementRunSpeed = 2f;

	// Token: 0x04000269 RID: 617
	[Tooltip("The calculated walking speed ratio")]
	[NonSerialized]
	public float walkingSpeedRatio = 0.4625f;

	// Token: 0x0400026A RID: 618
	[Space(5f)]
	[Tooltip("Speed as a ratio of maximum (0 - 1)")]
	public float currentNormalizedSpeed;

	// Token: 0x0400026B RID: 619
	[Tooltip("The desired movement speed")]
	public float desiredNormalizedSpeed;

	// Token: 0x0400026C RID: 620
	[Tooltip("The actual movement speed")]
	public float currentMovementSpeed = 1.85f;

	// Token: 0x0400026D RID: 621
	[Tooltip("Recovery rate")]
	public float breathRecoveryRate = 1f;

	// Token: 0x0400026E RID: 622
	[Header("Job")]
	public Occupation job;

	// Token: 0x0400026F RID: 623
	public Company director;

	// Token: 0x04000270 RID: 624
	[ReadOnly]
	public float societalClass;

	// Token: 0x04000271 RID: 625
	[Header("Personal Data")]
	public Descriptors descriptors;

	// Token: 0x04000272 RID: 626
	public CitizenOutfitController outfitController;

	// Token: 0x04000273 RID: 627
	public HandwritingPreset handwriting;

	// Token: 0x04000274 RID: 628
	[ReadOnly]
	public string birthday;

	// Token: 0x04000275 RID: 629
	[Space(7f)]
	public string citizenName = string.Empty;

	// Token: 0x04000276 RID: 630
	[NonSerialized]
	public string firstName = string.Empty;

	// Token: 0x04000277 RID: 631
	[NonSerialized]
	public string casualName = string.Empty;

	// Token: 0x04000278 RID: 632
	[NonSerialized]
	public string surName = string.Empty;

	// Token: 0x04000279 RID: 633
	[Space(7f)]
	[NonSerialized]
	public float genderScale = 0.5f;

	// Token: 0x0400027A RID: 634
	public Human.Gender gender;

	// Token: 0x0400027B RID: 635
	public Human.Gender birthGender;

	// Token: 0x0400027C RID: 636
	[Tooltip("How often this person uses the below slang terms")]
	[Space(7f)]
	public float slangUsage = 0.5f;

	// Token: 0x0400027D RID: 637
	[Space(7f)]
	public float sexuality = 0.5f;

	// Token: 0x0400027E RID: 638
	public float homosexuality = 0.5f;

	// Token: 0x0400027F RID: 639
	[NonSerialized]
	public List<Human.Gender> attractedTo = new List<Human.Gender>();

	// Token: 0x04000280 RID: 640
	[Space(7f)]
	public Citizen partner;

	// Token: 0x04000281 RID: 641
	public string anniversary;

	// Token: 0x04000282 RID: 642
	public Citizen paramour;

	// Token: 0x04000283 RID: 643
	[Space(7f)]
	[NonSerialized]
	public int fingerprintLoop = -1;

	// Token: 0x04000284 RID: 644
	public Human.BloodType bloodType = Human.BloodType.Apos;

	// Token: 0x04000285 RID: 645
	[Space(7f)]
	[NonSerialized]
	public int favColourIndex;

	// Token: 0x04000286 RID: 646
	[Header("Human Traits & Personality")]
	[ProgressBar("Humility", 1f, 9)]
	public float humility;

	// Token: 0x04000287 RID: 647
	[ProgressBar("Emotionality", 1f, 9)]
	public float emotionality;

	// Token: 0x04000288 RID: 648
	[ProgressBar("Extraversion", 1f, 9)]
	public float extraversion;

	// Token: 0x04000289 RID: 649
	[ProgressBar("Agreeableness", 1f, 9)]
	public float agreeableness;

	// Token: 0x0400028A RID: 650
	[ProgressBar("Conscientiousness", 1f, 9)]
	public float conscientiousness;

	// Token: 0x0400028B RID: 651
	[ProgressBar("Creativity", 1f, 9)]
	public float creativity;

	// Token: 0x0400028C RID: 652
	[NonSerialized]
	public float sleepNeedMultiplier = 1f;

	// Token: 0x0400028D RID: 653
	[NonSerialized]
	public float snoring = 1f;

	// Token: 0x0400028E RID: 654
	[NonSerialized]
	public float snoreDelay = 1.5f;

	// Token: 0x0400028F RID: 655
	[Space(7f)]
	[NonSerialized]
	public Vector2 limitHumility = new Vector2(0f, 1f);

	// Token: 0x04000290 RID: 656
	[NonSerialized]
	public Vector2 limitEmotionality = new Vector2(0f, 1f);

	// Token: 0x04000291 RID: 657
	[NonSerialized]
	public Vector2 limitExtraversion = new Vector2(0f, 1f);

	// Token: 0x04000292 RID: 658
	[NonSerialized]
	public Vector2 limitAgreeableness = new Vector2(0f, 1f);

	// Token: 0x04000293 RID: 659
	[NonSerialized]
	public Vector2 limitConscientiousness = new Vector2(0f, 1f);

	// Token: 0x04000294 RID: 660
	[NonSerialized]
	public Vector2 limitCreativity = new Vector2(0f, 1f);

	// Token: 0x04000295 RID: 661
	public List<Human.Trait> characterTraits = new List<Human.Trait>();

	// Token: 0x04000296 RID: 662
	[Header("Groups")]
	public List<GroupsController.SocialGroup> groups = new List<GroupsController.SocialGroup>();

	// Token: 0x04000297 RID: 663
	[Header("Status Stats")]
	public float nourishment;

	// Token: 0x04000298 RID: 664
	public float hydration;

	// Token: 0x04000299 RID: 665
	public float alertness;

	// Token: 0x0400029A RID: 666
	public float energy;

	// Token: 0x0400029B RID: 667
	[ProgressBar("Excitement", 1f, 7)]
	public float excitement;

	// Token: 0x0400029C RID: 668
	[ProgressBar("Chores", 1f, 7)]
	public float chores;

	// Token: 0x0400029D RID: 669
	[ProgressBar("Hygiene", 1f, 7)]
	public float hygiene;

	// Token: 0x0400029E RID: 670
	[ProgressBar("Bladder", 1f, 7)]
	public float bladder;

	// Token: 0x0400029F RID: 671
	[ProgressBar("Breath", 1f, 7)]
	public float breath = 1f;

	// Token: 0x040002A0 RID: 672
	public float heat;

	// Token: 0x040002A1 RID: 673
	public float drunk;

	// Token: 0x040002A2 RID: 674
	public float sick;

	// Token: 0x040002A3 RID: 675
	public float headache;

	// Token: 0x040002A4 RID: 676
	public float wet;

	// Token: 0x040002A5 RID: 677
	public float brokenLeg;

	// Token: 0x040002A6 RID: 678
	public float bruised;

	// Token: 0x040002A7 RID: 679
	public float blackEye;

	// Token: 0x040002A8 RID: 680
	public float blackedOut;

	// Token: 0x040002A9 RID: 681
	public float numb;

	// Token: 0x040002AA RID: 682
	public float poisoned;

	// Token: 0x040002AB RID: 683
	public float bleeding;

	// Token: 0x040002AC RID: 684
	public float wellRested;

	// Token: 0x040002AD RID: 685
	public float starchAddiction;

	// Token: 0x040002AE RID: 686
	public float syncDiskInstall;

	// Token: 0x040002AF RID: 687
	public float blinded;

	// Token: 0x040002B0 RID: 688
	public Human poisoner;

	// Token: 0x040002B1 RID: 689
	[Header("Acquaintances")]
	[NonSerialized]
	public List<Acquaintance> acquaintances = new List<Acquaintance>();

	// Token: 0x040002B2 RID: 690
	[Header("Vocab")]
	public Dictionary<DDSSaveClasses.TriggerPoint, List<DDSSaveClasses.DDSTreeSave>> dds = new Dictionary<DDSSaveClasses.TriggerPoint, List<DDSSaveClasses.DDSTreeSave>>();

	// Token: 0x040002B3 RID: 691
	[NonSerialized]
	public Human.ConversationInstance currentConversation;

	// Token: 0x040002B4 RID: 692
	public float nextCasualSpeechValidAt;

	// Token: 0x040002B5 RID: 693
	public Dictionary<Human, Human.Sighting> lastSightings = new Dictionary<Human, Human.Sighting>();

	// Token: 0x040002B6 RID: 694
	public int sightingMemoryLimit = 100;

	// Token: 0x040002B7 RID: 695
	private Dictionary<DDSSaveClasses.DDSTreeSave, List<Human.SpeechHistory>> speechHistory = new Dictionary<DDSSaveClasses.DDSTreeSave, List<Human.SpeechHistory>>();

	// Token: 0x040002B8 RID: 696
	[NonSerialized]
	public List<StateSaveData.MessageThreadSave> messageThreadsStarted = new List<StateSaveData.MessageThreadSave>();

	// Token: 0x040002B9 RID: 697
	[NonSerialized]
	public List<StateSaveData.MessageThreadSave> messageThreadFeatures = new List<StateSaveData.MessageThreadSave>();

	// Token: 0x040002BA RID: 698
	[NonSerialized]
	public List<StateSaveData.MessageThreadSave> messageThreadCCd = new List<StateSaveData.MessageThreadSave>();

	// Token: 0x040002BB RID: 699
	[Header("Possessions")]
	[NonSerialized]
	public Evidence addressBook;

	// Token: 0x040002BC RID: 700
	[NonSerialized]
	public bool setupAddressBook;

	// Token: 0x040002BD RID: 701
	[NonSerialized]
	public Evidence workID;

	// Token: 0x040002BE RID: 702
	[NonSerialized]
	public List<Interactable> birthdayCards = new List<Interactable>();

	// Token: 0x040002BF RID: 703
	public List<InteractablePreset> currentConsumables = new List<InteractablePreset>();

	// Token: 0x040002C0 RID: 704
	public List<int> trash = new List<int>();

	// Token: 0x040002C1 RID: 705
	public int anywhereTrash;

	// Token: 0x040002C2 RID: 706
	public Human.Death death;

	// Token: 0x040002C3 RID: 707
	public List<Human.Wound> currentWounds = new List<Human.Wound>();

	// Token: 0x040002C4 RID: 708
	public List<Human.WalletItem> walletItems = new List<Human.WalletItem>();

	// Token: 0x040002C5 RID: 709
	public Dictionary<string, Fact> factDictionary = new Dictionary<string, Fact>();

	// Token: 0x040002C6 RID: 710
	[Header("Personal Affects")]
	public List<InteractablePreset> personalAffects = new List<InteractablePreset>();

	// Token: 0x040002C7 RID: 711
	public List<InteractablePreset> workAffects = new List<InteractablePreset>();

	// Token: 0x040002C8 RID: 712
	[NonSerialized]
	public Interactable workPosition;

	// Token: 0x040002C9 RID: 713
	[NonSerialized]
	public Interactable sleepPosition;

	// Token: 0x040002CA RID: 714
	private int preferredBookCount = 3;

	// Token: 0x040002CB RID: 715
	public List<BookPreset> library = new List<BookPreset>();

	// Token: 0x040002CC RID: 716
	public List<BookPreset> nonShelfBooks = new List<BookPreset>();

	// Token: 0x040002CD RID: 717
	[NonSerialized]
	public int booksAwayFromShelf;

	// Token: 0x040002CE RID: 718
	public Dictionary<RetailItemPreset, int> itemRanking = new Dictionary<RetailItemPreset, int>();

	// Token: 0x040002CF RID: 719
	public Dictionary<CompanyPreset.CompanyCategory, NewAddress> favouritePlaces = new Dictionary<CompanyPreset.CompanyCategory, NewAddress>();

	// Token: 0x040002D0 RID: 720
	public Dictionary<RetailItemPreset, float> recentPurchases = new Dictionary<RetailItemPreset, float>();

	// Token: 0x040002D1 RID: 721
	[Header("Passwords")]
	public GameplayController.Passcode passcode;

	// Token: 0x040002D2 RID: 722
	public CharacterTrait passwordTrait;

	// Token: 0x040002D3 RID: 723
	[Header("Simulated Behaviour")]
	private List<float> simulatedPreviousBehaviour = new List<float>();

	// Token: 0x040002D4 RID: 724
	[Header("Misc")]
	public Vector2 lastUsedCCTVScreenPoint;

	// Token: 0x040002D5 RID: 725
	public bool updateMeshList;

	// Token: 0x040002D6 RID: 726
	[Header("Debug")]
	public Human.ConversationInstance debugConversation;

	// Token: 0x0200004A RID: 74
	public enum ShoeType
	{
		// Token: 0x040002D8 RID: 728
		normal,
		// Token: 0x040002D9 RID: 729
		boots,
		// Token: 0x040002DA RID: 730
		heel,
		// Token: 0x040002DB RID: 731
		barefoot
	}

	// Token: 0x0200004B RID: 75
	public enum MovementSpeed
	{
		// Token: 0x040002DD RID: 733
		stopped,
		// Token: 0x040002DE RID: 734
		walking,
		// Token: 0x040002DF RID: 735
		running
	}

	// Token: 0x0200004C RID: 76
	public enum Gender
	{
		// Token: 0x040002E1 RID: 737
		male,
		// Token: 0x040002E2 RID: 738
		female,
		// Token: 0x040002E3 RID: 739
		nonBinary
	}

	// Token: 0x0200004D RID: 77
	public enum BloodType
	{
		// Token: 0x040002E5 RID: 741
		unassigned,
		// Token: 0x040002E6 RID: 742
		Apos,
		// Token: 0x040002E7 RID: 743
		Aneg,
		// Token: 0x040002E8 RID: 744
		Bpos,
		// Token: 0x040002E9 RID: 745
		Bneg,
		// Token: 0x040002EA RID: 746
		Opos,
		// Token: 0x040002EB RID: 747
		Oneg,
		// Token: 0x040002EC RID: 748
		ABpos,
		// Token: 0x040002ED RID: 749
		ABneg
	}

	// Token: 0x0200004E RID: 78
	[Serializable]
	public class Trait
	{
		// Token: 0x040002EE RID: 750
		public string name;

		// Token: 0x040002EF RID: 751
		public int traitID;

		// Token: 0x040002F0 RID: 752
		public CharacterTrait trait;

		// Token: 0x040002F1 RID: 753
		public Human.Trait reason;

		// Token: 0x040002F2 RID: 754
		public string date;
	}

	// Token: 0x0200004F RID: 79
	[Serializable]
	public class Sighting
	{
		// Token: 0x040002F3 RID: 755
		public float time;

		// Token: 0x040002F4 RID: 756
		public Vector3 node;

		// Token: 0x040002F5 RID: 757
		public bool mov;

		// Token: 0x040002F6 RID: 758
		public Vector3 dest;

		// Token: 0x040002F7 RID: 759
		public bool run;

		// Token: 0x040002F8 RID: 760
		public int exp;

		// Token: 0x040002F9 RID: 761
		public bool drunk;

		// Token: 0x040002FA RID: 762
		public bool phone;

		// Token: 0x040002FB RID: 763
		public bool poi;

		// Token: 0x040002FC RID: 764
		public int sound;
	}

	// Token: 0x02000050 RID: 80
	[Serializable]
	public class ConversationInstance
	{
		// Token: 0x06000384 RID: 900 RVA: 0x0002DCF0 File Offset: 0x0002BEF0
		public void EndConversation()
		{
			this.active = false;
			if (this.participantA != null)
			{
				if (!this.participantA.isPlayer)
				{
					this.participantA.SetInConversation(null, true);
				}
				if (this.participantA.ai != null)
				{
					this.participantA.ai.faceTransform = null;
				}
			}
			if (this.participantB != null)
			{
				if (!this.participantB.isPlayer)
				{
					this.participantB.SetInConversation(null, true);
				}
				if (this.participantB.ai != null)
				{
					this.participantB.ai.faceTransform = null;
				}
			}
			if (this.participantC != null)
			{
				if (!this.participantC.isPlayer)
				{
					this.participantC.SetInConversation(null, true);
				}
				if (this.participantC.ai != null)
				{
					this.participantC.ai.faceTransform = null;
				}
			}
			if (this.participantD != null)
			{
				if (!this.participantD.isPlayer)
				{
					this.participantD.SetInConversation(null, true);
				}
				if (this.participantD.ai != null)
				{
					this.participantD.ai.faceTransform = null;
				}
			}
			this.room.activeConversations.Remove(this);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0002DE48 File Offset: 0x0002C048
		public void SetCurrentMessage(string instanceID)
		{
			if (this.tree.messageRef.TryGetValue(instanceID, ref this.currentMessage))
			{
				this.previouslyTalking = this.currentlyTalking;
				this.currentlyTalking = this.participantA;
				if (this.currentMessage.saidBy == 1)
				{
					this.currentlyTalking = this.participantB;
				}
				else if (this.currentMessage.saidBy == 2)
				{
					this.currentlyTalking = this.participantC;
				}
				else if (this.currentMessage.saidBy == 3)
				{
					this.currentlyTalking = this.participantD;
				}
				this.currentlyTalkingTo = this.participantA;
				if (this.currentMessage.saidTo == 1)
				{
					this.currentlyTalkingTo = this.participantB;
				}
				else if (this.currentMessage.saidTo == 2)
				{
					this.currentlyTalkingTo = this.participantC;
				}
				else if (this.currentMessage.saidTo == 3)
				{
					this.currentlyTalkingTo = this.participantD;
				}
				this.speechTriggered = false;
				return;
			}
			if (!this.tree.messages.Exists((DDSSaveClasses.DDSMessageSettings item) => item.instanceID == instanceID))
			{
				Game.LogError(string.Concat(new string[]
				{
					"...Message ",
					instanceID,
					" does not exist within tree ",
					this.tree.name,
					" messages (msg count: ",
					this.tree.messages.Count.ToString(),
					")"
				}), 2);
			}
			Game.LogError(this.tree.name + ": Failed to set current conversation message instance ID: " + instanceID, 2);
			this.EndConversation();
		}

		// Token: 0x040002FD RID: 765
		[Tooltip("True if active")]
		public bool active;

		// Token: 0x040002FE RID: 766
		public NewRoom room;

		// Token: 0x040002FF RID: 767
		[NonSerialized]
		public DDSSaveClasses.DDSTreeSave tree;

		// Token: 0x04000300 RID: 768
		public Human participantA;

		// Token: 0x04000301 RID: 769
		public Human participantB;

		// Token: 0x04000302 RID: 770
		public Human participantC;

		// Token: 0x04000303 RID: 771
		public Human participantD;

		// Token: 0x04000304 RID: 772
		[Space(7f)]
		public Human previouslyTalking;

		// Token: 0x04000305 RID: 773
		public Human currentlyTalking;

		// Token: 0x04000306 RID: 774
		public Human currentlyTalkingTo;

		// Token: 0x04000307 RID: 775
		public bool speechTriggered;

		// Token: 0x04000308 RID: 776
		[NonSerialized]
		public DDSSaveClasses.DDSMessageSettings currentMessage;

		// Token: 0x04000309 RID: 777
		[NonSerialized]
		public DDSSaveClasses.DDSMessageLink currentLink;

		// Token: 0x0400030A RID: 778
		public float linkDelay;

		// Token: 0x0400030B RID: 779
		[Header("Debug")]
		public float timeUntilNextSpeech;

		// Token: 0x0400030C RID: 780
		public int currentlyTalkingSpeechQueue;

		// Token: 0x0400030D RID: 781
		public string treeName;
	}

	// Token: 0x02000052 RID: 82
	public class DDSRank
	{
		// Token: 0x0400030F RID: 783
		public string id;

		// Token: 0x04000310 RID: 784
		public DDSSaveClasses.DDSMessageLink linkRef;

		// Token: 0x04000311 RID: 785
		public float rankRef;
	}

	// Token: 0x02000053 RID: 83
	public class SpeechHistory
	{
		// Token: 0x04000312 RID: 786
		public float timeStamp;

		// Token: 0x04000313 RID: 787
		public List<Human> participants = new List<Human>();
	}

	// Token: 0x02000054 RID: 84
	public enum DisposalType
	{
		// Token: 0x04000315 RID: 789
		anywhere,
		// Token: 0x04000316 RID: 790
		homeOnly,
		// Token: 0x04000317 RID: 791
		workOnly,
		// Token: 0x04000318 RID: 792
		homeOrWork
	}

	// Token: 0x02000055 RID: 85
	[Serializable]
	public class Wound
	{
		// Token: 0x0600038B RID: 907 RVA: 0x0002E024 File Offset: 0x0002C224
		public void Load()
		{
			if (CityData.Instance.GetHuman(this.humanID, out this.human, true))
			{
				Transform bodyAnchor = this.human.outfitController.GetBodyAnchor(this.anchor);
				this.interactable.parentTransform = bodyAnchor;
				this.interactable.MainSetupStart();
				this.interactable.OnLoad();
				this.interactable.MarkAsTrash(true, false, 0f);
				this.interactable.objectRef = this;
			}
			if (this.bloodPoolID > 0)
			{
				CityData.Instance.savableInteractableDictionary.TryGetValue(this.bloodPoolID, ref this.bloodPool);
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0002E0C8 File Offset: 0x0002C2C8
		public void ProcessBloodPoolForWound()
		{
			if (this.bloodPoolAmount > 0.01f)
			{
				if (this.bloodPoolID <= 0)
				{
					Vector3 worldPosition = this.interactable.GetWorldPosition(true);
					worldPosition.y = this.human.transform.position.y;
					this.bloodPool = InteractableCreator.Instance.CreateWorldInteractable(PrefabControls.Instance.bloodPool, this.human, this.human, this.human, worldPosition, this.human.transform.eulerAngles, null, null, "");
					if (this.bloodPool != null)
					{
						this.bloodPool.MarkAsTrash(true, true, SessionData.Instance.gameTime + 24f);
						this.bloodPoolID = this.bloodPool.id;
						return;
					}
				}
				else if (this.bloodPool != null)
				{
					this.bloodPool.SetValue(this.bloodPool.val + this.bloodPoolAmount);
				}
			}
		}

		// Token: 0x04000319 RID: 793
		public int humanID = -1;

		// Token: 0x0400031A RID: 794
		public Interactable interactable;

		// Token: 0x0400031B RID: 795
		public CitizenOutfitController.CharacterAnchor anchor;

		// Token: 0x0400031C RID: 796
		public float timestamp;

		// Token: 0x0400031D RID: 797
		public int bloodPoolID;

		// Token: 0x0400031E RID: 798
		public float bloodPoolAmount;

		// Token: 0x0400031F RID: 799
		[NonSerialized]
		public Human human;

		// Token: 0x04000320 RID: 800
		[NonSerialized]
		public Interactable bloodPool;
	}

	// Token: 0x02000056 RID: 86
	[Serializable]
	public class Death
	{
		// Token: 0x0600038E RID: 910 RVA: 0x0002E1CC File Offset: 0x0002C3CC
		public Death(Human newVictim, MurderController.Murder newMurder, Human newKiller, Interactable newWeapon)
		{
			this.isDead = true;
			if (newMurder != null)
			{
				this.murder = newMurder.murderID;
			}
			newMurder.death = this;
			this.victim = newVictim.humanID;
			this.location = newVictim.currentNodeCoord;
			this.time = SessionData.Instance.gameTime;
			newMurder.time = this.time;
			this.timeOfDeathRange = Toolbox.Instance.CreateTimeRange(this.time, GameplayControls.Instance.timeOfDeathAccuracy, false, true, 15);
			if (newWeapon != null)
			{
				this.weapon = newWeapon.id;
			}
			this.killer = newKiller.humanID;
			if (newVictim.currentRoom != null)
			{
				newVictim.currentRoom.containsDead = true;
			}
			newVictim.death = this;
			CityData.Instance.deadCitizensDirectory.Add(newVictim);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0002E2AE File Offset: 0x0002C4AE
		public void UpdateDeathLocation(NewNode newNode)
		{
			if (newNode == null)
			{
				return;
			}
			this.location = newNode.nodeCoord;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0002E2C8 File Offset: 0x0002C4C8
		public void SetReported(Human newFoundBy, Human.Death.ReportType newReportType)
		{
			if (!this.reported)
			{
				this.reported = true;
				Human human = this.GetVictim();
				if (human != null && human.currentNode != null)
				{
					this.UpdateDeathLocation(human.currentNode);
				}
				this.discoveredAt = SessionData.Instance.gameTime;
				this.discoveredBy = newFoundBy.humanID;
				this.reportType = newReportType;
				Human human2 = null;
				if (CityData.Instance.GetHuman(this.victim, out human2, true) && human2.ai != null)
				{
					foreach (Acquaintance acquaintance in human2.acquaintances)
					{
						if (acquaintance.with.humanID != this.killer && !(acquaintance.with.ai == null) && (acquaintance.known > SocialControls.Instance.knowMournThreshold || acquaintance.connections[0] == Acquaintance.ConnectionType.lover || acquaintance.connections[0] == Acquaintance.ConnectionType.paramour || acquaintance.connections[0] == Acquaintance.ConnectionType.housemate))
						{
							NewAIGoal newAIGoal = acquaintance.with.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.mourn);
							if (newAIGoal == null)
							{
								Game.Log("Create mourn goal for " + acquaintance.with.GetCitizenName(), 2);
								acquaintance.with.ai.CreateNewGoal(RoutineControls.Instance.mourn, 0f, 0f, null, null, null, null, null, -2);
							}
							else
							{
								newAIGoal.activeTime = 0f;
							}
						}
					}
				}
				MurderController.Instance.CoverUpFailCheck(this.GetVictim());
			}
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0002E4B8 File Offset: 0x0002C6B8
		public Human GetVictim()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.victim, out result, true);
			return result;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0002E4DC File Offset: 0x0002C6DC
		public Human GetKiller()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.killer, out result, true);
			return result;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0002E500 File Offset: 0x0002C700
		public Human GetDiscoverer()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.discoveredBy, out result, true);
			return result;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0002E524 File Offset: 0x0002C724
		public EvidenceTime GetTimeOfDeathEvidence()
		{
			this.timeOfDeathRange.y = Mathf.Min(this.timeOfDeathRange.y, SessionData.Instance.gameTime);
			return EvidenceCreator.Instance.GetTimeEvidence(this.timeOfDeathRange.x, this.timeOfDeathRange.y, "TimeOfDeath", "", this.victim, -1);
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0002E588 File Offset: 0x0002C788
		public NewGameLocation GetDeathLocation()
		{
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(this.location, ref newNode))
			{
				return newNode.gameLocation;
			}
			string text = "Cannot get death location from node coord ";
			Vector3 vector = this.location;
			Game.LogError(text + vector.ToString(), 2);
			return null;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0002E5DC File Offset: 0x0002C7DC
		public MurderController.Murder GetMurder()
		{
			MurderController.Murder murder = MurderController.Instance.activeMurders.Find((MurderController.Murder item) => item.murderID == this.murder);
			if (murder == null)
			{
				murder = MurderController.Instance.inactiveMurders.Find((MurderController.Murder item) => item.murderID == this.murder);
			}
			return murder;
		}

		// Token: 0x04000321 RID: 801
		[Header("Death")]
		public bool isDead;

		// Token: 0x04000322 RID: 802
		public Vector3 location;

		// Token: 0x04000323 RID: 803
		public float time;

		// Token: 0x04000324 RID: 804
		public Vector2 timeOfDeathRange;

		// Token: 0x04000325 RID: 805
		public int weapon;

		// Token: 0x04000326 RID: 806
		public int murder = -1;

		// Token: 0x04000327 RID: 807
		public int victim;

		// Token: 0x04000328 RID: 808
		public int killer;

		// Token: 0x04000329 RID: 809
		public int discoveredBy;

		// Token: 0x0400032A RID: 810
		public float discoveredAt;

		// Token: 0x0400032B RID: 811
		public bool reported;

		// Token: 0x0400032C RID: 812
		public Human.Death.ReportType reportType;

		// Token: 0x0400032D RID: 813
		public float smell;

		// Token: 0x02000057 RID: 87
		public enum ReportType
		{
			// Token: 0x0400032F RID: 815
			visual,
			// Token: 0x04000330 RID: 816
			smell,
			// Token: 0x04000331 RID: 817
			audio
		}
	}

	// Token: 0x02000059 RID: 89
	[Serializable]
	public class WalletItem
	{
		// Token: 0x04000334 RID: 820
		public Human.WalletItemType itemType;

		// Token: 0x04000335 RID: 821
		public int meta = -1;

		// Token: 0x04000336 RID: 822
		public int money;
	}

	// Token: 0x0200005A RID: 90
	public enum WalletItemType
	{
		// Token: 0x04000338 RID: 824
		nothing,
		// Token: 0x04000339 RID: 825
		money,
		// Token: 0x0400033A RID: 826
		evidence,
		// Token: 0x0400033B RID: 827
		key
	}

	// Token: 0x0200005B RID: 91
	public struct BookChoice
	{
		// Token: 0x0400033C RID: 828
		public BookPreset p;

		// Token: 0x0400033D RID: 829
		public float rank;
	}

	// Token: 0x0200005C RID: 92
	public enum NoteObject
	{
		// Token: 0x0400033F RID: 831
		note,
		// Token: 0x04000340 RID: 832
		letter,
		// Token: 0x04000341 RID: 833
		travelReceipt,
		// Token: 0x04000342 RID: 834
		vmailLetter
	}
}
