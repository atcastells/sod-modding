using System;
using System.Collections.Generic;
using UnityEngine.UI;

// Token: 0x02000587 RID: 1415
public class PhotoSelectButtonController : ButtonController
{
	// Token: 0x06001EEF RID: 7919 RVA: 0x001AD2D3 File Offset: 0x001AB4D3
	public void Setup(Human newCitizen, Case.CaseElement newCaseElement, InfoWindow newThisWindow)
	{
		base.SetupReferences();
		this.citizen = newCitizen;
		this.element = newCaseElement;
		this.thisWindow = newThisWindow;
		this.UpdateButtonText();
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x001AD2F8 File Offset: 0x001AB4F8
	public override void UpdateButtonText()
	{
		if (this.citizen != null)
		{
			this.photo.texture = this.citizen.evidenceEntry.GetPhoto(this.element.dk);
			this.text.text = this.citizen.evidenceEntry.GetNameForDataKey(this.element.dk);
		}
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x001AD360 File Offset: 0x001AB560
	public override void OnLeftClick()
	{
		if (InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor != null)
		{
			DialogController.Instance.askTarget = this.citizen;
			DialogController.Instance.askTargetKeys = this.citizen.evidenceEntry.GetTiedKeys(this.element.dk);
			Game.Log("Ask target: " + this.citizen.name, 2);
			Game.Log("Ask target keys: " + DialogController.Instance.askTargetKeys.Count.ToString(), 2);
			Human human = InteractionController.Instance.talkingTo.isActor as Human;
			Acquaintance acquaintance = null;
			if ((DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.name) || DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.photo)) && (human.FindAcquaintanceExists(this.citizen, out acquaintance) || this.citizen == human))
			{
				if (this.citizen == human)
				{
					human.speechController.Speak("4e8db6e9-04fe-4c5b-9bf6-05d8c3f8a230", false, false, this.citizen, null);
					this.MergeTargetKeys(Evidence.DataKey.name);
					this.MergeTargetKeys(Evidence.DataKey.photo);
					this.MergeTargetKeys(Evidence.DataKey.voice);
				}
				else if (acquaintance != null)
				{
					if (acquaintance.connections.Contains(Acquaintance.ConnectionType.lover))
					{
						human.speechController.Speak("694fc0ab-2b16-43ba-9ce0-8954a27bb652", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.name);
					}
					else if (acquaintance.connections.Contains(Acquaintance.ConnectionType.friend))
					{
						human.speechController.Speak("8b710c17-57f2-4157-b02f-2be3bbfde699", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.name);
					}
					else if (acquaintance.connections.Contains(Acquaintance.ConnectionType.neighbor))
					{
						human.speechController.Speak("bbc084dc-d268-4bc4-bd9f-9a8ee9824a06", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.name);
					}
					else if ((acquaintance.connections.Contains(Acquaintance.ConnectionType.workOther) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workTeam) || acquaintance.connections.Contains(Acquaintance.ConnectionType.familiarWork) || acquaintance.connections.Contains(Acquaintance.ConnectionType.boss)) && this.citizen.job != null && this.citizen.job.employer != null)
					{
						human.speechController.Speak("84ef41ab-03fe-4888-a8fe-b4c71486dca4", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.name);
					}
					else
					{
						human.speechController.Speak("0e443d05-2bd7-4a52-adcb-5957a5d82860", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.name);
					}
					float psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, human.citizenName + this.citizen.citizenName, false);
					if (psuedoRandomNumber <= 0.33f)
					{
						if (this.citizen.characterTraits.Exists((Human.Trait item) => item.trait.featureInAfflictionPool))
						{
							human.speechController.Speak("f98fdfc4-88ff-46bb-80eb-115995da7b73", false, false, this.citizen, null);
							this.MergeTargetKeys(Evidence.DataKey.randomAffliction);
							goto IL_3A8;
						}
					}
					if (psuedoRandomNumber > 0.33f && psuedoRandomNumber <= 0.66f)
					{
						if (this.citizen.characterTraits.Exists((Human.Trait item) => item.trait.featureInInterestPool))
						{
							human.speechController.Speak("7448c520-bf46-4da7-95cd-bbd9c20eb9ac", false, false, this.citizen, null);
							this.MergeTargetKeys(Evidence.DataKey.randomInterest);
							goto IL_3A8;
						}
					}
					if (this.citizen.job != null && this.citizen.job.employer != null)
					{
						human.speechController.Speak("314ca1e3-cd3f-4773-b32e-ec39c2ec3194", false, false, this.citizen, null);
						this.MergeTargetKeys(Evidence.DataKey.work);
					}
					IL_3A8:
					if (this.citizen.home == human.home)
					{
						human.speechController.Speak("3a98db60-c57f-4d6b-b2b3-bfb19d2da5c0", false, false, this.citizen, null);
						human.evidenceEntry.GetTiedKeys(Evidence.DataKey.address);
						foreach (Evidence.DataKey inputKey in human.evidenceEntry.GetTiedKeys(Evidence.DataKey.voice))
						{
							if (human.evidenceEntry.GetTiedKeys(inputKey).Contains(Evidence.DataKey.address))
							{
								this.MergeTargetKeys(Evidence.DataKey.address);
								break;
							}
						}
					}
					if (acquaintance.known >= SocialControls.Instance.knowImmediateLocationThreshold && !this.citizen.isDead && this.citizen.currentGameLocation.thisAsAddress != null && this.citizen.currentGameLocation.thisAsAddress.company != null)
					{
						human.speechController.Speak("d91b9493-a847-4074-833c-900e445095de", false, false, this.citizen, null);
					}
					else
					{
						human.RevealSighting(this.citizen, true, false, true);
					}
				}
			}
			else
			{
				bool flag = false;
				if (DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.photo))
				{
					human.RevealSighting(this.citizen, false, false, true);
					flag = true;
				}
				else
				{
					if (DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.name))
					{
						using (List<Acquaintance>.Enumerator enumerator2 = human.acquaintances.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Acquaintance acquaintance2 = enumerator2.Current;
								Human other = acquaintance2.GetOther(human);
								if (other.GetCitizenName() == this.citizen.GetCitizenName())
								{
									human.RevealSighting(other, true, false, true);
									flag = true;
									break;
								}
							}
							goto IL_706;
						}
					}
					if (DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.firstName))
					{
						using (List<Acquaintance>.Enumerator enumerator2 = human.acquaintances.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Acquaintance acquaintance3 = enumerator2.Current;
								Human other2 = acquaintance3.GetOther(human);
								if (other2.GetFirstName() == this.citizen.GetFirstName())
								{
									human.speechController.Speak("ed7d288d-9044-4979-a705-bf6cb583c652", false, false, other2, null);
									human.RevealSighting(other2, true, false, true);
									flag = true;
									break;
								}
							}
							goto IL_706;
						}
					}
					if (DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.surname))
					{
						using (List<Acquaintance>.Enumerator enumerator2 = human.acquaintances.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Acquaintance acquaintance4 = enumerator2.Current;
								Human other3 = acquaintance4.GetOther(human);
								if (other3.GetSurName() == this.citizen.GetSurName())
								{
									human.speechController.Speak("6bb725e2-f98d-42b9-ae77-b2f9ecfe70fd", false, false, other3, null);
									human.RevealSighting(other3, true, false, true);
									flag = true;
									break;
								}
							}
							goto IL_706;
						}
					}
					if (DialogController.Instance.askTargetKeys.Contains(Evidence.DataKey.initials))
					{
						foreach (Acquaintance acquaintance5 in human.acquaintances)
						{
							Human other4 = acquaintance5.GetOther(human);
							if (other4.GetInitials() == this.citizen.GetInitials())
							{
								human.speechController.Speak("6fedb3fb-31ba-4d15-a3c5-2f60f53eb652", false, false, other4, null);
								human.RevealSighting(other4, true, false, true);
								flag = true;
								break;
							}
						}
					}
				}
				IL_706:
				if (!flag)
				{
					human.speechController.Speak("a6815309-f9d4-40b0-8a1e-3ec3550c64a2", false, false, this.citizen, null);
					flag = true;
				}
			}
		}
		this.thisWindow.CloseWindow(true);
		base.OnLeftClick();
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x001ADAE8 File Offset: 0x001ABCE8
	private void MergeTargetKeys(Evidence.DataKey key)
	{
		foreach (Evidence.DataKey keyOne in DialogController.Instance.askTargetKeys)
		{
			this.citizen.evidenceEntry.MergeDataKeys(keyOne, key);
		}
	}

	// Token: 0x040028BB RID: 10427
	public Human citizen;

	// Token: 0x040028BC RID: 10428
	public Case.CaseElement element;

	// Token: 0x040028BD RID: 10429
	public RawImage photo;

	// Token: 0x040028BE RID: 10430
	public InfoWindow thisWindow;
}
