using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000592 RID: 1426
public class StateElementController : ButtonController
{
	// Token: 0x06001F23 RID: 7971 RVA: 0x001AF6E0 File Offset: 0x001AD8E0
	public void Setup(StatusController.StatusInstance newInstance)
	{
		this.statusInstance = newInstance;
		this.preset = this.statusInstance.preset;
		this.SetupReferences();
		if (!this.preset.enableProgressBar)
		{
			this.progressBar.gameObject.SetActive(false);
		}
		this.detailText.canvasRenderer.SetAlpha(0f);
		this.VisualUpdate();
		this.SetMinimized(false);
		if (this.maximized)
		{
			this.SetMaximized(false);
		}
		if (this.preset.displayTotalFineWhenMinimized)
		{
			this.fineText.canvasRenderer.SetAlpha(0f);
			this.fineText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x001AE20D File Offset: 0x001AC40D
	private void OnEnable()
	{
		this.VisualUpdate();
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x001AF790 File Offset: 0x001AD990
	public override void VisualUpdate()
	{
		if (this.preset == null)
		{
			return;
		}
		StatusController.Instance.activeStatusCounts.TryGetValue(this.statusInstance, ref this.counts);
		this.fineTotal = 0;
		Dictionary<StatusPreset.StatusCountConfig, List<StatusController.StatusCount>> dictionary = null;
		Dictionary<StatusPreset.StatusCountConfig, int> dictionary2 = null;
		if (this.preset.listCountsInDetailText || this.preset.displayFineTotalInMainText || this.preset.displayTotalFineWhenMinimized)
		{
			dictionary = new Dictionary<StatusPreset.StatusCountConfig, List<StatusController.StatusCount>>();
			dictionary2 = new Dictionary<StatusPreset.StatusCountConfig, int>();
			for (int i = 0; i < this.counts.Count; i++)
			{
				StatusController.StatusCount statusCount = this.counts[i];
				if (!dictionary.ContainsKey(statusCount.statusCountConfig))
				{
					dictionary.Add(statusCount.statusCountConfig, new List<StatusController.StatusCount>());
					dictionary2.Add(statusCount.statusCountConfig, 0);
				}
				dictionary[statusCount.statusCountConfig].Add(statusCount);
				int penaltyAmount = statusCount.GetPenaltyAmount();
				this.fineTotal += penaltyAmount;
				Dictionary<StatusPreset.StatusCountConfig, int> dictionary3 = dictionary2;
				StatusPreset.StatusCountConfig statusCountConfig = statusCount.statusCountConfig;
				dictionary3[statusCountConfig] += penaltyAmount;
			}
		}
		string text = string.Empty;
		if (this.isWanted)
		{
			text = "_w";
		}
		this.mainText.text = Strings.Get("ui.states", this.preset.name + text, Strings.Casing.asIs, false, false, true, Player.Instance);
		if (this.preset.displayCountCountsInMainText)
		{
			TextMeshProUGUI textMeshProUGUI = this.mainText;
			textMeshProUGUI.text = textMeshProUGUI.text + " (" + this.counts.Count.ToString() + ")";
		}
		if (this.preset.displayFineTotalInMainText)
		{
			TextMeshProUGUI textMeshProUGUI2 = this.mainText;
			textMeshProUGUI2.text = textMeshProUGUI2.text + " " + CityControls.Instance.cityCurrency + this.fineTotal.ToString();
		}
		this.detailText.text = string.Empty;
		bool flag = false;
		if (this.preset.includeDescription)
		{
			if (this.preset.replaceDescriptionBasedOnCounts)
			{
				TextMeshProUGUI textMeshProUGUI3 = this.detailText;
				textMeshProUGUI3.text += Strings.Get("ui.states", this.preset.name + "_description_" + this.counts[0].statusCountConfig.name, Strings.Casing.asIs, false, false, true, Player.Instance);
			}
			else
			{
				TextMeshProUGUI textMeshProUGUI4 = this.detailText;
				textMeshProUGUI4.text += Strings.Get("ui.states", this.preset.name + "_description" + text, Strings.Casing.asIs, false, false, true, Player.Instance);
			}
			if (this.preset.displayAddressInDetailText && this.statusInstance.address != null)
			{
				TextMeshProUGUI textMeshProUGUI5 = this.detailText;
				textMeshProUGUI5.text = textMeshProUGUI5.text + ": " + this.statusInstance.address.name;
			}
			if (this.preset.displayBuildingInDetailText && this.statusInstance.building != null)
			{
				TextMeshProUGUI textMeshProUGUI6 = this.detailText;
				textMeshProUGUI6.text = textMeshProUGUI6.text + ": " + this.statusInstance.building.name;
			}
			if (this.preset.listCountsInDetailText)
			{
				foreach (KeyValuePair<StatusPreset.StatusCountConfig, List<StatusController.StatusCount>> keyValuePair in dictionary)
				{
					string text2 = string.Empty;
					text2 = Strings.Get("ui.states", keyValuePair.Key.name + "_description", Strings.Casing.asIs, false, false, true, Player.Instance);
					string text3 = string.Empty;
					string text4 = string.Empty;
					if (keyValuePair.Value.Exists((StatusController.StatusCount item) => item.fineRecord.confirmed))
					{
						flag = true;
						text3 = "<u>";
						text4 = "</u>";
					}
					TextMeshProUGUI textMeshProUGUI7 = this.detailText;
					textMeshProUGUI7.text = string.Concat(new string[]
					{
						textMeshProUGUI7.text,
						"\n\n<margin-left=5%><size=90%><line-height=80%>-",
						text3,
						text2,
						text4,
						" (",
						CityControls.Instance.cityCurrency,
						dictionary2[keyValuePair.Key].ToString(),
						")"
					});
				}
			}
		}
		this.maximizedHeight = StatusController.Instance.elementDefaultHeight + this.detailText.preferredHeight + 14f;
		this.detailText.canvasRenderer.SetAlpha(StatusController.Instance.detailTextFadeInCurve.Evaluate(this.heightResizingProgress));
		Color colour = this.GetColour();
		this.icon.sprite = this.preset.icon;
		if (this.isWanted)
		{
			this.icon.sprite = this.preset.alternateIcon;
		}
		this.icon.color = colour;
		this.mainText.color = colour;
		this.juice.flashColour = colour;
		this.juice.pulsateColour = colour;
		if (this.progressBarImg != null)
		{
			this.progressBarImg.color = colour;
		}
		if ((this.preset.pulseBackground || this.isWanted || flag) && !this.juice.pulsateActive)
		{
			this.juice.Pulsate(true, false);
		}
		if (this.preset.pulseIcon || this.isWanted)
		{
			this.iconJuice.elements[0].originalColour = this.GetColour();
			this.iconJuice.pulsateColour = this.GetColour() + this.preset.pulseIconAdditiveColour;
			if (!this.iconJuice.pulsateActive)
			{
				this.iconJuice.Pulsate(true, false);
			}
		}
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x001AFD90 File Offset: 0x001ADF90
	public void SetRemove(bool val)
	{
		if (this.removing != val)
		{
			this.removing = val;
			this.removalTimer = 1f;
			this.detailText.gameObject.SetActive(!this.removing);
			this.juice.enabled = !this.removing;
			this.iconJuice.enabled = !this.removing;
			if (!this.removing)
			{
				if (this.xIcon != null)
				{
					Object.Destroy(this.xIcon.gameObject);
				}
				foreach (CanvasRenderer canvasRenderer in this.renderElements)
				{
					canvasRenderer.SetAlpha(1f);
				}
				this.detailText.canvasRenderer.SetAlpha(StatusController.Instance.detailTextFadeInCurve.Evaluate(this.heightResizingProgress));
				return;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.statusRemovedIcon, this.icon.transform);
			gameObject.GetComponent<Image>().color = this.mainText.color;
			this.xIcon = gameObject.GetComponent<RectTransform>();
			this.xIconRend = gameObject.GetComponent<CanvasRenderer>();
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x001AFEDC File Offset: 0x001AE0DC
	public Color GetColour()
	{
		Color color = this.preset.color;
		if (this.isWanted)
		{
			color = this.preset.alternateColour;
		}
		if (this.preset.overrrideColorWithCount)
		{
			StatusPreset.StatusCountConfig statusCountConfig = null;
			foreach (StatusController.StatusCount statusCount in this.counts)
			{
				if (statusCountConfig == null || statusCount.statusCountConfig.penalty > statusCountConfig.penalty)
				{
					statusCountConfig = statusCount.statusCountConfig;
					color = statusCount.statusCountConfig.colour;
				}
			}
		}
		if (this.preset.fadeToWhite && StatusController.Instance.activeStatusCounts.ContainsKey(this.statusInstance) && StatusController.Instance.activeStatusCounts[this.statusInstance].Count > 0)
		{
			color = Color.Lerp(Color.white, color, StatusController.Instance.activeStatusCounts[this.statusInstance][0].amount);
		}
		return color;
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x001AFFF0 File Offset: 0x001AE1F0
	public void SetMinimized(bool val)
	{
		if (this.minimized != val)
		{
			this.minimized = val;
			if (!this.minimized)
			{
				this.minimizeTimer = 2f;
			}
		}
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x001B0015 File Offset: 0x001AE215
	public void SetMaximized(bool val)
	{
		if (this.maximized != val)
		{
			this.maximized = val;
			if (this.maximized)
			{
				this.maximizeTimer = 2f;
			}
		}
	}

	// Token: 0x040028EF RID: 10479
	public TextMeshProUGUI mainText;

	// Token: 0x040028F0 RID: 10480
	public TextMeshProUGUI detailText;

	// Token: 0x040028F1 RID: 10481
	public TextMeshProUGUI fineText;

	// Token: 0x040028F2 RID: 10482
	public int displayedFine;

	// Token: 0x040028F3 RID: 10483
	public int fineTotal;

	// Token: 0x040028F4 RID: 10484
	public JuiceController iconJuice;

	// Token: 0x040028F5 RID: 10485
	public RectTransform progressBar;

	// Token: 0x040028F6 RID: 10486
	public Image progressBarImg;

	// Token: 0x040028F7 RID: 10487
	public StatusPreset preset;

	// Token: 0x040028F8 RID: 10488
	public StatusController.StatusInstance statusInstance;

	// Token: 0x040028F9 RID: 10489
	private List<StatusController.StatusCount> counts = new List<StatusController.StatusCount>();

	// Token: 0x040028FA RID: 10490
	public List<CanvasRenderer> renderElements = new List<CanvasRenderer>();

	// Token: 0x040028FB RID: 10491
	public bool minimized;

	// Token: 0x040028FC RID: 10492
	public float minimizeTimer = 2f;

	// Token: 0x040028FD RID: 10493
	public float widthResizingProgress = 1f;

	// Token: 0x040028FE RID: 10494
	public bool removing;

	// Token: 0x040028FF RID: 10495
	public float removalTimer = 1f;

	// Token: 0x04002900 RID: 10496
	public RectTransform xIcon;

	// Token: 0x04002901 RID: 10497
	public CanvasRenderer xIconRend;

	// Token: 0x04002902 RID: 10498
	public float maximizeTimer = 2f;

	// Token: 0x04002903 RID: 10499
	public bool maximized;

	// Token: 0x04002904 RID: 10500
	public float heightResizingProgress;

	// Token: 0x04002905 RID: 10501
	public float maximizedHeight = 42f;

	// Token: 0x04002906 RID: 10502
	public bool isWanted;
}
