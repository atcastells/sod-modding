using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020005FE RID: 1534
public class TooltipController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x14000049 RID: 73
	// (add) Token: 0x060021C4 RID: 8644 RVA: 0x001CABE0 File Offset: 0x001C8DE0
	// (remove) Token: 0x060021C5 RID: 8645 RVA: 0x001CAC18 File Offset: 0x001C8E18
	public event TooltipController.BeforeTooltipSpawn OnBeforeTooltipSpawn;

	// Token: 0x060021C6 RID: 8646 RVA: 0x001CAC50 File Offset: 0x001C8E50
	private void Start()
	{
		this.outline = base.gameObject.GetComponent<Outline>();
		this.img = base.gameObject.GetComponent<Image>();
		if (this.img != null)
		{
			this.originalSprite = this.img.sprite;
		}
		this.contextMenuBelongingToThis = base.gameObject.GetComponent<ContextMenuController>();
		this.GetText();
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x001CACB5 File Offset: 0x001C8EB5
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			this.SetPointerOver(true);
		}
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x001CACCA File Offset: 0x001C8ECA
	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		if (InputController.Instance.mouseInputMode)
		{
			this.SetPointerOver(false);
		}
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x001CACF3 File Offset: 0x001C8EF3
	public virtual void OnButtonHover()
	{
		this.SetPointerOver(true);
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x001CACFC File Offset: 0x001C8EFC
	public virtual void OnButtonExitHover()
	{
		this.SetPointerOver(false);
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x001CAD08 File Offset: 0x001C8F08
	public virtual void SetPointerOver(bool val)
	{
		if (this.isOver != val)
		{
			this.isOver = val;
			if (val)
			{
				if (InterfaceControls.Instance.enableTooltips && this.tooltipEnabled && this.handleOwnBehaviour)
				{
					this.moTimer = 0f;
					this.fadeIn = 0f;
					this.OnMouseEnterCustom();
					base.StartCoroutine("MouseOver");
					if (this.outline != null && this.enableOutlineMouseOver)
					{
						this.outline.enabled = true;
					}
					if (this.mouseOverSprite != null)
					{
						this.img.sprite = this.mouseOverSprite;
						return;
					}
				}
			}
			else
			{
				base.StopCoroutine("MouseOver");
				if (this.handleOwnBehaviour)
				{
					this.ForceClose();
				}
			}
		}
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnMouseEnterCustom()
	{
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x001CADD8 File Offset: 0x001C8FD8
	public virtual void GetText()
	{
		this.ForceClose();
		if (this.useMainDictionaryEntry && this.mainDictionaryKey.Length > 0)
		{
			this.mainText = Strings.Get(this.mainDictionary, this.mainDictionaryKey, Strings.Casing.asIs, false, false, false, null);
		}
		if (this.useDetailDictionaryEntry && this.detailDictionaryKey.Length > 0)
		{
			this.detailText = Strings.Get(this.detailDictionary, this.detailDictionaryKey, Strings.Casing.asIs, false, false, false, null);
		}
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x001CAE4F File Offset: 0x001C904F
	private IEnumerator MouseOver()
	{
		while (this.isOver)
		{
			this.moTimer += Time.deltaTime;
			if (this.mainText != null && this.detailText != null && this.mainText.Length + this.detailText.Length > 0)
			{
				if (this.spawnedTooltip == null)
				{
					this.contextMenuBelongingToThis = base.gameObject.GetComponent<ContextMenuController>();
					if ((this.contextMenuBelongingToThis == null || this.contextMenuBelongingToThis.spawnedMenu == null) && this.moTimer > InterfaceControls.Instance.toolTipDelay + this.additionalSpawnDelay)
					{
						if (TooltipController.activeTooltip != null)
						{
							TooltipController.activeTooltip.ForceClose();
						}
						this.OpenTooltip();
					}
				}
				else
				{
					if (this.updateTooltipPosition)
					{
						RectTransform component = base.gameObject.GetComponent<RectTransform>();
						RectTransform component2 = this.spawnedTooltip.GetComponent<RectTransform>();
						this.spawnedTooltip.transform.SetParent(component);
						if (this.useCursorPos && InputController.Instance.mouseInputMode)
						{
							Vector2 vector;
							vector..ctor(this.pos.x * component.sizeDelta.x, this.pos.y * component.sizeDelta.y);
							RectTransformUtility.ScreenPointToLocalPointInRectangle(component, Input.mousePosition, null, ref vector);
							component2.localPosition = vector;
							component2.position += new Vector3(this.cursorPosOffset.x, this.cursorPosOffset.y, 0f);
						}
						else
						{
							component2.position = base.transform.TransformPoint(new Vector2(this.pos.x * component.sizeDelta.x, this.pos.y * component.sizeDelta.y));
						}
						if (this.parentOverride == null)
						{
							this.spawnedTooltip.transform.SetParent(PrefabControls.Instance.tooltipsContainer);
						}
						else
						{
							this.spawnedTooltip.transform.SetParent(this.parentOverride);
						}
					}
					if (this.fadeIn < 1f)
					{
						this.fadeIn += Time.deltaTime * InterfaceControls.Instance.toolTipFadeInSpeed;
						this.fadeIn = Mathf.Clamp01(this.fadeIn);
						this.rend.SetAlpha(this.fadeIn);
						this.textRend.SetAlpha(this.fadeIn);
						if (this.tooltipText != null)
						{
							this.tooltipText.ForceMeshUpdate(false, false);
						}
					}
				}
			}
			if (!this.tooltipEnabled)
			{
				this.ForceClose();
				break;
			}
			if (this.spawnedTooltip != null && TooltipController.activeTooltip != this)
			{
				this.ForceClose();
				break;
			}
			yield return null;
		}
		this.ForceClose();
		yield break;
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x001CAE60 File Offset: 0x001C9060
	public void OpenTooltip()
	{
		TooltipController.activeTooltip = this;
		if (this.OnBeforeTooltipSpawn != null)
		{
			this.OnBeforeTooltipSpawn();
		}
		this.spawnedTooltip = Object.Instantiate<GameObject>(PrefabControls.Instance.tooltip, base.transform);
		this.tooltipText = this.spawnedTooltip.GetComponentInChildren<TextMeshProUGUI>();
		this.tooltipText.color = InterfaceControls.Instance.defaultTextColour;
		if (this.detailText.Length > 0)
		{
			this.tooltipText.text = ("<b>" + this.mainText + "</b> <line-height=130%>\n<line-height=100%><alpha=#AA>" + this.detailText).Trim();
		}
		else
		{
			this.tooltipText.text = ("<b>" + this.mainText + "</b>").Trim();
		}
		RectTransform component = this.spawnedTooltip.GetComponent<RectTransform>();
		if (!this.limitWidth)
		{
			component.sizeDelta = new Vector2(Mathf.Min(InterfaceControls.Instance.tooltipWidth, this.tooltipText.preferredWidth + 20f) + (float)this.extendTooltipWidth, component.sizeDelta.y);
		}
		else
		{
			component.sizeDelta = new Vector2(InterfaceControls.Instance.tooltipWidth + (float)this.extendTooltipWidth, component.sizeDelta.y);
		}
		component.sizeDelta = new Vector2(component.sizeDelta.x, this.tooltipText.preferredHeight + 22f);
		RectTransform component2 = base.gameObject.GetComponent<RectTransform>();
		if (this.useCursorPos && InputController.Instance.mouseInputMode)
		{
			Vector2 vector;
			vector..ctor(this.pos.x * component2.sizeDelta.x, this.pos.y * component2.sizeDelta.y);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(component2, this.ClampToWindow(Input.mousePosition), null, ref vector);
			component.localPosition = vector;
			component.position += new Vector3(this.cursorPosOffset.x, this.cursorPosOffset.y, 0f);
		}
		else
		{
			component.position = base.transform.TransformPoint(new Vector2(this.pos.x * component2.sizeDelta.x, this.pos.y * component2.sizeDelta.y));
		}
		this.spawnedTooltip.transform.SetParent(PrefabControls.Instance.tooltipsContainer);
		this.spawnedTooltip.transform.SetAsLastSibling();
		component.localScale = Vector3.one;
		component.localEulerAngles = Vector3.zero;
		this.CustomPositioning();
		component.localPosition = new Vector2(Mathf.Clamp(component.localPosition.x, (float)Screen.width * -0.5f, (float)Screen.width * 0.5f - component.sizeDelta.x), Mathf.Clamp(component.localPosition.y, (float)Screen.height * -0.5f + component.sizeDelta.y, (float)Screen.height * 0.5f));
		ActiveTooltip component3 = this.spawnedTooltip.GetComponent<ActiveTooltip>();
		component3.ttc = this;
		component3.setupComplete = true;
		this.rend = this.spawnedTooltip.GetComponent<CanvasRenderer>();
		this.textRend = this.tooltipText.GetComponent<CanvasRenderer>();
		this.fadeIn = 0f;
		this.rend.SetAlpha(0f);
		this.textRend.SetAlpha(0f);
		this.OnMouseOverCustom();
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x001CB1F0 File Offset: 0x001C93F0
	private Vector2 ClampToWindow(Vector2 rawPointerPosition)
	{
		Vector3[] array = new Vector3[4];
		PrefabControls.Instance.tooltipsCanvas.GetWorldCorners(array);
		float num = Mathf.Clamp(rawPointerPosition.x, array[0].x, array[2].x - this.spawnedTooltip.GetComponent<RectTransform>().sizeDelta.x);
		float num2 = Mathf.Clamp(rawPointerPosition.y, array[0].y, array[2].y);
		return new Vector2(num, num2);
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void CustomPositioning()
	{
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x001CB279 File Offset: 0x001C9479
	private void OnDisable()
	{
		if (this.spawnedTooltip != null)
		{
			this.ForceClose();
		}
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x001CB279 File Offset: 0x001C9479
	private void OnDestroy()
	{
		if (this.spawnedTooltip != null)
		{
			this.ForceClose();
		}
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x001CB28F File Offset: 0x001C948F
	public static void RemoveActiveTooltip()
	{
		if (TooltipController.activeTooltip != null)
		{
			TooltipController.activeTooltip.ForceClose();
		}
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x001CB2A8 File Offset: 0x001C94A8
	public void ForceClose()
	{
		this.isOver = false;
		this.moTimer = 0f;
		this.fadeIn = 0f;
		base.StopCoroutine("MouseOver");
		if (this.spawnedTooltip != null)
		{
			Object.Destroy(this.spawnedTooltip);
		}
		this.OnMouseOffCustom();
		if (this.outline != null && this.enableOutlineMouseOver)
		{
			this.outline.enabled = false;
		}
		if (this.mouseOverSprite != null)
		{
			this.img.sprite = this.originalSprite;
		}
		if (TooltipController.activeTooltip == this)
		{
			TooltipController.activeTooltip = null;
		}
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnMouseOverCustom()
	{
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void OnMouseOffCustom()
	{
	}

	// Token: 0x04002C43 RID: 11331
	[Header("Init")]
	public bool tooltipEnabled = true;

	// Token: 0x04002C44 RID: 11332
	public bool handleOwnBehaviour = true;

	// Token: 0x04002C45 RID: 11333
	[Tooltip("If this isn't null, the tooltip will spawn parented to this layer instead of the tooltip canvas.")]
	public RectTransform parentOverride;

	// Token: 0x04002C46 RID: 11334
	[Header("Text")]
	public bool useMainDictionaryEntry;

	// Token: 0x04002C47 RID: 11335
	public string mainDictionary = "ui.tooltips";

	// Token: 0x04002C48 RID: 11336
	public string mainDictionaryKey = string.Empty;

	// Token: 0x04002C49 RID: 11337
	public bool useDetailDictionaryEntry;

	// Token: 0x04002C4A RID: 11338
	public string detailDictionary = "ui.tooltips";

	// Token: 0x04002C4B RID: 11339
	public string detailDictionaryKey = string.Empty;

	// Token: 0x04002C4C RID: 11340
	public string mainText = string.Empty;

	// Token: 0x04002C4D RID: 11341
	public string detailText = string.Empty;

	// Token: 0x04002C4E RID: 11342
	[Header("State")]
	public bool isOver;

	// Token: 0x04002C4F RID: 11343
	[Tooltip("Added onto the default spawn time (seconds)")]
	[Header("Delay")]
	public float additionalSpawnDelay;

	// Token: 0x04002C50 RID: 11344
	public float moTimer;

	// Token: 0x04002C51 RID: 11345
	public GameObject spawnedTooltip;

	// Token: 0x04002C52 RID: 11346
	public TextMeshProUGUI tooltipText;

	// Token: 0x04002C53 RID: 11347
	public float fadeIn;

	// Token: 0x04002C54 RID: 11348
	public CanvasRenderer rend;

	// Token: 0x04002C55 RID: 11349
	public CanvasRenderer textRend;

	// Token: 0x04002C56 RID: 11350
	public Vector2 pos = new Vector2(0f, -0.5f);

	// Token: 0x04002C57 RID: 11351
	public bool useCursorPos;

	// Token: 0x04002C58 RID: 11352
	public Vector2 cursorPosOffset = new Vector2(10f, -7f);

	// Token: 0x04002C59 RID: 11353
	public bool limitWidth;

	// Token: 0x04002C5A RID: 11354
	public int extendTooltipWidth;

	// Token: 0x04002C5B RID: 11355
	private Outline outline;

	// Token: 0x04002C5C RID: 11356
	public bool enableOutlineMouseOver = true;

	// Token: 0x04002C5D RID: 11357
	private Image img;

	// Token: 0x04002C5E RID: 11358
	public Sprite mouseOverSprite;

	// Token: 0x04002C5F RID: 11359
	private Sprite originalSprite;

	// Token: 0x04002C60 RID: 11360
	public ContextMenuController contextMenuBelongingToThis;

	// Token: 0x04002C61 RID: 11361
	[Tooltip("While active, constantly update the tooltip postion")]
	public bool updateTooltipPosition;

	// Token: 0x04002C62 RID: 11362
	public static TooltipController activeTooltip;

	// Token: 0x020005FF RID: 1535
	// (Invoke) Token: 0x060021DA RID: 8666
	public delegate void BeforeTooltipSpawn();
}
