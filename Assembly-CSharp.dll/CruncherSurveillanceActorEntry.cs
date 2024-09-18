using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200029B RID: 667
public class CruncherSurveillanceActorEntry : MonoBehaviour
{
	// Token: 0x06000EE6 RID: 3814 RVA: 0x000D5B88 File Offset: 0x000D3D88
	public void Setup(SurveillanceApp newParent, Human newHuman)
	{
		this.appParent = newParent;
		this.human = newHuman;
		this.SetOnOver(false, true);
		this.LoadHeadshot();
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x000D5BA8 File Offset: 0x000D3DA8
	public void LoadHeadshot()
	{
		if (!this.loadedHeadshot)
		{
			List<Evidence.DataKey> list = new List<Evidence.DataKey>();
			list.Add(Evidence.DataKey.photo);
			this.headshotImg.texture = this.human.evidenceEntry.GetPhoto(list);
			this.loadedHeadshot = true;
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x000D5BF0 File Offset: 0x000D3DF0
	public void SetOnOver(bool val, bool forceUpdate = false)
	{
		if (val != this.isOver || forceUpdate)
		{
			this.isOver = val;
			this.UpdateText();
			if (this.isOver)
			{
				SceneRecorder.ActorCapture actorCapture = this.appParent.currentScene.aCap.Find((SceneRecorder.ActorCapture item) => item.id == this.human.humanID);
				if (actorCapture != null)
				{
					Human human = actorCapture.GetHuman();
					this.appParent.locator.localPosition = new Vector2(human.lastUsedCCTVScreenPoint.x * this.appParent.captureRect.sizeDelta.x, human.lastUsedCCTVScreenPoint.y * this.appParent.captureRect.sizeDelta.y);
					this.appParent.locator.gameObject.SetActive(true);
				}
				else
				{
					Game.Log("Unable to find actor cap with human ID " + this.human.humanID.ToString(), 2);
				}
			}
			this.namePopup.gameObject.SetActive(this.isOver);
			this.appParent.UpdateTimelineFlagging();
		}
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000D5D0C File Offset: 0x000D3F0C
	public void UpdateText()
	{
		this.popupText.text = this.human.evidenceEntry.GetNameForDataKey(Evidence.DataKey.photo);
		if (this.isOver && this.human.job != null && this.human.job.employer != null && this.appParent.controller.ic != null && this.appParent.controller.ic.interactable.node.gameLocation == this.human.job.employer.address)
		{
			this.human.evidenceEntry.MergeDataKeys(Evidence.DataKey.photo, Evidence.DataKey.name);
			this.human.evidenceEntry.MergeDataKeys(Evidence.DataKey.photo, Evidence.DataKey.work);
			this.human.evidenceEntry.AddDiscovery(Evidence.Discovery.jobDiscovery);
			this.popupText.text = this.human.evidenceEntry.GetNameForDataKey(Evidence.DataKey.photo) + "\n" + Strings.Get("jobs", this.human.job.preset.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
		}
		this.namePopup.sizeDelta = new Vector2(this.namePopup.sizeDelta.x, this.popupText.preferredHeight + 0.02f);
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x000D5E6F File Offset: 0x000D406F
	public void Press()
	{
		this.appParent.SelectActor(this);
	}

	// Token: 0x04001211 RID: 4625
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04001212 RID: 4626
	public RawImage headshotImg;

	// Token: 0x04001213 RID: 4627
	public bool loadedHeadshot;

	// Token: 0x04001214 RID: 4628
	public ComputerOSUIComponent component;

	// Token: 0x04001215 RID: 4629
	public RectTransform namePopup;

	// Token: 0x04001216 RID: 4630
	public TextMeshProUGUI popupText;

	// Token: 0x04001217 RID: 4631
	public SurveillanceApp appParent;

	// Token: 0x04001218 RID: 4632
	public Human human;

	// Token: 0x04001219 RID: 4633
	public bool isOver;
}
