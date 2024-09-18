using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000875 RID: 2165
	public interface ICustomSelectable : ICancelHandler, IEventSystemHandler
	{
		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06002D5B RID: 11611
		// (set) Token: 0x06002D5C RID: 11612
		Sprite disabledHighlightedSprite { get; set; }

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06002D5D RID: 11613
		// (set) Token: 0x06002D5E RID: 11614
		Color disabledHighlightedColor { get; set; }

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06002D5F RID: 11615
		// (set) Token: 0x06002D60 RID: 11616
		string disabledHighlightedTrigger { get; set; }

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06002D61 RID: 11617
		// (set) Token: 0x06002D62 RID: 11618
		bool autoNavUp { get; set; }

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06002D63 RID: 11619
		// (set) Token: 0x06002D64 RID: 11620
		bool autoNavDown { get; set; }

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06002D65 RID: 11621
		// (set) Token: 0x06002D66 RID: 11622
		bool autoNavLeft { get; set; }

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06002D67 RID: 11623
		// (set) Token: 0x06002D68 RID: 11624
		bool autoNavRight { get; set; }

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06002D69 RID: 11625
		// (remove) Token: 0x06002D6A RID: 11626
		event UnityAction CancelEvent;
	}
}
