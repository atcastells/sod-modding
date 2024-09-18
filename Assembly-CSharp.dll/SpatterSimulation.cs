using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000488 RID: 1160
[Serializable]
public class SpatterSimulation
{
	// Token: 0x060018ED RID: 6381 RVA: 0x001716A0 File Offset: 0x0016F8A0
	public SpatterSimulation(Human newHuman, Vector3 newLocalPosition, Vector3 newDirection, SpatterPatternPreset spatter, SpatterSimulation.EraseMode newEraseMode, float newSpatterCountMultiplier = 1f, bool newStickToActors = true)
	{
		this.preset = spatter;
		this.presetStr = spatter.name;
		this.eraseMode = newEraseMode;
		this.eraseModeTimeStamp = -1f;
		this.spatterCountMultiplier = newSpatterCountMultiplier;
		this.stickToActors = newStickToActors;
		this.createdAt = SessionData.Instance.gameTime;
		this.worldOrigin = newHuman.transform.TransformPoint(newLocalPosition);
		this.worldTarget = newHuman.transform.TransformPoint(newLocalPosition + newDirection);
		this.nodeCoord = CityData.Instance.RealPosToNodeInt(this.worldOrigin);
		NewNode newNode = null;
		if (!PathFinder.Instance.nodeMap.TryGetValue(this.nodeCoord, ref newNode))
		{
			newNode = newHuman.currentNode;
		}
		if (newNode == null)
		{
			return;
		}
		this.room = newNode.room;
		this.roomID = this.room.roomID;
		this.room.spatter.Add(this);
		GameplayController.Instance.spatter.Add(this);
		if (this.room.geometryLoaded)
		{
			this.Execute();
		}
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x001717F0 File Offset: 0x0016F9F0
	public SpatterSimulation(Vector3 newWorldPosition, Vector3 newWorldTarget, SpatterPatternPreset spatter, SpatterSimulation.EraseMode newEraseMode, float newSpatterCountMultiplier = 1f, bool newStickToActors = true)
	{
		this.preset = spatter;
		this.presetStr = spatter.name;
		this.eraseMode = newEraseMode;
		this.eraseModeTimeStamp = -1f;
		this.spatterCountMultiplier = newSpatterCountMultiplier * GameplayControls.Instance.bloodAmountMultiplier;
		this.stickToActors = newStickToActors;
		this.createdAt = SessionData.Instance.gameTime;
		this.worldOrigin = newWorldPosition;
		this.worldTarget = newWorldTarget;
		this.nodeCoord = CityData.Instance.RealPosToNodeInt(this.worldOrigin);
		NewNode newNode = null;
		if (!PathFinder.Instance.nodeMap.TryGetValue(this.nodeCoord, ref newNode))
		{
			return;
		}
		this.room = newNode.room;
		this.roomID = this.room.roomID;
		this.room.spatter.Add(this);
		GameplayController.Instance.spatter.Add(this);
		if (this.room.geometryLoaded)
		{
			this.Execute();
		}
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x00171920 File Offset: 0x0016FB20
	public void Execute()
	{
		this.isExecuted = true;
		this.executedAt = SessionData.Instance.gameTime;
		int num = Mathf.RoundToInt((float)this.preset.spatterCount * this.spatterCountMultiplier);
		AnimationCurve spreadCurve = this.preset.spreadCurve;
		Vector2 rayLength = this.preset.rayLength;
		for (int i = 0; i < num; i++)
		{
			float num2 = spreadCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false)) * this.preset.maxAngleX;
			float num3 = spreadCurve.Evaluate(Toolbox.Instance.Rand(0f, 1f, false)) * this.preset.maxAngleY;
			Vector3 vector = this.worldTarget + new Vector3(Toolbox.Instance.Rand(-num2, num2, false), Toolbox.Instance.Rand(-num3, num3, false), Toolbox.Instance.Rand(-num2, num2, false)) - this.worldOrigin;
			float num4 = Toolbox.Instance.Rand(rayLength.x, rayLength.y, false);
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(this.worldOrigin, vector), ref raycastHit, num4, Toolbox.Instance.spatterLayerMask))
			{
				GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.spatterSimulation, raycastHit.point, Quaternion.FromToRotation(Vector3.up, raycastHit.normal), raycastHit.transform);
				DecalProjector component = gameObject.GetComponent<DecalProjector>();
				SpatterSimulation.DecalSpawnData decalSpawnData = new SpatterSimulation.DecalSpawnData
				{
					spawnedProjector = component,
					sim = this
				};
				Citizen componentInParent = raycastHit.transform.GetComponentInParent<Citizen>();
				decalSpawnData.subObjectName = raycastHit.transform.name;
				if (componentInParent != null)
				{
					decalSpawnData.parentID = SpatterSimulation.ParentID.human;
					decalSpawnData.transformParentID = componentInParent.humanID;
				}
				else
				{
					NewDoor componentInParent2 = raycastHit.transform.GetComponentInParent<NewDoor>();
					if (componentInParent2 != null)
					{
						decalSpawnData.parentID = SpatterSimulation.ParentID.door;
						decalSpawnData.transformParentID = componentInParent2.wall.id;
					}
					else
					{
						InteractableController componentInParent3 = raycastHit.transform.GetComponentInParent<InteractableController>();
						if (componentInParent3 != null)
						{
							decalSpawnData.parentID = SpatterSimulation.ParentID.interactable;
							if (componentInParent3.interactable != null)
							{
								decalSpawnData.transformParentID = componentInParent3.interactable.id;
							}
						}
						else
						{
							NewRoom componentInParent4 = raycastHit.transform.GetComponentInParent<NewRoom>();
							if (componentInParent4 != null)
							{
								decalSpawnData.parentID = SpatterSimulation.ParentID.room;
								decalSpawnData.transformParentID = componentInParent4.roomID;
								gameObject.transform.SetParent(componentInParent4.transform, true);
							}
						}
					}
				}
				float num5 = raycastHit.distance / num4;
				float num6 = 0.05f;
				if (num5 < 0.5f)
				{
					num6 = 0.1f;
				}
				component.size = new Vector3(num6 + 0.0001f, num6 + 0.0001f, num6 + 0.0001f);
				decalSpawnData.size = component.size;
				gameObject.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x / num6) * num6, Mathf.Round(gameObject.transform.position.y / num6) * num6, Mathf.Round(gameObject.transform.position.z / num6) * num6);
				decalSpawnData.worldPos = gameObject.transform.position;
				if (num5 <= 0.33f)
				{
					component.material = this.preset.heavyMaterial;
					decalSpawnData.materialType = SpatterSimulation.DecalMaterialType.heavy;
				}
				else if (num5 <= 0.66f)
				{
					component.material = this.preset.mediumMaterial;
					decalSpawnData.materialType = SpatterSimulation.DecalMaterialType.medium;
				}
				else
				{
					component.material = this.preset.lightMaterial;
					decalSpawnData.materialType = SpatterSimulation.DecalMaterialType.light;
				}
				decalSpawnData.worldEuler = gameObject.transform.eulerAngles;
				this.decalsSpawned.Add(decalSpawnData);
			}
		}
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x00171D08 File Offset: 0x0016FF08
	public void Remove()
	{
		while (this.decalsSpawned.Count > 0)
		{
			SpatterSimulation.DecalSpawnData ds = this.decalsSpawned[0];
			if (ds != null)
			{
				if (ds.parentID == SpatterSimulation.ParentID.interactable)
				{
					if (ds.i == null)
					{
						ds.i = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == ds.transformParentID);
					}
					if (ds.i != null && ds.i.spawnedDecals != null)
					{
						ds.i.spawnedDecals.Remove(ds);
					}
				}
				if (ds.spawnedProjector != null)
				{
					SpatterSimulation.DecalSpawnData.RecycleDecalProjector(ds.spawnedProjector);
				}
			}
			this.decalsSpawned.RemoveAt(0);
		}
		if (this.room != null && this.room.spatter != null)
		{
			this.room.spatter.Remove(this);
		}
		GameplayController.Instance.spatter.Remove(this);
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x00171E37 File Offset: 0x00170037
	public void LoadFromSerializedData()
	{
		Toolbox.Instance.LoadDataFromResources<SpatterPatternPreset>(this.presetStr, out this.preset);
		CityData.Instance.roomDictionary.TryGetValue(this.roomID, ref this.room);
		this.UpdateSpawning();
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x00171E74 File Offset: 0x00170074
	public void UpdateSpawning()
	{
		using (List<SpatterSimulation.DecalSpawnData>.Enumerator enumerator = this.decalsSpawned.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SpatterSimulation.DecalSpawnData ds = enumerator.Current;
				if (ds.sim == null)
				{
					ds.sim = this;
				}
				if (ds.spawnedProjector == null)
				{
					Transform transform = null;
					if (ds.parentID == SpatterSimulation.ParentID.human)
					{
						Human human = null;
						if (CityData.Instance.GetHuman(ds.transformParentID, out human, true))
						{
							transform = Toolbox.Instance.SearchForTransform(human.transform, ds.subObjectName, false);
							if (transform == null)
							{
								Game.LogError(string.Concat(new string[]
								{
									"Unable to load spatter with parent ",
									ds.parentID.ToString(),
									" id ",
									ds.transformParentID.ToString(),
									" transform name: ",
									ds.subObjectName
								}), 2);
							}
						}
					}
					else if (ds.parentID == SpatterSimulation.ParentID.door)
					{
						NewDoor newDoor = null;
						if (CityData.Instance.doorDictionary.TryGetValue(ds.transformParentID, ref newDoor))
						{
							if (newDoor.spawnedDoor != null)
							{
								transform = newDoor.spawnedDoor.transform;
							}
							else if (transform == null)
							{
								Game.LogError(string.Concat(new string[]
								{
									"Unable to load spatter with parent ",
									ds.parentID.ToString(),
									" id ",
									ds.transformParentID.ToString(),
									" transform name: ",
									ds.subObjectName
								}), 2);
							}
						}
						else
						{
							Game.LogError("Unable to find door ID " + ds.transformParentID.ToString() + " for spatter", 2);
						}
					}
					else if (ds.parentID == SpatterSimulation.ParentID.interactable)
					{
						if (ds.i == null)
						{
							ds.i = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == ds.transformParentID);
						}
						if (ds.i != null)
						{
							if (ds.i.spawnedObject != null)
							{
								transform = Toolbox.Instance.SearchForTransform(ds.i.spawnedObject.transform, ds.subObjectName, false);
								if (transform == null)
								{
									Game.LogError(string.Concat(new string[]
									{
										"Unable to load spatter with parent ",
										ds.parentID.ToString(),
										" id ",
										ds.transformParentID.ToString(),
										" transform name: ",
										ds.subObjectName
									}), 2);
								}
							}
							else
							{
								if (ds.i.spawnedDecals == null)
								{
									ds.i.spawnedDecals = new List<SpatterSimulation.DecalSpawnData>();
								}
								if (!ds.i.spawnedDecals.Contains(ds))
								{
									ds.i.spawnedDecals.Add(ds);
								}
							}
						}
					}
					else if (ds.parentID == SpatterSimulation.ParentID.room)
					{
						NewRoom newRoom = null;
						if (CityData.Instance.roomDictionary.TryGetValue(ds.transformParentID, ref newRoom))
						{
							transform = newRoom.transform;
						}
					}
					if (!(transform == null))
					{
						ds.SpawnOnTransform(transform);
					}
				}
			}
		}
	}

	// Token: 0x0400216B RID: 8555
	[Header("Serialized References")]
	public Vector3 worldOrigin;

	// Token: 0x0400216C RID: 8556
	public Vector3 worldTarget;

	// Token: 0x0400216D RID: 8557
	public Vector3Int nodeCoord;

	// Token: 0x0400216E RID: 8558
	public string presetStr;

	// Token: 0x0400216F RID: 8559
	public int roomID = -1;

	// Token: 0x04002170 RID: 8560
	public SpatterSimulation.EraseMode eraseMode = SpatterSimulation.EraseMode.useDespawnTimeOnceExecuted;

	// Token: 0x04002171 RID: 8561
	public SpatterSimulation.ForceType force;

	// Token: 0x04002172 RID: 8562
	public float spatterCountMultiplier = 1f;

	// Token: 0x04002173 RID: 8563
	public float createdAt;

	// Token: 0x04002174 RID: 8564
	public bool isExecuted;

	// Token: 0x04002175 RID: 8565
	public float executedAt;

	// Token: 0x04002176 RID: 8566
	public float eraseModeTimeStamp = -1f;

	// Token: 0x04002177 RID: 8567
	public bool stickToActors = true;

	// Token: 0x04002178 RID: 8568
	public List<SpatterSimulation.DecalSpawnData> decalsSpawned = new List<SpatterSimulation.DecalSpawnData>();

	// Token: 0x04002179 RID: 8569
	[Header("Non-Serialized References")]
	[NonSerialized]
	public NewRoom room;

	// Token: 0x0400217A RID: 8570
	[NonSerialized]
	public SpatterPatternPreset preset;

	// Token: 0x02000489 RID: 1161
	public enum EraseMode
	{
		// Token: 0x0400217C RID: 8572
		neverOrManual,
		// Token: 0x0400217D RID: 8573
		onceExecutedAndOutOfBuildingPlusDespawnTime,
		// Token: 0x0400217E RID: 8574
		onceExecutedAndOutOfAddressPlusDespawnTime,
		// Token: 0x0400217F RID: 8575
		useDespawnTime,
		// Token: 0x04002180 RID: 8576
		useDespawnTimeOnceExecuted
	}

	// Token: 0x0200048A RID: 1162
	public enum ForceType
	{
		// Token: 0x04002182 RID: 8578
		bulletForward,
		// Token: 0x04002183 RID: 8579
		bulletBack,
		// Token: 0x04002184 RID: 8580
		punch
	}

	// Token: 0x0200048B RID: 1163
	[Serializable]
	public class DecalSpawnData
	{
		// Token: 0x060018F3 RID: 6387 RVA: 0x00172280 File Offset: 0x00170480
		public void SpawnOnTransform(Transform spawnTransform)
		{
			if (this.spawnedProjector == null && spawnTransform != null)
			{
				this.spawnedProjector = SpatterSimulation.DecalSpawnData.GetNewDecalProjector();
				if (this.spawnedProjector != null)
				{
					this.spawnedProjector.transform.SetParent(spawnTransform);
					this.spawnedProjector.size = this.size;
					this.spawnedProjector.gameObject.transform.eulerAngles = this.worldEuler;
					this.spawnedProjector.gameObject.transform.position = this.worldPos;
					if (this.materialType == SpatterSimulation.DecalMaterialType.heavy)
					{
						this.spawnedProjector.material = this.sim.preset.heavyMaterial;
						return;
					}
					if (this.materialType == SpatterSimulation.DecalMaterialType.medium)
					{
						this.spawnedProjector.material = this.sim.preset.mediumMaterial;
						return;
					}
					this.spawnedProjector.material = this.sim.preset.lightMaterial;
				}
			}
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x00172384 File Offset: 0x00170584
		public static void InitialisePool()
		{
			for (int i = 0; i < 30; i++)
			{
				DecalProjector component = Toolbox.Instance.SpawnObject(PrefabControls.Instance.spatterSimulation, null).GetComponent<DecalProjector>();
				component.transform.SetParent(null);
				component.transform.position = new Vector3(0f, -1000f, 0f);
				SpatterSimulation.DecalSpawnData.decalPool.Enqueue(component);
			}
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x001723EF File Offset: 0x001705EF
		public static DecalProjector GetNewDecalProjector()
		{
			if (SpatterSimulation.DecalSpawnData.decalPool.Count > 0)
			{
				return SpatterSimulation.DecalSpawnData.decalPool.Dequeue();
			}
			DecalProjector component = Toolbox.Instance.SpawnObject(PrefabControls.Instance.spatterSimulation, null).GetComponent<DecalProjector>();
			component.transform.SetParent(null);
			return component;
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x0017242F File Offset: 0x0017062F
		public static void RecycleDecalProjector(DecalProjector decalProjector)
		{
			decalProjector.transform.SetParent(null);
			decalProjector.transform.position = new Vector3(0f, -1000f, 0f);
			SpatterSimulation.DecalSpawnData.decalPool.Enqueue(decalProjector);
		}

		// Token: 0x04002185 RID: 8581
		public SpatterSimulation.ParentID parentID;

		// Token: 0x04002186 RID: 8582
		public int transformParentID = -1;

		// Token: 0x04002187 RID: 8583
		public string subObjectName;

		// Token: 0x04002188 RID: 8584
		public Vector3 worldPos;

		// Token: 0x04002189 RID: 8585
		public Vector3 worldEuler;

		// Token: 0x0400218A RID: 8586
		public Vector3 size;

		// Token: 0x0400218B RID: 8587
		public SpatterSimulation.DecalMaterialType materialType;

		// Token: 0x0400218C RID: 8588
		[NonSerialized]
		public DecalProjector spawnedProjector;

		// Token: 0x0400218D RID: 8589
		[NonSerialized]
		public Interactable i;

		// Token: 0x0400218E RID: 8590
		[NonSerialized]
		public SpatterSimulation sim;

		// Token: 0x0400218F RID: 8591
		private const int INITIAL_POOL_SIZE = 30;

		// Token: 0x04002190 RID: 8592
		private const float RECYCLED_Y_POSITION = -1000f;

		// Token: 0x04002191 RID: 8593
		[NonSerialized]
		private static Queue<DecalProjector> decalPool = new Queue<DecalProjector>();
	}

	// Token: 0x0200048C RID: 1164
	public enum DecalMaterialType
	{
		// Token: 0x04002193 RID: 8595
		light,
		// Token: 0x04002194 RID: 8596
		medium,
		// Token: 0x04002195 RID: 8597
		heavy
	}

	// Token: 0x0200048D RID: 1165
	public enum ParentID
	{
		// Token: 0x04002197 RID: 8599
		room,
		// Token: 0x04002198 RID: 8600
		human,
		// Token: 0x04002199 RID: 8601
		interactable,
		// Token: 0x0400219A RID: 8602
		door
	}
}
