using System;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class RagdollPositionUpdater : MonoBehaviour
{
	// Token: 0x06000FA4 RID: 4004 RVA: 0x000DE282 File Offset: 0x000DC482
	public void Setup(NewAIController newHuman)
	{
		this.ai = newHuman;
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x000DE28C File Offset: 0x000DC48C
	private void Update()
	{
		if (this.ai == null || this.ai.human.outfitController == null)
		{
			return;
		}
		Transform bodyAnchor = this.ai.human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.lowerTorso);
		Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(bodyAnchor.position + new Vector3(0f, 0.15f, 0f));
		NewNode newNode = null;
		PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode);
		if (newNode != this.ai.human.currentNode)
		{
			this.ai.SetParentPositionToRagdollLimbPosition();
		}
		if (bodyAnchor.position.y >= 8f && this.freeFallForceTimer <= 0f)
		{
			if (InteractionController.Instance.currentlyDragging != this.ai.dragController && (this.ai.human.currentNode.floorType == NewNode.FloorTileType.CeilingOnly || this.ai.human.currentNode.floorType == NewNode.FloorTileType.noneButIndoors || this.ai.human.currentNode.floorType == NewNode.FloorTileType.none) && this.ai.human.currentRoom.isOutsideWindow)
			{
				Vector2Int[] offsetArrayX = CityData.Instance.offsetArrayX4;
				int i = 0;
				while (i < offsetArrayX.Length)
				{
					Vector2Int vector2Int = offsetArrayX[i];
					Vector3Int vector3Int2 = this.ai.human.currentNode.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
					NewNode newNode2 = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int2, ref newNode2) && !newNode2.room.IsOutside() && !newNode2.room.isOutsideWindow)
					{
						Vector3 vector = this.ai.human.currentNode.position - newNode2.position;
						if (this.ai.human.animationController != null && this.ai.human.animationController.upperTorsoRB)
						{
							Vector3 vector2 = vector.normalized * 3000f;
							string text = "Adding force ";
							Vector3 vector3 = vector2;
							Game.Log(text + vector3.ToString() + " to ragdoll...", 2);
							this.ai.human.animationController.upperTorsoRB.AddForce(vector2, 0);
							this.freeFallForceTimer = 0.5f;
							return;
						}
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
		}
		else if (this.freeFallForceTimer > 0f)
		{
			this.freeFallForceTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0400132E RID: 4910
	[Tooltip("Reference to the AI controller object attached to the citizen")]
	[Header("References")]
	public NewAIController ai;

	// Token: 0x0400132F RID: 4911
	public float freeFallForceTimer;
}
