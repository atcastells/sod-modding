using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos.GamepadTemplateUI
{
	// Token: 0x020008D3 RID: 2259
	[RequireComponent(typeof(Image))]
	public class ControllerUIElement : MonoBehaviour
	{
		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x0600305F RID: 12383 RVA: 0x0021569E File Offset: 0x0021389E
		private bool hasEffects
		{
			get
			{
				return this._positiveUIEffect != null || this._negativeUIEffect != null;
			}
		}

		// Token: 0x06003060 RID: 12384 RVA: 0x002156BC File Offset: 0x002138BC
		private void Awake()
		{
			this._image = base.GetComponent<Image>();
			this._origColor = this._image.color;
			this._color = this._origColor;
			this.ClearLabels();
		}

		// Token: 0x06003061 RID: 12385 RVA: 0x002156F0 File Offset: 0x002138F0
		public void Activate(float amount)
		{
			amount = Mathf.Clamp(amount, -1f, 1f);
			if (this.hasEffects)
			{
				if (amount < 0f && this._negativeUIEffect != null)
				{
					this._negativeUIEffect.Activate(Mathf.Abs(amount));
				}
				if (amount > 0f && this._positiveUIEffect != null)
				{
					this._positiveUIEffect.Activate(Mathf.Abs(amount));
				}
			}
			else
			{
				if (this._isActive && amount == this._highlightAmount)
				{
					return;
				}
				this._highlightAmount = amount;
				this._color = Color.Lerp(this._origColor, this._highlightColor, this._highlightAmount);
			}
			this._isActive = true;
			this.RedrawImage();
			if (this._childElements.Length != 0)
			{
				for (int i = 0; i < this._childElements.Length; i++)
				{
					if (!(this._childElements[i] == null))
					{
						this._childElements[i].Activate(amount);
					}
				}
			}
		}

		// Token: 0x06003062 RID: 12386 RVA: 0x002157E4 File Offset: 0x002139E4
		public void Deactivate()
		{
			if (!this._isActive)
			{
				return;
			}
			this._color = this._origColor;
			this._highlightAmount = 0f;
			if (this._positiveUIEffect != null)
			{
				this._positiveUIEffect.Deactivate();
			}
			if (this._negativeUIEffect != null)
			{
				this._negativeUIEffect.Deactivate();
			}
			this._isActive = false;
			this.RedrawImage();
			if (this._childElements.Length != 0)
			{
				for (int i = 0; i < this._childElements.Length; i++)
				{
					if (!(this._childElements[i] == null))
					{
						this._childElements[i].Deactivate();
					}
				}
			}
		}

		// Token: 0x06003063 RID: 12387 RVA: 0x0021588C File Offset: 0x00213A8C
		public void SetLabel(string text, AxisRange labelType)
		{
			Text text2;
			switch (labelType)
			{
			case 0:
				text2 = this._label;
				break;
			case 1:
				text2 = this._positiveLabel;
				break;
			case 2:
				text2 = this._negativeLabel;
				break;
			default:
				text2 = null;
				break;
			}
			if (text2 != null)
			{
				text2.text = text;
			}
			if (this._childElements.Length != 0)
			{
				for (int i = 0; i < this._childElements.Length; i++)
				{
					if (!(this._childElements[i] == null))
					{
						this._childElements[i].SetLabel(text, labelType);
					}
				}
			}
		}

		// Token: 0x06003064 RID: 12388 RVA: 0x00215918 File Offset: 0x00213B18
		public void ClearLabels()
		{
			if (this._label != null)
			{
				this._label.text = string.Empty;
			}
			if (this._positiveLabel != null)
			{
				this._positiveLabel.text = string.Empty;
			}
			if (this._negativeLabel != null)
			{
				this._negativeLabel.text = string.Empty;
			}
			if (this._childElements.Length != 0)
			{
				for (int i = 0; i < this._childElements.Length; i++)
				{
					if (!(this._childElements[i] == null))
					{
						this._childElements[i].ClearLabels();
					}
				}
			}
		}

		// Token: 0x06003065 RID: 12389 RVA: 0x002159B8 File Offset: 0x00213BB8
		private void RedrawImage()
		{
			this._image.color = this._color;
		}

		// Token: 0x04004B1A RID: 19226
		[SerializeField]
		private Color _highlightColor = Color.white;

		// Token: 0x04004B1B RID: 19227
		[SerializeField]
		private ControllerUIEffect _positiveUIEffect;

		// Token: 0x04004B1C RID: 19228
		[SerializeField]
		private ControllerUIEffect _negativeUIEffect;

		// Token: 0x04004B1D RID: 19229
		[SerializeField]
		private Text _label;

		// Token: 0x04004B1E RID: 19230
		[SerializeField]
		private Text _positiveLabel;

		// Token: 0x04004B1F RID: 19231
		[SerializeField]
		private Text _negativeLabel;

		// Token: 0x04004B20 RID: 19232
		[SerializeField]
		private ControllerUIElement[] _childElements = new ControllerUIElement[0];

		// Token: 0x04004B21 RID: 19233
		private Image _image;

		// Token: 0x04004B22 RID: 19234
		private Color _color;

		// Token: 0x04004B23 RID: 19235
		private Color _origColor;

		// Token: 0x04004B24 RID: 19236
		private bool _isActive;

		// Token: 0x04004B25 RID: 19237
		private float _highlightAmount;
	}
}
