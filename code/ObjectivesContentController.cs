using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000584 RID: 1412
public class ObjectivesContentController : MonoBehaviour
{
	// Token: 0x06001EDE RID: 7902 RVA: 0x001ACFB0 File Offset: 0x001AB1B0
	public void Setup(WindowContentController newWcc)
	{
		this.wcc = newWcc;
		if (this.wcc.window.passedEvidence != null && this.wcc.window.passedEvidence.interactable != null && this.wcc.window.passedEvidence.interactable.jobParent != null)
		{
			this.job = this.wcc.window.passedEvidence.interactable.jobParent;
		}
		else if (this.wcc.window.passedInteractable != null && this.wcc.window.passedInteractable.jobParent != null)
		{
			this.job = this.wcc.window.passedInteractable.jobParent;
		}
		if (this.job != null)
		{
			this.job.AcquireInfo += this.UpdateJobDetails;
		}
		this.UpdateJobDetails();
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x001AD094 File Offset: 0x001AB294
	private void OnDestroy()
	{
		if (this.job != null)
		{
			this.job.AcquireInfo -= this.UpdateJobDetails;
		}
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x001AD0B8 File Offset: 0x001AB2B8
	public void UpdateJobDetails()
	{
		if (this.job != null && this.job.knowHandInLocation)
		{
			if (this.job.jobInfoDialogMsg == null || this.job.jobInfoDialogMsg.Length <= 0)
			{
				this.jobDetails.text = Strings.Get("ui.interface", "See job note for details.", Strings.Casing.asIs, false, false, false, null);
			}
			else
			{
				this.jobDetails.text = Strings.GetTextForComponent(this.job.jobInfoDialogMsg, this.job, null, null, "\n\n", false, null, Strings.LinkSetting.forceLinks, null);
			}
		}
		else
		{
			this.jobDetails.text = Strings.Get("ui.interface", "Persue current objectives detailed in the job note to acquire more details about the job...", Strings.Casing.asIs, false, false, false, null);
		}
		this.jobDetails.ForceMeshUpdate(false, false);
		this.pageRect.sizeDelta = new Vector2(this.pageRect.sizeDelta.x, Mathf.Max(this.jobDetails.GetPreferredValues().y + 32f, 466f));
		this.pageRect.anchoredPosition = new Vector2(0f, this.pageRect.sizeDelta.y * -0.5f);
	}

	// Token: 0x040028B3 RID: 10419
	[Header("References")]
	public WindowContentController wcc;

	// Token: 0x040028B4 RID: 10420
	public RectTransform pageRect;

	// Token: 0x040028B5 RID: 10421
	public RectTransform objectiveContainer;

	// Token: 0x040028B6 RID: 10422
	public SideJob job;

	// Token: 0x040028B7 RID: 10423
	public TextMeshProUGUI jobDetails;

	// Token: 0x040028B8 RID: 10424
	public List<ObjectiveContentListEntry> spawnedStartingObjectives = new List<ObjectiveContentListEntry>();

	// Token: 0x040028B9 RID: 10425
	[Header("Prefabs")]
	public GameObject elementPrefab;
}
