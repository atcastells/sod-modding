using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008E8 RID: 2280
	[Serializable]
	public class ToleranceSphere : ScriptableObject
	{
		// Token: 0x060030C4 RID: 12484 RVA: 0x00217C47 File Offset: 0x00215E47
		public ToleranceSphere(Vector3 worldPosition, float diameter, Color color, float preservationStrength, bool isHidden = false)
		{
			this.worldPosition = worldPosition;
			this.diameter = diameter;
			this.color = color;
			this.preservationStrength = preservationStrength;
			this.isHidden = isHidden;
		}

		// Token: 0x060030C5 RID: 12485 RVA: 0x00217C74 File Offset: 0x00215E74
		public void SetProperties(ToleranceSphereJson tSphereJson)
		{
			this.worldPosition = tSphereJson.worldPosition;
			this.diameter = tSphereJson.diameter;
			this.color = tSphereJson.color;
			this.preservationStrength = tSphereJson.preservationStrength;
			this.isHidden = tSphereJson.isHidden;
		}

		// Token: 0x060030C6 RID: 12486 RVA: 0x00217CB2 File Offset: 0x00215EB2
		public void SetProperties(Vector3 worldPosition, float diameter, Color color, float preservationStrength, bool isHidden = false)
		{
			this.worldPosition = worldPosition;
			this.diameter = diameter;
			this.color = color;
			this.preservationStrength = preservationStrength;
			this.isHidden = isHidden;
		}

		// Token: 0x04004BAF RID: 19375
		public Vector3 worldPosition;

		// Token: 0x04004BB0 RID: 19376
		public float diameter;

		// Token: 0x04004BB1 RID: 19377
		public Color color;

		// Token: 0x04004BB2 RID: 19378
		public float preservationStrength;

		// Token: 0x04004BB3 RID: 19379
		public bool isHidden;
	}
}
