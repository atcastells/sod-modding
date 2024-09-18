using System;

namespace Rewired
{
	// Token: 0x02000828 RID: 2088
	public interface IFlightPedalsTemplate : IControllerTemplate
	{
		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060027A4 RID: 10148
		IControllerTemplateAxis leftPedal { get; }

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060027A5 RID: 10149
		IControllerTemplateAxis rightPedal { get; }

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060027A6 RID: 10150
		IControllerTemplateAxis slide { get; }
	}
}
