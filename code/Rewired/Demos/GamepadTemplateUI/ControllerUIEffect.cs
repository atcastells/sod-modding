using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos.GamepadTemplateUI
{
	// Token: 0x020008D2 RID: 2258
	[RequireComponent(typeof(Image))]
	public class ControllerUIEffect : MonoBehaviour
	{
		// Token: 0x0600305A RID: 12378 RVA: 0x002155B2 File Offset: 0x002137B2
		private void Awake()
		{
			this._image = base.GetComponent<Image>();
			this._origColor = this._image.color;
			this._color = this._origColor;
		}

		// Token: 0x0600305B RID: 12379 RVA: 0x002155E0 File Offset: 0x002137E0
		public void Activate(float amount)
		{
			amount = Mathf.Clamp01(amount);
			if (this._isActive && amount == this._highlightAmount)
			{
				return;
			}
			this._highlightAmount = amount;
			this._color = Color.Lerp(this._origColor, this._highlightColor, this._highlightAmount);
			this._isActive = true;
			this.RedrawImage();
		}

		// Token: 0x0600305C RID: 12380 RVA: 0x00215638 File Offset: 0x00213838
		public void Deactivate()
		{
			if (!this._isActive)
			{
				return;
			}
			this._color = this._origColor;
			this._highlightAmount = 0f;
			this._isActive = false;
			this.RedrawImage();
		}

		// Token: 0x0600305D RID: 12381 RVA: 0x00215667 File Offset: 0x00213867
		private void RedrawImage()
		{
			this._image.color = this._color;
			this._image.enabled = this._isActive;
		}

		// Token: 0x04004B14 RID: 19220
		[SerializeField]
		private Color _highlightColor = Color.white;

		// Token: 0x04004B15 RID: 19221
		private Image _image;

		// Token: 0x04004B16 RID: 19222
		private Color _color;

		// Token: 0x04004B17 RID: 19223
		private Color _origColor;

		// Token: 0x04004B18 RID: 19224
		private bool _isActive;

		// Token: 0x04004B19 RID: 19225
		private float _highlightAmount;
	}
}
