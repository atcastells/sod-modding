using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200038C RID: 908
public class NewWall
{
	// Token: 0x060014AE RID: 5294 RVA: 0x0012DB7C File Offset: 0x0012BD7C
	public void Setup(DoorPairPreset newPreset, NewNode newNode, Vector2 newOffset, bool newIsExterior)
	{
		this.id = NewWall.assignID;
		NewWall.assignID++;
		this.wallOffset = newOffset;
		newNode.AddNewWall(this);
		this.preset = newPreset;
		this.isExterior = newIsExterior;
		if (this.wallOffset.x < 0f)
		{
			this.localEulerAngles = new Vector3(0f, 90f, 0f);
		}
		else if (this.wallOffset.x > 0f)
		{
			this.localEulerAngles = new Vector3(0f, 270f, 0f);
		}
		else if (this.wallOffset.y < 0f)
		{
			this.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else if (this.wallOffset.y > 0f)
		{
			this.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		this.position = this.node.position + new Vector3(this.wallOffset.x * (PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier - 0.2f), (float)newNode.floorHeight * -0.1f, this.wallOffset.y * (PathFinder.Instance.tileSize.y / (float)CityControls.Instance.nodeMultiplier - 0.2f));
		if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
		{
			this.SetDoorPairPreset(this.preset, true, false, true);
		}
		this.SetWallMaterial(this.node.room.defaultWallMaterial, this.node.room.defaultWallKey);
		if (SessionData.Instance.isFloorEdit)
		{
			if (this.physicalObject == null)
			{
				this.physicalObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.wall, this.node.room.transform);
				this.physicalObject.transform.localEulerAngles = this.localEulerAngles;
				this.physicalObject.transform.position = this.position;
				FloorEditWallDetector floorEditWallDetector = this.physicalObject.AddComponent<FloorEditWallDetector>();
				floorEditWallDetector.debugNodePosition = this.node.position;
				floorEditWallDetector.debugFloorHeight = this.node.floorHeight;
				floorEditWallDetector.wall = this;
				if (this.spawnedWall != null)
				{
					this.spawnedWall.transform.SetParent(this.physicalObject.transform, true);
				}
			}
			this.physicalObject.transform.position = this.position;
			this.editorTrigger = Toolbox.Instance.SpawnObject(PrefabControls.Instance.wallTrigger, this.physicalObject.transform);
			FloorEditController.Instance.wallTriggers.Add(this.editorTrigger);
		}
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x0012DE5C File Offset: 0x0012C05C
	public void Load(CitySaveData.WallCitySave data, NewNode newNode)
	{
		this.wallOffset = data.wo;
		this.id = data.id;
		NewWall.assignID = Mathf.Max(NewWall.assignID, this.id + 1);
		if (!CityConstructor.Instance.loadingWallsReference.ContainsKey(this.id))
		{
			CityConstructor.Instance.loadingWallsReference.Add(this.id, this);
		}
		Toolbox.Instance.LoadDataFromResources<DoorPairPreset>(data.p, out this.preset);
		foreach (CitySaveData.WallFrontageSave wallFrontageSave in data.fr)
		{
			WallFrontagePreset c = null;
			Toolbox.Instance.LoadDataFromResources<WallFrontagePreset>(wallFrontageSave.str, out c);
			if (c != null)
			{
				this.frontagePresets.Add(new NewWall.FrontageSetting
				{
					preset = c,
					matKey = wallFrontageSave.matKey,
					offset = wallFrontageSave.o
				});
			}
		}
		if (data.dm)
		{
			this.foundDoorMaterialKey = true;
			this.doorMatKey = data.dmk;
		}
		this.otherWallID = data.ow;
		this.parentWallID = data.pw;
		this.childWallID = data.cw;
		this.separateWall = data.sw;
		this.optimizationOverride = data.oo;
		this.optimizationAnchor = data.oa;
		this.nonOptimizedSegment = data.nos;
		this.isShortWall = data.isw;
		this.SetLockStrengthBase(data.ls);
		this.SetDoorStrengthBase(data.ds);
		newNode.AddNewWall(this);
		if (this.wallOffset.x < 0f)
		{
			this.localEulerAngles = new Vector3(0f, 90f, 0f);
		}
		else if (this.wallOffset.x > 0f)
		{
			this.localEulerAngles = new Vector3(0f, 270f, 0f);
		}
		else if (this.wallOffset.y < 0f)
		{
			this.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else if (this.wallOffset.y > 0f)
		{
			this.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		this.position = this.node.position + new Vector3(this.wallOffset.x * (PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier - 0.2f), (float)newNode.floorHeight * -0.1f, this.wallOffset.y * (PathFinder.Instance.tileSize.y / (float)CityControls.Instance.nodeMultiplier - 0.2f));
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x0012E140 File Offset: 0x0012C340
	public void SetDoorStrength(float newVal)
	{
		this.currentDoorStrength = newVal;
		this.currentDoorStrength = Mathf.Clamp01(this.currentDoorStrength);
		if (this.otherWall != null)
		{
			this.otherWall.currentDoorStrength = newVal;
		}
		if (this.door != null)
		{
			if ((this.door.doorInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.door.doorInteractable.controller) || (this.door.handleInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.door.handleInteractable.controller))
			{
				InteractionController.Instance.DisplayInteractionCursor(InteractionController.Instance.displayingInteraction, true);
			}
			if (this.currentDoorStrength < this.baseDoorStrength && !GameplayController.Instance.damagedDoors.Contains(this.door))
			{
				GameplayController.Instance.damagedDoors.Add(this.door);
			}
			if (this.currentDoorStrength <= 0f)
			{
				this.door.SetJammed(false, null, true);
				this.door.SetLocked(false, Player.Instance, true);
				this.door.SetKnowLockedStatus(true);
			}
		}
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x0012E26F File Offset: 0x0012C46F
	public void SetLockStrengthBase(float newVal)
	{
		this.baseLockStrength = newVal;
		if (this.otherWall != null)
		{
			this.otherWall.baseLockStrength = newVal;
		}
		this.ResetLockStrength();
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x0012E292 File Offset: 0x0012C492
	public void ResetLockStrength()
	{
		this.currentLockStrength = this.baseLockStrength;
		if (this.otherWall != null)
		{
			this.otherWall.currentLockStrength = this.baseLockStrength;
		}
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x0012E2B9 File Offset: 0x0012C4B9
	public void SetDoorStrengthBase(float newVal)
	{
		this.baseDoorStrength = newVal;
		if (this.otherWall != null)
		{
			this.otherWall.baseDoorStrength = newVal;
		}
		this.ResetDoorStrength();
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x0012E2DC File Offset: 0x0012C4DC
	public void ResetDoorStrength()
	{
		this.currentDoorStrength = this.baseDoorStrength;
		if (this.otherWall != null)
		{
			this.otherWall.currentDoorStrength = this.baseDoorStrength;
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x0012E304 File Offset: 0x0012C504
	public void SetCurrentLockStrength(float newVal)
	{
		this.currentLockStrength = newVal;
		this.currentLockStrength = Mathf.Clamp01(this.currentLockStrength);
		if (this.otherWall != null)
		{
			this.otherWall.currentLockStrength = this.currentLockStrength;
		}
		if (this.door != null)
		{
			if ((this.door.doorInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.door.doorInteractable.controller) || (this.door.handleInteractable != null && InteractionController.Instance.currentLookingAtInteractable == this.door.handleInteractable.controller))
			{
				InteractionController.Instance.DisplayInteractionCursor(InteractionController.Instance.displayingInteraction, true);
			}
			if (this.currentLockStrength <= 0f)
			{
				this.door.SetJammed(false, null, true);
				this.door.SetLocked(false, Player.Instance, true);
				this.door.SetKnowLockedStatus(true);
			}
		}
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0012E400 File Offset: 0x0012C600
	public void SpawnWall(bool prepForCombinedMeshes)
	{
		Toolbox.Instance.DestroyObject(this.spawnedWall);
		Toolbox.Instance.DestroyObject(this.blueprint);
		Toolbox.Instance.DestroyObject(this.spawnedCoving);
		Toolbox.Instance.DestroyObject(this.spawnedSteps);
		this.isShortWall = false;
		this.UpdateSegmentData();
		if (this.preset.optimizeSections && this.optimizationOverride)
		{
			return;
		}
		this.wallPrefabRef = null;
		string seedInput = this.position.ToString() + this.node.nodeCoord.ToString();
		if (this.optimizationAnchor && this.preset.parentWallsLong.Count > 0)
		{
			if (this.parentWall == this)
			{
				if (this.preset.parentWallsLong.Count > 0)
				{
					this.wallPrefabRef = this.preset.parentWallsLong[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.parentWallsLong.Count, seedInput, out seedInput)];
				}
			}
			else if (this.preset.childWallsLong.Count > 0)
			{
				this.wallPrefabRef = this.preset.childWallsLong[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.childWallsLong.Count, seedInput, out seedInput)];
			}
		}
		else if (this.preset.parentWallsShort.Count > 0)
		{
			this.isShortWall = true;
			if (this.parentWall == this)
			{
				if (this.preset.parentWallsShort.Count > 0)
				{
					this.wallPrefabRef = this.preset.parentWallsShort[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.parentWallsShort.Count, seedInput, out seedInput)];
				}
			}
			else if (this.preset.childWallsShort.Count > 0)
			{
				this.wallPrefabRef = this.preset.childWallsShort[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.childWallsShort.Count, seedInput, out seedInput)];
			}
		}
		if (this.wallPrefabRef == null)
		{
			return;
		}
		this.spawnedWall = Toolbox.Instance.SpawnObject(this.wallPrefabRef, this.node.room.transform);
		this.spawnedWall.transform.position = this.position;
		this.spawnedWall.transform.localEulerAngles = this.localEulerAngles;
		if (this.physicalObject != null)
		{
			this.spawnedWall.transform.SetParent(this.physicalObject.transform, true);
		}
		if (!prepForCombinedMeshes || this.preset.materialOverride != null)
		{
			this.SetWallMaterial(this.node.room.defaultWallMaterial, this.node.room.defaultWallKey);
		}
		if ((!prepForCombinedMeshes || this.preset.materialOverride != null) && this.spawnedWall != null)
		{
			MeshRenderer meshRenderer = this.spawnedWall.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = this.spawnedWall.AddComponent<MeshRenderer>();
			}
			bool includeStreetLighting = this.node.room.IsOutside();
			if (this.node.room.preset != null && this.node.room.preset.forceStreetLightLayer)
			{
				includeStreetLighting = this.node.room.preset.forceStreetLightLayer;
			}
			Toolbox.Instance.SetLightLayer(meshRenderer, this.node.building, includeStreetLighting);
		}
		if (this.parentWall == this && this.door != null)
		{
			this.door.SpawnDoor();
		}
		if (this.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance && this.node.room.preset != null && this.node.room.preset.steps != null && this.node.floorHeight < this.otherWall.node.floorHeight)
		{
			this.spawnedSteps = Toolbox.Instance.SpawnObject(this.node.room.preset.steps, this.node.room.transform);
			this.spawnedSteps.transform.position = this.node.position;
			this.spawnedSteps.transform.localEulerAngles = this.localEulerAngles;
		}
		if (this.node.room.allowCoving && this.node.room.gameLocation.designStyle.allowCoving)
		{
			if (this.optimizationAnchor)
			{
				this.spawnedCoving = Toolbox.Instance.SpawnObject(PrefabControls.Instance.covingLong, this.node.room.transform);
			}
			else
			{
				this.spawnedCoving = Toolbox.Instance.SpawnObject(PrefabControls.Instance.covingShort, this.node.room.transform);
			}
			this.spawnedCoving.transform.position = this.position + new Vector3(0f, (float)this.node.floor.defaultCeilingHeight * 0.1f, 0f);
			this.spawnedCoving.transform.localEulerAngles = this.localEulerAngles;
			if (!prepForCombinedMeshes || this.preset.materialOverride != null)
			{
				MeshRenderer meshRenderer2 = this.spawnedCoving.AddComponent<MeshRenderer>();
				Toolbox.Instance.SetLightLayer(meshRenderer2, this.node.building, this.node.room.preset.forceStreetLightLayer);
				MaterialsController.Instance.SetMaterialGroup(this.spawnedCoving, this.node.room.defaultWallMaterial, this.node.room.defaultWallKey, false, meshRenderer2);
			}
		}
		if (SessionData.Instance.isFloorEdit && this.physicalObject != null)
		{
			if (this.isShortWall)
			{
				this.blueprint = Toolbox.Instance.SpawnObject(PrefabControls.Instance.blueprintWallShort, this.physicalObject.transform);
				return;
			}
			this.blueprint = Toolbox.Instance.SpawnObject(PrefabControls.Instance.blueprintWallLong, this.physicalObject.transform);
		}
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x0012EA68 File Offset: 0x0012CC68
	public void RemoveWall()
	{
		if (this.node != null)
		{
			this.node.RemoveWall(this);
		}
		if (this.editorTrigger != null)
		{
			FloorEditController.Instance.wallTriggers.Remove(this.editorTrigger);
		}
		if (this.lightswitchInteractable != null)
		{
			this.lightswitchInteractable.SafeDelete(true);
		}
		if (this.spawnedCorner != null)
		{
			Toolbox.Instance.DestroyObject(this.spawnedCorner);
		}
		if (this.spawnedCornerCoving != null)
		{
			Toolbox.Instance.DestroyObject(this.spawnedCornerCoving);
		}
		if (this.spawnedCoving != null)
		{
			Toolbox.Instance.DestroyObject(this.spawnedCoving);
		}
		if (this.physicalObject != null)
		{
			Toolbox.Instance.DestroyObject(this.physicalObject);
		}
		if (this.door != null)
		{
			if (this.door.spawnedDoor != null)
			{
				Toolbox.Instance.DestroyObject(this.door.spawnedDoor);
			}
			Toolbox.Instance.DestroyObject(this.door.gameObject);
		}
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x0012EB84 File Offset: 0x0012CD84
	private void UpdateSegmentData()
	{
		if (this.wallOffset.x < 0f)
		{
			this.nonOptimizedSegment = this.node.localTileCoord.y;
			return;
		}
		if (this.wallOffset.x > 0f)
		{
			this.nonOptimizedSegment = 2 - this.node.localTileCoord.y;
			return;
		}
		if (this.wallOffset.y < 0f)
		{
			this.nonOptimizedSegment = 2 - this.node.localTileCoord.x;
			return;
		}
		if (this.wallOffset.y > 0f)
		{
			this.nonOptimizedSegment = this.node.localTileCoord.x;
		}
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x0012EC38 File Offset: 0x0012CE38
	public void SpawnCorner(bool prepForCombinedMeshes)
	{
		Toolbox.Instance.DestroyObject(this.spawnedCorner);
		Toolbox.Instance.DestroyObject(this.spawnedCornerCoving);
		Vector3Int vector3Int;
		vector3Int..ctor(Mathf.RoundToInt(this.wallOffset.y * 2f), Mathf.RoundToInt(this.wallOffset.x * 2f), 0);
		if (this.wallOffset.y == 0f)
		{
			vector3Int *= -1;
		}
		Vector3Int vector3Int2;
		vector3Int2..ctor(this.node.nodeCoord.x + vector3Int.x, this.node.nodeCoord.y + vector3Int.y, this.node.nodeCoord.z);
		NewNode newNode = null;
		if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode))
		{
			NewWall newWall = newNode.walls.Find((NewWall item) => item.wallOffset == this.wallOffset);
			if (newWall == null || newWall.preset.isFence)
			{
				vector3Int..ctor(Mathf.RoundToInt(this.wallOffset.x * 2f), Mathf.RoundToInt(this.wallOffset.y * 2f), 0);
				Vector3Int vector3Int3 = vector3Int2 + vector3Int;
				NewNode newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int3, ref newNode2))
				{
					Vector2 wallChk = new Vector2(-this.wallOffset.y, -this.wallOffset.x);
					if (this.wallOffset.y == 0f)
					{
						wallChk *= -1f;
					}
					NewWall newWall2 = newNode2.walls.Find((NewWall item) => item.wallOffset == wallChk);
					if (newWall2 != null)
					{
						new Vector2((float)vector3Int3.x + this.wallOffset.x * -2f, (float)vector3Int3.y + this.wallOffset.y * -2f);
						Vector2 wallChk2 = new Vector2(-this.wallOffset.y, -this.wallOffset.x);
						if (this.wallOffset.y == 0f)
						{
							wallChk2 *= -1f;
						}
						NewWall newWall3 = newNode.walls.Find((NewWall item) => item.wallOffset == wallChk2);
						if (newWall3 == null || newWall3.preset.isFence)
						{
							DoorPairPreset doorPairPreset = this.preset;
							if (this.preset.isFence && !newWall2.preset.isFence)
							{
								doorPairPreset = newWall2.preset;
							}
							NewBuilding building = this.node.building;
							if (this.otherWall != null)
							{
								building = this.otherWall.node.building;
							}
							bool flag = false;
							if (doorPairPreset.quoins.Count > 0 && building != null && building.preset != null && building.preset.enableExteriorQuoins && (this.isExterior || this.node.tile.isEdge) && (newWall2.isExterior || newWall2.node.tile.isEdge))
							{
								flag = true;
							}
							if (flag)
							{
								string text;
								this.cornerPrefabRef = doorPairPreset.quoins[Toolbox.Instance.GetPsuedoRandomNumberContained(0, doorPairPreset.quoins.Count, this.node.nodeCoord.ToString(), out text)];
							}
							else if (doorPairPreset.corners.Count > 0)
							{
								string text;
								this.cornerPrefabRef = doorPairPreset.corners[Toolbox.Instance.GetPsuedoRandomNumberContained(0, doorPairPreset.corners.Count, this.node.nodeCoord.ToString(), out text)];
							}
							if (this.cornerPrefabRef != null)
							{
								this.spawnedCorner = Toolbox.Instance.SpawnObject(this.cornerPrefabRef, this.node.room.transform);
								this.spawnedCorner.transform.localEulerAngles = this.localEulerAngles;
								Vector2 vector = Toolbox.Instance.RotateVector2CW(new Vector2(PathFinder.Instance.tileSize.x / (float)CityControls.Instance.nodeMultiplier * -0.5f, 0f), this.localEulerAngles.y);
								this.spawnedCorner.transform.position = this.position + new Vector3(vector.x, 0f, vector.y);
								bool includeStreetLighting = false;
								if (!prepForCombinedMeshes || this.preset.materialOverride != null)
								{
									MeshRenderer meshRenderer = null;
									if (meshRenderer == null)
									{
										meshRenderer = this.spawnedCorner.GetComponent<MeshRenderer>();
									}
									if (meshRenderer == null)
									{
										meshRenderer = this.spawnedCorner.AddComponent<MeshRenderer>();
									}
									includeStreetLighting = this.node.room.IsOutside();
									if (this.node.room.preset != null && this.node.room.preset.forceStreetLightLayer)
									{
										includeStreetLighting = this.node.room.preset.forceStreetLightLayer;
									}
									Toolbox.Instance.SetLightLayer(meshRenderer, this.node.building, includeStreetLighting);
									this.SetWallMaterial(this.node.room.defaultWallMaterial, this.node.room.defaultWallKey);
								}
								if (this.node.room.allowCoving && this.node.room.gameLocation.designStyle.allowCoving)
								{
									this.spawnedCornerCoving = Toolbox.Instance.SpawnObject(PrefabControls.Instance.covingCorner, this.spawnedCorner.transform);
									this.spawnedCornerCoving.transform.localPosition = new Vector3(-0.1f, (float)this.node.floor.defaultCeilingHeight * 0.1f, 0f);
									if (!prepForCombinedMeshes || this.preset.materialOverride != null)
									{
										MeshRenderer meshRenderer2;
										if (!prepForCombinedMeshes)
										{
											meshRenderer2 = this.spawnedCorner.AddComponent<MeshRenderer>();
										}
										else
										{
											meshRenderer2 = this.spawnedCorner.GetComponent<MeshRenderer>();
										}
										Toolbox.Instance.SetLightLayer(meshRenderer2, this.node.building, includeStreetLighting);
										MaterialsController.Instance.SetMaterialGroup(this.spawnedCornerCoving, this.node.room.defaultWallMaterial, this.node.room.defaultWallKey, false, meshRenderer2);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x0012F2EC File Offset: 0x0012D4EC
	public void SpawnFrontage(bool overrideWithKey = false, Toolbox.MaterialKey keyOverride = null)
	{
		while (this.spawnedFrontage.Count > 0)
		{
			Toolbox.Instance.DestroyObject(this.spawnedFrontage[0]);
			this.spawnedFrontage.RemoveAt(0);
		}
		foreach (NewWall.FrontageSetting frontageSetting in this.frontagePresets)
		{
			GameObject gameObject = Toolbox.Instance.SpawnObject(frontageSetting.preset.gameObject, this.node.room.transform);
			gameObject.transform.position = this.position + frontageSetting.offset;
			gameObject.transform.localEulerAngles = this.localEulerAngles;
			frontageSetting.mainTransform = gameObject.transform;
			Toolbox.MaterialKey key = frontageSetting.matKey;
			if (overrideWithKey && keyOverride != null)
			{
				key = keyOverride;
			}
			foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>(true))
			{
				Material material = MaterialsController.Instance.ApplyMaterialKey(meshRenderer, key);
				meshRenderer.staticShadowCaster = true;
				if (frontageSetting.preset != null && frontageSetting.preset.allowStaticBatching && material != null)
				{
					MeshFilter component = meshRenderer.gameObject.GetComponent<MeshFilter>();
					if (component != null && !component.gameObject.CompareTag("IgnoreStaticBatch") && !component.gameObject.CompareTag("RainWindowGlass"))
					{
						this.node.room.AddForStaticBatching(meshRenderer.gameObject, component.sharedMesh, material);
					}
				}
			}
			bool includeStreetLighting = this.node.room.IsOutside();
			if (this.node.room.preset != null && this.node.room.preset.forceStreetLightLayer)
			{
				includeStreetLighting = this.node.room.preset.forceStreetLightLayer;
			}
			Toolbox.Instance.SetLightLayer(gameObject, this.node.building, includeStreetLighting);
			if (frontageSetting.preset.isRainyWindow && frontageSetting.preset.rainyGlass != null)
			{
				if (Game.Instance.enableRainyWindows && (this.node.room.IsOutside() || this.otherWall.node.room.IsOutside()))
				{
					using (IEnumerator enumerator2 = gameObject.transform.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj = enumerator2.Current;
							Transform transform = (Transform)obj;
							if (transform.tag == "RainWindowGlass")
							{
								MeshRenderer component2 = transform.gameObject.GetComponent<MeshRenderer>();
								if (component2 != null)
								{
									component2.sharedMaterial = frontageSetting.preset.rainyGlass;
								}
							}
						}
						goto IL_35A;
					}
				}
				if (frontageSetting.preset.regularGlass != null)
				{
					foreach (object obj2 in gameObject.transform)
					{
						Transform transform2 = (Transform)obj2;
						if (transform2.tag == "RainWindowGlass")
						{
							MeshRenderer component3 = transform2.gameObject.GetComponent<MeshRenderer>();
							if (component3 != null)
							{
								component3.sharedMaterial = frontageSetting.preset.regularGlass;
							}
						}
					}
				}
			}
			IL_35A:
			this.spawnedFrontage.Add(gameObject);
			using (List<FurniturePreset.IntegratedInteractable>.Enumerator enumerator3 = frontageSetting.preset.integratedInteractables.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					FurniturePreset.IntegratedInteractable integrated = enumerator3.Current;
					if (integrated.pairToController != InteractableController.InteractableID.none)
					{
						InteractablePreset interactablePreset = integrated.preset;
						InteractableController interactableController = Enumerable.ToList<InteractableController>(gameObject.GetComponentsInChildren<InteractableController>(true)).Find((InteractableController item) => item.id == integrated.pairToController);
						Vector3 localPos = Vector3.zero;
						Vector3 zero = Vector3.zero;
						if (interactableController == null)
						{
							Game.Log("Unable for find corresponding controller for integrated interactable on " + this.preset.name, 2);
						}
						localPos = interactableController.transform.localPosition;
						zero = interactableController.transform.localEulerAngles;
						Interactable interactable = InteractableCreator.Instance.CreateTransformInteractable(interactablePreset, interactableController.transform, null, null, localPos, zero, null);
						interactableController.Setup(interactable);
						frontageSetting.createdInteractables.Add(interactable);
					}
				}
			}
		}
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x0012F7EC File Offset: 0x0012D9EC
	public void SetDoorPairPreset(DoorPairPreset newPreset, bool enableUpdate = true, bool newIsDivider = false, bool setPair = true)
	{
		if (this.preset != null)
		{
			if (this.preset.overrideWallNormal && newPreset == InteriorControls.Instance.wallNormal)
			{
				newPreset = this.preset.wallNormalOverrride;
			}
			else if (this.preset.overrideDuctLower && newPreset == InteriorControls.Instance.wallVentLower)
			{
				newPreset = this.preset.ductLowerOverrride;
			}
			else if (this.preset.overrideDuctUpper && newPreset == InteriorControls.Instance.wallVentUpper)
			{
				newPreset = this.preset.ductUpperOverrride;
			}
		}
		if (this.parentWall != null && this.parentWall.node != null && this.parentWall.node.gameLocation.thisAsAddress != null && this.parentWall.node.gameLocation.thisAsAddress.addressPreset != null && this.parentWall.node.room.preset != null && (this.childWall.node.gameLocation.thisAsStreet != null || this.parentWall.node.room.preset.replaceInsideAlso) && (!this.parentWall.node.room.preset.replaceOnlyIfOtherIs || this.parentWall.node.room.preset.onlyReplaceIf.Contains(this.childWall.node.room.roomType)))
		{
			if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.wall)
			{
				if (this.parentWall.node.room.preset.replaceWalls != null)
				{
					newPreset = this.parentWall.node.room.preset.replaceWalls;
				}
			}
			else if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.window || newPreset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
			{
				if (this.parentWall.node.room.preset.replaceWindows != null)
				{
					newPreset = this.parentWall.node.room.preset.replaceWindows;
				}
			}
			else if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.entrance && this.parentWall.node.room.preset.replaceEntrance != null)
			{
				newPreset = this.parentWall.node.room.preset.replaceEntrance;
			}
		}
		if (newPreset.raisedFloorOverride != null)
		{
			if (this.node.room.roomType.overrideFloorHeight)
			{
				if (this.node.room.roomType.floorHeight > 0)
				{
					newPreset = newPreset.raisedFloorOverride;
				}
			}
			else if (this.node.floorHeight > 0)
			{
				newPreset = newPreset.raisedFloorOverride;
			}
		}
		this.preset = newPreset;
		if (setPair)
		{
			if (this.parentWall != null && this.parentWall.preset != newPreset)
			{
				this.parentWall.SetDoorPairPreset(newPreset, true, false, false);
			}
			if (this.childWall != null && this.childWall.preset != newPreset)
			{
				this.childWall.SetDoorPairPreset(newPreset, true, false, false);
			}
		}
		if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
		{
			if (newPreset.canFeatureDoor)
			{
				if (this.door != null)
				{
					if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.id))
					{
						CityData.Instance.doorDictionary.Remove(this.door.wall.id);
					}
					if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.otherWall.id))
					{
						CityData.Instance.doorDictionary.Remove(this.door.wall.otherWall.id);
					}
					Toolbox.Instance.DestroyObject(this.door.gameObject);
				}
				RoomTypePreset roomType = this.node.room.roomType;
				NewRoom room = this.node.room;
				if (this.otherWall.node.room.roomType.doorPriority > roomType.doorPriority)
				{
					room = this.otherWall.node.room;
					roomType = this.otherWall.node.room.roomType;
				}
				else if (this.otherWall.node.room.roomType.doorPriority == roomType.doorPriority && this.otherWall.node.room.roomID > this.node.room.roomID)
				{
					room = this.otherWall.node.room;
					roomType = this.otherWall.node.room.roomType;
				}
				float num = 1f;
				if (roomType.chanceOfNoDoor > 0f)
				{
					num = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, room.roomID.ToString(), false);
				}
				if (this.node.room.gameLocation != this.otherWall.node.room.gameLocation || num >= roomType.chanceOfNoDoor)
				{
					bool flag = true;
					if (this.node.room.gameLocation == this.otherWall.node.room.gameLocation && roomType.chanceOfNoDoor > 0.01f)
					{
						if (this.node.walls.Exists((NewWall item) => item.wallOffset == this.wallOffset * -1f))
						{
							flag = false;
						}
						else if (this.otherWall.node.walls.Exists((NewWall item) => item.wallOffset == this.wallOffset))
						{
							flag = false;
						}
					}
					if (flag && (this.node.gameLocation.floor == null || this.node.gameLocation.floor.floor > CityControls.Instance.lowestFloor))
					{
						GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.door, this.node.room.transform);
						gameObject.transform.position = this.position;
						gameObject.transform.localEulerAngles = this.localEulerAngles;
						float num2 = 0f;
						float num3 = 0f;
						Vector2 rightWallOffset = Toolbox.Instance.RotateVector2CW(this.wallOffset, 90f);
						Vector2 leftWallOffset = -rightWallOffset;
						if (this.node.walls.Exists((NewWall item) => item.wallOffset == rightWallOffset && item.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance))
						{
							num3 += 10f;
						}
						if (this.node.walls.Exists((NewWall item) => item.wallOffset == leftWallOffset && item.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance))
						{
							num2 += 10f;
						}
						if (this.otherWall.node.walls.Exists((NewWall item) => item.wallOffset == rightWallOffset && item.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance))
						{
							num3 += 10f;
						}
						if (this.otherWall.node.walls.Exists((NewWall item) => item.wallOffset == leftWallOffset && item.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance))
						{
							num2 += 10f;
						}
						if (num3 > num2)
						{
							gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
						}
						this.door = gameObject.GetComponent<NewDoor>();
						this.door.Setup(this.parentWall);
						this.otherWall.door = this.door;
					}
				}
				if (this.parentWall.node.room != null && this.childWall.node.room != null)
				{
					this.parentWall.node.room.AddEntrance(this.parentWall.node, this.childWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
				if (this.childWall.node.room != null && this.parentWall.node.room != null)
				{
					this.childWall.node.room.AddEntrance(this.childWall.node, this.parentWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
			}
			else if (this.door != null)
			{
				if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.id))
				{
					CityData.Instance.doorDictionary.Remove(this.door.wall.id);
				}
				if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.otherWall.id))
				{
					CityData.Instance.doorDictionary.Remove(this.door.wall.otherWall.id);
				}
				Toolbox.Instance.DestroyObject(this.door.gameObject);
			}
		}
		else
		{
			if (this.door != null)
			{
				if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.id))
				{
					CityData.Instance.doorDictionary.Remove(this.door.wall.id);
				}
				if (CityData.Instance.doorDictionary.ContainsKey(this.door.wall.otherWall.id))
				{
					CityData.Instance.doorDictionary.Remove(this.door.wall.otherWall.id);
				}
				Toolbox.Instance.DestroyObject(this.door.gameObject);
			}
			if (newPreset.sectionClass == DoorPairPreset.WallSectionClass.window || newPreset.sectionClass == DoorPairPreset.WallSectionClass.windowLarge)
			{
				if (this.parentWall.node.room != null && this.childWall.node.room != null)
				{
					this.parentWall.node.room.AddEntrance(this.parentWall.node, this.childWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
				if (this.childWall.node.room != null && this.parentWall.node.room != null)
				{
					this.childWall.node.room.AddEntrance(this.childWall.node, this.parentWall.node, false, NewNode.NodeAccess.AccessType.adjacent, false);
				}
			}
			else if (this.parentWall.node != null && this.childWall.node != null)
			{
				if (this.parentWall.node.room != null && this.childWall.node.room != null)
				{
					this.parentWall.node.room.RemoveEntrance(this.parentWall.node, this.childWall.node);
				}
				if (this.childWall.node.room != null && this.parentWall.node.room != null)
				{
					this.childWall.node.room.RemoveEntrance(this.childWall.node, this.parentWall.node);
				}
			}
		}
		if (enableUpdate && this.parentWall != null && this.childWall != null)
		{
			try
			{
				GenerationController.Instance.UpdateWallsRoom(this.parentWall.node.room);
				GenerationController.Instance.UpdateWallsRoom(this.childWall.node.room);
			}
			catch
			{
			}
		}
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x001303E8 File Offset: 0x0012E5E8
	public void SelectFrontage()
	{
		if (this.node.room.preset.wallFrontage.Count > 0 && (this.otherWall == null || this.otherWall.frontagePresets.Count <= 0))
		{
			string text = this.node.nodeCoord.ToString();
			List<RoomConfiguration.WallFrontage> list = new List<RoomConfiguration.WallFrontage>();
			if (this.node.room.preset.oneFrontagePerNode)
			{
				if (this.node.walls.Exists((NewWall item) => item.frontagePresets.Count > 0 || item.otherWall.frontagePresets.Count > 0))
				{
					return;
				}
			}
			foreach (RoomConfiguration.WallFrontage wallFrontage in this.node.room.preset.wallFrontage)
			{
				if (wallFrontage.limitToBuildingTypes)
				{
					if (this.node.building != null)
					{
						if (!wallFrontage.limitedToBuildings.Contains(this.node.building.preset))
						{
							continue;
						}
					}
					else if (this.otherWall.node.building != null && !wallFrontage.limitedToBuildings.Contains(this.otherWall.node.building.preset))
					{
						continue;
					}
				}
				if (wallFrontage.wallPreset == this.preset)
				{
					if (!wallFrontage.onlyIfBorderingOutside)
					{
						list.Add(wallFrontage);
					}
					else if (this.node.room.IsOutside() || this.otherWall.node.room.IsOutside())
					{
						list.Add(wallFrontage);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			foreach (RoomConfiguration.WallFrontage wallFrontage2 in list)
			{
				foreach (WallFrontageClass frontageClass in wallFrontage2.insideFrontage)
				{
					WallFrontagePreset chosenPreset = Toolbox.Instance.SelectWallFrontage(this.node.room.gameLocation.designStyle, frontageClass, text);
					Toolbox.MaterialKey matKey = new Toolbox.MaterialKey();
					bool flag = false;
					if (chosenPreset.inheritColouringFromDecor && chosenPreset.variations.Count > 0)
					{
						if (chosenPreset.shareColours != FurniturePreset.ShareColours.none)
						{
							NewWall.FrontageSetting frontageSetting = null;
							Predicate<NewWall.FrontageSetting> <>9__1;
							foreach (NewNode newNode in this.node.room.nodes)
							{
								foreach (NewWall newWall in newNode.walls)
								{
									List<NewWall.FrontageSetting> list2 = newWall.frontagePresets;
									Predicate<NewWall.FrontageSetting> predicate;
									if ((predicate = <>9__1) == null)
									{
										predicate = (<>9__1 = ((NewWall.FrontageSetting item) => item.colors && item.preset.shareColours == chosenPreset.shareColours));
									}
									frontageSetting = list2.Find(predicate);
									if (frontageSetting != null)
									{
										matKey = frontageSetting.matKey;
										flag = true;
										break;
									}
								}
								if (frontageSetting != null)
								{
									break;
								}
							}
						}
						if (!flag)
						{
							matKey = MaterialsController.Instance.GenerateMaterialKey(chosenPreset.variations[Toolbox.Instance.RandContained(0, chosenPreset.variations.Count, text, out text)], this.node.room.colourScheme, this.node.room, true, null);
							flag = true;
						}
					}
					this.frontagePresets.Add(new NewWall.FrontageSetting
					{
						preset = chosenPreset,
						matKey = matKey,
						offset = wallFrontage2.localOffset,
						colors = flag
					});
				}
				if (this.otherWall != null)
				{
					foreach (WallFrontageClass frontageClass2 in wallFrontage2.outsideFrontage)
					{
						WallFrontagePreset chosenPreset = Toolbox.Instance.SelectWallFrontage(this.node.room.gameLocation.designStyle, frontageClass2, text);
						Toolbox.MaterialKey matKey2 = new Toolbox.MaterialKey();
						if (chosenPreset.inheritColouringFromDecor && chosenPreset.variations.Count > 0)
						{
							bool flag2 = false;
							if (chosenPreset.shareColours != FurniturePreset.ShareColours.none)
							{
								NewWall.FrontageSetting frontageSetting2 = null;
								Predicate<NewWall.FrontageSetting> <>9__2;
								Predicate<NewWall.FrontageSetting> <>9__3;
								foreach (NewNode newNode2 in this.node.room.nodes)
								{
									foreach (NewWall newWall2 in newNode2.walls)
									{
										List<NewWall.FrontageSetting> list3 = newWall2.frontagePresets;
										Predicate<NewWall.FrontageSetting> predicate2;
										if ((predicate2 = <>9__2) == null)
										{
											predicate2 = (<>9__2 = ((NewWall.FrontageSetting item) => item.preset.shareColours == chosenPreset.shareColours));
										}
										frontageSetting2 = list3.Find(predicate2);
										if (frontageSetting2 == null)
										{
											List<NewWall.FrontageSetting> list4 = newWall2.otherWall.frontagePresets;
											Predicate<NewWall.FrontageSetting> predicate3;
											if ((predicate3 = <>9__3) == null)
											{
												predicate3 = (<>9__3 = ((NewWall.FrontageSetting item) => item.preset.shareColours == chosenPreset.shareColours));
											}
											frontageSetting2 = list4.Find(predicate3);
										}
										if (frontageSetting2 != null)
										{
											matKey2 = frontageSetting2.matKey;
											flag2 = true;
											break;
										}
									}
									if (frontageSetting2 != null)
									{
										break;
									}
								}
							}
							if (!flag2)
							{
								matKey2 = MaterialsController.Instance.GenerateMaterialKey(chosenPreset.variations[Toolbox.Instance.GetPsuedoRandomNumberContained(0, chosenPreset.variations.Count, text, out text)], this.node.room.colourScheme, this.node.room, true, null);
								flag2 = true;
							}
						}
						this.otherWall.frontagePresets.Add(new NewWall.FrontageSetting
						{
							preset = chosenPreset,
							matKey = matKey2,
							offset = wallFrontage2.localOffset
						});
					}
				}
			}
		}
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x00130AEC File Offset: 0x0012ECEC
	public void SetWallMaterial(MaterialGroupPreset newMat, Toolbox.MaterialKey newKey)
	{
		if (this.preset.materialOverride != null)
		{
			newMat = this.preset.materialOverride;
		}
		if (Enumerable.FirstOrDefault<NewNode>(Enumerable.Where<NewNode>(this.node.room.nodes, (NewNode item) => item.floorType == NewNode.FloorTileType.CeilingOnly || item.floorType == NewNode.FloorTileType.noneButIndoors || item.tile.isStairwell || item.tile.isInvertedStairwell)) != null && newMat != null && newMat.noFloorReplacement != null)
		{
			newMat = newMat.noFloorReplacement;
		}
		MaterialsController.Instance.SetMaterialGroup(this.spawnedWall, newMat, newKey, false, null);
		MaterialsController.Instance.SetMaterialGroup(this.spawnedCorner, newMat, newKey, false, null);
		MaterialsController.Instance.SetMaterialGroup(this.spawnedCoving, newMat, newKey, false, null);
		MaterialsController.Instance.SetMaterialGroup(this.spawnedCornerCoving, newMat, newKey, false, null);
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x00130BC8 File Offset: 0x0012EDC8
	public void SetAsLightswitch(NewRoom newRoom)
	{
		if (!(newRoom != null))
		{
			if (this.containsLightswitch != null)
			{
				this.containsLightswitch.lightswitches.Remove(this);
				this.containsLightswitch.lightswitchInteractables.Remove(this.lightswitchInteractable);
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.containsLightswitch.debugLightswitches.Remove(this.id.ToString());
				}
			}
			if (this.lightswitchInteractable != null)
			{
				this.lightswitchInteractable = null;
			}
			this.containsLightswitch = null;
			return;
		}
		this.containsLightswitch = newRoom;
		if (!newRoom.lightswitches.Contains(this))
		{
			newRoom.lightswitches.Add(this);
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				newRoom.debugLightswitches.Add(this.id.ToString());
			}
		}
		new List<NewNode>().Add(this.node);
		new List<NewWall>().Add(this);
		if (this.lightswitchInteractable != null)
		{
			return;
		}
		bool flag = false;
		Vector2 vector = this.wallOffset * 2f;
		Vector2 vector2;
		vector2..ctor(vector.y, 0f);
		if (vector.x != 0f)
		{
			vector2..ctor(0f, vector.x);
		}
		foreach (NewWall newWall in this.node.walls)
		{
			if (newWall != this && newWall.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
			{
				Vector2 vector3 = newWall.wallOffset * 2f;
				Vector2 vector4;
				vector4..ctor(vector.x + vector3.x, vector.y + vector3.y);
				if (vector.y != 0f)
				{
					vector2.x = vector4.x;
				}
				else
				{
					vector2.y = vector4.y;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
			{
				Vector3Int vector3Int = this.node.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.room == this.node.room)
				{
					NewWall newWall2 = newNode.walls.Find((NewWall item) => item.wallOffset == this.wallOffset);
					if (newWall2 != null && newWall2.preset.sectionClass == DoorPairPreset.WallSectionClass.entrance)
					{
						vector2 = vector2Int;
						flag = true;
						break;
					}
				}
			}
		}
		float num = 1.325f;
		foreach (FurnitureLocation furnitureLocation in this.node.individualFurniture)
		{
			foreach (FurnitureClass furnitureClass in furnitureLocation.furnitureClasses)
			{
				if (furnitureClass.raiseLightswitch)
				{
					num = Mathf.Max(num, furnitureClass.lightswitchYOffset);
				}
			}
		}
		Vector3 localPos = this.node.room.transform.InverseTransformPoint(new Vector3(this.position.x + vector2.x * 0.625f, this.node.position.y + num, this.position.z + vector2.y * 0.625f));
		this.lightswitchInteractable = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.lightswitch, this.node.room.transform, null, null, localPos, this.localEulerAngles, null);
		this.lightswitchInteractable.SetSwitchState(newRoom.mainLightStatus, null, true, false, false);
		this.node.room.lightswitchInteractables.Add(this.lightswitchInteractable);
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x00131008 File Offset: 0x0012F208
	public CitySaveData.WallCitySave GenerateSaveData()
	{
		CitySaveData.WallCitySave wallCitySave = new CitySaveData.WallCitySave();
		wallCitySave.wo = this.wallOffset;
		wallCitySave.id = this.id;
		wallCitySave.p = this.preset.id;
		wallCitySave.ow = this.otherWall.id;
		wallCitySave.pw = this.parentWall.id;
		wallCitySave.cw = this.childWall.id;
		wallCitySave.oo = this.optimizationOverride;
		wallCitySave.oa = this.optimizationAnchor;
		wallCitySave.nos = this.nonOptimizedSegment;
		wallCitySave.isw = this.isShortWall;
		wallCitySave.ds = this.baseDoorStrength;
		wallCitySave.ls = this.baseLockStrength;
		wallCitySave.sw = this.separateWall;
		if (this.foundDoorMaterialKey)
		{
			wallCitySave.dm = true;
			wallCitySave.dmk = this.doorMatKey;
		}
		foreach (NewWall.FrontageSetting frontageSetting in this.frontagePresets)
		{
			wallCitySave.fr.Add(new CitySaveData.WallFrontageSave
			{
				str = frontageSetting.preset.name,
				matKey = frontageSetting.matKey,
				o = frontageSetting.offset
			});
		}
		if (this.containsLightswitch != null)
		{
			wallCitySave.cl = this.containsLightswitch.roomID;
		}
		return wallCitySave;
	}

	// Token: 0x04001961 RID: 6497
	[Header("Transform")]
	public Vector3 position;

	// Token: 0x04001962 RID: 6498
	public Vector3 localEulerAngles;

	// Token: 0x04001963 RID: 6499
	public GameObject physicalObject;

	// Token: 0x04001964 RID: 6500
	[Header("Location")]
	public NewNode node;

	// Token: 0x04001965 RID: 6501
	[Space(5f)]
	public Vector2 wallOffset;

	// Token: 0x04001966 RID: 6502
	public bool isExterior;

	// Token: 0x04001967 RID: 6503
	public bool separateWall;

	// Token: 0x04001968 RID: 6504
	[Header("Details")]
	public int id;

	// Token: 0x04001969 RID: 6505
	public static int assignID = 1;

	// Token: 0x0400196A RID: 6506
	public bool preventEntrance;

	// Token: 0x0400196B RID: 6507
	[Header("Door Config")]
	public bool foundDoorMaterialKey;

	// Token: 0x0400196C RID: 6508
	public Toolbox.MaterialKey doorMatKey;

	// Token: 0x0400196D RID: 6509
	public float baseDoorStrength;

	// Token: 0x0400196E RID: 6510
	public float currentDoorStrength;

	// Token: 0x0400196F RID: 6511
	public float baseLockStrength;

	// Token: 0x04001970 RID: 6512
	public float currentLockStrength;

	// Token: 0x04001971 RID: 6513
	[Header("Wall Pair")]
	public DoorPairPreset preset;

	// Token: 0x04001972 RID: 6514
	public NewWall otherWall;

	// Token: 0x04001973 RID: 6515
	public NewWall parentWall;

	// Token: 0x04001974 RID: 6516
	public NewWall childWall;

	// Token: 0x04001975 RID: 6517
	public List<NewWall.FrontageSetting> frontagePresets = new List<NewWall.FrontageSetting>();

	// Token: 0x04001976 RID: 6518
	[NonSerialized]
	public int otherWallID;

	// Token: 0x04001977 RID: 6519
	[NonSerialized]
	public int parentWallID;

	// Token: 0x04001978 RID: 6520
	[NonSerialized]
	public int childWallID;

	// Token: 0x04001979 RID: 6521
	[Header("Spawned Objects")]
	public bool optimizationOverride;

	// Token: 0x0400197A RID: 6522
	public bool optimizationAnchor;

	// Token: 0x0400197B RID: 6523
	public int nonOptimizedSegment;

	// Token: 0x0400197C RID: 6524
	public GameObject spawnedWall;

	// Token: 0x0400197D RID: 6525
	public GameObject wallPrefabRef;

	// Token: 0x0400197E RID: 6526
	public GameObject spawnedCorner;

	// Token: 0x0400197F RID: 6527
	public GameObject spawnedCoving;

	// Token: 0x04001980 RID: 6528
	public GameObject spawnedCornerCoving;

	// Token: 0x04001981 RID: 6529
	public GameObject cornerPrefabRef;

	// Token: 0x04001982 RID: 6530
	public GameObject spawnedSteps;

	// Token: 0x04001983 RID: 6531
	public GameObject editorTrigger;

	// Token: 0x04001984 RID: 6532
	public bool isShortWall;

	// Token: 0x04001985 RID: 6533
	private GameObject blueprint;

	// Token: 0x04001986 RID: 6534
	[NonSerialized]
	public Interactable lightswitchInteractable;

	// Token: 0x04001987 RID: 6535
	public NewDoor door;

	// Token: 0x04001988 RID: 6536
	public List<GameObject> spawnedFrontage = new List<GameObject>();

	// Token: 0x04001989 RID: 6537
	[Header("Lights")]
	public NewRoom containsLightswitch;

	// Token: 0x0400198A RID: 6538
	[Header("Windows")]
	public int windowUVHorizonalPosition = -1;

	// Token: 0x0400198B RID: 6539
	public BuildingPreset.WindowUVBlock windowUV;

	// Token: 0x0400198C RID: 6540
	[Header("Furniture")]
	public bool placedWallFurn;

	// Token: 0x0200038D RID: 909
	[Serializable]
	public class FrontageSetting
	{
		// Token: 0x0400198D RID: 6541
		public WallFrontagePreset preset;

		// Token: 0x0400198E RID: 6542
		public Toolbox.MaterialKey matKey;

		// Token: 0x0400198F RID: 6543
		public bool colors;

		// Token: 0x04001990 RID: 6544
		public Vector3 offset;

		// Token: 0x04001991 RID: 6545
		[NonSerialized]
		public List<Interactable> createdInteractables = new List<Interactable>();

		// Token: 0x04001992 RID: 6546
		[NonSerialized]
		public Transform mainTransform;
	}
}
