using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005BE RID: 1470
public class PopupMessageController : MonoBehaviour
{
	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06002037 RID: 8247 RVA: 0x001BAECC File Offset: 0x001B90CC
	// (remove) Token: 0x06002038 RID: 8248 RVA: 0x001BAF04 File Offset: 0x001B9104
	public event PopupMessageController.LeftButton OnLeftButton;

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06002039 RID: 8249 RVA: 0x001BAF3C File Offset: 0x001B913C
	// (remove) Token: 0x0600203A RID: 8250 RVA: 0x001BAF74 File Offset: 0x001B9174
	public event PopupMessageController.RightButton OnRightButton;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x0600203B RID: 8251 RVA: 0x001BAFAC File Offset: 0x001B91AC
	// (remove) Token: 0x0600203C RID: 8252 RVA: 0x001BAFE4 File Offset: 0x001B91E4
	public event PopupMessageController.LeftButton2 OnLeftButton2;

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x0600203D RID: 8253 RVA: 0x001BB01C File Offset: 0x001B921C
	// (remove) Token: 0x0600203E RID: 8254 RVA: 0x001BB054 File Offset: 0x001B9254
	public event PopupMessageController.RightButton2 OnRightButton2;

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x0600203F RID: 8255 RVA: 0x001BB08C File Offset: 0x001B928C
	// (remove) Token: 0x06002040 RID: 8256 RVA: 0x001BB0C4 File Offset: 0x001B92C4
	public event PopupMessageController.OptionButton OnOptionButton;

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06002041 RID: 8257 RVA: 0x001BB0F9 File Offset: 0x001B92F9
	public static PopupMessageController Instance
	{
		get
		{
			return PopupMessageController._instance;
		}
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x001BB100 File Offset: 0x001B9300
	public void Setup()
	{
		if (PopupMessageController._instance != null && PopupMessageController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			PopupMessageController._instance = this;
		}
		this.colourPicker.SetChosen(0);
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x001BB13C File Offset: 0x001B933C
	private void Update()
	{
		if (this.active && this.appearProgress < 1f)
		{
			this.appearProgress += 3.66f * Time.deltaTime;
			this.appearProgress = Mathf.Clamp01(this.appearProgress);
		}
		else if (!this.active && this.appearProgress > 0f)
		{
			this.appearProgress -= 10f * Time.deltaTime;
			this.appearProgress = Mathf.Clamp01(this.appearProgress);
		}
		else if (!this.active && this.appearProgress <= 0f)
		{
			this.vignetteObject.SetActive(false);
			this.rect.gameObject.SetActive(false);
			this.tutorialRect.gameObject.SetActive(false);
			return;
		}
		if (this.active && this.affectPauseState && SessionData.Instance.play)
		{
			SessionData.Instance.PauseGame(true, false, true);
		}
		float num = Mathf.Max(this.appearProgress * this.appearProgress * this.appearProgress, 0.2f);
		if (!this.tutorialActive)
		{
			this.canvasGroup.alpha = num;
			this.rect.localScale = new Vector3(num, num, num);
		}
		else
		{
			this.tutorialCanvasGroup.alpha = num;
			this.tutorialRect.localScale = new Vector3(num, num, num);
		}
		this.vignetteRenderer.SetAlpha(num);
		if (this.active && this.appearProgress >= 1f && !this.setupNav)
		{
			InterfaceController.Instance.UpdateCursorSprite();
			if (!this.tutorialActive)
			{
				using (List<LayoutGroup>.Enumerator enumerator = this.buttonLayouts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LayoutGroup layoutGroup = enumerator.Current;
						layoutGroup.enabled = true;
					}
					goto IL_203;
				}
			}
			foreach (LayoutGroup layoutGroup2 in this.tutorialButtonLayouts)
			{
				layoutGroup2.enabled = true;
			}
			IL_203:
			ButtonController[] componentsInChildren = this.canvasGroup.gameObject.GetComponentsInChildren<ButtonController>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].RefreshAutomaticNavigation();
			}
			this.setupNav = true;
		}
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x001BB398 File Offset: 0x001B9598
	public void PopupMessage(string newMsgString, bool enableLeftButton = true, bool enableRightButton = false, string LButton = "Cancel", string RButton = "", bool anyButtonClosesMsg = true, PopupMessageController.AffectPauseState newPauseState = PopupMessageController.AffectPauseState.automatic, bool enableInputField = false, string inputFieldDefault = "", bool closeMap = false, bool enableColourPicker = false, bool enableSecondaryLeftButton = false, bool enableSecondaryRightButton = false, string LButton2 = "", string RButton2 = "", bool enableOptionButton = false, string OButton = "", bool enableTextScrollView = false, string scrollViewText = "", string mainTextPreWrittenOverride = "")
	{
		if (this.active)
		{
			return;
		}
		this.setupNav = false;
		foreach (LayoutGroup layoutGroup in this.buttonLayouts)
		{
			layoutGroup.enabled = true;
		}
		if (InterfaceController.Instance.selectedElement != null)
		{
			Game.Log("Menu: Deselect button " + InterfaceController.Instance.selectedElement.name + " through selection", 2);
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		InputController.Instance.ResetCurrentButtonDown();
		if (!CameraController.Instance.cam.enabled)
		{
			CameraController.Instance.cam.enabled = true;
		}
		if ((newPauseState == PopupMessageController.AffectPauseState.automatic && SessionData.Instance.play) || newPauseState == PopupMessageController.AffectPauseState.yes)
		{
			this.affectPauseState = true;
		}
		else
		{
			this.affectPauseState = false;
		}
		if (SessionData.Instance.play)
		{
			SessionData.Instance.PauseGame(true, false, true);
		}
		if (closeMap)
		{
			MapController.Instance.CloseMap(true);
		}
		if (enableInputField)
		{
			this.inputField.text = inputFieldDefault;
			this.inputField.gameObject.SetActive(true);
		}
		else
		{
			this.inputField.gameObject.SetActive(false);
		}
		if (enableColourPicker)
		{
			this.colourPicker.gameObject.SetActive(true);
		}
		else
		{
			this.colourPicker.gameObject.SetActive(false);
		}
		this.anyButtonPressCloses = anyButtonClosesMsg;
		this.buttonActions.Clear();
		this.titleText.text = Strings.Get("ui.popups", newMsgString + "_title", Strings.Casing.asIs, false, false, false, null);
		if (mainTextPreWrittenOverride != null && mainTextPreWrittenOverride.Length > 0)
		{
			this.bodyText.text = mainTextPreWrittenOverride;
		}
		else
		{
			this.bodyText.text = Strings.ComposeText(Strings.Get("ui.popups", newMsgString + "_body", Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceNoLinks, null, null, false);
		}
		this.buttonActions.Add(LButton);
		this.buttonActions.Add(RButton);
		this.buttonActions.Add(LButton2);
		this.buttonActions.Add(RButton2);
		this.buttonActions.Add(OButton);
		if (enableRightButton)
		{
			this.rightButton.gameObject.SetActive(true);
			this.rightButton.SetInteractable(true);
			this.rightButton.text.text = Strings.Get("ui.popups", RButton, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.rightButton.OnSelect();
			}
		}
		else
		{
			this.rightButton.SetInteractable(false);
			this.rightButton.gameObject.SetActive(false);
		}
		if (enableLeftButton)
		{
			this.leftButton.gameObject.SetActive(true);
			this.leftButton.SetInteractable(true);
			this.leftButton.text.text = Strings.Get("ui.popups", LButton, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.leftButton.OnSelect();
			}
		}
		else
		{
			this.leftButton.SetInteractable(false);
			this.leftButton.gameObject.SetActive(false);
		}
		if (enableSecondaryRightButton)
		{
			this.rightButton2.gameObject.SetActive(true);
			this.rightButton2.SetInteractable(true);
			this.rightButton2.text.text = Strings.Get("ui.popups", RButton2, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.rightButton2.OnSelect();
			}
		}
		else
		{
			this.rightButton2.SetInteractable(false);
			this.rightButton2.gameObject.SetActive(false);
		}
		if (enableSecondaryLeftButton)
		{
			this.leftButton2.gameObject.SetActive(true);
			this.leftButton2.SetInteractable(true);
			this.leftButton2.text.text = Strings.Get("ui.popups", LButton2, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.leftButton2.OnSelect();
			}
		}
		else
		{
			this.leftButton2.SetInteractable(false);
			this.leftButton2.gameObject.SetActive(false);
		}
		if (enableOptionButton)
		{
			this.optionButton.gameObject.SetActive(true);
			this.optionButton.SetInteractable(true);
			this.optionButton.text.text = Strings.Get("ui.popups", OButton, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.optionButton.OnSelect();
			}
		}
		else
		{
			this.optionButton.SetInteractable(false);
			this.optionButton.gameObject.SetActive(false);
		}
		if (enableTextScrollView)
		{
			this.textScrollView.gameObject.SetActive(true);
			if (scrollViewText.Length > 0)
			{
				this.scrollViewTextObject.SetText(Strings.Get("ui.popups", scrollViewText, Strings.Casing.asIs, false, false, false, null), true);
				this.textScrollViewContent.sizeDelta = new Vector2(this.textScrollViewContent.sizeDelta.x, this.scrollViewTextObject.preferredHeight + 100f);
			}
		}
		else
		{
			this.textScrollView.gameObject.SetActive(false);
		}
		this.rect.gameObject.SetActive(true);
		this.vignetteObject.SetActive(true);
		this.appearProgress = 0f;
		this.active = true;
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			GraphicRaycaster[] componentsInChildren = infoWindow.GetComponentsInChildren<GraphicRaycaster>();
			if (componentsInChildren != null)
			{
				this.otherCanvasRaycasters.AddRange(componentsInChildren);
			}
			if (infoWindow.currentPinnedCaseElement != null && infoWindow.currentPinnedCaseElement.pinnedController != null)
			{
				GraphicRaycaster[] componentsInChildren2 = infoWindow.currentPinnedCaseElement.pinnedController.GetComponentsInChildren<GraphicRaycaster>();
				if (componentsInChildren2 != null)
				{
					this.otherCanvasRaycasters.AddRange(componentsInChildren2);
				}
			}
		}
		foreach (GraphicRaycaster graphicRaycaster in this.otherCanvasRaycasters)
		{
			if (graphicRaycaster != null)
			{
				graphicRaycaster.enabled = false;
			}
		}
		foreach (LayoutGroup layoutGroup2 in this.buttonLayouts)
		{
			layoutGroup2.enabled = false;
		}
		InterfaceController.Instance.UpdateCursorSprite();
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x001BBA64 File Offset: 0x001B9C64
	public void TutorialMessage(string newHelpSection, PopupMessageController.AffectPauseState newPauseState = PopupMessageController.AffectPauseState.automatic, bool closeMap = false, List<string> newSkipBlocks = null)
	{
		if (this.active)
		{
			return;
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (!SessionData.Instance.enableTutorialText)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.loadingOperationActive)
		{
			return;
		}
		this.helpPage = null;
		if (!Toolbox.Instance.allHelpContent.TryGetValue(newHelpSection, ref this.helpPage))
		{
			Game.Log("Unable to find help content: " + newHelpSection, 2);
			return;
		}
		this.setupNav = false;
		this.tutorialActive = true;
		this.helpPageNumber = 0;
		this.maxHelpPageNumber = 0;
		this.skipBlocks = newSkipBlocks;
		this.anyButtonPressCloses = false;
		foreach (LayoutGroup layoutGroup in this.tutorialButtonLayouts)
		{
			layoutGroup.enabled = true;
		}
		if (InterfaceController.Instance.selectedElement != null)
		{
			Game.Log("Menu: Deselect button " + InterfaceController.Instance.selectedElement.name + " through selection", 2);
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
		InputController.Instance.ResetCurrentButtonDown();
		if (!CameraController.Instance.cam.enabled)
		{
			CameraController.Instance.cam.enabled = true;
		}
		if ((newPauseState == PopupMessageController.AffectPauseState.automatic && SessionData.Instance.play) || newPauseState == PopupMessageController.AffectPauseState.yes)
		{
			this.affectPauseState = true;
		}
		else
		{
			this.affectPauseState = false;
		}
		if (SessionData.Instance.play)
		{
			SessionData.Instance.PauseGame(true, false, true);
		}
		if (closeMap)
		{
			MapController.Instance.CloseMap(true);
		}
		this.anyButtonPressCloses = false;
		this.SetHelpPage(0);
		this.tutorialRect.gameObject.SetActive(true);
		this.vignetteObject.SetActive(true);
		this.appearProgress = 0f;
		this.active = true;
		foreach (InfoWindow infoWindow in InterfaceController.Instance.activeWindows)
		{
			GraphicRaycaster[] componentsInChildren = infoWindow.GetComponentsInChildren<GraphicRaycaster>();
			if (componentsInChildren != null)
			{
				this.otherCanvasRaycasters.AddRange(componentsInChildren);
			}
			if (infoWindow.currentPinnedCaseElement != null && infoWindow.currentPinnedCaseElement.pinnedController != null)
			{
				GraphicRaycaster[] componentsInChildren2 = infoWindow.currentPinnedCaseElement.pinnedController.GetComponentsInChildren<GraphicRaycaster>();
				if (componentsInChildren2 != null)
				{
					this.otherCanvasRaycasters.AddRange(componentsInChildren2);
				}
			}
		}
		foreach (GraphicRaycaster graphicRaycaster in this.otherCanvasRaycasters)
		{
			if (graphicRaycaster != null)
			{
				graphicRaycaster.enabled = false;
			}
		}
		foreach (LayoutGroup layoutGroup2 in this.tutorialButtonLayouts)
		{
			layoutGroup2.enabled = false;
		}
		InterfaceController.Instance.UpdateCursorSprite();
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x001BBD8C File Offset: 0x001B9F8C
	public void SetHelpPage(int newNumber)
	{
		List<int> list2;
		List<string> list = Player.Instance.ParseDDSMessage(this.helpPage.messageID, null, out list2, false, null, false);
		if (list == null)
		{
			return;
		}
		this.maxHelpPageNumber = list.Count - 1;
		if (this.skipBlocks != null)
		{
			this.maxHelpPageNumber = list.FindAll((string item) => !this.skipBlocks.Contains(item)).Count - 1;
		}
		this.helpPageNumber = Mathf.Clamp(newNumber, 0, this.maxHelpPageNumber);
		Game.Log("Set help page: " + this.helpPageNumber.ToString() + "/" + this.maxHelpPageNumber.ToString(), 2);
		this.tutorialTitleText.text = Strings.Get("ui.handbook", this.helpPage.name, Strings.Casing.asIs, false, false, false, null) + " (" + (this.helpPageNumber + 1).ToString() + ")";
		new StringBuilder();
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (this.skipBlocks == null || !this.skipBlocks.Contains(list[i]))
			{
				if (this.helpPageNumber == num)
				{
					this.tutorialBodyText.text = Strings.ComposeText(Strings.Get("dds.blocks", list[i], Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceLinks, null, null, false);
					if (this.helpPage.contentDisplay.Count > i)
					{
						this.tutorialVideoPlayer.Setup(this.helpPage.contentDisplay[i].clip, this.helpPage.contentDisplay[i].image);
						break;
					}
					break;
				}
				else
				{
					num++;
				}
			}
		}
		this.buttonActions.Clear();
		this.buttonActions.Add("Previous");
		bool flag = true;
		bool flag2 = true;
		string key = "Next";
		if (num <= 0)
		{
			flag2 = false;
			this.buttonActions.Add("Next");
		}
		else if (num >= this.maxHelpPageNumber)
		{
			key = "Continue";
			this.buttonActions.Add("Continue");
		}
		else
		{
			this.buttonActions.Add("Next");
		}
		if (flag)
		{
			this.tutorialRightButton.gameObject.SetActive(true);
			this.tutorialRightButton.SetInteractable(true);
			this.tutorialRightButton.text.text = Strings.Get("ui.popups", key, Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.tutorialRightButton.OnSelect();
			}
		}
		else
		{
			this.tutorialRightButton.SetInteractable(false);
			this.tutorialRightButton.gameObject.SetActive(false);
		}
		if (flag2)
		{
			this.tutorialLeftButton.gameObject.SetActive(true);
			this.tutorialLeftButton.SetInteractable(true);
			this.tutorialLeftButton.text.text = Strings.Get("ui.popups", "Previous", Strings.Casing.asIs, false, false, false, null);
			if (InterfaceController.Instance.selectedElement == null && !InputController.Instance.mouseInputMode)
			{
				this.tutorialLeftButton.OnSelect();
				return;
			}
		}
		else
		{
			this.tutorialLeftButton.SetInteractable(false);
			this.tutorialLeftButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x001BC0BC File Offset: 0x001BA2BC
	public void RemoveMessage()
	{
		this.active = false;
		this.tutorialActive = false;
		this.buttonActions.Clear();
		InterfaceController.Instance.SetPlayerTextInput(false);
		foreach (GraphicRaycaster graphicRaycaster in this.otherCanvasRaycasters)
		{
			if (graphicRaycaster != null)
			{
				graphicRaycaster.enabled = true;
			}
		}
		if (this.affectPauseState)
		{
			SessionData.Instance.ResumeGame();
		}
		if (InterfaceController.Instance.selectedElement != null)
		{
			Game.Log("Menu: Deselect button " + InterfaceController.Instance.selectedElement.name + " through selection", 2);
			InterfaceController.Instance.selectedElement.OnDeselect();
		}
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x001BC194 File Offset: 0x001BA394
	public void OnButtonPress(int buttonVal)
	{
		if (!this.active || this.appearProgress < 1f)
		{
			return;
		}
		string text = this.buttonActions[buttonVal];
		if (text == "Cancel")
		{
			this.RemoveMessage();
		}
		else if (this.tutorialActive)
		{
			if (text == "Continue")
			{
				this.RemoveMessage();
			}
			else if (text == "Next")
			{
				this.SetHelpPage(this.helpPageNumber + 1);
			}
			else if (text == "Previous")
			{
				this.SetHelpPage(this.helpPageNumber - 1);
			}
		}
		if (buttonVal == 0)
		{
			if (this.OnLeftButton != null)
			{
				this.OnLeftButton();
			}
		}
		else if (buttonVal == 1)
		{
			if (this.OnRightButton != null)
			{
				this.OnRightButton();
			}
		}
		else if (buttonVal == 2)
		{
			if (this.OnLeftButton2 != null)
			{
				this.OnLeftButton2();
			}
		}
		else if (buttonVal == 3)
		{
			if (this.OnRightButton2 != null)
			{
				this.OnRightButton2();
			}
		}
		else if (buttonVal == 4 && this.OnOptionButton != null)
		{
			this.OnOptionButton();
		}
		if (this.anyButtonPressCloses)
		{
			this.RemoveMessage();
		}
	}

	// Token: 0x04002A45 RID: 10821
	[Header("References")]
	public RectTransform rect;

	// Token: 0x04002A46 RID: 10822
	public TextMeshProUGUI titleText;

	// Token: 0x04002A47 RID: 10823
	public TextMeshProUGUI bodyText;

	// Token: 0x04002A48 RID: 10824
	public ButtonController leftButton;

	// Token: 0x04002A49 RID: 10825
	public ButtonController rightButton;

	// Token: 0x04002A4A RID: 10826
	public ButtonController leftButton2;

	// Token: 0x04002A4B RID: 10827
	public ButtonController rightButton2;

	// Token: 0x04002A4C RID: 10828
	public ButtonController optionButton;

	// Token: 0x04002A4D RID: 10829
	public CustomScrollRect textScrollView;

	// Token: 0x04002A4E RID: 10830
	public RectTransform textScrollViewContent;

	// Token: 0x04002A4F RID: 10831
	public TextMeshProUGUI scrollViewTextObject;

	// Token: 0x04002A50 RID: 10832
	public TMP_InputField inputField;

	// Token: 0x04002A51 RID: 10833
	public MultiSelectController colourPicker;

	// Token: 0x04002A52 RID: 10834
	public List<LayoutGroup> buttonLayouts = new List<LayoutGroup>();

	// Token: 0x04002A53 RID: 10835
	public CanvasGroup canvasGroup;

	// Token: 0x04002A54 RID: 10836
	public CanvasRenderer vignetteRenderer;

	// Token: 0x04002A55 RID: 10837
	public GameObject vignetteObject;

	// Token: 0x04002A56 RID: 10838
	public List<GraphicRaycaster> otherCanvasRaycasters = new List<GraphicRaycaster>();

	// Token: 0x04002A57 RID: 10839
	[Space(7f)]
	public RectTransform tutorialRect;

	// Token: 0x04002A58 RID: 10840
	public TextMeshProUGUI tutorialTitleText;

	// Token: 0x04002A59 RID: 10841
	public TextMeshProUGUI tutorialBodyText;

	// Token: 0x04002A5A RID: 10842
	public ButtonController tutorialLeftButton;

	// Token: 0x04002A5B RID: 10843
	public ButtonController tutorialRightButton;

	// Token: 0x04002A5C RID: 10844
	public InterfaceVideoController tutorialVideoPlayer;

	// Token: 0x04002A5D RID: 10845
	public List<LayoutGroup> tutorialButtonLayouts = new List<LayoutGroup>();

	// Token: 0x04002A5E RID: 10846
	public HelpContentPage helpPage;

	// Token: 0x04002A5F RID: 10847
	public int helpPageNumber;

	// Token: 0x04002A60 RID: 10848
	public int maxHelpPageNumber;

	// Token: 0x04002A61 RID: 10849
	public List<string> skipBlocks = new List<string>();

	// Token: 0x04002A62 RID: 10850
	public CanvasGroup tutorialCanvasGroup;

	// Token: 0x04002A63 RID: 10851
	[Header("State")]
	public bool active;

	// Token: 0x04002A64 RID: 10852
	public bool tutorialActive;

	// Token: 0x04002A65 RID: 10853
	public float appearProgress;

	// Token: 0x04002A66 RID: 10854
	public List<string> buttonActions = new List<string>();

	// Token: 0x04002A67 RID: 10855
	public bool anyButtonPressCloses = true;

	// Token: 0x04002A68 RID: 10856
	public bool affectPauseState = true;

	// Token: 0x04002A69 RID: 10857
	private bool setupNav;

	// Token: 0x04002A6F RID: 10863
	private static PopupMessageController _instance;

	// Token: 0x020005BF RID: 1471
	public enum AffectPauseState
	{
		// Token: 0x04002A71 RID: 10865
		automatic,
		// Token: 0x04002A72 RID: 10866
		yes,
		// Token: 0x04002A73 RID: 10867
		no
	}

	// Token: 0x020005C0 RID: 1472
	// (Invoke) Token: 0x0600204C RID: 8268
	public delegate void LeftButton();

	// Token: 0x020005C1 RID: 1473
	// (Invoke) Token: 0x06002050 RID: 8272
	public delegate void RightButton();

	// Token: 0x020005C2 RID: 1474
	// (Invoke) Token: 0x06002054 RID: 8276
	public delegate void LeftButton2();

	// Token: 0x020005C3 RID: 1475
	// (Invoke) Token: 0x06002058 RID: 8280
	public delegate void RightButton2();

	// Token: 0x020005C4 RID: 1476
	// (Invoke) Token: 0x0600205C RID: 8284
	public delegate void OptionButton();
}
