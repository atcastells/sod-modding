using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000883 RID: 2179
	[AddComponentMenu("")]
	public class ThemedElement : MonoBehaviour
	{
		// Token: 0x06002E2B RID: 11819 RVA: 0x0020BD30 File Offset: 0x00209F30
		private void Start()
		{
			ControlMapper.ApplyTheme(this._elements);
		}

		// Token: 0x0400497B RID: 18811
		[SerializeField]
		private ThemedElement.ElementInfo[] _elements;

		// Token: 0x02000884 RID: 2180
		[Serializable]
		public class ElementInfo
		{
			// Token: 0x17000496 RID: 1174
			// (get) Token: 0x06002E2D RID: 11821 RVA: 0x0020BD3D File Offset: 0x00209F3D
			public string themeClass
			{
				get
				{
					return this._themeClass;
				}
			}

			// Token: 0x17000497 RID: 1175
			// (get) Token: 0x06002E2E RID: 11822 RVA: 0x0020BD45 File Offset: 0x00209F45
			public Component component
			{
				get
				{
					return this._component;
				}
			}

			// Token: 0x0400497C RID: 18812
			[SerializeField]
			private string _themeClass;

			// Token: 0x0400497D RID: 18813
			[SerializeField]
			private Component _component;
		}
	}
}
