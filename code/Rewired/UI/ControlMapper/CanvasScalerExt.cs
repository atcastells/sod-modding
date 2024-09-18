using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200084A RID: 2122
	[AddComponentMenu("")]
	public class CanvasScalerExt : CanvasScaler
	{
		// Token: 0x06002A7B RID: 10875 RVA: 0x002007B4 File Offset: 0x001FE9B4
		public void ForceRefresh()
		{
			this.Handle();
		}
	}
}
