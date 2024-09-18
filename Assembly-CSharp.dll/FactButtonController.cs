using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200056A RID: 1386
public class FactButtonController : ButtonController
{
	// Token: 0x06001E2C RID: 7724 RVA: 0x001A6D1C File Offset: 0x001A4F1C
	public void Setup(Evidence.FactLink newFactLink, InfoWindow newParentWindow)
	{
		this.parentWindow = newParentWindow;
		this.SetupReferences();
		this.link = newFactLink;
		this.fact = newFactLink.fact;
		if (this.fact != null && !this.fact.isSeen)
		{
			this.fact.OnSeen += this.OnSeen;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.factHideToggleButton, base.transform.parent);
		this.toggleHiddenButton = gameObject.GetComponent<ButtonController>();
		this.toggleHiddenButton.SetupReferences();
		this.toggleHiddenButton.OnPress += this.ToggleHidden;
		this.toggleHiddenButton.OnHoverChange += this.UpdatePulsate;
		this.UpdateTooltipText();
		this.VisualUpdate();
		this.fact.OnConnectingEvidenceChangeDataKey += this.VisualUpdate;
		this.fact.OnNewName += this.UpdateTooltipText;
		this.isSetup = true;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x001A6E18 File Offset: 0x001A5018
	public void OnSeen()
	{
		this.fact.OnSeen -= this.OnSeen;
		this.VisualUpdate();
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x001A6E37 File Offset: 0x001A5037
	public void ToggleHidden(ButtonController thisButton)
	{
		if (CasePanelController.Instance.activeCase != null)
		{
			CasePanelController.Instance.activeCase.ToggleHidden(this.fact);
		}
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x001A6E5A File Offset: 0x001A505A
	private void OnEnable()
	{
		if (!this.enabledFirstTime && this.isSetup)
		{
			this.enabledFirstTime = true;
		}
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x001A6E73 File Offset: 0x001A5073
	private void OnDisable()
	{
		if (this.fact != null && !this.fact.isSeen && this.enabledFirstTime)
		{
			this.fact.SetSeen();
		}
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x001A6EA0 File Offset: 0x001A50A0
	private void OnDestroy()
	{
		if (this.toggleHiddenButton != null)
		{
			this.toggleHiddenButton.OnPress -= this.ToggleHidden;
			this.toggleHiddenButton.OnHoverChange -= this.UpdatePulsate;
			Object.Destroy(this.toggleHiddenButton.gameObject);
		}
		this.UpdatePulsate(this, false);
		this.fact.OnConnectingEvidenceChangeDataKey -= this.VisualUpdate;
		this.fact.OnNewName -= this.UpdateTooltipText;
		if (this.fact != null && !this.fact.isSeen)
		{
			this.fact.OnSeen -= this.OnSeen;
		}
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x001A6F60 File Offset: 0x001A5160
	public override void VisualUpdate()
	{
		try
		{
			if (this.fact != null && this.parentWindow.passedEvidence != null)
			{
				Evidence evidence = this.fact.GetOther(this.parentWindow.passedEvidence);
				if (evidence == null && this.fact.fromEvidence.Count > 0)
				{
					evidence = this.fact.fromEvidence[0];
				}
				if (evidence != null && this.parentWindow.passedEvidence.children.Contains(evidence))
				{
					if (this.childOfThisIcon != null)
					{
						this.childOfThisIcon.gameObject.SetActive(true);
					}
					if (this.parentToThisIcon != null)
					{
						this.parentToThisIcon.gameObject.SetActive(false);
					}
				}
				else if (evidence != null && evidence.children.Contains(this.parentWindow.passedEvidence))
				{
					if (this.childOfThisIcon != null)
					{
						this.childOfThisIcon.gameObject.SetActive(false);
					}
					if (this.parentToThisIcon != null)
					{
						this.parentToThisIcon.gameObject.SetActive(true);
					}
				}
				else
				{
					if (this.childOfThisIcon != null)
					{
						this.childOfThisIcon.gameObject.SetActive(false);
					}
					if (this.parentToThisIcon != null)
					{
						this.parentToThisIcon.gameObject.SetActive(false);
					}
				}
				if (this.fact.iconSprite != null)
				{
					if (!(this.fact.iconSprite != null))
					{
						goto IL_318;
					}
					try
					{
						this.icon.sprite = this.fact.iconSprite;
						goto IL_318;
					}
					catch
					{
						goto IL_318;
					}
				}
				if (this.parentWindow != null && this.parentWindow.passedEvidence != null)
				{
					if (this.fact.fromEvidence != null && this.fact.fromEvidence.Contains(this.parentWindow.passedEvidence))
					{
						if (this.fact.toEvidence.Count <= 0 || this.fact.toEvidence[0] == null || !(this.fact.toEvidence[0].iconSprite != null))
						{
							goto IL_318;
						}
						Sprite icon = this.fact.toEvidence[0].GetIcon();
						if (!(icon != null))
						{
							goto IL_318;
						}
						try
						{
							this.icon.sprite = icon;
							goto IL_318;
						}
						catch
						{
							goto IL_318;
						}
					}
					if (this.fact.toEvidence != null && this.fact.toEvidence.Contains(this.parentWindow.passedEvidence) && this.fact.fromEvidence.Count > 0 && this.fact.fromEvidence[0] != null && this.fact.fromEvidence[0].iconSprite != null)
					{
						Sprite icon2 = this.fact.fromEvidence[0].GetIcon();
						if (icon2 != null)
						{
							try
							{
								this.icon.sprite = icon2;
							}
							catch
							{
							}
						}
					}
				}
				IL_318:
				this.UpdateTooltipText();
				if (this.isSeenIcon != null)
				{
					if (!this.fact.isSeen)
					{
						this.isSeenIcon.gameObject.SetActive(true);
					}
					else
					{
						this.isSeenIcon.gameObject.SetActive(false);
					}
				}
				bool flag = false;
				if (CasePanelController.Instance.activeCase != null)
				{
					string identifier = this.fact.GetIdentifier();
					if (CasePanelController.Instance.activeCase.hiddenConnections.Contains(identifier))
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.toggleHiddenButton.icon.sprite = this.hiddenConnection;
					this.toggleHiddenButton.background.color = this.hiddenColor;
				}
				else
				{
					this.toggleHiddenButton.icon.sprite = this.shownConnection;
					this.toggleHiddenButton.background.color = this.shownColor;
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x001A73C8 File Offset: 0x001A55C8
	public override void OnLeftClick()
	{
		foreach (Evidence evidence in this.fact.fromEvidence)
		{
			if (InterfaceController.Instance.GetWindow(evidence, this.fact.fromDataKeys) == null)
			{
				InterfaceController.Instance.SpawnWindow(evidence, Evidence.DataKey.name, this.fact.fromDataKeys, "", false, true, default(Vector2), null, null, null, true);
			}
		}
		foreach (Evidence evidence2 in this.fact.toEvidence)
		{
			if (InterfaceController.Instance.GetWindow(evidence2, this.fact.toDataKeys) == null)
			{
				InterfaceController.Instance.SpawnWindow(evidence2, Evidence.DataKey.name, this.fact.toDataKeys, "", false, true, default(Vector2), null, null, null, true);
			}
		}
		if (!this.fact.isSeen)
		{
			this.fact.SetSeen();
		}
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x001A7508 File Offset: 0x001A5708
	public override void UpdateTooltipText()
	{
		if (this.tooltip == null)
		{
			return;
		}
		this.tooltip.mainText = this.fact.GetName(null);
		this.text.text = this.tooltip.mainText;
		string empty = string.Empty;
		"<color=#" + ColorUtility.ToHtmlStringRGB(InterfaceControls.Instance.defaultTextColour) + ">";
		this.tooltip.detailText = empty;
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x001A7582 File Offset: 0x001A5782
	public override void OnHoverStart()
	{
		this.UpdatePulsate(this, true);
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x001A758C File Offset: 0x001A578C
	public override void OnHoverEnd()
	{
		this.UpdatePulsate(this, false);
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x001A7598 File Offset: 0x001A5798
	public void UpdatePulsate(ButtonController hoveredButton, bool mouseOver)
	{
		foreach (StringController stringController in CasePanelController.Instance.spawnedStrings)
		{
			bool toggle = false;
			if (mouseOver && stringController.connection.facts.Contains(this.fact))
			{
				toggle = true;
			}
			stringController.juice.Pulsate(toggle, false);
		}
	}

	// Token: 0x04002808 RID: 10248
	public Evidence.FactLink link;

	// Token: 0x04002809 RID: 10249
	public Fact fact;

	// Token: 0x0400280A RID: 10250
	public ButtonController toggleHiddenButton;

	// Token: 0x0400280B RID: 10251
	public Image parentToThisIcon;

	// Token: 0x0400280C RID: 10252
	public Image childOfThisIcon;

	// Token: 0x0400280D RID: 10253
	public Sprite shownConnection;

	// Token: 0x0400280E RID: 10254
	public Sprite hiddenConnection;

	// Token: 0x0400280F RID: 10255
	public Color shownColor = Color.white;

	// Token: 0x04002810 RID: 10256
	public Color hiddenColor = Color.gray;

	// Token: 0x04002811 RID: 10257
	public RectTransform isSeenIcon;

	// Token: 0x04002812 RID: 10258
	private bool isSetup;

	// Token: 0x04002813 RID: 10259
	private bool enabledFirstTime;

	// Token: 0x04002814 RID: 10260
	public bool inSlot;
}
