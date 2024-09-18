using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020004DF RID: 1247
public class StringController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001AF9 RID: 6905 RVA: 0x0018A0A0 File Offset: 0x001882A0
	public void Setup(CasePanelController.StringConnection newConnection)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.connection = newConnection;
		this.fromRect = this.connection.from.GetComponent<RectTransform>();
		this.toRect = this.connection.to.GetComponent<RectTransform>();
		this.img = base.gameObject.GetComponent<Image>();
		base.name = this.connection.from.name + " -> " + this.connection.to.name;
		int num = 0;
		foreach (Fact fact in this.connection.facts)
		{
			TooltipController tooltipController = base.gameObject.AddComponent<TooltipController>();
			tooltipController.useCursorPos = true;
			tooltipController.limitWidth = true;
			tooltipController.handleOwnBehaviour = false;
			tooltipController.cursorPosOffset = new Vector2((float)(10 + num), -7f);
			tooltipController.extendTooltipWidth = 50;
			this.tooltips.Add(fact, tooltipController);
			num += Mathf.RoundToInt(InterfaceControls.Instance.tooltipWidth) + 32;
			if (fact.isCustom)
			{
				this.contextMenu.disabledItems.Clear();
			}
		}
		this.connection.from.OnMoved += this.UpdatePosition;
		this.connection.to.OnMoved += this.UpdatePosition;
		foreach (Evidence.FactLink factLink in this.connection.links)
		{
		}
		if (!this.connection.from.connectedStrings.Contains(this))
		{
			this.connection.from.connectedStrings.Add(this);
		}
		if (!this.connection.to.connectedStrings.Contains(this))
		{
			this.connection.to.connectedStrings.Add(this);
		}
		this.contextMenu.OnOpenMenu += this.ForceCloseTooltip;
		this.UpdatePosition();
		this.UpdateStringColour();
		this.UpdateHidden();
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x0018A2F8 File Offset: 0x001884F8
	public void UpdatePosition()
	{
		if (this.rect == null)
		{
			try
			{
				this.rect = base.gameObject.GetComponent<RectTransform>();
			}
			catch
			{
				Game.Log("Trying to access a destroyed object.", 2);
				return;
			}
		}
		if (this.fromRect == null)
		{
			this.fromRect = this.connection.from.GetComponent<RectTransform>();
		}
		if (this.toRect == null)
		{
			this.toRect = this.connection.to.GetComponent<RectTransform>();
		}
		Vector3 vector = this.toRect.localPosition - this.fromRect.localPosition;
		this.rect.sizeDelta = new Vector2(vector.magnitude, this.rect.sizeDelta.y);
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		this.rect.rotation = Quaternion.Euler(0f, 0f, num);
		this.rect.localPosition = new Vector2(this.fromRect.localPosition.x, this.fromRect.localPosition.y + 70f);
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x0018A440 File Offset: 0x00188640
	public void ForceCloseTooltip()
	{
		foreach (KeyValuePair<Fact, TooltipController> keyValuePair in this.tooltips)
		{
			keyValuePair.Value.ForceClose();
		}
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x0018A498 File Offset: 0x00188698
	private void OnDestroy()
	{
		this.connection.from.OnMoved -= this.UpdatePosition;
		this.connection.to.OnMoved -= this.UpdatePosition;
		foreach (Evidence.FactLink factLink in this.connection.links)
		{
		}
		this.contextMenu.OnOpenMenu -= this.ForceCloseTooltip;
		if (this.connection.from.connectedStrings.Contains(this))
		{
			this.connection.from.connectedStrings.Remove(this);
		}
		if (this.connection.to.connectedStrings.Contains(this))
		{
			this.connection.to.connectedStrings.Remove(this);
		}
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0018A598 File Offset: 0x00188798
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (InterfaceControls.Instance.enableTooltips)
		{
			this.isOver = true;
			this.moTimer = 0f;
			this.fadeIn = 0f;
			base.StartCoroutine("MouseOver");
		}
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x0018A5CF File Offset: 0x001887CF
	private IEnumerator MouseOver()
	{
		while (this.isOver && this.contextMenu.spawnedMenu == null)
		{
			this.moTimer += Time.deltaTime;
			if (this.moTimer > InterfaceControls.Instance.toolTipDelay + this.additionalSpawnDelay)
			{
				foreach (KeyValuePair<Fact, TooltipController> keyValuePair in this.tooltips)
				{
					if (keyValuePair.Value.spawnedTooltip == null)
					{
						this.UpdateTooltipText(keyValuePair.Key);
						keyValuePair.Value.OpenTooltip();
						keyValuePair.Value.spawnedTooltip.GetComponent<Image>().raycastTarget = false;
						this.fadeIn = 0f;
					}
				}
				if (this.fadeIn < 1f)
				{
					this.fadeIn += Time.deltaTime * InterfaceControls.Instance.toolTipFadeInSpeed;
					this.fadeIn = Mathf.Clamp01(this.fadeIn);
					foreach (KeyValuePair<Fact, TooltipController> keyValuePair2 in this.tooltips)
					{
						keyValuePair2.Value.rend.SetAlpha(this.fadeIn);
						keyValuePair2.Value.textRend.SetAlpha(this.fadeIn);
						keyValuePair2.Value.tooltipText.ForceMeshUpdate(false, false);
					}
				}
			}
			yield return null;
		}
		foreach (KeyValuePair<Fact, TooltipController> keyValuePair3 in this.tooltips)
		{
			keyValuePair3.Value.ForceClose();
		}
		this.fadeIn = 0f;
		yield break;
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x0018A5E0 File Offset: 0x001887E0
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
		base.StopCoroutine("MouseOver");
		this.fadeIn = 0f;
		foreach (KeyValuePair<Fact, TooltipController> keyValuePair in this.tooltips)
		{
			keyValuePair.Value.ForceClose();
		}
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x0018A66C File Offset: 0x0018886C
	public void UpdateTooltipText(Fact fact)
	{
		TooltipController tooltipController = this.tooltips[fact];
		Game.Log("Interface: Updating tooltip text for fact " + fact.preset.name, 2);
		string name = fact.GetName(null);
		string text = string.Empty;
		if (fact.isCustom)
		{
			text += Strings.Get("evidence.generic", "string_customfact", Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			text += Strings.Get("evidence.generic", "string_autofact", Strings.Casing.asIs, false, false, false, null);
		}
		tooltipController.mainText = name.Trim();
		tooltipController.detailText = text.Trim();
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x0018A708 File Offset: 0x00188908
	public void UpdateStringColour()
	{
		InterfaceControls.EvidenceColours getColour = InterfaceControls.EvidenceColours.red;
		if (CasePanelController.Instance.activeCase != null)
		{
			bool flag = false;
			foreach (Case.StringColours stringColours in CasePanelController.Instance.activeCase.stringColours)
			{
				foreach (Evidence.FactLink factLink in this.connection.links)
				{
					if (stringColours.fromEv == factLink.thisEvidence.evID)
					{
						bool flag2 = true;
						foreach (Evidence evidence in factLink.destinationEvidence)
						{
							if (!stringColours.toEv.Contains(evidence.evID))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							bool flag3 = false;
							foreach (Evidence.DataKey dataKey in factLink.thisKeys)
							{
								if (stringColours.fromDK.Contains(dataKey))
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								flag3 = false;
								foreach (Evidence.DataKey dataKey2 in factLink.destinationKeys)
								{
									if (stringColours.toDK.Contains(dataKey2))
									{
										flag3 = true;
										break;
									}
								}
								if (flag3)
								{
									getColour = (InterfaceControls.EvidenceColours)stringColours.colIndex;
									Game.Log("Interface: Found entry for this string " + getColour.ToString(), 2);
									flag = true;
									break;
								}
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		InterfaceControls.PinColours pinColours = InterfaceControls.Instance.pinColours.Find((InterfaceControls.PinColours item) => item.colour == getColour);
		this.img.color = pinColours.actualColour;
		foreach (JuiceController.JuiceElement juiceElement in this.juice.elements)
		{
			juiceElement.originalColour = this.img.color;
		}
		this.juice.pulsateColour = this.img.color * this.pulsateColor;
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x0018AA20 File Offset: 0x00188C20
	[Button(null, 0)]
	public bool UpdateHidden()
	{
		if (CasePanelController.Instance.activeCase == null)
		{
			return false;
		}
		List<Fact> list = new List<Fact>();
		foreach (Fact fact in this.connection.facts)
		{
			string identifier = fact.GetIdentifier();
			if (CasePanelController.Instance.activeCase.hiddenConnections.Contains(identifier))
			{
				list.Add(fact);
			}
		}
		List<Fact> list2 = new List<Fact>(this.connection.facts);
		foreach (Fact fact2 in list)
		{
			list2.Remove(fact2);
		}
		if (list2.Count > 0)
		{
			base.gameObject.SetActive(true);
			return false;
		}
		base.gameObject.SetActive(false);
		return true;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x0018AB24 File Offset: 0x00188D24
	public void SetColour(InterfaceControls.EvidenceColours col)
	{
		if (CasePanelController.Instance.activeCase != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Interface: Set string colour ",
				col.ToString(),
				" to ",
				this.connection.links.Count.ToString(),
				" links"
			}), 2);
			foreach (Evidence.FactLink link in this.connection.links)
			{
				CasePanelController.Instance.activeCase.AddNewStringColour(link, col);
			}
			this.UpdateStringColour();
		}
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x0018ABF0 File Offset: 0x00188DF0
	public void RenameCustomLink()
	{
		foreach (Evidence.FactLink factLink in this.connection.links)
		{
		}
		PopupMessageController.Instance.PopupMessage("CustomFactRename", true, true, "Cancel", "Continue", true, PopupMessageController.AffectPauseState.no, true, Strings.Get("evidence.generic", "customlink", Strings.Casing.asIs, false, false, false, null), false, true, false, false, "", "", false, "", false, "", "");
		PopupMessageController.Instance.OnRightButton += this.OnContinueFactName;
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x0018ACA8 File Offset: 0x00188EA8
	public void OnContinueFactName()
	{
		foreach (Fact fact in this.connection.facts)
		{
			if (fact.isCustom)
			{
				FactCustom factCustom = fact as FactCustom;
				if (factCustom != null)
				{
					factCustom.SetCustomName(PopupMessageController.Instance.inputField.text);
				}
			}
		}
		this.SetColour(PopupMessageController.Instance.colourPicker.GetCurrentSelectedEvidenceColourValue());
		PopupMessageController.Instance.OnRightButton -= this.OnContinueFactName;
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x0018AD4C File Offset: 0x00188F4C
	public void RemoveCustomLink()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnContinueFactName;
		List<Fact> list = this.connection.facts.FindAll((Fact item) => item.isCustom);
		while (list.Count > 0)
		{
			Fact fact = list[0];
			for (int i = 0; i < fact.fromEvidence.Count; i++)
			{
				fact.fromEvidence[i].RemoveFactLink(fact);
			}
			for (int j = 0; j < fact.toEvidence.Count; j++)
			{
				fact.toEvidence[j].RemoveFactLink(fact);
			}
			GameplayController.Instance.factList.Remove(fact);
			list.RemoveAt(0);
		}
		CasePanelController.Instance.UpdateStrings();
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x0018AE24 File Offset: 0x00189024
	public void SetColourRed()
	{
		this.SetColour(InterfaceControls.EvidenceColours.red);
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x0018AE2D File Offset: 0x0018902D
	public void SetColourBlue()
	{
		this.SetColour(InterfaceControls.EvidenceColours.blue);
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x0018AE36 File Offset: 0x00189036
	public void SetColourYellow()
	{
		this.SetColour(InterfaceControls.EvidenceColours.yellow);
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x0018AE3F File Offset: 0x0018903F
	public void SetColourGreen()
	{
		this.SetColour(InterfaceControls.EvidenceColours.green);
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x0018AE48 File Offset: 0x00189048
	public void SetColourPurple()
	{
		this.SetColour(InterfaceControls.EvidenceColours.purple);
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x0018AE51 File Offset: 0x00189051
	public void SetColourWhite()
	{
		this.SetColour(InterfaceControls.EvidenceColours.white);
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x0018AE5A File Offset: 0x0018905A
	public void SetColourBlack()
	{
		this.SetColour(InterfaceControls.EvidenceColours.black);
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x0018AE64 File Offset: 0x00189064
	public void Hide()
	{
		if (CasePanelController.Instance.activeCase != null)
		{
			Game.Log("Interface: Hide string " + base.name, 2);
			foreach (Fact fact in this.connection.facts)
			{
				CasePanelController.Instance.activeCase.SetHidden(fact, true);
			}
		}
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x0018AEE8 File Offset: 0x001890E8
	[Button(null, 0)]
	public void UpdateDebugFactLinks()
	{
		this.debugLinks = this.connection.links;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x0018AEFC File Offset: 0x001890FC
	[Button(null, 0)]
	public void DisplayFactIdentifier()
	{
		if (this.connection != null)
		{
			foreach (Fact fact in this.connection.facts)
			{
				Game.Log(fact.GetIdentifier(), 2);
			}
		}
	}

	// Token: 0x040023A1 RID: 9121
	public RectTransform rect;

	// Token: 0x040023A2 RID: 9122
	public RectTransform fromRect;

	// Token: 0x040023A3 RID: 9123
	public RectTransform toRect;

	// Token: 0x040023A4 RID: 9124
	public Image img;

	// Token: 0x040023A5 RID: 9125
	public Color pulsateColor;

	// Token: 0x040023A6 RID: 9126
	public Dictionary<Fact, TooltipController> tooltips = new Dictionary<Fact, TooltipController>();

	// Token: 0x040023A7 RID: 9127
	public CasePanelController.StringConnection connection;

	// Token: 0x040023A8 RID: 9128
	public ContextMenuController contextMenu;

	// Token: 0x040023A9 RID: 9129
	public JuiceController juice;

	// Token: 0x040023AA RID: 9130
	public List<Evidence.FactLink> debugLinks = new List<Evidence.FactLink>();

	// Token: 0x040023AB RID: 9131
	public float cumulativeReliability;

	// Token: 0x040023AC RID: 9132
	public bool isOver;

	// Token: 0x040023AD RID: 9133
	public float additionalSpawnDelay;

	// Token: 0x040023AE RID: 9134
	public float moTimer;

	// Token: 0x040023AF RID: 9135
	[NonSerialized]
	public float fadeIn;
}
