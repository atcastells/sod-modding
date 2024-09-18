using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200041B RID: 1051
public class SceneRecorder
{
	// Token: 0x14000026 RID: 38
	// (add) Token: 0x060017E8 RID: 6120 RVA: 0x00165788 File Offset: 0x00163988
	// (remove) Token: 0x060017E9 RID: 6121 RVA: 0x001657C0 File Offset: 0x001639C0
	public event SceneRecorder.OnCapture OnNewCapture;

	// Token: 0x060017EA RID: 6122 RVA: 0x001657F8 File Offset: 0x001639F8
	public SceneRecorder(Interactable newInteractable)
	{
		this.interactable = newInteractable;
		this.coversRooms.Clear();
		if (this.interactable.node != null)
		{
			this.coversRooms.Add(this.interactable.node.room);
		}
		if (this.interactable != Player.Instance.interactable)
		{
			this.lastCaptureAt = SessionData.Instance.gameTime + Toolbox.Instance.GetPsuedoRandomNumber(0f, GameplayControls.Instance.captureInterval, newInteractable.seed, false);
			CitizenBehaviour.Instance.sceneRecorders.Add(this);
		}
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x001658B0 File Offset: 0x00163AB0
	public void RefreshCoveredArea()
	{
		this.coveredNodes.Clear();
		this.coveredNodes.Add(this.interactable.node, null);
		this.coversRooms.Clear();
		this.coversRooms.Add(this.interactable.node.room);
		float num = GameplayControls.Instance.captureFoV * 0.5f;
		foreach (NewNode newNode in this.interactable.node.room.nodes)
		{
			if (!this.coveredNodes.ContainsKey(newNode))
			{
				Vector3 vector = newNode.position + new Vector3(0f, 1f, 0f);
				Vector3 vector2 = vector - this.interactable.cvp;
				Vector3 vector3 = Quaternion.Euler(this.interactable.cve) * Vector3.forward;
				float num2 = Vector3.Angle(vector2, vector3);
				List<DataRaycastController.NodeRaycastHit> list;
				if ((this.interactable.node == newNode || (num2 >= -num && num2 <= num)) && Vector3.Distance(vector, this.interactable.cvp) <= GameplayControls.Instance.captureRange && DataRaycastController.Instance.NodeRaycast(this.interactable.node, newNode, out list, null, false))
				{
					this.coveredNodes.Add(newNode, null);
					if (!this.coversRooms.Contains(newNode.room))
					{
						this.coversRooms.Add(newNode.room);
					}
				}
			}
		}
		foreach (KeyValuePair<NewRoom, List<NewRoom.CullTreeEntry>> keyValuePair in this.interactable.node.room.cullingTree)
		{
			if (!keyValuePair.Key.isNullRoom)
			{
				foreach (NewNode newNode2 in keyValuePair.Key.nodes)
				{
					if (!this.coveredNodes.ContainsKey(newNode2))
					{
						Vector3 vector4 = newNode2.position + new Vector3(0f, 1f, 0f);
						Vector3 vector5 = vector4 - this.interactable.cvp;
						Vector3 vector6 = Quaternion.Euler(this.interactable.cve) * Vector3.forward;
						float num3 = Vector3.Angle(vector5, vector6);
						List<DataRaycastController.NodeRaycastHit> list2;
						if ((this.interactable.node == newNode2 || (num3 >= -num && num3 <= num)) && Vector3.Distance(vector4, this.interactable.cvp) <= GameplayControls.Instance.captureRange && DataRaycastController.Instance.NodeRaycast(this.interactable.node, newNode2, out list2, null, false))
						{
							List<int> list3 = null;
							foreach (DataRaycastController.NodeRaycastHit nodeRaycastHit in list2)
							{
								if (nodeRaycastHit.conditionalDoors != null && nodeRaycastHit.conditionalDoors.Count > 0)
								{
									if (list3 == null)
									{
										list3 = new List<int>();
									}
									list3.AddRange(nodeRaycastHit.conditionalDoors);
								}
							}
							this.coveredNodes.Add(newNode2, list3);
							if (!this.coversRooms.Contains(newNode2.room))
							{
								this.coversRooms.Add(newNode2.room);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x00165CA0 File Offset: 0x00163EA0
	public SceneRecorder.SceneCapture ExecuteCapture(bool onlyIfMovement, bool detailedCapture = false, bool prepToSaveCapture = true)
	{
		if (!this.interactable.sw0 && this.interactable != Player.Instance.interactable)
		{
			return null;
		}
		this.lastCaptureAt = SessionData.Instance.gameTime;
		if (onlyIfMovement && this.interactable.cap.Count > 0)
		{
			bool flag = true;
			if (this.interactable.cap[this.interactable.cap.Count - 1].aCap.Count > 0)
			{
				flag = false;
			}
			else
			{
				foreach (NewRoom newRoom in this.coversRooms)
				{
					if (newRoom.currentOccupants.Count > 0)
					{
						foreach (Actor actor in newRoom.currentOccupants)
						{
							Human human = (Human)actor;
							List<int> list;
							if (this.coveredNodes.TryGetValue(human.currentNode, ref list))
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag)
					{
						break;
					}
				}
			}
			if (flag)
			{
				return null;
			}
		}
		SceneRecorder.SceneCapture sceneCapture = new SceneRecorder.SceneCapture(this, detailedCapture);
		if (prepToSaveCapture)
		{
			for (int i = 0; i < this.interactable.cap.Count; i++)
			{
				if (!this.interactable.cap[i].k)
				{
					if (this.interactable.cap.Count > 1 && this.interactable.cap[i].t < SessionData.Instance.gameTime - GameplayControls.Instance.cameraCaptureMaxTime)
					{
						this.interactable.cap.RemoveAt(i);
						i--;
					}
					else
					{
						if (this.interactable.cap.Count < GameplayControls.Instance.cameraCaptureMemory)
						{
							break;
						}
						this.interactable.cap.RemoveAt(i);
						i--;
					}
				}
			}
			if (this.interactable.cap.Count < GameplayControls.Instance.cameraCaptureMemory)
			{
				this.interactable.cap.Add(sceneCapture);
			}
		}
		if (this.OnNewCapture != null)
		{
			this.OnNewCapture();
		}
		return sceneCapture;
	}

	// Token: 0x04001D78 RID: 7544
	public Interactable interactable;

	// Token: 0x04001D79 RID: 7545
	public List<NewRoom> coversRooms = new List<NewRoom>();

	// Token: 0x04001D7A RID: 7546
	public Dictionary<NewNode, List<int>> coveredNodes = new Dictionary<NewNode, List<int>>();

	// Token: 0x04001D7B RID: 7547
	public static List<ScenePoserController> scenePoserPool = new List<ScenePoserController>();

	// Token: 0x04001D7C RID: 7548
	public static Dictionary<string, List<GameObject>> objectPoserPool = new Dictionary<string, List<GameObject>>();

	// Token: 0x04001D7D RID: 7549
	public float lastCaptureAt;

	// Token: 0x04001D7E RID: 7550
	public static int assignCapID = 1;

	// Token: 0x0200041C RID: 1052
	// (Invoke) Token: 0x060017EF RID: 6127
	public delegate void OnCapture();

	// Token: 0x0200041D RID: 1053
	[Serializable]
	public class SceneCapture
	{
		// Token: 0x060017F2 RID: 6130 RVA: 0x00165F20 File Offset: 0x00164120
		public SceneCapture(SceneRecorder newRecorder, bool detailedCapture)
		{
			this.id = SceneRecorder.assignCapID;
			SceneRecorder.assignCapID++;
			this.recorder = newRecorder;
			this.t = SessionData.Instance.gameTime;
			if (this.recorder.interactable.isActor != null && this.recorder.interactable.isActor.isPlayer)
			{
				this.recorder.RefreshCoveredArea();
				this.drp = new List<SceneRecorder.DynamicRecordPosition>();
				this.drp.Add(new SceneRecorder.DynamicRecordPosition
				{
					pos = this.recorder.interactable.cvp,
					rot = this.recorder.interactable.cve
				});
			}
			for (int i = 0; i < this.recorder.coversRooms.Count; i++)
			{
				SceneRecorder.RoomCapture roomCapture = new SceneRecorder.RoomCapture();
				roomCapture.id = this.recorder.coversRooms[i].roomID;
				roomCapture.light = this.recorder.coversRooms[i].mainLightStatus;
				this.rCap.Add(roomCapture);
			}
			HashSet<NewDoor> hashSet = new HashSet<NewDoor>();
			HashSet<NewNode> hashSet2 = new HashSet<NewNode>();
			Vector3 captureWorldPosition = this.GetCaptureWorldPosition();
			foreach (KeyValuePair<NewNode, List<int>> keyValuePair in this.recorder.coveredNodes)
			{
				if (!hashSet2.Contains(keyValuePair.Key))
				{
					if (keyValuePair.Value != null && keyValuePair.Value.Count > 0)
					{
						bool flag = false;
						foreach (int num in keyValuePair.Value)
						{
							bool flag2 = true;
							NewDoor newDoor = null;
							if (CityData.Instance.doorDictionary.TryGetValue(num, ref newDoor))
							{
								if (!hashSet.Contains(newDoor))
								{
									this.dCap.Add(new SceneRecorder.DoorCapture(newDoor));
									hashSet.Add(newDoor);
								}
								if (newDoor.isClosed)
								{
									break;
								}
							}
							else
							{
								Game.LogError(string.Concat(new string[]
								{
									"Cannot find door at wall ",
									num.ToString(),
									" (",
									CityData.Instance.doorDictionary.Count.ToString(),
									")"
								}), 2);
							}
							if (flag2)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					foreach (Interactable interactable in keyValuePair.Key.interactables)
					{
						if (interactable.preset.showWorldObjectInSceneCapture)
						{
							if (interactable.preset.captureStateInSceneCapture)
							{
								this.oSCap.Add(new SceneRecorder.InteractableStateCapture(interactable));
							}
						}
						else if (interactable.preset.createProxy && interactable.preset.prefab != null && (!interactable.preset.onlyCreateProxyInDetailedCapture || detailedCapture))
						{
							float num2 = Vector3.Distance(interactable.GetWorldPosition(true), captureWorldPosition);
							float num3 = 999f;
							if (ObjectPoolingController.Instance.loadRanges.ContainsKey((int)interactable.preset.createProxyAtRange))
							{
								num3 = ObjectPoolingController.Instance.loadRanges[(int)interactable.preset.createProxyAtRange];
							}
							if (num2 < num3)
							{
								this.oCap.Add(new SceneRecorder.InteractableCapture(interactable));
							}
						}
					}
					hashSet2.Add(keyValuePair.Key);
				}
			}
			foreach (NewRoom newRoom in this.recorder.coversRooms)
			{
				foreach (Actor actor in newRoom.currentOccupants)
				{
					Human human = (Human)actor;
					if (!human.isPlayer)
					{
						if (!hashSet2.Contains(human.currentNode))
						{
							if (this.recorder.interactable.isActor != null && this.recorder.interactable.isActor.isPlayer)
							{
								Vector3 vector = human.currentNode.position + new Vector3(0f, 1f, 0f);
								Vector3 vector2 = vector - this.recorder.interactable.cvp;
								Vector3 vector3 = Quaternion.Euler(this.recorder.interactable.cve) * Vector3.forward;
								float num4 = Vector3.Angle(vector2, vector3);
								string[] array = new string[6];
								array[0] = "Gameplay: ";
								array[1] = human.name;
								array[2] = " is not on a visible node (node angle from cam: ";
								array[3] = num4.ToString();
								array[4] = "): ";
								int num5 = 5;
								Vector3 vector4 = vector;
								array[num5] = vector4.ToString();
								Game.Log(string.Concat(array), 2);
							}
						}
						else if (Vector3.Distance(human.transform.position, captureWorldPosition) <= GameplayControls.Instance.humanCaptureRange)
						{
							SceneRecorder.ActorCapture actorCapture = new SceneRecorder.ActorCapture(human, false);
							this.aCap.Add(actorCapture);
						}
					}
				}
			}
			hashSet2 = null;
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x0016653C File Offset: 0x0016473C
		public SceneCapture(SceneRecorder.SceneCapture copyFrom)
		{
			this.id = SceneRecorder.assignCapID;
			SceneRecorder.assignCapID++;
			this.recorder = copyFrom.recorder;
			this.t = copyFrom.t;
			this.drp = copyFrom.drp;
			this.k = copyFrom.k;
			this.rCap = new List<SceneRecorder.RoomCapture>(copyFrom.rCap);
			this.dCap = new List<SceneRecorder.DoorCapture>(copyFrom.dCap);
			this.aCap = new List<SceneRecorder.ActorCapture>(copyFrom.aCap);
			this.oCap = new List<SceneRecorder.InteractableCapture>(copyFrom.oCap);
			this.oSCap = new List<SceneRecorder.InteractableStateCapture>(copyFrom.oSCap);
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x00166624 File Offset: 0x00164824
		public NewGameLocation GetCaptureGamelocation()
		{
			NewNode newNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(this.GetCaptureWorldPosition(), false, true, false, default(Vector3Int), false, 0, false, 200);
			if (newNode != null)
			{
				return newNode.gameLocation;
			}
			return null;
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x00166664 File Offset: 0x00164864
		public NewRoom GetCaptureRoom()
		{
			NewNode newNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(this.GetCaptureWorldPosition(), false, true, false, default(Vector3Int), false, 0, false, 200);
			if (newNode != null)
			{
				return newNode.room;
			}
			return null;
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x001666A4 File Offset: 0x001648A4
		public float GetDecimalClock()
		{
			float result = 0f;
			int num;
			int num2;
			int num3;
			int num4;
			SessionData.WeekDay weekDay;
			SessionData.Month month;
			int num5;
			SessionData.Instance.ParseTimeData(this.t, out result, out num, out num2, out num3, out num4, out weekDay, out month, out num5);
			return result;
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x001666D8 File Offset: 0x001648D8
		public Vector3 GetCaptureWorldPosition()
		{
			if (this.drp != null && this.drp.Count > 0)
			{
				return this.drp[0].pos;
			}
			if (this.recorder != null)
			{
				return this.recorder.interactable.cvp;
			}
			Game.LogError("Unable to get capture interactable", 2);
			return Vector3.zero;
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x00166738 File Offset: 0x00164938
		public Vector3 GetCaptureWorldRotation()
		{
			if (this.drp != null && this.drp.Count > 0)
			{
				return this.drp[0].rot;
			}
			if (this.recorder != null)
			{
				return this.recorder.interactable.cve;
			}
			return Vector3.zero;
		}

		// Token: 0x04001D80 RID: 7552
		[NonSerialized]
		public SceneRecorder recorder;

		// Token: 0x04001D81 RID: 7553
		public int id;

		// Token: 0x04001D82 RID: 7554
		public List<SceneRecorder.DynamicRecordPosition> drp;

		// Token: 0x04001D83 RID: 7555
		public float t;

		// Token: 0x04001D84 RID: 7556
		public bool k;

		// Token: 0x04001D85 RID: 7557
		public List<SceneRecorder.RoomCapture> rCap = new List<SceneRecorder.RoomCapture>();

		// Token: 0x04001D86 RID: 7558
		public List<SceneRecorder.DoorCapture> dCap = new List<SceneRecorder.DoorCapture>();

		// Token: 0x04001D87 RID: 7559
		public List<SceneRecorder.ActorCapture> aCap = new List<SceneRecorder.ActorCapture>();

		// Token: 0x04001D88 RID: 7560
		public List<SceneRecorder.InteractableCapture> oCap = new List<SceneRecorder.InteractableCapture>();

		// Token: 0x04001D89 RID: 7561
		public List<SceneRecorder.InteractableStateCapture> oSCap = new List<SceneRecorder.InteractableStateCapture>();
	}

	// Token: 0x0200041E RID: 1054
	[Serializable]
	public class DynamicRecordPosition
	{
		// Token: 0x04001D8A RID: 7562
		public Vector3 pos;

		// Token: 0x04001D8B RID: 7563
		public Vector3 rot;
	}

	// Token: 0x0200041F RID: 1055
	[Serializable]
	public class RoomCapture
	{
		// Token: 0x060017FA RID: 6138 RVA: 0x0016678C File Offset: 0x0016498C
		public NewRoom GetRoom()
		{
			NewRoom result = null;
			CityData.Instance.roomDictionary.TryGetValue(this.id, ref result);
			return result;
		}

		// Token: 0x04001D8C RID: 7564
		public int id;

		// Token: 0x04001D8D RID: 7565
		public bool light;
	}

	// Token: 0x02000420 RID: 1056
	[Serializable]
	public class TransformCapture
	{
		// Token: 0x060017FC RID: 6140 RVA: 0x001667B4 File Offset: 0x001649B4
		public TransformCapture(Vector3 pos, Quaternion rot)
		{
			this.wP = new Vector3((float)Mathf.RoundToInt(pos.x * 100f) / 100f, (float)Mathf.RoundToInt(pos.y * 100f) / 100f, (float)Mathf.RoundToInt(pos.z * 100f) / 100f);
			this.wR = new Quaternion((float)Mathf.RoundToInt(rot.x * 100f) / 100f, (float)Mathf.RoundToInt(rot.y * 100f) / 100f, (float)Mathf.RoundToInt(rot.z * 100f) / 100f, (float)Mathf.RoundToInt(rot.w * 100f) / 100f);
		}

		// Token: 0x04001D8E RID: 7566
		public Vector3 wP;

		// Token: 0x04001D8F RID: 7567
		public Quaternion wR;
	}

	// Token: 0x02000421 RID: 1057
	[Serializable]
	public class DoorCapture
	{
		// Token: 0x060017FD RID: 6141 RVA: 0x00166885 File Offset: 0x00164A85
		public DoorCapture(NewDoor door)
		{
			this.id = door.wall.id;
			this.a = Mathf.RoundToInt(door.ajar);
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x001668B0 File Offset: 0x00164AB0
		public NewDoor GetDoor()
		{
			NewDoor result = null;
			CityData.Instance.doorDictionary.TryGetValue(this.id, ref result);
			return result;
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x001668D8 File Offset: 0x00164AD8
		public bool IsOpen()
		{
			return this.a != 0;
		}

		// Token: 0x04001D90 RID: 7568
		public int id;

		// Token: 0x04001D91 RID: 7569
		public int a;
	}

	// Token: 0x02000422 RID: 1058
	[Serializable]
	public class InteractableCapture : SceneRecorder.TransformCapture
	{
		// Token: 0x06001800 RID: 6144 RVA: 0x001668E8 File Offset: 0x00164AE8
		public InteractableCapture(Interactable newInter) : base(newInter.wPos, Quaternion.Euler(newInter.wEuler))
		{
			if (newInter.spawnedObject != null)
			{
				this.wP = newInter.spawnedObject.transform.position;
				this.wR = newInter.spawnedObject.transform.rotation;
			}
			this.p = newInter.preset.name;
			if (newInter.preset.isDecal)
			{
				ArtPreset artPreset = newInter.objectRef as ArtPreset;
				this.d = new List<string>();
				if (artPreset != null)
				{
					this.d.Add(artPreset.name);
				}
				else
				{
					Interactable.Passed passed = newInter.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.decalDynamicText);
					if (passed != null)
					{
						this.d.Add(passed.str);
					}
				}
				if (this.d.Count <= 0)
				{
					this.d.Add("");
				}
				if (newInter.controller != null && newInter.controller.decalProjector != null)
				{
					List<string> list = this.d;
					Vector3 size = newInter.controller.decalProjector.size;
					list.Add(size.x.ToString());
					List<string> list2 = this.d;
					size = newInter.controller.decalProjector.size;
					list2.Add(size.y.ToString());
				}
			}
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00166A68 File Offset: 0x00164C68
		public InteractablePreset GetPreset()
		{
			return Toolbox.Instance.GetInteractablePreset(this.p);
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x00166A7C File Offset: 0x00164C7C
		public void Load()
		{
			InteractablePreset pr = this.GetPreset();
			if (pr == null)
			{
				return;
			}
			if (SceneRecorder.objectPoserPool.ContainsKey(this.p) && SceneRecorder.objectPoserPool[this.p].Count > 0)
			{
				this.poser = SceneRecorder.objectPoserPool[this.p][0];
				SceneRecorder.objectPoserPool[this.p].RemoveAt(0);
			}
			else
			{
				if (SceneRecorder.objectPoserPool.Keys.Count > 30)
				{
					string text = Enumerable.FirstOrDefault<string>(SceneRecorder.objectPoserPool.Keys);
					for (int i = 0; i < SceneRecorder.objectPoserPool[text].Count; i++)
					{
						Toolbox.Instance.DestroyObject(SceneRecorder.objectPoserPool[text][i].gameObject);
					}
					SceneRecorder.objectPoserPool.Remove(text);
				}
				if (pr != null && pr.spawnable && pr.prefab != null)
				{
					this.poser = Toolbox.Instance.SpawnObject(pr.prefab, PrefabControls.Instance.poserContainer);
					this.poser.name = pr.prefab.name;
					LODGroup componentInChildren = this.poser.GetComponentInChildren<LODGroup>(true);
					if (componentInChildren != null)
					{
						Object.DestroyImmediate(componentInChildren);
					}
				}
			}
			if (this.poser != null)
			{
				this.poser.transform.position = this.wP;
				this.poser.transform.rotation = this.wR;
				this.poser.SetActive(true);
				GameObject gameObject = this.poser;
				string name = gameObject.name;
				string text2 = " ";
				Quaternion wR = this.wR;
				gameObject.name = name + text2 + wR.ToString();
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(this.wP), ref newNode))
				{
					Toolbox.Instance.SetLightLayer(this.poser, newNode.building, true);
				}
				if (pr.inheritColouringFromDecor && newNode != null)
				{
					if (pr.shareColoursWithFurniture != FurniturePreset.ShareColours.none)
					{
						FurnitureLocation furnitureLocation = newNode.room.individualFurniture.Find((FurnitureLocation item) => item.furniture.shareColours == pr.shareColoursWithFurniture);
						if (furnitureLocation != null)
						{
							MaterialsController.Instance.ApplyMaterialKey(this.poser, furnitureLocation.matKey);
						}
					}
					else
					{
						MaterialsController.Instance.ApplyMaterialKey(this.poser, newNode.room.miscKey);
					}
				}
				if (pr.isDecal)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("Attempting to set up decal poser...", 2);
					}
					InteractableController componentInChildren2 = this.poser.GetComponentInChildren<InteractableController>(true);
					DecalProjector componentInChildren3 = this.poser.GetComponentInChildren<DecalProjector>(true);
					componentInChildren2.decalProjector = componentInChildren3;
					if (componentInChildren2 != null && componentInChildren3 != null && this.d != null && this.d.Count > 0 && this.d[0].Length > 0)
					{
						ArtPreset artPreset = Toolbox.Instance.allArt.Find((ArtPreset item) => item.name == this.d[0]);
						Interactable.Passed dynamic = null;
						if (artPreset == null)
						{
							dynamic = new Interactable.Passed(Interactable.PassedVarType.decalDynamicText, 0f, this.d[0]);
						}
						componentInChildren2.SetupDecal(artPreset, dynamic, false);
						float x = componentInChildren3.size.x;
						float y = componentInChildren3.size.y;
						float.TryParse(this.d[1], ref x);
						float.TryParse(this.d[2], ref y);
						if (Game.Instance.printDebug)
						{
							Game.Log("Setting decal projector size: " + x.ToString() + ", " + y.ToString(), 2);
						}
						componentInChildren3.pivot = new Vector3(0f, 0f, 0.6f);
						componentInChildren3.size = new Vector3(x, y, 0.13f);
					}
				}
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00166EC8 File Offset: 0x001650C8
		public void Unload()
		{
			if (this.poser != null)
			{
				this.poser.SetActive(false);
				if (!SceneRecorder.objectPoserPool.ContainsKey(this.p))
				{
					SceneRecorder.objectPoserPool.Add(this.p, new List<GameObject>());
				}
				SceneRecorder.objectPoserPool[this.p].Add(this.poser);
				this.poser = null;
			}
		}

		// Token: 0x04001D92 RID: 7570
		public string p;

		// Token: 0x04001D93 RID: 7571
		public List<string> d;

		// Token: 0x04001D94 RID: 7572
		[NonSerialized]
		public GameObject poser;
	}

	// Token: 0x02000425 RID: 1061
	[Serializable]
	public class InteractableStateCapture
	{
		// Token: 0x0600180A RID: 6154 RVA: 0x00166F7C File Offset: 0x0016517C
		public InteractableStateCapture(Interactable i)
		{
			this.id = i.id;
			this.sw = i.sw0;
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x00166F9C File Offset: 0x0016519C
		public void Load()
		{
			Interactable interactable = this.GetInteractable();
			if (interactable != null)
			{
				if (Player.Instance.computerInteractable != null && Player.Instance.computerInteractable == interactable)
				{
					return;
				}
				if (interactable.sw0 != this.sw)
				{
					interactable.SetSwitchState(this.sw, null, false, false, true);
				}
			}
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x00166FEC File Offset: 0x001651EC
		public Interactable GetInteractable()
		{
			Interactable result = null;
			CityData.Instance.savableInteractableDictionary.TryGetValue(this.id, ref result);
			return result;
		}

		// Token: 0x04001D99 RID: 7577
		public int id;

		// Token: 0x04001D9A RID: 7578
		public bool sw;
	}

	// Token: 0x02000426 RID: 1062
	[Serializable]
	public class ActorCapture
	{
		// Token: 0x0600180D RID: 6157 RVA: 0x00167014 File Offset: 0x00165214
		public ActorCapture(Human newHuman, bool limbCapture)
		{
			this.id = newHuman.humanID;
			this.o = (int)newHuman.outfitController.currentOutfit;
			this.pos = new Vector3(Toolbox.Instance.RoundToPlaces(newHuman.transform.position.x, 2), Toolbox.Instance.RoundToPlaces(newHuman.transform.position.y, 2), Toolbox.Instance.RoundToPlaces(newHuman.transform.position.z, 2));
			this.rot = new Vector3(Toolbox.Instance.RoundToPlaces(newHuman.transform.eulerAngles.x, 2), Toolbox.Instance.RoundToPlaces(newHuman.transform.eulerAngles.y, 2), Toolbox.Instance.RoundToPlaces(newHuman.transform.eulerAngles.z, 2));
			if (newHuman.isMoving)
			{
				if (newHuman.isRunning)
				{
					this.sp = 2;
				}
				else
				{
					this.sp = 1;
				}
			}
			else
			{
				this.sp = 0;
			}
			if (newHuman.ai != null)
			{
				if (newHuman.ai.spawnedLeftItem != null)
				{
					this.lH = new SceneRecorder.HandItemCapture(newHuman.ai.spawnedLeftItem, newHuman.ai.spawnedLeftItem.transform.localPosition, newHuman.ai.spawnedLeftItem.transform.localRotation);
				}
				if (newHuman.ai.spawnedRightItem != null)
				{
					this.rH = new SceneRecorder.HandItemCapture(newHuman.ai.spawnedRightItem, newHuman.ai.spawnedRightItem.transform.localPosition, newHuman.ai.spawnedRightItem.transform.localRotation);
				}
			}
			if (newHuman.isDead || newHuman.isStunned)
			{
				limbCapture = true;
			}
			if (limbCapture)
			{
				this.limb = new List<SceneRecorder.LimbCapture>();
				using (List<CitizenOutfitController.AnchorConfig>.Enumerator enumerator = newHuman.outfitController.anchorConfig.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CitizenOutfitController.AnchorConfig anchorConfig = enumerator.Current;
						if (anchorConfig.captureInSurveillance && !(anchorConfig.trans == null))
						{
							SceneRecorder.LimbCapture limbCapture2 = new SceneRecorder.LimbCapture(anchorConfig.anchor, anchorConfig.trans.position, anchorConfig.trans.rotation);
							this.limb.Add(limbCapture2);
						}
					}
					return;
				}
			}
			if (newHuman.animationController != null)
			{
				this.main = (int)newHuman.animationController.idleAnimationState;
				this.arms = (int)newHuman.animationController.armsBoolAnimationState;
			}
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x001672B8 File Offset: 0x001654B8
		public Human GetHuman()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.id, out result, true);
			return result;
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x001672DC File Offset: 0x001654DC
		public void Load()
		{
			for (int i = 0; i < SceneRecorder.scenePoserPool.Count; i++)
			{
				if (SceneRecorder.scenePoserPool[i] == null)
				{
					SceneRecorder.scenePoserPool.RemoveAt(i);
					i--;
				}
			}
			if (SceneRecorder.scenePoserPool.Count > 0)
			{
				this.poser = SceneRecorder.scenePoserPool.Find((ScenePoserController item) => item.human != null && item.human.humanID == this.id);
				if (this.poser == null)
				{
					this.poser = SceneRecorder.scenePoserPool[0];
				}
				if (this.poser != null)
				{
					SceneRecorder.scenePoserPool.Remove(this.poser);
				}
			}
			else
			{
				if (SceneRecorder.scenePoserPool.Count > 30)
				{
					Toolbox.Instance.DestroyObject(SceneRecorder.scenePoserPool[0].gameObject);
					SceneRecorder.scenePoserPool.RemoveAt(0);
				}
				GameObject gameObject = Toolbox.Instance.SpawnObject(PrefabControls.Instance.scenePoserFigure, PrefabControls.Instance.poserContainer);
				this.poser = gameObject.GetComponent<ScenePoserController>();
			}
			this.poser.SetupCitizen(this);
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x001673F4 File Offset: 0x001655F4
		public void Unload()
		{
			if (this.poser != null)
			{
				if (!SceneRecorder.scenePoserPool.Contains(this.poser))
				{
					SceneRecorder.scenePoserPool.Add(this.poser);
				}
				this.poser.gameObject.SetActive(false);
				this.poser = null;
			}
		}

		// Token: 0x04001D9B RID: 7579
		public int id;

		// Token: 0x04001D9C RID: 7580
		public int o;

		// Token: 0x04001D9D RID: 7581
		public Vector3 pos;

		// Token: 0x04001D9E RID: 7582
		public Vector3 rot;

		// Token: 0x04001D9F RID: 7583
		public int main;

		// Token: 0x04001DA0 RID: 7584
		public int arms;

		// Token: 0x04001DA1 RID: 7585
		public int sp;

		// Token: 0x04001DA2 RID: 7586
		public List<SceneRecorder.LimbCapture> limb;

		// Token: 0x04001DA3 RID: 7587
		public SceneRecorder.HandItemCapture lH;

		// Token: 0x04001DA4 RID: 7588
		public SceneRecorder.HandItemCapture rH;

		// Token: 0x04001DA5 RID: 7589
		[NonSerialized]
		public ScenePoserController poser;
	}

	// Token: 0x02000427 RID: 1063
	[Serializable]
	public class LimbCapture : SceneRecorder.TransformCapture
	{
		// Token: 0x06001812 RID: 6162 RVA: 0x0016746E File Offset: 0x0016566E
		public LimbCapture(CitizenOutfitController.CharacterAnchor anchor, Vector3 pos, Quaternion rot) : base(pos, rot)
		{
			this.a = (int)anchor;
		}

		// Token: 0x04001DA6 RID: 7590
		public int a;
	}

	// Token: 0x02000428 RID: 1064
	[Serializable]
	public class HandItemCapture : SceneRecorder.TransformCapture
	{
		// Token: 0x06001813 RID: 6163 RVA: 0x0016747F File Offset: 0x0016567F
		public HandItemCapture(GameObject obj, Vector3 pos, Quaternion rot) : base(pos, rot)
		{
			this.i = obj.name;
		}

		// Token: 0x04001DA7 RID: 7591
		public string i;
	}
}
