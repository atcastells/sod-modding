using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000322 RID: 802
[Serializable]
public class SideJobStolenItem : SideJob
{
	// Token: 0x0600122E RID: 4654 RVA: 0x001027EC File Offset: 0x001009EC
	public SideJobStolenItem(JobPreset newPreset, SideJobController.JobPickData newData, bool immediatePost) : base(newPreset, newData, immediatePost)
	{
		this.theftTime = SessionData.Instance.gameTime - Toolbox.Instance.Rand(4f, 12f, false);
		this.SimulateTheft();
		Vector2 vector = Toolbox.Instance.CreateTimeRange(this.theftTime, 0.75f, true, true, 15);
		this.theftTimeFrom = vector.x;
		this.theftTimeTo = vector.y;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00102860 File Offset: 0x00100A60
	public void SimulateTheft()
	{
		Interactable item2 = base.GetItem(JobPreset.JobTag.A);
		if (item2 != null)
		{
			this.stolenItemRoom = item2.node.room.roomID;
			FurnitureLocation furnitureParent = item2.furnitureParent;
			if (furnitureParent != null)
			{
				foreach (Interactable interactable in furnitureParent.integratedInteractables)
				{
					interactable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
					interactable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
					interactable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
					interactable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
					interactable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
				}
			}
			float dirt = Toolbox.Instance.Rand(0.2f, 0.5f, false);
			NewNode.NodeAccess nodeAccess = this.poster.home.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door);
			PathFinder.PathData path = PathFinder.Instance.GetPath(nodeAccess.fromNode, item2.node, this.poster, null);
			if (path != null)
			{
				for (int i = 0; i < path.accessList.Count; i++)
				{
					NewNode.NodeAccess nodeAccess2 = path.accessList[i];
					Vector3 vector = Vector3.zero;
					if (i < path.accessList.Count - 1)
					{
						vector = path.accessList[i + 1].toNode.position - nodeAccess2.toNode.position;
					}
					new GameplayController.Footprint(this.purp, nodeAccess2.toNode.position, Quaternion.LookRotation(vector).eulerAngles, dirt, 0f, nodeAccess2.toNode.room);
				}
				for (int j = path.accessList.Count - 1; j > 0; j--)
				{
					NewNode.NodeAccess nodeAccess3 = path.accessList[j];
					Vector3 vector2 = Vector3.zero;
					if (j > 0)
					{
						vector2 = path.accessList[j - 1].toNode.position - nodeAccess3.toNode.position;
					}
					new GameplayController.Footprint(this.purp, nodeAccess3.toNode.position, Quaternion.LookRotation(vector2).eulerAngles, dirt, 0f, nodeAccess3.toNode.room);
				}
			}
			Toolbox.Instance.RetroactiveSurveillanceAddition(this.purp, this.purp.FindSafeTeleport(this.purp.home, false, true), item2.node, true, null, this.theftTime, 0.04f, ClothesPreset.OutfitCategory.outdoorsCasual);
			FurnitureLocation furnitureLocation;
			NewGameLocation.ObjectPlacement bestSpawnLocation = this.purp.home.GetBestSpawnLocation(item2.preset, false, this.purp, this.purp, null, out furnitureLocation, null, true, 3, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 3, null, false, null, null, null, "", false, false);
			if (bestSpawnLocation != null)
			{
				if (item2.inInventory)
				{
					item2.SetAsNotInventory(bestSpawnLocation.furnParent.anchorNode);
				}
				item2.ConvertToFurnitureSpawnedObject(bestSpawnLocation.furnParent, bestSpawnLocation.location, true, true);
				Game.Log(string.Concat(new string[]
				{
					"Jobs: Successfully relocated ",
					item2.name,
					" to ",
					item2.node.gameLocation.name,
					" (",
					this.purp.name,
					")"
				}), 2);
				item2.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.manualRemoval);
				item2.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.manualRemoval);
				item2.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.manualRemoval);
				using (List<NewNode.NodeAccess>.Enumerator enumerator2 = this.poster.home.entrances.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NewNode.NodeAccess nodeAccess4 = enumerator2.Current;
						if (nodeAccess4.door != null)
						{
							if (nodeAccess4.door.handleInteractable != null)
							{
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
							}
							if (nodeAccess4.door.doorInteractable != null)
							{
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
								nodeAccess4.door.handleInteractable.AddNewDynamicFingerprint(this.purp, Interactable.PrintLife.timed);
							}
						}
					}
					return;
				}
			}
			Game.LogError("Unable to get object placement position", 2);
			return;
		}
		Game.LogError("Unable to setup stolen item as it is not spawned", 2);
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00102D38 File Offset: 0x00100F38
	public override void Complete()
	{
		base.Complete();
		this.ReturnItem();
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x00102D48 File Offset: 0x00100F48
	public void ReturnItem()
	{
		Interactable item = base.GetItem(JobPreset.JobTag.A);
		if (item != null)
		{
			FurnitureLocation furnitureLocation;
			NewGameLocation.ObjectPlacement bestSpawnLocation = this.poster.home.GetBestSpawnLocation(item.preset, false, this.poster, this.poster, null, out furnitureLocation, null, true, 3, InteractablePreset.OwnedPlacementRule.prioritiseOwned, 3, null, false, null, null, null, "", false, false);
			if (bestSpawnLocation != null)
			{
				if (item.inInventory)
				{
					item.SetAsNotInventory(bestSpawnLocation.furnParent.anchorNode);
				}
				item.ConvertToFurnitureSpawnedObject(bestSpawnLocation.furnParent, bestSpawnLocation.location, true, true);
			}
			item.RemoveManuallyCreatedFingerprints();
		}
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00102DD4 File Offset: 0x00100FD4
	public override void DebugDisplayAnswers()
	{
		base.DebugDisplayAnswers();
		Interactable item = base.GetItem(JobPreset.JobTag.A);
		if (item != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Jobs: Stolen item: ",
				item.GetWorldPosition(true).ToString(),
				" at ",
				item.node.gameLocation.name,
				" ",
				item.node.room.name
			}), 2);
		}
	}

	// Token: 0x04001636 RID: 5686
	[Header("Serialized Data")]
	public float theftTime;

	// Token: 0x04001637 RID: 5687
	public float theftTimeFrom;

	// Token: 0x04001638 RID: 5688
	public float theftTimeTo;

	// Token: 0x04001639 RID: 5689
	public int stolenItemRoom;
}
