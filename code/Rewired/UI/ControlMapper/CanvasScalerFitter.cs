using System;
using Rewired.Utils;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200084B RID: 2123
	[RequireComponent(typeof(CanvasScalerExt))]
	public class CanvasScalerFitter : MonoBehaviour
	{
		// Token: 0x06002A7D RID: 10877 RVA: 0x002007C4 File Offset: 0x001FE9C4
		private void OnEnable()
		{
			this.canvasScaler = base.GetComponent<CanvasScalerExt>();
			this.Update();
			this.canvasScaler.ForceRefresh();
		}

		// Token: 0x06002A7E RID: 10878 RVA: 0x002007E3 File Offset: 0x001FE9E3
		private void Update()
		{
			if (Screen.width != this.screenWidth || Screen.height != this.screenHeight)
			{
				this.screenWidth = Screen.width;
				this.screenHeight = Screen.height;
				this.UpdateSize();
			}
		}

		// Token: 0x06002A7F RID: 10879 RVA: 0x0020081C File Offset: 0x001FEA1C
		private void UpdateSize()
		{
			if (this.canvasScaler.uiScaleMode != 1)
			{
				return;
			}
			if (this.breakPoints == null)
			{
				return;
			}
			float num = (float)Screen.width / (float)Screen.height;
			float num2 = float.PositiveInfinity;
			int num3 = 0;
			for (int i = 0; i < this.breakPoints.Length; i++)
			{
				float num4 = Mathf.Abs(num - this.breakPoints[i].screenAspectRatio);
				if ((num4 <= this.breakPoints[i].screenAspectRatio || MathTools.IsNear(this.breakPoints[i].screenAspectRatio, 0.01f)) && num4 < num2)
				{
					num2 = num4;
					num3 = i;
				}
			}
			this.canvasScaler.referenceResolution = this.breakPoints[num3].referenceResolution;
		}

		// Token: 0x040047C9 RID: 18377
		[SerializeField]
		private CanvasScalerFitter.BreakPoint[] breakPoints;

		// Token: 0x040047CA RID: 18378
		private CanvasScalerExt canvasScaler;

		// Token: 0x040047CB RID: 18379
		private int screenWidth;

		// Token: 0x040047CC RID: 18380
		private int screenHeight;

		// Token: 0x040047CD RID: 18381
		private Action ScreenSizeChanged;

		// Token: 0x0200084C RID: 2124
		[Serializable]
		private class BreakPoint
		{
			// Token: 0x040047CE RID: 18382
			[SerializeField]
			public string name;

			// Token: 0x040047CF RID: 18383
			[SerializeField]
			public float screenAspectRatio;

			// Token: 0x040047D0 RID: 18384
			[SerializeField]
			public Vector2 referenceResolution;
		}
	}
}
