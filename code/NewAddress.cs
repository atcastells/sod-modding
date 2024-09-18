using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000337 RID: 823
public class NewAddress : NewGameLocation
{
	// Token: 0x06001281 RID: 4737 RVA: 0x001062B4 File Offset: 0x001044B4
	public void Setup(NewFloor newFloor, LayoutConfiguration newType, DesignStylePreset newDefaultStyle)
	{
		newFloor.AddNewAddress(this);
		base.transform.SetParent(this.floor.gameObject.transform);
		base.transform.localPosition = Vector3.zero;
		this.id = NewAddress.assignID;
		NewAddress.assignID++;
		this.editorID = NewAddress.assignEditorID;
		NewAddress.assignEditorID++;
		this.preset = newType;
		if (this.preset.isOutside)
		{
			this.isOutside = true;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			this.editorColour = FloorEditController.Instance.editorAddressColours[this.editorID % FloorEditController.Instance.editorAddressColours.Count];
		}
		this.SetName();
		this.passcode = new GameplayController.Passcode(GameplayController.PasscodeType.address);
		this.passcode.id = this.id;
		if (!CityData.Instance.addressDictionary.ContainsKey(this.id))
		{
			CityData.Instance.addressDirectory.Add(this);
			CityData.Instance.addressDictionary.Add(this.id, this);
		}
		this.CreateEvidence();
		if (this.preset.roomLayout != null && this.preset.isLobby)
		{
			this.isLobby = true;
			this.building.AddLobby(this);
		}
		if (SessionData.Instance.isFloorEdit)
		{
			base.CommonSetup(false, null, newDefaultStyle);
			return;
		}
		base.CommonSetup(false, this.building.cityTile.district, newDefaultStyle);
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00106434 File Offset: 0x00104634
	public void GenerateRoomConfigs()
	{
		if (this.addressPreset != null)
		{
			foreach (NewRoom newRoom in this.rooms)
			{
				if (newRoom.preset != null && !newRoom.isNullRoom && (!this.preset.requiresHallway || this.preset.hallway != newRoom.preset))
				{
					newRoom.preset = null;
				}
				newRoom.openPlanElements.Clear();
			}
			using (List<RoomConfiguration>.Enumerator enumerator2 = this.addressPreset.roomConfig.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					RoomConfiguration cf = enumerator2.Current;
					NewRoom newRoom2 = this.rooms.Find((NewRoom item) => item.preset == null && item.roomType == cf.roomType);
					if (newRoom2 != null)
					{
						newRoom2.SetConfiguration(cf);
					}
					else
					{
						if (SessionData.Instance.isFloorEdit)
						{
							Game.Log(string.Concat(new string[]
							{
								"Unable to find an appropriate room for ",
								cf.name,
								" in ",
								base.name,
								" (address preset: ",
								this.addressPreset.presetName,
								")"
							}), 2);
						}
						if (cf.canBeOpenPlan && cf.openPlanRoom != null)
						{
							NewRoom newRoom3 = this.rooms.Find((NewRoom item) => item.roomType == cf.openPlanRoom);
							if (newRoom3 != null)
							{
								if (cf == null)
								{
									Game.LogError("Found a null reference when adding to open plan...", 2);
								}
								else
								{
									newRoom3.AddOpenPlanElement(cf);
								}
							}
						}
					}
				}
			}
			using (List<NewRoom>.Enumerator enumerator = this.rooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewRoom room = enumerator.Current;
					if (room.preset == null)
					{
						RoomConfiguration roomConfiguration = this.addressPreset.roomConfig.Find((RoomConfiguration item) => item.roomType == room.roomType);
						if (roomConfiguration != null)
						{
							room.SetConfiguration(roomConfiguration);
						}
						else if (room.roomType.forceConfiguration != null)
						{
							room.SetConfiguration(room.roomType.forceConfiguration);
						}
						else
						{
							string[] array = new string[5];
							array[0] = "Unable to find room for ";
							int num = 1;
							RoomTypePreset roomType = room.roomType;
							array[num] = ((roomType != null) ? roomType.ToString() : null);
							array[2] = " at address ";
							array[3] = this.id.ToString();
							array[4] = " setting as null...";
							Game.Log(string.Concat(array), 2);
							room.SetConfiguration(InteriorControls.Instance.nullConfig);
						}
					}
				}
			}
			this.generatedRoomConfigs = true;
			return;
		}
		foreach (NewRoom newRoom4 in this.rooms)
		{
			if (newRoom4.preset == null)
			{
				if (newRoom4.roomType.forceConfiguration != null)
				{
					newRoom4.SetConfiguration(newRoom4.roomType.forceConfiguration);
				}
				else
				{
					string[] array2 = new string[5];
					array2[0] = "Unable to find room for ";
					int num2 = 1;
					RoomTypePreset roomType2 = newRoom4.roomType;
					array2[num2] = ((roomType2 != null) ? roomType2.ToString() : null);
					array2[2] = " at address ";
					array2[3] = this.id.ToString();
					array2[4] = " setting as null...";
					Game.Log(string.Concat(array2), 2);
					newRoom4.SetConfiguration(InteriorControls.Instance.nullConfig);
				}
			}
		}
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00106888 File Offset: 0x00104A88
	public void CalculateLandValue()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			this.normalizedLandValue = (float)Toolbox.Instance.Rand(0, 5, false);
		}
		else
		{
			this.normalizedLandValue = (float)this.building.cityTile.landValue / 4f;
			this.normalizedLandValue += Mathf.Min((float)this.floor.floor / 4f, 5f);
			foreach (NewRoom newRoom in this.rooms)
			{
				this.normalizedLandValue += (float)newRoom.nodes.Count * 0.05f;
			}
			this.normalizedLandValue = Mathf.Clamp01(this.normalizedLandValue / 12f);
		}
		if (this.addressPreset != null)
		{
			this.normalizedLandValue = Mathf.Clamp(this.normalizedLandValue, this.addressPreset.minimumLandValue, this.addressPreset.maximumLandValue);
		}
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x001069AC File Offset: 0x00104BAC
	public void SetTargetMode(NewBuilding.AlarmTargetMode newMode, bool setResetTimer = true)
	{
		this.targetMode = newMode;
		Game.Log("Set target mode for " + base.name + ": " + this.targetMode.ToString(), 2);
		if (setResetTimer)
		{
			this.targetModeSetAt = SessionData.Instance.gameTime;
			if (this.targetMode != NewBuilding.AlarmTargetMode.illegalActivities && !GameplayController.Instance.alteredSecurityTargetsLocations.Contains(this))
			{
				GameplayController.Instance.alteredSecurityTargetsLocations.Add(this);
			}
		}
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00106A2C File Offset: 0x00104C2C
	public void Load(CitySaveData.AddressCitySave data, NewFloor newFloor)
	{
		this.id = data.id;
		NewAddress.assignID = Mathf.Max(NewAddress.assignID, this.id + 1);
		this.generatedRoomConfigs = true;
		base.name = data.name;
		base.transform.name = base.name;
		this.residenceNumber = data.residenceNumber;
		this.isLobby = data.isLobby;
		this.isOutside = data.isOutside;
		this.access = data.access;
		this.wood = data.wood;
		this.isLobbyAddress = data.isLobbyAddress;
		this.isOutsideAddress = data.isOutsideAddress;
		this.featuresNeonSignageHorizontal = data.neonHor;
		this.featuresNeonSignageVertical = data.neonVer;
		this.neonVerticalIndex = data.neonVerticalIndex;
		this.neonColour = data.neonColour;
		this.normalizedLandValue = data.landValue;
		this.passcode = data.passcode;
		this.hiddenSpareKey = data.hkey;
		this.breakerDoorsID = data.breakerDoors;
		this.breakerLightsID = data.breakerLights;
		this.breakerSecurityID = data.breakerSec;
		newFloor.AddNewAddress(this);
		base.transform.SetParent(this.floor.gameObject.transform);
		base.transform.localPosition = Vector3.zero;
		if (this.isLobbyAddress)
		{
			newFloor.lobbyAddress = this;
		}
		if (this.isOutsideAddress)
		{
			newFloor.outsideAddress = this;
		}
		if (data.neonFont != null && data.neonFont.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<NeonSignCharacters>(data.neonFont, out this.neonFont);
		}
		Toolbox.Instance.LoadDataFromResources<LayoutConfiguration>(data.preset, out this.preset);
		Toolbox.Instance.LoadDataFromResources<DesignStylePreset>(data.designStyle, out this.designStyle);
		if (data.address != null && data.address.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<AddressPreset>(data.address, out this.addressPreset);
			if (!CityData.Instance.addressTypeReference.ContainsKey(this.addressPreset))
			{
				CityData.Instance.addressTypeReference.Add(this.addressPreset, new List<NewAddress>());
			}
			CityData.Instance.addressTypeReference[this.addressPreset].Add(this);
		}
		if (!CityData.Instance.addressDictionary.ContainsKey(this.id))
		{
			CityData.Instance.addressDirectory.Add(this);
			CityData.Instance.addressDictionary.Add(this.id, this);
		}
		this.CreateEvidence();
		if (this.preset.roomLayout != null && this.preset.isLobby)
		{
			this.isLobby = true;
			this.building.AddLobby(this);
		}
		foreach (Vector3 vector in data.protectedNodes)
		{
			this.protectedNodes.Add(PathFinder.Instance.nodeMap[vector]);
		}
		base.CommonSetup(false, this.building.cityTile.district, this.designStyle);
		if (data.residence != null && data.residence.preset != null && data.residence.preset.Length > 0)
		{
			this.residence = base.gameObject.AddComponent<ResidenceController>();
			this.residence.Load(data.residence, this);
		}
		if (data.company != null && data.company.preset != null && data.company.preset.Length > 0)
		{
			this.company = new Company();
			this.company.Load(data.company, this);
		}
		int num = 0;
		using (List<CitySaveData.RoomCitySave>.Enumerator enumerator2 = data.rooms.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				CitySaveData.RoomCitySave room = enumerator2.Current;
				NewRoom newRoom;
				if (!room.isBaseNullRoom)
				{
					newRoom = Object.Instantiate<GameObject>(PrefabControls.Instance.room, base.transform).GetComponent<NewRoom>();
				}
				else
				{
					newRoom = this.nullRoom;
				}
				CitySaveData.RoomCitySave data2 = room;
				if (CityConstructor.Instance != null && CityConstructor.Instance.saveState != null)
				{
					StateSaveData.RoomStateSave roomStateSave = CityConstructor.Instance.saveState.rooms.Find((StateSaveData.RoomStateSave item) => item.id == room.id && item.decorOverride != null && item.decorOverride.Count > 0);
					if (roomStateSave != null)
					{
						if (Game.Instance.printDebug)
						{
							Game.Log(string.Concat(new string[]
							{
								"CityGen: Found save state room decor override for: ",
								room.name,
								" with ",
								roomStateSave.decorOverride[0].f.Count.ToString(),
								" furniture clusters..."
							}), 2);
						}
						data2 = roomStateSave.decorOverride[0];
						newRoom.decorEdit = true;
					}
				}
				newRoom.Load(data2, this);
				num += newRoom.furniture.Count;
			}
		}
		if (num <= 0 && this.residence != null)
		{
			Game.Log(base.name + " residential address has no furniture loaded...", 2);
			int num2 = 0;
			int num3 = 0;
			foreach (CitySaveData.RoomCitySave roomCitySave in data.rooms)
			{
				foreach (CitySaveData.FurnitureClusterCitySave furnitureClusterCitySave in roomCitySave.f)
				{
					num2++;
					num3 += furnitureClusterCitySave.objs.Count;
				}
			}
			Game.Log(string.Concat(new string[]
			{
				"... The save files contains ",
				num2.ToString(),
				" and ",
				num3.ToString(),
				" objects"
			}), 2);
		}
		if (this.residence != null && data.residence.mail > 0 && !CityConstructor.Instance.loadingFurnitureReference.TryGetValue(data.residence.mail, ref this.residence.mailbox))
		{
			Game.LogError("Cannot find mailbox furniture " + data.residence.mail.ToString(), 2);
		}
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x001070E8 File Offset: 0x001052E8
	public void AssignPurpose()
	{
		if (this.preset.assignPurpose)
		{
			List<NewAddress.AddressCalc> list = new List<NewAddress.AddressCalc>();
			List<AddressPreset> list2 = new List<AddressPreset>();
			AddressPreset addressPreset = null;
			string seed = this.seed;
			foreach (AddressPreset addressPreset2 in Toolbox.Instance.allAddressPresets)
			{
				if (!addressPreset2.disableThis && addressPreset2.compatible.Contains(this.preset) && (this.floor == null || ((float)this.floor.floor >= addressPreset2.minMaxFloors.x && (float)this.floor.floor <= addressPreset2.minMaxFloors.y)))
				{
					if (addressPreset2.debug)
					{
						Game.Log(string.Concat(new string[]
						{
							"Testing ",
							addressPreset2.name,
							" for ",
							this.id.ToString(),
							"..."
						}), 2);
					}
					if (SessionData.Instance.isFloorEdit)
					{
						list2.Add(addressPreset2);
					}
					else if (addressPreset2.limitToBuildings.Count <= 0 || addressPreset2.limitToBuildings.Contains(this.building.preset))
					{
						int num = 0;
						float num2 = (float)addressPreset2.baseScore + Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 0.1f, seed, out seed);
						if (this.nodes.Count >= addressPreset2.fitsUnitSizeMin && this.nodes.Count <= addressPreset2.fitsUnitSizeMax)
						{
							if (addressPreset2.debug)
							{
								Game.Log(string.Concat(new string[]
								{
									addressPreset2.name,
									" fits within unit size: ",
									this.nodes.Count.ToString(),
									" at ",
									this.id.ToString()
								}), 2);
							}
							num2 += 5f;
						}
						else if (addressPreset2.hardSizeLimits)
						{
							if (addressPreset2.debug)
							{
								Game.Log(string.Concat(new string[]
								{
									addressPreset2.name,
									" does not within unit size: ",
									this.nodes.Count.ToString(),
									" at ",
									this.id.ToString()
								}), 2);
								continue;
							}
							continue;
						}
						if (CityData.Instance.addressTypeReference.ContainsKey(addressPreset2))
						{
							num = CityData.Instance.addressTypeReference[addressPreset2].Count;
							if (addressPreset2.important && num <= 0)
							{
								list2.Add(addressPreset2);
								continue;
							}
							if (num >= addressPreset2.maxInstances)
							{
								continue;
							}
						}
						else if (addressPreset2.important)
						{
							list2.Add(addressPreset2);
						}
						num2 -= (float)(num * addressPreset2.baseScoreFrequencyPenalty);
						if (this.streetAccess != null)
						{
							StreetController thisAsStreet = this.streetAccess.fromNode.gameLocation.thisAsStreet;
							if (thisAsStreet == null)
							{
								thisAsStreet = this.streetAccess.toNode.gameLocation.thisAsStreet;
							}
							float num3 = 1f - Mathf.Abs(thisAsStreet.normalizedFootfall - addressPreset2.idealFootfall);
							num2 += num3 * addressPreset2.footfallMultiplier;
						}
						foreach (AddressPreset.AddressRule addressRule in addressPreset2.addressRules)
						{
							if (this.district.preset == addressRule.districtPreset)
							{
								num2 += (float)addressRule.scoreModifier;
							}
						}
						if (addressPreset2.forcePick)
						{
							addressPreset = addressPreset2;
							break;
						}
						list.Add(new NewAddress.AddressCalc
						{
							preset = addressPreset2,
							score = num2
						});
					}
				}
			}
			if (list.Count <= 0 && list2.Count <= 0 && addressPreset == null)
			{
				if (!this.isLobby)
				{
					Game.LogError(string.Concat(new string[]
					{
						"No address preset shortlist for ",
						this.preset.name,
						" nodes count = ",
						this.nodes.Count.ToString(),
						" @ ",
						this.building.name,
						" floor ",
						this.floor.floor.ToString()
					}), 2);
					return;
				}
				if (this.building != null && this.building.preset.lobbyPreset != null)
				{
					this.addressPreset = this.building.preset.lobbyPreset;
					return;
				}
				this.addressPreset = InteriorControls.Instance.lobbyAddressPreset;
				return;
			}
			else if (addressPreset != null)
			{
				if (addressPreset.debug)
				{
					Game.Log("Force picked " + addressPreset.name + " for " + this.id.ToString(), 2);
				}
				this.addressPreset = addressPreset;
			}
			else if (list2.Count > 0)
			{
				this.addressPreset = list2[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list2.Count, seed, out seed)];
			}
			else if (list.Count > 0)
			{
				list.Sort((NewAddress.AddressCalc p2, NewAddress.AddressCalc p1) => p1.score.CompareTo(p2.score));
				this.addressPreset = list[0].preset;
			}
		}
		else if (this.preset.addressPreset != null)
		{
			this.addressPreset = this.preset.addressPreset;
		}
		if (this.addressPreset == null)
		{
			if (SessionData.Instance.isFloorEdit)
			{
				Game.Log("No address preset for " + base.name, 2);
			}
			return;
		}
		this.access = this.addressPreset.access;
		if (!CityData.Instance.addressTypeReference.ContainsKey(this.addressPreset))
		{
			CityData.Instance.addressTypeReference.Add(this.addressPreset, new List<NewAddress>());
		}
		CityData.Instance.addressTypeReference[this.addressPreset].Add(this);
		if (this.addressPreset.company != null)
		{
			this.company = new Company();
			this.company.Setup(this.addressPreset.company, this);
		}
		if (this.addressPreset.residence != null)
		{
			this.residenceNumber = this.floor.assignResidence;
			this.floor.assignResidence++;
			this.residence = base.gameObject.AddComponent<ResidenceController>();
			this.residence.Setup(this.addressPreset.residence, this);
		}
		if (!this.generatedRoomConfigs)
		{
			this.GenerateRoomConfigs();
		}
		if (!SessionData.Instance.isFloorEdit)
		{
			foreach (NewRoom newRoom in this.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (NewWall newWall in newNode.walls)
					{
						newWall.SetDoorPairPreset(newWall.preset, true, false, true);
					}
				}
			}
		}
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x001078E8 File Offset: 0x00105AE8
	public void SetupNeonSigns()
	{
		if (this.addressPreset != null)
		{
			string seed = this.seed;
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seed, out seed) <= this.addressPreset.chanceOfNameSignHorizontal && this.addressPreset.signCharacterSet.Count > 0)
			{
				this.featuresNeonSignageHorizontal = true;
				this.neonColour = this.GetNeon();
				this.neonFont = this.addressPreset.signCharacterSet[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.addressPreset.signCharacterSet.Count, seed, out seed)];
			}
			if (Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seed, out seed) <= this.addressPreset.chanceOfNameSignVertical && this.addressPreset.possibleSigns.Count > 0)
			{
				this.featuresNeonSignageVertical = true;
				if (!this.featuresNeonSignageHorizontal)
				{
					this.neonColour = this.GetNeon();
				}
				this.neonVerticalIndex = Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.addressPreset.possibleSigns.Count, seed, out seed);
			}
		}
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x00107A04 File Offset: 0x00105C04
	private int GetNeon()
	{
		List<int> list = new List<int>();
		if (this.company != null)
		{
			foreach (string text in this.company.nameAltTags)
			{
				for (int i = 0; i < CityControls.Instance.neonColours.Count; i++)
				{
					if (CityControls.Instance.neonColours[i].colourTag == text)
					{
						list.Add(i);
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			for (int j = 0; j < CityControls.Instance.neonColours.Count; j++)
			{
				list.Add(j);
			}
		}
		return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, this.seed, false)];
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x00107AF0 File Offset: 0x00105CF0
	public void AddOwner(Human newOwner)
	{
		if (!this.owners.Contains(newOwner))
		{
			this.owners.Add(newOwner);
			this.averageHumility = 0f;
			this.averageEmotionality = 0f;
			this.averageExtraversion = 0f;
			this.averageAgreeableness = 0f;
			this.averageConscientiousness = 0f;
			this.averageCreativity = 0f;
			this.maxConscientiousness = 0f;
			foreach (Human human in this.owners)
			{
				this.averageHumility += human.humility;
				this.averageEmotionality += human.emotionality;
				this.averageExtraversion += human.extraversion;
				this.averageAgreeableness += human.agreeableness;
				this.averageConscientiousness += human.conscientiousness;
				this.averageCreativity += human.creativity;
				this.maxConscientiousness = Mathf.Max(this.maxConscientiousness, human.conscientiousness);
			}
			this.averageHumility /= (float)this.owners.Count;
			this.averageEmotionality /= (float)this.owners.Count;
			this.averageExtraversion /= (float)this.owners.Count;
			this.averageAgreeableness /= (float)this.owners.Count;
			this.averageConscientiousness /= (float)this.owners.Count;
			this.averageCreativity /= (float)this.owners.Count;
		}
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x00107CCC File Offset: 0x00105ECC
	public void AddInhabitant(Human newInhabitant)
	{
		if (!this.inhabitants.Contains(newInhabitant))
		{
			this.inhabitants.Add(newInhabitant);
		}
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x00107CE8 File Offset: 0x00105EE8
	public void RemoveInhabitant(Human newInhabitant)
	{
		if (this.inhabitants.Contains(newInhabitant))
		{
			this.inhabitants.Remove(newInhabitant);
		}
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00107D08 File Offset: 0x00105F08
	public void RemoveOwner(Human newOwner)
	{
		if (this.owners.Contains(newOwner))
		{
			this.owners.Remove(newOwner);
			this.averageHumility = 0f;
			this.averageEmotionality = 0f;
			this.averageExtraversion = 0f;
			this.averageAgreeableness = 0f;
			this.averageConscientiousness = 0f;
			this.averageCreativity = 0f;
			this.maxConscientiousness = 0f;
			foreach (Human human in this.owners)
			{
				this.averageHumility += human.humility;
				this.averageEmotionality += human.emotionality;
				this.averageExtraversion += human.extraversion;
				this.averageAgreeableness += human.agreeableness;
				this.averageConscientiousness += human.conscientiousness;
				this.averageCreativity += human.creativity;
				this.maxConscientiousness = Mathf.Max(this.maxConscientiousness, human.conscientiousness);
			}
			this.averageHumility /= (float)this.owners.Count;
			this.averageEmotionality /= (float)this.owners.Count;
			this.averageExtraversion /= (float)this.owners.Count;
			this.averageAgreeableness /= (float)this.owners.Count;
			this.averageConscientiousness /= (float)this.owners.Count;
			this.averageCreativity /= (float)this.owners.Count;
		}
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00107EE4 File Offset: 0x001060E4
	public void UpdateDesignStyle()
	{
		List<DesignStylePreset> list = new List<DesignStylePreset>();
		bool flag = false;
		if (!SessionData.Instance.isFloorEdit && this.preset.useBuildingDesignStyle)
		{
			if (Game.Instance.allowEchelons && this.building.preset.buildingFeaturesEchelonFloors && this.floor != null && this.floor.floor >= this.building.preset.echelonFloorStart)
			{
				list.Add(CityControls.Instance.echelonDesignStyle);
				flag = true;
			}
			else if (this.building.designStyle.compatibleAddressTypes.Contains(this.preset))
			{
				list.Add(this.building.designStyle);
				flag = true;
			}
		}
		if (!flag)
		{
			float num = Toolbox.Instance.GetNormalizedLandValue(this, false);
			foreach (DesignStylePreset designStylePreset in Toolbox.Instance.allDesignStyles)
			{
				if (designStylePreset.compatibleAddressTypes.Contains(this.preset) && designStylePreset.includeInPersonalityMatching)
				{
					if (SessionData.Instance.isFloorEdit && FloorEditController.Instance.forceBasementToggle.isOn)
					{
						if (!designStylePreset.isBasement)
						{
							continue;
						}
					}
					else if (this.floor.floor >= 0)
					{
						if (designStylePreset.isBasement)
						{
							continue;
						}
					}
					else if (!designStylePreset.isBasement)
					{
						continue;
					}
					if (designStylePreset.minimumWealth <= num)
					{
						float num2 = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.humility - this.averageHumility * 10f)));
						int num3 = 10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.emotionality - this.averageEmotionality * 10f));
						int num4 = 10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.extraversion - this.averageExtraversion * 10f));
						int num5 = 10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.agreeableness - this.averageAgreeableness * 10f));
						int num6 = 10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.conscientiousness - this.averageConscientiousness * 10f));
						int num7 = 10 - Mathf.RoundToInt(Mathf.Abs((float)designStylePreset.creativity - this.averageCreativity * 10f));
						int num8 = Mathf.FloorToInt((num2 + (float)num3 + (float)num4 + (float)num5 + (float)num6 + (float)num7) / 6f);
						for (int i = 0; i < num8; i++)
						{
							list.Add(designStylePreset);
						}
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			list.Add(CityControls.Instance.fallbackStyle);
			Game.Log("CityGen: No compatible design styles found for " + base.name + ", using backup style...", 2);
		}
		int num9;
		if (SessionData.Instance.isFloorEdit)
		{
			num9 = Toolbox.Instance.Rand(0, list.Count, false);
		}
		else
		{
			num9 = Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, this.id.ToString() + this.district.districtID.ToString(), false);
		}
		base.SetDesignStyle(list[num9]);
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00108238 File Offset: 0x00106438
	public void SetName()
	{
		int num = 0;
		List<NewAddress> list = this.floor.addresses.FindAll((NewAddress item) => item.preset == this.preset);
		if (list != null)
		{
			num += list.Count;
		}
		if (this.preset != null)
		{
			base.name = this.preset.name + " " + num.ToString();
		}
		base.gameObject.name = base.name;
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x001082B1 File Offset: 0x001064B1
	public void SetName(string newName)
	{
		base.name = newName;
		base.transform.name = base.name;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x001082CC File Offset: 0x001064CC
	public void SetAddressType(LayoutConfiguration newType)
	{
		if (newType != this.preset)
		{
			Game.Log("Set address to " + this.preset.name, 2);
			this.preset = newType;
			if (this.preset.isOutside)
			{
				this.isOutside = true;
			}
			this.SetName();
			if (newType.isLobby)
			{
				this.isLobby = true;
				this.building.AddLobby(this);
			}
		}
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x0010833E File Offset: 0x0010653E
	private void OnDestroy()
	{
		if (SessionData.Instance.isFloorEdit && this.floor != null)
		{
			this.floor.RemoveAddress(this);
		}
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x00108368 File Offset: 0x00106568
	public void OnDoorKnockByActor(NewDoor dc, float urgency, Actor byWho)
	{
		Game.Log(string.Concat(new string[]
		{
			"Debug: Knock on door ",
			base.name,
			" with urgency ",
			urgency.ToString(),
			" with occupants count: ",
			this.currentOccupants.Count.ToString()
		}), 2);
		Predicate<NewAIGoal> <>9__0;
		foreach (Actor actor in this.currentOccupants)
		{
			if (actor.isDead)
			{
				Game.Log("Debug: Occupant: isDead", 2);
			}
			else if (actor.isStunned)
			{
				Game.Log("Debug: Occupant: stunned", 2);
			}
			else if (actor.isPlayer)
			{
				Game.Log("Debug: Occupant: isPlayer", 2);
			}
			else
			{
				if (actor.isAsleep)
				{
					if (Toolbox.Instance.Rand(0f, 1f, false) > urgency + 0.1f)
					{
						Game.Log("Debug: " + actor.name + " was not woken by knock", 2);
						continue;
					}
					Game.Log("Debug: " + actor.name + " was woken up by knock", 2);
					actor.WakeUp(false);
				}
				Human human = actor as Human;
				if (human != null && (human.locationsOfAuthority.Contains(this) || (human.isEnforcer && human.isOnDuty)))
				{
					foreach (Actor actor2 in this.currentOccupants)
					{
						if (actor2.ai != null)
						{
							List<NewAIGoal> goals = actor2.ai.goals;
							Predicate<NewAIGoal> predicate;
							if ((predicate = <>9__0) == null)
							{
								predicate = (<>9__0 = ((NewAIGoal item) => item.preset == RoutineControls.Instance.answerDoorGoal && item.passedInteractable == dc.handleInteractable));
							}
							if (goals.Exists(predicate))
							{
								Game.Log("Debug: Answering door event already exists for " + actor2.name, 2);
								return;
							}
						}
					}
					Game.Log("Debug: " + actor.name + " answering door...", 2);
					actor.ai.AnswerDoor(dc, this, byWho);
					break;
				}
			}
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x001085E8 File Offset: 0x001067E8
	public override void CreateEvidence()
	{
		if (this.evidenceEntry == null && !this.isOutsideAddress && this.preset.roomLayout.Count > 0)
		{
			this.evidenceEntry = (EvidenceCreator.Instance.CreateEvidence("location", "Location" + this.id.ToString(), this, null, null, null, this.building.evidenceEntry, false, null) as EvidenceLocation);
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x0009014F File Offset: 0x0008E34F
	public override void SetupEvidence()
	{
		base.SetupEvidence();
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x00108658 File Offset: 0x00106858
	public EvidenceMultiPage CreateCalendar()
	{
		if (this.calendar == null)
		{
			this.calendar = (EvidenceCreator.Instance.CreateEvidence("Calendar", "Calendar" + this.id.ToString(), this, null, null, null, null, false, null) as EvidenceMultiPage);
			if (CityConstructor.Instance == null || CityConstructor.Instance.generateNew)
			{
				for (int i = 1; i < 13; i++)
				{
					this.calendar.AddStringContentToPage(i, Strings.Get("ui.interface", ((SessionData.Month)(i - 1)).ToString(), Strings.Casing.asIs, false, false, false, null), "\n\n", -1);
				}
				Dictionary<int, Dictionary<int, List<Human>>> dictionary = new Dictionary<int, Dictionary<int, List<Human>>>();
				foreach (Human human in this.owners)
				{
					if (!human.isPlayer)
					{
						string[] array = human.birthday.Split('/', 0);
						int num = 0;
						int num2 = 0;
						int.TryParse(array[0], ref num);
						int.TryParse(array[1], ref num2);
						if (!dictionary.ContainsKey(num))
						{
							dictionary.Add(num, new Dictionary<int, List<Human>>());
						}
						if (!dictionary[num].ContainsKey(num2))
						{
							dictionary[num].Add(num2, new List<Human>());
						}
						if (!dictionary[num][num2].Contains(human))
						{
							dictionary[num][num2].Add(human);
							this.calendar.AddEvidenceDiscoveryToPage(num, human.evidenceEntry, Evidence.Discovery.dateOfBirth);
						}
						foreach (Acquaintance acquaintance in human.acquaintances)
						{
							if (!acquaintance.with.isPlayer && acquaintance.known >= SocialControls.Instance.knowBirthdayThreshold)
							{
								string[] array2 = acquaintance.with.birthday.Split('/', 0);
								int num3 = 0;
								int num4 = 0;
								int.TryParse(array2[0], ref num3);
								int.TryParse(array2[1], ref num4);
								if (!dictionary.ContainsKey(num3))
								{
									dictionary.Add(num3, new Dictionary<int, List<Human>>());
								}
								if (!dictionary[num3].ContainsKey(num4))
								{
									dictionary[num3].Add(num4, new List<Human>());
								}
								if (!dictionary[num3][num4].Contains(acquaintance.with))
								{
									dictionary[num3][num4].Add(acquaintance.with);
									this.calendar.AddEvidenceDiscoveryToPage(num3, acquaintance.with.evidenceEntry, Evidence.Discovery.dateOfBirth);
								}
							}
						}
					}
				}
				for (int j = 1; j < 13; j++)
				{
					if (dictionary.ContainsKey(j))
					{
						for (int k = 1; k < 32; k++)
						{
							if (dictionary[j].ContainsKey(k))
							{
								string text = string.Empty;
								for (int l = 0; l < dictionary[j][k].Count; l++)
								{
									Human human2 = dictionary[j][k][l];
									if (l > 0)
									{
										text += ", ";
									}
									text += human2.GetCitizenName();
								}
								if (text.Length > 0)
								{
									this.calendar.AddStringContentToPage(j, string.Concat(new string[]
									{
										Toolbox.Instance.GetNumbericalStringReference(k),
										" — ",
										text,
										" ",
										Strings.Get("ui.interface", "birthday", Strings.Casing.asIs, false, false, false, null)
									}), "\n\n", k);
								}
							}
						}
					}
				}
			}
			else
			{
				foreach (Human human3 in this.owners)
				{
					if (!human3.isPlayer)
					{
						string[] array3 = human3.birthday.Split('/', 0);
						int page = 0;
						int num5 = 0;
						int.TryParse(array3[0], ref page);
						int.TryParse(array3[1], ref num5);
						this.calendar.AddEvidenceDiscoveryToPage(page, human3.evidenceEntry, Evidence.Discovery.dateOfBirth);
						foreach (Acquaintance acquaintance2 in human3.acquaintances)
						{
							if (!acquaintance2.with.isPlayer && acquaintance2.known >= SocialControls.Instance.knowBirthdayThreshold)
							{
								string[] array4 = acquaintance2.with.birthday.Split('/', 0);
								int page2 = 0;
								int num6 = 0;
								int.TryParse(array4[0], ref page2);
								int.TryParse(array4[1], ref num6);
								this.calendar.AddEvidenceDiscoveryToPage(page2, acquaintance2.with.evidenceEntry, Evidence.Discovery.dateOfBirth);
							}
						}
					}
				}
			}
			this.calendar.SetPage(SessionData.Instance.monthInt + 1, false);
		}
		return this.calendar;
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00108BD0 File Offset: 0x00106DD0
	public CitySaveData.AddressCitySave GenerateSaveData()
	{
		CitySaveData.AddressCitySave addressCitySave = new CitySaveData.AddressCitySave();
		addressCitySave.name = base.name;
		addressCitySave.residenceNumber = this.residenceNumber;
		addressCitySave.isLobby = this.isLobby;
		addressCitySave.isOutside = this.isOutside;
		addressCitySave.access = this.access;
		addressCitySave.neonHor = this.featuresNeonSignageHorizontal;
		addressCitySave.neonVer = this.featuresNeonSignageVertical;
		addressCitySave.neonVerticalIndex = this.neonVerticalIndex;
		addressCitySave.neonColour = this.neonColour;
		addressCitySave.landValue = this.normalizedLandValue;
		addressCitySave.passcode = this.passcode;
		addressCitySave.hkey = this.hiddenSpareKey;
		addressCitySave.breakerSec = this.breakerSecurityID;
		addressCitySave.breakerLights = this.breakerLightsID;
		addressCitySave.breakerDoors = this.breakerDoorsID;
		if (this.neonFont != null)
		{
			addressCitySave.neonFont = this.neonFont.name;
		}
		foreach (NewRoom newRoom in this.rooms)
		{
			addressCitySave.rooms.Add(newRoom.GenerateSaveData());
		}
		foreach (NewNode newNode in this.protectedNodes)
		{
			addressCitySave.protectedNodes.Add(newNode.nodeCoord);
		}
		addressCitySave.designStyle = this.designStyle.name;
		addressCitySave.id = this.id;
		addressCitySave.preset = this.preset.name;
		addressCitySave.wood = this.wood;
		if (this.addressPreset != null)
		{
			addressCitySave.address = this.addressPreset.name;
		}
		addressCitySave.isLobbyAddress = this.isLobbyAddress;
		addressCitySave.isOutsideAddress = this.isOutsideAddress;
		if (this.residence != null)
		{
			addressCitySave.residence = this.residence.GenerateSaveData();
		}
		if (this.company != null)
		{
			addressCitySave.company = this.company.GenerateSaveData();
		}
		bool devMode = Game.Instance.devMode;
		return addressCitySave;
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x00108E10 File Offset: 0x00107010
	public void CreateSignageHorizontal()
	{
		if (this.neonSignHorizontal != null)
		{
			Object.Destroy(this.neonSignHorizontal);
		}
		List<NewWall> list = new List<NewWall>();
		foreach (NewRoom newRoom in this.rooms)
		{
			foreach (NewNode newNode in newRoom.nodes)
			{
				foreach (NewWall newWall in newNode.walls)
				{
					if (newWall.node.gameLocation.thisAsStreet != null)
					{
						if (!newWall.node.tile.isObstacle)
						{
							list.Add(newWall);
						}
					}
					else if (newWall.otherWall.node.gameLocation.thisAsStreet != null && !newWall.otherWall.node.tile.isObstacle)
					{
						list.Add(newWall.otherWall);
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		int num = 999;
		List<List<NewWall>> list2 = new List<List<NewWall>>();
		while (list.Count > 0 && num > 0)
		{
			num--;
			List<NewWall> list3 = new List<NewWall>();
			list3.Add(list[0]);
			List<NewWall> list4 = new List<NewWall>();
			int num2 = 999;
			while (list3.Count > 0 && num2 > 0)
			{
				NewWall newWall2 = list3[0];
				list.Remove(newWall2);
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector2Int vector2Int = offsetArrayX[i];
					Vector3 vector = newWall2.node.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
					NewNode foundNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector, ref foundNode))
					{
						NewWall newWall3 = list.Find((NewWall item) => item.node == foundNode);
						if (newWall3 != null && newWall3.wallOffset == newWall2.wallOffset && !list3.Contains(newWall3) && !list4.Contains(newWall3))
						{
							list3.Add(newWall3);
						}
					}
				}
				list4.Add(newWall2);
				list3.Remove(newWall2);
				num2--;
			}
			list2.Add(list4);
		}
		List<NewWall> list5 = null;
		int num3 = -1;
		foreach (List<NewWall> list6 in list2)
		{
			bool flag = list6.Find((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance) != null;
			int num4 = 0;
			if (flag)
			{
				num4 = 2;
			}
			if (list5 == null || list6.Count + num4 > num3)
			{
				list5 = list6;
				num3 = list6.Count + num4;
			}
		}
		float num5 = (float)list5.Count * 1.8f;
		float num6 = 0f;
		List<GameObject> list7 = new List<GameObject>();
		if (this.company != null)
		{
			for (int j = 0; j < this.company.shortName.Length; j++)
			{
				try
				{
					string character = this.company.shortName.Substring(j, 1).ToLower();
					if (character == " ")
					{
						num6 += 0.4f;
						list7.Add(null);
					}
					else
					{
						NeonSignCharacters.NeonCharacter neonCharacter = this.neonFont.characterList.Find((NeonSignCharacters.NeonCharacter item) => item.character.ToLower() == character);
						if (neonCharacter != null)
						{
							GameObject prefab = neonCharacter.prefab;
							list7.Add(prefab);
							num6 += prefab.GetComponent<MeshRenderer>().bounds.size.x;
							if (j < base.name.Length - 1)
							{
								num6 += 0.1f;
							}
							if (num6 > num5)
							{
								return;
							}
						}
					}
				}
				catch
				{
					return;
				}
			}
		}
		Vector3 avNodePos = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		foreach (NewWall newWall4 in list5)
		{
			avNodePos += newWall4.node.nodeCoord;
			vector2 += newWall4.position;
		}
		vector2 /= (float)list5.Count;
		avNodePos /= (float)list5.Count;
		avNodePos = new Vector3(Mathf.Round(avNodePos.x), Mathf.Round(avNodePos.y), Mathf.Round(avNodePos.z));
		NewWall newWall5 = list5.Find((NewWall item) => item.node.nodeCoord == avNodePos);
		this.neonSignHorizontal = Object.Instantiate<GameObject>(PrefabControls.Instance.neonSign, newWall5.node.room.transform);
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetTRS(newWall5.position, Quaternion.Euler(newWall5.localEulerAngles + newWall5.node.room.transform.eulerAngles), Vector3.one);
		this.neonSignHorizontal.transform.position = identity.MultiplyPoint3x4(this.building.preset.horizontalSignOffset + this.addressPreset.horizontalSignOffset);
		this.neonSignHorizontal.transform.localEulerAngles = newWall5.localEulerAngles;
		newWall5.node.SetAsAudioSource(AudioControls.Instance.neonSignLoopSmall, new Vector3(0f, 3f, 0f));
		CityControls.NeonMaterial neonMaterial = CityControls.Instance.neonColours[this.neonColour];
		if (neonMaterial.regularMat == null)
		{
			neonMaterial.regularMat = Object.Instantiate<Material>(CityControls.Instance.neonMaterial);
			neonMaterial.regularMat.SetColor("_BaseColor", neonMaterial.neonColour * CityControls.Instance.neonIntensity);
			neonMaterial.flickingMat = Object.Instantiate<Material>(CityControls.Instance.neonMaterial);
			neonMaterial.flickingMat.SetColor("_BaseColor", neonMaterial.neonColour * CityControls.Instance.neonIntensity);
		}
		foreach (Light light in this.neonSignHorizontal.GetComponentsInChildren<Light>())
		{
			HDAdditionalLightData component = light.transform.gameObject.GetComponent<HDAdditionalLightData>();
			light.color = neonMaterial.neonColour;
			if (component.GetLightTypeAndShape() == 5)
			{
				component.shapeWidth = num6;
			}
		}
		this.neonSignHorizontal.transform.name = base.name;
		float num7 = 0f;
		List<MeshFilter> list8 = new List<MeshFilter>();
		for (int k = 0; k < list7.Count; k++)
		{
			if (list7[k] == null)
			{
				num7 += 0.5f;
			}
			else
			{
				Vector2 vector3 = list7[k].GetComponent<MeshRenderer>().bounds.size;
				num7 += vector3.x * 0.5f;
				GameObject gameObject = Object.Instantiate<GameObject>(list7[k], this.neonSignHorizontal.transform);
				gameObject.transform.localPosition = new Vector3(-num7 + num6 * 0.5f + 0.3f, vector3.y * -0.5f, 0f);
				if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, base.GetReplicableSeed(), false) <= 0.04f)
				{
					gameObject.GetComponent<MeshRenderer>().material = neonMaterial.flickingMat;
					gameObject.AddComponent<NeonLetterFlickerController>().neonMat = neonMaterial;
					this.featuresBrokenSign = true;
				}
				else
				{
					list8.Add(gameObject.GetComponent<MeshFilter>());
				}
				num7 += list7[k].GetComponent<MeshRenderer>().bounds.size.x * 0.5f;
				num7 += 0.1f;
			}
		}
		GameObject gameObject2 = new GameObject();
		CombineInstance[] array = new CombineInstance[list8.Count];
		for (int l = 0; l < list8.Count; l++)
		{
			array[l].mesh = list8[l].sharedMesh;
			array[l].transform = list8[l].transform.localToWorldMatrix;
			Object.Destroy(list8[l].gameObject);
		}
		MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(array, true, true);
		base.transform.gameObject.SetActive(true);
		meshCollider.sharedMesh = meshFilter.mesh;
		gameObject2.transform.SetParent(this.neonSignHorizontal.transform);
		gameObject2.transform.position = Vector3.zero;
		gameObject2.transform.eulerAngles = Vector3.zero;
		gameObject2.layer = 29;
		MeshRenderer component2 = gameObject2.GetComponent<MeshRenderer>();
		component2.sharedMaterial = neonMaterial.regularMat;
		component2.shadowCastingMode = 0;
		Toolbox.Instance.SetLightLayer(meshRenderer, this.building, true);
		meshRenderer.gameObject.isStatic = false;
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0010983C File Offset: 0x00107A3C
	public void CreateSignageVertical()
	{
		if (this.neonSignVertical != null)
		{
			Object.Destroy(this.neonSignVertical);
		}
		if (this.streetAccess == null)
		{
			return;
		}
		if (this.addressPreset.possibleSigns.Count <= 0)
		{
			return;
		}
		List<BuildingPreset.CableLinkPoint> list = new List<BuildingPreset.CableLinkPoint>();
		List<float> list2 = new List<float>();
		foreach (BuildingPreset.CableLinkPoint cableLinkPoint in this.building.preset.cableLinkPoints)
		{
			Vector3 vector = this.building.buildingModelBase.transform.TransformPoint(cableLinkPoint.localPos);
			if (vector.y >= 8f)
			{
				float num = Vector3.Distance(vector, this.streetAccess.fromNode.position);
				if (list.Count <= 0)
				{
					list.Add(cableLinkPoint);
					list2.Add(num);
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (num < list2[i])
						{
							list.Insert(i, cableLinkPoint);
							list2.Insert(i, num);
							break;
						}
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		BuildingPreset.CableLinkPoint cableLinkPoint2 = list[0];
		this.neonSignVertical = Object.Instantiate<GameObject>(this.addressPreset.possibleSigns[this.neonVerticalIndex], this.building.buildingModelBase.transform);
		this.neonSignVertical.transform.localEulerAngles = cableLinkPoint2.localRot + new Vector3(0f, 90f, 0f);
		this.neonSignVertical.transform.localPosition = cableLinkPoint2.localPos;
		NeonSignController component = this.neonSignVertical.GetComponent<NeonSignController>();
		if (component != null)
		{
			CityControls.NeonMaterial neonMaterial = CityControls.Instance.neonColours[this.neonColour];
			for (int j = 0; j < component.materialAnimations.Count; j++)
			{
				if (component.lightBools[j])
				{
					component.materialAnimations[j] = Object.Instantiate<Material>(component.materialAnimations[j]);
					if (component.useAddressColours)
					{
						if (component.changeBaseColour)
						{
							component.materialAnimations[j].SetColor("_BaseColor", neonMaterial.neonColour * CityControls.Instance.neonIntensity);
						}
						if (component.changeAltColour1)
						{
							component.materialAnimations[j].SetColor("_Color1", neonMaterial.neonColour * CityControls.Instance.neonIntensity);
						}
						if (component.changeAltColour2)
						{
							component.materialAnimations[j].SetColor("_Color2", neonMaterial.altColour2 * CityControls.Instance.neonIntensity);
						}
						if (component.changeAltColour3)
						{
							component.materialAnimations[j].SetColor("_Color3", neonMaterial.altColour3 * CityControls.Instance.neonIntensity);
						}
					}
				}
			}
			foreach (Light light in this.neonSignVertical.GetComponentsInChildren<Light>())
			{
				light.transform.gameObject.GetComponent<HDAdditionalLightData>();
				light.color = neonMaterial.neonColour;
			}
		}
		this.neonSignVertical.transform.name = base.name;
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00109BB4 File Offset: 0x00107DB4
	public void GenerateJobPathingData()
	{
		this.accessRef = new NativeMultiHashMap<float3, int>(0, 4);
		this.accessPositions = new NativeHashMap<int, float3>(0, 4);
		this.toNodeReference = new NativeHashMap<int, float3>(0, 4);
		this.noPassRef = new NativeList<float3>(0, 4);
		foreach (NewNode newNode in this.nodes)
		{
			if (newNode.noPassThrough)
			{
				float3 @float = Toolbox.Instance.ToFloat3(newNode.nodeCoord);
				this.noPassRef.Add(ref @float);
			}
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode.accessToOtherNodes)
			{
				if (keyValuePair.Value.walkingAccess && !keyValuePair.Value.employeeDoor && !keyValuePair.Value.toNode.noPassThrough)
				{
					this.accessRef.Add(Toolbox.Instance.ToFloat3(newNode.nodeCoord), keyValuePair.Value.id);
					this.accessPositions.TryAdd(keyValuePair.Value.id, CityData.Instance.RealPosToNodeFloat(keyValuePair.Value.worldAccessPoint));
					this.toNodeReference.TryAdd(keyValuePair.Value.id, Toolbox.Instance.ToFloat3(keyValuePair.Value.toNode.nodeCoord));
				}
			}
		}
		foreach (NewNode.NodeAccess nodeAccess in this.entrances)
		{
			if (nodeAccess.walkingAccess && !nodeAccess.employeeDoor && !nodeAccess.toNode.noPassThrough)
			{
				this.accessRef.Add(Toolbox.Instance.ToFloat3(nodeAccess.fromNode.nodeCoord), nodeAccess.id);
				this.accessPositions.TryAdd(nodeAccess.id, CityData.Instance.RealPosToNodeFloat(nodeAccess.worldAccessPoint));
				this.toNodeReference.TryAdd(nodeAccess.id, Toolbox.Instance.ToFloat3(nodeAccess.toNode.nodeCoord));
			}
		}
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x00109E84 File Offset: 0x00108084
	public void CalculateRoomOwnership()
	{
		foreach (NewRoom newRoom in this.rooms)
		{
			List<RoomConfiguration> list = new List<RoomConfiguration>(newRoom.openPlanElements);
			if (!list.Contains(newRoom.preset))
			{
				if (newRoom.preset == null)
				{
					Game.LogError("Null room preset reference found in " + base.name, 2);
				}
				else
				{
					list.Add(newRoom.preset);
				}
			}
			foreach (RoomConfiguration roomConfiguration in list)
			{
				if (roomConfiguration.useOwnership)
				{
					if (!this.roomsBelongTo.ContainsKey(roomConfiguration.roomType))
					{
						this.roomsBelongTo.Add(roomConfiguration.roomType, new Dictionary<NewRoom, List<Human>>());
					}
					List<Human> list2 = new List<Human>(this.inhabitants);
					foreach (KeyValuePair<NewRoom, List<Human>> keyValuePair in this.roomsBelongTo[roomConfiguration.roomType])
					{
						foreach (Human human in keyValuePair.Value)
						{
							list2.Remove(human);
						}
					}
					if (roomConfiguration.belongsToJob.Count > 0)
					{
						for (int i = 0; i < list2.Count; i++)
						{
							Human human2 = list2[i];
							if (human2.job == null || human2.job.preset == null || !roomConfiguration.belongsToJob.Contains(human2.job.preset))
							{
								list2.RemoveAt(i);
								i--;
							}
						}
					}
					Human human3 = null;
					for (int j = 0; j < roomConfiguration.assignBelongsToOwners; j++)
					{
						if (j < list2.Count)
						{
							Human human4 = list2[j];
							if (human3 != null && roomConfiguration.preferCouples && human3.partner != null)
							{
								if (!list2.Contains(human3.partner))
								{
									break;
								}
								human4 = human3.partner;
							}
							if (human4 != null)
							{
								newRoom.AddOwner(human4);
								human3 = human4;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x0010A15C File Offset: 0x0010835C
	public void SelectAirVentLocations()
	{
		if (this.addressPreset == null)
		{
			return;
		}
		string seed = this.seed;
		int num = Mathf.RoundToInt(Toolbox.Instance.GetPsuedoRandomNumberContained(this.addressPreset.airVentRange.x, this.addressPreset.airVentRange.y + 1f, seed, out seed));
		if (num > 0)
		{
			List<NewAddress.AirVentLocation> list = new List<NewAddress.AirVentLocation>();
			using (List<NewRoom>.Enumerator enumerator = this.rooms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewRoom newRoom = enumerator.Current;
					for (int i = 0; i < newRoom.preset.chanceOfRoofVent; i++)
					{
						list.Add(new NewAddress.AirVentLocation
						{
							room = newRoom,
							location = NewAddress.AirVent.ceiling
						});
					}
					for (int j = 0; j < newRoom.preset.chanceOfWallVentLower; j++)
					{
						list.Add(new NewAddress.AirVentLocation
						{
							room = newRoom,
							location = NewAddress.AirVent.wallLower
						});
					}
					for (int k = 0; k < newRoom.preset.chanceOfWallVentUpper; k++)
					{
						list.Add(new NewAddress.AirVentLocation
						{
							room = newRoom,
							location = NewAddress.AirVent.wallUpper
						});
					}
				}
				goto IL_1EB;
			}
			IL_149:
			if (list.Count <= 0)
			{
				return;
			}
			NewAddress.AirVentLocation airVentLocation = list[Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, seed, out seed)];
			airVentLocation.room.AddRandomAirVent(airVentLocation.location);
			if (airVentLocation.room.airVents.Count >= airVentLocation.room.preset.maximumVents)
			{
				for (int l = 0; l < list.Count; l++)
				{
					if (list[l].room == airVentLocation.room)
					{
						list.RemoveAt(l);
						l--;
					}
				}
			}
			num--;
			IL_1EB:
			if (num > 0)
			{
				goto IL_149;
			}
		}
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0010A36C File Offset: 0x0010856C
	public void PickPassword()
	{
		this.passcode.digits.Clear();
		string seed = this.seed;
		this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, seed, out seed));
		this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, seed, out seed));
		this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, seed, out seed));
		this.passcode.digits.Add(Toolbox.Instance.GetPsuedoRandomNumberContained(0, 10, seed, out seed));
		if (this.company != null)
		{
			if (this.passcode.used && this.owners.Count > 0)
			{
				Interactable interactable = this.owners[0].WriteNote(Human.NoteObject.note, "65ad8237-a50a-4fbe-95c0-1c14de71912c", this.owners[0], this, 0, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, null, false, 0, 0, null);
				if (interactable != null)
				{
					this.passcode.notes.Add(interactable.id);
				}
			}
			using (List<Occupation>.Enumerator enumerator = this.company.companyRoster.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Occupation occupation = enumerator.Current;
					if (occupation.employee != null && this.passcode.used && occupation.employee.home != null)
					{
						Interactable interactable2 = occupation.employee.WriteNote(Human.NoteObject.note, "1e7df673-89e6-45b0-8e23-1cff0bbca97f", occupation.employee, occupation.employee.home, 1, InteractablePreset.OwnedPlacementRule.both, 2, null, false, 0, 0, null);
						if (interactable2 != null)
						{
							this.passcode.notes.Add(interactable2.id);
						}
					}
				}
				return;
			}
		}
		if (this.passcode.used && this.owners.Count > 0)
		{
			Interactable interactable3 = this.owners[0].WriteNote(Human.NoteObject.note, "65ad8237-a50a-4fbe-95c0-1c14de71912c", this.owners[0], this, 1, InteractablePreset.OwnedPlacementRule.both, 3, null, false, 0, 0, null);
			if (interactable3 != null)
			{
				this.passcode.notes.Add(interactable3.id);
			}
		}
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x0010A59C File Offset: 0x0010879C
	public void SetAlarm(bool newVal, Human target)
	{
		if (this.addressPreset != null && this.building != null && !this.addressPreset.useOwnSecuritySystem)
		{
			this.building.SetAlarm(newVal, target, this.floor);
			return;
		}
		if (this.alarmActive != newVal)
		{
			this.alarmActive = newVal;
			if (this.alarmActive)
			{
				if (!GameplayController.Instance.activeAlarmsLocations.Contains(this))
				{
					GameplayController.Instance.activeAlarmsLocations.Add(this);
				}
				using (List<Actor>.Enumerator enumerator = this.currentOccupants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Actor actor = enumerator.Current;
						if (!actor.isPlayer && !actor.isAsleep && !actor.isDead && !actor.isStunned && !(actor.ai == null))
						{
							if (actor.locationsOfAuthority.Contains(this))
							{
								actor.OnInvestigate(actor, 1);
							}
							actor.AddNerve(CitizenControls.Instance.nerveAlarm, null);
						}
					}
					goto IL_188;
				}
			}
			this.alarmTimer = 0f;
			GameplayController.Instance.activeAlarmsLocations.Remove(this);
			if (this.building != null && this.addressPreset.alarmLocksDownFloor)
			{
				foreach (KeyValuePair<int, NewFloor> keyValuePair in this.building.floors)
				{
					keyValuePair.Value.SetAlarmLockdown(false, null);
				}
			}
			this.alarmTargets.Clear();
			IL_188:
			if (Player.Instance.currentGameLocation == this)
			{
				StatusController.Instance.ForceStatusCheck();
			}
			foreach (Interactable interactable in this.alarms)
			{
				interactable.SetCustomState1(this.alarmActive, null, true, false, false);
			}
			foreach (Interactable interactable2 in this.sentryGuns)
			{
				interactable2.SetCustomState1(this.alarmActive, null, true, false, false);
			}
			foreach (Interactable interactable3 in this.otherSecurity)
			{
				interactable3.SetCustomState1(this.alarmActive, null, true, false, false);
			}
			if (Player.Instance.currentBuilding == this)
			{
				Player.Instance.OnRoomChange();
			}
		}
		if (this.alarmActive)
		{
			if (target != null)
			{
				Game.Log("Adding alarm target " + target.name, 2);
				if (target.isPlayer)
				{
					SessionData.Instance.TutorialTrigger("alarms", false);
					if (this.building != null)
					{
						StatusController.Instance.SetWantedInBuilding(this.building, GameplayControls.Instance.buildingWantedTime);
					}
				}
				if (!this.alarmTargets.Contains(target))
				{
					this.alarmTargets.Add(target);
				}
			}
			this.alarmTimer = this.GetAlarmTime();
			if (this.building != null && this.addressPreset.alarmLocksDownFloor && this.floor != null)
			{
				this.floor.SetAlarmLockdown(true, null);
			}
		}
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x0010A92C File Offset: 0x00108B2C
	public float GetAlarmTime()
	{
		return Mathf.Lerp(GameplayControls.Instance.buildingAlarmTime.x, GameplayControls.Instance.buildingAlarmTime.y, 0f);
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x0010A958 File Offset: 0x00108B58
	public override bool IsAlarmSystemTarget(Human human)
	{
		if (human == null)
		{
			return false;
		}
		if (human.isDead || human.isStunned)
		{
			return false;
		}
		if (!this.addressPreset.useOwnSecuritySystem && this.building != null)
		{
			return this.building.IsAlarmSystemTarget(human);
		}
		if (this.targetMode == NewBuilding.AlarmTargetMode.notPlayer)
		{
			if (!human.isPlayer)
			{
				return true;
			}
		}
		else
		{
			if (this.targetMode == NewBuilding.AlarmTargetMode.everybody)
			{
				return true;
			}
			if (this.targetMode == NewBuilding.AlarmTargetMode.illegalActivities)
			{
				if (this.alarmTargets.Contains(human) || ((this.building != null & human.isPlayer) && SessionData.Instance.gameTime < this.building.wantedInBuilding))
				{
					return true;
				}
			}
			else
			{
				if (this.targetMode == NewBuilding.AlarmTargetMode.nobody)
				{
					return false;
				}
				if (this.targetMode == NewBuilding.AlarmTargetMode.nonResidents)
				{
					return this.inhabitants.Contains(human);
				}
			}
		}
		return false;
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x0010AA34 File Offset: 0x00108C34
	public override bool IsAlarmActive(out float retAlarmTimer, out NewBuilding.AlarmTargetMode retTargetMode, out List<Human> retTargets)
	{
		retAlarmTimer = 0f;
		retTargetMode = NewBuilding.AlarmTargetMode.illegalActivities;
		retTargets = null;
		if (this.building != null && this.addressPreset != null && !this.addressPreset.useOwnSecuritySystem)
		{
			retAlarmTimer = this.building.alarmTimer;
			retTargetMode = this.building.targetMode;
			retTargets = this.building.alarmTargets;
			return this.building.alarmActive;
		}
		retAlarmTimer = this.alarmTimer;
		retTargetMode = this.targetMode;
		retTargets = this.alarmTargets;
		return this.alarmActive;
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x0010AAC8 File Offset: 0x00108CC8
	public void AddSentryGun(Interactable newInteractable)
	{
		this.sentryGuns.Add(newInteractable);
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x0010AAD6 File Offset: 0x00108CD6
	public void AddOtherSecurity(Interactable newInteractable)
	{
		this.otherSecurity.Add(newInteractable);
		if (this.company != null)
		{
			newInteractable.SetCustomState2(!this.company.openForBusinessDesired, null, true, true, false);
			return;
		}
		newInteractable.SetCustomState2(true, null, true, true, false);
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x0010AB10 File Offset: 0x00108D10
	public void SetBreakerSecurity(Interactable newObject)
	{
		this.breakerSecurity = newObject;
		if (this.breakerSecurity != null)
		{
			this.breakerSecurityID = this.breakerSecurity.id;
		}
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x0010AB32 File Offset: 0x00108D32
	public void SetBreakerLights(Interactable newObject)
	{
		this.breakerLights = newObject;
		if (this.breakerLights != null)
		{
			this.breakerLightsID = this.breakerLights.id;
		}
	}

	// Token: 0x060012A5 RID: 4773 RVA: 0x0010AB54 File Offset: 0x00108D54
	public void SetBreakerDoors(Interactable newObject)
	{
		this.breakerDoors = newObject;
		if (this.breakerDoors != null)
		{
			this.breakerDoorsID = this.breakerDoors.id;
		}
	}

	// Token: 0x060012A6 RID: 4774 RVA: 0x0010AB76 File Offset: 0x00108D76
	public override bool IsOutside()
	{
		return this.isOutside || this.isOutsideAddress;
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x0010AB90 File Offset: 0x00108D90
	public Interactable GetBreakerSecurity()
	{
		if (this.breakerSecurity != null)
		{
			return this.breakerSecurity;
		}
		if (this.breakerSecurityID > -1)
		{
			this.breakerSecurity = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerSecurityID);
		}
		if (this.breakerSecurity != null)
		{
			return this.breakerSecurity;
		}
		if (this.floor != null)
		{
			return this.floor.GetBreakerSecurity();
		}
		return null;
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x0010AC00 File Offset: 0x00108E00
	public Interactable GetBreakerLights()
	{
		if (this.breakerLights != null)
		{
			return this.breakerLights;
		}
		if (this.breakerLightsID > -1)
		{
			this.breakerLights = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerLightsID);
		}
		if (this.breakerLights != null)
		{
			return this.breakerLights;
		}
		if (this.floor != null)
		{
			return this.floor.GetBreakerLights();
		}
		return null;
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x0010AC70 File Offset: 0x00108E70
	public Interactable GetBreakerDoors()
	{
		if (this.breakerDoors != null)
		{
			return this.breakerDoors;
		}
		if (this.breakerDoorsID > -1)
		{
			this.breakerDoors = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.breakerDoorsID);
		}
		if (this.breakerDoors != null)
		{
			return this.breakerDoors;
		}
		if (this.floor != null)
		{
			return this.floor.GetBreakerDoors();
		}
		return null;
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x0010ACE0 File Offset: 0x00108EE0
	public NewNode GetDestinationNode()
	{
		if (Player.Instance.currentGameLocation == this)
		{
			return Player.Instance.FindSafeTeleport(this, false, true);
		}
		NewNode result = null;
		List<NewNode.NodeAccess> list = this.entrances.FindAll((NewNode.NodeAccess item) => (item.accessType == NewNode.NodeAccess.AccessType.door || item.accessType == NewNode.NodeAccess.AccessType.openDoorway) && !item.employeeDoor && (item.fromNode.gameLocation != this || item.toNode.gameLocation != this));
		NewNode.NodeAccess nodeAccess = null;
		if (list.Count > 0)
		{
			nodeAccess = list[0];
			if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.door)
			{
				if (list.Exists((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.openDoorway))
				{
					nodeAccess = list.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.openDoorway);
				}
			}
		}
		if (nodeAccess != null)
		{
			if (base.IsPublicallyOpen(true) || this.isLobby)
			{
				if (nodeAccess.toNode.gameLocation == this)
				{
					result = nodeAccess.toNode;
				}
				else
				{
					result = nodeAccess.fromNode;
				}
			}
			else if (nodeAccess.toNode.gameLocation == this)
			{
				result = nodeAccess.fromNode;
			}
			else
			{
				result = nodeAccess.toNode;
			}
		}
		else
		{
			nodeAccess = this.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.openDoorway && !item.employeeDoor);
			if (nodeAccess != null)
			{
				if (nodeAccess.toNode.gameLocation == this)
				{
					result = nodeAccess.fromNode;
				}
				else
				{
					result = nodeAccess.toNode;
				}
			}
			else
			{
				nodeAccess = this.entrances.Find((NewNode.NodeAccess item) => item.walkingAccess);
				if (nodeAccess != null)
				{
					if (nodeAccess.toNode.gameLocation == this)
					{
						result = nodeAccess.fromNode;
					}
					else
					{
						result = nodeAccess.toNode;
					}
				}
				else if (this.entrances.Count > 0)
				{
					nodeAccess = this.entrances[0];
					if (nodeAccess.toNode.gameLocation == this)
					{
						result = nodeAccess.fromNode;
					}
					else
					{
						result = nodeAccess.toNode;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x0010AEE8 File Offset: 0x001090E8
	private void OnDisable()
	{
		if (Game.Instance.useJobSystem)
		{
			try
			{
				this.accessRef.Dispose();
				this.accessPositions.Dispose();
				this.toNodeReference.Dispose();
				this.noPassRef.Dispose();
			}
			catch
			{
			}
		}
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x0010AF44 File Offset: 0x00109144
	[Button(null, 0)]
	public void IsThisOpen()
	{
		if (this.company != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Is ",
				base.name,
				" Open? Actual: ",
				this.company.openForBusinessActual.ToString(),
				" desired: ",
				this.company.openForBusinessDesired.ToString(),
				" open calc: ",
				this.company.IsOpenAtThisTime(SessionData.Instance.gameTime, SessionData.Instance.decimalClock, SessionData.Instance.day).ToString()
			}), 2);
			float nextOrPreviousGameTimeForThisHour = SessionData.Instance.GetNextOrPreviousGameTimeForThisHour(SessionData.Instance.gameTime, SessionData.Instance.decimalClock, SessionData.Instance.day, this.company.daysOpen, this.company.retailOpenHours.x, this.company.retailOpenHours.y);
			Game.Log(string.Concat(new string[]
			{
				"Next open ",
				nextOrPreviousGameTimeForThisHour.ToString(),
				": ",
				SessionData.Instance.GameTimeToClock12String(nextOrPreviousGameTimeForThisHour, false),
				", ",
				SessionData.Instance.ShortDateString(nextOrPreviousGameTimeForThisHour, false)
			}), 2);
			string text = "Open hours: ";
			Vector2 retailOpenHours = this.company.retailOpenHours;
			Game.Log(text + retailOpenHours.ToString(), 2);
		}
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x0010B0BE File Offset: 0x001092BE
	[Button(null, 0)]
	public void SetPlayerResidence()
	{
		if (this.residence != null)
		{
			PlayerApartmentController.Instance.BuyNewResidence(this.residence, false);
		}
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x0010B0E0 File Offset: 0x001092E0
	public void AddVandalism(Interactable interactable)
	{
		if (interactable == null)
		{
			return;
		}
		if (!this.vandalism.Exists((NewAddress.Vandalism item) => item.obj == interactable.id))
		{
			Game.Log("Adding vandalism for object " + interactable.name + " at " + base.name, 2);
			this.vandalism.Add(new NewAddress.Vandalism
			{
				obj = interactable.id,
				fine = Mathf.RoundToInt(interactable.val * (float)GameplayControls.Instance.vandalismFineMultiplier),
				time = SessionData.Instance.gameTime
			});
			this.SideJobObjectiveCheck();
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x0010B19C File Offset: 0x0010939C
	public void AddVandalism(Vector3 window)
	{
		if (!this.vandalism.Exists((NewAddress.Vandalism item) => item.win == window))
		{
			string text = "Adding vandalism for window ";
			Vector3 window2 = window;
			Game.Log(text + window2.ToString() + " at " + base.name, 2);
			this.vandalism.Add(new NewAddress.Vandalism
			{
				win = window,
				fine = GameplayControls.Instance.breakingWindowsFine,
				time = SessionData.Instance.gameTime
			});
			this.SideJobObjectiveCheck();
		}
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x0010B240 File Offset: 0x00109440
	public void AddVandalism(int fine)
	{
		Game.Log("Adding vandalism " + fine.ToString() + " at " + base.name, 2);
		this.vandalism.Add(new NewAddress.Vandalism
		{
			fine = GameplayControls.Instance.breakingWindowsFine,
			time = SessionData.Instance.gameTime
		});
		this.SideJobObjectiveCheck();
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x0010B2A8 File Offset: 0x001094A8
	private void SideJobObjectiveCheck()
	{
		foreach (Case @case in CasePanelController.Instance.activeCases)
		{
			foreach (Case.ResolveQuestion resolveQuestion in @case.resolveQuestions.FindAll((Case.ResolveQuestion item) => item.revengeObjLoc == this.id))
			{
				if (!resolveQuestion.completedRevenge && resolveQuestion.UpdateCorrect(@case, false))
				{
					Game.Log("Jobs: Manually complete vandalism objective", 2);
					resolveQuestion.completedRevenge = true;
				}
			}
		}
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x0010B368 File Offset: 0x00109568
	public void RemoveVandalism(Interactable interactable)
	{
		if (interactable == null)
		{
			return;
		}
		int num = this.vandalism.FindIndex((NewAddress.Vandalism item) => item.obj == interactable.id);
		if (num > -1)
		{
			this.vandalism.RemoveAt(num);
		}
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x0010B3B4 File Offset: 0x001095B4
	public void RemoveVandalism(Vector3 window)
	{
		int num = this.vandalism.FindIndex((NewAddress.Vandalism item) => item.win == window);
		if (num > -1)
		{
			this.vandalism.RemoveAt(num);
		}
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x0010B3F8 File Offset: 0x001095F8
	public int GetVandalismDamage(bool includeObjects = true, bool includeWindows = true, bool includeMisc = true)
	{
		int num = 0;
		foreach (NewAddress.Vandalism vandalism in this.vandalism)
		{
			if ((includeObjects || vandalism.obj <= -1) && (includeWindows || vandalism.win.y <= -100f) && (includeMisc || vandalism.obj > -1 || vandalism.win.y > -100f))
			{
				num += vandalism.fine;
			}
		}
		return num;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x0010B490 File Offset: 0x00109690
	public override void AddOccupant(Actor newOcc)
	{
		base.AddOccupant(newOcc);
		if (this.otherSecurity.Count > 0 && this.currentOccupants.Exists((Actor item) => item.locationsOfAuthority.Contains(this)))
		{
			foreach (Interactable interactable in this.otherSecurity)
			{
				interactable.SetCustomState2(false, null, true, true, false);
			}
		}
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x0010B514 File Offset: 0x00109714
	public override void RemoveOccupant(Actor remOcc)
	{
		base.RemoveOccupant(remOcc);
		if (this.otherSecurity.Count > 0 && (this.company == null || !this.company.openForBusinessDesired) && !this.currentOccupants.Exists((Actor item) => item.locationsOfAuthority.Contains(this)))
		{
			foreach (Interactable interactable in this.otherSecurity)
			{
				interactable.SetCustomState2(true, null, true, true, false);
			}
		}
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x0010B5AC File Offset: 0x001097AC
	public string GetPassword()
	{
		if (this.addressPreset == null || !this.addressPreset.needsPassword)
		{
			return string.Empty;
		}
		string text = CityData.Instance.seed + this.addressPreset.name;
		string mainList = this.addressPreset.dictionaryPasswordSources[Toolbox.Instance.GetPsuedoRandomNumber(0, this.addressPreset.dictionaryPasswordSources.Count, text, false)];
		string text2 = NameGenerator.Instance.GenerateName("", 0f, mainList, 1f, "", 0f, text);
		Game.Log("Gameplay: Address password for " + base.name + " is: " + text2, 2);
		return text2;
	}

	// Token: 0x040016B8 RID: 5816
	[Header("Address Contents")]
	public AddressSaveData saveData;

	// Token: 0x040016B9 RID: 5817
	public int loadedVarIndex = -1;

	// Token: 0x040016BA RID: 5818
	public List<NewWall> generatedInteriorEntrances = new List<NewWall>();

	// Token: 0x040016BB RID: 5819
	public List<NewNode> protectedNodes = new List<NewNode>();

	// Token: 0x040016BC RID: 5820
	public bool featuresNeonSignageHorizontal;

	// Token: 0x040016BD RID: 5821
	public bool featuresNeonSignageVertical;

	// Token: 0x040016BE RID: 5822
	public NeonSignCharacters neonFont;

	// Token: 0x040016BF RID: 5823
	public int neonColour;

	// Token: 0x040016C0 RID: 5824
	public GameObject neonSignHorizontal;

	// Token: 0x040016C1 RID: 5825
	public GameObject neonSignVertical;

	// Token: 0x040016C2 RID: 5826
	public int neonVerticalIndex = -1;

	// Token: 0x040016C3 RID: 5827
	public bool featuresBrokenSign;

	// Token: 0x040016C4 RID: 5828
	public bool generatedRoomConfigs;

	// Token: 0x040016C5 RID: 5829
	public List<NewAddress.Vandalism> vandalism = new List<NewAddress.Vandalism>();

	// Token: 0x040016C6 RID: 5830
	[Header("Details")]
	public int id;

	// Token: 0x040016C7 RID: 5831
	public static int assignID = 1;

	// Token: 0x040016C8 RID: 5832
	public int editorID;

	// Token: 0x040016C9 RID: 5833
	public static int assignEditorID = 1;

	// Token: 0x040016CA RID: 5834
	public LayoutConfiguration preset;

	// Token: 0x040016CB RID: 5835
	public Color editorColour = Color.cyan;

	// Token: 0x040016CC RID: 5836
	public Color wood;

	// Token: 0x040016CD RID: 5837
	public bool isOutsideAddress;

	// Token: 0x040016CE RID: 5838
	public bool isLobbyAddress;

	// Token: 0x040016CF RID: 5839
	public float normalizedLandValue;

	// Token: 0x040016D0 RID: 5840
	public bool hiddenSpareKey;

	// Token: 0x040016D1 RID: 5841
	[Header("Inhabitants")]
	public List<Human> owners = new List<Human>();

	// Token: 0x040016D2 RID: 5842
	public List<Human> inhabitants = new List<Human>();

	// Token: 0x040016D3 RID: 5843
	public List<Human> favouredCustomers = new List<Human>();

	// Token: 0x040016D4 RID: 5844
	public AddressPreset addressPreset;

	// Token: 0x040016D5 RID: 5845
	public ResidenceController residence;

	// Token: 0x040016D6 RID: 5846
	public Company company;

	// Token: 0x040016D7 RID: 5847
	public bool interiorLightsEnabled = true;

	// Token: 0x040016D8 RID: 5848
	public Dictionary<RoomTypePreset, Dictionary<NewRoom, List<Human>>> roomsBelongTo = new Dictionary<RoomTypePreset, Dictionary<NewRoom, List<Human>>>();

	// Token: 0x040016D9 RID: 5849
	[Space(7f)]
	public float averageHumility;

	// Token: 0x040016DA RID: 5850
	public float averageEmotionality;

	// Token: 0x040016DB RID: 5851
	public float averageExtraversion;

	// Token: 0x040016DC RID: 5852
	public float averageAgreeableness;

	// Token: 0x040016DD RID: 5853
	public float averageConscientiousness;

	// Token: 0x040016DE RID: 5854
	public float maxConscientiousness;

	// Token: 0x040016DF RID: 5855
	public float averageCreativity;

	// Token: 0x040016E0 RID: 5856
	[Header("For Sale")]
	[NonSerialized]
	public Interactable saleNote;

	// Token: 0x040016E1 RID: 5857
	[Header("Alarms")]
	public List<Interactable> alarms = new List<Interactable>();

	// Token: 0x040016E2 RID: 5858
	public List<Interactable> sentryGuns = new List<Interactable>();

	// Token: 0x040016E3 RID: 5859
	public List<Interactable> otherSecurity = new List<Interactable>();

	// Token: 0x040016E4 RID: 5860
	public bool alarmActive;

	// Token: 0x040016E5 RID: 5861
	public NewBuilding.AlarmTargetMode targetMode;

	// Token: 0x040016E6 RID: 5862
	public float targetModeSetAt;

	// Token: 0x040016E7 RID: 5863
	public List<Human> alarmTargets = new List<Human>();

	// Token: 0x040016E8 RID: 5864
	public float alarmTimer;

	// Token: 0x040016E9 RID: 5865
	public int breakerSecurityID = -1;

	// Token: 0x040016EA RID: 5866
	public int breakerDoorsID = -1;

	// Token: 0x040016EB RID: 5867
	public int breakerLightsID = -1;

	// Token: 0x040016EC RID: 5868
	[NonSerialized]
	public Interactable breakerSecurity;

	// Token: 0x040016ED RID: 5869
	[NonSerialized]
	public Interactable breakerDoors;

	// Token: 0x040016EE RID: 5870
	[NonSerialized]
	public Interactable breakerLights;

	// Token: 0x040016EF RID: 5871
	public float breakerSecurityState = 1f;

	// Token: 0x040016F0 RID: 5872
	public float breakerLightsState = 1f;

	// Token: 0x040016F1 RID: 5873
	public float breakerDoorsState = 1f;

	// Token: 0x040016F2 RID: 5874
	[Header("AI Navigation")]
	public Dictionary<NewAddress.PathKey, List<NewNode.NodeAccess>> internalRoutes = new Dictionary<NewAddress.PathKey, List<NewNode.NodeAccess>>();

	// Token: 0x040016F3 RID: 5875
	public bool generatedEntranceWeights;

	// Token: 0x040016F4 RID: 5876
	public NativeMultiHashMap<float3, int> accessRef;

	// Token: 0x040016F5 RID: 5877
	public NativeHashMap<int, float3> accessPositions;

	// Token: 0x040016F6 RID: 5878
	public NativeHashMap<int, float3> toNodeReference;

	// Token: 0x040016F7 RID: 5879
	public NativeList<float3> noPassRef;

	// Token: 0x040016F8 RID: 5880
	[Header("Passwords")]
	public GameplayController.Passcode passcode;

	// Token: 0x040016F9 RID: 5881
	public EvidenceMultiPage calendar;

	// Token: 0x040016FA RID: 5882
	[Header("Debug")]
	public GameObject floorEditDebugParent;

	// Token: 0x040016FB RID: 5883
	public List<CompanyOpenHoursPreset.CompanyShift> debugCompanyShifts = new List<CompanyOpenHoursPreset.CompanyShift>();

	// Token: 0x02000338 RID: 824
	[Serializable]
	public class Vandalism
	{
		// Token: 0x040016FC RID: 5884
		public float time;

		// Token: 0x040016FD RID: 5885
		public int fine;

		// Token: 0x040016FE RID: 5886
		public int obj = -1;

		// Token: 0x040016FF RID: 5887
		public Vector3 win = new Vector3(0f, -100f, 0f);
	}

	// Token: 0x02000339 RID: 825
	public struct PathKey : IEquatable<NewAddress.PathKey>
	{
		// Token: 0x060012C3 RID: 4803 RVA: 0x0010B84A File Offset: 0x00109A4A
		public PathKey(NewNode locOne, NewNode locTwo)
		{
			this.origin = locOne;
			this.destination = locTwo;
			this.hasHash = false;
			this.hash = 0;
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0010B868 File Offset: 0x00109A68
		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				this.hash = HashCode.Combine<NewNode, NewNode>(this.origin, this.destination);
				this.hasHash = true;
			}
			return this.hash;
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0010B896 File Offset: 0x00109A96
		bool IEquatable<NewAddress.PathKey>.Equals(NewAddress.PathKey other)
		{
			return other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x04001700 RID: 5888
		public NewNode origin;

		// Token: 0x04001701 RID: 5889
		public NewNode destination;

		// Token: 0x04001702 RID: 5890
		private bool hasHash;

		// Token: 0x04001703 RID: 5891
		private int hash;
	}

	// Token: 0x0200033A RID: 826
	public enum AirVent
	{
		// Token: 0x04001705 RID: 5893
		ceiling,
		// Token: 0x04001706 RID: 5894
		wallUpper,
		// Token: 0x04001707 RID: 5895
		wallLower
	}

	// Token: 0x0200033B RID: 827
	public struct AirVentLocation
	{
		// Token: 0x04001708 RID: 5896
		public NewRoom room;

		// Token: 0x04001709 RID: 5897
		public NewAddress.AirVent location;
	}

	// Token: 0x0200033C RID: 828
	public class AddressCalc
	{
		// Token: 0x0400170A RID: 5898
		public AddressPreset preset;

		// Token: 0x0400170B RID: 5899
		public float score;
	}
}
