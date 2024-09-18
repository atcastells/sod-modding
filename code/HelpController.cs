using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005E7 RID: 1511
public class HelpController : MonoBehaviour
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x0600213F RID: 8511 RVA: 0x001C5BAD File Offset: 0x001C3DAD
	public static HelpController Instance
	{
		get
		{
			return HelpController._instance;
		}
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x001C5BB4 File Offset: 0x001C3DB4
	public void Setup(WindowContentController newContentController)
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		if (HelpController._instance != null && HelpController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			HelpController._instance = this;
		}
		this.wcc = newContentController;
		this.SetPageSize(new Vector2(740f, 738f));
		this.contentsText.text = Strings.Get("ui.handbook", "contentsheader", Strings.Casing.asIs, false, false, false, null);
		if (InterfaceController.Instance.openHelpToPage.Length > 0)
		{
			this.DisplayHelpPage(InterfaceController.Instance.openHelpToPage);
			InterfaceController.Instance.openHelpToPage = "";
		}
		else
		{
			this.DisplayHelpContents();
		}
		this.isSetup = true;
		SessionData.Instance.OnTutorialNotificationChange += this.UpdateHelpButtonList;
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x001C5C90 File Offset: 0x001C3E90
	public void SetPageSize(Vector2 newSize)
	{
		string text = "Interface: Set help size: ";
		Vector2 vector = newSize;
		Game.Log(text + vector.ToString(), 2);
		this.rect.sizeDelta = newSize;
		this.wcc.normalSize = this.rect.sizeDelta;
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x001C5CE0 File Offset: 0x001C3EE0
	public void DisplayHelpContents()
	{
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.helpContents.gameObject.SetActive(true);
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		this.page.SetActive(false);
		this.UpdateHelpButtonList();
		this.backButtonTop.RefreshAutomaticNavigation();
		this.backButtonBottom.RefreshAutomaticNavigation();
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x001C5D6C File Offset: 0x001C3F6C
	public void UpdateHelpButtonList()
	{
		try
		{
			if (this.helpContents != null && this.helpContents.gameObject != null && this.helpContents.gameObject.activeSelf)
			{
				Game.Log("Interface: Updating help list...", 2);
				List<HelpContentPage> list = new List<HelpContentPage>();
				foreach (KeyValuePair<string, HelpContentPage> keyValuePair in Toolbox.Instance.allHelpContent)
				{
					if (!keyValuePair.Value.disabled)
					{
						if (this.searchInputField.text.Length <= 0)
						{
							list.Add(keyValuePair.Value);
						}
						else if (Strings.Get("ui.handbook", keyValuePair.Value.name, Strings.Casing.asIs, false, false, false, null).ToLower().Contains(this.searchInputField.text.ToLower()))
						{
							list.Add(keyValuePair.Value);
						}
					}
				}
				for (int i = 0; i < this.helpContentButtons.Count; i++)
				{
					ButtonController buttonController = this.helpContentButtons[i];
					bool flag = false;
					for (int j = 0; j < list.Count; j++)
					{
						HelpContentPage helpContentPage = list[j];
						if (Strings.Get("ui.handbook", helpContentPage.name, Strings.Casing.asIs, false, false, false, null).ToLower() == buttonController.text.text.ToLower())
						{
							buttonController.icon.enabled = false;
							list.RemoveAt(j);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						buttonController.OnPress -= this.DisplayHelpPage;
						Object.Destroy(buttonController.gameObject);
						this.helpContentButtons.RemoveAt(i);
						i--;
					}
				}
				foreach (HelpContentPage helpContentPage2 in list)
				{
					ButtonController component = Object.Instantiate<GameObject>(this.helpContentButtonPrefab, this.helpContentButtonParent).GetComponent<ButtonController>();
					this.helpContentButtons.Add(component);
					component.text.text = Strings.Get("ui.handbook", helpContentPage2.name, Strings.Casing.asIs, false, false, false, null);
					component.name = helpContentPage2.name;
					component.icon.enabled = false;
					component.OnPress += this.DisplayHelpPage;
				}
				this.helpContentButtonParent.sizeDelta = new Vector2(this.helpContentButtonParent.sizeDelta.x, Mathf.Max((float)this.helpContentButtons.Count * 72f + 24f, 466f));
				this.SetPageSize(new Vector2(this.rect.sizeDelta.x, this.helpContentButtonParent.sizeDelta.y + 400f));
				for (int k = 0; k < this.helpContentButtons.Count; k++)
				{
					this.helpContentButtons[k].RefreshAutomaticNavigation();
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x001C60D0 File Offset: 0x001C42D0
	private void OnDestroy()
	{
		SessionData.Instance.OnTutorialNotificationChange -= this.UpdateHelpButtonList;
		foreach (ButtonController buttonController in this.helpContentButtons)
		{
			buttonController.OnPress -= this.DisplayHelpPage;
		}
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x001C6144 File Offset: 0x001C4344
	public void ClearSearchButton()
	{
		this.searchInputField.text = string.Empty;
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x001C6156 File Offset: 0x001C4356
	public void DisplayHelpPage(ButtonController button)
	{
		this.LoadHelpPage(button.name);
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x001C6164 File Offset: 0x001C4364
	public void DisplayHelpPage(string pageName)
	{
		this.LoadHelpPage(pageName.ToLower());
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x001C6174 File Offset: 0x001C4374
	public void LoadHelpPage(string h)
	{
		HelpContentPage helpContentPage = null;
		if (!Toolbox.Instance.allHelpContent.TryGetValue(h, ref helpContentPage))
		{
			return;
		}
		if (InterfaceController.Instance.selectedElement != null)
		{
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		this.page.SetActive(true);
		this.helpContents.gameObject.SetActive(false);
		Game.Log("Interface: Displaying help content " + helpContentPage.name, 2);
		this.page.SetActive(true);
		this.helpTitle.text = Strings.Get("ui.handbook", helpContentPage.name, Strings.Casing.asIs, false, false, false, null);
		StringBuilder stringBuilder = new StringBuilder();
		List<int> list2;
		List<string> list = Player.Instance.ParseDDSMessage(helpContentPage.messageID, null, out list2, false, null, false);
		for (int i = 0; i < list.Count; i++)
		{
			string text = Strings.ComposeText(Strings.Get("dds.blocks", list[i], Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceLinks, null, null, false);
			if (i < list.Count - 1)
			{
				text += "\n\n";
			}
			stringBuilder.Append(text);
		}
		this.helpContent.text = stringBuilder.ToString();
		float num = 0f;
		this.helpContent.rectTransform.sizeDelta = new Vector2(this.helpContent.rectTransform.sizeDelta.x, this.helpContent.preferredHeight);
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, 0f);
		num += this.helpContent.preferredHeight;
		while (this.videos.Count > 0)
		{
			Object.Destroy(this.videos[0].gameObject);
			this.videos.RemoveAt(0);
		}
		for (int j = 0; j < helpContentPage.contentDisplay.Count; j++)
		{
			InterfaceVideoController component = Object.Instantiate<GameObject>(PrefabControls.Instance.interfaceVideo, this.page.transform).GetComponent<InterfaceVideoController>();
			component.Setup(helpContentPage.contentDisplay[j].clip, helpContentPage.contentDisplay[j].image);
			this.videos.Add(component);
			num += 300f;
		}
		this.SetPageSize(new Vector2(this.rect.sizeDelta.x, Mathf.Max(num + 400f, 648f)));
		this.backButtonTop.juice.GetOriginalRectSize();
		this.backButtonBottom.juice.GetOriginalRectSize();
		SessionData.Instance.UpdateTutorialNotifications();
	}

	// Token: 0x04002B80 RID: 11136
	public RectTransform rect;

	// Token: 0x04002B81 RID: 11137
	public WindowContentController wcc;

	// Token: 0x04002B82 RID: 11138
	public bool isSetup;

	// Token: 0x04002B83 RID: 11139
	[Header("Contents")]
	public RectTransform helpContents;

	// Token: 0x04002B84 RID: 11140
	public TMP_InputField searchInputField;

	// Token: 0x04002B85 RID: 11141
	public RectTransform helpContentButtonParent;

	// Token: 0x04002B86 RID: 11142
	public TextMeshProUGUI contentsText;

	// Token: 0x04002B87 RID: 11143
	public List<InterfaceVideoController> videos = new List<InterfaceVideoController>();

	// Token: 0x04002B88 RID: 11144
	[Header("Page")]
	public GameObject page;

	// Token: 0x04002B89 RID: 11145
	public TextMeshProUGUI helpTitle;

	// Token: 0x04002B8A RID: 11146
	public TextMeshProUGUI helpContent;

	// Token: 0x04002B8B RID: 11147
	public ButtonController backButtonTop;

	// Token: 0x04002B8C RID: 11148
	public ButtonController backButtonBottom;

	// Token: 0x04002B8D RID: 11149
	public VerticalLayoutGroup layoutGroup;

	// Token: 0x04002B8E RID: 11150
	[Header("Content")]
	public List<ButtonController> helpContentButtons = new List<ButtonController>();

	// Token: 0x04002B8F RID: 11151
	public GameObject helpContentButtonPrefab;

	// Token: 0x04002B90 RID: 11152
	private static HelpController _instance;
}
