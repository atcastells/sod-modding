using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005BC RID: 1468
public class ProgressBarController : MonoBehaviour
{
	// Token: 0x14000040 RID: 64
	// (add) Token: 0x06002029 RID: 8233 RVA: 0x001BA65C File Offset: 0x001B885C
	// (remove) Token: 0x0600202A RID: 8234 RVA: 0x001BA694 File Offset: 0x001B8894
	public event ProgressBarController.ValueChange OnProgressChange;

	// Token: 0x0600202B RID: 8235 RVA: 0x001BA6CC File Offset: 0x001B88CC
	private void Awake()
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		if (this.usePips)
		{
			this.SetupPips();
		}
		if (this.displayProgress)
		{
			this.progressTextRect = this.progressText.gameObject.GetComponent<RectTransform>();
		}
		else if (this.progressText != null)
		{
			Object.Destroy(this.progressText);
		}
		this.VisualUpdate();
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x001BA738 File Offset: 0x001B8938
	public void SetupPips()
	{
		this.displayProgress = false;
		foreach (ProgressBarPipController progressBarPipController in this.pips)
		{
			Object.Destroy(progressBarPipController.gameObject);
		}
		this.pips.Clear();
		for (int i = 0; i < this.pipNumber; i++)
		{
			GameObject gameObject;
			if (this.pipObject != null)
			{
				gameObject = Object.Instantiate<GameObject>(this.pipObject, this.barRect);
			}
			else
			{
				gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.progressBarPip, this.barRect);
			}
			ProgressBarPipController component = gameObject.GetComponent<ProgressBarPipController>();
			component.bar = this;
			component.rect = component.gameObject.GetComponent<RectTransform>();
			component.img = component.gameObject.GetComponent<Image>();
			this.pips.Add(component);
		}
		if (this.progressRect != null)
		{
			Object.Destroy(this.progressRect.gameObject);
		}
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x001BA848 File Offset: 0x001B8A48
	private void Start()
	{
		if (this.setNameOnStart)
		{
			this.SetName(this.barName);
		}
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x001BA860 File Offset: 0x001B8A60
	public void SetName(string newName)
	{
		if (this.barTitle == null)
		{
			return;
		}
		this.barTitle.text = Strings.Get("ui.interface", newName, Strings.Casing.asIs, false, false, false, null);
		base.gameObject.name = newName + " bar";
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x001BA8B0 File Offset: 0x001B8AB0
	public void SetValue(float setTo)
	{
		this.value = Mathf.Clamp(setTo, this.barMin, this.barMax);
		this.progress = (this.value - this.barMin) / (this.barMax - this.barMin);
		if (this.useFloorValueForPercent)
		{
			this.progressInt = Mathf.Clamp(Mathf.FloorToInt(this.progress * 100f), 0, 100);
		}
		else
		{
			this.progressInt = Mathf.Clamp(Mathf.RoundToInt(this.progress * 100f), 0, 100);
		}
		this.pipValue = Mathf.RoundToInt(this.progress * (float)this.pipNumber);
		this.VisualUpdate();
		if (this.OnProgressChange != null)
		{
			this.OnProgressChange(this.value, this.progressInt);
		}
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x001BA97C File Offset: 0x001B8B7C
	public void SetSecondaryValue(float setTo)
	{
		if (!this.useSecondaryPipValue)
		{
			return;
		}
		this.secondaryValue = Mathf.Clamp(setTo, this.barMin, this.barMax);
		this.secondaryProgress = (this.secondaryValue - this.barMin) / (this.barMax - this.barMin);
		this.secondaryPipValue = Mathf.RoundToInt(this.secondaryProgress * (float)this.pipNumber);
		this.VisualUpdate();
		if (this.OnProgressChange != null)
		{
			this.OnProgressChange(this.value, this.progressInt);
		}
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x001BAA0C File Offset: 0x001B8C0C
	public void VisualUpdate()
	{
		if (this.displayProgress & this.progressText != null)
		{
			this.progressText.text = this.progressInt.ToString();
			if (this.displayPercentageSign)
			{
				TextMeshProUGUI textMeshProUGUI = this.progressText;
				textMeshProUGUI.text += "%";
			}
		}
		if (this.progressRect != null)
		{
			float num = this.barRect.rect.width * this.progress;
			this.progressRect.offsetMax = new Vector2(num, this.progressRect.offsetMax.y);
		}
		if (this.usePips)
		{
			if (this.barRect != null)
			{
				this.pipXSize = this.barRect.rect.width / (float)this.pipNumber;
			}
			for (int i = 0; i < this.pips.Count; i++)
			{
				ProgressBarPipController progressBarPipController = this.pips[i];
				if (progressBarPipController == null)
				{
					this.pips.RemoveAt(i);
					i--;
				}
				else
				{
					progressBarPipController.rect.anchoredPosition = new Vector2((float)i * this.pipXSize, progressBarPipController.rect.anchoredPosition.y);
					progressBarPipController.rect.sizeDelta = new Vector2(this.pipXSize, progressBarPipController.rect.sizeDelta.y);
					if (i < this.pipValue)
					{
						progressBarPipController.SetFilled(true, true);
					}
					else if (this.useSecondaryPipValue && i < this.secondaryPipValue)
					{
						progressBarPipController.SetFilled(false, true);
					}
					else
					{
						progressBarPipController.SetFilled(false, false);
					}
				}
			}
		}
		if (this.displayProgress)
		{
			float width = this.progressRect.rect.width;
			float num2 = this.barRect.rect.width - width;
			if (width >= this.progressTextRect.sizeDelta.x)
			{
				this.progressTextRect.anchorMin = new Vector2(1f, 0.5f);
				this.progressTextRect.anchorMax = new Vector2(1f, 0.5f);
				this.progressTextRect.pivot = new Vector2(1f, 0.5f);
				this.progressText.alignment = 4100;
				this.progressTextRect.sizeDelta = new Vector2(75f, this.progressTextRect.sizeDelta.y);
				this.progressTextRect.localPosition = new Vector2(this.progressRect.rect.width - 6f, this.progressTextRect.localPosition.y);
				return;
			}
			if (num2 >= this.progressTextRect.sizeDelta.x)
			{
				this.progressTextRect.anchorMin = new Vector2(0f, 0.5f);
				this.progressTextRect.anchorMax = new Vector2(0f, 0.5f);
				this.progressTextRect.pivot = new Vector2(0f, 0.5f);
				this.progressText.alignment = 4097;
				this.progressTextRect.sizeDelta = new Vector2(75f, this.progressTextRect.sizeDelta.y);
				this.progressTextRect.localPosition = new Vector2(this.progressRect.rect.width + 6f, this.progressTextRect.localPosition.y);
				return;
			}
			this.progressTextRect.anchorMin = new Vector2(0.5f, 0.5f);
			this.progressTextRect.anchorMax = new Vector2(0.5f, 0.5f);
			this.progressTextRect.pivot = new Vector2(0.5f, 0.5f);
			this.progressText.alignment = 4098;
			this.progressTextRect.sizeDelta = new Vector2(this.barRect.rect.width, this.progressTextRect.sizeDelta.y);
			this.progressTextRect.localPosition = new Vector2(this.barRect.rect.width * 0.5f, this.progressTextRect.localPosition.y);
		}
	}

	// Token: 0x04002A29 RID: 10793
	public string barName = "New Bar";

	// Token: 0x04002A2A RID: 10794
	public float value;

	// Token: 0x04002A2B RID: 10795
	public float secondaryValue;

	// Token: 0x04002A2C RID: 10796
	public float barMin;

	// Token: 0x04002A2D RID: 10797
	public float barMax = 1f;

	// Token: 0x04002A2E RID: 10798
	public float progress;

	// Token: 0x04002A2F RID: 10799
	public float secondaryProgress;

	// Token: 0x04002A30 RID: 10800
	private int progressInt;

	// Token: 0x04002A31 RID: 10801
	public bool usePips;

	// Token: 0x04002A32 RID: 10802
	public GameObject pipObject;

	// Token: 0x04002A33 RID: 10803
	public int pipValue;

	// Token: 0x04002A34 RID: 10804
	public int pipNumber = 5;

	// Token: 0x04002A35 RID: 10805
	public bool useSecondaryPipValue;

	// Token: 0x04002A36 RID: 10806
	public int secondaryPipValue;

	// Token: 0x04002A37 RID: 10807
	public bool displayProgress = true;

	// Token: 0x04002A38 RID: 10808
	public bool displayPercentageSign;

	// Token: 0x04002A39 RID: 10809
	public bool setNameOnStart = true;

	// Token: 0x04002A3A RID: 10810
	public bool useFloorValueForPercent = true;

	// Token: 0x04002A3C RID: 10812
	public RectTransform rect;

	// Token: 0x04002A3D RID: 10813
	public TextMeshProUGUI barTitle;

	// Token: 0x04002A3E RID: 10814
	public TextMeshProUGUI progressText;

	// Token: 0x04002A3F RID: 10815
	public RectTransform barRect;

	// Token: 0x04002A40 RID: 10816
	public RectTransform progressRect;

	// Token: 0x04002A41 RID: 10817
	private RectTransform progressTextRect;

	// Token: 0x04002A42 RID: 10818
	private float pipXSize = 1f;

	// Token: 0x04002A43 RID: 10819
	public List<ProgressBarPipController> pips = new List<ProgressBarPipController>();

	// Token: 0x04002A44 RID: 10820
	public ProgressBarPipController hoverOverPip;

	// Token: 0x020005BD RID: 1469
	// (Invoke) Token: 0x06002034 RID: 8244
	public delegate void ValueChange(float newValue, int percentage);
}
