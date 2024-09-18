using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006C7 RID: 1735
[CreateAssetMenu(fileName = "building_data", menuName = "Database/Building Preset")]
public class BuildingPreset : SoCustomComparison
{
	// Token: 0x060024F5 RID: 9461 RVA: 0x001E31D8 File Offset: 0x001E13D8
	[Button(null, 0)]
	public void GenerateWindowData()
	{
		this.sortedWindows.Clear();
		this.sortedWindows.TrimExcess();
		List<BuildingPreset.WindowUVBlock> list = new List<BuildingPreset.WindowUVBlock>();
		List<Vector2> list2 = new List<Vector2>();
		int num = 0;
		this.floorCount = 0;
		Transform[] componentsInChildren = this.prefab.transform.GetComponentsInChildren<Transform>(true);
		Vector3 vector = Vector3.one;
		Vector3 vector2 = Vector2.zero;
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(vector2), vector);
		foreach (Transform transform in componentsInChildren)
		{
			MeshFilter component = transform.gameObject.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh == this.captureMesh)
			{
				string[] array2 = new string[10];
				array2[0] = "Capture mesh found in prefab! Local scale is ";
				int num2 = 1;
				Vector3 vector3 = transform.localScale;
				array2[num2] = vector3.x.ToString();
				array2[2] = ", ";
				int num3 = 3;
				vector3 = transform.localScale;
				array2[num3] = vector3.y.ToString();
				array2[4] = ", ";
				int num4 = 5;
				vector3 = transform.localScale;
				array2[num4] = vector3.z.ToString();
				array2[6] = ", rotation: ";
				int num5 = 7;
				vector3 = transform.localEulerAngles;
				array2[num5] = vector3.ToString();
				array2[8] = ",  pos: ";
				int num6 = 9;
				vector3 = transform.localPosition;
				array2[num6] = vector3.ToString();
				Debug.Log(string.Concat(array2));
				vector = transform.localScale;
				vector2 = transform.localEulerAngles;
				matrix4x = Matrix4x4.TRS(transform.localPosition, Quaternion.Euler(vector2), vector);
				break;
			}
		}
		for (int j = 0; j < this.windowMap.width; j++)
		{
			for (int k = 0; k < this.windowMap.height; k++)
			{
				Vector2 vector4;
				vector4..ctor((float)j, (float)k);
				if (!list2.Contains(vector4) && this.windowMap.GetPixel(j, k) == Color.white)
				{
					num++;
					BuildingPreset.WindowUVBlock windowUVBlock = new BuildingPreset.WindowUVBlock();
					windowUVBlock.originPixel = vector4;
					windowUVBlock.rectSize = Vector2.one;
					List<Vector2> list3 = new List<Vector2>();
					List<Vector2> list4 = new List<Vector2>();
					list3.Add(vector4);
					while (list3.Count > 0)
					{
						Vector2 vector5 = list3[0];
						list4.Add(vector5);
						list2.Add(vector5);
						windowUVBlock.rectSize.x = Mathf.Max(windowUVBlock.rectSize.x, vector5.x - windowUVBlock.originPixel.x + 1f);
						windowUVBlock.rectSize.y = Mathf.Max(windowUVBlock.rectSize.y, vector5.y - windowUVBlock.originPixel.y + 1f);
						foreach (Vector2 vector6 in this.offsetArrayX4)
						{
							Vector2 vector7;
							vector7..ctor(vector5.x + vector6.x, vector5.y + vector6.y);
							if (vector7.x >= 0f && vector7.x < (float)this.windowMap.width && vector7.y >= 0f && vector7.y < (float)this.windowMap.height && !list2.Contains(vector7) && !list3.Contains(vector7) && !list4.Contains(vector7))
							{
								if (this.windowMap.GetPixel((int)vector7.x, (int)vector7.y) == Color.white)
								{
									list3.Add(vector7);
								}
								else
								{
									list2.Add(vector7);
								}
							}
						}
						list3.RemoveAt(0);
					}
					windowUVBlock.centrePixel = new Vector2(windowUVBlock.originPixel.x + Mathf.Floor(windowUVBlock.rectSize.x * 0.5f), windowUVBlock.originPixel.y + Mathf.Floor(windowUVBlock.rectSize.y * 0.5f));
					Vector2 vector8;
					vector8..ctor(windowUVBlock.centrePixel.x / (float)this.windowMap.width, windowUVBlock.centrePixel.y / (float)this.windowMap.height);
					Vector2 vector9;
					vector9..ctor((windowUVBlock.centrePixel.x + 1f) / (float)this.windowMap.width, windowUVBlock.centrePixel.y / (float)this.windowMap.height);
					windowUVBlock.localMeshPositionLeft = matrix4x.MultiplyPoint3x4(this.UvTo3D(vector8));
					windowUVBlock.localMeshPositionRight = matrix4x.MultiplyPoint3x4(this.UvTo3D(vector9));
					if (windowUVBlock.localMeshPositionLeft == Vector3.zero || windowUVBlock.localMeshPositionRight == Vector3.zero)
					{
						string text = "Unable to find local mesh position at ";
						Vector2 vector10 = vector8;
						string text2 = vector10.ToString();
						string text3 = " - ";
						vector10 = vector9;
						Game.Log(text + text2 + text3 + vector10.ToString(), 2);
					}
					else
					{
						list.Add(windowUVBlock);
					}
				}
			}
		}
		Debug.Log("Found " + list.Count.ToString() + " windows...");
		Dictionary<int, List<BuildingPreset.WindowUVBlock>> dictionary = new Dictionary<int, List<BuildingPreset.WindowUVBlock>>();
		list.Sort((BuildingPreset.WindowUVBlock p1, BuildingPreset.WindowUVBlock p2) => p1.localMeshPositionLeft.y.CompareTo(p2.localMeshPositionLeft.y));
		int num7 = 1;
		float y = list[0].localMeshPositionLeft.y;
		dictionary.Add(num7, new List<BuildingPreset.WindowUVBlock>());
		dictionary[num7].Add(list[0]);
		list[0].floor = num7;
		for (int l = 1; l < list.Count; l++)
		{
			BuildingPreset.WindowUVBlock windowUVBlock2 = list[l];
			if (Mathf.Abs(windowUVBlock2.localMeshPositionLeft.y - y) > 1f)
			{
				num7++;
				this.floorCount = num7;
			}
			if (!dictionary.ContainsKey(num7))
			{
				dictionary.Add(num7, new List<BuildingPreset.WindowUVBlock>());
			}
			dictionary[num7].Add(windowUVBlock2);
			windowUVBlock2.floor = num7;
			y = windowUVBlock2.localMeshPositionLeft.y;
		}
		Debug.Log("Floor count according to found windows: " + num7.ToString() + " (+ ground)");
		for (int m = 0; m < list.Count; m++)
		{
			BuildingPreset.WindowUVBlock windowUVBlock3 = list[m];
			if (Mathf.Round(windowUVBlock3.localMeshPositionLeft.x * 100f) / 100f == Mathf.Round(windowUVBlock3.localMeshPositionRight.x * 100f) / 100f)
			{
				if (windowUVBlock3.localMeshPositionRight.x < 0f)
				{
					windowUVBlock3.side = new Vector2(-1f, 0f);
				}
				else if (windowUVBlock3.localMeshPositionRight.x >= 0f)
				{
					windowUVBlock3.side = new Vector2(1f, 0f);
				}
			}
			else if (Mathf.Round(windowUVBlock3.localMeshPositionLeft.z * 100f) / 100f == Mathf.Round(windowUVBlock3.localMeshPositionRight.z * 100f) / 100f)
			{
				if (windowUVBlock3.localMeshPositionRight.z < 0f)
				{
					windowUVBlock3.side = new Vector2(0f, -1f);
				}
				else if (windowUVBlock3.localMeshPositionRight.z >= 0f)
				{
					windowUVBlock3.side = new Vector2(0f, 1f);
				}
			}
		}
		for (int n = 0; n < list.Count; n++)
		{
			BuildingPreset.WindowUVBlock thisBlock = list[n];
			List<BuildingPreset.WindowUVBlock> list5 = list.FindAll((BuildingPreset.WindowUVBlock item) => item.floor == thisBlock.floor && item.side == thisBlock.side && Mathf.Round(item.localMeshPositionLeft.x * 100f) / 100f == Mathf.Round(thisBlock.localMeshPositionLeft.x * 100f) / 100f && Mathf.Round(item.localMeshPositionLeft.z * 100f) / 100f == Mathf.Round(thisBlock.localMeshPositionLeft.z * 100f) / 100f && item != thisBlock);
			if (list5.Count > 0)
			{
				for (int num8 = 0; num8 < list5.Count; num8++)
				{
					BuildingPreset.WindowUVBlock windowUVBlock4 = list5[num8];
					thisBlock.originPixel = new Vector2(Math.Min(thisBlock.originPixel.x, windowUVBlock4.originPixel.x), Math.Min(thisBlock.originPixel.y, windowUVBlock4.originPixel.y));
					Vector2 vector11;
					vector11..ctor(windowUVBlock4.originPixel.x + windowUVBlock4.rectSize.x, windowUVBlock4.originPixel.y + windowUVBlock4.rectSize.y);
					thisBlock.rectSize = new Vector2(Mathf.Max(thisBlock.rectSize.x, vector11.x - windowUVBlock4.originPixel.x), Mathf.Max(thisBlock.rectSize.y, vector11.y - windowUVBlock4.originPixel.y));
					dictionary[windowUVBlock4.floor].Remove(windowUVBlock4);
					list.Remove(windowUVBlock4);
				}
				thisBlock.centrePixel = new Vector2(thisBlock.originPixel.x + Mathf.Floor(thisBlock.rectSize.x * 0.5f), thisBlock.originPixel.y + Mathf.Floor(thisBlock.rectSize.y * 0.5f));
			}
		}
		for (int num9 = 1; num9 <= this.floorCount; num9++)
		{
			List<BuildingPreset.WindowUVBlock> list6 = dictionary[num9];
			BuildingPreset.WindowUVFloor windowUVFloor = new BuildingPreset.WindowUVFloor();
			windowUVFloor.front = list6.FindAll((BuildingPreset.WindowUVBlock item) => item.side == new Vector2(0f, 1f));
			windowUVFloor.front.Sort((BuildingPreset.WindowUVBlock p1, BuildingPreset.WindowUVBlock p2) => p1.localMeshPositionLeft.x.CompareTo(p2.localMeshPositionLeft.x));
			windowUVFloor.front.Reverse();
			for (int num10 = 0; num10 < windowUVFloor.front.Count; num10++)
			{
				windowUVFloor.front[num10].horizonal = num10;
			}
			windowUVFloor.back = list6.FindAll((BuildingPreset.WindowUVBlock item) => item.side == new Vector2(0f, -1f));
			windowUVFloor.back.Sort((BuildingPreset.WindowUVBlock p1, BuildingPreset.WindowUVBlock p2) => p1.localMeshPositionLeft.x.CompareTo(p2.localMeshPositionLeft.x));
			for (int num11 = 0; num11 < windowUVFloor.back.Count; num11++)
			{
				windowUVFloor.back[num11].horizonal = num11;
			}
			windowUVFloor.left = list6.FindAll((BuildingPreset.WindowUVBlock item) => item.side == new Vector2(-1f, 0f));
			windowUVFloor.left.Sort((BuildingPreset.WindowUVBlock p1, BuildingPreset.WindowUVBlock p2) => p1.localMeshPositionLeft.z.CompareTo(p2.localMeshPositionLeft.z));
			windowUVFloor.left.Reverse();
			for (int num12 = 0; num12 < windowUVFloor.left.Count; num12++)
			{
				windowUVFloor.left[num12].horizonal = num12;
			}
			windowUVFloor.right = list6.FindAll((BuildingPreset.WindowUVBlock item) => item.side == new Vector2(1f, 0f));
			windowUVFloor.right.Sort((BuildingPreset.WindowUVBlock p1, BuildingPreset.WindowUVBlock p2) => p1.localMeshPositionLeft.z.CompareTo(p2.localMeshPositionLeft.z));
			for (int num13 = 0; num13 < windowUVFloor.right.Count; num13++)
			{
				windowUVFloor.right[num13].horizonal = num13;
			}
			this.sortedWindows.Add(windowUVFloor);
		}
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x001E3E14 File Offset: 0x001E2014
	[Button(null, 0)]
	public void GenerateAddonData()
	{
		this.cableLinkPoints = new List<BuildingPreset.CableLinkPoint>();
		this.sideSignPoints = new List<BuildingPreset.CableLinkPoint>();
		Transform[] componentsInChildren = this.prefab.transform.GetComponentsInChildren<Transform>(true);
		Vector3 vector = Vector3.one;
		Vector3 vector2 = Vector2.zero;
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(vector2), vector);
		foreach (Transform transform in componentsInChildren)
		{
			MeshFilter component = transform.gameObject.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh == this.captureMesh)
			{
				string[] array2 = new string[10];
				array2[0] = "Capture mesh found in prefab! Local scale is ";
				int num = 1;
				Vector3 vector3 = transform.localScale;
				array2[num] = vector3.x.ToString();
				array2[2] = ", ";
				int num2 = 3;
				vector3 = transform.localScale;
				array2[num2] = vector3.y.ToString();
				array2[4] = ", ";
				int num3 = 5;
				vector3 = transform.localScale;
				array2[num3] = vector3.z.ToString();
				array2[6] = ", rotation: ";
				int num4 = 7;
				vector3 = transform.localEulerAngles;
				array2[num4] = vector3.ToString();
				array2[8] = ",  pos: ";
				int num5 = 9;
				vector3 = transform.localPosition;
				array2[num5] = vector3.ToString();
				Debug.Log(string.Concat(array2));
				vector = transform.localScale;
				vector2 = transform.localEulerAngles;
				matrix4x = Matrix4x4.TRS(transform.localPosition, Quaternion.Euler(vector2), vector);
				break;
			}
		}
		for (int j = 0; j < this.addonMap.width; j++)
		{
			for (int k = 0; k < this.addonMap.height; k++)
			{
				Vector2 vector4;
				vector4..ctor((float)j, (float)k);
				Color pixel = this.addonMap.GetPixel(j, k);
				if (pixel == Color.red || pixel == Color.green)
				{
					BuildingPreset.CableLinkPoint cableLinkPoint = default(BuildingPreset.CableLinkPoint);
					Vector3 vector5 = this.UvTo3D(new Vector2(vector4.x / (float)this.addonMap.width, vector4.y / (float)this.addonMap.height));
					Vector2 uv;
					uv..ctor(vector4.x / (float)this.addonMap.width, vector4.y / (float)this.addonMap.height);
					Vector2 uv2;
					uv2..ctor((vector4.x + 1f) / (float)this.addonMap.width, vector4.y / (float)this.addonMap.height);
					Vector3 vector6 = this.UvTo3D(uv);
					Vector3 vector7 = this.UvTo3D(uv2);
					if (Mathf.Round(vector6.x * 100f) / 100f == Mathf.Round(vector7.x * 100f) / 100f)
					{
						if (vector7.x < 0f)
						{
							cableLinkPoint.localRot = new Vector3(0f, 270f, 0f);
						}
						else if (vector7.x >= 0f)
						{
							cableLinkPoint.localRot = new Vector3(0f, 90f, 0f);
						}
					}
					else if (Mathf.Round(vector6.z * 100f) / 100f == Mathf.Round(vector7.z * 100f) / 100f)
					{
						if (vector7.z < 0f)
						{
							cableLinkPoint.localRot = new Vector3(0f, 180f, 0f);
						}
						else if (vector7.z >= 0f)
						{
							cableLinkPoint.localRot = new Vector3(0f, 0f, 0f);
						}
					}
					cableLinkPoint.localPos = matrix4x.MultiplyPoint3x4(vector5);
					cableLinkPoint.localRot += vector2;
					if (pixel == Color.red)
					{
						this.cableLinkPoints.Add(cableLinkPoint);
					}
					else if (pixel == Color.green)
					{
						this.sideSignPoints.Add(cableLinkPoint);
					}
				}
			}
		}
		Debug.Log(string.Concat(new string[]
		{
			"Found ",
			this.cableLinkPoints.Count.ToString(),
			" cable link points, ",
			this.sideSignPoints.Count.ToString(),
			" side sign points."
		}));
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x001E4294 File Offset: 0x001E2494
	[Button(null, 0)]
	public void CalculateMeshHeight()
	{
		Bounds bounds;
		bounds..ctor(this.prefab.transform.position, Vector3.zero);
		foreach (Collider collider in this.prefab.transform.GetComponentsInChildren<Collider>())
		{
			bounds.Encapsulate(collider.bounds);
		}
		this.meshHeight = bounds.size.y;
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x001E4300 File Offset: 0x001E2500
	public Vector3 UvTo3D(Vector2 uv)
	{
		if (this.captureMesh == null || !this.captureMesh.isReadable)
		{
			Game.LogError("Mesh is not readable!", 2);
			return Vector3.zero;
		}
		int[] triangles = this.captureMesh.triangles;
		Vector2[] uv2 = this.captureMesh.uv;
		Vector3[] vertices = this.captureMesh.vertices;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector2 vector = uv2[triangles[i]];
			Vector2 vector2 = uv2[triangles[i + 1]];
			Vector2 vector3 = uv2[triangles[i + 2]];
			float num = this.Area(vector, vector2, vector3);
			if (num != 0f)
			{
				float num2 = this.Area(vector2, vector3, uv) / num;
				if (num2 >= 0f)
				{
					float num3 = this.Area(vector3, vector, uv) / num;
					if (num3 >= 0f)
					{
						float num4 = this.Area(vector, vector2, uv) / num;
						if (num4 >= 0f)
						{
							return num2 * vertices[triangles[i]] + num3 * vertices[triangles[i + 1]] + num4 * vertices[triangles[i + 2]];
						}
					}
				}
			}
		}
		return Vector3.zero;
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x001E4440 File Offset: 0x001E2640
	public float Area(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		Vector2 vector = p1 - p3;
		Vector2 vector2 = p2 - p3;
		return (vector.x * vector2.y - vector.y * vector2.x) / 2f;
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x001E447E File Offset: 0x001E267E
	public BuildingPreset.InteriorFloorSetting GetFloorSetting(int floor, int index)
	{
		if (floor >= 0)
		{
			return this.floorLayouts[index];
		}
		return this.basementLayouts[index];
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x001E44A0 File Offset: 0x001E26A0
	public int GetResidenceCount()
	{
		int num = 0;
		foreach (BuildingPreset.InteriorFloorSetting interiorFloorSetting in this.floorLayouts)
		{
			int num2 = 0;
			if (interiorFloorSetting.blueprints.Count > 0)
			{
				num2 = 99999;
			}
			foreach (TextAsset textAsset in interiorFloorSetting.blueprints)
			{
				int num3 = 0;
				FloorSaveData floorSaveData = null;
				if (CityData.Instance.floorData.TryGetValue(textAsset.name, ref floorSaveData))
				{
					using (List<AddressSaveData>.Enumerator enumerator3 = floorSaveData.a_d.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.p_n == "Apartment")
							{
								num3++;
							}
						}
					}
				}
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			num += num2;
		}
		foreach (BuildingPreset.InteriorFloorSetting interiorFloorSetting2 in this.basementLayouts)
		{
			int num4 = 0;
			if (interiorFloorSetting2.blueprints.Count > 0)
			{
				num4 = 99999;
			}
			foreach (TextAsset textAsset2 in interiorFloorSetting2.blueprints)
			{
				int num5 = 0;
				FloorSaveData floorSaveData2 = null;
				if (CityData.Instance.floorData.TryGetValue(textAsset2.name, ref floorSaveData2))
				{
					using (List<AddressSaveData>.Enumerator enumerator3 = floorSaveData2.a_d.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.p_n == "Apartment")
							{
								num5++;
							}
						}
					}
				}
				if (num5 < num4)
				{
					num4 = num5;
				}
			}
			num += num4;
		}
		return num;
	}

	// Token: 0x04003177 RID: 12663
	public bool disable;

	// Token: 0x04003178 RID: 12664
	[Header("Models")]
	[Tooltip("Reference to the building model prefab")]
	public GameObject prefab;

	// Token: 0x04003179 RID: 12665
	[Tooltip("The emission texture used to light up windows on this model (unlit)")]
	public Texture2D emissionMapUnlit;

	// Token: 0x0400317A RID: 12666
	[Tooltip("The emission texture used to light up windows on this model (lit)")]
	public Texture2D emissionMapLit;

	// Token: 0x0400317B RID: 12667
	[Tooltip("The height of this building")]
	public float buildingHeight;

	// Token: 0x0400317C RID: 12668
	[Tooltip("The local position of the lightning rod")]
	public Vector3 lightningRodLocalPos;

	// Token: 0x0400317D RID: 12669
	[Tooltip("The material to use on default walls. Leave blank to use default (brick)")]
	public List<MaterialGroupPreset> defaultExteriorWallMaterial;

	// Token: 0x0400317E RID: 12670
	[Tooltip("The material key to use on the exterior of the building")]
	public Toolbox.MaterialKey exteriorKey;

	// Token: 0x0400317F RID: 12671
	[Tooltip("Check if this building supports alley blocks")]
	public bool enableAlleywayWalls = true;

	// Token: 0x04003180 RID: 12672
	[Tooltip("Allow this building to feature quoins")]
	public bool enableExteriorQuoins = true;

	// Token: 0x04003181 RID: 12673
	[Space(7f)]
	public bool overrideEvidencePhotoSettings;

	// Token: 0x04003182 RID: 12674
	[EnableIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoPos = new Vector3(0f, 2.25f, 23f);

	// Token: 0x04003183 RID: 12675
	[EnableIf("overrideEvidencePhotoSettings")]
	public Vector3 relativeCamPhotoEuler = new Vector3(-45f, 180f, 0f);

	// Token: 0x04003184 RID: 12676
	[Header("Environment")]
	public bool overrideDistrictEnvironment = true;

	// Token: 0x04003185 RID: 12677
	[EnableIf("overrideDistrictEnvironment")]
	public SessionData.SceneProfile sceneProfile = SessionData.SceneProfile.indoors;

	// Token: 0x04003186 RID: 12678
	[Range(0f, 5f)]
	[Tooltip("The max amount of lost and found items to spawn at one time")]
	[Header("Special")]
	public int maxLostAndFound;

	// Token: 0x04003187 RID: 12679
	[Tooltip("The layouts of above-ground floors, starting with ground floor")]
	[Header("Blueprints")]
	public List<BuildingPreset.InteriorFloorSetting> floorLayouts = new List<BuildingPreset.InteriorFloorSetting>();

	// Token: 0x04003188 RID: 12680
	[Tooltip("The layouts of below-ground floors, starting with basement level 1")]
	public List<BuildingPreset.InteriorFloorSetting> basementLayouts = new List<BuildingPreset.InteriorFloorSetting>();

	// Token: 0x04003189 RID: 12681
	[Tooltip("How many control rooms should this building feature?")]
	public Vector2 controlRoomRange = Vector2.zero;

	// Token: 0x0400318A RID: 12682
	public List<DesignStylePreset> forceBuildingDesignStyles = new List<DesignStylePreset>();

	// Token: 0x0400318B RID: 12683
	public StairwellPreset stairwellRegular;

	// Token: 0x0400318C RID: 12684
	public StairwellPreset stairwellLarge;

	// Token: 0x0400318D RID: 12685
	[Header("Echelons")]
	public bool buildingFeaturesEchelonFloors;

	// Token: 0x0400318E RID: 12686
	[EnableIf("buildingFeaturesEchelonFloors")]
	public int echelonFloorStart = 10;

	// Token: 0x0400318F RID: 12687
	[Space(7f)]
	public bool overrideGrubiness;

	// Token: 0x04003190 RID: 12688
	[EnableIf("overrideGrubiness")]
	public float grubinessOverride;

	// Token: 0x04003191 RID: 12689
	[Header("Zoning")]
	public BuildingPreset.ZoneType displayedZone = BuildingPreset.ZoneType.privateProperty;

	// Token: 0x04003192 RID: 12690
	public bool allowedInAllDistricts = true;

	// Token: 0x04003193 RID: 12691
	[Tooltip("Appears in this district")]
	[DisableIf("allowedInAllDistricts")]
	public List<DistrictPreset> allowedInDistricts = new List<DistrictPreset>();

	// Token: 0x04003194 RID: 12692
	[Tooltip("Appears in density range: Not required but choices will be weighted towards this")]
	public BuildingPreset.Density densityMinimum;

	// Token: 0x04003195 RID: 12693
	public BuildingPreset.Density densityMaximum = BuildingPreset.Density.veryHigh;

	// Token: 0x04003196 RID: 12694
	[Tooltip("Appears in land value range: Not required but choices will be weighted towards this")]
	public BuildingPreset.LandValue landValueMinimum;

	// Token: 0x04003197 RID: 12695
	public BuildingPreset.LandValue landValueMaximum = BuildingPreset.LandValue.veryHigh;

	// Token: 0x04003198 RID: 12696
	[Tooltip("Try and make sure the city has at least this many buildings of this type")]
	[Space(7f)]
	public int minimum = 1;

	// Token: 0x04003199 RID: 12697
	[Range(0f, 10f)]
	[Tooltip("How important is it that the city features the above minimum amount of buildings?")]
	public int featureImportance = 4;

	// Token: 0x0400319A RID: 12698
	[Tooltip("Hard limit on the number of buildings per city")]
	public int hardLimit = 99;

	// Token: 0x0400319B RID: 12699
	[Range(0f, 1f)]
	[Tooltip("Desired ratio on the number of these buildings (1 means the whole city can be these)")]
	public float desiredRatio = 0.05f;

	// Token: 0x0400319C RID: 12700
	[Range(0f, 10f)]
	[Tooltip("Modernity: Used to choose decor- how modern the building is")]
	public int modernity = 5;

	// Token: 0x0400319D RID: 12701
	[Tooltip("Used in choosing decor: The lobby area room type")]
	public AddressPreset lobbyPreset;

	// Token: 0x0400319E RID: 12702
	[Tooltip("True if this is supposed to not have floors")]
	public bool nonEnterable;

	// Token: 0x0400319F RID: 12703
	[Tooltip("True if this is a boundary piece")]
	public bool boundary;

	// Token: 0x040031A0 RID: 12704
	[Tooltip("True if this is a boundary corner piece")]
	public bool boundaryCorner;

	// Token: 0x040031A1 RID: 12705
	[Header("Naming")]
	public bool overrideNaming;

	// Token: 0x040031A2 RID: 12706
	[EnableIf("overrideNaming")]
	public List<string> possibleNames = new List<string>();

	// Token: 0x040031A3 RID: 12707
	[Header("Map")]
	public bool customDrawOnMap;

	// Token: 0x040031A4 RID: 12708
	public Texture2D tex;

	// Token: 0x040031A5 RID: 12709
	[Header("Window Mapping")]
	[Tooltip("The mesh to use to find the window coordinates, cable coordinates")]
	public Mesh captureMesh;

	// Token: 0x040031A6 RID: 12710
	[Tooltip("A map with white blocks mapping the window areas: IMPORTANT: Make sure texture image compression is off & read/write is on.")]
	public Texture2D windowMap;

	// Token: 0x040031A7 RID: 12711
	[Tooltip("A map with red pixels for cable connections and green for external signage: IMPORTANT: Make sure texture image compression is off & read/write is on.")]
	public Texture2D addonMap;

	// Token: 0x040031A8 RID: 12712
	public List<BuildingPreset.WindowUVFloor> sortedWindows = new List<BuildingPreset.WindowUVFloor>();

	// Token: 0x040031A9 RID: 12713
	public int floorCount = 1;

	// Token: 0x040031AA RID: 12714
	public float meshHeight;

	// Token: 0x040031AB RID: 12715
	[Header("Building Addon Points")]
	public List<BuildingPreset.CableLinkPoint> cableLinkPoints = new List<BuildingPreset.CableLinkPoint>();

	// Token: 0x040031AC RID: 12716
	public AnimationCurve cableSpawnChanceOverHeight = AnimationCurve.Linear(0f, 1f, 1f, 0.4f);

	// Token: 0x040031AD RID: 12717
	public List<BuildingPreset.CableLinkPoint> sideSignPoints = new List<BuildingPreset.CableLinkPoint>();

	// Token: 0x040031AE RID: 12718
	[Header("Signage")]
	public List<GameObject> possibleNeonSigns = new List<GameObject>();

	// Token: 0x040031AF RID: 12719
	public Vector2 signsPerBuildingRange = new Vector2(0f, 3f);

	// Token: 0x040031B0 RID: 12720
	[Tooltip("Offset for horizontal lettering signs")]
	public Vector3 horizontalSignOffset = new Vector3(0f, 4.046f, 0f);

	// Token: 0x040031B1 RID: 12721
	[Header("Smokestacks")]
	public bool featuresSmokestack;

	// Token: 0x040031B2 RID: 12722
	[Tooltip("Interval in gametime")]
	[EnableIf("featuresSmokestack")]
	public Vector2 spawnInterval = new Vector2(0.1f, 0.33f);

	// Token: 0x040031B3 RID: 12723
	[EnableIf("featuresSmokestack")]
	public GameObject spritePrefab;

	// Token: 0x040031B4 RID: 12724
	[EnableIf("featuresSmokestack")]
	public Vector3 spawnOffset;

	// Token: 0x040031B5 RID: 12725
	private Vector2[] offsetArrayX4 = new Vector2[]
	{
		new Vector2(0f, -1f),
		new Vector2(-1f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f)
	};

	// Token: 0x020006C8 RID: 1736
	public enum Density
	{
		// Token: 0x040031B7 RID: 12727
		low,
		// Token: 0x040031B8 RID: 12728
		medium,
		// Token: 0x040031B9 RID: 12729
		high,
		// Token: 0x040031BA RID: 12730
		veryHigh
	}

	// Token: 0x020006C9 RID: 1737
	public enum LandValue
	{
		// Token: 0x040031BC RID: 12732
		veryLow,
		// Token: 0x040031BD RID: 12733
		low,
		// Token: 0x040031BE RID: 12734
		medium,
		// Token: 0x040031BF RID: 12735
		high,
		// Token: 0x040031C0 RID: 12736
		veryHigh
	}

	// Token: 0x020006CA RID: 1738
	[Serializable]
	public class InteriorFloorSetting
	{
		// Token: 0x040031C1 RID: 12737
		[Tooltip("This setting will appear for x floors")]
		public int floorsWithThisSetting = 1;

		// Token: 0x040031C2 RID: 12738
		[Tooltip("Possible floor presets (choose at random)")]
		public List<TextAsset> blueprints = new List<TextAsset>();

		// Token: 0x040031C3 RID: 12739
		[Tooltip("How far air vents are allowed to extrude from the outer wall of the building (0 if none)")]
		public int airVentMaximumExtrusion;

		// Token: 0x040031C4 RID: 12740
		[Tooltip("Possible floor variants featuring control rooms (choose at random)")]
		public List<TextAsset> controlRoomVariants = new List<TextAsset>();

		// Token: 0x040031C5 RID: 12741
		[Tooltip("Forces the main building model to be visible on this floor")]
		public bool forceShowModel;

		// Token: 0x040031C6 RID: 12742
		[Tooltip("When player is on this floor, force these model parents to be hidden")]
		public List<string> forceHideModels = new List<string>();

		// Token: 0x040031C7 RID: 12743
		[Tooltip("When the player is on this floor, in this specific room type, force this model parents to be hidden (overrides outside rooms)")]
		public List<BuildingPreset.ForceHideModelsForRoom> forceHideModelsInRooms = new List<BuildingPreset.ForceHideModelsForRoom>();

		// Token: 0x040031C8 RID: 12744
		[Tooltip("When the player is outside on this floor, force this model parents to be hidden")]
		public List<string> forceHideModelsOutside = new List<string>();

		// Token: 0x040031C9 RID: 12745
		public bool overrideCeilingHeight;

		// Token: 0x040031CA RID: 12746
		public int newCeilingHeight = 51;
	}

	// Token: 0x020006CB RID: 1739
	[Serializable]
	public class ForceHideModelsForRoom
	{
		// Token: 0x040031CB RID: 12747
		public RoomConfiguration roomConfig;

		// Token: 0x040031CC RID: 12748
		public List<string> forceHideModels = new List<string>();
	}

	// Token: 0x020006CC RID: 1740
	public enum ZoneType
	{
		// Token: 0x040031CE RID: 12750
		residential,
		// Token: 0x040031CF RID: 12751
		commercial,
		// Token: 0x040031D0 RID: 12752
		industrial,
		// Token: 0x040031D1 RID: 12753
		municipal,
		// Token: 0x040031D2 RID: 12754
		publicProperty,
		// Token: 0x040031D3 RID: 12755
		privateProperty
	}

	// Token: 0x020006CD RID: 1741
	[Serializable]
	public struct CableLinkPoint
	{
		// Token: 0x040031D4 RID: 12756
		public Vector3 localPos;

		// Token: 0x040031D5 RID: 12757
		public Vector3 localRot;
	}

	// Token: 0x020006CE RID: 1742
	[Serializable]
	public class WindowUVFloor
	{
		// Token: 0x040031D6 RID: 12758
		public List<BuildingPreset.WindowUVBlock> front = new List<BuildingPreset.WindowUVBlock>();

		// Token: 0x040031D7 RID: 12759
		public List<BuildingPreset.WindowUVBlock> back = new List<BuildingPreset.WindowUVBlock>();

		// Token: 0x040031D8 RID: 12760
		public List<BuildingPreset.WindowUVBlock> left = new List<BuildingPreset.WindowUVBlock>();

		// Token: 0x040031D9 RID: 12761
		public List<BuildingPreset.WindowUVBlock> right = new List<BuildingPreset.WindowUVBlock>();
	}

	// Token: 0x020006CF RID: 1743
	[Serializable]
	public class WindowUVBlock
	{
		// Token: 0x040031DA RID: 12762
		public Vector2 originPixel;

		// Token: 0x040031DB RID: 12763
		public Vector2 rectSize;

		// Token: 0x040031DC RID: 12764
		public Vector2 centrePixel;

		// Token: 0x040031DD RID: 12765
		public Vector3 localMeshPositionLeft;

		// Token: 0x040031DE RID: 12766
		public Vector3 localMeshPositionRight;

		// Token: 0x040031DF RID: 12767
		[Space(7f)]
		public int floor;

		// Token: 0x040031E0 RID: 12768
		public Vector2 side;

		// Token: 0x040031E1 RID: 12769
		public int horizonal;
	}
}
