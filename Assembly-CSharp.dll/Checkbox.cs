using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004E3 RID: 1251
public class Checkbox : MonoBehaviour
{
	// Token: 0x06001B1D RID: 6941 RVA: 0x0018B1F0 File Offset: 0x001893F0
	private void Awake()
	{
		this.Set(this.ticked);
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x0018B1FE File Offset: 0x001893FE
	public void Toggle()
	{
		if (this.ticked)
		{
			this.ticked = false;
		}
		else
		{
			this.ticked = true;
			if (this.orCheckbox != null)
			{
				this.orCheckbox.Set(false);
			}
		}
		this.SetImage();
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x0018B238 File Offset: 0x00189438
	public void Set(bool setTo)
	{
		this.ticked = setTo;
		this.SetImage();
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x0018B247 File Offset: 0x00189447
	public void SetImage()
	{
		if (this.ticked)
		{
			this.graphic.sprite = this.tickedSprite;
			return;
		}
		this.graphic.sprite = this.unTickedSprite;
	}

	// Token: 0x040023B6 RID: 9142
	public bool ticked;

	// Token: 0x040023B7 RID: 9143
	public Image graphic;

	// Token: 0x040023B8 RID: 9144
	public Sprite unTickedSprite;

	// Token: 0x040023B9 RID: 9145
	public Sprite tickedSprite;

	// Token: 0x040023BA RID: 9146
	public Checkbox orCheckbox;
}
