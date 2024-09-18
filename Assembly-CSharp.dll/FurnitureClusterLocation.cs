using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003B6 RID: 950
[Serializable]
public class FurnitureClusterLocation : IComparable<FurnitureClusterLocation>
{
	// Token: 0x06001584 RID: 5508 RVA: 0x00139984 File Offset: 0x00137B84
	public FurnitureClusterLocation(NewNode newAnchor, FurnitureCluster newPreset, int newAngle, float newRank)
	{
		this.cluster = newPreset;
		this.anchorNode = newAnchor;
		this.angle = newAngle;
		this.ranking = newRank;
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x001399C0 File Offset: 0x00137BC0
	public void LoadFurnitureToWorld(bool forceSpawnImmediate = false)
	{
		if (!this.loadedGeometry)
		{
			foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in this.clusterObjectMap)
			{
				foreach (FurnitureLocation furniture in keyValuePair.Value)
				{
					ObjectPoolingController.Instance.MarkAsToLoad(furniture, forceSpawnImmediate);
				}
			}
			this.loadedGeometry = true;
			return;
		}
		if (forceSpawnImmediate)
		{
			foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair2 in this.clusterObjectMap)
			{
				foreach (FurnitureLocation furnitureLocation in keyValuePair2.Value)
				{
					foreach (MeshRenderer meshRenderer in furnitureLocation.meshes)
					{
						meshRenderer.enabled = true;
					}
				}
			}
		}
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x00139B24 File Offset: 0x00137D24
	public void UnloadFurniture(bool removeIntegratedInteractables, FurnitureClusterLocation.RemoveInteractablesOption removeSpawnedInteractables)
	{
		if (this.loadedGeometry)
		{
			foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in this.clusterObjectMap)
			{
				foreach (FurnitureLocation furnitureLocation in keyValuePair.Value)
				{
					ObjectPoolingController.Instance.MarkAsNotNeeded(furnitureLocation);
					furnitureLocation.DespawnObject();
					if (removeIntegratedInteractables)
					{
						furnitureLocation.RemoveIntegratedInteractables();
					}
					if (removeSpawnedInteractables == FurnitureClusterLocation.RemoveInteractablesOption.remove)
					{
						furnitureLocation.RemoveSpawnedInteractables();
					}
					else if (removeSpawnedInteractables == FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage)
					{
						for (int i = 0; i < furnitureLocation.spawnedInteractables.Count; i++)
						{
							PlayerApartmentController.Instance.MoveItemToStorage(furnitureLocation.spawnedInteractables[i]);
						}
					}
				}
			}
			this.loadedGeometry = false;
		}
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x00139C20 File Offset: 0x00137E20
	public void DeleteCluster(bool removeIntegratedInteractables, FurnitureClusterLocation.RemoveInteractablesOption removeSpawnedInteractables)
	{
		foreach (FurnitureLocation furnitureLocation in new List<FurnitureLocation>(this.clusterList))
		{
			this.DeleteFurniture(furnitureLocation.id, removeIntegratedInteractables, removeSpawnedInteractables);
		}
		Game.Log("Removing furniture cluster for " + this.anchorNode.room.GetName(), 2);
		this.anchorNode.room.furniture.Remove(this);
		ObjectPoolingController.Instance.UpdateObjectRanges();
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00139CC0 File Offset: 0x00137EC0
	public void DeleteFurniture(int deleteID, bool removeIntegratedInteractables, FurnitureClusterLocation.RemoveInteractablesOption removeSpawnedInteractables)
	{
		foreach (KeyValuePair<NewNode, List<FurnitureLocation>> keyValuePair in this.clusterObjectMap)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				if (keyValuePair.Value[i].id == deleteID)
				{
					ObjectPoolingController.Instance.MarkAsNotNeeded(keyValuePair.Value[i]);
					if (removeIntegratedInteractables && keyValuePair.Value[i] != null)
					{
						keyValuePair.Value[i].RemoveIntegratedInteractables();
					}
					if (removeSpawnedInteractables == FurnitureClusterLocation.RemoveInteractablesOption.remove && keyValuePair.Value[i] != null)
					{
						keyValuePair.Value[i].RemoveSpawnedInteractables();
					}
					else if (removeSpawnedInteractables == FurnitureClusterLocation.RemoveInteractablesOption.moveToStorage && keyValuePair.Value[i] != null)
					{
						for (int j = 0; j < keyValuePair.Value[i].spawnedInteractables.Count; j++)
						{
							PlayerApartmentController.Instance.MoveItemToStorage(keyValuePair.Value[i].spawnedInteractables[j]);
						}
					}
					this.anchorNode.room.individualFurniture.Remove(keyValuePair.Value[i]);
					this.anchorNode.room.decorEdit = true;
					ObjectPoolingController.Instance.MarkAsNotNeeded(keyValuePair.Value[i]);
					keyValuePair.Value[i].DespawnObject();
					keyValuePair.Value.RemoveAt(i);
					i--;
				}
			}
		}
		for (int k = 0; k < this.clusterList.Count; k++)
		{
			if (this.clusterList[k].id == deleteID)
			{
				this.clusterList.RemoveAt(k);
				k--;
			}
		}
		ObjectPoolingController.Instance.UpdateObjectRanges();
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x00139EC8 File Offset: 0x001380C8
	public int CompareTo(FurnitureClusterLocation otherObject)
	{
		return this.ranking.CompareTo(otherObject.ranking);
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x00139EDC File Offset: 0x001380DC
	public CitySaveData.FurnitureClusterCitySave GenerateSaveData()
	{
		CitySaveData.FurnitureClusterCitySave furnitureClusterCitySave = new CitySaveData.FurnitureClusterCitySave();
		furnitureClusterCitySave.cluster = this.cluster.name;
		furnitureClusterCitySave.anchorNode = this.anchorNode.nodeCoord;
		furnitureClusterCitySave.angle = this.angle;
		furnitureClusterCitySave.ranking = this.ranking;
		foreach (FurnitureLocation furnitureLocation in this.clusterList)
		{
			if (!(furnitureLocation.furniture == null))
			{
				CitySaveData.FurnitureClusterObjectCitySave furnitureClusterObjectCitySave = new CitySaveData.FurnitureClusterObjectCitySave();
				furnitureClusterObjectCitySave.id = furnitureLocation.id;
				furnitureClusterObjectCitySave.offset = furnitureLocation.offset;
				furnitureClusterObjectCitySave.furnitureClasses = new List<string>();
				furnitureClusterObjectCitySave.up = furnitureLocation.userPlaced;
				foreach (FurnitureClass furnitureClass in furnitureLocation.furnitureClasses)
				{
					furnitureClusterObjectCitySave.furnitureClasses.Add(furnitureClass.name);
				}
				furnitureClusterObjectCitySave.angle = furnitureLocation.angle;
				furnitureClusterObjectCitySave.anchorNode = furnitureLocation.anchorNode.nodeCoord;
				furnitureClusterObjectCitySave.coversNodes = new List<Vector3Int>();
				foreach (NewNode newNode in furnitureLocation.coversNodes)
				{
					furnitureClusterObjectCitySave.coversNodes.Add(newNode.nodeCoord);
				}
				furnitureClusterObjectCitySave.furniture = furnitureLocation.furniture.name;
				if (furnitureLocation.art != null)
				{
					furnitureClusterObjectCitySave.art = furnitureLocation.art.name;
				}
				furnitureClusterObjectCitySave.matKey = furnitureLocation.matKey;
				furnitureClusterObjectCitySave.artMatKet = furnitureLocation.artMatKey;
				furnitureClusterObjectCitySave.fovDirection = furnitureLocation.fovDirection;
				furnitureClusterObjectCitySave.useFOVBLock = furnitureLocation.useFOVBLock;
				furnitureClusterObjectCitySave.fovMaxDistance = furnitureLocation.fovMaxDistance;
				furnitureClusterObjectCitySave.scale = furnitureLocation.scaleMultiplier;
				foreach (KeyValuePair<FurnitureLocation.OwnerKey, int> keyValuePair in furnitureLocation.ownerMap)
				{
					if (keyValuePair.Key.human != null)
					{
						furnitureClusterObjectCitySave.owners.Add(keyValuePair.Key.human.humanID);
					}
					else if (keyValuePair.Key.address != null)
					{
						furnitureClusterObjectCitySave.owners.Add(-keyValuePair.Key.address.id);
					}
				}
				furnitureClusterCitySave.objs.Add(furnitureClusterObjectCitySave);
			}
		}
		return furnitureClusterCitySave;
	}

	// Token: 0x04001AB4 RID: 6836
	[Header("Cluster Setup")]
	public Dictionary<NewNode, List<FurnitureLocation>> clusterObjectMap = new Dictionary<NewNode, List<FurnitureLocation>>();

	// Token: 0x04001AB5 RID: 6837
	[NonSerialized]
	public List<FurnitureLocation> clusterList = new List<FurnitureLocation>();

	// Token: 0x04001AB6 RID: 6838
	public FurnitureCluster cluster;

	// Token: 0x04001AB7 RID: 6839
	public NewNode anchorNode;

	// Token: 0x04001AB8 RID: 6840
	public int angle;

	// Token: 0x04001AB9 RID: 6841
	public float ranking;

	// Token: 0x04001ABA RID: 6842
	[Header("In-Game")]
	public bool loadedGeometry;

	// Token: 0x020003B7 RID: 951
	public enum RemoveInteractablesOption
	{
		// Token: 0x04001ABC RID: 6844
		keep,
		// Token: 0x04001ABD RID: 6845
		remove,
		// Token: 0x04001ABE RID: 6846
		moveToStorage
	}
}
