using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004DE RID: 1246
public class PinnedQuickMenuController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001AEC RID: 6892 RVA: 0x00189A4C File Offset: 0x00187C4C
	public void Setup(PinnedItemController newParent)
	{
		this.active = true;
		this.parentPinned = newParent;
		foreach (CanvasRenderer canvasRenderer in this.renderers)
		{
			canvasRenderer.SetAlpha(0f);
		}
		if (this.parentPinned.evidence != null)
		{
			if (!(this.parentPinned.evidence is EvidenceLocation))
			{
				if (this.locateOnMapButton != null)
				{
					Object.Destroy(this.locateOnMapButton.gameObject);
				}
				if (this.plotRouteButton != null)
				{
					Object.Destroy(this.plotRouteButton.gameObject);
				}
			}
			if (InterfaceController.Instance.selectedPinned.Count > 1)
			{
				if (this.locateOnMapButton != null)
				{
					Object.Destroy(this.locateOnMapButton.gameObject);
				}
				if (this.plotRouteButton != null)
				{
					Object.Destroy(this.plotRouteButton.gameObject);
				}
				if (this.newLinkButton != null)
				{
					Object.Destroy(this.newLinkButton.gameObject);
				}
			}
		}
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = Vector3.one;
		float num = 128f;
		if (this.parentPinned.caseElement.m)
		{
			num = 16f;
		}
		base.transform.position = this.parentPinned.transform.position - new Vector3(0f, num * this.parentPinned.transform.lossyScale.y, 0f);
		if (this.locateOnMapButton != null)
		{
			this.activeButtons.Add(this.locateOnMapButton);
		}
		if (this.plotRouteButton != null)
		{
			this.activeButtons.Add(this.plotRouteButton);
		}
		if (this.toggleCollapseButton != null)
		{
			this.activeButtons.Add(this.toggleCollapseButton);
		}
		if (this.toggleCrossOutButton != null)
		{
			this.activeButtons.Add(this.toggleCrossOutButton);
		}
		if (this.stickyNoteButton != null)
		{
			this.activeButtons.Add(this.stickyNoteButton);
		}
		if (this.newLinkButton != null)
		{
			this.activeButtons.Add(this.newLinkButton);
		}
		if (this.contextMenuButton != null)
		{
			this.activeButtons.Add(this.contextMenuButton);
		}
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x00189CD8 File Offset: 0x00187ED8
	public void Remove(bool instant = false)
	{
		Game.Log("Interface: Remove quick menu", 2);
		this.active = false;
		PinnedItemController.activeQuickMenu = null;
		if (instant)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x00189D00 File Offset: 0x00187F00
	private void Update()
	{
		float num = 128f;
		if (this.parentPinned != null && this.parentPinned.transform != null && this != null && base.transform != null)
		{
			base.transform.rotation = Quaternion.identity;
			if (this.parentPinned.caseElement.m)
			{
				num = 16f;
			}
			base.transform.position = this.parentPinned.transform.position - new Vector3(0f, num * this.parentPinned.transform.lossyScale.y, 0f);
		}
		if (this.active && this.appearProgress < 1f)
		{
			this.appearProgress += Time.deltaTime / 0.25f;
			this.appearProgress = Mathf.Clamp01(this.appearProgress);
			using (List<CanvasRenderer>.Enumerator enumerator = this.renderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CanvasRenderer canvasRenderer = enumerator.Current;
					if (!(canvasRenderer == null))
					{
						canvasRenderer.SetAlpha(this.appearProgress);
					}
				}
				goto IL_1B9;
			}
		}
		if (!this.active && this.appearProgress > 0f)
		{
			this.appearProgress -= Time.deltaTime / 0.2f;
			this.appearProgress = Mathf.Clamp01(this.appearProgress);
			foreach (CanvasRenderer canvasRenderer2 in this.renderers)
			{
				if (!(canvasRenderer2 == null))
				{
					canvasRenderer2.SetAlpha(this.appearProgress);
				}
			}
		}
		IL_1B9:
		if (!this.active && this.appearProgress <= 0f && this != null && base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.parentPinned == null && this != null && base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x00189F4C File Offset: 0x0018814C
	public void LocateOnMapButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.LocateOnMap();
		}
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x00189F67 File Offset: 0x00188167
	public void PlotRouteButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.PlotRoute();
		}
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x00189F82 File Offset: 0x00188182
	public void ToggleCollapseButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.ToggleCollapse();
		}
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x00189F9D File Offset: 0x0018819D
	public void ToggleCrossOutButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.ToggleCrossedOut();
		}
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x00189FB8 File Offset: 0x001881B8
	public void StickyNoteButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.NewStickyNote();
		}
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x00189FD3 File Offset: 0x001881D3
	public void NewLinkButton()
	{
		if (this.parentPinned != null)
		{
			CasePanelController.Instance.CustomStringLinkSelection(this.parentPinned, false);
		}
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x00189FF4 File Offset: 0x001881F4
	public void ContextMenuButton()
	{
		if (this.parentPinned != null)
		{
			this.parentPinned.UpdateContextMenuOptions();
			if (this.parentPinned.contextMenu != null)
			{
				this.parentPinned.contextMenu.OpenMenu();
			}
		}
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x0018A032 File Offset: 0x00188232
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.isOver = true;
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x0018A03B File Offset: 0x0018823B
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		this.isOver = false;
		if (this.parentPinned != null && !this.parentPinned.isOver)
		{
			this.Remove(false);
		}
	}

	// Token: 0x04002394 RID: 9108
	[Header("Components")]
	public List<CanvasRenderer> renderers = new List<CanvasRenderer>();

	// Token: 0x04002395 RID: 9109
	public PinnedItemController parentPinned;

	// Token: 0x04002396 RID: 9110
	public ButtonController locateOnMapButton;

	// Token: 0x04002397 RID: 9111
	public ButtonController plotRouteButton;

	// Token: 0x04002398 RID: 9112
	public ButtonController toggleCollapseButton;

	// Token: 0x04002399 RID: 9113
	public ButtonController toggleCrossOutButton;

	// Token: 0x0400239A RID: 9114
	public ButtonController stickyNoteButton;

	// Token: 0x0400239B RID: 9115
	public ButtonController newLinkButton;

	// Token: 0x0400239C RID: 9116
	public ButtonController contextMenuButton;

	// Token: 0x0400239D RID: 9117
	public List<ButtonController> activeButtons = new List<ButtonController>();

	// Token: 0x0400239E RID: 9118
	[Header("State")]
	public bool isOver;

	// Token: 0x0400239F RID: 9119
	public bool active = true;

	// Token: 0x040023A0 RID: 9120
	public float appearProgress;
}
