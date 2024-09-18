using System;

namespace Rewired
{
	// Token: 0x0200082E RID: 2094
	public sealed class FlightPedalsTemplate : ControllerTemplate, IFlightPedalsTemplate, IControllerTemplate
	{
		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060028A8 RID: 10408 RVA: 0x001FAB8D File Offset: 0x001F8D8D
		IControllerTemplateAxis IFlightPedalsTemplate.leftPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(0);
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060028A9 RID: 10409 RVA: 0x001FAB96 File Offset: 0x001F8D96
		IControllerTemplateAxis IFlightPedalsTemplate.rightPedal
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(1);
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060028AA RID: 10410 RVA: 0x001FAB9F File Offset: 0x001F8D9F
		IControllerTemplateAxis IFlightPedalsTemplate.slide
		{
			get
			{
				return base.GetElement<IControllerTemplateAxis>(2);
			}
		}

		// Token: 0x060028AB RID: 10411 RVA: 0x001FAB73 File Offset: 0x001F8D73
		public FlightPedalsTemplate(object payload) : base(payload)
		{
		}

		// Token: 0x040046E7 RID: 18151
		public static readonly Guid typeGuid = new Guid("f6fe76f8-be2a-4db2-b853-9e3652075913");

		// Token: 0x040046E8 RID: 18152
		public const int elementId_leftPedal = 0;

		// Token: 0x040046E9 RID: 18153
		public const int elementId_rightPedal = 1;

		// Token: 0x040046EA RID: 18154
		public const int elementId_slide = 2;
	}
}
