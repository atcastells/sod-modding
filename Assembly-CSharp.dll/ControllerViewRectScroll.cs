using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class ControllerViewRectScroll : MonoBehaviour
{
	// Token: 0x06001DEB RID: 7659 RVA: 0x001A4776 File Offset: 0x001A2976
	private void OnEnable()
	{
		InputController.Instance.OnInputModeChange += this.OnInputModeChange;
		this.OnInputModeChange();
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x001A4794 File Offset: 0x001A2994
	private void OnDisable()
	{
		InputController.Instance.OnInputModeChange -= this.OnInputModeChange;
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x001A47AC File Offset: 0x001A29AC
	public void OnInputModeChange()
	{
		try
		{
			if (InputController.Instance.mouseInputMode)
			{
				if (this != null)
				{
					base.enabled = false;
				}
			}
			else if (this != null)
			{
				base.enabled = true;
			}
		}
		catch
		{
		}
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x001A47FC File Offset: 0x001A29FC
	private void Update()
	{
		if (InputController.Instance.mouseInputMode)
		{
			base.enabled = false;
			return;
		}
		if (this.controlEnabled)
		{
			Vector2 vector;
			vector..ctor(InputController.Instance.GetAxisRelative("ContentMoveAxisX") * this.sensitivity, InputController.Instance.GetAxisRelative("ContentMoveAxisY") * this.sensitivity);
			if (!this.scrollRect.horizontal)
			{
				vector.x = 0f;
			}
			if (!this.scrollRect.vertical)
			{
				vector.y = 0f;
			}
			this.scrollRect.content.anchoredPosition -= vector;
		}
	}

	// Token: 0x040027BE RID: 10174
	public bool controlEnabled;

	// Token: 0x040027BF RID: 10175
	public CustomScrollRect scrollRect;

	// Token: 0x040027C0 RID: 10176
	public float sensitivity = 10f;
}
