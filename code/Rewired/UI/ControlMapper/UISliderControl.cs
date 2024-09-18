using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	// Token: 0x02000899 RID: 2201
	[AddComponentMenu("")]
	public class UISliderControl : UIControl
	{
		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06002EAC RID: 11948 RVA: 0x0020CFA9 File Offset: 0x0020B1A9
		// (set) Token: 0x06002EAD RID: 11949 RVA: 0x0020CFB1 File Offset: 0x0020B1B1
		public bool showIcon
		{
			get
			{
				return this._showIcon;
			}
			set
			{
				if (this.iconImage == null)
				{
					return;
				}
				this.iconImage.gameObject.SetActive(value);
				this._showIcon = value;
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06002EAE RID: 11950 RVA: 0x0020CFDA File Offset: 0x0020B1DA
		// (set) Token: 0x06002EAF RID: 11951 RVA: 0x0020CFE2 File Offset: 0x0020B1E2
		public bool showSlider
		{
			get
			{
				return this._showSlider;
			}
			set
			{
				if (this.slider == null)
				{
					return;
				}
				this.slider.gameObject.SetActive(value);
				this._showSlider = value;
			}
		}

		// Token: 0x06002EB0 RID: 11952 RVA: 0x0020D00C File Offset: 0x0020B20C
		public override void SetCancelCallback(Action cancelCallback)
		{
			base.SetCancelCallback(cancelCallback);
			if (cancelCallback == null || this.slider == null)
			{
				return;
			}
			if (this.slider is ICustomSelectable)
			{
				(this.slider as ICustomSelectable).CancelEvent += delegate()
				{
					cancelCallback.Invoke();
				};
				return;
			}
			EventTrigger eventTrigger = this.slider.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = this.slider.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.callback = new EventTrigger.TriggerEvent();
			entry.eventID = 16;
			entry.callback.AddListener(delegate(BaseEventData data)
			{
				cancelCallback.Invoke();
			});
			if (eventTrigger.triggers == null)
			{
				eventTrigger.triggers = new List<EventTrigger.Entry>();
			}
			eventTrigger.triggers.Add(entry);
		}

		// Token: 0x040049D9 RID: 18905
		public Image iconImage;

		// Token: 0x040049DA RID: 18906
		public Slider slider;

		// Token: 0x040049DB RID: 18907
		private bool _showIcon;

		// Token: 0x040049DC RID: 18908
		private bool _showSlider;
	}
}
