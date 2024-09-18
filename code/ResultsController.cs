using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005F8 RID: 1528
public class ResultsController : MonoBehaviour
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060021AA RID: 8618 RVA: 0x001C99DA File Offset: 0x001C7BDA
	public static ResultsController Instance
	{
		get
		{
			return ResultsController._instance;
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x001C99E4 File Offset: 0x001C7BE4
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		if (ResultsController._instance != null && ResultsController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ResultsController._instance = this;
		}
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(740f, 768f));
		this.titleText.text = Strings.Get("ui.interface", "Investigation Results", Strings.Casing.asIs, false, false, false, null) + ": " + this.wcc.window.passedCase.name;
		this.descriptionText.text = Strings.Get("ui.interface", "results_description", Strings.Casing.asIs, false, false, false, null);
		this.UpdateResolveFields();
		if (this.wcc.window.passedCase.isSolved)
		{
			this.questionsBar.SetValue(this.wcc.window.passedCase.questionsRank * 5f);
			this.victimsBar.SetValue(this.wcc.window.passedCase.victimsRank * 5f);
			this.successText.text = Strings.Get("ui.interface", "Case solved", Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			this.questionsBar.SetValue(0f);
			this.victimsBar.SetValue(0f);
			this.successText.text = Strings.Get("ui.interface", "Case unsolved", Strings.Casing.asIs, false, false, false, null);
		}
		this.rankText.text = Strings.Get("ui.interface", this.wcc.window.passedCase.rank.ToString(), Strings.Casing.asIs, false, false, false, null);
		this.closeCaseButton.SetInteractable(this.wcc.window.passedCase.isSolved);
		this.isSetup = true;
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x001C9BD4 File Offset: 0x001C7DD4
	public void UpdateResolveFields()
	{
		CasePanelController.Instance.SetPickModeActive(false, null);
		while (this.spawnedInputFields.Count > 0)
		{
			Object.Destroy(this.spawnedInputFields[0].gameObject);
			this.spawnedInputFields.RemoveAt(0);
		}
		foreach (Case.ResolveQuestion resolveQuestion in this.wcc.window.passedCase.resolveQuestions)
		{
			if (!resolveQuestion.displayOnlyAtPhase || this.wcc.window.passedCase == null || this.wcc.window.passedCase.job == null || this.wcc.window.passedCase.job.phase >= resolveQuestion.displayAtPhase)
			{
				InputFieldController component = Object.Instantiate<GameObject>(this.inputFieldPrefab, this.pageRect).GetComponent<InputFieldController>();
				component.Setup(resolveQuestion, this.wcc.window.passedCase, true);
				this.spawnedInputFields.Add(component);
			}
		}
		this.questionsBar.transform.SetAsLastSibling();
		this.victimsBar.transform.SetAsLastSibling();
		this.rankImage.transform.SetAsLastSibling();
		this.successText.transform.SetAsLastSibling();
		this.closeCaseButton.rect.SetAsLastSibling();
		this.SetPageSize(new Vector2(740f, Mathf.Max(768f, (float)this.spawnedInputFields.Count * 152f + 640f)));
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x001C9D84 File Offset: 0x001C7F84
	public void SetPageSize(Vector2 newSize)
	{
		string text = "Set page size: ";
		Vector2 vector = newSize;
		Game.Log(text + vector.ToString(), 2);
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x00002265 File Offset: 0x00000465
	public void CloseCaseButton()
	{
	}

	// Token: 0x04002C02 RID: 11266
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002C03 RID: 11267
	public RectTransform pageRect;

	// Token: 0x04002C04 RID: 11268
	public WindowContentController wcc;

	// Token: 0x04002C05 RID: 11269
	public TextMeshProUGUI titleText;

	// Token: 0x04002C06 RID: 11270
	public TextMeshProUGUI descriptionText;

	// Token: 0x04002C07 RID: 11271
	public TextMeshProUGUI successText;

	// Token: 0x04002C08 RID: 11272
	public GameObject inputFieldPrefab;

	// Token: 0x04002C09 RID: 11273
	public ButtonController closeCaseButton;

	// Token: 0x04002C0A RID: 11274
	public LayoutGroup layout;

	// Token: 0x04002C0B RID: 11275
	public ProgressBarController questionsBar;

	// Token: 0x04002C0C RID: 11276
	public ProgressBarController victimsBar;

	// Token: 0x04002C0D RID: 11277
	public Image rankImage;

	// Token: 0x04002C0E RID: 11278
	public TextMeshProUGUI rankText;

	// Token: 0x04002C0F RID: 11279
	[Header("State")]
	public bool isSetup;

	// Token: 0x04002C10 RID: 11280
	public List<InputFieldController> spawnedInputFields = new List<InputFieldController>();

	// Token: 0x04002C11 RID: 11281
	private static ResultsController _instance;
}
