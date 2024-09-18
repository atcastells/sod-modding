using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005C5 RID: 1477
public class ProgressBarPipController : ButtonController
{
	// Token: 0x0600205F RID: 8287 RVA: 0x001BC321 File Offset: 0x001BA521
	private void Awake()
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.button = base.gameObject.GetComponent<Button>();
		this.img = base.gameObject.GetComponent<Image>();
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x001BC356 File Offset: 0x001BA556
	public override void OnHoverStart()
	{
		if (this.button.interactable)
		{
			this.bar.hoverOverPip = this;
		}
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x001BC371 File Offset: 0x001BA571
	public override void OnHoverEnd()
	{
		this.bar.hoverOverPip = null;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x001BC380 File Offset: 0x001BA580
	public void SetFilled(bool newVal, bool secondaryFilled)
	{
		this.filled = newVal;
		if (this.filled)
		{
			this.img.color = this.filledColour;
			if (this.button != null)
			{
				this.button.interactable = true;
				this.button.image.raycastTarget = true;
			}
		}
		else if (secondaryFilled)
		{
			this.img.color = this.secondaryColour;
			if (this.button != null)
			{
				this.button.interactable = true;
				this.button.image.raycastTarget = true;
			}
		}
		else
		{
			this.img.color = this.unfilledColour;
			if (this.button != null)
			{
				this.button.interactable = false;
				this.button.image.raycastTarget = false;
			}
		}
		this.UpdateAdditionalHighlight();
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x001BC460 File Offset: 0x001BA660
	public int GetPipNumber()
	{
		return this.bar.pips.FindIndex((ProgressBarPipController item) => item == this);
	}

	// Token: 0x04002A74 RID: 10868
	public Image img;

	// Token: 0x04002A75 RID: 10869
	public Color unfilledColour = Color.grey;

	// Token: 0x04002A76 RID: 10870
	public Color filledColour = Color.cyan;

	// Token: 0x04002A77 RID: 10871
	public Color secondaryColour = Color.yellow;

	// Token: 0x04002A78 RID: 10872
	public ProgressBarController bar;

	// Token: 0x04002A79 RID: 10873
	public bool filled;

	// Token: 0x04002A7A RID: 10874
	public bool secondaryFilled;
}
