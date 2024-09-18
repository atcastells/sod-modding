using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class ReflectionProbeController : MonoBehaviour
{
	// Token: 0x06000A78 RID: 2680 RVA: 0x0009E300 File Offset: 0x0009C500
	public void Setup()
	{
		this.probe.RenderProbe(null);
	}

	// Token: 0x04000A8F RID: 2703
	public ReflectionProbe probe;
}
