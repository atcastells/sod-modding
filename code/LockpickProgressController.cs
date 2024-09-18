using System;
using UnityEngine;

// Token: 0x02000522 RID: 1314
public class LockpickProgressController : MonoBehaviour
{
	// Token: 0x06001C7A RID: 7290 RVA: 0x00196BE9 File Offset: 0x00194DE9
	public void SetBarMax(float val)
	{
		this.barMax = val;
		this.UpdateBar();
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x00196BF8 File Offset: 0x00194DF8
	public void SetAmount(float val)
	{
		if (this.amount != val)
		{
			this.amount = val;
			this.UpdateBar();
			if (this.progress >= 1f && !this.completed)
			{
				this.juice.Pulsate(false, false);
				this.juice.elements[0].originalColour = this.depletedColor;
				this.juice.Flash(1, false, default(Color), 5f);
				this.completed = true;
			}
		}
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x00196C7C File Offset: 0x00194E7C
	public void UpdateBar()
	{
		this.progress = Mathf.Clamp01(this.amount / this.barMax);
		this.bar.sizeDelta = new Vector2(this.progress * (this.rect.sizeDelta.x - 31f), this.bar.sizeDelta.y);
	}

	// Token: 0x040025DA RID: 9690
	public float amount;

	// Token: 0x040025DB RID: 9691
	public float barMax = 1f;

	// Token: 0x040025DC RID: 9692
	public float progress;

	// Token: 0x040025DD RID: 9693
	public RectTransform rect;

	// Token: 0x040025DE RID: 9694
	public RectTransform bar;

	// Token: 0x040025DF RID: 9695
	public JuiceController juice;

	// Token: 0x040025E0 RID: 9696
	public Color depletedColor;

	// Token: 0x040025E1 RID: 9697
	private bool completed;
}
