using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005FA RID: 1530
public class SuspectListEntryController : MonoBehaviour
{
	// Token: 0x060021B3 RID: 8627 RVA: 0x001CA0D8 File Offset: 0x001C82D8
	public void Setup(GameplayController.History sec)
	{
		this.key = sec;
		GameplayController.Instance.evidenceDictionary.TryGetValue(this.key.evID, ref this.evidence);
		this.VisualUpdate();
		this.evidence.OnNewName += this.VisualUpdate;
		this.evidence.OnNoteAdded += this.VisualUpdate;
		this.evidence.OnDataKeyChange += this.VisualUpdate;
		this.button.OnPress += this.OpenEvidence;
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x001CA170 File Offset: 0x001C8370
	public void OpenEvidence(ButtonController press)
	{
		InterfaceController instance = InterfaceController.Instance;
		Evidence passedEvidence = this.evidence;
		Evidence.DataKey passedEvidenceKey = Evidence.DataKey.name;
		List<Evidence.DataKey> keys = this.key.keys;
		string presetName = "";
		bool worldInteraction = false;
		bool autoPosition = true;
		Interactable interactable = this.evidence.interactable;
		instance.SpawnWindow(passedEvidence, passedEvidenceKey, keys, presetName, worldInteraction, autoPosition, default(Vector2), interactable, null, null, true);
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x001CA1BC File Offset: 0x001C83BC
	public void VisualUpdate()
	{
		if (this.evidence != null)
		{
			if (this.evidenceImage != null && this.key != null)
			{
				this.evidenceImage.texture = this.evidence.GetPhoto(this.key.keys);
			}
			if (this.button != null && this.button.icon != null)
			{
				this.button.icon.sprite = this.evidence.GetIcon();
			}
			if (this.nameText != null && this.key != null)
			{
				this.nameText.text = this.evidence.GetNameForDataKey(this.key.keys);
			}
		}
		string text = string.Empty;
		if (this.key != null && this.key.locationID > 0)
		{
			NewAddress newAddress = null;
			if (CityData.Instance.addressDictionary.TryGetValue(this.key.locationID, ref newAddress) && newAddress.evidenceEntry != null)
			{
				text = newAddress.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
			}
		}
		if (this.key != null && this.key.lastAccess > -1f)
		{
			this.timeText.text = string.Concat(new string[]
			{
				SessionData.Instance.TimeString(this.key.lastAccess, false),
				", ",
				SessionData.Instance.LongDateString(this.key.lastAccess, true, true, true, true, true, true, false, true),
				"\n",
				text
			});
			return;
		}
		this.timeText.text = text;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x001CA35C File Offset: 0x001C855C
	private void OnDestroy()
	{
		this.button.OnPress -= this.OpenEvidence;
		this.evidence.OnNewName -= this.VisualUpdate;
		this.evidence.OnNoteAdded -= this.VisualUpdate;
		this.evidence.OnDataKeyChange -= this.VisualUpdate;
	}

	// Token: 0x04002C22 RID: 11298
	public RectTransform rect;

	// Token: 0x04002C23 RID: 11299
	public ButtonController button;

	// Token: 0x04002C24 RID: 11300
	public Evidence evidence;

	// Token: 0x04002C25 RID: 11301
	public GameplayController.History key;

	// Token: 0x04002C26 RID: 11302
	public TextMeshProUGUI nameText;

	// Token: 0x04002C27 RID: 11303
	public TextMeshProUGUI timeText;

	// Token: 0x04002C28 RID: 11304
	public RawImage evidenceImage;
}
