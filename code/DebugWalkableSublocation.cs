using System;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class DebugWalkableSublocation : MonoBehaviour
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x000AA521 File Offset: 0x000A8721
	public void Setup(NewNode newNode, NewNode.NodeSpace newSpace)
	{
		this.node = newNode;
		this.space = newSpace;
		base.enabled = true;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x000AA538 File Offset: 0x000A8738
	private void Update()
	{
		if (this.node != null && this.space != null)
		{
			if (this.space.occ == NewNode.NodeSpaceOccupancy.empty)
			{
				this.rend.sharedMaterial = this.unoccupiedMat;
				return;
			}
			if (this.space.occ == NewNode.NodeSpaceOccupancy.reserved)
			{
				this.rend.sharedMaterial = this.occupiedDestinationMat;
				return;
			}
			if (this.space.occ == NewNode.NodeSpaceOccupancy.position)
			{
				this.rend.sharedMaterial = this.occupiedActualMat;
			}
		}
	}

	// Token: 0x04000CFC RID: 3324
	public NewNode node;

	// Token: 0x04000CFD RID: 3325
	public MeshRenderer rend;

	// Token: 0x04000CFE RID: 3326
	public Material unoccupiedMat;

	// Token: 0x04000CFF RID: 3327
	public Material occupiedActualMat;

	// Token: 0x04000D00 RID: 3328
	public Material occupiedDestinationMat;

	// Token: 0x04000D01 RID: 3329
	public NewNode.NodeSpace space;
}
