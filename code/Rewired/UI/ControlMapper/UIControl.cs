using System;
using TMPro;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000891 RID: 2193
	[AddComponentMenu("")]
	public class UIControl : MonoBehaviour
	{
		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06002E8B RID: 11915 RVA: 0x0020C999 File Offset: 0x0020AB99
		public int id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x0020C9A1 File Offset: 0x0020ABA1
		private void Awake()
		{
			this._id = UIControl.GetNextUid();
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06002E8D RID: 11917 RVA: 0x0020C9AE File Offset: 0x0020ABAE
		// (set) Token: 0x06002E8E RID: 11918 RVA: 0x0020C9B6 File Offset: 0x0020ABB6
		public bool showTitle
		{
			get
			{
				return this._showTitle;
			}
			set
			{
				if (this.title == null)
				{
					return;
				}
				this.title.gameObject.SetActive(value);
				this._showTitle = value;
			}
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x00002265 File Offset: 0x00000465
		public virtual void SetCancelCallback(Action cancelCallback)
		{
		}

		// Token: 0x06002E90 RID: 11920 RVA: 0x0020C9DF File Offset: 0x0020ABDF
		private static int GetNextUid()
		{
			if (UIControl._uidCounter == 2147483647)
			{
				UIControl._uidCounter = 0;
			}
			int uidCounter = UIControl._uidCounter;
			UIControl._uidCounter++;
			return uidCounter;
		}

		// Token: 0x040049C5 RID: 18885
		public TMP_Text title;

		// Token: 0x040049C6 RID: 18886
		private int _id;

		// Token: 0x040049C7 RID: 18887
		private bool _showTitle;

		// Token: 0x040049C8 RID: 18888
		private static int _uidCounter;
	}
}
