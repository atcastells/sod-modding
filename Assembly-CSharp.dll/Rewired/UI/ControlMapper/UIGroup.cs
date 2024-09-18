using System;
using TMPro;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000895 RID: 2197
	[AddComponentMenu("")]
	public class UIGroup : MonoBehaviour
	{
		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06002E9E RID: 11934 RVA: 0x0020CC58 File Offset: 0x0020AE58
		// (set) Token: 0x06002E9F RID: 11935 RVA: 0x0020CC79 File Offset: 0x0020AE79
		public string labelText
		{
			get
			{
				if (!(this._label != null))
				{
					return string.Empty;
				}
				return this._label.text;
			}
			set
			{
				if (this._label == null)
				{
					return;
				}
				this._label.text = value;
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06002EA0 RID: 11936 RVA: 0x0020CC96 File Offset: 0x0020AE96
		public Transform content
		{
			get
			{
				return this._content;
			}
		}

		// Token: 0x06002EA1 RID: 11937 RVA: 0x0020CC9E File Offset: 0x0020AE9E
		public void SetLabelActive(bool state)
		{
			if (this._label == null)
			{
				return;
			}
			this._label.gameObject.SetActive(state);
		}

		// Token: 0x040049D2 RID: 18898
		[SerializeField]
		private TMP_Text _label;

		// Token: 0x040049D3 RID: 18899
		[SerializeField]
		private Transform _content;
	}
}
