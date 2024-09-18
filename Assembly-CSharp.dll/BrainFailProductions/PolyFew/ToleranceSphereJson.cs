using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008E9 RID: 2281
	[Serializable]
	public class ToleranceSphereJson
	{
		// Token: 0x060030C7 RID: 12487 RVA: 0x00217CD9 File Offset: 0x00215ED9
		public ToleranceSphereJson(Vector3 worldPosition, float diameter, Color color, float preservationStrength, bool isHidden = false)
		{
			this.worldPosition = worldPosition;
			this.diameter = diameter;
			this.color = color;
			this.preservationStrength = preservationStrength;
			this.isHidden = isHidden;
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x00217D06 File Offset: 0x00215F06
		public ToleranceSphereJson(ToleranceSphere toleranceSphere)
		{
			if (toleranceSphere == null)
			{
				return;
			}
			this.DumpFromToleranceSphere(toleranceSphere);
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x00217D1F File Offset: 0x00215F1F
		public void SetProperties(Vector3 worldPosition, float diameter, Color color, float preservationStrength, bool isHidden = false)
		{
			this.worldPosition = worldPosition;
			this.diameter = diameter;
			this.color = color;
			this.preservationStrength = preservationStrength;
			this.isHidden = isHidden;
		}

		// Token: 0x060030CA RID: 12490 RVA: 0x00217D48 File Offset: 0x00215F48
		public void DumpFromToleranceSphere(ToleranceSphere toleranceSphere)
		{
			if (toleranceSphere == null)
			{
				return;
			}
			this.worldPosition = toleranceSphere.worldPosition;
			this.diameter = toleranceSphere.diameter;
			this.color = toleranceSphere.color;
			this.preservationStrength = toleranceSphere.preservationStrength;
			this.isHidden = toleranceSphere.isHidden;
		}

		// Token: 0x060030CB RID: 12491 RVA: 0x00217D9C File Offset: 0x00215F9C
		public void DumpToToleranceSphere(ref ToleranceSphere toleranceSphere)
		{
			if (toleranceSphere == null)
			{
				return;
			}
			toleranceSphere.worldPosition = this.worldPosition;
			toleranceSphere.diameter = this.diameter;
			toleranceSphere.color = this.color;
			toleranceSphere.preservationStrength = this.preservationStrength;
			toleranceSphere.isHidden = this.isHidden;
		}

		// Token: 0x04004BB4 RID: 19380
		public Vector3 worldPosition;

		// Token: 0x04004BB5 RID: 19381
		public float diameter;

		// Token: 0x04004BB6 RID: 19382
		public Color color;

		// Token: 0x04004BB7 RID: 19383
		public float preservationStrength;

		// Token: 0x04004BB8 RID: 19384
		public bool isHidden;
	}
}
