using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000376 RID: 886
public class NewRoom : Controller, IComparable<NewRoom>, IEquatable<NewRoom>
{
	// Token: 0x060013F9 RID: 5113 RVA: 0x0011D797 File Offset: 0x0011B997
	bool IEquatable<NewRoom>.Equals(NewRoom other)
	{
		return other.GetHashCode() == this.GetHashCode();
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x0011E51F File Offset: 0x0011C71F
	public override int GetHashCode()
	{
		if (!this.hasHash)
		{
			this.hash = base.GetHashCode();
			this.hasHash = true;
		}
		return this.hash;
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0011E544 File Offset: 0x0011C744
	public Color GetShadowTint(Color lightColour, float intensity)
	{
		Color color = Color.black;
		if (this.defaultWallKey.colour3.a >= 0.1f)
		{
			color = Color.Lerp(color, this.defaultWallKey.colour3, intensity);
		}
		if (this.defaultWallKey.colour2.a >= 0.1f)
		{
			color = Color.Lerp(color, this.defaultWallKey.colour2, intensity);
		}
		if (this.defaultWallKey.colour1.a >= 0.1f)
		{
			color = Color.Lerp(color, this.defaultWallKey.colour1, intensity);
		}
		return Color.Lerp(color, lightColour, intensity);
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0011E5E0 File Offset: 0x0011C7E0
	public void SetupLayoutOnly(NewGameLocation newAddress, RoomTypePreset newRoomType, int loadFloorRoomID = -1)
	{
		if (loadFloorRoomID <= -1)
		{
			this.roomFloorID = NewRoom.assignRoomFloorID;
			NewRoom.assignRoomFloorID++;
		}
		else
		{
			this.roomFloorID = loadFloorRoomID;
		}
		this.roomID = NewRoom.assignRoomID;
		NewRoom.assignRoomID++;
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.keyAtStart = newAddress.seed;
		}
		if (SessionData.Instance.isFloorEdit)
		{
			this.seed = Toolbox.Instance.GenerateUniqueID();
		}
		else
		{
			this.seed = (this.roomID + newAddress.district.districtID).ToString() + CityData.Instance.seed + newRoomType.name + (this.roomID * this.roomID).ToString();
		}
		if (newAddress.building != null)
		{
			if (newAddress.floor != null)
			{
				this.seed += newAddress.floor.floor.ToString();
				this.seed += newAddress.floor.addresses.Count.ToString();
			}
			this.seed += newAddress.building.buildingID.ToString();
		}
		this.SetType(newRoomType);
		this.UpdateEmission = (Action)Delegate.Combine(this.UpdateEmission, new Action(this.UpdateEmissionTex));
		newAddress.AddNewRoom(this);
		base.transform.SetParent(this.gameLocation.gameObject.transform);
		base.transform.localPosition = Vector3.zero;
		this.passcode = new GameplayController.Passcode(GameplayController.PasscodeType.room);
		this.passcode.id = this.roomID;
		if (SessionData.Instance.isFloorEdit)
		{
			this.SetVisible(true, true, "Created at start", false);
		}
		if (!CityData.Instance.roomDictionary.ContainsKey(this.roomID))
		{
			CityData.Instance.roomDictionary.Add(this.roomID, this);
		}
		if (!CityData.Instance.roomDirectory.Contains(this))
		{
			CityData.Instance.roomDirectory.Add(this);
		}
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0011E81C File Offset: 0x0011CA1C
	public void SetupAll(NewGameLocation newAddress, RoomConfiguration newPreset, int loadFloorRoomID = -1)
	{
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.keyAtStart = newAddress.seed;
		}
		this.SetupLayoutOnly(newAddress, newPreset.roomType, loadFloorRoomID);
		this.SetConfiguration(newPreset);
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0011E858 File Offset: 0x0011CA58
	public void SetConfiguration(RoomConfiguration newPreset)
	{
		this.preset = newPreset;
		if (SessionData.Instance.isFloorEdit)
		{
			Game.Log("Setting room as " + newPreset.presetName, 2);
		}
		this.SetType(this.preset.roomType);
		if (SessionData.Instance.isFloorEdit)
		{
			this.seed = Toolbox.Instance.GenerateUniqueID();
		}
		if (this.preset == CityControls.Instance.nullDefaultRoom)
		{
			this.isNullRoom = true;
		}
		if (this.gameLocation != null && this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.residence != null && this.preset.roomType == InteriorControls.Instance.bedroomType)
		{
			this.gameLocation.thisAsAddress.residence.AddBedroom(this);
		}
		if (this.preset.forceOutside != RoomConfiguration.OutsideSetting.dontChange)
		{
			foreach (NewNode newNode in this.nodes)
			{
				if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceInside)
				{
					newNode.SetAsOutside(false);
				}
				else if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
				{
					newNode.SetAsOutside(true);
				}
			}
		}
		this.allowCoving = this.preset.allowCoving;
		if (SessionData.Instance.isFloorEdit)
		{
			this.SetRoomName();
		}
		if (this.gameLocation.thisAsAddress != null && this.building != null && this.preset == CityControls.Instance.nullDefaultRoom)
		{
			this.isOutsideWindow = true;
		}
		this.SetFloorMaterial(CityControls.Instance.defaultFloorMaterialGroup, CityControls.Instance.defaultFloorMaterialGroup.variations[0], true);
		this.SetCeilingMaterial(CityControls.Instance.defaultCeilingMaterialGroup, CityControls.Instance.defaultFloorMaterialGroup.variations[0], true);
		this.SetWallMaterialDefault(CityControls.Instance.defaultWallMaterialGroup, CityControls.Instance.defaultFloorMaterialGroup.variations[0], true);
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugDecor.Add("Setting Default Wall Material: " + CityControls.Instance.defaultWallMaterialGroup.material.name);
		}
		if (SessionData.Instance.isFloorEdit)
		{
			this.SetVisible(true, true, "Created at start", false);
		}
		this.SetMainLights(this.preset.lightsOnAtStart, "OnStart " + this.preset.lightsOnAtStart.ToString(), null, false, true);
		this.SetupEnvrionment();
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x0011EB20 File Offset: 0x0011CD20
	public void SetType(RoomTypePreset newRoomType)
	{
		if (SessionData.Instance.isFloorEdit && FloorEditController.Instance.roomSelection == this)
		{
			Game.Log("Set room type: " + newRoomType.ToString(), 2);
		}
		this.roomType = newRoomType;
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x0011EB60 File Offset: 0x0011CD60
	public void Load(CitySaveData.RoomCitySave data, NewGameLocation newGameLoc)
	{
		this.name = data.name;
		base.transform.name = this.name;
		this.UpdateEmission = (Action)Delegate.Combine(this.UpdateEmission, new Action(this.UpdateEmissionTex));
		foreach (string text in data.openPlanElements)
		{
			RoomConfiguration roomConfiguration = null;
			Toolbox.Instance.LoadDataFromResources<RoomConfiguration>(text, out roomConfiguration);
			if (roomConfiguration != null)
			{
				this.openPlanElements.Add(roomConfiguration);
			}
		}
		this.roomFloorID = data.floorID;
		this.roomID = data.id;
		NewRoom.assignRoomID = Mathf.Max(NewRoom.assignRoomID, this.roomID + 1);
		this.furnitureAssignID = data.fID;
		this.interactableAssignID = data.iID;
		this.isBaseNullRoom = data.isBaseNullRoom;
		this.passcode = data.password;
		this.ceilingFans = data.cf;
		newGameLoc.AddNewRoom(this);
		if (this.isBaseNullRoom)
		{
			newGameLoc.nullRoom = this;
		}
		Toolbox.Instance.LoadDataFromResources<RoomConfiguration>(data.preset, out this.preset);
		this.SetConfiguration(this.preset);
		this.reachableFromEntrance = data.reachableFromEntrance;
		this.isOutsideWindow = data.isOutsideWindow;
		this.allowCoving = data.allowCoving;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.floorMaterial, out this.floorMaterial);
		this.floorMatKey = data.floorMatKey;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.ceilingMaterial, out this.ceilingMaterial);
		this.ceilingMatKey = data.ceilingMatKey;
		Toolbox.Instance.LoadDataFromResources<MaterialGroupPreset>(data.defaultWallMaterial, out this.defaultWallMaterial);
		this.defaultWallKey = data.defaultWallKey;
		this.miscKey = data.miscKey;
		if (data.colourScheme != null && data.colourScheme.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<ColourSchemePreset>(data.colourScheme, out this.colourScheme);
		}
		if (data.mainLightPreset != null && data.mainLightPreset.Length > 0)
		{
			Toolbox.Instance.LoadDataFromResources<RoomLightingPreset>(data.mainLightPreset, out this.mainLightPreset);
		}
		this.hasBeenDecorated = true;
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			List<string> list = this.debugDecor;
			string[] array = new string[6];
			array[0] = "Setting loaded materials: ";
			int num = 1;
			MaterialGroupPreset materialGroupPreset = this.defaultWallMaterial;
			array[num] = ((materialGroupPreset != null) ? materialGroupPreset.ToString() : null);
			array[2] = ", ";
			int num2 = 3;
			Material material = this.floorMat;
			array[num2] = ((material != null) ? material.ToString() : null);
			array[4] = ", ";
			int num3 = 5;
			MaterialGroupPreset materialGroupPreset2 = this.ceilingMaterial;
			array[num3] = ((materialGroupPreset2 != null) ? materialGroupPreset2.ToString() : null);
			list.Add(string.Concat(array));
		}
		this.middleRoomPosition = data.middle;
		this.loadBelongsTo.AddRange(data.owners);
		base.transform.SetParent(this.gameLocation.gameObject.transform);
		base.transform.localPosition = Vector3.zero;
		this.SetMainLights(this.preset.lightsOnAtStart, "OnLoad " + this.preset.lightsOnAtStart.ToString(), null, false, true);
		if (CityData.Instance.roomDictionary.ContainsKey(this.roomID))
		{
			if (CityData.Instance.roomDictionary[this.roomID] != this)
			{
				NewRoom newRoom = CityData.Instance.roomDictionary[this.roomID];
				CityData.Instance.roomDirectory.Remove(newRoom);
				CityData.Instance.roomDictionary.Remove(this.roomID);
				newRoom.roomID = -1;
				CityData.Instance.roomDictionary.Add(this.roomID, this);
				CityData.Instance.roomDirectory.Add(this);
			}
		}
		else
		{
			CityData.Instance.roomDictionary.Add(this.roomID, this);
			CityData.Instance.roomDirectory.Add(this);
		}
		foreach (CitySaveData.NodeCitySave nodeCitySave in data.nodes)
		{
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(nodeCitySave.nc, ref newNode))
			{
				newNode.Load(nodeCitySave, this);
			}
		}
		foreach (CitySaveData.AirVentSave airVentSave in data.airVents)
		{
			AirDuctGroup.AirVent airVent = new AirDuctGroup.AirVent(airVentSave);
			airVent.ventID = airVentSave.id;
			airVent.room = this;
			Vector3Int node = airVentSave.node;
			PathFinder.Instance.nodeMap.TryGetValue(airVentSave.node, ref airVent.node);
			Vector3Int rNode = airVentSave.rNode;
			PathFinder.Instance.nodeMap.TryGetValue(airVentSave.rNode, ref airVent.roomNode);
			if (airVentSave.wall > -1)
			{
				if (CityConstructor.Instance.loadingWallsReference.ContainsKey(airVentSave.wall))
				{
					airVent.wall = CityConstructor.Instance.loadingWallsReference[airVentSave.wall];
					this.airVents.Add(airVent);
					this.LoadVent(airVent);
				}
				else
				{
					Game.LogError("Unable to load vent " + airVentSave.id.ToString() + " as unable to find wall ID " + airVentSave.wall.ToString(), 2);
				}
			}
			else
			{
				this.airVents.Add(airVent);
				this.LoadVent(airVent);
			}
		}
		foreach (CitySaveData.LightZoneSave lightZoneSave in data.lightZones)
		{
			List<NewNode> list2 = new List<NewNode>();
			foreach (Vector3Int vector3Int in lightZoneSave.n)
			{
				NewNode newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
				{
					list2.Add(newNode2);
				}
			}
			NewRoom.LightZoneData lightZoneData = new NewRoom.LightZoneData(this, list2);
			lightZoneData.areaLightColour = lightZoneSave.areaLightColour;
			lightZoneData.areaLightBrightness = lightZoneSave.areaLightBright;
			this.lightZones.Add(lightZoneData);
		}
		foreach (CitySaveData.FurnitureClusterCitySave furnitureClusterCitySave in data.f)
		{
			FurnitureCluster newPreset = null;
			Toolbox.Instance.LoadDataFromResources<FurnitureCluster>(furnitureClusterCitySave.cluster, out newPreset);
			if (PathFinder.Instance.nodeMap.ContainsKey(furnitureClusterCitySave.anchorNode))
			{
				FurnitureClusterLocation furnitureClusterLocation = new FurnitureClusterLocation(PathFinder.Instance.nodeMap[furnitureClusterCitySave.anchorNode], newPreset, furnitureClusterCitySave.angle, furnitureClusterCitySave.ranking);
				foreach (CitySaveData.FurnitureClusterObjectCitySave furnitureClusterObjectCitySave in furnitureClusterCitySave.objs)
				{
					List<FurnitureClass> list3 = new List<FurnitureClass>();
					foreach (string text2 in furnitureClusterObjectCitySave.furnitureClasses)
					{
						FurnitureClass furnitureClass = null;
						Toolbox.Instance.LoadDataFromResources<FurnitureClass>(text2, out furnitureClass);
						if (furnitureClass != null)
						{
							list3.Add(furnitureClass);
						}
					}
					List<NewNode> list4 = new List<NewNode>();
					foreach (Vector3Int vector3Int2 in furnitureClusterObjectCitySave.coversNodes)
					{
						list4.Add(PathFinder.Instance.nodeMap[vector3Int2]);
					}
					FurnitureLocation furnitureLocation = new FurnitureLocation(furnitureClusterObjectCitySave.id, furnitureClusterLocation, list3, furnitureClusterObjectCitySave.angle, PathFinder.Instance.nodeMap[furnitureClusterObjectCitySave.anchorNode], list4, furnitureClusterObjectCitySave.useFOVBLock, furnitureClusterObjectCitySave.fovDirection, furnitureClusterObjectCitySave.fovMaxDistance, furnitureClusterObjectCitySave.scale, furnitureClusterObjectCitySave.up, furnitureClusterObjectCitySave.offset);
					Toolbox.Instance.LoadDataFromResources<FurniturePreset>(furnitureClusterObjectCitySave.furniture, out furnitureLocation.furniture);
					if (furnitureClusterObjectCitySave.art != null && furnitureClusterObjectCitySave.art.Length > 0)
					{
						Toolbox.Instance.LoadDataFromResources<ArtPreset>(furnitureClusterObjectCitySave.art, out furnitureLocation.art);
						furnitureLocation.pickedArt = true;
					}
					furnitureLocation.matKey = furnitureClusterObjectCitySave.matKey;
					furnitureLocation.artMatKey = furnitureClusterObjectCitySave.artMatKet;
					furnitureLocation.pickedMaterials = true;
					foreach (NewNode newNode3 in furnitureLocation.coversNodes)
					{
						if (!furnitureClusterLocation.clusterObjectMap.ContainsKey(newNode3))
						{
							furnitureClusterLocation.clusterObjectMap.Add(newNode3, new List<FurnitureLocation>());
						}
						furnitureClusterLocation.clusterObjectMap[newNode3].Add(furnitureLocation);
						if (!furnitureClusterLocation.clusterList.Contains(furnitureLocation))
						{
							furnitureClusterLocation.clusterList.Add(furnitureLocation);
						}
					}
					furnitureLocation.loadOwners.AddRange(furnitureClusterObjectCitySave.owners);
					furnitureLocation.pickedOwners = true;
					if (!CityConstructor.Instance.loadingFurnitureReference.ContainsKey(furnitureLocation.id))
					{
						CityConstructor.Instance.loadingFurnitureReference.Add(furnitureLocation.id, furnitureLocation);
					}
				}
				this.AddFurniture(furnitureClusterLocation, false, true, false);
			}
			else
			{
				string text3 = "Unable to find node for furniture cluster save! ";
				Vector3Int anchorNode = furnitureClusterCitySave.anchorNode;
				Game.LogError(text3 + anchorNode.ToString() + " Attempting to scan node map... " + PathFinder.Instance.nodeMap.Count.ToString(), 2);
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					foreach (KeyValuePair<Vector3, NewNode> keyValuePair in PathFinder.Instance.nodeMap)
					{
						if (Mathf.RoundToInt(keyValuePair.Key.x) == Mathf.RoundToInt((float)furnitureClusterCitySave.anchorNode.x) && Mathf.RoundToInt(keyValuePair.Key.y) == Mathf.RoundToInt((float)furnitureClusterCitySave.anchorNode.y) && Mathf.RoundToInt(keyValuePair.Key.z) == Mathf.RoundToInt((float)furnitureClusterCitySave.anchorNode.z))
						{
							Game.LogError(string.Concat(new string[]
							{
								"Found the proper node!! Something isn't right here...",
								Mathf.RoundToInt(keyValuePair.Key.x).ToString(),
								" ",
								Mathf.RoundToInt(keyValuePair.Key.y).ToString(),
								" ",
								Mathf.RoundToInt(keyValuePair.Key.z).ToString()
							}), 2);
						}
					}
				}
			}
		}
		if (!Game.Instance.enableNewRealtimeTimeCullingSystem)
		{
			this.ct = data.cullTree;
		}
		this.above = data.above;
		this.below = data.below;
		this.adj = data.adj;
		this.occ = data.occ;
		if (!this.calculatedWorldPos || this.middleRoomPosition == Vector3.zero)
		{
			this.UpdateWorldPositionAndBoundsSize();
			this.middleRoomPosition = this.worldPos;
		}
		this.SetupEnvrionment();
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x0011F830 File Offset: 0x0011DA30
	public void LoadCullingTree()
	{
		if (Game.Instance.enableNewRealtimeTimeCullingSystem)
		{
			return;
		}
		if (Game.Instance.generateCullingInGame)
		{
			return;
		}
		if (this.ct == null)
		{
			Game.LogError("Room " + this.roomID.ToString() + " has not been loaded...", 2);
			return;
		}
		foreach (CitySaveData.CullTreeSave cullTreeSave in this.ct)
		{
			NewRoom newRoom = null;
			if (cullTreeSave.r >= 0)
			{
				if (!CityData.Instance.roomDictionary.TryGetValue(cullTreeSave.r, ref newRoom))
				{
					Game.LogError("Unable to find room id " + cullTreeSave.r.ToString() + " in room dictionary...", 2);
				}
				else
				{
					NewRoom.CullTreeEntry cullTreeEntry = new NewRoom.CullTreeEntry(cullTreeSave.d);
					if (!this.cullingTree.ContainsKey(newRoom))
					{
						this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
					}
					this.cullingTree[newRoom].Add(cullTreeEntry);
					if (cullTreeSave.d != null)
					{
						foreach (int num in cullTreeSave.d)
						{
							if (!this.doorCheckSet.Contains(num))
							{
								this.doorCheckSet.Add(num);
							}
						}
					}
					this.loadedCullTreeFromSave = true;
				}
			}
		}
		foreach (int num2 in this.above)
		{
			this.aboveRooms.Add(CityData.Instance.roomDictionary[num2]);
		}
		foreach (int num3 in this.below)
		{
			this.belowRooms.Add(CityData.Instance.roomDictionary[num3]);
		}
		foreach (int num4 in this.adj)
		{
			this.adjacentRooms.Add(CityData.Instance.roomDictionary[num4]);
		}
		foreach (int num5 in this.occ)
		{
			this.nonAudioOccludedRooms.Add(CityData.Instance.roomDictionary[num5]);
		}
		this.ct = null;
		this.above = null;
		this.below = null;
		this.adj = null;
		this.occ = null;
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x0011FB8C File Offset: 0x0011DD8C
	public void UpdateColourSchemeAndMaterials()
	{
		if (this.gameLocation.isOutside)
		{
			return;
		}
		string seedInput = this.roomID.ToString();
		List<ColourSchemePreset> list = new List<ColourSchemePreset>();
		bool flag = false;
		if (!SessionData.Instance.isFloorEdit)
		{
			if (this.preset.decorSetting == RoomConfiguration.DecorSetting.borrowFromBuilding)
			{
				if (this.building != null && Game.Instance.allowEchelons && this.building.preset.buildingFeaturesEchelonFloors && this.floor != null && this.floor.floor >= this.building.preset.echelonFloorStart)
				{
					this.colourScheme = CityControls.Instance.echelonColourScheme;
					this.defaultWallKey = CityData.Instance.echelonDefaultWallKey;
					this.SetWallMaterialDefault(CityControls.Instance.echelonDefaultWallMaterial, null, false);
					this.floorMatKey = CityData.Instance.echelonFloorMatKey;
					this.SetFloorMaterial(CityControls.Instance.echelonFloorMaterial, null, false);
					this.ceilingMatKey = CityData.Instance.echelonCeilingMatKey;
					this.SetCeilingMaterial(CityControls.Instance.echelonCeilingMaterial, null, false);
				}
				else
				{
					this.colourScheme = this.building.colourScheme;
					this.defaultWallKey = this.building.defaultWallKey;
					this.SetWallMaterialDefault(this.building.defaultWallMaterial, null, false);
					this.floorMatKey = this.building.floorMatKey;
					this.SetFloorMaterial(this.building.floorMaterial, null, false);
					this.ceilingMatKey = this.building.ceilingMatKey;
					this.SetCeilingMaterial(this.building.ceilingMaterial, null, false);
				}
				flag = true;
				this.hasBeenDecorated = true;
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.debugDecor.Add("New Decor: Borrowed from building (" + this.building.defaultWallMaterial.name + ")");
				}
			}
			else if (this.preset.decorSetting == RoomConfiguration.DecorSetting.borrowFromBelow)
			{
				foreach (NewNode newNode in this.nodes)
				{
					Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(0, 0, -1);
					NewNode newNode2 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && newNode2.room.hasBeenDecorated && !newNode2.room.preset.excludeFromOthersCopyingDecorStyle)
					{
						this.colourScheme = newNode2.room.colourScheme;
						this.defaultWallKey = newNode2.room.defaultWallKey;
						this.SetWallMaterialDefault(newNode2.room.defaultWallMaterial, null, false);
						this.floorMatKey = newNode2.room.floorMatKey;
						this.SetFloorMaterial(newNode2.room.floorMaterial, null, false);
						this.ceilingMatKey = newNode2.room.ceilingMatKey;
						this.SetCeilingMaterial(newNode2.room.ceilingMaterial, null, false);
						flag = true;
						this.hasBeenDecorated = true;
						if (Game.Instance.devMode && Game.Instance.collectDebugData)
						{
							this.debugDecor.Add("New Decor: Borrowed from below (" + newNode2.room.defaultWallMaterial.name + ")");
							break;
						}
						break;
					}
				}
			}
		}
		if (this.preset.decorSetting != RoomConfiguration.DecorSetting.ownStyle && !flag && this.entrances.Count > 0)
		{
			List<NewRoom> list2 = new List<NewRoom>();
			List<NewRoom> list3 = new List<NewRoom>();
			list2.Add(this);
			int num = 99;
			while (list2.Count > 0 && num > 0)
			{
				list2.Sort();
				list2.Reverse();
				NewRoom newRoom = list2[0];
				if (newRoom.hasBeenDecorated && !newRoom.preset.excludeFromOthersCopyingDecorStyle)
				{
					this.colourScheme = newRoom.colourScheme;
					this.defaultWallKey = newRoom.defaultWallKey;
					this.SetWallMaterialDefault(newRoom.defaultWallMaterial, null, false);
					this.floorMatKey = newRoom.floorMatKey;
					this.SetFloorMaterial(newRoom.floorMaterial, null, false);
					this.ceilingMatKey = newRoom.ceilingMatKey;
					this.SetCeilingMaterial(newRoom.ceilingMaterial, null, false);
					flag = true;
					this.hasBeenDecorated = true;
					return;
				}
				if (newRoom.entrances.Count > 0)
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
					{
						if (nodeAccess.toNode.gameLocation == this.gameLocation && !list2.Contains(nodeAccess.toNode.room) && !list3.Contains(nodeAccess.toNode.room))
						{
							list2.Add(nodeAccess.toNode.room);
						}
					}
				}
				list3.Add(newRoom);
				list2.RemoveAt(0);
				num--;
			}
		}
		if (this.preset.chanceOfOverrideMatIfGroundFloor > 0f || this.preset.chanceOfOverrideMatIfStairwell > 0f || this.preset.chanceOfOverrideMatIfBasement > 0f)
		{
			bool flag2 = false;
			bool flag3 = false;
			if (this.preset.chanceOfOverrideMatIfGroundFloor > 0f && this.floor.floor == 0)
			{
				if (!this.building.triedGroundFloorRandom && this.preset.chanceOfOverrideMatIfGroundFloor >= Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput))
				{
					this.building.groundFloorOverride = true;
				}
				this.building.triedGroundFloorRandom = true;
				flag2 = this.building.groundFloorOverride;
			}
			else if (this.preset.chanceOfOverrideMatIfBasement > 0f && this.floor.floor < 0)
			{
				if (!this.building.triedBasementRandom && this.preset.chanceOfOverrideMatIfBasement >= Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput))
				{
					this.building.basementFloorOverride = true;
				}
				this.building.triedBasementRandom = true;
				flag3 = this.building.basementFloorOverride;
			}
			if (this.preset.chanceOfOverrideMatIfStairwell > 0f)
			{
				using (HashSet<NewNode>.Enumerator enumerator = this.nodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.tile.isStairwell)
						{
							if (this.preset.chanceOfOverrideMatIfStairwell >= Toolbox.Instance.RandContained(0f, 1f, seedInput, out seedInput))
							{
								flag2 = true;
								break;
							}
							break;
						}
					}
				}
			}
			if (flag2)
			{
				if (this.preset.floorOverrides.Count > 0 || this.building.floorMaterialOverride != null)
				{
					if (this.building.floorMaterialOverride == null)
					{
						this.building.floorMaterialOverride = this.preset.floorOverrides[Toolbox.Instance.RandContained(0, this.preset.floorOverrides.Count, seedInput, out seedInput)];
					}
					this.SetFloorMaterial(this.building.floorMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.floorMaterialOverride.name + ")");
					}
				}
				if (this.preset.wallOverrides.Count > 0 || this.building.defaultWallMaterialOverride != null)
				{
					if (this.building.defaultWallMaterialOverride == null)
					{
						this.building.defaultWallMaterialOverride = this.preset.wallOverrides[Toolbox.Instance.RandContained(0, this.preset.wallOverrides.Count, seedInput, out seedInput)];
					}
					this.SetWallMaterialDefault(this.building.defaultWallMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.defaultWallMaterialOverride.name + ")");
					}
				}
				if (this.preset.ceilingOverrides.Count > 0 || this.building.ceilingMaterialOverride != null)
				{
					if (this.building.ceilingMaterialOverride == null)
					{
						this.building.ceilingMaterialOverride = this.preset.ceilingOverrides[Toolbox.Instance.RandContained(0, this.preset.ceilingOverrides.Count, seedInput, out seedInput)];
					}
					this.SetCeilingMaterial(this.building.ceilingMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.ceilingMaterialOverride.name + ")");
					}
				}
			}
			else if (flag3)
			{
				if (this.preset.floorOverrides.Count > 0 || this.building.basementFloorMaterialOverride != null)
				{
					if (this.building.basementFloorMaterialOverride == null)
					{
						this.building.basementFloorMaterialOverride = this.preset.floorOverrides[Toolbox.Instance.RandContained(0, this.preset.floorOverrides.Count, seedInput, out seedInput)];
					}
					this.SetFloorMaterial(this.building.basementFloorMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.basementFloorMaterialOverride.name + ")");
					}
				}
				if (this.preset.wallOverrides.Count > 0 || this.building.basementDefaultWallMaterialOverride != null)
				{
					if (this.building.basementDefaultWallMaterialOverride == null)
					{
						this.building.basementDefaultWallMaterialOverride = this.preset.wallOverrides[Toolbox.Instance.RandContained(0, this.preset.wallOverrides.Count, seedInput, out seedInput)];
					}
					this.SetWallMaterialDefault(this.building.basementDefaultWallMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.basementDefaultWallMaterialOverride.name + ")");
					}
				}
				if (this.preset.ceilingOverrides.Count > 0 || this.building.basementCeilingMaterialOverride != null)
				{
					if (this.building.basementCeilingMaterialOverride == null)
					{
						this.building.basementCeilingMaterialOverride = this.preset.ceilingOverrides[Toolbox.Instance.RandContained(0, this.preset.ceilingOverrides.Count, seedInput, out seedInput)];
					}
					this.SetCeilingMaterial(this.building.basementCeilingMaterialOverride, null, false);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.debugDecor.Add("New Decor: Building override from preset (" + this.building.basementCeilingMaterialOverride.name + ")");
					}
				}
			}
		}
		if (!flag)
		{
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				this.debugDecor.Add("New Decor: Generate own decor style...");
			}
			if (this.preset.forceColourSchemes.Count > 0)
			{
				list.AddRange(this.preset.forceColourSchemes);
			}
			else
			{
				foreach (ColourSchemePreset colourSchemePreset in Toolbox.Instance.allColourSchemes)
				{
					if (this.gameLocation.designStyle == null)
					{
						Game.Log("this as address", 2);
					}
					float num2 = (float)(10 - Mathf.RoundToInt(Mathf.Abs((float)colourSchemePreset.modernity - ((float)this.gameLocation.designStyle.modernity * 0.9f + this.gameLocation.thisAsAddress.averageCreativity))));
					int num3 = 10 - Mathf.RoundToInt((float)Mathf.Abs(colourSchemePreset.cleanness - this.preset.cleanness));
					int num4 = 10 - Mathf.RoundToInt(Mathf.Abs((float)colourSchemePreset.loudness - (this.gameLocation.thisAsAddress.averageExtraversion * 7.5f + (1f - this.gameLocation.thisAsAddress.averageAgreeableness) * 2.5f)));
					int num5 = 10 - Mathf.RoundToInt(Mathf.Abs((float)colourSchemePreset.emotive - (this.gameLocation.thisAsAddress.averageEmotionality * 7f + this.gameLocation.thisAsAddress.averageCreativity * 3f)));
					int num6 = Mathf.FloorToInt((num2 + (float)num3 + (float)num4 + (float)num5) / 8f);
					for (int i = 0; i < num6; i++)
					{
						list.Add(colourSchemePreset);
					}
				}
			}
			if (list.Count <= 0)
			{
				list.Add(CityControls.Instance.fallbackColourScheme);
			}
			this.colourScheme = list[Toolbox.Instance.RandContained(0, list.Count, seedInput, out seedInput)];
			float normalizedLandValue = Toolbox.Instance.GetNormalizedLandValue(this.gameLocation, false);
			MaterialGroupPreset materialGroupPreset = Toolbox.Instance.SelectMaterial(this.preset.roomClass, normalizedLandValue, this.gameLocation.designStyle, MaterialGroupPreset.MaterialType.walls, seedInput, out seedInput);
			MaterialGroupPreset.MaterialVariation newVar = materialGroupPreset.variations[Toolbox.Instance.RandContained(0, materialGroupPreset.variations.Count, seedInput, out seedInput)];
			this.SetWallMaterialDefault(materialGroupPreset, newVar, true);
			MaterialGroupPreset materialGroupPreset2 = Toolbox.Instance.SelectMaterial(this.preset.roomClass, normalizedLandValue, this.gameLocation.designStyle, MaterialGroupPreset.MaterialType.ceiling, seedInput, out seedInput);
			MaterialGroupPreset.MaterialVariation newVar2 = materialGroupPreset2.variations[Toolbox.Instance.RandContained(0, materialGroupPreset2.variations.Count, seedInput, out seedInput)];
			this.SetCeilingMaterial(materialGroupPreset2, newVar2, true);
			MaterialGroupPreset materialGroupPreset3 = Toolbox.Instance.SelectMaterial(this.preset.roomClass, normalizedLandValue, this.gameLocation.designStyle, MaterialGroupPreset.MaterialType.floor, seedInput, out seedInput);
			MaterialGroupPreset.MaterialVariation newVar3 = materialGroupPreset3.variations[Toolbox.Instance.RandContained(0, materialGroupPreset3.variations.Count, seedInput, out seedInput)];
			this.SetFloorMaterial(materialGroupPreset3, newVar3, true);
			this.hasBeenDecorated = true;
		}
		this.miscKey = MaterialsController.Instance.GenerateMaterialKey(InteriorControls.Instance.defaultVariation, this.colourScheme, this, false, null);
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00120AD0 File Offset: 0x0011ECD0
	public void AddNewNode(NewNode newNode)
	{
		if (!this.nodes.Contains(newNode))
		{
			if (newNode.room != null)
			{
				newNode.room.RemoveNode(newNode);
			}
			if (newNode.gameLocation != null)
			{
				newNode.gameLocation.RemoveNode(newNode);
			}
			this.nodes.Add(newNode);
			if (this.roomType.overrideFloorHeight)
			{
				newNode.SetFloorHeight(this.roomType.floorHeight);
			}
			else if (this.floor != null)
			{
				newNode.SetFloorHeight(this.floor.defaultFloorHeight);
			}
			newNode.room = this;
			newNode.gameLocation = this.gameLocation;
			newNode.floor = this.floor;
			newNode.building = this.building;
			this.gameLocation.nodes.Add(newNode);
			foreach (NewWall newWall in newNode.walls)
			{
				if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
				{
					if (newWall.parentWall.node.room != null)
					{
						newWall.parentWall.node.room.AddEntrance(newWall.parentWall.node, newWall.childWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
					}
					if (newWall.childWall.node.room != null)
					{
						newWall.childWall.node.room.AddEntrance(newWall.childWall.node, newWall.parentWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
					}
				}
				else
				{
					if (newWall.parentWall.node.room != null)
					{
						newWall.parentWall.node.room.RemoveEntrance(newWall.parentWall.node, newWall.childWall.node);
					}
					if (newWall.childWall.node.room != null)
					{
						newWall.childWall.node.room.RemoveEntrance(newWall.childWall.node, newWall.parentWall.node);
					}
				}
			}
			if (SessionData.Instance.isFloorEdit || SessionData.Instance.isTestScene || CityConstructor.Instance.generateNew)
			{
				GenerationController.Instance.UpdateGeometryFloor(this.floor, "NewRoom");
			}
			if (SessionData.Instance.isFloorEdit)
			{
				this.SetRoomName();
			}
			if (newNode.tile.isStairwell && newNode.tile.stairwell != null)
			{
				newNode.tile.stairwell.transform.SetParent(base.transform, true);
			}
			else if (newNode.tile.isInvertedStairwell && newNode.tile.elevator != null)
			{
				newNode.tile.elevator.transform.SetParent(base.transform, true);
			}
			if (newNode.floorType == NewNode.FloorTileType.CeilingOnly || newNode.floorType == NewNode.FloorTileType.noneButIndoors)
			{
				int num = 99;
				for (int i = 1; i < num; i++)
				{
					Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(0, 0, -i);
					NewNode newNode2 = null;
					if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2))
					{
						break;
					}
					if (newNode2.floorType == NewNode.FloorTileType.floorOnly)
					{
						this.SetLowerRoom(newNode2.room);
						break;
					}
					if (newNode2.floorType != NewNode.FloorTileType.noneButIndoors)
					{
						break;
					}
					newNode2.room.allowCoving = false;
				}
			}
			if (this.preset != null)
			{
				if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceInside)
				{
					newNode.SetAsOutside(false);
					return;
				}
				if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
				{
					newNode.SetAsOutside(true);
				}
			}
		}
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x00120EA8 File Offset: 0x0011F0A8
	public string GetName()
	{
		string result = string.Empty;
		if (this.gameLocation.thisAsAddress != null)
		{
			if (this.gameLocation.isLobby)
			{
				result = this.gameLocation.name;
			}
			else if (this.preset != null)
			{
				result = this.gameLocation.name + " " + Strings.Get("names.rooms", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
		}
		else
		{
			result = this.gameLocation.name;
		}
		return result;
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x00120F38 File Offset: 0x0011F138
	public void SetRoomName()
	{
		if (this.gameLocation.thisAsAddress != null)
		{
			if (this.gameLocation.isLobby)
			{
				this.name = this.gameLocation.name;
			}
			else if (this.preset != null)
			{
				this.name = this.gameLocation.name + " " + Strings.Get("names.rooms", this.preset.name, Strings.Casing.asIs, false, false, false, null);
			}
		}
		else
		{
			this.name = this.gameLocation.name;
		}
		base.gameObject.name = this.name;
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x00120FE0 File Offset: 0x0011F1E0
	public void RemoveNode(NewNode newNode)
	{
		if (this.nodes.Contains(newNode))
		{
			newNode.accessToOtherNodes.Clear();
			foreach (NewWall newWall in newNode.walls)
			{
				if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
				{
					try
					{
						if (newWall.parentWall.node.room != null && newWall.childWall.node.room != null)
						{
							newWall.parentWall.node.room.RemoveEntrance(newWall.parentWall.node, newWall.childWall.node);
						}
						if (newWall.childWall.node.room != null && newWall.parentWall.node.room != null)
						{
							newWall.childWall.node.room.RemoveEntrance(newWall.childWall.node, newWall.parentWall.node);
						}
					}
					catch
					{
					}
				}
			}
			this.nodes.Remove(newNode);
			if (this.roomType.overrideFloorHeight)
			{
				newNode.SetFloorHeight(this.gameLocation.floor.defaultFloorHeight);
			}
			this.gameLocation.RemoveNode(newNode);
			newNode.room = null;
			newNode.gameLocation = null;
			newNode.floor = null;
			newNode.building = null;
			if (SessionData.Instance.isFloorEdit || SessionData.Instance.isTestScene || CityConstructor.Instance.generateNew)
			{
				GenerationController.Instance.UpdateGeometryFloor(this.floor, "NewRoom");
			}
			if (SessionData.Instance.isFloorEdit)
			{
				this.SetRoomName();
			}
		}
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x001211CC File Offset: 0x0011F3CC
	public void UpdateWorldPositionAndBoundsSize()
	{
		int num = 9999;
		int num2 = 9999;
		int num3 = -1;
		int num4 = -1;
		this.worldPos = Vector3.zero;
		foreach (NewNode newNode in this.nodes)
		{
			num = Mathf.Min(num, newNode.nodeCoord.x);
			num2 = Mathf.Min(num2, newNode.nodeCoord.y);
			num3 = Mathf.Max(num3, newNode.nodeCoord.x);
			num4 = Mathf.Max(num4, newNode.nodeCoord.y);
			this.worldPos += newNode.position;
		}
		if (this.nodes.Count > 0)
		{
			this.worldPos /= (float)this.nodes.Count;
		}
		this.boundsSize = new Vector2((float)(num3 - num + 1), (float)(num4 - num2 + 1));
		this.calculatedWorldPos = true;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x001212E4 File Offset: 0x0011F4E4
	public void AddOpenPlanElement(RoomConfiguration newElement)
	{
		if (!this.openPlanElements.Contains(newElement))
		{
			this.openPlanElements.Add(newElement);
		}
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x00121300 File Offset: 0x0011F500
	public void SetFloorMaterial(MaterialGroupPreset newMat, MaterialGroupPreset.MaterialVariation newVar, bool getNewKey = true)
	{
		this.floorMaterial = newMat;
		if (getNewKey)
		{
			this.floorMatKey = MaterialsController.Instance.GenerateMaterialKey(newVar, this.colourScheme, this, true, null);
		}
		if (this.combinedFloor == null)
		{
			using (HashSet<NewNode>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewNode newNode = enumerator.Current;
					newNode.room.floorMat = MaterialsController.Instance.SetMaterialGroup(newNode.spawnedFloor, newMat, this.floorMatKey, false, null);
				}
				return;
			}
		}
		this.floorMat = MaterialsController.Instance.SetMaterialGroup(this.combinedFloor, newMat, this.floorMatKey, false, null);
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x001213C4 File Offset: 0x0011F5C4
	public void SetCeilingMaterial(MaterialGroupPreset newMat, MaterialGroupPreset.MaterialVariation newVar, bool getNewKey = true)
	{
		this.ceilingMaterial = newMat;
		if (getNewKey)
		{
			this.ceilingMatKey = MaterialsController.Instance.GenerateMaterialKey(newVar, this.colourScheme, this, true, null);
		}
		if (this.combinedCeiling == null)
		{
			if (this.ceilingMat != null && this.preset.boostCeilingEmission)
			{
				Object.Destroy(this.ceilingMat);
			}
			using (HashSet<NewNode>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewNode newNode = enumerator.Current;
					if (this.ceilingMat == null)
					{
						this.ceilingMat = MaterialsController.Instance.SetMaterialGroup(newNode.spawnedCeiling, newMat, this.ceilingMatKey, this.preset.boostCeilingEmission, null);
						this.uniqueCeilingMaterial = this.preset.boostCeilingEmission;
						if (this.preset.boostCeilingEmission && this.ceilingMat != null && this.uniqueCeilingMaterial)
						{
							if (this.mainLightStatus)
							{
								Color color = Color.white;
								if (this.lightZones.Count > 0)
								{
									color = this.lightZones[0].areaLightColour;
								}
								this.ceilingMat.SetColor("_EmissiveColor", this.preset.ceilingEmissionBoost * color);
							}
							else
							{
								this.ceilingMat.SetColor("_EmissiveColor", Color.black);
							}
						}
					}
					else
					{
						MaterialsController.Instance.ApplyMaterial(newNode.spawnedCeiling, this.ceilingMat);
					}
				}
				return;
			}
		}
		if (this.ceilingMat != null && this.preset.boostCeilingEmission)
		{
			Object.Destroy(this.ceilingMat);
		}
		this.ceilingMat = MaterialsController.Instance.SetMaterialGroup(this.combinedCeiling, newMat, this.ceilingMatKey, this.preset.boostCeilingEmission, null);
		this.uniqueCeilingMaterial = this.preset.boostCeilingEmission;
		if (this.preset.boostCeilingEmission && this.ceilingMat != null && this.uniqueCeilingMaterial)
		{
			if (this.mainLightStatus)
			{
				Color color2 = Color.white;
				if (this.lightZones.Count > 0)
				{
					color2 = this.lightZones[0].areaLightColour;
				}
				this.ceilingMat.SetColor("_EmissiveColor", this.preset.ceilingEmissionBoost * color2);
				return;
			}
			this.ceilingMat.SetColor("_EmissiveColor", Color.black);
		}
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0012165C File Offset: 0x0011F85C
	public void SetWallMaterialDefault(MaterialGroupPreset newMat, MaterialGroupPreset.MaterialVariation newVar, bool getNewKey = true)
	{
		this.defaultWallMaterial = newMat;
		if (getNewKey)
		{
			this.defaultWallKey = MaterialsController.Instance.GenerateMaterialKey(newVar, this.colourScheme, this, true, null);
		}
		if (this.combinedWalls == null)
		{
			using (HashSet<NewNode>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewNode newNode = enumerator.Current;
					foreach (NewWall newWall in newNode.walls)
					{
						newWall.SetWallMaterial(newMat, this.defaultWallKey);
					}
				}
				return;
			}
		}
		if (Enumerable.FirstOrDefault<NewNode>(Enumerable.Where<NewNode>(this.nodes, (NewNode item) => item.floorType == NewNode.FloorTileType.CeilingOnly || item.floorType == NewNode.FloorTileType.noneButIndoors || item.tile.isStairwell || item.tile.isInvertedStairwell)) != null && newMat != null && newMat.noFloorReplacement != null)
		{
			newMat = newMat.noFloorReplacement;
		}
		this.wallMat = MaterialsController.Instance.SetMaterialGroup(this.combinedWalls, newMat, this.defaultWallKey, false, null);
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x00121790 File Offset: 0x0011F990
	public void ToggleMainLights(Actor actor = null)
	{
		if (this.mainLightStatus)
		{
			this.SetMainLights(false, "Toggle false", actor, false, false);
			return;
		}
		this.SetMainLights(true, "Toggle true", actor, false, false);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x001217BC File Offset: 0x0011F9BC
	public void SetMainLights(bool newVal, string debug, Actor actor = null, bool forceInstant = false, bool forceUpdate = false)
	{
		bool flag = false;
		if (newVal != this.mainLightStatus || forceUpdate)
		{
			flag = true;
		}
		this.mainLightStatus = newVal;
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugLightswitches.Add("TURN " + newVal.ToString() + " : " + debug);
		}
		foreach (Interactable interactable in this.mainLights)
		{
			interactable.SetSwitchState(this.mainLightStatus, actor, true, false, forceInstant);
		}
		foreach (NewWall newWall in this.lightswitches)
		{
			newWall.lightswitchInteractable.SetSwitchState(this.mainLightStatus, actor, true, false, forceInstant);
		}
		if (this.preset.useAdditionalAreaLights)
		{
			foreach (NewRoom.LightZoneData lightZoneData in this.lightZones)
			{
				if (lightZoneData.spawnedAreaLight != null)
				{
					lightZoneData.spawnedAreaLight.gameObject.SetActive(this.mainLightStatus);
				}
			}
		}
		if (this.preset.boostCeilingEmission && this.ceilingMat != null && this.uniqueCeilingMaterial)
		{
			if (this.mainLightStatus)
			{
				Color color = Color.white;
				if (this.lightZones.Count > 0)
				{
					color = this.lightZones[0].areaLightColour;
				}
				this.ceilingMat.SetColor("_EmissiveColor", this.preset.ceilingEmissionBoost * color);
			}
			else
			{
				this.ceilingMat.SetColor("_EmissiveColor", Color.black);
			}
		}
		if (flag)
		{
			this.UpdateEmissionEndOfFrame();
		}
		foreach (NewRoom newRoom in this.commonRooms)
		{
			if (!(newRoom == this))
			{
				newRoom.mainLightStatus = newVal;
				foreach (Interactable interactable2 in newRoom.mainLights)
				{
					interactable2.SetSwitchState(newRoom.mainLightStatus, actor, true, false, forceInstant);
				}
				foreach (NewWall newWall2 in newRoom.lightswitches)
				{
					newWall2.lightswitchInteractable.SetSwitchState(newRoom.mainLightStatus, actor, true, false, forceInstant);
				}
				if (flag)
				{
					newRoom.UpdateEmissionEndOfFrame();
				}
			}
		}
		if (actor != null && this.lightswitches.Count > 0 && actor.isPlayer && Player.Instance.illegalStatus)
		{
			List<NewRoom> list = new List<NewRoom>(this.commonRooms);
			list.Add(this);
			foreach (NewNode.NodeAccess nodeAccess in this.entrances)
			{
				if (nodeAccess.walkingAccess && (nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway || nodeAccess.accessType == NewNode.NodeAccess.AccessType.bannister || (nodeAccess.door != null && !nodeAccess.door.isClosed)) && !list.Contains(nodeAccess.toNode.room))
				{
					list.Add(nodeAccess.toNode.room);
				}
			}
			foreach (NewRoom newRoom2 in list)
			{
				foreach (Actor actor2 in newRoom2.currentOccupants)
				{
					if (!actor2.isDead && actor2.ai != null && !actor2.ai.seesOnPersuit)
					{
						NewWall newWall3 = this.lightswitches[Toolbox.Instance.Rand(0, this.lightswitches.Count, true)];
						if (newWall3 != null && newWall3.node != null)
						{
							Game.Log("...Trigger investigation with lightswitch", 2);
							if (Game.Instance.devMode)
							{
								Debug.DrawRay(actor2.lookAtThisTransform.position, newWall3.node.position - actor2.lookAtThisTransform.position, Color.blue, 1.5f);
							}
							try
							{
								actor2.ai.Investigate(newWall3.node, newWall3.node.position, null, NewAIController.ReactionState.investigatingSight, CitizenControls.Instance.sightingMinInvestigationTimeMP, Player.Instance.trespassingEscalation, false, 1f, null);
							}
							catch
							{
								Game.LogError("Unable to trigger AI investigation into light switch usage!", 2);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x00121DB0 File Offset: 0x0011FFB0
	public void SetSecondaryLight(bool newVal, bool forceUpdate = false)
	{
		bool flag = false;
		if (forceUpdate || newVal != this.GetSecondaryLightStatus())
		{
			flag = true;
		}
		if (flag)
		{
			this.UpdateEmissionEndOfFrame();
		}
		foreach (NewRoom newRoom in this.commonRooms)
		{
			if (!(newRoom == this) && flag)
			{
				newRoom.UpdateEmissionEndOfFrame();
			}
		}
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x00121E28 File Offset: 0x00120028
	public void UpdateEmissionEndOfFrame()
	{
		Toolbox.Instance.InvokeEndOfFrame(this.UpdateEmission, "");
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x00121E40 File Offset: 0x00120040
	public void UpdateEmissionTex()
	{
		if (this.building != null && this.building.preset != null && this.building.preset.emissionMapLit != null && this.building.emissionTextureInstanced != null)
		{
			bool flag = false;
			if (this.mainLightStatus)
			{
				Color color = Color.Lerp(Color.white, this.defaultWallKey.colour1, 0.52f);
				using (List<NewWall>.Enumerator enumerator = this.windowsWithUVData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NewWall newWall = enumerator.Current;
						Color[] pixels = this.building.preset.emissionMapLit.GetPixels((int)newWall.windowUV.originPixel.x, (int)newWall.windowUV.originPixel.y, (int)newWall.windowUV.rectSize.x, (int)newWall.windowUV.rectSize.y);
						for (int i = 0; i < pixels.Length; i++)
						{
							pixels[i] *= color;
						}
						this.building.emissionTextureInstanced.SetPixels((int)newWall.windowUV.originPixel.x, (int)newWall.windowUV.originPixel.y, (int)newWall.windowUV.rectSize.x, (int)newWall.windowUV.rectSize.y, pixels);
						flag = true;
					}
					goto IL_3F2;
				}
			}
			if (this.GetSecondaryLightStatus())
			{
				Color color2 = Color.Lerp(Color.gray, this.defaultWallKey.colour1, 0.52f);
				using (List<NewWall>.Enumerator enumerator = this.windowsWithUVData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NewWall newWall2 = enumerator.Current;
						Color[] pixels2 = this.building.preset.emissionMapLit.GetPixels((int)newWall2.windowUV.originPixel.x, (int)newWall2.windowUV.originPixel.y, (int)newWall2.windowUV.rectSize.x, (int)newWall2.windowUV.rectSize.y);
						for (int j = 0; j < pixels2.Length; j++)
						{
							pixels2[j] *= color2;
						}
						this.building.emissionTextureInstanced.SetPixels((int)newWall2.windowUV.originPixel.x, (int)newWall2.windowUV.originPixel.y, (int)newWall2.windowUV.rectSize.x, (int)newWall2.windowUV.rectSize.y, pixels2);
						flag = true;
					}
					goto IL_3F2;
				}
			}
			foreach (NewWall newWall3 in this.windowsWithUVData)
			{
				try
				{
					Color[] pixels3 = this.building.emissionTextureUnlit.GetPixels((int)newWall3.windowUV.originPixel.x, (int)newWall3.windowUV.originPixel.y, (int)newWall3.windowUV.rectSize.x, (int)newWall3.windowUV.rectSize.y);
					this.building.emissionTextureInstanced.SetPixels((int)newWall3.windowUV.originPixel.x, (int)newWall3.windowUV.originPixel.y, (int)newWall3.windowUV.rectSize.x, (int)newWall3.windowUV.rectSize.y, pixels3);
				}
				catch
				{
					Game.LogError("Error with window texture! Check this is the correct size: " + this.building.preset.name, 2);
				}
				flag = true;
			}
			IL_3F2:
			if (flag && !CitizenBehaviour.Instance.buildingEmissionTexturesToUpdate.Contains(this.building))
			{
				CitizenBehaviour.Instance.buildingEmissionTexturesToUpdate.Add(this.building);
			}
		}
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x001222D4 File Offset: 0x001204D4
	public void AddMainLight(Interactable newLight)
	{
		if (!this.mainLights.Contains(newLight))
		{
			this.mainLights.Add(newLight);
			newLight.SetSwitchState(this.mainLightStatus, null, false, true, false);
		}
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x00122300 File Offset: 0x00120500
	public void AddSecondaryLight(Interactable newLight)
	{
		if (!this.secondaryLights.Contains(newLight))
		{
			this.secondaryLights.Add(newLight);
		}
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x0012231C File Offset: 0x0012051C
	public void AddEntrance(NewNode fromNode, NewNode toNode, bool forceAccessType = false, NewNode.NodeAccess.AccessType forcedAccessType = NewNode.NodeAccess.AccessType.adjacent, bool forceWalkable = false)
	{
		if (!this.entrances.Exists((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode))
		{
			NewDoor newDoorway = null;
			NewWall newWall = fromNode.walls.Find((NewWall item) => item.otherWall != null && item.otherWall.node == toNode && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.wall && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventUpper && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventLower && item.parentWall.preset.sectionClass != DoorPairPreset.WallSectionClass.ventTop);
			if (newWall != null)
			{
				newDoorway = newWall.parentWall.door;
				newWall = newWall.parentWall;
			}
			NewNode.NodeAccess nodeAccess = new NewNode.NodeAccess(fromNode, toNode, newWall, newDoorway, forceAccessType, forcedAccessType, forceWalkable);
			this.entrances.Add(nodeAccess);
		}
		if (fromNode.gameLocation != toNode.gameLocation)
		{
			fromNode.gameLocation.AddEntrance(fromNode, toNode, forceAccessType, forcedAccessType, forceWalkable);
		}
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x001223F0 File Offset: 0x001205F0
	public void RemoveEntrance(NewNode fromNode, NewNode toNode)
	{
		NewNode.NodeAccess nodeAccess = this.entrances.Find((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode);
		if (nodeAccess != null)
		{
			nodeAccess.walkingAccess = false;
			if (!SessionData.Instance.isFloorEdit && Game.Instance.collectDebugData && this.gameLocation.entrances.Exists((NewNode.NodeAccess item) => item.fromNode == fromNode && item.toNode == toNode))
			{
				Vector3 vector = Vector3.zero;
				if (nodeAccess != null)
				{
					vector = nodeAccess.worldAccessPoint;
				}
				string[] array = new string[8];
				array[0] = "CityGen: Entrance being removed from room that is still present in gamelocation: ";
				array[1] = fromNode.room.name;
				array[2] = " & ";
				array[3] = toNode.room.name;
				array[4] = " World access: ";
				int num = 5;
				Vector3 vector2 = vector;
				array[num] = vector2.ToString();
				array[6] = " node: ";
				int num2 = 7;
				vector2 = fromNode.position;
				array[num2] = vector2.ToString();
				Game.Log(string.Concat(array), 2);
			}
			this.entrances.Remove(nodeAccess);
		}
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x00122518 File Offset: 0x00120718
	public void SetVisible(bool val, bool forceUpdate, string newDebug, bool immediateLoad = false)
	{
		if (Game.Instance.devMode && Game.Instance.collectDebugData)
		{
			this.debugCulling = val.ToString() + ": " + newDebug;
		}
		if (base.gameObject == null)
		{
			return;
		}
		if (base.gameObject.activeSelf != this.isVisible)
		{
			forceUpdate = true;
		}
		if (this.isVisible != val || forceUpdate)
		{
			this.isVisible = val;
			base.gameObject.SetActive(this.isVisible);
			foreach (Actor actor in this.currentOccupants)
			{
				actor.SetVisible(this.isVisible, false);
			}
			foreach (BugController bugController in this.spawnedBugs)
			{
				bugController.enabled = this.isVisible;
			}
			if (val)
			{
				if (!this.geometryLoaded)
				{
					GenerationController.Instance.LoadGeometryRoom(this);
				}
				CityData.Instance.visibleRooms.Add(this);
				if (!Player.Instance.inAirVent && this.explorationLevel < 1)
				{
					this.SetExplorationLevel(1);
				}
				foreach (FurnitureClusterLocation furnitureClusterLocation in this.furniture)
				{
					if (!furnitureClusterLocation.loadedGeometry)
					{
						furnitureClusterLocation.LoadFurnitureToWorld(immediateLoad);
					}
				}
				this.ExecuteStaticBatching();
				foreach (NewNode newNode in this.nodes)
				{
					for (int i = 0; i < newNode.interactables.Count; i++)
					{
						newNode.interactables[i].LoadInteractableToWorld(false, immediateLoad);
					}
				}
				foreach (Interactable interactable in this.lightswitchInteractables)
				{
					interactable.LoadInteractableToWorld(false, immediateLoad);
				}
				foreach (SpatterSimulation spatterSimulation in this.spatter)
				{
					if (!spatterSimulation.isExecuted)
					{
						spatterSimulation.Execute();
					}
					else
					{
						spatterSimulation.UpdateSpawning();
					}
				}
				foreach (PipeConstructor.PipeGroup pipeGroup in this.pipes)
				{
					pipeGroup.SetVisible(true);
				}
				using (List<Interactable>.Enumerator enumerator5 = this.mainLights.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						Interactable interactable2 = enumerator5.Current;
						if (interactable2 != null && interactable2.lightController != null)
						{
							interactable2.lightController.SetCulled(false, false);
						}
					}
					goto IL_417;
				}
			}
			CityData.Instance.visibleRooms.Remove(this);
			foreach (NewNode newNode2 in this.nodes)
			{
				for (int j = 0; j < newNode2.interactables.Count; j++)
				{
					Interactable interactable3 = newNode2.interactables[j];
					if (interactable3.loadedGeometry)
					{
						interactable3.UnloadInteractable();
					}
				}
			}
			foreach (Interactable interactable4 in this.lightswitchInteractables)
			{
				if (interactable4.loadedGeometry)
				{
					interactable4.UnloadInteractable();
				}
			}
			foreach (PipeConstructor.PipeGroup pipeGroup2 in this.pipes)
			{
				if (!SessionData.Instance.pipesToUnload.Contains(pipeGroup2))
				{
					SessionData.Instance.pipesToUnload.Add(pipeGroup2);
					Toolbox.Instance.InvokeEndOfFrame(SessionData.Instance.UnloadPipes, "Unload pipes on room deactivate");
				}
			}
			IL_417:
			if (SessionData.Instance.startedGame)
			{
				this.UpdateFootprints(!val);
			}
			if (!SessionData.Instance.isFloorEdit && GenerationController.Instance.spawnedRooms.Count > ObjectPoolingController.Instance.maxRoomCache && Game.Instance.roomCacheLimit)
			{
				GenerationController.Instance.UnloadOldestRooms();
			}
		}
		if (val)
		{
			using (List<NewNode.NodeAccess>.Enumerator enumerator8 = this.entrances.GetEnumerator())
			{
				while (enumerator8.MoveNext())
				{
					NewNode.NodeAccess nodeAccess = enumerator8.Current;
					if (nodeAccess.door != null)
					{
						if (!nodeAccess.door.parentedWall.node.room.geometryLoaded)
						{
							GenerationController.Instance.LoadGeometryRoom(nodeAccess.door.parentedWall.node.room);
							nodeAccess.door.parentedWall.node.room.SetVisible(false, false, "Parented visibility", immediateLoad);
						}
						if (!nodeAccess.door.transform.parent.gameObject.activeInHierarchy)
						{
							if (nodeAccess.door.wall.childWall.node.room == this)
							{
								nodeAccess.door.ParentToRoom(nodeAccess.door.wall.childWall.node.room);
							}
							else if (nodeAccess.door.wall.parentWall.node.room == this)
							{
								nodeAccess.door.ParentToRoom(nodeAccess.door.wall.parentWall.node.room);
							}
						}
						if (nodeAccess.door.preset.closeBehaviour == DoorPreset.ClosingBehaviour.closeOnCull && !nodeAccess.door.isClosed)
						{
							nodeAccess.door.SetOpen(0f, null, true, 1f);
						}
					}
				}
				return;
			}
		}
		foreach (NewNode.NodeAccess nodeAccess2 in this.entrances)
		{
			if (nodeAccess2.door != null && nodeAccess2.door.playerRoom == this)
			{
				if (nodeAccess2.door.wall.childWall.node.room.isVisible)
				{
					nodeAccess2.door.ParentToRoom(nodeAccess2.door.wall.childWall.node.room);
				}
				else if (nodeAccess2.door.wall.parentWall.node.room.isVisible)
				{
					nodeAccess2.door.ParentToRoom(nodeAccess2.door.wall.parentWall.node.room);
				}
			}
		}
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x00122D64 File Offset: 0x00120F64
	public void AddForStaticBatching(FurnitureLocation loc)
	{
		if (Game.Instance.enableRuntimeStaticBatching && loc.furniture.allowStaticBatching && loc.spawnedObject != null)
		{
			foreach (MeshFilter meshFilter in loc.spawnedObject.GetComponentsInChildren<MeshFilter>(true))
			{
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					MeshRenderer component = meshFilter.gameObject.GetComponent<MeshRenderer>();
					if (component != null && component.sharedMaterial != null && !meshFilter.gameObject.CompareTag("IgnoreStaticBatch") && !meshFilter.gameObject.CompareTag("RainWindowGlass"))
					{
						this.AddForStaticBatching(meshFilter.gameObject, meshFilter.sharedMesh, component.sharedMaterial);
					}
				}
			}
		}
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x00122E3C File Offset: 0x0012103C
	public void AddForStaticBatching(GameObject obj, Mesh objectMesh, Material objectMat)
	{
		if (Game.Instance.enableRuntimeStaticBatching)
		{
			NewRoom.StaticBatchKey staticBatchKey = new NewRoom.StaticBatchKey
			{
				mesh = objectMesh,
				mat = objectMat
			};
			if (!this.staticBatchDictionary.ContainsKey(staticBatchKey))
			{
				this.staticBatchDictionary.Add(staticBatchKey, new List<GameObject>());
				this.staticBatchDictionary[staticBatchKey].Add(obj);
				return;
			}
			if (!this.staticBatchDictionary[staticBatchKey].Contains(obj))
			{
				this.staticBatchDictionary[staticBatchKey].Add(obj);
			}
		}
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x00122EC8 File Offset: 0x001210C8
	public void ExecuteStaticBatching()
	{
		if (Game.Instance.enableRuntimeStaticBatching)
		{
			foreach (KeyValuePair<NewRoom.StaticBatchKey, List<GameObject>> keyValuePair in this.staticBatchDictionary)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					if (keyValuePair.Value[i] == null)
					{
						keyValuePair.Value.RemoveAt(i);
						i--;
					}
				}
				if (keyValuePair.Value.Count > 1)
				{
					try
					{
						StaticBatchingUtility.Combine(keyValuePair.Value.ToArray(), keyValuePair.Value[0]);
					}
					catch
					{
						Game.LogError("Error trying to execute static batching...", 2);
					}
				}
			}
			this.staticBatchDictionary.Clear();
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00122FBC File Offset: 0x001211BC
	public void QueueFootprintUpdate()
	{
		if (!base.gameObject.activeSelf)
		{
			this.footprintUpdateQueued = false;
			return;
		}
		this.footprintUpdateQueued = true;
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00122FDC File Offset: 0x001211DC
	public void UpdateFootprints(bool forceRemoveAll = false)
	{
		if (!this.isVisible)
		{
			return;
		}
		HashSet<GameplayController.Footprint> hashSet;
		if (GameplayController.Instance.activeFootprints.ContainsKey(this) && !forceRemoveAll)
		{
			hashSet = new HashSet<GameplayController.Footprint>(GameplayController.Instance.activeFootprints[this]);
		}
		else
		{
			hashSet = new HashSet<GameplayController.Footprint>();
		}
		for (int i = 0; i < this.spawnedFootprints.Count; i++)
		{
			FootprintController footprintController = this.spawnedFootprints[i];
			if (hashSet.Contains(footprintController.footprint))
			{
				hashSet.Remove(footprintController.footprint);
			}
			else
			{
				FootprintController.RecycleFootprint(footprintController);
				this.spawnedFootprints.RemoveAt(i);
				i--;
			}
		}
		foreach (GameplayController.Footprint newFootprint in hashSet)
		{
			FootprintController newFootprint2 = FootprintController.GetNewFootprint();
			newFootprint2.transform.SetParent(base.transform);
			newFootprint2.Setup(newFootprint);
			this.spawnedFootprints.Add(newFootprint2);
		}
		this.footprintUpdateQueued = false;
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x001230F0 File Offset: 0x001212F0
	public void EnableLight(bool val)
	{
		this.enabledLights = val;
		foreach (Interactable interactable in this.mainLights)
		{
			if (interactable.lightController != null)
			{
				interactable.lightController.gameObject.SetActive(this.enabledLights);
			}
		}
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x00123168 File Offset: 0x00121368
	public void ConnectNodes()
	{
		if (SessionData.Instance.isFloorEdit)
		{
			foreach (NewNode newNode in this.nodes)
			{
				newNode.accessToOtherNodes.Clear();
			}
		}
		this.UpdateWorldPositionAndBoundsSize();
		this.entrances.Clear();
		if (!Game.Instance.enableNewRealtimeTimeCullingSystem && this.gameLocation as NewAddress != null && (this.gameLocation as NewAddress).preset == CityControls.Instance.outsideLayoutConfig)
		{
			return;
		}
		foreach (NewNode newNode2 in this.nodes)
		{
			if (newNode2.tile.isStairwell && !newNode2.isConnected)
			{
				newNode2.tile.ConnectStairwell();
			}
			newNode2.UpdateWalkableSublocations();
			if (!newNode2.isConnected)
			{
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX8;
				for (int i = 0; i < offsetArrayX.Length; i++)
				{
					Vector2Int v2 = offsetArrayX[i];
					Vector3Int vector3Int;
					vector3Int..ctor(newNode2.nodeCoord.x + v2.x, newNode2.nodeCoord.y + v2.y, newNode2.nodeCoord.z);
					NewNode newNode3 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode3) && (!newNode2.room.blockedAccess.ContainsKey(newNode2) || !newNode2.room.blockedAccess[newNode2].Contains(newNode3)))
					{
						bool flag = false;
						if (Mathf.Abs(v2.x) + Mathf.Abs(v2.y) == 2)
						{
							Vector2 vector;
							vector..ctor((float)v2.x * 0.5f, (float)v2.y * 0.5f);
							flag = true;
							Vector2 offset1 = new Vector2(0f, vector.y);
							Vector2 offset2 = new Vector2(vector.x, 0f);
							if (newNode2.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
							{
								goto IL_3A6;
							}
							offset1 = new Vector2(0f, -vector.y);
							offset2 = new Vector2(-vector.x, 0f);
							if (newNode3.walls.Exists((NewWall item) => item.wallOffset == offset1 || item.wallOffset == offset2))
							{
								goto IL_3A6;
							}
						}
						NewWall newWall = newNode2.walls.Find((NewWall item) => item.wallOffset * 2f == v2);
						if (newWall == null || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.window || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge || newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
						{
							if (newNode3.tile.isStairwell)
							{
								if (flag)
								{
									goto IL_3A6;
								}
								Vector3Int vector3Int2;
								vector3Int2..ctor(newNode3.nodeCoord.x, newNode3.nodeCoord.y, newNode3.nodeCoord.z - 1);
								NewNode newNode4 = null;
								if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode4))
								{
									if (newNode4.stairwellUpperLink)
									{
										newNode2.AddAccessToOtherNode(newNode4, true, true, NewNode.NodeAccess.AccessType.adjacent, false);
										goto IL_3A6;
									}
									if (newNode4.tile.isStairwell)
									{
										newNode4.tile.ConnectStairwell();
										if (newNode4.stairwellUpperLink)
										{
											newNode2.AddAccessToOtherNode(newNode4, true, true, NewNode.NodeAccess.AccessType.adjacent, false);
											goto IL_3A6;
										}
									}
								}
								if (!newNode3.stairwellLowerLink)
								{
									goto IL_3A6;
								}
							}
							newNode2.AddAccessToOtherNode(newNode3, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
						}
					}
					IL_3A6:;
				}
			}
		}
		foreach (NewNode newNode5 in this.nodes)
		{
			if (Game.Instance.enableNewRealtimeTimeCullingSystem || this.gameLocation.thisAsAddress != null)
			{
				if (newNode5.floorType == NewNode.FloorTileType.floorOnly || newNode5.floorType == NewNode.FloorTileType.noneButIndoors || (Game.Instance.enableNewRealtimeTimeCullingSystem && newNode5.floorType == NewNode.FloorTileType.none))
				{
					Vector3Int vector3Int3;
					vector3Int3..ctor(newNode5.nodeCoord.x, newNode5.nodeCoord.y, newNode5.nodeCoord.z + 1);
					NewNode newNode6 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int3, ref newNode6))
					{
						if ((Game.Instance.enableNewRealtimeTimeCullingSystem || newNode6.gameLocation.thisAsAddress != null) && (newNode6.floorType == NewNode.FloorTileType.noneButIndoors || newNode6.floorType == NewNode.FloorTileType.CeilingOnly || (Game.Instance.enableNewRealtimeTimeCullingSystem && newNode5.floorType == NewNode.FloorTileType.none)))
						{
							if (!newNode5.tile.isStairwell && !newNode6.tile.isStairwell)
							{
								newNode5.AddAccessToOtherNode(newNode6, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
								newNode6.AddAccessToOtherNode(newNode5, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
							}
							if (!newNode5.room.atriumRooms.Contains(newNode6.room))
							{
								newNode5.room.atriumRooms.Add(newNode6.room);
							}
						}
						if (this.atriumTop == null)
						{
							this.atriumTop = newNode6.room;
						}
						int j = 1;
						while (j < 20)
						{
							Vector3Int vector3Int4;
							vector3Int4..ctor(newNode5.nodeCoord.x, newNode5.nodeCoord.y, newNode5.nodeCoord.z + j);
							if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode6))
							{
								break;
							}
							if (newNode6.floorType == NewNode.FloorTileType.CeilingOnly)
							{
								this.atriumTop = newNode6.room;
								if (!newNode5.room.atriumRooms.Contains(newNode6.room))
								{
									newNode5.room.atriumRooms.Add(newNode6.room);
									break;
								}
								break;
							}
							else
							{
								if (newNode6.floorType != NewNode.FloorTileType.floorOnly && newNode6.floorType != NewNode.FloorTileType.noneButIndoors && (!Game.Instance.enableNewRealtimeTimeCullingSystem || newNode5.floorType != NewNode.FloorTileType.none))
								{
									break;
								}
								if (newNode6.room.mainLightPreset != null && newNode6.room.mainLightPreset.lightingPreset != null && newNode6.room.mainLightPreset.lightingPreset.isAtriumLight && !newNode5.room.atriumRooms.Contains(newNode6.room))
								{
									newNode5.room.atriumRooms.Add(newNode6.room);
								}
								if ((float)this.nodes.Count / 38f >= (float)j && !newNode5.room.atriumRooms.Contains(newNode6.room))
								{
									newNode5.room.atriumRooms.Add(newNode6.room);
								}
								j++;
							}
						}
					}
				}
				if (newNode5.floorType == NewNode.FloorTileType.CeilingOnly || newNode5.floorType == NewNode.FloorTileType.noneButIndoors || (Game.Instance.enableNewRealtimeTimeCullingSystem && newNode5.floorType == NewNode.FloorTileType.none))
				{
					Vector3Int vector3Int5;
					vector3Int5..ctor(newNode5.nodeCoord.x, newNode5.nodeCoord.y, newNode5.nodeCoord.z - 1);
					NewNode newNode7 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int5, ref newNode7) && (Game.Instance.enableNewRealtimeTimeCullingSystem || newNode7.gameLocation.thisAsAddress != null) && (newNode7.floorType == NewNode.FloorTileType.noneButIndoors || newNode7.floorType == NewNode.FloorTileType.floorOnly))
					{
						if (!newNode5.tile.isStairwell && !newNode7.tile.isStairwell)
						{
							newNode5.AddAccessToOtherNode(newNode7, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
							newNode7.AddAccessToOtherNode(newNode5, true, false, NewNode.NodeAccess.AccessType.adjacent, false);
						}
						if (!newNode5.room.atriumRooms.Contains(newNode7.room))
						{
							newNode5.room.atriumRooms.Add(newNode7.room);
						}
					}
				}
			}
			bool flag2 = true;
			foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode5.accessToOtherNodes)
			{
				if (keyValuePair.Value.walkingAccess)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				newNode5.isInaccessable = true;
				newNode5.noAccess = true;
			}
		}
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x00123A64 File Offset: 0x00121C64
	public void ApplyBlockedAccess()
	{
		foreach (NewNode newNode in this.nodes)
		{
			newNode.UpdateWalkableSublocations();
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
			{
				Vector3Int vector3Int;
				vector3Int..ctor(newNode.nodeCoord.x + vector2Int.x, newNode.nodeCoord.y + vector2Int.y, newNode.nodeCoord.z);
				NewNode newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && newNode.room != null && newNode.room.blockedAccess != null && newNode.room.blockedAccess.ContainsKey(newNode))
				{
					try
					{
						if (newNode.room.blockedAccess[newNode].Contains(newNode2))
						{
							newNode.RemoveAccessToOtherNode(newNode2, true);
						}
					}
					catch
					{
					}
				}
			}
		}
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x00123B9C File Offset: 0x00121D9C
	public void GenerateCullingTree(bool debugMode = false)
	{
		if (this.completedTreeCull && !debugMode)
		{
			return;
		}
		if (debugMode && this.specificRoomCullingDebug != null)
		{
			Game.Log("SPECIFIC: show debug display for culling of room: " + this.specificRoomCullingDebug.GetName() + " from " + this.GetName(), 2);
		}
		if (debugMode)
		{
			this.cullingTree.Clear();
			this.doorCheckSet.Clear();
		}
		if (!this.cullingTree.ContainsKey(this))
		{
			this.cullingTree.Add(this, new List<NewRoom.CullTreeEntry>());
			this.cullingTree[this].Add(new NewRoom.CullTreeEntry(null));
		}
		if ((this.preset.roomType != CityControls.Instance.nullDefaultRoom || this.gameLocation.thisAsStreet != null) && this.nodes.Count > 0)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = false;
			if (this.gameLocation.thisAsStreet != null)
			{
				flag = true;
			}
			bool flag2 = this.IsOutside();
			foreach (NewNode.NodeAccess nodeAccess in this.entrances)
			{
				NewRoom room = nodeAccess.toNode.room;
				if (!this.cullingTree.ContainsKey(room) || ((this.cullingTree[room] == null || this.cullingTree[room][0].requiredOpenDoors != null) && this.cullingTree[room][0].requiredOpenDoors.Count > 0))
				{
					List<int> list = new List<int>();
					if (nodeAccess.door != null && !nodeAccess.door.preset.isTransparent)
					{
						list.Add(nodeAccess.door.wall.id);
					}
					bool flag3;
					if (!this.cullingTree.ContainsKey(room))
					{
						this.cullingTree.Add(room, new List<NewRoom.CullTreeEntry>());
						flag3 = true;
					}
					else if (list.Count <= 0)
					{
						if (this.cullingTree[room].Count > 0)
						{
							this.cullingTree[room].Clear();
						}
						flag3 = true;
					}
					else
					{
						flag3 = true;
						for (int i = 0; i < this.cullingTree[room].Count; i++)
						{
							NewRoom.CullTreeEntry cullTreeEntry = this.cullingTree[room][i];
							if (cullTreeEntry.requiredOpenDoors == null || cullTreeEntry.requiredOpenDoors.Count <= 0)
							{
								flag3 = false;
								break;
							}
							if (Enumerable.SequenceEqual<int>(cullTreeEntry.requiredOpenDoors, list))
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						if (debugMode)
						{
							this.SpawnDebugCullingObject(nodeAccess.worldAccessPoint, room, nodeAccess, list, CullingDebugController.CullDebugType.adjacent, null, null);
						}
						this.cullingTree[room].Add(new NewRoom.CullTreeEntry(list));
						if (room.atriumTop != null)
						{
							if (!this.cullingTree.ContainsKey(room.atriumTop))
							{
								this.cullingTree.Add(room.atriumTop, new List<NewRoom.CullTreeEntry>());
							}
							else
							{
								this.cullingTree[room.atriumTop].Clear();
							}
							this.cullingTree[room.atriumTop].Add(new NewRoom.CullTreeEntry(list));
							if (debugMode)
							{
								this.SpawnDebugCullingObject(Enumerable.First<NewNode>(room.atriumTop.nodes).position, room.atriumTop, nodeAccess, list, CullingDebugController.CullDebugType.atriumTop, room, null);
							}
						}
						if (list != null)
						{
							foreach (int num4 in list)
							{
								if (!this.doorCheckSet.Contains(num4))
								{
									this.doorCheckSet.Add(num4);
								}
							}
						}
					}
				}
			}
			if (!flag && this.building != null && this.floor != null && this.floor.floor > 0)
			{
				foreach (NewNode.NodeAccess nodeAccess2 in this.entrances)
				{
					if (nodeAccess2.toNode.room.IsOutside())
					{
						Vector2 vector = (nodeAccess2.toNode.nodeCoord - nodeAccess2.fromNode.nodeCoord).normalized;
						if (this.building != null && this.building.directionalCullingTrees != null && this.building.directionalCullingTrees.ContainsKey(vector))
						{
							foreach (KeyValuePair<NewRoom, List<NewRoom.CullTreeEntry>> keyValuePair in this.building.directionalCullingTrees[vector])
							{
								if (!this.cullingTree.ContainsKey(keyValuePair.Key))
								{
									this.cullingTree.Add(keyValuePair.Key, keyValuePair.Value);
									int j = 0;
									while (j < keyValuePair.Value.Count)
									{
										NewRoom.CullTreeEntry cullTreeEntry2 = null;
										try
										{
											cullTreeEntry2 = keyValuePair.Value[j];
										}
										catch
										{
											goto IL_579;
										}
										goto IL_51D;
										IL_579:
										j++;
										continue;
										IL_51D:
										if (cullTreeEntry2 != null && cullTreeEntry2.requiredOpenDoors != null)
										{
											foreach (int num5 in cullTreeEntry2.requiredOpenDoors)
											{
												if (!this.doorCheckSet.Contains(num5))
												{
													this.doorCheckSet.Add(num5);
												}
											}
											goto IL_579;
										}
										goto IL_579;
									}
								}
							}
							string[] array = new string[6];
							array[0] = "CityGen: Added precalculated entries for building ";
							array[1] = this.building.buildingID.ToString();
							array[2] = " facing ";
							int num6 = 3;
							Vector2 vector2 = vector;
							array[num6] = vector2.ToString();
							array[4] = " entries: ";
							array[5] = this.building.directionalCullingTrees[vector].Count.ToString();
							Game.Log(string.Concat(array), 2);
						}
					}
				}
			}
			if (debugMode && this.specificRoomCullingDebug != null)
			{
				Game.Log("SPECIFIC: Checking against " + CityData.Instance.roomDirectory.Count.ToString() + " other rooms...", 2);
			}
			foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
			{
				if (!(newRoom == this))
				{
					if (newRoom.nodes.Count <= 0)
					{
						if (debugMode && this.specificRoomCullingDebug == newRoom)
						{
							Game.Log("SPECIFIC: This room doesn't feature any nodes, so skipping culling tree generation", 2);
						}
					}
					else
					{
						if (debugMode && this.specificRoomCullingDebug == newRoom)
						{
							Game.Log("SPECIFIC: Checking against " + newRoom.GetName(), 2);
						}
						bool flag4 = false;
						if (newRoom.gameLocation.thisAsStreet != null)
						{
							flag4 = true;
						}
						bool flag5 = newRoom.IsOutside();
						if (newRoom.preset.roomType == CityControls.Instance.nullDefaultRoom && !flag4)
						{
							if (debugMode && this.specificRoomCullingDebug == newRoom)
							{
								Game.Log("SPECIFIC: This room is a null default room", 2);
							}
						}
						else if (newRoom.isOutsideWindow)
						{
							if (debugMode && this.specificRoomCullingDebug == newRoom)
							{
								Game.Log("SPECIFIC: This room is marked as 'isOutsideWindow", 2);
							}
						}
						else
						{
							float num7 = Vector3.Distance(this.worldPos, newRoom.worldPos);
							float num8 = Mathf.Lerp(CullingControls.Instance.outsideHeightDistanceBoost.x, CullingControls.Instance.outsideHeightDistanceBoost.y, this.worldPos.y / 86f);
							if (num7 > CullingControls.Instance.outsideDistanceMax + num8)
							{
								if (debugMode && this.specificRoomCullingDebug == newRoom)
								{
									Game.Log("SPECIFIC: This room is outside of the maximum possible range", 2);
								}
							}
							else if (flag5 && (flag2 || this.entrances.Exists((NewNode.NodeAccess item) => item.GetOtherRoom(this).IsOutside())) && num7 <= CullingControls.Instance.outsideDistanceMax + num8)
							{
								if (!this.cullingTree.ContainsKey(newRoom))
								{
									this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
								}
								else
								{
									this.cullingTree[newRoom].Clear();
								}
								this.cullingTree[newRoom].Add(new NewRoom.CullTreeEntry(null));
								if (debugMode && this.specificRoomCullingDebug == newRoom)
								{
									Game.Log("SPECIFIC: This room is outside and being auto added as it's above floor 1", 2);
								}
							}
							else
							{
								if (flag)
								{
									if (!flag4)
									{
										if (!flag5)
										{
											if (Mathf.Abs(newRoom.floor.floor) == 1)
											{
												bool flag6 = true;
												foreach (NewNode.NodeAccess nodeAccess3 in newRoom.entrances)
												{
													if (nodeAccess3.accessType == NewNode.NodeAccess.AccessType.bannister && nodeAccess3.toNode.room.atriumRooms.Count > 0)
													{
														flag6 = false;
														break;
													}
												}
												if (flag6)
												{
													if (debugMode && this.specificRoomCullingDebug == newRoom)
													{
														Game.Log("SPECIFIC: This room is being skipped due to floor", 2);
														continue;
													}
													continue;
												}
											}
											else if (newRoom.floor.floor != 0)
											{
												if (debugMode && this.specificRoomCullingDebug == newRoom)
												{
													Game.Log("SPECIFIC: This room is != 0", 2);
													continue;
												}
												continue;
											}
										}
										else if (newRoom.building != null)
										{
											if (num7 <= CullingControls.Instance.outsideDistanceMax + num8)
											{
												if (!this.cullingTree.ContainsKey(newRoom))
												{
													this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
												}
												else
												{
													this.cullingTree[newRoom].Clear();
												}
												this.cullingTree[newRoom].Add(new NewRoom.CullTreeEntry(null));
												if (debugMode && this.specificRoomCullingDebug == newRoom)
												{
													Game.Log("SPECIFIC: This room is being auto added as it's outside and the building is visible", 2);
													continue;
												}
												continue;
											}
											else if (debugMode && this.specificRoomCullingDebug == newRoom)
											{
												Game.Log("SPECIFIC: This room would be auto added, but it is outside of the max distance", 2);
											}
										}
									}
								}
								else if (!flag && !flag4)
								{
									if (flag5)
									{
										if (num7 <= CullingControls.Instance.outsideDistanceMax + num8)
										{
											if (!this.cullingTree.ContainsKey(newRoom))
											{
												this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
											}
											else
											{
												this.cullingTree[newRoom].Clear();
											}
											this.cullingTree[newRoom].Add(new NewRoom.CullTreeEntry(null));
											if (debugMode && this.specificRoomCullingDebug == newRoom)
											{
												Game.Log("SPECIFIC: This room is being auto added as it's outside and the building is visible", 2);
												continue;
											}
											continue;
										}
										else if (debugMode && this.specificRoomCullingDebug == newRoom)
										{
											Game.Log("SPECIFIC: This room would be auto added, but it is outside of the max distance", 2);
										}
									}
									if (this.building != newRoom.building && !flag5)
									{
										if (Mathf.Abs(this.floor.floor - newRoom.floor.floor) > 1 || this.floor.floor < 0 || newRoom.floor.floor < 0)
										{
											if (debugMode && this.specificRoomCullingDebug == newRoom)
											{
												Game.Log("SPECIFIC: This room is being skipped due to it not being too high/low", 2);
												continue;
											}
											continue;
										}
									}
									else if (this.building == newRoom.building && (flag5 && flag2) && !this.cullingTree.ContainsKey(newRoom))
									{
										this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
										List<int> newRequiredDoors = new List<int>();
										this.cullingTree[newRoom].Add(new NewRoom.CullTreeEntry(newRequiredDoors));
										if (debugMode && this.specificRoomCullingDebug == newRoom)
										{
											Game.Log("SPECIFIC: This room is being auto added as it's outside and in the same building", 2);
											continue;
										}
										continue;
									}
								}
								bool flag7 = false;
								if (this.floor != null && newRoom.floor != null && this.floor.floor == newRoom.floor.floor)
								{
									flag7 = true;
								}
								if (this.cullingTree.ContainsKey(newRoom) && this.cullingTree[newRoom] != null && this.cullingTree[newRoom].Count > 0)
								{
									try
									{
										if (this.cullingTree[newRoom][0].requiredOpenDoors == null || this.cullingTree[newRoom][0].requiredOpenDoors.Count <= 0)
										{
											if (debugMode && this.specificRoomCullingDebug == newRoom)
											{
												Game.Log("SPECIFIC: This room is already contained within the culling dictionary", 2);
											}
											num3++;
											continue;
										}
									}
									catch
									{
									}
								}
								num++;
								bool flag8 = false;
								foreach (NewNode.NodeAccess nodeAccess4 in this.entrances)
								{
									if (nodeAccess4 != null && (!flag || !flag4 || nodeAccess4.accessType == NewNode.NodeAccess.AccessType.streetToStreet))
									{
										float num9 = Vector3.Distance(nodeAccess4.worldAccessPoint, newRoom.worldPos);
										if (!flag4 && flag5)
										{
											if (flag || flag2)
											{
												if (num9 > CullingControls.Instance.fromOutsideToInsideDistanceMax)
												{
													continue;
												}
											}
											else
											{
												num9 = Vector3.Distance(new Vector3(nodeAccess4.worldAccessPoint.x, 0f, nodeAccess4.worldAccessPoint.z), new Vector3(newRoom.worldPos.x, 0f, newRoom.worldPos.z));
												if (num9 > CullingControls.Instance.fromInsideToInsideDistanceMax)
												{
													continue;
												}
											}
										}
										else if (num9 > CullingControls.Instance.outsideDistanceMax + num8)
										{
											continue;
										}
										if (!(nodeAccess4.fromNode.room == nodeAccess4.toNode.room) && (flag || flag4 || flag5 || nodeAccess4.accessType != NewNode.NodeAccess.AccessType.window || newRoom.floor.floor == 0))
										{
											Vector3 vector3 = newRoom.worldPos - nodeAccess4.worldAccessPoint;
											NewNode newNode = nodeAccess4.toNode;
											NewNode newNode2 = nodeAccess4.fromNode;
											if (nodeAccess4.toNode.room == this)
											{
												newNode = nodeAccess4.fromNode;
												newNode2 = nodeAccess4.toNode;
											}
											float num10 = Vector3.Angle(vector3, (newNode.position - newNode2.position).normalized);
											if (num10 >= -CullingControls.Instance.visibleRoomFoV && num10 <= CullingControls.Instance.visibleRoomFoV)
											{
												foreach (NewNode.NodeAccess nodeAccess5 in newRoom.entrances)
												{
													if (nodeAccess5 != null && !(nodeAccess5.fromNode.room == nodeAccess5.toNode.room) && (!flag || !flag4 || nodeAccess5.accessType == NewNode.NodeAccess.AccessType.streetToStreet))
													{
														num2++;
														NewNode newNode3 = nodeAccess5.toNode;
														NewNode newNode4 = nodeAccess5.fromNode;
														if (nodeAccess5.toNode.room == newRoom)
														{
															newNode3 = nodeAccess5.fromNode;
															newNode4 = nodeAccess5.toNode;
														}
														float num11 = Vector3.Angle(this.worldPos - nodeAccess5.worldAccessPoint, (newNode3.nodeCoord - newNode4.nodeCoord).normalized);
														if (num11 >= -CullingControls.Instance.visibleRoomFoV && num11 <= CullingControls.Instance.visibleRoomFoV)
														{
															if (nodeAccess4.toNode == null || nodeAccess5.fromNode == null)
															{
																continue;
															}
															if (((flag7 || (flag2 && flag5)) && flag == flag4 && Vector3.Distance(nodeAccess4.worldAccessPoint, nodeAccess5.worldAccessPoint) <= 5f) | (!flag7 && Vector3.Distance(nodeAccess4.worldAccessPoint, nodeAccess5.worldAccessPoint) <= 2f))
															{
																if (!this.cullingTree.ContainsKey(newRoom))
																{
																	this.cullingTree.Add(newRoom, new List<NewRoom.CullTreeEntry>());
																	List<int> list2 = new List<int>();
																	if (nodeAccess4.door != null && nodeAccess4.door.wall != null)
																	{
																		try
																		{
																			list2.Add(nodeAccess4.door.wall.id);
																		}
																		catch
																		{
																			Game.Log("Failed to add entrance to DList", 2);
																		}
																	}
																	if (nodeAccess5.door != null && nodeAccess5.door.wall != null)
																	{
																		try
																		{
																			list2.Add(nodeAccess5.door.wall.id);
																		}
																		catch
																		{
																			Game.Log("Failed to add entrance to DList", 2);
																		}
																	}
																	this.cullingTree[newRoom].Add(new NewRoom.CullTreeEntry(list2));
																	if (debugMode && this.specificRoomCullingDebug == newRoom)
																	{
																		Game.Log("SPECIFIC: Being added through a valid entrance", 2);
																	}
																}
															}
															else
															{
																List<DataRaycastController.NodeRaycastHit> list3;
																DataRaycastController.Instance.EntranceRaycast(nodeAccess4, nodeAccess5, out list3, false);
																if (debugMode)
																{
																	Game.Log(string.Concat(new string[]
																	{
																		"Checking entrance ",
																		nodeAccess4.name,
																		" and ",
																		nodeAccess5.name,
																		": ",
																		DataRaycastController.Instance.EntranceRaycast(nodeAccess4, nodeAccess5, out list3, false).ToString(),
																		" length: ",
																		list3.Count.ToString()
																	}), 2);
																}
																new List<NewRoom>();
																for (int k = 1; k < list3.Count; k++)
																{
																	DataRaycastController.NodeRaycastHit nodeRaycastHit = list3[k];
																	NewNode newNode5 = null;
																	if (PathFinder.Instance.nodeMap.TryGetValue(nodeRaycastHit.coord, ref newNode5) && newNode5.room != null && !(newNode5.room == this))
																	{
																		bool flag9 = false;
																		if (debugMode)
																		{
																			Game.Log("Add: False", 2);
																		}
																		CullingDebugController.CullDebugType newType = CullingDebugController.CullDebugType.succeededOvr;
																		if (!this.cullingTree.ContainsKey(newNode5.room))
																		{
																			this.cullingTree.Add(newNode5.room, new List<NewRoom.CullTreeEntry>());
																			flag9 = true;
																			newType = CullingDebugController.CullDebugType.succeededNew;
																			if (debugMode)
																			{
																				Game.Log("Add: True: No existing " + newNode5.room.name, 2);
																			}
																			if (debugMode && this.specificRoomCullingDebug == newNode5.room)
																			{
																				Game.Log("SPECIFIC: Being added through valid data raycast", 2);
																			}
																		}
																		else if (nodeRaycastHit.conditionalDoors == null || nodeRaycastHit.conditionalDoors.Count <= 0)
																		{
																			if (this.cullingTree[newNode5.room].Count > 0)
																			{
																				this.cullingTree[newNode5.room].Clear();
																			}
																			flag9 = true;
																			if (debugMode)
																			{
																				Game.Log("Add: True: Requies no open doors", 2);
																			}
																			if (debugMode && this.specificRoomCullingDebug == newNode5.room)
																			{
																				Game.Log("SPECIFIC: Being added through valid data raycast", 2);
																			}
																		}
																		else
																		{
																			flag9 = true;
																			if (debugMode)
																			{
																				Game.Log("Add: True: Existing is found: " + newNode5.room.name, 2);
																			}
																			if (debugMode && this.specificRoomCullingDebug == newNode5.room)
																			{
																				Game.Log("SPECIFIC: Being added through valid data raycast", 2);
																			}
																			int l = 0;
																			while (l < this.cullingTree[newNode5.room].Count)
																			{
																				NewRoom.CullTreeEntry cullTreeEntry3 = null;
																				try
																				{
																					cullTreeEntry3 = this.cullingTree[newNode5.room][l];
																				}
																				catch
																				{
																					goto IL_143E;
																				}
																				goto IL_13A3;
																				IL_143E:
																				l++;
																				continue;
																				IL_13A3:
																				if (cullTreeEntry3 == null)
																				{
																					goto IL_143E;
																				}
																				if (cullTreeEntry3.requiredOpenDoors == null || cullTreeEntry3.requiredOpenDoors.Count <= 0)
																				{
																					flag9 = false;
																					if (debugMode)
																					{
																						Game.Log("Add: False: Existing has no door requirements", 2);
																						break;
																					}
																					break;
																				}
																				else
																				{
																					if (Enumerable.SequenceEqual<int>(cullTreeEntry3.requiredOpenDoors, nodeRaycastHit.conditionalDoors))
																					{
																						if (debugMode)
																						{
																							Game.Log("Add: False: Duplicate door requirements found: ", 2);
																							foreach (int num12 in nodeRaycastHit.conditionalDoors)
																							{
																								Game.Log(num12, 2);
																							}
																						}
																						flag9 = false;
																						break;
																					}
																					goto IL_143E;
																				}
																			}
																		}
																		if (flag9)
																		{
																			if (debugMode)
																			{
																				this.SpawnDebugCullingObject(newNode5.position, newNode5.room, nodeAccess4, nodeRaycastHit.conditionalDoors, newType, null, nodeAccess5);
																			}
																			this.cullingTree[newNode5.room].Add(new NewRoom.CullTreeEntry(nodeRaycastHit.conditionalDoors));
																			if (newNode5.room.atriumTop != null)
																			{
																				if (!this.cullingTree.ContainsKey(newNode5.room.atriumTop))
																				{
																					this.cullingTree.Add(newNode5.room.atriumTop, new List<NewRoom.CullTreeEntry>());
																				}
																				this.cullingTree[newNode5.room.atriumTop].Clear();
																				this.cullingTree[newNode5.room.atriumTop].Add(new NewRoom.CullTreeEntry(nodeRaycastHit.conditionalDoors));
																				if (debugMode)
																				{
																					this.SpawnDebugCullingObject(Enumerable.First<NewNode>(newNode5.room.atriumTop.nodes).position, newNode5.room.atriumTop, nodeAccess4, nodeRaycastHit.conditionalDoors, CullingDebugController.CullDebugType.atriumTop, newNode5.room, null);
																				}
																			}
																			if (nodeRaycastHit.conditionalDoors != null)
																			{
																				foreach (int num13 in nodeRaycastHit.conditionalDoors)
																				{
																					if (!this.doorCheckSet.Contains(num13))
																					{
																						this.doorCheckSet.Add(num13);
																					}
																				}
																			}
																			if (newNode5.room == newRoom && (nodeRaycastHit.conditionalDoors == null || nodeRaycastHit.conditionalDoors.Count <= 0))
																			{
																				flag8 = true;
																			}
																		}
																		else if (debugMode && this.specificRoomCullingDebug == newNode5.room)
																		{
																			Game.Log("SPECIFIC: Was not added because of invalid data raycast", 2);
																		}
																	}
																}
																list3 = null;
															}
														}
														if (flag8)
														{
															break;
														}
													}
												}
												if (flag8)
												{
													break;
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
			List<NewRoom> list4 = new List<NewRoom>();
			List<NewNode.NodeAccess> list5 = new List<NewNode.NodeAccess>();
			List<int> list6 = new List<int>();
			list4.Clear();
			list4.Add(this);
			list6.Clear();
			list6.Add(0);
			list5.Clear();
			list5.Add(null);
			this.nonAudioOccludedRooms.Clear();
			while (list4.Count > 0)
			{
				NewRoom newRoom2 = list4[0];
				int num14 = list6[0];
				NewNode.NodeAccess nodeAccess6 = list5[0];
				if (num14 >= 3)
				{
					break;
				}
				foreach (NewNode.NodeAccess nodeAccess7 in newRoom2.entrances)
				{
					Mathf.Abs(Enumerable.FirstOrDefault<NewNode>(this.nodes).nodeCoord.z - nodeAccess7.toNode.nodeCoord.z);
					if (nodeAccess7.accessType == NewNode.NodeAccess.AccessType.openDoorway || nodeAccess7.accessType == NewNode.NodeAccess.AccessType.bannister)
					{
						if (!list4.Contains(nodeAccess7.toNode.room) && !this.nonAudioOccludedRooms.Contains(nodeAccess7.toNode.room))
						{
							list4.Add(nodeAccess7.toNode.room);
							list6.Add(num14 + 1);
							list5.Add(nodeAccess7);
						}
					}
					else if (nodeAccess7.accessType == NewNode.NodeAccess.AccessType.verticalSpace && !list4.Contains(nodeAccess7.toNode.room) && !this.nonAudioOccludedRooms.Contains(nodeAccess7.toNode.room))
					{
						list4.Add(nodeAccess7.toNode.room);
						list6.Add(num14 + 1);
						list5.Add(nodeAccess7);
					}
				}
				if (newRoom2 != this)
				{
					this.nonAudioOccludedRooms.Add(newRoom2);
				}
				list4.RemoveAt(0);
				list6.RemoveAt(0);
				list5.RemoveAt(0);
			}
			this.aboveRooms.Clear();
			this.belowRooms.Clear();
			this.adjacentRooms.Clear();
			if (this.gameLocation.thisAsAddress != null)
			{
				foreach (NewNode newNode6 in this.nodes)
				{
					Vector3Int vector3Int = newNode6.nodeCoord + new Vector3Int(0, 0, 1);
					Vector3Int vector3Int2 = newNode6.nodeCoord + new Vector3Int(0, 0, -1);
					NewNode newNode7 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode7) && !newNode7.room.isOutsideWindow && newNode7.gameLocation.thisAsAddress != null && !this.aboveRooms.Contains(newNode7.room))
					{
						this.aboveRooms.Add(newNode7.room);
					}
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode7) && !newNode7.room.isOutsideWindow && newNode7.gameLocation.thisAsAddress != null && !this.belowRooms.Contains(newNode7.room))
					{
						this.belowRooms.Add(newNode7.room);
					}
					foreach (NewWall newWall in newNode6.walls)
					{
						if (newWall.node != newNode6)
						{
							if (!newWall.node.room.isOutsideWindow && newWall.node.gameLocation.thisAsAddress != null && !this.adjacentRooms.Contains(newWall.node.room))
							{
								this.adjacentRooms.Add(newWall.node.room);
							}
						}
						else if (!newWall.otherWall.node.room.isOutsideWindow && newWall.otherWall.node.gameLocation.thisAsAddress != null && !this.adjacentRooms.Contains(newWall.otherWall.node.room))
						{
							this.adjacentRooms.Add(newWall.otherWall.node.room);
						}
					}
				}
			}
			float num15 = (float)num / (float)CityData.Instance.roomDirectory.Count;
			float num16 = (float)num3 / (float)CityData.Instance.roomDirectory.Count;
		}
		else if (debugMode && this.specificRoomCullingDebug != null)
		{
			Game.Log("SPECIFIC: The current room is not valid for (most) culling generation rules", 2);
		}
		this.completedTreeCull = true;
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x001258CC File Offset: 0x00123ACC
	private void SpawnDebugCullingObject(Vector3 worldPos, NewRoom room, NewNode.NodeAccess parentEntrance, List<int> depDoors, CullingDebugController.CullDebugType newType, NewRoom atriumTopOf = null, NewNode.NodeAccess otherEntrance = null)
	{
		if (!Game.Instance.devMode)
		{
			return;
		}
		int num = this.spawnPathDebug.FindIndex((CullingDebugController item) => item.transform.position == worldPos);
		if (num > -1)
		{
			Toolbox.Instance.DestroyObject(this.spawnPathDebug[num].gameObject);
			this.spawnPathDebug.RemoveAt(num);
		}
		List<NewDoor> list = new List<NewDoor>();
		if (depDoors != null)
		{
			foreach (int num2 in depDoors)
			{
				list.Add(CityData.Instance.doorDictionary[num2]);
			}
		}
		GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.debugNodeDisplay, base.transform);
		CullingDebugController component = gameObject.GetComponent<CullingDebugController>();
		component.Setup(room, parentEntrance, list, newType, atriumTopOf, otherEntrance);
		this.spawnPathDebug.Add(component);
		gameObject.transform.position = worldPos;
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x001259E0 File Offset: 0x00123BE0
	public void SetLowerRoom(NewRoom newRoom)
	{
		this.lowerRoom = newRoom;
		this.lowerRoom.allowCoving = false;
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x001259F8 File Offset: 0x00123BF8
	public void AddOccupant(Actor newOcc)
	{
		if (!this.currentOccupants.Contains(newOcc))
		{
			this.currentOccupants.Add(newOcc);
			this.actorUpdate = true;
			for (int i = 0; i < this.audibleLoopingSounds.Count; i++)
			{
				this.audibleLoopingSounds[i].UpdateOcclusion(false);
			}
			if (newOcc.isDead)
			{
				this.containsDead = true;
			}
		}
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x00125A60 File Offset: 0x00123C60
	public void RemoveOccupant(Actor remOcc)
	{
		if (this.currentOccupants.Contains(remOcc))
		{
			this.currentOccupants.Remove(remOcc);
			if (this.currentOccupants.Count <= 0)
			{
				this.actorUpdate = false;
				this.containsDead = false;
				return;
			}
			if (this.containsDead)
			{
				bool flag = true;
				using (HashSet<Actor>.Enumerator enumerator = this.currentOccupants.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.isDead)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					this.containsDead = false;
				}
			}
		}
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x00125B04 File Offset: 0x00123D04
	public void AddFurniture(FurnitureClusterLocation newFurn, bool generateNew, bool addPathBlocking = true, bool immediateSpawn = false)
	{
		if (!this.furniture.Contains(newFurn))
		{
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				if (this.clustersPlaced != null && this.clustersPlaced.Length > 0)
				{
					this.clustersPlaced += ", ";
				}
				this.clustersPlaced += newFurn.cluster.presetName;
			}
			FurnitureLocation furnitureLocation = null;
			string text = this.seed;
			foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in newFurn.clusterObjectMap)
			{
				using (List<FurnitureLocation>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						FurnitureLocation obj = enumerator2.Current;
						if (generateNew)
						{
							if (obj.furniture == null)
							{
								foreach (FurnitureClass furnClass in obj.furnitureClasses)
								{
									text = Toolbox.Instance.RandContained(0, 999999, text, out text).ToString();
									obj.furniture = GenerationController.Instance.PickFurniture(furnClass, this, text, false);
									if (obj.furniture != null)
									{
										break;
									}
									Game.Log("CityGen: No furniture found with design style", 2);
								}
								if (obj.furnitureClasses == null || obj.furnitureClasses.Count <= 0)
								{
									Game.Log("CityGen: No furniture classes found for " + newFurn.cluster.name, 2);
								}
								if (obj.furniture == null)
								{
									continue;
								}
							}
							if (obj.furniture.isArt && !obj.pickedArt)
							{
								obj.art = GenerationController.Instance.PickArt(obj.furniture.artOrientation, this);
								this.gameLocation.artPieces.Add(obj.art);
								obj.pickedArt = true;
							}
							if (!obj.pickedMaterials)
							{
								if (obj.furniture.inheritColouringFromDecor && obj.furniture.variations.Count > 0)
								{
									bool flag = false;
									if (obj.furniture.shareColours != FurniturePreset.ShareColours.none)
									{
										FurnitureLocation furnitureLocation2 = this.individualFurniture.Find((FurnitureLocation item) => item.furniture.shareColours == obj.furniture.shareColours && item != obj);
										if (furnitureLocation2 != null)
										{
											obj.matKey = furnitureLocation2.matKey;
											flag = true;
										}
									}
									if (!flag)
									{
										obj.matKey = MaterialsController.Instance.GenerateMaterialKey(obj.furniture.variations[Toolbox.Instance.RandContained(0, obj.furniture.variations.Count, text, out text)], this.colourScheme, this, obj.furniture.inheritGrubFromDecor, null);
									}
								}
								obj.pickedMaterials = true;
							}
							if (!obj.pickedOwners)
							{
								obj.pickedOwners = true;
								if (obj.furnitureClasses[0].copyFromPreviouslyPlacedInCluster && furnitureLocation != null)
								{
									foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair2 in furnitureLocation.ownerMap)
									{
										if (keyValuePair2.Key.human != null)
										{
											obj.AssignOwner(keyValuePair2.Key.human, false);
										}
										else
										{
											obj.AssignOwner(keyValuePair2.Key.address, false);
										}
									}
									obj.UpdateIntegratedObjectOwnership();
								}
								else if (obj.furnitureClasses[0].assignHomelessOwners)
								{
									if (CityData.Instance.homlessAssign.Count > 0)
									{
										int num = Toolbox.Instance.RandContained(0, CityData.Instance.homlessAssign.Count, text, out text);
										obj.AssignOwner(CityData.Instance.homlessAssign[num], true);
									}
								}
								else if (obj.furnitureClasses[0].assignBelongsToOwners > 0)
								{
									if (obj.furnitureClasses[0].assignMailbox && this.gameLocation.building != null)
									{
										List<NewAddress> list = new List<NewAddress>();
										foreach (KeyValuePair<int, NewFloor> keyValuePair3 in this.gameLocation.building.floors)
										{
											foreach (NewAddress newAddress in keyValuePair3.Value.addresses)
											{
												if (newAddress.residence != null && newAddress.residence.mailbox == null)
												{
													list.Add(newAddress);
												}
											}
										}
										list.Sort((NewAddress p1, NewAddress p2) => p1.residence.GetResidenceNumber().CompareTo(p2.residence.GetResidenceNumber()));
										int num2 = 0;
										for (int i = 0; i < obj.furnitureClasses[0].assignBelongsToOwners; i++)
										{
											if (num2 < list.Count)
											{
												NewAddress newAddress2 = list[num2];
												while (newAddress2.residence == null || newAddress2.residence.mailbox != null)
												{
													num2++;
													if (num2 >= list.Count)
													{
														break;
													}
													newAddress2 = list[num2];
												}
												if (num2 >= list.Count)
												{
													break;
												}
												newAddress2.residence.mailbox = obj;
												obj.AssignOwner(newAddress2, true);
												num2++;
											}
										}
									}
									else
									{
										if (!this.gameLocation.furnitureBelongsTo.ContainsKey(obj.furnitureClasses[0].ownershipClass))
										{
											this.gameLocation.furnitureBelongsTo.Add(obj.furnitureClasses[0].ownershipClass, new Dictionary<FurnitureLocation, List<Human>>());
										}
										List<Human> list2 = new List<Human>();
										if (obj.furnitureClasses[0].ownershipSource == FurnitureClass.OwnershipSource.addressInhabitants && this.gameLocation.thisAsAddress != null)
										{
											list2.AddRange(this.gameLocation.thisAsAddress.inhabitants);
										}
										else if (obj.furnitureClasses[0].ownershipSource == FurnitureClass.OwnershipSource.buildingResidences && this.gameLocation.building != null)
										{
											foreach (KeyValuePair<int, NewFloor> keyValuePair4 in this.gameLocation.building.floors)
											{
												foreach (NewAddress newAddress3 in keyValuePair4.Value.addresses)
												{
													if (newAddress3.residence != null && newAddress3.residence.mailbox == null)
													{
														using (List<Human>.Enumerator enumerator7 = newAddress3.owners.GetEnumerator())
														{
															while (enumerator7.MoveNext())
															{
																Human h = enumerator7.Current;
																if (!list2.Exists((Human item) => item.home == h.home))
																{
																	list2.Add(h);
																}
															}
														}
													}
												}
											}
										}
										foreach (KeyValuePair<FurnitureLocation, List<Human>> keyValuePair5 in this.gameLocation.furnitureBelongsTo[obj.furnitureClasses[0].ownershipClass])
										{
											foreach (Human human5 in keyValuePair5.Value)
											{
												list2.Remove(human5);
											}
										}
										if (obj.furnitureClasses[0].onlyPickFromRoomOwners && this.belongsTo.Count > 0)
										{
											for (int j = 0; j < list2.Count; j++)
											{
												if (!this.belongsTo.Contains(list2[j]))
												{
													list2.RemoveAt(j);
													j--;
												}
											}
										}
										if (obj.furniture.isWorkPosition)
										{
											List<Human> list3 = new List<Human>();
											for (int k = 0; k < list2.Count; k++)
											{
												Human human = list2[k];
												if (human.job != null)
												{
													if (human.job.preset.ownsWorkPosition && human.job.preset.preferredRooms.Contains(this.preset) && obj.furniture.integratedInteractables.Exists((FurniturePreset.IntegratedInteractable item) => item.preset.specialCaseFlag == human.job.preset.jobPostion))
													{
														list3.Add(human);
													}
													else if (human.job.preset.preferredRooms.Count > 0)
													{
														list2.RemoveAt(k);
														k--;
													}
												}
											}
											foreach (Human human2 in list3)
											{
												list2.Remove(human2);
												list2.Insert(0, human2);
											}
										}
										list2.Sort();
										list2.Reverse();
										Human human3 = null;
										int num3 = 0;
										for (int l = 0; l < obj.furnitureClasses[0].assignBelongsToOwners; l++)
										{
											if (num3 < list2.Count)
											{
												Human human4 = list2[num3];
												if (human3 != null && obj.furnitureClasses[0].preferCouples && human3.partner != null)
												{
													if (!list2.Contains(human3.partner))
													{
														break;
													}
													human4 = human3.partner;
												}
												obj.AssignOwner(human4, false);
												human3 = human4;
												num3++;
											}
										}
										if (obj.furnitureClasses[0].assignBelongsToOwners > 0)
										{
											obj.UpdateIntegratedObjectOwnership();
										}
									}
								}
								if (obj != furnitureLocation)
								{
									furnitureLocation = obj;
								}
							}
							if (obj.furniture.furnitureGroup != FurniturePreset.FurnitureGroup.none && !this.furnitureGroups.ContainsKey(obj.furniture.furnitureGroup))
							{
								this.furnitureGroups.Add(obj.furniture.furnitureGroup, obj.furniture.groupID);
							}
							foreach (InteractablePreset interactablePreset in obj.furniture.spawnObjectsOnPlacement)
							{
								if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, CityData.Instance.seed + interactablePreset.presetName + this.roomID.ToString() + obj.anchorNode.nodeCoord.ToString(), false) <= obj.furniture.spawnObjectOnChance)
								{
									this.gameLocation.AddToPlacementPool(interactablePreset, null, null, null, null, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, null);
								}
							}
							if (obj.furnitureClasses.Count > 0)
							{
								if (obj.furnitureClasses.Exists((FurnitureClass item) => item.raiseLightswitch))
								{
									obj.RaiseLightswitch();
								}
							}
							obj.AssignID(this);
						}
						if (Game.Instance.devMode && Game.Instance.collectDebugData && SessionData.Instance.startedGame)
						{
							Game.Log(string.Concat(new string[]
							{
								"Decor: Adding new furniture: ",
								obj.furnitureClasses[0].name,
								" to ",
								this.GetName(),
								" (Generate new: ",
								generateNew.ToString(),
								", id: ",
								obj.id.ToString(),
								")"
							}), 2);
						}
						if (obj != null)
						{
							if (obj.furniture == null)
							{
								Game.LogError(string.Concat(new string[]
								{
									"Furniture ",
									obj.id.ToString(),
									" at ",
									this.name,
									", ",
									this.gameLocation.name,
									" has no preset reference..."
								}), 2);
							}
							else
							{
								if (!this.individualFurniture.Contains(obj))
								{
									this.individualFurniture.Add(obj);
								}
								keyValuePair.Key.AddFurniture(obj);
								if (obj.furniture.classes == null || obj.furniture.classes.Count <= 0 || obj.furniture.classes[0] == null)
								{
									Game.LogError(obj.furniture.name + " has no assigned furniture class!", 2);
								}
								else if (obj.furniture.classes[0].wallPiece)
								{
									foreach (FurnitureClass.FurnitureWallRule furnitureWallRule in obj.furniture.classes[0].wallRules)
									{
										Vector2Int offsetFromDirection = CityData.Instance.GetOffsetFromDirection(furnitureWallRule.wallDirection);
										Vector2 vector = Toolbox.Instance.RotateVector2CW(furnitureWallRule.nodeOffset, (float)obj.angle);
										Vector3Int vector3Int = obj.anchorNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
										Vector2 rotatedWallOffset = Toolbox.Instance.RotateVector2CW(offsetFromDirection, (float)obj.angle) * 0.5f;
										rotatedWallOffset = new Vector2(Mathf.Round(rotatedWallOffset.x * 2f) / 2f, Mathf.Round(rotatedWallOffset.y * 2f) / 2f);
										NewNode newNode = null;
										if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
										{
											NewWall newWall = newNode.walls.Find((NewWall item) => item.wallOffset == rotatedWallOffset);
											if (newWall != null)
											{
												newWall.placedWallFurn = true;
											}
										}
									}
								}
								foreach (FurnitureClass furnitureClass in obj.furnitureClasses)
								{
									if (furnitureClass.noPassThrough)
									{
										keyValuePair.Key.noPassThrough = true;
									}
									if (furnitureClass.noAccessNeeded)
									{
										this.noAccessNodes.Add(keyValuePair.Key);
									}
								}
								if (obj.furniture.isJobBoard && !CityData.Instance.jobBoardsDirectory.Contains(obj))
								{
									CityData.Instance.jobBoardsDirectory.Add(obj);
								}
								if (addPathBlocking)
								{
									foreach (NewNode newNode2 in obj.coversNodes)
									{
										using (List<FurnitureClass>.Enumerator enumerator3 = obj.furnitureClasses.GetEnumerator())
										{
											while (enumerator3.MoveNext())
											{
												if (enumerator3.Current.occupiesTile)
												{
													newNode2.SetAllowNewFurniture(false);
												}
											}
										}
									}
									this.AddFurnitureBlockedAccess(obj);
									this.AddFOVBlock(obj);
									this.AddCustomNodeWeights(obj);
								}
								if (generateNew && obj.furniture.alterAreaLighting)
								{
									foreach (NewRoom.LightZoneData lightZoneData in this.lightZones)
									{
										if (obj.furniture.possibleColours.Count > 0)
										{
											string seedInput = CityData.Instance.seed + this.roomID.ToString() + lightZoneData.nodeList.Count.ToString();
											Color color = obj.furniture.possibleColours[Toolbox.Instance.RandContained(0, obj.furniture.possibleColours.Count, seedInput, out seedInput)];
											if (obj.furniture.lightOperation == DistrictPreset.AffectStreetAreaLights.lerp)
											{
												lightZoneData.areaLightColour = Color.Lerp(lightZoneData.areaLightColour, color, obj.furniture.lightAmount);
											}
											else if (obj.furniture.lightOperation == DistrictPreset.AffectStreetAreaLights.multiply)
											{
												lightZoneData.areaLightColour *= color * obj.furniture.lightAmount;
											}
											else if (obj.furniture.lightOperation == DistrictPreset.AffectStreetAreaLights.add)
											{
												lightZoneData.areaLightColour += color * obj.furniture.lightAmount;
											}
											if (lightZoneData.spawnedAreaLight != null)
											{
												lightZoneData.spawnedAreaLight.color = lightZoneData.areaLightColour;
											}
										}
										lightZoneData.areaLightBrightness += obj.furniture.brightnessModifier;
										lightZoneData.debug.Add("Area light modification from furniture: " + obj.furniture.brightnessModifier.ToString());
										if (lightZoneData.spawnedAreaLight != null)
										{
											lightZoneData.aAdditional.intensity = lightZoneData.areaLightBrightness;
										}
									}
								}
							}
						}
					}
				}
			}
			this.furniture.Add(newFurn);
			if (newFurn.cluster.isBreakerBox && this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.addressPreset != null)
			{
				if (this.gameLocation.thisAsAddress.addressPreset.useOwnBreakerBox && this.gameLocation.thisAsAddress.breakerLightsID <= -1)
				{
					this.gameLocation.thisAsAddress.breakerLightsID = 0;
				}
				else if (this.gameLocation.floor != null && this.gameLocation.floor.breakerLightsID <= -1)
				{
					this.gameLocation.floor.breakerLightsID = 0;
				}
			}
			if (this.geometryLoaded)
			{
				newFurn.LoadFurnitureToWorld(immediateSpawn);
			}
			if (SessionData.Instance.startedGame)
			{
				ObjectPoolingController.Instance.UpdateObjectRanges();
				this.decorEdit = true;
				return;
			}
		}
		else
		{
			Game.Log("This furniture already exists here: " + ((newFurn != null) ? newFurn.ToString() : null), 2);
		}
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x00127194 File Offset: 0x00125394
	public FurnitureLocation AddFurnitureCustom(PlayerApartmentController.FurniturePlacement newPlacement)
	{
		if (newPlacement == null || newPlacement.anchorNode == null || newPlacement.preset == null)
		{
			return null;
		}
		FurnitureClusterLocation furnitureClusterLocation = new FurnitureClusterLocation(newPlacement.anchorNode, PlayerApartmentController.Instance.nullCluster, 0, 0f);
		string text = "Decor: Adding new furniture (new) ";
		string text2 = newPlacement.preset.name;
		string text3 = " with an offset of ";
		Vector3 offset = newPlacement.offset;
		Game.Log(text + text2 + text3 + offset.ToString(), 2);
		FurnitureLocation furnitureLocation = new FurnitureLocation(furnitureClusterLocation, newPlacement.preset.classes, newPlacement.angle, newPlacement.anchorNode, newPlacement.coversNodes, false, Vector3.zero, 0, Vector3.one, true, newPlacement.offset);
		furnitureLocation.furniture = newPlacement.preset;
		furnitureLocation.pickedMaterials = true;
		furnitureLocation.matKey = newPlacement.materialKey;
		if (newPlacement.art != null)
		{
			furnitureLocation.art = newPlacement.art;
			furnitureLocation.pickedArt = true;
		}
		foreach (NewNode newNode in furnitureLocation.coversNodes)
		{
			if (!furnitureClusterLocation.clusterObjectMap.ContainsKey(newNode))
			{
				furnitureClusterLocation.clusterObjectMap.Add(newNode, new List<FurnitureLocation>());
			}
			furnitureClusterLocation.clusterObjectMap[newNode].Add(furnitureLocation);
			if (!furnitureClusterLocation.clusterList.Contains(furnitureLocation))
			{
				furnitureClusterLocation.clusterList.Add(furnitureLocation);
			}
		}
		this.AddFurniture(furnitureClusterLocation, true, false, true);
		return furnitureLocation;
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x00127324 File Offset: 0x00125524
	public FurnitureLocation AddFurnitureCustom(FurnitureLocation newPlacement)
	{
		if (newPlacement == null)
		{
			Game.Log("Decor: Existing furniture class is null!", 2);
			return null;
		}
		if (newPlacement.anchorNode == null)
		{
			Game.Log("Decor: Existing furniture has null anchor node!", 2);
			return null;
		}
		string[] array = new string[6];
		array[0] = "Decor: Adding new furniture (existing) ";
		array[1] = newPlacement.furniture.name;
		array[2] = " at anchor ";
		int num = 3;
		Vector3 vector = newPlacement.anchorNode.position;
		array[num] = vector.ToString();
		array[4] = " with an offset of ";
		int num2 = 5;
		vector = newPlacement.offset;
		array[num2] = vector.ToString();
		Game.Log(string.Concat(array), 2);
		FurnitureClusterLocation furnitureClusterLocation = new FurnitureClusterLocation(newPlacement.anchorNode, PlayerApartmentController.Instance.nullCluster, 0, 0f);
		newPlacement.cluster = furnitureClusterLocation;
		furnitureClusterLocation.loadedGeometry = false;
		foreach (NewNode newNode in newPlacement.coversNodes)
		{
			if (!furnitureClusterLocation.clusterObjectMap.ContainsKey(newNode))
			{
				furnitureClusterLocation.clusterObjectMap.Add(newNode, new List<FurnitureLocation>());
			}
			furnitureClusterLocation.clusterObjectMap[newNode].Add(newPlacement);
			if (!furnitureClusterLocation.clusterList.Contains(newPlacement))
			{
				furnitureClusterLocation.clusterList.Add(newPlacement);
			}
		}
		this.AddFurniture(furnitureClusterLocation, false, false, true);
		return newPlacement;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x00127480 File Offset: 0x00125680
	public void AddFurnitureBlockedAccess(FurnitureLocation obj)
	{
		foreach (FurnitureClass furnitureClass in obj.furnitureClasses)
		{
			foreach (FurnitureClass.BlockedAccess blockedAccess in furnitureClass.blockedAccess)
			{
				if (!blockedAccess.disabled)
				{
					Vector2 vector = Toolbox.Instance.RotateVector2CW(blockedAccess.nodeOffset, (float)obj.angle);
					Vector3Int vector3Int = obj.anchorNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
					{
						foreach (CityData.BlockingDirection dir in blockedAccess.blocked)
						{
							Vector2 v = CityData.Instance.GetOffsetFromDirection(dir);
							Vector2 vector2 = Toolbox.Instance.RotateVector2CW(v, (float)obj.angle);
							Vector3Int vector3Int2 = newNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y), 0);
							NewNode newNode2 = null;
							if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2))
							{
								try
								{
									if (!newNode.room.blockedAccess.ContainsKey(newNode))
									{
										newNode.room.blockedAccess.Add(newNode, new List<NewNode>());
									}
									newNode.room.blockedAccess[newNode].Add(newNode2);
								}
								catch
								{
								}
								try
								{
									if (!newNode2.room.blockedAccess.ContainsKey(newNode2))
									{
										newNode2.room.blockedAccess.Add(newNode2, new List<NewNode>());
									}
									newNode2.room.blockedAccess[newNode2].Add(newNode);
								}
								catch
								{
								}
							}
						}
					}
					if (blockedAccess.blockExteriorDiagonals && blockedAccess.blocked.Contains(CityData.BlockingDirection.behindLeft))
					{
						Vector2 vector3 = Toolbox.Instance.RotateVector2CW(new Vector2(-1f, 0f), (float)obj.angle);
						Vector3Int vector3Int3 = vector3Int + new Vector3Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), 0);
						NewNode newNode3 = null;
						if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int3, ref newNode3))
						{
							Vector2 vector4 = Toolbox.Instance.RotateVector2CW(new Vector2(0f, -1f), (float)obj.angle);
							Vector3Int vector3Int4 = vector3Int + new Vector3Int(Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y), 0);
							NewNode newNode4 = null;
							if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int4, ref newNode4))
							{
								try
								{
									if (!newNode3.room.blockedAccess.ContainsKey(newNode3))
									{
										newNode3.room.blockedAccess.Add(newNode3, new List<NewNode>());
									}
									newNode3.room.blockedAccess[newNode3].Add(newNode4);
								}
								catch
								{
								}
								try
								{
									if (!newNode4.room.blockedAccess.ContainsKey(newNode4))
									{
										newNode4.room.blockedAccess.Add(newNode4, new List<NewNode>());
									}
									newNode4.room.blockedAccess[newNode4].Add(newNode3);
								}
								catch
								{
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x001278E0 File Offset: 0x00125AE0
	public void AddCustomNodeWeights(FurnitureLocation obj)
	{
		foreach (FurnitureClass furnitureClass in obj.furnitureClasses)
		{
			foreach (FurnitureClass.CustomNodeWeighting customNodeWeighting in furnitureClass.customNodeWeights)
			{
				if (!customNodeWeighting.disabled)
				{
					Vector2 vector = Toolbox.Instance.RotateVector2CW(customNodeWeighting.nodeOffset, (float)obj.angle);
					Vector3Int vector3Int = obj.anchorNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
					{
						newNode.AddToNodeWeightMultiplier(customNodeWeighting.nodeWeightModifier);
					}
				}
			}
		}
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x001279EC File Offset: 0x00125BEC
	private void AddFOVBlock(FurnitureLocation obj)
	{
		if (obj.useFOVBLock)
		{
			Vector2 vector = Toolbox.Instance.RotateVector2CW(obj.fovDirection, (float)obj.angle);
			foreach (NewNode newNode in obj.coversNodes)
			{
				Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
				for (int i = 0; i < obj.fovMaxDistance; i++)
				{
					NewNode newNode2 = null;
					if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) || !(newNode2.room == this) || !newNode2.allowNewFurniture)
					{
						break;
					}
					newNode2.SetAllowNewFurniture(false);
					vector3Int += new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
				}
			}
		}
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x00127AF8 File Offset: 0x00125CF8
	public bool AddRandomAirVent(NewAddress.AirVent ventType)
	{
		AirDuctGroup.AirVent airVent = null;
		if (ventType == NewAddress.AirVent.ceiling)
		{
			List<NewNode> list = new List<NewNode>();
			using (HashSet<NewNode>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NewNode node = enumerator.Current;
					Vector3Int vector3Int;
					vector3Int..ctor(node.nodeCoord.x, node.nodeCoord.y, node.nodeCoord.z * 3 + 2);
					if (this.building.validVentSpace.ContainsKey(vector3Int) && !this.mainLights.Exists((Interactable item) => item.node == node) && !this.airVents.Exists((AirDuctGroup.AirVent item) => item.node == node))
					{
						bool flag = true;
						using (List<NewWall>.Enumerator enumerator2 = node.walls.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NewWall wall = enumerator2.Current;
								if (wall.otherWall.node.room.airVents.Exists((AirDuctGroup.AirVent item) => item.wall == wall.otherWall || item.wall == wall))
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							for (int i = 0; i < node.walls.Count + 1; i++)
							{
								list.Add(node);
							}
						}
					}
				}
			}
			if (list.Count <= 0)
			{
				return false;
			}
			airVent = new AirDuctGroup.AirVent(ventType, this);
			airVent.node = list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, this.seed, false)];
			airVent.debugNode = airVent.node.position;
			Vector3Int vector3Int2;
			vector3Int2..ctor(airVent.node.nodeCoord.x, airVent.node.nodeCoord.y, airVent.node.nodeCoord.z * 3 + 1);
			if (this.building.validVentSpace.ContainsKey(vector3Int2))
			{
				this.building.validVentSpace.Remove(vector3Int2);
			}
		}
		else if (ventType == NewAddress.AirVent.wallLower || ventType == NewAddress.AirVent.wallUpper)
		{
			List<NewWall> list2 = new List<NewWall>();
			List<Vector3Int> list3 = new List<Vector3Int>();
			foreach (NewNode newNode in this.nodes)
			{
				if (!newNode.tile.isStairwell)
				{
					using (List<NewWall>.Enumerator enumerator2 = newNode.walls.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NewWall wall = enumerator2.Current;
							if (wall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall && !this.airVents.Exists((AirDuctGroup.AirVent item) => item.wall == wall || item.wall != wall.otherWall) && !wall.otherWall.node.room.airVents.Exists((AirDuctGroup.AirVent item) => item.wall == wall || item.wall != wall.otherWall) && !wall.otherWall.node.room.airVents.Exists((AirDuctGroup.AirVent item) => item.ventType == NewAddress.AirVent.ceiling && (item.node == wall.otherWall.node || item.node == wall.node)))
							{
								if (ventType == NewAddress.AirVent.wallUpper)
								{
									Vector3Int vector3Int3;
									vector3Int3..ctor(wall.otherWall.node.nodeCoord.x, wall.otherWall.node.nodeCoord.y, wall.otherWall.node.nodeCoord.z * 3 + 1);
									if (!this.building.validVentSpace.ContainsKey(vector3Int3))
									{
										continue;
									}
									if (wall.otherWall.node.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.ventUpper))
									{
										continue;
									}
									if (wall.node.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.ventUpper))
									{
										continue;
									}
									list3.Add(new Vector3Int(wall.node.nodeCoord.x, wall.node.nodeCoord.y, wall.node.nodeCoord.z * 3 + 1));
								}
								else if (ventType == NewAddress.AirVent.wallLower)
								{
									Vector3Int vector3Int4;
									vector3Int4..ctor(wall.otherWall.node.nodeCoord.x, wall.otherWall.node.nodeCoord.y, wall.otherWall.node.nodeCoord.z * 3);
									if (!this.building.validVentSpace.ContainsKey(vector3Int4))
									{
										continue;
									}
									if (wall.otherWall.node.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.ventLower))
									{
										continue;
									}
									if (wall.node.walls.Exists((NewWall item) => item.preset.sectionClass == DoorPairPreset.WallSectionClass.ventLower))
									{
										continue;
									}
									list3.Add(new Vector3Int(wall.node.nodeCoord.x, wall.node.nodeCoord.y, wall.node.nodeCoord.z * 3));
								}
								list2.Add(wall);
							}
						}
					}
				}
			}
			if (list2.Count <= 0)
			{
				Game.Log("CityGen: No valid wall vent locations for " + this.name, 2);
				return false;
			}
			airVent = new AirDuctGroup.AirVent(ventType, this);
			int psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0, list2.Count, this.seed, false);
			airVent.wall = list2[psuedoRandomNumber];
			airVent.node = airVent.wall.otherWall.node;
			airVent.roomNode = airVent.wall.node;
			airVent.debugNode = airVent.node.position;
			airVent.debugRoomNode = airVent.roomNode.position;
			if (this.building.validVentSpace.ContainsKey(list3[psuedoRandomNumber]))
			{
				this.building.validVentSpace.Remove(list3[psuedoRandomNumber]);
			}
			if (ventType == NewAddress.AirVent.wallLower)
			{
				if (airVent.wall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall)
				{
					airVent.wall.SetDoorPairPreset(InteriorControls.Instance.wallVentLower, false, false, true);
				}
				Vector3Int vector3Int5;
				vector3Int5..ctor(airVent.wall.node.nodeCoord.x, airVent.wall.node.nodeCoord.y, airVent.wall.node.nodeCoord.z * 3 + 1);
				if (this.building.validVentSpace.ContainsKey(vector3Int5))
				{
					this.building.validVentSpace.Remove(vector3Int5);
				}
			}
			else if (ventType == NewAddress.AirVent.wallUpper)
			{
				if (airVent.wall.preset.sectionClass == DoorPairPreset.WallSectionClass.wall)
				{
					airVent.wall.SetDoorPairPreset(InteriorControls.Instance.wallVentUpper, false, false, true);
				}
				Vector3Int vector3Int6;
				vector3Int6..ctor(airVent.wall.node.nodeCoord.x, airVent.wall.node.nodeCoord.y, airVent.wall.node.nodeCoord.z * 3);
				if (this.building.validVentSpace.ContainsKey(vector3Int6))
				{
					this.building.validVentSpace.Remove(vector3Int6);
				}
			}
		}
		if (airVent != null)
		{
			this.airVents.Add(airVent);
			this.LoadVent(airVent);
			return true;
		}
		return false;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x001283AC File Offset: 0x001265AC
	private void LoadVent(AirDuctGroup.AirVent vent)
	{
		if (vent.ventType == NewAddress.AirVent.ceiling)
		{
			vent.node.SetCeilingVent(true);
			return;
		}
		if (vent.ventType == NewAddress.AirVent.wallLower && vent.wall.parentWall != null)
		{
			vent.wall.SetDoorPairPreset(InteriorControls.Instance.wallVentLower, true, false, true);
			return;
		}
		if (vent.ventType == NewAddress.AirVent.wallUpper && vent.wall.parentWall != null)
		{
			vent.wall.SetDoorPairPreset(InteriorControls.Instance.wallVentUpper, true, false, true);
		}
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0012842B File Offset: 0x0012662B
	public void AddDuctGroup(AirDuctGroup newGroup)
	{
		if (!this.ductGroups.Contains(newGroup))
		{
			this.ductGroups.Add(newGroup);
		}
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x00128448 File Offset: 0x00126648
	public void AddOwner(Human newOwner)
	{
		if (!this.belongsTo.Contains(newOwner))
		{
			if (!this.gameLocation.thisAsAddress.roomsBelongTo.ContainsKey(this.preset.roomType))
			{
				this.gameLocation.thisAsAddress.roomsBelongTo.Add(this.preset.roomType, new Dictionary<NewRoom, List<Human>>());
			}
			if (!this.gameLocation.thisAsAddress.roomsBelongTo[this.preset.roomType].ContainsKey(this))
			{
				this.gameLocation.thisAsAddress.roomsBelongTo[this.preset.roomType].Add(this, new List<Human>());
			}
			this.gameLocation.thisAsAddress.roomsBelongTo[this.preset.roomType][this].Add(newOwner);
			this.belongsTo.Add(newOwner);
		}
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x00128538 File Offset: 0x00126738
	public void LoadOwners()
	{
		foreach (int num in this.loadBelongsTo)
		{
			if (num > -1)
			{
				Human newOwner = null;
				if (CityData.Instance.GetHuman(num, out newOwner, true))
				{
					this.AddOwner(newOwner);
				}
			}
		}
		foreach (FurnitureLocation furnitureLocation in this.individualFurniture)
		{
			foreach (int num2 in furnitureLocation.loadOwners)
			{
				if (num2 > 0)
				{
					Human newOwner2 = null;
					if (CityData.Instance.GetHuman(num2, out newOwner2, true))
					{
						furnitureLocation.AssignOwner(newOwner2, false);
					}
				}
				else if (num2 < 0)
				{
					furnitureLocation.AssignOwner(CityData.Instance.addressDictionary[-num2], false);
				}
			}
			if (!furnitureLocation.createdInteractables)
			{
				furnitureLocation.CreateInteractables();
			}
			else if (furnitureLocation.loadOwners.Count > 0)
			{
				furnitureLocation.UpdateIntegratedObjectOwnership();
			}
		}
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0012868C File Offset: 0x0012688C
	public void PickPassword()
	{
		if (this.passcode.used)
		{
			HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
			hashSet.Add(this);
			if (this.belongsTo != null && this.belongsTo.Count > 0)
			{
				bool flag = false;
				this.passcode.digits.Clear();
				this.passcode.digits.AddRange(this.belongsTo[0].passcode.digits);
				if (!flag)
				{
					float num = 0.5f;
					if (this.belongsTo[0].characterTraits.Exists((Human.Trait item) => item.trait.name == "Quirk-Forgetful"))
					{
						num = 1f;
					}
					if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, this.seed, false) <= num)
					{
						Interactable interactable = this.belongsTo[0].WriteNote(Human.NoteObject.note, "65a356f3-72da-4378-b0c5-9d6ad2f6de98", this.belongsTo[0], this.gameLocation, 3, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, null, false, 0, 0, null);
						if (interactable != null)
						{
							this.passcode.notes.Add(interactable.id);
						}
						else
						{
							Game.Log("Error placing note outside of room! Trying again... " + this.name, 2);
							interactable = this.belongsTo[0].WriteNote(Human.NoteObject.note, "65a356f3-72da-4378-b0c5-9d6ad2f6de98", this.belongsTo[0], this.gameLocation, 3, InteractablePreset.OwnedPlacementRule.ownedOnly, 3, hashSet, false, 0, 0, null);
							if (interactable != null)
							{
								this.passcode.notes.Add(interactable.id);
							}
						}
					}
					bool flag2 = false;
					List<string> list = new List<string>();
					List<InteractablePreset.OwnedPlacementRule> list2 = new List<InteractablePreset.OwnedPlacementRule>();
					List<Human> list3 = new List<Human>();
					if (this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.company != null)
					{
						list.Add("5012b526-cfb0-4ff3-b39c-4effb810e86a");
						list2.Add(InteractablePreset.OwnedPlacementRule.nonOwnedOnly);
						list3.Add(this.belongsTo[0]);
					}
					if (this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.company != null && this.gameLocation.thisAsAddress.company.receptionist != null)
					{
						list.Add("85e316f2-528c-4354-a620-24992f8cc9e2");
						list2.Add(InteractablePreset.OwnedPlacementRule.ownedOnly);
						list3.Add(this.gameLocation.thisAsAddress.company.receptionist);
					}
					if (this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.company != null && this.gameLocation.thisAsAddress.company.janitor != null)
					{
						list.Add("d9e2526d-37f8-4e9e-9da9-a68f54065dd0");
						list2.Add(InteractablePreset.OwnedPlacementRule.ownedOnly);
						list3.Add(this.gameLocation.thisAsAddress.company.janitor);
					}
					while (!flag2)
					{
						if (list.Count <= 0)
						{
							return;
						}
						int psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, this.seed, false);
						Interactable interactable2 = this.belongsTo[0].WriteNote(Human.NoteObject.note, list[psuedoRandomNumber], list3[psuedoRandomNumber], this.gameLocation, 0, list2[psuedoRandomNumber], 0, hashSet, false, 0, 0, null);
						if (interactable2 != null)
						{
							this.passcode.notes.Add(interactable2.id);
							return;
						}
						Game.Log("Error placing note outside of room! Trying again... " + this.name, 2);
						interactable2 = this.belongsTo[0].WriteNote(Human.NoteObject.note, list[psuedoRandomNumber], list3[psuedoRandomNumber], this.gameLocation, 0, list2[psuedoRandomNumber], 0, hashSet, false, 0, 0, null);
						if (interactable2 != null)
						{
							this.passcode.notes.Add(interactable2.id);
							return;
						}
						list.RemoveAt(psuedoRandomNumber);
						list2.RemoveAt(psuedoRandomNumber);
						list3.RemoveAt(psuedoRandomNumber);
					}
				}
			}
			else
			{
				Game.Log("CityGen: Cannot pick a password for room that belongs to nobody: " + this.preset.name, 2);
			}
		}
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x00128AA4 File Offset: 0x00126CA4
	public void SetupEnvrionment()
	{
		if (this.preset.ambientZone != null)
		{
			AudioController.AmbientZoneInstance ambientZoneInstance;
			if (AudioController.Instance.ambientZoneReference.TryGetValue(this.preset.ambientZone, ref ambientZoneInstance))
			{
				AudioController.Instance.ambientZoneReference[this.preset.ambientZone].rooms.Add(this);
				return;
			}
			Game.LogError("Ambient zone " + this.preset.ambientZone.name + " not loaded!", 2);
		}
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x00128B30 File Offset: 0x00126D30
	public void SetExplorationLevel(int newLevel)
	{
		if (newLevel != this.explorationLevel)
		{
			int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.moneyForLocations));
			if (newLevel > this.explorationLevel && this.explorationLevel <= 0 && (float)num > 0f)
			{
				GameplayController.Instance.AddMoney(num, false, "moneyforblueprints");
			}
			this.explorationLevel = newLevel;
			if (this.explorationLevel >= 1 && this.gameLocation.evidenceEntry != null)
			{
				this.gameLocation.evidenceEntry.OnPlayerArrival();
			}
			foreach (RectTransform rectTransform in this.mapDoors)
			{
				if (this.explorationLevel >= 1)
				{
					if (!rectTransform.gameObject.activeSelf)
					{
						rectTransform.gameObject.SetActive(true);
					}
				}
				else if (rectTransform.gameObject.activeSelf)
				{
					rectTransform.gameObject.SetActive(false);
				}
			}
			foreach (NewNode.NodeAccess nodeAccess in this.entrances)
			{
				if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.adjacent || nodeAccess.accessType == NewNode.NodeAccess.AccessType.bannister || nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway || nodeAccess.accessType == NewNode.NodeAccess.AccessType.streetToStreet)
				{
					NewRoom otherRoom = nodeAccess.GetOtherRoom(this);
					if (otherRoom.explorationLevel < this.explorationLevel)
					{
						otherRoom.SetExplorationLevel(this.explorationLevel);
					}
				}
			}
			if (this.explorationLevel >= 2)
			{
				foreach (NewNode newNode in this.nodes)
				{
					foreach (AirDuctGroup.AirDuctSection airDuctSection in newNode.airDucts)
					{
						if (airDuctSection.level < 2)
						{
							airDuctSection.SetDiscovered(true);
						}
					}
				}
				foreach (AirDuctGroup.AirVent airVent in this.airVents)
				{
					airVent.SetDiscovered(true);
				}
			}
			if (this.airVents.Count > 0)
			{
				MapController.Instance.AddDuctUpdateCall(this.floor.mapDucts);
			}
			if (this.gameLocation.mapButton != null)
			{
				MapController.Instance.AddUpdateCall(this.gameLocation.mapButton);
			}
		}
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x00128DDC File Offset: 0x00126FDC
	public bool TestForDynamicShadowsUpdate()
	{
		if (this.actorUpdate)
		{
			foreach (Actor actor in this.currentOccupants)
			{
				Human human = (Human)actor;
				if (!human.isPlayer && !human.isAsleep && !human.isDead)
				{
					return true;
				}
			}
		}
		foreach (NewNode.NodeAccess nodeAccess in this.entrances)
		{
			if (nodeAccess.door != null && nodeAccess.door.animating)
			{
				return true;
			}
		}
		foreach (Interactable interactable in this.worldObjects)
		{
			if (interactable.controller != null && interactable.controller.physicsOn)
			{
				return true;
			}
		}
		foreach (NewNode newNode in this.nodes)
		{
			foreach (Interactable interactable2 in newNode.interactables)
			{
				if (interactable2.controller != null && interactable2.controller.doorMovement != null && interactable2.controller.doorMovement.isAnimating)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x00128FCC File Offset: 0x001271CC
	public int CompareTo(NewRoom otherObject)
	{
		return this.roomType.cyclePriority.CompareTo(otherObject.roomType.cyclePriority);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x00128FEC File Offset: 0x001271EC
	public CitySaveData.RoomCitySave GenerateSaveData()
	{
		CitySaveData.RoomCitySave roomCitySave = new CitySaveData.RoomCitySave();
		roomCitySave.name = this.name;
		foreach (NewNode newNode in this.nodes)
		{
			roomCitySave.nodes.Add(newNode.GenerateSaveData());
		}
		foreach (RoomConfiguration roomConfiguration in this.openPlanElements)
		{
			roomCitySave.openPlanElements.Add(roomConfiguration.name);
		}
		foreach (NewRoom newRoom in this.commonRooms)
		{
			roomCitySave.commonRooms.Add(newRoom.roomID);
		}
		foreach (AirDuctGroup.AirVent airVent in this.airVents)
		{
			CitySaveData.AirVentSave airVentSave = new CitySaveData.AirVentSave
			{
				ventType = airVent.ventType,
				id = airVent.ventID
			};
			if (airVent.node != null)
			{
				airVentSave.node = airVent.node.nodeCoord;
			}
			if (airVent.wall != null)
			{
				airVentSave.wall = airVent.wall.id;
			}
			if (airVent.roomNode != null)
			{
				airVentSave.rNode = airVent.roomNode.nodeCoord;
			}
			roomCitySave.airVents.Add(airVentSave);
		}
		foreach (NewRoom.LightZoneData lightZoneData in this.lightZones)
		{
			CitySaveData.LightZoneSave lightZoneSave = new CitySaveData.LightZoneSave();
			foreach (NewNode newNode2 in lightZoneData.nodeList)
			{
				lightZoneSave.n.Add(newNode2.nodeCoord);
			}
			lightZoneSave.areaLightBright = lightZoneData.areaLightBrightness;
			lightZoneSave.areaLightColour = lightZoneData.areaLightColour;
			roomCitySave.lightZones.Add(lightZoneSave);
		}
		roomCitySave.id = this.roomID;
		roomCitySave.fID = this.furnitureAssignID;
		roomCitySave.iID = this.interactableAssignID;
		roomCitySave.floorID = this.roomFloorID;
		roomCitySave.preset = this.preset.name;
		roomCitySave.reachableFromEntrance = this.reachableFromEntrance;
		roomCitySave.isOutsideWindow = this.isOutsideWindow;
		roomCitySave.allowCoving = this.allowCoving;
		roomCitySave.floorMaterial = this.floorMaterial.name;
		roomCitySave.floorMatKey = this.floorMatKey;
		roomCitySave.ceilingMaterial = this.ceilingMaterial.name;
		roomCitySave.ceilingMatKey = this.ceilingMatKey;
		roomCitySave.defaultWallMaterial = this.defaultWallMaterial.name;
		roomCitySave.defaultWallKey = this.defaultWallKey;
		roomCitySave.miscKey = this.miscKey;
		if (this.colourScheme != null)
		{
			roomCitySave.colourScheme = this.colourScheme.name;
		}
		if (this.mainLightPreset != null)
		{
			roomCitySave.mainLightPreset = this.mainLightPreset.name;
		}
		roomCitySave.isBaseNullRoom = this.isBaseNullRoom;
		roomCitySave.middle = this.middleRoomPosition;
		roomCitySave.password = this.passcode;
		roomCitySave.cf = this.ceilingFans;
		foreach (Human human in this.belongsTo)
		{
			roomCitySave.owners.Add(human.humanID);
		}
		foreach (FurnitureClusterLocation furnitureClusterLocation in this.furniture)
		{
			roomCitySave.f.Add(furnitureClusterLocation.GenerateSaveData());
		}
		foreach (KeyValuePair<NewRoom, List<NewRoom.CullTreeEntry>> keyValuePair in this.cullingTree)
		{
			foreach (NewRoom.CullTreeEntry cullTreeEntry in keyValuePair.Value)
			{
				CitySaveData.CullTreeSave cullTreeSave = new CitySaveData.CullTreeSave
				{
					r = keyValuePair.Key.roomID
				};
				cullTreeSave.d = cullTreeEntry.requiredOpenDoors;
				roomCitySave.cullTree.Add(cullTreeSave);
			}
		}
		foreach (NewRoom newRoom2 in this.aboveRooms)
		{
			roomCitySave.above.Add(newRoom2.roomID);
		}
		foreach (NewRoom newRoom3 in this.belowRooms)
		{
			roomCitySave.below.Add(newRoom3.roomID);
		}
		foreach (NewRoom newRoom4 in this.adjacentRooms)
		{
			roomCitySave.adj.Add(newRoom4.roomID);
		}
		foreach (NewRoom newRoom5 in this.nonAudioOccludedRooms)
		{
			roomCitySave.occ.Add(newRoom5.roomID);
		}
		return roomCitySave;
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x00129640 File Offset: 0x00127840
	public NewNode GetRandomNode()
	{
		Random random = new Random();
		NewNode[] array = Enumerable.ToArray<NewNode>(this.nodes);
		return array[random.Next(array.Length)];
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0012966C File Offset: 0x0012786C
	public NewNode GetRandomEntranceNode()
	{
		if (this.entrances.Count > 0)
		{
			NewNode.NodeAccess nodeAccess = this.entrances[Toolbox.Instance.Rand(0, this.entrances.Count, false)];
			return nodeAccess.fromNode;
		}
		return this.GetRandomNode();
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x001296BC File Offset: 0x001278BC
	public bool IsAccessAllowed(Human human)
	{
		int num;
		string text;
		bool result = !human.IsTrespassing(this, out num, out text, true);
		if (Game.Instance.collectDebugData && human.isPlayer)
		{
			Game.Log("Player: " + text, 2);
		}
		return result;
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x00129700 File Offset: 0x00127900
	public void RemoveAllInhabitantFurniture(bool removeSkipAddressInhabitantsFurniture, FurnitureClusterLocation.RemoveInteractablesOption spawnedOnFurnitureRemovalOption)
	{
		foreach (FurnitureClusterLocation furnitureClusterLocation in new List<FurnitureClusterLocation>(this.furniture))
		{
			if (removeSkipAddressInhabitantsFurniture || !furnitureClusterLocation.cluster.skipIfNoAddressInhabitants)
			{
				furnitureClusterLocation.DeleteCluster(true, spawnedOnFurnitureRemovalOption);
				furnitureClusterLocation.UnloadFurniture(true, spawnedOnFurnitureRemovalOption);
				Game.Log("Removing furniture cluster for " + this.GetName(), 2);
				this.furniture.Remove(furnitureClusterLocation);
			}
		}
		foreach (NewNode newNode in this.nodes)
		{
			List<Interactable> list = newNode.interactables.FindAll((Interactable item) => item.wo && !this.mainLights.Contains(item));
			for (int i = 0; i < list.Count; i++)
			{
				if (SessionData.Instance.isFloorEdit || !PlayerApartmentController.Instance.itemStorage.Contains(list[i]))
				{
					list[i].SafeDelete(false);
				}
			}
			newNode.SetAllowNewFurniture(true);
			newNode.noPassThrough = false;
			foreach (NewWall newWall in newNode.walls)
			{
				newWall.placedWallFurn = false;
				if (newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && !newWall.preset.divider)
				{
					newNode.SetAllowNewFurniture(false);
				}
			}
			for (int j = 0; j < newNode.interactables.Count; j++)
			{
				if (newNode.interactables[j] == null)
				{
					newNode.interactables.RemoveAt(j);
					j--;
				}
			}
		}
		this.blockedAccess = new Dictionary<NewNode, List<NewNode>>();
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0012991C File Offset: 0x00127B1C
	public void SetSteam(bool val)
	{
		if (val)
		{
			if (!this.steamOn)
			{
				this.steamOn = true;
				this.steamLastSwitched = SessionData.Instance.gameTime;
				if (this.steamController != null)
				{
					this.steamController.SteamStateChanged();
					return;
				}
			}
		}
		else if (!this.steamControllingInteractables.Exists((Interactable item) => item.sw0) && this.steamOn)
		{
			this.steamOn = false;
			this.steamLastSwitched = SessionData.Instance.gameTime;
			if (this.steamController != null)
			{
				this.steamController.SteamStateChanged();
			}
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x001299CC File Offset: 0x00127BCC
	public bool IsOutside()
	{
		if (this.preset != null)
		{
			if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceInside)
			{
				return false;
			}
			if (this.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
			{
				return true;
			}
		}
		return this.gameLocation.IsOutside() && this.isOutsideWindow;
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x00129A20 File Offset: 0x00127C20
	public List<NewRoom> GetAdjacentRooms()
	{
		List<NewRoom> list = new List<NewRoom>();
		foreach (NewNode newNode in this.nodes)
		{
			foreach (NewWall newWall in newNode.walls)
			{
				if (newWall.otherWall != null)
				{
					NewNode node = newWall.otherWall.node;
					if (node != null && node.room != null && node.room != this && !list.Contains(node.room))
					{
						list.Add(node.room);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x00129B08 File Offset: 0x00127D08
	public List<NewRoom> GetAboveRooms()
	{
		List<NewRoom> list = new List<NewRoom>();
		foreach (NewNode newNode in this.nodes)
		{
			Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(0, 0, 1);
			NewNode newNode2 = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && !list.Contains(newNode2.room))
			{
				list.Add(newNode2.room);
			}
		}
		return list;
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x00129BA4 File Offset: 0x00127DA4
	public List<NewRoom> GetBelowRooms()
	{
		List<NewRoom> list = new List<NewRoom>();
		foreach (NewNode newNode in this.nodes)
		{
			Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(0, 0, -1);
			NewNode newNode2 = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && !list.Contains(newNode2.room))
			{
				list.Add(newNode2.room);
			}
		}
		return list;
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x00129C40 File Offset: 0x00127E40
	public List<NewRoom> GetAboveAndBelowRooms()
	{
		List<NewRoom> list = new List<NewRoom>();
		foreach (NewNode newNode in this.nodes)
		{
			Vector3Int vector3Int = newNode.nodeCoord + new Vector3Int(0, 0, 1);
			Vector3Int vector3Int2 = newNode.nodeCoord + new Vector3Int(0, 0, -1);
			NewNode newNode2 = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode2) && !list.Contains(newNode2.room))
			{
				list.Add(newNode2.room);
			}
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2) && !list.Contains(newNode2.room))
			{
				list.Add(newNode2.room);
			}
		}
		return list;
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x00129D2C File Offset: 0x00127F2C
	[Button(null, 0)]
	public void DisplaySublocations()
	{
		if (this.sublocationParent != null)
		{
			Object.Destroy(this.sublocationParent);
		}
		this.sublocationParent = new GameObject();
		this.sublocationParent.transform.SetParent(base.transform);
		this.sublocationParent.transform.localPosition = Vector3.zero;
		foreach (NewNode newNode in this.nodes)
		{
			foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in newNode.walkableNodeSpace)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.walkPointSphere, this.sublocationParent.transform);
				gameObject.transform.position = keyValuePair.Value.position;
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.sublocationDebugObjects.Add(gameObject);
				}
				gameObject.GetComponent<DebugWalkableSublocation>().Setup(newNode, keyValuePair.Value);
			}
			foreach (Interactable interactable in newNode.interactables)
			{
				if (interactable.usagePoint != null)
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(PrefabControls.Instance.usePointSphere, this.sublocationParent.transform);
					gameObject2.name = interactable.name;
					Vector3 usageWorldPosition = interactable.usagePoint.GetUsageWorldPosition(newNode.position, null);
					gameObject2.transform.position = new Vector3(usageWorldPosition.x, interactable.wPos.y, usageWorldPosition.z);
					gameObject2.transform.rotation = Quaternion.LookRotation(interactable.usagePoint.worldLookAtPoint - gameObject2.transform.position, Vector3.up);
					if (Game.Instance.devMode && Game.Instance.collectDebugData)
					{
						this.sublocationDebugObjects.Add(gameObject2);
					}
				}
			}
		}
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00129FAC File Offset: 0x001281AC
	[Button(null, 0)]
	public void RemoveSublocationsDisplay()
	{
		if (this.sublocationParent != null)
		{
			Object.Destroy(this.sublocationParent);
		}
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x00129FC8 File Offset: 0x001281C8
	[Button("Teleport Player", 0)]
	public void DebugTeleportPlayerToLocation()
	{
		Random random = new Random();
		NewNode[] array = Enumerable.ToArray<NewNode>(this.nodes);
		NewNode teleportLocation = array[random.Next(array.Length)];
		Player.Instance.Teleport(teleportLocation, null, true, false);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0012A001 File Offset: 0x00128201
	[Button(null, 0)]
	public void DebugCullingDisplay()
	{
		while (this.spawnPathDebug.Count > 0)
		{
			Toolbox.Instance.DestroyObject(this.spawnPathDebug[0].gameObject);
			this.spawnPathDebug.RemoveAt(0);
		}
		this.GenerateCullingTree(true);
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x0012A044 File Offset: 0x00128244
	[Button(null, 0)]
	public void GetMainLightData()
	{
		this.mainLightObjects.Clear();
		foreach (Interactable interactable in this.mainLights)
		{
			if (interactable.controller != null)
			{
				this.mainLightObjects.Add(interactable.controller);
			}
		}
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0012A0BC File Offset: 0x001282BC
	[Button(null, 0)]
	public void ToggleExteriorWindowDebug()
	{
		if (this.exteriorWindowDebug.Count > 0)
		{
			while (this.exteriorWindowDebug.Count > 0)
			{
				Object.Destroy(this.exteriorWindowDebug[0]);
				this.exteriorWindowDebug.RemoveAt(0);
			}
			return;
		}
		foreach (NewWall newWall in this.windowsWithUVData)
		{
			(newWall.windowUV.localMeshPositionLeft + newWall.windowUV.localMeshPositionRight) / 2f;
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.pathfindNodeDebug, this.building.buildingModelBase.transform);
			gameObject.transform.localPosition = (newWall.windowUV.localMeshPositionLeft + newWall.windowUV.localMeshPositionRight) / 2f;
			gameObject.transform.SetParent(base.transform, true);
			Object @object = gameObject;
			string[] array = new string[8];
			array[0] = "wHP: ";
			array[1] = newWall.windowUVHorizonalPosition.ToString();
			array[2] = " floor: ";
			array[3] = newWall.windowUV.floor.ToString();
			array[4] = " side: ";
			int num = 5;
			Vector2 side = newWall.windowUV.side;
			array[num] = side.ToString();
			array[6] = " horz: ";
			array[7] = newWall.windowUV.horizonal.ToString();
			@object.name = string.Concat(array);
			this.exteriorWindowDebug.Add(gameObject);
		}
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0012A26C File Offset: 0x0012846C
	[Button(null, 0)]
	public void TestFurniturePlacementBlockingCheck()
	{
		Dictionary<NewNode, List<NewNode>> dictionary = new Dictionary<NewNode, List<NewNode>>();
		List<NewNode> list = new List<NewNode>();
		List<string> list2 = new List<string>();
		Game.Log(GenerationController.Instance.IsFurniturePlacementValid(this, ref dictionary, ref list, ref list, true, out list2, false), 2);
		foreach (string print in list2)
		{
			Game.Log(print, 2);
		}
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x0012A2EC File Offset: 0x001284EC
	[Button("Test Furniture Placement Blocking Check (Ignore No Passthrough)", 0)]
	public void TestFurniturePlacementBlockingCheckIgnoreNoPassthrough()
	{
		Dictionary<NewNode, List<NewNode>> dictionary = new Dictionary<NewNode, List<NewNode>>();
		List<NewNode> list = new List<NewNode>();
		List<string> list2 = new List<string>();
		Game.Log(GenerationController.Instance.IsFurniturePlacementValid(this, ref dictionary, ref list, ref list, true, out list2, true), 2);
		foreach (string print in list2)
		{
			Game.Log(print, 2);
		}
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0012A36C File Offset: 0x0012856C
	[Button(null, 0)]
	public void DisplayNodePositions()
	{
		foreach (NewNode newNode in this.nodes)
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			gameObject.transform.SetParent(base.transform, true);
			gameObject.AddComponent<DebugNode>().Setup(newNode);
			gameObject.transform.position = newNode.position;
			Object @object = gameObject;
			string[] array = new string[7];
			array[0] = newNode.nodeCoord.ToString();
			array[1] = ": Tile: ";
			int num = 2;
			Vector3Int globalTileCoord = newNode.tile.globalTileCoord;
			array[num] = globalTileCoord.ToString();
			array[3] = " ";
			array[4] = newNode.floorType.ToString();
			array[5] = " Use opt ceiling: ";
			array[6] = newNode.tile.useOptimizedCeiling.ToString();
			@object.name = string.Concat(array);
			this.nodeDebug.Add(gameObject);
		}
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x0012A480 File Offset: 0x00128680
	[Button(null, 0)]
	public void RemoveNodePositions()
	{
		while (this.nodeDebug.Count > 0)
		{
			Object.Destroy(this.nodeDebug[0]);
			this.nodeDebug.RemoveAt(0);
		}
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0012A4B0 File Offset: 0x001286B0
	public int GetWallCount()
	{
		int num = 0;
		foreach (NewNode newNode in this.nodes)
		{
			num += newNode.walls.Count;
		}
		return num;
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x0012A510 File Offset: 0x00128710
	[Button(null, 0)]
	public void GetAIActions()
	{
		foreach (KeyValuePair<AIActionPreset, List<Interactable>> keyValuePair in this.actionReference)
		{
			Game.Log(keyValuePair.Key.name + " (" + keyValuePair.Value.Count.ToString() + ")", 2);
		}
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x0012A594 File Offset: 0x00128794
	[Button(null, 0)]
	public void GetInteractables()
	{
		foreach (NewNode newNode in this.nodes)
		{
			foreach (Interactable interactable in newNode.interactables)
			{
				Game.Log(interactable.name + " (" + interactable.id.ToString() + ")", 2);
			}
		}
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0012A640 File Offset: 0x00128840
	[Button(null, 0)]
	public void ListContainedInteractables()
	{
		foreach (NewNode newNode in this.nodes)
		{
			for (int i = 0; i < newNode.interactables.Count; i++)
			{
				Game.Log("Contained in " + this.name + ": " + newNode.interactables[i].name, 2);
			}
		}
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0012A6D0 File Offset: 0x001288D0
	[Button(null, 0)]
	public void ListActionReferences()
	{
		foreach (KeyValuePair<AIActionPreset, List<Interactable>> keyValuePair in this.actionReference)
		{
			foreach (Interactable interactable in keyValuePair.Value)
			{
				Game.Log(string.Concat(new string[]
				{
					keyValuePair.Key.ToString(),
					": ",
					interactable.GetName(),
					" ",
					interactable.id.ToString(),
					" at ",
					interactable.GetWorldPosition(true).ToString()
				}), 2);
			}
		}
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0012A7C8 File Offset: 0x001289C8
	public SessionData.SceneProfile GetEnvironment()
	{
		if (!SessionData.Instance.isFloorEdit)
		{
			if (this.preset != null && this.preset.overrideAddressEnvironment)
			{
				if (this.defaultWallKey.grubiness > 0.5f)
				{
					return this.preset.sceneDirty;
				}
				return this.preset.sceneClean;
			}
			else
			{
				if (this.gameLocation != null && this.gameLocation.thisAsAddress != null && this.gameLocation.thisAsAddress.addressPreset != null && this.gameLocation.thisAsAddress.addressPreset.overrideBuildingEnvironment)
				{
					return this.gameLocation.thisAsAddress.addressPreset.sceneProfile;
				}
				if (this.building != null && this.building.preset.overrideDistrictEnvironment && !this.IsOutside())
				{
					return this.building.preset.sceneProfile;
				}
				if (this.gameLocation != null && this.gameLocation.district != null)
				{
					return this.gameLocation.district.preset.sceneProfile;
				}
			}
		}
		return SessionData.SceneProfile.outdoors;
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x0012A900 File Offset: 0x00128B00
	public void AddGas(float amount)
	{
		this.gasLevel += amount;
		this.gasLevel = Mathf.Clamp01(this.gasLevel);
		if (this.gasLevel > 0f && !GameplayController.Instance.gasRooms.Contains(this))
		{
			GameplayController.Instance.gasRooms.Add(this);
		}
		else if (this.gasLevel <= 0f)
		{
			GameplayController.Instance.gasRooms.Remove(this);
		}
		if (this.gasLevel >= 0.95f)
		{
			this.lastRoomGassed = SessionData.Instance.gameTime;
		}
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x0012A998 File Offset: 0x00128B98
	[Button(null, 0)]
	public void RebuildCullingTree()
	{
		this.completedTreeCull = false;
		while (this.spawnPathDebug.Count > 0)
		{
			Toolbox.Instance.DestroyObject(this.spawnPathDebug[0].gameObject);
			this.spawnPathDebug.RemoveAt(0);
		}
		Game.Log("Build culling tree for " + this.name, 2);
		this.GenerateCullingTree(true);
		Player.Instance.UpdateCullingOnEndOfFrame();
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x0012AA0A File Offset: 0x00128C0A
	[Button(null, 0)]
	public void IsThisOutside()
	{
		Game.Log(this.IsOutside(), 2);
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x0012AA20 File Offset: 0x00128C20
	[Button(null, 0)]
	public void SpawnModularRoomElements()
	{
		List<MeshFilter> list;
		Dictionary<NewBuilding, List<MeshFilter>> dictionary;
		List<MeshFilter> list2;
		List<MeshFilter> list3;
		MeshPoolingController.Instance.SpawnModularRoomElements(this, true, out list, out dictionary, out list2, out list3);
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x0012AA44 File Offset: 0x00128C44
	[Button(null, 0)]
	public void ListLoadedFurniture()
	{
		Game.Log(this.GetName() + " has " + this.furniture.Count.ToString() + " furniture clusters", 2);
		Game.Log(this.GetName() + " has " + this.individualFurniture.Count.ToString() + " individual furniture items", 2);
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x0012AAAD File Offset: 0x00128CAD
	public bool GetSecondaryLightStatus()
	{
		return this.secondaryLights.Exists((Interactable item) => item.sw0);
	}

	// Token: 0x04001870 RID: 6256
	public string name;

	// Token: 0x04001871 RID: 6257
	[Header("Location")]
	public NewBuilding building;

	// Token: 0x04001872 RID: 6258
	public NewFloor floor;

	// Token: 0x04001873 RID: 6259
	public NewGameLocation gameLocation;

	// Token: 0x04001874 RID: 6260
	public NewRoom lowerRoom;

	// Token: 0x04001875 RID: 6261
	[Header("Room Contents")]
	public int furnitureAssignID = 1;

	// Token: 0x04001876 RID: 6262
	public int interactableAssignID = 1;

	// Token: 0x04001877 RID: 6263
	public HashSet<NewNode> nodes = new HashSet<NewNode>();

	// Token: 0x04001878 RID: 6264
	public List<RoomConfiguration> openPlanElements = new List<RoomConfiguration>();

	// Token: 0x04001879 RID: 6265
	public List<NewNode.NodeAccess> entrances = new List<NewNode.NodeAccess>();

	// Token: 0x0400187A RID: 6266
	public List<NewRoom.RoomDivider> roomDividers = new List<NewRoom.RoomDivider>();

	// Token: 0x0400187B RID: 6267
	public List<NewRoom.LightZoneData> lightZones = new List<NewRoom.LightZoneData>();

	// Token: 0x0400187C RID: 6268
	public Vector3 middleRoomPosition;

	// Token: 0x0400187D RID: 6269
	public List<NewRoom> commonRooms = new List<NewRoom>();

	// Token: 0x0400187E RID: 6270
	public HashSet<Actor> currentOccupants = new HashSet<Actor>();

	// Token: 0x0400187F RID: 6271
	public GameObject streetObjectContainer;

	// Token: 0x04001880 RID: 6272
	public HashSet<Interactable> tamperedInteractables = new HashSet<Interactable>();

	// Token: 0x04001881 RID: 6273
	public List<NewNode> noAccessNodes = new List<NewNode>();

	// Token: 0x04001882 RID: 6274
	public HashSet<Interactable> worldObjects = new HashSet<Interactable>();

	// Token: 0x04001883 RID: 6275
	public List<Human.ConversationInstance> activeConversations = new List<Human.ConversationInstance>();

	// Token: 0x04001884 RID: 6276
	public List<NewWall> windows = new List<NewWall>();

	// Token: 0x04001885 RID: 6277
	public List<AudioController.LoopingSoundInfo> audibleLoopingSounds = new List<AudioController.LoopingSoundInfo>();

	// Token: 0x04001886 RID: 6278
	public Dictionary<FurniturePreset.FurnitureGroup, int> furnitureGroups = new Dictionary<FurniturePreset.FurnitureGroup, int>();

	// Token: 0x04001887 RID: 6279
	public List<Interactable> heatSources = new List<Interactable>();

	// Token: 0x04001888 RID: 6280
	public List<PipeConstructor.PipeGroup> pipes = new List<PipeConstructor.PipeGroup>();

	// Token: 0x04001889 RID: 6281
	[Header("Details")]
	public int roomFloorID = -1;

	// Token: 0x0400188A RID: 6282
	public static int assignRoomFloorID = 1;

	// Token: 0x0400188B RID: 6283
	public int roomID = -1;

	// Token: 0x0400188C RID: 6284
	public static int assignRoomID = 1;

	// Token: 0x0400188D RID: 6285
	public string seed;

	// Token: 0x0400188E RID: 6286
	public RoomTypePreset roomType;

	// Token: 0x0400188F RID: 6287
	public RoomConfiguration preset;

	// Token: 0x04001890 RID: 6288
	public Vector3 worldPos;

	// Token: 0x04001891 RID: 6289
	public bool calculatedWorldPos;

	// Token: 0x04001892 RID: 6290
	public Vector2 boundsSize;

	// Token: 0x04001893 RID: 6291
	public bool geometryLoaded;

	// Token: 0x04001894 RID: 6292
	public bool reachableFromEntrance;

	// Token: 0x04001895 RID: 6293
	public bool isOutsideWindow;

	// Token: 0x04001896 RID: 6294
	public bool isNullRoom;

	// Token: 0x04001897 RID: 6295
	public bool isBaseNullRoom;

	// Token: 0x04001898 RID: 6296
	public bool featuresStairwell;

	// Token: 0x04001899 RID: 6297
	public bool uniqueCeilingMaterial;

	// Token: 0x0400189A RID: 6298
	public bool containsDead;

	// Token: 0x0400189B RID: 6299
	public bool decorEdit;

	// Token: 0x0400189C RID: 6300
	public bool isVisible;

	// Token: 0x0400189D RID: 6301
	public bool musicPlaying;

	// Token: 0x0400189E RID: 6302
	public float musicStartedAt;

	// Token: 0x0400189F RID: 6303
	[Header("Decor")]
	public bool allowCoving;

	// Token: 0x040018A0 RID: 6304
	public MaterialGroupPreset floorMaterial;

	// Token: 0x040018A1 RID: 6305
	public Toolbox.MaterialKey floorMatKey;

	// Token: 0x040018A2 RID: 6306
	public Material floorMat;

	// Token: 0x040018A3 RID: 6307
	public MaterialGroupPreset ceilingMaterial;

	// Token: 0x040018A4 RID: 6308
	public Toolbox.MaterialKey ceilingMatKey;

	// Token: 0x040018A5 RID: 6309
	public Material ceilingMat;

	// Token: 0x040018A6 RID: 6310
	public MaterialGroupPreset defaultWallMaterial;

	// Token: 0x040018A7 RID: 6311
	public Toolbox.MaterialKey defaultWallKey;

	// Token: 0x040018A8 RID: 6312
	public Material wallMat;

	// Token: 0x040018A9 RID: 6313
	public bool hasBeenDecorated;

	// Token: 0x040018AA RID: 6314
	public Toolbox.MaterialKey miscKey;

	// Token: 0x040018AB RID: 6315
	public ColourSchemePreset colourScheme;

	// Token: 0x040018AC RID: 6316
	[Header("Lights")]
	public RoomLightingPreset mainLightPreset;

	// Token: 0x040018AD RID: 6317
	public bool mainLightStatus;

	// Token: 0x040018AE RID: 6318
	public List<NewWall> lightswitches = new List<NewWall>();

	// Token: 0x040018AF RID: 6319
	public List<Interactable> lightswitchInteractables = new List<Interactable>();

	// Token: 0x040018B0 RID: 6320
	public List<Interactable> mainLights = new List<Interactable>();

	// Token: 0x040018B1 RID: 6321
	public List<Interactable> secondaryLights = new List<Interactable>();

	// Token: 0x040018B2 RID: 6322
	public bool enabledLights = true;

	// Token: 0x040018B3 RID: 6323
	public List<NewWall> windowsWithUVData = new List<NewWall>();

	// Token: 0x040018B4 RID: 6324
	public int ceilingFans = -1;

	// Token: 0x040018B5 RID: 6325
	private bool hasHash;

	// Token: 0x040018B6 RID: 6326
	private int hash;

	// Token: 0x040018B7 RID: 6327
	public List<GenerationController.OverrideData> overrideData = new List<GenerationController.OverrideData>();

	// Token: 0x040018B8 RID: 6328
	private bool actorUpdate;

	// Token: 0x040018B9 RID: 6329
	[Header("Occlusion")]
	public Dictionary<NewRoom, List<NewRoom.CullTreeEntry>> cullingTree = new Dictionary<NewRoom, List<NewRoom.CullTreeEntry>>();

	// Token: 0x040018BA RID: 6330
	public HashSet<int> doorCheckSet = new HashSet<int>();

	// Token: 0x040018BB RID: 6331
	public HashSet<NewRoom> nonAudioOccludedRooms = new HashSet<NewRoom>();

	// Token: 0x040018BC RID: 6332
	public HashSet<NewDoor> openDoors = new HashSet<NewDoor>();

	// Token: 0x040018BD RID: 6333
	public HashSet<NewDoor> closedDoors = new HashSet<NewDoor>();

	// Token: 0x040018BE RID: 6334
	public HashSet<NewRoom> adjacentRooms = new HashSet<NewRoom>();

	// Token: 0x040018BF RID: 6335
	public HashSet<NewRoom> aboveRooms = new HashSet<NewRoom>();

	// Token: 0x040018C0 RID: 6336
	public HashSet<NewRoom> belowRooms = new HashSet<NewRoom>();

	// Token: 0x040018C1 RID: 6337
	public NewRoom atriumTop;

	// Token: 0x040018C2 RID: 6338
	public List<NewRoom> atriumRooms = new List<NewRoom>();

	// Token: 0x040018C3 RID: 6339
	public GameObject combinedWalls;

	// Token: 0x040018C4 RID: 6340
	public MeshRenderer combinedWallRend;

	// Token: 0x040018C5 RID: 6341
	public Dictionary<NewBuilding, GameObject> additionalWalls = new Dictionary<NewBuilding, GameObject>();

	// Token: 0x040018C6 RID: 6342
	public GameObject combinedFloor;

	// Token: 0x040018C7 RID: 6343
	public MeshRenderer combinedFloorRend;

	// Token: 0x040018C8 RID: 6344
	public GameObject combinedCeiling;

	// Token: 0x040018C9 RID: 6345
	public MeshRenderer combinedCeilingRend;

	// Token: 0x040018CA RID: 6346
	public int ambientSoundLevel;

	// Token: 0x040018CB RID: 6347
	private List<CitySaveData.CullTreeSave> ct;

	// Token: 0x040018CC RID: 6348
	private List<int> above;

	// Token: 0x040018CD RID: 6349
	private List<int> below;

	// Token: 0x040018CE RID: 6350
	private List<int> adj;

	// Token: 0x040018CF RID: 6351
	private List<int> occ;

	// Token: 0x040018D0 RID: 6352
	[Header("Furniture")]
	public List<FurnitureClusterLocation> furniture;

	// Token: 0x040018D1 RID: 6353
	public List<FurnitureLocation> individualFurniture;

	// Token: 0x040018D2 RID: 6354
	private Dictionary<NewRoom.StaticBatchKey, List<GameObject>> staticBatchDictionary = new Dictionary<NewRoom.StaticBatchKey, List<GameObject>>();

	// Token: 0x040018D3 RID: 6355
	[NonSerialized]
	public Dictionary<FurnitureClass, List<FurniturePreset>> pickFurnitureCache;

	// Token: 0x040018D4 RID: 6356
	[NonSerialized]
	public Dictionary<Vector3, NewNode> localizedRoomNodeMaps;

	// Token: 0x040018D5 RID: 6357
	[Header("Footprints")]
	public bool footprintUpdateQueued;

	// Token: 0x040018D6 RID: 6358
	public List<FootprintController> spawnedFootprints = new List<FootprintController>();

	// Token: 0x040018D7 RID: 6359
	[Header("AI Navigation")]
	public Dictionary<NewNode, List<NewNode>> blockedAccess = new Dictionary<NewNode, List<NewNode>>();

	// Token: 0x040018D8 RID: 6360
	public Dictionary<AIActionPreset, List<Interactable>> actionReference = new Dictionary<AIActionPreset, List<Interactable>>();

	// Token: 0x040018D9 RID: 6361
	public Dictionary<InteractablePreset.SpecialCase, List<Interactable>> specialCaseInteractables = new Dictionary<InteractablePreset.SpecialCase, List<Interactable>>();

	// Token: 0x040018DA RID: 6362
	[Header("Ownership")]
	private List<int> loadBelongsTo = new List<int>();

	// Token: 0x040018DB RID: 6363
	public List<Human> belongsTo = new List<Human>();

	// Token: 0x040018DC RID: 6364
	[Header("Exploration")]
	[Tooltip("Is this room shown on the map?")]
	public int explorationLevel;

	// Token: 0x040018DD RID: 6365
	public List<RectTransform> mapDoors = new List<RectTransform>();

	// Token: 0x040018DE RID: 6366
	[Header("Air Vents")]
	public List<AirDuctGroup.AirVent> airVents = new List<AirDuctGroup.AirVent>();

	// Token: 0x040018DF RID: 6367
	public List<AirDuctGroup> ductGroups = new List<AirDuctGroup>();

	// Token: 0x040018E0 RID: 6368
	[Header("Passwords")]
	public GameplayController.Passcode passcode;

	// Token: 0x040018E1 RID: 6369
	[Header("Crime Scene Elements")]
	public List<SpatterSimulation> spatter = new List<SpatterSimulation>();

	// Token: 0x040018E2 RID: 6370
	[Header("Environment")]
	public List<Interactable> steamControllingInteractables = new List<Interactable>();

	// Token: 0x040018E3 RID: 6371
	public bool steamOn;

	// Token: 0x040018E4 RID: 6372
	public float steamLastSwitched;

	// Token: 0x040018E5 RID: 6373
	public SteamController steamController;

	// Token: 0x040018E6 RID: 6374
	public List<BugController> spawnedBugs = new List<BugController>();

	// Token: 0x040018E7 RID: 6375
	public float gasLevel;

	// Token: 0x040018E8 RID: 6376
	public float lastRoomGassed;

	// Token: 0x040018E9 RID: 6377
	[Header("Debug")]
	public GenerationDebugController debugController;

	// Token: 0x040018EA RID: 6378
	public Action UpdateEmission;

	// Token: 0x040018EB RID: 6379
	public bool completedTreeCull;

	// Token: 0x040018EC RID: 6380
	public List<string> debugLightswitches = new List<string>();

	// Token: 0x040018ED RID: 6381
	public int cullingDebugLoadReference;

	// Token: 0x040018EE RID: 6382
	private List<CullingDebugController> spawnPathDebug = new List<CullingDebugController>();

	// Token: 0x040018EF RID: 6383
	public string debugCulling;

	// Token: 0x040018F0 RID: 6384
	public NewRoom specificRoomCullingDebug;

	// Token: 0x040018F1 RID: 6385
	public bool loadedCullTreeFromSave;

	// Token: 0x040018F2 RID: 6386
	public List<InteractableController> mainLightObjects = new List<InteractableController>();

	// Token: 0x040018F3 RID: 6387
	public List<string> debugDecor = new List<string>();

	// Token: 0x040018F4 RID: 6388
	private List<GameObject> exteriorWindowDebug = new List<GameObject>();

	// Token: 0x040018F5 RID: 6389
	private List<GameObject> nodeDebug = new List<GameObject>();

	// Token: 0x040018F6 RID: 6390
	public List<string> debugAddActions = new List<string>();

	// Token: 0x040018F7 RID: 6391
	public string clustersPlaced;

	// Token: 0x040018F8 RID: 6392
	public string itemsPlaced;

	// Token: 0x040018F9 RID: 6393
	public int poolSizeOnPlacement;

	// Token: 0x040018FA RID: 6394
	public string palcementKey1;

	// Token: 0x040018FB RID: 6395
	public string palcementKey2;

	// Token: 0x040018FC RID: 6396
	public string palcementKey3;

	// Token: 0x040018FD RID: 6397
	public string palcementKey4;

	// Token: 0x040018FE RID: 6398
	public string palcementKey5;

	// Token: 0x040018FF RID: 6399
	public string palcementKey51;

	// Token: 0x04001900 RID: 6400
	public string palcementKey52;

	// Token: 0x04001901 RID: 6401
	public string palcementKey6;

	// Token: 0x04001902 RID: 6402
	public string keyAtStart;

	// Token: 0x04001903 RID: 6403
	[Space(7f)]
	private GameObject sublocationParent;

	// Token: 0x04001904 RID: 6404
	private List<GameObject> sublocationDebugObjects = new List<GameObject>();

	// Token: 0x02000377 RID: 887
	[Serializable]
	public class RoomDivider
	{
		// Token: 0x04001905 RID: 6405
		public NewRoom fromRoom;

		// Token: 0x04001906 RID: 6406
		public NewRoom toRoom;

		// Token: 0x04001907 RID: 6407
		public List<NewWall> dividerWalls;
	}

	// Token: 0x02000378 RID: 888
	[Serializable]
	public class LightZoneData
	{
		// Token: 0x06001459 RID: 5209 RVA: 0x0012AD9C File Offset: 0x00128F9C
		public LightZoneData(NewRoom newRoom, List<NewNode> newNodeList)
		{
			this.room = newRoom;
			this.nodeList = newNodeList;
			this.centreWorldPosition = Vector3.zero;
			Vector2 vector;
			vector..ctor(999999f, -999999f);
			Vector2 vector2;
			vector2..ctor(999999f, -999999f);
			float num = 0f;
			foreach (NewNode newNode in this.nodeList)
			{
				this.centreWorldPosition += newNode.position;
				vector = new Vector2(Mathf.Min(vector.x, newNode.position.x), Mathf.Max(vector.y, newNode.position.x));
				vector2 = new Vector2(Mathf.Min(vector2.x, newNode.position.z), Mathf.Max(vector2.y, newNode.position.z));
				num += 1f;
			}
			if (num > 0f)
			{
				this.centreWorldPosition /= num;
			}
			this.worldSize = new Vector2(vector.y - vector.x, vector2.y - vector2.x);
			float num2 = 99999f;
			foreach (NewNode newNode2 in this.nodeList)
			{
				float num3 = Vector3.Distance(newNode2.position, this.centreWorldPosition);
				if (num3 < num2)
				{
					num2 = num3;
					this.centreNode = newNode2;
				}
			}
			if (this.room.preset.useAdditionalAreaLights & (CityConstructor.Instance == null || CityConstructor.Instance.generateNew))
			{
				this.areaLightColour = this.room.preset.areaLightColor;
				this.areaLightBrightness = this.room.preset.areaLightBrightness;
				this.debug.Add("Set to room preset area light brightness: " + this.areaLightBrightness.ToString());
				if (this.room.preset.useDistrictSettingsAsBase && this.room.gameLocation.district != null && this.room.gameLocation.district.preset.alterStreetAreaLighting)
				{
					string seedInput = CityData.Instance.seed + this.room.roomID.ToString() + this.nodeList.Count.ToString();
					Color color = this.room.gameLocation.district.preset.possibleColours[Toolbox.Instance.RandContained(0, this.room.gameLocation.district.preset.possibleColours.Count, seedInput, out seedInput)];
					if (this.room.gameLocation.district.preset.lightOperation == DistrictPreset.AffectStreetAreaLights.lerp)
					{
						this.areaLightColour = Color.Lerp(this.areaLightColour, color, this.room.gameLocation.district.preset.lightAmount);
					}
					else if (this.room.gameLocation.district.preset.lightOperation == DistrictPreset.AffectStreetAreaLights.multiply)
					{
						this.areaLightColour *= color * this.room.gameLocation.district.preset.lightAmount;
					}
					else if (this.room.gameLocation.district.preset.lightOperation == DistrictPreset.AffectStreetAreaLights.add)
					{
						this.areaLightColour += color * this.room.gameLocation.district.preset.lightAmount;
					}
					this.areaLightBrightness += this.room.gameLocation.district.preset.brightnessModifier;
					this.debug.Add("Add district modifier to brightness: " + this.room.gameLocation.district.preset.brightnessModifier.ToString());
				}
			}
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x0012B21C File Offset: 0x0012941C
		private void FindBestLightPosition()
		{
			if (this.room.mainLightPreset == null && !this.room.preset.useAdditionalAreaLights)
			{
				this.allowLight = false;
				return;
			}
			List<NewRoom.LightZoneData.LightNodeRank> list = new List<NewRoom.LightZoneData.LightNodeRank>();
			int num = 0;
			this.allowLight = true;
			foreach (NewNode newNode in this.nodeList)
			{
				if (!(this.room.gameLocation.thisAsStreet != null) || !this.room.preset.useAdditionalAreaLights)
				{
					if (newNode.floorType != NewNode.FloorTileType.floorAndCeiling && newNode.floorType != NewNode.FloorTileType.CeilingOnly)
					{
						num++;
						continue;
					}
					if (this.room.gameLocation.floor.defaultCeilingHeight >= 44)
					{
						if (newNode.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level >= 1))
						{
							num++;
							continue;
						}
					}
					if (this.room.gameLocation.floor.defaultCeilingHeight < 44)
					{
						if (newNode.airDucts.Exists((AirDuctGroup.AirDuctSection item) => item.level == 1))
						{
							num++;
							continue;
						}
					}
					if (newNode.individualFurniture.Exists((FurnitureLocation item) => item.furnitureClasses[0].ceilingPiece && item.furnitureClasses[0].blocksCeiling))
					{
						num++;
						continue;
					}
				}
				list.Add(new NewRoom.LightZoneData.LightNodeRank
				{
					node = newNode,
					rank = Vector3.Distance(newNode.position, this.centreWorldPosition)
				});
			}
			if (num + 1 >= this.nodeList.Count)
			{
				this.allowLight = false;
			}
			if (this.allowLight && list.Count > 0)
			{
				list.Sort((NewRoom.LightZoneData.LightNodeRank p1, NewRoom.LightZoneData.LightNodeRank p2) => p1.rank.CompareTo(p2.rank));
				List<NewNode> list2 = new List<NewNode>();
				list2.Add(list[0].node);
				float rank = list[0].rank;
				list.RemoveAt(0);
				for (int i = 0; i < list.Count; i++)
				{
					NewRoom.LightZoneData.LightNodeRank lightNodeRank = list[i];
					if (lightNodeRank.rank > rank + 0.01f)
					{
						break;
					}
					bool flag = false;
					foreach (NewNode newNode2 in list2)
					{
						if (Vector3.Distance(lightNodeRank.node.nodeCoord, newNode2.nodeCoord) <= 1.5f)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						list2.Add(lightNodeRank.node);
					}
				}
				this.lightSpawnPosition = Vector3.zero;
				foreach (NewNode newNode3 in list2)
				{
					this.lightSpawnPosition += newNode3.position;
				}
				this.lightSpawnPosition /= (float)list2.Count;
			}
			else
			{
				this.allowLight = false;
			}
			this.bestPosFound = true;
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0012B5C4 File Offset: 0x001297C4
		public void CreateMainLight()
		{
			if (!this.bestPosFound)
			{
				this.FindBestLightPosition();
			}
			if (!this.allowLight)
			{
				return;
			}
			int num = 42;
			if (this.centreNode.floor != null)
			{
				num = this.centreNode.floor.defaultCeilingHeight;
			}
			if (this.centreNode.room.roomType.overrideFloorHeight)
			{
				num = this.centreNode.room.roomType.floorHeight;
			}
			Vector3 worldPos;
			worldPos..ctor(this.lightSpawnPosition.x, this.lightSpawnPosition.y + (float)(num - this.centreNode.floorHeight - 1) * 0.1f, this.lightSpawnPosition.z);
			if (this.room.mainLightPreset == null)
			{
				Game.Log("No light presets for " + this.room.name, 2);
				return;
			}
			Vector3 zero = Vector3.zero;
			if (this.worldSize.x > this.worldSize.y)
			{
				zero..ctor(0f, 90f, 0f);
			}
			InteractableCreator instance = InteractableCreator.Instance;
			List<InteractablePreset> lightObjects = this.room.mainLightPreset.lightObjects;
			Toolbox instance2 = Toolbox.Instance;
			int lowerRange = 0;
			int count = this.room.mainLightPreset.lightObjects.Count;
			Vector3 vector = this.centreWorldPosition;
			instance.CreateMainLightInteractable(lightObjects[instance2.GetPsuedoRandomNumber(lowerRange, count, vector.ToString() + this.room.roomID.ToString(), false)], this.room, worldPos, zero, this.room.mainLightPreset.lightingPreset, null, this.nodeList.Count);
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0012B76C File Offset: 0x0012996C
		public bool CreateAreaLight()
		{
			if (!this.bestPosFound)
			{
				this.FindBestLightPosition();
			}
			if (!this.allowLight)
			{
				return false;
			}
			if (this.nodeList.Count < this.room.preset.minimumLightZoneSizeForAreaLights)
			{
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(InteriorControls.Instance.roomAreaLight, this.room.transform);
			int num = 42;
			if (this.centreNode.floor != null)
			{
				num = this.centreNode.floor.defaultCeilingHeight;
			}
			if (this.centreNode.room.roomType.overrideFloorHeight)
			{
				num = this.centreNode.room.roomType.floorHeight;
			}
			gameObject.transform.localPosition = this.room.transform.InverseTransformPoint(this.lightSpawnPosition + new Vector3(0f, (float)(num - this.centreNode.floorHeight - 1) * 0.1f, 0f)) + this.room.preset.areaLightOffset;
			this.spawnedAreaLight = gameObject.GetComponentInChildren<Light>();
			this.aAdditional = this.spawnedAreaLight.transform.GetComponent<HDAdditionalLightData>();
			if (this.room == null || this.room.building == null)
			{
				this.aAdditional.lightlayersMask = 2;
			}
			else if (this.room.building != null)
			{
				if (this.room.building.interiorLightCullingLayer == 0)
				{
					this.aAdditional.lightlayersMask = 4;
				}
				else if (this.room.building.interiorLightCullingLayer == 1)
				{
					this.aAdditional.lightlayersMask = 8;
				}
				else if (this.room.building.interiorLightCullingLayer == 2)
				{
					this.aAdditional.lightlayersMask = 16;
				}
				else if (this.room.building.interiorLightCullingLayer == 3)
				{
					this.aAdditional.lightlayersMask = 32;
				}
			}
			Vector2 vector = this.worldSize * this.room.preset.areaLightCoverageMultiplier;
			this.aAdditional.SetAreaLightSize(new Vector2(Mathf.Max(vector.x, 0.1f), Mathf.Max(vector.y, 0.1f)));
			this.spawnedAreaLight.color = this.areaLightColour;
			this.aAdditional.intensity = this.areaLightBrightness;
			this.aAdditional.range = this.room.preset.areaLightRange;
			this.aAdditional.shadowDimmer = this.room.preset.areaLightShadowDimmer;
			this.aAdditional.penumbraTint = true;
			if (this.room.preset.areaLightingShadowTint)
			{
				if (this.room.preset.overrideAreaLightShadowTint)
				{
					this.aAdditional.shadowTint = this.room.preset.areaLightShadowTintOverride;
				}
				else
				{
					this.aAdditional.shadowTint = this.room.GetShadowTint(this.areaLightColour, this.room.preset.areaLightingShadowTintIntensity);
				}
			}
			this.spawnedAreaLight.gameObject.SetActive(this.room.mainLightStatus);
			return true;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0012BAA5 File Offset: 0x00129CA5
		public void RemoveAreaLight()
		{
			if (this.spawnedAreaLight != null)
			{
				Object.Destroy(this.spawnedAreaLight.gameObject);
			}
			this.spawnedAreaLight = null;
		}

		// Token: 0x04001908 RID: 6408
		public NewRoom room;

		// Token: 0x04001909 RID: 6409
		public List<NewNode> nodeList;

		// Token: 0x0400190A RID: 6410
		public Vector3 centreWorldPosition;

		// Token: 0x0400190B RID: 6411
		public Vector3 lightSpawnPosition;

		// Token: 0x0400190C RID: 6412
		public Vector2 worldSize;

		// Token: 0x0400190D RID: 6413
		public NewNode centreNode;

		// Token: 0x0400190E RID: 6414
		public Light spawnedAreaLight;

		// Token: 0x0400190F RID: 6415
		public HDAdditionalLightData aAdditional;

		// Token: 0x04001910 RID: 6416
		public bool allowLight = true;

		// Token: 0x04001911 RID: 6417
		public bool bestPosFound;

		// Token: 0x04001912 RID: 6418
		public List<string> debug = new List<string>();

		// Token: 0x04001913 RID: 6419
		public Color areaLightColour = Color.white;

		// Token: 0x04001914 RID: 6420
		public float areaLightBrightness;

		// Token: 0x02000379 RID: 889
		public class LightNodeRank
		{
			// Token: 0x04001915 RID: 6421
			public NewNode node;

			// Token: 0x04001916 RID: 6422
			public float rank;
		}
	}

	// Token: 0x0200037B RID: 891
	public struct StaticBatchKey
	{
		// Token: 0x06001465 RID: 5221 RVA: 0x0012BB2C File Offset: 0x00129D2C
		public bool Equals(NewRoom.StaticBatchKey other)
		{
			return object.Equals(other, this);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0012BB44 File Offset: 0x00129D44
		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			NewRoom.StaticBatchKey staticBatchKey = (NewRoom.StaticBatchKey)obj;
			return staticBatchKey.mesh == this.mesh && staticBatchKey.mat == this.mat;
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x0012BBA0 File Offset: 0x00129DA0
		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add<Mesh>(this.mesh);
			hashCode.Add<Material>(this.mat);
			return hashCode.ToHashCode();
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0012BBD6 File Offset: 0x00129DD6
		public static bool operator ==(NewRoom.StaticBatchKey c1, NewRoom.StaticBatchKey c2)
		{
			return c1.Equals(c2);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0012BBE0 File Offset: 0x00129DE0
		public static bool operator !=(NewRoom.StaticBatchKey c1, NewRoom.StaticBatchKey c2)
		{
			return !c1.Equals(c2);
		}

		// Token: 0x0400191C RID: 6428
		public Mesh mesh;

		// Token: 0x0400191D RID: 6429
		public Material mat;
	}

	// Token: 0x0200037C RID: 892
	public struct PathKey : IEquatable<NewRoom.PathKey>
	{
		// Token: 0x0600146A RID: 5226 RVA: 0x0012BBED File Offset: 0x00129DED
		public PathKey(NewNode locOne, NewNode locTwo)
		{
			this.origin = locOne;
			this.destination = locTwo;
			this.hasHash = false;
			this.hash = 0;
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0012BC0B File Offset: 0x00129E0B
		public override int GetHashCode()
		{
			if (!this.hasHash)
			{
				this.hash = HashCode.Combine<NewNode, NewNode>(this.origin, this.destination);
				this.hasHash = true;
			}
			return this.hash;
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x0012BC39 File Offset: 0x00129E39
		bool IEquatable<NewRoom.PathKey>.Equals(NewRoom.PathKey other)
		{
			return other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x0400191E RID: 6430
		public NewNode origin;

		// Token: 0x0400191F RID: 6431
		public NewNode destination;

		// Token: 0x04001920 RID: 6432
		private bool hasHash;

		// Token: 0x04001921 RID: 6433
		private int hash;
	}

	// Token: 0x0200037D RID: 893
	[Serializable]
	public class CullTreeEntry
	{
		// Token: 0x0600146D RID: 5229 RVA: 0x0012BC56 File Offset: 0x00129E56
		public CullTreeEntry(List<int> newRequiredDoors)
		{
			this.requiredOpenDoors = newRequiredDoors;
		}

		// Token: 0x04001922 RID: 6434
		public List<int> requiredOpenDoors;
	}
}
