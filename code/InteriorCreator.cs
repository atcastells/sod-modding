using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class InteriorCreator : Creator
{
	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060009AF RID: 2479 RVA: 0x00094A68 File Offset: 0x00092C68
	public static InteriorCreator Instance
	{
		get
		{
			return InteriorCreator._instance;
		}
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00094A6F File Offset: 0x00092C6F
	private void Awake()
	{
		if (InteriorCreator._instance != null && InteriorCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InteriorCreator._instance = this;
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00094A9D File Offset: 0x00092C9D
	public override void StartLoading()
	{
		Game.Log("CityGen: Generating building interiors...", 2);
		base.StartCoroutine("GenChunk");
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00094AB6 File Offset: 0x00092CB6
	private IEnumerator GenChunk()
	{
		int cursor = 0;
		float num = (CityData.Instance.citySize.x * CityData.Instance.citySize.y - 36f) / 100f;
		List<SyncDiskPreset> list = new List<SyncDiskPreset>();
		foreach (SyncDiskPreset syncDiskPreset in Toolbox.Instance.allSyncDisks)
		{
			if (!syncDiskPreset.disabled)
			{
				list.Insert(Toolbox.Instance.SeedRand(0, list.Count), syncDiskPreset);
			}
		}
		foreach (SyncDiskPreset syncDiskPreset2 in list)
		{
			float num2 = GameplayControls.Instance.commonSyncDisksPer200Citizens;
			if (syncDiskPreset2.rarity == SyncDiskPreset.Rarity.medium)
			{
				num2 = GameplayControls.Instance.mediumSyncDisksPer200Citizens;
			}
			else if (syncDiskPreset2.rarity == SyncDiskPreset.Rarity.rare)
			{
				num2 = GameplayControls.Instance.rareSyncDisksPer200Citizens;
			}
			else if (syncDiskPreset2.rarity == SyncDiskPreset.Rarity.veryRare)
			{
				num2 = GameplayControls.Instance.veryRareSyncDisksPer200Citizens;
			}
			int num3 = Mathf.Max(Mathf.RoundToInt((float)CityData.Instance.citizenDirectory.Count / 200f * num2), 1);
			HashSet<Citizen> hashSet = new HashSet<Citizen>();
			List<Citizen> list2 = new List<Citizen>();
			foreach (Citizen citizen in CityData.Instance.citizenDirectory)
			{
				if (citizen.societalClass >= syncDiskPreset2.minimumWealthLevel)
				{
					int num4 = 0;
					bool flag = true;
					foreach (SyncDiskPreset.TraitPick traitPick in syncDiskPreset2.traits)
					{
						bool flag2 = false;
						if (traitPick.rule == CharacterTrait.RuleType.ifAnyOfThese)
						{
							using (List<CharacterTrait>.Enumerator enumerator4 = traitPick.traitList.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									CharacterTrait searchTrait = enumerator4.Current;
									if (citizen.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = true;
										break;
									}
								}
								goto IL_36D;
							}
							goto IL_20F;
						}
						goto IL_20F;
						IL_36D:
						if (!flag2 && traitPick.mustPassForApplication)
						{
							flag = false;
							num4 = 0;
							break;
						}
						if (flag2)
						{
							num4 += traitPick.appliedFrequency;
							continue;
						}
						continue;
						IL_20F:
						if (traitPick.rule == CharacterTrait.RuleType.ifAllOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator4 = traitPick.traitList.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									CharacterTrait searchTrait = enumerator4.Current;
									if (!citizen.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = false;
										break;
									}
								}
								goto IL_36D;
							}
						}
						if (traitPick.rule == CharacterTrait.RuleType.ifNoneOfThese)
						{
							flag2 = true;
							using (List<CharacterTrait>.Enumerator enumerator4 = traitPick.traitList.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									CharacterTrait searchTrait = enumerator4.Current;
									if (citizen.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = false;
										break;
									}
								}
								goto IL_36D;
							}
						}
						if (traitPick.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese && citizen.partner != null)
						{
							using (List<CharacterTrait>.Enumerator enumerator4 = traitPick.traitList.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									CharacterTrait searchTrait = enumerator4.Current;
									if (citizen.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
									{
										flag2 = true;
										break;
									}
								}
							}
							goto IL_36D;
						}
						goto IL_36D;
					}
					if (flag)
					{
						num4 = Mathf.RoundToInt((float)(num4 * syncDiskPreset2.traitWeight));
						for (int i = 0; i < num4; i++)
						{
							list2.Add(citizen);
							if (!hashSet.Contains(citizen))
							{
								hashSet.Add(citizen);
							}
						}
						if (citizen.job != null && citizen.job.employer != null && syncDiskPreset2.occupation.Contains(citizen.job.preset))
						{
							for (int j = 0; j < syncDiskPreset2.occupationWeight; j++)
							{
								list2.Add(citizen);
								if (!hashSet.Contains(citizen))
								{
									hashSet.Add(citizen);
								}
							}
						}
					}
				}
			}
			if (hashSet.Count <= 0)
			{
				foreach (Citizen citizen2 in CityData.Instance.citizenDirectory)
				{
					if (citizen2.societalClass >= syncDiskPreset2.minimumWealthLevel)
					{
						list2.Add(citizen2);
					}
				}
				if (hashSet.Count <= 0)
				{
					list2.Add(CityData.Instance.citizenDirectory[Toolbox.Instance.SeedRand(0, CityData.Instance.citizenDirectory.Count)]);
				}
			}
			for (int k = 0; k < num3; k++)
			{
				int num5 = Toolbox.Instance.SeedRand(0, list2.Count);
				Citizen owner = list2[num5];
				bool flag3 = false;
				if (owner.job != null && owner.job.employer != null && owner.job.preset.ownsWorkPosition && Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, owner.seed, false) > 0.75f)
				{
					flag3 = true;
				}
				if (flag3)
				{
					owner.job.employer.address.AddToPlacementPool(syncDiskPreset2.interactable, owner, owner, null, null, syncDiskPreset2.interactable.securityLevel, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 10, syncDiskPreset2, null);
				}
				else if (owner.home != null)
				{
					owner.home.AddToPlacementPool(syncDiskPreset2.interactable, owner, owner, null, null, syncDiskPreset2.interactable.securityLevel, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 10, syncDiskPreset2, null);
				}
				if (hashSet.Count > 1)
				{
					list2.RemoveAll((Citizen item) => item == owner);
					hashSet.Remove(owner);
				}
			}
		}
		Game.Log("CityGen: Starting interior generation, load state key: " + Toolbox.Instance.lastRandomNumberKey, 2);
		this.threadedInteriorCreationActive = true;
		while (cursor < CityData.Instance.floorDirectory.Count + CityData.Instance.streetDirectory.Count || this.threads.Count > 0)
		{
			if (cursor < CityData.Instance.floorDirectory.Count + CityData.Instance.streetDirectory.Count)
			{
				int num6 = this.threads.Count;
				while (num6 < Game.Instance.maxThreads && cursor < CityData.Instance.floorDirectory.Count + CityData.Instance.streetDirectory.Count)
				{
					if (cursor < CityData.Instance.streetDirectory.Count)
					{
						InteriorCreator.LoaderThread loaderThread = new InteriorCreator.LoaderThread
						{
							street = CityData.Instance.streetDirectory[cursor]
						};
						loaderThread.thread = base.StartCoroutine(this.ThreadedInteriorGeneration(loaderThread));
					}
					else
					{
						InteriorCreator.LoaderThread loaderThread2 = new InteriorCreator.LoaderThread
						{
							floor = CityData.Instance.floorDirectory[cursor - CityData.Instance.streetDirectory.Count]
						};
						loaderThread2.thread = base.StartCoroutine(this.ThreadedInteriorGeneration(loaderThread2));
					}
					int num7 = cursor;
					cursor = num7 + 1;
					num6++;
				}
			}
			CityConstructor.Instance.loadingProgress = (float)cursor / (float)(CityData.Instance.floorDirectory.Count + CityData.Instance.streetDirectory.Count);
			yield return null;
		}
		this.threadedInteriorCreationActive = false;
		Game.Log("CityGen: Load state key at end of interior generation: " + Toolbox.Instance.lastRandomNumberKey, 2);
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom.furniture)
			{
				foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in furnitureClusterLocation.clusterObjectMap)
				{
					using (List<FurnitureLocation>.Enumerator enumerator8 = keyValuePair.Value.GetEnumerator())
					{
						while (enumerator8.MoveNext())
						{
							FurnitureLocation furniture = enumerator8.Current;
							if (furniture != null && furniture.furniture != null && furniture.furniture.createSelfEmployed != null && furniture.ownerMap.Count <= 0)
							{
								Game.Log("CityGen: Creating self employed from furniture " + furniture.furniture.name + "...", 2);
								Occupation occupation = CityData.Instance.unemployedDirectory.Find((Occupation item) => item.employee != null && item.employee.home != null);
								if (occupation == null)
								{
									Game.Log("CityGen: ... Could not find valid unemployed from pool of: " + CityData.Instance.unemployedDirectory.Count.ToString(), 2);
								}
								else
								{
									Interactable interactable = furniture.integratedInteractables.Find((Interactable item) => item.pt == (int)furniture.furniture.workPositionID);
									if (interactable == null)
									{
										Game.Log("CityGen: ... Could not find work position: " + furniture.furniture.workPositionID.ToString() + " interactables: " + furniture.integratedInteractables.Count.ToString(), 2);
									}
									else
									{
										CityConstructor.Instance.CreateSelfEmployed(furniture.furniture.createSelfEmployed, occupation.employee, interactable);
									}
								}
							}
						}
					}
				}
			}
			newRoom.ApplyBlockedAccess();
		}
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			if (!newAddress.generatedEntranceWeights)
			{
				foreach (NewNode.NodeAccess nodeAccess in newAddress.entrances)
				{
					nodeAccess.PreComputeEntranceWeights();
				}
				newAddress.generatedEntranceWeights = true;
				if (Game.Instance.useJobSystem)
				{
					newAddress.GenerateJobPathingData();
				}
			}
		}
		foreach (Citizen citizen3 in CityData.Instance.citizenDirectory)
		{
			if (citizen3.job.employer != null && citizen3.job.preset.jobAIPosition == OccupationPreset.JobAI.workPosition && citizen3.job.preset.ownsWorkPosition && citizen3.workPosition == null)
			{
				int num8 = 0;
				List<string> list3 = new List<string>();
				foreach (NewRoom newRoom2 in citizen3.job.employer.address.rooms)
				{
					if (newRoom2.specialCaseInteractables.ContainsKey(citizen3.job.preset.jobPostion))
					{
						num8 += newRoom2.specialCaseInteractables[citizen3.job.preset.jobPostion].Count;
						using (List<Interactable>.Enumerator enumerator11 = newRoom2.specialCaseInteractables[citizen3.job.preset.jobPostion].GetEnumerator())
						{
							while (enumerator11.MoveNext())
							{
								Interactable d = enumerator11.Current;
								Occupation occupation2 = citizen3.job.employer.companyRoster.Find((Occupation item) => item.employee != null && item.employee.workPosition == d);
								if (occupation2 != null)
								{
									list3.Add(string.Concat(new string[]
									{
										d.preset.name,
										" ",
										occupation2.employee.GetCitizenName(),
										" (",
										occupation2.preset.name,
										")"
									}));
								}
								else if (d.belongsTo != null && d.belongsTo.job != null && d.belongsTo.job.employer != null)
								{
									List<string> list4 = list3;
									string[] array = new string[8];
									array[0] = d.preset.name;
									array[1] = " Owned by ";
									int num9 = 2;
									Human belongsTo = d.belongsTo;
									array[num9] = ((belongsTo != null) ? belongsTo.ToString() : null);
									array[3] = " (";
									array[4] = d.belongsTo.job.preset.name;
									array[5] = " at ";
									array[6] = d.belongsTo.job.employer.name;
									array[7] = ")";
									list4.Add(string.Concat(array));
								}
							}
						}
					}
				}
				Game.Log(string.Concat(new string[]
				{
					"CityGen: ",
					citizen3.GetCitizenName(),
					"(",
					citizen3.job.preset.name,
					") doesn't have a desk at ",
					citizen3.job.employer.name,
					", ",
					citizen3.job.employer.address.building.name,
					" (",
					citizen3.job.employer.GetNumberOfFilledJobs().ToString(),
					" employees, ",
					num8.ToString(),
					" valid work positions). Removing from position..."
				}), 2);
				citizen3.SetJob(CitizenCreator.Instance.CreateUnemployed());
			}
		}
		foreach (KeyValuePair<int, NewDoor> keyValuePair2 in CityData.Instance.doorDictionary)
		{
			keyValuePair2.Value.PlaceKeys();
		}
		base.SetComplete();
		yield break;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00094AC5 File Offset: 0x00092CC5
	private IEnumerator ThreadedInteriorGeneration(InteriorCreator.LoaderThread loaderReference)
	{
		loaderReference.isDone = false;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (loaderReference.floor != null)
		{
			for (int i = 0; i < loaderReference.floor.addresses.Count; i++)
			{
				GenerationController.Instance.GenerateGeometry(loaderReference.floor.addresses[i]);
			}
		}
		else if (loaderReference.street != null)
		{
			foreach (NewRoom newRoom in loaderReference.street.rooms)
			{
				newRoom.UpdateWorldPositionAndBoundsSize();
				GenerationController.Instance.GenerateLightZones(newRoom);
			}
		}
		this.threads.Add(loaderReference);
		Game.Log("CityGen: Interior creator threads: " + this.threads.Count.ToString(), 2);
		Thread thread = new Thread(delegate()
		{
			if (loaderReference.floor != null)
			{
				for (int k = 0; k < loaderReference.floor.addresses.Count; k++)
				{
					GenerationController.Instance.GenerateAddressDecor(loaderReference.floor.addresses[k]);
				}
			}
			else if (loaderReference.street != null)
			{
				GenerationController.Instance.FurnishRoom(loaderReference.street.rooms[0]);
			}
			loaderReference.isDone = true;
		});
		thread.Start();
		while (!loaderReference.isDone || thread.IsAlive)
		{
			yield return null;
		}
		List<NewGameLocation> list = new List<NewGameLocation>();
		if (loaderReference.street != null)
		{
			list.Add(loaderReference.street);
		}
		else if (loaderReference.floor != null)
		{
			list.AddRange(loaderReference.floor.addresses);
		}
		foreach (NewGameLocation newGameLocation in list)
		{
			foreach (NewRoom newRoom2 in newGameLocation.rooms)
			{
				for (int j = 0; j < newRoom2.lightZones.Count; j++)
				{
					NewRoom.LightZoneData lightZoneData = newRoom2.lightZones[j];
					if (lightZoneData.nodeList == null || lightZoneData.nodeList.Count <= 0)
					{
						newRoom2.lightZones.Remove(lightZoneData);
						j--;
					}
					else
					{
						bool flag = true;
						using (List<Interactable>.Enumerator enumerator3 = newRoom2.mainLights.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (Vector3.Distance(enumerator3.Current.node.position, lightZoneData.centreWorldPosition) < InteriorControls.Instance.lightZoneMinDistance)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag && newRoom2.preset.useMainLights)
						{
							lightZoneData.CreateMainLight();
						}
					}
				}
				foreach (FurnitureClusterLocation furnitureClusterLocation in newRoom2.furniture)
				{
					foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in furnitureClusterLocation.clusterObjectMap)
					{
						foreach (FurnitureLocation furnitureLocation in keyValuePair.Value)
						{
							if (furnitureLocation != null && !furnitureLocation.createdInteractables)
							{
								furnitureLocation.CreateInteractables();
							}
						}
					}
				}
			}
			newGameLocation.PlaceObjects();
		}
		Game.Log("CityGen: Interior creator threads: " + this.threads.Count.ToString(), 2);
		this.threads.Remove(loaderReference);
		yield break;
	}

	// Token: 0x040009EA RID: 2538
	public int loadChunk = 1;

	// Token: 0x040009EB RID: 2539
	public bool threadedInteriorCreationActive;

	// Token: 0x040009EC RID: 2540
	[NonSerialized]
	public List<InteriorCreator.LoaderThread> threads = new List<InteriorCreator.LoaderThread>();

	// Token: 0x040009ED RID: 2541
	private static InteriorCreator _instance;

	// Token: 0x0200017A RID: 378
	public class LoaderThread
	{
		// Token: 0x040009EE RID: 2542
		public Coroutine thread;

		// Token: 0x040009EF RID: 2543
		public StreetController street;

		// Token: 0x040009F0 RID: 2544
		public NewFloor floor;

		// Token: 0x040009F1 RID: 2545
		public bool isDone;
	}
}
