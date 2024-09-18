using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021C RID: 540
[Serializable]
public class AddressSaveData
{
	// Token: 0x04000DD0 RID: 3536
	public string p_n;

	// Token: 0x04000DD1 RID: 3537
	public Color e_c = Color.cyan;

	// Token: 0x04000DD2 RID: 3538
	public List<AddressLayoutVariation> vs = new List<AddressLayoutVariation>();
}
