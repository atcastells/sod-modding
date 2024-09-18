using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public class ObjectPoolingController : MonoBehaviour
{
	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060017C6 RID: 6086 RVA: 0x001646A1 File Offset: 0x001628A1
	public static ObjectPoolingController Instance
	{
		get
		{
			return ObjectPoolingController._instance;
		}
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x001646A8 File Offset: 0x001628A8
	private void Awake()
	{
		if (ObjectPoolingController._instance != null && ObjectPoolingController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ObjectPoolingController._instance = this;
		}
		foreach (ObjectPoolingController.ObjectLoadRangeConfig objectLoadRangeConfig in this.loadRangeConfig)
		{
			this.loadRanges.Add((int)objectLoadRangeConfig.range, objectLoadRangeConfig.loadDistance);
		}
		this.UpdateObjectRange = (Action)Delegate.Combine(this.UpdateObjectRange, new Action(this.ExecuteUpdateObjectRanges));
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x0016475C File Offset: 0x0016295C
	private void LateUpdate()
	{
		this.ExecuteInteractableCheckingPool(this.loadPooledObjectPerFrame, this.loadNewObjectPerFrame);
		this.ExecuteFurnitureCheckingPool(this.maximumRangeCheckingPerFrame);
		if (this.interactableLoadList.Count <= 0)
		{
			this.interactablesToLoadCount = this.interactableLoadList.Count;
			if (this.furnitureCheckingPool.Count <= 0)
			{
				base.enabled = false;
			}
		}
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x001647BC File Offset: 0x001629BC
	private void ExecuteInteractableCheckingPool(int maxPooledLoops, int maxNewObjectLoops)
	{
		int num = 0;
		int num2 = 0;
		this.interactablesToLoadCount = this.interactableLoadList.Count;
		while (this.interactableLoadList.Count > 0)
		{
			bool flag = false;
			Interactable interactable = Enumerable.First<Interactable>(this.interactableLoadList);
			if (interactable == null || interactable.preset == null)
			{
				this.interactableLoadList.Remove(interactable);
			}
			else
			{
				interactable.SpawnObject(out flag);
				this.interactableLoadList.Remove(interactable);
				if (flag)
				{
					num2++;
				}
				else
				{
					num++;
				}
				if (num2 >= maxPooledLoops)
				{
					return;
				}
				if (num >= maxNewObjectLoops)
				{
					return;
				}
			}
		}
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x00164848 File Offset: 0x00162A48
	private void ExecuteFurnitureCheckingPool(int maxLoops)
	{
		int num = 0;
		while (this.furnitureCheckingPool.Count > 0)
		{
			FurnitureLocation furnitureLocation = Enumerable.First<FurnitureLocation>(this.furnitureCheckingPool);
			float num2;
			bool enabled = this.SpawnRangeCheck(furnitureLocation, out num2);
			for (int i = 0; i < furnitureLocation.meshes.Count; i++)
			{
				MeshRenderer meshRenderer = furnitureLocation.meshes[i];
				if (meshRenderer == null)
				{
					furnitureLocation.meshes.RemoveAt(i);
				}
				else
				{
					meshRenderer.enabled = enabled;
				}
			}
			this.furnitureCheckingPool.Remove(furnitureLocation);
			num++;
			if (num >= maxLoops)
			{
				return;
			}
		}
		this.furntiureCheckCount = this.furnitureCheckingPool.Count;
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x001648F4 File Offset: 0x00162AF4
	public void MarkAsToLoad(Interactable interactable)
	{
		if (interactable.controller != null && interactable.controller.isVisible)
		{
			return;
		}
		if (interactable.printDebug)
		{
			Game.Log("Interactable " + interactable.name + " Mark to load", 2);
		}
		if (this.useRange && !Game.Instance.freeCam && !interactable.preset.excludeFromVisibilityRangeChecks)
		{
			float num;
			if (interactable.loadedGeometry && interactable.controller != null)
			{
				if (this.SpawnRangeCheck(interactable, out num))
				{
					interactable.controller.SetVisible(true, false);
					return;
				}
				if (!this.interactableRangeToEnableDisableList.Contains(interactable))
				{
					this.interactableRangeToEnableDisableList.Add(interactable);
					return;
				}
			}
			else if (this.SpawnRangeCheck(interactable, out num))
			{
				if (!this.useGradualSpawning || Game.Instance.freeCam)
				{
					bool flag;
					interactable.SpawnObject(out flag);
					return;
				}
				if (!this.interactableLoadList.Contains(interactable))
				{
					this.interactableLoadList.Add(interactable);
					base.enabled = true;
					return;
				}
			}
			else if (!this.interactableRangeToLoadList.Contains(interactable))
			{
				this.interactableRangeToLoadList.Add(interactable);
				return;
			}
		}
		else
		{
			if (interactable.loadedGeometry && interactable.controller != null)
			{
				interactable.controller.SetVisible(true, false);
				return;
			}
			if (this.useGradualSpawning && !Game.Instance.freeCam)
			{
				if (!this.interactableLoadList.Contains(interactable))
				{
					this.interactableLoadList.Add(interactable);
					base.enabled = true;
					return;
				}
			}
			else
			{
				bool flag;
				interactable.SpawnObject(out flag);
			}
		}
	}

	// Token: 0x060017CC RID: 6092 RVA: 0x00164A84 File Offset: 0x00162C84
	public void MarkAsNotNeeded(Interactable interactable)
	{
		if (interactable.printDebug)
		{
			Game.Log("Interactable " + interactable.name + " Mark not needed", 2);
		}
		if (interactable.controller != null && !interactable.preset.excludeFromVisibilityRangeChecks)
		{
			interactable.controller.SetVisible(false, false);
		}
		this.interactableRangeToLoadList.Remove(interactable);
		this.interactableRangeToEnableDisableList.Remove(interactable);
		this.interactableLoadList.Remove(interactable);
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x00164B03 File Offset: 0x00162D03
	public void MarkAsToLoad(FurnitureLocation furniture, bool forceSpawnImmediate = false)
	{
		furniture.SpawnObject(forceSpawnImmediate);
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00164B0C File Offset: 0x00162D0C
	public void MarkAsNotNeeded(FurnitureLocation furniture)
	{
		this.furnitureRangeToLoadList.Remove(furniture);
		this.furnitureRangeToEnableDisableList.Remove(furniture);
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x00164B28 File Offset: 0x00162D28
	public void UpdateObjectRanges()
	{
		Toolbox.Instance.InvokeEndOfFrame(this.UpdateObjectRange, "Update object ranges");
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x00164B3F File Offset: 0x00162D3F
	public void ExecuteUpdateObjectRanges()
	{
		this.ExecuteUpdateObjectRanges(false);
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x00164B48 File Offset: 0x00162D48
	public void ExecuteUpdateObjectRanges(bool forceImmediateSpawning = false)
	{
		foreach (FurnitureLocation furnitureLocation in this.furnitureRangeToEnableDisableList)
		{
			if (!this.furnitureCheckingPool.Contains(furnitureLocation))
			{
				this.furnitureCheckingPool.Add(furnitureLocation);
			}
		}
		if (forceImmediateSpawning)
		{
			Game.Log("Gameplay: Executing immediate update of object/furniture ranges: " + this.furnitureCheckingPool.Count.ToString(), 2);
			this.ExecuteFurnitureCheckingPool(99999);
		}
		if (this.furnitureCheckingPool.Count > 0 && !base.enabled)
		{
			base.enabled = true;
		}
		for (int i = 0; i < this.furnitureRangeToLoadList.Count; i++)
		{
			FurnitureLocation furnitureLocation2 = this.furnitureRangeToLoadList[i];
			float num;
			if (this.SpawnRangeCheck(furnitureLocation2, out num))
			{
				furnitureLocation2.SpawnObject(true);
				this.furnitureRangeToLoadList.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < this.interactableRangeToEnableDisableList.Count; j++)
		{
			Interactable interactable = this.interactableRangeToEnableDisableList[j];
			if (interactable.controller != null)
			{
				if (interactable.preset.excludeFromVisibilityRangeChecks)
				{
					interactable.controller.SetVisible(true, false);
				}
				else
				{
					float num;
					interactable.controller.SetVisible(this.SpawnRangeCheck(interactable, out num), false);
				}
			}
		}
		for (int k = 0; k < this.interactableRangeToLoadList.Count; k++)
		{
			Interactable interactable2 = this.interactableRangeToLoadList[k];
			float num;
			if (this.SpawnRangeCheck(interactable2, out num))
			{
				if (this.useGradualSpawning && !Game.Instance.freeCam && !forceImmediateSpawning)
				{
					if (!this.interactableLoadList.Contains(interactable2))
					{
						this.interactableLoadList.Add(interactable2);
						base.enabled = true;
					}
				}
				else
				{
					bool flag;
					interactable2.SpawnObject(out flag);
				}
				this.interactableRangeToLoadList.RemoveAt(k);
				k--;
			}
		}
		if (forceImmediateSpawning)
		{
			this.ExecuteInteractableCheckingPool(99999, 99999);
		}
		this.interactablesRangeCount = this.interactableRangeToLoadList.Count;
		this.updateObjectRangesTimer = 0f;
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x00164D74 File Offset: 0x00162F74
	public bool SpawnRangeCheck(Interactable interactable, out float distance)
	{
		distance = Vector3.Distance(CameraController.Instance.cam.transform.position, interactable.wPos);
		if (interactable == null || interactable.preset == null)
		{
			return false;
		}
		float num = 999f;
		if (this.loadRanges.ContainsKey((int)interactable.preset.spawnRange))
		{
			num = this.loadRanges[(int)interactable.preset.spawnRange];
		}
		else
		{
			Game.LogError("There was a problem reading range from " + interactable.preset.name + ", check the spawn range setting...", 2);
		}
		return distance <= num;
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x00164E14 File Offset: 0x00163014
	public bool SpawnRangeCheck(FurnitureLocation furniture, out float distance)
	{
		distance = Vector3.Distance(CameraController.Instance.cam.transform.position, furniture.anchorNode.position);
		if (furniture == null || furniture.furniture == null)
		{
			return false;
		}
		float num = 999f;
		if (!SessionData.Instance.isFloorEdit)
		{
			if (this.loadRanges.ContainsKey((int)furniture.furniture.spawnRange))
			{
				num = this.loadRanges[(int)furniture.furniture.spawnRange];
			}
			else
			{
				Game.LogError("There was a problem reading range from " + furniture.furniture.name + ", check the spawn range setting...", 2);
			}
		}
		return distance <= num;
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x00164EC8 File Offset: 0x001630C8
	public GameObject GetInteractableObject(Interactable interactable, out bool wasPooled, out bool isSelf)
	{
		if (interactable.printDebug)
		{
			Game.Log("Interactable " + interactable.name + " Get interactable (spawn/draw from pool)", 2);
		}
		isSelf = false;
		wasPooled = false;
		GameObject gameObject = null;
		HashSet<Interactable> hashSet = null;
		if (this.usePooling && !interactable.preset.excludeFromObjectPooling && this.interactablePool.TryGetValue(interactable.preset, ref hashSet))
		{
			if (hashSet.Contains(interactable))
			{
				gameObject = interactable.spawnedObject;
				this.interactableInstancesSaved++;
				this.RemoveFromPool(interactable);
				isSelf = true;
				wasPooled = true;
			}
			else if (hashSet.Count > 0)
			{
				Interactable interactable2 = Enumerable.First<Interactable>(hashSet);
				gameObject = interactable2.spawnedObject;
				this.interactableInstancesSaved++;
				this.RemoveFromPool(interactable2);
				gameObject.transform.SetParent(interactable.spawnParent);
				wasPooled = true;
			}
		}
		if (gameObject == null)
		{
			try
			{
				gameObject = Toolbox.Instance.SpawnObject(interactable.preset.prefab, interactable.spawnParent);
			}
			catch
			{
				Game.LogError("Unable to spawn object " + interactable.name, 2);
			}
			this.interactablesLoaded++;
		}
		if (this.useRange && !Game.Instance.freeCam && !this.interactableRangeToEnableDisableList.Contains(interactable))
		{
			this.interactableRangeToEnableDisableList.Add(interactable);
		}
		return gameObject;
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x0016502C File Offset: 0x0016322C
	public void RemoveFromPool(Interactable interactable)
	{
		this.interactablePool[interactable.preset].Remove(interactable);
		this.entirePoolReference.Remove(interactable);
		this.interactablesCurrentlyPooled = this.entirePoolReference.Count;
		this.interactableFullPercentage = (float)this.entirePoolReference.Count / (float)this.maximumInteractablePoolCache;
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x0016508C File Offset: 0x0016328C
	public void PoolInteractable(Interactable interactable)
	{
		if (this.interactablesCurrentlyPooled >= this.maximumInteractablePoolCache)
		{
			Interactable interactable2 = Enumerable.First<Interactable>(this.entirePoolReference);
			interactable2.DespawnObject();
			this.RemoveFromPool(interactable2);
		}
		if (!this.interactablePool.ContainsKey(interactable.preset))
		{
			this.interactablePool.Add(interactable.preset, new HashSet<Interactable>());
		}
		this.interactablePool[interactable.preset].Add(interactable);
		this.entirePoolReference.Add(interactable);
		this.interactablesCurrentlyPooled = this.entirePoolReference.Count;
		this.interactableFullPercentage = (float)this.entirePoolReference.Count / (float)this.maximumInteractablePoolCache;
	}

	// Token: 0x04001D40 RID: 7488
	[Header("Settings")]
	[Tooltip("Spawn object gradually instead of all at once")]
	public bool useGradualSpawning = true;

	// Token: 0x04001D41 RID: 7489
	[EnableIf("useGradualSpawning")]
	public int loadNewObjectPerFrame = 1;

	// Token: 0x04001D42 RID: 7490
	[EnableIf("useGradualSpawning")]
	public int loadPooledObjectPerFrame = 5;

	// Token: 0x04001D43 RID: 7491
	[Tooltip("Store uneeded objects in a list ready to be re-used")]
	public bool usePooling = true;

	// Token: 0x04001D44 RID: 7492
	[Tooltip("The maximum number of unrendered but loaded interactables to keep in the object pool.")]
	[EnableIf("usePooling")]
	public int maximumInteractablePoolCache = 1000;

	// Token: 0x04001D45 RID: 7493
	[Tooltip("Use range for loading in objects")]
	public bool useRange = true;

	// Token: 0x04001D46 RID: 7494
	[ReorderableList]
	public List<ObjectPoolingController.ObjectLoadRangeConfig> loadRangeConfig = new List<ObjectPoolingController.ObjectLoadRangeConfig>();

	// Token: 0x04001D47 RID: 7495
	[EnableIf("useRange")]
	public int maximumRangeCheckingPerFrame = 50;

	// Token: 0x04001D48 RID: 7496
	[Tooltip("Maximum number of rooms that are loaded at once before oldest is unloaded")]
	public int maxRoomCache = 100;

	// Token: 0x04001D49 RID: 7497
	[Header("Stats")]
	[ReadOnly]
	public int interactablesLoaded;

	// Token: 0x04001D4A RID: 7498
	[ReadOnly]
	public int interactablesToLoadCount;

	// Token: 0x04001D4B RID: 7499
	[ReadOnly]
	public int interactablesRangeCount;

	// Token: 0x04001D4C RID: 7500
	[NonSerialized]
	public float updateObjectRangesTimer;

	// Token: 0x04001D4D RID: 7501
	[ReadOnly]
	[Space(7f)]
	public int furntiureCheckCount;

	// Token: 0x04001D4E RID: 7502
	[ReadOnly]
	public int furnitureToLoadCount;

	// Token: 0x04001D4F RID: 7503
	[ReadOnly]
	public int furnitureRangeCount;

	// Token: 0x04001D50 RID: 7504
	[ReadOnly]
	[Space(7f)]
	public int interactablesCurrentlyPooled;

	// Token: 0x04001D51 RID: 7505
	[ReadOnly]
	public int interactableInstancesSaved;

	// Token: 0x04001D52 RID: 7506
	[ReadOnly]
	public float interactableFullPercentage;

	// Token: 0x04001D53 RID: 7507
	[ReadOnly]
	[Space(7f)]
	public int roomsLoaded;

	// Token: 0x04001D54 RID: 7508
	public Dictionary<int, float> loadRanges = new Dictionary<int, float>();

	// Token: 0x04001D55 RID: 7509
	public List<Interactable> interactableRangeToLoadList = new List<Interactable>();

	// Token: 0x04001D56 RID: 7510
	[NonSerialized]
	public List<Interactable> interactableRangeToEnableDisableList = new List<Interactable>();

	// Token: 0x04001D57 RID: 7511
	public HashSet<Interactable> interactableLoadList = new HashSet<Interactable>();

	// Token: 0x04001D58 RID: 7512
	private List<FurnitureLocation> furnitureRangeToLoadList = new List<FurnitureLocation>();

	// Token: 0x04001D59 RID: 7513
	[NonSerialized]
	public HashSet<FurnitureLocation> furnitureRangeToEnableDisableList = new HashSet<FurnitureLocation>();

	// Token: 0x04001D5A RID: 7514
	public Dictionary<InteractablePreset, HashSet<Interactable>> interactablePool = new Dictionary<InteractablePreset, HashSet<Interactable>>();

	// Token: 0x04001D5B RID: 7515
	private HashSet<Interactable> entirePoolReference = new HashSet<Interactable>();

	// Token: 0x04001D5C RID: 7516
	private HashSet<FurnitureLocation> furnitureCheckingPool = new HashSet<FurnitureLocation>();

	// Token: 0x04001D5D RID: 7517
	private Action UpdateObjectRange;

	// Token: 0x04001D5E RID: 7518
	private static ObjectPoolingController _instance;

	// Token: 0x02000414 RID: 1044
	public enum ObjectLoadRange
	{
		// Token: 0x04001D60 RID: 7520
		veryClose,
		// Token: 0x04001D61 RID: 7521
		close,
		// Token: 0x04001D62 RID: 7522
		medium,
		// Token: 0x04001D63 RID: 7523
		far,
		// Token: 0x04001D64 RID: 7524
		veryFar,
		// Token: 0x04001D65 RID: 7525
		maximum
	}

	// Token: 0x02000415 RID: 1045
	[Serializable]
	public class ObjectLoadRangeConfig
	{
		// Token: 0x04001D66 RID: 7526
		public ObjectPoolingController.ObjectLoadRange range;

		// Token: 0x04001D67 RID: 7527
		public float loadDistance;
	}
}
