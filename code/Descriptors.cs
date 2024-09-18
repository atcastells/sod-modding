using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D7 RID: 215
[Serializable]
public class Descriptors
{
	// Token: 0x06000618 RID: 1560 RVA: 0x00060964 File Offset: 0x0005EB64
	public Descriptors(Human newCitizen)
	{
		this.citizen = newCitizen;
		this.visualDistinctiveness = Toolbox.Instance.GetPsuedoRandomNumberContained(0.4f, 0.6f, newCitizen.seed, out newCitizen.seed);
		this.GenerateEthnicity();
		this.GenerateEyes();
		this.GenerateHair();
		this.GenerateBuild();
		this.heightCM = (float)Mathf.RoundToInt(Toolbox.Instance.RandomRangeWeightedSeedContained(SocialStatistics.Instance.heightMinMax.x, SocialStatistics.Instance.heightMinMax.y, SocialStatistics.Instance.averageHeight, newCitizen.seed, out newCitizen.seed, 6));
		if (this.heightCM < 153f)
		{
			this.height = Descriptors.Height.veryShort;
		}
		else if (this.heightCM < 168f)
		{
			this.height = Descriptors.Height.hShort;
		}
		else if (this.heightCM < 183f)
		{
			this.height = Descriptors.Height.hAverage;
		}
		else if (this.heightCM < 198f)
		{
			this.height = Descriptors.Height.tall;
		}
		else
		{
			this.height = Descriptors.Height.veryTall;
		}
		float num = 2f;
		if (this.build == Descriptors.BuildType.skinny)
		{
			num = 0.3f + Toolbox.Instance.GetPsuedoRandomNumberContained(-0.1f, 0.1f, newCitizen.seed, out newCitizen.seed);
			this.visualDistinctiveness += 0.1f;
		}
		else if (this.build == Descriptors.BuildType.average)
		{
			num = 0.4f + Toolbox.Instance.GetPsuedoRandomNumberContained(-0.1f, 0.1f, newCitizen.seed, out newCitizen.seed);
		}
		else if (this.build == Descriptors.BuildType.overweight)
		{
			num = 0.5f + Toolbox.Instance.GetPsuedoRandomNumberContained(-0.1f, 0.1f, newCitizen.seed, out newCitizen.seed);
			this.visualDistinctiveness += 0.125f;
		}
		else if (this.build == Descriptors.BuildType.muscular)
		{
			num = 0.5f + Toolbox.Instance.GetPsuedoRandomNumberContained(-0.1f, 0.1f, newCitizen.seed, out newCitizen.seed);
			this.visualDistinctiveness += 0.2f;
		}
		this.weightKG = (float)Mathf.RoundToInt(this.heightCM * num);
		float num2 = this.heightCM / SocialStatistics.Instance.averageHeight;
		float num3 = this.weightKG / SocialStatistics.Instance.averageWeight;
		this.citizen.SetMaxHealth(num2 * num3, false);
		this.citizen.SetCombatHeft(num2 * num3 * CitizenControls.Instance.citizenCombatHeftMultiplier);
		this.citizen.SetRecoveryRate(CitizenControls.Instance.citizenBaseRecoveryRate);
		this.citizen.SetCombatSkill(Toolbox.Instance.GetPsuedoRandomNumberContained(CitizenControls.Instance.citizenBaseCombatSkillRange.x, CitizenControls.Instance.citizenBaseCombatSkillRange.y, newCitizen.seed, out newCitizen.seed));
		this.citizen.SetMaxNerve(this.citizen.combatSkill, true);
		this.GenerateFootwearPreference();
		float num4 = Mathf.Clamp01(num2 * num3 - 0.5f + (this.citizen.genderScale - 0.5f));
		float weightedValue = Mathf.Lerp(CitizenControls.Instance.shoeSizeRange.x, CitizenControls.Instance.shoeSizeRange.y, num4);
		this.shoeSize = Mathf.RoundToInt(Toolbox.Instance.RandomRangeWeightedSeedContained(CitizenControls.Instance.shoeSizeRange.x, CitizenControls.Instance.shoeSizeRange.y, weightedValue, newCitizen.seed, out newCitizen.seed, 6));
		CityData.Instance.averageShoeSize += (float)this.shoeSize;
		this.GenerateFacialFeatures();
		this.visualDistinctiveness = Mathf.Clamp01(this.visualDistinctiveness);
		if (this.hairType == Descriptors.HairStyle.bald)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.bald);
			return;
		}
		if (this.hairType == Descriptors.HairStyle.shortHair)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.shortHair);
			return;
		}
		if (this.hairType == Descriptors.HairStyle.longHair)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.longHair);
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00060DC0 File Offset: 0x0005EFC0
	private void GenerateEthnicity()
	{
		do
		{
			Descriptors.EthnicitySetting newEth = new Descriptors.EthnicitySetting();
			newEth.group = Toolbox.Instance.RandomEthnicGroup(this.citizen.seed);
			newEth.ratio = 1f;
			newEth.stats = SocialStatistics.Instance.ethnicityStats.Find((SocialStatistics.EthnicityStats item) => item.group == newEth.group);
			if (!this.ethnicities.Exists((Descriptors.EthnicitySetting item) => item.group == newEth.group))
			{
				this.ethnicities.Add(newEth);
			}
		}
		while (this.ethnicities.Count <= 1 && Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) <= SocialStatistics.Instance.chanceOf2ndEthnicity);
		if (this.ethnicities.Count == 2)
		{
			this.ethnicities[0].ratio = (float)Toolbox.Instance.GetPsuedoRandomNumberContained(50, 90, this.citizen.seed, out this.citizen.seed) / 100f;
			this.ethnicities[1].ratio = 1f - this.ethnicities[0].ratio;
		}
		this.citizen.AddCharacterTrait(this.ethnicities[0].stats.ethTraits[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.ethnicities[0].stats.ethTraits.Count, this.citizen.seed, out this.citizen.seed)]);
		this.GenerateNameAndSkinColour();
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x00060F7C File Offset: 0x0005F17C
	public void GenerateNameAndSkinColour()
	{
		bool flag = false;
		Descriptors.EthnicGroup ethnicGroup = Descriptors.EthnicGroup.westEuropean;
		Descriptors.EthnicGroup ethnicGroup2 = Descriptors.EthnicGroup.westEuropean;
		foreach (Descriptors.EthnicitySetting ethnicitySetting in this.ethnicities)
		{
			Color color = Color.Lerp(ethnicitySetting.stats.skinColourRange1, ethnicitySetting.stats.skinColourRange2, Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.citizen.seed, out this.citizen.seed));
			if (flag)
			{
				this.skinColour = Color.Lerp(this.skinColour, color, ethnicitySetting.ratio);
				if (ethnicitySetting.ratio >= Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.citizen.seed, out this.citizen.seed))
				{
					ethnicGroup = ethnicitySetting.group;
				}
				if (ethnicitySetting.ratio >= Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.citizen.seed, out this.citizen.seed))
				{
					ethnicGroup2 = ethnicitySetting.group;
				}
			}
			else
			{
				this.skinColour = color;
				ethnicGroup = ethnicitySetting.group;
				ethnicGroup2 = ethnicitySetting.group;
				flag = true;
			}
			if (ethnicitySetting.stats.overrideFirst)
			{
				ethnicGroup = ethnicitySetting.stats.overrideNameFirst;
			}
			if (ethnicitySetting.stats.overrideSur)
			{
				ethnicGroup2 = ethnicitySetting.stats.overrideNameSur;
			}
		}
		string text = "male";
		if (this.citizen.gender == Human.Gender.female)
		{
			text = "female";
		}
		int num = 99;
		string text2;
		string text3;
		string text4;
		bool flag2;
		string text5;
		this.citizen.firstName = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup.ToString() + ".first." + text, 1f, null, 0f, out text2, out text3, out text4, out flag2, out text5, this.citizen.seed);
		this.citizen.surName = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup2.ToString() + ".sur", 1f, null, 0f, out text2, out text3, out text4, out flag2, out text5, this.citizen.seed);
		while (CityData.Instance.citizenDirectory.Exists((Citizen item) => item != this.citizen && item.firstName == this.citizen.firstName && item.surName == this.citizen.surName) && num > 0)
		{
			Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.citizen.seed, out this.citizen.seed);
			this.citizen.firstName = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup.ToString() + ".first." + text, 1f, null, 0f, out text2, out text3, out text4, out flag2, out text5, this.citizen.seed);
			this.citizen.surName = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup2.ToString() + ".sur", 1f, null, 0f, out text2, out text3, out text4, out flag2, out text5, this.citizen.seed);
			num--;
		}
		this.citizen.citizenName = this.citizen.firstName + " " + this.citizen.surName;
		this.citizen.casualName = Strings.Get("names." + ethnicGroup.ToString() + ".first." + text, this.citizen.firstName, Strings.Casing.asIs, true, false, false, null);
		this.citizen.gameObject.name = this.citizen.citizenName;
		this.citizen.name = this.citizen.citizenName;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00061384 File Offset: 0x0005F584
	private void GenerateEyes()
	{
		List<Descriptors.EyeColour> list = new List<Descriptors.EyeColour>();
		foreach (Descriptors.EthnicitySetting ethnicitySetting in this.ethnicities)
		{
			for (int i = 0; i < Mathf.RoundToInt((float)ethnicitySetting.stats.blueEyesRatio * ethnicitySetting.ratio); i++)
			{
				list.Add(Descriptors.EyeColour.blueEyes);
			}
			for (int j = 0; j < Mathf.RoundToInt((float)ethnicitySetting.stats.brownEyesRatio * ethnicitySetting.ratio); j++)
			{
				list.Add(Descriptors.EyeColour.brownEyes);
			}
			for (int k = 0; k < Mathf.RoundToInt((float)ethnicitySetting.stats.greenEyesRatio * ethnicitySetting.ratio); k++)
			{
				list.Add(Descriptors.EyeColour.greenEyes);
			}
			for (int l = 0; l < Mathf.RoundToInt((float)ethnicitySetting.stats.greyEyesRatio * ethnicitySetting.ratio); l++)
			{
				list.Add(Descriptors.EyeColour.greyEyes);
			}
		}
		this.eyeColour = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.citizen.seed, out this.citizen.seed)];
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000614C4 File Offset: 0x0005F6C4
	private void GenerateHair()
	{
		List<Descriptors.HairColour> list = new List<Descriptors.HairColour>();
		List<Descriptors.HairStyle> list2 = new List<Descriptors.HairStyle>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		float num15 = Mathf.Clamp01((float)(this.citizen.GetAge() - 33) * 3f / 100f);
		foreach (Descriptors.EthnicitySetting ethnicitySetting in this.ethnicities)
		{
			num += Mathf.RoundToInt((float)ethnicitySetting.stats.blackHairRatio * ethnicitySetting.ratio * (1f - num15));
			num2 += Mathf.RoundToInt((float)ethnicitySetting.stats.brownHairRatio * ethnicitySetting.ratio * (1f - num15));
			num3 += Mathf.RoundToInt((float)ethnicitySetting.stats.blondeHairRatio * ethnicitySetting.ratio * (1f - num15));
			num4 += Mathf.RoundToInt((float)ethnicitySetting.stats.gingerHairRatio * ethnicitySetting.ratio * (1f - num15));
			num5 += Mathf.RoundToInt((float)ethnicitySetting.stats.RedHairRatio * ethnicitySetting.ratio);
			num6 += Mathf.RoundToInt((float)ethnicitySetting.stats.blueHairRatio * ethnicitySetting.ratio);
			num7 += Mathf.RoundToInt((float)ethnicitySetting.stats.greenHairRatio * ethnicitySetting.ratio);
			num8 += Mathf.RoundToInt((float)ethnicitySetting.stats.purpleHairRatio * ethnicitySetting.ratio);
			num9 += Mathf.RoundToInt((float)ethnicitySetting.stats.pinkHairRatio * ethnicitySetting.ratio);
			num10 += Mathf.RoundToInt((float)ethnicitySetting.stats.greyHairRatio * ethnicitySetting.ratio) + Mathf.RoundToInt(num15 * 25f);
			num11 += Mathf.RoundToInt((float)ethnicitySetting.stats.whiteHairRatio * ethnicitySetting.ratio) + Mathf.RoundToInt(num15 * 25f);
			float num16 = (this.citizen.extraversion + this.citizen.emotionality + this.citizen.creativity) / 3f;
			num5 += Mathf.RoundToInt((float)SocialStatistics.Instance.RedHairRatio * num16);
			num6 += Mathf.RoundToInt((float)SocialStatistics.Instance.blueHairRatio * num16);
			num7 += Mathf.RoundToInt((float)SocialStatistics.Instance.greenHairRatio * num16);
			num8 += Mathf.RoundToInt((float)SocialStatistics.Instance.purpleHairRatio * num16);
			num9 += Mathf.RoundToInt((float)SocialStatistics.Instance.pinkHairRatio * num16);
			if (this.citizen.gender == Human.Gender.male)
			{
				num12 += Mathf.RoundToInt((float)ethnicitySetting.stats.baldHairRatioMale * ethnicitySetting.ratio) + Mathf.RoundToInt(num15 * 20f);
				num13 += Mathf.RoundToInt((float)ethnicitySetting.stats.shortHairRatioMale * ethnicitySetting.ratio);
				num14 = Mathf.RoundToInt((float)ethnicitySetting.stats.longHairRatioMale * ethnicitySetting.ratio);
			}
			else if (this.citizen.gender == Human.Gender.female)
			{
				num12 += Mathf.RoundToInt((float)ethnicitySetting.stats.baldHairRatioFemale * ethnicitySetting.ratio);
				num13 += Mathf.RoundToInt((float)ethnicitySetting.stats.shortHairRatioFemale * ethnicitySetting.ratio);
				num14 = Mathf.RoundToInt((float)ethnicitySetting.stats.longHairRatioFemale * ethnicitySetting.ratio);
			}
		}
		for (int i = 0; i < Mathf.Max(num, 1); i++)
		{
			list.Add(Descriptors.HairColour.black);
		}
		for (int j = 0; j < Mathf.Max(num2, 1); j++)
		{
			list.Add(Descriptors.HairColour.brown);
		}
		for (int k = 0; k < Mathf.Max(num3, 1); k++)
		{
			list.Add(Descriptors.HairColour.blonde);
		}
		for (int l = 0; l < Mathf.Max(num4, 1); l++)
		{
			list.Add(Descriptors.HairColour.ginger);
		}
		for (int m = 0; m < Mathf.Max(num5, 1); m++)
		{
			list.Add(Descriptors.HairColour.red);
		}
		for (int n = 0; n < Mathf.Max(num6, 1); n++)
		{
			list.Add(Descriptors.HairColour.blue);
		}
		for (int num17 = 0; num17 < Mathf.Max(num7, 1); num17++)
		{
			list.Add(Descriptors.HairColour.green);
		}
		for (int num18 = 0; num18 < Mathf.Max(num8, 1); num18++)
		{
			list.Add(Descriptors.HairColour.purple);
		}
		for (int num19 = 0; num19 < Mathf.Max(num9, 1); num19++)
		{
			list.Add(Descriptors.HairColour.pink);
		}
		for (int num20 = 0; num20 < Mathf.Max(num10, 1); num20++)
		{
			list.Add(Descriptors.HairColour.grey);
		}
		for (int num21 = 0; num21 < Mathf.Max(num11, 1); num21++)
		{
			list.Add(Descriptors.HairColour.white);
		}
		for (int num22 = 0; num22 < Mathf.Max(num12, 1); num22++)
		{
			list2.Add(Descriptors.HairStyle.bald);
		}
		for (int num23 = 0; num23 < Mathf.Max(num13, 1); num23++)
		{
			list2.Add(Descriptors.HairStyle.shortHair);
		}
		for (int num24 = 0; num24 < Mathf.Max(num14, 1); num24++)
		{
			list2.Add(Descriptors.HairStyle.longHair);
		}
		this.hairColourCategory = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.citizen.seed, out this.citizen.seed)];
		this.hairType = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, this.citizen.seed, out this.citizen.seed)];
		SocialStatistics.HairSetting hairSetting = SocialStatistics.Instance.hairColourSettings.Find((SocialStatistics.HairSetting item) => item.colour == this.hairColourCategory);
		this.hairColour = Color.Lerp(hairSetting.hairColourRange1, hairSetting.hairColourRange2, Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.citizen.seed, out this.citizen.seed));
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x00061AF8 File Offset: 0x0005FCF8
	private void GenerateBuild()
	{
		int num = Mathf.Max(Mathf.RoundToInt((float)SocialStatistics.Instance.skinnyRatio * (0.6f + this.citizen.conscientiousness)), 0);
		int num2 = Mathf.Max(SocialStatistics.Instance.averageRatio, 0);
		int num3 = Mathf.Max(Mathf.RoundToInt((float)SocialStatistics.Instance.overweightRatio * (1.6f - this.citizen.conscientiousness)), 0);
		int num4 = Mathf.Max(Mathf.RoundToInt((float)SocialStatistics.Instance.muscleyRatio * (0.5f + this.citizen.conscientiousness)), 0);
		List<Descriptors.BuildType> list = new List<Descriptors.BuildType>();
		for (int i = 0; i < num; i++)
		{
			list.Add(Descriptors.BuildType.skinny);
		}
		for (int j = 0; j < num2; j++)
		{
			list.Add(Descriptors.BuildType.average);
		}
		for (int k = 0; k < num3; k++)
		{
			list.Add(Descriptors.BuildType.overweight);
		}
		for (int l = 0; l < num4; l++)
		{
			list.Add(Descriptors.BuildType.muscular);
		}
		this.build = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.citizen.seed, out this.citizen.seed)];
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00061C30 File Offset: 0x0005FE30
	private void GenerateFacialFeatures()
	{
		new List<Descriptors.FacialFeature>();
		int num = Mathf.RoundToInt((1f - this.citizen.societalClass) * (float)this.citizen.GetAge() * ((float)SocialStatistics.Instance.scaringRatio / 100f));
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < num)
		{
			Descriptors.FacialFeaturesSetting facialFeaturesSetting = default(Descriptors.FacialFeaturesSetting);
			facialFeaturesSetting.feature = Descriptors.FacialFeature.scaring;
			facialFeaturesSetting.id = 0;
			this.facialFeatures.Add(facialFeaturesSetting);
		}
		if (this.citizen.gender == Human.Gender.male || this.citizen.gender == Human.Gender.nonBinary)
		{
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < SocialStatistics.Instance.menWithBeards)
			{
				Descriptors.FacialFeaturesSetting facialFeaturesSetting2 = default(Descriptors.FacialFeaturesSetting);
				facialFeaturesSetting2.feature = Descriptors.FacialFeature.beard;
				facialFeaturesSetting2.id = 0;
				this.facialFeatures.Add(facialFeaturesSetting2);
			}
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < SocialStatistics.Instance.menWithMoustaches)
			{
				Descriptors.FacialFeaturesSetting facialFeaturesSetting3 = default(Descriptors.FacialFeaturesSetting);
				facialFeaturesSetting3.feature = Descriptors.FacialFeature.moustache;
				facialFeaturesSetting3.id = 0;
				this.facialFeatures.Add(facialFeaturesSetting3);
			}
		}
		int num2 = Mathf.RoundToInt((1f - this.citizen.societalClass) * (100f - (float)this.citizen.GetAge()) * ((float)SocialStatistics.Instance.piercingRatio / 100f));
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < num2)
		{
			Descriptors.FacialFeaturesSetting facialFeaturesSetting4 = default(Descriptors.FacialFeaturesSetting);
			facialFeaturesSetting4.feature = Descriptors.FacialFeature.piercing;
			facialFeaturesSetting4.id = 0;
			this.facialFeatures.Add(facialFeaturesSetting4);
		}
		int num3 = Mathf.RoundToInt((1f - this.citizen.societalClass) * (100f - (float)this.citizen.GetAge()) * ((float)SocialStatistics.Instance.TattooRatio / 100f));
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < num3)
		{
			Descriptors.FacialFeaturesSetting facialFeaturesSetting5 = default(Descriptors.FacialFeaturesSetting);
			facialFeaturesSetting5.feature = Descriptors.FacialFeature.tattoo;
			facialFeaturesSetting5.id = 0;
			this.facialFeatures.Add(facialFeaturesSetting5);
		}
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < SocialStatistics.Instance.glassesRatio)
		{
			Descriptors.FacialFeaturesSetting facialFeaturesSetting6 = default(Descriptors.FacialFeaturesSetting);
			facialFeaturesSetting6.feature = Descriptors.FacialFeature.glasses;
			facialFeaturesSetting6.id = 0;
			this.facialFeatures.Add(facialFeaturesSetting6);
		}
		if (Toolbox.Instance.GetPsuedoRandomNumberContained(0, 100, this.citizen.seed, out this.citizen.seed) < SocialStatistics.Instance.moleRatio)
		{
			Descriptors.FacialFeaturesSetting facialFeaturesSetting7 = default(Descriptors.FacialFeaturesSetting);
			facialFeaturesSetting7.feature = Descriptors.FacialFeature.mole;
			facialFeaturesSetting7.id = 0;
			this.facialFeatures.Add(facialFeaturesSetting7);
		}
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x00061F40 File Offset: 0x00060140
	private void GenerateFootwearPreference()
	{
		List<Human.ShoeType> list = new List<Human.ShoeType>();
		foreach (Human.ShoeType shoeType in Toolbox.Instance.allShoeTypes)
		{
			if (shoeType != Human.ShoeType.barefoot)
			{
				int num = 5;
				int num2 = 5;
				int num3 = 5;
				int num4 = 5;
				int num5 = 5;
				int num6 = 5;
				int num7 = 5;
				if (shoeType == Human.ShoeType.heel)
				{
					num = 0;
					num4 = 7;
				}
				else if (shoeType == Human.ShoeType.boots)
				{
					num = 7;
					num6 = 8;
				}
				int num8 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num - this.citizen.genderScale * 10f));
				float num9 = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)num2 - this.citizen.humility * 10f)));
				int num10 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num3 - this.citizen.emotionality * 10f));
				int num11 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num4 - this.citizen.extraversion * 10f));
				int num12 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num5 - this.citizen.agreeableness * 10f));
				int num13 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num6 - this.citizen.conscientiousness * 10f));
				int num14 = 10 - Mathf.RoundToInt(Mathf.Abs((float)num7 - this.citizen.creativity * 10f));
				int num15 = Mathf.Max(Mathf.FloorToInt((num9 + (float)num10 + (float)num11 + (float)num12 + (float)num13 + (float)num14 + (float)num8) / 7f), 1);
				for (int i = 0; i < num15; i++)
				{
					list.Add(shoeType);
				}
			}
		}
		this.footwear = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.citizen.seed, out this.citizen.seed)];
		if (this.footwear == Human.ShoeType.normal)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.shoesNormal);
			return;
		}
		if (this.footwear == Human.ShoeType.heel)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.shoesHeels);
			return;
		}
		if (this.footwear == Human.ShoeType.boots)
		{
			this.citizen.AddCharacterTrait(CitizenControls.Instance.shoesBoots);
		}
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x000621A0 File Offset: 0x000603A0
	public static float DescriptorComparison(Descriptors comp1, Descriptors comp2)
	{
		return 1f;
	}

	// Token: 0x0400065D RID: 1629
	[NonSerialized]
	public Human citizen;

	// Token: 0x0400065E RID: 1630
	public float visualDistinctiveness = 0.5f;

	// Token: 0x0400065F RID: 1631
	public Descriptors.BuildType build = Descriptors.BuildType.average;

	// Token: 0x04000660 RID: 1632
	public Descriptors.Height height = Descriptors.Height.hAverage;

	// Token: 0x04000661 RID: 1633
	public float heightCM = 175f;

	// Token: 0x04000662 RID: 1634
	public float weightKG = 50f;

	// Token: 0x04000663 RID: 1635
	public int shoeSize = 10;

	// Token: 0x04000664 RID: 1636
	public Human.ShoeType footwear;

	// Token: 0x04000665 RID: 1637
	public List<Descriptors.EthnicitySetting> ethnicities = new List<Descriptors.EthnicitySetting>();

	// Token: 0x04000666 RID: 1638
	public Color skinColour = Color.white;

	// Token: 0x04000667 RID: 1639
	public Descriptors.HairColour hairColourCategory;

	// Token: 0x04000668 RID: 1640
	public Color hairColour = Color.black;

	// Token: 0x04000669 RID: 1641
	public Descriptors.HairStyle hairType = Descriptors.HairStyle.shortHair;

	// Token: 0x0400066A RID: 1642
	public Descriptors.EyeColour eyeColour;

	// Token: 0x0400066B RID: 1643
	public List<Descriptors.FacialFeaturesSetting> facialFeatures = new List<Descriptors.FacialFeaturesSetting>();

	// Token: 0x020000D8 RID: 216
	public enum Age
	{
		// Token: 0x0400066D RID: 1645
		youngAdult,
		// Token: 0x0400066E RID: 1646
		adult,
		// Token: 0x0400066F RID: 1647
		old
	}

	// Token: 0x020000D9 RID: 217
	public enum BuildType
	{
		// Token: 0x04000671 RID: 1649
		skinny,
		// Token: 0x04000672 RID: 1650
		average,
		// Token: 0x04000673 RID: 1651
		overweight,
		// Token: 0x04000674 RID: 1652
		muscular
	}

	// Token: 0x020000DA RID: 218
	public enum Height
	{
		// Token: 0x04000676 RID: 1654
		veryShort,
		// Token: 0x04000677 RID: 1655
		hShort,
		// Token: 0x04000678 RID: 1656
		hAverage,
		// Token: 0x04000679 RID: 1657
		tall,
		// Token: 0x0400067A RID: 1658
		veryTall
	}

	// Token: 0x020000DB RID: 219
	public enum EthnicGroup
	{
		// Token: 0x0400067C RID: 1660
		westEuropean,
		// Token: 0x0400067D RID: 1661
		eastEuropean,
		// Token: 0x0400067E RID: 1662
		scandinavian,
		// Token: 0x0400067F RID: 1663
		mediterranean,
		// Token: 0x04000680 RID: 1664
		hispanic,
		// Token: 0x04000681 RID: 1665
		african,
		// Token: 0x04000682 RID: 1666
		indian,
		// Token: 0x04000683 RID: 1667
		chinese,
		// Token: 0x04000684 RID: 1668
		japanese,
		// Token: 0x04000685 RID: 1669
		korean,
		// Token: 0x04000686 RID: 1670
		nativeAmerican,
		// Token: 0x04000687 RID: 1671
		middleEastern,
		// Token: 0x04000688 RID: 1672
		australian,
		// Token: 0x04000689 RID: 1673
		africanAmerican,
		// Token: 0x0400068A RID: 1674
		islander,
		// Token: 0x0400068B RID: 1675
		northAmerican,
		// Token: 0x0400068C RID: 1676
		southAmerican
	}

	// Token: 0x020000DC RID: 220
	[Serializable]
	public class EthnicitySetting : IComparable<Descriptors.EthnicitySetting>
	{
		// Token: 0x06000623 RID: 1571 RVA: 0x000621F7 File Offset: 0x000603F7
		public int CompareTo(Descriptors.EthnicitySetting otherObject)
		{
			return this.ratio.CompareTo(otherObject.ratio);
		}

		// Token: 0x0400068D RID: 1677
		public Descriptors.EthnicGroup group;

		// Token: 0x0400068E RID: 1678
		public float ratio;

		// Token: 0x0400068F RID: 1679
		public SocialStatistics.EthnicityStats stats;
	}

	// Token: 0x020000DD RID: 221
	public enum HairColour
	{
		// Token: 0x04000691 RID: 1681
		black,
		// Token: 0x04000692 RID: 1682
		brown,
		// Token: 0x04000693 RID: 1683
		blonde,
		// Token: 0x04000694 RID: 1684
		ginger,
		// Token: 0x04000695 RID: 1685
		red,
		// Token: 0x04000696 RID: 1686
		blue,
		// Token: 0x04000697 RID: 1687
		green,
		// Token: 0x04000698 RID: 1688
		purple,
		// Token: 0x04000699 RID: 1689
		pink,
		// Token: 0x0400069A RID: 1690
		grey,
		// Token: 0x0400069B RID: 1691
		white
	}

	// Token: 0x020000DE RID: 222
	public enum HairStyle
	{
		// Token: 0x0400069D RID: 1693
		bald,
		// Token: 0x0400069E RID: 1694
		shortHair,
		// Token: 0x0400069F RID: 1695
		longHair
	}

	// Token: 0x020000DF RID: 223
	public enum EyeColour
	{
		// Token: 0x040006A1 RID: 1697
		blueEyes,
		// Token: 0x040006A2 RID: 1698
		brownEyes,
		// Token: 0x040006A3 RID: 1699
		greenEyes,
		// Token: 0x040006A4 RID: 1700
		greyEyes
	}

	// Token: 0x020000E0 RID: 224
	[Serializable]
	public struct FacialFeaturesSetting
	{
		// Token: 0x040006A5 RID: 1701
		public Descriptors.FacialFeature feature;

		// Token: 0x040006A6 RID: 1702
		public int id;
	}

	// Token: 0x020000E1 RID: 225
	public enum FacialFeature
	{
		// Token: 0x040006A8 RID: 1704
		scaring,
		// Token: 0x040006A9 RID: 1705
		beard,
		// Token: 0x040006AA RID: 1706
		moustache,
		// Token: 0x040006AB RID: 1707
		piercing,
		// Token: 0x040006AC RID: 1708
		tattoo,
		// Token: 0x040006AD RID: 1709
		glasses,
		// Token: 0x040006AE RID: 1710
		mole
	}
}
