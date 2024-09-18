using System;
using UnityEngine;

// Token: 0x020001F5 RID: 501
public class ClusterEditorFurniture : MonoBehaviour
{
	// Token: 0x06000B93 RID: 2963 RVA: 0x000A9C89 File Offset: 0x000A7E89
	public void Setup(FurniturePreset newFurn)
	{
		this.furnPreset = newFurn;
		this.furnClass = this.furnPreset.classes[0];
	}

	// Token: 0x04000CC3 RID: 3267
	public FurniturePreset furnPreset;

	// Token: 0x04000CC4 RID: 3268
	public FurnitureClass furnClass;
}
