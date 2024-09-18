using System;
using TMPro;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200087C RID: 2172
	[AddComponentMenu("")]
	public class InputRow : MonoBehaviour
	{
		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06002D91 RID: 11665 RVA: 0x0020B16E File Offset: 0x0020936E
		// (set) Token: 0x06002D92 RID: 11666 RVA: 0x0020B176 File Offset: 0x00209376
		public ButtonInfo[] buttons { get; private set; }

		// Token: 0x06002D93 RID: 11667 RVA: 0x0020B17F File Offset: 0x0020937F
		public void Initialize(int rowIndex, string label, Action<int, ButtonInfo> inputFieldActivatedCallback)
		{
			this.rowIndex = rowIndex;
			this.label.text = label;
			this.inputFieldActivatedCallback = inputFieldActivatedCallback;
			this.buttons = base.transform.GetComponentsInChildren<ButtonInfo>(true);
		}

		// Token: 0x06002D94 RID: 11668 RVA: 0x0020B1AD File Offset: 0x002093AD
		public void OnButtonActivated(ButtonInfo buttonInfo)
		{
			if (this.inputFieldActivatedCallback == null)
			{
				return;
			}
			this.inputFieldActivatedCallback.Invoke(this.rowIndex, buttonInfo);
		}

		// Token: 0x0400492F RID: 18735
		public TMP_Text label;

		// Token: 0x04004931 RID: 18737
		private int rowIndex;

		// Token: 0x04004932 RID: 18738
		private Action<int, ButtonInfo> inputFieldActivatedCallback;
	}
}
