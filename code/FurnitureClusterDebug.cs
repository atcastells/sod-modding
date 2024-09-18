using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class FurnitureClusterDebug : MonoBehaviour
{
	// Token: 0x06000BAF RID: 2991 RVA: 0x000AA64C File Offset: 0x000A884C
	public void Setup(FurnitureCluster newCluster, NewNode newNode)
	{
		this.node = newNode;
		base.transform.position = CityData.Instance.NodeToRealpos(this.node.nodeCoord);
		this.cluster = newCluster;
		FurnitureCluster furnitureCluster = this.cluster;
		string text = (furnitureCluster != null) ? furnitureCluster.ToString() : null;
		string text2 = " @ ";
		Vector3Int nodeCoord = this.node.nodeCoord;
		base.name = text + text2 + nodeCoord.ToString();
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x000AA6C7 File Offset: 0x000A88C7
	public void AddEntry(FurnitureClusterDebug.DebugFurnitureAnglePlacement newEntry)
	{
		this.entries.Add(newEntry);
		if (newEntry.isValid)
		{
			base.name = "*" + base.name;
			this.rend.material = this.validMaterial;
		}
	}

	// Token: 0x04000D0C RID: 3340
	public MeshRenderer rend;

	// Token: 0x04000D0D RID: 3341
	public FurnitureCluster cluster;

	// Token: 0x04000D0E RID: 3342
	public NewNode node;

	// Token: 0x04000D0F RID: 3343
	public List<FurnitureClusterDebug.DebugFurnitureAnglePlacement> entries = new List<FurnitureClusterDebug.DebugFurnitureAnglePlacement>();

	// Token: 0x04000D10 RID: 3344
	public Material validMaterial;

	// Token: 0x04000D11 RID: 3345
	public Material invalidMaterial;

	// Token: 0x02000200 RID: 512
	[Serializable]
	public class DebugFurnitureAnglePlacement
	{
		// Token: 0x04000D12 RID: 3346
		public string name;

		// Token: 0x04000D13 RID: 3347
		public int angle;

		// Token: 0x04000D14 RID: 3348
		public bool isValid;

		// Token: 0x04000D15 RID: 3349
		public List<NewNode> coversNodes;

		// Token: 0x04000D16 RID: 3350
		public List<string> log;

		// Token: 0x04000D17 RID: 3351
		[Space(7f)]
		public List<string> pathingLog;
	}
}
