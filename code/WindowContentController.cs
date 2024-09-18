using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000596 RID: 1430
public class WindowContentController : MonoBehaviour
{
	// Token: 0x06001F37 RID: 7991 RVA: 0x001B01E5 File Offset: 0x001AE3E5
	private void Awake()
	{
		this.GetReferences();
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x001B01F0 File Offset: 0x001AE3F0
	private void GetReferences()
	{
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		if (this.pageImg == null)
		{
			this.pageImg = base.gameObject.GetComponentInChildren<Image>();
		}
		this.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x001B024C File Offset: 0x001AE44C
	private void OnEnable()
	{
		if (this.tabController != null && this.tabController.preset.contentType == WindowTabPreset.TabContentType.facts)
		{
			this.window.item.PositionSpawnedFacts(10f, 6f);
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x001B0289 File Offset: 0x001AE489
	public void SetAlwaysCentred(bool newVal)
	{
		this.alwaysCentred = newVal;
		if (this.alwaysCentred)
		{
			this.centred = true;
			this.CentrePage();
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x001B02A8 File Offset: 0x001AE4A8
	public void CentrePage()
	{
		if (this.rect == null)
		{
			this.GetReferences();
		}
		this.rect.anchoredPosition = Vector2.zero;
		ZoomContent component = base.gameObject.GetComponent<ZoomContent>();
		if (component != null)
		{
			component.ResetPivot();
			this.UpdateFitScale();
			component.SetZoom(this.fitScale);
		}
		this.centred = true;
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x001B0310 File Offset: 0x001AE510
	public void UpdateFitScale()
	{
		Vector2 size = this.window.scrollRect.GetComponent<RectTransform>().rect.size;
		float num = size.x / (this.normalSize.x + this.window.centringTollerance * 0.5f);
		float num2 = size.y / (this.normalSize.y + this.window.centringTollerance * 0.5f);
		if (this.tabController.preset.fitToScaleX && this.tabController.preset.fitToScaleY)
		{
			this.fitScale = (float)Mathf.RoundToInt(Mathf.Min(num, num2) * 100f) / 100f;
			return;
		}
		if (this.tabController.preset.fitToScaleX && !this.tabController.preset.fitToScaleY)
		{
			this.fitScale = (float)Mathf.RoundToInt(num * 100f) / 100f;
			return;
		}
		if (!this.tabController.preset.fitToScaleX && this.tabController.preset.fitToScaleY)
		{
			this.fitScale = (float)Mathf.RoundToInt(num2 * 100f) / 100f;
		}
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x001B0444 File Offset: 0x001AE644
	public void LoadContent()
	{
		while (this.spawnedContent.Count > 0)
		{
			Object.Destroy(this.spawnedContent[0]);
			this.spawnedContent.RemoveAt(0);
		}
		this.spawnedText.Clear();
		string text = this.window.passedEvidence.preset.ddsDocumentID;
		if (this.window.passedEvidence.overrideDDS != null && this.window.passedEvidence.overrideDDS.Length > 0)
		{
			text = this.window.passedEvidence.overrideDDS;
		}
		if (!Toolbox.Instance.allDDSTrees.TryGetValue(text, ref this.content))
		{
			Game.LogError(string.Concat(new string[]
			{
				"Evidence: Cannot find content for ",
				this.window.name,
				" tree ",
				text,
				" (",
				this.window.passedEvidence.preset.name,
				"). Original doc ID: ",
				this.window.passedEvidence.preset.ddsDocumentID
			}), 2);
			return;
		}
		if (this.content.treeType == DDSSaveClasses.TreeType.vmail)
		{
			DDSSaveClasses.DDSTreeSave printedVmailDoc = Toolbox.Instance.allDDSTrees["04f9b47f-e525-4aef-82b2-3cffe52cc061"];
			this.rect.sizeDelta = printedVmailDoc.document.size;
			this.pageRect.sizeDelta = printedVmailDoc.document.size;
			this.normalSize = printedVmailDoc.document.size;
			this.rect.localPosition = Vector3.zero;
			this.rect.localScale = Vector3.one;
			this.pageRect.localPosition = Vector3.zero;
			this.pageRect.localScale = Vector3.one;
			this.pageImg.sprite = DDSControls.Instance.backgroundSprites.Find((Sprite item) => item.name == printedVmailDoc.document.background);
			this.pageImg.type = printedVmailDoc.document.fill;
			this.pageImg.color = printedVmailDoc.document.colour;
			Game.Log("Evidence: Loaded document from printed vmail doc...", 2);
			printedVmailDoc.messages.Sort((DDSSaveClasses.DDSMessageSettings p1, DDSSaveClasses.DDSMessageSettings p2) => p1.order.CompareTo(p2.order));
			foreach (DDSSaveClasses.DDSMessageSettings msg in printedVmailDoc.messages)
			{
				this.ConstructContent(msg);
			}
			EvidencePrintedVmail evidencePrintedVmail = this.window.passedEvidence as EvidencePrintedVmail;
			if (evidencePrintedVmail != null)
			{
				this.thread = evidencePrintedVmail.thread;
				this.msgIndex = evidencePrintedVmail.msgIndexID;
			}
			this.UpdateNoteText();
		}
		else
		{
			this.rect.sizeDelta = this.content.document.size;
			this.pageRect.sizeDelta = this.content.document.size;
			this.normalSize = this.content.document.size;
			this.rect.localPosition = Vector3.zero;
			this.rect.localScale = Vector3.one;
			this.pageRect.localPosition = Vector3.zero;
			this.pageRect.localScale = Vector3.one;
			this.pageImg.sprite = DDSControls.Instance.backgroundSprites.Find((Sprite item) => item.name == this.content.document.background);
			this.pageImg.type = this.content.document.fill;
			this.pageImg.color = this.content.document.colour;
			Game.Log("Evidence: Set document fill type to " + this.content.document.fill.ToString(), 2);
			this.content.messages.Sort((DDSSaveClasses.DDSMessageSettings p1, DDSSaveClasses.DDSMessageSettings p2) => p1.order.CompareTo(p2.order));
			foreach (DDSSaveClasses.DDSMessageSettings msg2 in this.content.messages)
			{
				this.ConstructContent(msg2);
			}
		}
		this.CentrePage();
		this.TextOverflowCheck();
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x001B08D4 File Offset: 0x001AEAD4
	public void ConstructContent(DDSSaveClasses.DDSMessageSettings msg)
	{
		this.window.clearTextButton.gameObject.SetActive(false);
		GameObject gameObject = Object.Instantiate<GameObject>(DDSControls.Instance.elementPrefab, this.pageRect);
		this.spawnedContent.Add(gameObject);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.sizeDelta = msg.size;
		component.localPosition = msg.pos;
		component.localEulerAngles = new Vector3(0f, 0f, msg.rot);
		Toolbox.Instance.SetAnchor(component, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
		if (msg.msgID != null && msg.msgID.Length > 0)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(DDSControls.Instance.textComponent, component);
			TextMeshProUGUI component2 = gameObject2.GetComponent<TextMeshProUGUI>();
			object obj = this.window.passedEvidence;
			if (this.window.passedInteractable != null)
			{
				obj = this.window.passedInteractable;
			}
			if (this.window.passedEvidence.interactable != null)
			{
				obj = this.window.passedEvidence.interactable;
			}
			component2.text = Strings.GetTextForComponent(msg.msgID, obj, this.window.passedEvidence.writer, this.window.passedEvidence.reciever, "\n", false, null, Strings.LinkSetting.automatic, null);
			if (msg.isHandwriting)
			{
				if (this.window.passedEvidence.writer != null && this.window.passedEvidence.writer.handwriting != null)
				{
					component2.font = this.window.passedEvidence.writer.handwriting.fontAsset;
				}
				else
				{
					component2.font = DDSControls.Instance.defaultHandwritingFont;
				}
				this.window.clearTextButton.gameObject.SetActive(true);
			}
			else
			{
				component2.font = DDSControls.Instance.fonts.Find((TMP_FontAsset item) => item.name == msg.font);
			}
			component2.color = msg.col;
			component2.fontSize = msg.fontSize;
			component2.characterSpacing = msg.charSpace;
			component2.wordSpacing = msg.wordSpace;
			component2.lineSpacing = msg.lineSpace;
			component2.paragraphSpacing = msg.paraSpace;
			TextAlignmentOptions alignment = 513;
			if (msg.alignH == 0)
			{
				if (msg.alignV == 0)
				{
					alignment = 257;
				}
				else if (msg.alignV == 1)
				{
					alignment = 4097;
				}
				else if (msg.alignV == 2)
				{
					alignment = 1025;
				}
			}
			else if (msg.alignH == 1)
			{
				if (msg.alignV == 0)
				{
					alignment = 258;
				}
				else if (msg.alignV == 1)
				{
					alignment = 514;
				}
				else if (msg.alignV == 2)
				{
					alignment = 1026;
				}
			}
			else if (msg.alignH == 2)
			{
				if (msg.alignV == 0)
				{
					alignment = 260;
				}
				else if (msg.alignV == 1)
				{
					alignment = 4100;
				}
				else if (msg.alignV == 2)
				{
					alignment = 1028;
				}
			}
			component2.alignment = alignment;
			this.spawnedText.Add(msg, component2);
		}
		else
		{
			GameObject gameObject3 = DDSControls.Instance.elementPrefabs.Find((GameObject item) => item.name == msg.elementName);
			if (gameObject3 != null)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject3, component);
				Image componentInChildren = gameObject2.GetComponentInChildren<Image>();
				if (componentInChildren != null)
				{
					componentInChildren.color = msg.col;
				}
				else
				{
					TextMeshProUGUI componentInChildren2 = gameObject2.GetComponentInChildren<TextMeshProUGUI>();
					if (componentInChildren2 != null)
					{
						componentInChildren2.color = msg.col;
					}
				}
			}
		}
		if (msg.usePages)
		{
			this.elementText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		}
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x001B0D44 File Offset: 0x001AEF44
	public void UpdateNoteText()
	{
		bool flag = true;
		foreach (KeyValuePair<DDSSaveClasses.DDSMessageSettings, TextMeshProUGUI> keyValuePair in this.spawnedText)
		{
			if (this.content.treeType == DDSSaveClasses.TreeType.vmail)
			{
				string text = string.Empty;
				if (this.thread != null)
				{
					if (this.msgIndex < this.thread.messages.Count)
					{
						string text2 = this.thread.messages[this.msgIndex];
						DDSSaveClasses.DDSMessageSettings ddsmessageSettings = null;
						if (this.content.messageRef.TryGetValue(text2, ref ddsmessageSettings))
						{
							if (!flag)
							{
								text += "\n\n";
							}
							Human human = null;
							Human human2 = null;
							CityData.Instance.GetHuman(this.thread.senders[this.msgIndex], out human, true);
							CityData.Instance.GetHuman(this.thread.recievers[this.msgIndex], out human2, true);
							string text3 = string.Empty;
							string text4 = string.Empty;
							human = Strings.GetVmailSender(this.thread, this.msgIndex, out text4);
							human2 = Strings.GetVmailReciever(this.thread, this.msgIndex, out text3);
							if (human != null)
							{
								Strings.LinkData linkData = Strings.AddOrGetLink(human.evidenceEntry, null);
								if (linkData != null)
								{
									text4 = string.Concat(new string[]
									{
										"<link=",
										linkData.id.ToString(),
										">",
										text4,
										"</link>"
									});
								}
							}
							if (human2 != null)
							{
								Strings.LinkData linkData2 = Strings.AddOrGetLink(human2.evidenceEntry, null);
								if (linkData2 != null)
								{
									text3 = string.Concat(new string[]
									{
										"<link=",
										linkData2.id.ToString(),
										">",
										text3,
										"</link>"
									});
								}
							}
							text = string.Concat(new string[]
							{
								Strings.Get("computer", "To", Strings.Casing.asIs, false, false, false, null),
								": ",
								text3,
								"\n",
								Strings.Get("computer", "From", Strings.Casing.asIs, false, false, false, null),
								": ",
								text4,
								"\n",
								SessionData.Instance.GameTimeToClock12String(this.thread.timestamps[this.msgIndex], false),
								"\n",
								SessionData.Instance.LongDateString(this.thread.timestamps[this.msgIndex], true, true, true, true, true, true, false, false)
							});
							Human human3 = null;
							Human human4 = null;
							Human human5 = null;
							Human human6 = null;
							CityData.Instance.GetHuman(this.thread.participantA, out human3, true);
							CityData.Instance.GetHuman(this.thread.participantB, out human4, true);
							CityData.Instance.GetHuman(this.thread.participantC, out human5, true);
							CityData.Instance.GetHuman(this.thread.participantD, out human6, true);
							text = text + "\n\n" + Strings.GetTextForComponent(ddsmessageSettings.msgID, new VMailApp.VmailParsingData
							{
								thread = this.thread,
								messageIndex = this.msgIndex
							}, human, human2, "\n\n", false, null, Strings.LinkSetting.automatic, null);
							flag = false;
						}
					}
					else
					{
						Game.LogError(string.Concat(new string[]
						{
							"Message index ",
							this.msgIndex.ToString(),
							" is out of scope for ",
							this.thread.messages.Count.ToString(),
							" messages"
						}), 2);
					}
				}
				else
				{
					Game.LogError("Thread is null for vmail!", 2);
				}
				keyValuePair.Value.text = text;
			}
			else
			{
				string msgID = keyValuePair.Key.msgID;
				if (this.window.passedEvidence != null && this.window.passedEvidence.interactable != null && this.window.passedEvidence.interactable.preset.summaryMessageSource != null && this.window.passedEvidence.interactable.preset.summaryMessageSource.Length > 0)
				{
					DDSSaveClasses.DDSMessageSave ddsmessageSave = null;
					if (Toolbox.Instance.allDDSMessages.TryGetValue(this.window.passedEvidence.interactable.preset.summaryMessageSource, ref ddsmessageSave))
					{
						Game.Log("Override with message id " + this.window.passedEvidence.interactable.preset.summaryMessageSource, 2);
						msgID = ddsmessageSave.id;
					}
				}
				keyValuePair.Value.text = Strings.GetTextForComponent(msgID, this.window.passedInteractable, this.window.passedEvidence.writer, this.window.passedEvidence.reciever, "\n", false, null, Strings.LinkSetting.automatic, null);
			}
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x001B125C File Offset: 0x001AF45C
	public void TextOverflowCheck()
	{
		if (this.elementText != null)
		{
			RectTransform component = this.elementText.gameObject.GetComponent<RectTransform>();
			Vector2 preferredValues = this.elementText.GetPreferredValues();
			Game.Log("Interface: Overflow check for page " + this.tabController.preset.tabName + ": " + this.elementText.isTextOverflowing.ToString(), 2);
			Game.Log("Interface: Overflowcheck preffered height = " + preferredValues.y.ToString(), 2);
			Game.Log("Interface: Overflowcheck actual height = " + component.rect.height.ToString(), 2);
			if (preferredValues.y > component.rect.height && this.elementText.overflowMode != 5)
			{
				Game.Log("Interface: Setting overflow mode to page " + this.tabController.preset.tabName, 2);
				this.elementText.overflowMode = 5;
				this.elementText.ForceMeshUpdate(false, false);
				this.elementText.pageToDisplay = this.page;
				this.pageControls = Object.Instantiate<GameObject>(PrefabControls.Instance.evidenceContentPageControls, base.transform);
				this.pageControls.transform.SetAsLastSibling();
				foreach (object obj in this.pageControls.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.CompareTag("ContentButton1") && this.nextPage == null)
					{
						this.nextPage = transform.gameObject.GetComponent<ButtonController>();
					}
					else if (transform.CompareTag("ContentButton2") && this.prevPage == null)
					{
						this.prevPage = transform.gameObject.GetComponent<ButtonController>();
					}
					else if (transform.CompareTag("PagePipDisplay"))
					{
						for (int i = 0; i < this.pagePips.Count; i++)
						{
							Object.Destroy(this.pagePips[i].gameObject);
						}
						for (int j = 0; j < this.elementText.textInfo.pageCount; j++)
						{
							GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.pagePip, transform);
							this.pagePips.Add(gameObject.GetComponent<Image>());
						}
					}
				}
				this.SetPage(1, true);
				return;
			}
			if (this.pageControls != null)
			{
				Object.Destroy(this.pageControls);
				return;
			}
		}
		else if (this.pageControls != null)
		{
			Object.Destroy(this.pageControls);
		}
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x001B153C File Offset: 0x001AF73C
	public void SetPage(int newPage, bool forceUpdate = false)
	{
		newPage = Mathf.Clamp(newPage, 1, this.elementText.textInfo.pageCount);
		if (newPage != this.page || forceUpdate)
		{
			this.page = newPage;
			this.elementText.pageToDisplay = this.page;
			if (this.page > 1)
			{
				this.prevPage.SetInteractable(true);
			}
			else
			{
				this.prevPage.SetInteractable(false);
			}
			if (this.page < this.elementText.textInfo.pageCount)
			{
				this.nextPage.SetInteractable(true);
			}
			else
			{
				this.nextPage.SetInteractable(false);
			}
			this.window.UpdateControllerNavigationEndOfFrame();
			this.UpdatePips();
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x001B15F0 File Offset: 0x001AF7F0
	public void NextPage()
	{
		this.SetPage(this.page + 1, false);
		AudioController.Instance.Play2DSound(AudioControls.Instance.pageForward, null, 1f);
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x001B161C File Offset: 0x001AF81C
	public void PrevPage()
	{
		this.SetPage(this.page - 1, false);
		AudioController.Instance.Play2DSound(AudioControls.Instance.pageBack, null, 1f);
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x001B1648 File Offset: 0x001AF848
	private void UpdatePips()
	{
		for (int i = 0; i < this.pagePips.Count; i++)
		{
			if (i == this.page - 1)
			{
				this.pagePips[i].color = Color.white;
			}
			else
			{
				this.pagePips[i].color = Color.grey;
			}
		}
	}

	// Token: 0x0400290F RID: 10511
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002910 RID: 10512
	public Button tabButton;

	// Token: 0x04002911 RID: 10513
	public WindowTabController tabController;

	// Token: 0x04002912 RID: 10514
	public InfoWindow window;

	// Token: 0x04002913 RID: 10515
	public ZoomContent zoomController;

	// Token: 0x04002914 RID: 10516
	public RectTransform pageRect;

	// Token: 0x04002915 RID: 10517
	public Image pageImg;

	// Token: 0x04002916 RID: 10518
	public Vector2 normalSize;

	// Token: 0x04002917 RID: 10519
	public bool centred;

	// Token: 0x04002918 RID: 10520
	public bool alwaysCentred;

	// Token: 0x04002919 RID: 10521
	public float fitScale = 1f;

	// Token: 0x0400291A RID: 10522
	public bool clearTextMode;

	// Token: 0x0400291B RID: 10523
	private StateSaveData.MessageThreadSave thread;

	// Token: 0x0400291C RID: 10524
	private int msgIndex;

	// Token: 0x0400291D RID: 10525
	[Header("Content")]
	[NonSerialized]
	public DDSSaveClasses.DDSTreeSave content;

	// Token: 0x0400291E RID: 10526
	public Dictionary<DDSSaveClasses.DDSMessageSettings, TextMeshProUGUI> spawnedText = new Dictionary<DDSSaveClasses.DDSMessageSettings, TextMeshProUGUI>();

	// Token: 0x0400291F RID: 10527
	private List<GameObject> spawnedContent = new List<GameObject>();

	// Token: 0x04002920 RID: 10528
	public TextMeshProUGUI elementText;

	// Token: 0x04002921 RID: 10529
	private GameObject pageControls;

	// Token: 0x04002922 RID: 10530
	public List<Image> pagePips = new List<Image>();

	// Token: 0x04002923 RID: 10531
	public int page = -1;

	// Token: 0x04002924 RID: 10532
	public ButtonController nextPage;

	// Token: 0x04002925 RID: 10533
	public ButtonController prevPage;
}
