using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class WalkableRecorder : MonoBehaviour
{
	// Token: 0x06000BE3 RID: 3043 RVA: 0x000AB4E8 File Offset: 0x000A96E8
	[Button("Load Furniture Object", 0)]
	public void LoadFurniture()
	{
		this.LoadClass();
		if (this.furnitureParent != null)
		{
			Object.DestroyImmediate(this.furnitureParent.gameObject);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.furniture.prefab, this.furnitureAnchor);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		this.furnitureParent = gameObject.transform;
		this.LoadWalkable();
		this.LoadBlockedArea();
		this.LoadSubObjectSetup();
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x000AB570 File Offset: 0x000A9770
	[Button("Load Class from Furniture Preset", 0)]
	public void LoadClass()
	{
		if (this.furniture != null)
		{
			if (this.furnitureParent != null)
			{
				Object.DestroyImmediate(this.furnitureParent.gameObject);
			}
			this.furnitureClass = this.furniture.classes[0];
			this.ClearWalkable();
			this.ClearBlockedDisplay();
			this.LoadWalkable();
			this.LoadBlockedArea();
		}
	}

	// Token: 0x06000BE5 RID: 3045 RVA: 0x000AB5D8 File Offset: 0x000A97D8
	[Button("Load Walkable Nodespace Area", 0)]
	public void LoadWalkable()
	{
		this.ClearWalkable();
		using (List<FurnitureClass.FurniureWalkSubLocations>.Enumerator enumerator = this.furnitureClass.sublocations.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FurnitureClass.FurniureWalkSubLocations loc = enumerator.Current;
				WalkableRecorder.TileSetup tileSetup = this.tiles.Find((WalkableRecorder.TileSetup item) => item.offset == loc.offset);
				if (tileSetup != null)
				{
					foreach (Vector3 localPosition in loc.sublocations)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(this.walkableObject, tileSetup.trans);
						gameObject.transform.localPosition = localPosition;
						this.walkableDisplay.Add(gameObject.transform);
						gameObject.transform.SetParent(this.walkableNodeParent);
					}
				}
			}
		}
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x000AB6E4 File Offset: 0x000A98E4
	[Button("Automatic Walkable Nodespace Area Generation", 0)]
	public void AutomaticWalkable()
	{
		this.ClearWalkable();
		Vector3[] array = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0.594f, 0f, 0.594f),
			new Vector3(-0.594f, 0f, 0.594f),
			new Vector3(0.594f, 0f, -0.594f),
			new Vector3(-0.594f, 0f, -0.594f),
			new Vector3(-0.594f, 0f, 0f),
			new Vector3(0f, 0f, -0.594f),
			new Vector3(0.594f, 0f, 0f),
			new Vector3(0f, 0f, 0.594f)
		};
		int num = 0;
		while ((float)num < this.furnitureClass.objectSize.x)
		{
			int num2 = 0;
			while ((float)num2 < this.furnitureClass.objectSize.y)
			{
				Vector2 offset = new Vector2((float)num, (float)num2);
				WalkableRecorder.TileSetup tileSetup = this.tiles.Find((WalkableRecorder.TileSetup item) => item.offset == offset);
				if (tileSetup != null)
				{
					foreach (Vector3 vector in array)
					{
						RaycastHit raycastHit;
						if (!Physics.Raycast(new Ray(tileSetup.trans.position + vector + new Vector3(0f, -0.01f, 0f), Vector3.up), ref raycastHit, 5.4f))
						{
							GameObject gameObject = Object.Instantiate<GameObject>(this.walkableObject, tileSetup.trans);
							gameObject.transform.localPosition = vector;
							this.walkableDisplay.Add(gameObject.transform);
							gameObject.transform.SetParent(this.walkableNodeParent);
						}
					}
				}
				num2++;
			}
			num++;
		}
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x000AB91C File Offset: 0x000A9B1C
	[Button("Save Walkable Nodespace Area", 0)]
	public void RecordWalkable()
	{
		new List<Transform>();
		this.furnitureClass.sublocations.Clear();
		for (int i = 0; i < this.walkableDisplay.Count; i++)
		{
			WalkableRecorder.<>c__DisplayClass20_0 CS$<>8__locals1 = new WalkableRecorder.<>c__DisplayClass20_0();
			Transform transform = this.walkableDisplay[i];
			if (!(transform == null))
			{
				CS$<>8__locals1.offsetThis = Vector2.zero;
				Vector3 localPosition = transform.localPosition;
				while (localPosition.x < -0.9f)
				{
					localPosition.x += 1.8f;
					WalkableRecorder.<>c__DisplayClass20_0 CS$<>8__locals2 = CS$<>8__locals1;
					CS$<>8__locals2.offsetThis.x = CS$<>8__locals2.offsetThis.x + 1f;
				}
				while (localPosition.z < -0.9f)
				{
					localPosition.z += 1.8f;
					WalkableRecorder.<>c__DisplayClass20_0 CS$<>8__locals3 = CS$<>8__locals1;
					CS$<>8__locals3.offsetThis.y = CS$<>8__locals3.offsetThis.y + 1f;
				}
				string text = "point found at tile ";
				Vector2 offsetThis = CS$<>8__locals1.offsetThis;
				Debug.Log(text + offsetThis.ToString());
				if (!this.furnitureClass.sublocations.Exists((FurnitureClass.FurniureWalkSubLocations item) => item.offset == CS$<>8__locals1.offsetThis))
				{
					this.furnitureClass.sublocations.Add(new FurnitureClass.FurniureWalkSubLocations
					{
						offset = CS$<>8__locals1.offsetThis
					});
				}
				FurnitureClass.FurniureWalkSubLocations furniureWalkSubLocations = this.furnitureClass.sublocations.Find((FurnitureClass.FurniureWalkSubLocations item) => item.offset == CS$<>8__locals1.offsetThis);
				Vector3 vector = this.tiles.Find((WalkableRecorder.TileSetup item) => item.offset == CS$<>8__locals1.offsetThis).trans.InverseTransformPoint(transform.position);
				furniureWalkSubLocations.sublocations.Add(new Vector3(this.RoundToPlaces(vector.x, 3), this.RoundToPlaces(vector.y, 3), this.RoundToPlaces(vector.z, 3)));
				this.furnitureClass.blockDefaultSublocations = true;
			}
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x000ABAE0 File Offset: 0x000A9CE0
	[Button("Clear Walkable Nodepace Display", 0)]
	public void ClearWalkable()
	{
		while (this.walkableDisplay.Count > 0)
		{
			if (this.walkableDisplay[0] != null)
			{
				Object.DestroyImmediate(this.walkableDisplay[0].gameObject);
			}
			this.walkableDisplay.RemoveAt(0);
		}
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x000ABB34 File Offset: 0x000A9D34
	public float RoundToPlaces(float input, int decimals)
	{
		int num = 100;
		if (decimals == 1)
		{
			num = 10;
		}
		else if (decimals == 2)
		{
			num = 100;
		}
		else if (decimals == 3)
		{
			num = 1000;
		}
		else if (decimals == 4)
		{
			num = 10000;
		}
		return (float)Mathf.RoundToInt(input * (float)num) / (float)num;
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x000ABB7C File Offset: 0x000A9D7C
	[Button("Load Sub Object Placement Configuration", 0)]
	public void LoadSubObjectSetup()
	{
		foreach (FurniturePreset.SubObject subObject in this.furniture.subObjects)
		{
			if (subObject != null)
			{
				Transform transform = this.furnitureParent;
				if (subObject.parent != null && subObject.parent.Length > 0)
				{
					transform = this.SearchForTransform(this.furnitureParent, subObject.parent);
				}
				if (transform == null)
				{
					transform = this.furnitureParent;
					Debug.Log("Unable to find parent: " + subObject.parent);
				}
				if (!(transform == null))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.subObjectPrefab, transform);
					gameObject.transform.localPosition = subObject.localPos;
					gameObject.transform.localEulerAngles = subObject.localRot;
					SubObjectPlacement component = gameObject.GetComponent<SubObjectPlacement>();
					component.belongsTo = subObject.belongsTo;
					component.preset = subObject.preset;
					component.security = subObject.security;
					this.subObjectDisplay.Add(component);
					component.OnClassChanged();
				}
			}
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x000ABCA4 File Offset: 0x000A9EA4
	[Button("Spawn Random Sub Object Examples", 0)]
	public void SpawnRandomSubObjects()
	{
		this.subObjectDisplay.Clear();
		this.subObjectDisplay = Enumerable.ToList<SubObjectPlacement>(this.furnitureParent.GetComponentsInChildren<SubObjectPlacement>());
		foreach (SubObjectPlacement subObjectPlacement in this.subObjectDisplay)
		{
			subObjectPlacement.SpawnRandomObject();
		}
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x000ABD18 File Offset: 0x000A9F18
	[Button("Clear Random Sub Object Examples", 0)]
	public void ClearRandomSubObjects()
	{
		this.subObjectDisplay.Clear();
		this.subObjectDisplay = Enumerable.ToList<SubObjectPlacement>(this.furnitureParent.GetComponentsInChildren<SubObjectPlacement>());
		foreach (SubObjectPlacement subObjectPlacement in this.subObjectDisplay)
		{
			subObjectPlacement.RemoveRandomObject();
		}
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x000ABD8C File Offset: 0x000A9F8C
	[Button("Save Sub Object Placement Configuration", 0)]
	public void RecordSubObjectPlacements()
	{
		new List<FurniturePreset.SubObject>();
		this.furniture.subObjects = new List<FurniturePreset.SubObject>();
		for (int i = 0; i < this.furnitureParent.childCount; i++)
		{
			Transform child = this.furnitureParent.GetChild(i);
			SubObjectPlacement component = child.GetComponent<SubObjectPlacement>();
			if (component != null)
			{
				FurniturePreset.SubObject subObject = new FurniturePreset.SubObject();
				subObject.preset = component.preset;
				subObject.localPos = component.transform.localPosition;
				subObject.localRot = component.transform.localEulerAngles;
				subObject.belongsTo = component.belongsTo;
				subObject.security = component.security;
				this.furniture.subObjects.Add(subObject);
			}
			for (int j = 0; j < child.childCount; j++)
			{
				Transform child2 = child.GetChild(j);
				SubObjectPlacement component2 = child2.GetComponent<SubObjectPlacement>();
				if (component2 != null)
				{
					FurniturePreset.SubObject subObject2 = new FurniturePreset.SubObject();
					subObject2.preset = component2.preset;
					subObject2.localPos = component2.transform.localPosition;
					subObject2.localRot = component2.transform.localEulerAngles;
					subObject2.parent = child2.transform.parent.name;
					subObject2.belongsTo = component2.belongsTo;
					subObject2.security = component2.security;
					this.furniture.subObjects.Add(subObject2);
				}
				for (int k = 0; k < child2.childCount; k++)
				{
					Transform child3 = child2.GetChild(k);
					SubObjectPlacement component3 = child3.GetComponent<SubObjectPlacement>();
					if (component3 != null)
					{
						FurniturePreset.SubObject subObject3 = new FurniturePreset.SubObject();
						subObject3.preset = component3.preset;
						subObject3.localPos = component3.transform.localPosition;
						subObject3.localRot = component3.transform.localEulerAngles;
						subObject3.parent = child3.transform.parent.name;
						subObject3.belongsTo = component3.belongsTo;
						subObject3.security = component3.security;
						this.furniture.subObjects.Add(subObject3);
					}
				}
			}
		}
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x000ABFC0 File Offset: 0x000AA1C0
	[Button("Clear Sub Object Placement Display", 0)]
	public void ClearSubObjectDisplay()
	{
		while (this.subObjectDisplay.Count > 0)
		{
			if (this.subObjectDisplay[0] != null)
			{
				Object.DestroyImmediate(this.subObjectDisplay[0].gameObject);
			}
			this.subObjectDisplay.RemoveAt(0);
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000AC014 File Offset: 0x000AA214
	public Transform SearchForTransform(Transform parent, string search)
	{
		Transform transform = null;
		foreach (object obj in parent)
		{
			Transform transform2 = (Transform)obj;
			if (transform2.name == search)
			{
				transform = transform2;
				return transform;
			}
			if (transform2.childCount > 0)
			{
				transform = this.SearchForTransform(transform2, search);
				if (transform)
				{
					return transform;
				}
			}
		}
		return transform;
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x000AC09C File Offset: 0x000AA29C
	[Button("Load Blocked Area", 0)]
	public void LoadBlockedArea()
	{
		this.ClearBlockedDisplay();
		using (List<WalkableRecorder.TileSetup>.Enumerator enumerator = this.tiles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				WalkableRecorder.TileSetup t = enumerator.Current;
				for (int i = 0; i < this.offsetArrayX8.Length; i++)
				{
					Vector2 offset = this.offsetArrayX8[i];
					CityData.BlockingDirection blockingDirection = i + CityData.BlockingDirection.behindLeft;
					if (!this.blockedDisplay.Exists((DebugBlockingSelector item) => item.tile.offset == t.offset - offset && item.offset == -offset))
					{
						GameObject gameObject = GameObject.CreatePrimitive(3);
						gameObject.transform.SetParent(t.trans);
						gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 1.8f);
						MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
						component.shadowCastingMode = 0;
						component.sharedMaterial = this.nonBlockedMaterial;
						gameObject.transform.localPosition = new Vector3(offset.x * 0.9f, 0f, offset.y * 0.9f);
						Vector3 localPosition = gameObject.transform.localPosition;
						gameObject.transform.rotation = Quaternion.LookRotation(localPosition, Vector3.up);
						gameObject.transform.SetParent(this.blockingParent, true);
						Vector2 nodeOffset = t.offset * -1f;
						Object @object = gameObject;
						string[] array = new string[6];
						array[0] = "Tile ";
						int num = 1;
						Vector2 vector = nodeOffset;
						array[num] = vector.ToString();
						array[2] = " ";
						array[3] = blockingDirection.ToString();
						array[4] = " ";
						int num2 = 5;
						vector = offset;
						array[num2] = vector.ToString();
						@object.name = string.Concat(array);
						DebugBlockingSelector debugBlockingSelector = gameObject.AddComponent<DebugBlockingSelector>();
						debugBlockingSelector.Setup(t, blockingDirection, this, offset);
						this.blockedDisplay.Add(debugBlockingSelector);
						FurnitureClass.BlockedAccess blockedAccess = this.furnitureClass.blockedAccess.Find((FurnitureClass.BlockedAccess item) => item.nodeOffset == nodeOffset);
						if (blockedAccess != null && blockedAccess.blocked.Contains(blockingDirection))
						{
							debugBlockingSelector.SetB();
						}
					}
				}
			}
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x000AC328 File Offset: 0x000AA528
	[Button("Save Blocked Area", 0)]
	public void SaveBlockedArea()
	{
		this.furnitureClass.blockedAccess.Clear();
		int num = 0;
		foreach (DebugBlockingSelector debugBlockingSelector in this.blockedDisplay)
		{
			if (debugBlockingSelector.blocked)
			{
				Vector2 nodeOffset = debugBlockingSelector.tile.offset * -1f;
				FurnitureClass.BlockedAccess blockedAccess = this.furnitureClass.blockedAccess.Find((FurnitureClass.BlockedAccess item) => item.nodeOffset == nodeOffset);
				if (blockedAccess == null)
				{
					blockedAccess = new FurnitureClass.BlockedAccess
					{
						nodeOffset = nodeOffset
					};
					this.furnitureClass.blockedAccess.Add(blockedAccess);
				}
				if (!blockedAccess.blocked.Contains(debugBlockingSelector.dir))
				{
					blockedAccess.blocked.Add(debugBlockingSelector.dir);
				}
				num++;
			}
		}
		if (num <= 0 && !this.furnitureClass.noPassThrough)
		{
			this.furnitureClass.noBlocking = true;
		}
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x000AC448 File Offset: 0x000AA648
	[Button("Clear Blocked Display", 0)]
	public void ClearBlockedDisplay()
	{
		while (this.blockedDisplay.Count > 0)
		{
			if (this.blockedDisplay[0] != null)
			{
				Object.DestroyImmediate(this.blockedDisplay[0].gameObject);
			}
			this.blockedDisplay.RemoveAt(0);
		}
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x000AC49B File Offset: 0x000AA69B
	[Button("Save All", 0)]
	public void SaveAll()
	{
		this.RecordWalkable();
		this.SaveBlockedArea();
		this.RecordSubObjectPlacements();
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x000AC4AF File Offset: 0x000AA6AF
	[Button("Clear All", 0)]
	public void ClearAll()
	{
		this.ClearBlockedDisplay();
		this.ClearWalkable();
		this.ClearSubObjectDisplay();
		this.furnitureClass = null;
		this.furniture = null;
		if (this.furnitureParent != null)
		{
			Object.DestroyImmediate(this.furnitureParent.gameObject);
		}
	}

	// Token: 0x04000D43 RID: 3395
	[Header("Assign")]
	public FurniturePreset furniture;

	// Token: 0x04000D44 RID: 3396
	public FurnitureClass furnitureClass;

	// Token: 0x04000D45 RID: 3397
	public Transform furnitureParent;

	// Token: 0x04000D46 RID: 3398
	[Header("Components")]
	public Transform furnitureAnchor;

	// Token: 0x04000D47 RID: 3399
	public Transform walkableNodeParent;

	// Token: 0x04000D48 RID: 3400
	public GameObject walkableObject;

	// Token: 0x04000D49 RID: 3401
	public List<WalkableRecorder.TileSetup> tiles = new List<WalkableRecorder.TileSetup>();

	// Token: 0x04000D4A RID: 3402
	public Transform blockingParent;

	// Token: 0x04000D4B RID: 3403
	public Material nonBlockedMaterial;

	// Token: 0x04000D4C RID: 3404
	public Material blockedMaterial;

	// Token: 0x04000D4D RID: 3405
	public List<DebugBlockingSelector> blockedDisplay = new List<DebugBlockingSelector>();

	// Token: 0x04000D4E RID: 3406
	public List<Transform> walkableDisplay = new List<Transform>();

	// Token: 0x04000D4F RID: 3407
	public List<SubObjectPlacement> subObjectDisplay = new List<SubObjectPlacement>();

	// Token: 0x04000D50 RID: 3408
	[Header("Load Subobjects")]
	public GameObject subObjectPrefab;

	// Token: 0x04000D51 RID: 3409
	private Vector2[] offsetArrayX8 = new Vector2[]
	{
		new Vector2(-1f, -1f),
		new Vector2(0f, -1f),
		new Vector2(1f, -1f),
		new Vector2(-1f, 0f),
		new Vector2(1f, 0f),
		new Vector2(-1f, 1f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	// Token: 0x0200020B RID: 523
	[Serializable]
	public class TileSetup
	{
		// Token: 0x04000D52 RID: 3410
		public Vector2 offset;

		// Token: 0x04000D53 RID: 3411
		public Transform trans;
	}
}
