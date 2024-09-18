using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005F7 RID: 1527
public class ResolveOptionsController : MonoBehaviour
{
	// Token: 0x060021A2 RID: 8610 RVA: 0x001C9724 File Offset: 0x001C7924
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(740f, 648f));
		this.titleText.text = Strings.Get("ui.interface", "Case Options", Strings.Casing.asIs, false, false, false, null);
		if (this.wcc != null && this.wcc.window != null)
		{
			ResolveController componentInChildren = this.wcc.window.GetComponentInChildren<ResolveController>(true);
			if (componentInChildren != null)
			{
				this.submitButton.SetInteractable(componentInChildren.submitButton.interactable);
			}
		}
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x001C97D0 File Offset: 0x001C79D0
	private void OnEnable()
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.job != null && CasePanelController.Instance.activeCase.job.post != null)
		{
			this.openJobPostButton.SetInteractable(true);
		}
		else
		{
			this.openJobPostButton.SetInteractable(false);
		}
		if (this.wcc != null && this.wcc.window != null)
		{
			ResolveController componentInChildren = this.wcc.window.GetComponentInChildren<ResolveController>(true);
			if (componentInChildren != null)
			{
				this.submitButton.SetInteractable(componentInChildren.submitButton.interactable);
			}
		}
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x001C987C File Offset: 0x001C7A7C
	public void HelpButton()
	{
		InterfaceController.Instance.OpenNotebookNoPause("", true);
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x001C9890 File Offset: 0x001C7A90
	public void OpenJobPostButton()
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.job != null && CasePanelController.Instance.activeCase.job.post != null)
		{
			InterfaceController instance = InterfaceController.Instance;
			Evidence evidence = CasePanelController.Instance.activeCase.job.post.evidence;
			Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name;
			List<Evidence.DataKey> passedEvidenceKeys = null;
			string presetName = "";
			bool worldInteraction = false;
			bool autoPosition = true;
			Interactable post = CasePanelController.Instance.activeCase.job.post;
			instance.SpawnWindow(evidence, passedEvidenceKey, passedEvidenceKeys, presetName, worldInteraction, autoPosition, default(Vector2), post, null, null, true);
		}
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x001C9920 File Offset: 0x001C7B20
	public void SubmitCaseButton()
	{
		ResolveController componentInChildren = this.wcc.window.GetComponentInChildren<ResolveController>(true);
		if (componentInChildren != null && componentInChildren.submitButton.interactable)
		{
			componentInChildren.SubmitButton();
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x001C995C File Offset: 0x001C7B5C
	public void CloseCaseButton()
	{
		ResolveController componentInChildren = this.wcc.window.GetComponentInChildren<ResolveController>(true);
		if (componentInChildren != null)
		{
			componentInChildren.CloseCaseButton();
		}
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x001C998C File Offset: 0x001C7B8C
	public void SetPageSize(Vector2 newSize)
	{
		string text = "Set page size: ";
		Vector2 vector = newSize;
		Game.Log(text + vector.ToString(), 2);
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x04002BFC RID: 11260
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002BFD RID: 11261
	public RectTransform pageRect;

	// Token: 0x04002BFE RID: 11262
	public WindowContentController wcc;

	// Token: 0x04002BFF RID: 11263
	public TextMeshProUGUI titleText;

	// Token: 0x04002C00 RID: 11264
	public ButtonController submitButton;

	// Token: 0x04002C01 RID: 11265
	public ButtonController openJobPostButton;
}
