using System;
using UnityEngine;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x0200087B RID: 2171
	[AddComponentMenu("")]
	public class InputFieldInfo : UIElementInfo
	{
		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06002D86 RID: 11654 RVA: 0x0020B119 File Offset: 0x00209319
		// (set) Token: 0x06002D87 RID: 11655 RVA: 0x0020B121 File Offset: 0x00209321
		public int actionId { get; set; }

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06002D88 RID: 11656 RVA: 0x0020B12A File Offset: 0x0020932A
		// (set) Token: 0x06002D89 RID: 11657 RVA: 0x0020B132 File Offset: 0x00209332
		public AxisRange axisRange { get; set; }

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06002D8A RID: 11658 RVA: 0x0020B13B File Offset: 0x0020933B
		// (set) Token: 0x06002D8B RID: 11659 RVA: 0x0020B143 File Offset: 0x00209343
		public int actionElementMapId { get; set; }

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06002D8C RID: 11660 RVA: 0x0020B14C File Offset: 0x0020934C
		// (set) Token: 0x06002D8D RID: 11661 RVA: 0x0020B154 File Offset: 0x00209354
		public ControllerType controllerType { get; set; }

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06002D8E RID: 11662 RVA: 0x0020B15D File Offset: 0x0020935D
		// (set) Token: 0x06002D8F RID: 11663 RVA: 0x0020B165 File Offset: 0x00209365
		public int controllerId { get; set; }
	}
}
