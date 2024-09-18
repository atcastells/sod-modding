using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005A4 RID: 1444
public class NotificationController : MonoBehaviour
{
	// Token: 0x06001F81 RID: 8065 RVA: 0x001B21D6 File Offset: 0x001B03D6
	private void OnEnable()
	{
		this.UpdateNotifications();
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x001B21DE File Offset: 0x001B03DE
	public void AddNotification(int val)
	{
		this.notifications += val;
		this.UpdateNotifications();
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x001B21F4 File Offset: 0x001B03F4
	public void SetNotifications(int val)
	{
		this.notifications = val;
		this.UpdateNotifications();
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x001B2204 File Offset: 0x001B0404
	public void UpdateNotifications()
	{
		try
		{
			if (this.notifications > 0)
			{
				if (base.gameObject != null)
				{
					base.gameObject.SetActive(true);
				}
				this.numberText.text = this.notifications.ToString();
				if (this.HUDNotificationsIcon != null)
				{
					this.HUDNotificationsIcon.gameObject.SetActive(true);
				}
			}
			else
			{
				if (base.gameObject != null)
				{
					base.gameObject.SetActive(false);
				}
				if (this.HUDNotificationsIcon != null)
				{
					this.HUDNotificationsIcon.gameObject.SetActive(false);
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x001B22BC File Offset: 0x001B04BC
	private void Update()
	{
		this.time += Time.deltaTime * 0.5f;
		if (this.time >= 1f)
		{
			this.time = 0f;
		}
		float num = InterfaceControls.Instance.notificationGlowCurve.Evaluate(this.time);
		this.glowRect.localScale = new Vector3(num, num, num);
		this.glowImg.color = Color.Lerp(InterfaceControls.Instance.notificationColorMin, InterfaceControls.Instance.notificationColorMax, num);
	}

	// Token: 0x04002975 RID: 10613
	[Header("Components")]
	public TextMeshProUGUI numberText;

	// Token: 0x04002976 RID: 10614
	public JuiceController juice;

	// Token: 0x04002977 RID: 10615
	public RectTransform glowRect;

	// Token: 0x04002978 RID: 10616
	public Image glowImg;

	// Token: 0x04002979 RID: 10617
	public RectTransform HUDNotificationsIcon;

	// Token: 0x0400297A RID: 10618
	[Header("State")]
	private float time;

	// Token: 0x0400297B RID: 10619
	public int notifications;
}
