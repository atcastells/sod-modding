using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008B0 RID: 2224
	[AddComponentMenu("")]
	[RequireComponent(typeof(Image))]
	public class TouchButtonExample : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06002F92 RID: 12178 RVA: 0x00210F63 File Offset: 0x0020F163
		// (set) Token: 0x06002F93 RID: 12179 RVA: 0x00210F6B File Offset: 0x0020F16B
		public bool isPressed { get; private set; }

		// Token: 0x06002F94 RID: 12180 RVA: 0x00210F74 File Offset: 0x0020F174
		private void Awake()
		{
			if (SystemInfo.deviceType == 1)
			{
				this.allowMouseControl = false;
			}
		}

		// Token: 0x06002F95 RID: 12181 RVA: 0x00210F85 File Offset: 0x0020F185
		private void Restart()
		{
			this.isPressed = false;
		}

		// Token: 0x06002F96 RID: 12182 RVA: 0x00210F8E File Offset: 0x0020F18E
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (!this.allowMouseControl && TouchButtonExample.IsMousePointerId(eventData.pointerId))
			{
				return;
			}
			this.isPressed = true;
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x00210FAD File Offset: 0x0020F1AD
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (!this.allowMouseControl && TouchButtonExample.IsMousePointerId(eventData.pointerId))
			{
				return;
			}
			this.isPressed = false;
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x00210FCC File Offset: 0x0020F1CC
		private static bool IsMousePointerId(int id)
		{
			return id == -1 || id == -2 || id == -3;
		}

		// Token: 0x04004A5F RID: 19039
		public bool allowMouseControl = true;
	}
}
