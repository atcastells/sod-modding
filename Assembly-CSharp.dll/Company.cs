using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class Company
{
	// Token: 0x06000746 RID: 1862 RVA: 0x0006F0E4 File Offset: 0x0006D2E4
	public void Setup(CompanyPreset newPreset, NewAddress newAddress)
	{
		this.preset = newPreset;
		this.companyID = Company.assignCompanyID;
		Company.assignCompanyID++;
		if (newAddress != null)
		{
			this.seed = (this.companyID * newAddress.id).ToString();
		}
		else
		{
			this.seed = (this.companyID * this.companyID).ToString();
		}
		this.SetAddress(newAddress);
		this.SetPlaceOfBusiness(newAddress);
		this.topSalary = SocialControls.Instance.wageRanges[(int)this.preset.topSalary];
		this.minimumSalary = SocialControls.Instance.wageRanges[(int)this.preset.minimumSalary];
		if (this.preset.possibleUniformColours.Count > 0)
		{
			string text;
			this.uniformColour = this.preset.possibleUniformColours[Toolbox.Instance.RandContained(0, this.preset.possibleUniformColours.Count, this.seed, out text)];
		}
		this.retailOpenHours = this.preset.workHours.retailOpenHours;
		this.publicFacing = this.preset.publicFacing;
		this.monday = this.preset.workHours.monday;
		this.tuesday = this.preset.workHours.tuesday;
		this.wednesday = this.preset.workHours.wednesday;
		this.thursday = this.preset.workHours.thursday;
		this.friday = this.preset.workHours.friday;
		this.saturday = this.preset.workHours.saturday;
		this.sunday = this.preset.workHours.sunday;
		foreach (CompanyOpenHoursPreset.CompanyShift companyShift in this.preset.workHours.shifts)
		{
			CompanyOpenHoursPreset.CompanyShift companyShift2 = new CompanyOpenHoursPreset.CompanyShift();
			companyShift2.decimalHours = companyShift.decimalHours;
			companyShift2.friday = companyShift.friday;
			companyShift2.monday = companyShift.monday;
			companyShift2.shiftType = companyShift.shiftType;
			companyShift2.saturday = companyShift.saturday;
			companyShift2.sunday = companyShift.sunday;
			companyShift2.thursday = companyShift.thursday;
			companyShift2.tuesday = companyShift.tuesday;
			companyShift2.wednesday = companyShift.wednesday;
			this.shifts.Add(companyShift2);
		}
		CityData.Instance.companyDirectory.Add(this);
		int num = 0;
		CompanyStructurePreset.OccupationSettings companyStructure = this.preset.structure.companyStructure;
		List<CompanyStructurePreset.OccupationSettings> list = new List<CompanyStructurePreset.OccupationSettings>();
		list.Add(companyStructure);
		Dictionary<CompanyStructurePreset.OccupationSettings, int> dictionary = new Dictionary<CompanyStructurePreset.OccupationSettings, int>();
		dictionary.Add(companyStructure, 0);
		Dictionary<CompanyStructurePreset.OccupationSettings, List<Occupation>> dictionary2 = new Dictionary<CompanyStructurePreset.OccupationSettings, List<Occupation>>();
		dictionary2.Add(companyStructure, null);
		int num2 = 9999;
		bool flag = true;
		while (list.Count > 0 && num2 > 0)
		{
			CompanyStructurePreset.OccupationSettings jobSettings = list[0];
			int num3 = dictionary[jobSettings];
			this.numberOfRankLevels = Mathf.Max(this.numberOfRankLevels, num3);
			int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(jobSettings.positionsMinimum, jobSettings.positionsMaximum, this.seed, out this.seed);
			bool flag2 = false;
			List<Occupation> list2 = new List<Occupation>();
			Predicate<CompanyOpenHoursPreset.CompanyShift> <>9__0;
			for (int i = 0; i < psuedoRandomNumberContained; i++)
			{
				Occupation occupation = new Occupation();
				list2.Add(occupation);
				this.numberOfJobPositions++;
				if (flag)
				{
					occupation.isOwner = true;
					flag = false;
				}
				occupation.preset = jobSettings.occupation;
				occupation.paygrade = jobSettings.payGrade;
				if (dictionary2.ContainsKey(jobSettings) && dictionary2[jobSettings] != null && dictionary2[jobSettings].Count > 0)
				{
					occupation.boss = dictionary2[jobSettings][Toolbox.Instance.GetPsuedoRandomNumberContained(0, dictionary2[jobSettings].Count, this.seed, out this.seed)];
				}
				if (!flag2)
				{
					flag2 = true;
					occupation.teamLeader = true;
				}
				occupation.teamID = num;
				CompanyOpenHoursPreset.CompanyShift companyShift3 = null;
				if (jobSettings.occupation.shiftTimeIsImportant)
				{
					List<CompanyOpenHoursPreset.CompanyShift> list3 = this.shifts;
					Predicate<CompanyOpenHoursPreset.CompanyShift> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((CompanyOpenHoursPreset.CompanyShift item) => item.shiftType == jobSettings.occupation.shiftType));
					}
					companyShift3 = list3.Find(predicate);
				}
				if (companyShift3 == null)
				{
					for (int j = 0; j < this.shifts.Count; j++)
					{
						CompanyOpenHoursPreset.CompanyShift companyShift4 = this.shifts[j];
						int openHoursCoverageCountForShift = this.GetOpenHoursCoverageCountForShift(companyShift4);
						int num4 = 0;
						if (companyShift3 != null)
						{
							num4 = this.GetOpenHoursCoverageCountForShift(companyShift3);
						}
						if (companyShift3 == null || openHoursCoverageCountForShift < num4)
						{
							companyShift3 = companyShift4;
						}
						else if (companyShift3 != null && openHoursCoverageCountForShift == num4 && Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > 0.5f)
						{
							companyShift3 = companyShift4;
						}
					}
				}
				occupation.shift = companyShift3;
				companyShift3.assigned.Add(occupation);
				if (Game.Instance.collectDebugData && Game.Instance.devMode)
				{
					companyShift3.debugAssigned++;
				}
				occupation.employer = this;
				this.companyRoster.Add(occupation);
				CityData.Instance.jobsDirectory.Add(occupation);
			}
			num++;
			List<CompanyStructurePreset.OccupationSettings> list4 = new List<CompanyStructurePreset.OccupationSettings>();
			CompanyStructurePreset.BossConfig bossConfig = jobSettings as CompanyStructurePreset.BossConfig;
			if (bossConfig != null)
			{
				using (List<CompanyStructurePreset.Hierarchy1Config>.Enumerator enumerator2 = bossConfig.subordinates.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CompanyStructurePreset.Hierarchy1Config hierarchy1Config = enumerator2.Current;
						list4.Add(hierarchy1Config);
					}
					goto IL_6E0;
				}
				goto IL_5F1;
			}
			goto IL_5F1;
			IL_6E0:
			foreach (CompanyStructurePreset.OccupationSettings occupationSettings in list4)
			{
				list.Add(occupationSettings);
				dictionary.Add(occupationSettings, num3 + 1);
				dictionary2.Add(occupationSettings, list2);
			}
			list.RemoveAt(0);
			num2--;
			continue;
			IL_5F1:
			CompanyStructurePreset.Hierarchy1Config hierarchy1Config2 = jobSettings as CompanyStructurePreset.Hierarchy1Config;
			if (hierarchy1Config2 != null)
			{
				using (List<CompanyStructurePreset.Hierarchy2Config>.Enumerator enumerator4 = hierarchy1Config2.subordinates.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						CompanyStructurePreset.Hierarchy2Config hierarchy2Config = enumerator4.Current;
						list4.Add(hierarchy2Config);
					}
					goto IL_6E0;
				}
			}
			CompanyStructurePreset.Hierarchy2Config hierarchy2Config2 = jobSettings as CompanyStructurePreset.Hierarchy2Config;
			if (hierarchy2Config2 != null)
			{
				using (List<CompanyStructurePreset.Hierarchy3Config>.Enumerator enumerator5 = hierarchy2Config2.subordinates.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						CompanyStructurePreset.Hierarchy3Config hierarchy3Config = enumerator5.Current;
						list4.Add(hierarchy3Config);
					}
					goto IL_6E0;
				}
			}
			CompanyStructurePreset.Hierarchy3Config hierarchy3Config2 = jobSettings as CompanyStructurePreset.Hierarchy3Config;
			if (hierarchy3Config2 != null)
			{
				foreach (CompanyStructurePreset.OccupationSettings occupationSettings2 in hierarchy3Config2.subordinates)
				{
					CompanyStructurePreset.Hierarchy4Config hierarchy4Config = (CompanyStructurePreset.Hierarchy4Config)occupationSettings2;
					list4.Add(hierarchy4Config);
				}
				goto IL_6E0;
			}
			goto IL_6E0;
		}
		if (this.monday)
		{
			this.daysOpen.Add(SessionData.WeekDay.monday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.monday);
		}
		if (this.tuesday)
		{
			this.daysOpen.Add(SessionData.WeekDay.tuesday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.tuesday);
		}
		if (this.wednesday)
		{
			this.daysOpen.Add(SessionData.WeekDay.wednesday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.wednesday);
		}
		if (this.thursday)
		{
			this.daysOpen.Add(SessionData.WeekDay.thursday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.thursday);
		}
		if (this.friday)
		{
			this.daysOpen.Add(SessionData.WeekDay.friday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.friday);
		}
		if (this.saturday)
		{
			this.daysOpen.Add(SessionData.WeekDay.saturday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.saturday);
		}
		if (this.sunday)
		{
			this.daysOpen.Add(SessionData.WeekDay.sunday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.sunday);
		}
		foreach (Occupation occupation2 in this.companyRoster)
		{
			occupation2.Setup();
		}
		this.companyRoster.Sort();
		if (!SessionData.Instance.isFloorEdit)
		{
			foreach (MenuPreset menuPreset in this.preset.menus)
			{
				foreach (InteractablePreset interactablePreset in menuPreset.itemsSold)
				{
					float num5 = (interactablePreset.value.y - interactablePreset.value.x) / 4f * Toolbox.Instance.GetPsuedoRandomNumberContained(-0.5f, 0.5f, this.seed, out this.seed);
					int num6 = Mathf.RoundToInt(interactablePreset.value.x + num5);
					if (this.placeOfBusiness != null && this.placeOfBusiness.thisAsAddress != null)
					{
						num6 = Mathf.RoundToInt(Mathf.Lerp(interactablePreset.value.x, interactablePreset.value.y, (float)this.address.building.cityTile.landValue / 4f) + num5);
					}
					if (!this.prices.ContainsKey(interactablePreset))
					{
						this.prices.Add(interactablePreset, num6);
					}
					else
					{
						this.prices[interactablePreset] = num6;
					}
				}
			}
		}
		this.CreateEvidence();
		this.CreateItemSingletons();
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0006FBBC File Offset: 0x0006DDBC
	public int GetOpenHoursCoverageCountForShift(CompanyOpenHoursPreset.CompanyShift sft)
	{
		int count = sft.assigned.Count;
		List<Occupation> list = sft.assigned.FindAll((Occupation item) => !item.preset.countsTowardsOpenHoursCoverage);
		return count - list.Count;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0006FC08 File Offset: 0x0006DE08
	public void SetAddress(NewAddress newAdd)
	{
		this.address = newAdd;
		if (this.address != null)
		{
			this.address.company = this;
			if (this.director != null)
			{
				this.address.AddOwner(this.director);
			}
			foreach (Occupation occupation in this.companyRoster)
			{
				if (occupation.employee != null)
				{
					occupation.employee.AddToKeyring(this.address, true);
					this.address.AddInhabitant(occupation.employee);
					if (!occupation.employee.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.workGoal))
					{
						occupation.employee.ai.CreateNewGoal(RoutineControls.Instance.workGoal, 0f, 0f, null, null, null, null, null, -2);
					}
				}
			}
			this.CreateEvidence();
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0006FD3C File Offset: 0x0006DF3C
	public void SetPlaceOfBusiness(NewGameLocation newLoc)
	{
		this.placeOfBusiness = newLoc;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0006FD48 File Offset: 0x0006DF48
	public void Load(CitySaveData.CompanyCitySave data, NewAddress newAddress)
	{
		this.companyID = data.id;
		Company.assignCompanyID = Mathf.Max(Company.assignCompanyID, this.companyID + 1);
		Toolbox.Instance.LoadDataFromResources<CompanyPreset>(data.preset, out this.preset);
		this.SetAddress(newAddress);
		if (this.placeOfBusiness == null)
		{
			this.SetPlaceOfBusiness(this.address);
		}
		this.CreateEvidence();
		this.topSalary = data.topSalary;
		this.minimumSalary = data.minimumSalary;
		this.publicFacing = data.publicFacing;
		this.shortName = data.shortName;
		this.nameAltTags = data.nameAltTags;
		this.name = newAddress.name;
		this.monday = data.monday;
		this.tuesday = data.tuesday;
		this.wednesday = data.wednesday;
		this.thursday = data.thursday;
		this.friday = data.friday;
		this.saturday = data.saturday;
		this.sunday = data.sunday;
		this.retailOpenHours = data.retailOpenHours;
		if (this.preset.possibleUniformColours.Count > 0)
		{
			string text;
			this.uniformColour = this.preset.possibleUniformColours[Toolbox.Instance.RandContained(0, this.preset.possibleUniformColours.Count, (this.companyID * this.address.id).ToString(), out text)];
		}
		foreach (CompanyOpenHoursPreset.CompanyShift companyShift in this.preset.workHours.shifts)
		{
			CompanyOpenHoursPreset.CompanyShift companyShift2 = new CompanyOpenHoursPreset.CompanyShift();
			companyShift2.decimalHours = companyShift.decimalHours;
			companyShift2.friday = companyShift.friday;
			companyShift2.monday = companyShift.monday;
			companyShift2.shiftType = companyShift.shiftType;
			companyShift2.saturday = companyShift.saturday;
			companyShift2.sunday = companyShift.sunday;
			companyShift2.thursday = companyShift.thursday;
			companyShift2.tuesday = companyShift.tuesday;
			companyShift2.wednesday = companyShift.wednesday;
			this.shifts.Add(companyShift2);
		}
		if (this.monday)
		{
			this.daysOpen.Add(SessionData.WeekDay.monday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.monday);
		}
		if (this.tuesday)
		{
			this.daysOpen.Add(SessionData.WeekDay.tuesday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.tuesday);
		}
		if (this.wednesday)
		{
			this.daysOpen.Add(SessionData.WeekDay.wednesday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.wednesday);
		}
		if (this.thursday)
		{
			this.daysOpen.Add(SessionData.WeekDay.thursday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.thursday);
		}
		if (this.friday)
		{
			this.daysOpen.Add(SessionData.WeekDay.friday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.friday);
		}
		if (this.saturday)
		{
			this.daysOpen.Add(SessionData.WeekDay.saturday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.saturday);
		}
		if (this.sunday)
		{
			this.daysOpen.Add(SessionData.WeekDay.sunday);
		}
		else
		{
			this.daysClosed.Add(SessionData.WeekDay.sunday);
		}
		this.passedWorkLocationID = data.passedWorkLocation;
		foreach (CitySaveData.OccupationCitySave data2 in data.companyRoster)
		{
			Occupation occupation = new Occupation();
			occupation.Load(data2, this);
			this.companyRoster.Add(occupation);
			CityData.Instance.jobsDirectory.Add(occupation);
		}
		this.companyRoster.Sort();
		for (int i = 0; i < data.menuItems.Count; i++)
		{
			InteractablePreset interactablePreset = null;
			Toolbox.Instance.LoadDataFromResources<InteractablePreset>(data.menuItems[i], out interactablePreset);
			if (interactablePreset != null)
			{
				this.prices.Add(interactablePreset, data.itemCosts[i]);
			}
		}
		CityData.Instance.companyDirectory.Add(this);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00070174 File Offset: 0x0006E374
	public void GenerateFakeSalesRecords()
	{
		if (this.preset.recordSalesData && this.preset.previousFakeSalesRecords > 0)
		{
			List<Citizen> list = new List<Citizen>();
			if (this.preset.requiredTraits.Count > 0)
			{
				using (List<Citizen>.Enumerator enumerator = CityData.Instance.citizenDirectory.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Citizen citizen = enumerator.Current;
						foreach (Human.Trait trait in citizen.characterTraits)
						{
							if (trait.trait != null && this.preset.requiredTraits.Contains(trait.trait))
							{
								list.Add(citizen);
								break;
							}
						}
					}
					goto IL_E2;
				}
			}
			list.AddRange(CityData.Instance.citizenDirectory);
			IL_E2:
			List<InteractablePreset> list2 = new List<InteractablePreset>(this.prices.Keys);
			Game.Log(string.Concat(new string[]
			{
				"CityGen: Generating ",
				this.preset.previousFakeSalesRecords.ToString(),
				" fake sales records for ",
				this.name,
				" with pool of ",
				list.Count.ToString(),
				" potential customers and ",
				list2.Count.ToString(),
				" items..."
			}), 2);
			for (int i = 0; i < this.preset.previousFakeSalesRecords; i++)
			{
				if (list.Count > 0)
				{
					float time = SessionData.Instance.gameTime - (float)Toolbox.Instance.Rand(3, 48, false);
					int num = Toolbox.Instance.Rand(1, 3, false);
					List<InteractablePreset> list3 = new List<InteractablePreset>();
					for (int j = 0; j < num; j++)
					{
						InteractablePreset interactablePreset = list2[Toolbox.Instance.Rand(0, list2.Count, false)];
						if (!list3.Contains(interactablePreset))
						{
							list3.Add(interactablePreset);
						}
					}
					Citizen citizen2 = list[Toolbox.Instance.Rand(0, list.Count, false)];
					this.AddSalesRecord(citizen2, list3, time);
					list.Remove(citizen2);
				}
			}
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000703D8 File Offset: 0x0006E5D8
	public void UpdateName()
	{
		string text = "names.";
		float prefixChance = 0f;
		string text2 = "names.";
		float mainChance = 1f;
		string suffixList = "names." + this.preset.suffixList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.suffixList.Count, this.seed, out this.seed)];
		float suffixChance = 1f;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		string empty4 = string.Empty;
		if (this.preset.useBuildingOverrideName && this.address != null && this.address.building != null && this.address.building.nameOverride != null)
		{
			this.name = this.address.building.nameOverride.name;
			empty2 = this.name;
			if (this.preset.overrideSuffixList.Count > 0)
			{
				suffixList = "names." + this.preset.overrideSuffixList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.overrideSuffixList.Count, this.seed, out this.seed)];
				string text3;
				string text4;
				NameGenerator.Instance.GenerateName(string.Empty, 0f, string.Empty, 0f, suffixList, 1f, false, this.preset.aliterationWeight, this.preset.aliterationWeight, out text3, out text4, out empty3, out flag2, out empty4, this.seed).Trim();
				this.name = this.name + " " + empty3;
				flag2 = true;
			}
		}
		else if (this.preset.useBuildingName && this.address != null && this.address.building != null)
		{
			this.name = this.address.building.name;
		}
		else
		{
			float upperRange = this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance + this.preset.useStreetNameChance + this.preset.useOwnerFirstNameChance + this.preset.useOwnerSurNameChance;
			float psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, upperRange, this.seed, out this.seed);
			bool mainIsCitizenName;
			if (psuedoRandomNumberContained < this.preset.useCompanyNameListChance)
			{
				if (this.preset.prefixList.Count > 0)
				{
					text += this.preset.prefixList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.prefixList.Count, this.seed, out this.seed)];
					prefixChance = this.preset.prefixChance;
				}
				else
				{
					prefixChance = 0f;
				}
				if (this.preset.mainNamingList.Count > 0)
				{
					text2 += this.preset.mainNamingList[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.mainNamingList.Count, this.seed, out this.seed)];
					mainChance = this.preset.mainChance;
				}
				mainIsCitizenName = false;
				if (text2 == "this.")
				{
					Game.Log("Error: Null string in company naming", 2);
				}
			}
			else if (psuedoRandomNumberContained >= this.preset.useCompanyNameListChance && psuedoRandomNumberContained < this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance)
			{
				text2 = "this." + this.address.building.name;
				mainIsCitizenName = false;
				flag = false;
				if (text2 == "this.")
				{
					Game.Log("Error: Null string in company naming", 2);
				}
			}
			else if (psuedoRandomNumberContained >= this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance && psuedoRandomNumberContained < this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance + this.preset.useStreetNameChance)
			{
				try
				{
					text2 = "this." + this.address.building.mainEntrance.node.gameLocation.name;
				}
				catch
				{
					try
					{
						text2 = "this." + this.address.building.additionalEntrances[0].node.gameLocation.name;
					}
					catch
					{
						text2 = "this." + this.address.building.name;
						if (text2 == "this.")
						{
							Game.Log("Error: Null string in company naming", 2);
						}
					}
				}
				NewNode.NodeAccess nodeAccess = this.address.entrances.Find((NewNode.NodeAccess item) => item.toNode.gameLocation.thisAsStreet != null);
				if (nodeAccess != null)
				{
					flag = false;
					text2 = "this." + nodeAccess.toNode.gameLocation.name;
				}
				mainIsCitizenName = false;
				if (text2 == "this.")
				{
					Game.Log("Error: Null string in company naming", 2);
				}
			}
			else if (psuedoRandomNumberContained >= this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance + this.preset.useStreetNameChance && psuedoRandomNumberContained < this.preset.useCompanyNameListChance + this.preset.useDistrictNameChance + this.preset.useStreetNameChance + this.preset.useOwnerFirstNameChance)
			{
				if (this.director != null)
				{
					text2 = "this." + this.director.GetFirstName();
				}
				else
				{
					Descriptors.EthnicGroup chosenGroup = Descriptors.EthnicGroup.northAmerican;
					string text5 = "male";
					if (this.address != null)
					{
						if (this.address.addressPreset.ethnicityMatters)
						{
							chosenGroup = this.address.addressPreset.ethnicity;
						}
						else
						{
							chosenGroup = this.address.building.cityTile.district.EthnictiyBasedOnDominance();
							if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, this.seed, out this.seed) > 0.5f)
							{
								text5 = "female";
							}
						}
					}
					SocialStatistics.EthnicityStats ethnicityStats = SocialStatistics.Instance.ethnicityStats.Find((SocialStatistics.EthnicityStats item) => item.group == chosenGroup);
					string text6 = chosenGroup.ToString();
					if (ethnicityStats.overrideFirst)
					{
						text6 = ethnicityStats.overrideNameFirst.ToString();
					}
					string text4;
					text2 = "this." + NameGenerator.Instance.GenerateName(null, 0f, "names." + text6 + ".first." + text5, 1f, null, 0f, out empty, out empty2, out empty3, out flag3, out text4, this.seed);
				}
				mainIsCitizenName = true;
				if (text2 == "this.")
				{
					Game.Log("Error: Null string in company naming", 2);
				}
			}
			else
			{
				if (this.director != null)
				{
					text2 = "this." + this.director.GetSurName();
				}
				else
				{
					Descriptors.EthnicGroup chosenGroup = Descriptors.EthnicGroup.northAmerican;
					if (this.address != null)
					{
						if (this.address.addressPreset.ethnicityMatters)
						{
							chosenGroup = this.address.addressPreset.ethnicity;
						}
						else
						{
							chosenGroup = this.address.building.cityTile.district.EthnictiyBasedOnDominance();
						}
					}
					SocialStatistics.EthnicityStats ethnicityStats2 = SocialStatistics.Instance.ethnicityStats.Find((SocialStatistics.EthnicityStats item) => item.group == chosenGroup);
					string text7 = chosenGroup.ToString();
					if (ethnicityStats2.overrideSur)
					{
						text7 = ethnicityStats2.overrideNameSur.ToString();
					}
					string text4;
					text2 = "this." + NameGenerator.Instance.GenerateName(null, 0f, "names." + text7 + ".sur", 1f, null, 0f, out empty, out empty2, out empty3, out flag3, out text4, this.seed);
				}
				mainIsCitizenName = true;
				flag = false;
				if (text2 == "this.")
				{
					Game.Log("Error: Null string in company naming", 2);
				}
			}
			this.name = NameGenerator.Instance.GenerateName(text, prefixChance, text2, mainChance, suffixList, suffixChance, mainIsCitizenName, this.preset.aliterationWeight, this.preset.aliterationWeight, out empty, out empty2, out empty3, out flag2, out empty4, this.seed).Trim();
		}
		if (flag2)
		{
			this.shortName = this.name;
		}
		else if (flag)
		{
			this.shortName = empty + " " + empty2;
		}
		else
		{
			this.shortName = empty3;
		}
		if (empty4.Length > 0)
		{
			this.nameAltTags = Enumerable.ToList<string>(empty4.Split(';', 0));
		}
		if (this.address != null && !this.preset.isSelfEmployed)
		{
			this.address.name = this.name;
			this.address.transform.name = this.name;
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00070D2C File Offset: 0x0006EF2C
	public bool IsOpenAtThisTime(float atTime)
	{
		float decimalHour = 0f;
		int day = 0;
		int num;
		int num2;
		int num3;
		SessionData.Instance.ParseTimeData(atTime, out decimalHour, out day, out num, out num2, out num3);
		return this.IsOpenAtThisTime(atTime, decimalHour, (SessionData.WeekDay)day);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00070D5F File Offset: 0x0006EF5F
	public bool IsOpenAtThisTime(float atTime, float decimalHour, SessionData.WeekDay day)
	{
		return SessionData.Instance.GetNextOrPreviousGameTimeForThisHour(atTime, decimalHour, day, this.daysOpen, this.retailOpenHours.x, this.retailOpenHours.y) <= SessionData.Instance.gameTime;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00070D9C File Offset: 0x0006EF9C
	public bool IsOpenAtDecimalTime(SessionData.WeekDay day, float hour)
	{
		float num = this.retailOpenHours.x;
		float y = this.retailOpenHours.y;
		if (this.retailOpenHours.x >= this.retailOpenHours.y && hour < this.retailOpenHours.x)
		{
			num = this.retailOpenHours.x - 24f;
		}
		if (hour >= num && hour <= y)
		{
			if (day == SessionData.WeekDay.monday && this.monday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.tuesday && this.tuesday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.wednesday && this.wednesday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.thursday && this.thursday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.friday && this.friday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.saturday && this.saturday)
			{
				return true;
			}
			if (day == SessionData.WeekDay.sunday && this.sunday)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00070E68 File Offset: 0x0006F068
	public void CreateEvidence()
	{
		if (!this.createdEvidence && this.address != null)
		{
			this.employeeRoster = (EvidenceCreator.Instance.CreateEvidence("CompanyRoster", "CompanyRoster" + this.companyID.ToString(), this.address, this.director, null, null, this.address.evidenceEntry, false, null) as EvidenceMultiPage);
			if (this.preset.recordSalesData)
			{
				this.salesRecords = EvidenceCreator.Instance.CreateEvidence("SalesRecords", "SalesRecords" + this.companyID.ToString(), this.address, this.director, null, null, this.address.evidenceEntry, false, null);
			}
			if (this.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.caffeine) || this.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.meal) || this.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.snack))
			{
				this.menu = EvidenceCreator.Instance.CreateEvidence("Menu", "Menu" + this.companyID.ToString(), this.address, this.director, null, null, this.address.evidenceEntry, false, null);
			}
			this.createdEvidence = true;
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00070FB4 File Offset: 0x0006F1B4
	public void CreateItemSingletons()
	{
		foreach (KeyValuePair<InteractablePreset, int> keyValuePair in this.prices)
		{
			if (!(keyValuePair.Key.retailItem == null) && !(keyValuePair.Key.spawnEvidence != GameplayControls.Instance.retailItemSoldDiscovery))
			{
				List<object> list = new List<object>();
				list.Add(keyValuePair.Key);
				list.Add(this);
				list.Add(keyValuePair.Key);
				Evidence evidence = EvidenceCreator.Instance.CreateEvidence("RetailItemSingleton", keyValuePair.Key.name + this.companyID.ToString(), null, null, null, null, null, false, list);
				this.itemSingletons.Add(keyValuePair.Key.retailItem, evidence);
			}
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x000710AC File Offset: 0x0006F2AC
	public void SetupEvidence()
	{
		if (this.address.addressPreset.evidenceIconLarge != null)
		{
			this.address.evidenceEntry.SetNewIcon(this.address.addressPreset.evidenceIconLarge);
			return;
		}
		this.address.evidenceEntry.SetNewIcon(InterfaceControls.Instance.companyIconLarge);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0007110C File Offset: 0x0006F30C
	public void OpenCloseCheck()
	{
		if (this.IsOpenAtThisTime(SessionData.Instance.gameTime, SessionData.Instance.decimalClock, SessionData.Instance.day))
		{
			if (!this.openForBusinessDesired || this.openForBusinessDesired != this.openForBusinessActual)
			{
				this.SetOpen(true, false);
				return;
			}
		}
		else if (this.openForBusinessDesired || this.openForBusinessDesired != this.openForBusinessActual)
		{
			this.SetOpen(false, false);
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0007117C File Offset: 0x0006F37C
	public void SetOpen(bool openClosed, bool forceActual = false)
	{
		bool flag = false;
		if (openClosed != this.openForBusinessDesired)
		{
			flag = true;
		}
		this.openForBusinessDesired = openClosed;
		int num = 0;
		if (this.address != null && this.address.otherSecurity.Count > 0 && !this.openForBusinessDesired && !this.address.currentOccupants.Exists((Actor item) => item.locationsOfAuthority.Contains(this.address)))
		{
			foreach (Interactable interactable in this.address.otherSecurity)
			{
				interactable.SetCustomState2(true, null, true, true, false);
			}
		}
		if (!this.openForBusinessDesired && this.address != null && this.address.entrances.Count > 0)
		{
			NewNode.NodeAccess mainEntrance = this.address.GetMainEntrance();
			if (mainEntrance != null)
			{
				NewGameLocation otherGameLocation = mainEntrance.GetOtherGameLocation(this.address);
				bool flag2 = false;
				if (Vector3.Distance(mainEntrance.worldAccessPoint, Player.Instance.transform.position) > 30f)
				{
					flag2 = true;
				}
				List<Actor> list = new List<Actor>(this.address.currentOccupants);
				if (Player.Instance.currentGameLocation == this.address)
				{
					Player.Instance.UpdateTrespassing(false);
				}
				foreach (Actor actor in list)
				{
					Citizen citizen = actor as Citizen;
					if (!(citizen == null) && !(actor == Player.Instance) && citizen.job.employer != this)
					{
						bool allowEnforcersEverywhere = false;
						num++;
						if (citizen.ai != null)
						{
							if (citizen.ai.currentGoal != null)
							{
								allowEnforcersEverywhere = citizen.ai.currentGoal.preset.allowEnforcersEverywhere;
							}
							NewNode newNode = citizen.FindSafeTeleport(otherGameLocation, false, true);
							if (!citizen.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.toGoWalkGoal))
							{
								citizen.ai.CreateNewGoal(RoutineControls.Instance.toGoWalkGoal, 0f, 0f, newNode, null, null, null, null, -2);
							}
							if (flag2 && !citizen.visible)
							{
								citizen.Teleport(newNode, null, true, false);
							}
							citizen.ai.AITick(false, false);
						}
						citizen.UpdateTrespassing(allowEnforcersEverywhere);
					}
				}
			}
			if (Player.Instance.currentGameLocation == this.address && flag)
			{
				InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Closing Time", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.run, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
				if (Player.Instance.spendingTimeMode)
				{
					Player.Instance.SetSpendingTimeMode(false);
				}
			}
		}
		if ((this.currentStaff.Count > 0 && num <= 0) || forceActual)
		{
			this.openForBusinessActual = this.openForBusinessDesired;
			if (this.openForBusinessActual)
			{
				this.debugLastOpenedAt = SessionData.Instance.GameTimeToClock24String(SessionData.Instance.gameTime, false);
				this.OnActualOpen();
				return;
			}
			this.debugLastClosedAt = SessionData.Instance.GameTimeToClock24String(SessionData.Instance.gameTime, false);
			this.OnActualClose();
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00071520 File Offset: 0x0006F720
	public void OnAddressCitizenEnter(Citizen cc)
	{
		if (cc == null)
		{
			return;
		}
		if (cc.job.employer == this)
		{
			this.currentStaff.Add(cc.job);
			if (this.openForBusinessActual != this.openForBusinessDesired)
			{
				this.SetOpen(this.openForBusinessDesired, false);
			}
		}
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00071574 File Offset: 0x0006F774
	public void OnAddressCitizenExit(Citizen cc)
	{
		if (cc == null)
		{
			return;
		}
		if (cc.job.employer == this)
		{
			if (this.openForBusinessActual != this.openForBusinessDesired)
			{
				this.SetOpen(this.openForBusinessDesired, false);
			}
			this.currentStaff.Remove(cc.job);
		}
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x000715C6 File Offset: 0x0006F7C6
	public int GetNumberOfFilledJobs()
	{
		return this.companyRoster.FindAll((Occupation item) => item.employee != null).Count;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000715F8 File Offset: 0x0006F7F8
	public void OnActualOpen()
	{
		if (this.address != null)
		{
			foreach (NewNode.NodeAccess nodeAccess in this.address.entrances)
			{
				if (nodeAccess.door != null)
				{
					nodeAccess.door.lockSetting = NewDoor.LockSetting.keepUnlocked;
				}
			}
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00071674 File Offset: 0x0006F874
	public void OnActualClose()
	{
		if (this.address != null)
		{
			foreach (NewNode.NodeAccess nodeAccess in this.address.entrances)
			{
				if (nodeAccess.door != null && nodeAccess.door != null)
				{
					nodeAccess.door.lockSetting = NewDoor.LockSetting.keepLocked;
				}
			}
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x000716FC File Offset: 0x0006F8FC
	public void AddSalesRecord(Human who, InteractablePreset what, float time)
	{
		if (!this.preset.recordSalesData)
		{
			return;
		}
		while (this.sales.Count > RoutineControls.Instance.salesRecordsThreshold)
		{
			this.sales.RemoveAt(this.sales.Count - 1);
		}
		for (int i = 0; i < this.sales.Count; i++)
		{
			if (time > this.sales[i].time)
			{
				this.sales.Insert(i, new Company.SalesRecord(this, who, what, time));
				return;
			}
		}
		this.sales.Add(new Company.SalesRecord(this, who, what, time));
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0007179C File Offset: 0x0006F99C
	public void AddSalesRecord(Human who, List<InteractablePreset> what, float time)
	{
		if (!this.preset.recordSalesData)
		{
			return;
		}
		while (this.sales.Count > RoutineControls.Instance.salesRecordsThreshold)
		{
			this.sales.RemoveAt(this.sales.Count - 1);
		}
		for (int i = 0; i < this.sales.Count; i++)
		{
			if (time > this.sales[i].time)
			{
				this.sales.Insert(i, new Company.SalesRecord(this, who, what, time));
				return;
			}
		}
		this.sales.Add(new Company.SalesRecord(this, who, what, time));
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0007183C File Offset: 0x0006FA3C
	public CitySaveData.CompanyCitySave GenerateSaveData()
	{
		CitySaveData.CompanyCitySave companyCitySave = new CitySaveData.CompanyCitySave();
		companyCitySave.preset = this.preset.name;
		companyCitySave.id = this.companyID;
		foreach (Occupation occupation in this.companyRoster)
		{
			companyCitySave.companyRoster.Add(occupation.GenerateSaveData());
		}
		companyCitySave.topSalary = this.topSalary;
		companyCitySave.minimumSalary = this.minimumSalary;
		companyCitySave.publicFacing = this.publicFacing;
		companyCitySave.shortName = this.shortName;
		companyCitySave.nameAltTags = this.nameAltTags;
		companyCitySave.monday = this.monday;
		companyCitySave.tuesday = this.tuesday;
		companyCitySave.wednesday = this.wednesday;
		companyCitySave.thursday = this.thursday;
		companyCitySave.friday = this.friday;
		companyCitySave.saturday = this.saturday;
		companyCitySave.sunday = this.sunday;
		companyCitySave.retailOpenHours = this.retailOpenHours;
		companyCitySave.passedWorkLocation = this.passedWorkLocationID;
		foreach (KeyValuePair<InteractablePreset, int> keyValuePair in this.prices)
		{
			companyCitySave.menuItems.Add(keyValuePair.Key.name);
			companyCitySave.itemCosts.Add(keyValuePair.Value);
		}
		return companyCitySave;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000719CC File Offset: 0x0006FBCC
	public void UpdatePassedWorkPosition()
	{
		if (this.passedWorkLocationID > 0 && this.passedWorkLocationID > 0)
		{
			Interactable interactable = null;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(this.passedWorkLocationID, ref interactable))
			{
				this.passedWorkPosition = interactable;
				this.SetPlaceOfBusiness(this.passedWorkPosition.node.gameLocation);
			}
		}
	}

	// Token: 0x04000792 RID: 1938
	public string name;

	// Token: 0x04000793 RID: 1939
	public string seed;

	// Token: 0x04000794 RID: 1940
	[Header("Location")]
	public NewAddress address;

	// Token: 0x04000795 RID: 1941
	public NewGameLocation placeOfBusiness;

	// Token: 0x04000796 RID: 1942
	[Header("Details")]
	public int companyID;

	// Token: 0x04000797 RID: 1943
	public static int assignCompanyID = 1;

	// Token: 0x04000798 RID: 1944
	public string shortName;

	// Token: 0x04000799 RID: 1945
	public List<string> nameAltTags = new List<string>();

	// Token: 0x0400079A RID: 1946
	public int numberOfRankLevels;

	// Token: 0x0400079B RID: 1947
	public int numberOfJobPositions;

	// Token: 0x0400079C RID: 1948
	[NonSerialized]
	public CompanyPreset preset;

	// Token: 0x0400079D RID: 1949
	public List<CompanyOpenHoursPreset.CompanyShift> shifts = new List<CompanyOpenHoursPreset.CompanyShift>();

	// Token: 0x0400079E RID: 1950
	public List<Occupation> companyRoster = new List<Occupation>();

	// Token: 0x0400079F RID: 1951
	public float topSalary;

	// Token: 0x040007A0 RID: 1952
	public float minimumSalary;

	// Token: 0x040007A1 RID: 1953
	public Human director;

	// Token: 0x040007A2 RID: 1954
	public Human receptionist;

	// Token: 0x040007A3 RID: 1955
	public Human janitor;

	// Token: 0x040007A4 RID: 1956
	public Human security;

	// Token: 0x040007A5 RID: 1957
	public bool publicFacing = true;

	// Token: 0x040007A6 RID: 1958
	public Color uniformColour = Color.black;

	// Token: 0x040007A7 RID: 1959
	[NonSerialized]
	public int passedWorkLocationID = -1;

	// Token: 0x040007A8 RID: 1960
	[NonSerialized]
	public Interactable passedWorkPosition;

	// Token: 0x040007A9 RID: 1961
	[Header("Opening Hours")]
	public bool monday = true;

	// Token: 0x040007AA RID: 1962
	public bool tuesday = true;

	// Token: 0x040007AB RID: 1963
	public bool wednesday = true;

	// Token: 0x040007AC RID: 1964
	public bool thursday = true;

	// Token: 0x040007AD RID: 1965
	public bool friday = true;

	// Token: 0x040007AE RID: 1966
	public bool saturday = true;

	// Token: 0x040007AF RID: 1967
	public bool sunday;

	// Token: 0x040007B0 RID: 1968
	public List<SessionData.WeekDay> daysOpen = new List<SessionData.WeekDay>();

	// Token: 0x040007B1 RID: 1969
	public List<SessionData.WeekDay> daysClosed = new List<SessionData.WeekDay>();

	// Token: 0x040007B2 RID: 1970
	public Vector2 retailOpenHours = new Vector2(8f, 17f);

	// Token: 0x040007B3 RID: 1971
	public bool openForBusinessDesired;

	// Token: 0x040007B4 RID: 1972
	public bool openForBusinessActual;

	// Token: 0x040007B5 RID: 1973
	public List<Occupation> currentStaff = new List<Occupation>();

	// Token: 0x040007B6 RID: 1974
	[NonSerialized]
	public EvidenceMultiPage employeeRoster;

	// Token: 0x040007B7 RID: 1975
	[NonSerialized]
	public Evidence menu;

	// Token: 0x040007B8 RID: 1976
	[NonSerialized]
	public Evidence salesRecords;

	// Token: 0x040007B9 RID: 1977
	private bool createdEvidence;

	// Token: 0x040007BA RID: 1978
	[Header("Sales")]
	public Dictionary<InteractablePreset, int> prices = new Dictionary<InteractablePreset, int>();

	// Token: 0x040007BB RID: 1979
	public List<Company.SalesRecord> sales = new List<Company.SalesRecord>();

	// Token: 0x040007BC RID: 1980
	public Dictionary<RetailItemPreset, Evidence> itemSingletons = new Dictionary<RetailItemPreset, Evidence>();

	// Token: 0x040007BD RID: 1981
	public List<string> debugAddressSet = new List<string>();

	// Token: 0x040007BE RID: 1982
	public string debugLastOpenedAt;

	// Token: 0x040007BF RID: 1983
	public string debugLastClosedAt;

	// Token: 0x02000101 RID: 257
	[Serializable]
	public class SalesRecord
	{
		// Token: 0x06000761 RID: 1889 RVA: 0x00071B18 File Offset: 0x0006FD18
		public SalesRecord(Company newCompany, Human newPunter, InteractablePreset newItem, float newTime)
		{
			this.companyID = newCompany.companyID;
			this.punterID = newPunter.humanID;
			this.time = newTime;
			this.items.Add(newItem.name);
			if (newCompany.prices.ContainsKey(newItem))
			{
				this.cost = Toolbox.Instance.RoundToPlaces((float)newCompany.prices[newItem], 2);
			}
			this.SpawnFact();
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00071B9C File Offset: 0x0006FD9C
		public SalesRecord(Company newCompany, Human newPunter, List<InteractablePreset> newItem, float newTime)
		{
			this.companyID = newCompany.companyID;
			this.punterID = newPunter.humanID;
			this.time = newTime;
			this.cost = 0f;
			foreach (InteractablePreset interactablePreset in newItem)
			{
				this.items.Add(interactablePreset.name);
				if (newCompany.prices.ContainsKey(interactablePreset))
				{
					this.cost += (float)newCompany.prices[interactablePreset];
				}
			}
			this.cost = Toolbox.Instance.RoundToPlaces(this.cost, 2);
			this.SpawnFact();
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00071C78 File Offset: 0x0006FE78
		public Company GetCompany()
		{
			return CityData.Instance.companyDirectory.Find((Company item) => item.companyID == this.companyID);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00071C98 File Offset: 0x0006FE98
		public Human GetPunter()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.punterID, out result, true);
			return result;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00071CBC File Offset: 0x0006FEBC
		public void SpawnFact()
		{
			if (this.fact == null)
			{
				Evidence timeEvidence = EvidenceCreator.Instance.GetTimeEvidence(this.time, this.time, "time", "", -1, -1);
				List<Evidence> list = new List<Evidence>();
				Company company = this.GetCompany();
				if (company != null)
				{
					list.Add(company.placeOfBusiness.evidenceEntry);
					list.Add(timeEvidence);
				}
				List<Evidence> list2 = new List<Evidence>();
				Human punter = this.GetPunter();
				if (punter != null)
				{
					list2.Add(punter.evidenceEntry);
					list2.Add(company.salesRecords);
				}
				List<object> list3 = new List<object>();
				list3.Add(this);
				this.fact = EvidenceCreator.Instance.CreateFact("Purchased", null, null, list2, list, false, list3, null, null, false);
			}
		}

		// Token: 0x040007C0 RID: 1984
		public int companyID;

		// Token: 0x040007C1 RID: 1985
		public int punterID;

		// Token: 0x040007C2 RID: 1986
		public List<string> items = new List<string>();

		// Token: 0x040007C3 RID: 1987
		public float time;

		// Token: 0x040007C4 RID: 1988
		public float cost;

		// Token: 0x040007C5 RID: 1989
		[NonSerialized]
		public Fact fact;
	}
}
