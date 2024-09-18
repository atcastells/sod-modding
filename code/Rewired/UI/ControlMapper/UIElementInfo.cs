using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000894 RID: 2196
	[AddComponentMenu("")]
	public abstract class UIElementInfo : MonoBehaviour, ISelectHandler, IEventSystemHandler
	{
		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06002E9A RID: 11930 RVA: 0x0020CBD0 File Offset: 0x0020ADD0
		// (remove) Token: 0x06002E9B RID: 11931 RVA: 0x0020CC08 File Offset: 0x0020AE08
		public event Action<GameObject> OnSelectedEvent;

		// Token: 0x06002E9C RID: 11932 RVA: 0x0020CC3D File Offset: 0x0020AE3D
		public void OnSelect(BaseEventData eventData)
		{
			if (this.OnSelectedEvent != null)
			{
				this.OnSelectedEvent.Invoke(base.gameObject);
			}
		}

		// Token: 0x040049CE RID: 18894
		public string identifier;

		// Token: 0x040049CF RID: 18895
		public int intData;

		// Token: 0x040049D0 RID: 18896
		public TMP_Text text;
	}
}
