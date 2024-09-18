using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000624 RID: 1572
public class EvidenceCreator : MonoBehaviour
{
	// Token: 0x17000112 RID: 274
	// (get) Token: 0x060022DF RID: 8927 RVA: 0x001D4EEC File Offset: 0x001D30EC
	public static EvidenceCreator Instance
	{
		get
		{
			return EvidenceCreator._instance;
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x001D4EF3 File Offset: 0x001D30F3
	private void Awake()
	{
		if (EvidenceCreator._instance != null && EvidenceCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		EvidenceCreator._instance = this;
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x001D4F24 File Offset: 0x001D3124
	public Evidence GetDateEvidence(string date, string evidenceType = "date", string parentID = "", int owner = -1, int writer = -1, int reciever = -1)
	{
		string generateEvidenceKey = string.Concat(new string[]
		{
			evidenceType,
			"|",
			date,
			"|",
			parentID,
			"|",
			writer.ToString(),
			"|",
			reciever.ToString()
		});
		Evidence evidence = GameplayController.Instance.dateEvidence.Find((EvidenceDate item) => item.evID == generateEvidenceKey);
		if (evidence == null)
		{
			Evidence newParent = null;
			GameplayController.Instance.evidenceDictionary.TryGetValue(parentID, ref newParent);
			Human newOwner = null;
			CityData.Instance.GetHuman(owner, out newOwner, true);
			Human newWriter = null;
			CityData.Instance.GetHuman(writer, out newWriter, true);
			Human newReciever = null;
			CityData.Instance.GetHuman(reciever, out newWriter, true);
			evidence = EvidenceCreator.Instance.CreateEvidence(evidenceType, generateEvidenceKey, null, newOwner, newWriter, newReciever, newParent, false, null);
		}
		return evidence;
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x001D5014 File Offset: 0x001D3214
	public EvidenceTime GetTimeEvidenceRange(float time, float accuracyRange, bool limitToNow, bool round, int roundToMinutes, string evidenceType = "time", string parentID = "", int writer = -1, int receiver = -1)
	{
		Vector2 vector = Toolbox.Instance.CreateTimeRange(time, accuracyRange, limitToNow, round, roundToMinutes);
		return this.GetTimeEvidence(vector.x, vector.y, evidenceType, parentID, writer, receiver);
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x001D5050 File Offset: 0x001D3250
	public EvidenceTime GetTimeEvidence(string evidenceID)
	{
		EvidenceTime evidenceTime = GameplayController.Instance.timeEvidence.Find((EvidenceTime item) => item.evID == evidenceID);
		if (evidenceTime == null)
		{
			string[] array = evidenceID.Split('|', 0);
			if (array.Length <= 1)
			{
				return null;
			}
			Evidence newParent = null;
			if (array.Length > 3)
			{
				GameplayController.Instance.evidenceDictionary.TryGetValue(array[3], ref newParent);
			}
			Human newWriter = null;
			if (array.Length > 4)
			{
				int id = -1;
				int.TryParse(array[4], ref id);
				CityData.Instance.GetHuman(id, out newWriter, true);
			}
			Human newReciever = null;
			if (array.Length > 5)
			{
				int id2 = -1;
				int.TryParse(array[5], ref id2);
				CityData.Instance.GetHuman(id2, out newWriter, true);
			}
			evidenceTime = (EvidenceCreator.Instance.CreateEvidence(array[0], evidenceID, null, null, newWriter, newReciever, newParent, false, null) as EvidenceTime);
		}
		return evidenceTime;
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x001D5134 File Offset: 0x001D3334
	public EvidenceTime GetTimeEvidence(float from, float to, string evidenceType = "time", string parentID = "", int writer = -1, int reciever = -1)
	{
		string generateEvidenceKey = string.Concat(new string[]
		{
			evidenceType,
			"|",
			from.ToString(),
			"|",
			to.ToString(),
			"|",
			parentID,
			"|",
			writer.ToString(),
			"|",
			reciever.ToString()
		});
		EvidenceTime evidenceTime = GameplayController.Instance.timeEvidence.Find((EvidenceTime item) => item.evID == generateEvidenceKey);
		if (evidenceTime == null)
		{
			Evidence newParent = null;
			GameplayController.Instance.evidenceDictionary.TryGetValue(parentID, ref newParent);
			Human newWriter = null;
			CityData.Instance.GetHuman(writer, out newWriter, true);
			Human newReciever = null;
			CityData.Instance.GetHuman(reciever, out newWriter, true);
			evidenceTime = (EvidenceCreator.Instance.CreateEvidence(evidenceType, generateEvidenceKey, null, null, newWriter, newReciever, newParent, false, null) as EvidenceTime);
		}
		return evidenceTime;
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x001D5230 File Offset: 0x001D3430
	public Evidence CreateEvidence(string presetName, string newID, Controller newController = null, Human newOwner = null, Human newWriter = null, Human newReciever = null, Evidence newParent = null, bool forceDiscoveryOnCreate = false, List<object> passedObjects = null)
	{
		EvidencePreset preset = null;
		presetName = presetName.ToLower();
		if (Toolbox.Instance.evidencePresetDictionary.TryGetValue(presetName, ref preset))
		{
			return this.CreateEvidence(preset, newID, newController, newOwner, newWriter, newReciever, newParent, forceDiscoveryOnCreate, passedObjects);
		}
		Game.LogError("Cannot find evidence " + presetName, 2);
		return null;
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x001D5284 File Offset: 0x001D3484
	public Evidence CreateEvidence(EvidencePreset preset, string newID, Controller newController = null, Human newOwner = null, Human newWriter = null, Human newReciever = null, Evidence newParent = null, bool forceDiscoveryOnCreate = false, List<object> passedObjects = null)
	{
		if (preset == null)
		{
			if (newController != null && newParent != null)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Null evidence preset (Controller: ",
					newController.name,
					" Parent: ",
					newParent.GetNameForDataKey(Evidence.DataKey.name),
					")"
				}), 2);
			}
			else if (newController != null)
			{
				Game.LogError("Null evidence preset (Controller: " + newController.name + ")", 2);
			}
			else if (newParent != null)
			{
				Game.LogError("Null evidence preset (Parent: " + newParent.GetNameForDataKey(Evidence.DataKey.name) + ")", 2);
			}
			else
			{
				Game.LogError("Null evidence preset", 2);
			}
		}
		string text = "Evidence";
		if (preset.subClass.Length > 0)
		{
			text = "Evidence" + preset.subClass;
		}
		object[] array = new object[]
		{
			preset,
			newID,
			newController,
			passedObjects
		};
		Evidence evidence = null;
		try
		{
			evidence = (Activator.CreateInstance(Type.GetType(text), array) as Evidence);
		}
		catch
		{
			Game.Log("Misc Error: Unable to create subclass " + text, 2);
			return null;
		}
		if (newParent != null)
		{
			evidence.SetParent(newParent);
		}
		if (newOwner != null)
		{
			evidence.SetBelongsTo(newOwner);
		}
		if (newWriter != null)
		{
			evidence.SetWriter(newWriter);
		}
		if (newReciever != null)
		{
			evidence.SetReciever(newReciever);
		}
		if (StartingEvidenceCreator.Instance == null || StartingEvidenceCreator.Instance.called)
		{
			evidence.Compile();
			if (forceDiscoveryOnCreate)
			{
				evidence.SetFound(true);
			}
		}
		else
		{
			CityConstructor.Instance.evidenceToCompile.Add(evidence);
		}
		return evidence;
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x001D543C File Offset: 0x001D363C
	public Fact CreateFact(string presetName, Evidence fromEvidenceSingular = null, Evidence toEvidenceSingular = null, List<Evidence> fromEvidence = null, List<Evidence> toEvidence = null, bool forceDiscoveryOnCreate = false, List<object> passedObjects = null, List<Evidence.DataKey> overrideFromKeys = null, List<Evidence.DataKey> overrideToKeys = null, bool isCustomFact = false)
	{
		if (fromEvidence == null)
		{
			fromEvidence = new List<Evidence>();
			fromEvidence.Add(fromEvidenceSingular);
		}
		if (toEvidence == null)
		{
			toEvidence = new List<Evidence>();
			toEvidence.Add(toEvidenceSingular);
		}
		FactPreset preset = null;
		try
		{
			presetName = presetName.ToLower();
			preset = Toolbox.Instance.factPresetDictionary[presetName];
		}
		catch
		{
			Game.LogError("Cannot find fact " + presetName, 2);
			return null;
		}
		if (!preset.allowDuplicates)
		{
			Fact fact = GameplayController.Instance.factList.Find((Fact item) => item.preset == preset && item.fromEvidence.Count > 0 && item.toEvidence.Count > 0 && item.fromEvidence[0] == fromEvidenceSingular && item.toEvidence[0] == toEvidenceSingular);
			if (fact != null)
			{
				Game.Log("Evidence: (Duplicate evidence detected for " + fact.GetName(null) + ", returning duplicate)", 2);
				if (forceDiscoveryOnCreate)
				{
					fact.SetFound(true);
				}
				return fact;
			}
		}
		if (!preset.allowReverseDuplicates)
		{
			Fact fact2 = GameplayController.Instance.factList.Find((Fact item) => item.preset == preset && item.fromEvidence.Count > 0 && item.toEvidence.Count > 0 && item.fromEvidence[0] == toEvidenceSingular && item.toEvidence[0] == fromEvidenceSingular);
			if (fact2 != null)
			{
				Game.Log("Evidence: (Duplicate evidence detected for " + fact2.GetName(null) + ", returning duplicate)", 2);
				if (forceDiscoveryOnCreate)
				{
					fact2.SetFound(true);
				}
				return fact2;
			}
		}
		string text = "Fact" + preset.subClass;
		object[] array = new object[]
		{
			preset,
			fromEvidence,
			toEvidence,
			passedObjects,
			overrideFromKeys,
			overrideToKeys,
			isCustomFact
		};
		Fact fact3 = Activator.CreateInstance(Type.GetType(text), array) as Fact;
		if (fact3 == null)
		{
			Game.LogError("Unable to create subclass " + text, 2);
			return null;
		}
		if (forceDiscoveryOnCreate)
		{
			fact3.SetFound(true);
		}
		return fact3;
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x001D5614 File Offset: 0x001D3814
	public Fact CreateFactFromSerializedString(string str)
	{
		string[] array = str.Split('|', 0);
		string presetName = string.Empty;
		if (array.Length != 0)
		{
			presetName = array[0];
		}
		List<Evidence> list = new List<Evidence>();
		if (array.Length > 1)
		{
			foreach (string text in array[1].Split(',', 0))
			{
				if (text != null && text.Length > 0)
				{
					Evidence evidence = null;
					if (Toolbox.Instance.TryGetEvidence(text, out evidence))
					{
						list.Add(evidence);
					}
				}
			}
		}
		List<Evidence> list2 = new List<Evidence>();
		if (array.Length > 2)
		{
			foreach (string text2 in array[2].Split(',', 0))
			{
				if (text2 != null && text2.Length > 0)
				{
					Evidence evidence2 = null;
					if (Toolbox.Instance.TryGetEvidence(text2, out evidence2))
					{
						list2.Add(evidence2);
					}
				}
			}
		}
		List<Evidence.DataKey> list3 = new List<Evidence.DataKey>();
		if (array.Length > 3)
		{
			foreach (string text3 in array[3].Split(',', 0))
			{
				if (text3 != null && text3.Length > 0)
				{
					int num = -1;
					if (int.TryParse(text3, ref num))
					{
						list3.Add((Evidence.DataKey)num);
					}
				}
			}
		}
		List<Evidence.DataKey> list4 = new List<Evidence.DataKey>();
		if (array.Length > 4)
		{
			foreach (string text4 in array[4].Split(',', 0))
			{
				if (text4 != null && text4.Length > 0)
				{
					int num2 = -1;
					if (int.TryParse(text4, ref num2))
					{
						list4.Add((Evidence.DataKey)num2);
					}
				}
			}
		}
		bool forceDiscoveryOnCreate = false;
		if (array.Length > 5)
		{
			int num3 = -1;
			if (int.TryParse(array[5], ref num3))
			{
				forceDiscoveryOnCreate = Convert.ToBoolean(num3);
			}
		}
		bool flag = false;
		if (array.Length > 6)
		{
			int num4 = -1;
			if (int.TryParse(array[6], ref num4))
			{
				flag = Convert.ToBoolean(num4);
			}
		}
		bool flag2 = false;
		if (array.Length > 7)
		{
			int num5 = -1;
			if (int.TryParse(array[7], ref num5))
			{
				flag2 = Convert.ToBoolean(num5);
			}
		}
		string customName = string.Empty;
		if (flag2 && array.Length > 8)
		{
			customName = array[8];
		}
		Fact fact = this.CreateFact(presetName, null, null, list, list2, forceDiscoveryOnCreate, null, list3, list4, flag2);
		if (flag)
		{
			fact.SetSeen();
		}
		if (flag2)
		{
			fact.SetCustomName(customName);
		}
		return fact;
	}

	// Token: 0x04002D39 RID: 11577
	public bool globalEntries;

	// Token: 0x04002D3A RID: 11578
	private static EvidenceCreator _instance;
}
