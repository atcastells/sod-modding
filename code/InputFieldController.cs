using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000574 RID: 1396
public class InputFieldController : MonoBehaviour
{
	// Token: 0x06001E92 RID: 7826 RVA: 0x001AA264 File Offset: 0x001A8464
	public void Setup(Case.ResolveQuestion newQuestion, Case newCase, bool newResultsMode = false)
	{
		this.resultsMode = newResultsMode;
		this.belongsToCase = newCase;
		this.question = newQuestion;
		this.question.inputField = this;
		this.titleText.text = this.question.GetText(this.belongsToCase, true, true);
		this.progress.rect.gameObject.SetActive(false);
		Game.Log("Jobs: Setting up input field for " + this.question.name, 2);
		string text = string.Empty;
		for (int i = 0; i < this.question.correctAnswers.Count; i++)
		{
			string text2 = this.question.correctAnswers[i];
			if (this.question.inputType == Case.InputType.citizen)
			{
				int number = -1;
				int.TryParse(text2, ref number);
				Human human = CityData.Instance.citizenDirectory.Find((Citizen item) => item.humanID == number);
				if (human != null)
				{
					text += human.GetCitizenName();
				}
				if (i < this.question.correctAnswers.Count - 1)
				{
					text += ", ";
				}
			}
			else if (this.question.inputType == Case.InputType.location)
			{
				int number = -1;
				int.TryParse(text2, ref number);
				NewGameLocation newGameLocation = CityData.Instance.gameLocationDirectory.Find((NewGameLocation item) => (item.thisAsAddress != null && item.thisAsAddress.id == number) || (item.thisAsStreet != null && item.thisAsStreet.streetID == number));
				if (newGameLocation != null)
				{
					text += newGameLocation.name;
				}
				if (i < this.question.correctAnswers.Count - 1)
				{
					text += ", ";
				}
			}
			else if (this.question.inputType == Case.InputType.item)
			{
				Evidence evidence = null;
				if (GameplayController.Instance.evidenceDictionary.TryGetValue(text2, ref evidence))
				{
					text += evidence.name;
					if (i < this.question.correctAnswers.Count - 1)
					{
						text += ", ";
					}
				}
			}
		}
		if (this.question.inputType == Case.InputType.revengeObjective)
		{
			this.inputNameButton.gameObject.SetActive(false);
			this.selectButton.gameObject.SetActive(false);
			RevengeObjective revengeObjective = this.question.GetRevengeObjective();
			if (revengeObjective != null)
			{
				if (revengeObjective.specialConditions.Contains(RevengeObjective.SpecialConditions.trackProgressFromAddressQuestion))
				{
					this.progress.rect.gameObject.SetActive(true);
					Game.Log("Jobs: Setting progress bar for " + this.question.name + ": " + this.question.progress.ToString(), 2);
					this.progress.SetValue(this.question.progress);
					this.question.OnProgressChange += this.ProgressChange;
				}
				else
				{
					Game.Log("Jobs: Unable to get 'track progress from address question' in case revenge objective " + this.question.name, 2);
				}
			}
			else
			{
				Game.Log("Jobs: Unable to get revenge objective " + this.question.name, 2);
			}
		}
		else if (this.question.inputType == Case.InputType.objective || this.question.inputType == Case.InputType.arrestPurp || this.question.inputType == Case.InputType.saveVictim)
		{
			this.inputNameButton.gameObject.SetActive(false);
			this.selectButton.gameObject.SetActive(false);
		}
		else if (this.resultsMode)
		{
			this.inputNameButton.SetInteractable(false);
			if (this.question.isCorrect)
			{
				this.checkbox.sprite = this.tickSprite;
				this.rewardedGraphic.gameObject.SetActive(true);
			}
			else
			{
				this.checkbox.sprite = this.crossSprite;
				this.rewardedGraphic.gameObject.SetActive(false);
				int penalty = this.question.penalty;
				if (this.question.isOptional && this.belongsToCase.isSolved)
				{
				}
			}
		}
		else
		{
			this.selectButton.icon.gameObject.SetActive(false);
			this.selectButton.text.text = Strings.Get("ui.interface", "Select from Board...", Strings.Casing.asIs, false, false, false, null);
			Game.Log("Chapter: New input field: " + this.titleText.text + " correct answer(s): " + text, 2);
		}
		if (this.question.inputType == Case.InputType.item)
		{
			this.inputNameButton.SetInteractable(false);
			if (this.question.inputtedEvidence != null && this.question.inputtedEvidence.Length > 0)
			{
				Evidence selectedEvidence = null;
				if (GameplayController.Instance.evidenceDictionary.TryGetValue(this.question.inputtedEvidence, ref selectedEvidence))
				{
					this.SetSelectedEvidence(selectedEvidence);
				}
			}
		}
		this.inputNameButton.text.text = this.question.input;
		this.OnInputEdited();
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x001AA750 File Offset: 0x001A8950
	public void ProgressChange(Case.ResolveQuestion q)
	{
		Game.Log("Jobs: Setting progress bar for " + this.question.name + ": " + this.question.progress.ToString(), 2);
		this.progress.SetValue(this.question.progress);
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x001AA7A4 File Offset: 0x001A89A4
	public void OpenTextInputButton()
	{
		if (!this.resultsMode)
		{
			PopupMessageController.Instance.PopupMessage("input_" + this.question.inputType.ToString(), true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, true, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.inputField.text = this.question.input;
			PopupMessageController.Instance.OnLeftButton += this.OnInputTextPopupCancel;
			PopupMessageController.Instance.OnRightButton += this.OnInputTextPopupConfirm;
		}
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x001AA85F File Offset: 0x001A8A5F
	public void OnInputTextPopupCancel()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnInputTextPopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnInputTextPopupConfirm;
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x001AA890 File Offset: 0x001A8A90
	public void OnInputTextPopupConfirm()
	{
		PopupMessageController.Instance.OnLeftButton -= this.OnInputTextPopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnInputTextPopupConfirm;
		this.inputNameButton.text.text = PopupMessageController.Instance.inputField.text;
		this.OnInputEdited();
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x001AA8F0 File Offset: 0x001A8AF0
	private void OnDestroy()
	{
		this.question.OnProgressChange -= this.ProgressChange;
		PopupMessageController.Instance.OnLeftButton -= this.OnInputTextPopupCancel;
		PopupMessageController.Instance.OnRightButton -= this.OnInputTextPopupConfirm;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x001AA940 File Offset: 0x001A8B40
	public void OnInputEdited()
	{
		if (!this.resultsMode)
		{
			this.question.input = this.inputNameButton.text.text;
			Game.Log("Chapter: Saving question input " + this.inputNameButton.text.text + " to question " + this.question.name, 2);
			this.question.UpdateValid(this.belongsToCase);
			if (this.belongsToCase != null)
			{
				this.belongsToCase.ValidationCheck();
				if (this.belongsToCase.job != null)
				{
					this.belongsToCase.job.HandleObjectiveProgress();
				}
			}
			this.question.UpdateCorrect(this.belongsToCase, true);
		}
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x001AA9FC File Offset: 0x001A8BFC
	public void UpdateCheckbox()
	{
		if (this.question.isValid)
		{
			this.checkbox.sprite = this.tickSprite;
			this.inputNameButton.SetButtonBaseColour(this.validInputColor);
			return;
		}
		this.checkbox.sprite = this.emptySprite;
		this.inputNameButton.SetButtonBaseColour(this.invalidInputColor);
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x001AAA5B File Offset: 0x001A8C5B
	public void OnSelectButton()
	{
		if (!this.resultsMode)
		{
			CasePanelController.Instance.SetPickModeActive(true, this);
		}
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x001AAA74 File Offset: 0x001A8C74
	public void SetSelectedEvidence(Evidence newI)
	{
		if (newI != null)
		{
			this.inputtedEvidence = newI;
			this.question.inputtedEvidence = newI.evID;
			if (this.inputtedEvidence.interactable != null)
			{
				this.question.input = this.inputtedEvidence.interactable.GetName();
				this.inputNameButton.text.text = this.inputtedEvidence.interactable.GetName();
			}
			else
			{
				this.question.input = this.inputtedEvidence.GetNameForDataKey(Evidence.DataKey.name);
				this.inputNameButton.text.text = this.question.input;
			}
		}
		else
		{
			this.question.inputtedEvidence = string.Empty;
			this.question.input = "...";
			this.inputNameButton.text.text = "...";
		}
		this.OnInputEdited();
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x001AAB58 File Offset: 0x001A8D58
	public void OnPick(Evidence newSelection, List<Evidence.DataKey> keys)
	{
		if (!this.resultsMode)
		{
			EvidenceCitizen evidenceCitizen = newSelection as EvidenceCitizen;
			if (evidenceCitizen != null && CasePanelController.Instance.pickForField.question.inputType == Case.InputType.citizen)
			{
				this.inputNameButton.text.text = evidenceCitizen.witnessController.GetCitizenName();
			}
			else
			{
				EvidenceLocation evidenceLocation = newSelection as EvidenceLocation;
				if (evidenceLocation != null && CasePanelController.Instance.pickForField.question.inputType == Case.InputType.location)
				{
					this.inputNameButton.text.text = evidenceLocation.locationController.name;
				}
				else if (CasePanelController.Instance.pickForField.question.inputType == Case.InputType.item)
				{
					this.SetSelectedEvidence(newSelection);
				}
			}
			CasePanelController.Instance.SetPickModeActive(false, null);
			this.OnInputEdited();
		}
	}

	// Token: 0x0400286C RID: 10348
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x0400286D RID: 10349
	public ButtonController inputNameButton;

	// Token: 0x0400286E RID: 10350
	public ButtonController selectButton;

	// Token: 0x0400286F RID: 10351
	public TextMeshProUGUI titleText;

	// Token: 0x04002870 RID: 10352
	public Image checkbox;

	// Token: 0x04002871 RID: 10353
	public Sprite tickSprite;

	// Token: 0x04002872 RID: 10354
	public Sprite crossSprite;

	// Token: 0x04002873 RID: 10355
	public Sprite emptySprite;

	// Token: 0x04002874 RID: 10356
	public ProgressBarController progress;

	// Token: 0x04002875 RID: 10357
	public RectTransform rewardedGraphic;

	// Token: 0x04002876 RID: 10358
	[Header("State")]
	public bool resultsMode;

	// Token: 0x04002877 RID: 10359
	public Case belongsToCase;

	// Token: 0x04002878 RID: 10360
	public Case.ResolveQuestion question;

	// Token: 0x04002879 RID: 10361
	[NonSerialized]
	public Evidence inputtedEvidence;

	// Token: 0x0400287A RID: 10362
	public Color invalidInputColor = Color.red;

	// Token: 0x0400287B RID: 10363
	public Color validInputColor = Color.white;
}
