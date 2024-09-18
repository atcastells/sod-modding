using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000557 RID: 1367
public class CallLogsContentController : MonoBehaviour
{
	// Token: 0x06001DC4 RID: 7620 RVA: 0x001A3354 File Offset: 0x001A1554
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.windowContent == null)
		{
			this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		}
		this.CheckEnabled();
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x001A33A5 File Offset: 0x001A15A5
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x001A33C8 File Offset: 0x001A15C8
	public void CheckEnabled()
	{
		NewBuilding building = this.parentWindow.passedEvidence.interactable.node.building;
		if (this.incoming)
		{
			this.titleText.text = Strings.Get("ui.interface", "Incoming calls from", Strings.Casing.asIs, false, false, false, null) + " " + building.name;
		}
		else
		{
			this.titleText.text = Strings.Get("ui.interface", "Outgoing calls from", Strings.Casing.asIs, false, false, false, null) + " " + building.name;
		}
		List<TelephoneController.PhoneCall> list;
		if (this.incoming)
		{
			list = building.callLog.FindAll((TelephoneController.PhoneCall item) => item.toNS != null && item.toNS.interactable != null && (item.toNS.interactable.node != null & item.toNS.interactable.node.gameLocation.thisAsAddress != null) && item.toNS.interactable.node.building == building);
		}
		else
		{
			list = building.callLog.FindAll((TelephoneController.PhoneCall item) => item.fromNS != null && item.fromNS.interactable != null && (item.fromNS.interactable.node != null & item.fromNS.interactable.node.gameLocation.thisAsAddress != null) && item.fromNS.interactable.node.building == building);
		}
		while (this.spawnedEntries.Count > 0)
		{
			Object.Destroy(this.spawnedEntries[0].gameObject);
			this.spawnedEntries.RemoveAt(0);
		}
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			TelephoneController.PhoneCall newLogged = list[i];
			CallLogsEntryController component = Object.Instantiate<GameObject>(this.entryPrefab, base.transform).GetComponent<CallLogsEntryController>();
			component.Setup(newLogged, building);
			num += component.rect.sizeDelta.y + this.layout.spacing;
			this.spawnedEntries.Add(component);
		}
		this.windowContent.pageRect.sizeDelta = new Vector2(this.windowContent.pageRect.sizeDelta.x, Mathf.Max(this.windowContent.pageRect.sizeDelta.y, num + 38f));
		this.windowContent.rect.sizeDelta = new Vector2(this.windowContent.pageRect.sizeDelta.x, Mathf.Max(this.windowContent.pageRect.sizeDelta.y, num + 100f));
	}

	// Token: 0x040027A1 RID: 10145
	public WindowContentController windowContent;

	// Token: 0x040027A2 RID: 10146
	public InfoWindow parentWindow;

	// Token: 0x040027A3 RID: 10147
	public TextMeshProUGUI descriptionText;

	// Token: 0x040027A4 RID: 10148
	public GameObject entryPrefab;

	// Token: 0x040027A5 RID: 10149
	public bool incoming;

	// Token: 0x040027A6 RID: 10150
	public TextMeshProUGUI titleText;

	// Token: 0x040027A7 RID: 10151
	public List<CallLogsEntryController> spawnedEntries = new List<CallLogsEntryController>();

	// Token: 0x040027A8 RID: 10152
	public VerticalLayoutGroup layout;
}
