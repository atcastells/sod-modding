using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004EB RID: 1259
public class ContextMenuPanelController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06001B3B RID: 6971 RVA: 0x0018BB34 File Offset: 0x00189D34
	public void Setup(ContextMenuController newController)
	{
		this.cmc = newController;
		this.rect = base.gameObject.GetComponent<RectTransform>();
		float num = -4f;
		foreach (ContextMenuController.ContextMenuButtonSetup contextMenuButtonSetup in this.cmc.menuButtons)
		{
			if (!this.cmc.disabledItems.Contains(contextMenuButtonSetup.commandString) && (!contextMenuButtonSetup.devOnly || Game.Instance.devMode))
			{
				ContextButtonController component = Object.Instantiate<GameObject>(PrefabControls.Instance.contextMenuButton, this.rect).GetComponent<ContextButtonController>();
				component.Setup(this.cmc, this, contextMenuButtonSetup);
				this.spawnedButtons.Add(component);
				component.rect.anchoredPosition = new Vector2(0f, num);
				num -= 34f;
				float num2 = InterfaceControls.Instance.contextMenuWidth;
				if (!this.cmc.useGlobalWidth)
				{
					num2 = this.cmc.width;
				}
				this.rect.sizeDelta = new Vector2(num2, -num + 4f);
			}
		}
		for (int i = 0; i < this.spawnedButtons.Count; i++)
		{
			this.spawnedButtons[i].RefreshAutomaticNavigation();
		}
		if (!InputController.Instance.mouseInputMode && this.spawnedButtons.Count > 0)
		{
			if (InterfaceController.Instance.selectedElement != null)
			{
				InterfaceController.Instance.selectedElement.OnDeselect();
			}
			this.spawnedButtons[0].OnSelect();
		}
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x0018BCE8 File Offset: 0x00189EE8
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			this.isOver = true;
		}
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x0018BCFD File Offset: 0x00189EFD
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!InterfaceController.Instance.StupidUnityChangeToTheWayOnPointerExitHandles(eventData, base.transform))
		{
			return;
		}
		if (InputController.Instance.mouseInputMode)
		{
			this.isOver = false;
		}
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x0018BD28 File Offset: 0x00189F28
	private void Update()
	{
		if (!this.isOver && InputController.Instance.mouseInputMode && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
		{
			this.cmc.ForceClose();
		}
		if (this.cmc == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040023E4 RID: 9188
	public RectTransform rect;

	// Token: 0x040023E5 RID: 9189
	private bool isOver;

	// Token: 0x040023E6 RID: 9190
	public ContextMenuController cmc;

	// Token: 0x040023E7 RID: 9191
	public List<ContextButtonController> spawnedButtons = new List<ContextButtonController>();
}
