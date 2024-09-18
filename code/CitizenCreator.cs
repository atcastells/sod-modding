using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class CitizenCreator : Creator
{
	// Token: 0x1700003E RID: 62
	// (get) Token: 0x0600098F RID: 2447 RVA: 0x000932A6 File Offset: 0x000914A6
	public static CitizenCreator Instance
	{
		get
		{
			return CitizenCreator._instance;
		}
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x000932AD File Offset: 0x000914AD
	private void Awake()
	{
		if (CitizenCreator._instance != null && CitizenCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CitizenCreator._instance = this;
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x000932DB File Offset: 0x000914DB
	public override void StartLoading()
	{
		Game.Log("CityGen: Populating with citizens...", 2);
		base.StartCoroutine("Populate");
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x000932F4 File Offset: 0x000914F4
	private IEnumerator Populate()
	{
		string seed = CityData.Instance.seed;
		int citCursor;
		if (!CityConstructor.Instance.generateNew)
		{
			citCursor = 0;
			while (citCursor < CityConstructor.Instance.currentData.citizens.Count)
			{
				for (int i = 0; i < this.loadChunk; i++)
				{
					Citizen component = Object.Instantiate<GameObject>(PrefabControls.Instance.citizen, this.citizenHolder.transform).GetComponent<Citizen>();
					CitizenBehaviour.Instance.visibleHumans++;
					component.outfitController.debugOverride = false;
					component.GetComponent<NewAIController>().Setup(component);
					component.Load(CityConstructor.Instance.currentData.citizens[citCursor]);
					component.SetVisible(false, true);
					if (!component.isPlayer)
					{
						CityData.Instance.citizenDirectory.Add(component);
						CityData.Instance.citizenDictionary.Add(component.humanID, component);
					}
					int num = citCursor;
					citCursor = num + 1;
					if (citCursor >= CityConstructor.Instance.currentData.citizens.Count)
					{
						break;
					}
				}
				CityConstructor.Instance.loadingProgress = (float)citCursor / (float)CityConstructor.Instance.currentData.citizens.Count;
				yield return null;
			}
			using (List<CitySaveData.HumanCitySave>.Enumerator enumerator = CityConstructor.Instance.currentData.citizens.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CitySaveData.HumanCitySave humanCitySave = enumerator.Current;
					Human human = null;
					if (CityData.Instance.GetHuman(humanCitySave.humanID, out human, true))
					{
						if (humanCitySave.partner > -1)
						{
							Human human2 = null;
							if (CityData.Instance.GetHuman(humanCitySave.partner, out human2, true))
							{
								human.SetPartner(human2 as Citizen);
								human2.SetPartner(human as Citizen);
							}
						}
						if (humanCitySave.paramour > -1)
						{
							Human human3 = null;
							if (CityData.Instance.GetHuman(humanCitySave.paramour, out human3, true))
							{
								human.paramour = (human3 as Citizen);
								human3.paramour = (human as Citizen);
							}
						}
						human.LoadAcquaintances(humanCitySave);
					}
				}
				goto IL_1409;
			}
		}
		citCursor = 0;
		int employedCitizens = 0;
		int unemployedCitizens = 0;
		int citizensOrCouplesToSpawn = 0;
		List<ResidenceController> list = new List<ResidenceController>();
		int num2 = Mathf.Max(Mathf.FloorToInt(CityData.Instance.citySize.x * CityData.Instance.citySize.y * 0.1f), 3);
		List<ResidenceController> allVacantResidences = new List<ResidenceController>();
		foreach (ResidenceController residenceController in CityData.Instance.residenceDirectory)
		{
			if (residenceController.preset != null && residenceController.preset.habitable)
			{
				allVacantResidences.Add(residenceController);
			}
		}
		allVacantResidences.Sort();
		allVacantResidences.Reverse();
		for (int j = 0; j < num2; j++)
		{
			ResidenceController residenceController2;
			if (j == 0)
			{
				residenceController2 = allVacantResidences[Toolbox.Instance.GetPsuedoRandomNumberContained(0, Mathf.Min(5, allVacantResidences.Count - 1), seed, out seed)];
			}
			else if (j == 1)
			{
				residenceController2 = allVacantResidences[Toolbox.Instance.GetPsuedoRandomNumberContained(Mathf.RoundToInt((float)allVacantResidences.Count * 0.33f), Mathf.RoundToInt((float)allVacantResidences.Count * 0.66f), seed, out seed)];
			}
			else if (j == 2)
			{
				residenceController2 = allVacantResidences[Toolbox.Instance.GetPsuedoRandomNumberContained(Mathf.RoundToInt((float)allVacantResidences.Count * 0.75f), allVacantResidences.Count, seed, out seed)];
			}
			else
			{
				residenceController2 = allVacantResidences[Toolbox.Instance.GetPsuedoRandomNumberContained(0, allVacantResidences.Count, seed, out seed)];
			}
			if (residenceController2 != null)
			{
				allVacantResidences.Remove(residenceController2);
				list.Add(residenceController2);
			}
		}
		foreach (ResidenceController residenceController3 in allVacantResidences)
		{
			citizensOrCouplesToSpawn += Mathf.Max(residenceController3.bedrooms.Count, 1);
		}
		citizensOrCouplesToSpawn = Mathf.FloorToInt((float)citizensOrCouplesToSpawn * CityData.Instance.populationMultiplier) - 6;
		int apartmentCapacity = citizensOrCouplesToSpawn;
		int homelessToSpawn = Mathf.FloorToInt(CityData.Instance.citySize.x * CityData.Instance.citySize.y * CityControls.Instance.homelessMultiplier * CityData.Instance.populationMultiplier);
		int totalHomelessToSpawn = homelessToSpawn;
		List<Citizen> withoutJobs = new List<Citizen>();
		List<Occupation> freeJobs = new List<Occupation>(CityData.Instance.jobsDirectory);
		List<CompanyPreset> selfEmployedAutoCreate = new List<CompanyPreset>();
		foreach (CompanyPreset companyPreset in Toolbox.Instance.allCompanyPresets)
		{
			if (companyPreset.isSelfEmployed && companyPreset.autoCreate)
			{
				int num3 = Mathf.FloorToInt((float)withoutJobs.Count * companyPreset.cityPopRatio);
				num3 = Mathf.Clamp(num3, companyPreset.minimumNumber, companyPreset.maximumNumber);
				for (int k = 0; k < num3; k++)
				{
					selfEmployedAutoCreate.Add(companyPreset);
				}
			}
		}
		selfEmployedAutoCreate.Sort((CompanyPreset p1, CompanyPreset p2) => p2.priority.CompareTo(p1.priority));
		freeJobs.Sort(Occupation.FillPriorityComparison);
		freeJobs.Reverse();
		foreach (OccupationPreset occupationPreset in Toolbox.Instance.allCriminalJobs)
		{
			for (int l = 0; l < occupationPreset.minimumPerCity; l++)
			{
				freeJobs.Insert(0, this.CreateCriminal(occupationPreset));
			}
		}
		Game.Log("CityGen: >---------Citizen Generation---------<", 2);
		Game.Log("CityGen: Use a population multiplier of " + CityData.Instance.populationMultiplier.ToString(), 2);
		Game.Log(string.Concat(new string[]
		{
			"CityGen: The city will generate ",
			citizensOrCouplesToSpawn.ToString(),
			" homed singles or couples based on ",
			apartmentCapacity.ToString(),
			" city apartments..."
		}), 2);
		Game.Log("CityGen: The city will spawn " + homelessToSpawn.ToString() + " homeless...", 2);
		Game.Log("CityGen: There are " + freeJobs.Count.ToString() + " job in the city...", 2);
		List<ResidenceController> allInhabitedResidences = new List<ResidenceController>();
		List<Citizen> citizensToHouse = new List<Citizen>();
		int setupPhaseCursor = 0;
		int populatePhase = 0;
		float spawnProgress = 0f;
		float jobProgress = 0f;
		float housingProgress = 0f;
		float homelessProgress = 0f;
		float miscProgress = 0f;
		new HashSet<string>();
		while (populatePhase < 5)
		{
			if (populatePhase == 0)
			{
				int num4 = 0;
				while (num4 < this.loadChunk && citizensOrCouplesToSpawn > 0)
				{
					Citizen component2 = Object.Instantiate<GameObject>(PrefabControls.Instance.citizen, this.citizenHolder.transform).GetComponent<Citizen>();
					component2.outfitController.debugOverride = false;
					component2.GetComponent<NewAIController>().Setup(component2);
					CityData.Instance.homedDirectory.Add(component2);
					component2.SetSexualityAndGender();
					if (!component2.isPlayer)
					{
						CityData.Instance.citizenDirectory.Add(component2);
						CityData.Instance.citizenDictionary.Add(component2.humanID, component2);
						withoutJobs.Add(component2);
						citizensToHouse.Add(component2);
					}
					if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, component2.seed, out component2.seed) < SocialStatistics.Instance.seriousRelationshipsRatio)
					{
						Citizen component3 = Object.Instantiate<GameObject>(PrefabControls.Instance.citizen, this.citizenHolder.transform).GetComponent<Citizen>();
						component3.outfitController.debugOverride = false;
						component3.GetComponent<NewAIController>().Setup(component3);
						component3.GenerateSuitableGenderAndSexualityForParnter(component2);
						if (!component3.isPlayer)
						{
							CityData.Instance.citizenDirectory.Add(component3);
							CityData.Instance.citizenDictionary.Add(component3.humanID, component3);
							withoutJobs.Add(component3);
							citizensToHouse.Add(component3);
						}
						component2.SetPartner(component3);
						component3.SetPartner(component2);
						citCursor += 2;
					}
					int num = citizensOrCouplesToSpawn;
					citizensOrCouplesToSpawn = num - 1;
					num4++;
				}
				spawnProgress = (float)(apartmentCapacity - citizensOrCouplesToSpawn) / (float)apartmentCapacity;
				if (citizensOrCouplesToSpawn <= 0)
				{
					Game.Log(string.Concat(new string[]
					{
						"CityGen: ",
						CityData.Instance.citizenDirectory.Count.ToString(),
						" homed citizens created, ",
						citCursor.ToString(),
						" (",
						Mathf.RoundToInt((float)citCursor / (float)CityData.Instance.citizenDirectory.Count * 100f).ToString(),
						"%) are in a partnership"
					}), 2);
					int num = populatePhase;
					populatePhase = num + 1;
				}
			}
			else if (populatePhase == 1)
			{
				int num5 = 0;
				while (num5 < this.loadChunk && withoutJobs.Count > 0)
				{
					Citizen citizen = withoutJobs[Toolbox.Instance.GetPsuedoRandomNumberContained(0, withoutJobs.Count, seed, out seed)];
					if (selfEmployedAutoCreate.Count > 0 && (citizen.partner == null || (citizen.partner.job != null && !citizen.partner.job.preset.selfEmployed)))
					{
						CityConstructor.Instance.CreateSelfEmployed(selfEmployedAutoCreate[0], citizen, null);
						int num = employedCitizens;
						employedCitizens = num + 1;
						citizen.CalculateAge();
						citizen.SetPersonality();
						CityData.Instance.employedCitizens++;
						Game.Log("CityGen: Auto create self employed: " + selfEmployedAutoCreate[0].name + " " + citizen.GetCitizenName(), 2);
						selfEmployedAutoCreate.RemoveAt(0);
						withoutJobs.Remove(citizen);
					}
					else
					{
						Occupation occupation;
						if (freeJobs.Count > 0)
						{
							occupation = freeJobs[Toolbox.Instance.GetPsuedoRandomNumberContained(0, Mathf.Min(5, freeJobs.Count), seed, out seed)];
							int num = employedCitizens;
							employedCitizens = num + 1;
						}
						else
						{
							occupation = this.CreateUnemployed();
							int num = unemployedCitizens;
							unemployedCitizens = num + 1;
						}
						citizen.SetJob(occupation);
						citizen.CalculateAge();
						citizen.SetPersonality();
						CityData.Instance.employedCitizens++;
						freeJobs.Remove(occupation);
						withoutJobs.Remove(citizen);
					}
					num5++;
				}
				jobProgress = (float)(CityData.Instance.citizenDirectory.Count - withoutJobs.Count) / (float)CityData.Instance.citizenDirectory.Count;
				if (withoutJobs.Count <= 0)
				{
					int num = populatePhase;
					populatePhase = num + 1;
					Game.Log(string.Concat(new string[]
					{
						"CityGen: ",
						employedCitizens.ToString(),
						" citizens are employed (",
						Mathf.RoundToInt((float)employedCitizens / (float)CityData.Instance.citizenDirectory.Count * 100f).ToString(),
						"%)"
					}), 2);
					citizensToHouse.Sort((Citizen p1, Citizen p2) => p2.societalClass.CompareTo(p1.societalClass));
				}
			}
			else if (populatePhase == 2)
			{
				for (int m = 0; m < this.loadChunk; m++)
				{
					if (Player.Instance.home == null)
					{
						List<ResidenceController> list2 = allVacantResidences.FindAll((ResidenceController item) => Game.Instance.preferredStartingBuildings.Contains(item.building.preset) && item.address.floor.floor >= 2 && item.address.floor.floor <= 3 && item.bedrooms.Count >= 1);
						if (list2.Count <= 0)
						{
							list2 = allVacantResidences.FindAll((ResidenceController item) => item.address.floor.floor >= 0 && item.address.floor.floor <= 4 && item.address.residence.bedrooms.Count > 0);
						}
						if (list2.Count <= 0)
						{
							list2 = allVacantResidences.FindAll((ResidenceController item) => item.address.floor.floor >= 0);
						}
						Game.Log("CityGen: Picking from " + list2.Count.ToString() + " player apartments", 2);
						ResidenceController residenceController4 = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, seed, out seed)];
						if (residenceController4 != null)
						{
							Player.Instance.SetResidence(residenceController4, true);
							allVacantResidences.Remove(residenceController4);
							allInhabitedResidences.Remove(residenceController4);
						}
					}
					if (citizensToHouse.Count <= 0)
					{
						break;
					}
					Citizen citizen2 = citizensToHouse[0];
					if (citizen2.home != null)
					{
						citizensToHouse.Remove(citizen2);
					}
					else
					{
						ResidenceController residenceController5 = null;
						bool flag = false;
						if (allVacantResidences.Count > 0)
						{
							residenceController5 = allVacantResidences[0];
							flag = true;
						}
						else if (allInhabitedResidences.Count > 0)
						{
							allInhabitedResidences.Sort(ResidenceController.RoommateComparison);
							int n = 0;
							while (n < allInhabitedResidences.Count)
							{
								if (citizen2.job != null && citizen2.job.preset.selfEmployed)
								{
									if (allInhabitedResidences[n].address.owners.Exists((Human item) => item.job != null && item.job.preset.selfEmployed))
									{
										n++;
										continue;
									}
								}
								residenceController5 = allInhabitedResidences[n];
								flag = true;
								break;
							}
						}
						else
						{
							Game.LogError("CityGen: There are no possible places to move this citizen in. Something is wrong with your code :(", 2);
						}
						if (flag)
						{
							citizen2.SetResidence(residenceController5, true);
							citizensToHouse.Remove(citizen2);
							residenceController5.bedroomsTaken++;
							if (citizen2.partner != null)
							{
								citizen2.partner.SetResidence(residenceController5, true);
								citizensToHouse.Remove(citizen2.partner);
							}
							if (allVacantResidences.Contains(residenceController5))
							{
								CityData.Instance.inhabitedResidences++;
								allVacantResidences.Remove(residenceController5);
							}
							if (residenceController5.bedroomsTaken < residenceController5.bedrooms.Count)
							{
								if (!allInhabitedResidences.Contains(residenceController5))
								{
									allInhabitedResidences.Add(residenceController5);
								}
							}
							else if (allInhabitedResidences.Contains(residenceController5))
							{
								allInhabitedResidences.Remove(residenceController5);
							}
						}
						if (citizensToHouse.Count <= 0)
						{
							Game.Log("CityGen: " + CityData.Instance.inhabitedResidences.ToString() + " inhabited apartments out of " + CityData.Instance.residenceDirectory.Count.ToString(), 2);
							break;
						}
					}
				}
				housingProgress = (float)(CityData.Instance.citizenDirectory.Count - citizensToHouse.Count) / (float)CityData.Instance.citizenDirectory.Count;
				if (citizensToHouse.Count <= 0)
				{
					int num = populatePhase;
					populatePhase = num + 1;
				}
			}
			else if (populatePhase == 3)
			{
				int num6 = 0;
				while (num6 < this.loadChunk && homelessToSpawn > 0)
				{
					Citizen component4 = Object.Instantiate<GameObject>(PrefabControls.Instance.citizen, this.citizenHolder.transform).GetComponent<Citizen>();
					component4.outfitController.debugOverride = false;
					component4.GetComponent<NewAIController>().Setup(component4);
					component4.isHomeless = true;
					CityData.Instance.homelessDirectory.Add(component4);
					CityData.Instance.homlessAssign.Add(component4);
					component4.SetSexualityAndGender();
					Occupation job = this.CreateUnemployed();
					component4.SetJob(job);
					component4.CalculateAge();
					component4.AddCharacterTrait(CitizenControls.Instance.destitute);
					component4.SetPersonality();
					if (!component4.isPlayer)
					{
						CityData.Instance.citizenDirectory.Add(component4);
						CityData.Instance.citizenDictionary.Add(component4.humanID, component4);
					}
					int num = homelessToSpawn;
					homelessToSpawn = num - 1;
					num6++;
				}
				if (homelessToSpawn <= 0)
				{
					homelessProgress = 1f;
					int num = populatePhase;
					populatePhase = num + 1;
				}
				else
				{
					homelessProgress = 1f - (float)homelessToSpawn / (float)totalHomelessToSpawn;
				}
			}
			else if (populatePhase == 4)
			{
				for (int num7 = 0; num7 < this.loadChunk; num7++)
				{
					Citizen citizen3 = CityData.Instance.citizenDirectory[setupPhaseCursor];
					citizen3.SetupGeneral();
					citizen3.GenerateRoutineGoals();
					citizen3.outfitController.debugOverride = false;
					citizen3.outfitController.GenerateOutfits(false);
					int num = setupPhaseCursor;
					setupPhaseCursor = num + 1;
					if (setupPhaseCursor >= CityData.Instance.citizenDirectory.Count)
					{
						break;
					}
				}
				miscProgress = (float)setupPhaseCursor / (float)CityData.Instance.citizenDirectory.Count;
				if (setupPhaseCursor >= CityData.Instance.citizenDirectory.Count)
				{
					int num = populatePhase;
					populatePhase = num + 1;
				}
			}
			CityConstructor.Instance.loadingProgress = (spawnProgress + jobProgress + housingProgress + homelessProgress + miscProgress) / 5f;
			yield return null;
		}
		allVacantResidences = null;
		withoutJobs = null;
		freeJobs = null;
		selfEmployedAutoCreate = null;
		allInhabitedResidences = null;
		citizensToHouse = null;
		IL_1409:
		Game.Log("CityGen: Total population: " + CityData.Instance.citizenDirectory.Count.ToString(), 2);
		Game.Log("CityGen: >--------------------------------<", 2);
		base.SetComplete();
		yield break;
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00093304 File Offset: 0x00091504
	public Occupation CreateUnemployed()
	{
		Occupation occupation = new Occupation();
		occupation.preset = this.unemployedPreset;
		occupation.employer = null;
		occupation.paygrade = 0f;
		occupation.name = Strings.Get("jobs", occupation.preset.name, Strings.Casing.asIs, false, false, false, null);
		CityData.Instance.unemployedDirectory.Add(occupation);
		return occupation;
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x00093368 File Offset: 0x00091568
	public Occupation CreateCriminal(OccupationPreset preset)
	{
		Occupation occupation = new Occupation();
		occupation.preset = preset;
		occupation.employer = null;
		occupation.Setup();
		occupation.paygrade = preset.societalClass;
		CityData.Instance.criminalJobDirectory.Add(occupation);
		CityData.Instance.jobsDirectory.Add(occupation);
		return occupation;
	}

	// Token: 0x040009B9 RID: 2489
	public int loadChunk = 10;

	// Token: 0x040009BA RID: 2490
	public GameObject unemploymentHolder;

	// Token: 0x040009BB RID: 2491
	public GameObject criminalHolder;

	// Token: 0x040009BC RID: 2492
	public OccupationPreset unemployedPreset;

	// Token: 0x040009BD RID: 2493
	public OccupationPreset retiredPreset;

	// Token: 0x040009BE RID: 2494
	public GameObject citizenObj;

	// Token: 0x040009BF RID: 2495
	public Texture agentTexture;

	// Token: 0x040009C0 RID: 2496
	public Texture suspectTexture;

	// Token: 0x040009C1 RID: 2497
	public GameObject citizenHolder;

	// Token: 0x040009C2 RID: 2498
	public int rUnemployed = 10;

	// Token: 0x040009C3 RID: 2499
	public int rRetired = 10;

	// Token: 0x040009C4 RID: 2500
	private static CitizenCreator _instance;
}
