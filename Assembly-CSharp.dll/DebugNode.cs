using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class DebugNode : MonoBehaviour
{
	// Token: 0x06000BA2 RID: 2978 RVA: 0x000AA0B1 File Offset: 0x000A82B1
	public void Setup(NewNode newNode)
	{
		this.node = newNode;
		this.RefreshData();
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x000AA0C0 File Offset: 0x000A82C0
	[Button(null, 0)]
	public void RefreshData()
	{
		this.coordinate = this.node.nodeCoord;
		this.tileCoordinate = this.node.tile.globalTileCoord;
		this.localTileCoordinate = this.node.localTileCoord;
		this.isConnected = this.node.isConnected;
		this.accessToOtherNodes = new List<NewNode.NodeAccess>(this.node.accessToOtherNodes.Values);
		this.upperStairwellLink = this.node.stairwellUpperLink;
		this.lowerStairwellLink = this.node.stairwellLowerLink;
		this.isTileStairwell = this.node.tile.isStairwell;
		this.isTileInvertedStairwell = this.node.tile.isInvertedStairwell;
		this.floorType = this.node.floorType;
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x000AA19C File Offset: 0x000A839C
	[Button(null, 0)]
	public void ToggleDisplayConnections()
	{
		while (this.spawnedConnections.Count > 0)
		{
			Object.Destroy(this.spawnedConnections[0]);
			this.spawnedConnections.RemoveAt(0);
		}
		if (!this.displaySpawnedConnections)
		{
			foreach (NewNode.NodeAccess nodeAccess in this.accessToOtherNodes)
			{
				GameObject gameObject = GameObject.CreatePrimitive(3);
				gameObject.transform.SetParent(base.transform, true);
				gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				gameObject.transform.position = nodeAccess.toNode.position + new Vector3(0f, 0.5f, 0f);
				gameObject.name = nodeAccess.name;
				this.spawnedConnections.Add(gameObject);
			}
			this.displaySpawnedConnections = true;
			return;
		}
		this.displaySpawnedConnections = false;
	}

	// Token: 0x04000CE1 RID: 3297
	public NewNode node;

	// Token: 0x04000CE2 RID: 3298
	public Vector3 coordinate;

	// Token: 0x04000CE3 RID: 3299
	public Vector3 tileCoordinate;

	// Token: 0x04000CE4 RID: 3300
	public Vector2Int localTileCoordinate;

	// Token: 0x04000CE5 RID: 3301
	public bool isConnected;

	// Token: 0x04000CE6 RID: 3302
	public List<NewNode.NodeAccess> accessToOtherNodes = new List<NewNode.NodeAccess>();

	// Token: 0x04000CE7 RID: 3303
	public bool upperStairwellLink;

	// Token: 0x04000CE8 RID: 3304
	public bool lowerStairwellLink;

	// Token: 0x04000CE9 RID: 3305
	public bool isTileStairwell;

	// Token: 0x04000CEA RID: 3306
	public bool isTileInvertedStairwell;

	// Token: 0x04000CEB RID: 3307
	public NewNode.FloorTileType floorType;

	// Token: 0x04000CEC RID: 3308
	private bool displaySpawnedConnections;

	// Token: 0x04000CED RID: 3309
	public List<GameObject> spawnedConnections = new List<GameObject>();
}
