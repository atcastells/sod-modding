using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using UnityEngine;

// Token: 0x020003E7 RID: 999
[Serializable]
public class Interactable : IEquatable<Interactable>
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x0600168B RID: 5771 RVA: 0x00150ED0 File Offset: 0x0014F0D0
	// (remove) Token: 0x0600168C RID: 5772 RVA: 0x00150F08 File Offset: 0x0014F108
	public event Interactable.SwitchChange OnSwitchChange;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x0600168D RID: 5773 RVA: 0x00150F40 File Offset: 0x0014F140
	// (remove) Token: 0x0600168E RID: 5774 RVA: 0x00150F78 File Offset: 0x0014F178
	public event Interactable.State1Change OnState1Change;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x0600168F RID: 5775 RVA: 0x00150FB0 File Offset: 0x0014F1B0
	// (remove) Token: 0x06001690 RID: 5776 RVA: 0x00150FE8 File Offset: 0x0014F1E8
	public event Interactable.Deleted OnDelete;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06001691 RID: 5777 RVA: 0x00151020 File Offset: 0x0014F220
	// (remove) Token: 0x06001692 RID: 5778 RVA: 0x00151058 File Offset: 0x0014F258
	public event Interactable.RemovedFromWorld OnRemovedFromWorld;

	// Token: 0x06001693 RID: 5779 RVA: 0x00151090 File Offset: 0x0014F290
	public Interactable(InteractablePreset newPreset)
	{
		this.preset = newPreset;
		this.p = this.preset.presetName;
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x0011D797 File Offset: 0x0011B997
	bool IEquatable<Interactable>.Equals(Interactable other)
	{
		return other.GetHashCode() == this.GetHashCode();
	}

	// Token: 0x06001695 RID: 5781 RVA: 0x0015117A File Offset: 0x0014F37A
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x00151184 File Offset: 0x0014F384
	public void MainSetupStart()
	{
		this.isSetup = false;
		CityData.Instance.interactableDirectory.Add(this);
		if (this.id != 0)
		{
			Interactable interactable;
			if (CityData.Instance.savableInteractableDictionary.TryGetValue(this.id, ref interactable))
			{
				Game.LogError(string.Concat(new string[]
				{
					"Interactable ",
					this.id.ToString(),
					" already exists (Existing: ",
					interactable.GetName(),
					" at ",
					interactable.GetWorldPosition(true).ToString(),
					" Loaded from state save: ",
					interactable.wasLoadedFromSave.ToString(),
					", New: ",
					this.p,
					" at ",
					this.GetWorldPosition(true).ToString(),
					") (",
					CityData.Instance.interactableDirectory.Count.ToString(),
					" interactables with ID assign of ",
					Interactable.worldAssignID.ToString(),
					")"
				}), 2);
			}
			else
			{
				CityData.Instance.savableInteractableDictionary.Add(this.id, this);
			}
		}
		this.UpdatePassedVariables();
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x001512D4 File Offset: 0x0014F4D4
	public void UpdatePassedVariables()
	{
		if (this.pv != null)
		{
			for (int i = 0; i < this.pv.Count; i++)
			{
				Interactable.Passed p = this.pv[i];
				if (p.varType == Interactable.PassedVarType.jobID)
				{
					if (SideJobController.Instance.allJobsDictionary.TryGetValue((int)p.value, ref this.jobParent))
					{
						Game.Log("Jobs: Interactable " + this.id.ToString() + " assigned job ID " + p.value.ToString(), 2);
					}
					else
					{
						Game.LogError(string.Concat(new string[]
						{
							"Jobs: Unable to find job with ID ",
							((int)p.value).ToString(),
							", current jobs loaded: ",
							SideJobController.Instance.allJobsDictionary.Count.ToString(),
							". If no jobs are loaded, this is probably an object created in city data that has been assigned to a mission..."
						}), 2);
					}
				}
				else if (p.varType == Interactable.PassedVarType.murderID)
				{
					this.murderParent = MurderController.Instance.activeMurders.Find((MurderController.Murder item) => item.murderID == (int)p.value);
					if (this.murderParent == null)
					{
						this.murderParent = MurderController.Instance.inactiveMurders.Find((MurderController.Murder item) => item.murderID == (int)p.value);
					}
					if (this.murderParent == null)
					{
						Game.LogError(string.Concat(new string[]
						{
							"Murder: Unable to find murder with ID ",
							((int)p.value).ToString(),
							", current murders loaded: ",
							(MurderController.Instance.activeMurders.Count + MurderController.Instance.inactiveMurders.Count).ToString(),
							". If no murders are loaded, this is probably an object created in city data that has been assigned to a murder..."
						}), 2);
					}
				}
				else if (p.varType == Interactable.PassedVarType.consumableAmount)
				{
					this.cs = p.value;
				}
				else if (p.varType == Interactable.PassedVarType.isTrash)
				{
					this.MarkAsTrash(true, true, p.value);
					if (this.preset != null && this.preset.consumableAmount > 0f && this.cs > 0.09f)
					{
						this.cs = Toolbox.Instance.Rand(0f, 0.09f, false);
					}
				}
				else if (p.varType == Interactable.PassedVarType.jobTag)
				{
					if (this.jobParent != null)
					{
						if (this.jobParent.activeJobItems == null)
						{
							this.jobParent.activeJobItems = new Dictionary<JobPreset.JobTag, Interactable>();
						}
						if (this.jobParent.activeJobItems.ContainsKey((JobPreset.JobTag)Mathf.RoundToInt(p.value)))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Jobs: Item tag ",
								((JobPreset.JobTag)Mathf.RoundToInt(p.value)).ToString(),
								" already existings for job! This interactable: ",
								this.id.ToString(),
								", exisiting interactable: ",
								this.jobParent.activeJobItems[(JobPreset.JobTag)Mathf.RoundToInt(p.value)].id.ToString(),
								" ",
								this.jobParent.activeJobItems[(JobPreset.JobTag)Mathf.RoundToInt(p.value)].GetName()
							}), 2);
						}
						else
						{
							Game.Log(string.Concat(new string[]
							{
								"Jobs: Loading in job item ",
								((JobPreset.JobTag)Mathf.RoundToInt(p.value)).ToString(),
								" for job ",
								this.jobParent.jobID.ToString(),
								" (",
								this.id.ToString(),
								")"
							}), 2);
							this.jobParent.activeJobItems.Add((JobPreset.JobTag)Mathf.RoundToInt(p.value), this);
						}
					}
					else if (this.murderParent != null)
					{
						if (this.murderParent.activeMurderItems == null)
						{
							this.murderParent.activeMurderItems = new Dictionary<JobPreset.JobTag, Interactable>();
						}
						if (this.murderParent.activeMurderItems.ContainsKey((JobPreset.JobTag)Mathf.RoundToInt(p.value)))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Murder: Item tag ",
								((JobPreset.JobTag)Mathf.RoundToInt(p.value)).ToString(),
								" already existings for murder!  This interactable: ",
								this.id.ToString(),
								", exisiting interactable: ",
								this.murderParent.activeMurderItems[(JobPreset.JobTag)Mathf.RoundToInt(p.value)].id.ToString(),
								" ",
								this.murderParent.activeMurderItems[(JobPreset.JobTag)Mathf.RoundToInt(p.value)].GetName()
							}), 2);
						}
						else
						{
							Game.Log(string.Concat(new string[]
							{
								"Jobs: Loading in murder item ",
								((JobPreset.JobTag)Mathf.RoundToInt(p.value)).ToString(),
								" (",
								this.id.ToString(),
								")"
							}), 2);
							this.murderParent.activeMurderItems.Add((JobPreset.JobTag)Mathf.RoundToInt(p.value), this);
						}
					}
					else
					{
						Game.LogError("Job/Murder tagged items are being assiged before murder ID. Does this murder exist? This is probably an object created in city data that has been assigned to a murder...", 2);
					}
				}
				else if (p.varType == Interactable.PassedVarType.groupID)
				{
					this.group = GroupsController.Instance.groups.Find((GroupsController.SocialGroup item) => (float)item.id == p.value);
				}
				else if (p.varType == Interactable.PassedVarType.addressID)
				{
					CityData.Instance.addressDictionary.TryGetValue(Mathf.RoundToInt(p.value), ref this.forSale);
				}
				else if (p.varType == Interactable.PassedVarType.decal)
				{
					ArtPreset artPreset = null;
					if (Toolbox.Instance.LoadDataFromResources<ArtPreset>(p.str, out artPreset))
					{
						this.objectRef = artPreset;
					}
				}
				else if (p.varType == Interactable.PassedVarType.ownedByAddress)
				{
					NewAddress newAddress = null;
					if (CityData.Instance.addressDictionary.TryGetValue(Mathf.RoundToInt(p.value), ref newAddress))
					{
						this.objectRef = newAddress;
					}
				}
			}
		}
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x00151980 File Offset: 0x0014FB80
	public void OnCreate()
	{
		this.seed = this.id.ToString();
		if (this.preset.spawnable && this.preset.prefab != null)
		{
			this.lEuler += this.preset.prefabLocalEuler;
		}
		this.SetValue((float)Toolbox.Instance.RandContained(Mathf.FloorToInt(this.preset.value.x), Mathf.CeilToInt(this.preset.value.y) + 1, this.seed, out this.seed));
		this.cs = this.preset.consumableAmount;
		if (this.preset.useOwnColourSettings)
		{
			Toolbox.MaterialKey materialKey = new Toolbox.MaterialKey();
			materialKey.baseMaterial = null;
			if (this.preset.mainColour != InteractablePreset.InteractableColourSetting.none)
			{
				if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.randomDecorColour && this.node != null)
				{
					materialKey.mainColour = MaterialsController.Instance.GetColourFromScheme(this.node.room.colourScheme, MaterialGroupPreset.MaterialColour.any, this.node.room, this.node.building);
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.ownersFavColour && this.belongsTo != null)
				{
					materialKey.mainColour = SocialStatistics.Instance.favouriteColoursPool[this.belongsTo.favColourIndex];
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.randomColour)
				{
					materialKey.mainColour = SocialStatistics.Instance.favouriteColoursPool[Toolbox.Instance.RandContained(0, SocialStatistics.Instance.favouriteColoursPool.Count, this.seed, out this.seed)];
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.syncDisk && this.syncDisk != null)
				{
					GameplayControls.SyncDiskColour syncDiskColour = GameplayControls.Instance.syncDiskColours.Find((GameplayControls.SyncDiskColour item) => item.category == this.syncDisk.manufacturer);
					if (syncDiskColour != null)
					{
						materialKey.mainColour = syncDiskColour.mainColour;
					}
				}
			}
			if (this.preset.customColour1 != InteractablePreset.InteractableColourSetting.none)
			{
				if (this.preset.customColour1 == InteractablePreset.InteractableColourSetting.randomDecorColour && this.node != null)
				{
					materialKey.colour1 = MaterialsController.Instance.GetColourFromScheme(this.node.room.colourScheme, MaterialGroupPreset.MaterialColour.any, this.node.room, this.node.building);
				}
				else if (this.preset.customColour1 == InteractablePreset.InteractableColourSetting.ownersFavColour && this.belongsTo != null)
				{
					materialKey.colour1 = SocialStatistics.Instance.favouriteColoursPool[this.belongsTo.favColourIndex];
				}
				else if (this.preset.customColour1 == InteractablePreset.InteractableColourSetting.randomColour)
				{
					materialKey.colour1 = SocialStatistics.Instance.favouriteColoursPool[Toolbox.Instance.RandContained(0, SocialStatistics.Instance.favouriteColoursPool.Count, this.seed, out this.seed)];
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.syncDisk && this.syncDisk != null)
				{
					GameplayControls.SyncDiskColour syncDiskColour2 = GameplayControls.Instance.syncDiskColours.Find((GameplayControls.SyncDiskColour item) => item.category == this.syncDisk.manufacturer);
					if (syncDiskColour2 != null)
					{
						materialKey.colour1 = syncDiskColour2.colour1;
					}
				}
			}
			if (this.preset.customColour2 != InteractablePreset.InteractableColourSetting.none)
			{
				if (this.preset.customColour2 == InteractablePreset.InteractableColourSetting.randomDecorColour && this.node != null)
				{
					materialKey.colour2 = MaterialsController.Instance.GetColourFromScheme(this.node.room.colourScheme, MaterialGroupPreset.MaterialColour.any, this.node.room, this.node.building);
				}
				else if (this.preset.customColour2 == InteractablePreset.InteractableColourSetting.ownersFavColour && this.belongsTo != null)
				{
					materialKey.colour2 = SocialStatistics.Instance.favouriteColoursPool[this.belongsTo.favColourIndex];
				}
				else if (this.preset.customColour2 == InteractablePreset.InteractableColourSetting.randomColour)
				{
					materialKey.colour2 = SocialStatistics.Instance.favouriteColoursPool[Toolbox.Instance.RandContained(0, SocialStatistics.Instance.favouriteColoursPool.Count, this.seed, out this.seed)];
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.syncDisk && this.syncDisk != null)
				{
					GameplayControls.SyncDiskColour syncDiskColour3 = GameplayControls.Instance.syncDiskColours.Find((GameplayControls.SyncDiskColour item) => item.category == this.syncDisk.manufacturer);
					if (syncDiskColour3 != null)
					{
						materialKey.colour2 = syncDiskColour3.colour2;
					}
				}
			}
			if (this.preset.customColour3 != InteractablePreset.InteractableColourSetting.none)
			{
				if (this.preset.customColour3 == InteractablePreset.InteractableColourSetting.randomDecorColour && this.node != null)
				{
					materialKey.colour3 = MaterialsController.Instance.GetColourFromScheme(this.node.room.colourScheme, MaterialGroupPreset.MaterialColour.any, this.node.room, this.node.building);
				}
				else if (this.preset.customColour3 == InteractablePreset.InteractableColourSetting.ownersFavColour && this.belongsTo != null)
				{
					materialKey.colour3 = SocialStatistics.Instance.favouriteColoursPool[this.belongsTo.favColourIndex];
				}
				else if (this.preset.customColour3 == InteractablePreset.InteractableColourSetting.randomColour)
				{
					materialKey.colour3 = SocialStatistics.Instance.favouriteColoursPool[Toolbox.Instance.RandContained(0, SocialStatistics.Instance.favouriteColoursPool.Count, this.seed, out this.seed)];
				}
				else if (this.preset.mainColour == InteractablePreset.InteractableColourSetting.syncDisk && this.syncDisk != null)
				{
					GameplayControls.SyncDiskColour syncDiskColour4 = GameplayControls.Instance.syncDiskColours.Find((GameplayControls.SyncDiskColour item) => item.category == this.syncDisk.manufacturer);
					if (syncDiskColour4 != null)
					{
						materialKey.colour2 = syncDiskColour4.colour2;
					}
				}
			}
			if (this.preset.inheritGrubValue)
			{
				materialKey.grubiness = this.node.room.defaultWallKey.grubiness;
			}
			this.SetMaterialKey(materialKey);
		}
		if (this.preset.subSpawnPositions.Count > 0)
		{
			this.ssp = new List<InteractablePreset.SubSpawnSlot>(this.preset.subSpawnPositions);
		}
		if (!this.ml)
		{
			this.sw0 = this.preset.startingSwitchState;
		}
		this.sw1 = this.preset.startingCustomState1;
		this.sw2 = this.preset.startingCustomState2;
		this.sw3 = this.preset.startingCustomState3;
		this.locked = this.preset.startingLockState;
		if (this.preset.markAsTrashOnCreate)
		{
			this.MarkAsTrash(true, false, 0f);
		}
		this.drm = this.preset.distanceRecognitionEnabled;
		this.UpdateName(false, Evidence.DataKey.name);
		if (SessionData.Instance.startedGame || (CityConstructor.Instance != null && CityConstructor.Instance.loadState > CityConstructor.LoadState.savingData))
		{
			this.cr = true;
			if (this.preset.recordCreationTime)
			{
				if (this.pv == null)
				{
					this.pv = new List<Interactable.Passed>();
				}
				this.pv.Add(new Interactable.Passed(Interactable.PassedVarType.creationTime, SessionData.Instance.gameTime, null));
			}
		}
		this.UpdateWorldPositionAndNode(true);
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerSecurity)
		{
			if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.thisAsAddress.addressPreset.useOwnBreakerBox)
			{
				this.node.gameLocation.thisAsAddress.SetBreakerSecurity(this);
			}
			else if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.floor != null)
			{
				this.node.gameLocation.floor.SetBreakerSecurity(this);
			}
		}
		else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerDoors)
		{
			if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.thisAsAddress.addressPreset.useOwnBreakerBox)
			{
				this.node.gameLocation.thisAsAddress.SetBreakerDoors(this);
			}
			else if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.floor != null)
			{
				this.node.gameLocation.floor.SetBreakerDoors(this);
			}
		}
		else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerLights)
		{
			if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.thisAsAddress.addressPreset.useOwnBreakerBox)
			{
				this.node.gameLocation.thisAsAddress.SetBreakerLights(this);
			}
			else if (this.node.gameLocation.thisAsAddress != null && this.node.gameLocation.floor != null)
			{
				this.node.gameLocation.floor.SetBreakerLights(this);
			}
		}
		this.MainSetupEnd();
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x00152294 File Offset: 0x00150494
	public void OnLoad()
	{
		this.seed = this.id.ToString();
		this.currentActions = new Dictionary<InteractablePreset.InteractionKey, Interactable.InteractableCurrentAction>();
		this.highlightActions = new List<InteractablePreset.InteractionAction>();
		this.disabledActions = new List<InteractablePreset.InteractionAction>();
		this.aiActionReference = new Dictionary<AIActionPreset, InteractablePreset.InteractionAction>();
		this.actionAudioEventOverrides = new Dictionary<AIActionPreset, AudioEvent>();
		this.loopingAudio = new List<AudioController.LoopingSoundInfo>();
		if (this.df == null)
		{
			this.df = new List<Interactable.DynamicFingerprint>();
		}
		if (this.cap == null)
		{
			this.cap = new List<SceneRecorder.SceneCapture>();
		}
		if (this.sCap == null)
		{
			this.sCap = new List<SceneRecorder.SceneCapture>();
		}
		if (!Toolbox.Instance.objectPresetDictionary.TryGetValue(this.p, ref this.preset))
		{
			Game.LogError("Interactables unable to find preset for " + this.p, 2);
		}
		if (this.b > 0 && this.belongsTo == null && CityData.Instance.GetHuman(this.b, out this.belongsTo, true))
		{
			this.SetOwner(this.belongsTo, false);
		}
		if (this.r > 0 && this.reciever == null && CityData.Instance.GetHuman(this.r, out this.reciever, true))
		{
			this.SetReciever(this.reciever);
		}
		if (this.w > 0 && this.writer == null && CityData.Instance.GetHuman(this.w, out this.writer, true))
		{
			this.SetWriter(this.writer);
		}
		if (this.bo != null && this.bo.Length > 0 && this.book == null)
		{
			Toolbox.Instance.LoadDataFromResources<BookPreset>(this.bo, out this.book);
		}
		if (this.sd != null && this.sd.Length > 0 && this.syncDisk == null)
		{
			Toolbox.Instance.LoadDataFromResources<SyncDiskPreset>(this.sd, out this.syncDisk);
		}
		this.SetValue(this.val);
		if (!this.wo && this.fp > -1 && this.furnitureParent == null)
		{
			if (CityConstructor.Instance.loadingFurnitureReference.TryGetValue(this.fp, ref this.furnitureParent))
			{
				if (this.fsoi <= -1 && !this.furnitureParent.integratedInteractables.Contains(this))
				{
					this.furnitureParent.integratedInteractables.Add(this);
				}
			}
			else
			{
				string[] array = new string[9];
				array[0] = "Unable to find furniture parent: ";
				array[1] = this.fp.ToString();
				array[2] = " (loading reference contains: ";
				array[3] = CityConstructor.Instance.loadingFurnitureReference.Count.ToString();
				array[4] = ") Interactable: ";
				array[5] = this.name;
				array[6] = " world pos: ";
				int num = 7;
				Vector3 vector = this.wPos;
				array[num] = vector.ToString();
				array[8] = " [If this is contained within evidence board it could be waiting to be deleted by the game]";
				Game.LogError(string.Concat(array), 2);
			}
		}
		if (!this.wo && this.fsoi > -1 && this.furnitureParent != null && this.furnitureParent.furniture != null && this.subObject == null)
		{
			if (this.fsoi >= this.furnitureParent.furniture.subObjects.Count)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Furniture sub object out of range! Are you sure this is the correct furniture? ",
					this.fsoi.ToString(),
					"/",
					this.furnitureParent.furniture.subObjects.Count.ToString(),
					" spawning ",
					this.name,
					" on ",
					this.furnitureParent.furniture.name
				}), 2);
			}
			else
			{
				this.subObject = this.furnitureParent.furniture.subObjects[this.fsoi];
			}
		}
		if (this.dp > -1 && this.parentTransform == null)
		{
			NewDoor newDoor = null;
			if (CityData.Instance.doorDictionary.TryGetValue(this.dp, ref newDoor))
			{
				this.parentTransform = newDoor.transform;
			}
			else
			{
				Game.LogError("Unable to find door: " + this.dp.ToString(), 2);
			}
		}
		Interactable.worldAssignID = Mathf.Max(this.id + 1, Interactable.worldAssignID);
		if (this.inv > 0)
		{
			if (CityData.Instance.GetHuman(this.inv, out this.inInventory, true))
			{
				this.SetInInventory(this.inInventory);
			}
			else
			{
				Game.LogError(string.Concat(new string[]
				{
					"Unable to find receiver citizen ID ",
					this.inv.ToString(),
					" for object ",
					this.preset.name,
					". Are you trying to load this object before citizens are created?"
				}), 2);
			}
		}
		if (this.df != null && this.df.Count > 0 && !GameplayController.Instance.objectsWithDynamicPrints.Contains(this))
		{
			GameplayController.Instance.objectsWithDynamicPrints.Add(this);
		}
		PathFinder.Instance.nodeMap.TryGetValue(this.spNode, ref this.spawnNode);
		this.UpdateWorldPositionAndNode(false);
		if (this.lp != null && this.lp.Length > 0 && this.isLight == null)
		{
			Toolbox.Instance.LoadDataFromResources<LightingPreset>(this.lp, out this.isLight);
			this.SetAsLight(this.isLight, this.lzs, this.ml, this.lcd);
		}
		if (this.wPos != this.spWPos || this.wEuler != this.spWEuler)
		{
			this.mov = true;
		}
		else
		{
			this.mov = false;
		}
		if (this.mtr > 0f)
		{
			this.MarkAsTrash(true, true, this.mtr);
		}
		this.MainSetupEnd();
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x00152888 File Offset: 0x00150A88
	public void MainSetupEnd()
	{
		if (this.evidence == null && this.preset.useEvidence)
		{
			NewGameLocation gameLocation = null;
			if (this.node != null)
			{
				gameLocation = this.node.gameLocation;
			}
			this.evidence = Toolbox.Instance.GetOrCreateEvidenceForInteractable(this.preset, "I" + this.id.ToString(), this, this.belongsTo, this.writer, this.reciever, this.jobParent, gameLocation, this.preset.retailItem, this.pv);
			if (this.dds != null && this.dds.Length > 0)
			{
				this.SetDDSOverride(this.dds);
			}
		}
		if (this.evidence != null)
		{
			this.evidence.interactable = this;
			this.evidence.interactablePreset = this.preset;
		}
		if (this.preset.isClock)
		{
			SessionData.Instance.OnHourChange += this.OnHourChange;
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.hospitalBed)
		{
			if (!GameplayController.Instance.hospitalBeds.Contains(this))
			{
				GameplayController.Instance.hospitalBeds.Add(this);
			}
		}
		else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.caseTray && !CityData.Instance.caseTrays.Contains(this))
		{
			CityData.Instance.caseTrays.Add(this);
		}
		if (this.preset.spawnable && this.subObject != null)
		{
			this.furnitureParent.spawnedInteractables.Add(this);
		}
		else if (this.furnitureParent != null && this.subObject == null)
		{
			this.furnitureParent.integratedInteractables.Add(this);
		}
		if (this.wasLoadedFromSave && this.preset.dontLoadSwitchStates)
		{
			this.ResetToDefaultSwitchState();
		}
		else
		{
			this.SetSwitchState(this.sw0, null, false, true, false);
			this.SetCustomState1(this.sw1, null, false, true, false);
			this.SetCustomState2(this.sw2, null, false, true, false);
			this.SetCustomState3(this.sw3, null, false, true, false);
			this.SetLockedState(this.locked, null, false, true);
		}
		this.SetPhysicsPickupState(this.phy, null, false, true);
		this.UpdateCurrentActions();
		foreach (InteractablePreset.InteractionAction interactionAction in this.preset.GetActions(0))
		{
			if (interactionAction.usableByAI)
			{
				this.aiActionReference.Add(interactionAction.action, interactionAction);
			}
		}
		if (this.preset.includeLock != null)
		{
			if (this.furnitureParent == null)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Unable to create lock for ",
					this.GetName(),
					" (",
					this.id.ToString(),
					") as furniture parent is null: ",
					this.fp.ToString()
				}), 2);
			}
			else
			{
				Vector3 lockOffset = this.preset.lockOffset;
				Vector3 zero = Vector3.zero;
				this.lockInteractable = InteractableCreator.Instance.CreateInteractableLock(this.preset.includeLock, this.furnitureParent, this.belongsTo, lockOffset, zero, InteractableController.InteractableID.A);
				this.lockInteractable.thisDoor = this;
				this.lockInteractable.SetLockedState(this.locked, null, true, false);
				this.lockInteractable.SetPasswordSource(this.passwordSource);
				this.lockInteractable.SetValue(Toolbox.Instance.GetPsuedoRandomNumber(Mathf.Clamp01(this.preset.lockStrength.x), Mathf.Clamp01(this.preset.lockStrength.y), this.seed, false));
				this.SetValue(this.lockInteractable.val);
				this.SetOwner(this.belongsTo, false);
				if (this.loadedGeometry)
				{
					this.SpawnLock();
				}
			}
		}
		else if (this.preset.isSelfLock)
		{
			this.lockInteractable = this;
			this.lockInteractable.thisDoor = this;
			this.lockInteractable.SetPasswordSource(this.passwordSource);
			this.SetValue(Toolbox.Instance.GetPsuedoRandomNumber(Mathf.Clamp01(this.preset.lockStrength.x), Mathf.Clamp01(this.preset.lockStrength.y), this.seed, false));
			this.SetOwner(this.belongsTo, false);
		}
		this.usagePoint = new Interactable.UsagePoint(this.preset.useSetting, this, this.node);
		if (this.node != null)
		{
			if (this.preset.specialCaseFlag != InteractablePreset.SpecialCase.none)
			{
				if (!this.node.room.specialCaseInteractables.ContainsKey(this.preset.specialCaseFlag))
				{
					this.node.room.specialCaseInteractables.Add(this.preset.specialCaseFlag, new List<Interactable>());
				}
				if (!this.node.room.specialCaseInteractables[this.preset.specialCaseFlag].Contains(this))
				{
					this.node.room.specialCaseInteractables[this.preset.specialCaseFlag].Add(this);
				}
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityDoor)
			{
				if (this.node != null)
				{
					if (this.node.floor != null)
					{
						this.node.floor.AddSecurityDoor(this);
					}
					else
					{
						string text = "Cannot place security door at node ";
						Vector3Int nodeCoord = this.node.nodeCoord;
						Game.LogError(text + nodeCoord.ToString() + " with no floor!", 2);
					}
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.alarmSystem)
			{
				if (this.node != null)
				{
					if (this.node.gameLocation != null)
					{
						if (this.node.gameLocation.thisAsAddress != null)
						{
							this.node.gameLocation.thisAsAddress.alarms.Add(this);
						}
						if (this.node.building != null)
						{
							this.node.building.alarms.Add(this);
						}
					}
					this.SetValue(this.GetSecurityStrength());
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun)
			{
				if (this.node != null)
				{
					this.cvp = this.wPos + new Vector3(0f, (float)this.furnitureParent.anchorNode.floor.defaultCeilingHeight * 0.1f - 0.56f, 0f);
					this.cve = this.wEuler + new Vector3(0f, 0f * this.furnitureParent.scaleMultiplier.x, 0f) + new Vector3(20f, 0f, 0f);
					if (this.node.gameLocation != null)
					{
						if (this.node.gameLocation.thisAsAddress != null)
						{
							this.node.gameLocation.thisAsAddress.AddSentryGun(this);
						}
						if (this.node.building != null)
						{
							this.node.building.AddSentryGun(this);
						}
					}
					this.SetValue(this.GetSecurityStrength());
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.otherSecuritySystem || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.gasReleaseSystem)
			{
				if (this.node != null)
				{
					if (this.node.gameLocation != null)
					{
						if (this.node.gameLocation.thisAsAddress != null)
						{
							this.node.gameLocation.thisAsAddress.AddOtherSecurity(this);
						}
						if (this.node.building != null)
						{
							this.node.building.AddOtherSecurity(this);
						}
					}
					this.SetValue(this.GetSecurityStrength());
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityCamera)
			{
				float num = 0f;
				Vector3 zero2 = Vector3.zero;
				if (this.node.building != null && this.furnitureParent != null && this.furnitureParent.anchorNode != null && this.furnitureParent.anchorNode.floor != null)
				{
					num = (float)this.furnitureParent.anchorNode.floor.defaultCeilingHeight * 0.1f - 0.57f;
					zero2..ctor(20f, 0f, 0f);
				}
				this.cvp = this.wPos + new Vector3(0f, num, 0f);
				this.cve = this.wEuler + new Vector3(0f, 0f * this.furnitureParent.scaleMultiplier.x, 0f) + zero2;
				if (!SessionData.Instance.isFloorEdit)
				{
					this.sceneRecorder = new SceneRecorder(this);
					if (this.node.building != null)
					{
						this.node.building.AddSecurityCamera(this);
					}
					this.node.gameLocation.AddSecurityCamera(this);
					CityData.Instance.surveillanceDirectory.Add(this.sceneRecorder);
					foreach (SceneRecorder.SceneCapture sceneCapture in this.cap)
					{
						sceneCapture.recorder = this.sceneRecorder;
					}
					foreach (SceneRecorder.SceneCapture sceneCapture2 in this.sCap)
					{
						sceneCapture2.recorder = this.sceneRecorder;
					}
				}
				this.force = true;
				this.SetValue(this.GetSecurityStrength());
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.burningBarrel)
			{
				GameplayController.Instance.burningBarrels.Add(this);
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.codebreaker)
			{
				GameplayController.Instance.activeGadgets.Add(this);
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.doorWedge)
			{
				GameplayController.Instance.activeGadgets.Add(this);
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.telephone)
			{
				Interactable.Passed passed = null;
				if (this.pv != null)
				{
					passed = this.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.phoneNumber);
				}
				if (passed == null)
				{
					this.t = new Telephone(this);
					if (this.pv == null)
					{
						this.pv = new List<Interactable.Passed>();
					}
					this.pv.Add(new Interactable.Passed(Interactable.PassedVarType.phoneNumber, (float)this.t.number, null));
					this.force = true;
				}
				else if (this.t == null)
				{
					this.t = new Telephone(this, Mathf.RoundToInt(passed.value));
				}
				this.speechController = this.node.gameLocation.gameObject.AddComponent<SpeechController>();
				this.speechController.phoneLine = this.t;
				this.speechController.interactable = this;
				if (this.t.activeReceiver == null)
				{
					if (this.t.activeCall.Count > 0)
					{
						this.SetSwitchState(true, null, true, false, false);
					}
					else
					{
						this.SetSwitchState(false, null, true, false, false);
					}
				}
				else
				{
					this.SetSwitchState(false, null, true, false, false);
				}
			}
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.fingerprint)
		{
			if (!GameplayController.Instance.confirmedPrints.ContainsKey(this.print[0].worldPos))
			{
				string text2 = "Player: Creating new fingerprint evidence at ";
				Vector3 worldPos = this.print[0].worldPos;
				Game.Log(text2 + worldPos.ToString(), 2);
				GameplayController.Instance.confirmedPrints.Add(this.print[0].worldPos, this);
			}
		}
		else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.footprint && !GameplayController.Instance.confirmedFootprints.ContainsKey(this.print[0].worldPos))
		{
			string text3 = "Player: Creating new footprint evidence at ";
			Vector3 worldPos = this.print[0].worldPos;
			Game.Log(text3 + worldPos.ToString(), 2);
			GameplayController.Instance.confirmedFootprints.Add(this.print[0].worldPos, this);
		}
		if (this.preset.affectRoomSteamLevel && this.node != null && this.node.room.steamControllingInteractables != null)
		{
			this.node.room.steamControllingInteractables.Add(this);
		}
		if (!SessionData.Instance.startedGame)
		{
			if (this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
			{
				CityConstructor.Instance.updateSwitchState.Add(this);
			}
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.addressBook && this.belongsTo != null && this.belongsTo.home != null)
		{
			if (this.pv == null)
			{
				this.pv = new List<Interactable.Passed>();
			}
			this.pv.Add(new Interactable.Passed(Interactable.PassedVarType.addressID, (float)this.belongsTo.home.id, null));
		}
		this.isSetup = true;
		if (this.rem)
		{
			this.SafeDelete(false);
			return;
		}
		if (this.rPl)
		{
			this.RemoveFromPlacement();
		}
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x00153654 File Offset: 0x00151854
	public void SpawnCheck()
	{
		if (this.node != null && this.node.room != null && this.node.room.geometryLoaded)
		{
			if (this.furnitureParent != null)
			{
				if (this.furnitureParent.spawnedObject != null)
				{
					this.LoadInteractableToWorld(false, false);
					return;
				}
			}
			else
			{
				this.LoadInteractableToWorld(false, false);
			}
		}
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x001536BC File Offset: 0x001518BC
	public void GenerateLightData()
	{
		this.lcd = new Interactable.LightConfiguration();
		Color colour = Color.white;
		if (this.node != null && this.node.room != null && this.node.room.preset.cleanness <= 4)
		{
			colour = Color.Lerp(this.isLight.warmColours[Toolbox.Instance.RandContained(0, this.isLight.warmColours.Count, this.seed, out this.seed)].colourOne, this.isLight.warmColours[Toolbox.Instance.RandContained(0, this.isLight.warmColours.Count, this.seed, out this.seed)].colourTwo, Toolbox.Instance.RandContained(0f, 1f, this.seed, out this.seed));
		}
		else if (this.node != null && this.node.room != null && this.node.room.preset.cleanness >= 6)
		{
			colour = Color.Lerp(this.isLight.coolColours[Toolbox.Instance.RandContained(0, this.isLight.coolColours.Count, this.seed, out this.seed)].colourOne, this.isLight.coolColours[Toolbox.Instance.RandContained(0, this.isLight.coolColours.Count, this.seed, out this.seed)].colourTwo, Toolbox.Instance.RandContained(0f, 1f, this.seed, out this.seed));
		}
		else
		{
			Color color = Color.Lerp(this.isLight.coolColours[Toolbox.Instance.RandContained(0, this.isLight.coolColours.Count, this.seed, out this.seed)].colourOne, this.isLight.warmColours[Toolbox.Instance.RandContained(0, this.isLight.warmColours.Count, this.seed, out this.seed)].colourOne, Toolbox.Instance.RandContained(0.33f, 0.66f, this.seed, out this.seed));
			Color color2 = Color.Lerp(this.isLight.coolColours[Toolbox.Instance.RandContained(0, this.isLight.coolColours.Count, this.seed, out this.seed)].colourTwo, this.isLight.warmColours[Toolbox.Instance.RandContained(0, this.isLight.warmColours.Count, this.seed, out this.seed)].colourTwo, Toolbox.Instance.RandContained(0.33f, 0.66f, this.seed, out this.seed));
			colour = Color.Lerp(color, color2, 0.5f);
		}
		this.lcd.colour = colour;
		this.lcd.range = this.isLight.defaultRange;
		float num = this.isLight.defaultIntensity;
		float num2 = 1f;
		if (this.node.room.lowerRoom != null)
		{
			num2 = 1.33f;
		}
		if (this.lzs < 0)
		{
			num = (float)this.node.room.nodes.Count * num2 * 100f - 50f;
		}
		else
		{
			num = (float)this.lzs * num2 * 100f - 50f;
		}
		if (this.node.room.preset.wellLit)
		{
			num *= 1.5f;
			this.lcd.range *= 3f;
		}
		this.lcd.intensity = Mathf.Clamp(num, this.isLight.intensityRange.x, this.isLight.intensityRange.y);
		this.lcd.flickerColourMultiplier = Toolbox.Instance.RandContained(this.isLight.flickerMultiplierRange.x, this.isLight.flickerMultiplierRange.y, this.seed, out this.seed);
		this.lcd.pulseSpeed = Toolbox.Instance.RandContained(this.isLight.flickerPulseRange.x, this.isLight.flickerPulseRange.y, this.seed, out this.seed);
		this.lcd.intervalTime = Toolbox.Instance.RandContained(this.isLight.flickerNormalityIntervalRange.x, this.isLight.flickerNormalityIntervalRange.y, this.seed, out this.seed);
		float num3 = 0f;
		if (this.node.room != null && this.node.room.gameLocation.floor != null && this.node.room.gameLocation.floor.floor <= CityControls.Instance.lowestFloor)
		{
			num3 = CityControls.Instance.lowestFloorIncreaseFlickerChance;
		}
		if (this.isLight.chanceOfFlicker + num3 >= Toolbox.Instance.RandContained(0f, 1f, this.seed, out this.seed))
		{
			this.lcd.flicker = true;
			return;
		}
		this.lcd.flicker = false;
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x00153C54 File Offset: 0x00151E54
	public void SetMaterialKey(Toolbox.MaterialKey newMatKey)
	{
		this.mk = newMatKey;
		if (this.mk != null)
		{
			if (this.controller != null)
			{
				using (List<MeshRenderer>.Enumerator enumerator = this.controller.meshes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MeshRenderer renderer = enumerator.Current;
						MaterialsController.Instance.ApplyMaterialKey(renderer, this.mk);
					}
					return;
				}
			}
			if (this.spawnedObject != null)
			{
				MaterialsController.Instance.ApplyMaterialKey(this.spawnedObject, this.mk);
			}
		}
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x00153D00 File Offset: 0x00151F00
	public void SetPolymorphicReference(object newRef)
	{
		this.objectRef = newRef;
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x00153D0C File Offset: 0x00151F0C
	public void SetValue(float newValue)
	{
		this.val = newValue;
		if (this.preset == null)
		{
			return;
		}
		if (this.preset.isMoney)
		{
			this.UpdateName(false, Evidence.DataKey.name);
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.doorWedge)
		{
			NewDoor newDoor = null;
			if (CityData.Instance.doorDictionary.TryGetValue((int)this.val, ref newDoor))
			{
				this.objectRef = newDoor;
				newDoor.SetJammed(true, this, true);
			}
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerLights || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerDoors || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerSecurity)
		{
			if (this.val >= 1f)
			{
				GameplayController.Instance.closedBreakers.Remove(this);
				if (!this.sw0)
				{
					this.SetSwitchState(true, null, true, true, false);
				}
			}
			else
			{
				if (!GameplayController.Instance.closedBreakers.Contains(this))
				{
					GameplayController.Instance.closedBreakers.Add(this);
				}
				if (this.sw0)
				{
					this.SetSwitchState(false, null, true, true, false);
				}
			}
			if (this.controller != null && this.controller.doorMovement != null)
			{
				this.controller.doorMovement.SetOpen(this.val, null, false);
			}
		}
		else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.bloodPool && this.spawnedObject != null)
		{
			float num = 0.3f + this.val * 0.01f;
			this.spawnedObject.transform.localScale = new Vector3(Mathf.Clamp(num, 0.1f, 1.5f), Mathf.Clamp(num, 0.1f, 1.5f), Mathf.Clamp(num, 0.1f, 1.5f));
		}
		if (this.thisDoor != null)
		{
			this.thisDoor.val = this.val;
			return;
		}
		if (this.lockInteractable != null)
		{
			this.lockInteractable.val = this.val;
		}
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x00153F00 File Offset: 0x00152100
	public void SetDDSOverride(string newTreeID)
	{
		if (newTreeID.Length > 0)
		{
			this.dds = newTreeID;
			if (this.pv == null)
			{
				this.pv = new List<Interactable.Passed>();
				this.pv.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, newTreeID));
			}
			else
			{
				Interactable.Passed passed = this.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.ddsOverride);
				if (passed != null)
				{
					passed.str = newTreeID;
				}
				else
				{
					this.pv.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, newTreeID));
				}
			}
			if (this.evidence != null)
			{
				this.evidence.SetOverrideDDS(this.dds);
				return;
			}
			Game.LogError("Trying to set DDS override but there is not (yet) any evidence: " + this.name, 2);
		}
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x00153FCC File Offset: 0x001521CC
	public void AssignIDWorld()
	{
		this.id = Interactable.worldAssignID;
		while (CityData.Instance.savableInteractableDictionary.ContainsKey(this.id) || CityData.Instance.metaObjectDictionary.ContainsKey(this.id))
		{
			this.id++;
		}
		Interactable.worldAssignID = this.id + 1;
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00154030 File Offset: 0x00152230
	public void AssignRoomBasedID(NewRoom r)
	{
		this.id = r.roomID * 1000 + r.interactableAssignID;
		if (r.interactableAssignID >= 1000 || CityData.Instance.savableInteractableDictionary.ContainsKey(this.id) || CityData.Instance.metaObjectDictionary.ContainsKey(this.id))
		{
			Game.LogError(string.Concat(new string[]
			{
				"Error trying to assign room id of ",
				this.id.ToString(),
				" to room ",
				r.roomID.ToString(),
				", so using world ID instead... (This should be fine if the game has started)"
			}), 2);
			this.AssignIDWorld();
			return;
		}
		r.interactableAssignID++;
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			r.itemsPlaced = r.itemsPlaced + ", " + this.preset.presetName;
		}
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x00154124 File Offset: 0x00152324
	public void AssignFurnitureBasedID(FurnitureLocation f)
	{
		if (f == null)
		{
			Game.LogError("Unable to assign furntiure integrated ID as a null furniture reference has been passed!", 2);
			return;
		}
		this.id = -(f.id * 30 + f.integratedIDAssign);
		f.integratedIDAssign++;
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x0015415A File Offset: 0x0015235A
	public void MoveInteractable(Vector3 newWorldPos, Vector3 newEulerAngle, bool updateSpawnPosition)
	{
		if (!this.wo)
		{
			this.ConvertToWorldObject(false);
		}
		this.wPos = newWorldPos;
		this.wEuler = newEulerAngle;
		this.lPos = newWorldPos;
		this.lEuler = newEulerAngle;
		this.mov = true;
		this.UpdateWorldPositionAndNode(updateSpawnPosition);
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x00154198 File Offset: 0x00152398
	public void SetNewPositionAndParent(Transform newParent, Vector3 newLocalPos, Vector3 newLocalEuler, bool updateSpawnPosition)
	{
		this.furnitureParent = null;
		this.parentTransform = newParent;
		this.lPos = newLocalPos;
		this.lEuler = newLocalEuler;
		if (this.controller != null)
		{
			this.controller.transform.SetParent(newParent, true);
			this.controller.transform.localPosition = this.lPos;
			this.controller.transform.localEulerAngles = this.lEuler;
		}
		this.UpdateWorldPositionAndNode(updateSpawnPosition);
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x00154218 File Offset: 0x00152418
	public void SetNewPosition(Vector3 newLocalPos, Vector3 newLocalEuler, bool updateSpawnPosition)
	{
		this.lPos = newLocalPos;
		this.lEuler = newLocalEuler;
		if (this.controller != null)
		{
			this.controller.transform.localPosition = this.lPos;
			this.controller.transform.localEulerAngles = this.lEuler;
		}
		this.UpdateWorldPositionAndNode(updateSpawnPosition);
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00154275 File Offset: 0x00152475
	public Vector3 GetWorldPosition(bool useSpawnedPosition = true)
	{
		if (this.controller != null && useSpawnedPosition)
		{
			return this.controller.transform.position;
		}
		return this.wPos;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x001542A0 File Offset: 0x001524A0
	public Vector3 UpdateWorldPositionAndNode(bool updateSpawnPosition)
	{
		NewNode newNode = null;
		if (this.isActor != null)
		{
			this.wPos = this.isActor.transform.position;
			this.wEuler = this.isActor.transform.eulerAngles;
			newNode = this.isActor.currentNode;
		}
		else if (this.inInventory != null)
		{
			this.wPos = this.inInventory.transform.position;
			this.wEuler = this.inInventory.transform.eulerAngles;
			newNode = this.inInventory.currentNode;
		}
		else if (this.wo)
		{
			Vector3 center = this.wPos;
			if (this.controller != null && this.controller.coll != null)
			{
				center = this.controller.coll.bounds.center;
			}
			if ((this.wPos.y <= -50f || this.wPos.y > 1000f) && (InteractionController.Instance.carryingObject == null || InteractionController.Instance.carryingObject.interactable != this))
			{
				if (this.controller != null)
				{
					this.controller.SetPhysics(false, null);
				}
				if (Game.Instance.printDebug)
				{
					string text = "Removing ";
					string text2 = this.name;
					string text3 = " as it is ouside of gameworld: ";
					Vector3 vector = this.wPos;
					Game.Log(text + text2 + text3 + vector.ToString(), 2);
				}
				this.SafeDelete(false);
				return Vector3.zero;
			}
			if (!PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(center), ref newNode))
			{
				newNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(center, false, true, false, default(Vector3Int), false, 0, false, 2000);
				float num = 99999f;
				if (newNode != null)
				{
					num = Vector3.Distance(center, newNode.position);
					string text4 = string.Empty;
					if (this.controller != null)
					{
						string text5 = "Physics on: ";
						string text6 = this.controller.physicsOn.ToString();
						string text7 = ", RB: ";
						Rigidbody rb = this.controller.rb;
						text4 = text5 + text6 + text7 + ((rb != null) ? rb.ToString() : null);
					}
					if (Game.Instance.printDebug)
					{
						string[] array = new string[11];
						array[0] = this.name;
						array[1] = " ";
						array[2] = this.id.ToString();
						array[3] = ": Found closest node to world position ";
						int num2 = 4;
						Vector3 vector = center;
						array[num2] = vector.ToString();
						array[5] = " manually: ";
						int num3 = 6;
						vector = newNode.position;
						array[num3] = vector.ToString();
						array[7] = " distance: ";
						array[8] = num.ToString();
						array[9] = ". This could be very slow and should be corrected. ";
						array[10] = text4;
						Game.Log(string.Concat(array), 2);
					}
				}
				else if (Game.Instance.printDebug)
				{
					string text8 = "Unable to find node for ";
					string text9 = this.name;
					string text10 = " at ";
					Vector3 vector = center;
					Game.LogError(text8 + text9 + text10 + vector.ToString(), 2);
				}
				if (num > 35f && (InteractionController.Instance.carryingObject == null || InteractionController.Instance.carryingObject.interactable != this))
				{
					Game.LogError(string.Concat(new string[]
					{
						"Distance is too far away from anything (",
						num.ToString(),
						"), removing interactable ",
						this.preset.name,
						" ",
						this.name,
						" ",
						this.id.ToString(),
						": If this is important, make sure this doesn't happen!"
					}), 2);
					this.SafeDelete(false);
				}
			}
			if (newNode != null && this.worldObjectRoomParent != newNode.room)
			{
				if (this.worldObjectRoomParent != null)
				{
					this.worldObjectRoomParent.worldObjects.Remove(this);
				}
				this.worldObjectRoomParent = newNode.room;
				this.parentTransform = newNode.room.transform;
				if (this.controller != null && this.preset.apartmentPlacementMode == InteractablePreset.ApartmentPlacementMode.physics)
				{
					this.parentTransform = newNode.room.transform;
					this.controller.transform.SetParent(this.parentTransform, true);
				}
				if (!this.worldObjectRoomParent.worldObjects.Contains(this))
				{
					this.worldObjectRoomParent.worldObjects.Add(this);
				}
			}
			if (this.parentTransform != null)
			{
				this.lPos = this.parentTransform.InverseTransformPoint(this.wPos);
			}
		}
		else if (this.parentTransform != null && this.furnitureParent == null)
		{
			this.wPos = this.parentTransform.TransformPoint(this.lPos);
			this.wEuler = this.parentTransform.eulerAngles + this.lEuler;
			if (this.wPos.y <= -100f && this.controller != null)
			{
				this.controller.SetPhysics(false, null);
				this.SafeDelete(false);
				return Vector3.zero;
			}
			if (!PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(this.wPos), ref newNode))
			{
				if (this.furnitureParent != null)
				{
					newNode = this.furnitureParent.anchorNode;
					float num4 = float.PositiveInfinity;
					using (List<NewNode>.Enumerator enumerator = this.furnitureParent.coversNodes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NewNode newNode2 = enumerator.Current;
							float num5 = Vector3.Distance(this.node.position, this.wPos);
							if (num5 < num4)
							{
								newNode = newNode2;
								num4 = num5;
							}
						}
						goto IL_82E;
					}
				}
				newNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(this.wPos, false, true, false, default(Vector3Int), false, 0, false, 200);
				if (newNode != null)
				{
					string[] array2 = new string[7];
					array2[0] = "Found closest node to world position ";
					int num6 = 1;
					Vector3 vector = this.wPos;
					array2[num6] = vector.ToString();
					array2[2] = " manually: ";
					int num7 = 3;
					vector = newNode.position;
					array2[num7] = vector.ToString();
					array2[4] = " distance: ";
					array2[5] = Vector3.Distance(this.wPos, newNode.position).ToString();
					array2[6] = ". This could be very slow and should be corrected";
					Game.Log(string.Concat(array2), 2);
				}
				else
				{
					string text11 = "Unable to find node for ";
					string text12 = this.name;
					string text13 = " at ";
					Vector3 vector = this.wPos;
					Game.LogError(text11 + text12 + text13 + vector.ToString(), 2);
				}
			}
		}
		else if (this.furnitureParent != null)
		{
			this.wPos = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0f, (float)(this.furnitureParent.angle + this.furnitureParent.diagonalAngle), 0f)), Vector3.one).MultiplyPoint3x4(this.lPos) + this.furnitureParent.anchorNode.position;
			this.wEuler = new Vector3(0f, (float)(this.furnitureParent.angle + this.furnitureParent.diagonalAngle), 0f) + this.lEuler;
			Toolbox instance = Toolbox.Instance;
			Vector3 worldPos = this.wPos;
			bool onlyAccessibleNodes = false;
			bool checkUpAndDown = true;
			bool limitToDirection = false;
			int z = this.furnitureParent.anchorNode.nodeCoord.z;
			newNode = instance.FindClosestValidNodeToWorldPosition(worldPos, onlyAccessibleNodes, checkUpAndDown, limitToDirection, default(Vector3Int), true, z, false, 200);
			if (newNode == null)
			{
				Game.Log(this.name + " unable to find a close by node, using furniture node area...", 2);
				newNode = this.furnitureParent.anchorNode;
				if (this.furnitureParent.coversNodes.Count > 0)
				{
					float num8 = float.PositiveInfinity;
					foreach (NewNode newNode3 in this.furnitureParent.coversNodes)
					{
						float num9 = Vector3.Distance(newNode3.position, this.wPos);
						if (num9 < num8)
						{
							newNode = newNode3;
							num8 = num9;
						}
					}
				}
			}
		}
		IL_82E:
		if (this.isActor == null)
		{
			if (newNode != this.node)
			{
				if (this.node != null)
				{
					this.node.RemoveInteractable(this);
				}
				if (newNode != null)
				{
					this.node = newNode;
					newNode.AddInteractable(this);
				}
			}
			if (this.spawnedObject != null)
			{
				this.spawnedObject.transform.localPosition = this.lPos;
				this.spawnedObject.transform.localEulerAngles = this.lEuler;
			}
			if (this.usagePoint != null)
			{
				this.usagePoint.PositionUpdate();
			}
		}
		else
		{
			this.node = this.isActor.currentNode;
		}
		if (Game.Instance.collectDebugData && this.controller != null)
		{
			this.controller.debugLocalEuler = this.lEuler;
			this.controller.debugLocalPos = this.lPos;
			if (this.node != null)
			{
				this.controller.debugNodeCoord = this.node.nodeCoord;
				this.controller.debugRoom = this.node.room;
			}
			this.controller.debugInteractablePredictedWorldPos = this.wPos;
		}
		if (updateSpawnPosition)
		{
			if (this.save && this.isSetup)
			{
				this.spCh = true;
			}
			this.spawnNode = this.node;
			if (this.node != null)
			{
				this.spNode = this.node.nodeCoord;
			}
			this.spawnParent = this.parentTransform;
			this.spWPos = this.wPos;
			this.spWEuler = this.wEuler;
			this.originalPosition = true;
			this.distanceFromSpawn = 0f;
			this.SetTampered(false);
		}
		else
		{
			this.distanceFromSpawn = Vector3.Distance(this.wPos, this.spWPos);
			if (this.distanceFromSpawn > 0.01f)
			{
				this.SetOriginalPosition(false, false);
			}
			else
			{
				this.originalPosition = true;
				this.distanceFromSpawn = 0f;
			}
			if (this.preset.physicsProfile != null && this.preset.tamperEnabled)
			{
				if (this.distanceFromSpawn >= GameplayControls.Instance.physicsTamperDistance + this.preset.physicsProfile.tamperDistanceModifier)
				{
					if (!this.isTampered)
					{
						this.SetTampered(true);
					}
				}
				else if (this.isTampered)
				{
					this.SetTampered(false);
				}
			}
		}
		if (this.loopingAudio != null)
		{
			foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.loopingAudio)
			{
				loopingSoundInfo.UpdateWorldPosition(this.wPos, this.node);
			}
		}
		if (this.isSetup && (this.wPos != this.spWPos || this.wEuler != this.spWEuler))
		{
			this.mov = true;
		}
		else
		{
			this.mov = false;
		}
		return this.wPos;
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00154DD8 File Offset: 0x00152FD8
	public void SetPasswordSource(object newPSource)
	{
		this.passwordSource = newPSource;
		if (newPSource == this)
		{
			this.passcode.used = true;
		}
		else if (newPSource as NewAddress != null)
		{
			(newPSource as NewAddress).passcode.used = true;
		}
		else if (newPSource as NewRoom != null)
		{
			(newPSource as NewRoom).passcode.used = true;
		}
		else if (newPSource as Human != null)
		{
			(newPSource as Human).passcode.used = true;
		}
		if (Game.Instance.collectDebugData && this.controller != null)
		{
			this.controller.debugPasswordSource = this.passwordSource;
		}
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x00154E8C File Offset: 0x0015308C
	public void SetOwner(Human newOwner, bool updateName = true)
	{
		this.belongsTo = newOwner;
		if (updateName)
		{
			this.UpdateName(false, Evidence.DataKey.name);
		}
		if (this.lockInteractable != null && this.lockInteractable != this && this.passwordSource == null)
		{
			this.lockInteractable.SetOwner(this.belongsTo, true);
			if (this.preset.passwordSource == RoomConfiguration.RoomPasswordPreference.interactableBelongsTo && this.belongsTo != null)
			{
				this.SetPasswordSource(this.belongsTo);
				this.lockInteractable.SetPasswordSource(this.belongsTo);
			}
			else if (this.preset.passwordSource == RoomConfiguration.RoomPasswordPreference.thisRoom && this.node != null)
			{
				this.SetPasswordSource(this.node.room);
				this.lockInteractable.SetPasswordSource(this.node.room);
			}
			else if (this.preset.passwordSource == RoomConfiguration.RoomPasswordPreference.thisAddress && this.node != null)
			{
				this.SetPasswordSource(this.node.room.gameLocation);
				this.lockInteractable.SetPasswordSource(this.node.room.gameLocation);
			}
		}
		if (newOwner != null)
		{
			this.b = newOwner.humanID;
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.sleepPosition)
			{
				newOwner.SetBed(this);
			}
			if (newOwner.job != null && newOwner.job.preset.ownsWorkPosition && newOwner.job.preset.jobPostion != InteractablePreset.SpecialCase.none && newOwner.job.preset.jobPostion == this.preset.specialCaseFlag)
			{
				newOwner.SetWorkFurniture(this);
			}
		}
		else
		{
			this.b = -1;
		}
		if (Game.Instance.collectDebugData && this.controller != null)
		{
			this.controller.debugOwnedBy = this.belongsTo;
			this.controller.debugWrittenBy = this.writer;
			this.controller.debugReceivedBy = this.reciever;
			if (this.furnitureParent != null)
			{
				this.controller.debugFurnitureOwnedBy = this.furnitureParent.debugOwners;
			}
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x00155090 File Offset: 0x00153290
	public void SetWriter(Human newWriter)
	{
		this.writer = newWriter;
		if (this.writer != null)
		{
			this.w = this.writer.humanID;
			return;
		}
		this.w = -1;
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x001550C0 File Offset: 0x001532C0
	public void SetReciever(Human newReciever)
	{
		this.reciever = newReciever;
		if (this.reciever != null)
		{
			this.r = this.reciever.humanID;
			return;
		}
		this.r = -1;
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x001550F0 File Offset: 0x001532F0
	public string GetName()
	{
		string text = string.Empty;
		if (this.nEvKey > -1 && this.evidence != null)
		{
			text = this.evidence.GetNameForDataKey((Evidence.DataKey)this.nEvKey);
		}
		else if (this.preset.autoName)
		{
			if (this.preset.includeBelongsTo)
			{
				if (this.belongsTo != null)
				{
					if (this.preset.useApartmentName && this.belongsTo.home != null)
					{
						text = this.belongsTo.home.name + " ";
					}
					else if (this.preset.useNameShorthand)
					{
						text = this.belongsTo.GetInitialledName() + " ";
					}
					else
					{
						text = this.belongsTo.GetCitizenName() + " ";
					}
				}
				else if (this.preset.useApartmentName && this.objectRef != null)
				{
					NewAddress newAddress = this.objectRef as NewAddress;
					if (newAddress != null)
					{
						text = newAddress.name + " ";
					}
				}
			}
			if (this.preset.isMoney)
			{
				text = CityControls.Instance.cityCurrency + this.val.ToString();
			}
			else if (this.book != null)
			{
				DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
				if (Toolbox.Instance.allDDSMessages.TryGetValue(this.book.ddsMessage, ref ddsmessageSave))
				{
					DDSSaveClasses.DDSBlockCondition ddsblockCondition = ddsmessageSave.blocks.Find((DDSSaveClasses.DDSBlockCondition item) => item.alwaysDisplay);
					if (ddsblockCondition != null)
					{
						text = Strings.Get("dds.blocks", ddsblockCondition.blockID, Strings.Casing.asIs, false, true, false, null);
					}
				}
			}
			else
			{
				text += Strings.Get("evidence.names", this.preset.presetName, Strings.Casing.asIs, false, false, false, null);
			}
		}
		else
		{
			NewDoor newDoor = this.objectRef as NewDoor;
			if (newDoor != null)
			{
				NewRoom room = newDoor.wall.node.room;
				if (room == Player.Instance.currentRoom)
				{
					room = newDoor.wall.otherWall.node.room;
				}
				if (room.gameLocation != Player.Instance.currentGameLocation)
				{
					text = room.gameLocation.evidenceEntry.GetNameForDataKey(Evidence.DataKey.location);
				}
				else
				{
					text = room.GetName();
				}
			}
		}
		return text;
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x0015536C File Offset: 0x0015356C
	public void UpdateName(bool assignNewNamingEvidenceKey = false, Evidence.DataKey newKey = Evidence.DataKey.name)
	{
		if (assignNewNamingEvidenceKey)
		{
			this.nEvKey = (int)newKey;
		}
		this.name = this.GetName();
		if (this.controller != null)
		{
			this.controller.name = this.name;
			this.controller.gameObject.name = this.name;
			if (InteractionController.Instance.currentLookingAtInteractable == this.controller)
			{
				InteractionController.Instance.UpdateInteractionText();
			}
		}
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x001553E8 File Offset: 0x001535E8
	public void SetInInventory(Human newActor)
	{
		if (this.node != null)
		{
			this.node.RemoveInteractable(this);
			this.node.room.tamperedInteractables.Remove(this);
		}
		this.node = null;
		if (Game.Instance.collectDebugData)
		{
			if (newActor == Player.Instance)
			{
				Game.Log("Player: Set in inventory: " + this.name, 2);
			}
			else if (newActor != null)
			{
				newActor.SelectedDebug("Set in inventory: " + this.name, Actor.HumanDebug.actions);
			}
		}
		if (newActor != this.inInventory && this.inInventory != null)
		{
			this.AddNewDynamicFingerprint(this.inInventory, Interactable.PrintLife.timed);
			this.AddNewDynamicFingerprint(this.inInventory, Interactable.PrintLife.timed);
			this.inInventory.inventory.Remove(this);
			if (this.inInventory.ai != null && this.inInventory.ai.currentWeapon == this)
			{
				this.inInventory.ai.UpdateCurrentWeapon();
			}
		}
		if (newActor != null)
		{
			newActor.inventory.Add(this);
			this.SetOriginalPosition(false, true);
			if (newActor.ai != null)
			{
				newActor.ai.UpdateCurrentWeapon();
			}
		}
		this.inInventory = newActor;
		if (this.inInventory != null)
		{
			this.ConvertToWorldObject(false);
			this.inv = this.inInventory.humanID;
			this.RemoveFromPlacement();
			if (SessionData.Instance.startedGame)
			{
				this.AddNewDynamicFingerprint(this.inInventory, Interactable.PrintLife.timed);
				this.AddNewDynamicFingerprint(this.inInventory, Interactable.PrintLife.timed);
			}
			if (this.inInventory.ai != null && this.preset.inventoryCarryItem)
			{
				this.inInventory.ai.UpdateHeldItems(AIActionPreset.ActionStateFlag.none);
			}
		}
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.tracker)
		{
			List<Interactable> list = CityData.Instance.interactableDirectory.FindAll((Interactable item) => item.preset.specialCaseFlag == InteractablePreset.SpecialCase.tracker);
			list.Sort((Interactable p1, Interactable p2) => p1.id.CompareTo(p2.id));
			int num = list.IndexOf(this);
			num = Mathf.Clamp(num % PrefabControls.Instance.motionTrackerColors.Count, 0, PrefabControls.Instance.motionTrackerColors.Count - 1);
			Color colour = PrefabControls.Instance.motionTrackerColors[num];
			MapController.Instance.AddNewTrackedObject(this.inInventory.transform, PrefabControls.Instance.mapCharacterMarker, new Vector2(22f, 22f), colour, true, this.inInventory.evidenceEntry);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Tracking citizen...", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.location, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x001556D0 File Offset: 0x001538D0
	public void SetAsNotInventory(NewNode newNode)
	{
		Human human = null;
		if (this.inInventory != null)
		{
			human = this.inInventory;
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.tracker)
			{
				MapController.Instance.RemoveTrackedObject(this.inInventory.transform);
			}
			if (this.inInventory.isPlayer)
			{
				for (int i = 0; i < FirstPersonItemController.Instance.slots.Count; i++)
				{
					FirstPersonItemController.InventorySlot inventorySlot = FirstPersonItemController.Instance.slots[i];
					if (inventorySlot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic && inventorySlot.GetInteractable() == this)
					{
						FirstPersonItemController.Instance.EmptySlot(inventorySlot, false, false, false, false);
					}
				}
			}
		}
		if (this.node != null)
		{
			this.node.RemoveInteractable(this);
		}
		this.node = newNode;
		this.SetInInventory(null);
		if (newNode != null)
		{
			newNode.AddInteractable(this);
		}
		this.inInventory = null;
		this.inv = 0;
		this.rPl = false;
		if (human != null && human.ai != null)
		{
			human.ai.UpdateCurrentWeapon();
		}
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x001557D8 File Offset: 0x001539D8
	public void UpdateCurrentActions()
	{
		if (InteractionController.Instance.lockedInInteraction != this && (InteractionController.Instance.currentLookingAtInteractable == null || InteractionController.Instance.currentLookingAtInteractable.interactable != this))
		{
			return;
		}
		List<InteractablePreset.InteractionAction> list = null;
		if (InteractionController.Instance.lockedInInteraction == this)
		{
			list = this.preset.GetActions(InteractionController.Instance.lockedInInteractionRef + 1);
		}
		else
		{
			list = this.preset.GetActions(0);
		}
		list.Sort((InteractablePreset.InteractionAction p1, InteractablePreset.InteractionAction p2) => p2.action.inputPriority.CompareTo(p1.action.inputPriority));
		foreach (InteractablePreset.InteractionKey interactionKey in InteractionController.Instance.allInteractionKeys)
		{
			if (!this.currentActions.ContainsKey(interactionKey))
			{
				this.currentActions.Add(interactionKey, new Interactable.InteractableCurrentAction());
			}
			this.currentActions[interactionKey].enabled = false;
			this.currentActions[interactionKey].display = false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			InteractablePreset.InteractionAction interactionAction = list[i];
			if (interactionAction.GetInteractionKey() != InteractablePreset.InteractionKey.none)
			{
				Interactable.InteractableCurrentAction interactableCurrentAction = this.currentActions[interactionAction.GetInteractionKey()];
				if (interactableCurrentAction != null && !interactableCurrentAction.enabled)
				{
					if (interactionAction.useStrikethrough)
					{
						interactableCurrentAction.display = true;
					}
					if (this.disabledActions.Contains(interactionAction))
					{
						if (interactionAction.action.debug)
						{
							Game.Log("Debug: Action " + interactionAction.action.name + " In disabled actions (interactable)", 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else if (this.isActor != null && this.isActor.ai != null && (this.isActor.ai.ko || this.isActor.isAsleep || this.isActor.isStunned) && interactableCurrentAction.currentAction != null && interactableCurrentAction.currentAction.interactionName == "Talk To")
					{
						if (interactionAction.action.debug)
						{
							Game.Log(string.Concat(new string[]
							{
								"Debug: Action ",
								interactionAction.action.name,
								" disabled because ",
								this.isActor.ai.ko.ToString(),
								" ",
								this.isActor.ai.restrained.ToString(),
								" ",
								this.isActor.isAsleep.ToString(),
								" ",
								this.isActor.isStunned.ToString()
							}), 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else if (Player.Instance.disabledActions.Contains(interactionAction.interactionName.ToLower()))
					{
						if (interactionAction.action.debug)
						{
							Game.Log("Debug: Action " + interactionAction.action.name + " In disabled actions (player)", 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else if (!interactionAction.availableWhileLockedIn && InteractionController.Instance.lockedInInteraction != null)
					{
						if (interactionAction.action.debug)
						{
							Game.Log("Debug: Action " + interactionAction.action.name + " Not available while locked in", 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else if (!interactionAction.availableWhileJumping && !Player.Instance.fps.m_CharacterController.isGrounded)
					{
						if (interactionAction.action.debug)
						{
							Game.Log("Debug: Action " + interactionAction.action.name + " Not available while jumping", 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else if (InteractionController.Instance.currentlyDragging != null)
					{
						if (interactionAction.action.debug)
						{
							Game.Log("Debug: Action " + interactionAction.action.name + " Not available: Player dragging body", 2);
						}
						interactableCurrentAction.enabled = false;
						interactableCurrentAction.currentAction = interactionAction;
					}
					else
					{
						if (!interactionAction.availableWhileIllegal)
						{
							if (Player.Instance.illegalStatus)
							{
								if (!interactionAction.onlyAvailableToRestrainedWhileIllegal || this.isActor == null || this.isActor.ai == null || !this.isActor.ai.restrained)
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " Not available while illegal (player illegal state)", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (!interactionAction.availableWhileWitnessesToIllegal && Player.Instance.witnessesToIllegalActivity.Count > 0)
							{
								bool flag = true;
								foreach (Actor actor in Player.Instance.witnessesToIllegalActivity)
								{
									if (!actor.isAsleep && !actor.isDead && !actor.isStunned && !actor.isMachine && (!(actor.ai != null) || !actor.ai.restrained))
									{
										flag = false;
									}
								}
								if (!flag)
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " Not available while illegal (witness to illegal activity)", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
						}
						if (interactionAction.actionCost > 0 && GameplayController.Instance.money < interactionAction.actionCost)
						{
							interactableCurrentAction.enabled = false;
							interactableCurrentAction.currentAction = interactionAction;
						}
						else
						{
							if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyAvailableInFastForward)
							{
								if (!Player.Instance.spendingTimeMode)
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " Not available: Time isn't in fast forward", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase != InteractablePreset.InteractionAction.SpecialCase.availableInFastForward && Player.Instance.spendingTimeMode)
							{
								if (interactionAction.action.debug)
								{
									Game.Log("Debug: Action " + interactionAction.action.name + " Not available: Time is in fast forward", 2);
								}
								interactableCurrentAction.enabled = false;
								interactableCurrentAction.currentAction = interactionAction;
								goto IL_1390;
							}
							if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyIfDeadAsleepOrUncon)
							{
								if (this.isActor != null && !this.isActor.isAsleep && !this.isActor.isDead && !this.isActor.isStunned)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyInNormalTimeAndAwakeNonDialog)
							{
								if (Player.Instance.spendingTimeMode || Player.Instance.playerKOInProgress || InteractionController.Instance.dialogMode)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.nonDialog)
							{
								if (InteractionController.Instance.dialogMode)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.search)
							{
								if (this.isActor != null)
								{
									if (this.isActor.ai != null && this.isActor.escalationLevel > 0 && !this.isActor.ai.restrained)
									{
										if (interactionAction.action.debug)
										{
											Game.Log(string.Concat(new string[]
											{
												"Debug: Action ",
												interactionAction.action.name,
												" invalid: ",
												this.isActor.name,
												" has escalation level ",
												this.isActor.escalationLevel.ToString()
											}), 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = interactionAction;
										goto IL_1390;
									}
									if (this.isActor.isMoving)
									{
										if (interactionAction.action.debug)
										{
											Game.Log(string.Concat(new string[]
											{
												"Debug: Action ",
												interactionAction.action.name,
												" invalid: ",
												this.isActor.name,
												" is moving"
											}), 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = interactionAction;
										goto IL_1390;
									}
									if (!Player.Instance.stealthMode && !this.isActor.isAsleep && !this.isActor.isDead && !this.isActor.isStunned && (this.isActor.ai == null || !this.isActor.ai.restrained))
									{
										if (interactionAction.action.debug)
										{
											Game.Log(string.Concat(new string[]
											{
												"Debug: Action ",
												interactionAction.action.name,
												" invalid: Stealth: ",
												Player.Instance.stealthMode.ToString(),
												" asleep: ",
												this.isActor.isAsleep.ToString(),
												" dead: ",
												this.isActor.isDead.ToString(),
												" ko: ",
												this.isActor.isStunned.ToString()
											}), 2);
										}
										interactableCurrentAction.enabled = false;
										interactableCurrentAction.currentAction = interactionAction;
										goto IL_1390;
									}
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.nonCombat)
							{
								if (this.isActor != null && this.isActor.ai != null && this.isActor.escalationLevel > 0)
								{
									if (interactionAction.action.debug)
									{
										Game.Log(string.Concat(new string[]
										{
											"Debug: Action ",
											interactionAction.action.name,
											" invalid: ",
											this.isActor.name,
											" has escalation level ",
											this.isActor.escalationLevel.ToString()
										}), 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.nonCombatOrRestrained)
							{
								if (this.isActor != null && this.isActor.ai != null && ((this.isActor.escalationLevel > 0 && !this.isActor.ai.restrained) || (this.isActor.ai.inFleeState && !this.isActor.ai.restrained)))
								{
									if (interactionAction.action.debug)
									{
										Game.Log(string.Concat(new string[]
										{
											"Debug: Action ",
											interactionAction.action.name,
											" invalid: ",
											this.isActor.name,
											" has escalation level ",
											this.isActor.escalationLevel.ToString()
										}), 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.knockOnDoor)
							{
								NewDoor newDoor = this.objectRef as NewDoor;
								if (!(newDoor != null))
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								NewAddress newAddress = null;
								if (newDoor.wall.otherWall.node.gameLocation.thisAsAddress != null)
								{
									if (Player.Instance.currentRoom == newDoor.wall.node.room)
									{
										newAddress = newDoor.wall.otherWall.node.gameLocation.thisAsAddress;
									}
									else
									{
										newAddress = newDoor.wall.node.gameLocation.thisAsAddress;
									}
								}
								if (!(newAddress != null))
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								if (newAddress.residence == null && newAddress.company == null)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.originalPlace)
							{
								if (!this.originalPosition && this.spR)
								{
									if (interactionAction.action.debug)
									{
										string[] array = new string[8];
										array[0] = "Debug: Action ";
										array[1] = interactionAction.action.name;
										array[2] = " invalid: Object original position is false! ";
										array[3] = this.distanceFromSpawn.ToString();
										array[4] = ": ";
										int num = 5;
										Vector3 vector = this.spWPos;
										array[num] = vector.ToString();
										array[6] = " != ";
										int num2 = 7;
										vector = this.wPos;
										array[num2] = vector.ToString();
										Game.Log(string.Concat(array), 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyIfRestrained)
							{
								if (!(this.isActor != null) || !(this.isActor.ai != null) || !this.isActor.ai.restrained)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyIfNotRestrained)
							{
								if (!(this.isActor != null) || !(this.isActor.ai != null) || this.isActor.ai.restrained || this.isActor.isDead)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.ifInventoryItemDrawn)
							{
								if (BioScreenController.Instance.selectedSlot == null || BioScreenController.Instance.selectedSlot.GetInteractable() == null || !BioScreenController.Instance.selectedSlot.GetInteractable().preset.isInventoryItem)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.caseFormsNeeded)
							{
								if (!CasePanelController.Instance.activeCases.Exists((Case item) => item.caseStatus == Case.CaseStatus.handInNotCollected))
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.activeCaseHandInReady)
							{
								if (CasePanelController.Instance.activeCase == null || CasePanelController.Instance.activeCase.caseStatus != Case.CaseStatus.handInCollected || !CasePanelController.Instance.activeCase.handInValid || (!CasePanelController.Instance.activeCase.handIn.Contains(this.id) && (!(this.objectRef as NewDoor != null) || !CasePanelController.Instance.activeCase.handIn.Contains(-(this.objectRef as NewDoor).wall.id))))
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyIfSick)
							{
								if (Player.Instance.sick <= 0f)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.onlyIfMultiPageHasPages)
							{
								if (this.evidence == null)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								EvidenceMultiPage evidenceMultiPage = this.evidence as EvidenceMultiPage;
								if (evidenceMultiPage == null)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								if (evidenceMultiPage.pageContent.Count <= 0)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.furniturePlacement)
							{
								if (!PlayerApartmentController.Instance.furniturePlacementMode || PlayerApartmentController.Instance.furnPlacement == null)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							else if (interactionAction.specialCase == InteractablePreset.InteractionAction.SpecialCase.decorItemPlacement)
							{
								if (!(InteractionController.Instance.carryingObject != null))
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								if (!InteractionController.Instance.carryingObject.apartmentPlacementIsValid)
								{
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							if (interactionAction.action.unavailableWhenItemSelected && BioScreenController.Instance.selectedSlot != null && (BioScreenController.Instance.selectedSlot.interactableID > -1 || BioScreenController.Instance.selectedSlot.isStatic != FirstPersonItemController.InventorySlot.StaticSlot.holster))
							{
								if (interactionAction.action.unavailableWhenItemsSelected == null || interactionAction.action.unavailableWhenItemsSelected.Count <= 0)
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " Unavailable while items drawn", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
								if (interactionAction.action.unavailableWhenItemsSelected.Contains(BioScreenController.Instance.selectedSlot.GetFirstPersonItem()))
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " Unavailable while items drawn", 2);
									}
									interactableCurrentAction.enabled = false;
									interactableCurrentAction.currentAction = interactionAction;
									goto IL_1390;
								}
							}
							if (interactionAction.action.onlyAvailableWhenItemSelected && BioScreenController.Instance.selectedSlot != null && !interactionAction.action.availableWhenItemsSelected.Contains(BioScreenController.Instance.selectedSlot.GetFirstPersonItem()))
							{
								if (interactionAction.action.debug)
								{
									Game.Log("Debug: Action " + interactionAction.action.name + " Unavailable because item isn't drawn", 2);
								}
								interactableCurrentAction.enabled = false;
								interactableCurrentAction.currentAction = interactionAction;
							}
							else
							{
								bool flag2 = true;
								foreach (InteractablePreset.IfSwitchState ifSwitchState in interactionAction.onlyActiveIf)
								{
									if (this.GetSwitchQuery(ifSwitchState.switchState) != ifSwitchState.boolIs)
									{
										flag2 = false;
										break;
									}
								}
								if (!flag2)
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " invalid: Unavailable switches", 2);
									}
									interactableCurrentAction.enabled = false;
									if (interactableCurrentAction.display)
									{
										interactableCurrentAction.currentAction = interactionAction;
									}
								}
								else
								{
									if (interactionAction.action.debug)
									{
										Game.Log("Debug: Action " + interactionAction.action.name + " is valid", 2);
									}
									interactableCurrentAction.display = true;
									interactableCurrentAction.enabled = true;
									interactableCurrentAction.currentAction = interactionAction;
								}
							}
						}
					}
				}
			}
			IL_1390:;
		}
		InteractionController.Instance.OnPlayerLookAtChange();
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x00156BB8 File Offset: 0x00154DB8
	public virtual void SetSwitchState(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false, bool forceInstantLights = false)
	{
		if (this.sw0 != val || forceUpdate)
		{
			this.sw0 = val;
			if (this.controller != null)
			{
				this.controller.debugSwitchState = this.sw0;
				this.controller.UpdateSwitchSync();
			}
			if (this.lightController != null && this.preset.lightswitch == InteractablePreset.Switch.switchState)
			{
				this.lightController.SetOn(this.sw0, forceInstantLights);
			}
			this.UpdateCurrentActions();
			if (this.preset.resetSwitchStates && this.sw0 != this.preset.startingSwitchState)
			{
				if (!GameplayController.Instance.switchRessetingObjects.ContainsKey(this))
				{
					GameplayController.Instance.switchRessetingObjects.Add(this, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.switchRessetingObjects[this] = SessionData.Instance.gameTime;
				}
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					goto IL_186;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					doorNodes = newDoor.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.switchState)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.sw0, doorNodes, interactor);
				}
			}
			IL_186:
			if (this.controller != null)
			{
				if (this.controller.doorMovement != null)
				{
					if (this.sw0 && this.controller.doorMovement.desiredTransition != 1f)
					{
						this.controller.doorMovement.SetOpen(1f, interactor, false);
					}
					else if (!this.sw0 && this.controller.doorMovement.desiredTransition != 0f)
					{
						this.controller.doorMovement.SetOpen(0f, interactor, false);
					}
					if (this.controller.secondaryDoorMovement != null)
					{
						if (this.sw0 && this.controller.secondaryDoorMovement.desiredTransition != 1f)
						{
							this.controller.secondaryDoorMovement.SetOpen(1f, interactor, false);
						}
						else if (!this.sw0 && this.controller.secondaryDoorMovement.desiredTransition != 0f)
						{
							this.controller.secondaryDoorMovement.SetOpen(0f, interactor, false);
						}
					}
					if (this.controller.thirdDoorMovement != null)
					{
						if (this.sw0 && this.controller.thirdDoorMovement.desiredTransition != 1f)
						{
							this.controller.thirdDoorMovement.SetOpen(1f, interactor, false);
						}
						else if (!this.sw0 && this.controller.thirdDoorMovement.desiredTransition != 0f)
						{
							this.controller.thirdDoorMovement.SetOpen(0f, interactor, false);
						}
					}
				}
				if (this.controller.securitySystem != null)
				{
					if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityCamera)
					{
						this.controller.securitySystem.SetActive(this.sw0, true);
					}
					else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun)
					{
						if (this.sw0 && this.sw1)
						{
							this.controller.securitySystem.SetActive(true, true);
						}
						else
						{
							this.controller.securitySystem.SetActive(false, true);
						}
					}
				}
				if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.lightswitch && this.controller.meshes.Count > 1)
				{
					if (this.sw0)
					{
						this.controller.meshes[0].sharedMaterial = InteriorControls.Instance.newLightswitchMaterial;
						this.controller.meshes[1].sharedMaterial = InteriorControls.Instance.newLightswithSwitchMaterial;
					}
					else
					{
						this.controller.meshes[0].sharedMaterial = InteriorControls.Instance.pulsingLightswitch;
						this.controller.meshes[1].sharedMaterial = InteriorControls.Instance.pulsingLightswitchSwitch;
					}
				}
			}
			if (this.preset.affectRoomSteamLevel && this.node != null)
			{
				this.node.room.SetSteam(this.sw0);
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerLights)
			{
				if (this.node == null || !(this.node.gameLocation.floor != null))
				{
					goto IL_7C4;
				}
				using (List<NewAddress>.Enumerator enumerator2 = this.node.gameLocation.floor.addresses.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NewAddress newAddress = enumerator2.Current;
						if (newAddress.GetBreakerLights() == this)
						{
							foreach (NewRoom newRoom in newAddress.rooms)
							{
								foreach (Interactable interactable in newRoom.mainLights)
								{
									interactable.SetCustomState2(!this.sw0, null, true, false, false);
								}
							}
						}
					}
					goto IL_7C4;
				}
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerSecurity)
			{
				if (this.node == null || !(this.node.gameLocation.floor != null))
				{
					goto IL_7C4;
				}
				using (List<NewAddress>.Enumerator enumerator2 = this.node.gameLocation.floor.addresses.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NewAddress newAddress2 = enumerator2.Current;
						if (newAddress2.GetBreakerSecurity() == this)
						{
							foreach (Interactable interactable2 in newAddress2.securityCameras)
							{
								interactable2.SetSwitchState(this.sw0, null, true, false, false);
							}
							foreach (Interactable interactable3 in newAddress2.sentryGuns)
							{
								interactable3.SetSwitchState(this.sw0, null, true, false, false);
							}
							foreach (Interactable interactable4 in newAddress2.otherSecurity)
							{
								interactable4.SetSwitchState(this.sw0, null, true, false, false);
							}
						}
					}
					goto IL_7C4;
				}
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.breakerDoors)
			{
				if (this.node != null && this.node.gameLocation.floor != null)
				{
					bool flag = false;
					foreach (NewAddress newAddress3 in this.node.gameLocation.floor.addresses)
					{
						if (newAddress3.breakerDoors == this)
						{
							this.node.gameLocation.floor.SetAlarmLockdown(!this.sw0, newAddress3);
							flag = true;
						}
					}
					if (!flag)
					{
						this.node.gameLocation.floor.SetAlarmLockdown(!this.sw0, null);
					}
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.stovetopKettle && this.sw0 && !GameplayController.Instance.activeKettles.Contains(this))
			{
				GameplayController.Instance.activeKettles.Add(this);
			}
			IL_7C4:
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.securityCamera || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.otherSecuritySystem || this.preset.specialCaseFlag == InteractablePreset.SpecialCase.gasReleaseSystem)
			{
				if (!this.sw0 && !GameplayController.Instance.turnedOffSecurity.Contains(this))
				{
					GameplayController.Instance.turnedOffSecurity.Add(this);
				}
				else if (this.sw0)
				{
					GameplayController.Instance.turnedOffSecurity.Remove(this);
				}
			}
			this.UpdateLoopingAudioParams();
			if (this.OnSwitchChange != null)
			{
				this.OnSwitchChange();
			}
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x00157494 File Offset: 0x00155694
	public virtual void SetCustomState1(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false, bool forceInstantLights = false)
	{
		if (this.sw1 != val || forceUpdate)
		{
			this.sw1 = val;
			if (this.controller != null)
			{
				this.controller.debugState1 = this.sw1;
				this.controller.UpdateSwitchSync();
			}
			if (this.lightController != null)
			{
				if (this.preset.lightswitch == InteractablePreset.Switch.custom1)
				{
					this.lightController.SetOn(this.sw1, forceInstantLights);
				}
				this.lightController.SetUnscrewed(this.sw1, forceInstantLights);
			}
			this.UpdateCurrentActions();
			if (this.preset.resetSwitchStates && this.sw1 != this.preset.startingCustomState1)
			{
				if (!GameplayController.Instance.switchRessetingObjects.ContainsKey(this))
				{
					GameplayController.Instance.switchRessetingObjects.Add(this, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.switchRessetingObjects[this] = SessionData.Instance.gameTime;
				}
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					goto IL_19B;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					doorNodes = newDoor.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.custom1)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.sw1, doorNodes, interactor);
				}
			}
			IL_19B:
			if (this.controller != null && this.controller.securitySystem != null && this.preset.specialCaseFlag == InteractablePreset.SpecialCase.sentryGun)
			{
				if (this.sw0 && this.sw1)
				{
					this.controller.securitySystem.SetActive(true, true);
				}
				else
				{
					this.controller.securitySystem.SetActive(false, true);
				}
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.tracker)
			{
				if (this.sw1)
				{
					GameplayController.Instance.AddMotionTracker(this, 3);
				}
				else
				{
					GameplayController.Instance.RemoveMotionTracker(this);
				}
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.grenade)
			{
				if (this.sw1)
				{
					GameplayController.Instance.AddProxyDetonator(this, 3f);
				}
				else
				{
					GameplayController.Instance.RemoveProxyDetonator(this);
				}
			}
			if (this.OnState1Change != null)
			{
				this.OnState1Change();
			}
		}
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x0015772C File Offset: 0x0015592C
	public virtual void SetCustomState2(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false, bool forceInstantLights = false)
	{
		if (this.sw2 != val || forceUpdate)
		{
			this.sw2 = val;
			this.UpdateCurrentActions();
			if (this.preset.resetSwitchStates && this.sw2 != this.preset.startingCustomState2)
			{
				if (!GameplayController.Instance.switchRessetingObjects.ContainsKey(this))
				{
					GameplayController.Instance.switchRessetingObjects.Add(this, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.switchRessetingObjects[this] = SessionData.Instance.gameTime;
				}
			}
			if (this.controller != null)
			{
				this.controller.UpdateSwitchSync();
			}
			if (this.lightController != null)
			{
				if (this.preset.lightswitch == InteractablePreset.Switch.custom2)
				{
					this.lightController.SetOn(this.sw2, forceInstantLights);
				}
				this.lightController.SetClosedBreaker(this.sw2, false);
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.grenade)
			{
				if (this.sw2)
				{
					if (!GameplayController.Instance.activeGrenades.Contains(this))
					{
						GameplayController.Instance.activeGrenades.Add(this);
					}
				}
				else
				{
					GameplayController.Instance.activeGrenades.Remove(this);
					this.SetValue(GameplayControls.Instance.proxyGrenadeFuse);
					this.recentCallCheck = 0f;
				}
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					return;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					doorNodes = newDoor.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.custom2)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.sw2, doorNodes, interactor);
				}
			}
		}
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0015793C File Offset: 0x00155B3C
	public virtual void SetCustomState3(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false, bool forceInstantLights = false)
	{
		if (this.sw3 != val || forceUpdate)
		{
			this.sw3 = val;
			this.UpdateCurrentActions();
			if (this.preset.resetSwitchStates && this.sw3 != this.preset.startingCustomState3)
			{
				if (!GameplayController.Instance.switchRessetingObjects.ContainsKey(this))
				{
					GameplayController.Instance.switchRessetingObjects.Add(this, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.switchRessetingObjects[this] = SessionData.Instance.gameTime;
				}
			}
			if (this.controller != null)
			{
				this.controller.UpdateSwitchSync();
			}
			if (this.lightController != null && this.preset.lightswitch == InteractablePreset.Switch.custom3)
			{
				this.lightController.SetOn(this.sw3, forceInstantLights);
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					return;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					doorNodes = newDoor.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.custom3)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.sw3, doorNodes, interactor);
				}
			}
		}
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x00157AD0 File Offset: 0x00155CD0
	public virtual void SetLockedState(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false)
	{
		if (this.locked != val || forceUpdate)
		{
			this.locked = val;
			if (this.preset.resetSwitchStates && this.locked != this.preset.startingLockState)
			{
				if (!GameplayController.Instance.switchRessetingObjects.ContainsKey(this))
				{
					GameplayController.Instance.switchRessetingObjects.Add(this, SessionData.Instance.gameTime);
				}
				else
				{
					GameplayController.Instance.switchRessetingObjects[this] = SessionData.Instance.gameTime;
				}
			}
			if (this.controller != null)
			{
				this.controller.UpdateSwitchSync();
			}
			this.UpdateCurrentActions();
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					newDoor.SetLocked(this.locked, interactor, true);
				}
			}
			if (this.lockInteractable != null && this.lockInteractable != this)
			{
				this.lockInteractable.SetLockedState(this.locked, interactor, true, false);
			}
			if (this.preset.useMaterialChanges && this.spawnedObject != null)
			{
				if (this.locked && this.preset.lockOnMaterial != null)
				{
					this.spawnedObject.GetComponent<MeshRenderer>().material = this.preset.lockOnMaterial;
				}
				else if (!this.locked && this.preset.lockOffMaterial != null)
				{
					this.spawnedObject.GetComponent<MeshRenderer>().material = this.preset.lockOffMaterial;
				}
			}
			if (this.thisDoor != null)
			{
				this.thisDoor.SetLockedState(this.locked, interactor, true, false);
				if (this.controller != null && this.locked && this.controller.flash != null)
				{
					this.controller.flash.Flash(3);
				}
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					return;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor2 = this.objectRef as NewDoor;
				if (newDoor2 != null)
				{
					doorNodes = newDoor2.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.lockState)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.locked, doorNodes, interactor);
				}
			}
		}
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x00157D68 File Offset: 0x00155F68
	public virtual void SetPhysicsPickupState(bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false)
	{
		if (this.phy != val || forceUpdate)
		{
			this.phy = val;
			this.UpdateCurrentActions();
			if (this.controller != null)
			{
				this.controller.UpdateSwitchSync();
			}
			if (!playSFX)
			{
				if (!this.preset.switchSFX.Exists((InteractablePreset.IfSwitchStateSFX item) => item.isLoop))
				{
					return;
				}
			}
			List<NewNode> doorNodes = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null)
				{
					doorNodes = newDoor.bothNodesForAudioSource;
				}
			}
			foreach (InteractablePreset.IfSwitchStateSFX ifSwitchStateSFX in this.preset.switchSFX)
			{
				if (ifSwitchStateSFX.switchState == InteractablePreset.Switch.carryPhysicsObject)
				{
					this.UpdateSwitchStateAudio(ifSwitchStateSFX, this.phy, doorNodes, interactor);
				}
			}
		}
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x00157E68 File Offset: 0x00156068
	public void ResetToDefaultSwitchState()
	{
		this.SetSwitchState(this.preset.startingSwitchState, null, true, false, false);
		this.SetSwitchState(this.preset.startingCustomState1, null, true, false, false);
		this.SetSwitchState(this.preset.startingCustomState2, null, true, false, false);
		this.SetSwitchState(this.preset.startingCustomState3, null, true, false, false);
		this.SetSwitchState(this.preset.startingLockState, null, true, false, false);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00157EE0 File Offset: 0x001560E0
	public void UpdateSwitchStateAudio(InteractablePreset.IfSwitchStateSFX aud, bool swState, List<NewNode> doorNodes, Actor interactor)
	{
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		if (aud.onlyIfInSyncBed)
		{
			flag = (Player.Instance.hideInteractable != null && Player.Instance.hideInteractable.preset.specialCaseFlag == InteractablePreset.SpecialCase.syncBed);
		}
		if (aud.onlyIfNotInSyncBed)
		{
			flag = (Player.Instance.hideInteractable == null || Player.Instance.hideInteractable.preset.specialCaseFlag != InteractablePreset.SpecialCase.syncBed);
		}
		if (aud.onlyIfNeonSign)
		{
			flag2 = false;
			if (this.objectRef != null)
			{
				NewDoor newDoor = this.objectRef as NewDoor;
				if (newDoor != null && newDoor.featuresNeonSign)
				{
					flag2 = true;
				}
			}
		}
		if (this.inInventory != null)
		{
			flag3 = false;
		}
		if (aud.boolIs == swState && flag && flag2 && flag3 && !this.rPl)
		{
			if (!aud.isLoop)
			{
				AudioController.Instance.PlayWorldOneShot(aud.triggerAudio, interactor, this.node, this.wPos, this, null, 1f, doorNodes, false, null, false);
				return;
			}
			if (aud.isMusicPlayer)
			{
				if (!GameplayController.Instance.activeMusicPlayers.Contains(this))
				{
					GameplayController.Instance.activeMusicPlayers.Add(this);
				}
				this.UpdateMusicPlayer();
				return;
			}
			if (!this.loopingAudio.Exists((AudioController.LoopingSoundInfo item) => item.eventPreset == aud.triggerAudio))
			{
				SessionData.TelevisionChannel isBroadcast = null;
				if (aud.isBroadcast)
				{
					isBroadcast = SessionData.Instance.televisionChannels[0];
				}
				AudioController.LoopingSoundInfo loopingSoundInfo = AudioController.Instance.PlayWorldLooping(aud.triggerAudio, interactor, this, null, 1f, false, true, isBroadcast, aud);
				if (loopingSoundInfo != null)
				{
					this.loopingAudio.Add(loopingSoundInfo);
				}
				if (aud.passOpenParam || aud.passCSParam || aud.passDoorDirParam)
				{
					this.UpdateLoopingAudioParams();
					return;
				}
			}
		}
		else if (aud.isLoop && this.loopingAudio != null)
		{
			if (aud.isMusicPlayer)
			{
				foreach (AudioController.LoopingSoundInfo loopingSoundInfo2 in this.loopingAudio.FindAll((AudioController.LoopingSoundInfo item) => this.preset.musicTracks.Contains(item.eventPreset)))
				{
					AudioController.Instance.StopSound(loopingSoundInfo2, aud.stop, "Music player off");
					this.loopingAudio.Remove(loopingSoundInfo2);
				}
				GameplayController.Instance.activeMusicPlayers.Remove(this);
				return;
			}
			AudioController.LoopingSoundInfo loopingSoundInfo3 = this.loopingAudio.Find((AudioController.LoopingSoundInfo item) => item.eventPreset == aud.triggerAudio);
			if (loopingSoundInfo3 != null)
			{
				AudioController.Instance.StopSound(loopingSoundInfo3, aud.stop, "Switch is different to state (" + this.val.ToString() + ")");
				this.loopingAudio.Remove(loopingSoundInfo3);
			}
		}
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x00158218 File Offset: 0x00156418
	public void MusicPlayerNextTrack(int add)
	{
		this.SetValue((float)Mathf.RoundToInt(this.val + (float)add));
		if (this.val >= (float)this.preset.musicTracks.Count)
		{
			this.SetValue(0f);
		}
		if (this.val < 0f)
		{
			this.SetValue((float)(this.preset.musicTracks.Count - 1));
		}
		this.UpdateMusicPlayer();
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0015828C File Offset: 0x0015648C
	public void UpdateMusicPlayer()
	{
		int num = Mathf.RoundToInt(this.val);
		if (num >= this.preset.musicTracks.Count)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = this.preset.musicTracks.Count - 1;
		}
		try
		{
			AudioEvent trackToPlay = this.preset.musicTracks[num];
			if (trackToPlay != null)
			{
				foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.loopingAudio)
				{
					if (loopingSoundInfo.eventPreset == trackToPlay)
					{
						if (loopingSoundInfo.UpdatePlayState() == null)
						{
							continue;
						}
						this.SetValue((float)Mathf.RoundToInt(this.val + 1f));
						if (this.val >= (float)this.preset.musicTracks.Count)
						{
							this.SetValue(0f);
						}
						if (this.val < 0f)
						{
							this.SetValue((float)(this.preset.musicTracks.Count - 1));
						}
						num = Mathf.RoundToInt(this.val);
						try
						{
							trackToPlay = this.preset.musicTracks[num];
							continue;
						}
						catch
						{
							continue;
						}
					}
					if (this.preset.musicTracks.Contains(loopingSoundInfo.eventPreset))
					{
						AudioController.Instance.StopSound(loopingSoundInfo, AudioController.StopType.immediate, "Music player different track");
						this.loopingAudio.Remove(loopingSoundInfo);
					}
				}
				if (!this.loopingAudio.Exists((AudioController.LoopingSoundInfo item) => item.eventPreset == trackToPlay))
				{
					AudioController.LoopingSoundInfo loopingSoundInfo2 = AudioController.Instance.PlayWorldLooping(trackToPlay, null, this, null, 1f, false, true, null, null);
					if (loopingSoundInfo2 != null)
					{
						this.loopingAudio.Add(loopingSoundInfo2);
					}
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x001584B0 File Offset: 0x001566B0
	public void UpdateLoopingAudioParams()
	{
		if (this.loopingAudio == null)
		{
			this.loopingAudio = new List<AudioController.LoopingSoundInfo>();
		}
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.loopingAudio)
		{
			bool flag = false;
			if (loopingSoundInfo.interactableLoopInfo != null)
			{
				if (loopingSoundInfo.interactableLoopInfo.passOpenParam)
				{
					float num = 0f;
					if (this.sw0)
					{
						num = 1f;
					}
					if (this.controller != null && this.controller.doorMovement != null)
					{
						num = this.controller.doorMovement.currentTransition;
					}
					if (this.controller != null && this.controller.secondaryDoorMovement != null)
					{
						num = this.controller.secondaryDoorMovement.currentTransition;
					}
					if (this.controller != null && this.controller.thirdDoorMovement != null)
					{
						num = this.controller.thirdDoorMovement.currentTransition;
					}
					loopingSoundInfo.audioEvent.setParameterByName("Open", num, false);
					PLAYBACK_STATE playback_STATE = 2;
					loopingSoundInfo.audioEvent.getPlaybackState(ref playback_STATE);
					flag = true;
				}
				if (loopingSoundInfo.interactableLoopInfo.passCSParam)
				{
					loopingSoundInfo.audioEvent.setParameterByName("ConsumableState", this.cs, false);
					PLAYBACK_STATE playback_STATE2 = 2;
					loopingSoundInfo.audioEvent.getPlaybackState(ref playback_STATE2);
					flag = true;
				}
				if (loopingSoundInfo.interactableLoopInfo.passDoorDirParam && this.controller != null && this.controller.doorMovement != null && this.controller.doorMovement.isAnimating)
				{
					if (this.controller.doorMovement.isOpening)
					{
						loopingSoundInfo.audioEvent.setParameterByName("DoorDirection", 1f, false);
					}
					else
					{
						loopingSoundInfo.audioEvent.setParameterByName("DoorDirection", 0f, false);
					}
				}
				if (flag)
				{
					loopingSoundInfo.UpdateOcclusion(true);
				}
			}
		}
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x001586D4 File Offset: 0x001568D4
	public void OnInteraction(InteractablePreset.InteractionKey input, Actor who)
	{
		Interactable.InteractableCurrentAction interactableCurrentAction = null;
		if (this.currentActions.TryGetValue(input, ref interactableCurrentAction) && interactableCurrentAction.enabled)
		{
			this.OnInteraction(interactableCurrentAction.currentAction, who, true, 0f);
		}
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x00158710 File Offset: 0x00156910
	public void OnInteraction(InteractablePreset.InteractionAction action, Actor who, bool allowDelays = true, float additionalDelay = 0f)
	{
		if (action == null)
		{
			return;
		}
		if (allowDelays && who != null && who.ai != null && action.aiUsageDelay + additionalDelay > 0f && who.visible && !who.isDead && !who.ai.queuedActions.Exists((NewAIController.QueuedAction item) => item.actionSetting == action && item.interactable == this))
		{
			who.ai.queuedActions.Add(new NewAIController.QueuedAction
			{
				actionSetting = action,
				delay = action.aiUsageDelay + additionalDelay,
				interactable = this
			});
			return;
		}
		if (action.actionCost > 0 && who != null && who.isPlayer)
		{
			GameplayController.Instance.AddMoney(-action.actionCost, true, "Action cost");
		}
		if (who != null && who.isPlayer)
		{
			if (action.actionIsIllegal && InteractionController.Instance.GetValidPlayerActionIllegal(this, Player.Instance.currentNode, true, true))
			{
				InteractionController.Instance.SetIllegalActionActive(true);
			}
			if (action.action.holsterCurrentItemOnAction && BioScreenController.Instance.selectedSlot != null && BioScreenController.Instance.selectedSlot.isStatic != FirstPersonItemController.InventorySlot.StaticSlot.holster)
			{
				BioScreenController.Instance.SelectSlot(FirstPersonItemController.Instance.slots.Find((FirstPersonItemController.InventorySlot item) => item.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.holster), false, false);
			}
		}
		ActionController.Instance.ExecuteAction(action.action, this, this.node, who);
		if (who != null && !who.isPlayer && Toolbox.Instance.Rand(0f, 1f, false) <= 0.75f)
		{
			this.AddNewDynamicFingerprint(who as Human, Interactable.PrintLife.timed);
		}
		if (action.isHidingPlace && who != null && (!action.onlyHidingPlaceIfPublic || !who.isTrespassing))
		{
			who.SetHiding(true, this);
		}
		foreach (InteractablePreset.SwitchState switchState in action.effectSwitchStates)
		{
			if (switchState.switchState == InteractablePreset.Switch.switchState)
			{
				if (this.lockInteractable != null && this.locked && (this.belongsTo == Player.Instance || this.belongsTo != who))
				{
					if (this.node != null && this.preset.attemptedOpenSound != null)
					{
						List<NewNode> list = null;
						if (this.objectRef != null)
						{
							NewDoor newDoor = this.objectRef as NewDoor;
							if (newDoor != null)
							{
								list = newDoor.bothNodesForAudioSource;
							}
						}
						AudioController instance = AudioController.Instance;
						AudioEvent attemptedOpenSound = this.preset.attemptedOpenSound;
						NewNode location = this.node;
						Vector3 worldPosition = this.wPos;
						List<NewNode> additionalSources = list;
						instance.PlayWorldOneShot(attemptedOpenSound, who, location, worldPosition, this, null, 1f, additionalSources, false, null, false);
					}
					InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "Locked", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					Game.Log("Cannot affect switch state of interactable " + this.name + " because it is locked.", 2);
					return;
				}
				this.SetSwitchState(switchState.boolIs, who, true, false, false);
			}
			else if (switchState.switchState == InteractablePreset.Switch.lockState)
			{
				this.SetLockedState(switchState.boolIs, who, true, false);
			}
			else if (switchState.switchState == InteractablePreset.Switch.custom1)
			{
				this.SetCustomState1(switchState.boolIs, who, true, false, false);
			}
			else if (switchState.switchState == InteractablePreset.Switch.custom2)
			{
				this.SetCustomState2(switchState.boolIs, who, true, false, false);
			}
			else if (switchState.switchState == InteractablePreset.Switch.custom3)
			{
				this.SetCustomState3(switchState.boolIs, who, true, false, false);
			}
			else if (switchState.switchState == InteractablePreset.Switch.carryPhysicsObject)
			{
				this.SetPhysicsPickupState(switchState.boolIs, who, true, false);
			}
		}
		if (this.controller != null && action.soundEvent != null && action.playOnTrigger)
		{
			List<NewNode> list2 = null;
			if (this.objectRef != null)
			{
				NewDoor newDoor2 = this.objectRef as NewDoor;
				if (newDoor2 != null)
				{
					list2 = newDoor2.bothNodesForAudioSource;
				}
			}
			NewNode newNode = this.node;
			Vector3 vector = this.wPos;
			if (newNode == null)
			{
				PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(vector), ref newNode);
			}
			if (action.soundEvent.debug)
			{
				string[] array = new string[8];
				array[0] = "Audio: Audio from interactable action ";
				array[1] = action.interactionName;
				array[2] = ": Who= ";
				array[3] = ((who != null) ? who.ToString() : null);
				array[4] = " node= ";
				int num = 5;
				NewNode newNode2 = newNode;
				array[num] = ((newNode2 != null) ? newNode2.ToString() : null);
				array[6] = " worldPos= ";
				int num2 = 7;
				Vector3 vector2 = vector;
				array[num2] = vector2.ToString();
				Game.Log(string.Concat(array), 2);
			}
			AudioController instance2 = AudioController.Instance;
			AudioEvent soundEvent = action.soundEvent;
			NewNode location2 = newNode;
			Vector3 worldPosition2 = vector;
			List<NewNode> additionalSources = list2;
			instance2.PlayWorldOneShot(soundEvent, who, location2, worldPosition2, this, null, 1f, additionalSources, false, null, false);
		}
		if (this.node != null)
		{
			if (action.action.tamperAction)
			{
				if (who != null && who.isTrespassing)
				{
					if (!this.node.room.tamperedInteractables.Contains(this))
					{
						this.node.room.tamperedInteractables.Add(this);
					}
				}
				else
				{
					this.node.room.tamperedInteractables.Remove(this);
				}
			}
			if (action.action.tamperResetAction && this.node.room.tamperedInteractables.Contains(this))
			{
				this.node.room.tamperedInteractables.Remove(this);
			}
		}
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x00158D5C File Offset: 0x00156F5C
	public void LoadInteractableToWorld(bool respawn = false, bool forceSpawnImmediate = false)
	{
		if (this.preset.spawnable)
		{
			if (this.spawnedObject != null)
			{
				if (respawn)
				{
					this.DespawnObject();
				}
				else if (this.inInventory == null && !this.rPl)
				{
					ObjectPoolingController.Instance.MarkAsToLoad(this);
					return;
				}
			}
			if (this.inInventory == null && !this.rPl)
			{
				if (forceSpawnImmediate)
				{
					bool flag;
					this.SpawnObject(out flag);
					return;
				}
				ObjectPoolingController.Instance.MarkAsToLoad(this);
			}
		}
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x00158DDE File Offset: 0x00156FDE
	public void SetSpawnPositionRelevent(bool val)
	{
		this.spR = val;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x00158DE8 File Offset: 0x00156FE8
	public void SpawnObject(out bool wasPooled)
	{
		wasPooled = false;
		if (this.controller != null || this.preset == null)
		{
			return;
		}
		this.spawnParent = null;
		if (this.parentTransform == null && this.worldObjectRoomParent != null)
		{
			this.parentTransform = this.worldObjectRoomParent.transform;
		}
		if (this.furnitureParent != null)
		{
			if (this.furnitureParent.spawnedObject == null)
			{
				string[] array = new string[14];
				array[0] = "Misc Error: Trying to spawn interactable with furniture parent before an interactable spawned upon it: ";
				array[1] = this.preset.name;
				array[2] = " on node ";
				int num = 3;
				Vector3Int nodeCoord = this.node.nodeCoord;
				array[num] = nodeCoord.ToString();
				array[4] = " world: ";
				int num2 = 5;
				Vector3 position = this.wPos;
				array[num2] = position.ToString();
				array[6] = " local: ";
				int num3 = 7;
				position = this.lPos;
				array[num3] = position.ToString();
				array[8] = " on furniture ";
				array[9] = this.furnitureParent.furniture.name;
				array[10] = " on node ";
				int num4 = 11;
				nodeCoord = this.furnitureParent.anchorNode.nodeCoord;
				array[num4] = nodeCoord.ToString();
				array[12] = " world: ";
				int num5 = 13;
				position = this.furnitureParent.anchorNode.position;
				array[num5] = position.ToString();
				Game.Log(string.Concat(array), 2);
				return;
			}
			this.spawnParent = this.furnitureParent.spawnedObject.transform;
			if (this.subObject != null && this.subObject.parent.Length > 0)
			{
				this.spawnParent = Toolbox.Instance.SearchForTransform(this.furnitureParent.spawnedObject.transform, this.subObject.parent, false);
				if (this.spawnParent == null && Game.Instance.devMode && Game.Instance.printDebug)
				{
					Game.LogError(string.Concat(new string[]
					{
						"SubObject not found for ",
						this.preset.name,
						" in furniture ",
						this.furnitureParent.furniture.name,
						" Parent name = ",
						this.subObject.parent
					}), 2);
					foreach (Transform transform in Toolbox.Instance.GetAllTransforms(this.furnitureParent.spawnedObject.transform))
					{
						Game.Log(string.Concat(new string[]
						{
							"Listing children transform for ",
							this.furnitureParent.furniture.name,
							": ",
							transform.name,
							" (children: ",
							transform.childCount.ToString(),
							")"
						}), 2);
					}
				}
			}
		}
		else
		{
			if (this.parentTransform == null && this.worldObjectRoomParent != null)
			{
				this.parentTransform = this.worldObjectRoomParent.transform;
			}
			this.spawnParent = this.parentTransform;
		}
		bool flag = false;
		this.spawnedObject = ObjectPoolingController.Instance.GetInteractableObject(this, out flag, out wasPooled);
		if (this.spawnedObject == null)
		{
			return;
		}
		this.spawnedObject.transform.localEulerAngles = this.lEuler;
		this.spawnedObject.transform.localPosition = this.lPos;
		InteractableController interactableController = new List<InteractableController>(this.spawnedObject.GetComponentsInChildren<InteractableController>()).Find((InteractableController item) => item.id == (InteractableController.InteractableID)this.pt && this.pt != 11);
		if (interactableController != null)
		{
			interactableController.Setup(this);
		}
		else
		{
			string text = "Misc Error: Unable to find a matching interactable controller for ";
			string text2 = this.preset.name;
			string text3 = " id: ";
			InteractableController.InteractableID interactableID = (InteractableController.InteractableID)this.pt;
			Game.Log(text + text2 + text3 + interactableID.ToString(), 2);
		}
		if (!flag)
		{
			if (this.preset.inheritColouringFromDecor)
			{
				if (this.preset.shareColoursWithFurniture != FurniturePreset.ShareColours.none)
				{
					FurnitureLocation furnitureLocation = this.node.room.individualFurniture.Find((FurnitureLocation item) => item.furniture.shareColours == this.preset.shareColoursWithFurniture);
					if (furnitureLocation != null)
					{
						if (this.controller != null)
						{
							using (List<MeshRenderer>.Enumerator enumerator = this.controller.meshes.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MeshRenderer renderer = enumerator.Current;
									MaterialsController.Instance.ApplyMaterialKey(renderer, furnitureLocation.matKey);
								}
								goto IL_5C3;
							}
						}
						MaterialsController.Instance.ApplyMaterialKey(this.spawnedObject, furnitureLocation.matKey);
					}
				}
				else
				{
					if (this.controller != null)
					{
						using (List<MeshRenderer>.Enumerator enumerator = this.controller.meshes.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								MeshRenderer renderer2 = enumerator.Current;
								MaterialsController.Instance.ApplyMaterialKey(renderer2, this.node.room.miscKey);
							}
							goto IL_5C3;
						}
					}
					MaterialsController.Instance.ApplyMaterialKey(this.spawnedObject, this.node.room.miscKey);
				}
			}
			else if (this.preset.useOwnColourSettings && this.mk != null)
			{
				if (this.controller != null)
				{
					using (List<MeshRenderer>.Enumerator enumerator = this.controller.meshes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MeshRenderer renderer3 = enumerator.Current;
							MaterialsController.Instance.ApplyMaterialKey(renderer3, this.mk);
						}
						goto IL_5C3;
					}
				}
				MaterialsController.Instance.ApplyMaterialKey(this.spawnedObject, this.mk);
			}
			IL_5C3:
			if (this.preset.useMaterialChanges && this.controller != null && this.controller.meshes.Count > 0)
			{
				if (this.locked && this.preset.lockOnMaterial != null)
				{
					this.controller.meshes[0].material = this.preset.lockOnMaterial;
				}
				else if (!this.locked && this.preset.lockOffMaterial != null)
				{
					this.controller.meshes[0].material = this.preset.lockOffMaterial;
				}
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.lightswitch && this.controller != null && this.controller.meshes.Count > 1)
			{
				if (this.sw0)
				{
					this.controller.meshes[0].sharedMaterial = InteriorControls.Instance.newLightswitchMaterial;
					this.controller.meshes[1].sharedMaterial = InteriorControls.Instance.newLightswithSwitchMaterial;
				}
				else
				{
					this.controller.meshes[0].sharedMaterial = InteriorControls.Instance.pulsingLightswitch;
					this.controller.meshes[1].sharedMaterial = InteriorControls.Instance.pulsingLightswitchSwitch;
				}
			}
			if (this.book != null)
			{
				MeshFilter component = this.spawnedObject.GetComponent<MeshFilter>();
				if (component.sharedMesh != this.book.bookMesh)
				{
					component.sharedMesh = this.book.bookMesh;
					this.spawnedObject.GetComponent<BoxCollider>().center = component.sharedMesh.bounds.center;
					this.spawnedObject.GetComponent<BoxCollider>().size = component.sharedMesh.bounds.size;
				}
				this.spawnedObject.GetComponent<MeshRenderer>().sharedMaterial = this.book.bookMaterial;
			}
			if (this.node != null)
			{
				bool includeStreetLighting = false;
				if (this.node.room.preset != null)
				{
					includeStreetLighting = this.node.room.preset.forceStreetLightLayer;
				}
				if (this.controller != null)
				{
					using (List<MeshRenderer>.Enumerator enumerator = this.controller.meshes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MeshRenderer meshRend = enumerator.Current;
							Toolbox.Instance.SetLightLayer(meshRend, this.node.building, includeStreetLighting);
						}
						goto IL_899;
					}
				}
				Toolbox.Instance.SetLightLayer(this.spawnedObject, this.node.building, includeStreetLighting);
			}
		}
		IL_899:
		this.loadedGeometry = true;
		ObjectPoolingController.Instance.interactableLoadList.Remove(this);
		this.OnSpawn();
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x001596E0 File Offset: 0x001578E0
	public void UnloadInteractable()
	{
		ObjectPoolingController.Instance.MarkAsNotNeeded(this);
		if (this.preset.spawnable && this.loadedGeometry && ObjectPoolingController.Instance.usePooling)
		{
			ObjectPoolingController.Instance.PoolInteractable(this);
			this.loadedGeometry = false;
		}
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x00159720 File Offset: 0x00157920
	public void DespawnObject()
	{
		if (InteractionController.Instance.carryingObject != null && InteractionController.Instance.carryingObject.interactable == this)
		{
			Game.LogError("Despawning a carried object: " + this.name, 2);
		}
		if (this.preset == null && !Toolbox.Instance.objectPresetDictionary.TryGetValue(this.p, ref this.preset))
		{
			Game.LogError("Interactables unable to find preset for " + this.p, 2);
		}
		if (this.preset.spawnable)
		{
			ObjectPoolingController.Instance.MarkAsNotNeeded(this);
			if (this.spawnedObject != null)
			{
				Toolbox.Instance.DestroyObject(this.spawnedObject);
				ObjectPoolingController.Instance.interactablesLoaded--;
			}
			this.loadedGeometry = false;
		}
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x001597F8 File Offset: 0x001579F8
	public void OnSpawn()
	{
		if (this.isLight != null)
		{
			if (this.node == null)
			{
				this.UpdateWorldPositionAndNode(false);
				if (this.node == null)
				{
					return;
				}
			}
			if (this.controller != null)
			{
				this.lightController = this.controller.lightController;
				if (this.lightController == null)
				{
					this.lightController = this.controller.gameObject.GetComponentInChildren<LightController>();
				}
			}
			if (this.lightController != null)
			{
				this.lightController.Setup(this.node.room, this, this.lcd, this.isLight, this.lzs, this.ceilingFan);
				this.lightController.SetUnscrewed(this.sw1, true);
				if (this.preset.lightswitch == InteractablePreset.Switch.switchState)
				{
					this.lightController.SetOn(this.sw0, true);
				}
				else if (this.preset.lightswitch == InteractablePreset.Switch.custom1)
				{
					this.lightController.SetOn(this.sw1, true);
				}
				else if (this.preset.lightswitch == InteractablePreset.Switch.custom2)
				{
					this.lightController.SetOn(this.sw2, true);
				}
				else if (this.preset.lightswitch == InteractablePreset.Switch.custom3)
				{
					this.lightController.SetOn(this.sw3, true);
				}
				if (this.ceilingFan != null)
				{
					Toolbox.Instance.DestroyObject(this.ceilingFan.gameObject);
				}
				if (this.node.room.ceilingFans > -1 && this.ml && this.lightController.preset != null && this.lightController.preset.allowCeilingFans)
				{
					this.ceilingFan = Toolbox.Instance.SpawnObject(this.node.room.mainLightPreset.ceilingFans[this.node.room.ceilingFans], this.spawnedObject.transform).transform;
					this.ceilingFan.transform.localPosition = Vector3.zero;
					Toolbox.Instance.SetLightLayer(this.ceilingFan.gameObject, this.node.building, this.node.room.preset.forceStreetLightLayer);
				}
			}
			else
			{
				string text = "Misc Error: No controller for ";
				GameObject gameObject = this.spawnedObject;
				Game.Log(text + ((gameObject != null) ? gameObject.ToString() : null) + " (needed for light)", 2);
			}
		}
		this.SpawnLock();
		if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.bloodPool && this.spawnedObject != null)
		{
			float num = 0.3f + this.val * 0.01f;
			this.spawnedObject.transform.localScale = new Vector3(Mathf.Clamp(num, 0.1f, 1.5f), Mathf.Clamp(num, 0.1f, 1.5f), Mathf.Clamp(num, 0.1f, 1.5f));
		}
		if (this.spawnedDecals != null && this.spawnedObject != null)
		{
			foreach (SpatterSimulation.DecalSpawnData decalSpawnData in this.spawnedDecals)
			{
				if (decalSpawnData.spawnedProjector == null)
				{
					Transform transform = Toolbox.Instance.SearchForTransform(this.spawnedObject.transform, decalSpawnData.subObjectName, false);
					if (transform != null)
					{
						decalSpawnData.SpawnOnTransform(transform);
					}
					else
					{
						Game.LogError(string.Concat(new string[]
						{
							"Unable to load spatter with parent ",
							decalSpawnData.parentID.ToString(),
							" id ",
							decalSpawnData.transformParentID.ToString(),
							" transform name: ",
							decalSpawnData.subObjectName
						}), 2);
					}
				}
			}
		}
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x00159BE8 File Offset: 0x00157DE8
	public void SpawnLock()
	{
		if (this.lockInteractable != null && this.lockInteractable != this)
		{
			if (this.lockInteractable.spawnedObject != null)
			{
				Toolbox.Instance.DestroyObject(this.spawnedObject);
			}
			Transform newParent = this.spawnedObject.transform;
			if (this.controller != null && this.controller.lockParentOverride != null)
			{
				newParent = this.controller.lockParentOverride;
			}
			this.lockInteractable.spawnedObject = Toolbox.Instance.SpawnObject(this.preset.includeLock.prefab, newParent);
			this.lockInteractable.spawnedObject.transform.localPosition = this.preset.lockOffset;
			this.lockInteractable.loadedGeometry = true;
			InteractableController component = this.lockInteractable.spawnedObject.GetComponent<InteractableController>();
			Toolbox.Instance.SetLightLayer(this.lockInteractable.spawnedObject, this.node.building, false);
			component.Setup(this.lockInteractable);
			if (this.preset.includeLock.useMaterialChanges)
			{
				if (this.lockInteractable.locked && this.lockInteractable.preset.lockOnMaterial != null)
				{
					this.lockInteractable.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractable.preset.lockOnMaterial;
					return;
				}
				if (!this.lockInteractable.locked && this.lockInteractable.preset.lockOffMaterial != null)
				{
					this.lockInteractable.spawnedObject.GetComponent<MeshRenderer>().material = this.lockInteractable.preset.lockOffMaterial;
				}
			}
		}
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x00159DA0 File Offset: 0x00157FA0
	public void MarkAsTrash(bool val, bool forceTime = false, float forcedTime = 0f)
	{
		if (val)
		{
			if (forceTime)
			{
				this.mtr = forcedTime;
				if (!CleanupController.Instance.trash.Contains(this))
				{
					bool flag = false;
					for (int i = 0; i < CleanupController.Instance.trash.Count; i++)
					{
						Interactable interactable = CleanupController.Instance.trash[i];
						if (this.mtr <= interactable.mtr)
						{
							CleanupController.Instance.trash.Insert(i, this);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						CleanupController.Instance.trash.Add(this);
						return;
					}
				}
			}
			else
			{
				this.mtr = SessionData.Instance.gameTime + 0.01f;
				if (CleanupController.Instance != null && !CleanupController.Instance.trash.Contains(this))
				{
					CleanupController.Instance.trash.Add(this);
					return;
				}
			}
		}
		else
		{
			this.mtr = -1f;
			if (CleanupController.Instance != null)
			{
				CleanupController.Instance.trash.Remove(this);
			}
		}
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x00159EAC File Offset: 0x001580AC
	public void RemoveFromPlacement()
	{
		this.rPl = true;
		if (this.node != null)
		{
			this.node.RemoveInteractable(this);
			if (this.node.room != null && this.node.room.tamperedInteractables != null)
			{
				this.node.room.tamperedInteractables.Remove(this);
			}
		}
		if (this.spawnNode != null)
		{
			this.spawnNode.RemoveInteractable(this);
			if (this.spawnNode.room != null && this.spawnNode.room.tamperedInteractables != null)
			{
				this.spawnNode.room.tamperedInteractables.Remove(this);
			}
		}
		GameplayController.Instance.interactablesMoved.Remove(this);
		ObjectPoolingController.Instance.MarkAsNotNeeded(this);
		if (this.preset == null)
		{
			Toolbox.Instance.LoadDataFromResources<InteractablePreset>(this.p, out this.preset);
		}
		if (this.preset != null)
		{
			if (this.preset.isClock)
			{
				SessionData.Instance.OnHourChange -= this.OnHourChange;
			}
			if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.tracker)
			{
				GameplayController.Instance.RemoveMotionTracker(this);
			}
			else if (this.preset.specialCaseFlag == InteractablePreset.SpecialCase.grenade)
			{
				GameplayController.Instance.RemoveProxyDetonator(this);
				GameplayController.Instance.activeGrenades.Remove(this);
			}
			if (this.preset.resetBehaviour.Count > 0 && this.node != null && this.node.gameLocation != null)
			{
				this.node.gameLocation.resetBehaviourObjects.Remove(this);
			}
		}
		if (this.furnitureParent != null)
		{
			foreach (NewNode newNode in this.furnitureParent.coversNodes)
			{
				if (newNode != null)
				{
					newNode.RemoveInteractable(this);
				}
			}
			this.furnitureParent.integratedInteractables.Remove(this);
			this.furnitureParent.spawnedInteractables.Remove(this);
		}
		if (this.worldObjectRoomParent != null)
		{
			this.worldObjectRoomParent.worldObjects.Remove(this);
			this.worldObjectRoomParent = null;
		}
		this.DespawnObject();
		if (this.loopingAudio != null)
		{
			for (int i = 0; i < this.loopingAudio.Count; i++)
			{
				AudioController.Instance.StopSound(this.loopingAudio[i], AudioController.StopType.immediate, "Remove object from world");
				this.loopingAudio.RemoveAt(i);
			}
		}
		if (this.OnRemovedFromWorld != null)
		{
			this.OnRemovedFromWorld();
		}
		this.UpdateLoopingAudioParams();
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0015A168 File Offset: 0x00158368
	public void SafeDelete(bool removeFromInventory = false)
	{
		if (removeFromInventory && this.inInventory != null)
		{
			this.SetAsNotInventory(null);
		}
		this.RemoveFromPlacement();
		this.MarkAsTrash(true, false, 0f);
		if (this.IsSafeToDelete())
		{
			this.Delete();
			return;
		}
		if (this.inInventory == null && this.jobParent != null)
		{
			foreach (KeyValuePair<JobPreset.JobTag, Interactable> keyValuePair in this.jobParent.activeJobItems)
			{
				if (keyValuePair.Value == this)
				{
					this.jobParent.OnDestroyMissionObject(keyValuePair.Value);
				}
			}
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x0015A228 File Offset: 0x00158428
	public void Delete()
	{
		this.rem = true;
		if (this.inInventory != null)
		{
			this.SetAsNotInventory(null);
		}
		this.RemoveFromPlacement();
		if (this.isLight != null && this.node != null && this.node.room != null)
		{
			this.node.room.secondaryLights.Remove(this);
			this.node.room.mainLights.Remove(this);
		}
		if (this.worldObjectRoomParent != null)
		{
			this.worldObjectRoomParent.worldObjects.Remove(this);
			this.worldObjectRoomParent = null;
		}
		if (this.jobParent != null)
		{
			List<JobPreset.JobTag> list = new List<JobPreset.JobTag>();
			foreach (KeyValuePair<JobPreset.JobTag, Interactable> keyValuePair in this.jobParent.activeJobItems)
			{
				if (keyValuePair.Value == this)
				{
					this.jobParent.OnDestroyMissionObject(keyValuePair.Value);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (JobPreset.JobTag jobTag in list)
			{
				this.jobParent.activeJobItems.Remove(jobTag);
			}
		}
		if (this.mtr > -1f && CleanupController.Instance != null)
		{
			CleanupController.Instance.trash.Remove(this);
		}
		if (this.evidence != null)
		{
			List<InfoWindow> list2 = InterfaceController.Instance.activeWindows.FindAll((InfoWindow item) => item.passedEvidence == this.evidence);
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i].CloseWindow(false);
			}
			if (!SessionData.Instance.isFloorEdit)
			{
				foreach (Case @case in CasePanelController.Instance.activeCases)
				{
					for (int j = 0; j < @case.caseElements.Count; j++)
					{
						if (@case.caseElements[j].id == this.evidence.evID)
						{
							@case.caseElements.RemoveAt(j);
							this.evidence.OnDataKeyChange -= CasePanelController.Instance.UpdatePinned;
							this.evidence.OnDiscoverConnectedFact -= CasePanelController.Instance.UpdateStrings;
							this.evidence.OnDiscoverChild -= CasePanelController.Instance.UpdateStrings;
							j--;
						}
					}
				}
				foreach (Case case2 in CasePanelController.Instance.archivedCases)
				{
					for (int k = 0; k < case2.caseElements.Count; k++)
					{
						if (case2.caseElements[k].id == this.evidence.evID)
						{
							case2.caseElements.RemoveAt(k);
							this.evidence.OnDataKeyChange -= CasePanelController.Instance.UpdatePinned;
							this.evidence.OnDiscoverConnectedFact -= CasePanelController.Instance.UpdateStrings;
							this.evidence.OnDiscoverChild -= CasePanelController.Instance.UpdateStrings;
							k--;
						}
					}
				}
				foreach (GameplayController.History history in GameplayController.Instance.history.FindAll((GameplayController.History item) => item.evID == this.evidence.evID))
				{
					GameplayController.Instance.history.Remove(history);
					GameplayController.Instance.itemOnlyHistory.Remove(history);
				}
				GameplayController.Instance.evidenceDictionary.Remove(this.evidence.evID);
			}
		}
		if (this.preset.specialCaseFlag != InteractablePreset.SpecialCase.none && this.node != null && this.node.room != null && this.node.room.specialCaseInteractables.ContainsKey(this.preset.specialCaseFlag))
		{
			this.node.room.specialCaseInteractables[this.preset.specialCaseFlag].Remove(this);
		}
		if (!this.cr)
		{
			CleanupController.Instance.removedCityDataItems.Add(this.id);
		}
		Game.Log(string.Concat(new string[]
		{
			"Object: Deleting object ",
			this.name,
			" id ",
			this.id.ToString(),
			" from interactable directory..."
		}), 2);
		CityData.Instance.interactableDirectory.Remove(this);
		CityData.Instance.savableInteractableDictionary.Remove(this.id);
		GameplayController.Instance.activeGadgets.Remove(this);
		if (this.objectRef != null)
		{
			Human.Wound wound = this.objectRef as Human.Wound;
			if (wound != null)
			{
				Human human = null;
				if (CityData.Instance.GetHuman(wound.humanID, out human, true))
				{
					human.currentWounds.Remove(wound);
				}
				if (wound.bloodPool != null)
				{
					wound.bloodPool.SafeDelete(false);
				}
			}
		}
		if (this.OnDelete != null)
		{
			this.OnDelete(this);
		}
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x0015A80C File Offset: 0x00158A0C
	public bool IsSafeToDelete()
	{
		if ((this.controller == null || this.node == null || this.rPl || !this.node.room.isVisible) && this.inInventory == null && (this.evidence == null || !GameplayController.Instance.history.Exists((GameplayController.History item) => item.evID == this.evidence.evID)) && (this.jobParent == null || this.jobParent.state == SideJob.JobState.ended || !Enumerable.Contains<Interactable>(this.jobParent.activeJobItems.Values, this)) && !InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => (this.evidence != null && item.passedEvidence == this.evidence) || item.passedInteractable == this))
		{
			bool flag = true;
			if (this.evidence != null)
			{
				using (List<Case>.Enumerator enumerator = CasePanelController.Instance.activeCases.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.caseElements.Exists((Case.CaseElement item) => item.id == this.evidence.evID))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					using (List<Case>.Enumerator enumerator = CasePanelController.Instance.archivedCases.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.caseElements.Exists((Case.CaseElement item) => item.id == this.evidence.evID))
							{
								flag = false;
								break;
							}
						}
					}
				}
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0015A9A4 File Offset: 0x00158BA4
	public bool IsSaveStateEligable()
	{
		if (this.preset.dontSaveWithSaveGames && (!this.wo || (this.wo && !this.preset.onlySaveWithSaveGamesIfWorldObject)))
		{
			return false;
		}
		if (this.rem)
		{
			return false;
		}
		if (this.force)
		{
			return true;
		}
		if (this.cr)
		{
			return true;
		}
		if (this.spCh)
		{
			return true;
		}
		if (this.mov && this.wo)
		{
			return true;
		}
		if (this.df.Count > 0)
		{
			return true;
		}
		if (this.inInventory != null)
		{
			return true;
		}
		if (!this.preset.dontSaveSwitchStates)
		{
			if (this.sw0 != this.preset.startingSwitchState)
			{
				return true;
			}
			if (this.sw1 != this.preset.startingCustomState1)
			{
				return true;
			}
			if (this.sw2 != this.preset.startingCustomState2)
			{
				return true;
			}
			if (this.sw3 != this.preset.startingCustomState3)
			{
				return true;
			}
			if (this.locked != this.preset.startingLockState)
			{
				return true;
			}
		}
		return this.phy || (this.controller != null && this.controller.physicsOn);
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x0015AAD8 File Offset: 0x00158CD8
	public string GetReasonForSaveStateEligable()
	{
		if (this.force)
		{
			return "Forced save";
		}
		if (this.cr)
		{
			return "Created after start";
		}
		if (this.spCh)
		{
			return "Spawn position changed";
		}
		if (this.mov && this.wo)
		{
			return "Has been moved";
		}
		if (this.df.Count > 0)
		{
			return "Features dynamic prints";
		}
		if (this.inInventory != null)
		{
			return "Is in inventory";
		}
		if (this.sw0 != this.preset.startingSwitchState)
		{
			return "SW0 is different from starting state";
		}
		if (this.sw1 != this.preset.startingCustomState1)
		{
			return "SW1 is different from starting state";
		}
		if (this.sw2 != this.preset.startingCustomState2)
		{
			return "SW2 is different from starting state";
		}
		if (this.sw3 != this.preset.startingCustomState3)
		{
			return "SW3 is different from starting state";
		}
		if (this.locked != this.preset.startingLockState)
		{
			return "Locked is different from starting state";
		}
		if (this.phy)
		{
			return "Physics pick up state";
		}
		if (this.controller != null && this.controller.physicsOn)
		{
			return "Physics on";
		}
		return "Isn't savable";
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x0015AC00 File Offset: 0x00158E00
	public void SetAsLight(LightingPreset newLightPreset, int newLightZoneSize, bool newIsMainLight, Interactable.LightConfiguration preconfiguredLight)
	{
		this.isLight = newLightPreset;
		this.lzs = newLightZoneSize;
		this.ml = newIsMainLight;
		this.lcd = preconfiguredLight;
		if (this.lcd == null)
		{
			this.GenerateLightData();
		}
		if (this.node == null)
		{
			Game.LogError("Unable to Set as Light due to no node! " + this.name, 2);
			return;
		}
		if (this.ml)
		{
			this.node.room.AddMainLight(this);
			return;
		}
		this.node.room.AddSecondaryLight(this);
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0015AC84 File Offset: 0x00158E84
	public void SetNextAIInteraction(NewAIAction newAction, NewAIController ai)
	{
		if (ai.nextAIAction != null && ai.nextAIAction != this)
		{
			ai.nextAIAction.nextAIInteraction = null;
			ai.nextAIAction = null;
		}
		this.nextAIInteraction = newAction;
		if (newAction != null)
		{
			newAction.goal.aiController.nextAIAction = this;
		}
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x0015ACD0 File Offset: 0x00158ED0
	public void OnDoorMovementClosed()
	{
		if (!this.locked && this.preset.armLockOnClose && this.lockInteractable != null)
		{
			InteractablePreset.InteractionAction action = this.lockInteractable.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.effectSwitchStates.Exists((InteractablePreset.SwitchState item2) => item2.switchState == InteractablePreset.Switch.lockState && item2.boolIs));
			this.lockInteractable.OnInteraction(action, null, true, 0f);
		}
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x00002265 File Offset: 0x00000465
	public void OnDoorMovementOpened()
	{
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0015AD44 File Offset: 0x00158F44
	public List<int> GetPasswordFromSource(out List<string> notePlacements)
	{
		notePlacements = new List<string>();
		if (this.passwordSource == null)
		{
			Game.Log("No password source for " + this.name, 2);
		}
		Interactable interactable = this.passwordSource as Interactable;
		if (interactable != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Password source is interactable ",
				interactable.id.ToString(),
				" (",
				interactable.GetName(),
				")"
			}), 2);
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				notePlacements.Add(interactable.passcode.GetNotePlacements());
			}
			return interactable.passcode.GetDigits();
		}
		Human human = this.passwordSource as Human;
		if (human != null)
		{
			Game.Log("Password source is human " + human.humanID.ToString(), 2);
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				notePlacements.Add(human.passcode.GetNotePlacements());
			}
			return human.passcode.GetDigits();
		}
		NewRoom newRoom = this.passwordSource as NewRoom;
		if (newRoom != null)
		{
			Game.Log("Password source is room " + newRoom.roomID.ToString(), 2);
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				notePlacements.Add(newRoom.passcode.GetNotePlacements());
			}
			return newRoom.passcode.GetDigits();
		}
		NewAddress newAddress = this.passwordSource as NewAddress;
		if (newAddress != null)
		{
			Game.Log("Password source is address " + newAddress.id.ToString(), 2);
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				notePlacements.Add(newAddress.passcode.GetNotePlacements());
			}
			return newAddress.passcode.GetDigits();
		}
		Game.Log("Returned no password: 0000", 2);
		return Enumerable.ToList<int>(new int[4]);
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x0015AF44 File Offset: 0x00159144
	public GameplayController.Passcode GetPasswordSource()
	{
		if (this.passwordSource == null)
		{
			Game.Log("No password source for " + this.name, 2);
		}
		else
		{
			string text = "Password source is ";
			object obj = this.passwordSource;
			Game.Log(text + ((obj != null) ? obj.ToString() : null), 2);
		}
		Interactable interactable = this.passwordSource as Interactable;
		if (interactable != null)
		{
			return interactable.passcode;
		}
		Human human = this.passwordSource as Human;
		if (human != null)
		{
			return human.passcode;
		}
		NewRoom newRoom = this.passwordSource as NewRoom;
		if (newRoom != null)
		{
			return newRoom.passcode;
		}
		NewAddress newAddress = this.passwordSource as NewAddress;
		if (newAddress != null)
		{
			return newAddress.passcode;
		}
		return null;
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x0015B000 File Offset: 0x00159200
	public void AddPasswordSourceToAcquired()
	{
		GameplayController.Passcode passcode = null;
		if (this.passwordSource == null)
		{
			Game.Log("No password source for " + this.name, 2);
		}
		else
		{
			string text = "Password source is ";
			object obj = this.passwordSource;
			Game.Log(text + ((obj != null) ? obj.ToString() : null), 2);
		}
		Interactable interactable = this.passwordSource as Interactable;
		if (interactable != null)
		{
			passcode = interactable.passcode;
		}
		Human human = this.passwordSource as Human;
		if (human != null)
		{
			passcode = human.passcode;
		}
		NewRoom newRoom = this.passwordSource as NewRoom;
		if (newRoom != null)
		{
			passcode = newRoom.passcode;
		}
		NewAddress newAddress = this.passwordSource as NewAddress;
		if (newAddress != null)
		{
			passcode = newAddress.passcode;
		}
		if (passcode != null)
		{
			GameplayController.Instance.AddPasscode(passcode, true);
		}
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x0015B0D0 File Offset: 0x001592D0
	public void SetActionHighlight(string newString, bool val)
	{
		Game.Log("Object: Looking to highlight action: " + newString, 2);
		InteractablePreset.InteractionAction interactionAction = this.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
		if (interactionAction == null)
		{
			interactionAction = this.preset.GetActions(1).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
			if (interactionAction == null)
			{
				interactionAction = this.preset.GetActions(2).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
			}
		}
		if (interactionAction == null)
		{
			Game.Log(newString + " action not found", 2);
			return;
		}
		if (val)
		{
			if (!this.highlightActions.Contains(interactionAction))
			{
				this.highlightActions.Add(interactionAction);
			}
		}
		else
		{
			this.highlightActions.Remove(interactionAction);
		}
		InteractionController.Instance.UpdateInteractionText();
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x0015B1B0 File Offset: 0x001593B0
	public void SetActionDisable(string newString, bool val)
	{
		Game.Log(string.Concat(new string[]
		{
			"Object: Disable ",
			this.name,
			" : ",
			newString,
			"..."
		}), 2);
		InteractablePreset.InteractionAction interactionAction = this.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
		if (interactionAction == null)
		{
			interactionAction = this.preset.GetActions(1).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
			if (interactionAction == null)
			{
				interactionAction = this.preset.GetActions(2).Find((InteractablePreset.InteractionAction item) => item.interactionName.ToLower() == newString.ToLower());
			}
		}
		if (interactionAction == null)
		{
			Game.Log("Object: ...Cannot find action " + newString + ", aborting...", 2);
			return;
		}
		if (val)
		{
			if (!this.disabledActions.Contains(interactionAction))
			{
				Game.Log("Object: Adding " + interactionAction.interactionName + " to disabled actions", 2);
				this.disabledActions.Add(interactionAction);
			}
		}
		else
		{
			Game.Log("Object: Removing " + interactionAction.interactionName + " from disabled actions", 2);
			this.disabledActions.Remove(interactionAction);
		}
		this.UpdateCurrentActions();
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x0015B2EC File Offset: 0x001594EC
	public void SetOriginalPosition(bool newVal, bool setGameTime = true)
	{
		if (this.originalPosition != newVal)
		{
			this.originalPosition = newVal;
			if (this.originalPosition)
			{
				this.wPos = this.spWPos;
				this.wEuler = this.spWEuler;
				if (this.wo && this.fp > -1)
				{
					this.ResetToFurnitureObject(true);
				}
				else
				{
					this.UpdateWorldPositionAndNode(false);
				}
				this.SetTampered(false);
				this.distanceFromSpawn = 0f;
				if (this.spawnNode != null && this.spawnNode.gameLocation != null && this.spawnNode.gameLocation.thisAsAddress != null)
				{
					this.spawnNode.gameLocation.thisAsAddress.RemoveVandalism(this);
				}
				if (this.controller != null && this.controller.rb != null)
				{
					this.controller.rb.MovePosition(this.spWPos);
					this.controller.rb.MoveRotation(Quaternion.Euler(this.spWEuler));
					this.controller.rb.velocity = Vector3.zero;
					this.controller.rb.angularVelocity = Vector3.zero;
					SessionData.Instance.ExecuteSyncPhysics(SessionData.PhysicsSyncType.now);
				}
				GameplayController.Instance.interactablesMoved.Remove(this);
				return;
			}
			if (setGameTime)
			{
				this.lma = SessionData.Instance.gameTime;
			}
			if (this.spR && !GameplayController.Instance.interactablesMoved.Contains(this))
			{
				GameplayController.Instance.interactablesMoved.Add(this);
			}
		}
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0015B484 File Offset: 0x00159684
	public void SetTampered(bool val)
	{
		if (this.isTampered != val && this.spawnNode != null && this.spawnNode.room != null)
		{
			this.isTampered = val;
			if (this.isTampered)
			{
				if (!Player.Instance.isTrespassing)
				{
					this.spawnNode.room.tamperedInteractables.Remove(this);
					return;
				}
				if (!this.spawnNode.room.tamperedInteractables.Contains(this))
				{
					this.spawnNode.room.tamperedInteractables.Add(this);
					return;
				}
			}
			else if (this.spawnNode.room.tamperedInteractables.Contains(this))
			{
				this.spawnNode.room.tamperedInteractables.Remove(this);
			}
		}
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0015B550 File Offset: 0x00159750
	public void AddNewDynamicFingerprint(Human from, Interactable.PrintLife life)
	{
		if (from == null)
		{
			return;
		}
		if (!this.preset.fingerprintsEnabled)
		{
			return;
		}
		if (!this.preset.enableDynamicFingerprints)
		{
			return;
		}
		if (this.df == null)
		{
			this.df = new List<Interactable.DynamicFingerprint>();
		}
		if (from.isPlayer)
		{
			return;
		}
		if (this.preset.overrideMaxDynamicFingerprints)
		{
			if (this.df.Count >= this.preset.maxDynamicFingerprints)
			{
				return;
			}
		}
		else if (this.df.Count >= GameplayControls.Instance.maxDynamicPrintsPerObject)
		{
			return;
		}
		Interactable.DynamicFingerprint dynamicFingerprint = new Interactable.DynamicFingerprint
		{
			id = from.humanID,
			created = SessionData.Instance.gameTime,
			seed = Toolbox.Instance.GenerateSeed(4, false, ""),
			life = life
		};
		this.df.Add(dynamicFingerprint);
		if (!GameplayController.Instance.objectsWithDynamicPrints.Contains(this))
		{
			GameplayController.Instance.objectsWithDynamicPrints.Add(this);
		}
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x0015B64C File Offset: 0x0015984C
	public void RemoveDynamicPrint(Interactable.DynamicFingerprint print)
	{
		if (this.df == null)
		{
			this.df = new List<Interactable.DynamicFingerprint>();
		}
		this.df.Remove(print);
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x0015B670 File Offset: 0x00159870
	public void OnHourChange()
	{
		if (this.preset.hourlyChime != null && SessionData.Instance.gameTime > this.lhc && SessionData.Instance.startedGame)
		{
			this.lhc = SessionData.Instance.gameTime;
			for (int i = 0; i < AudioController.Instance.delayedSound.Count; i++)
			{
				if (AudioController.Instance.delayedSound[i].worldPosition == this.wPos)
				{
					AudioController.Instance.delayedSound.RemoveAt(i);
					i--;
				}
			}
			if (this.preset.chimeEqualToHour)
			{
				int j;
				for (j = Mathf.FloorToInt(SessionData.Instance.FloatMinutes24H(SessionData.Instance.decimalClock)); j > 12; j -= 12)
				{
				}
				for (int k = 0; k < j; k++)
				{
					AudioController.Instance.PlayOneShotDelayed(this.preset.chimeDelay * (float)k, this.preset.hourlyChime, null, this.node, this.wPos, null, 1f, null, false);
				}
				return;
			}
			AudioController.Instance.PlayWorldOneShot(this.preset.hourlyChime, null, this.node, this.wPos, this, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0015B7C0 File Offset: 0x001599C0
	public void OnLockpick()
	{
		if (!this.locked)
		{
			Game.Log("locked: " + this.locked.ToString(), 2);
			return;
		}
		InteractionController.Instance.SetLockedInInteractionMode(this, 0, true);
		Player.Instance.TransformPlayerController(GameplayControls.Instance.lockpickEnter, GameplayControls.Instance.lockpickExit, this, null, false, false, 0f, false, default(Vector3), 1f, true);
		InteractionController.Instance.SetInteractionAction(0f, this.val, Mathf.LerpUnclamped(GameplayControls.Instance.lockpickEffectivenessRange.x, GameplayControls.Instance.lockpickEffectivenessRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingEfficiencyModifier)), "lockpicking", true, true, this.controller.transform, true);
		Player.Instance.isLockpicking = true;
		InteractionController.Instance.OnInteractionActionProgressChange += this.OnLockpickProgressChange;
		InteractionController.Instance.OnInteractionActionCompleted += this.OnCompleteLockpick;
		InteractionController.Instance.OnReturnFromLockedIn += this.OnReturnFromLockpick;
		InteractionController.Instance.OnInteractionActionLookedAway += this.OnLockpickLookedAway;
		AudioController.Instance.StopSound(this.actionLoop, AudioController.StopType.immediate, "Stop lockpick");
		if (Player.Instance.home != Player.Instance.currentNode.gameLocation && !Enumerable.Contains<NewGameLocation>(Player.Instance.apartmentsOwned, Player.Instance.currentNode.gameLocation))
		{
			StatusController.Instance.AddFineRecord(Player.Instance.currentNode.gameLocation.thisAsAddress, this, StatusController.CrimeType.breakingAndEntering, false, -1, false);
		}
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0015B968 File Offset: 0x00159B68
	public void OnLockpickLookedAway()
	{
		AudioController.Instance.StopSound(this.actionLoop, AudioController.StopType.immediate, "Stop lockpick");
		this.audioLoopStarted = false;
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x0015B988 File Offset: 0x00159B88
	public void OnLockpickProgressChange(float amountChangeThisFrame, float amountToal)
	{
		if (!this.audioLoopStarted)
		{
			this.audioLoopStarted = true;
			if (InteractionController.Instance.interactionActionLookAt.name.Contains("Padlock", 5))
			{
				this.actionLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.lockpickMetal, Player.Instance, this, null, 1f, false, false, null, null);
			}
			else
			{
				this.actionLoop = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.lockpick, Player.Instance, this, null, 1f, false, false, null, null);
			}
		}
		this.SetValue(this.val - amountChangeThisFrame);
		InteractionController.Instance.DisplayInteractionCursor(InteractionController.Instance.displayingInteraction, true);
		if (this.val <= 0f)
		{
			Game.Log("Set " + this.name + " unlocked", 2);
			this.SetLockedState(false, Player.Instance, true, true);
			if (this.thisDoor != null)
			{
				this.thisDoor.SetLockedState(false, Player.Instance, true, true);
			}
		}
		GameplayController.Instance.UseLockpick(amountChangeThisFrame);
		if (GameplayController.Instance.lockPicks <= 0)
		{
			InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "not_enough_lockpicks", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.lockpick, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x0015BAE8 File Offset: 0x00159CE8
	public void OnCompleteLockpick()
	{
		AudioController.Instance.StopSound(this.actionLoop, AudioController.StopType.fade, "Complete lockpick");
		this.actionLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnLockpickProgressChange;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteLockpick;
		Player.Instance.isLockpicking = false;
		InteractionController.Instance.SetLockedInInteractionMode(null, 0, true);
		StatusController.Instance.RemoveFineRecord(null, this, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0015BB68 File Offset: 0x00159D68
	public void OnReturnFromLockpick()
	{
		AudioController.Instance.StopSound(this.actionLoop, AudioController.StopType.fade, "Return from lockpick");
		this.actionLoop = null;
		InteractionController.Instance.OnInteractionActionProgressChange -= this.OnLockpickProgressChange;
		InteractionController.Instance.OnReturnFromLockedIn -= this.OnReturnFromLockpick;
		InteractionController.Instance.OnInteractionActionCompleted -= this.OnCompleteLockpick;
		InteractionController.Instance.OnInteractionActionLookedAway -= this.OnLockpickLookedAway;
		Player.Instance.isLockpicking = false;
		Player.Instance.ReturnFromTransform(false, true);
		InterfaceController instance = InterfaceController.Instance;
		InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
		int newNumerical = 0;
		string newMessage = GameplayController.Instance.lockPicks.ToString() + " " + Strings.Get("ui.gamemessage", "lockpick_deplete", Strings.Casing.asIs, false, false, false, null);
		InterfaceControls.Icon newIcon = InterfaceControls.Icon.lockpick;
		AudioEvent additionalSFX = null;
		bool colourOverride = false;
		RectTransform moneyNotificationIcon = InterfaceController.Instance.moneyNotificationIcon;
		instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, moneyNotificationIcon, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		StatusController.Instance.RemoveFineRecord(null, this, StatusController.CrimeType.breakingAndEntering, true, true);
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x0015BC6C File Offset: 0x00159E6C
	public void ForcePhysicsActive(bool forceSpawnLocation, bool applyForce, Vector3 force = default(Vector3), ForceMode forceMode = 2, bool useThrowingForce = false)
	{
		Game.Log("Object: Forcing physics active on " + this.name + "...", 2);
		if (this.node == null)
		{
			this.UpdateWorldPositionAndNode(false);
		}
		if (this.controller == null && this.node != null)
		{
			if (this.node.room != null && this.node.room.gameObject != null && !this.node.room.gameObject.activeSelf && forceSpawnLocation)
			{
				Game.Log("Object: ... Force spawn location " + this.node.room.name, 2);
				this.node.room.SetVisible(true, true, "Force physics spawn", true);
			}
			if (!(this.node.room != null) || !(this.node.room.gameObject != null) || !this.node.room.gameObject.activeSelf)
			{
				Game.Log("Object: ... Spawning on ground...", 2);
				Vector2 vector = Random.insideUnitCircle * 0.5f;
				Vector3 vector2 = this.node.position;
				if (this.node.walkableNodeSpace.Count > 0)
				{
					List<NewNode.NodeSpace> list = new List<NewNode.NodeSpace>(this.node.walkableNodeSpace.Values);
					vector2 = list[Toolbox.Instance.Rand(0, list.Count, false)].position + new Vector3(vector.x, 0f, vector.y);
				}
				else
				{
					vector2 += new Vector3(vector.x, 0f, vector.y);
				}
				Vector3 newEulerAngle;
				newEulerAngle..ctor(0f, Toolbox.Instance.Rand(0f, 360f, false));
				this.MoveInteractable(vector2, newEulerAngle, true);
				return;
			}
			this.LoadInteractableToWorld(false, true);
			this.controller.DropThis(useThrowingForce);
			if (applyForce && !useThrowingForce)
			{
				this.controller.rb.AddForce(force, 2);
				return;
			}
		}
		else if (this.controller != null)
		{
			this.controller.DropThis(useThrowingForce);
			if (applyForce && !useThrowingForce)
			{
				this.controller.rb.AddForce(force, 2);
			}
		}
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x0015BEBC File Offset: 0x0015A0BC
	public void ConvertToFurnitureSpawnedObject(FurnitureLocation newFurniture, FurniturePreset.SubObject newSubObject, bool updatePosition = true, bool updateSpawnPosition = true)
	{
		this.wo = false;
		if (this.worldObjectRoomParent != null)
		{
			this.worldObjectRoomParent.worldObjects.Remove(this);
			this.worldObjectRoomParent = null;
		}
		this.furnitureParent = newFurniture;
		this.fp = newFurniture.id;
		this.subObject = newSubObject;
		this.fsoi = newFurniture.furniture.subObjects.IndexOf(this.subObject);
		this.lPos = this.subObject.localPos;
		this.lEuler = this.subObject.localRot;
		if (this.subObject != null && !this.furnitureParent.spawnedInteractables.Contains(this))
		{
			this.furnitureParent.spawnedInteractables.Add(this);
		}
		if (updatePosition)
		{
			this.UpdateWorldPositionAndNode(updateSpawnPosition);
		}
		if (this.furnitureParent != null && this.furnitureParent.spawnedObject != null)
		{
			this.LoadInteractableToWorld(false, false);
		}
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0015BFAC File Offset: 0x0015A1AC
	public void ConvertToWorldObject(bool updatePosition = true)
	{
		if (this.furnitureParent != null)
		{
			this.furnitureParent.integratedInteractables.Remove(this);
			this.furnitureParent.spawnedInteractables.Remove(this);
		}
		this.wo = true;
		this.furnitureParent = null;
		this.subObject = null;
		if (updatePosition)
		{
			this.UpdateWorldPositionAndNode(false);
		}
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x0015C008 File Offset: 0x0015A208
	public void ResetToFurnitureObject(bool updatePosition = true)
	{
		Game.Log(string.Concat(new string[]
		{
			"Resetting ",
			this.name,
			" to furniture object at furniture ID ",
			this.fp.ToString(),
			" and fsoi ",
			this.fsoi.ToString(),
			"..."
		}), 2);
		this.wo = false;
		if (this.worldObjectRoomParent != null)
		{
			this.worldObjectRoomParent.worldObjects.Remove(this);
			this.worldObjectRoomParent = null;
		}
		this.DespawnObject();
		if (this.fp > -1 && this.furnitureParent == null)
		{
			NewNode newNode = null;
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(this.spWPos);
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
			{
				this.furnitureParent = newNode.room.individualFurniture.Find((FurnitureLocation item) => item.id == this.fp);
				if (this.furnitureParent == null)
				{
					foreach (NewRoom newRoom in newNode.gameLocation.rooms)
					{
						this.furnitureParent = newRoom.individualFurniture.Find((FurnitureLocation item) => item.id == this.fp);
						if (this.furnitureParent != null)
						{
							break;
						}
					}
				}
			}
			if (this.furnitureParent == null)
			{
				foreach (NewRoom newRoom2 in CityData.Instance.roomDirectory)
				{
					this.furnitureParent = newRoom2.individualFurniture.Find((FurnitureLocation item) => item.id == this.fp);
					if (this.furnitureParent != null)
					{
						break;
					}
				}
			}
			if (this.furnitureParent != null)
			{
				if (this.fsoi <= -1 && !this.furnitureParent.integratedInteractables.Contains(this))
				{
					this.furnitureParent.integratedInteractables.Add(this);
				}
			}
			else
			{
				Game.LogError("Unable to find furniture parent: " + this.fp.ToString(), 2);
			}
		}
		if (this.fsoi > -1 && this.furnitureParent != null && this.furnitureParent.furniture != null && this.subObject == null)
		{
			if (this.fsoi >= this.furnitureParent.furniture.subObjects.Count)
			{
				Game.LogError("Furniture sub object out of range! Are you sure this is the correct furniture? " + this.fsoi.ToString(), 2);
			}
			else
			{
				this.subObject = this.furnitureParent.furniture.subObjects[this.fsoi];
			}
		}
		if (this.furnitureParent != null && this.subObject != null)
		{
			Game.Log("...Found furniture parent and sub object index...", 2);
			this.ConvertToFurnitureSpawnedObject(this.furnitureParent, this.subObject, updatePosition, false);
		}
		if (updatePosition && this.node != null && this.node.room != null && this.node.room.geometryLoaded)
		{
			if (this.furnitureParent != null)
			{
				if (this.furnitureParent.spawnedObject != null)
				{
					this.LoadInteractableToWorld(false, false);
					return;
				}
			}
			else
			{
				this.LoadInteractableToWorld(false, false);
			}
		}
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x0015C350 File Offset: 0x0015A550
	public bool IsLitter()
	{
		if (this.preset.isLitter)
		{
			return true;
		}
		if (this.preset.isInventoryItem && this.preset.consumableAmount > 0f && this.cs <= 0.1f)
		{
			return true;
		}
		if (this.pv != null)
		{
			if (this.pv.Exists((Interactable.Passed item) => item.varType == Interactable.PassedVarType.isTrash))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0015C3D4 File Offset: 0x0015A5D4
	public bool PickUpTarget(Human pickerUpper, bool pickUpLitter = false)
	{
		if (pickerUpper.isStunned || pickerUpper.isDead || pickerUpper.currentNerve <= 0.05f)
		{
			return false;
		}
		if (this.inInventory != null)
		{
			return false;
		}
		if (this.rem)
		{
			return false;
		}
		if (this.rPl)
		{
			return false;
		}
		if (this.phy)
		{
			return false;
		}
		if (!this.preset.spawnable || this.preset.prefab == null)
		{
			return false;
		}
		if (this.controller != null && this.controller.isCarriedByPlayer)
		{
			return false;
		}
		if (this.controller != null && this.controller.physicsOn)
		{
			return false;
		}
		if (!this.preset.AIWillCorrectPosition)
		{
			return false;
		}
		if (FirstPersonItemController.Instance.slots.Exists((FirstPersonItemController.InventorySlot item) => item.interactableID > -1 && item.interactableID == this.id))
		{
			return false;
		}
		if (pickUpLitter && this.IsLitter())
		{
			return true;
		}
		if (!(this.preset.physicsProfile != null) || !this.preset.tamperEnabled || !this.isTampered || !this.aiActionReference.ContainsKey(RoutineControls.Instance.pickupFromFloor))
		{
			return false;
		}
		if (this.preset.isMoney)
		{
			return true;
		}
		if (this.preset.isInventoryItem && this.preset.consumableAmount > 0f && this.cs > 0f && this.belongsTo != pickerUpper && pickerUpper.locationsOfAuthority.Contains(this.node.gameLocation))
		{
			return true;
		}
		if (this.IsLitter())
		{
			return !pickerUpper.isLitterBug;
		}
		return pickerUpper.locationsOfAuthority.Contains(this.spawnNode.gameLocation);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0015C5A4 File Offset: 0x0015A7A4
	public void RemoveManuallyCreatedFingerprints()
	{
		for (int i = 0; i < this.df.Count; i++)
		{
			Interactable.DynamicFingerprint dynamicFingerprint = this.df[i];
			if (dynamicFingerprint != null && dynamicFingerprint.life == Interactable.PrintLife.manualRemoval)
			{
				this.RemoveDynamicPrint(dynamicFingerprint);
				i--;
			}
		}
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x0015C5EB File Offset: 0x0015A7EB
	public float GetReachDistance()
	{
		return GameplayControls.Instance.interactionRange * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.reachModifier)) + this.preset.rangeModifier;
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x0015C618 File Offset: 0x0015A818
	public float GetSecurityStrength()
	{
		if (this.node != null && this.node.gameLocation != null)
		{
			return Toolbox.Instance.GetNormalizedLandValue(this.node.gameLocation, false) * GameplayControls.Instance.sabotageLandValueMP;
		}
		Toolbox instance = Toolbox.Instance;
		float lowerRange = 0f;
		float upperRange = 0.5f;
		Vector3 vector = this.wPos;
		return instance.GetPsuedoRandomNumber(lowerRange, upperRange, vector.ToString() + this.id.ToString(), false) * GameplayControls.Instance.sabotageLandValueMP;
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x0015C6A8 File Offset: 0x0015A8A8
	public bool IsInteractablePhysicsObject()
	{
		bool result = false;
		if (this.preset != null && this.preset.physicsProfile != null && this.preset.reactWithExternalStimuli)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x0015C6E8 File Offset: 0x0015A8E8
	public bool GetSwitchQuery(InteractablePreset.Switch switchState)
	{
		if (switchState == InteractablePreset.Switch.switchState)
		{
			return this.sw0;
		}
		if (switchState == InteractablePreset.Switch.lockState)
		{
			return this.locked;
		}
		if (switchState == InteractablePreset.Switch.custom1)
		{
			return this.sw1;
		}
		if (switchState == InteractablePreset.Switch.custom2)
		{
			return this.sw2;
		}
		if (switchState == InteractablePreset.Switch.custom3)
		{
			return this.sw3;
		}
		if (switchState == InteractablePreset.Switch.lockedIn)
		{
			if (InteractionController.Instance.lockedInInteraction != null)
			{
				return true;
			}
		}
		else
		{
			if (switchState == InteractablePreset.Switch.sprinting)
			{
				return Player.Instance.isRunning;
			}
			if (switchState == InteractablePreset.Switch.enforcersInside)
			{
				if (this.node.gameLocation.currentOccupants.Exists((Actor item) => item.isEnforcer && item.isOnDuty))
				{
					return true;
				}
			}
			else if (switchState == InteractablePreset.Switch.ko)
			{
				if (this.isActor != null && this.isActor.isStunned)
				{
					return true;
				}
			}
			else
			{
				if (switchState == InteractablePreset.Switch.securityGrid)
				{
					return this.sw0 && (this.sw1 || this.sw2);
				}
				if (switchState == InteractablePreset.Switch.carryPhysicsObject)
				{
					return this.phy;
				}
			}
		}
		return false;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x0015C7DC File Offset: 0x0015A9DC
	public void SetSwtichByType(InteractablePreset.Switch switchState, bool val, Actor interactor, bool playSFX = true, bool forceUpdate = false)
	{
		if (switchState == InteractablePreset.Switch.switchState)
		{
			this.SetSwitchState(val, interactor, playSFX, forceUpdate, false);
			return;
		}
		if (switchState == InteractablePreset.Switch.lockState)
		{
			this.SetLockedState(val, interactor, playSFX, forceUpdate);
			return;
		}
		if (switchState == InteractablePreset.Switch.custom1)
		{
			this.SetCustomState1(val, interactor, playSFX, forceUpdate, false);
			return;
		}
		if (switchState == InteractablePreset.Switch.custom2)
		{
			this.SetCustomState2(val, interactor, playSFX, forceUpdate, false);
			return;
		}
		if (switchState == InteractablePreset.Switch.custom3)
		{
			this.SetCustomState3(val, interactor, playSFX, forceUpdate, false);
			return;
		}
		if (switchState == InteractablePreset.Switch.carryPhysicsObject)
		{
			this.SetPhysicsPickupState(val, interactor, playSFX, forceUpdate);
		}
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x0015C854 File Offset: 0x0015AA54
	public bool TryGetCreationTime(out float creationTime)
	{
		creationTime = 0f;
		if (this.pv != null)
		{
			foreach (Interactable.Passed passed in this.pv)
			{
				if (passed.varType == Interactable.PassedVarType.creationTime)
				{
					creationTime = passed.value;
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x04001B86 RID: 7046
	[Header("Serializable")]
	public string name;

	// Token: 0x04001B87 RID: 7047
	public int id;

	// Token: 0x04001B88 RID: 7048
	[NonSerialized]
	public static int worldAssignID = 100000000;

	// Token: 0x04001B89 RID: 7049
	public Vector3 wPos;

	// Token: 0x04001B8A RID: 7050
	public Vector3 wEuler;

	// Token: 0x04001B8B RID: 7051
	public Vector3 lPos;

	// Token: 0x04001B8C RID: 7052
	public Vector3 lEuler;

	// Token: 0x04001B8D RID: 7053
	public Vector3 spWPos;

	// Token: 0x04001B8E RID: 7054
	public Vector3 spWEuler;

	// Token: 0x04001B8F RID: 7055
	public Vector3Int spNode;

	// Token: 0x04001B90 RID: 7056
	public bool spR;

	// Token: 0x04001B91 RID: 7057
	public string p;

	// Token: 0x04001B92 RID: 7058
	public List<Interactable.Passed> pv;

	// Token: 0x04001B93 RID: 7059
	public int fp = -1;

	// Token: 0x04001B94 RID: 7060
	public int fsoi = -1;

	// Token: 0x04001B95 RID: 7061
	public int dp = -1;

	// Token: 0x04001B96 RID: 7062
	public Toolbox.MaterialKey mk;

	// Token: 0x04001B97 RID: 7063
	public Interactable.LightConfiguration lcd;

	// Token: 0x04001B98 RID: 7064
	public string lp;

	// Token: 0x04001B99 RID: 7065
	public string bp;

	// Token: 0x04001B9A RID: 7066
	public string sdp;

	// Token: 0x04001B9B RID: 7067
	public string dds;

	// Token: 0x04001B9C RID: 7068
	public int pt;

	// Token: 0x04001B9D RID: 7069
	public int w = -1;

	// Token: 0x04001B9E RID: 7070
	public int r = -1;

	// Token: 0x04001B9F RID: 7071
	public int b = -1;

	// Token: 0x04001BA0 RID: 7072
	public int inv = -1;

	// Token: 0x04001BA1 RID: 7073
	public float val;

	// Token: 0x04001BA2 RID: 7074
	public bool ins;

	// Token: 0x04001BA3 RID: 7075
	public float lma;

	// Token: 0x04001BA4 RID: 7076
	public float cs;

	// Token: 0x04001BA5 RID: 7077
	public bool wo;

	// Token: 0x04001BA6 RID: 7078
	public int opp;

	// Token: 0x04001BA7 RID: 7079
	public string bo;

	// Token: 0x04001BA8 RID: 7080
	public string sd;

	// Token: 0x04001BA9 RID: 7081
	public bool sw0;

	// Token: 0x04001BAA RID: 7082
	public bool sw1;

	// Token: 0x04001BAB RID: 7083
	public bool sw2;

	// Token: 0x04001BAC RID: 7084
	public bool sw3;

	// Token: 0x04001BAD RID: 7085
	public bool locked;

	// Token: 0x04001BAE RID: 7086
	public bool phy;

	// Token: 0x04001BAF RID: 7087
	public bool drm;

	// Token: 0x04001BB0 RID: 7088
	public int lzs = -1;

	// Token: 0x04001BB1 RID: 7089
	public bool ml;

	// Token: 0x04001BB2 RID: 7090
	public int pto;

	// Token: 0x04001BB3 RID: 7091
	public List<InteractablePreset.SubSpawnSlot> ssp;

	// Token: 0x04001BB4 RID: 7092
	public float mtr = -1f;

	// Token: 0x04001BB5 RID: 7093
	public bool cr;

	// Token: 0x04001BB6 RID: 7094
	public bool rem;

	// Token: 0x04001BB7 RID: 7095
	public bool rPl;

	// Token: 0x04001BB8 RID: 7096
	public bool spCh;

	// Token: 0x04001BB9 RID: 7097
	public bool force;

	// Token: 0x04001BBA RID: 7098
	public List<Interactable.DynamicFingerprint> df = new List<Interactable.DynamicFingerprint>();

	// Token: 0x04001BBB RID: 7099
	public List<Interactable.SavedPrint> print = new List<Interactable.SavedPrint>();

	// Token: 0x04001BBC RID: 7100
	public List<SceneRecorder.SceneCapture> cap = new List<SceneRecorder.SceneCapture>();

	// Token: 0x04001BBD RID: 7101
	public List<SceneRecorder.SceneCapture> sCap = new List<SceneRecorder.SceneCapture>();

	// Token: 0x04001BBE RID: 7102
	public int nEvKey = -1;

	// Token: 0x04001BBF RID: 7103
	public float lhc;

	// Token: 0x04001BC0 RID: 7104
	public bool ft;

	// Token: 0x04001BC1 RID: 7105
	public GameplayController.Passcode passcode;

	// Token: 0x04001BC2 RID: 7106
	[Header("Non Serialized")]
	[NonSerialized]
	public bool mov;

	// Token: 0x04001BC3 RID: 7107
	[NonSerialized]
	public bool audioLoopStarted;

	// Token: 0x04001BC4 RID: 7108
	[NonSerialized]
	public Telephone t;

	// Token: 0x04001BC5 RID: 7109
	[NonSerialized]
	public string seed;

	// Token: 0x04001BC6 RID: 7110
	[NonSerialized]
	public bool save;

	// Token: 0x04001BC7 RID: 7111
	[NonSerialized]
	public bool isTampered;

	// Token: 0x04001BC8 RID: 7112
	[NonSerialized]
	public float distanceFromSpawn;

	// Token: 0x04001BC9 RID: 7113
	[NonSerialized]
	public bool originalPosition = true;

	// Token: 0x04001BCA RID: 7114
	[NonSerialized]
	public Vector3 cvp;

	// Token: 0x04001BCB RID: 7115
	[NonSerialized]
	public Vector3 cve;

	// Token: 0x04001BCC RID: 7116
	[NonSerialized]
	public Evidence evidence;

	// Token: 0x04001BCD RID: 7117
	[NonSerialized]
	public SceneRecorder sceneRecorder;

	// Token: 0x04001BCE RID: 7118
	[NonSerialized]
	public Transform spawnParent;

	// Token: 0x04001BCF RID: 7119
	[NonSerialized]
	public Transform parentTransform;

	// Token: 0x04001BD0 RID: 7120
	[NonSerialized]
	public Human inInventory;

	// Token: 0x04001BD1 RID: 7121
	[NonSerialized]
	public InteractablePreset preset;

	// Token: 0x04001BD2 RID: 7122
	[NonSerialized]
	public FurnitureLocation furnitureParent;

	// Token: 0x04001BD3 RID: 7123
	[NonSerialized]
	public FurniturePreset.SubObject subObject;

	// Token: 0x04001BD4 RID: 7124
	[NonSerialized]
	public SideJob jobParent;

	// Token: 0x04001BD5 RID: 7125
	[NonSerialized]
	public MurderController.Murder murderParent;

	// Token: 0x04001BD6 RID: 7126
	[NonSerialized]
	public SpeechController speechController;

	// Token: 0x04001BD7 RID: 7127
	[NonSerialized]
	public InteractableController controller;

	// Token: 0x04001BD8 RID: 7128
	[NonSerialized]
	public LightController lightController;

	// Token: 0x04001BD9 RID: 7129
	[NonSerialized]
	public Interactable lockInteractable;

	// Token: 0x04001BDA RID: 7130
	[NonSerialized]
	public Interactable thisDoor;

	// Token: 0x04001BDB RID: 7131
	[NonSerialized]
	public object passwordSource;

	// Token: 0x04001BDC RID: 7132
	[NonSerialized]
	public GameObject spawnedObject;

	// Token: 0x04001BDD RID: 7133
	[NonSerialized]
	public NewNode node;

	// Token: 0x04001BDE RID: 7134
	[NonSerialized]
	public NewNode spawnNode;

	// Token: 0x04001BDF RID: 7135
	[NonSerialized]
	public NewRoom worldObjectRoomParent;

	// Token: 0x04001BE0 RID: 7136
	[NonSerialized]
	public Interactable.UsagePoint usagePoint;

	// Token: 0x04001BE1 RID: 7137
	[NonSerialized]
	public NewAIAction nextAIInteraction;

	// Token: 0x04001BE2 RID: 7138
	[NonSerialized]
	public LightingPreset isLight;

	// Token: 0x04001BE3 RID: 7139
	[NonSerialized]
	public object objectRef;

	// Token: 0x04001BE4 RID: 7140
	[NonSerialized]
	public Human writer;

	// Token: 0x04001BE5 RID: 7141
	[NonSerialized]
	public Human reciever;

	// Token: 0x04001BE6 RID: 7142
	[NonSerialized]
	public Human belongsTo;

	// Token: 0x04001BE7 RID: 7143
	[NonSerialized]
	public Actor isActor;

	// Token: 0x04001BE8 RID: 7144
	[NonSerialized]
	public BookPreset book;

	// Token: 0x04001BE9 RID: 7145
	[NonSerialized]
	public SyncDiskPreset syncDisk;

	// Token: 0x04001BEA RID: 7146
	[NonSerialized]
	public GroupsController.SocialGroup group;

	// Token: 0x04001BEB RID: 7147
	[NonSerialized]
	public float recentCallCheck;

	// Token: 0x04001BEC RID: 7148
	[NonSerialized]
	private Transform ceilingFan;

	// Token: 0x04001BED RID: 7149
	[NonSerialized]
	public NewAddress forSale;

	// Token: 0x04001BEE RID: 7150
	[NonSerialized]
	public List<Human> proxy;

	// Token: 0x04001BEF RID: 7151
	[NonSerialized]
	public List<SpatterSimulation.DecalSpawnData> spawnedDecals;

	// Token: 0x04001BF0 RID: 7152
	[NonSerialized]
	public AudioController.LoopingSoundInfo actionLoop;

	// Token: 0x04001BF1 RID: 7153
	[NonSerialized]
	public bool loadedGeometry;

	// Token: 0x04001BF2 RID: 7154
	[NonSerialized]
	public Dictionary<InteractablePreset.InteractionKey, Interactable.InteractableCurrentAction> currentActions = new Dictionary<InteractablePreset.InteractionKey, Interactable.InteractableCurrentAction>();

	// Token: 0x04001BF3 RID: 7155
	[NonSerialized]
	public List<InteractablePreset.InteractionAction> highlightActions = new List<InteractablePreset.InteractionAction>();

	// Token: 0x04001BF4 RID: 7156
	[NonSerialized]
	public List<InteractablePreset.InteractionAction> disabledActions = new List<InteractablePreset.InteractionAction>();

	// Token: 0x04001BF5 RID: 7157
	[NonSerialized]
	public Dictionary<AIActionPreset, InteractablePreset.InteractionAction> aiActionReference = new Dictionary<AIActionPreset, InteractablePreset.InteractionAction>();

	// Token: 0x04001BF6 RID: 7158
	[NonSerialized]
	public float readingDelay;

	// Token: 0x04001BF7 RID: 7159
	[NonSerialized]
	public Dictionary<AIActionPreset, AudioEvent> actionAudioEventOverrides = new Dictionary<AIActionPreset, AudioEvent>();

	// Token: 0x04001BF8 RID: 7160
	[NonSerialized]
	public List<AudioController.LoopingSoundInfo> loopingAudio = new List<AudioController.LoopingSoundInfo>();

	// Token: 0x04001BF9 RID: 7161
	[NonSerialized]
	private bool isSetup;

	// Token: 0x04001BFA RID: 7162
	[NonSerialized]
	public bool wasLoadedFromSave;

	// Token: 0x04001BFB RID: 7163
	[NonSerialized]
	public bool printDebug;

	// Token: 0x020003E8 RID: 1000
	[Serializable]
	public class LightConfiguration
	{
		// Token: 0x04001C00 RID: 7168
		public Color colour;

		// Token: 0x04001C01 RID: 7169
		public float intensity;

		// Token: 0x04001C02 RID: 7170
		public float flickerColourMultiplier;

		// Token: 0x04001C03 RID: 7171
		public float pulseSpeed;

		// Token: 0x04001C04 RID: 7172
		public float intervalTime;

		// Token: 0x04001C05 RID: 7173
		public bool flicker;

		// Token: 0x04001C06 RID: 7174
		public float range;
	}

	// Token: 0x020003E9 RID: 1001
	[Serializable]
	public class SavedPrint
	{
		// Token: 0x04001C07 RID: 7175
		public Vector3 worldPos;

		// Token: 0x04001C08 RID: 7176
		public int interactableID;
	}

	// Token: 0x020003EA RID: 1002
	[Serializable]
	public class DynamicFingerprint
	{
		// Token: 0x04001C09 RID: 7177
		public int id;

		// Token: 0x04001C0A RID: 7178
		public float created;

		// Token: 0x04001C0B RID: 7179
		public string seed;

		// Token: 0x04001C0C RID: 7180
		public Interactable.PrintLife life;
	}

	// Token: 0x020003EB RID: 1003
	public enum PrintLife
	{
		// Token: 0x04001C0E RID: 7182
		timed,
		// Token: 0x04001C0F RID: 7183
		manualRemoval
	}

	// Token: 0x020003EC RID: 1004
	public class InteractableCurrentAction
	{
		// Token: 0x04001C10 RID: 7184
		public InteractablePreset.InteractionAction currentAction;

		// Token: 0x04001C11 RID: 7185
		public bool display;

		// Token: 0x04001C12 RID: 7186
		public bool enabled;

		// Token: 0x04001C13 RID: 7187
		public string overrideInteractionName;

		// Token: 0x04001C14 RID: 7188
		public bool forcePositioning;

		// Token: 0x04001C15 RID: 7189
		public ControlDisplayController.ControlPositioning forcePosition;

		// Token: 0x04001C16 RID: 7190
		public bool highlight;
	}

	// Token: 0x020003ED RID: 1005
	[Serializable]
	public class UsagePoint
	{
		// Token: 0x06001702 RID: 5890 RVA: 0x0015C9B0 File Offset: 0x0015ABB0
		public UsagePoint(InteractablePreset.AIUseSetting newPreset, Interactable newInteractable, NewNode newNode)
		{
			this.useSetting = newPreset;
			this.interactable = newInteractable;
			this.node = newNode;
			this.PositionUpdate();
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0015C9EC File Offset: 0x0015ABEC
		public void PositionUpdate()
		{
			if (this.interactable.controller != null)
			{
				this.useageWorldPosition = this.interactable.controller.transform.TransformPoint(this.useSetting.usageOffset);
				this.worldLookAtPoint = this.interactable.controller.transform.TransformPoint(this.useSetting.facingOffset);
			}
			else
			{
				Vector3 vector = Vector3.one;
				if (this.interactable.preset.prefab != null)
				{
					vector = this.interactable.preset.prefabLocalScale;
				}
				Matrix4x4 matrix4x = Matrix4x4.TRS(this.interactable.wPos, Quaternion.Euler(this.interactable.wEuler), vector);
				this.useageWorldPosition = matrix4x.MultiplyPoint3x4(this.useSetting.usageOffset);
				this.worldLookAtPoint = matrix4x.MultiplyPoint3x4(this.useSetting.facingOffset);
			}
			if (this.interactable.isActor != null && this.interactable.isActor.isPlayer && Player.Instance.currentNode != null && Player.Instance.currentNode.walkableNodeSpace.Count <= 0)
			{
				NewNode.NodeSpace nodeSpace = null;
				float num = 999999f;
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector2 vector2 = offsetArrayX[i];
					Vector3 vector3 = Player.Instance.currentNode.nodeCoord + new Vector3Int((int)vector2.x, (int)vector2.y, 0);
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3, ref newNode) && newNode.walkableNodeSpace.Count > 0 && newNode.room == Player.Instance.currentRoom)
					{
						foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in newNode.walkableNodeSpace)
						{
							float num2 = Vector3.Distance(this.useageWorldPosition, keyValuePair.Key);
							if (num2 < num)
							{
								nodeSpace = keyValuePair.Value;
								num = num2;
							}
						}
					}
				}
				if (nodeSpace != null)
				{
					this.useageWorldPosition = nodeSpace.position;
				}
			}
			if (this.useSetting.useNodeFloorPosition && this.interactable.node != null)
			{
				this.useageWorldPosition.y = this.interactable.node.position.y;
			}
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x0015CC90 File Offset: 0x0015AE90
		public Vector3 GetUsageWorldPosition(Vector3 userPos, Actor actor)
		{
			if (this.interactable.isActor != null || (this.interactable.mov && this.interactable.wo))
			{
				this.PositionUpdate();
			}
			Vector3 positionWithInvertedZ = this.useageWorldPosition;
			if (this.useSetting.useDoorBehaviour && actor != null)
			{
				NewDoor newDoor = this.interactable.objectRef as NewDoor;
				if (newDoor != null && actor.ai != null && actor.ai.currentAction != null && actor.ai.currentAction.forcedNode == null)
				{
					userPos = Toolbox.Instance.GetDoorSideNode(actor.currentNode, newDoor).position;
				}
				Vector3 zero = Vector3.zero;
				Vector3 vector = Vector3.one;
				if (this.interactable.preset.prefab != null)
				{
					vector = this.interactable.preset.prefab.transform.localScale;
				}
				if (this.interactable.furnitureParent != null)
				{
					Vector3 localScale = this.interactable.furnitureParent.furniture.prefab.transform.localScale;
					vector..ctor(this.interactable.furnitureParent.scaleMultiplier.x * localScale.x, this.interactable.furnitureParent.scaleMultiplier.y * localScale.y, this.interactable.furnitureParent.scaleMultiplier.z * localScale.z);
				}
				if (Matrix4x4.TRS(this.interactable.wPos, Quaternion.Euler(this.interactable.wEuler), vector).inverse.MultiplyPoint3x4(userPos).z < 0f)
				{
					positionWithInvertedZ = this.GetPositionWithInvertedZ();
				}
			}
			if (this.useSetting.useSittingOffset && actor != null && actor.modelParent != null)
			{
				float num = CitizenControls.Instance.sittingYOffset * actor.modelParent.transform.localScale.y - CitizenControls.Instance.sittingYOffset;
				positionWithInvertedZ.y -= num;
			}
			if (this.useSetting.useArmsStandingOffset && actor != null && actor.modelParent != null)
			{
				float num2 = CitizenControls.Instance.armsStandingYOffset * actor.modelParent.transform.localScale.y - CitizenControls.Instance.armsStandingYOffset;
				positionWithInvertedZ.y -= num2;
			}
			return positionWithInvertedZ;
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x0015CF20 File Offset: 0x0015B120
		private Vector3 GetPositionWithInvertedZ()
		{
			Vector3 usageOffset = this.useSetting.usageOffset;
			usageOffset.z *= -1f;
			Vector3 result = Vector3.zero;
			if (this.interactable.controller != null)
			{
				result = this.interactable.controller.transform.TransformPoint(usageOffset);
			}
			else
			{
				Vector3 vector = Vector3.one;
				if (this.interactable.preset.prefab != null)
				{
					vector = this.interactable.preset.prefab.transform.localScale;
				}
				if (this.interactable.furnitureParent != null)
				{
					Vector3 localScale = this.interactable.furnitureParent.furniture.prefab.transform.localScale;
					vector..ctor(this.interactable.furnitureParent.scaleMultiplier.x * localScale.x, this.interactable.furnitureParent.scaleMultiplier.y * localScale.y, this.interactable.furnitureParent.scaleMultiplier.z * localScale.z);
				}
				result = Matrix4x4.TRS(this.interactable.wPos, Quaternion.Euler(this.interactable.wEuler), vector).MultiplyPoint3x4(usageOffset);
			}
			if (this.useSetting.useNodeFloorPosition && this.interactable.node != null)
			{
				result.y = this.interactable.node.position.y;
			}
			return result;
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x0015D0A8 File Offset: 0x0015B2A8
		public bool TrySetUser(Interactable.UsePointSlot slot, Human newUser, string debug = "")
		{
			Human human = null;
			this.TryGetUserAtSlot(slot, out human);
			if (human != null && newUser != null && human != newUser)
			{
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					if (this.slotLog.Count > 50)
					{
						this.slotLog.RemoveAt(0);
					}
					this.slotLog.Add(string.Concat(new string[]
					{
						"Unable to set slot ",
						slot.ToString(),
						" to ",
						newUser.GetCitizenName(),
						": ",
						debug
					}));
				}
				return false;
			}
			if (!this.users.ContainsKey(slot))
			{
				this.users.Add(slot, newUser);
			}
			else
			{
				this.users[slot] = newUser;
			}
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				if (this.slotLog.Count > 50)
				{
					this.slotLog.RemoveAt(0);
				}
				if (newUser != null)
				{
					this.slotLog.Add(string.Concat(new string[]
					{
						"Setting slot ",
						slot.ToString(),
						" to ",
						newUser.GetCitizenName(),
						": ",
						debug
					}));
				}
				else
				{
					this.slotLog.Add("Setting slot " + slot.ToString() + " to empty: " + debug);
				}
				if (slot == Interactable.UsePointSlot.defaultSlot)
				{
					this.debugDefaultSlot = newUser;
				}
				else if (slot == Interactable.UsePointSlot.slot1)
				{
					this.debugSlot1 = newUser;
				}
				else if (slot == Interactable.UsePointSlot.slot2)
				{
					this.debugSlot2 = newUser;
				}
			}
			return true;
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0015D26C File Offset: 0x0015B46C
		public void SetReserved(GroupsController.SocialGroup group)
		{
			if (this.reserved != null && this.reserved != group)
			{
				if (this.reserved.reserved == null)
				{
					this.reserved.reserved = new List<Interactable>();
				}
				foreach (Interactable interactable in this.reserved.reserved)
				{
					interactable.usagePoint.reserved = null;
				}
				if (this.reserved != null && this.reserved.reserved != null)
				{
					this.reserved.reserved.Clear();
				}
			}
			this.reserved = group;
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0015D328 File Offset: 0x0015B528
		public bool TryGetUserAtSlot(Interactable.UsePointSlot slot, out Human user)
		{
			user = null;
			return this.users.TryGetValue(slot, ref user) && !(user == null);
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0015D34C File Offset: 0x0015B54C
		public void RemoveUserFromAllSlots(Human user)
		{
			foreach (Interactable.UsePointSlot usePointSlot in new List<Interactable.UsePointSlot>(this.users.Keys))
			{
				if (this.users.ContainsKey(usePointSlot) && this.users[usePointSlot] == user)
				{
					this.users[usePointSlot] = null;
				}
			}
		}

		// Token: 0x04001C17 RID: 7191
		public InteractablePreset.AIUseSetting useSetting;

		// Token: 0x04001C18 RID: 7192
		[NonSerialized]
		public Interactable interactable;

		// Token: 0x04001C19 RID: 7193
		public NewNode node;

		// Token: 0x04001C1A RID: 7194
		public Dictionary<Interactable.UsePointSlot, Human> users = new Dictionary<Interactable.UsePointSlot, Human>();

		// Token: 0x04001C1B RID: 7195
		[NonSerialized]
		public GroupsController.SocialGroup reserved;

		// Token: 0x04001C1C RID: 7196
		[SerializeField]
		private Vector3 useageWorldPosition;

		// Token: 0x04001C1D RID: 7197
		public Vector3 worldLookAtPoint;

		// Token: 0x04001C1E RID: 7198
		public Human debugDefaultSlot;

		// Token: 0x04001C1F RID: 7199
		public Human debugSlot1;

		// Token: 0x04001C20 RID: 7200
		public Human debugSlot2;

		// Token: 0x04001C21 RID: 7201
		public List<string> slotLog = new List<string>();
	}

	// Token: 0x020003EE RID: 1006
	public enum UsePointSlot
	{
		// Token: 0x04001C23 RID: 7203
		defaultSlot,
		// Token: 0x04001C24 RID: 7204
		slot1,
		// Token: 0x04001C25 RID: 7205
		slot2
	}

	// Token: 0x020003EF RID: 1007
	public enum PassedVarType
	{
		// Token: 0x04001C27 RID: 7207
		jobID,
		// Token: 0x04001C28 RID: 7208
		humanID,
		// Token: 0x04001C29 RID: 7209
		noteID,
		// Token: 0x04001C2A RID: 7210
		roomID,
		// Token: 0x04001C2B RID: 7211
		addressID,
		// Token: 0x04001C2C RID: 7212
		time,
		// Token: 0x04001C2D RID: 7213
		savedSceneCapID,
		// Token: 0x04001C2E RID: 7214
		menuIndex,
		// Token: 0x04001C2F RID: 7215
		vmailThreadID,
		// Token: 0x04001C30 RID: 7216
		consumableAmount,
		// Token: 0x04001C31 RID: 7217
		companyID,
		// Token: 0x04001C32 RID: 7218
		stringInteractablePreset,
		// Token: 0x04001C33 RID: 7219
		isTrash,
		// Token: 0x04001C34 RID: 7220
		jobTag,
		// Token: 0x04001C35 RID: 7221
		groupID,
		// Token: 0x04001C36 RID: 7222
		ddsOverride,
		// Token: 0x04001C37 RID: 7223
		metaObjectID,
		// Token: 0x04001C38 RID: 7224
		murderID,
		// Token: 0x04001C39 RID: 7225
		decal,
		// Token: 0x04001C3A RID: 7226
		decalDynamicText,
		// Token: 0x04001C3B RID: 7227
		ownedByAddress,
		// Token: 0x04001C3C RID: 7228
		vmailThreadMsgIndex,
		// Token: 0x04001C3D RID: 7229
		phoneNumber,
		// Token: 0x04001C3E RID: 7230
		lostItemPreset,
		// Token: 0x04001C3F RID: 7231
		lostItemBuilding,
		// Token: 0x04001C40 RID: 7232
		lostItemReward,
		// Token: 0x04001C41 RID: 7233
		lostItemFloorX,
		// Token: 0x04001C42 RID: 7234
		lostItemFloorY,
		// Token: 0x04001C43 RID: 7235
		creationTime
	}

	// Token: 0x020003F0 RID: 1008
	[Serializable]
	public class Passed
	{
		// Token: 0x0600170A RID: 5898 RVA: 0x0015D3D4 File Offset: 0x0015B5D4
		public Passed(Interactable.PassedVarType newType, float newVal, string newStr = null)
		{
			this.varType = newType;
			this.value = newVal;
			this.str = newStr;
		}

		// Token: 0x04001C44 RID: 7236
		public Interactable.PassedVarType varType;

		// Token: 0x04001C45 RID: 7237
		public float value = -1f;

		// Token: 0x04001C46 RID: 7238
		public string str;
	}

	// Token: 0x020003F1 RID: 1009
	// (Invoke) Token: 0x0600170C RID: 5900
	public delegate void SwitchChange();

	// Token: 0x020003F2 RID: 1010
	// (Invoke) Token: 0x06001710 RID: 5904
	public delegate void State1Change();

	// Token: 0x020003F3 RID: 1011
	// (Invoke) Token: 0x06001714 RID: 5908
	public delegate void Deleted(Interactable destroyed);

	// Token: 0x020003F4 RID: 1012
	// (Invoke) Token: 0x06001718 RID: 5912
	public delegate void RemovedFromWorld();
}
