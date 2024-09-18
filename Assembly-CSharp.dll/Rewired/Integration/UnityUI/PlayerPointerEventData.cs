using System;
using System.Text;
using Rewired.UI;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI
{
	// Token: 0x0200083C RID: 2108
	public class PlayerPointerEventData : PointerEventData
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x060029A9 RID: 10665 RVA: 0x001FCF1B File Offset: 0x001FB11B
		// (set) Token: 0x060029AA RID: 10666 RVA: 0x001FCF23 File Offset: 0x001FB123
		public int playerId { get; set; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x060029AB RID: 10667 RVA: 0x001FCF2C File Offset: 0x001FB12C
		// (set) Token: 0x060029AC RID: 10668 RVA: 0x001FCF34 File Offset: 0x001FB134
		public int inputSourceIndex { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x060029AD RID: 10669 RVA: 0x001FCF3D File Offset: 0x001FB13D
		// (set) Token: 0x060029AE RID: 10670 RVA: 0x001FCF45 File Offset: 0x001FB145
		public IMouseInputSource mouseSource { get; set; }

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x060029AF RID: 10671 RVA: 0x001FCF4E File Offset: 0x001FB14E
		// (set) Token: 0x060029B0 RID: 10672 RVA: 0x001FCF56 File Offset: 0x001FB156
		public ITouchInputSource touchSource { get; set; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x060029B1 RID: 10673 RVA: 0x001FCF5F File Offset: 0x001FB15F
		// (set) Token: 0x060029B2 RID: 10674 RVA: 0x001FCF67 File Offset: 0x001FB167
		public PointerEventType sourceType { get; set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x060029B3 RID: 10675 RVA: 0x001FCF70 File Offset: 0x001FB170
		// (set) Token: 0x060029B4 RID: 10676 RVA: 0x001FCF78 File Offset: 0x001FB178
		public int buttonIndex { get; set; }

		// Token: 0x060029B5 RID: 10677 RVA: 0x001FCF81 File Offset: 0x001FB181
		public PlayerPointerEventData(EventSystem eventSystem) : base(eventSystem)
		{
			this.playerId = -1;
			this.inputSourceIndex = -1;
			this.buttonIndex = -1;
		}

		// Token: 0x060029B6 RID: 10678 RVA: 0x001FCFA0 File Offset: 0x001FB1A0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Player Id</b>: " + this.playerId.ToString());
			string text = "<b>Mouse Source</b>: ";
			IMouseInputSource mouseSource = this.mouseSource;
			stringBuilder.AppendLine(text + ((mouseSource != null) ? mouseSource.ToString() : null));
			stringBuilder.AppendLine("<b>Input Source Index</b>: " + this.inputSourceIndex.ToString());
			string text2 = "<b>Touch Source/b>: ";
			ITouchInputSource touchSource = this.touchSource;
			stringBuilder.AppendLine(text2 + ((touchSource != null) ? touchSource.ToString() : null));
			stringBuilder.AppendLine("<b>Source Type</b>: " + this.sourceType.ToString());
			stringBuilder.AppendLine("<b>Button Index</b>: " + this.buttonIndex.ToString());
			stringBuilder.Append(base.ToString());
			return stringBuilder.ToString();
		}
	}
}
