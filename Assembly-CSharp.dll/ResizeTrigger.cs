using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x02000031 RID: 49
public class ResizeTrigger : MonoBehaviour
{
	// Token: 0x060001C6 RID: 454 RVA: 0x0000E743 File Offset: 0x0000C943
	public void TriggerGraffitiChecks()
	{
		base.StartCoroutine(this.DoResizeTrigger());
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000E752 File Offset: 0x0000C952
	private IEnumerator DoResizeTrigger()
	{
		Game.Log("Start is triggered", 2);
		Vector3 size = this.decal.size;
		bool obscured;
		Texture texture;
		do
		{
			this.maxResizeTimes--;
			obscured = false;
			yield return new WaitForSeconds(5f);
			base.gameObject.GetComponent<BoxCollider>().size = new Vector3(this.decal.size.x * (1f + this.hitboxSizeModifier), this.decal.size.y * (1f + this.hitboxSizeModifier), 0.75f);
			foreach (Collider collider in Physics.OverlapBox(base.transform.position, base.transform.GetComponent<BoxCollider>().size / 2f, base.transform.rotation, this.layerMask))
			{
				if ((this.layerMask & 1 << collider.gameObject.layer) != 0)
				{
					obscured = true;
					break;
				}
				Game.Log("Grafitti being obscured is " + obscured.ToString(), 2);
			}
			if (!obscured)
			{
				goto IL_30E;
			}
			Game.Log("tries to resize", 2);
			texture = this.decal.material.GetTexture("_BaseColorMap");
			this.decal.size = new Vector3((float)texture.width * this.pixelScaleMultiplier, (float)texture.height * this.pixelScaleMultiplier, 0.13f);
		}
		while (this.maxResizeTimes > 0);
		for (int j = 0; j < this.maxResizeTimes; j++)
		{
			this.decal.size = new Vector3((float)texture.width / this.pixelScaleMultiplier, (float)texture.height / this.pixelScaleMultiplier, 0.13f);
		}
		for (int k = 0; k < this.maxRepositionTimes; k++)
		{
			float num = this.maxRepositionDistance / (float)this.maxRepositionTimes * (float)k;
			Vector3 vector;
			vector..ctor(Random.Range(-num, num), Random.Range(-num, num), 0f);
			foreach (Collider collider2 in Physics.OverlapBox(base.transform.position + vector, base.transform.GetComponent<BoxCollider>().size / 2f, base.transform.rotation, this.layerMask))
			{
				if ((this.layerMask & 1 << collider2.gameObject.layer) != 0)
				{
					base.transform.position + vector;
					break;
				}
				Game.Log("Grafitti being obscured is " + obscured.ToString(), 2);
			}
		}
		Vector3 position = base.transform.position;
		IL_30E:
		yield break;
	}

	// Token: 0x0400010D RID: 269
	public LayerMask layerMask;

	// Token: 0x0400010E RID: 270
	public DecalProjector decal;

	// Token: 0x0400010F RID: 271
	[Range(-0.1f, 0.2f)]
	public float hitboxSizeModifier;

	// Token: 0x04000110 RID: 272
	public float pixelScaleMultiplier = 0.1f;

	// Token: 0x04000111 RID: 273
	[Range(0f, 3000f)]
	public int maxResizeTimes = 50;

	// Token: 0x04000112 RID: 274
	[Range(0f, 3000f)]
	public int maxRepositionTimes = 100;

	// Token: 0x04000113 RID: 275
	[Range(0f, 10f)]
	public float maxRepositionDistance = 3f;
}
