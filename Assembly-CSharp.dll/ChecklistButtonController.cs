using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004E4 RID: 1252
public class ChecklistButtonController : ButtonController
{
	// Token: 0x06001B22 RID: 6946 RVA: 0x0018B274 File Offset: 0x00189474
	public void Setup(Objective newObjective)
	{
		this.SetupReferences();
		this.objective = newObjective;
		this.text.text = this.objective.name;
		this.text.ForceMeshUpdate(false, false);
		Vector2 preferredValues = this.text.GetPreferredValues();
		this.rect.sizeDelta = new Vector2(this.rect.sizeDelta.x, Mathf.Max((float)Mathf.CeilToInt(28f + preferredValues.y), 62f));
		this.icon.sprite = this.objective.sprite;
		this.objective.OnProgressChange += this.OnObjectiveProgressChange;
		this.OnObjectiveProgressChange();
		InterfaceController.Instance.objectiveList.Add(this);
	}

	// Token: 0x06001B23 RID: 6947 RVA: 0x0018B33C File Offset: 0x0018953C
	public void OnObjectiveProgressChange()
	{
		InterfaceController.Instance.ActivateObjectivesDisplay();
		if (this.objective.progress >= 1f)
		{
			this.objective.OnProgressChange -= this.OnObjectiveProgressChange;
			if (this.icon != null)
			{
				this.icon.sprite = this.checkedSprite;
			}
			if (this.flash != null)
			{
				this.flash.Flash(2);
			}
		}
		if (this.progressRect != null)
		{
			if (this.objective.progress <= 0f)
			{
				this.progressRect.gameObject.SetActive(false);
			}
			else
			{
				this.progressRect.offsetMax = new Vector2(-6f - (this.rect.rect.width - 12f) * (1f - this.objective.progress), this.progressRect.offsetMax.y);
				this.progressRect.gameObject.SetActive(true);
			}
		}
		CasePanelController.Instance.UpdateResolveNotifications();
	}

	// Token: 0x06001B24 RID: 6948 RVA: 0x0018B457 File Offset: 0x00189657
	public void OnComplete()
	{
		this.Remove();
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x0018B45F File Offset: 0x0018965F
	public void Remove()
	{
		this.fadeOut = true;
	}

	// Token: 0x040023BB RID: 9147
	public Objective objective;

	// Token: 0x040023BC RID: 9148
	public CanvasRenderer bgRend;

	// Token: 0x040023BD RID: 9149
	public CanvasRenderer textRend;

	// Token: 0x040023BE RID: 9150
	public CanvasRenderer progressBGrend;

	// Token: 0x040023BF RID: 9151
	public CanvasRenderer barRend;

	// Token: 0x040023C0 RID: 9152
	public CanvasRenderer iconRend;

	// Token: 0x040023C1 RID: 9153
	public float fadeInProgress;

	// Token: 0x040023C2 RID: 9154
	public bool fadeOut;

	// Token: 0x040023C3 RID: 9155
	public float strikeThroughProgress;

	// Token: 0x040023C4 RID: 9156
	public Vector2 desiredAnchoredPosition;

	// Token: 0x040023C5 RID: 9157
	public Sprite checkedSprite;

	// Token: 0x040023C6 RID: 9158
	public RectTransform progressRect;

	// Token: 0x040023C7 RID: 9159
	public FlashController flash;

	// Token: 0x040023C8 RID: 9160
	public List<CanvasRenderer> childRendereres = new List<CanvasRenderer>();
}
