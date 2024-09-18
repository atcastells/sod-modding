using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public class PipeConstructor : MonoBehaviour
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06001845 RID: 6213 RVA: 0x0016A231 File Offset: 0x00168431
	public static PipeConstructor Instance
	{
		get
		{
			return PipeConstructor._instance;
		}
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x0016A238 File Offset: 0x00168438
	private void Awake()
	{
		if (PipeConstructor._instance != null && PipeConstructor._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		PipeConstructor._instance = this;
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x0016A268 File Offset: 0x00168468
	public List<NewWall> WallPathfind(NewWall from, NewWall to, PipeConstructor.PipeGroup existingGroup)
	{
		HashSet<NewWall> hashSet = new HashSet<NewWall>();
		HashSet<NewWall> hashSet2 = new HashSet<NewWall>();
		hashSet2.Add(from);
		Dictionary<NewWall, NewWall> dictionary = new Dictionary<NewWall, NewWall>();
		Dictionary<NewWall, float> dictionary2 = new Dictionary<NewWall, float>();
		dictionary2.Add(from, 0f);
		Dictionary<NewWall, float> dictionary3 = new Dictionary<NewWall, float>();
		dictionary3.Add(from, Vector3.Distance(from.position, to.position));
		int num = 0;
		while (hashSet2.Count > 0)
		{
			NewWall current = Enumerable.FirstOrDefault<NewWall>(hashSet2);
			float num2 = float.PositiveInfinity;
			foreach (NewWall newWall in hashSet2)
			{
				if (dictionary3.ContainsKey(newWall) && dictionary3[newWall] < num2)
				{
					num2 = dictionary3[newWall];
					current = newWall;
				}
			}
			if (current == to)
			{
				List<NewWall> list = new List<NewWall>();
				list.Add(current);
				while (dictionary.ContainsKey(current))
				{
					NewWall current2 = current;
					current = dictionary[current];
					if (current == null)
					{
						break;
					}
					if (current != current2)
					{
						list.Add(current);
					}
				}
				list.Reverse();
				return list;
			}
			if (current.node != null)
			{
				using (List<NewWall>.Enumerator enumerator2 = current.node.walls.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NewWall w = enumerator2.Current;
						if (w != current && !w.preset.ignoreCullingRaycasts && !hashSet.Contains(w) && !hashSet2.Contains(w))
						{
							float num3 = dictionary2[current] + 1f;
							if (Mathf.Abs(w.wallOffset.y) == Mathf.Abs(current.wallOffset.y) && Mathf.Abs(w.wallOffset.x) == Mathf.Abs(current.wallOffset.x))
							{
								num3 += 1f;
							}
							if (existingGroup != null && existingGroup.routes.Find((PipeConstructor.PipeRoute item) => item.w == w.id) != null)
							{
								num3 = 0.5f;
							}
							hashSet2.Add(w);
							if (!dictionary2.ContainsKey(w) || num3 < dictionary2[w])
							{
								if (!dictionary.ContainsKey(w))
								{
									dictionary.Add(w, current);
								}
								else
								{
									dictionary[w] = current;
								}
								if (!dictionary2.ContainsKey(w))
								{
									dictionary2.Add(w, num3);
								}
								else
								{
									dictionary2[w] = num3;
								}
								float num4 = dictionary2[w] + Vector3.Distance(w.node.position, to.node.position);
								if (!dictionary3.ContainsKey(w))
								{
									dictionary3.Add(w, num4);
								}
								else
								{
									dictionary3[w] = num4;
								}
							}
						}
					}
				}
			}
			foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX8)
			{
				Vector3Int vector3Int = current.node.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
				{
					using (List<NewWall>.Enumerator enumerator2 = newNode.walls.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NewWall w = enumerator2.Current;
							if (w != current && !w.preset.ignoreCullingRaycasts && !hashSet.Contains(w) && !hashSet2.Contains(w))
							{
								float num5 = Vector3.Distance(current.position, w.position);
								if (num5 <= 1.801f)
								{
									float num6 = 0f;
									if (newNode.room != current.node.room)
									{
										if (current.node.accessToOtherNodes.ContainsKey(newNode))
										{
											NewNode.NodeAccess nodeAccess = current.node.accessToOtherNodes[newNode];
											if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.door)
											{
												num6 = 3f;
											}
											else if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway)
											{
												num6 = 1f;
											}
											else
											{
												num6 = 8f;
											}
										}
										else
										{
											num6 = 12f;
										}
									}
									float num7 = dictionary2[current] + num5 + num6;
									if (existingGroup != null && existingGroup.routes.Find((PipeConstructor.PipeRoute item) => item.w == w.id) != null)
									{
										num7 = 1f;
									}
									hashSet2.Add(w);
									if (!dictionary2.ContainsKey(w) || num7 < dictionary2[w])
									{
										if (!dictionary.ContainsKey(w))
										{
											dictionary.Add(w, current);
										}
										else
										{
											dictionary[w] = current;
										}
										if (!dictionary2.ContainsKey(w))
										{
											dictionary2.Add(w, num7);
										}
										else
										{
											dictionary2[w] = num7;
										}
										float num8 = dictionary2[w] + Vector3.Distance(w.node.position, to.node.position);
										if (!dictionary3.ContainsKey(w))
										{
											dictionary3.Add(w, num8);
										}
										else
										{
											dictionary3[w] = num8;
										}
									}
								}
							}
						}
					}
				}
			}
			Predicate<NewWall> <>9__2;
			for (int j = 0; j < 2; j++)
			{
				Vector3Int vector3Int2 = current.node.nodeCoord + new Vector3Int(0, 0, 1);
				if (j == 1)
				{
					vector3Int2 = current.node.nodeCoord + new Vector3Int(0, 0, -1);
				}
				NewNode newNode2 = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2))
				{
					PipeConstructor.<>c__DisplayClass12_3 CS$<>8__locals4 = new PipeConstructor.<>c__DisplayClass12_3();
					PipeConstructor.<>c__DisplayClass12_3 CS$<>8__locals5 = CS$<>8__locals4;
					List<NewWall> walls = newNode2.walls;
					Predicate<NewWall> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((NewWall item) => item.wallOffset == current.wallOffset));
					}
					CS$<>8__locals5.w = walls.Find(predicate);
					if (CS$<>8__locals4.w != null && !CS$<>8__locals4.w.preset.ignoreCullingRaycasts && CS$<>8__locals4.w.preset.sectionClass != DoorPairPreset.WallSectionClass.entrance && CS$<>8__locals4.w.preset.sectionClass != DoorPairPreset.WallSectionClass.windowLarge && !hashSet.Contains(CS$<>8__locals4.w) && !hashSet2.Contains(CS$<>8__locals4.w))
					{
						float num9;
						if (current.node.accessToOtherNodes.ContainsKey(newNode2))
						{
							if (current.node.accessToOtherNodes[newNode2].accessType == NewNode.NodeAccess.AccessType.verticalSpace)
							{
								num9 = 1f;
							}
							else
							{
								num9 = 15f;
							}
						}
						else
						{
							num9 = 18f;
						}
						float num10 = dictionary2[current] + Vector3.Distance(current.position, CS$<>8__locals4.w.position) + num9;
						if (existingGroup != null && existingGroup.routes.Find((PipeConstructor.PipeRoute item) => item.w == CS$<>8__locals4.w.id) != null)
						{
							num10 = 0.5f;
						}
						hashSet2.Add(CS$<>8__locals4.w);
						if (!dictionary2.ContainsKey(CS$<>8__locals4.w) || num10 < dictionary2[CS$<>8__locals4.w])
						{
							if (!dictionary.ContainsKey(CS$<>8__locals4.w))
							{
								dictionary.Add(CS$<>8__locals4.w, current);
							}
							else
							{
								dictionary[CS$<>8__locals4.w] = current;
							}
							if (!dictionary2.ContainsKey(CS$<>8__locals4.w))
							{
								dictionary2.Add(CS$<>8__locals4.w, num10);
							}
							else
							{
								dictionary2[CS$<>8__locals4.w] = num10;
							}
							float num11 = dictionary2[CS$<>8__locals4.w] + Vector3.Distance(CS$<>8__locals4.w.node.position, to.node.position);
							if (!dictionary3.ContainsKey(CS$<>8__locals4.w))
							{
								dictionary3.Add(CS$<>8__locals4.w, num11);
							}
							else
							{
								dictionary3[CS$<>8__locals4.w] = num11;
							}
						}
					}
				}
			}
			hashSet2.Remove(current);
			hashSet.Add(current);
			num++;
			if (num > 9999)
			{
				return null;
			}
		}
		return null;
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x0016AC5C File Offset: 0x00168E5C
	public bool IsLeftOf(NewWall one, NewWall two)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetTRS(two.position, Quaternion.Euler(two.localEulerAngles + two.node.room.transform.eulerAngles), Vector3.one);
		return identity.inverse.MultiplyPoint3x4(one.position).x < -0.1f;
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x0016ACCC File Offset: 0x00168ECC
	public bool IsRightOf(NewWall one, NewWall two)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetTRS(two.position, Quaternion.Euler(two.localEulerAngles + two.node.room.transform.eulerAngles), Vector3.one);
		return identity.inverse.MultiplyPoint3x4(one.position).x > 0.1f;
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x0016AD3C File Offset: 0x00168F3C
	public bool IsFrontOf(NewWall one, NewWall two)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetTRS(two.position, Quaternion.Euler(two.localEulerAngles + two.node.room.transform.eulerAngles), Vector3.one);
		return identity.inverse.MultiplyPoint3x4(one.position).z > 0.1f;
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x0016ADAC File Offset: 0x00168FAC
	public void GeneratePipes()
	{
		Dictionary<Interactable, NewWall> dictionary = new Dictionary<Interactable, NewWall>();
		Dictionary<Interactable, PipeConstructor.PipeGroup> dictionary2 = new Dictionary<Interactable, PipeConstructor.PipeGroup>();
		new Dictionary<NewAddress, PipeConstructor.PipeGroup>();
		string seed = CityData.Instance.seed;
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			if (newAddress.securityCameras.Count > 0 || newAddress.sentryGuns.Count > 0 || newAddress.otherSecurity.Count > 0 || newAddress.alarms.Count > 0)
			{
				Interactable breakerSecurity = newAddress.GetBreakerSecurity();
				if (breakerSecurity != null)
				{
					PipeConstructor.PipeGroup pipeGroup = null;
					NewWall newWall = null;
					if (dictionary2.ContainsKey(breakerSecurity))
					{
						pipeGroup = dictionary2[breakerSecurity];
						newWall = dictionary[breakerSecurity];
					}
					else
					{
						pipeGroup = new PipeConstructor.PipeGroup(PipeConstructor.PipeType.wire);
						dictionary2.Add(breakerSecurity, pipeGroup);
						using (List<FurnitureLocation>.Enumerator enumerator2 = breakerSecurity.node.individualFurniture.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FurnitureLocation furn = enumerator2.Current;
								if (furn.furniture.classes[0].wallPiece)
								{
									newWall = breakerSecurity.node.walls.Find((NewWall item) => item.localEulerAngles.y + item.node.room.transform.eulerAngles.y == (float)furn.angle);
									if (newWall != null)
									{
										dictionary.Add(breakerSecurity, newWall);
										break;
									}
								}
							}
						}
					}
					foreach (Interactable interactable in newAddress.securityCameras)
					{
						if (interactable.node.walls.Count > 0)
						{
							NewWall from = interactable.node.walls[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interactable.node.walls.Count, seed, out seed)];
							pipeGroup.AddPipeRoute(from, newWall, new int[]
							{
								default(int),
								6
							}, new int[]
							{
								5
							});
						}
					}
					foreach (Interactable interactable2 in newAddress.sentryGuns)
					{
						if (interactable2.node.walls.Count > 0)
						{
							NewWall from2 = interactable2.node.walls[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interactable2.node.walls.Count, seed, out seed)];
							pipeGroup.AddPipeRoute(from2, newWall, new int[]
							{
								default(int),
								6
							}, new int[]
							{
								5
							});
						}
					}
					foreach (Interactable interactable3 in newAddress.otherSecurity)
					{
						if (interactable3.node.walls.Count > 0)
						{
							NewWall from3 = interactable3.node.walls[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interactable3.node.walls.Count, seed, out seed)];
							pipeGroup.AddPipeRoute(from3, newWall, new int[]
							{
								default(int),
								6
							}, new int[]
							{
								5
							});
						}
					}
					using (List<Interactable>.Enumerator enumerator3 = newAddress.alarms.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Interactable alarm = enumerator3.Current;
							if (alarm.node.walls.Count > 0)
							{
								if (alarm.preset.name == "AlarmSwitch")
								{
									NewWall newWall2 = alarm.node.walls.Find((NewWall item) => item.localEulerAngles.y + item.node.room.transform.eulerAngles.y == alarm.wEuler.y);
									if (newWall2 != null)
									{
										pipeGroup.AddPipeRoute(newWall2, newWall, 5, 5);
									}
								}
								else
								{
									NewWall newWall3 = alarm.node.walls.Find((NewWall item) => item.localEulerAngles.y + item.node.room.transform.eulerAngles.y == alarm.wEuler.y);
									if (newWall3 != null)
									{
										pipeGroup.AddPipeRoute(newWall3, newWall, -1, 5);
									}
								}
							}
						}
					}
					Interactable breakerSecurity2 = newAddress.GetBreakerSecurity();
					if (breakerSecurity2 != null)
					{
						foreach (Interactable interactable4 in breakerSecurity.node.floor.securityDoors)
						{
							if (interactable4.node.gameLocation.thisAsAddress.GetBreakerDoors() == breakerSecurity2 && interactable4.node.walls.Count > 0)
							{
								NewWall from4 = interactable4.node.walls[Toolbox.Instance.GetPsuedoRandomNumberContained(0, interactable4.node.walls.Count, seed, out seed)];
								pipeGroup.AddPipeRoute(from4, newWall, 0, 5);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x0016B340 File Offset: 0x00169540
	[Button(null, 0)]
	public void GetWall()
	{
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			foreach (NewRoom newRoom in newAddress.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (NewWall newWall in newNode.walls)
					{
						if (newWall.id == this.debugGetWall1)
						{
							string[] array = new string[6];
							array[0] = newWall.id.ToString();
							array[1] = ": ";
							int num = 2;
							Vector3 vector = newWall.position;
							array[num] = vector.ToString();
							array[3] = ", ";
							int num2 = 4;
							vector = newWall.localEulerAngles;
							array[num2] = vector.ToString();
							array[5] = newWall.node.room.transform.eulerAngles.ToString();
							Game.Log(string.Concat(array), 2);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x0016B524 File Offset: 0x00169724
	[Button(null, 0)]
	public void LeftRightCheck()
	{
		NewWall newWall = null;
		NewWall newWall2 = null;
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			foreach (NewRoom newRoom in newAddress.rooms)
			{
				foreach (NewNode newNode in newRoom.nodes)
				{
					foreach (NewWall newWall3 in newNode.walls)
					{
						if (newWall3.id == this.debugGetWall1)
						{
							newWall = newWall3;
						}
						if (newWall3.id == this.debugGetWall2)
						{
							newWall2 = newWall3;
						}
					}
				}
			}
		}
		if (newWall != null && newWall2 != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Is ",
				this.debugGetWall1.ToString(),
				" left of ",
				this.debugGetWall2.ToString(),
				": ",
				this.IsLeftOf(newWall, newWall2).ToString()
			}), 2);
			Game.Log(string.Concat(new string[]
			{
				"Is ",
				this.debugGetWall1.ToString(),
				" right of ",
				this.debugGetWall2.ToString(),
				": ",
				this.IsRightOf(newWall, newWall2).ToString()
			}), 2);
		}
	}

	// Token: 0x04001E3C RID: 7740
	[Header("Components")]
	public List<PipeConstructor.PipeSetup> pipeConfig = new List<PipeConstructor.PipeSetup>();

	// Token: 0x04001E3D RID: 7741
	[Header("Generated")]
	public List<PipeConstructor.PipeGroup> generated = new List<PipeConstructor.PipeGroup>();

	// Token: 0x04001E3E RID: 7742
	public int debugGetWall1;

	// Token: 0x04001E3F RID: 7743
	public int debugGetWall2;

	// Token: 0x04001E40 RID: 7744
	private static PipeConstructor _instance;

	// Token: 0x0200043A RID: 1082
	[Serializable]
	public class PipeSetup
	{
		// Token: 0x04001E41 RID: 7745
		public PipeConstructor.PipeType type;

		// Token: 0x04001E42 RID: 7746
		public Material material;

		// Token: 0x04001E43 RID: 7747
		public List<GameObject> models = new List<GameObject>();
	}

	// Token: 0x0200043B RID: 1083
	public enum PipeType
	{
		// Token: 0x04001E45 RID: 7749
		wire,
		// Token: 0x04001E46 RID: 7750
		wire2
	}

	// Token: 0x0200043C RID: 1084
	[Serializable]
	public class PipeGroup
	{
		// Token: 0x06001850 RID: 6224 RVA: 0x0016B745 File Offset: 0x00169945
		public PipeGroup(PipeConstructor.PipeType newType)
		{
			this.type = (int)newType;
			PipeConstructor.Instance.generated.Add(this);
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x0016B77A File Offset: 0x0016997A
		public void AddPipeRoute(NewWall from, NewWall to, int sourceIndex, int endIndex)
		{
			this.AddPipeRoute(from, to, new int[]
			{
				sourceIndex
			}, new int[]
			{
				endIndex
			});
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x0016B79C File Offset: 0x0016999C
		public void AddPipeRoute(NewWall from, NewWall to, int[] sourceIndex, int[] endIndex)
		{
			if (from == null)
			{
				return;
			}
			if (to == null)
			{
				return;
			}
			List<NewWall> list = PipeConstructor.Instance.WallPathfind(from, to, this);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					NewWall current = list[i];
					if (!this.rooms.Contains(current.node.room.roomID))
					{
						this.rooms.Add(current.node.room.roomID);
					}
					PipeConstructor.PipeRoute pipeRoute = this.routes.Find((PipeConstructor.PipeRoute item) => item.w == current.id);
					if (pipeRoute == null)
					{
						pipeRoute = new PipeConstructor.PipeRoute();
						pipeRoute.w = current.id;
						this.routes.Add(pipeRoute);
					}
					NewWall newWall = null;
					if (i < list.Count - 1)
					{
						newWall = list[i + 1];
					}
					NewWall newWall2 = null;
					if (i > 0)
					{
						newWall2 = list[i - 1];
					}
					if (i == 0)
					{
						foreach (int num in sourceIndex)
						{
							if (num >= 0 && !pipeRoute.s.Contains(num))
							{
								pipeRoute.s.Add(num);
							}
						}
					}
					else if (i == list.Count - 1)
					{
						foreach (int num2 in endIndex)
						{
							if (num2 >= 0 && !pipeRoute.s.Contains(num2))
							{
								pipeRoute.s.Add(num2);
							}
						}
					}
					if (newWall != null)
					{
						if (newWall.node.nodeCoord.z < current.node.nodeCoord.z)
						{
							if (!pipeRoute.s.Contains(1))
							{
								pipeRoute.s.Add(1);
							}
						}
						else if (newWall.node.nodeCoord.z > current.node.nodeCoord.z && !pipeRoute.s.Contains(0))
						{
							pipeRoute.s.Add(0);
						}
					}
					if (newWall2 != null)
					{
						if (newWall2.node.nodeCoord.z < current.node.nodeCoord.z)
						{
							if (!pipeRoute.s.Contains(1))
							{
								pipeRoute.s.Add(1);
							}
						}
						else if (newWall2.node.nodeCoord.z > current.node.nodeCoord.z && !pipeRoute.s.Contains(0))
						{
							pipeRoute.s.Add(0);
						}
					}
					if (!pipeRoute.s.Contains(2))
					{
						if (newWall != null && newWall2 != null && newWall.node.nodeCoord.z == current.node.nodeCoord.z && newWall2.node.nodeCoord.z == current.node.nodeCoord.z && ((PipeConstructor.Instance.IsLeftOf(newWall, current) && PipeConstructor.Instance.IsRightOf(newWall2, current)) || (PipeConstructor.Instance.IsLeftOf(newWall2, current) && PipeConstructor.Instance.IsRightOf(newWall, current))))
						{
							pipeRoute.s.Add(2);
						}
						else
						{
							if (newWall != null && newWall.node.nodeCoord.z == current.node.nodeCoord.z)
							{
								if (PipeConstructor.Instance.IsLeftOf(newWall, current))
								{
									if (!pipeRoute.s.Contains(3))
									{
										pipeRoute.s.Add(3);
									}
								}
								else if (PipeConstructor.Instance.IsRightOf(newWall, current))
								{
									if (!pipeRoute.s.Contains(4))
									{
										pipeRoute.s.Add(4);
									}
								}
								else if (PipeConstructor.Instance.IsFrontOf(newWall, current) && !pipeRoute.s.Contains(7))
								{
									pipeRoute.s.Add(7);
								}
							}
							if (newWall2 != null && newWall2.node.nodeCoord.z == current.node.nodeCoord.z)
							{
								if (PipeConstructor.Instance.IsLeftOf(newWall2, current))
								{
									if (!pipeRoute.s.Contains(3) && !pipeRoute.s.Contains(2))
									{
										pipeRoute.s.Add(3);
									}
								}
								else if (PipeConstructor.Instance.IsRightOf(newWall2, current))
								{
									if (!pipeRoute.s.Contains(4) && !pipeRoute.s.Contains(2))
									{
										pipeRoute.s.Add(4);
									}
								}
								else if (PipeConstructor.Instance.IsFrontOf(newWall2, current) && !pipeRoute.s.Contains(7))
								{
									pipeRoute.s.Add(7);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x0016BCB4 File Offset: 0x00169EB4
		public void AddToRoomsAsReferences()
		{
			foreach (int num in this.rooms)
			{
				NewRoom newRoom = null;
				if (CityData.Instance.roomDictionary.TryGetValue(num, ref newRoom) && !newRoom.pipes.Contains(this))
				{
					newRoom.pipes.Add(this);
				}
			}
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x0016BD30 File Offset: 0x00169F30
		public void SetVisible(bool val)
		{
			this.isVisible = val;
			if (this.spawned != null)
			{
				this.spawned.SetActive(this.isVisible);
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x0016BD58 File Offset: 0x00169F58
		public void Spawn()
		{
			if (this.spawned == null)
			{
				List<MeshFilter> list = new List<MeshFilter>();
				PipeConstructor.PipeSetup pipeSetup = PipeConstructor.Instance.pipeConfig.Find((PipeConstructor.PipeSetup item) => item.type == (PipeConstructor.PipeType)this.type);
				if (pipeSetup == null)
				{
					return;
				}
				foreach (PipeConstructor.PipeRoute pipeRoute in this.routes)
				{
					NewWall newWall = null;
					if (this.TryGetWall(pipeRoute.w, out newWall))
					{
						using (List<int>.Enumerator enumerator2 = pipeRoute.s.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								int num = enumerator2.Current;
								if (num < pipeSetup.models.Count)
								{
									GameObject gameObject = Object.Instantiate<GameObject>(pipeSetup.models[num], PipeConstructor.Instance.transform);
									gameObject.transform.position = newWall.position;
									gameObject.transform.localEulerAngles = newWall.localEulerAngles + newWall.node.room.transform.eulerAngles;
									list.Add(gameObject.GetComponent<MeshFilter>());
									if (num == 6 && newWall.node.floor != null)
									{
										gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (float)newWall.node.floor.defaultCeilingHeight * 0.1f, gameObject.transform.position.z);
									}
								}
								else
								{
									Game.Log("Unable to get piece index " + num.ToString(), 2);
								}
							}
							continue;
						}
					}
					Game.Log("Unable to find wall ID " + pipeRoute.w.ToString(), 2);
				}
				this.spawned = new GameObject();
				this.spawned.transform.SetParent(PipeConstructor.Instance.transform);
				this.spawned.transform.position = Vector3.zero;
				this.spawned.transform.eulerAngles = Vector3.zero;
				CombineInstance[] array = new CombineInstance[list.Count];
				int i = 0;
				while (i < list.Count)
				{
					if (!(list[i].transform == this.spawned.transform))
					{
						array[i].mesh = list[i].sharedMesh;
						array[i].transform = list[i].transform.localToWorldMatrix;
						Object.Destroy(list[i].gameObject);
						i++;
					}
				}
				MeshFilter meshFilter = this.spawned.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = this.spawned.AddComponent<MeshRenderer>();
				meshFilter.mesh = new Mesh();
				meshFilter.mesh.CombineMeshes(array, true, true);
				this.spawned.SetActive(true);
				meshRenderer.sharedMaterial = pipeSetup.material;
				this.spawned.transform.localPosition = Vector3.zero;
				this.spawned.transform.localEulerAngles = Vector3.zero;
				NewRoom newRoom = null;
				if (this.rooms.Count > 0)
				{
					CityData.Instance.roomDictionary.TryGetValue(this.rooms[0], ref newRoom);
				}
				if (newRoom != null)
				{
					Toolbox.Instance.SetLightLayer(this.spawned, newRoom.building, false);
				}
			}
			if (this.spawned != null)
			{
				this.spawned.SetActive(false);
				this.SetVisible(false);
			}
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0016C138 File Offset: 0x0016A338
		public bool TryGetWall(int input, out NewWall output)
		{
			output = null;
			Predicate<NewWall> <>9__0;
			foreach (int num in this.rooms)
			{
				NewRoom newRoom = null;
				if (CityData.Instance.roomDictionary.TryGetValue(num, ref newRoom))
				{
					foreach (NewNode newNode in newRoom.nodes)
					{
						List<NewWall> walls = newNode.walls;
						Predicate<NewWall> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((NewWall item) => item.id == input));
						}
						output = walls.Find(predicate);
						if (output != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x04001E47 RID: 7751
		public int type;

		// Token: 0x04001E48 RID: 7752
		public List<PipeConstructor.PipeRoute> routes = new List<PipeConstructor.PipeRoute>();

		// Token: 0x04001E49 RID: 7753
		public List<int> rooms = new List<int>();

		// Token: 0x04001E4A RID: 7754
		[NonSerialized]
		public GameObject spawned;

		// Token: 0x04001E4B RID: 7755
		[NonSerialized]
		public bool isVisible;
	}

	// Token: 0x0200043F RID: 1087
	[Serializable]
	public class PipeRoute
	{
		// Token: 0x04001E4F RID: 7759
		public int w;

		// Token: 0x04001E50 RID: 7760
		public List<int> s = new List<int>();
	}
}
