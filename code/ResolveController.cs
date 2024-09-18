using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005F5 RID: 1525
public class ResolveController : MonoBehaviour
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06002193 RID: 8595 RVA: 0x001C8E2D File Offset: 0x001C702D
	public static ResolveController Instance
	{
		get
		{
			return ResolveController._instance;
		}
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x001C8E34 File Offset: 0x001C7034
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		if (ResolveController._instance != null && ResolveController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ResolveController._instance = this;
		}
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(740f, 768f));
		this.titleText.text = Strings.Get("ui.interface", "Resolve Case", Strings.Casing.asIs, false, false, false, null);
		this.descriptionText.text = Strings.Get("ui.interface", "resolvecase_description", Strings.Casing.asIs, false, false, false, null);
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.caseType == Case.CaseType.mainStory)
		{
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (ChapterController.Instance.currentPart >= 31 && ChapterController.Instance.currentPart < 58 && !chapterIntro.completed)
			{
				this.changeLeadButton.gameObject.SetActive(true);
				this.changeLeadButton.SetInteractable(true);
			}
			else
			{
				this.changeLeadButton.gameObject.SetActive(false);
				this.changeLeadButton.SetInteractable(false);
			}
		}
		else
		{
			Object.Destroy(this.changeLeadButton.gameObject);
		}
		this.UpdateResolveFields();
		this.isSetup = true;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x001C8F88 File Offset: 0x001C7188
	public void UpdateResolveFields()
	{
		CasePanelController.Instance.SetPickModeActive(false, null);
		while (this.spawnedInputFields.Count > 0)
		{
			Object.Destroy(this.spawnedInputFields[0].gameObject);
			this.spawnedInputFields.RemoveAt(0);
		}
		if (CasePanelController.Instance.activeCase != null && (CasePanelController.Instance.activeCase.caseStatus == Case.CaseStatus.handInCollected || CasePanelController.Instance.activeCase.caseStatus == Case.CaseStatus.closable))
		{
			foreach (Case.ResolveQuestion resolveQuestion in CasePanelController.Instance.activeCase.resolveQuestions)
			{
				if (!resolveQuestion.displayOnlyAtPhase || CasePanelController.Instance.activeCase.job == null || CasePanelController.Instance.activeCase.job.phase >= resolveQuestion.displayAtPhase)
				{
					InputFieldController component = Object.Instantiate<GameObject>(this.inputFieldPrefab, this.pageRect).GetComponent<InputFieldController>();
					component.Setup(resolveQuestion, CasePanelController.Instance.activeCase, false);
					this.spawnedInputFields.Add(component);
				}
			}
		}
		this.invalidText.transform.SetAsLastSibling();
		this.lineBreak1.SetAsLastSibling();
		this.submitButton.rect.SetAsLastSibling();
		this.closeCaseButton.rect.SetAsLastSibling();
		this.lineBreak2.SetAsLastSibling();
		if (CasePanelController.Instance.activeCase != null)
		{
			CasePanelController.Instance.activeCase.ValidationCheck();
		}
		this.ValidationUpdate();
		this.SetPageSize(new Vector2(740f, Mathf.Max(768f, (float)this.spawnedInputFields.Count * 152f + 500f)));
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x001C9158 File Offset: 0x001C7358
	public void ValidationUpdate()
	{
		if (CasePanelController.Instance.activeCase == null)
		{
			this.isValid = false;
			this.wcc.window.CloseWindow(false);
			return;
		}
		this.isValid = CasePanelController.Instance.activeCase.handInValid;
		if (this.invalidText != null)
		{
			this.invalidText.gameObject.SetActive(false);
		}
		if (this.spawnedInputFields.Count <= 0)
		{
			this.isValid = false;
		}
		if (CasePanelController.Instance.activeCase.job != null)
		{
			if (CasePanelController.Instance.activeCase.resolveQuestions.Exists((Case.ResolveQuestion item) => item.displayOnlyAtPhase && CasePanelController.Instance.activeCase.job.phase < item.displayAtPhase))
			{
				this.invalidText.text = Strings.Get("ui.interface", "You cannot hand this case in yet as not all objectives have been revealed...", Strings.Casing.asIs, false, false, false, null);
				this.invalidText.gameObject.SetActive(true);
				this.isValid = false;
			}
		}
		int num = 0;
		if (!this.invalidText.gameObject.activeSelf)
		{
			foreach (InputFieldController inputFieldController in this.spawnedInputFields)
			{
				if (!inputFieldController.question.isValid && !inputFieldController.question.isOptional)
				{
					this.invalidText.text = Strings.Get("ui.interface", "Please answer the question", Strings.Casing.asIs, false, false, false, null) + ": " + inputFieldController.titleText.text;
					this.invalidText.gameObject.SetActive(true);
					this.isValid = false;
				}
				if (inputFieldController.question.isValid)
				{
					num++;
				}
			}
		}
		this.submitButton.SetInteractable(this.isValid);
		CasePanelController.Instance.activeCase.handInValid = this.isValid;
		ResolveOptionsController componentInChildren = this.wcc.window.GetComponentInChildren<ResolveOptionsController>(true);
		if (componentInChildren != null)
		{
			componentInChildren.submitButton.SetInteractable(this.isValid);
		}
		this.submitButton.UpdateButtonText();
		TextMeshProUGUI text = this.submitButton.text;
		text.text = string.Concat(new string[]
		{
			text.text,
			" (",
			num.ToString(),
			"/",
			this.spawnedInputFields.Count.ToString(),
			")"
		});
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x001C93E0 File Offset: 0x001C75E0
	public void SetPageSize(Vector2 newSize)
	{
		string text = "Set page size: ";
		Vector2 vector = newSize;
		Game.Log(text + vector.ToString(), 2);
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x001C942E File Offset: 0x001C762E
	private void OnDestroy()
	{
		CasePanelController.Instance.SetPickModeActive(false, null);
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x001C943C File Offset: 0x001C763C
	public void SubmitButton()
	{
		if (CasePanelController.Instance.activeCase != null)
		{
			Interactable closestHandIn = CasePanelController.Instance.activeCase.GetClosestHandIn();
			if (closestHandIn != null)
			{
				MapController.Instance.PlotPlayerRoute(closestHandIn.node, true, null);
			}
		}
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x001C947C File Offset: 0x001C767C
	public void ChangeLeadButton()
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.caseType == Case.CaseType.mainStory && ChapterController.Instance != null && ChapterController.Instance.chapterScript != null && ChapterController.Instance.currentPart >= 31 && ChapterController.Instance.currentPart < 58)
		{
			ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
			if (chapterIntro != null)
			{
				chapterIntro.ExecuteChangeLeadsManual();
				this.wcc.window.CloseWindow(false);
			}
		}
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x001C9514 File Offset: 0x001C7714
	public void CloseCaseButton()
	{
		if (CasePanelController.Instance.activeCase != null)
		{
			if (CasePanelController.Instance.activeCase.caseType == Case.CaseType.mainStory && !Game.Instance.sandboxMode)
			{
				PopupMessageController.Instance.PopupMessage("CloseCaseStory", true, false, "Cancel", "", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
				return;
			}
			PopupMessageController.Instance.PopupMessage("CloseCase", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnLeftButton += this.CancelCloseCase;
			PopupMessageController.Instance.OnRightButton += this.ConfirmCloseCurrentCase;
		}
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x001C95FE File Offset: 0x001C77FE
	public void CancelCloseCase()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelCloseCase;
		PopupMessageController.Instance.OnRightButton -= this.ConfirmCloseCurrentCase;
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x001C962C File Offset: 0x001C782C
	public void ConfirmCloseCurrentCase()
	{
		PopupMessageController.Instance.OnLeftButton -= this.CancelCloseCase;
		PopupMessageController.Instance.OnRightButton -= this.ConfirmCloseCurrentCase;
		if (CasePanelController.Instance.activeCase == null)
		{
			return;
		}
		Game.Log("Player: Close current case: " + CasePanelController.Instance.activeCase.name, 2);
		if (CasePanelController.Instance.activeCase.job != null)
		{
			CasePanelController.Instance.activeCase.job.End();
		}
		this.wcc.window.CloseWindow(false);
		CasePanelController.Instance.CloseCase(CasePanelController.Instance.activeCase);
	}

	// Token: 0x04002BE9 RID: 11241
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002BEA RID: 11242
	public RectTransform pageRect;

	// Token: 0x04002BEB RID: 11243
	public WindowContentController wcc;

	// Token: 0x04002BEC RID: 11244
	public TextMeshProUGUI titleText;

	// Token: 0x04002BED RID: 11245
	public TextMeshProUGUI descriptionText;

	// Token: 0x04002BEE RID: 11246
	public GameObject inputFieldPrefab;

	// Token: 0x04002BEF RID: 11247
	public TextMeshProUGUI invalidText;

	// Token: 0x04002BF0 RID: 11248
	public RectTransform lineBreak1;

	// Token: 0x04002BF1 RID: 11249
	public ButtonController submitButton;

	// Token: 0x04002BF2 RID: 11250
	public ButtonController changeLeadButton;

	// Token: 0x04002BF3 RID: 11251
	public ButtonController closeCaseButton;

	// Token: 0x04002BF4 RID: 11252
	public RectTransform lineBreak2;

	// Token: 0x04002BF5 RID: 11253
	public LayoutGroup layout;

	// Token: 0x04002BF6 RID: 11254
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002BF7 RID: 11255
	public bool isValid;

	// Token: 0x04002BF8 RID: 11256
	public List<InputFieldController> spawnedInputFields = new List<InputFieldController>();

	// Token: 0x04002BF9 RID: 11257
	private static ResolveController _instance;
}
