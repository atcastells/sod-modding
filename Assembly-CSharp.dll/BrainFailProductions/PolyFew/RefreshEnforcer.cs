using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008E4 RID: 2276
	[ExecuteInEditMode]
	public class RefreshEnforcer : MonoBehaviour
	{
		// Token: 0x06003097 RID: 12439 RVA: 0x002170C3 File Offset: 0x002152C3
		private void Start()
		{
			Object.DestroyImmediate(this);
		}

		// Token: 0x06003098 RID: 12440 RVA: 0x00002265 File Offset: 0x00000465
		private void Update()
		{
		}
	}
}
