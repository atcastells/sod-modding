using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004A4 RID: 1188
public class Toolbox : MonoBehaviour
{
	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06001947 RID: 6471 RVA: 0x001744B7 File Offset: 0x001726B7
	// (set) Token: 0x06001948 RID: 6472 RVA: 0x001744BE File Offset: 0x001726BE
	public static GameObject PoolingGroup { get; private set; }

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06001949 RID: 6473 RVA: 0x001744C6 File Offset: 0x001726C6
	public static Toolbox Instance
	{
		get
		{
			return Toolbox._instance;
		}
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x001744D0 File Offset: 0x001726D0
	private void Awake()
	{
		if (Toolbox._instance != null && Toolbox._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Toolbox._instance = this;
		}
		this.allShoeTypes = Enumerable.ToList<Human.ShoeType>(Enumerable.Cast<Human.ShoeType>(Enum.GetValues(typeof(Human.ShoeType))));
		this.allDataKeys = Enumerable.ToList<Evidence.DataKey>(Enumerable.Cast<Evidence.DataKey>(Enum.GetValues(typeof(Evidence.DataKey))));
		this.allCompanyCategories = Enumerable.ToList<CompanyPreset.CompanyCategory>(Enumerable.Cast<CompanyPreset.CompanyCategory>(Enum.GetValues(typeof(CompanyPreset.CompanyCategory))));
		this.allEthnicities = Enumerable.ToList<Descriptors.EthnicGroup>(Enumerable.Cast<Descriptors.EthnicGroup>(Enum.GetValues(typeof(Descriptors.EthnicGroup))));
		this.allConnectionTypes = Enumerable.ToList<Acquaintance.ConnectionType>(Enumerable.Cast<Acquaintance.ConnectionType>(Enum.GetValues(typeof(Acquaintance.ConnectionType))));
		this.allTreeTriggers = Enumerable.ToList<DDSSaveClasses.TreeTriggers>(Enumerable.Cast<DDSSaveClasses.TreeTriggers>(Enum.GetValues(typeof(DDSSaveClasses.TreeTriggers))));
		this.allOutfitCategories = Enumerable.ToList<ClothesPreset.OutfitCategory>(Enumerable.Cast<ClothesPreset.OutfitCategory>(Enum.GetValues(typeof(ClothesPreset.OutfitCategory))));
		this.allCharacterAnchors = Enumerable.ToList<CitizenOutfitController.CharacterAnchor>(Enumerable.Cast<CitizenOutfitController.CharacterAnchor>(Enum.GetValues(typeof(CitizenOutfitController.CharacterAnchor))));
		this.LoadAll();
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x00174608 File Offset: 0x00172808
	private void Start()
	{
		if (SocialStatistics.Instance != null)
		{
			SocialStatistics.Instance.ethnicityFrequencies.Sort();
			SocialStatistics.Instance.ethnicityFrequencies.Reverse();
			foreach (SocialStatistics.EthnicityFrequency ethnicityFrequency in SocialStatistics.Instance.ethnicityFrequencies)
			{
				this.totalEthnictiyFrequencyCount += ethnicityFrequency.frequency;
				for (int i = 0; i < ethnicityFrequency.frequency; i++)
				{
					this.rEthnicity.Add(ethnicityFrequency.ethnicity);
				}
			}
		}
		this.LoadDDS();
		this.aiSightingLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			15,
			18,
			23,
			25,
			5,
			31,
			26
		});
		this.interactionRayLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			12,
			15,
			18,
			20,
			5
		});
		this.interactionRayLayerMaskNoRoomMesh = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			12,
			15,
			18,
			20,
			5,
			29
		});
		this.printDetectionRayLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			15,
			18,
			20,
			31,
			5
		});
		this.sceneCaptureLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			5,
			7,
			15,
			17,
			18,
			20,
			21,
			22,
			24,
			30,
			31
		});
		this.mugShotCaptureLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			5,
			7,
			15,
			17,
			18,
			20,
			21,
			22,
			30,
			31
		});
		this.physicalObjectsLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			5,
			6,
			12,
			15,
			17,
			18,
			30,
			31
		});
		this.playerMovementLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			3,
			4,
			7,
			8,
			9,
			10,
			11,
			13,
			14,
			16,
			24,
			25,
			26,
			27,
			28,
			29
		});
		this.autoTravelMovementLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			3,
			4,
			7,
			8,
			9,
			10,
			11,
			13,
			14,
			16,
			24,
			25,
			26,
			27,
			28
		});
		this.spatterLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			4,
			7,
			28,
			29
		});
		this.textToImageMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			15
		});
		this.lightCullingMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			4,
			18,
			29,
			20
		});
		this.heldObjectsObjectsLayerMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			4,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			24,
			27,
			29
		});
		this.sniperLOSMask = this.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
		{
			0,
			4,
			7,
			24,
			27,
			29
		});
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x00174868 File Offset: 0x00172A68
	private void LoadDDS()
	{
		if (SessionData.Instance != null && !SessionData.Instance.isDialogEdit)
		{
			Game.Log("Menu: Loading DDS...", 2);
			this.LoadDDSFilesFromPath(Application.streamingAssetsPath + "/DDS");
			if (Game.Instance != null && Game.Instance.allowMods && ModLoader.Instance.modsLoaded)
			{
				for (int i = 0; i < ModLoader.Instance.sortedModsList.Count; i++)
				{
					ModSettingsData modSettingsData = ModLoader.Instance.sortedModsList[i];
					if (modSettingsData.modData.enabled)
					{
						string text = modSettingsData.modData.directory + "/StreamingAssets/DDS";
						if (Directory.Exists(text))
						{
							Debug.Log("Mods: Directory " + text + " exists within mod " + modSettingsData.modData.modProfile.name);
							foreach (DirectoryInfo directoryInfo in new DirectoryInfo(text).GetDirectories())
							{
								string text2 = directoryInfo.FullName + "/BundleSettings.txt";
								if (File.Exists(text2))
								{
									string text3 = string.Empty;
									using (StreamReader streamReader = File.OpenText(text2))
									{
										text3 = streamReader.ReadToEnd();
									}
									DDSBundle ddsbundle = JsonUtility.FromJson<DDSBundle>(text3);
									if (ddsbundle != null)
									{
										Game.Log(string.Concat(new string[]
										{
											"Mods: Successfully parsed a DDS bundle config: ",
											ddsbundle.displayName,
											" (",
											ddsbundle.languageCode,
											")"
										}), 2);
										ddsbundle.path = directoryInfo.FullName;
										this.LoadDDSFilesFromPath(directoryInfo.FullName);
										string text4 = directoryInfo.FullName + "/Strings";
										if (Directory.Exists(text4))
										{
											foreach (DirectoryInfo directoryInfo2 in new DirectoryInfo(text4).GetDirectories())
											{
												string text5 = directoryInfo2.FullName + "/dds.blocks.csv";
												if (File.Exists(text5))
												{
													Game.Log(string.Concat(new string[]
													{
														"Mods: Found a blocks file for ",
														ddsbundle.displayName,
														" (",
														directoryInfo2.Name,
														")"
													}), 2);
													if (directoryInfo2.Name.ToLower() == Game.Instance.language.ToLower())
													{
														Game.Log(string.Concat(new string[]
														{
															"Mods: dds.blocks with language ",
															directoryInfo2.Name,
															" matches game language ",
															Game.Instance.language,
															", loading to game..."
														}), 2);
														Strings.LoadLanguageFileToGame("dds.blocks", text5, false);
													}
													if (directoryInfo2.Name.ToLower() == "english" && Game.Instance.language.ToLower() != "english")
													{
														Strings.LoadLanguageFileToGame("dds.blocks", text5, true);
													}
												}
											}
										}
										else
										{
											Game.LogError("Cannot find a strings language for " + ddsbundle.displayName, 2);
										}
									}
								}
								else
								{
									Game.Log("Mods: No DDS bundle config exists at " + text2, 2);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x00174BD8 File Offset: 0x00172DD8
	public void LoadDDSFilesFromPath(string path)
	{
		for (int i = 0; i < 3; i++)
		{
			string text = "/Blocks";
			string text2 = ".block";
			if (i == 1)
			{
				text = "/Messages";
				text2 = ".msg";
			}
			else if (i == 2)
			{
				text = "/Trees";
				text2 = ".tree";
			}
			int num = 0;
			foreach (FileInfo fileInfo in Enumerable.ToList<FileInfo>(new DirectoryInfo(path + text).GetFiles("*" + text2, 0)))
			{
				using (StreamReader streamReader = File.OpenText(fileInfo.FullName))
				{
					string text3 = streamReader.ReadToEnd();
					if (i == 0)
					{
						DDSSaveClasses.DDSBlockSave ddsblockSave = JsonUtility.FromJson<DDSSaveClasses.DDSBlockSave>(text3);
						if (this.allDDSBlocks.ContainsKey(ddsblockSave.id))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Duplicate block exists: ",
								ddsblockSave.id,
								" Existing name: ",
								this.allDDSBlocks[ddsblockSave.id].name,
								", new block name: ",
								ddsblockSave.name
							}), 2);
						}
						else
						{
							this.allDDSBlocks.Add(ddsblockSave.id, ddsblockSave);
						}
						num++;
					}
					else if (i == 1)
					{
						DDSSaveClasses.DDSMessageSave ddsmessageSave = JsonUtility.FromJson<DDSSaveClasses.DDSMessageSave>(text3);
						if (this.allDDSMessages.ContainsKey(ddsmessageSave.id))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Duplicate message exists: ",
								ddsmessageSave.id,
								" Existing name: ",
								this.allDDSMessages[ddsmessageSave.id].name,
								", new message name: ",
								ddsmessageSave.name
							}), 2);
						}
						else
						{
							this.allDDSMessages.Add(ddsmessageSave.id, ddsmessageSave);
						}
						num++;
					}
					else if (i == 2)
					{
						DDSSaveClasses.DDSTreeSave ddstreeSave = JsonUtility.FromJson<DDSSaveClasses.DDSTreeSave>(text3);
						ddstreeSave.messageRef = new Dictionary<string, DDSSaveClasses.DDSMessageSettings>();
						foreach (DDSSaveClasses.DDSMessageSettings ddsmessageSettings in ddstreeSave.messages)
						{
							if (!ddstreeSave.messageRef.ContainsKey(ddsmessageSettings.instanceID))
							{
								ddstreeSave.messageRef.Add(ddsmessageSettings.instanceID, ddsmessageSettings);
							}
						}
						if (this.allDDSTrees.ContainsKey(ddstreeSave.id))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Duplicate tree exists: ",
								ddstreeSave.id,
								" Existing name: ",
								this.allDDSTrees[ddstreeSave.id].name,
								", new tree name: ",
								ddstreeSave.name
							}), 2);
						}
						else
						{
							this.allDDSTrees.Add(ddstreeSave.id, ddstreeSave);
						}
						if (ddstreeSave.treeType == DDSSaveClasses.TreeType.newspaper)
						{
							this.allArticleTrees.Add(ddstreeSave);
						}
						num++;
					}
				}
			}
			if (Game.Instance.devMode)
			{
				if (i == 0)
				{
					Game.Log("Menu: Successfully loaded " + num.ToString() + " blocks from " + path, 2);
				}
				else if (i == 1)
				{
					Game.Log("Menu: Successfully loaded " + num.ToString() + " messages from " + path, 2);
				}
				else if (i == 2)
				{
					Game.Log("Menu: Successfully loaded " + num.ToString() + " trees from " + path, 2);
				}
			}
		}
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x00174FA0 File Offset: 0x001731A0
	private void ProcessLoadedScriptableObject(ScriptableObject so)
	{
		Type type = so.GetType();
		string name = so.name;
		if (!this.resourcesCache.ContainsKey(type))
		{
			this.resourcesCache.Add(type, new Dictionary<string, ScriptableObject>());
		}
		ScriptableObjectIDSystem scriptableObjectIDSystem = so as ScriptableObjectIDSystem;
		if (scriptableObjectIDSystem != null)
		{
			this.resourcesCache[type].Add(scriptableObjectIDSystem.id, so);
		}
		else
		{
			this.resourcesCache[type].Add(name, so);
		}
		if (so is AddressPreset)
		{
			this.allAddressPresets.Add(so as AddressPreset);
			return;
		}
		if (so is RoomConfiguration)
		{
			this.allRoomConfigs.Add(so as RoomConfiguration);
			return;
		}
		if (so is InteractablePreset)
		{
			InteractablePreset interactablePreset = so as InteractablePreset;
			this.objectPresetDictionary.Add(so.name, interactablePreset);
			if (interactablePreset.weapon != null)
			{
				this.allWeapons.Add(interactablePreset);
			}
			foreach (SubObjectClassPreset subObjectClassPreset in interactablePreset.subObjectClasses)
			{
				if (subObjectClassPreset == null)
				{
					Game.LogError("Null subobject class detected in " + interactablePreset.name, 2);
				}
				if (!this.subObjectsDictionary.ContainsKey(subObjectClassPreset))
				{
					this.subObjectsDictionary.Add(subObjectClassPreset, new List<InteractablePreset>());
				}
				this.subObjectsDictionary[subObjectClassPreset].Add(interactablePreset);
			}
			if (interactablePreset.alwaysPlaceAtGameLocation && interactablePreset.autoPlacement != InteractablePreset.AutoPlacement.never)
			{
				this.placeAtGameLocationInteractables.Add(interactablePreset);
			}
			if (interactablePreset.placeIfFiltersPresentInOwner && interactablePreset.autoPlacement != InteractablePreset.AutoPlacement.never)
			{
				this.placePerOwnerInteractables.Add(interactablePreset);
				return;
			}
		}
		else
		{
			if (so is DesignStylePreset)
			{
				DesignStylePreset designStylePreset = so as DesignStylePreset;
				this.allDesignStyles.Add(designStylePreset);
				return;
			}
			if (so is FurnitureCluster)
			{
				FurnitureCluster furnitureCluster = so as FurnitureCluster;
				this.allFurnitureClusters.Add(furnitureCluster);
				return;
			}
			if (!(so is FurnitureClass))
			{
				if (so is WindowStylePreset)
				{
					WindowStylePreset windowStylePreset = so as WindowStylePreset;
					string text = windowStylePreset.name.ToLower();
					InterfaceController.Instance.windowDictionary.Add(text, windowStylePreset);
					return;
				}
				if (so is FurniturePreset)
				{
					FurniturePreset furniturePreset = so as FurniturePreset;
					this.allFurniture.Add(furniturePreset);
					if (furniturePreset.prefab != null)
					{
						MeshFilter component = furniturePreset.prefab.GetComponent<MeshFilter>();
						if (component != null && !this.furnitureMeshReference.ContainsKey(component.sharedMesh))
						{
							this.furnitureMeshReference.Add(component.sharedMesh, furniturePreset);
							return;
						}
					}
				}
				else
				{
					if (so is HelpContentPage)
					{
						this.allHelpContent.Add((so as HelpContentPage).name, so as HelpContentPage);
						return;
					}
					if (so is CompanyPreset)
					{
						this.allCompanyPresets.Add(so as CompanyPreset);
						return;
					}
					if (so is ColourSchemePreset)
					{
						this.allColourSchemes.Add(so as ColourSchemePreset);
						return;
					}
					if (so is AIGoalPreset)
					{
						this.allGoals.Add(so as AIGoalPreset);
						return;
					}
					if (so is DialogPreset)
					{
						this.allDialog.Add(so as DialogPreset);
						return;
					}
					if (so is EvidencePreset)
					{
						EvidencePreset evidencePreset = so as EvidencePreset;
						this.evidencePresetDictionary.Add(evidencePreset.name.ToLower(), evidencePreset);
						return;
					}
					if (so is JobPreset)
					{
						this.allSideJobs.Add(so as JobPreset);
						return;
					}
					if (so is DistrictPreset)
					{
						this.allDistricts.Add(so as DistrictPreset);
						return;
					}
					if (so is HandwritingPreset)
					{
						this.allHandwriting.Add(so as HandwritingPreset);
						return;
					}
					if (so is FactPreset)
					{
						FactPreset factPreset = so as FactPreset;
						this.factPresetDictionary.Add(factPreset.name.ToLower(), factPreset);
						return;
					}
					if (so is MurderPreset)
					{
						this.allMurderPresets.Add(so as MurderPreset);
						return;
					}
					if (so is MurderMO)
					{
						this.allMurderMOs.Add(so as MurderMO);
						return;
					}
					if (so is RetailItemPreset)
					{
						this.allItems.Add(so as RetailItemPreset);
						return;
					}
					if (so is CharacterTrait)
					{
						this.allCharacterTraits.Add(so as CharacterTrait);
						return;
					}
					if (so is BookPreset)
					{
						this.allBooks.Add(so as BookPreset);
						return;
					}
					if (so is ArtPreset)
					{
						ArtPreset artPreset = so as ArtPreset;
						this.allArt.Add(artPreset);
						return;
					}
					if (so is ClothesPreset)
					{
						ClothesPreset clothesPreset = so as ClothesPreset;
						this.allClothes.Add(clothesPreset);
						this.clothesDictionary.Add(clothesPreset.name, clothesPreset);
						return;
					}
					if (so is OccupationPreset)
					{
						OccupationPreset occupationPreset = so as OccupationPreset;
						this.allJobs.Add(occupationPreset);
						if (occupationPreset.isCriminal)
						{
							this.allCriminalJobs.Add(occupationPreset);
							return;
						}
					}
					else
					{
						if (so is RoomLightingPreset)
						{
							this.allRoomLighting.Add(so as RoomLightingPreset);
							return;
						}
						if (so is StreetTilePreset)
						{
							this.allStreetTiles.Add(so as StreetTilePreset);
							return;
						}
						if (so is MaterialGroupPreset)
						{
							MaterialGroupPreset materialGroupPreset = so as MaterialGroupPreset;
							if (!this.materialProperties.ContainsKey(materialGroupPreset.material))
							{
								this.materialProperties.Add(materialGroupPreset.material, materialGroupPreset);
							}
							this.allMaterialGroups.Add(materialGroupPreset);
							return;
						}
						if (so is CloudSaveData)
						{
							this.devCloudSaveConfig = (so as CloudSaveData);
							return;
						}
						if (so is StatusPreset)
						{
							this.allStatuses.Add(so as StatusPreset);
							return;
						}
						if (so is WallFrontagePreset)
						{
							WallFrontagePreset wallFrontagePreset = so as WallFrontagePreset;
							this.allWallFrontage.Add(wallFrontagePreset);
							return;
						}
						if (so is SyncDiskPreset)
						{
							this.allSyncDisks.Add(so as SyncDiskPreset);
							return;
						}
						if (so is BroadcastSchedule)
						{
							this.allBroadcasts.Add(so as BroadcastSchedule);
							return;
						}
						if (so is DDSScope)
						{
							DDSScope ddsscope = so as DDSScope;
							this.scopeDictionary.Add(ddsscope.name, ddsscope);
							if (ddsscope.isGlobal)
							{
								this.globalScopeDictionary.Add(ddsscope.name, ddsscope);
								return;
							}
						}
						else if (so is GroupPreset)
						{
							GroupPreset groupPreset = so as GroupPreset;
							this.allGroups.Add(groupPreset);
							this.groupsDictionary.Add(groupPreset.name, groupPreset);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x001755F4 File Offset: 0x001737F4
	private void LoadAll()
	{
		foreach (ScriptableObject so in AssetLoader.Instance.GetAllPresets())
		{
			this.ProcessLoadedScriptableObject(so);
		}
		this.allAmbientZones = AssetLoader.Instance.GetAllAmbientZones();
		foreach (CharacterTrait characterTrait in this.allCharacterTraits)
		{
			if (!characterTrait.isTrait)
			{
				this.reasons.Add(characterTrait);
			}
			else if (characterTrait.pickStage == 0)
			{
				this.stage0Traits.Add(characterTrait);
			}
			else if (characterTrait.pickStage == 1)
			{
				this.stage1Traits.Add(characterTrait);
			}
			else if (characterTrait.pickStage == 2)
			{
				this.stage2Traits.Add(characterTrait);
			}
			else
			{
				this.stage3Traits.Add(characterTrait);
			}
		}
		foreach (DialogPreset dialogPreset in this.allDialog)
		{
			if (dialogPreset.defaultOption)
			{
				this.defaultDialogOptions.Add(dialogPreset);
			}
		}
		foreach (FurniturePreset furniturePreset in this.allFurniture)
		{
			if (furniturePreset.prefab != null)
			{
				MeshFilter component = furniturePreset.prefab.GetComponent<MeshFilter>();
				if (component != null && !this.furnitureMeshReference.ContainsKey(component.sharedMesh))
				{
					this.furnitureMeshReference.Add(component.sharedMesh, furniturePreset);
				}
			}
			if (furniturePreset.universalDesignStyle)
			{
				using (List<DesignStylePreset>.Enumerator enumerator5 = this.allDesignStyles.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						DesignStylePreset designStylePreset = enumerator5.Current;
						if (designStylePreset == null)
						{
							Game.Log("Furniture design style blank for " + furniturePreset.name, 2);
						}
						else
						{
							if (!this.furnitureDesignStyleRef.ContainsKey(designStylePreset))
							{
								this.furnitureDesignStyleRef.Add(designStylePreset, new List<FurniturePreset>());
							}
							this.furnitureDesignStyleRef[designStylePreset].Add(furniturePreset);
						}
					}
					goto IL_2B2;
				}
				goto IL_228;
			}
			goto IL_228;
			IL_2B2:
			foreach (RoomTypeFilter roomTypeFilter in furniturePreset.allowedRoomFilters)
			{
				foreach (RoomClassPreset roomClassPreset in roomTypeFilter.roomClasses)
				{
					if (!this.furnitureRoomTypeRef.ContainsKey(roomClassPreset))
					{
						this.furnitureRoomTypeRef.Add(roomClassPreset, new HashSet<FurniturePreset>());
					}
					if (!this.furnitureRoomTypeRef[roomClassPreset].Contains(furniturePreset))
					{
						this.furnitureRoomTypeRef[roomClassPreset].Add(furniturePreset);
					}
				}
			}
			continue;
			IL_228:
			foreach (DesignStylePreset designStylePreset2 in furniturePreset.designStyles)
			{
				if (designStylePreset2 == null)
				{
					Game.Log("Furniture design style blank for " + furniturePreset.name, 2);
				}
				else
				{
					if (!this.furnitureDesignStyleRef.ContainsKey(designStylePreset2))
					{
						this.furnitureDesignStyleRef.Add(designStylePreset2, new List<FurniturePreset>());
					}
					this.furnitureDesignStyleRef[designStylePreset2].Add(furniturePreset);
				}
			}
			goto IL_2B2;
		}
		foreach (MaterialGroupPreset materialGroupPreset in this.allMaterialGroups)
		{
			foreach (MaterialGroupPreset.MaterialSettings materialSettings in materialGroupPreset.designStyles)
			{
				if (!this.materialDesignStyleRef.ContainsKey(materialSettings.designStyle))
				{
					this.materialDesignStyleRef.Add(materialSettings.designStyle, new Dictionary<MaterialGroupPreset.MaterialType, List<MaterialGroupPreset>>());
				}
				if (!this.materialDesignStyleRef[materialSettings.designStyle].ContainsKey(materialGroupPreset.materialType))
				{
					this.materialDesignStyleRef[materialSettings.designStyle].Add(materialGroupPreset.materialType, new List<MaterialGroupPreset>());
				}
				this.materialDesignStyleRef[materialSettings.designStyle][materialGroupPreset.materialType].Add(materialGroupPreset);
			}
		}
		foreach (WallFrontagePreset wallFrontagePreset in this.allWallFrontage)
		{
			if (wallFrontagePreset.universalDesignStyle)
			{
				using (List<DesignStylePreset>.Enumerator enumerator5 = this.allDesignStyles.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						DesignStylePreset designStylePreset3 = enumerator5.Current;
						if (!this.wallFrontageStyleRef.ContainsKey(designStylePreset3))
						{
							this.wallFrontageStyleRef.Add(designStylePreset3, new Dictionary<WallFrontageClass, List<WallFrontagePreset>>());
						}
						foreach (WallFrontageClass wallFrontageClass in wallFrontagePreset.classes)
						{
							if (!this.wallFrontageStyleRef[designStylePreset3].ContainsKey(wallFrontageClass))
							{
								this.wallFrontageStyleRef[designStylePreset3].Add(wallFrontageClass, new List<WallFrontagePreset>());
							}
							this.wallFrontageStyleRef[designStylePreset3][wallFrontageClass].Add(wallFrontagePreset);
						}
					}
					continue;
				}
			}
			foreach (DesignStylePreset designStylePreset4 in wallFrontagePreset.designStyles)
			{
				if (!this.wallFrontageStyleRef.ContainsKey(designStylePreset4))
				{
					this.wallFrontageStyleRef.Add(designStylePreset4, new Dictionary<WallFrontageClass, List<WallFrontagePreset>>());
				}
				foreach (WallFrontageClass wallFrontageClass2 in wallFrontagePreset.classes)
				{
					if (!this.wallFrontageStyleRef[designStylePreset4].ContainsKey(wallFrontageClass2))
					{
						this.wallFrontageStyleRef[designStylePreset4].Add(wallFrontageClass2, new List<WallFrontagePreset>());
					}
					this.wallFrontageStyleRef[designStylePreset4][wallFrontageClass2].Add(wallFrontagePreset);
				}
			}
		}
		FootprintController.InitialisePool();
		SpatterSimulation.DecalSpawnData.InitialisePool();
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x00175DE8 File Offset: 0x00173FE8
	public float RoundToPlaces(float input, int decimals)
	{
		int num = 1;
		for (int i = 0; i < decimals; i++)
		{
			num *= 10;
		}
		return (float)Mathf.RoundToInt(input * (float)num) / (float)num;
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x00175E18 File Offset: 0x00174018
	public string AddZeros(float num, int decimals)
	{
		string text = num.ToString();
		string[] array = text.Split('.', 0);
		if (array.Length > 1)
		{
			int num2 = decimals - array[1].Length;
			for (int i = 0; i < num2; i++)
			{
				text += "0";
			}
		}
		else
		{
			text += ".";
			for (int j = 0; j < decimals; j++)
			{
				text += "0";
			}
		}
		return text;
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x00175E8C File Offset: 0x0017408C
	public float RoundToPlaces(double input, int decimals)
	{
		float num = (float)input;
		int num2 = 100;
		if (decimals == 1)
		{
			num2 = 10;
		}
		else if (decimals == 2)
		{
			num2 = 100;
		}
		else if (decimals == 3)
		{
			num2 = 1000;
		}
		else if (decimals == 4)
		{
			num2 = 10000;
		}
		return (float)Mathf.RoundToInt(num * (float)num2) / (float)num2;
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x00175ED4 File Offset: 0x001740D4
	public float TravelTimeEstimate(Human cc, NewNode origin, NewNode destination)
	{
		if (origin == null)
		{
			Game.LogError("Travel time estimate: Origin  is null!", 2);
			return 0f;
		}
		if (destination == null)
		{
			Game.LogError("Travel time estimate: Destination is null!", 2);
			return 0f;
		}
		if (origin == destination)
		{
			return 0f;
		}
		float num = CitizenControls.Instance.baseCitizenWalkSpeed;
		if (cc != null)
		{
			num = cc.speedMultiplier * CitizenControls.Instance.baseCitizenWalkSpeed;
		}
		Vector2Int vector2Int = new Vector2Int(origin.tile.globalTileCoord.x, origin.tile.globalTileCoord.y);
		Vector2Int vector2Int2;
		vector2Int2..ctor(destination.tile.globalTileCoord.x, destination.tile.globalTileCoord.y);
		float num2 = Vector2.Distance(vector2Int, vector2Int2) * CityControls.Instance.travelTimeCrowFliesMultiplierEstimate;
		if (origin.building != null && origin.building == destination.building)
		{
			num2 += (float)Mathf.Abs(origin.floor.floor - destination.floor.floor);
			if (origin.floor != destination.floor)
			{
				num2 += 2f;
			}
		}
		else
		{
			if (origin.building != null)
			{
				num2 += 2f;
			}
			if (destination.building != null)
			{
				num2 += 2f;
			}
			num2 += (float)(origin.tile.globalTileCoord.z + destination.tile.globalTileCoord.z);
		}
		return Mathf.Max(num2 * PathFinder.Instance.tileSize.x / num * 0.01f * CityControls.Instance.travelTimeMultiplier + 0.0167f, 0f);
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x00176084 File Offset: 0x00174284
	public void AddToTravelTimeRecords(Actor cc, float discrepency)
	{
		if (Game.Instance.collectRoutineTimingInfo)
		{
			Game.Instance.AddOnTimeEntry(cc, discrepency);
		}
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x0017609E File Offset: 0x0017429E
	public int TravelTimeEstimateMinutes(Citizen cc, NewNode origin, NewNode destination)
	{
		return Mathf.RoundToInt(this.TravelTimeEstimate(cc, origin, destination) * 60f);
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x001760B4 File Offset: 0x001742B4
	public float RandomRangeWeighted(float minimum, float maximum, float weightedValue, int stepResolution = 5)
	{
		int num = this.Rand(0, stepResolution + 1, false);
		return this.Rand(Mathf.Lerp(minimum, weightedValue, (float)num / (float)stepResolution), Mathf.Lerp(maximum, weightedValue, (float)num / (float)stepResolution), false);
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x001760F0 File Offset: 0x001742F0
	public float RandomRangeWeightedSeedContained(float minimum, float maximum, float weightedValue, string input, out string output, int stepResolution = 5)
	{
		int psuedoRandomNumberContained = Toolbox.Instance.GetPsuedoRandomNumberContained(0, stepResolution + 1, input, out output);
		return Toolbox.Instance.GetPsuedoRandomNumberContained(Mathf.Lerp(minimum, weightedValue, (float)psuedoRandomNumberContained / (float)stepResolution), Mathf.Lerp(maximum, weightedValue, (float)psuedoRandomNumberContained / (float)stepResolution), output, out output);
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x0017613C File Offset: 0x0017433C
	public float MinDistanceFromPath(NewNode pathOrigin, NewNode pathDestination, Vector3 inputPosition)
	{
		if (pathOrigin == null || pathDestination == null)
		{
			return 9999f;
		}
		PathFinder.PathData path = PathFinder.Instance.GetPath(pathOrigin, pathDestination, null, null);
		float num = Vector2.Distance(pathOrigin.position, inputPosition);
		if (path.accessList != null)
		{
			for (int i = 0; i < Mathf.Max(path.accessList.Count - 1, 0); i++)
			{
				Vector3 position = path.accessList[i].fromNode.position;
				Vector3 position2 = path.accessList[i + 1].fromNode.position;
				Vector3 vector = position;
				float num2 = Mathf.Abs(position2.x - position.x);
				float num3 = Mathf.Abs(position2.z - position.z);
				if (num2 >= num3)
				{
					int num4 = Mathf.RoundToInt(Mathf.Clamp(inputPosition.x, Mathf.Min(position.x, position2.x), Mathf.Max(position.x, position2.x)));
					vector..ctor((float)num4, 0f, (position.z + position2.z) / 2f);
				}
				else
				{
					int num5 = Mathf.RoundToInt(Mathf.Clamp(inputPosition.z, Mathf.Min(position.z, position2.z), Mathf.Max(position.z, position2.z)));
					vector..ctor((position.x + position2.x) / 2f, 0f, (float)num5);
				}
				float num6 = Vector3.Distance(vector, inputPosition);
				if (num6 < num)
				{
					num = num6;
				}
			}
		}
		return num;
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x001762D4 File Offset: 0x001744D4
	public Rect RectTransformToScreenSpace(RectTransform transform)
	{
		Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Rect result;
		result..ctor(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
		result.x -= transform.pivot.x * vector.x;
		result.y -= (1f - transform.pivot.y) * vector.y;
		result.y = (float)Screen.height - result.y;
		return result;
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x0017638C File Offset: 0x0017458C
	public void InvokeEndOfFrame(Action action, string newDebug)
	{
		if (action == null)
		{
			return;
		}
		if (this.invokeEndOfFrame.Contains(action))
		{
			return;
		}
		this.invokeEndOfFrame.Add(action);
		this.debugInvokeEndOfFrame.Add(newDebug);
		if (!this.endOfFrameInvoke)
		{
			this.endOfFrameInvoke = true;
			base.StartCoroutine(this.ExeEndOfFrame());
		}
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x001763E1 File Offset: 0x001745E1
	private IEnumerator ExeEndOfFrame()
	{
		bool wait = true;
		while (wait)
		{
			wait = false;
			yield return new WaitForEndOfFrame();
		}
		while (this.invokeEndOfFrame.Count > 0)
		{
			Action action = Enumerable.First<Action>(this.invokeEndOfFrame);
			action.Invoke();
			this.invokeEndOfFrame.Remove(action);
			this.debugInvokeEndOfFrame.RemoveAt(0);
		}
		this.invokeEndOfFrame.Clear();
		this.debugInvokeEndOfFrame.Clear();
		this.endOfFrameInvoke = false;
		yield break;
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x001763F0 File Offset: 0x001745F0
	public void UpdateButtonListPositions(List<ButtonController> buttons, float edgeMargin = 5f, float iconMargin = 4f)
	{
		if (buttons.Count <= 0)
		{
			return;
		}
		RectTransform component = buttons[0].transform.parent.GetComponent<RectTransform>();
		float num = edgeMargin;
		float num2 = -edgeMargin;
		Vector2 vector;
		vector..ctor(component.rect.width, 1f);
		for (int i = 0; i < buttons.Count; i++)
		{
			RectTransform component2 = buttons[i].gameObject.GetComponent<RectTransform>();
			component2.anchoredPosition = new Vector2(num, num2);
			num += component2.sizeDelta.x + iconMargin;
			if (num > vector.x - component2.sizeDelta.x - edgeMargin)
			{
				num2 -= component2.sizeDelta.y + iconMargin;
				num = iconMargin;
			}
			vector.y = Mathf.Abs(num2 - component2.sizeDelta.y - edgeMargin);
		}
		component.sizeDelta = new Vector2(component.sizeDelta.x, vector.y);
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x001764F0 File Offset: 0x001746F0
	public bool GameTimeRangeOverlap(Vector2 range1, Vector2 range2, bool equalsIsOverlapping = true)
	{
		bool result;
		if (equalsIsOverlapping)
		{
			result = (range1.x <= range2.y && range2.x <= range1.y);
		}
		else
		{
			result = (range1.x < range2.y && range2.x < range1.y);
		}
		return result;
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x00176548 File Offset: 0x00174748
	public bool DecimalTimeRangeOverlap(Vector2 range1, Vector2 range2, bool equalsIsOverlapping = true)
	{
		if (range1.y < range1.x)
		{
			range1.y += 24f;
			range2.x += 24f;
			range2.y += 24f;
		}
		if (range2.y < range2.x)
		{
			range2.y += 24f;
			range1.x += 24f;
			range1.y += 24f;
		}
		bool result;
		if (equalsIsOverlapping)
		{
			result = (range1.x <= range2.y && range2.x <= range1.y);
		}
		else
		{
			result = (range1.x < range2.y && range2.x < range1.y);
		}
		return result;
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x0017661C File Offset: 0x0017481C
	public Vector2 RotateVector2ACW(Vector2 v, float degrees)
	{
		float num = Mathf.Sin(degrees * 0.017453292f);
		float num2 = Mathf.Cos(degrees * 0.017453292f);
		float x = v.x;
		float y = v.y;
		v.x = num2 * x - num * y;
		v.y = num * x + num2 * y;
		return v;
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x00176670 File Offset: 0x00174870
	public Vector2 RotateVector2CW(Vector2 v, float degrees)
	{
		float num = Mathf.Sin(-degrees * 0.017453292f);
		float num2 = Mathf.Cos(-degrees * 0.017453292f);
		float x = v.x;
		float y = v.y;
		v.x = num2 * x - num * y;
		v.y = num * x + num2 * y;
		return v;
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x001766C4 File Offset: 0x001748C4
	public Descriptors.EthnicGroup RandomEthnicGroup(string seed)
	{
		return this.rEthnicity[this.GetPsuedoRandomNumber(0, this.rEthnicity.Count, seed, false)];
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x001766E8 File Offset: 0x001748E8
	public Color GetRenderTexturePixel(RenderTexture rt)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		Texture2D texture2D = new Texture2D(rt.width, rt.height);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), 0, 0);
		RenderTexture.active = active;
		return texture2D.GetPixel(0, 0);
	}

	// Token: 0x06001963 RID: 6499 RVA: 0x00176744 File Offset: 0x00174944
	public void SetLightLayer(GameObject objectWithMesh, NewBuilding building, bool includeStreetLighting = false)
	{
		foreach (MeshRenderer meshRend in objectWithMesh.GetComponentsInChildren<MeshRenderer>())
		{
			this.SetLightLayer(meshRend, building, includeStreetLighting);
		}
	}

	// Token: 0x06001964 RID: 6500 RVA: 0x00176774 File Offset: 0x00174974
	public void SetLightLayer(MeshRenderer meshRend, NewBuilding building, bool includeStreetLighting = false)
	{
		if (meshRend == null)
		{
			return;
		}
		int num = 0;
		if (includeStreetLighting)
		{
			num = 2;
		}
		if (building != null)
		{
			if (building.interiorLightCullingLayer == 0)
			{
				meshRend.renderingLayerMask = (uint)(5 + num);
				return;
			}
			if (building.interiorLightCullingLayer == 1)
			{
				meshRend.renderingLayerMask = (uint)(9 + num);
				return;
			}
			if (building.interiorLightCullingLayer == 2)
			{
				meshRend.renderingLayerMask = (uint)(17 + num);
				return;
			}
			if (building.interiorLightCullingLayer == 3)
			{
				meshRend.renderingLayerMask = (uint)(33 + num);
				return;
			}
		}
		else
		{
			num = 2;
			meshRend.renderingLayerMask = (uint)(num + 1);
		}
	}

	// Token: 0x06001965 RID: 6501 RVA: 0x001767F4 File Offset: 0x001749F4
	public bool LoadDataFromResources<T>(string name, out T output) where T : ScriptableObject
	{
		output = default(T);
		Type typeFromHandle = typeof(T);
		Dictionary<string, ScriptableObject> dictionary;
		if (this.resourcesCache.TryGetValue(typeFromHandle, ref dictionary))
		{
			ScriptableObject scriptableObject;
			if (dictionary.TryGetValue(name, ref scriptableObject))
			{
				output = (scriptableObject as T);
				return true;
			}
			string[] array = new string[5];
			array[0] = "Resources load error: Could not find any files of type ";
			int num = 1;
			Type type = typeFromHandle;
			array[num] = ((type != null) ? type.ToString() : null);
			array[2] = " with name or ID ";
			array[3] = name.ToLower();
			array[4] = ", scaning for name... (this could take a while)";
			Game.LogError(string.Concat(array), 2);
			using (Dictionary<string, ScriptableObject>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ScriptableObject> keyValuePair = enumerator.Current;
					if (keyValuePair.Value.name == name)
					{
						Game.Log("Resources load successful using name system. This should be re-saved for more efficiency.", 2);
						output = (keyValuePair.Value as T);
						return true;
					}
				}
				return false;
			}
		}
		string text = "Resources load error: Could not find any files of type ";
		Type type2 = typeFromHandle;
		Game.LogError(text + ((type2 != null) ? type2.ToString() : null), 2);
		return false;
	}

	// Token: 0x06001966 RID: 6502 RVA: 0x00176920 File Offset: 0x00174B20
	public List<T> GetList<T>(params T[] elements)
	{
		return new List<T>(elements);
	}

	// Token: 0x06001967 RID: 6503 RVA: 0x00176928 File Offset: 0x00174B28
	public float HeuristicCostEstimate(NewNode start, NewNode goal)
	{
		return Vector2Int.Distance(start.floorCoord, goal.floorCoord);
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x0017693C File Offset: 0x00174B3C
	public List<NewNode> ConstructPathAccurate(Dictionary<NewNode, NewNode> cameFrom, NewNode current)
	{
		List<NewNode> list = new List<NewNode>();
		list.Add(current);
		while (cameFrom.ContainsKey(current))
		{
			NewNode newNode = current;
			current = cameFrom[current];
			if (current == null)
			{
				Game.Log("Pathfinder: Current is null, invalid path", 2);
				break;
			}
			if (current != newNode)
			{
				list.Add(current);
			}
		}
		list.Reverse();
		return list;
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x00176994 File Offset: 0x00174B94
	public Evidence GetOrCreateEvidenceForInteractable(InteractablePreset preset, string newID, Interactable interactable, Human belongsTo, Human writer, Human reciever, SideJob jobParent, NewGameLocation gameLocation, RetailItemPreset retailItem, List<Interactable.Passed> passedVars)
	{
		Evidence evidence = null;
		if (GameplayController.Instance.evidenceDictionary.TryGetValue(newID, ref evidence))
		{
			return evidence;
		}
		if (gameLocation == null && interactable != null && interactable.node != null)
		{
			gameLocation = interactable.node.gameLocation;
		}
		if (preset.useEvidence)
		{
			if (preset.useSingleton != null)
			{
				if (!SessionData.Instance.isFloorEdit)
				{
					evidence = GameplayController.Instance.singletonEvidence.Find((Evidence item) => item.preset == preset.useSingleton);
				}
			}
			else if (preset.findEvidence != InteractablePreset.FindEvidence.none)
			{
				if (preset.findEvidence == InteractablePreset.FindEvidence.addressBook)
				{
					if (belongsTo == null)
					{
						Game.LogError("Address book created without 'Belongs To' reference", 2);
					}
					evidence = belongsTo.addressBook;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.telephone)
				{
					evidence = CityData.Instance.telephone;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.hospitalBed)
				{
					evidence = CityData.Instance.hospitalBed;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.companyRoster)
				{
					evidence = gameLocation.thisAsAddress.company.employeeRoster;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.residentRoster)
				{
					evidence = gameLocation.building.residentRoster;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.salesRecords)
				{
					evidence = gameLocation.thisAsAddress.company.salesRecords;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.menu)
				{
					evidence = gameLocation.thisAsAddress.company.menu;
				}
				else if (preset.findEvidence == InteractablePreset.FindEvidence.workID)
				{
					evidence = belongsTo.workID;
				}
				else if (preset.findEvidence != InteractablePreset.FindEvidence.IDCard && preset.findEvidence == InteractablePreset.FindEvidence.calendar)
				{
					if (gameLocation == null)
					{
						Game.LogError("Unable to get calendar as gamelocation is null!", 2);
					}
					else
					{
						if (gameLocation != null && gameLocation.thisAsAddress != null)
						{
							if (gameLocation.thisAsAddress.calendar != null)
							{
								evidence = gameLocation.thisAsAddress.calendar;
							}
							else
							{
								evidence = gameLocation.thisAsAddress.CreateCalendar();
							}
						}
						if (evidence == null & gameLocation != null)
						{
							Game.LogError("Unable to get calendar for " + gameLocation.name, 2);
						}
					}
				}
			}
			else
			{
				List<object> list = new List<object>();
				if (passedVars != null)
				{
					foreach (Interactable.Passed passed in passedVars)
					{
						list.Add(passed);
					}
				}
				if (interactable != null)
				{
					list.Add(interactable);
				}
				if (preset != null)
				{
					list.Add(preset);
				}
				if (retailItem != null)
				{
					list.Add(retailItem);
				}
				Evidence newParent = null;
				if (preset.locationIsParent && gameLocation != null)
				{
					newParent = gameLocation.evidenceEntry;
				}
				EvidenceCreator instance = EvidenceCreator.Instance;
				EvidencePreset spawnEvidence = preset.spawnEvidence;
				List<object> passedObjects = list;
				evidence = instance.CreateEvidence(spawnEvidence, newID, belongsTo, belongsTo, writer, reciever, newParent, false, passedObjects);
			}
		}
		return evidence;
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x00176CE4 File Offset: 0x00174EE4
	public bool TryGetEvidence(string evID, out Evidence evidence)
	{
		evidence = null;
		if (GameplayController.Instance.evidenceDictionary.TryGetValue(evID, ref evidence))
		{
			return true;
		}
		if (evID.Length > 4)
		{
			string text = evID.Substring(0, 4);
			string text2 = string.Empty;
			if (evID.Length > 10)
			{
				text2 = evID.Substring(0, 10);
			}
			if (text == "Date")
			{
				string date = evID.Substring(4, evID.Length - 4);
				evidence = EvidenceCreator.Instance.GetDateEvidence(date, "date", "", -1, -1, -1);
			}
			else if (text2 == "StickyNote")
			{
				int assignStickyNoteID = InterfaceController.assignStickyNoteID;
				int.TryParse(evID.Substring(10, evID.Length - 10), ref assignStickyNoteID);
				evidence = (EvidenceCreator.Instance.CreateEvidence("PlayerStickyNote", "StickyNote" + assignStickyNoteID.ToString(), null, null, null, null, null, true, null) as EvidenceStickyNote);
			}
			else
			{
				evidence = EvidenceCreator.Instance.GetTimeEvidence(evID);
				if (evidence == null)
				{
					string text3 = string.Empty;
					if (evID.Length > 1)
					{
						text3 = evID.Substring(0, 1);
					}
					if (text3 == "I")
					{
						string text4 = evID.Substring(1, evID.Length - 1);
						int iID = -1;
						int.TryParse(text4, ref iID);
						if (iID > -1)
						{
							Interactable interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == iID);
							if (interactable != null)
							{
								RetailItemPreset retailItem = null;
								if (interactable.preset != null)
								{
									retailItem = interactable.preset.retailItem;
								}
								NewGameLocation gameLocation = null;
								if (interactable.node != null)
								{
									gameLocation = interactable.node.gameLocation;
								}
								evidence = Toolbox.Instance.GetOrCreateEvidenceForInteractable(interactable.preset, evID, interactable, interactable.belongsTo, interactable.writer, interactable.reciever, interactable.jobParent, gameLocation, retailItem, interactable.pv);
								return true;
							}
							MetaObject metaObject = CityData.Instance.FindMetaObject(iID);
							if (metaObject != null)
							{
								evidence = metaObject.GetEvidence(false, default(Vector3Int));
								return true;
							}
							Game.LogError("Unable to find interactable " + iID.ToString() + " loaded interactables: " + CityData.Instance.interactableDirectory.Count.ToString(), 2);
						}
						else
						{
							Game.LogError("Unable to parse valid interactable ID from " + evID, 2);
						}
					}
					Game.LogError("Unable to find evidence for ID " + evID, 2);
				}
			}
		}
		return evidence != null;
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x00176F74 File Offset: 0x00175174
	public Interactable SpawnSpareKey(NewAddress ad, string loadGUID = null)
	{
		if (ad.hiddenSpareKey)
		{
			return null;
		}
		if (ad.inhabitants.Count <= 0)
		{
			return null;
		}
		List<NewRoom> list = new List<NewRoom>();
		List<NewRoom> list2 = new List<NewRoom>();
		List<NewNode> list3 = new List<NewNode>();
		List<FurnitureLocation> list4 = new List<FurnitureLocation>();
		List<FurniturePreset.SubObject> list5 = new List<FurniturePreset.SubObject>();
		List<NewRoom> list6 = new List<NewRoom>();
		foreach (NewNode.NodeAccess nodeAccess in ad.entrances)
		{
			if (nodeAccess.wall != null)
			{
				if (nodeAccess.wall.node.gameLocation != ad && nodeAccess.wall.node.gameLocation.isLobby)
				{
					if (!list.Contains(nodeAccess.wall.node.room))
					{
						list.Add(nodeAccess.wall.node.room);
						list2.Add(nodeAccess.wall.otherWall.node.room);
						list3.Add(nodeAccess.wall.node);
					}
				}
				else if (nodeAccess.wall.otherWall.node.gameLocation != ad && nodeAccess.wall.otherWall.node.gameLocation.isLobby && !list.Contains(nodeAccess.wall.otherWall.node.room))
				{
					list.Add(nodeAccess.wall.otherWall.node.room);
					list2.Add(nodeAccess.wall.node.room);
					list3.Add(nodeAccess.wall.otherWall.node);
				}
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			NewRoom newRoom = list[i];
			NewRoom newRoom2 = list2[i];
			NewNode newNode = list3[i];
			int j = 0;
			while (j < newRoom.individualFurniture.Count)
			{
				FurnitureLocation furnitureLocation = null;
				try
				{
					furnitureLocation = newRoom.individualFurniture[j];
				}
				catch
				{
					goto IL_316;
				}
				goto IL_209;
				IL_316:
				j++;
				continue;
				IL_209:
				if (furnitureLocation == null)
				{
					goto IL_316;
				}
				if (furnitureLocation.furniture.subObjects.Exists((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.keyHidingPlace) && Vector3.Distance(furnitureLocation.anchorNode.nodeCoord, newNode.nodeCoord) <= 2.1f)
				{
					List<FurniturePreset.SubObject> list7 = new List<FurniturePreset.SubObject>();
					list7.AddRange(furnitureLocation.furniture.subObjects.FindAll((FurniturePreset.SubObject item) => item.preset == InteriorControls.Instance.keyHidingPlace));
					for (int k = 0; k < list7.Count; k++)
					{
						FurniturePreset.SubObject ownCheck = list7[k];
						if (furnitureLocation.integratedInteractables.Find((Interactable item) => item.subObject == ownCheck) == null)
						{
							list5.Add(ownCheck);
							list4.Add(furnitureLocation);
							list6.Add(newRoom2);
						}
					}
					goto IL_316;
				}
				goto IL_316;
			}
		}
		if (list5.Count > 0)
		{
			int psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0, list5.Count, ad.seed, false);
			FurniturePreset.SubObject pickedPos = list5[psuedoRandomNumber];
			FurnitureLocation furnitureLocation2 = list4[psuedoRandomNumber];
			NewRoom newRoom3 = list6[psuedoRandomNumber];
			furnitureLocation2.furniture.subObjects.FindIndex((FurniturePreset.SubObject item) => item == pickedPos);
			List<Interactable.Passed> list8 = new List<Interactable.Passed>();
			list8.Add(new Interactable.Passed(Interactable.PassedVarType.roomID, (float)newRoom3.roomID, null));
			Interactable result = InteractableCreator.Instance.CreateFurnitureSpawnedInteractableThreadSafe(InteriorControls.Instance.key, furnitureLocation2.anchorNode.room, furnitureLocation2, pickedPos, ad.inhabitants[0], ad.inhabitants[0], null, list8, null, null, "");
			ad.hiddenSpareKey = true;
			return result;
		}
		return null;
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x001773DC File Offset: 0x001755DC
	public float GetAngleForOffset(Vector2 offset1)
	{
		float num;
		for (num = Mathf.DeltaAngle(Mathf.Atan2(offset1.y, offset1.x) * 57.29578f, Mathf.Atan2(0f, 0f) * 57.29578f) - 90f; num < 0f; num += 360f)
		{
		}
		return num;
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x00177434 File Offset: 0x00175634
	public Vector2 GetOffsetFromAngle(int angle)
	{
		int num = Mathf.RoundToInt((float)angle / 45f) * 45;
		Vector2 result;
		result..ctor(0f, 1f);
		if (num == 45)
		{
			result..ctor(1f, 1f);
		}
		else if (num == 90)
		{
			result..ctor(1f, 0f);
		}
		else if (num == 135)
		{
			result..ctor(1f, -1f);
		}
		else if (num == 180)
		{
			result..ctor(0f, -1f);
		}
		else if (num == 225)
		{
			result..ctor(-1f, -1f);
		}
		else if (num == 270)
		{
			result..ctor(-1f, 0f);
		}
		else if (num == 315)
		{
			result..ctor(-1f, 1f);
		}
		return result;
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x00177520 File Offset: 0x00175720
	public float GetAngleBetween(Vector3 origin, Vector3 lookAt)
	{
		return (Quaternion.LookRotation(origin - lookAt, Vector3.up).eulerAngles.y + 180f) % 360f;
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x00177558 File Offset: 0x00175758
	private Vector3 GetAveragePosition(List<NewNode> nodes)
	{
		Vector3 vector = Vector3.zero;
		foreach (NewNode newNode in nodes)
		{
			vector += newNode.position;
		}
		return vector / (float)nodes.Count;
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x001775C0 File Offset: 0x001757C0
	public bool IsWorkDay(int day, Citizen cit)
	{
		if (cit.job == null)
		{
			return false;
		}
		if (cit.job.employer == null)
		{
			return false;
		}
		bool result = false;
		while (day >= 7)
		{
			day -= 7;
		}
		day = Mathf.Clamp(day, 0, 6);
		if (day == 0 && cit.job.shift.monday)
		{
			result = true;
		}
		else if (day == 1 && cit.job.shift.tuesday)
		{
			result = true;
		}
		else if (day == 2 && cit.job.shift.wednesday)
		{
			result = true;
		}
		else if (day == 3 && cit.job.shift.thursday)
		{
			result = true;
		}
		else if (day == 4 && cit.job.shift.friday)
		{
			result = true;
		}
		else if (day == 5 && cit.job.shift.saturday)
		{
			result = true;
		}
		else if (day == 6 && cit.job.shift.sunday)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x001776B8 File Offset: 0x001758B8
	public Interactable FindNearestWithAction(AIActionPreset action, NewRoom startRoom, Human person, AIActionPreset.FindSetting findSetting, bool overrideWithHome = true, HashSet<NewRoom> ignore = null, NewGameLocation restrictTo = null, NewBuilding restrictToBuilding = null, bool useSpecialCasesOnly = false, InteractablePreset.SpecialCase mustBeSpecial = InteractablePreset.SpecialCase.none, bool filterWithRoomType = false, List<RoomTypePreset> roomTypeFilter = null, bool preferUnused = true, bool enforcersAllowedEverywhere = false, float robberyPriority = 0f, List<Interactable> avoidInteractables = null, List<InteractablePreset> shopItems = null, bool printDebug = false, bool mustContainDesireCategory = false, CompanyPreset.CompanyCategory containDesireCategory = CompanyPreset.CompanyCategory.meal, bool excludeAIUsingThis = false)
	{
		HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
		HashSet<NewRoom> hashSet2 = new HashSet<NewRoom>();
		hashSet.Add(startRoom);
		int num = 1000;
		if (printDebug)
		{
			Game.Log("FindNearestWithAction: " + action.name + " Starting with room " + startRoom.name, 2);
		}
		if (Game.Instance.collectDebugData && person != null)
		{
			person.SelectedDebug(string.Concat(new string[]
			{
				"Finding nearest ",
				action.presetName,
				" starting ",
				startRoom.GetName(),
				" findSetting: ",
				findSetting.ToString(),
				" overrideW/Home: ",
				overrideWithHome.ToString()
			}), Actor.HumanDebug.actions);
		}
		bool flag = false;
		bool flag2 = true;
		bool flag3 = false;
		if (restrictToBuilding != null && startRoom.gameLocation.building != restrictToBuilding)
		{
			flag2 = false;
		}
		Predicate<Interactable> <>9__0;
		Predicate<InteractablePreset> <>9__1;
		while (hashSet.Count > 0 && num > 0)
		{
			NewRoom newRoom = Enumerable.First<NewRoom>(hashSet);
			if (newRoom != null)
			{
				if (!flag && restrictTo != null && restrictTo == newRoom.gameLocation)
				{
					flag = true;
					if (printDebug)
					{
						Game.Log("FindNearestWithAction: Reached resticted location", 2);
					}
				}
				if (!flag2 && restrictToBuilding != null && newRoom.gameLocation.building == restrictToBuilding)
				{
					flag2 = true;
					if (printDebug)
					{
						Game.Log("FindNearestWithAction: Reached resticted building", 2);
					}
				}
				if ((ignore == null || !ignore.Contains(newRoom)) && (restrictTo == null || newRoom.gameLocation == restrictTo) && (restrictToBuilding == null || newRoom.building == restrictToBuilding))
				{
					bool flag4 = false;
					if (!filterWithRoomType || roomTypeFilter.Contains(newRoom.roomType))
					{
						if (overrideWithHome && person.home != null && newRoom.gameLocation == person.home)
						{
							flag4 = true;
						}
						else if (findSetting == AIActionPreset.FindSetting.allAreas)
						{
							flag4 = true;
						}
						else if (findSetting == AIActionPreset.FindSetting.homeOnly)
						{
							if (newRoom.gameLocation == person.home && !person.home.isCrimeScene)
							{
								flag4 = true;
							}
						}
						else if (findSetting == AIActionPreset.FindSetting.workOnly)
						{
							if (person.job != null && person.job.employer != null && (newRoom.gameLocation == person.job.employer.placeOfBusiness || newRoom.gameLocation == person.job.employer.address))
							{
								flag4 = true;
							}
						}
						else if (findSetting == AIActionPreset.FindSetting.nonTrespassing)
						{
							int count;
							string text;
							if (!person.IsTrespassing(newRoom, out count, out text, enforcersAllowedEverywhere))
							{
								flag4 = true;
							}
						}
						else if (findSetting == AIActionPreset.FindSetting.onlyPublic)
						{
							bool forPlayer = false;
							if (person != null)
							{
								forPlayer = person.isPlayer;
							}
							flag4 = newRoom.gameLocation.IsPublicallyOpen(forPlayer);
						}
					}
					if (shopItems != null && flag4)
					{
						if (newRoom.gameLocation.thisAsAddress != null && newRoom.gameLocation.thisAsAddress.company != null)
						{
							bool flag5 = true;
							foreach (InteractablePreset interactablePreset in shopItems)
							{
								if (!newRoom.gameLocation.thisAsAddress.company.prices.ContainsKey(interactablePreset))
								{
									flag5 = false;
									break;
								}
							}
							if (!flag5)
							{
								flag4 = false;
							}
						}
						else
						{
							flag4 = false;
						}
					}
					if (!flag2 && restrictToBuilding != null && flag4)
					{
						if (flag3)
						{
							if (newRoom.building != null && newRoom.building != restrictToBuilding)
							{
								flag4 = false;
							}
						}
						else if (newRoom.gameLocation.building == null)
						{
							flag3 = true;
							if (printDebug)
							{
								Game.Log("FindNearestWithAction: Has exited start building", 2);
							}
						}
					}
					if (flag4)
					{
						if (printDebug)
						{
							string[] array = new string[5];
							array[0] = "FindNearestWithAction: Scanning with room ";
							array[1] = newRoom.name;
							array[2] = " (";
							int num2 = 3;
							int count = newRoom.actionReference.Count;
							array[num2] = count.ToString();
							array[4] = " actions present)";
							Game.Log(string.Concat(array), 2);
						}
						if (newRoom.actionReference.ContainsKey(action))
						{
							Interactable interactable = null;
							float num3 = -9999f;
							foreach (Interactable interactable2 in newRoom.actionReference[action])
							{
								if (printDebug)
								{
									Game.Log("FindNearestWithAction: Scanning object " + interactable2.name, 2);
								}
								if (excludeAIUsingThis && interactable2.nextAIInteraction != null)
								{
									if (printDebug)
									{
										Game.Log("FindNearestWithAction: Object " + interactable2.name + " has existing user", 2);
									}
								}
								else if (!interactable2.originalPosition)
								{
									if (printDebug)
									{
										Game.Log("FindNearestWithAction: Object " + interactable2.name + " has been moved by player!", 2);
									}
								}
								else
								{
									if (mustContainDesireCategory)
									{
										if (interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.fridge)
										{
											if (interactable2.furnitureParent != null)
											{
												List<Interactable> spawnedInteractables = interactable2.furnitureParent.spawnedInteractables;
												Predicate<Interactable> predicate;
												if ((predicate = <>9__0) == null)
												{
													predicate = (<>9__0 = ((Interactable item) => item.preset.retailItem != null && item.preset.retailItem.desireCategory == containDesireCategory));
												}
												if (!spawnedInteractables.Exists(predicate))
												{
													continue;
												}
											}
										}
										else if (interactable2.preset.menuOverride != null)
										{
											List<InteractablePreset> itemsSold = interactable2.preset.menuOverride.itemsSold;
											Predicate<InteractablePreset> predicate2;
											if ((predicate2 = <>9__1) == null)
											{
												predicate2 = (<>9__1 = ((InteractablePreset item) => item.retailItem != null && item.retailItem.desireCategory == containDesireCategory));
											}
											if (!itemsSold.Exists(predicate2))
											{
												continue;
											}
										}
										else if (newRoom.gameLocation.thisAsAddress != null && newRoom.gameLocation.thisAsAddress.company != null)
										{
											bool flag6 = false;
											foreach (KeyValuePair<InteractablePreset, int> keyValuePair in newRoom.gameLocation.thisAsAddress.company.prices)
											{
												if (keyValuePair.Key.retailItem != null && keyValuePair.Key.retailItem.desireCategory == containDesireCategory)
												{
													flag6 = true;
													break;
												}
											}
											if (!flag6)
											{
												continue;
											}
										}
										else if (interactable2.furnitureParent != null && interactable2.furnitureParent.ownerMap.Count > 0)
										{
											bool flag7 = false;
											foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair2 in interactable2.furnitureParent.ownerMap)
											{
												if (keyValuePair2.Key.human != null && keyValuePair2.Key.human.job != null && keyValuePair2.Key.human.job.employer != null && keyValuePair2.Key.human.job.preset.selfEmployed)
												{
													foreach (KeyValuePair<InteractablePreset, int> keyValuePair3 in keyValuePair2.Key.human.job.employer.prices)
													{
														if (keyValuePair3.Key.retailItem != null && keyValuePair3.Key.retailItem.desireCategory == containDesireCategory)
														{
															flag7 = true;
															break;
														}
													}
												}
												if (flag7)
												{
													break;
												}
											}
											if (!flag7)
											{
												continue;
											}
										}
									}
									if ((!useSpecialCasesOnly || interactable2.preset.specialCaseFlag == mustBeSpecial) && (!action.requiresTelephone || (interactable2.t != null && !(interactable2.t.activeReceiver != null) && (!action.requiresTelephoneNoCall || interactable2.t.activeCall == null || interactable2.t.activeCall.Count <= 0))))
									{
										Human human = null;
										interactable2.usagePoint.TryGetUserAtSlot(action.usageSlot, out human);
										if (!preferUnused || (human == null && interactable2.usagePoint.reserved == null && interactable2.node.occupiedSpace.Count <= 0))
										{
											return interactable2;
										}
										float num4 = (float)interactable2.preset.AIPriority;
										if (robberyPriority > 0f && interactable2.furnitureParent != null && interactable2.furnitureParent.furnitureClasses.Count > 0)
										{
											num4 += (float)interactable2.furnitureParent.furnitureClasses[0].aiRobberyPriority * robberyPriority;
										}
										if (avoidInteractables != null && avoidInteractables.Contains(interactable2))
										{
											num4 -= 100f;
										}
										if (human == null)
										{
											num4 += 5f + Toolbox.Instance.Rand(0f, 1f, false);
										}
										if (interactable2.node.occupiedSpace.Count <= 0)
										{
											num4 += 7f + Toolbox.Instance.Rand(0f, 1f, false);
										}
										if (interactable2.usagePoint.reserved != null)
										{
											num4 -= 6f;
										}
										if (interactable == null || num4 > num3)
										{
											interactable = interactable2;
											num3 = num4;
										}
									}
								}
							}
							if (interactable != null)
							{
								return interactable;
							}
						}
					}
				}
				if ((!flag || restrictTo == newRoom.gameLocation || restrictTo == null) && (!flag2 || restrictToBuilding == newRoom.gameLocation.building || restrictToBuilding == null))
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
					{
						if (nodeAccess.walkingAccess)
						{
							NewRoom otherRoom = nodeAccess.GetOtherRoom(newRoom);
							if (!hashSet.Contains(otherRoom) && !hashSet2.Contains(otherRoom))
							{
								hashSet.Add(otherRoom);
							}
						}
					}
				}
				if (flag && restrictTo != null)
				{
					List<NewRoom> list = new List<NewRoom>();
					foreach (NewRoom newRoom2 in hashSet)
					{
						if (newRoom2.gameLocation != restrictTo)
						{
							list.Add(newRoom2);
						}
					}
					foreach (NewRoom newRoom3 in list)
					{
						hashSet.Remove(newRoom3);
					}
				}
			}
			hashSet2.Add(newRoom);
			hashSet.Remove(newRoom);
			num--;
			if (printDebug && num <= 0)
			{
				Game.Log("FindNearestWithAction: Failsafe count reached", 2);
			}
		}
		if (Game.Instance.collectDebugData)
		{
			person != null;
		}
		return null;
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x00178294 File Offset: 0x00176494
	public Company FindNearestThatSells(InteractablePreset sellsItem, NewGameLocation startLocation)
	{
		HashSet<NewGameLocation> hashSet = new HashSet<NewGameLocation>();
		HashSet<NewGameLocation> hashSet2 = new HashSet<NewGameLocation>();
		hashSet.Add(startLocation);
		int num = CityData.Instance.gameLocationDirectory.Count + 1;
		while (hashSet.Count > 0 && num > 0)
		{
			NewGameLocation newGameLocation = Enumerable.First<NewGameLocation>(hashSet);
			if (newGameLocation != null)
			{
				if (newGameLocation.thisAsAddress != null && newGameLocation.thisAsAddress.company != null && newGameLocation.thisAsAddress.company.prices.ContainsKey(sellsItem))
				{
					return newGameLocation.thisAsAddress.company;
				}
				foreach (NewNode.NodeAccess nodeAccess in newGameLocation.entrances)
				{
					NewGameLocation otherGameLocation = nodeAccess.GetOtherGameLocation(newGameLocation);
					if (!hashSet.Contains(otherGameLocation) && !hashSet2.Contains(otherGameLocation))
					{
						hashSet.Add(otherGameLocation);
					}
				}
			}
			hashSet2.Add(newGameLocation);
			hashSet.Remove(newGameLocation);
			num--;
		}
		return null;
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x001783A8 File Offset: 0x001765A8
	public string GetNumbericalStringReference(int number)
	{
		string text = number.ToString();
		if (number >= 11 && number <= 13)
		{
			text += Strings.Get("ui.interface", "th", Strings.Casing.asIs, false, false, false, null);
		}
		else if (text.Length > 0 && text.Substring(text.Length - 1, 1) == "1")
		{
			text += Strings.Get("ui.interface", "st", Strings.Casing.asIs, false, false, false, null);
		}
		else if (text.Length > 0 && text.Substring(text.Length - 1, 1) == "2")
		{
			text += Strings.Get("ui.interface", "nd", Strings.Casing.asIs, false, false, false, null);
		}
		else if (text.Length > 0 && text.Substring(text.Length - 1, 1) == "3")
		{
			text += Strings.Get("ui.interface", "rd", Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			text += Strings.Get("ui.interface", "th", Strings.Casing.asIs, false, false, false, null);
		}
		return text;
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x001784C9 File Offset: 0x001766C9
	public Vector2 Rotate(Vector2 aPoint, float aDegree)
	{
		return Quaternion.Euler(0f, 0f, aDegree) * aPoint;
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x001784EC File Offset: 0x001766EC
	public List<Vector2> PlotLine(Vector2 point1, Vector2 point2)
	{
		List<Vector2> list = new List<Vector2>();
		int num = Mathf.Abs((int)point2.x - (int)point1.x);
		int num2 = (point1.x < point2.x) ? 1 : -1;
		int num3 = -Mathf.Abs((int)point2.y - (int)point1.y);
		int num4 = (point1.y < point2.y) ? 1 : -1;
		int num5 = num + num3;
		for (;;)
		{
			list.Add(new Vector2(point1.x, point1.y));
			if (point1.x == point2.x && point1.y == point2.y)
			{
				break;
			}
			int num6 = 2 * num5;
			if (num6 >= num3)
			{
				num5 += num3;
				point1.x += (float)num2;
			}
			if (num6 <= num)
			{
				num5 += num;
				point1.y += (float)num4;
			}
		}
		return list;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x001785C0 File Offset: 0x001767C0
	public Quaternion ClampRotation(Quaternion q, float minimumUpDown, float maximumUpDown, float minimumLeftRight, float maximumLeftRight)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float num = 114.59156f * Mathf.Atan(q.x);
		num = Mathf.Clamp(num, minimumUpDown, maximumUpDown);
		q.x = Mathf.Tan(0.008726646f * num);
		float num2 = 114.59156f * Mathf.Atan(q.y);
		num2 = Mathf.Clamp(num2, minimumLeftRight, maximumLeftRight);
		q.y = Mathf.Tan(0.008726646f * num2);
		return q;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x0017866C File Offset: 0x0017686C
	public float ClampAngle(float angle, float min, float max)
	{
		if (angle < 90f || angle > 270f)
		{
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (max > 180f)
			{
				max -= 360f;
			}
			if (min > 180f)
			{
				min -= 360f;
			}
		}
		angle = Mathf.Clamp(angle, min, max);
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle;
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x001786D8 File Offset: 0x001768D8
	public void ShuffleList(ref List<CharacterTrait> list)
	{
		if (list == null || list.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			CharacterTrait characterTrait = list[i];
			if (characterTrait == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else
			{
				int num = this.Rand(i, list.Count, false);
				list[i] = list[num];
				list[num] = characterTrait;
			}
		}
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00178750 File Offset: 0x00176950
	public void ShuffleListSeedContained(ref List<CharacterTrait> list, string input, out string output)
	{
		output = input;
		if (list == null || list.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			CharacterTrait characterTrait = list[i];
			if (characterTrait == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else
			{
				int num = this.GetPsuedoRandomNumberContained(i, list.Count, input, out output);
				num = Mathf.Clamp(num, 0, list.Count - 1);
				list[i] = list[num];
				list[num] = characterTrait;
			}
		}
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x001787DC File Offset: 0x001769DC
	public void ShuffleList(ref List<Human.WalletItem> list)
	{
		if (list == null || list.Count <= 1)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Human.WalletItem walletItem = list[i];
			if (walletItem == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else
			{
				int num = Toolbox.Instance.Rand(i, list.Count, false);
				num = Mathf.Clamp(num, 0, list.Count - 1);
				list[i] = list[num];
				list[num] = walletItem;
			}
		}
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00178862 File Offset: 0x00176A62
	public GameObject SpawnObject(GameObject newObj, Transform newParent)
	{
		return Object.Instantiate<GameObject>(newObj, newParent);
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x0017886B File Offset: 0x00176A6B
	public GameObject SpawnObject(GameObject newObj, Vector3 newPos, Quaternion newRot, Transform newParent)
	{
		return Object.Instantiate<GameObject>(newObj, newPos, newRot, newParent);
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x00178877 File Offset: 0x00176A77
	public void DestroyObject(GameObject newObj)
	{
		if (newObj != null)
		{
			Object.Destroy(newObj);
		}
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x00178888 File Offset: 0x00176A88
	public Material SpawnMaterial(Material newObj)
	{
		return Object.Instantiate<Material>(newObj);
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x00178890 File Offset: 0x00176A90
	public Vector3 GetLocalEulerAtRotation(Transform transform, Quaternion targetRotation)
	{
		return (Quaternion.Inverse(transform.parent.rotation) * targetRotation).eulerAngles;
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x001788BC File Offset: 0x00176ABC
	public List<int> GetKeyCodeFromString(string str)
	{
		List<int> list = new List<int>();
		try
		{
			for (int i = 0; i < 4; i++)
			{
				string text = str.Substring(i, 1).ToLower();
				if (text == "a" || text == "b" || text == "c")
				{
					list.Add(2);
				}
				else if (text == "d" || text == "e" || text == "f")
				{
					list.Add(3);
				}
				else if (text == "g" || text == "h" || text == "i")
				{
					list.Add(4);
				}
				else if (text == "j" || text == "k" || text == "l")
				{
					list.Add(5);
				}
				else if (text == "m" || text == "n" || text == "o")
				{
					list.Add(6);
				}
				else if (text == "p" || text == "r" || text == "s" || text == "q")
				{
					list.Add(7);
				}
				else if (text == "t" || text == "u" || text == "v")
				{
					list.Add(8);
				}
				else if (text == "w" || text == "x" || text == "y" || text == "z")
				{
					list.Add(9);
				}
				else
				{
					list.Add(0);
				}
			}
		}
		catch
		{
		}
		return list;
	}

	// Token: 0x06001981 RID: 6529 RVA: 0x00178AC4 File Offset: 0x00176CC4
	public string GenerateEvidenceIdentifier(Evidence ev)
	{
		string text = string.Empty;
		text = text + ev.preset.name + "|";
		if (ev.controller != null)
		{
			text += ev.controller.name;
		}
		text += "|";
		if (ev.parent != null)
		{
			text += ev.parent.preset.name;
		}
		return text;
	}

	// Token: 0x06001982 RID: 6530 RVA: 0x00178B3C File Offset: 0x00176D3C
	public string GenerateUniqueID()
	{
		return Guid.NewGuid().ToString();
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x00178B5C File Offset: 0x00176D5C
	public Interactable FindClosestObjectTo(InteractablePreset objectType, Vector3 closestTo, NewBuilding constrainToBuilding, NewGameLocation constrainToLocation, NewRoom constrainToRoom, out float distance, bool publicOnly = false)
	{
		distance = 0f;
		List<Interactable> list = new List<Interactable>();
		List<NewGameLocation> list2 = new List<NewGameLocation>();
		List<NewRoom> list3 = new List<NewRoom>();
		if (constrainToLocation != null)
		{
			list2.Add(constrainToLocation);
		}
		else
		{
			if (constrainToBuilding != null)
			{
				using (Dictionary<int, NewFloor>.Enumerator enumerator = constrainToBuilding.floors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, NewFloor> keyValuePair = enumerator.Current;
						list2.AddRange(keyValuePair.Value.addresses);
					}
					goto IL_148;
				}
			}
			if (publicOnly)
			{
				List<Company> list4 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset != null && item.preset.publicFacing && item.openForBusinessActual && item.openForBusinessDesired && item.placeOfBusiness != null && !item.placeOfBusiness.isCrimeScene);
				if (list4.Count <= 0)
				{
					list4 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset != null && item.preset.publicFacing);
				}
				using (List<Company>.Enumerator enumerator2 = list4.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Company company = enumerator2.Current;
						list2.Add(company.placeOfBusiness);
					}
					goto IL_148;
				}
			}
			list2.AddRange(CityData.Instance.gameLocationDirectory);
		}
		IL_148:
		if (constrainToRoom != null)
		{
			list3.Add(constrainToRoom);
		}
		else if (constrainToLocation != null)
		{
			list3.AddRange(constrainToLocation.rooms);
		}
		else
		{
			foreach (NewGameLocation newGameLocation in list2)
			{
				list3.AddRange(newGameLocation.rooms);
			}
		}
		Predicate<Interactable> <>9__2;
		foreach (NewRoom newRoom in list3)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				List<Interactable> list5 = list;
				List<Interactable> interactables = newNode.interactables;
				Predicate<Interactable> predicate;
				if ((predicate = <>9__2) == null)
				{
					predicate = (<>9__2 = ((Interactable item) => item.preset == objectType));
				}
				list5.AddRange(interactables.FindAll(predicate));
			}
		}
		Interactable result = null;
		distance = float.PositiveInfinity;
		foreach (Interactable interactable in list)
		{
			float num = Vector3.Distance(interactable.GetWorldPosition(true), closestTo);
			if (num < distance)
			{
				distance = num;
				result = interactable;
			}
		}
		return result;
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x00178E4C File Offset: 0x0017704C
	public FurnitureLocation FindFurnitureWithinGameLocation(NewGameLocation location, FurnitureClass furnitureClass, out NewRoom room)
	{
		room = null;
		foreach (NewRoom newRoom in location.rooms)
		{
			if (!(newRoom == null))
			{
				foreach (FurnitureLocation furnitureLocation in newRoom.individualFurniture)
				{
					if (furnitureLocation.furnitureClasses.Contains(furnitureClass))
					{
						room = newRoom;
						return furnitureLocation;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x00178EFC File Offset: 0x001770FC
	public void SetRectSize(RectTransform trs, float left, float top, float right, float bottom)
	{
		trs.offsetMin = new Vector2(left, bottom);
		trs.offsetMax = new Vector2(-right, -top);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x00178F1C File Offset: 0x0017711C
	public Rect GetWorldRect(RectTransform rt, Vector2 scale)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		Vector3 vector = array[0];
		Vector2 vector2;
		vector2..ctor(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
		return new Rect(vector, vector2);
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x00178F84 File Offset: 0x00177184
	public int CreateLayerMask(Toolbox.LayerMaskMode castMode, params int[] aLayers)
	{
		bool flag = false;
		if (castMode == Toolbox.LayerMaskMode.castAllExcept)
		{
			flag = true;
		}
		int num = 0;
		foreach (int num2 in aLayers)
		{
			num |= 1 << num2;
		}
		if (flag)
		{
			num = ~num;
		}
		return num;
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x00178FC0 File Offset: 0x001771C0
	public NewNode FindClosestValidNodeToWorldPosition(Vector3 worldPos, bool onlyAccessibleNodes = false, bool checkUpAndDown = true, bool limitToDirection = false, Vector3Int limitedDirection = default(Vector3Int), bool limitToFloor = false, int limitedToFloor = 0, bool outsideOnly = false, int safety = 200)
	{
		if (limitToFloor)
		{
			worldPos.y = (float)limitedToFloor * PathFinder.Instance.nodeSize.z;
		}
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(worldPos);
		List<Vector3Int> list = new List<Vector3Int>();
		list.Add(vector3Int);
		List<Vector3> list2 = new List<Vector3>();
		while (list.Count > 0 && safety > 0)
		{
			Vector3Int vector3Int2 = list[0];
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode))
			{
				if (onlyAccessibleNodes)
				{
					if (newNode.accessToOtherNodes.Count > 0 && (!outsideOnly || newNode.isOutside || newNode.room.IsOutside()))
					{
						return newNode;
					}
				}
				else if (!outsideOnly || newNode.isOutside || newNode.room.IsOutside())
				{
					return newNode;
				}
			}
			if (limitToDirection)
			{
				Vector3Int vector3Int3 = vector3Int2 + limitedDirection;
				vector3Int3.x = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int3.x, PathFinder.Instance.nodeRangeX.x, PathFinder.Instance.nodeRangeX.y));
				vector3Int3.y = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int3.y, PathFinder.Instance.nodeRangeY.x, PathFinder.Instance.nodeRangeY.y));
				vector3Int3.z = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int3.z, PathFinder.Instance.nodeRangeZ.x, PathFinder.Instance.nodeRangeZ.y));
				if (!list.Contains(vector3Int3) && !list2.Contains(vector3Int3))
				{
					list.Add(vector3Int3);
				}
			}
			else if (checkUpAndDown && !limitToFloor)
			{
				foreach (Vector3Int vector3Int4 in CityData.Instance.offsetArrayX6)
				{
					Vector3Int vector3Int5 = vector3Int2 + vector3Int4;
					vector3Int5.x = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int5.x, PathFinder.Instance.nodeRangeX.x, PathFinder.Instance.nodeRangeX.y));
					vector3Int5.y = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int5.y, PathFinder.Instance.nodeRangeY.x, PathFinder.Instance.nodeRangeY.y));
					vector3Int5.z = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int5.z, PathFinder.Instance.nodeRangeZ.x, PathFinder.Instance.nodeRangeZ.y));
					if (!list.Contains(vector3Int5) && !list2.Contains(vector3Int5))
					{
						list.Add(vector3Int5);
					}
				}
			}
			else
			{
				foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
				{
					Vector3Int vector3Int6 = vector3Int2 + new Vector3Int(vector2Int.x, vector2Int.y);
					vector3Int6.x = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int6.x, PathFinder.Instance.nodeRangeX.x, PathFinder.Instance.nodeRangeX.y));
					vector3Int6.y = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int6.y, PathFinder.Instance.nodeRangeY.x, PathFinder.Instance.nodeRangeY.y));
					vector3Int6.z = Mathf.RoundToInt(Mathf.Clamp((float)vector3Int6.z, PathFinder.Instance.nodeRangeZ.x, PathFinder.Instance.nodeRangeZ.y));
					if (!list.Contains(vector3Int6) && !list2.Contains(vector3Int6))
					{
						list.Add(vector3Int6);
					}
				}
			}
			list2.Add(vector3Int2);
			list.RemoveAt(0);
			safety--;
		}
		string text = "Tried to find closest node to world position ";
		Vector3 vector = worldPos;
		Game.LogError(text + vector.ToString() + " but failed. It could be nodes are not yet set up? Or this coordinate is rediculously far away from anything!", 2);
		return null;
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x001793D8 File Offset: 0x001775D8
	public MaterialGroupPreset GetMaterialProperties(Material mat)
	{
		MaterialGroupPreset result = null;
		if (!this.materialProperties.TryGetValue(mat, ref result))
		{
			Game.Log("Unable to find specific material properties for material " + mat.name, 2);
		}
		return result;
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x00179410 File Offset: 0x00177610
	public FurniturePreset GetFurnitureFromMesh(Mesh mesh)
	{
		if (mesh == null)
		{
			return null;
		}
		FurniturePreset result = null;
		if (this.furnitureMeshReference != null)
		{
			this.furnitureMeshReference.TryGetValue(mesh, ref result);
		}
		return result;
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x00179444 File Offset: 0x00177644
	public InteractablePreset GetInteractablePreset(string interactableName)
	{
		InteractablePreset result = null;
		try
		{
			result = this.objectPresetDictionary[interactableName];
		}
		catch
		{
			Game.LogError("Cannot find object " + interactableName, 2);
			return null;
		}
		return result;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x0017948C File Offset: 0x0017768C
	public Quaternion TransformRotation(Quaternion worldRotation, Transform targetsLocal)
	{
		return Quaternion.Inverse(targetsLocal.rotation) * worldRotation;
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x0017948C File Offset: 0x0017768C
	public Quaternion InverseTransformRotation(Quaternion localRotation, Transform target)
	{
		return Quaternion.Inverse(target.rotation) * localRotation;
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x001794A0 File Offset: 0x001776A0
	public void Shoot(Actor fromThis, Vector3 muzzlePoint, Vector3 aimPoint, float aimRange, float accuracy, float damage, MurderWeaponPreset weapon, bool ejectBrass, Vector3 ejectBrassPoint, bool forcePhysicsEjectBrass)
	{
		Vector3 vector = aimPoint - muzzlePoint;
		if (weapon != null && weapon.muzzleFlash != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(weapon.muzzleFlash, PrefabControls.Instance.mapContainer);
			gameObject.transform.position = muzzlePoint;
			gameObject.transform.rotation = Quaternion.LookRotation(vector, gameObject.transform.up);
		}
		float num = Mathf.Lerp(0.2f, 0f, accuracy);
		Vector3 vector2 = vector.normalized + new Vector3(Toolbox.Instance.Rand(-num, num, false), Toolbox.Instance.Rand(-num, num, false), Toolbox.Instance.Rand(-num, num, false));
		if (weapon.fireEvent != null)
		{
			AudioController.Instance.PlayWorldOneShot(weapon.fireEvent, fromThis, fromThis.currentNode, muzzlePoint, null, null, 1f, null, false, null, false);
		}
		if (ejectBrass && weapon.shellCasing != null)
		{
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(weapon.shellCasing, fromThis as Human, fromThis as Human, null, ejectBrassPoint, Vector3.zero, null, null, "");
			if (interactable != null)
			{
				interactable.MarkAsTrash(true, false, 0f);
				Vector3 vector3 = fromThis.transform.right * Toolbox.Instance.Rand(3.5f, 4.5f, false) + new Vector3(Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false), Toolbox.Instance.Rand(-0.1f, 0.1f, false));
				if (interactable.controller != null)
				{
					interactable.controller.DropThis(false);
					interactable.controller.rb.AddForce(vector3, 2);
				}
				else
				{
					interactable.ForcePhysicsActive(forcePhysicsEjectBrass, true, vector3, 2, false);
				}
			}
		}
		RaycastHit hit;
		if (Physics.Raycast(muzzlePoint, vector2, ref hit, weapon.maximumBulletRange, Toolbox.Instance.physicalObjectsLayerMask))
		{
			Vector3 vector4 = hit.point - muzzlePoint;
			Debug.DrawRay(muzzlePoint, vector4, Color.yellow, 2f);
			Actor actor = hit.transform.gameObject.GetComponent<Actor>();
			if (actor == null)
			{
				actor = hit.transform.gameObject.GetComponentInParent<Actor>();
			}
			float amount = damage;
			if (hit.distance > aimRange)
			{
				amount = Mathf.Lerp(damage, 0f, (hit.distance - aimRange) / (weapon.maximumBulletRange - aimRange));
			}
			if (actor != null && actor != fromThis)
			{
				Debug.DrawRay(muzzlePoint, vector4, Color.green, 2f);
				bool enableKill = false;
				if (fromThis.ai != null && actor.ai != null && fromThis.ai.GetCurrentKillTarget() == actor)
				{
					enableKill = true;
				}
				if (actor.isPlayer)
				{
					if (weapon.impactEventPlayer != null)
					{
						AudioController.Instance.PlayWorldOneShot(weapon.impactEventPlayer, actor, actor.currentNode, hit.point, null, null, 1f, null, false, null, false);
					}
				}
				else if (weapon.impactEventBody != null)
				{
					AudioController.Instance.PlayWorldOneShot(weapon.impactEventBody, actor, actor.currentNode, hit.point, null, null, 1f, null, false, null, false);
				}
				actor.RecieveDamage(amount, fromThis, hit.point, vector4, weapon.forwardSpatter, weapon.backSpatter, SpatterSimulation.EraseMode.useDespawnTime, false, false, 0f, 1f, enableKill, true, 1f);
				if (weapon.entryWound != null)
				{
					Citizen citizen = actor as Citizen;
					if (citizen != null)
					{
						citizen.CreateWoundClosestToPoint(hit.point, hit.normal, weapon.entryWound, weapon);
						return;
					}
				}
			}
			else
			{
				if (weapon.impactEvent != null)
				{
					AudioController.Instance.PlayWorldOneShot(weapon.impactEvent, actor, actor.currentNode, hit.point, null, null, 1f, null, false, null, false);
				}
				BreakableWindowController component = hit.transform.gameObject.GetComponent<BreakableWindowController>();
				if (component != null && !component.isBroken)
				{
					component.AddBulletHole(weapon, hit.point, vector2.normalized, fromThis, false, hit.normal);
					return;
				}
				InteractableController component2 = hit.transform.gameObject.GetComponent<InteractableController>();
				if (!(component2 != null) || component2.interactable == null)
				{
					Debug.DrawRay(muzzlePoint, vector4, Color.red, 2f);
					this.CreateBulletSurfaceContactFX(weapon, hit);
					return;
				}
				if (weapon != null && weapon.bulletRicochet != null)
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(weapon.bulletRicochet, PrefabControls.Instance.mapContainer);
					gameObject2.transform.position = hit.point;
					gameObject2.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				}
				if (component2.isCarriedByPlayer)
				{
					if (UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.holdingBlocksBullets) <= 0f)
					{
						component2.DropThis(false);
						return;
					}
				}
				else if (component2.interactable.preset.breakable)
				{
					component2.BreakObject(hit.point, hit.normal, 1f, fromThis);
					return;
				}
			}
		}
		else
		{
			Debug.DrawRay(muzzlePoint, vector2, Color.yellow, 2f);
		}
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x00179A30 File Offset: 0x00177C30
	public void CreateBulletSurfaceContactFX(MurderWeaponPreset weapon, RaycastHit hit)
	{
		if (weapon.bulletRicochet != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(weapon.bulletRicochet, PrefabControls.Instance.mapContainer);
			gameObject.transform.position = hit.point;
			gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		}
		if (weapon.bulletHole != null)
		{
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(weapon.bulletHole, null, null, null, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles, null, null, "");
			if (interactable != null)
			{
				interactable.MarkAsTrash(true, false, 0f);
			}
		}
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x00179AE8 File Offset: 0x00177CE8
	public float GetPsuedoRandomNumber(float lowerRange, float upperRange, string str, bool updateLastKey = false)
	{
		double num = new Random(str.GetHashCode()).NextDouble();
		if (updateLastKey)
		{
			if (InteriorCreator.Instance != null && InteriorCreator.Instance.threadedInteriorCreationActive)
			{
				Game.LogError("Updating load state key while interior generation is ative! This shouldn't happen!", 2);
			}
			this.lastRandomNumberKey = num.ToString();
			this.lastRandomNumberKey = this.lastRandomNumberKey.Substring(2);
		}
		return Mathf.Lerp(lowerRange, upperRange, (float)num);
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x00179B58 File Offset: 0x00177D58
	public int GetPsuedoRandomNumber(int lowerRange, int upperRange, string str, bool updateLastKey = false)
	{
		double num = new Random(str.GetHashCode()).NextDouble();
		if (updateLastKey)
		{
			if (InteriorCreator.Instance != null && InteriorCreator.Instance.threadedInteriorCreationActive)
			{
				Game.LogError("Updating load state key while interior generation is ative! This shouldn't happen!", 2);
			}
			this.lastRandomNumberKey = num.ToString();
			this.lastRandomNumberKey = this.lastRandomNumberKey.Substring(2);
		}
		return Mathf.FloorToInt(Mathf.Lerp((float)lowerRange, (float)upperRange, (float)num));
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x00179BD0 File Offset: 0x00177DD0
	public float Rand(float min, float max, bool definitelyNotPartOfCityGeneration = false)
	{
		if (!definitelyNotPartOfCityGeneration && !SessionData.Instance.startedGame && CityConstructor.Instance != null && CityConstructor.Instance.loadingOperationActive && !CityConstructor.Instance.preSimActive)
		{
			Game.LogError("Generating random number without seed before game has started! If this is part of the city generation process, it could result in unpredictable cities...", 2);
		}
		return Random.Range(min, max);
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x00179C23 File Offset: 0x00177E23
	public int Rand(int min, int max, bool definitelyNotPartOfCityGeneration = false)
	{
		if (!definitelyNotPartOfCityGeneration && !SessionData.Instance.startedGame && CityConstructor.Instance != null && CityConstructor.Instance.loadingOperationActive)
		{
			Game.LogError("Generating random number without seed before game has started! If this is part of the city generation process, it could result in unpredictable cities...", 2);
		}
		return Random.Range(min, max);
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x00179C60 File Offset: 0x00177E60
	public float SeedRand(float min, float max)
	{
		if (min == max)
		{
			return min;
		}
		float psuedoRandomNumber;
		if (this.lastRandomNumberKey.Length <= 0)
		{
			psuedoRandomNumber = this.GetPsuedoRandomNumber(min, max, CityData.Instance.seed, true);
			Game.Log("CityGen: First random from seed: " + CityData.Instance.seed + " = " + psuedoRandomNumber.ToString(), 2);
		}
		else
		{
			psuedoRandomNumber = this.GetPsuedoRandomNumber(min, max, this.lastRandomNumberKey, true);
		}
		return psuedoRandomNumber;
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x00179CD0 File Offset: 0x00177ED0
	public int SeedRand(int min, int max)
	{
		if (min == max)
		{
			return min;
		}
		int psuedoRandomNumber;
		if (this.lastRandomNumberKey.Length <= 0)
		{
			psuedoRandomNumber = this.GetPsuedoRandomNumber(min, max, CityData.Instance.seed, true);
			Game.Log("CityGen: First random from seed: " + CityData.Instance.seed + " = " + this.lastRandomNumberKey, 2);
		}
		else
		{
			psuedoRandomNumber = this.GetPsuedoRandomNumber(min, max, this.lastRandomNumberKey, true);
		}
		return psuedoRandomNumber;
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x00179D3F File Offset: 0x00177F3F
	public float VectorToRandom(Vector2 vec)
	{
		return this.Rand(vec.x, vec.y, false);
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x00179D54 File Offset: 0x00177F54
	public float VectorToRandomSeedContained(Vector2 vec, string input, out string output)
	{
		return this.GetPsuedoRandomNumberContained(vec.x, vec.y, input, out output);
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x00179D6A File Offset: 0x00177F6A
	public float RandContained(float min, float max, string seedInput, out string seedOutput)
	{
		return this.GetPsuedoRandomNumberContained(min, max, seedInput, out seedOutput);
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x00179D77 File Offset: 0x00177F77
	public int RandContained(int min, int max, string seedInput, out string seedOutput)
	{
		return this.GetPsuedoRandomNumberContained(min, max, seedInput, out seedOutput);
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x00179D84 File Offset: 0x00177F84
	public float GetPsuedoRandomNumberContained(float lowerRange, float upperRange, string seedInput, out string seedOutput)
	{
		seedOutput = seedInput;
		double num = new Random(seedInput.GetHashCode()).NextDouble();
		seedOutput = num.ToString();
		seedOutput = seedOutput.Substring(2);
		return Mathf.Lerp(lowerRange, upperRange, (float)num);
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x00179DC8 File Offset: 0x00177FC8
	public int GetPsuedoRandomNumberContained(int lowerRange, int upperRange, string seedInput, out string seedOutput)
	{
		seedOutput = seedInput;
		int num = 0;
		if (seedInput != null)
		{
			num = seedInput.GetHashCode();
		}
		double num2 = new Random(num).NextDouble();
		seedOutput = num2.ToString();
		seedOutput = seedOutput.Substring(2);
		return Mathf.FloorToInt(Mathf.Lerp((float)lowerRange, (float)upperRange, (float)num2));
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x00179E18 File Offset: 0x00178018
	public bool DDSTraitConditionLogicAcquaintance(Human thisPerson, Acquaintance acquaintance, DDSSaveClasses.TraitConditionType logic, ref List<string> traitList)
	{
		Human otherPerson = null;
		if (acquaintance != null)
		{
			otherPerson = acquaintance.GetOther(thisPerson);
		}
		return this.DDSTraitConditionLogic(thisPerson, otherPerson, logic, ref traitList);
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x00179E40 File Offset: 0x00178040
	public bool DDSTraitConditionLogic(Human thisPerson, Human otherPerson, DDSSaveClasses.TraitConditionType logic, ref List<string> traitList)
	{
		bool result = false;
		if (traitList.Count <= 0 || thisPerson == null || thisPerson.characterTraits == null)
		{
			return true;
		}
		if (logic == DDSSaveClasses.TraitConditionType.IfAnyOfThese || logic == DDSSaveClasses.TraitConditionType.IfNoneOfThese || logic == DDSSaveClasses.TraitConditionType.IfAllOfThese)
		{
			using (List<Human.Trait>.Enumerator enumerator = thisPerson.characterTraits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Human.Trait trait = enumerator.Current;
					if (logic == DDSSaveClasses.TraitConditionType.IfAnyOfThese && traitList.Contains(trait.trait.name))
					{
						result = true;
						break;
					}
					if (logic == DDSSaveClasses.TraitConditionType.IfNoneOfThese)
					{
						if (traitList.Contains(trait.trait.name))
						{
							result = false;
							break;
						}
						result = true;
					}
					else if (logic == DDSSaveClasses.TraitConditionType.IfAllOfThese)
					{
						if (!traitList.Contains(trait.trait.name))
						{
							result = false;
							break;
						}
						result = true;
					}
				}
				return result;
			}
		}
		if (otherPerson != null)
		{
			using (List<Human.Trait>.Enumerator enumerator = otherPerson.characterTraits.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Human.Trait trait2 = enumerator.Current;
					if (logic == DDSSaveClasses.TraitConditionType.otherAnyOfThese && traitList.Contains(trait2.trait.name))
					{
						result = true;
						break;
					}
					if (logic == DDSSaveClasses.TraitConditionType.otherNoneOfThese)
					{
						if (traitList.Contains(trait2.trait.name))
						{
							result = false;
							break;
						}
						result = true;
					}
					else if (logic == DDSSaveClasses.TraitConditionType.otherAllOfThese)
					{
						if (!traitList.Contains(trait2.trait.name))
						{
							result = false;
							break;
						}
						result = true;
					}
				}
				return result;
			}
		}
		result = false;
		return result;
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x00002265 File Offset: 0x00000465
	public void LoadInteractableToWorld()
	{
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x00179FCC File Offset: 0x001781CC
	public string ToBase26(int myNumber)
	{
		myNumber++;
		LinkedList<int> linkedList = new LinkedList<int>();
		while (myNumber > 26)
		{
			int num = myNumber % 26;
			if (num == 0)
			{
				myNumber = myNumber / 26 - 1;
				linkedList.AddFirst(26);
			}
			else
			{
				myNumber /= 26;
				linkedList.AddFirst(num);
			}
		}
		if (myNumber > 0)
		{
			linkedList.AddFirst(myNumber);
		}
		return new string(Enumerable.ToArray<char>(Enumerable.Select<int, char>(linkedList, (int s) => (char)(65 + s - 1))));
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x0017A050 File Offset: 0x00178250
	public string GenerateSeed(int digits = 16, bool useSeed = false, string newSeed = "")
	{
		string text = string.Empty;
		for (int i = 0; i < digits; i++)
		{
			float num;
			if (useSeed)
			{
				num = Toolbox.Instance.RandContained(0f, 1f, newSeed, out newSeed);
			}
			else
			{
				num = Toolbox.Instance.Rand(0f, 1f, false);
			}
			if (num <= 0.33f)
			{
				text += this.seedLetters[Toolbox.Instance.Rand(0, this.seedLetters.Length, false)].ToString();
			}
			else if (num >= 0.66f)
			{
				text += this.seedLetters[Toolbox.Instance.Rand(0, this.seedLetters.Length, false)].ToString().ToLower();
			}
			else
			{
				text += this.seedNumbers[Toolbox.Instance.Rand(0, this.seedNumbers.Length, false)].ToString();
			}
		}
		return text;
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x0017A149 File Offset: 0x00178349
	public bool RaycastCheck(Transform from, Transform to, float maxRange, out RaycastHit hit)
	{
		return this.RaycastCheck(from.position, to, maxRange, out hit);
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x0017A15C File Offset: 0x0017835C
	public bool RaycastCheck(Vector3 from, Transform to, float maxRange, out RaycastHit hit)
	{
		Vector3 vector = to.transform.position - from;
		return Physics.Raycast(new Ray(from, vector), ref hit, maxRange, this.aiSightingLayerMask, 2) && hit.transform == to;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x0017A1A8 File Offset: 0x001783A8
	public bool RaycastCheck(Vector3 from, Collider to, float maxRange, out RaycastHit hit)
	{
		Vector3 vector = to.bounds.center - from;
		return Physics.Raycast(new Ray(from, vector), ref hit, maxRange, this.aiSightingLayerMask, 2) && hit.collider == to;
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x0017A1F4 File Offset: 0x001783F4
	public void SetPivot(RectTransform rectTransform, Vector2 pivot)
	{
		Vector3 vector = rectTransform.pivot - pivot;
		vector.Scale(rectTransform.rect.size);
		vector.Scale(rectTransform.localScale);
		vector = rectTransform.rotation * vector;
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= vector;
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x0017A260 File Offset: 0x00178460
	public void SetAnchor(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
	{
		Vector3 position = rectTransform.position;
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;
		rectTransform.position = position;
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x0017A289 File Offset: 0x00178489
	public Transform[] GetAllTransforms(Transform t)
	{
		return t.GetComponentsInChildren<Transform>(true);
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x0017A294 File Offset: 0x00178494
	public Transform SearchForTransform(Transform parent, string search, bool printDebug = false)
	{
		if (parent.name == search)
		{
			return parent;
		}
		Transform transform = null;
		foreach (object obj in parent)
		{
			Transform transform2 = (Transform)obj;
			if (printDebug)
			{
				Game.Log(string.Concat(new string[]
				{
					"Transform search (",
					search,
					"/",
					parent.name,
					"): ",
					transform2.name
				}), 2);
			}
			if (transform2.name == search)
			{
				transform = transform2;
				return transform;
			}
			if (transform2.childCount > 0)
			{
				transform = this.SearchForTransform(transform2, search, printDebug);
				if (transform)
				{
					return transform;
				}
			}
		}
		return transform;
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0017A374 File Offset: 0x00178574
	public List<Transform> GetTagsWithinTransform(Transform parent, string tag)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform transform in this.GetAllTransforms(parent))
		{
			if (transform.CompareTag(tag))
			{
				list.Add(transform);
			}
		}
		return list;
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x0017A3B4 File Offset: 0x001785B4
	public void NewVmailThread(Human from, List<Human> otherParticipiants, string treeID, float timeStamp, int progress = 999, StateSaveData.CustomDataSource overrideDataSource = StateSaveData.CustomDataSource.sender, int newDataSourceID = -1)
	{
		Human to = null;
		Human to2 = null;
		Human to3 = null;
		List<Human> list = new List<Human>();
		if (otherParticipiants.Count >= 1)
		{
			to = otherParticipiants[0];
		}
		if (otherParticipiants.Count >= 2)
		{
			to2 = otherParticipiants[0];
		}
		if (otherParticipiants.Count >= 3)
		{
			to3 = otherParticipiants[0];
		}
		if (otherParticipiants.Count >= 4)
		{
			for (int i = 4; i < otherParticipiants.Count; i++)
			{
				list.Add(otherParticipiants[i]);
			}
		}
		this.NewVmailThread(from, to, to2, to3, list, treeID, timeStamp, progress, overrideDataSource, newDataSourceID);
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x0017A444 File Offset: 0x00178644
	public StateSaveData.MessageThreadSave NewVmailThread(Human from, Human to1, Human to2, Human to3, List<Human> cc, string treeID, float timeStamp, int progress = 999, StateSaveData.CustomDataSource overrideDataSource = StateSaveData.CustomDataSource.sender, int newDataSourceID = -1)
	{
		DDSSaveClasses.DDSTreeSave ddstreeSave = null;
		if (from == null)
		{
			Game.LogError("Cannot from 'from'", 2);
			return null;
		}
		if (!this.allDDSTrees.TryGetValue(treeID, ref ddstreeSave))
		{
			Game.LogError("Cannot find vmail tree " + treeID, 2);
			return null;
		}
		StateSaveData.MessageThreadSave messageThreadSave = new StateSaveData.MessageThreadSave
		{
			participantA = from.humanID,
			msgType = DDSSaveClasses.TreeType.vmail,
			treeID = treeID,
			threadID = GameplayController.Instance.assignMessageThreadID,
			ds = overrideDataSource,
			dsID = newDataSourceID
		};
		GameplayController.Instance.assignMessageThreadID++;
		if (to1 != null)
		{
			messageThreadSave.participantB = to1.humanID;
		}
		if (to2 != null)
		{
			messageThreadSave.participantC = to2.humanID;
		}
		if (to3 != null)
		{
			messageThreadSave.participantD = to3.humanID;
		}
		if (cc != null)
		{
			foreach (Human human in cc)
			{
				messageThreadSave.cc.Add(human.humanID);
			}
		}
		string text = ddstreeSave.startingMessage;
		DDSSaveClasses.DDSMessageSettings ddsmessageSettings = null;
		int num = Mathf.Min(progress, ddstreeSave.messageRef.Count);
		bool flag = false;
		while (num > 0 || !flag)
		{
			if (!flag)
			{
				flag = true;
			}
			else
			{
				num--;
			}
			if (!ddstreeSave.messageRef.TryGetValue(text, ref ddsmessageSettings))
			{
				break;
			}
			messageThreadSave.messages.Add(ddsmessageSettings.instanceID);
			messageThreadSave.timestamps.Add(timeStamp);
			if (ddsmessageSettings.saidBy <= 0)
			{
				if (from != null)
				{
					messageThreadSave.senders.Add(from.humanID);
				}
				else
				{
					messageThreadSave.senders.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidBy == 1)
			{
				if (to1 != null)
				{
					messageThreadSave.senders.Add(to1.humanID);
				}
				else
				{
					messageThreadSave.senders.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidBy == 2)
			{
				if (to2 != null)
				{
					messageThreadSave.senders.Add(to2.humanID);
				}
				else
				{
					messageThreadSave.senders.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidBy == 3)
			{
				if (to3 != null)
				{
					messageThreadSave.senders.Add(to3.humanID);
				}
				else
				{
					messageThreadSave.senders.Add(-1);
				}
			}
			if (ddsmessageSettings.saidTo <= 0)
			{
				if (from != null)
				{
					messageThreadSave.recievers.Add(from.humanID);
				}
				else
				{
					messageThreadSave.recievers.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidTo == 1)
			{
				if (to1 != null)
				{
					messageThreadSave.recievers.Add(to1.humanID);
				}
				else
				{
					messageThreadSave.recievers.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidTo == 2)
			{
				if (to2 != null)
				{
					messageThreadSave.recievers.Add(to2.humanID);
				}
				else
				{
					messageThreadSave.recievers.Add(-1);
				}
			}
			else if (ddsmessageSettings.saidTo == 3)
			{
				if (to3 != null)
				{
					messageThreadSave.recievers.Add(to3.humanID);
				}
				else
				{
					messageThreadSave.recievers.Add(-1);
				}
			}
			List<Human.DDSRank> messageTreeLinkRankings = this.GetMessageTreeLinkRankings(messageThreadSave, ddsmessageSettings);
			if (messageTreeLinkRankings.Count <= 0)
			{
				break;
			}
			if (SessionData.Instance.startedGame)
			{
				timeStamp = Mathf.Lerp(timeStamp, Mathf.Max(SessionData.Instance.gameTime - 4f, timeStamp), Toolbox.Instance.Rand(0f, 0.75f, false));
			}
			else
			{
				timeStamp = Mathf.Lerp(timeStamp, Mathf.Max(SessionData.Instance.gameTime - 4f, timeStamp), Toolbox.Instance.SeedRand(0f, 0.75f));
			}
			text = messageTreeLinkRankings[0].linkRef.to;
		}
		from.messageThreadsStarted.Add(messageThreadSave);
		if (from != null)
		{
			from.messageThreadFeatures.Add(messageThreadSave);
		}
		if (to1 != null)
		{
			to1.messageThreadFeatures.Add(messageThreadSave);
		}
		if (to2 != null)
		{
			to2.messageThreadFeatures.Add(messageThreadSave);
		}
		if (to3 != null)
		{
			to3.messageThreadFeatures.Add(messageThreadSave);
		}
		if (cc != null)
		{
			foreach (Human human2 in cc)
			{
				human2.messageThreadCCd.Add(messageThreadSave);
			}
		}
		GameplayController.Instance.messageThreads.Add(messageThreadSave.threadID, messageThreadSave);
		return messageThreadSave;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x0017A8F0 File Offset: 0x00178AF0
	public List<Human.DDSRank> GetMessageTreeLinkRankings(StateSaveData.MessageThreadSave thread, DDSSaveClasses.DDSMessageSettings thisMsg)
	{
		List<Human.DDSRank> list = new List<Human.DDSRank>();
		Human human = null;
		CityData.Instance.GetHuman(thread.participantA, out human, true);
		if (thisMsg.saidBy == 1)
		{
			CityData.Instance.GetHuman(thread.participantB, out human, true);
		}
		else if (thisMsg.saidBy == 2)
		{
			CityData.Instance.GetHuman(thread.participantC, out human, true);
		}
		else if (thisMsg.saidBy == 3)
		{
			CityData.Instance.GetHuman(thread.participantD, out human, true);
		}
		string seedInput = thread.treeID + CityData.Instance.seed + thisMsg.instanceID;
		foreach (DDSSaveClasses.DDSMessageLink ddsmessageLink in thisMsg.links)
		{
			float num = Toolbox.Instance.GetPsuedoRandomNumberContained(-0.01f, 0.01f, seedInput, out seedInput);
			if (ddsmessageLink.useWeights)
			{
				num += ddsmessageLink.choiceWeight;
			}
			if (ddsmessageLink.useKnowLike || ddsmessageLink.useTraits)
			{
				DDSSaveClasses.DDSTreeSave ddstreeSave = null;
				if (!this.allDDSTrees.TryGetValue(thread.treeID, ref ddstreeSave))
				{
					Game.LogError("Cannot find vmail tree " + thread.treeID, 2);
					return list;
				}
				DDSSaveClasses.DDSMessageSettings ddsmessageSettings = ddstreeSave.messageRef[ddsmessageLink.to];
				Human human2 = null;
				CityData.Instance.GetHuman(thread.participantA, out human2, true);
				if (ddsmessageSettings.saidBy == 1)
				{
					CityData.Instance.GetHuman(thread.participantB, out human2, true);
				}
				else if (ddsmessageSettings.saidBy == 2)
				{
					CityData.Instance.GetHuman(thread.participantC, out human2, true);
				}
				else if (ddsmessageSettings.saidBy == 3)
				{
					CityData.Instance.GetHuman(thread.participantD, out human2, true);
				}
				if (ddsmessageLink.useKnowLike)
				{
					Acquaintance acquaintance = null;
					if (human.FindAcquaintanceExists(human2, out acquaintance))
					{
						float num2 = Mathf.Abs(acquaintance.like - ddsmessageLink.like) + Mathf.Abs(acquaintance.known - ddsmessageLink.know);
						num += (2f - num2) / 2f;
					}
				}
				if (ddsmessageLink.useTraits && this.DDSTraitConditionLogic(human, human2, ddsmessageLink.traitConditions, ref ddsmessageLink.traits))
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

	// Token: 0x060019AC RID: 6572 RVA: 0x0017ABAC File Offset: 0x00178DAC
	public void ProgressVmailThread(StateSaveData.MessageThreadSave thread, int addProgress)
	{
		DDSSaveClasses.DDSTreeSave ddstreeSave = null;
		if (!this.allDDSTrees.TryGetValue(thread.treeID, ref ddstreeSave))
		{
			Game.LogError("Cannot find vmail tree " + thread.treeID, 2);
			return;
		}
		Human human = null;
		CityData.Instance.GetHuman(thread.participantA, out human, true);
		string text = ddstreeSave.startingMessage;
		float num = -999999f;
		for (int i = 0; i < thread.timestamps.Count; i++)
		{
			if (thread.timestamps[i] > num)
			{
				num = thread.timestamps[i];
				text = thread.messages[i];
			}
		}
		DDSSaveClasses.DDSMessageSettings ddsmessageSettings = null;
		if (!ddstreeSave.messageRef.TryGetValue(text, ref ddsmessageSettings))
		{
			Game.LogError("Cannot find message instance ID " + text, 2);
			return;
		}
		List<Human.DDSRank> messageTreeLinkRankings = this.GetMessageTreeLinkRankings(thread, ddsmessageSettings);
		if (messageTreeLinkRankings.Count <= 0)
		{
			return;
		}
		if (SessionData.Instance.startedGame)
		{
			num = Mathf.Lerp(num, Mathf.Max(SessionData.Instance.gameTime - 4f, num), Toolbox.Instance.Rand(0f, 0.75f, false));
		}
		else
		{
			num = Mathf.Lerp(num, Mathf.Max(SessionData.Instance.gameTime - 4f, num), Toolbox.Instance.SeedRand(0f, 0.75f));
		}
		text = messageTreeLinkRankings[0].linkRef.to;
		int j = Mathf.Min(addProgress, ddstreeSave.messageRef.Count);
		while (j > 0)
		{
			j--;
			if (!ddstreeSave.messageRef.TryGetValue(text, ref ddsmessageSettings))
			{
				break;
			}
			thread.messages.Add(ddsmessageSettings.instanceID);
			thread.timestamps.Add(num);
			if (ddsmessageSettings.saidBy <= 0)
			{
				thread.senders.Add(human.humanID);
			}
			else if (ddsmessageSettings.saidBy == 1)
			{
				thread.senders.Add(thread.participantB);
			}
			else if (ddsmessageSettings.saidBy == 2)
			{
				thread.senders.Add(thread.participantC);
			}
			else if (ddsmessageSettings.saidBy == 3)
			{
				thread.senders.Add(thread.participantD);
			}
			if (ddsmessageSettings.saidTo <= 0)
			{
				thread.recievers.Add(human.humanID);
			}
			else if (ddsmessageSettings.saidTo == 1)
			{
				thread.recievers.Add(thread.participantB);
			}
			else if (ddsmessageSettings.saidTo == 2)
			{
				thread.recievers.Add(thread.participantC);
			}
			else if (ddsmessageSettings.saidTo == 3)
			{
				thread.recievers.Add(thread.participantD);
			}
			List<Human.DDSRank> messageTreeLinkRankings2 = this.GetMessageTreeLinkRankings(thread, ddsmessageSettings);
			if (messageTreeLinkRankings2.Count <= 0)
			{
				break;
			}
			if (SessionData.Instance.startedGame)
			{
				num = Mathf.Lerp(num, Mathf.Max(SessionData.Instance.gameTime - 4f, num), Toolbox.Instance.Rand(0f, 0.5f, false));
			}
			else
			{
				num = Mathf.Lerp(num, Mathf.Max(SessionData.Instance.gameTime - 4f, num), Toolbox.Instance.SeedRand(0f, 0.5f));
			}
			text = messageTreeLinkRankings2[0].linkRef.to;
		}
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x0017AEE8 File Offset: 0x001790E8
	public bool GetVmailParticipant(Human initiator, DDSSaveClasses.DDSParticipant participant, List<Human> banned, out Human chosen)
	{
		chosen = null;
		int num = 4;
		string seedInput = initiator.humanID.ToString();
		while ((chosen == null || banned.Contains(chosen) || chosen == initiator) && num > 0)
		{
			num--;
			if (participant.connection == Acquaintance.ConnectionType.anyAcquaintance && initiator.acquaintances.Count > 0)
			{
				chosen = initiator.acquaintances[this.RandContained(0, initiator.acquaintances.Count, seedInput, out seedInput)].with;
			}
			else if (participant.connection == Acquaintance.ConnectionType.anyone || participant.connection == Acquaintance.ConnectionType.anyoneNotPlayer)
			{
				int num2 = 99;
				while (chosen == null || chosen == initiator)
				{
					if (CityData.Instance.citizenDirectory.Count > 0)
					{
						chosen = CityData.Instance.citizenDirectory[this.RandContained(0, CityData.Instance.citizenDirectory.Count, seedInput, out seedInput)];
					}
					num2--;
					if (num2 <= 0)
					{
						break;
					}
				}
			}
			else
			{
				if (participant.connection == Acquaintance.ConnectionType.relationshipMatch)
				{
					float num3 = -99999f;
					using (List<Citizen>.Enumerator enumerator = CityData.Instance.citizenDirectory.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Citizen citizen = enumerator.Current;
							float num4 = Toolbox.Instance.RandContained(0f, 5f, seedInput, out seedInput);
							if (citizen.partner == null)
							{
								num4 += 10f;
							}
							if (citizen.attractedTo.Contains(initiator.gender))
							{
								num4 += 10f;
							}
							if (num4 > num3)
							{
								num3 = num4;
								chosen = citizen;
							}
						}
						continue;
					}
				}
				if (participant.connection >= Acquaintance.ConnectionType.corpDove && participant.connection <= Acquaintance.ConnectionType.pestControl)
				{
					chosen = initiator;
					return true;
				}
				foreach (Acquaintance acquaintance in initiator.acquaintances)
				{
					if (acquaintance.connections.Contains(participant.connection))
					{
						chosen = acquaintance.with;
					}
					if (participant.connection == Acquaintance.ConnectionType.paramour && acquaintance.secretConnection == participant.connection)
					{
						chosen = acquaintance.with;
					}
				}
			}
		}
		return !(chosen == null);
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x0017B158 File Offset: 0x00179358
	public MaterialGroupPreset SelectMaterial(RoomClassPreset roomType, float wealthLevel, DesignStylePreset designStyle, MaterialGroupPreset.MaterialType materialType, string seedInput, out string seedOutput)
	{
		List<MaterialGroupPreset> list = new List<MaterialGroupPreset>();
		if (this.materialDesignStyleRef.ContainsKey(designStyle) && this.materialDesignStyleRef[designStyle].ContainsKey(materialType))
		{
			Predicate<MaterialGroupPreset.MaterialSettings> <>9__0;
			foreach (MaterialGroupPreset materialGroupPreset in this.materialDesignStyleRef[designStyle][materialType])
			{
				if (wealthLevel >= materialGroupPreset.minimumWealth)
				{
					bool flag = false;
					using (List<RoomTypeFilter>.Enumerator enumerator2 = materialGroupPreset.allowedRoomFilters.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.roomClasses.Contains(roomType))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						List<MaterialGroupPreset.MaterialSettings> designStyles = materialGroupPreset.designStyles;
						Predicate<MaterialGroupPreset.MaterialSettings> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((MaterialGroupPreset.MaterialSettings item) => item.designStyle == designStyle));
						}
						MaterialGroupPreset.MaterialSettings materialSettings = designStyles.Find(predicate);
						if (materialSettings != null)
						{
							for (int i = 0; i < materialSettings.weighting; i++)
							{
								list.Add(materialGroupPreset);
							}
						}
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			if (materialType == MaterialGroupPreset.MaterialType.floor || materialType == MaterialGroupPreset.MaterialType.other)
			{
				if (roomType.name != "Null")
				{
					Game.Log("CityGen: Using fallback floor material for " + roomType.name, 2);
				}
				list.Add(CityControls.Instance.fallbackFloorMat);
			}
			else if (materialType == MaterialGroupPreset.MaterialType.ceiling)
			{
				if (roomType.name != "Null")
				{
					Game.Log("CityGen: Using fallback ceiling material for " + roomType.name, 2);
				}
				list.Add(CityControls.Instance.fallbackCeilingMat);
			}
			else if (materialType == MaterialGroupPreset.MaterialType.walls)
			{
				if (roomType.name != "Null")
				{
					Game.Log("CityGen: Using fallback walls material for " + roomType.name, 2);
				}
				list.Add(CityControls.Instance.fallbackWallMat);
			}
		}
		return list[this.GetPsuedoRandomNumberContained(0, list.Count, seedInput, out seedOutput)];
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x0017B394 File Offset: 0x00179594
	public WallFrontagePreset SelectWallFrontage(DesignStylePreset designStyle, WallFrontageClass frontageClass, string seed)
	{
		if (this.wallFrontageStyleRef.ContainsKey(designStyle) && this.wallFrontageStyleRef[designStyle].ContainsKey(frontageClass))
		{
			string text;
			return this.wallFrontageStyleRef[designStyle][frontageClass][this.RandContained(0, this.wallFrontageStyleRef[designStyle][frontageClass].Count, seed, out text)];
		}
		return null;
	}

	// Token: 0x060019B0 RID: 6576 RVA: 0x0017B400 File Offset: 0x00179600
	public float GetNormalizedLandValue(NewGameLocation location, bool print = false)
	{
		float num = 0f;
		if (location.thisAsAddress != null)
		{
			num = location.thisAsAddress.normalizedLandValue;
			if (print)
			{
				Game.Log("Default land value: " + location.thisAsAddress.normalizedLandValue.ToString(), 2);
			}
			if (location.thisAsAddress.residence != null)
			{
				foreach (Human human in location.thisAsAddress.inhabitants)
				{
					if (human.societalClass > num)
					{
						num = Mathf.Max(new float[]
						{
							human.societalClass
						});
						if (print)
						{
							Game.Log("Owner " + human.name + " has greater societal class: " + human.societalClass.ToString(), 2);
						}
					}
				}
			}
			if (location.thisAsAddress.addressPreset != null)
			{
				num = Mathf.Clamp(num, location.thisAsAddress.addressPreset.minimumLandValue, location.thisAsAddress.addressPreset.maximumLandValue);
			}
		}
		else if (location.nodes.Count > 0)
		{
			num = (float)location.nodes[0].tile.cityTile.landValue / 4f;
		}
		return num;
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x0017B560 File Offset: 0x00179760
	public float GetNormalizedLandValue(NewBuilding location)
	{
		return (float)location.cityTile.landValue / 4f;
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x0017B574 File Offset: 0x00179774
	public List<Human> GetFingerprintOwnerPool(NewRoom room, FurnitureLocation furn, Interactable inter, RoomConfiguration.PrintsSource source, Vector3 worldPos, bool forceFind)
	{
		List<Human> list = new List<Human>();
		if (furn != null && room == null)
		{
			room = furn.anchorNode.room;
		}
		else if (inter != null && room == null && inter.node != null)
		{
			room = inter.node.room;
		}
		bool flag = false;
		int num = 8;
		if (!forceFind)
		{
			num = 1;
		}
		while (!flag && num > 0)
		{
			if (source == RoomConfiguration.PrintsSource.buildingResidents)
			{
				if (room != null)
				{
					NewFloor floor = room.floor;
					if (floor != null)
					{
						using (List<NewAddress>.Enumerator enumerator = floor.addresses.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								NewAddress newAddress = enumerator.Current;
								list.AddRange(newAddress.inhabitants);
							}
							goto IL_141;
						}
					}
					if (room.building != null)
					{
						foreach (KeyValuePair<int, NewFloor> keyValuePair in room.building.floors)
						{
							foreach (NewAddress newAddress2 in keyValuePair.Value.addresses)
							{
								list.AddRange(newAddress2.inhabitants);
							}
						}
					}
				}
				IL_141:
				source = RoomConfiguration.PrintsSource.inhabitants;
			}
			else if (source == RoomConfiguration.PrintsSource.inhabitants)
			{
				if (room != null && room.gameLocation.thisAsAddress != null)
				{
					list.AddRange(room.gameLocation.thisAsAddress.inhabitants);
				}
				source = RoomConfiguration.PrintsSource.publicAll;
			}
			else if (source == RoomConfiguration.PrintsSource.inhabitantsAndCustomers)
			{
				if (room != null)
				{
					if (room.gameLocation.thisAsAddress != null)
					{
						list.AddRange(room.gameLocation.thisAsAddress.inhabitants);
					}
					if (room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.favouredCustomers.Count > 0)
					{
						list.AddRange(room.gameLocation.thisAsAddress.favouredCustomers);
					}
				}
				source = RoomConfiguration.PrintsSource.publicAll;
			}
			else if (source == RoomConfiguration.PrintsSource.owners)
			{
				if (inter != null && inter.belongsTo != null)
				{
					list.Add(inter.belongsTo);
				}
				else
				{
					if (furn != null && furn.ownerMap != null && furn.ownerMap.Count > 0)
					{
						using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator3 = furn.ownerMap.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair2 = enumerator3.Current;
								if (keyValuePair2.Key.human != null)
								{
									list.Add(keyValuePair2.Key.human);
								}
								else if (keyValuePair2.Key.address != null)
								{
									foreach (Human human in keyValuePair2.Key.address.owners)
									{
										list.Add(human);
									}
								}
							}
							goto IL_32D;
						}
					}
					if (room != null && room.belongsTo.Count > 0)
					{
						list.AddRange(room.belongsTo);
					}
				}
				IL_32D:
				source = RoomConfiguration.PrintsSource.inhabitants;
			}
			else if (source == RoomConfiguration.PrintsSource.writers)
			{
				if (inter != null && inter.writer != null)
				{
					list.Add(inter.writer);
				}
				else
				{
					if (furn != null && furn.ownerMap != null && furn.ownerMap.Count > 0)
					{
						using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator3 = furn.ownerMap.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair3 = enumerator3.Current;
								if (keyValuePair3.Key.human != null)
								{
									list.Add(keyValuePair3.Key.human);
								}
								else if (keyValuePair3.Key.address != null)
								{
									foreach (Human human2 in keyValuePair3.Key.address.owners)
									{
										list.Add(human2);
									}
								}
							}
							goto IL_45B;
						}
					}
					if (room != null && room.belongsTo.Count > 0)
					{
						list.AddRange(room.belongsTo);
					}
				}
				IL_45B:
				source = RoomConfiguration.PrintsSource.owners;
			}
			else if (source == RoomConfiguration.PrintsSource.receivers)
			{
				if (inter != null && inter.reciever != null)
				{
					list.Add(inter.reciever);
				}
				else
				{
					if (furn != null && furn.ownerMap != null && furn.ownerMap.Count > 0)
					{
						using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator3 = furn.ownerMap.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair4 = enumerator3.Current;
								if (keyValuePair4.Key.human != null)
								{
									list.Add(keyValuePair4.Key.human);
								}
								else if (keyValuePair4.Key.address != null)
								{
									foreach (Human human3 in keyValuePair4.Key.address.owners)
									{
										list.Add(human3);
									}
								}
							}
							goto IL_58A;
						}
					}
					if (room != null && room.belongsTo.Count > 0)
					{
						list.AddRange(room.belongsTo);
					}
				}
				IL_58A:
				source = RoomConfiguration.PrintsSource.owners;
			}
			else if (source == RoomConfiguration.PrintsSource.ownersAndWriters)
			{
				if (inter != null && (inter.reciever != null || inter.writer != null))
				{
					if (inter.writer != null)
					{
						list.Add(inter.writer);
					}
					if (inter.reciever != null)
					{
						list.Add(inter.reciever);
					}
				}
				else
				{
					if (furn != null && furn.ownerMap != null && furn.ownerMap.Count > 0)
					{
						using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator3 = furn.ownerMap.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair5 = enumerator3.Current;
								if (keyValuePair5.Key.human != null)
								{
									list.Add(keyValuePair5.Key.human);
								}
								else if (keyValuePair5.Key.address != null)
								{
									foreach (Human human4 in keyValuePair5.Key.address.owners)
									{
										list.Add(human4);
									}
								}
							}
							goto IL_6F2;
						}
					}
					if (room != null && room.belongsTo.Count > 0)
					{
						list.AddRange(room.belongsTo);
					}
				}
				IL_6F2:
				source = RoomConfiguration.PrintsSource.owners;
			}
			else if (source == RoomConfiguration.PrintsSource.ownersWritersReceivers)
			{
				if (inter != null && (inter.reciever != null || inter.writer != null || inter.belongsTo != null))
				{
					if (inter.belongsTo != null)
					{
						list.Add(inter.belongsTo);
					}
					if (inter.writer != null)
					{
						list.Add(inter.writer);
					}
					if (inter.reciever != null)
					{
						list.Add(inter.reciever);
					}
				}
				else
				{
					if (furn != null && furn.ownerMap != null && furn.ownerMap.Count > 0)
					{
						using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator3 = furn.ownerMap.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair6 = enumerator3.Current;
								if (keyValuePair6.Key.human != null)
								{
									list.Add(keyValuePair6.Key.human);
								}
								else if (keyValuePair6.Key.address != null)
								{
									foreach (Human human5 in keyValuePair6.Key.address.owners)
									{
										list.Add(human5);
									}
								}
							}
							goto IL_885;
						}
					}
					if (room != null && room.belongsTo.Count > 0)
					{
						list.AddRange(room.belongsTo);
					}
				}
				IL_885:
				source = RoomConfiguration.PrintsSource.owners;
			}
			else if (source == RoomConfiguration.PrintsSource.customersAll)
			{
				if (room != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.favouredCustomers.Count > 0)
				{
					list.AddRange(room.gameLocation.thisAsAddress.favouredCustomers);
				}
				source = RoomConfiguration.PrintsSource.publicAll;
			}
			else if (source == RoomConfiguration.PrintsSource.customersMale)
			{
				if (room != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.favouredCustomers.Count > 0)
				{
					List<Human> list2 = room.gameLocation.thisAsAddress.favouredCustomers.FindAll((Human item) => item.gender == Human.Gender.male || (item.gender == Human.Gender.nonBinary && item.genderScale >= 0.5f));
					if (list2.Count > 0)
					{
						list.AddRange(list2);
					}
				}
				source = RoomConfiguration.PrintsSource.inhabitants;
			}
			else if (source == RoomConfiguration.PrintsSource.customersFemale)
			{
				if (room != null && room.gameLocation.thisAsAddress != null && room.gameLocation.thisAsAddress.favouredCustomers.Count > 0)
				{
					List<Human> list3 = room.gameLocation.thisAsAddress.favouredCustomers.FindAll((Human item) => item.gender == Human.Gender.female || (item.gender == Human.Gender.nonBinary && item.genderScale < 0.5f));
					if (list3.Count > 0)
					{
						list.AddRange(list3);
					}
				}
				source = RoomConfiguration.PrintsSource.inhabitants;
			}
			else if (source == RoomConfiguration.PrintsSource.publicAll)
			{
				list.AddRange(CityData.Instance.citizenDirectory);
			}
			else if (source == RoomConfiguration.PrintsSource.killer && MurderController.Instance.currentMurderer != null)
			{
				list.Add(MurderController.Instance.currentMurderer);
			}
			if (list.Count > 0)
			{
				flag = true;
			}
			num--;
		}
		if (list.Count <= 0)
		{
			list.Add(CityData.Instance.citizenDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.citizenDirectory.Count, worldPos.ToString(), false)]);
		}
		return list;
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x0017C0BC File Offset: 0x0017A2BC
	public void SpawnWindowAfterSeconds(Evidence ev, float after)
	{
		base.StartCoroutine(this.SpawnTelephoneEntryWindow(ev, after));
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x0017C0CD File Offset: 0x0017A2CD
	private IEnumerator SpawnTelephoneEntryWindow(Evidence ev, float after)
	{
		float timer = 0f;
		while (timer < 2f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		SessionData.Instance.PauseGame(true, false, true);
		InterfaceController.Instance.SpawnWindow(ev, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
		yield break;
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x0017C0DC File Offset: 0x0017A2DC
	public CityInfoData GenerateCityInfoFile(FileInfo citySave)
	{
		if (citySave == null)
		{
			return null;
		}
		CitySaveData citySaveData = null;
		using (StreamReader streamReader = File.OpenText(citySave.FullName))
		{
			citySaveData = JsonUtility.FromJson<CitySaveData>(streamReader.ReadToEnd());
		}
		if (citySaveData != null)
		{
			CityInfoData cityInfoData = new CityInfoData();
			cityInfoData.cityName = citySaveData.cityName;
			cityInfoData.build = Game.Instance.buildID;
			cityInfoData.shareCode = Toolbox.Instance.GetShareCode(ref citySaveData);
			cityInfoData.citySize = citySaveData.citySize;
			cityInfoData.population = citySaveData.population;
			string text = citySave.FullName.Substring(0, citySave.FullName.Length - 4) + ".txt";
			string text2 = JsonUtility.ToJson(cityInfoData, true);
			using (StreamWriter streamWriter = File.CreateText(text))
			{
				streamWriter.Write(text2);
			}
			Game.Log("Generate city info file: " + text, 2);
			return cityInfoData;
		}
		return null;
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x0017C1E4 File Offset: 0x0017A3E4
	public string GetTelephoneNumberString(int number)
	{
		string text = number.ToString();
		string text2 = string.Empty;
		int num = 0;
		for (int i = 0; i < text.Length; i++)
		{
			text2 += text.get_Chars(i).ToString();
			num++;
			if (num >= 3 && i < text.Length - 1)
			{
				num = -1;
				text2 += "—";
			}
		}
		return text2;
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x0017C24C File Offset: 0x0017A44C
	public int GetLockpicksNeeded(float lockStrength)
	{
		bool flag = false;
		int num = 0;
		while (lockStrength > 0f)
		{
			float num2 = Mathf.LerpUnclamped(GameplayControls.Instance.lockpickEffectivenessRange.x, GameplayControls.Instance.lockpickEffectivenessRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingEfficiencyModifier));
			float num3 = num2;
			if (!flag)
			{
				num3 = GameplayController.Instance.currentLockpickStrength * num2;
				flag = true;
			}
			lockStrength -= num3;
			num++;
		}
		return num;
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x0017C2B8 File Offset: 0x0017A4B8
	public Vector2 CreateTimeRange(float actualTime, float accuracyMargin, bool limitToNow, bool round, int roundToMinutes)
	{
		float num = this.Rand(0f, 1f, false);
		float num2 = actualTime - num * accuracyMargin;
		float num3 = actualTime + (1f - num * accuracyMargin);
		if (limitToNow && num3 > SessionData.Instance.gameTime)
		{
			float num4 = num3 - SessionData.Instance.gameTime;
			num2 -= num4;
			num3 = SessionData.Instance.gameTime;
		}
		if (round)
		{
			float num5 = (float)roundToMinutes * 0.016666668f;
			num2 = (float)Mathf.FloorToInt(num2 / num5) * num5;
			num3 = (float)Mathf.CeilToInt(num3 / num5) * num5;
		}
		return new Vector2(num2, num3);
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x0017C347 File Offset: 0x0017A547
	public void ScrollScrollRectOLD(CustomScrollRect scrollRect, Vector3 targetPos, bool allowHorizontal, bool allowVertical, float timeTaken = 0.2f, float extraScrollThreshold = 0.2f)
	{
		base.StartCoroutine(this.ExecuteScrollScrollRectOLD(scrollRect, targetPos, allowHorizontal, allowVertical, timeTaken, extraScrollThreshold));
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x0017C35F File Offset: 0x0017A55F
	private IEnumerator ExecuteScrollScrollRectOLD(CustomScrollRect scrollRect, Vector3 targetPos, bool allowHorizontal, bool allowVertical, float timeTaken = 0.2f, float extraScrollThreshold = 0.2f)
	{
		float progress = 0f;
		Canvas.ForceUpdateCanvases();
		Vector3 vector = scrollRect.content.InverseTransformPoint(targetPos) + new Vector3(scrollRect.content.sizeDelta.x * scrollRect.content.pivot.x, scrollRect.content.sizeDelta.y * scrollRect.content.pivot.y, 0f);
		Vector2 vector2;
		vector2..ctor(Mathf.Clamp01(vector.x / scrollRect.content.sizeDelta.x), Mathf.Clamp01(vector.y / scrollRect.content.sizeDelta.y));
		float num = scrollRect.horizontalNormalizedPosition;
		if (allowHorizontal)
		{
			float num2 = (0.5f - vector2.x) * 2f * extraScrollThreshold;
			num = Mathf.Clamp(vector2.x - num2, 0f, 1f);
		}
		float num3 = scrollRect.verticalNormalizedPosition;
		if (allowVertical)
		{
			float num4 = (0.5f - vector2.y) * 2f * extraScrollThreshold;
			num3 = Mathf.Clamp(vector2.y - num4, 0f, 1f);
		}
		Vector2 newPos = new Vector2(num, num3);
		string text = "Scroll rect pos: ";
		Vector2 vector3 = newPos;
		Game.Log(text + vector3.ToString(), 2);
		while (progress < 1f)
		{
			progress += Time.deltaTime / timeTaken;
			scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, newPos, progress);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x0017C394 File Offset: 0x0017A594
	public void ScrollRectPosition(CustomScrollRect scrollRect, RectTransform target, bool allowHorizontal, bool allowVertical, float timeTaken = 0.2f)
	{
		Canvas.ForceUpdateCanvases();
		if (!scrollRect.horizontal)
		{
			allowHorizontal = false;
		}
		if (!scrollRect.vertical)
		{
			allowVertical = false;
		}
		Vector2 vector = new Vector2(scrollRect.viewport.rect.width, scrollRect.viewport.rect.height) * -1f;
		vector *= scrollRect.content.pivot - new Vector2(0.5f, 0.5f);
		Vector2 vector2 = scrollRect.transform.InverseTransformPoint(scrollRect.content.position) - scrollRect.transform.InverseTransformPoint(target.position) + vector;
		Vector2 vector3;
		vector3..ctor((scrollRect.content.sizeDelta.y - scrollRect.viewport.rect.height) * -(1f - scrollRect.content.pivot.y), (scrollRect.content.sizeDelta.y - scrollRect.viewport.rect.height) * scrollRect.content.pivot.y);
		Vector2 vector4;
		vector4..ctor((scrollRect.content.sizeDelta.x - scrollRect.viewport.rect.width) * -(1f - scrollRect.content.pivot.x), (scrollRect.content.sizeDelta.x - scrollRect.viewport.rect.width) * scrollRect.content.pivot.x);
		vector2..ctor(Mathf.Clamp(vector2.x, vector4.x, vector4.y), Mathf.Clamp(vector2.y, vector3.x, vector3.y));
		if (!allowHorizontal)
		{
			vector2.x = scrollRect.content.anchoredPosition.x;
		}
		if (!allowVertical)
		{
			vector2.y = scrollRect.content.anchoredPosition.y;
		}
		base.StartCoroutine(this.LerpScrollRect(scrollRect, vector2, timeTaken));
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x0017C5C6 File Offset: 0x0017A7C6
	private IEnumerator LerpScrollRect(CustomScrollRect scrollRect, Vector2 anchoredPos, float timeTaken = 0.2f)
	{
		float progress = 0f;
		Canvas.ForceUpdateCanvases();
		while (progress < 1f)
		{
			progress += Time.deltaTime / timeTaken;
			progress = Mathf.Clamp01(progress);
			scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, anchoredPos, progress);
			yield return null;
		}
		scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, anchoredPos, progress);
		yield break;
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x0017C5E4 File Offset: 0x0017A7E4
	public NewNode PickNearbyNode(NewNode toThis)
	{
		HashSet<NewNode> hashSet = new HashSet<NewNode>();
		HashSet<NewNode> hashSet2 = new HashSet<NewNode>();
		hashSet.Add(toThis);
		List<NewNode> list = new List<NewNode>();
		int num = 20;
		while (hashSet.Count > 0 && num > 0)
		{
			NewNode newNode = Enumerable.FirstOrDefault<NewNode>(hashSet);
			if (newNode != toThis)
			{
				list.Add(newNode);
			}
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode.accessToOtherNodes)
			{
				if (keyValuePair.Value.walkingAccess && !hashSet2.Contains(keyValuePair.Key) && !hashSet.Contains(keyValuePair.Key))
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
			hashSet.Remove(newNode);
			hashSet2.Add(newNode);
			num--;
			if (list.Count >= 4)
			{
				break;
			}
		}
		if (list.Count > 0)
		{
			return list[Toolbox.Instance.Rand(0, list.Count, false)];
		}
		return null;
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x0017C6F8 File Offset: 0x0017A8F8
	public NewNode GetDoorSideNode(NewNode currentNode, NewDoor door)
	{
		if (door.wall.node.room == currentNode.room)
		{
			return door.wall.node;
		}
		if (door.wall.otherWall.node.room == currentNode.room)
		{
			return door.wall.otherWall.node;
		}
		if (door.wall.node.gameLocation == currentNode.gameLocation)
		{
			return door.wall.node;
		}
		if (door.wall.otherWall.node.gameLocation == currentNode.gameLocation)
		{
			return door.wall.otherWall.node;
		}
		Dictionary<NewRoom, NewNode> dictionary = new Dictionary<NewRoom, NewNode>();
		dictionary.Add(door.wall.node.room, door.wall.node);
		dictionary.Add(door.wall.otherWall.node.room, door.wall.otherWall.node);
		Dictionary<NewRoom, NewNode> dictionary2 = new Dictionary<NewRoom, NewNode>();
		int num = 800;
		while (dictionary.Count > 0 && num > 0)
		{
			KeyValuePair<NewRoom, NewNode> keyValuePair = Enumerable.FirstOrDefault<KeyValuePair<NewRoom, NewNode>>(dictionary);
			if (currentNode.room == keyValuePair.Key)
			{
				return keyValuePair.Value;
			}
			foreach (NewNode.NodeAccess nodeAccess in keyValuePair.Key.entrances)
			{
				if (!(nodeAccess.door == door) && nodeAccess.walkingAccess)
				{
					if (!dictionary.ContainsKey(nodeAccess.fromNode.room) && !dictionary2.ContainsKey(nodeAccess.fromNode.room))
					{
						dictionary.Add(nodeAccess.fromNode.room, keyValuePair.Value);
					}
					if (!dictionary.ContainsKey(nodeAccess.toNode.room) && !dictionary2.ContainsKey(nodeAccess.toNode.room))
					{
						dictionary.Add(nodeAccess.toNode.room, keyValuePair.Value);
					}
				}
			}
			dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
			dictionary.Remove(keyValuePair.Key);
			num--;
		}
		return door.wall.node;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x0017C974 File Offset: 0x0017AB74
	[Button(null, 0)]
	public void TestTimeRangeOverlap()
	{
		Game.Log(this.DecimalTimeRangeOverlap(this.debugTimeRange1, this.debugTimeRange2, true), 2);
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x0017C994 File Offset: 0x0017AB94
	public void AutomaticNavigationSetup(ref List<Button> selectables, float differenceBuffer = 2f)
	{
		foreach (Button button in selectables)
		{
			Navigation navigation = default(Navigation);
			navigation.mode = 4;
			Vector3 position = button.transform.position;
			for (int i = 0; i < 4; i++)
			{
				Selectable selectable = null;
				float num = float.PositiveInfinity;
				foreach (Selectable selectable2 in selectables)
				{
					if (!(selectable2 == button))
					{
						Vector3 position2 = selectable2.transform.position;
						Vector3 vector = position2 - position;
						float num2 = Vector3.Distance(position2, position);
						if (i == 0 && vector.x > differenceBuffer)
						{
							if (num2 < num)
							{
								selectable = selectable2;
								num = num2;
							}
						}
						else if (i == 1 && vector.x < -differenceBuffer)
						{
							if (num2 < num)
							{
								selectable = selectable2;
								num = num2;
							}
						}
						else if (i == 2 && vector.y > differenceBuffer)
						{
							if (num2 < num)
							{
								selectable = selectable2;
								num = num2;
							}
						}
						else if (i == 3 && vector.y < -differenceBuffer && num2 < num)
						{
							selectable = selectable2;
							num = num2;
						}
					}
				}
				if (selectable != null)
				{
					if (i == 0)
					{
						navigation.selectOnRight = selectable;
					}
					else if (i == 1)
					{
						navigation.selectOnLeft = selectable;
					}
					else if (i == 2)
					{
						navigation.selectOnUp = selectable;
					}
					else
					{
						navigation.selectOnDown = selectable;
					}
				}
			}
			button.navigation = navigation;
		}
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x0017CB64 File Offset: 0x0017AD64
	public void AddNavigationInput(Selectable selectable, Selectable newLeft = null, Selectable newRight = null, Selectable newUp = null, Selectable newDown = null, bool clearOld = false)
	{
		Navigation navigation = default(Navigation);
		navigation.mode = 4;
		Navigation navigation2 = navigation;
		if (!clearOld)
		{
			navigation2.selectOnLeft = selectable.FindSelectableOnLeft();
			navigation2.selectOnRight = selectable.FindSelectableOnRight();
			navigation2.selectOnUp = selectable.FindSelectableOnUp();
			navigation2.selectOnDown = selectable.FindSelectableOnDown();
		}
		if (newLeft != null)
		{
			navigation2.selectOnLeft = newLeft;
		}
		if (newRight != null)
		{
			navigation2.selectOnRight = newRight;
		}
		if (newUp != null)
		{
			navigation2.selectOnUp = newUp;
		}
		if (newDown != null)
		{
			navigation2.selectOnDown = newDown;
		}
		if (selectable != null)
		{
			selectable.navigation = navigation2;
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x00002265 File Offset: 0x00000465
	public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
	{
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x0017CC14 File Offset: 0x0017AE14
	public bool GetRelocateAuthority(Actor actor, Interactable obj)
	{
		if (actor == Player.Instance && obj.preset.relocateIfPlacedInPlayersHome && actor.currentGameLocation == Player.Instance.home)
		{
			return true;
		}
		if (obj.preset.relocationAuthority == InteractablePreset.RelocationAuthority.AIAndOwnersCanRelocate)
		{
			if (actor.ai != null || obj.belongsTo == actor)
			{
				return true;
			}
		}
		else
		{
			if (obj.preset.relocationAuthority == InteractablePreset.RelocationAuthority.anyoneCanRelocate)
			{
				return true;
			}
			if (obj.preset.relocationAuthority == InteractablePreset.RelocationAuthority.nooneCanRelocate)
			{
				return false;
			}
			if (obj.preset.relocationAuthority == InteractablePreset.RelocationAuthority.ownerCanRelocate && obj.belongsTo == actor)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x0017CCC0 File Offset: 0x0017AEC0
	public NewNode GetNearestGroundLevelOutside(Vector3 pos)
	{
		return this.FindClosestValidNodeToWorldPosition(pos, false, false, false, default(Vector3Int), true, 0, true, 400);
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x0017CCE8 File Offset: 0x0017AEE8
	public void HandleLaserBehaviour(SecuritySystem secSystem, GameObject laser, Light laserLight, float maxRange = 16f)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(laser.transform.position, laser.transform.forward), ref raycastHit, maxRange, Toolbox.Instance.aiSightingLayerMask, 2))
		{
			Transform parent = laser.transform.parent;
			laser.transform.parent = null;
			laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, raycastHit.distance);
			laser.transform.parent = parent;
			if (laserLight != null)
			{
				laserLight.range = raycastHit.distance;
			}
			laser.name = raycastHit.transform.name + " (" + raycastHit.distance.ToString() + ")";
			Human human = null;
			Player componentInChildren = raycastHit.transform.GetComponentInChildren<Player>();
			if (componentInChildren != null)
			{
				human = componentInChildren;
			}
			else
			{
				Citizen componentInChildren2 = raycastHit.transform.GetComponentInChildren<Citizen>();
				if (componentInChildren2 != null)
				{
					human = componentInChildren2;
				}
			}
			if (human != null && secSystem != null && human.currentNode != null && human.currentNode.gameLocation.thisAsAddress != null && human.currentNode.gameLocation.thisAsAddress.IsAlarmSystemTarget(human) && (!human.isPlayer || !Game.Instance.invisiblePlayer))
			{
				human.currentNode.gameLocation.thisAsAddress.SetAlarm(true, human);
				if (!secSystem.seenIllegalThisCheck.Contains(human))
				{
					secSystem.seenIllegalThisCheck.Add(human);
				}
				if (!secSystem.seesIllegal.ContainsKey(human))
				{
					secSystem.seesIllegal.Add(human, 1f);
				}
				else
				{
					secSystem.seesIllegal[human] = 1f;
				}
				secSystem.OnInvestigate(human, 2);
				return;
			}
		}
		else
		{
			laser.name = "Laser hit null (" + maxRange.ToString() + ")";
			Transform parent2 = laser.transform.parent;
			laser.transform.parent = null;
			laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, maxRange);
			if (laserLight != null)
			{
				laserLight.range = maxRange;
			}
			laser.transform.parent = parent2;
		}
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x0017CF64 File Offset: 0x0017B164
	public Interactable GetLocalizedSnapshot(Interactable obj)
	{
		Interactable interactable = null;
		NewNode newNode = obj.node;
		float num = -999999f;
		string print = string.Empty;
		if (obj.node.gameLocation != null)
		{
			List<NewNode> list = new List<NewNode>();
			foreach (NewRoom newRoom in obj.node.gameLocation.rooms)
			{
				if (newRoom.nodes.Count > 0 && newRoom.entrances.Count > 0)
				{
					foreach (NewNode newNode2 in newRoom.nodes)
					{
						list.Add(newNode2);
					}
				}
			}
			Game.Log("Evaluating " + list.Count.ToString() + " nodes...", 2);
			foreach (NewNode newNode3 in list)
			{
				float num2 = Toolbox.Instance.Rand(0f, 5f, false);
				float num3 = Vector3.Distance(newNode3.position, obj.node.position);
				float num4 = Mathf.InverseLerp(GameplayControls.Instance.missionPhotoMinMaxDistance.x, GameplayControls.Instance.missionPhotoMinMaxDistance.y, num3);
				num2 += GameplayControls.Instance.missionPhotoDistanceScoreCurve.Evaluate(num4);
				float num5 = 0f;
				foreach (FurnitureLocation furnitureLocation in newNode3.room.individualFurniture)
				{
					if (furnitureLocation.coversNodes.Contains(newNode3) && furnitureLocation.furniture.classes.Count > 0)
					{
						if (furnitureLocation.furniture.classes[0].discourageMissionPhotos)
						{
							num5 -= 1000f;
						}
						if (furnitureLocation.furniture.classes[0].occupiesTile)
						{
							num5 -= 10f;
						}
						if (furnitureLocation.furniture.classes[0].wallPiece)
						{
							num5 -= 5f;
						}
					}
				}
				num2 += num5;
				using (HashSet<Actor>.Enumerator enumerator5 = newNode3.room.currentOccupants.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						if (enumerator5.Current.currentNode == newNode3)
						{
							num2 -= 10f;
						}
					}
				}
				List<DataRaycastController.NodeRaycastHit> list2;
				if (DataRaycastController.Instance.NodeRaycast(newNode3, obj.node, out list2, null, false))
				{
					num2 += 100f;
				}
				if (num2 > num)
				{
					newNode = newNode3;
					num = num2;
					print = string.Concat(new string[]
					{
						"Score: ",
						num.ToString(),
						" (Distance norm: ",
						num4.ToString(),
						" = ",
						Mathf.InverseLerp(GameplayControls.Instance.missionPhotoMinMaxDistance.x, GameplayControls.Instance.missionPhotoMinMaxDistance.y, num3).ToString(),
						") furn : ",
						num5.ToString(),
						")"
					});
					Game.Log(print, 2);
				}
			}
		}
		Game.Log(print, 2);
		if (newNode != null)
		{
			NewNode currentNode = Player.Instance.currentNode;
			Player.Instance.interactable.node = newNode;
			Player.Instance.interactable.cvp = newNode.position + new Vector3(0f, Toolbox.Instance.Rand(1.6f, 2f, false), 0f);
			Vector3 normalized = (obj.node.position + new Vector3(Toolbox.Instance.Rand(-1.2f, 1.2f, false), Toolbox.Instance.Rand(1f, 3f, false), Toolbox.Instance.Rand(-1.2f, 1.2f, false)) - Player.Instance.interactable.cvp).normalized;
			Player.Instance.interactable.cve = Quaternion.LookRotation(normalized, Vector3.up).eulerAngles;
			SceneRecorder.SceneCapture sceneCapture = Player.Instance.sceneRecorder.ExecuteCapture(false, true, true);
			string[] array = new string[6];
			array[0] = "Player: New capture: ";
			int num6 = 1;
			SceneRecorder.SceneCapture sceneCapture2 = sceneCapture;
			array[num6] = ((sceneCapture2 != null) ? sceneCapture2.ToString() : null);
			array[2] = " from ";
			int num7 = 3;
			Vector3 vector = Player.Instance.interactable.cvp;
			array[num7] = vector.ToString();
			array[4] = ", euler ";
			int num8 = 5;
			vector = Player.Instance.interactable.cve;
			array[num8] = vector.ToString();
			Game.Log(string.Concat(array), 2);
			Player.Instance.sceneRecorder.interactable.sCap.Add(sceneCapture);
			Interactable.Passed passed = new Interactable.Passed(Interactable.PassedVarType.savedSceneCapID, (float)sceneCapture.id, null);
			List<Interactable.Passed> list3 = new List<Interactable.Passed>();
			list3.Add(passed);
			interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.surveillancePrintout, Player.Instance, Player.Instance, null, Player.Instance.transform.position, Vector3.zero, list3, null, "");
			if (interactable != null)
			{
				interactable.RemoveFromPlacement();
				interactable.MarkAsTrash(true, false, 0f);
			}
			Player.Instance.interactable.node = currentNode;
		}
		else
		{
			Game.LogError("Unable to find a valid capture node!", 2);
		}
		return interactable;
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x0017D590 File Offset: 0x0017B790
	public void RetroactiveSurveillanceAddition(Human who, NewNode routeFrom, NewNode routeTo, bool addReturnJourney, NewNode returnTo, float arrivalTime, float stayTime, ClothesPreset.OutfitCategory outfit)
	{
		PathFinder.PathData path = PathFinder.Instance.GetPath(routeFrom, routeTo, who, null);
		if (path != null)
		{
			Dictionary<NewRoom, List<NewNode>> dictionary = new Dictionary<NewRoom, List<NewNode>>();
			Dictionary<NewNode, List<Interactable>> dictionary2 = new Dictionary<NewNode, List<Interactable>>();
			foreach (NewNode.NodeAccess nodeAccess in path.accessList)
			{
				if (nodeAccess.toNode.gameLocation.securityCameras.Count > 0)
				{
					foreach (Interactable interactable in nodeAccess.toNode.gameLocation.securityCameras)
					{
						if (interactable.sceneRecorder.coveredNodes.ContainsKey(nodeAccess.toNode))
						{
							if (!dictionary.ContainsKey(nodeAccess.toNode.room))
							{
								dictionary.Add(nodeAccess.toNode.room, new List<NewNode>());
							}
							dictionary[nodeAccess.toNode.room].Add(nodeAccess.toNode);
							if (!dictionary2.ContainsKey(nodeAccess.toNode))
							{
								dictionary2.Add(nodeAccess.toNode, new List<Interactable>());
							}
							dictionary2[nodeAccess.toNode].Add(interactable);
						}
					}
				}
			}
			float backwardsTime = arrivalTime;
			HashSet<SceneRecorder.SceneCapture> addedToCaptures = new HashSet<SceneRecorder.SceneCapture>();
			NewNode newNode = null;
			Predicate<SceneRecorder.ActorCapture> <>9__2;
			Predicate<SceneRecorder.SceneCapture> <>9__0;
			Predicate<SceneRecorder.SceneCapture> <>9__1;
			for (int i = path.accessList.Count - 1; i > 0; i--)
			{
				NewNode fromNode = path.accessList[i].fromNode;
				if (dictionary2.ContainsKey(fromNode))
				{
					Vector3 vector = Vector3.zero;
					if (newNode != null && fromNode != null)
					{
						vector = newNode.position - fromNode.position;
					}
					vector.y = 0f;
					foreach (Interactable interactable2 in dictionary2[fromNode])
					{
						List<SceneRecorder.SceneCapture> cap = interactable2.cap;
						Predicate<SceneRecorder.SceneCapture> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((SceneRecorder.SceneCapture item) => item.t > backwardsTime - 0.0167f && item.t < backwardsTime + 0.0167f && !addedToCaptures.Contains(item)));
						}
						List<SceneRecorder.SceneCapture> list = cap.FindAll(predicate);
						if (list.Count > 0)
						{
							using (List<SceneRecorder.SceneCapture>.Enumerator enumerator3 = list.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									SceneRecorder.SceneCapture sceneCapture = enumerator3.Current;
									if (!addedToCaptures.Contains(sceneCapture))
									{
										List<SceneRecorder.ActorCapture> aCap = sceneCapture.aCap;
										Predicate<SceneRecorder.ActorCapture> predicate2;
										if ((predicate2 = <>9__2) == null)
										{
											predicate2 = (<>9__2 = ((SceneRecorder.ActorCapture item) => item.id == who.humanID));
										}
										int num = aCap.FindIndex(predicate2);
										if (num > -1)
										{
											SceneRecorder.ActorCapture actorCapture = sceneCapture.aCap[num];
											actorCapture.o = (int)outfit;
											actorCapture.pos = fromNode.position;
											actorCapture.rot = Quaternion.LookRotation(vector).eulerAngles;
											actorCapture.sp = 1;
										}
										else
										{
											SceneRecorder.ActorCapture actorCapture2 = new SceneRecorder.ActorCapture(who, false);
											actorCapture2.o = (int)outfit;
											actorCapture2.pos = fromNode.position;
											actorCapture2.rot = Quaternion.LookRotation(vector).eulerAngles;
											actorCapture2.sp = 1;
											actorCapture2.arms = 0;
											actorCapture2.main = 0;
											sceneCapture.aCap.Add(actorCapture2);
										}
										addedToCaptures.Add(sceneCapture);
									}
								}
								continue;
							}
						}
						List<SceneRecorder.SceneCapture> cap2 = interactable2.cap;
						Predicate<SceneRecorder.SceneCapture> predicate3;
						if ((predicate3 = <>9__1) == null)
						{
							predicate3 = (<>9__1 = ((SceneRecorder.SceneCapture item) => item.t > backwardsTime - GameplayControls.Instance.captureInterval * 0.6f && item.t < backwardsTime + GameplayControls.Instance.captureInterval * 0.6f && !addedToCaptures.Contains(item)));
						}
						list = cap2.FindAll(predicate3);
						if (list == null || list.Count <= 0)
						{
							SceneRecorder.SceneCapture sceneCapture2 = null;
							foreach (SceneRecorder.SceneCapture sceneCapture3 in interactable2.cap)
							{
								if (sceneCapture3.t <= backwardsTime && (sceneCapture2 == null || sceneCapture3.t > sceneCapture2.t))
								{
									sceneCapture2 = sceneCapture3;
								}
							}
							if (sceneCapture2 != null)
							{
								SceneRecorder.SceneCapture sceneCapture4 = new SceneRecorder.SceneCapture(sceneCapture2);
								sceneCapture4.t = backwardsTime;
								sceneCapture4.aCap = new List<SceneRecorder.ActorCapture>();
								SceneRecorder.ActorCapture actorCapture3 = new SceneRecorder.ActorCapture(who, false);
								actorCapture3.o = (int)outfit;
								actorCapture3.pos = fromNode.position;
								actorCapture3.rot = Quaternion.LookRotation(vector).eulerAngles;
								actorCapture3.sp = 1;
								actorCapture3.arms = 0;
								actorCapture3.main = 0;
								sceneCapture4.aCap.Add(actorCapture3);
								interactable2.cap.Add(sceneCapture4);
								interactable2.cap.Sort((SceneRecorder.SceneCapture p1, SceneRecorder.SceneCapture p2) => p1.t.CompareTo(p2.t));
							}
						}
					}
				}
				backwardsTime -= 0.005f;
				newNode = fromNode;
			}
			if (addReturnJourney)
			{
				if (returnTo == null)
				{
					returnTo = routeFrom;
				}
				if (returnTo != routeFrom)
				{
					path = PathFinder.Instance.GetPath(routeTo, returnTo, who, null);
				}
				if (path != null)
				{
					float forwardsTime = arrivalTime + stayTime;
					addedToCaptures = new HashSet<SceneRecorder.SceneCapture>();
					NewNode newNode2 = null;
					Predicate<SceneRecorder.ActorCapture> <>9__6;
					Predicate<SceneRecorder.SceneCapture> <>9__4;
					Predicate<SceneRecorder.SceneCapture> <>9__5;
					for (int j = path.accessList.Count - 1; j > 0; j--)
					{
						NewNode fromNode2 = path.accessList[j].fromNode;
						if (j < path.accessList.Count - 1)
						{
							newNode2 = path.accessList[j + 1].fromNode;
						}
						if (dictionary2.ContainsKey(fromNode2))
						{
							Vector3 vector2 = Vector3.zero;
							if (newNode2 != null && fromNode2 != null)
							{
								vector2 = newNode2.position - fromNode2.position;
							}
							vector2.y = 0f;
							foreach (Interactable interactable3 in dictionary2[fromNode2])
							{
								List<SceneRecorder.SceneCapture> cap3 = interactable3.cap;
								Predicate<SceneRecorder.SceneCapture> predicate4;
								if ((predicate4 = <>9__4) == null)
								{
									predicate4 = (<>9__4 = ((SceneRecorder.SceneCapture item) => item.t > forwardsTime - 0.0167f && item.t < forwardsTime + 0.0167f && !addedToCaptures.Contains(item)));
								}
								List<SceneRecorder.SceneCapture> list2 = cap3.FindAll(predicate4);
								if (list2.Count > 0)
								{
									using (List<SceneRecorder.SceneCapture>.Enumerator enumerator3 = list2.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											SceneRecorder.SceneCapture sceneCapture5 = enumerator3.Current;
											if (!addedToCaptures.Contains(sceneCapture5))
											{
												List<SceneRecorder.ActorCapture> aCap2 = sceneCapture5.aCap;
												Predicate<SceneRecorder.ActorCapture> predicate5;
												if ((predicate5 = <>9__6) == null)
												{
													predicate5 = (<>9__6 = ((SceneRecorder.ActorCapture item) => item.id == who.humanID));
												}
												int num2 = aCap2.FindIndex(predicate5);
												if (num2 > -1)
												{
													SceneRecorder.ActorCapture actorCapture4 = sceneCapture5.aCap[num2];
													actorCapture4.o = (int)outfit;
													actorCapture4.pos = fromNode2.position;
													actorCapture4.rot = Quaternion.LookRotation(vector2).eulerAngles;
													actorCapture4.sp = 1;
												}
												else
												{
													SceneRecorder.ActorCapture actorCapture5 = new SceneRecorder.ActorCapture(who, false);
													actorCapture5.o = (int)outfit;
													actorCapture5.pos = fromNode2.position;
													actorCapture5.rot = Quaternion.LookRotation(vector2).eulerAngles;
													actorCapture5.sp = 1;
													actorCapture5.arms = 0;
													actorCapture5.main = 0;
													sceneCapture5.aCap.Add(actorCapture5);
												}
												addedToCaptures.Add(sceneCapture5);
											}
										}
										continue;
									}
								}
								List<SceneRecorder.SceneCapture> cap4 = interactable3.cap;
								Predicate<SceneRecorder.SceneCapture> predicate6;
								if ((predicate6 = <>9__5) == null)
								{
									predicate6 = (<>9__5 = ((SceneRecorder.SceneCapture item) => item.t > forwardsTime - GameplayControls.Instance.captureInterval * 0.6f && item.t < forwardsTime + GameplayControls.Instance.captureInterval * 0.6f && !addedToCaptures.Contains(item)));
								}
								list2 = cap4.FindAll(predicate6);
								if (list2 == null || list2.Count <= 0)
								{
									SceneRecorder.SceneCapture sceneCapture6 = null;
									foreach (SceneRecorder.SceneCapture sceneCapture7 in interactable3.cap)
									{
										if (sceneCapture7.t <= backwardsTime && (sceneCapture6 == null || sceneCapture7.t > sceneCapture6.t))
										{
											sceneCapture6 = sceneCapture7;
										}
									}
									if (sceneCapture6 != null)
									{
										SceneRecorder.SceneCapture sceneCapture8 = new SceneRecorder.SceneCapture(sceneCapture6);
										sceneCapture8.t = backwardsTime;
										sceneCapture8.aCap = new List<SceneRecorder.ActorCapture>();
										SceneRecorder.ActorCapture actorCapture6 = new SceneRecorder.ActorCapture(who, false);
										actorCapture6.o = (int)outfit;
										actorCapture6.pos = fromNode2.position;
										actorCapture6.rot = Quaternion.LookRotation(vector2).eulerAngles;
										actorCapture6.sp = 1;
										actorCapture6.arms = 0;
										actorCapture6.main = 0;
										sceneCapture8.aCap.Add(actorCapture6);
										interactable3.cap.Add(sceneCapture8);
										interactable3.cap.Sort((SceneRecorder.SceneCapture p1, SceneRecorder.SceneCapture p2) => p1.t.CompareTo(p2.t));
									}
								}
							}
						}
						forwardsTime -= 0.005f;
					}
					return;
				}
				string text = "Unable to get path for ";
				Vector3 position = routeTo.position;
				string text2 = position.ToString();
				string text3 = " to ";
				position = returnTo.position;
				Game.LogError(text + text2 + text3 + position.ToString(), 2);
				return;
			}
		}
		else
		{
			string text4 = "Unable to get path for ";
			Vector3 position = routeFrom.position;
			string text5 = position.ToString();
			string text6 = " to ";
			position = routeTo.position;
			Game.LogError(text4 + text5 + text6 + position.ToString(), 2);
		}
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x0017E020 File Offset: 0x0017C220
	public void ExplodeGrenade(Interactable grenade)
	{
		if (grenade == null)
		{
			return;
		}
		float num = 8.5f;
		float radius = 9.5f;
		float num2 = 0.33f;
		grenade.UpdateWorldPositionAndNode(true);
		Vector3 vector = grenade.GetWorldPosition(true);
		if (grenade.inInventory != null)
		{
			vector = grenade.inInventory.transform.position;
		}
		if (grenade.preset == InteriorControls.Instance.activeFlashbomb)
		{
			radius = 15f;
			num = 4f;
			num2 = 0.05f;
			Object.Instantiate<GameObject>(PrefabControls.Instance.flashBombFlash, PrefabControls.Instance.mapContainer).transform.position = vector;
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.flashBombDetonate, grenade.belongsTo, grenade.node, vector, null, null, 1f, null, false, null, false);
		}
		else if (grenade.preset == InteriorControls.Instance.activeIncapacitator)
		{
			radius = 10f;
			num = 8.5f;
			num2 = 0.66f;
			Object.Instantiate<GameObject>(PrefabControls.Instance.incapacitatorFlash, PrefabControls.Instance.mapContainer).transform.position = vector;
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.incapacitatorDetonate, grenade.belongsTo, grenade.node, vector, null, null, 1f, null, false, null, false);
		}
		GameplayController.Instance.activeGrenades.Remove(grenade);
		Dictionary<Human, float> dictionary = null;
		Dictionary<NewNode, float> nodeCoverageFromRadius = this.GetNodeCoverageFromRadius(grenade, radius, out dictionary);
		string[] array = new string[9];
		array[0] = "Gameplay: Explode grenade ";
		array[1] = grenade.name;
		array[2] = " at ";
		int num3 = 3;
		Vector3 wPos = grenade.wPos;
		array[num3] = wPos.ToString();
		array[4] = ", affecting ";
		array[5] = nodeCoverageFromRadius.Count.ToString();
		array[6] = " nodes & ";
		array[7] = dictionary.Count.ToString();
		array[8] = " humans";
		Game.Log(string.Concat(array), 2);
		foreach (KeyValuePair<NewNode, float> keyValuePair in nodeCoverageFromRadius)
		{
			float num4 = keyValuePair.Value * num;
			if (num4 > 0.2f)
			{
				foreach (Interactable interactable in keyValuePair.Key.interactables)
				{
					if (interactable != grenade && interactable.preset.physicsProfile != null && interactable.preset.reactWithExternalStimuli && interactable.inInventory == null)
					{
						interactable.ForcePhysicsActive(true, true, (interactable.wPos - grenade.wPos).normalized * keyValuePair.Value * num, 2, false);
					}
				}
				foreach (NewWall newWall in keyValuePair.Key.walls)
				{
					foreach (GameObject gameObject in newWall.spawnedFrontage)
					{
						foreach (BreakableWindowController breakableWindowController in gameObject.GetComponentsInChildren<BreakableWindowController>())
						{
							Vector3 averagePosition = breakableWindowController.GetAveragePosition();
							breakableWindowController.BreakWindow(averagePosition, (averagePosition - grenade.node.position).normalized * num4, null, true);
						}
					}
					if (newWall.otherWall != null)
					{
						foreach (GameObject gameObject2 in newWall.otherWall.spawnedFrontage)
						{
							foreach (BreakableWindowController breakableWindowController2 in gameObject2.GetComponentsInChildren<BreakableWindowController>())
							{
								Vector3 averagePosition2 = breakableWindowController2.GetAveragePosition();
								breakableWindowController2.BreakWindow(averagePosition2, (averagePosition2 - grenade.node.position).normalized * num4, null, true);
							}
						}
					}
				}
			}
			if (grenade.preset == InteriorControls.Instance.activeFlashbomb)
			{
				foreach (FurnitureLocation furnitureLocation in keyValuePair.Key.individualFurniture)
				{
					foreach (Interactable interactable2 in furnitureLocation.integratedInteractables)
					{
						if (interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityCamera || interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun || interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.otherSecuritySystem)
						{
							Game.Log("Flashbomb disabled " + interactable2.name, 2);
							interactable2.SetSwitchState(false, null, true, true, false);
							interactable2.SetValue(0f);
						}
					}
				}
			}
		}
		foreach (KeyValuePair<Human, float> keyValuePair2 in dictionary)
		{
			if (!keyValuePair2.Key.isStunned && !keyValuePair2.Key.isDead)
			{
				Game.Log("Grenade dmg normalized:" + keyValuePair2.Key.name + " " + keyValuePair2.Value.ToString(), 2);
				if (grenade.preset == InteriorControls.Instance.activeFlashbomb)
				{
					keyValuePair2.Key.AddBlinded(keyValuePair2.Value * 10f + 0.1f);
				}
				Vector3 position = keyValuePair2.Key.lookAtThisTransform.position;
				keyValuePair2.Key.RecieveDamage(keyValuePair2.Value * num * num2, grenade.belongsTo, position, position - grenade.wPos, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 3f);
			}
		}
		grenade.Delete();
	}

	// Token: 0x060019C9 RID: 6601 RVA: 0x0017E708 File Offset: 0x0017C908
	public Dictionary<NewNode, float> GetNodeCoverageFromRadius(Interactable grenade, float radius, out Dictionary<Human, float> humanOutput)
	{
		Dictionary<NewNode, float> dictionary = new Dictionary<NewNode, float>();
		humanOutput = new Dictionary<Human, float>();
		grenade.UpdateWorldPositionAndNode(true);
		NewNode newNode = grenade.node;
		if (grenade.inInventory != null)
		{
			newNode = grenade.inInventory.currentNode;
			humanOutput.Add(grenade.inInventory, 1f);
		}
		HashSet<NewNode> hashSet = new HashSet<NewNode>();
		hashSet.Add(newNode);
		dictionary.Add(newNode, 1f);
		int num = 200;
		while (hashSet.Count > 0 && num > 0)
		{
			NewNode newNode2 = Enumerable.FirstOrDefault<NewNode>(hashSet);
			float num2;
			if (newNode2 == newNode)
			{
				num2 = 1f;
			}
			else
			{
				num2 = Mathf.Clamp01((radius - Vector3.Distance(grenade.wPos, newNode2.position)) / radius);
			}
			if (!dictionary.ContainsKey(newNode2))
			{
				dictionary.Add(newNode2, num2);
			}
			foreach (Actor actor in newNode2.room.currentOccupants)
			{
				Human human = actor as Human;
				if (human != null)
				{
					float num3 = Mathf.Clamp01((radius - Vector3.Distance(grenade.wPos, human.transform.position)) / radius);
					if (!humanOutput.ContainsKey(human))
					{
						humanOutput.Add(human, num3);
					}
					else
					{
						humanOutput[human] = Mathf.Max(humanOutput[human], num3);
					}
				}
			}
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode2.accessToOtherNodes)
			{
				List<DataRaycastController.NodeRaycastHit> list;
				if (!hashSet.Contains(keyValuePair.Key) && !dictionary.ContainsKey(keyValuePair.Key) && Vector3.Distance(keyValuePair.Key.position, grenade.wPos) <= radius && DataRaycastController.Instance.NodeRaycast(newNode, keyValuePair.Key, out list, null, false))
				{
					if (keyValuePair.Value.accessType == NewNode.NodeAccess.AccessType.door)
					{
						if (keyValuePair.Value.door != null)
						{
							if (!keyValuePair.Value.door.isClosed)
							{
								hashSet.Add(keyValuePair.Key);
							}
						}
						else
						{
							hashSet.Add(keyValuePair.Key);
						}
					}
					else if (keyValuePair.Value.accessType != NewNode.NodeAccess.AccessType.window)
					{
						hashSet.Add(keyValuePair.Key);
					}
				}
			}
			hashSet.Remove(newNode2);
			num--;
		}
		return dictionary;
	}

	// Token: 0x060019CA RID: 6602 RVA: 0x0017E9A8 File Offset: 0x0017CBA8
	public bool RankRoomShadiness(NewRoom room, out float score)
	{
		score = 0f;
		if (room == null)
		{
			return false;
		}
		if (room.nodes.Count <= 0)
		{
			return false;
		}
		if (room.entrances.Count <= 0)
		{
			return false;
		}
		if (!room.gameLocation.IsPublicallyOpen(false))
		{
			return false;
		}
		score = Toolbox.Instance.Rand(0f, 1f, false);
		if (room.gameLocation.thisAsStreet != null)
		{
			score += (1f - room.gameLocation.thisAsStreet.normalizedFootfall) * 10f;
		}
		else
		{
			if (room.floor != null && room.floor.floor < 0)
			{
				score += 5f;
			}
			score += Mathf.Min((float)room.nodes.Count * 0.3f, 5f);
		}
		score += (float)room.preset.shadinessValue;
		score -= (float)(room.currentOccupants.Count * 2);
		score += room.defaultWallKey.grubiness * 10f;
		return true;
	}

	// Token: 0x060019CB RID: 6603 RVA: 0x0017EAC8 File Offset: 0x0017CCC8
	public bool RankNodeShadiness(NewNode node, out float score)
	{
		score = 0f;
		if (node.accessToOtherNodes.Count <= 0)
		{
			return false;
		}
		score = Toolbox.Instance.Rand(0f, 1f, false);
		score += (float)(10 - node.individualFurniture.Count * 2);
		score += (float)(10 - node.interactables.Count * 2);
		if (node.allowNewFurniture)
		{
			score += 5f;
		}
		foreach (NewWall newWall in node.walls)
		{
			if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
			{
				score -= 10f;
			}
			else if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window)
			{
				score -= 3f;
			}
			else if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
			{
				score -= 6f;
			}
			else
			{
				score += 1f;
			}
		}
		return true;
	}

	// Token: 0x060019CC RID: 6604 RVA: 0x0017EBDC File Offset: 0x0017CDDC
	public void TriggerBriefcaseBomb(Interactable briefcase, Human actor)
	{
		if (briefcase == null)
		{
			return;
		}
		Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.activeIncapacitator, actor, null, null, Player.Instance.transform.position + new Vector3(0f, 3.5f, 0f), Vector3.zero, null, null, "");
		if (interactable != null)
		{
			interactable.SetInInventory(actor);
			interactable.SetCustomState2(true, actor, true, true, false);
			interactable.SetValue(GameplayControls.Instance.thrownGrenadeFuse * 0.5f);
		}
	}

	// Token: 0x060019CD RID: 6605 RVA: 0x0017EC64 File Offset: 0x0017CE64
	public Interactable GetMailbox(Human forHuman)
	{
		if (forHuman != null)
		{
			if (forHuman.home != null && forHuman.home.residence != null)
			{
				if (forHuman.home.residence.mailbox != null)
				{
					using (Dictionary<FurnitureLocation.OwnerKey, int>.Enumerator enumerator = forHuman.home.residence.mailbox.ownerMap.GetEnumerator())
					{
						Predicate<Interactable> <>9__0;
						Predicate<Interactable> <>9__1;
						while (enumerator.MoveNext())
						{
							KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair = enumerator.Current;
							if (keyValuePair.Key.human == forHuman)
							{
								List<Interactable> integratedInteractables = forHuman.home.residence.mailbox.integratedInteractables;
								Predicate<Interactable> predicate;
								if ((predicate = <>9__0) == null)
								{
									predicate = (<>9__0 = ((Interactable item) => item.objectRef != null && item.objectRef as NewAddress == forHuman.home));
								}
								Interactable interactable = integratedInteractables.Find(predicate);
								if (interactable != null)
								{
									return interactable;
								}
							}
							else if (keyValuePair.Key.address == forHuman.home)
							{
								List<Interactable> integratedInteractables2 = forHuman.home.residence.mailbox.integratedInteractables;
								Predicate<Interactable> predicate2;
								if ((predicate2 = <>9__1) == null)
								{
									predicate2 = (<>9__1 = ((Interactable item) => item.objectRef != null && item.objectRef as NewAddress == forHuman.home));
								}
								Interactable interactable2 = integratedInteractables2.Find(predicate2);
								if (interactable2 != null)
								{
									return interactable2;
								}
							}
						}
						goto IL_1C6;
					}
				}
				Game.LogError("Unable to locate mailbox: " + forHuman.name, 2);
			}
			else
			{
				Game.LogError("Unable to locate home: " + forHuman.name, 2);
			}
		}
		else
		{
			Game.LogError(" Unable to get person!", 2);
		}
		IL_1C6:
		return null;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x0017EE4C File Offset: 0x0017D04C
	public bool IsStoryMissionActive(out Chapter script, out int chapter)
	{
		chapter = -1;
		script = null;
		if (Game.Instance.sandboxMode)
		{
			return false;
		}
		if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
		{
			script = ChapterController.Instance.chapterScript;
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (chapterIntro != null)
			{
				if (chapterIntro.completed)
				{
					return false;
				}
				chapter = ChapterController.Instance.currentPart;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060019CF RID: 6607 RVA: 0x0017EECC File Offset: 0x0017D0CC
	public string GetShareCode(ref CitySaveData cityData)
	{
		if (cityData == null)
		{
			return string.Empty;
		}
		return this.GetShareCode(cityData.cityName, (int)cityData.citySize.x, (int)cityData.citySize.y, cityData.build, cityData.seed);
	}

	// Token: 0x060019D0 RID: 6608 RVA: 0x0017EF18 File Offset: 0x0017D118
	public string GetShareCode(string cityName, int citySizeX, int citySizeY, string version, string seed)
	{
		int num = CityControls.Instance.citySizes.FindIndex((CityControls.CitySize item) => Mathf.RoundToInt(item.v2.x) == citySizeX && Mathf.RoundToInt(item.v2.y) == citySizeY);
		if (num < 0)
		{
			num = 0;
		}
		return string.Concat(new string[]
		{
			Strings.RemoveCharacters(cityName, true, false, true, false),
			".",
			num.ToString(),
			".",
			this.VersionToNumbers(version).ToString(),
			".",
			Strings.RemoveCharacters(seed, true, false, true, false)
		});
	}

	// Token: 0x060019D1 RID: 6609 RVA: 0x0017EFB8 File Offset: 0x0017D1B8
	public void ParseShareCode(string input, out string cityName, out int citySizeX, out int citySizeY, out string version, out string seed)
	{
		string[] array = input.Split('.', 0);
		cityName = string.Empty;
		citySizeX = 5;
		citySizeY = 5;
		version = string.Empty;
		seed = string.Empty;
		if (array.Length != 0)
		{
			cityName = Strings.RemoveCharacters(array[0], true, false, true, false);
		}
		if (array.Length > 1)
		{
			if (Game.Instance.smallCitiesOnly)
			{
				citySizeX = 5;
				citySizeY = 5;
			}
			else
			{
				int val = 0;
				int.TryParse(array[1], ref val);
				Vector2 citySizeFromValue = this.GetCitySizeFromValue(val);
				citySizeX = Mathf.RoundToInt(citySizeFromValue.x);
				citySizeY = Mathf.RoundToInt(citySizeFromValue.y);
			}
		}
		if (array.Length > 2)
		{
			int numbers = 0;
			int.TryParse(array[2], ref numbers);
			version = this.NumbersToVersion(numbers);
		}
		if (array.Length > 3)
		{
			seed = Strings.RemoveCharacters(array[3], true, false, true, false);
		}
		if (seed == null || seed.Length <= 0)
		{
			seed = Toolbox.Instance.GenerateSeed(16, false, "");
		}
	}

	// Token: 0x060019D2 RID: 6610 RVA: 0x0017F0A4 File Offset: 0x0017D2A4
	public int VersionToNumbers(string version)
	{
		int result = 0;
		string[] array = version.Split('.', 0);
		string text = string.Empty;
		foreach (string text2 in array)
		{
			text += text2;
		}
		int.TryParse(text, ref result);
		return result;
	}

	// Token: 0x060019D3 RID: 6611 RVA: 0x0017F0EC File Offset: 0x0017D2EC
	public string NumbersToVersion(int numbers)
	{
		string text = numbers.ToString();
		if (text.Length > 2)
		{
			text = text.Substring(0, 2) + "." + text.Substring(2);
		}
		return text;
	}

	// Token: 0x060019D4 RID: 6612 RVA: 0x0017F125 File Offset: 0x0017D325
	public Vector2 GetCitySizeFromValue(int val)
	{
		if (val < CityControls.Instance.citySizes.Count)
		{
			return CityControls.Instance.citySizes[val].v2;
		}
		return new Vector2(5f, 5f);
	}

	// Token: 0x060019D5 RID: 6613 RVA: 0x0017F15E File Offset: 0x0017D35E
	public Vector3 ToVector3(Vector3Int input)
	{
		return new Vector3((float)input.x, (float)input.y, (float)input.z);
	}

	// Token: 0x060019D6 RID: 6614 RVA: 0x0017F17D File Offset: 0x0017D37D
	public float3 ToFloat3(Vector3Int input)
	{
		return new float3((float)input.x, (float)input.y, (float)input.z);
	}

	// Token: 0x060019D7 RID: 6615 RVA: 0x0017F19C File Offset: 0x0017D39C
	public Vector2 ToVector2(Vector2Int input)
	{
		return new Vector2((float)input.x, (float)input.y);
	}

	// Token: 0x060019D8 RID: 6616 RVA: 0x0017F1B4 File Offset: 0x0017D3B4
	public GameplayController.HotelGuest GetHotelRoom(Human person)
	{
		if (person == null)
		{
			return null;
		}
		List<GameplayController.HotelGuest> list = GameplayController.Instance.hotelGuests.FindAll((GameplayController.HotelGuest item) => item.humanID == person.humanID);
		if (list.Count > 0)
		{
			GameplayController.HotelGuest result = list[0];
			if (list.Count > 1)
			{
				foreach (GameplayController.HotelGuest hotelGuest in list)
				{
					NewAddress address = hotelGuest.GetAddress();
					if (person.job != null && person.job.employer != null && address != null && person.job.employer.address != null && person.job.employer.address.building == address.building)
					{
						result = hotelGuest;
						break;
					}
					if (person.currentBuilding == address.building)
					{
						result = hotelGuest;
						break;
					}
				}
			}
			return result;
		}
		return null;
	}

	// Token: 0x060019D9 RID: 6617 RVA: 0x0017F2F8 File Offset: 0x0017D4F8
	public FileInfo GetCityFile(string code, out string codeVersion, out string codeSeed)
	{
		string empty = string.Empty;
		int codeSizeX = 5;
		int codeSizeY = 5;
		this.ParseShareCode(code, out empty, out codeSizeX, out codeSizeY, out codeVersion, out codeSeed);
		return this.GetCityFile(empty, codeSeed, codeSizeX, codeSizeY, codeVersion);
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x0017F32C File Offset: 0x0017D52C
	public FileInfo GetCityFile(string codeName, string codeSeed, int codeSizeX, int codeSizeY, string codeVersion)
	{
		Game.Log("Menu: Searching for city file: " + codeName + " with seed " + codeSeed, 2);
		FileInfo fileInfo = null;
		bool flag = false;
		List<FileInfo> list = Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Cities").GetFiles("*.cit", 1));
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + "/Legacy");
		list.AddRange(Enumerable.ToList<FileInfo>(directoryInfo.GetFiles("*.cit", 1)));
		foreach (FileInfo fileInfo2 in list)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			int num = -1;
			int num2 = -1;
			string empty3 = string.Empty;
			this.ParseShareCode(fileInfo2.Name.Substring(0, fileInfo2.Name.Length - fileInfo2.Extension.Length), out empty, out num, out num2, out empty3, out empty2);
			if (empty2 == codeSeed && codeName == empty && num == codeSizeX && num2 == codeSizeY && !flag && empty3 == codeVersion)
			{
				fileInfo = fileInfo2;
				flag = true;
				Game.Log("Menu: Found a file version match for " + codeName + ": " + empty3, 2);
				break;
			}
		}
		if (fileInfo == null || !flag)
		{
			foreach (FileInfo fileInfo3 in Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.persistentDataPath + "/Cities").GetFiles()))
			{
				if (fileInfo3.Extension.ToLower() == ".cit" || fileInfo3.Extension.ToLower() == ".citb")
				{
					string empty4 = string.Empty;
					string empty5 = string.Empty;
					int num3 = -1;
					int num4 = -1;
					string empty6 = string.Empty;
					this.ParseShareCode(fileInfo3.Name.Substring(0, fileInfo3.Name.Length - fileInfo3.Extension.Length), out empty4, out num3, out num4, out empty6, out empty5);
					if (empty5 == codeSeed && codeName == empty4 && num3 == codeSizeX && num4 == codeSizeY && !flag && empty6 == codeVersion)
					{
						fileInfo = fileInfo3;
						flag = true;
						Game.Log("Menu: Found a file version match for " + codeName + ": " + empty6, 2);
						break;
					}
				}
			}
		}
		if ((fileInfo == null || !flag) && Game.Instance.allowMods)
		{
			foreach (FileInfo fileInfo4 in ModLoader.Instance.GetActiveCities())
			{
				if (fileInfo4.Extension.ToLower() == ".cit" || fileInfo4.Extension.ToLower() == ".citb")
				{
					string empty7 = string.Empty;
					string empty8 = string.Empty;
					int num5 = -1;
					int num6 = -1;
					string empty9 = string.Empty;
					this.ParseShareCode(fileInfo4.Name.Substring(0, fileInfo4.Name.Length - fileInfo4.Extension.Length), out empty7, out num5, out num6, out empty9, out empty8);
					if (empty8 == codeSeed && codeName == empty7 && num5 == codeSizeX && num6 == codeSizeY && !flag && empty9 == codeVersion)
					{
						fileInfo = fileInfo4;
						flag = true;
						Game.Log("Menu: Found a file version match for " + codeName + ": " + empty9, 2);
						break;
					}
				}
			}
		}
		return fileInfo;
	}

	// Token: 0x060019DB RID: 6619 RVA: 0x0017F6E4 File Offset: 0x0017D8E4
	public bool IsConsoleBuild()
	{
		return Game.Instance.forceConsoleBuildFlag;
	}

	// Token: 0x060019DC RID: 6620 RVA: 0x0017F6F8 File Offset: 0x0017D8F8
	public string CensorText(string inputText)
	{
		if (this.censor == null)
		{
			this.censor = base.gameObject.GetComponent<Censor>();
		}
		if (this.censor == null)
		{
			Game.LogError("Censor class not found! Unable to censor text...", 2);
			return inputText;
		}
		return this.censor.CensorText(inputText);
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x0017F74C File Offset: 0x0017D94C
	public bool TryGetSniperVantagePoint(Human sniper, NewGameLocation requiredTargetSite, out NewWall vantagePoint, out float vantageScore, List<NewNode.NodeAccess> accessCheckList = null)
	{
		vantageScore = -1f;
		vantagePoint = null;
		HashSet<NewBuilding> hashSet = new HashSet<NewBuilding>();
		if (requiredTargetSite.thisAsStreet != null)
		{
			List<NewBuilding> list = new List<NewBuilding>();
			foreach (NewNode.NodeAccess nodeAccess in requiredTargetSite.entrances)
			{
				if (nodeAccess.fromNode.gameLocation != nodeAccess.toNode.gameLocation && nodeAccess.wall != null)
				{
					NewGameLocation gameLocation = nodeAccess.toNode.gameLocation;
					if (gameLocation == requiredTargetSite)
					{
						gameLocation = nodeAccess.fromNode.gameLocation;
					}
					if (gameLocation.building != null && !list.Contains(gameLocation.building))
					{
						list.Add(gameLocation.building);
					}
				}
			}
			using (List<NewBuilding>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					NewBuilding newBuilding = enumerator2.Current;
					if (!hashSet.Contains(newBuilding))
					{
						float num = -1f;
						NewWall newWall = null;
						new List<NewGameLocation>();
						if (this.ScanBuildingForSniperVantagePoints(sniper, newBuilding, requiredTargetSite, out newWall, out num, ref accessCheckList) && num > vantageScore)
						{
							vantageScore = num;
							vantagePoint = newWall;
						}
						hashSet.Add(newBuilding);
					}
				}
				goto IL_24C;
			}
		}
		if (requiredTargetSite.thisAsAddress != null)
		{
			foreach (NewNode.NodeAccess nodeAccess2 in requiredTargetSite.entrances)
			{
				if (nodeAccess2.accessType == NewNode.NodeAccess.AccessType.window && nodeAccess2.fromNode.gameLocation != nodeAccess2.toNode.gameLocation && nodeAccess2.wall != null && (nodeAccess2.fromNode.room.isOutsideWindow || nodeAccess2.toNode.room.isOutsideWindow))
				{
					Vector3 zero = Vector3.zero;
					NewBuilding facingBuildingFromWindow = this.GetFacingBuildingFromWindow(nodeAccess2, out zero);
					if (facingBuildingFromWindow != null && !hashSet.Contains(facingBuildingFromWindow))
					{
						float num2 = -1f;
						NewWall newWall2 = null;
						new List<NewGameLocation>();
						if (this.ScanBuildingForSniperVantagePoints(sniper, facingBuildingFromWindow, requiredTargetSite, out newWall2, out num2, ref accessCheckList) && num2 > vantageScore)
						{
							vantageScore = num2;
							vantagePoint = newWall2;
						}
						hashSet.Add(facingBuildingFromWindow);
					}
				}
			}
		}
		IL_24C:
		return vantagePoint != null;
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x0017F9D4 File Offset: 0x0017DBD4
	private bool ScanBuildingForSniperVantagePoints(Human sniper, NewBuilding building, NewGameLocation requiredTargetSite, out NewWall vantagePoint, out float vantageScore, ref List<NewNode.NodeAccess> accessCheckList)
	{
		vantageScore = -1f;
		vantagePoint = null;
		if (sniper == null)
		{
			return false;
		}
		if (building == null)
		{
			return false;
		}
		if (requiredTargetSite == null)
		{
			return false;
		}
		int num = 0;
		int num2 = 4;
		if (requiredTargetSite.thisAsAddress != null && requiredTargetSite.thisAsAddress.floor != null)
		{
			num = requiredTargetSite.thisAsAddress.floor.floor;
			num2 = 2;
		}
		if (building.floors.ContainsKey(2))
		{
			for (int i = Mathf.Max(1, num - num2); i <= num + num2; i++)
			{
				if (building.floors.ContainsKey(i))
				{
					foreach (NewAddress newAddress in building.floors[i].addresses)
					{
						if (newAddress.addressPreset != null && newAddress.addressPreset.allowSniperVantagePoint)
						{
							if (newAddress.entrances.Exists((NewNode.NodeAccess item) => item.walkingAccess))
							{
								foreach (NewNode.NodeAccess nodeAccess in newAddress.entrances)
								{
									int num3;
									string text;
									if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.window && nodeAccess.fromNode.gameLocation != nodeAccess.toNode.gameLocation && nodeAccess.wall != null && (nodeAccess.fromNode.room.isOutsideWindow || nodeAccess.toNode.room.isOutsideWindow) && !sniper.IsTrespassing(nodeAccess.fromNode.room, out num3, out text, false) && newAddress.company == null && newAddress.inhabitants.Count <= 0)
									{
										float num4 = -1f + Toolbox.Instance.Rand(0f, 0.1f, true) + newAddress.addressPreset.vantagePointBoost;
										if (accessCheckList != null)
										{
											using (List<NewNode.NodeAccess>.Enumerator enumerator3 = accessCheckList.GetEnumerator())
											{
												while (enumerator3.MoveNext())
												{
													NewNode.NodeAccess nodeAccess2 = enumerator3.Current;
													List<DataRaycastController.NodeRaycastHit> list;
													if (DataRaycastController.Instance.NodeRaycast(nodeAccess.fromNode, nodeAccess2.toNode, out list, null, false))
													{
														num4 += 1f;
													}
												}
												goto IL_2A1;
											}
											goto IL_24D;
										}
										goto IL_24D;
										IL_2A1:
										if (num4 > 0f && num4 > vantageScore)
										{
											vantagePoint = nodeAccess.wall;
											vantageScore = num4;
											continue;
										}
										continue;
										IL_24D:
										foreach (NewNode toNode in requiredTargetSite.nodes)
										{
											List<DataRaycastController.NodeRaycastHit> list;
											if (DataRaycastController.Instance.NodeRaycast(nodeAccess.fromNode, toNode, out list, null, false))
											{
												num4 += 1f;
											}
										}
										goto IL_2A1;
									}
								}
							}
						}
					}
				}
			}
		}
		if (vantagePoint != null)
		{
			if (Game.Instance.printDebug)
			{
				string[] array = new string[10];
				array[0] = "Murder: Best sniper for vantage point over ";
				array[1] = requiredTargetSite.name;
				array[2] = " in ";
				array[3] = building.name;
				array[4] = " is ";
				int num5 = 5;
				Vector3 position = vantagePoint.position;
				array[num5] = position.ToString();
				array[6] = " at ";
				array[7] = vantagePoint.node.room.GetName();
				array[8] = ": ";
				array[9] = vantageScore.ToString();
				Game.Log(string.Concat(array), 2);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060019DF RID: 6623 RVA: 0x0017FDEC File Offset: 0x0017DFEC
	public bool TryGetSniperVantagePoint(NewGameLocation vantageLocation, out NewWall vantagePoint, out float vantageScore, out List<NewGameLocation> possibleTargetSites, NewGameLocation requiredTargetSite = null)
	{
		vantageScore = -1f;
		vantagePoint = null;
		possibleTargetSites = new List<NewGameLocation>();
		if (vantageLocation == null)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Murder: Input vantage location is null, returning null...", 2);
			}
			return false;
		}
		if (vantageLocation.floor == null)
		{
			if (Game.Instance.printDebug)
			{
				Game.Log("Murder: Input vantage location floor is null, returning null...", 2);
			}
			return false;
		}
		foreach (NewRoom vantageRoom in vantageLocation.rooms)
		{
			NewWall newWall = null;
			float num = -999999f;
			List<NewGameLocation> list = new List<NewGameLocation>();
			if (this.TryGetSniperVantagePoint(vantageRoom, out newWall, out num, out list, requiredTargetSite))
			{
				if (newWall != null && num > vantageScore)
				{
					vantagePoint = newWall;
					vantageScore = num;
				}
				foreach (NewGameLocation newGameLocation in list)
				{
					if (!possibleTargetSites.Contains(newGameLocation))
					{
						possibleTargetSites.Add(newGameLocation);
					}
				}
			}
		}
		if (vantagePoint != null)
		{
			if (Game.Instance.printDebug)
			{
				string[] array = new string[13];
				array[0] = "Murder: Sniper vantage point for ";
				array[1] = ((requiredTargetSite != null) ? requiredTargetSite.ToString() : null);
				array[2] = " at ";
				int num2 = 3;
				Vector3 position = vantagePoint.position;
				array[num2] = position.ToString();
				array[4] = " found at ";
				array[5] = vantagePoint.node.gameLocation.name;
				array[6] = " ";
				array[7] = vantagePoint.node.room.GetName();
				array[8] = " with score of ";
				array[9] = vantageScore.ToString();
				array[10] = " and ";
				array[11] = possibleTargetSites.Count.ToString();
				array[12] = " target sites";
				Game.Log(string.Concat(array), 2);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060019E0 RID: 6624 RVA: 0x0017FFEC File Offset: 0x0017E1EC
	public bool TryGetSniperVantagePoint(NewRoom vantageRoom, out NewWall vantagePoint, out float vantageScore, out List<NewGameLocation> possibleTargetSites, NewGameLocation requiredTargetSite = null)
	{
		vantageScore = -1f;
		vantagePoint = null;
		possibleTargetSites = new List<NewGameLocation>();
		if (vantageRoom == null)
		{
			return false;
		}
		if (vantageRoom.floor == null)
		{
			return false;
		}
		if (Game.Instance.printDebug)
		{
			Game.Log(string.Concat(new string[]
			{
				"Murder: Scanning ",
				vantageRoom.GetName(),
				" for a valid vantage point with ",
				vantageRoom.entrances.Count.ToString(),
				" entrances..."
			}), 2);
		}
		int num = 2;
		int floor = vantageRoom.floor.floor;
		Dictionary<Vector3, float> dictionary = new Dictionary<Vector3, float>();
		Dictionary<Vector3, List<NewGameLocation>> dictionary2 = new Dictionary<Vector3, List<NewGameLocation>>();
		foreach (NewNode.NodeAccess nodeAccess in vantageRoom.entrances)
		{
			if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.window)
			{
				if (nodeAccess.fromNode.gameLocation != nodeAccess.toNode.gameLocation && nodeAccess.wall != null)
				{
					NewNode newNode = nodeAccess.fromNode;
					NewNode newNode2 = nodeAccess.toNode;
					if (nodeAccess.toNode.room.isOutsideWindow)
					{
						newNode = nodeAccess.toNode;
						newNode2 = nodeAccess.fromNode;
					}
					if (newNode2 != null && newNode2.accessToOtherNodes.Count > 0 && !newNode2.isInaccessable)
					{
						if (newNode.room.isOutsideWindow)
						{
							Vector3 zero = Vector3.zero;
							NewBuilding facingBuildingFromWindow = this.GetFacingBuildingFromWindow(nodeAccess, out zero);
							if (facingBuildingFromWindow != null)
							{
								if (Game.Instance.printDebug)
								{
									string[] array = new string[6];
									array[0] = "Murder: Found facing building ";
									array[1] = facingBuildingFromWindow.name;
									array[2] = " from window ";
									int num2 = 3;
									Vector3 vector = nodeAccess.worldAccessPoint;
									array[num2] = vector.ToString();
									array[4] = " at ";
									array[5] = vantageRoom.GetName();
									Game.Log(string.Concat(array), 2);
								}
								if (requiredTargetSite == null || requiredTargetSite.building == facingBuildingFromWindow)
								{
									float num3 = -1f;
									List<NewGameLocation> list = new List<NewGameLocation>();
									if (requiredTargetSite != null || !dictionary.TryGetValue(zero, ref num3))
									{
										for (int i = Mathf.Max(0, floor - num); i <= floor + num; i++)
										{
											if (facingBuildingFromWindow.floors.ContainsKey(i))
											{
												foreach (NewAddress newAddress in facingBuildingFromWindow.floors[i].addresses)
												{
													if (requiredTargetSite != null && requiredTargetSite != newAddress)
													{
														if (Game.Instance.printDebug)
														{
															Game.Log("Murder: Scanning " + newAddress.name + " but it is not the required target site " + requiredTargetSite.name, 2);
														}
													}
													else
													{
														if (requiredTargetSite != null && Game.Instance.printDebug)
														{
															Game.Log("Murder: Scanning required target site", 2);
														}
														if (newAddress.entrances.Exists((NewNode.NodeAccess item) => item.walkingAccess))
														{
															foreach (NewNode.NodeAccess nodeAccess2 in newAddress.entrances)
															{
																if (nodeAccess2.accessType == NewNode.NodeAccess.AccessType.window && nodeAccess2.fromNode.gameLocation != nodeAccess2.toNode.gameLocation)
																{
																	NewNode newNode3 = nodeAccess2.fromNode;
																	NewNode newNode4 = nodeAccess2.toNode;
																	if (nodeAccess2.toNode.room.isOutsideWindow)
																	{
																		newNode3 = nodeAccess2.toNode;
																		newNode4 = nodeAccess2.fromNode;
																	}
																	if (newNode4 != null && newNode4.accessToOtherNodes.Count > 0 && !newNode4.isInaccessable && newNode3.room.isOutsideWindow)
																	{
																		Vector3 zero2 = Vector3.zero;
																		if (this.GetFacingBuildingFromWindow(nodeAccess2, out zero2) == vantageRoom.building)
																		{
																			num3 += 10f;
																			if (requiredTargetSite != null)
																			{
																				float num4 = Mathf.Max(50f - Vector3.Distance(nodeAccess2.worldAccessPoint, nodeAccess.worldAccessPoint), 0f);
																				num3 += num4;
																			}
																			if (!list.Contains(newNode4.gameLocation))
																			{
																				list.Add(newNode4.gameLocation);
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
										if (requiredTargetSite == null)
										{
											dictionary.Add(zero, num3);
											dictionary2.Add(zero, list);
										}
									}
									if (requiredTargetSite == null)
									{
										dictionary2.TryGetValue(zero, ref list);
									}
									num3 += Toolbox.Instance.Rand(-0.99f, 0f, true);
									if (num3 > 0f && num3 > vantageScore)
									{
										vantagePoint = nodeAccess.wall;
										vantageScore = num3;
										possibleTargetSites = new List<NewGameLocation>(list);
									}
								}
							}
							else if (Game.Instance.printDebug)
							{
								string text = "Murder: Could not find facing building from window ";
								Vector3 vector = nodeAccess.worldAccessPoint;
								Game.Log(text + vector.ToString() + " at " + vantageRoom.GetName(), 2);
							}
						}
						else if (Game.Instance.printDebug)
						{
							string text2 = "Murder: Could not find an outside window at ";
							Vector3 vector = nodeAccess.worldAccessPoint;
							Game.Log(text2 + vector.ToString() + " at " + vantageRoom.GetName(), 2);
						}
					}
				}
				else if (Game.Instance.printDebug)
				{
					string text3 = "Murder: Could not find an outside window at ";
					Vector3 vector = nodeAccess.worldAccessPoint;
					Game.Log(text3 + vector.ToString() + " at " + vantageRoom.GetName(), 2);
				}
			}
			else if (Game.Instance.printDebug)
			{
				string[] array2 = new string[7];
				array2[0] = "Murder: Access at ";
				int num5 = 1;
				Vector3 vector = nodeAccess.worldAccessPoint;
				array2[num5] = vector.ToString();
				array2[2] = " at ";
				array2[3] = vantageRoom.GetName();
				array2[4] = " is not a window (";
				array2[5] = nodeAccess.accessType.ToString();
				array2[6] = ")";
				Game.Log(string.Concat(array2), 2);
			}
		}
		if (vantagePoint != null)
		{
			if (Game.Instance.printDebug)
			{
				string[] array3 = new string[11];
				array3[0] = "Murder: Sniper vantage point at ";
				int num6 = 1;
				Vector3 vector = vantagePoint.position;
				array3[num6] = vector.ToString();
				array3[2] = " found at ";
				array3[3] = vantagePoint.node.gameLocation.name;
				array3[4] = " ";
				array3[5] = vantagePoint.node.room.GetName();
				array3[6] = " with score of ";
				array3[7] = vantageScore.ToString();
				array3[8] = " and ";
				array3[9] = possibleTargetSites.Count.ToString();
				array3[10] = " target sites";
				Game.Log(string.Concat(array3), 2);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x00180760 File Offset: 0x0017E960
	private NewBuilding GetFacingBuildingFromWindow(NewNode.NodeAccess windowAccess, out Vector3 windowDir)
	{
		NewBuilding result = null;
		windowDir = Vector3.zero;
		NewNode newNode;
		NewNode newNode2;
		if (windowAccess.fromNode.room.isOutsideWindow)
		{
			newNode = windowAccess.fromNode;
			newNode2 = windowAccess.toNode;
		}
		else
		{
			if (!windowAccess.toNode.room.isOutsideWindow)
			{
				return null;
			}
			newNode = windowAccess.toNode;
			newNode2 = windowAccess.fromNode;
		}
		windowDir = (newNode.position - newNode2.position).normalized;
		if (newNode2.building != null)
		{
			Vector2Int vector2Int = newNode2.building.cityTile.cityCoord + new Vector2Int(Mathf.RoundToInt(windowDir.x), Mathf.RoundToInt(windowDir.z));
			CityTile cityTile = null;
			if (HighlanderSingleton<CityBoundaryAndTiles>.Instance.cityTiles.TryGetValue(vector2Int, ref cityTile) && cityTile.building != null)
			{
				result = cityTile.building;
			}
		}
		return result;
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x00180854 File Offset: 0x0017EA54
	public Telephone GetClosestTelephone(Actor toActor, float maxDistance = 18f, bool prioritiseSameLocation = true, bool payPhonesOnly = false, bool mustHaveValidAccess = true)
	{
		if (toActor == null)
		{
			return null;
		}
		if (toActor.currentGameLocation == null)
		{
			return null;
		}
		Telephone result = null;
		float num = 9999999f;
		foreach (KeyValuePair<int, Telephone> keyValuePair in CityData.Instance.phoneDictionary)
		{
			int num2;
			string text;
			if (keyValuePair.Value.interactable != null && (!payPhonesOnly || keyValuePair.Value.interactable.preset.isPayphone) && (!mustHaveValidAccess || !toActor.IsTrespassing(keyValuePair.Value.interactable.node.room, out num2, out text, true)))
			{
				float num3 = Vector3.Distance(keyValuePair.Value.interactable.wPos, toActor.transform.position);
				if (num3 <= maxDistance)
				{
					if (prioritiseSameLocation && keyValuePair.Value.interactable.node.gameLocation == toActor.currentGameLocation)
					{
						num3 -= maxDistance;
					}
					if (num3 < num)
					{
						result = keyValuePair.Value;
						num = num3;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0400220A RID: 8714
	private bool endOfFrameInvoke;

	// Token: 0x0400220B RID: 8715
	private HashSet<Action> invokeEndOfFrame = new HashSet<Action>();

	// Token: 0x0400220C RID: 8716
	private List<string> debugInvokeEndOfFrame = new List<string>();

	// Token: 0x0400220D RID: 8717
	public Censor censor;

	// Token: 0x0400220F RID: 8719
	public List<Human.ShoeType> allShoeTypes = new List<Human.ShoeType>();

	// Token: 0x04002210 RID: 8720
	public List<Evidence.DataKey> allDataKeys = new List<Evidence.DataKey>();

	// Token: 0x04002211 RID: 8721
	public List<Descriptors.EthnicGroup> allEthnicities = new List<Descriptors.EthnicGroup>();

	// Token: 0x04002212 RID: 8722
	public List<DDSSaveClasses.TreeTriggers> allTreeTriggers = new List<DDSSaveClasses.TreeTriggers>();

	// Token: 0x04002213 RID: 8723
	public List<Acquaintance.ConnectionType> allConnectionTypes = new List<Acquaintance.ConnectionType>();

	// Token: 0x04002214 RID: 8724
	public List<CompanyPreset.CompanyCategory> allCompanyCategories = new List<CompanyPreset.CompanyCategory>();

	// Token: 0x04002215 RID: 8725
	public List<RetailItemPreset> allItems = new List<RetailItemPreset>();

	// Token: 0x04002216 RID: 8726
	public List<ArtPreset> allArt = new List<ArtPreset>();

	// Token: 0x04002217 RID: 8727
	public List<CompanyPreset> allCompanyPresets = new List<CompanyPreset>();

	// Token: 0x04002218 RID: 8728
	public List<OccupationPreset> allJobs = new List<OccupationPreset>();

	// Token: 0x04002219 RID: 8729
	public List<OccupationPreset> allCriminalJobs = new List<OccupationPreset>();

	// Token: 0x0400221A RID: 8730
	public List<StreetTilePreset> allStreetTiles = new List<StreetTilePreset>();

	// Token: 0x0400221B RID: 8731
	public List<ClothesPreset.OutfitCategory> allOutfitCategories = new List<ClothesPreset.OutfitCategory>();

	// Token: 0x0400221C RID: 8732
	public List<CitizenOutfitController.CharacterAnchor> allCharacterAnchors = new List<CitizenOutfitController.CharacterAnchor>();

	// Token: 0x0400221D RID: 8733
	public List<SyncDiskPreset> allSyncDisks = new List<SyncDiskPreset>();

	// Token: 0x0400221E RID: 8734
	public List<AmbientZone> allAmbientZones = new List<AmbientZone>();

	// Token: 0x0400221F RID: 8735
	public List<JobPreset> allSideJobs = new List<JobPreset>();

	// Token: 0x04002220 RID: 8736
	public List<DistrictPreset> allDistricts = new List<DistrictPreset>();

	// Token: 0x04002221 RID: 8737
	public List<HandwritingPreset> allHandwriting = new List<HandwritingPreset>();

	// Token: 0x04002222 RID: 8738
	public List<BroadcastSchedule> allBroadcasts = new List<BroadcastSchedule>();

	// Token: 0x04002223 RID: 8739
	public Dictionary<string, EvidencePreset> evidencePresetDictionary = new Dictionary<string, EvidencePreset>();

	// Token: 0x04002224 RID: 8740
	public Dictionary<string, FactPreset> factPresetDictionary = new Dictionary<string, FactPreset>();

	// Token: 0x04002225 RID: 8741
	public List<GroupPreset> allGroups = new List<GroupPreset>();

	// Token: 0x04002226 RID: 8742
	public Dictionary<string, GroupPreset> groupsDictionary = new Dictionary<string, GroupPreset>();

	// Token: 0x04002227 RID: 8743
	public Dictionary<string, DDSScope> scopeDictionary = new Dictionary<string, DDSScope>();

	// Token: 0x04002228 RID: 8744
	public Dictionary<string, DDSScope> globalScopeDictionary = new Dictionary<string, DDSScope>();

	// Token: 0x04002229 RID: 8745
	public CloudSaveData devCloudSaveConfig;

	// Token: 0x0400222A RID: 8746
	public Dictionary<string, InteractablePreset> objectPresetDictionary = new Dictionary<string, InteractablePreset>();

	// Token: 0x0400222B RID: 8747
	public List<InteractablePreset> placeAtGameLocationInteractables = new List<InteractablePreset>();

	// Token: 0x0400222C RID: 8748
	public List<InteractablePreset> placePerOwnerInteractables = new List<InteractablePreset>();

	// Token: 0x0400222D RID: 8749
	public Dictionary<SubObjectClassPreset, List<InteractablePreset>> subObjectsDictionary = new Dictionary<SubObjectClassPreset, List<InteractablePreset>>();

	// Token: 0x0400222E RID: 8750
	public Dictionary<string, AudioEvent> voiceActedDictionary = new Dictionary<string, AudioEvent>();

	// Token: 0x0400222F RID: 8751
	public List<MurderPreset> allMurderPresets = new List<MurderPreset>();

	// Token: 0x04002230 RID: 8752
	public List<MurderMO> allMurderMOs = new List<MurderMO>();

	// Token: 0x04002231 RID: 8753
	public List<CharacterTrait> allCharacterTraits = new List<CharacterTrait>();

	// Token: 0x04002232 RID: 8754
	public List<CharacterTrait> stage0Traits = new List<CharacterTrait>();

	// Token: 0x04002233 RID: 8755
	public List<CharacterTrait> stage1Traits = new List<CharacterTrait>();

	// Token: 0x04002234 RID: 8756
	public List<CharacterTrait> stage2Traits = new List<CharacterTrait>();

	// Token: 0x04002235 RID: 8757
	public List<CharacterTrait> stage3Traits = new List<CharacterTrait>();

	// Token: 0x04002236 RID: 8758
	public List<CharacterTrait> reasons = new List<CharacterTrait>();

	// Token: 0x04002237 RID: 8759
	public List<BookPreset> allBooks = new List<BookPreset>();

	// Token: 0x04002238 RID: 8760
	public List<AddressPreset> allAddressPresets = new List<AddressPreset>();

	// Token: 0x04002239 RID: 8761
	public List<RoomConfiguration> allRoomConfigs = new List<RoomConfiguration>();

	// Token: 0x0400223A RID: 8762
	public List<DesignStylePreset> allDesignStyles;

	// Token: 0x0400223B RID: 8763
	public List<MaterialGroupPreset> allMaterialGroups = new List<MaterialGroupPreset>();

	// Token: 0x0400223C RID: 8764
	public List<WallFrontagePreset> allWallFrontage = new List<WallFrontagePreset>();

	// Token: 0x0400223D RID: 8765
	public Dictionary<DesignStylePreset, List<FurniturePreset>> furnitureDesignStyleRef = new Dictionary<DesignStylePreset, List<FurniturePreset>>();

	// Token: 0x0400223E RID: 8766
	public Dictionary<RoomClassPreset, HashSet<FurniturePreset>> furnitureRoomTypeRef = new Dictionary<RoomClassPreset, HashSet<FurniturePreset>>();

	// Token: 0x0400223F RID: 8767
	public Dictionary<DesignStylePreset, Dictionary<MaterialGroupPreset.MaterialType, List<MaterialGroupPreset>>> materialDesignStyleRef = new Dictionary<DesignStylePreset, Dictionary<MaterialGroupPreset.MaterialType, List<MaterialGroupPreset>>>();

	// Token: 0x04002240 RID: 8768
	public Dictionary<DesignStylePreset, Dictionary<WallFrontageClass, List<WallFrontagePreset>>> wallFrontageStyleRef = new Dictionary<DesignStylePreset, Dictionary<WallFrontageClass, List<WallFrontagePreset>>>();

	// Token: 0x04002241 RID: 8769
	public List<FurnitureCluster> allFurnitureClusters;

	// Token: 0x04002242 RID: 8770
	public List<FurniturePreset> allFurniture = new List<FurniturePreset>();

	// Token: 0x04002243 RID: 8771
	public List<RoomLightingPreset> allRoomLighting;

	// Token: 0x04002244 RID: 8772
	public List<ColourSchemePreset> allColourSchemes;

	// Token: 0x04002245 RID: 8773
	public List<AIGoalPreset> allGoals;

	// Token: 0x04002246 RID: 8774
	public List<DialogPreset> allDialog;

	// Token: 0x04002247 RID: 8775
	public List<DialogPreset> defaultDialogOptions;

	// Token: 0x04002248 RID: 8776
	public List<InteractablePreset> allWeapons = new List<InteractablePreset>();

	// Token: 0x04002249 RID: 8777
	public Dictionary<string, DDSSaveClasses.DDSBlockSave> allDDSBlocks = new Dictionary<string, DDSSaveClasses.DDSBlockSave>();

	// Token: 0x0400224A RID: 8778
	public Dictionary<string, DDSSaveClasses.DDSMessageSave> allDDSMessages = new Dictionary<string, DDSSaveClasses.DDSMessageSave>();

	// Token: 0x0400224B RID: 8779
	public Dictionary<string, DDSSaveClasses.DDSTreeSave> allDDSTrees = new Dictionary<string, DDSSaveClasses.DDSTreeSave>();

	// Token: 0x0400224C RID: 8780
	public List<DDSSaveClasses.DDSTreeSave> allArticleTrees = new List<DDSSaveClasses.DDSTreeSave>();

	// Token: 0x0400224D RID: 8781
	public Dictionary<string, HelpContentPage> allHelpContent = new Dictionary<string, HelpContentPage>();

	// Token: 0x0400224E RID: 8782
	public List<ClothesPreset> allClothes = new List<ClothesPreset>();

	// Token: 0x0400224F RID: 8783
	public Dictionary<string, ClothesPreset> clothesDictionary = new Dictionary<string, ClothesPreset>();

	// Token: 0x04002250 RID: 8784
	public List<StatusPreset> allStatuses = new List<StatusPreset>();

	// Token: 0x04002251 RID: 8785
	public int aiSightingLayerMask;

	// Token: 0x04002252 RID: 8786
	public int interactionRayLayerMask;

	// Token: 0x04002253 RID: 8787
	public int interactionRayLayerMaskNoRoomMesh;

	// Token: 0x04002254 RID: 8788
	public int printDetectionRayLayerMask;

	// Token: 0x04002255 RID: 8789
	public int sceneCaptureLayerMask;

	// Token: 0x04002256 RID: 8790
	public int mugShotCaptureLayerMask;

	// Token: 0x04002257 RID: 8791
	public int physicalObjectsLayerMask;

	// Token: 0x04002258 RID: 8792
	public int playerMovementLayerMask;

	// Token: 0x04002259 RID: 8793
	public int autoTravelMovementLayerMask;

	// Token: 0x0400225A RID: 8794
	public int heldObjectsObjectsLayerMask;

	// Token: 0x0400225B RID: 8795
	public int spatterLayerMask;

	// Token: 0x0400225C RID: 8796
	public int textToImageMask;

	// Token: 0x0400225D RID: 8797
	public int lightCullingMask;

	// Token: 0x0400225E RID: 8798
	public int sniperLOSMask;

	// Token: 0x0400225F RID: 8799
	private List<Descriptors.EthnicGroup> rEthnicity = new List<Descriptors.EthnicGroup>();

	// Token: 0x04002260 RID: 8800
	public int totalEthnictiyFrequencyCount;

	// Token: 0x04002261 RID: 8801
	public char[] alphabet = new char[]
	{
		'A',
		'B',
		'C',
		'D',
		'E',
		'F',
		'G',
		'H',
		'I',
		'J',
		'K',
		'L',
		'M',
		'N',
		'O',
		'P',
		'Q',
		'R',
		'S',
		'T',
		'U',
		'V',
		'W',
		'X',
		'Y',
		'Z'
	};

	// Token: 0x04002262 RID: 8802
	private Dictionary<Type, Dictionary<string, ScriptableObject>> resourcesCache = new Dictionary<Type, Dictionary<string, ScriptableObject>>();

	// Token: 0x04002263 RID: 8803
	public Dictionary<Material, MaterialGroupPreset> materialProperties = new Dictionary<Material, MaterialGroupPreset>();

	// Token: 0x04002264 RID: 8804
	public Dictionary<Mesh, FurniturePreset> furnitureMeshReference = new Dictionary<Mesh, FurniturePreset>();

	// Token: 0x04002265 RID: 8805
	public string lastRandomNumberKey = string.Empty;

	// Token: 0x04002266 RID: 8806
	private char[] seedLetters = new char[]
	{
		'A',
		'B',
		'C',
		'D',
		'E',
		'F',
		'G',
		'H',
		'I',
		'J',
		'K',
		'L',
		'M',
		'N',
		'O',
		'P',
		'Q',
		'R',
		'S',
		'T',
		'U',
		'V',
		'W',
		'X',
		'Y',
		'Z'
	};

	// Token: 0x04002267 RID: 8807
	private char[] seedNumbers = new char[]
	{
		'0',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9'
	};

	// Token: 0x04002268 RID: 8808
	[Header("Debug")]
	public Vector2 debugTimeRange1;

	// Token: 0x04002269 RID: 8809
	public Vector2 debugTimeRange2;

	// Token: 0x0400226A RID: 8810
	private static Toolbox _instance;

	// Token: 0x020004A5 RID: 1189
	[Serializable]
	public class MaterialKey
	{
		// Token: 0x060019E4 RID: 6628 RVA: 0x00180CB2 File Offset: 0x0017EEB2
		public bool Equals(Toolbox.MaterialKey other)
		{
			return object.Equals(other, this);
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x00180CBC File Offset: 0x0017EEBC
		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			Toolbox.MaterialKey materialKey = (Toolbox.MaterialKey)obj;
			if (materialKey.baseMaterial != null && this.baseMaterial != null)
			{
				return materialKey.baseMaterial.name == this.baseMaterial.name && this.Approximately(materialKey.mainColour, this.mainColour) && this.Approximately(materialKey.colour1, this.colour1) && this.Approximately(materialKey.colour2, this.colour2) && this.Approximately(materialKey.colour3, this.colour3) && Mathf.Approximately(materialKey.grubiness, this.grubiness);
			}
			return this.Approximately(materialKey.mainColour, this.mainColour) && this.Approximately(materialKey.colour1, this.colour1) && this.Approximately(materialKey.colour2, this.colour2) && this.Approximately(materialKey.colour3, this.colour3) && Mathf.Approximately(materialKey.grubiness, this.grubiness);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00180DF0 File Offset: 0x0017EFF0
		private bool Approximately(Color colour1, Color colour2)
		{
			return Mathf.Approximately(colour1.r, colour2.r) && Mathf.Approximately(colour1.g, colour2.g) && Mathf.Approximately(colour1.b, colour2.b) && Mathf.Approximately(colour1.a, colour2.a);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00180E4C File Offset: 0x0017F04C
		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			if (this.baseMaterial != null)
			{
				hashCode.Add<string>(this.baseMaterial.name);
			}
			hashCode.Add<Color>(this.mainColour);
			hashCode.Add<Color>(this.colour1);
			hashCode.Add<Color>(this.colour2);
			hashCode.Add<Color>(this.colour3);
			hashCode.Add<float>(this.grubiness);
			return hashCode.ToHashCode();
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x00180EC9 File Offset: 0x0017F0C9
		public static bool operator ==(Toolbox.MaterialKey c1, Toolbox.MaterialKey c2)
		{
			return (c1 == null && c2 == null) || ((c1 != null || c2 == null) && (c1 == null || c2 != null) && c1.Equals(c2));
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x00180EEA File Offset: 0x0017F0EA
		public static bool operator !=(Toolbox.MaterialKey c1, Toolbox.MaterialKey c2)
		{
			return (c1 != null || c2 != null) && ((c1 == null && c2 != null) || (c1 != null && c2 == null) || !c1.Equals(c2));
		}

		// Token: 0x0400226B RID: 8811
		public Material baseMaterial;

		// Token: 0x0400226C RID: 8812
		public Color mainColour;

		// Token: 0x0400226D RID: 8813
		public Color colour1;

		// Token: 0x0400226E RID: 8814
		public Color colour2;

		// Token: 0x0400226F RID: 8815
		public Color colour3;

		// Token: 0x04002270 RID: 8816
		public float grubiness;
	}

	// Token: 0x020004A6 RID: 1190
	public struct SpecialItemPlacement
	{
		// Token: 0x04002271 RID: 8817
		public string reference;

		// Token: 0x04002272 RID: 8818
		public InteractablePreset preset;

		// Token: 0x04002273 RID: 8819
		public Human belongsTo;

		// Token: 0x04002274 RID: 8820
		public object passedObject;
	}

	// Token: 0x020004A7 RID: 1191
	public enum LayerMaskMode
	{
		// Token: 0x04002276 RID: 8822
		castAllExcept,
		// Token: 0x04002277 RID: 8823
		onlyCast
	}
}
