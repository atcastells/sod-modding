using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NaughtyAttributes;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000408 RID: 1032
public class MeshPoolingController : MonoBehaviour
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06001799 RID: 6041 RVA: 0x00162727 File Offset: 0x00160927
	public static MeshPoolingController Instance
	{
		get
		{
			return MeshPoolingController._instance;
		}
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x0016272E File Offset: 0x0016092E
	private void Awake()
	{
		if (MeshPoolingController._instance != null && MeshPoolingController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MeshPoolingController._instance = this;
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0016275C File Offset: 0x0016095C
	public void SpawnMeshesForRoom(NewRoom room)
	{
		bool flag = Game.Instance.combineRoomMeshes;
		if (SessionData.Instance.isFloorEdit)
		{
			flag = false;
		}
		if (!flag)
		{
			List<MeshFilter> list;
			Dictionary<NewBuilding, List<MeshFilter>> dictionary;
			List<MeshFilter> list2;
			List<MeshFilter> list3;
			this.SpawnModularRoomElements(room, flag, out list, out dictionary, out list2, out list3);
		}
		else
		{
			this.GetCombinedRoomMeshes(room, out room.combinedFloor, out room.combinedWalls, out room.additionalWalls, out room.combinedCeiling, out room.combinedFloorRend, out room.combinedWallRend, out room.combinedCeilingRend);
		}
		this.SpawnExtraRoomElements(room);
		room.geometryLoaded = true;
		room.gameObject.SetActive(true);
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x001627E4 File Offset: 0x001609E4
	public void SpawnModularRoomElements(NewRoom room, bool prepForCombineMeshes, out List<MeshFilter> wallChildMeshes, out Dictionary<NewBuilding, List<MeshFilter>> separateWallChildMeshes, out List<MeshFilter> floorChildMeshes, out List<MeshFilter> ceilingChildMeshes)
	{
		wallChildMeshes = null;
		separateWallChildMeshes = null;
		floorChildMeshes = null;
		ceilingChildMeshes = null;
		if (prepForCombineMeshes)
		{
			wallChildMeshes = new List<MeshFilter>();
			separateWallChildMeshes = new Dictionary<NewBuilding, List<MeshFilter>>();
			floorChildMeshes = new List<MeshFilter>();
			ceilingChildMeshes = new List<MeshFilter>();
		}
		foreach (NewNode newNode in room.nodes)
		{
			newNode.SpawnFloor(prepForCombineMeshes);
			newNode.SpawnCeiling(prepForCombineMeshes);
			if (prepForCombineMeshes)
			{
				if (newNode.spawnedFloor != null)
				{
					floorChildMeshes.Add(newNode.spawnedFloor.GetComponent<MeshFilter>());
				}
				if (newNode.spawnedCeiling != null)
				{
					ceilingChildMeshes.Add(newNode.spawnedCeiling.GetComponent<MeshFilter>());
				}
			}
			foreach (NewWall newWall in newNode.walls)
			{
				newWall.SpawnWall(prepForCombineMeshes);
				newWall.SpawnCorner(prepForCombineMeshes);
				if (prepForCombineMeshes && newWall.preset.materialOverride == null)
				{
					if (newWall.spawnedWall != null)
					{
						if (newWall.separateWall && newWall.otherWall != null && newWall.otherWall.node.building != null)
						{
							if (!separateWallChildMeshes.ContainsKey(newWall.otherWall.node.building))
							{
								separateWallChildMeshes.Add(newWall.otherWall.node.building, new List<MeshFilter>());
							}
							separateWallChildMeshes[newWall.otherWall.node.building].Add(newWall.spawnedWall.GetComponent<MeshFilter>());
						}
						else
						{
							wallChildMeshes.Add(newWall.spawnedWall.GetComponent<MeshFilter>());
						}
					}
					if (newWall.spawnedCorner != null)
					{
						if (newWall.separateWall && newWall.otherWall != null && newWall.otherWall.node.building != null)
						{
							if (!separateWallChildMeshes.ContainsKey(newWall.otherWall.node.building))
							{
								separateWallChildMeshes.Add(newWall.otherWall.node.building, new List<MeshFilter>());
							}
							separateWallChildMeshes[newWall.otherWall.node.building].Add(newWall.spawnedCorner.GetComponent<MeshFilter>());
						}
						else
						{
							wallChildMeshes.Add(newWall.spawnedCorner.GetComponent<MeshFilter>());
						}
					}
					if (newWall.spawnedCoving != null)
					{
						if (newWall.separateWall && newWall.otherWall != null && newWall.otherWall.node.building != null)
						{
							if (!separateWallChildMeshes.ContainsKey(newWall.otherWall.node.building))
							{
								separateWallChildMeshes.Add(newWall.otherWall.node.building, new List<MeshFilter>());
							}
							separateWallChildMeshes[newWall.otherWall.node.building].Add(newWall.spawnedCoving.GetComponent<MeshFilter>());
						}
						else
						{
							wallChildMeshes.Add(newWall.spawnedCoving.GetComponent<MeshFilter>());
						}
					}
					if (newWall.spawnedCornerCoving != null && newWall.otherWall != null && newWall.otherWall.node.building != null)
					{
						if (newWall.separateWall)
						{
							if (!separateWallChildMeshes.ContainsKey(newWall.otherWall.node.building))
							{
								separateWallChildMeshes.Add(newWall.otherWall.node.building, new List<MeshFilter>());
							}
							separateWallChildMeshes[newWall.otherWall.node.building].Add(newWall.spawnedCornerCoving.GetComponent<MeshFilter>());
						}
						else
						{
							wallChildMeshes.Add(newWall.spawnedCornerCoving.GetComponent<MeshFilter>());
						}
					}
				}
			}
		}
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x00162BFC File Offset: 0x00160DFC
	public void SpawnExtraRoomElements(NewRoom room)
	{
		foreach (NewNode newNode in room.nodes)
		{
			foreach (NewWall newWall in newNode.walls)
			{
				newWall.SpawnFrontage(false, null);
			}
			if (newNode.tile.isInvertedStairwell && newNode.tile.elevator == null)
			{
				newNode.tile.SetAsStairwell(newNode.tile.isInvertedStairwell, true, true);
			}
			if (newNode.tile.isStairwell && newNode.tile.stairwell == null)
			{
				newNode.tile.SetAsStairwell(newNode.tile.isStairwell, true, false);
			}
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x00162D00 File Offset: 0x00160F00
	public void GetCombinedRoomMeshes(NewRoom room, out GameObject floor, out GameObject walls, out Dictionary<NewBuilding, GameObject> additionalWalls, out GameObject ceiling, out MeshRenderer floorRend, out MeshRenderer wallsRend, out MeshRenderer ceilingRend)
	{
		floor = null;
		floorRend = null;
		walls = null;
		wallsRend = null;
		additionalWalls = new Dictionary<NewBuilding, GameObject>();
		ceiling = null;
		ceilingRend = null;
		MeshPoolingController.RoomMeshCache roomMeshCache = null;
		if (this.roomMeshes.TryGetValue(room, ref roomMeshCache))
		{
			floor = this.CreateGameObjectFromMesh(roomMeshCache.floorMesh, room, room.GetName() + " Floor Mesh", Game.Instance.roomFloorShadowMode, out floorRend);
			this.ProcessFloor(floor, room);
			walls = this.CreateGameObjectFromMesh(roomMeshCache.wallMesh, room, room.GetName() + " Walls Mesh", Game.Instance.roomWallShadowMode, out wallsRend);
			this.ProcessWall(walls, room, null);
			ceiling = this.CreateGameObjectFromMesh(roomMeshCache.ceilingMesh, room, room.GetName() + " Ceiling Mesh", Game.Instance.roomCeilingShadowMode, out ceilingRend);
			this.ProcessCeiling(ceiling, room);
			if (roomMeshCache.additionalWallMesh == null)
			{
				return;
			}
			int num = 1;
			using (Dictionary<NewBuilding, Mesh>.Enumerator enumerator = roomMeshCache.additionalWallMesh.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<NewBuilding, Mesh> keyValuePair = enumerator.Current;
					MeshRenderer meshRenderer;
					GameObject gameObject = this.CreateGameObjectFromMesh(keyValuePair.Value, room, room.GetName() + " Separate Wall Mesh" + num.ToString(), Game.Instance.roomWallShadowMode, out meshRenderer);
					this.ProcessWall(gameObject, room, null);
					if (!additionalWalls.ContainsKey(keyValuePair.Key))
					{
						additionalWalls.Add(keyValuePair.Key, null);
					}
					additionalWalls[keyValuePair.Key] = gameObject;
					num++;
				}
				return;
			}
		}
		Mesh mesh;
		Mesh mesh2;
		Dictionary<NewBuilding, Mesh> dictionary;
		Mesh mesh3;
		this.BuildCombinedMeshesForRoom(room, out mesh, out mesh2, out dictionary, out mesh3, true, out floor, out walls, out additionalWalls, out ceiling, out floorRend, out wallsRend, out ceilingRend);
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x00162EC0 File Offset: 0x001610C0
	public void BuildCombinedMeshesForRoom(NewRoom room, out Mesh floorMesh, out Mesh wallMesh, out Dictionary<NewBuilding, Mesh> additionalWallMeshes, out Mesh ceilingMesh, bool returnGameObjects, out GameObject floor, out GameObject walls, out Dictionary<NewBuilding, GameObject> additionalWalls, out GameObject ceiling, out MeshRenderer floorRend, out MeshRenderer wallsRend, out MeshRenderer ceilingRend)
	{
		floorMesh = null;
		wallMesh = null;
		additionalWallMeshes = null;
		ceilingMesh = null;
		floor = null;
		walls = null;
		additionalWalls = new Dictionary<NewBuilding, GameObject>();
		ceiling = null;
		floorRend = null;
		wallsRend = null;
		ceilingRend = null;
		List<MeshFilter> list = null;
		Dictionary<NewBuilding, List<MeshFilter>> dictionary = null;
		List<MeshFilter> list2 = null;
		List<MeshFilter> list3 = null;
		this.SpawnModularRoomElements(room, true, out list, out dictionary, out list2, out list3);
		if (list.Count > 0 || dictionary.Count > 0)
		{
			if (Game.Instance.useJobSystemForMeshCombination)
			{
				wallMesh = this.CombineMeshesWithMeshAPI(ref list);
			}
			else
			{
				wallMesh = this.CombineMeshes(ref list);
			}
			while (list.Count > 0)
			{
				list[0].gameObject.SetActive(false);
				Object.Destroy(list[0].gameObject);
				list.RemoveAt(0);
			}
			if (returnGameObjects)
			{
				walls = this.CreateGameObjectFromMesh(wallMesh, room, room.GetName() + " Floor Mesh", Game.Instance.roomWallShadowMode, out wallsRend);
				this.ProcessWall(walls, room, null);
			}
		}
		if (dictionary.Count > 0)
		{
			if (additionalWallMeshes == null)
			{
				additionalWallMeshes = new Dictionary<NewBuilding, Mesh>();
			}
			int num = 1;
			foreach (KeyValuePair<NewBuilding, List<MeshFilter>> keyValuePair in dictionary)
			{
				List<MeshFilter> value = keyValuePair.Value;
				Mesh mesh;
				if (Game.Instance.useJobSystemForMeshCombination)
				{
					mesh = this.CombineMeshesWithMeshAPI(ref value);
				}
				else
				{
					mesh = this.CombineMeshes(ref value);
				}
				if (!additionalWallMeshes.ContainsKey(keyValuePair.Key))
				{
					additionalWallMeshes.Add(keyValuePair.Key, null);
				}
				additionalWallMeshes[keyValuePair.Key] = mesh;
				if (returnGameObjects)
				{
					if (additionalWalls == null)
					{
						additionalWalls = new Dictionary<NewBuilding, GameObject>();
					}
					MeshRenderer meshRenderer;
					GameObject gameObject = this.CreateGameObjectFromMesh(mesh, room, room.GetName() + " Separate Wall Mesh" + num.ToString(), Game.Instance.roomWallShadowMode, out meshRenderer);
					this.ProcessWall(gameObject, room, keyValuePair.Key);
					if (!additionalWalls.ContainsKey(keyValuePair.Key))
					{
						additionalWalls.Add(keyValuePair.Key, null);
					}
					additionalWalls[keyValuePair.Key] = gameObject;
				}
				num++;
			}
			List<MeshFilter> list4 = new List<MeshFilter>();
			using (Dictionary<NewBuilding, List<MeshFilter>>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<NewBuilding, List<MeshFilter>> keyValuePair2 = enumerator.Current;
					list4.AddRange(keyValuePair2.Value);
				}
				goto IL_29C;
			}
			IL_25F:
			if (list4[0] != null)
			{
				list4[0].gameObject.SetActive(false);
				Object.Destroy(list4[0].gameObject);
			}
			list4.RemoveAt(0);
			IL_29C:
			if (list4.Count > 0)
			{
				goto IL_25F;
			}
		}
		if (list2.Count > 0)
		{
			if (Game.Instance.useJobSystemForMeshCombination)
			{
				floorMesh = this.CombineMeshesWithMeshAPI(ref list2);
			}
			else
			{
				floorMesh = this.CombineMeshes(ref list2);
			}
			while (list2.Count > 0)
			{
				list2[0].gameObject.SetActive(false);
				Object.Destroy(list2[0].gameObject);
				list2.RemoveAt(0);
			}
			if (returnGameObjects)
			{
				floor = this.CreateGameObjectFromMesh(floorMesh, room, room.GetName() + " Floor Mesh", Game.Instance.roomFloorShadowMode, out floorRend);
				this.ProcessFloor(floor, room);
			}
		}
		if (list3.Count > 0)
		{
			if (Game.Instance.useJobSystemForMeshCombination)
			{
				ceilingMesh = this.CombineMeshesWithMeshAPI(ref list3);
			}
			else
			{
				ceilingMesh = this.CombineMeshes(ref list3);
			}
			while (list3.Count > 0)
			{
				list3[0].gameObject.SetActive(false);
				Object.Destroy(list3[0].gameObject);
				list3.RemoveAt(0);
			}
			if (returnGameObjects)
			{
				ceiling = this.CreateGameObjectFromMesh(ceilingMesh, room, room.GetName() + " Ceiling Mesh", Game.Instance.roomCeilingShadowMode, out ceilingRend);
				this.ProcessCeiling(ceiling, room);
			}
		}
		if (this.roomMeshes.Count < this.maxCache)
		{
			MeshPoolingController.RoomMeshCache roomMeshCache = new MeshPoolingController.RoomMeshCache();
			roomMeshCache.floorMesh = floorMesh;
			roomMeshCache.wallMesh = wallMesh;
			roomMeshCache.additionalWallMesh = additionalWallMeshes;
			roomMeshCache.ceilingMesh = ceilingMesh;
			if (!this.roomMeshes.ContainsKey(room))
			{
				this.roomMeshes.Add(room, roomMeshCache);
				this.generatedRoomMeshes = this.roomMeshes.Keys.Count;
				return;
			}
			this.roomMeshes[room] = roomMeshCache;
		}
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x00163360 File Offset: 0x00161560
	public Mesh CombineMeshes(ref List<MeshFilter> childMeshes)
	{
		CombineInstance[] array = new CombineInstance[childMeshes.Count];
		for (int i = 0; i < childMeshes.Count; i++)
		{
			array[i].mesh = childMeshes[i].sharedMesh;
			array[i].transform = childMeshes[i].transform.localToWorldMatrix;
			Object.Destroy(childMeshes[i].gameObject);
		}
		Mesh mesh = new Mesh();
		mesh.CombineMeshes(array, true, true);
		if (Game.Instance.autoWeldVertices)
		{
			mesh = MeshPoolingController.WeldVertices(mesh);
		}
		if (Game.Instance.optimizeCombinedMeshes)
		{
			mesh.Optimize();
		}
		return mesh;
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x0016340C File Offset: 0x0016160C
	public Mesh CombineMeshesWithMeshAPI(ref List<MeshFilter> meshFilters)
	{
		if (meshFilters.Count < 0)
		{
			return null;
		}
		MeshPoolingController.ProcessMeshDataJob processMeshDataJob = default(MeshPoolingController.ProcessMeshDataJob);
		processMeshDataJob.CreateInputArrays(meshFilters.Count);
		List<Mesh> list = new List<Mesh>(meshFilters.Count);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < meshFilters.Count; i++)
		{
			MeshFilter meshFilter = meshFilters[i];
			GameObject gameObject = meshFilter.gameObject;
			Mesh sharedMesh = meshFilter.sharedMesh;
			list.Add(sharedMesh);
			processMeshDataJob.vertexStart[num3] = num;
			processMeshDataJob.indexStart[num3] = num2;
			processMeshDataJob.xform[num3] = gameObject.transform.localToWorldMatrix;
			num += sharedMesh.vertexCount;
			num2 += (int)sharedMesh.GetIndexCount(0);
			processMeshDataJob.bounds[num3] = new float3x2(new float3(float.PositiveInfinity), new float3(float.NegativeInfinity));
			num3++;
		}
		processMeshDataJob.meshData = Mesh.AcquireReadOnlyMeshData(list);
		Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		processMeshDataJob.outputMesh = meshDataArray[0];
		processMeshDataJob.outputMesh.SetIndexBufferParams(num2, 1);
		processMeshDataJob.outputMesh.SetVertexBufferParams(num, new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(0, 0, 3, 0),
			new VertexAttributeDescriptor(1, 0, 3, 1),
			new VertexAttributeDescriptor(2, 0, 4, 2),
			new VertexAttributeDescriptor(4, 0, 2, 3)
		});
		JobHandle jobHandle = IJobParallelForExtensions.Schedule<MeshPoolingController.ProcessMeshDataJob>(processMeshDataJob, num3, 4, default(JobHandle));
		Mesh mesh = new Mesh();
		mesh.name = "CombinedMesh";
		SubMeshDescriptor subMeshDescriptor;
		subMeshDescriptor..ctor(0, num2, 0);
		subMeshDescriptor.firstVertex = 0;
		subMeshDescriptor.vertexCount = num;
		jobHandle.Complete();
		processMeshDataJob.outputMesh.subMeshCount = 1;
		processMeshDataJob.outputMesh.SetSubMesh(0, subMeshDescriptor, 13);
		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, new Mesh[]
		{
			mesh
		}, 13);
		mesh.RecalculateBounds();
		if (Game.Instance.autoWeldVertices)
		{
			mesh = MeshPoolingController.WeldVertices(mesh);
		}
		if (Game.Instance.optimizeCombinedMeshes)
		{
			mesh.Optimize();
		}
		processMeshDataJob.meshData.Dispose();
		processMeshDataJob.bounds.Dispose();
		for (int j = 0; j < meshFilters.Count; j++)
		{
			Object.Destroy(meshFilters[j].gameObject);
		}
		if (this.bakeMeshesWithJobSystem)
		{
			IJobParallelForExtensions.Schedule<MeshPoolingController.BakeJob>(new MeshPoolingController.BakeJob(mesh.GetInstanceID()), 1, 1, default(JobHandle)).Complete();
		}
		return mesh;
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x001636AA File Offset: 0x001618AA
	public static Mesh WeldVertices(Mesh aMesh)
	{
		return aMesh;
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x001636B0 File Offset: 0x001618B0
	public GameObject CreateGameObjectFromMesh(Mesh mesh, NewRoom room, string newName, ShadowCastingMode shadowMode, out MeshRenderer meshRenderer)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = newName;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshRenderer = gameObject.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.cookingOptions = this.colliderCookingOptions;
		if (meshRenderer != null)
		{
			meshRenderer.shadowCastingMode = shadowMode;
			meshRenderer.staticShadowCaster = true;
		}
		meshCollider.sharedMesh = mesh;
		meshFilter.sharedMesh = mesh;
		gameObject.SetActive(true);
		gameObject.transform.SetParent(room.transform);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.eulerAngles = Vector3.zero;
		gameObject.layer = 29;
		return gameObject;
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x00163758 File Offset: 0x00161958
	public void ProcessWall(GameObject wallObject, NewRoom room, NewBuilding building = null)
	{
		bool flag = false;
		if (room.IsOutside() || room.gameLocation.IsOutside() || room.preset.forceStreetLightLayer || room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
		{
			flag = true;
		}
		if (flag && (building != null || room.building != null))
		{
			if (building != null)
			{
				if (building.extMat != null)
				{
					MaterialsController.Instance.SetMaterialGroup(wallObject, building.extWallMaterial, building.preset.exteriorKey, false, null);
				}
				else
				{
					building.extMat = MaterialsController.Instance.SetMaterialGroup(wallObject, building.extWallMaterial, building.preset.exteriorKey, false, null);
				}
			}
			else if (room.building != null)
			{
				if (room.building.extMat != null)
				{
					MaterialsController.Instance.SetMaterialGroup(wallObject, room.building.extWallMaterial, room.building.preset.exteriorKey, false, null);
				}
				else
				{
					room.building.extMat = MaterialsController.Instance.SetMaterialGroup(wallObject, room.building.extWallMaterial, room.building.preset.exteriorKey, false, null);
				}
			}
		}
		else
		{
			MaterialGroupPreset preset = room.defaultWallMaterial;
			if (Enumerable.FirstOrDefault<NewNode>(Enumerable.Where<NewNode>(room.nodes, (NewNode item) => item.floorType == NewNode.FloorTileType.CeilingOnly || item.floorType == NewNode.FloorTileType.noneButIndoors || item.tile.isStairwell || item.tile.isInvertedStairwell)) != null && room.defaultWallMaterial != null && room.defaultWallMaterial.noFloorReplacement != null)
			{
				preset = room.defaultWallMaterial.noFloorReplacement;
			}
			room.wallMat = MaterialsController.Instance.SetMaterialGroup(wallObject, preset, room.defaultWallKey, false, null);
		}
		MeshRenderer component = wallObject.GetComponent<MeshRenderer>();
		Toolbox.Instance.SetLightLayer(component, room.building, flag);
		component.shadowCastingMode = Game.Instance.roomWallShadowMode;
		component.staticShadowCaster = true;
		wallObject.tag = "WallsMesh";
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x00163960 File Offset: 0x00161B60
	public void ProcessFloor(GameObject floorObject, NewRoom room)
	{
		bool includeStreetLighting = false;
		if (room.IsOutside() || room.gameLocation.IsOutside() || room.preset.forceStreetLightLayer || room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
		{
			includeStreetLighting = true;
		}
		MeshRenderer component = floorObject.GetComponent<MeshRenderer>();
		room.floorMat = MaterialsController.Instance.SetMaterialGroup(room.combinedFloor, room.floorMaterial, room.floorMatKey, false, null);
		Toolbox.Instance.SetLightLayer(component, room.building, includeStreetLighting);
		component.shadowCastingMode = Game.Instance.roomFloorShadowMode;
		floorObject.tag = "FloorMesh";
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x001639FC File Offset: 0x00161BFC
	public void ProcessCeiling(GameObject ceilingObject, NewRoom room)
	{
		bool includeStreetLighting = false;
		if (room.IsOutside() || room.gameLocation.IsOutside() || room.preset.forceStreetLightLayer || room.preset.forceOutside == RoomConfiguration.OutsideSetting.forceOutside)
		{
			includeStreetLighting = true;
		}
		MeshRenderer component = ceilingObject.GetComponent<MeshRenderer>();
		if (room.preset.boostCeilingEmission && room.ceilingMat != null)
		{
			Object.Destroy(room.ceilingMat);
		}
		room.ceilingMat = MaterialsController.Instance.SetMaterialGroup(room.combinedCeiling, room.ceilingMaterial, room.ceilingMatKey, room.preset.boostCeilingEmission, null);
		room.uniqueCeilingMaterial = room.preset.boostCeilingEmission;
		if (room.preset.boostCeilingEmission && room.ceilingMat != null && room.uniqueCeilingMaterial)
		{
			if (room.mainLightStatus)
			{
				Color color = Color.white;
				if (room.lightZones.Count > 0)
				{
					color = room.lightZones[0].areaLightColour;
				}
				room.ceilingMat.SetColor("_EmissiveColor", room.preset.ceilingEmissionBoost * color);
			}
			else
			{
				room.ceilingMat.SetColor("_EmissiveColor", Color.black);
			}
		}
		Toolbox.Instance.SetLightLayer(component, room.building, includeStreetLighting);
		component.shadowCastingMode = Game.Instance.roomCeilingShadowMode;
		ceilingObject.tag = "CeilingMesh";
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x00163B60 File Offset: 0x00161D60
	private void Update()
	{
		if (!this.backgroundCachingEnabled)
		{
			Game.Log("Meshes: Disabling mesh caching process", 2);
			base.enabled = false;
			return;
		}
		this.uncachedRooms = this.toCache.Count;
		if (this.toCache.Count > 0 && this.roomMeshes.Count < this.maxCache && this.IsBackgroundCachingAllowed())
		{
			if (this.frameCounter <= 1)
			{
				if (!this.roomMeshes.ContainsKey(this.toCache[0]))
				{
					Mesh mesh;
					Mesh mesh2;
					Dictionary<NewBuilding, Mesh> dictionary;
					Mesh mesh3;
					GameObject gameObject;
					GameObject gameObject2;
					Dictionary<NewBuilding, GameObject> dictionary2;
					GameObject gameObject3;
					MeshRenderer meshRenderer;
					MeshRenderer meshRenderer2;
					MeshRenderer meshRenderer3;
					this.BuildCombinedMeshesForRoom(this.toCache[0], out mesh, out mesh2, out dictionary, out mesh3, false, out gameObject, out gameObject2, out dictionary2, out gameObject3, out meshRenderer, out meshRenderer2, out meshRenderer3);
					this.toCache.RemoveAt(0);
				}
				else
				{
					this.toCache.RemoveAt(0);
				}
				this.frameCounter = this.cacheRoomPerXFrames;
			}
			else
			{
				this.frameCounter--;
			}
		}
		if (this.toCache.Count <= 0 || this.roomMeshes.Count >= this.maxCache)
		{
			Game.Log("Meshes: Stopping mesh caching process: " + this.toCache.Count.ToString() + " left to cache", 2);
			this.uncachedRooms = 0;
			this.backgroundCachingEnabled = false;
			base.enabled = false;
		}
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x00163CAB File Offset: 0x00161EAB
	private bool IsBackgroundCachingAllowed()
	{
		return SessionData.Instance.startedGame && SessionData.Instance.play && !CutSceneController.Instance.cutSceneActive && !Player.Instance.isOnStreet;
	}

	// Token: 0x060017A9 RID: 6057 RVA: 0x00163CE0 File Offset: 0x00161EE0
	private IEnumerator ThreadedMeshGeneration(MeshPoolingController.LoaderThread loaderReference)
	{
		loaderReference.isDone = false;
		this.threads.Add(loaderReference);
		Game.Log("Meshes: Mesh creator threads: " + this.threads.Count.ToString(), 2);
		Thread thread = new Thread(delegate()
		{
			Mesh mesh;
			Mesh mesh2;
			Dictionary<NewBuilding, Mesh> dictionary;
			Mesh mesh3;
			GameObject gameObject;
			GameObject gameObject2;
			Dictionary<NewBuilding, GameObject> dictionary2;
			GameObject gameObject3;
			MeshRenderer meshRenderer;
			MeshRenderer meshRenderer2;
			MeshRenderer meshRenderer3;
			this.BuildCombinedMeshesForRoom(loaderReference.location, out mesh, out mesh2, out dictionary, out mesh3, false, out gameObject, out gameObject2, out dictionary2, out gameObject3, out meshRenderer, out meshRenderer2, out meshRenderer3);
			loaderReference.isDone = true;
		});
		thread.Start();
		while (!loaderReference.isDone || thread.IsAlive)
		{
			yield return null;
		}
		this.threads.Remove(loaderReference);
		yield break;
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x00163CF8 File Offset: 0x00161EF8
	[Button(null, 0)]
	public void StartCachingProcess()
	{
		Game.Log("Meshes: Starting mesh caching process...", 2);
		this.backgroundCachingEnabled = true;
		this.toCache = new List<NewRoom>();
		foreach (NewRoom newRoom in CityData.Instance.roomDirectory)
		{
			if (!this.roomMeshes.ContainsKey(newRoom) && newRoom.nodes.Count >= 2)
			{
				this.toCache.Add(newRoom);
			}
		}
		this.toCache.Sort((NewRoom p2, NewRoom p1) => p1.nodes.Count.CompareTo(p2.nodes.Count));
		base.enabled = this.backgroundCachingEnabled;
	}

	// Token: 0x04001CFB RID: 7419
	[Header("State")]
	[ReadOnly]
	public int generatedRoomMeshes;

	// Token: 0x04001CFC RID: 7420
	[Header("Settings")]
	public MeshColliderCookingOptions colliderCookingOptions;

	// Token: 0x04001CFD RID: 7421
	public bool bakeMeshesWithJobSystem = true;

	// Token: 0x04001CFE RID: 7422
	[ReadOnly]
	[Header("Background Caching")]
	public bool backgroundCachingEnabled;

	// Token: 0x04001CFF RID: 7423
	public int cacheRoomPerXFrames = 2;

	// Token: 0x04001D00 RID: 7424
	private int frameCounter;

	// Token: 0x04001D01 RID: 7425
	[InfoBox("How many room meshes this will cache; possibly make this a game settings option?", 0)]
	public int maxCache = 2000;

	// Token: 0x04001D02 RID: 7426
	[ReadOnly]
	public int uncachedRooms;

	// Token: 0x04001D03 RID: 7427
	private List<NewRoom> toCache = new List<NewRoom>();

	// Token: 0x04001D04 RID: 7428
	[NonSerialized]
	public List<MeshPoolingController.LoaderThread> threads = new List<MeshPoolingController.LoaderThread>();

	// Token: 0x04001D05 RID: 7429
	public Dictionary<NewRoom, MeshPoolingController.RoomMeshCache> roomMeshes = new Dictionary<NewRoom, MeshPoolingController.RoomMeshCache>();

	// Token: 0x04001D06 RID: 7430
	private static MeshPoolingController _instance;

	// Token: 0x02000409 RID: 1033
	[Serializable]
	public class RoomMeshCache
	{
		// Token: 0x04001D07 RID: 7431
		public Mesh floorMesh;

		// Token: 0x04001D08 RID: 7432
		public Mesh wallMesh;

		// Token: 0x04001D09 RID: 7433
		public Dictionary<NewBuilding, Mesh> additionalWallMesh;

		// Token: 0x04001D0A RID: 7434
		public Mesh ceilingMesh;
	}

	// Token: 0x0200040A RID: 1034
	public class LoaderThread
	{
		// Token: 0x04001D0B RID: 7435
		public Coroutine thread;

		// Token: 0x04001D0C RID: 7436
		public NewRoom location;

		// Token: 0x04001D0D RID: 7437
		public bool isDone;
	}

	// Token: 0x0200040B RID: 1035
	[BurstCompile]
	private struct ProcessMeshDataJob : IJobParallelFor
	{
		// Token: 0x060017AE RID: 6062 RVA: 0x00163E11 File Offset: 0x00162011
		public void CreateInputArrays(int meshCount)
		{
			this.vertexStart = new NativeArray<int>(meshCount, 3, 0);
			this.indexStart = new NativeArray<int>(meshCount, 3, 0);
			this.xform = new NativeArray<float4x4>(meshCount, 3, 0);
			this.bounds = new NativeArray<float3x2>(meshCount, 3, 0);
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00163E4C File Offset: 0x0016204C
		public void Execute(int index)
		{
			Mesh.MeshData meshData = this.meshData[index];
			int vertexCount = meshData.vertexCount;
			float4x4 float4x = this.xform[index];
			int num = this.vertexStart[index];
			if (!this.tempVertices.IsCreated || this.tempVertices.Length < vertexCount)
			{
				if (this.tempVertices.IsCreated)
				{
					this.tempVertices.Dispose();
				}
				this.tempVertices = new NativeArray<float3>(vertexCount, 2, 0);
			}
			if (!this.tempNormals.IsCreated || this.tempNormals.Length < vertexCount)
			{
				if (this.tempNormals.IsCreated)
				{
					this.tempNormals.Dispose();
				}
				this.tempNormals = new NativeArray<float3>(vertexCount, 2, 0);
			}
			if (!this.tempTangents.IsCreated || this.tempTangents.Length < vertexCount)
			{
				if (this.tempTangents.IsCreated)
				{
					this.tempTangents.Dispose();
				}
				this.tempTangents = new NativeArray<float4>(vertexCount, 2, 0);
			}
			if (!this.tempUVs.IsCreated || this.tempUVs.Length < vertexCount)
			{
				if (this.tempUVs.IsCreated)
				{
					this.tempUVs.Dispose();
				}
				this.tempUVs = new NativeArray<float2>(vertexCount, 2, 0);
			}
			meshData.GetVertices(this.tempVertices.Reinterpret<Vector3>());
			meshData.GetNormals(this.tempNormals.Reinterpret<Vector3>());
			meshData.GetTangents(this.tempTangents.Reinterpret<Vector4>());
			meshData.GetUVs(0, this.tempUVs.Reinterpret<Vector2>());
			NativeArray<Vector3> vertexData = this.outputMesh.GetVertexData<Vector3>(0);
			NativeArray<Vector3> vertexData2 = this.outputMesh.GetVertexData<Vector3>(1);
			NativeArray<Vector4> vertexData3 = this.outputMesh.GetVertexData<Vector4>(2);
			NativeArray<Vector2> vertexData4 = this.outputMesh.GetVertexData<Vector2>(3);
			for (int i = 0; i < vertexCount; i++)
			{
				float3 @float = this.tempVertices[i];
				@float = math.mul(float4x, new float4(@float, 1f)).xyz;
				vertexData[i + num] = @float;
				float3 float2 = this.tempNormals[i];
				float2 = math.normalize(math.mul(float4x, new float4(float2, 0f)).xyz);
				vertexData2[i + num] = float2;
				float4 float3 = this.tempTangents[i];
				float3 = math.mul(float4x, float3).xyzw;
				vertexData3[i + num] = float3;
				float2 float4 = this.tempUVs[i];
				vertexData4[i + num] = float4;
			}
			int num2 = this.indexStart[index];
			int indexCount = meshData.GetSubMesh(0).indexCount;
			NativeArray<int> indexData = this.outputMesh.GetIndexData<int>();
			if (meshData.indexFormat == null)
			{
				NativeArray<ushort> indexData2 = meshData.GetIndexData<ushort>();
				for (int j = 0; j < indexCount; j++)
				{
					indexData[j + num2] = num + (int)indexData2[j];
				}
				return;
			}
			NativeArray<int> indexData3 = meshData.GetIndexData<int>();
			for (int k = 0; k < indexCount; k++)
			{
				indexData[k + num2] = num + indexData3[k];
			}
		}

		// Token: 0x04001D0E RID: 7438
		[ReadOnly]
		public Mesh.MeshDataArray meshData;

		// Token: 0x04001D0F RID: 7439
		public Mesh.MeshData outputMesh;

		// Token: 0x04001D10 RID: 7440
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> vertexStart;

		// Token: 0x04001D11 RID: 7441
		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> indexStart;

		// Token: 0x04001D12 RID: 7442
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<float4x4> xform;

		// Token: 0x04001D13 RID: 7443
		public NativeArray<float3x2> bounds;

		// Token: 0x04001D14 RID: 7444
		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float3> tempVertices;

		// Token: 0x04001D15 RID: 7445
		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float3> tempNormals;

		// Token: 0x04001D16 RID: 7446
		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float4> tempTangents;

		// Token: 0x04001D17 RID: 7447
		[NativeDisableContainerSafetyRestriction]
		private NativeArray<float2> tempUVs;
	}

	// Token: 0x0200040C RID: 1036
	[BurstCompile]
	public struct BakeJob : IJobParallelFor
	{
		// Token: 0x060017B0 RID: 6064 RVA: 0x00164193 File Offset: 0x00162393
		public BakeJob(int mId)
		{
			this.meshId = mId;
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x0016419C File Offset: 0x0016239C
		public void Execute(int index)
		{
			Physics.BakeMesh(this.meshId, false);
		}

		// Token: 0x04001D18 RID: 7448
		private int meshId;
	}
}
