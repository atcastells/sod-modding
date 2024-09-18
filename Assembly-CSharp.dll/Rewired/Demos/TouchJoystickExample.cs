using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	// Token: 0x020008B1 RID: 2225
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("")]
	public class TouchJoystickExample : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06002F9A RID: 12186 RVA: 0x00210FED File Offset: 0x0020F1ED
		// (set) Token: 0x06002F9B RID: 12187 RVA: 0x00210FF5 File Offset: 0x0020F1F5
		public Vector2 position { get; private set; }

		// Token: 0x06002F9C RID: 12188 RVA: 0x00210FFE File Offset: 0x0020F1FE
		private void Start()
		{
			if (SystemInfo.deviceType == 1)
			{
				this.allowMouseControl = false;
			}
			this.StoreOrigValues();
		}

		// Token: 0x06002F9D RID: 12189 RVA: 0x00211018 File Offset: 0x0020F218
		private void Update()
		{
			if ((float)Screen.width != this.origScreenResolution.x || (float)Screen.height != this.origScreenResolution.y || Screen.orientation != this.origScreenOrientation)
			{
				this.Restart();
				this.StoreOrigValues();
			}
		}

		// Token: 0x06002F9E RID: 12190 RVA: 0x00211064 File Offset: 0x0020F264
		private void Restart()
		{
			this.hasFinger = false;
			(base.transform as RectTransform).anchoredPosition = this.origAnchoredPosition;
			this.position = Vector2.zero;
		}

		// Token: 0x06002F9F RID: 12191 RVA: 0x00211090 File Offset: 0x0020F290
		private void StoreOrigValues()
		{
			this.origAnchoredPosition = (base.transform as RectTransform).anchoredPosition;
			this.origWorldPosition = base.transform.position;
			this.origScreenResolution = new Vector2((float)Screen.width, (float)Screen.height);
			this.origScreenOrientation = Screen.orientation;
		}

		// Token: 0x06002FA0 RID: 12192 RVA: 0x002110E8 File Offset: 0x0020F2E8
		private void UpdateValue(Vector3 value)
		{
			Vector3 vector = this.origWorldPosition - value;
			vector.y = -vector.y;
			vector /= (float)this.radius;
			this.position = new Vector2(-vector.x, vector.y);
		}

		// Token: 0x06002FA1 RID: 12193 RVA: 0x00211136 File Offset: 0x0020F336
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.hasFinger)
			{
				return;
			}
			if (!this.allowMouseControl && TouchJoystickExample.IsMousePointerId(eventData.pointerId))
			{
				return;
			}
			this.hasFinger = true;
			this.lastFingerId = eventData.pointerId;
		}

		// Token: 0x06002FA2 RID: 12194 RVA: 0x0021116A File Offset: 0x0020F36A
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (eventData.pointerId != this.lastFingerId)
			{
				return;
			}
			if (!this.allowMouseControl && TouchJoystickExample.IsMousePointerId(eventData.pointerId))
			{
				return;
			}
			this.Restart();
		}

		// Token: 0x06002FA3 RID: 12195 RVA: 0x00211198 File Offset: 0x0020F398
		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (!this.hasFinger || eventData.pointerId != this.lastFingerId)
			{
				return;
			}
			Vector3 vector;
			vector..ctor(eventData.position.x - this.origWorldPosition.x, eventData.position.y - this.origWorldPosition.y);
			vector = Vector3.ClampMagnitude(vector, (float)this.radius);
			Vector3 vector2 = this.origWorldPosition + vector;
			base.transform.position = vector2;
			this.UpdateValue(vector2);
		}

		// Token: 0x06002FA4 RID: 12196 RVA: 0x00210FCC File Offset: 0x0020F1CC
		private static bool IsMousePointerId(int id)
		{
			return id == -1 || id == -2 || id == -3;
		}

		// Token: 0x04004A61 RID: 19041
		public bool allowMouseControl = true;

		// Token: 0x04004A62 RID: 19042
		public int radius = 50;

		// Token: 0x04004A63 RID: 19043
		private Vector2 origAnchoredPosition;

		// Token: 0x04004A64 RID: 19044
		private Vector3 origWorldPosition;

		// Token: 0x04004A65 RID: 19045
		private Vector2 origScreenResolution;

		// Token: 0x04004A66 RID: 19046
		private ScreenOrientation origScreenOrientation;

		// Token: 0x04004A67 RID: 19047
		[NonSerialized]
		private bool hasFinger;

		// Token: 0x04004A68 RID: 19048
		[NonSerialized]
		private int lastFingerId;
	}
}
