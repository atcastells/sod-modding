using System;
using TMPro;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class CallLogsEntryController : MonoBehaviour
{
	// Token: 0x06001DCB RID: 7627 RVA: 0x001A36FC File Offset: 0x001A18FC
	public void Setup(TelephoneController.PhoneCall newLogged, NewBuilding newBuilding)
	{
		this.logged = newLogged;
		this.building = newBuilding;
		this.fromButton.text.text = Strings.Get("evidence.generic", "From", Strings.Casing.asIs, false, false, false, null) + " > ";
		if (this.logged.fromNS != null && this.logged.fromNS.interactable != null)
		{
			if (this.logged.fromNS.interactable.node.building != null && this.logged.fromNS.interactable.node.building == this.building)
			{
				TextMeshProUGUI text = this.fromButton.text;
				text.text += this.logged.fromNS.interactable.node.gameLocation.name;
			}
			else if (this.logged.fromNS.interactable.node.building != null)
			{
				TextMeshProUGUI text2 = this.fromButton.text;
				text2.text += this.logged.fromNS.interactable.node.building.name;
			}
			else if (this.logged.fromNS.interactable.node.gameLocation != null)
			{
				TextMeshProUGUI text3 = this.fromButton.text;
				text3.text += this.logged.fromNS.interactable.node.gameLocation.name;
			}
		}
		else
		{
			Game.LogError("Unable to get telephone from " + this.logged.from.ToString(), 2);
			TextMeshProUGUI text4 = this.fromButton.text;
			text4.text += Strings.Get("evidence.generic", "Unknown Location", Strings.Casing.asIs, false, false, false, null).ToUpper();
		}
		this.toButton.text.text = Strings.Get("evidence.generic", "To", Strings.Casing.asIs, false, false, false, null) + " > ";
		if (this.logged.toNS != null && this.logged.toNS.interactable != null)
		{
			if (this.logged.toNS.interactable.node.building != null && this.logged.toNS.interactable.node.building == this.building)
			{
				TextMeshProUGUI text5 = this.toButton.text;
				text5.text += this.logged.toNS.interactable.node.gameLocation.name;
			}
			else if (this.logged.toNS.interactable.node.building != null)
			{
				TextMeshProUGUI text6 = this.toButton.text;
				text6.text += this.logged.toNS.interactable.node.building.name;
			}
			else if (this.logged.toNS.interactable.node.gameLocation != null)
			{
				TextMeshProUGUI text7 = this.toButton.text;
				text7.text += this.logged.toNS.interactable.node.gameLocation.name;
			}
		}
		else
		{
			Game.LogError("Unable to get telephone from " + this.logged.to.ToString(), 2);
			TextMeshProUGUI text8 = this.toButton.text;
			text8.text += Strings.Get("evidence.generic", "Unknown Location", Strings.Casing.asIs, false, false, false, null).ToUpper();
		}
		try
		{
			Strings.LinkData linkData = Strings.AddOrGetLink(EvidenceCreator.Instance.GetTimeEvidence(this.logged.time, this.logged.time, "TelephoneCall", this.logged.fromNS.telephoneEntry.evID + ">" + this.logged.toNS.telephoneEntry.evID, -1, -1), null);
			this.timeText.text = string.Concat(new string[]
			{
				"<link=",
				linkData.id.ToString(),
				">",
				SessionData.Instance.ShortDateString(this.logged.time, false),
				" ",
				SessionData.Instance.GameTimeToClock24String(this.logged.time, false),
				"</link>"
			});
			this.durationText.text = string.Empty;
		}
		catch
		{
			this.timeText.text = string.Empty;
			this.durationText.text = string.Empty;
		}
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x001A3C18 File Offset: 0x001A1E18
	public void FromButton()
	{
		try
		{
			if (this.logged.fromNS != null && this.logged.fromNS.interactable != null)
			{
				if (this.logged.fromNS.interactable.node.building != null && this.logged.fromNS.interactable.node.building == this.building)
				{
					SessionData.Instance.PauseGame(true, false, true);
					InterfaceController.Instance.SpawnWindow(this.logged.fromNS.interactable.node.gameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
				}
				else if (this.logged.fromNS.interactable.node.building != null)
				{
					SessionData.Instance.PauseGame(true, false, true);
					InterfaceController.Instance.SpawnWindow(this.logged.fromNS.interactable.node.building.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
				}
				else if (this.logged.fromNS.interactable.node.gameLocation != null)
				{
					SessionData.Instance.PauseGame(true, false, true);
					InterfaceController.Instance.SpawnWindow(this.logged.fromNS.interactable.node.gameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
				}
			}
		}
		catch
		{
			Game.Log("Unable to open call 'from' location", 2);
		}
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x001A3DF4 File Offset: 0x001A1FF4
	public void ToButton()
	{
		try
		{
			if (this.logged.toNS.interactable.node.building != null && this.logged.toNS.interactable.node.building == this.building)
			{
				SessionData.Instance.PauseGame(true, false, true);
				InterfaceController.Instance.SpawnWindow(this.logged.toNS.interactable.node.gameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			}
			else if (this.logged.toNS.interactable.node.building != null)
			{
				SessionData.Instance.PauseGame(true, false, true);
				InterfaceController.Instance.SpawnWindow(this.logged.toNS.interactable.node.building.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			}
			else if (this.logged.toNS.interactable.node.gameLocation != null)
			{
				SessionData.Instance.PauseGame(true, false, true);
				InterfaceController.Instance.SpawnWindow(this.logged.toNS.interactable.node.gameLocation.evidenceEntry, Evidence.DataKey.name, null, "", false, true, default(Vector2), null, null, null, true);
			}
		}
		catch
		{
			Game.Log("Unable to open call 'to' location", 2);
		}
	}

	// Token: 0x040027AA RID: 10154
	public RectTransform rect;

	// Token: 0x040027AB RID: 10155
	public NewBuilding building;

	// Token: 0x040027AC RID: 10156
	public TelephoneController.PhoneCall logged;

	// Token: 0x040027AD RID: 10157
	public TextMeshProUGUI timeText;

	// Token: 0x040027AE RID: 10158
	public TextMeshProUGUI durationText;

	// Token: 0x040027AF RID: 10159
	public ButtonController fromButton;

	// Token: 0x040027B0 RID: 10160
	public ButtonController toButton;
}
