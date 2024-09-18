using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public class ItemController : MonoBehaviour
{
	// Token: 0x14000038 RID: 56
	// (add) Token: 0x06001EA2 RID: 7842 RVA: 0x001AACA0 File Offset: 0x001A8EA0
	// (remove) Token: 0x06001EA3 RID: 7843 RVA: 0x001AACD8 File Offset: 0x001A8ED8
	public event ItemController.UpdateUnseenFacts OnUpdateUnseenFacts;

	// Token: 0x06001EA4 RID: 7844 RVA: 0x001AAD10 File Offset: 0x001A8F10
	public void Setup(InfoWindow newParent)
	{
		this.parentWindow = newParent;
		this.parentWindow.passedEvidence.OnDataKeyChange += this.UpdateNameDisplay;
		this.parentWindow.passedEvidence.OnDiscoverConnectedFact += this.UpdateFactsDisplay;
		this.parentWindow.passedEvidence.OnDataKeyChange += this.UpdateFactsDisplay;
		this.UpdateNameDisplay();
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x001AAD80 File Offset: 0x001A8F80
	private void OnDestroy()
	{
		if (this.newCustomFactButton != null)
		{
			this.newCustomFactButton.OnPress -= this.NewCustomFactButton;
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.UpdateNameDisplay;
		this.parentWindow.passedEvidence.OnDiscoverConnectedFact -= this.UpdateFactsDisplay;
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.UpdateFactsDisplay;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x001AAE06 File Offset: 0x001A9006
	public void UpdateNameDisplay()
	{
		this.parentWindow.SetName(this.parentWindow.passedEvidence.GetNameForDataKey(this.parentWindow.evidenceKeys));
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x001AAE30 File Offset: 0x001A9030
	public void UpdateFactsDisplay()
	{
		if (this.factContent == null)
		{
			Game.Log("Interface: Fact content is null", 2);
			return;
		}
		this.debugFacts.Add("Update fact display");
		Game.Log("Interface: Update fact display", 2);
		if (this.newCustomFactButton == null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.newCustomFactButton, this.factContent.gameObject.transform);
			this.newCustomFactButton = gameObject.GetComponent<ButtonController>();
			this.newCustomFactButton.SetupReferences();
			this.newCustomFactButton.OnPress += this.NewCustomFactButton;
		}
		List<Evidence.FactLink> list = new List<Evidence.FactLink>();
		foreach (Evidence.FactLink factLink in this.parentWindow.passedEvidence.GetFactsForDataKey(this.parentWindow.evidenceKeys))
		{
			this.debugFacts.Add("Fact: " + factLink.fact.name + "...");
			if (factLink.fact.isFound)
			{
				this.debugFacts.Add("Fact: ... Is found");
				if (!list.Contains(factLink))
				{
					bool flag = false;
					foreach (Evidence.FactLink factLink2 in list)
					{
						if (factLink2.fact == factLink.fact && factLink2.thisEvidence == factLink.thisEvidence)
						{
							bool flag2 = true;
							foreach (Evidence evidence in factLink2.destinationEvidence)
							{
								if (!factLink.destinationEvidence.Contains(evidence))
								{
									this.debugFacts.Add("Fact: ... Same to evidence!");
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								this.debugFacts.Add("Fact: ... Duplicate!");
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						this.debugFacts.Add("Fact: ... Added to required.");
						list.Add(factLink);
					}
				}
			}
		}
		for (int i = 0; i < this.spawnedFactButtons.Count; i++)
		{
			FactButtonController fbc = this.spawnedFactButtons[i];
			if (fbc == null)
			{
				this.spawnedFactButtons.RemoveAt(i);
				i--;
			}
			else
			{
				int num = list.FindIndex((Evidence.FactLink item) => item.fact == fbc.fact);
				if (num > -1)
				{
					list.RemoveAt(num);
				}
				else
				{
					if (!fbc.fact.isSeen)
					{
						fbc.fact.OnSeen -= this.UpdateUnSeenFacts;
					}
					Object.Destroy(fbc.gameObject);
					this.spawnedFactButtons.RemoveAt(i);
					i--;
				}
			}
		}
		foreach (Evidence.FactLink factLink3 in list)
		{
			FactButtonController component = Object.Instantiate<GameObject>(PrefabControls.Instance.factButton, this.factContent.gameObject.transform).GetComponent<FactButtonController>();
			component.Setup(factLink3, this.parentWindow);
			this.spawnedFactButtons.Add(component);
			if (!factLink3.fact.isSeen)
			{
				factLink3.fact.OnSeen += this.UpdateUnSeenFacts;
			}
		}
		this.PositionSpawnedFacts(10f, 6f);
		this.UpdateUnSeenFacts();
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x001AB238 File Offset: 0x001A9438
	public void PositionSpawnedFacts(float edgeMargin = 10f, float iconMargin = 6f)
	{
		if (this.factContent == null)
		{
			return;
		}
		float num = -edgeMargin;
		float num2 = 0f;
		this.spawnedFactButtons.Sort((FactButtonController p1, FactButtonController p2) => p2.fact.preset.factRank.CompareTo(p1.fact.preset.factRank));
		for (int i = 0; i < this.spawnedFactButtons.Count; i++)
		{
			FactButtonController factButtonController = this.spawnedFactButtons[i];
			RectTransform rect = factButtonController.rect;
			factButtonController.inSlot = false;
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, num);
			if (factButtonController.toggleHiddenButton != null)
			{
				factButtonController.toggleHiddenButton.rect.anchoredPosition = new Vector2(-24f, num);
			}
			num2 = Mathf.Abs(num - rect.sizeDelta.y - edgeMargin);
			num -= rect.sizeDelta.y + iconMargin;
		}
		if (this.newCustomFactButton != null)
		{
			RectTransform rect2 = this.newCustomFactButton.rect;
			rect2.anchoredPosition = new Vector2(rect2.anchoredPosition.x, num);
			num2 = Mathf.Abs(num - rect2.sizeDelta.y - edgeMargin);
			num -= rect2.sizeDelta.y + iconMargin;
		}
		RectTransform component = this.factContent.gameObject.transform.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, Mathf.Max(component.sizeDelta.y, num2));
		this.factContent.pageImg.gameObject.GetComponent<RectTransform>().sizeDelta = component.sizeDelta;
		this.factContent.normalSize = component.sizeDelta;
		this.factContent.UpdateFitScale();
		this.factContent.rect.anchoredPosition = new Vector2(this.factContent.rect.anchoredPosition.x, -99999f);
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x001AB430 File Offset: 0x001A9630
	public void UpdateUnSeenFacts()
	{
		this.unSeenFacts = 0;
		foreach (FactButtonController factButtonController in this.spawnedFactButtons)
		{
			if (factButtonController.fact.isSeen)
			{
				factButtonController.fact.OnSeen -= this.UpdateUnSeenFacts;
			}
			else
			{
				this.unSeenFacts++;
			}
		}
		if (this.prevUnSeenFacts != this.unSeenFacts)
		{
			this.prevUnSeenFacts = this.unSeenFacts;
			if (this.OnUpdateUnseenFacts != null)
			{
				this.OnUpdateUnseenFacts(this.unSeenFacts);
			}
		}
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x001AB4EC File Offset: 0x001A96EC
	public void NewCustomFactButton(ButtonController thisButton)
	{
		if (CasePanelController.Instance.activeCase != null && CasePanelController.Instance.activeCase.caseElements.Contains(this.parentWindow.currentPinnedCaseElement) && this.parentWindow.currentPinnedCaseElement.pinnedController != null)
		{
			CasePanelController.Instance.CustomStringLinkSelection(this.parentWindow.currentPinnedCaseElement.pinnedController, false);
		}
	}

	// Token: 0x0400287E RID: 10366
	[NonSerialized]
	public InfoWindow parentWindow;

	// Token: 0x0400287F RID: 10367
	public WindowContentController childEvContent;

	// Token: 0x04002880 RID: 10368
	public List<ButtonController> spawnedChildEvButtons = new List<ButtonController>();

	// Token: 0x04002881 RID: 10369
	public WindowContentController factContent;

	// Token: 0x04002882 RID: 10370
	public List<FactButtonController> spawnedFactButtons = new List<FactButtonController>();

	// Token: 0x04002883 RID: 10371
	public ButtonController newCustomFactButton;

	// Token: 0x04002884 RID: 10372
	public int unSeenFacts;

	// Token: 0x04002885 RID: 10373
	private int prevUnSeenFacts = -1;

	// Token: 0x04002886 RID: 10374
	public List<string> debugFacts = new List<string>();

	// Token: 0x02000578 RID: 1400
	// (Invoke) Token: 0x06001EAD RID: 7853
	public delegate void UpdateUnseenFacts(int val);
}
