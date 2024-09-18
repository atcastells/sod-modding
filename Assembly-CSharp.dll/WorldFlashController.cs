using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class WorldFlashController : MonoBehaviour
{
	// Token: 0x06001A73 RID: 6771 RVA: 0x001859C2 File Offset: 0x00183BC2
	public void Flash(int newRepeat)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.flashActive)
		{
			this.repeat += newRepeat;
			return;
		}
		base.StartCoroutine("FlashColour", newRepeat);
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x001859F6 File Offset: 0x00183BF6
	public IEnumerator FlashColour(int newRepeat)
	{
		this.flashActive = true;
		this.repeat = newRepeat;
		int cycle = 0;
		float progress = 0f;
		while (cycle < this.repeat && progress < 2f)
		{
			progress += this.speed * Time.deltaTime;
			if (progress < 1f)
			{
				this.rend.material = this.onMaterial;
			}
			else
			{
				this.rend.material = this.offMaterial;
			}
			if (progress >= 2f)
			{
				int num = cycle;
				cycle = num + 1;
				progress = 0f;
			}
			yield return null;
		}
		if (this.controller.interactable.locked && this.controller.interactable.preset.lockOnMaterial != null)
		{
			this.rend.material = this.controller.interactable.preset.lockOnMaterial;
		}
		else if (!this.controller.interactable.locked && this.controller.interactable.preset.lockOffMaterial != null)
		{
			this.rend.material = this.controller.interactable.preset.lockOffMaterial;
		}
		this.flashActive = false;
		yield break;
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x00185A0C File Offset: 0x00183C0C
	private void OnDisable()
	{
		if (this.flashActive)
		{
			base.StopCoroutine("FlashColour");
			this.flashActive = false;
			this.repeat = 0;
			if (this.controller.interactable.locked && this.controller.interactable.preset.lockOnMaterial != null)
			{
				this.rend.material = this.controller.interactable.preset.lockOnMaterial;
				return;
			}
			if (!this.controller.interactable.locked && this.controller.interactable.preset.lockOffMaterial != null)
			{
				this.rend.material = this.controller.interactable.preset.lockOffMaterial;
			}
		}
	}

	// Token: 0x0400231F RID: 8991
	public InteractableController controller;

	// Token: 0x04002320 RID: 8992
	public MeshRenderer rend;

	// Token: 0x04002321 RID: 8993
	public Material offMaterial;

	// Token: 0x04002322 RID: 8994
	public Material onMaterial;

	// Token: 0x04002323 RID: 8995
	public float speed = 10f;

	// Token: 0x04002324 RID: 8996
	public bool flashActive;

	// Token: 0x04002325 RID: 8997
	private int repeat;
}
