using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000896 RID: 2198
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("")]
	public class UIImageHelper : MonoBehaviour
	{
		// Token: 0x06002EA3 RID: 11939 RVA: 0x0020CCC0 File Offset: 0x0020AEC0
		public void SetEnabledState(bool newState)
		{
			this.currentState = newState;
			UIImageHelper.State state = newState ? this.enabledState : this.disabledState;
			if (state == null)
			{
				return;
			}
			Image component = base.gameObject.GetComponent<Image>();
			if (component == null)
			{
				Debug.LogError("Image is missing!");
				return;
			}
			state.Set(component);
		}

		// Token: 0x06002EA4 RID: 11940 RVA: 0x0020CD11 File Offset: 0x0020AF11
		public void SetEnabledStateColor(Color color)
		{
			this.enabledState.color = color;
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x0020CD1F File Offset: 0x0020AF1F
		public void SetDisabledStateColor(Color color)
		{
			this.disabledState.color = color;
		}

		// Token: 0x06002EA6 RID: 11942 RVA: 0x0020CD30 File Offset: 0x0020AF30
		public void Refresh()
		{
			UIImageHelper.State state = this.currentState ? this.enabledState : this.disabledState;
			Image component = base.gameObject.GetComponent<Image>();
			if (component == null)
			{
				return;
			}
			state.Set(component);
		}

		// Token: 0x040049D4 RID: 18900
		[SerializeField]
		private UIImageHelper.State enabledState;

		// Token: 0x040049D5 RID: 18901
		[SerializeField]
		private UIImageHelper.State disabledState;

		// Token: 0x040049D6 RID: 18902
		private bool currentState;

		// Token: 0x02000897 RID: 2199
		[Serializable]
		private class State
		{
			// Token: 0x06002EA8 RID: 11944 RVA: 0x0020CD71 File Offset: 0x0020AF71
			public void Set(Image image)
			{
				if (image == null)
				{
					return;
				}
				image.color = this.color;
			}

			// Token: 0x040049D7 RID: 18903
			[SerializeField]
			public Color color;
		}
	}
}
