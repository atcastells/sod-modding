using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class FurnitureClusterEditor : MonoBehaviour
{
	// Token: 0x06000BB3 RID: 2995 RVA: 0x000AA718 File Offset: 0x000A8918
	[Button(null, 0)]
	public void ScanTilesForFurniture()
	{
		this.ClearClusterList();
		this.clusterElements.Clear();
		object[] array = AssetLoader.Instance.GetAllFurniture().ToArray();
		object[] array2 = array;
		List<FurniturePreset> list = new List<FurniturePreset>();
		foreach (FurniturePreset furniturePreset in array2)
		{
			list.Add(furniturePreset);
		}
		Transform[] componentsInChildren = this.furnitureParent.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = componentsInChildren[i];
			string furnitureModelName = transform.name;
			if (furnitureModelName.Length > 7 && furnitureModelName.Substring(furnitureModelName.Length - 7) == "(Clone)")
			{
				furnitureModelName = furnitureModelName.Substring(0, furnitureModelName.Length - 7);
			}
			FurniturePreset furniturePreset2 = list.Find((FurniturePreset item) => item.prefab.name == furnitureModelName);
			if (furniturePreset2 != null)
			{
				WalkableRecorder.TileSetup tileSetup = null;
				float num = float.PositiveInfinity;
				foreach (WalkableRecorder.TileSetup tileSetup2 in this.tiles)
				{
					float num2 = Vector3.Distance(tileSetup2.trans.position, transform.transform.position);
					if (num2 < num)
					{
						num = num2;
						tileSetup = tileSetup2;
					}
				}
				if (tileSetup != null)
				{
					FurnitureCluster.FurnitureFacing facingForFurnitureAngle = this.GetFacingForFurnitureAngle(transform.transform.eulerAngles.y);
					string[] array3 = new string[8];
					array3[0] = "Found furniture ";
					array3[1] = furniturePreset2.name;
					array3[2] = " (";
					array3[3] = furniturePreset2.classes[0].name;
					array3[4] = ") at ";
					int num3 = 5;
					Vector2 offset = tileSetup.offset;
					array3[num3] = offset.ToString();
					array3[6] = " facing ";
					array3[7] = facingForFurnitureAngle.ToString();
					Debug.Log(string.Concat(array3));
					transform.transform.position = tileSetup.trans.position;
					transform.transform.eulerAngles = new Vector3(0f, (float)this.GetAngleForFurnitureFacing(facingForFurnitureAngle), 0f);
					ClusterEditorFurniture clusterEditorFurniture = transform.gameObject.GetComponent<ClusterEditorFurniture>();
					if (clusterEditorFurniture == null)
					{
						clusterEditorFurniture = transform.gameObject.AddComponent<ClusterEditorFurniture>();
					}
					clusterEditorFurniture.Setup(furniturePreset2);
					this.spawnedFurniture.Add(clusterEditorFurniture);
					FurnitureCluster.FurnitureClusterRule furnitureClusterRule = new FurnitureCluster.FurnitureClusterRule();
					furnitureClusterRule.placements.Add(tileSetup.offset);
					furnitureClusterRule.furnitureClass = clusterEditorFurniture.furnClass;
					furnitureClusterRule.facing = facingForFurnitureAngle;
					furnitureClusterRule.chanceOfPlacementAttempt = 1f;
					furnitureClusterRule.importantToCluster = true;
					furnitureClusterRule.placementScoreBoost = 1;
					furnitureClusterRule.useFovBlock = false;
					furnitureClusterRule.localScale = transform.transform.localScale;
					this.clusterElements.Add(furnitureClusterRule);
				}
			}
		}
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x000AAA20 File Offset: 0x000A8C20
	[Button(null, 0)]
	public void SpawnAlternateFurniture()
	{
		this.ClearAllFurniture();
		for (int i = 0; i < this.clusterElements.Count; i++)
		{
			FurnitureCluster.FurnitureClusterRule furnitureClusterRule = this.clusterElements[i];
			Vector2 v = furnitureClusterRule.placements[0];
			Vector2 placementPos = this.RotateVector2CW(v, 0f);
			int angleForFurnitureFacing = this.GetAngleForFurnitureFacing(furnitureClusterRule.facing);
			FurniturePreset randomFurnitureForElement = this.GetRandomFurnitureForElement(furnitureClusterRule);
			if (randomFurnitureForElement != null)
			{
				WalkableRecorder.TileSetup tileSetup = this.tiles.Find((WalkableRecorder.TileSetup item) => item.offset == placementPos);
				if (tileSetup != null)
				{
					Transform trans = tileSetup.trans;
					if (trans != null)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(randomFurnitureForElement.prefab, this.furnitureParent);
						gameObject.transform.position = trans.position;
						if (furnitureClusterRule.furnitureClass.ceilingPiece)
						{
							gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 4.2000003f, gameObject.transform.localPosition.z);
						}
						gameObject.transform.localEulerAngles = new Vector3(0f, (float)angleForFurnitureFacing, 0f);
						Vector3 localScale = randomFurnitureForElement.prefab.transform.localScale;
						gameObject.transform.localScale = new Vector3(localScale.x * furnitureClusterRule.localScale.x, localScale.y * furnitureClusterRule.localScale.y, localScale.z * furnitureClusterRule.localScale.z);
						ClusterEditorFurniture clusterEditorFurniture = gameObject.AddComponent<ClusterEditorFurniture>();
						clusterEditorFurniture.Setup(randomFurnitureForElement);
						this.spawnedFurniture.Add(clusterEditorFurniture);
					}
				}
			}
		}
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x000AABE4 File Offset: 0x000A8DE4
	[Button(null, 0)]
	public void LoadCluster()
	{
		if (this.cluster != null)
		{
			this.ClearAllFurniture();
			this.ClearClusterList();
			this.clusterElements = new List<FurnitureCluster.FurnitureClusterRule>(this.cluster.clusterElements);
			this.SpawnAlternateFurniture();
		}
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x000AAC1C File Offset: 0x000A8E1C
	private Vector2 RotateVector2CW(Vector2 v, float degrees)
	{
		float num = Mathf.Sin(-degrees * 0.017453292f);
		float num2 = Mathf.Cos(-degrees * 0.017453292f);
		float x = v.x;
		float y = v.y;
		v.x = num2 * x - num * y;
		v.y = num * x + num2 * y;
		return v;
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x000AAC70 File Offset: 0x000A8E70
	private FurniturePreset GetRandomFurnitureForElement(FurnitureCluster.FurnitureClusterRule inputElement)
	{
		object[] array = AssetLoader.Instance.GetAllFurniture().ToArray();
		object[] array2 = array;
		FurniturePreset result = null;
		List<FurniturePreset> list = new List<FurniturePreset>();
		foreach (FurniturePreset furniturePreset in array2)
		{
			if (furniturePreset.classes.Contains(inputElement.furnitureClass))
			{
				list.Add(furniturePreset);
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x000AACE8 File Offset: 0x000A8EE8
	[Button(null, 0)]
	public void SaveToCluster()
	{
		if (this.cluster != null)
		{
			this.cluster.clusterElements = new List<FurnitureCluster.FurnitureClusterRule>(this.clusterElements);
			return;
		}
		Debug.Log("No cluster selected!");
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x000AAD1C File Offset: 0x000A8F1C
	[Button(null, 0)]
	public void ClearAllFurniture()
	{
		while (this.spawnedFurniture.Count > 0)
		{
			if (this.spawnedFurniture[0] != null)
			{
				Object.DestroyImmediate(this.spawnedFurniture[0].gameObject);
			}
			this.spawnedFurniture.RemoveAt(0);
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x000AAD6F File Offset: 0x000A8F6F
	[Button(null, 0)]
	public void ClearClusterList()
	{
		this.clusterElements.Clear();
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x000AAD7C File Offset: 0x000A8F7C
	private int GetAngleForFurnitureFacing(FurnitureCluster.FurnitureFacing facing)
	{
		if (facing == FurnitureCluster.FurnitureFacing.up)
		{
			return 0;
		}
		if (facing == FurnitureCluster.FurnitureFacing.down)
		{
			return 180;
		}
		if (facing == FurnitureCluster.FurnitureFacing.left)
		{
			return 270;
		}
		if (facing == FurnitureCluster.FurnitureFacing.right)
		{
			return 90;
		}
		return 0;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x000AADA0 File Offset: 0x000A8FA0
	private FurnitureCluster.FurnitureFacing GetFacingForFurnitureAngle(float angle)
	{
		while (angle > 360f)
		{
			angle -= 360f;
		}
		while (angle < 0f)
		{
			angle += 360f;
		}
		int num = Mathf.RoundToInt(angle / 90f) * 90;
		if (num == 0)
		{
			return FurnitureCluster.FurnitureFacing.up;
		}
		if (num == 90)
		{
			return FurnitureCluster.FurnitureFacing.right;
		}
		if (num == 180)
		{
			return FurnitureCluster.FurnitureFacing.down;
		}
		if (num == 270)
		{
			return FurnitureCluster.FurnitureFacing.left;
		}
		return FurnitureCluster.FurnitureFacing.up;
	}

	// Token: 0x04000D18 RID: 3352
	[Header("Current")]
	public FurnitureCluster cluster;

	// Token: 0x04000D19 RID: 3353
	public List<FurnitureCluster.FurnitureClusterRule> clusterElements = new List<FurnitureCluster.FurnitureClusterRule>();

	// Token: 0x04000D1A RID: 3354
	[Header("Components")]
	public Transform furnitureParent;

	// Token: 0x04000D1B RID: 3355
	public List<WalkableRecorder.TileSetup> tiles = new List<WalkableRecorder.TileSetup>();

	// Token: 0x04000D1C RID: 3356
	public List<ClusterEditorFurniture> spawnedFurniture = new List<ClusterEditorFurniture>();
}
