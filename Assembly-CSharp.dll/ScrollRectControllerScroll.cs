using System;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class ScrollRectControllerScroll : MonoBehaviour
{
	// Token: 0x06001D34 RID: 7476 RVA: 0x0019E7A8 File Offset: 0x0019C9A8
	private void OnEnable()
	{
		this.SetupReferences();
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x0019E7B0 File Offset: 0x0019C9B0
	private void SetupReferences()
	{
		if (this.scrollRect == null)
		{
			this.scrollRect = base.gameObject.GetComponent<CustomScrollRect>();
		}
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0019E7D4 File Offset: 0x0019C9D4
	private void Update()
	{
		if (!this.autoScrollingEnabled)
		{
			return;
		}
		if (this.scrollRect == null)
		{
			base.enabled = false;
			return;
		}
		if (this.scrollRect.isActiveAndEnabled && InputController.Instance != null && InputController.Instance.player != null)
		{
			float axisRelative = InputController.Instance.GetAxisRelative("ContentMoveAxisY");
			if (Mathf.Abs(axisRelative) > 0.01f)
			{
				Vector2 anchoredPosition = this.scrollRect.content.anchoredPosition;
				anchoredPosition.y += -axisRelative * this.scrollSpeed;
				this.scrollRect.SetAnchorPos(anchoredPosition);
			}
		}
	}

	// Token: 0x040026F3 RID: 9971
	[Header("Settings")]
	public bool autoScrollingEnabled = true;

	// Token: 0x040026F4 RID: 9972
	public float scrollSpeed = 4.5f;

	// Token: 0x040026F5 RID: 9973
	[Header("References")]
	public CustomScrollRect scrollRect;
}
