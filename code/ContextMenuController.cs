using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020004E7 RID: 1255
public class ContextMenuController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06001B2D RID: 6957 RVA: 0x0018B5C0 File Offset: 0x001897C0
	// (remove) Token: 0x06001B2E RID: 6958 RVA: 0x0018B5F8 File Offset: 0x001897F8
	public event ContextMenuController.OpenedMenu OnOpenMenu;

	// Token: 0x06001B2F RID: 6959 RVA: 0x0018B62D File Offset: 0x0018982D
	public void OnPointerClick(PointerEventData eventData)
	{
		if (InputController.Instance.mouseInputMode)
		{
			if (eventData.button == 1 && !this.useLeftButton)
			{
				this.OpenMenu();
			}
			if (eventData.button == null && this.useLeftButton)
			{
				this.OpenMenu();
			}
		}
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x0018B668 File Offset: 0x00189868
	public void OpenMenu()
	{
		if (this.spawnedMenu == null)
		{
			if (ContextMenuController.activeMenu != null)
			{
				ContextMenuController.activeMenu.ForceClose();
			}
			AudioController.Instance.Play2DSound(AudioControls.Instance.itemEditAppear, null, 1f);
			int num = 0;
			foreach (ContextMenuController.ContextMenuButtonSetup contextMenuButtonSetup in this.menuButtons)
			{
				if (!this.disabledItems.Contains(contextMenuButtonSetup.commandString) && (!contextMenuButtonSetup.devOnly || Game.Instance.devMode))
				{
					num++;
				}
			}
			if (num <= 0)
			{
				return;
			}
			ContextMenuController.activeMenu = this;
			TooltipController.RemoveActiveTooltip();
			this.spawnedMenu = Object.Instantiate<GameObject>(PrefabControls.Instance.contextMenuPanel, base.transform);
			this.spawnedMenu.GetComponent<ContextMenuPanelController>().Setup(this);
			this.menuRect = this.spawnedMenu.GetComponent<RectTransform>();
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			if (this.useCursorPos && InputController.Instance.mouseInputMode)
			{
				Vector2 vector;
				vector..ctor(this.pos.x * component.sizeDelta.x, this.pos.y * component.sizeDelta.y);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(component, this.ClampToWindow(Input.mousePosition), null, ref vector);
				this.menuRect.localPosition = vector;
				this.menuRect.position += new Vector3(this.cursorPosOffset.x, this.cursorPosOffset.y, 0f);
			}
			else
			{
				this.menuRect.position = base.transform.TransformPoint(new Vector2(this.pos.x * component.sizeDelta.x, this.pos.y * component.sizeDelta.y));
			}
			this.menuRect.SetParent(PrefabControls.Instance.contextMenuContainer);
			this.menuRect.rotation = Quaternion.identity;
			this.menuRect.localScale = Vector2.one;
			this.menuRect.localPosition = new Vector2(Mathf.Clamp(this.menuRect.localPosition.x, (float)Screen.width * -0.5f, (float)Screen.width * 0.5f - this.menuRect.sizeDelta.x), Mathf.Clamp(this.menuRect.localPosition.y, (float)Screen.height * -0.5f + this.menuRect.sizeDelta.y, (float)Screen.height * 0.5f));
			this.menuRect.SetAsLastSibling();
			if (this.OnOpenMenu != null)
			{
				this.OnOpenMenu();
			}
		}
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x0018B968 File Offset: 0x00189B68
	public void OnCommand(ContextButtonController button)
	{
		this.lastButton = button;
		this.commandObject.Invoke(button.setup.commandString, 0f);
		this.ForceClose();
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x0018B992 File Offset: 0x00189B92
	public void ForceClose()
	{
		Game.Log("Interface: Force close context menu", 2);
		Object.Destroy(this.spawnedMenu);
		ContextMenuController.activeMenu = null;
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x0018B9B0 File Offset: 0x00189BB0
	private Vector2 ClampToWindow(Vector2 rawPointerPosition)
	{
		Vector3[] array = new Vector3[4];
		PrefabControls.Instance.tooltipsCanvas.GetWorldCorners(array);
		float num = Mathf.Clamp(rawPointerPosition.x, array[0].x, array[2].x - this.menuRect.sizeDelta.x);
		float num2 = Mathf.Clamp(rawPointerPosition.y, array[0].y, array[2].y);
		return new Vector2(num, num2);
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x0018BA34 File Offset: 0x00189C34
	public void SetScreenPosition(Vector2 pointerPosition)
	{
		if (this.spawnedMenu == null)
		{
			return;
		}
		pointerPosition = this.ClampToWindow(pointerPosition);
		string text = "Clamped pointer: ";
		Vector2 vector = pointerPosition;
		Game.Log(text + vector.ToString(), 2);
		Vector2 vector2;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(PrefabControls.Instance.contextMenuContainer, pointerPosition, null, ref vector2))
		{
			string text2 = "Local: ";
			vector = vector2;
			Game.Log(text2 + vector.ToString(), 2);
			this.menuRect.localPosition = vector2;
		}
	}

	// Token: 0x040023CC RID: 9164
	[Header("Usage")]
	public bool useLeftButton;

	// Token: 0x040023CD RID: 9165
	[Header("Positioning & Size")]
	public Vector2 pos = new Vector2(0f, -0.5f);

	// Token: 0x040023CE RID: 9166
	public bool useCursorPos;

	// Token: 0x040023CF RID: 9167
	[EnableIf("useCursorPos")]
	public Vector2 cursorPosOffset = new Vector2(10f, -7f);

	// Token: 0x040023D0 RID: 9168
	public bool useGlobalWidth = true;

	// Token: 0x040023D1 RID: 9169
	[DisableIf("useGlobalWidth")]
	public float width = 190f;

	// Token: 0x040023D2 RID: 9170
	[Header("Configuration")]
	public ContextMenuController.MenuFlag flag;

	// Token: 0x040023D3 RID: 9171
	[ReorderableList]
	public List<ContextMenuController.ContextMenuButtonSetup> menuButtons = new List<ContextMenuController.ContextMenuButtonSetup>();

	// Token: 0x040023D4 RID: 9172
	[Tooltip("Disabled items")]
	public List<string> disabledItems = new List<string>();

	// Token: 0x040023D5 RID: 9173
	public MonoBehaviour commandObject;

	// Token: 0x040023D6 RID: 9174
	[Header("Spawned")]
	public static ContextMenuController activeMenu;

	// Token: 0x040023D7 RID: 9175
	public ContextButtonController lastButton;

	// Token: 0x040023D8 RID: 9176
	public GameObject spawnedMenu;

	// Token: 0x040023D9 RID: 9177
	private RectTransform menuRect;

	// Token: 0x020004E8 RID: 1256
	public enum MenuFlag
	{
		// Token: 0x040023DC RID: 9180
		none,
		// Token: 0x040023DD RID: 9181
		pinnedSelected
	}

	// Token: 0x020004E9 RID: 1257
	[Serializable]
	public class ContextMenuButtonSetup
	{
		// Token: 0x040023DE RID: 9182
		public string commandString;

		// Token: 0x040023DF RID: 9183
		[Space(5f)]
		public bool useText = true;

		// Token: 0x040023E0 RID: 9184
		public string overrideText;

		// Token: 0x040023E1 RID: 9185
		[Space(5f)]
		public bool useColour;

		// Token: 0x040023E2 RID: 9186
		public Color colour;

		// Token: 0x040023E3 RID: 9187
		[Space(5f)]
		public bool devOnly;
	}

	// Token: 0x020004EA RID: 1258
	// (Invoke) Token: 0x06001B38 RID: 6968
	public delegate void OpenedMenu();
}
