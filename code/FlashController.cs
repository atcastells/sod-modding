using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000500 RID: 1280
public class FlashController : MonoBehaviour
{
	// Token: 0x06001BA0 RID: 7072 RVA: 0x0018E6AC File Offset: 0x0018C8AC
	private void Start()
	{
		if (this.colourCodeElements.Count <= 0)
		{
			this.colourCodeElements.Add(base.gameObject.GetComponent<Image>());
		}
		if (this.getNormalColourAtStart)
		{
			this.normalColour = this.colourCodeElements[0].color;
		}
	}

	// Token: 0x06001BA1 RID: 7073 RVA: 0x0018E6FC File Offset: 0x0018C8FC
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

	// Token: 0x06001BA2 RID: 7074 RVA: 0x0018E730 File Offset: 0x0018C930
	public IEnumerator FlashColour(int newRepeat)
	{
		this.flashActive = true;
		this.repeat = newRepeat;
		int cycle = 0;
		float progress = 0f;
		while (cycle < this.repeat && progress < 2f)
		{
			progress += this.speed * Time.deltaTime;
			float num;
			if (progress <= 1f)
			{
				num = progress;
			}
			else
			{
				num = 2f - progress;
			}
			for (int i = 0; i < this.colourCodeElements.Count; i++)
			{
				this.colourCodeElements[i].color = Color.Lerp(this.normalColour, this.flashColour, num);
			}
			if (progress >= 2f)
			{
				int num2 = cycle;
				cycle = num2 + 1;
				progress = 0f;
			}
			yield return null;
		}
		for (int j = 0; j < this.colourCodeElements.Count; j++)
		{
			this.colourCodeElements[j].color = this.normalColour;
		}
		this.flashActive = false;
		yield break;
	}

	// Token: 0x06001BA3 RID: 7075 RVA: 0x0018E748 File Offset: 0x0018C948
	private void OnDisable()
	{
		base.StopCoroutine("FlashColour");
		this.flashActive = false;
		this.repeat = 0;
		for (int i = 0; i < this.colourCodeElements.Count; i++)
		{
			this.colourCodeElements[i].color = this.normalColour;
		}
	}

	// Token: 0x04002450 RID: 9296
	public List<Image> colourCodeElements = new List<Image>();

	// Token: 0x04002451 RID: 9297
	public bool getNormalColourAtStart = true;

	// Token: 0x04002452 RID: 9298
	public Color normalColour = Color.white;

	// Token: 0x04002453 RID: 9299
	public Color flashColour = Color.red;

	// Token: 0x04002454 RID: 9300
	public float speed = 10f;

	// Token: 0x04002455 RID: 9301
	private bool flashActive;

	// Token: 0x04002456 RID: 9302
	private int repeat;
}
