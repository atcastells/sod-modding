using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class DistrictController : Controller, IComparable<DistrictController>
{
	// Token: 0x060007B1 RID: 1969 RVA: 0x00074AD4 File Offset: 0x00072CD4
	public void Setup(DistrictPreset newPreset)
	{
		this.districtID = DistrictController.assignID;
		DistrictController.assignID++;
		this.seed = this.districtID.ToString() + newPreset.name;
		base.name = "District " + this.districtID.ToString();
		base.transform.name = base.name;
		this.preset = newPreset;
		HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Add(this);
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00074B58 File Offset: 0x00072D58
	public void Load(CitySaveData.DistrictCitySave data)
	{
		this.districtID = data.districtID;
		DistrictController.assignID = Mathf.Max(DistrictController.assignID, this.districtID + 1);
		base.name = data.name;
		base.transform.name = base.name;
		this.averageLandValue = data.averageLandValue;
		this.dominantEthnicities = data.dominantEthnicities;
		Toolbox.Instance.LoadDataFromResources<DistrictPreset>(data.preset, out this.preset);
		HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Add(this);
		foreach (CitySaveData.BlockCitySave data2 in data.blocks)
		{
			Object.Instantiate<GameObject>(PrefabControls.Instance.block, PrefabControls.Instance.cityContainer.transform).GetComponent<BlockController>().Load(data2, this);
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00074C50 File Offset: 0x00072E50
	public void AddCityTile(CityTile newCityTile)
	{
		if (!this.cityTiles.Contains(newCityTile))
		{
			if (newCityTile.district != null)
			{
				newCityTile.district.cityTiles.Remove(newCityTile);
			}
			newCityTile.SetDensity((BuildingPreset.Density)Mathf.Lerp((float)this.preset.minimumDensity, (float)this.preset.maximumDensity, Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed)));
			newCityTile.SetLandVlaue((BuildingPreset.LandValue)Mathf.Lerp((float)this.preset.minimumLandValue, (float)this.preset.maximumLandValue, Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed)));
			newCityTile.district = this;
			this.cityTiles.Add(newCityTile);
			newCityTile.transform.SetParent(base.transform, true);
		}
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00074D3B File Offset: 0x00072F3B
	public void AddBlock(BlockController newBlock)
	{
		if (!this.blocks.Contains(newBlock))
		{
			newBlock.transform.SetParent(base.transform, true);
			this.blocks.Add(newBlock);
		}
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00074D6C File Offset: 0x00072F6C
	public void PopulateData()
	{
		foreach (CityTile cityTile in this.cityTiles)
		{
			this.averageLandValue += (float)cityTile.landValue;
		}
		this.averageLandValue /= (float)this.cityTiles.Count;
		if (this.averageLandValue < 0f)
		{
			this.averageLandValue = 0f;
		}
		int num = 0;
		this.dominantEthnicities.Clear();
		for (int i = 0; i < SocialStatistics.Instance.ethnicityFrequencies.Count; i++)
		{
			SocialStatistics.EthnicityFrequency freq = SocialStatistics.Instance.ethnicityFrequencies[i];
			SocialStatistics.EthnicityFrequency ethnicityFrequency = new SocialStatistics.EthnicityFrequency();
			ethnicityFrequency.ethnicity = freq.ethnicity;
			ethnicityFrequency.frequency = freq.frequency;
			if (this.preset.affectEthnicity)
			{
				SocialStatistics.EthnicityFrequency ethnicityFrequency2 = this.preset.ethnicityFrequencyModifiers.Find((SocialStatistics.EthnicityFrequency item) => item.ethnicity == freq.ethnicity);
				if (ethnicityFrequency2 != null)
				{
					ethnicityFrequency.frequency += ethnicityFrequency2.frequency;
				}
			}
			num += ethnicityFrequency.frequency;
			this.dominantEthnicities.Add(ethnicityFrequency);
		}
		foreach (SocialStatistics.EthnicityFrequency ethnicityFrequency3 in this.dominantEthnicities)
		{
			ethnicityFrequency3.frequency = Mathf.RoundToInt((float)ethnicityFrequency3.frequency / (float)num);
		}
		this.UpdateName();
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00074F28 File Offset: 0x00073128
	public void UpdateName()
	{
		int num = 99;
		bool flag = true;
		string text = string.Concat(new string[]
		{
			CityData.Instance.seed,
			this.averageLandValue.ToString(),
			this.blocks.Count.ToString(),
			this.cityTiles.Count.ToString(),
			this.preset.name,
			this.districtID.ToString()
		});
		while (flag || (HighlanderSingleton<CityDistricts>.Instance.districtDirectory.Exists((DistrictController item) => item != this && item.name == base.name) && num > 0))
		{
			Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, text, out text);
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, text, out text) < this.preset.prefixOrSuffixChance)
			{
				if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, text, out text) < 0.5f)
				{
					base.name = NameGenerator.Instance.GenerateName(this.preset.prefixList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.prefixList.Count, text, out text)], 1f, this.preset.mainNamingList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.mainNamingList.Count, text, out text)], 1f, "", 0f, text);
				}
				else
				{
					base.name = NameGenerator.Instance.GenerateName("", 0f, this.preset.mainNamingList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.mainNamingList.Count, text, out text)], 1f, this.preset.suffixList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.suffixList.Count, text, out text)], 1f, text);
				}
			}
			else
			{
				base.name = NameGenerator.Instance.GenerateName("", 0f, this.preset.mainNamingList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.mainNamingList.Count, text, out text)], 1f, "", 0f, text);
			}
			flag = false;
			num--;
		}
		base.transform.name = base.name;
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x000751B0 File Offset: 0x000733B0
	public Descriptors.EthnicGroup EthnictiyBasedOnDominance()
	{
		if (this.dominantEthnicities.Count > 0)
		{
			float psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed);
			float num = 0f;
			for (int i = 0; i < this.dominantEthnicities.Count; i++)
			{
				SocialStatistics.EthnicityFrequency ethnicityFrequency = this.dominantEthnicities[i];
				if (psuedoRandomNumberContained >= num && psuedoRandomNumberContained < num + (float)ethnicityFrequency.frequency)
				{
					return ethnicityFrequency.ethnicity;
				}
			}
		}
		return Toolbox.Instance.RandomEthnicGroup(this.seed);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0007523C File Offset: 0x0007343C
	public int CompareTo(DistrictController otherObject)
	{
		return this.cityTiles.Count.CompareTo(otherObject.cityTiles.Count);
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00075268 File Offset: 0x00073468
	public CitySaveData.DistrictCitySave GenerateSaveData()
	{
		CitySaveData.DistrictCitySave districtCitySave = new CitySaveData.DistrictCitySave();
		districtCitySave.name = base.name;
		districtCitySave.districtID = this.districtID;
		districtCitySave.averageLandValue = this.averageLandValue;
		districtCitySave.dominantEthnicities = this.dominantEthnicities;
		districtCitySave.preset = this.preset.name;
		foreach (BlockController blockController in this.blocks)
		{
			districtCitySave.blocks.Add(blockController.GenerateSaveData());
		}
		return districtCitySave;
	}

	// Token: 0x040007E8 RID: 2024
	[Header("ID")]
	public int districtID;

	// Token: 0x040007E9 RID: 2025
	public static int assignID = 1;

	// Token: 0x040007EA RID: 2026
	public string seed;

	// Token: 0x040007EB RID: 2027
	[Header("Location")]
	public List<BlockController> blocks = new List<BlockController>();

	// Token: 0x040007EC RID: 2028
	public List<CityTile> cityTiles = new List<CityTile>();

	// Token: 0x040007ED RID: 2029
	[Header("Details")]
	public DistrictPreset preset;

	// Token: 0x040007EE RID: 2030
	public float averageLandValue;

	// Token: 0x040007EF RID: 2031
	public List<SocialStatistics.EthnicityFrequency> dominantEthnicities = new List<SocialStatistics.EthnicityFrequency>();
}
