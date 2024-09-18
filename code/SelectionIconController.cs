using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000530 RID: 1328
public class SelectionIconController : MonoBehaviour
{
	// Token: 0x06001D08 RID: 7432 RVA: 0x0019CD2C File Offset: 0x0019AF2C
	public void Setup(Interactable newInteractable)
	{
		this.interactable = newInteractable;
		if (this.interactable.preset.iconOverride != null)
		{
			this.image.sprite = this.interactable.preset.iconOverride;
		}
		else if (this.interactable.evidence != null)
		{
			this.image.sprite = this.interactable.evidence.GetIcon();
		}
		this.rend.SetAlpha(this.alpha);
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x0019CDB0 File Offset: 0x0019AFB0
	private void Update()
	{
		if (!this.destroy)
		{
			if (this.alpha < 1f)
			{
				this.alpha += Time.deltaTime / 0.1f;
				this.alpha = Mathf.Clamp01(this.alpha);
				this.rend.SetAlpha(this.alpha);
			}
			else if (this.fadeIn)
			{
				this.fadeIn = false;
			}
			if (this.highlighted)
			{
				if (this.highlightProgress < 1f)
				{
					this.highlightProgress += Time.deltaTime / 0.1f;
					this.highlightProgress = Mathf.Clamp01(this.highlightProgress);
					this.image.color = Color.Lerp(this.unHighlightedColor, this.highlightedColor, this.highlightProgress);
					return;
				}
			}
			else if (this.highlightProgress > 0f)
			{
				this.highlightProgress -= Time.deltaTime / 0.1f;
				this.highlightProgress = Mathf.Clamp01(this.highlightProgress);
				this.image.color = Color.Lerp(this.unHighlightedColor, this.highlightedColor, this.highlightProgress);
			}
			return;
		}
		if (this.alpha > 0f)
		{
			this.alpha -= Time.deltaTime / 0.1f;
			this.alpha = Mathf.Clamp01(this.alpha);
			this.rend.SetAlpha(this.alpha);
			return;
		}
		InteractionController.Instance.selectionIcons.Remove(this);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x0019CF3E File Offset: 0x0019B13E
	public void Remove()
	{
		this.destroy = true;
	}

	// Token: 0x06001D0B RID: 7435 RVA: 0x0019CF47 File Offset: 0x0019B147
	public void SetHighlighted(bool val)
	{
		if (this.highlighted != val)
		{
			this.highlighted = val;
		}
	}

	// Token: 0x040026B2 RID: 9906
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x040026B3 RID: 9907
	public CanvasRenderer rend;

	// Token: 0x040026B4 RID: 9908
	public Image image;

	// Token: 0x040026B5 RID: 9909
	[Header("State")]
	public bool highlighted;

	// Token: 0x040026B6 RID: 9910
	public bool fadeIn = true;

	// Token: 0x040026B7 RID: 9911
	public bool destroy;

	// Token: 0x040026B8 RID: 9912
	public float alpha;

	// Token: 0x040026B9 RID: 9913
	public Interactable interactable;

	// Token: 0x040026BA RID: 9914
	public float highlightProgress;

	// Token: 0x040026BB RID: 9915
	[Header("Settings")]
	public Color highlightedColor = Color.white;

	// Token: 0x040026BC RID: 9916
	public Color unHighlightedColor = Color.white;
}
