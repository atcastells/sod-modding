using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004BE RID: 1214
public class OutlineController : MonoBehaviour
{
	// Token: 0x06001A47 RID: 6727 RVA: 0x00182A09 File Offset: 0x00180C09
	public void Setup()
	{
		this.outlineActive = false;
		this.isSetup = true;
	}

	// Token: 0x06001A48 RID: 6728 RVA: 0x00182A1C File Offset: 0x00180C1C
	public void SetOutlineActive(bool val)
	{
		this.outlineActive = val;
		foreach (MeshRenderer meshRenderer in this.meshesToOutline)
		{
			if (!(meshRenderer == null))
			{
				if (this.outlineActive)
				{
					meshRenderer.gameObject.layer = 30;
				}
				else
				{
					meshRenderer.gameObject.layer = this.normalLayer;
				}
			}
		}
		if (this.outlineActive && !this.actor.visible)
		{
			this.actor.SetVisible(true, false);
		}
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x00182AC4 File Offset: 0x00180CC4
	public void SetColor(Color newCol)
	{
		foreach (MeshRenderer meshRenderer in this.meshesToOutline)
		{
			if (!(meshRenderer == null))
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				meshRenderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetColor("_SelectionColor", newCol);
				meshRenderer.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x00182B3C File Offset: 0x00180D3C
	public void SetAlpha(float val)
	{
		foreach (MeshRenderer meshRenderer in this.meshesToOutline)
		{
			if (!(meshRenderer == null))
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				meshRenderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat("_AlphaVal", val);
				meshRenderer.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}

	// Token: 0x040022D7 RID: 8919
	public int normalLayer = 24;

	// Token: 0x040022D8 RID: 8920
	public Actor actor;

	// Token: 0x040022D9 RID: 8921
	public List<MeshRenderer> meshesToOutline = new List<MeshRenderer>();

	// Token: 0x040022DA RID: 8922
	public bool outlineActive;

	// Token: 0x040022DB RID: 8923
	public bool isSetup;
}
