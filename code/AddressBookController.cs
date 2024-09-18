using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class AddressBookController : MonoBehaviour
{
	// Token: 0x06001D57 RID: 7511 RVA: 0x0019EF18 File Offset: 0x0019D118
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
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x0019EF9C File Offset: 0x0019D19C
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001D59 RID: 7513 RVA: 0x0019EFFC File Offset: 0x0019D1FC
	public void CheckEnabled()
	{
		this.descriptionText.text = string.Empty;
		HashSet<Human> hashSet = new HashSet<Human>();
		string text = string.Empty;
		NewAddress newAddress = null;
		Evidence passedEvidence = this.parentWindow.passedEvidence;
		if (this.parentWindow.passedInteractable != null)
		{
			Interactable.Passed passed = this.parentWindow.passedInteractable.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.addressID);
			if (passed != null)
			{
				CityData.Instance.addressDictionary.TryGetValue(Mathf.RoundToInt(passed.value), ref newAddress);
			}
		}
		else if (this.parentWindow.passedEvidence.interactable != null)
		{
			Interactable.Passed passed2 = this.parentWindow.passedEvidence.interactable.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.addressID);
			if (passed2 != null)
			{
				CityData.Instance.addressDictionary.TryGetValue(Mathf.RoundToInt(passed2.value), ref newAddress);
			}
		}
		if (this.parentWindow.passedInteractable != null && this.parentWindow.passedInteractable.belongsTo != null)
		{
			newAddress = this.parentWindow.passedInteractable.belongsTo.home;
		}
		if (newAddress == null)
		{
			return;
		}
		Game.Log("Checking " + newAddress.inhabitants.Count.ToString() + " inhabitants...", 2);
		HashSet<string> hashSet2 = new HashSet<string>();
		List<Human> list = new List<Human>(newAddress.inhabitants);
		list.Sort((Human p1, Human p2) => p1.humanID.CompareTo(p2.humanID));
		foreach (Human human in list)
		{
			Game.Log(human.GetCitizenName() + " has " + human.acquaintances.Count.ToString() + " aquaintances...", 2);
			string text2 = string.Empty;
			if (human.handwriting != null)
			{
				text2 = "<font=\"" + human.handwriting.fontAsset.name + "\">";
			}
			List<Acquaintance> list2 = new List<Acquaintance>(human.acquaintances);
			list2.Sort((Acquaintance p2, Acquaintance p1) => p1.known.CompareTo(p2.known));
			foreach (Acquaintance acquaintance in list2)
			{
				if (!(acquaintance.with.home == null) && !(acquaintance.from.home == null))
				{
					if (human != acquaintance.with && (acquaintance.with.home.telephones == null || acquaintance.with.home.telephones.Count <= 0))
					{
						Game.Log(acquaintance.with.GetCitizenName() + " has no telephone at their home!", 2);
					}
					else if (human != acquaintance.from && (acquaintance.from.home.telephones == null || acquaintance.from.home.telephones.Count <= 0))
					{
						Game.Log(acquaintance.from.GetCitizenName() + " has no telephone at their home!", 2);
					}
					else if (acquaintance.known >= SocialControls.Instance.telephoneBookInclusionThreshold)
					{
						Game.Log("Aq know enough to feature: " + acquaintance.known.ToString(), 2);
						if (!hashSet.Contains(acquaintance.with) && !newAddress.inhabitants.Contains(acquaintance.with))
						{
							Strings.LinkData linkData = Strings.AddOrGetLink(acquaintance.with.home.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
							{
								Evidence.DataKey.name,
								Evidence.DataKey.location,
								Evidence.DataKey.address,
								Evidence.DataKey.telephoneNumber
							}));
							Strings.LinkData linkData2 = Strings.AddOrGetLink(acquaintance.with.home.telephones[0].telephoneEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
							{
								Evidence.DataKey.initialedName,
								Evidence.DataKey.address,
								Evidence.DataKey.telephoneNumber
							}));
							string text3 = string.Empty;
							Strings.LinkData linkData3;
							if (hashSet2.Contains(text3))
							{
								text3 = acquaintance.with.GetInitialledName();
								linkData3 = Strings.AddOrGetLink(acquaintance.with.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
								{
									Evidence.DataKey.initialedName,
									Evidence.DataKey.address,
									Evidence.DataKey.telephoneNumber
								}));
							}
							else
							{
								text3 = acquaintance.with.GetCasualName();
								linkData3 = Strings.AddOrGetLink(acquaintance.with.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
								{
									Evidence.DataKey.firstName,
									Evidence.DataKey.address,
									Evidence.DataKey.telephoneNumber
								}));
							}
							hashSet2.Add(text3);
							text = string.Concat(new string[]
							{
								text,
								"\n",
								text2,
								"<link=",
								linkData3.id.ToString(),
								">",
								text3,
								"</link>\n<link=",
								linkData.id.ToString(),
								">",
								acquaintance.with.home.name,
								"</link>\n<link=",
								linkData2.id.ToString(),
								">",
								acquaintance.with.home.telephones[0].numberString,
								"</link>\n"
							});
							hashSet.Add(acquaintance.with);
						}
						else
						{
							Game.Log("Closed set contains " + acquaintance.with.GetCitizenName(), 2);
						}
						Game.Log("Aq from " + acquaintance.from.GetCitizenName(), 2);
						if (!hashSet.Contains(acquaintance.from) && !newAddress.inhabitants.Contains(acquaintance.from) && acquaintance.from.home != null && acquaintance.from.home.telephones.Count > 0)
						{
							Strings.LinkData linkData4 = Strings.AddOrGetLink(acquaintance.from.home.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
							{
								Evidence.DataKey.initialedName,
								Evidence.DataKey.address,
								Evidence.DataKey.telephoneNumber
							}));
							Strings.LinkData linkData5 = Strings.AddOrGetLink(acquaintance.from.home.telephones[0].telephoneEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
							{
								Evidence.DataKey.initialedName,
								Evidence.DataKey.address,
								Evidence.DataKey.telephoneNumber
							}));
							string text4 = string.Empty;
							Strings.LinkData linkData6;
							if (hashSet2.Contains(text4))
							{
								text4 = acquaintance.from.GetInitialledName();
								linkData6 = Strings.AddOrGetLink(acquaintance.from.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
								{
									Evidence.DataKey.initialedName,
									Evidence.DataKey.address,
									Evidence.DataKey.telephoneNumber
								}));
							}
							else
							{
								text4 = acquaintance.from.GetCasualName();
								linkData6 = Strings.AddOrGetLink(acquaintance.from.evidenceEntry, Toolbox.Instance.GetList<Evidence.DataKey>(new Evidence.DataKey[]
								{
									Evidence.DataKey.firstName,
									Evidence.DataKey.address,
									Evidence.DataKey.telephoneNumber
								}));
							}
							hashSet2.Add(text4);
							text = string.Concat(new string[]
							{
								text,
								"\n",
								text2,
								"<link=",
								linkData6.id.ToString(),
								">",
								text4,
								"</link>\n<link=",
								linkData4.id.ToString(),
								">",
								acquaintance.from.home.name,
								"</link>\n<link=",
								linkData5.id.ToString(),
								">",
								acquaintance.from.home.telephones[0].numberString,
								"</link>\n"
							});
							hashSet.Add(acquaintance.with);
						}
						else
						{
							Game.Log("Closed set contains " + acquaintance.from.GetCitizenName(), 2);
						}
					}
				}
			}
		}
		this.descriptionText.text = text;
	}

	// Token: 0x04002711 RID: 10001
	public WindowContentController windowContent;

	// Token: 0x04002712 RID: 10002
	public InfoWindow parentWindow;

	// Token: 0x04002713 RID: 10003
	public TextMeshProUGUI descriptionText;
}
